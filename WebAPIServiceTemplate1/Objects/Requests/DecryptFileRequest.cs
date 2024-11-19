using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAPIServiceTemplate1.Objects.Requests
{
    public class DecryptFileRequest 
    {
        private string _identificationType { get; set; }
        private string _identification { get; set; }

        [Required]
        public string IdentificationType
        {
            get { return _identificationType; }
            set { _identificationType = value.ToUpper(); }
        }
        [Required]
        public string Identification
        {
            get { return _identification; }
            set { _identification = value; }
        }

        [Required]
        public byte[] File { get; set; }
    }
}