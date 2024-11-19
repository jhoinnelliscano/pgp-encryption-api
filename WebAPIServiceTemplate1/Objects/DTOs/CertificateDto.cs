using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPIServiceTemplate1.Objects.DTOs
{
    public class CertificateDto
    {
        public bool Created { get; set; }
        public string PublicKey { get; set; }
        public bool EmailSend { get; set; }
        public string ExpireDate { get; set; }
    }
}