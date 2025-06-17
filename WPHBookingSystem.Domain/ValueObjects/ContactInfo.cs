using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPHBookingSystem.Domain.ValueObjects
{
    /// <summary>
    /// Represents contact information for a guest or booking.
    /// This is a value object that encapsulates phone number and address information
    /// with validation to ensure data integrity.
    /// 
    /// Value objects are immutable and their equality is based on their content
    /// rather than their identity.
    /// </summary>
    public class ContactInfo
    {
        /// <summary>
        /// Gets the phone number associated with this contact information.
        /// The phone number is validated to ensure it's not null or empty.
        /// </summary>
        public string Phone { get; private set; }

        /// <summary>
        /// Gets the address associated with this contact information.
        /// The address is validated to ensure it's not null or empty.
        /// </summary>
        public string Address { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactInfo"/> class
        /// with the specified phone number and address.
        /// </summary>
        /// <param name="phone">The phone number. Cannot be null, empty, or whitespace.</param>
        /// <param name="address">The address. Cannot be null, empty, or whitespace.</param>
        /// <exception cref="ArgumentException">Thrown when phone or address is null, empty, or whitespace.</exception>
        public ContactInfo(string phone, string address)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone cannot be empty.", nameof(phone));
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Address cannot be empty.", nameof(address));

            Phone = phone;
            Address = address;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current ContactInfo instance.
        /// Two ContactInfo objects are considered equal if they have the same phone and address.
        /// </summary>
        /// <param name="obj">The object to compare with the current ContactInfo instance.</param>
        /// <returns>true if the specified object is equal to the current ContactInfo instance; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is not ContactInfo other) return false;
            return Phone == other.Phone && Address == other.Address;
        }

        /// <summary>
        /// Serves as the default hash function for ContactInfo objects.
        /// The hash code is based on the combination of Phone and Address properties.
        /// </summary>
        /// <returns>A hash code for the current ContactInfo object.</returns>
        public override int GetHashCode() => HashCode.Combine(Phone, Address);
    }
}
