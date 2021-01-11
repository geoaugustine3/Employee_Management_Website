using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EmployeeManagement.Models
{
    public class ClaimsStore
    {
        public static List<Claim> AllClaims = new List<Claim>()  // 'AllClaims' is a property of 'ClaimsStore' class. 
        {                                                        // 'AllClaims'contains the List of 'Claim' class objects.     
        new Claim("Create Role", "Create Role"),      // Overloaded constructor of 'Claim' class.
        new Claim("Edit Role","Edit Role"),           // Overloaded constructor of 'Claim' class.
        new Claim("Delete Role","Delete Role")        // Overloaded constructor of 'Claim' class.
        };
    }
}
