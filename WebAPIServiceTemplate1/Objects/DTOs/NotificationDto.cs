using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPIServiceTemplate1.Objects.DTOs
{
    public class NotificationDto
    {
        public KeyDto ClientInfo { get; set; }
        public string CertificatePath { get; set; }

        public NotificationDto(KeyDto clientInfo, string certificatePath)
        {
            ClientInfo = clientInfo;
            CertificatePath = certificatePath;
        }

    }
}