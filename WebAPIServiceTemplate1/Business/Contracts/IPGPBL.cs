using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIServiceTemplate1.Objects.DTOs;

namespace WebAPIServiceTemplate1.Business
{
    public interface IPGPBL
    {
        CertificateDto GenerateKey(KeyDto keyDto);

        byte[] EncriptFile(string identificaton, string identificationType, byte[] file);

        byte[] DecriptFile(string identificaton, string identificationType, byte[] file);
    }
}
