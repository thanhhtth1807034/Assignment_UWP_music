using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace T1808aUWP.Entity
{
    class Song
    {
        public long id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string singer { get; set; }
        public string author { get; set; }
        public string thumbnail { get; set; }
        public string link { get; set; }

        public Dictionary<string, string> Validate()
        {
            var errors = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(name))
            {
                errors.Add("name", "Name is required!");
            }
            else if (name.Length < 3 || name.Length > 30)
            {
                errors.Add("name", "Name must be 3 to 30 characters!");
            }

            if (string.IsNullOrEmpty(singer))
            {
                errors.Add("single", "Single is required!");
            }
            else if (singer.Length < 2 || singer.Length > 30)
            {
                errors.Add("singer", "Singer must be 2 to 30 characters!");
            }

            if (string.IsNullOrEmpty(author))
            {
                errors.Add("author", "Author is required!");
            }
            else if (author.Length < 2 || author.Length > 30)
            {
                errors.Add("author", "Author must be 2 to 30 characters !");
            }

            if (string.IsNullOrEmpty(description))
            {
                errors.Add("description", "description is required!");
            }

            if (string.IsNullOrEmpty(link))
            {
                errors.Add("link", "link is required!");
            }

            if (string.IsNullOrEmpty(thumbnail))
            {
                errors.Add("thumbnail", "thumbnail is required!");
            }

            return errors;
        }
    }
}
