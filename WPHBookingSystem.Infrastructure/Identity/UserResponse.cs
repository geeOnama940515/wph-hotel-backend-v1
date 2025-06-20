using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPHBookingSystem.Infrastructure.Identity
{
    public class UserResponse
    {
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string UserId { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;

            public List<string>? Roles { get; set; }
    }
}
