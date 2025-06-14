using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPHBookingSystem.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string Firstname { get; set; }
        public string LastName { get; set; }

        [NotMapped]
        public string Fullname
        {
            get
            {
                return string.Format("{0} {1}", this.Firstname, this.LastName);
            }
        }
        [NotMapped]
        public string Role { get; set; }
    }
}
