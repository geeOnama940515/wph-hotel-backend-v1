using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPHBookingSystem.Domain.ValueObjects
{
    public class ContactInfo
    {
        public string Phone { get; private set; }
        public string Address { get; private set; }

        public ContactInfo(string phone, string address)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone cannot be empty.", nameof(phone));
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Address cannot be empty.", nameof(address));

            Phone = phone;
            Address = address;
        }

        // Override equality for value object behavior:
        public override bool Equals(object? obj)
        {
            if (obj is not ContactInfo other) return false;
            return Phone == other.Phone && Address == other.Address;
        }

        public override int GetHashCode() => HashCode.Combine(Phone, Address);
    }
}
