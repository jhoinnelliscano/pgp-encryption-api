using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace WebAPIServiceTemplate1.Objects.DTOs
{
    public class KeyDto
    {
        public string IdentificationType { get; set; }
        public string Identification { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int KeyLength { get; set; }

        public string UserName
        {
            get
            {
                var strUser = IdentificationType + "." + Identification + ".";
                var length = Convert.ToInt32(ConfigurationManager.AppSettings["UserIdLength"].ToString()) - strUser.Length;
                length = length < 0 ? 0 : length;
                var code = GenerateCode(length);
                return strUser + code;
            }
        }


        private string GenerateCode(int length)
        {
            Random random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var code =  new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            return code;
        }

        public string GeneratePassword()
        {
            Password = Guid.NewGuid().ToString();
            return Password;
        }
    }
}