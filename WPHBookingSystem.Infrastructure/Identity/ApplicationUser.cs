using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPHBookingSystem.Infrastructure.Identity
{
    /// <summary>
    /// Custom user entity that extends ASP.NET Core Identity's IdentityUser.
    /// Provides additional user properties specific to the hotel booking system
    /// while maintaining compatibility with ASP.NET Core Identity framework.
    /// 
    /// This class adds first name, last name, and role information to the standard
    /// Identity user properties like email, username, and password.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        public string Firstname { get; set; }

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets the user's full name by combining first name and last name.
        /// This property is not mapped to the database and is computed at runtime.
        /// </summary>
        [NotMapped]
        public string Fullname
        {
            get
            {
                return string.Format("{0} {1}", this.Firstname, this.LastName);
            }
        }

        /// <summary>
        /// Gets or sets the user's role in the system.
        /// This property is not mapped to the database and is used for role management.
        /// </summary>
        [NotMapped]
        public string Role { get; set; }

        public bool isDeleted { get; set; } = false;
    }
}
