using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPHBookingSystem.Domain.Exceptions
{
    /// <summary>
    /// Represents errors that occur during domain logic execution.
    /// This exception is thrown when business rules are violated or when
    /// domain operations cannot be completed due to invalid state or data.
    /// 
    /// Domain exceptions should be used to communicate business rule violations
    /// and domain-specific errors rather than technical or infrastructure issues.
    /// </summary>
    [Serializable]
    public class DomainException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainException"/> class
        /// with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the domain error.</param>
        public DomainException(string message) : base(message) { }
    }
}
