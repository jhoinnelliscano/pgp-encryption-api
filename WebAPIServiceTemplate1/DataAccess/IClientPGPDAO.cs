using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIServiceTemplate1.Models;

namespace WebAPIServiceTemplate1.DataAccess
{
    public interface IClientPGPDAO
    {
        Cliente GetClient(string identification);

        IList<Cliente> GetClients();

        IList<ClientePGP> GetClientsWithCertificate();

        ClientePGP GetClientPGP(string identification, string identificationType);

        IList<CertificadoPGP> GetCertificates();

        void InsertClientCertificate(ClientePGP clienteCertificadoPGP);

        void InactiveClientCertificate(long clientePGPId);

        void UpdateClientCertificateToSent(long clientePGPId);
    }
}
