using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebAPIServiceTemplate1.Validations;

namespace WebAPIServiceTemplate1.Objects.Requests
{
    public class GenerateKeyRequest
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
        public string Email { get; set; }
        public string Description { get; set; }

        [Required]
        [ValidateKeyLength(ErrorMessage="Valores permitidos: 1024, 2048, 4096")]
        public int KeyLength { get; set; }
    }
}