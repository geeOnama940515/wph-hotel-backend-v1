using WPHBookingSystem.Application.DTOs.Booking;

namespace WPHBookingSystem.Application.Interfaces.Services
{
    /// <summary>
    /// Service interface for sending emails related to booking operations.
    /// 
    /// This service provides functionality for sending booking confirmations,
    /// updates, and other email notifications to guests.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends a booking confirmation email to the guest.
        /// </summary>
        /// <param name="bookingDto">The booking details to include in the email</param>
        /// <param name="guestEmail">The email address of the guest</param>
        /// <param name="guestName">The name of the guest</param>
        /// <returns>True if the email was sent successfully; otherwise, false</returns>
        Task<bool> SendBookingConfirmationAsync(BookingDto bookingDto, string guestEmail, string guestName);

        /// <summary>
        /// Sends a booking update notification email to the guest.
        /// </summary>
        /// <param name="bookingDto">The updated booking details</param>
        /// <param name="guestEmail">The email address of the guest</param>
        /// <param name="guestName">The name of the guest</param>
        /// <param name="updateType">The type of update (dates, status, etc.)</param>
        /// <returns>True if the email was sent successfully; otherwise, false</returns>
        Task<bool> SendBookingUpdateAsync(BookingDto bookingDto, string guestEmail, string guestName, string updateType);

        /// <summary>
        /// Sends a booking cancellation email to the guest.
        /// </summary>
        /// <param name="bookingDto">The cancelled booking details</param>
        /// <param name="guestEmail">The email address of the guest</param>
        /// <param name="guestName">The name of the guest</param>
        /// <returns>True if the email was sent successfully; otherwise, false</returns>
        Task<bool> SendBookingCancellationAsync(BookingDto bookingDto, string guestEmail, string guestName);
    }
} 