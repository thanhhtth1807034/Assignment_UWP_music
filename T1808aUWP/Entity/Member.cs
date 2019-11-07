using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace T1808aUWP.Entity
{
    class Member
    {
        public long id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string avatar { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public string introduction { get; set; }
        public int gender { get; set; }
        public string birthday { get; set; }
        public string email { get; set; }
        public string password { get; set; }

        public Dictionary<string, string> Validate()
        {
            var errors = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(firstName))
            {
                errors.Add("firstName", "firstName is required!");
            }
            else if (firstName.Length < 3 || firstName.Length > 30)
            {
                errors.Add("firstName", "firstName must be 3 to 30 characters!");
            }

            if (string.IsNullOrEmpty(lastName))
            {
                errors.Add("lastName", "lastName is required!");
            }
            else if (lastName.Length < 3 || lastName.Length > 30)
            {
                errors.Add("lastName", "lastName must be 3 to 30 characters!");
            }

            if (string.IsNullOrEmpty(avatar))
            {
                errors.Add("avatar", "avatar is required!");
            }

            if (string.IsNullOrEmpty(address))
            {
                errors.Add("address", "address is required!");
            }

            if (string.IsNullOrEmpty(phone))
            {
                errors.Add("phone", "phone is required!");
            }

            if (string.IsNullOrEmpty(introduction))
            {
                errors.Add("introduction", "introduction is required!");
            }

            if (string.IsNullOrEmpty(birthday))
            {
                errors.Add("birthday", "birthday is required!");
            }

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
