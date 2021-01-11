using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Utilities
{
    public class ValidEmailDomainAttribute : ValidationAttribute
    {
        private readonly string allowedDomain;

        public ValidEmailDomainAttribute(string allowedDomain)
        {
            this.allowedDomain = allowedDomain;
        }
        
        public override bool IsValid(object value)
        {
            // 'Object' data type is converted to string and splits email id into two strings based on '@'
            string[] strings = value.ToString().Split('@');

            // Second string (received 'domain name') is converted to uppercase  and 
            // 'allowedDomain' (required 'domain name') is converted to uppercase, then compares they are same. 
            return strings[1].ToUpper() == allowedDomain.ToUpper();
        }
    }
}
