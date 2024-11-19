using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using WebAPIServiceTemplate1.Business.PGP;
using WebAPIServiceTemplate1.DataAccess;
using WebAPIServiceTemplate1.Models;
using WebAPIServiceTemplate1.Objects.DTOs;

namespace WebAPIServiceTemplate1.Business
{
    public class PGPBL : IPGPBL
    {
        private readonly string _path = ConfigurationManager.AppSettings["PGPPath"].ToString();
        private readonly string _pathByClient = ConfigurationManager.AppSettings["PathByClient"].ToString();
        private readonly string _pgpPublicKey = ConfigurationManager.AppSettings["PgpPublicKey"].ToString();
        private readonly string _pgpPrivateKey = ConfigurationManager.AppSettings["PgpPrivateKey"].ToString();

        private IClientPGPDAO _clientPGPDAO;

        public PGPBL()
        {
            this._clientPGPDAO = (IClientPGPDAO)new ClientPGPDAO();
        }

        public CertificateDto GenerateKey(KeyDto keyDto)
        {
            var code = Guid.NewGuid().ToString();
            DateTime dateTime = DateTime.Now.AddYears(Convert.ToInt32(ConfigurationManager.AppSettings["PgpYears"].ToString()));
            string str1 = this._pathByClient.Replace("[PATH]", this._path).Replace("[IDENTIFICATIONTYPE]", keyDto.IdentificationType).Replace("[IDENTIFICATION]", keyDto.Identification);
            string str2 = this._pgpPublicKey.Replace("[PATHCLIENT]", str1).Replace("[CODE]", code);
            string str3 = this._pgpPrivateKey.Replace("[PATHCLIENT]", str1).Replace("[CODE]", code);
            try
            {
                Cliente client = this._clientPGPDAO.GetClient(keyDto.Identification);
                if (client == null)
                    throw new Exception("Cliente no existe.");

                keyDto.Name = client.Nombre;
                PgpManager.GenerateKeys(str1, str2, str3, keyDto.UserName, keyDto.GeneratePassword(), keyDto.KeyLength);
                if (!Directory.Exists(str1) || !File.Exists(str2))
                    throw new Exception("Ocurrio un error al generar la clave.");
                CertificadoPGP certificadoPgp = new CertificadoPGP()
                {
                    Clave = PgpPassword.EncryptPassword(keyDto.Password),
                    Estado = "A",
                    FechaCaducidad = dateTime,
                    Email = keyDto.Email,
                    UbicacionCertificadoPublico = str2,
                    UbicacionCertificadoPrivado = str3,
                    CertificadoPublico = File.ReadAllText(str2),
                    CertificadoPrivado = File.ReadAllText(str3),
                    LongitudCifrado = keyDto.KeyLength
                };
                ClientePGP clienteCertificadoPGP = new ClientePGP()
                {
                    CertificadoPGP = certificadoPgp,
                    IdCliente = client.Id,
                    Estado = "A",
                    CorreoNotificacion = keyDto.Email,
                    ClaveEnviada = "N",
                    Observaciones = "Certificado Creado"
                };
                this._clientPGPDAO.InsertClientCertificate(clienteCertificadoPGP);
                bool flag;
                try
                {
                    NotificationBL.SendEmail(new NotificationDto(keyDto, str2));
                    flag = true;
                    this._clientPGPDAO.UpdateClientCertificateToSent((long)clienteCertificadoPGP.Id);
                }
                catch (Exception ex)
                {
                    flag = false;
                }

                bool directoryDeleted = false;
                try
                {
                    new DirectoryInfo(str1).Delete(true);
                    directoryDeleted = true;
                }
                catch (Exception ex)
                {
                    directoryDeleted = false;
                }

 
                return new CertificateDto()
                {
                    Created = clienteCertificadoPGP.Id != 0,
                    PublicKey = certificadoPgp.CertificadoPublico,
                    EmailSend = flag,
                    ExpireDate = dateTime.ToShortDateString()
                };
            }
            catch (Exception ex)
            {
                if (Directory.Exists(str1))
                    new DirectoryInfo(str1).Delete(true);
                throw ex;
            }
        }

        public byte[] EncriptFile(string identificaton, string identificationType, byte[] file)
        {
            ClientePGP clientPgp = this._clientPGPDAO.GetClientPGP(identificaton, identificationType);
            if (clientPgp == null)
                return (byte[])null;
            string path;
            if (!File.Exists(clientPgp.CertificadoPGP.UbicacionCertificadoPublico))
            {
                path = this._pgpPublicKey.Replace("[PATHCLIENT]", this._pathByClient).Replace("[CODE]", Guid.NewGuid().ToString().Substring(1, 4));
                File.Create(path);
                File.WriteAllText(path, clientPgp.CertificadoPGP.CertificadoPublico);
            }
            else
                path = clientPgp.CertificadoPGP.UbicacionCertificadoPublico;
            byte[] publicKey = File.ReadAllBytes(path);
            return PgpManager.Encrypt(file, publicKey);
        }

        public byte[] DecriptFile(string identificaton, string identificationType, byte[] file)
        {
            ClientePGP clientPgp = this._clientPGPDAO.GetClientPGP(identificaton, identificationType);
            if (clientPgp == null)
                return (byte[])null;
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter((Stream)memoryStream);
            streamWriter.Write(clientPgp.CertificadoPGP.CertificadoPrivado);
            streamWriter.Flush();
            memoryStream.Position = 0L;
            return PgpManager.Decrypt(file, (Stream)memoryStream, PgpPassword.DecryptPassword(clientPgp.CertificadoPGP.Clave));
        }
    }

}