using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebAPIServiceTemplate1.Validations
{
    public class ValidateKeyLength : ValidationAttribute
    {
        public ValidateKeyLength() : base("")
        { }

        public override bool IsValid(object value)
        {
            try
            {
                int length = Convert.ToInt32(value);
                var legthList = ConfigurationManager.AppSettings["KeyLengths"].ToString().Split(',');

                return legthList.Any(x => Convert.ToInt32(x) == length);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}