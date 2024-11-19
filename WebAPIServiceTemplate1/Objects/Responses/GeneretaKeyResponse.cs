using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPIServiceTemplate1.Objects.Responses
{
    public class GeneretaKeyResponse
    {
        public bool IsGenerated { get; set; }
        public string  PublicKey { get; set; }
        public string ExpireDate { get; set; }
        public string ClientIdentification { get; set; }
    }
}