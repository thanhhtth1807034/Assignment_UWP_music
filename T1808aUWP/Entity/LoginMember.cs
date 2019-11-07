using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T1808aUWP.Entity
{
    class LoginMember
    {
        public string email { get; set; }
        public string password { get; set; }

        public Dictionary<string, string> Validate()
        {
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrEmpty(email))
            {
                errors.Add("email", "email is required!");
            }

            if (string.IsNullOrEmpty(password))
            {
                errors.Add("password", "password is required!");
            }
            return errors;
        }
    }
}
