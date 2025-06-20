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
    public class ChangePasswordRequest
    {
        public string UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
