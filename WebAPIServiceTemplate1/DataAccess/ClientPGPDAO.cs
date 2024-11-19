using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using WebAPIServiceTemplate1.Models;

namespace WebAPIServiceTemplate1.DataAccess
{
    public class ClientPGPDAO : IClientPGPDAO
    {
        private DAVCORAPIEntities _dbModel { get; set; }

        public ClientPGPDAO()
        {
            this._dbModel = new DAVCORAPIEntities();
        }

        public IList<Cliente> GetClients()
        {
            using (DAVCORAPIEntities davcorapiEntities = new DAVCORAPIEntities())
                return (IList<Cliente>)davcorapiEntities.Cliente.ToList<Cliente>();
        }

        public Cliente GetClient(string identification)
        {
            using (DAVCORAPIEntities davcorapiEntities = new DAVCORAPIEntities())
                return davcorapiEntities.Cliente.FirstOrDefault<Cliente>((Expression<Func<Cliente, bool>>)(x => x.NumeroDocumento.Equals(identification)));
        }

        public IList<ClientePGP> GetClientsWithCertificate()
        {
            using (DAVCORAPIEntities davcorapiEntities = new DAVCORAPIEntities())
                return (IList<ClientePGP>)davcorapiEntities.ClientePGP.ToList<ClientePGP>();
        }

        public ClientePGP GetClientPGP(string identification, string identificationType)
        {
            using (DAVCORAPIEntities davcorapiEntities = new DAVCORAPIEntities())
                return davcorapiEntities.ClientePGP.Include("CertificadoPGP").FirstOrDefault<ClientePGP>((Expression<Func<ClientePGP, bool>>)
                    (x => x.Cliente.NumeroDocumento.Equals(identification) 
                          && x.Cliente.TipoDocumento.Equals(identificationType)
                          && x.Estado.Equals("A")
                          && x.CertificadoPGP.Estado.Equals("A")
                          ));
        }

        public IList<CertificadoPGP> GetCertificates()
        {
            using (DAVCORAPIEntities davcorapiEntities = new DAVCORAPIEntities())
                return (IList<CertificadoPGP>)davcorapiEntities.CertificadoPGP.ToList<CertificadoPGP>();
        }

        public void InsertClientCertificate(ClientePGP clienteCertificadoPGP)
        {
            using (DAVCORAPIEntities davcorapiEntities = new DAVCORAPIEntities())
            {
                davcorapiEntities.ClientePGP.Add(clienteCertificadoPGP);
                davcorapiEntities.SaveChanges();
            }
        }

        public void InactiveClientCertificate(long clientePGPId)
        {
            using (DAVCORAPIEntities davcorapiEntities = new DAVCORAPIEntities())
            {
                davcorapiEntities.ClientePGP.Find((object)clientePGPId).Estado = "I";
                davcorapiEntities.SaveChanges();
            }
        }

        public void UpdateClientCertificateToSent(long clientePGPId)
        {
            using (DAVCORAPIEntities davcorapiEntities = new DAVCORAPIEntities())
            {
                davcorapiEntities.ClientePGP.Find((object)clientePGPId).ClaveEnviada = "S";
                davcorapiEntities.SaveChanges();
            }
        }
    }
}