using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NETCore.MailKit;
using NETCore.MailKit.Core;
using NETCore.MailKit.Infrastructure.Internal;
using System.Text;
using WPHBookingSystem.Application.DTOs.Booking;
using WPHBookingSystem.Application.DTOs.Email;
using WPHBookingSystem.Application.Interfaces.Services;

namespace WPHBookingSystem.Infrastructure.Services
{
    /// <summary>
    /// Implementation of the email service for sending booking-related emails using MailKit.
    /// 
    /// This service uses MailKit to send HTML-formatted emails with booking confirmations,
    /// updates, and cancellations to guests. MailKit provides better reliability and
    /// easier configuration compared to System.Net.Mail.
    /// </summary>
    public class EmailConfigService : IEmailSenderService
    {
        private readonly EmailSettings _emailSettings;
        //private readonly EmailOptions _options;
        private readonly ILogger<EmailService> _logger;

        private readonly EmailService _emailService;

        public EmailConfigService(IOptions<EmailSettings> options, ILogger<EmailService> logger)
        {
            _emailSettings = options.Value;
            _emailService = new EmailService(new MailKitProvider(new MailKitOptions
            {
                // TODO : Secure this using sercets.json
                Port = 587,
                SenderName = _emailSettings.FromName,
                Server = _emailSettings.SmtpHost,
                SenderEmail = _emailSettings.FromEmail,
                Account = _emailSettings.Username,
                Password = _emailSettings.Password,
                Security = true

            }));

           // _emailSettings = options.Value;
            _logger = logger;
        }

        /// <summary>
        /// Sends a booking confirmation email to the guest.
        /// </summary>
        public async Task<bool> SendBookingConfirmationAsync(BookingDto bookingDto, string guestEmail, string guestName)
        {
            try
            {
                var subject = $"WPH Hotel - Booking Confirmation #{bookingDto.Id:N}";
                var htmlBody = GenerateBookingConfirmationHtml(bookingDto, guestName);
                
                return await SendEmailAsync(guestEmail, subject, htmlBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send booking confirmation email to {GuestEmail}", guestEmail);
                return false;
            }
        }

        /// <summary>
        /// Sends a booking update notification email to the guest.
        /// </summary>
        public async Task<bool> SendBookingUpdateAsync(BookingDto bookingDto, string guestEmail, string guestName, string updateType)
        {
            try
            {
                var subject = $"WPH Hotel - Booking Update #{bookingDto.Id:N}";
                var htmlBody = GenerateBookingUpdateHtml(bookingDto, guestName, updateType);
                
                return await SendEmailAsync(guestEmail, subject, htmlBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send booking update email to {GuestEmail}", guestEmail);
                return false;
            }
        }

        /// <summary>
        /// Sends a booking cancellation email to the guest.
        /// </summary>
        public async Task<bool> SendBookingCancellationAsync(BookingDto bookingDto, string guestEmail, string guestName)
        {
            try
            {
                var subject = $"WPH Hotel - Booking Cancellation #{bookingDto.Id:N}";
                var htmlBody = GenerateBookingCancellationHtml(bookingDto, guestName);
                
                return await SendEmailAsync(guestEmail, subject, htmlBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send booking cancellation email to {GuestEmail}", guestEmail);
                return false;
            }
        }

        /// <summary>
        /// Sends an email using MailKit SMTP client.
        /// </summary>
        private async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {

                await _emailService.SendAsync(toEmail,subject,htmlBody,isHtml:true);                
                _logger.LogInformation("Email sent successfully to {ToEmail}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {ToEmail}. Error: {ErrorMessage}", toEmail, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Generates HTML content for booking confirmation email.
        /// </summary>
        private string GenerateBookingConfirmationHtml(BookingDto booking, string guestName)
        {
            var html = new StringBuilder();
            
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang=\"en\">");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset=\"UTF-8\">");
            html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            html.AppendLine("    <title>Booking Confirmation</title>");
            html.AppendLine("    <style>");
            html.AppendLine("        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; }");
            html.AppendLine("        .container { max-width: 600px; margin: 0 auto; padding: 20px; }");
            html.AppendLine("        .header { background-color: #2c3e50; color: white; padding: 20px; text-align: center; }");
            html.AppendLine("        .content { background-color: #f8f9fa; padding: 30px; }");
            html.AppendLine("        .booking-details { background-color: white; padding: 20px; margin: 20px 0; border-radius: 5px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }");
            html.AppendLine("        .detail-row { display: flex; justify-content: space-between; margin: 10px 0; padding: 8px 0; border-bottom: 1px solid #eee; }");
            html.AppendLine("        .detail-label { font-weight: bold; color: #2c3e50; }");
            html.AppendLine("        .detail-value { color: #555; }");
            html.AppendLine("        .footer { background-color: #34495e; color: white; padding: 20px; text-align: center; font-size: 14px; }");
            html.AppendLine("        .logo { max-width: 150px; height: auto; }");
            html.AppendLine("        .special-request { background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; margin: 15px 0; border-radius: 5px; }");
            html.AppendLine("        .booking-token { background-color: #d1ecf1; border: 1px solid #bee5eb; padding: 10px; margin: 10px 0; border-radius: 5px; text-align: center; font-weight: bold; }");
            html.AppendLine("        .view-summary-btn { display: inline-block; background-color: #27ae60; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; margin: 20px 0; font-weight: bold; }");
            html.AppendLine("        .view-summary-btn:hover { background-color: #229954; }");
            html.AppendLine("        .summary-section { text-align: center; margin: 30px 0; }");
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("    <div class=\"container\">");
            html.AppendLine("        <div class=\"header\">");
            html.AppendLine($"            <h1>{_emailSettings.HotelInfo.Name}</h1>");
            html.AppendLine("            <p>Booking Confirmation</p>");
            html.AppendLine("        </div>");
            html.AppendLine("        <div class=\"content\">");
            html.AppendLine($"            <h2>Dear {guestName},</h2>");
            html.AppendLine("            <p>Thank you for choosing to stay with us! Your booking has been confirmed.</p>");
            html.AppendLine("            <div class=\"booking-token\">");
            html.AppendLine($"                <strong>Booking Reference:</strong> #{booking.Id:N}");
            html.AppendLine("            </div>");
            html.AppendLine("            <div class=\"booking-details\">");
            html.AppendLine("                <h3>Booking Details</h3>");
            html.AppendLine("                <table style=\"width: 100%; border-collapse: collapse;\">");
            html.AppendLine("                    <tr><td style=\"font-weight: bold; padding: 8px;\">Guest Name:</td><td style=\"padding: 8px;\">{guestName}</td></tr>");
            html.AppendLine($"                    <tr><td style=\"font-weight: bold; padding: 8px;\">Room:</td><td style=\"padding: 8px;\">{booking.RoomName}</td></tr>");
            html.AppendLine($"                    <tr><td style=\"font-weight: bold; padding: 8px;\">Number of Guests:</td><td style=\"padding: 8px;\">{booking.Guests}</td></tr>");
            html.AppendLine($"                    <tr><td style=\"font-weight: bold; padding: 8px;\">Check-in:</td><td style=\"padding: 8px;\">{booking.CheckIn:dddd, MMMM dd, yyyy}</td></tr>");
            html.AppendLine($"                    <tr><td style=\"font-weight: bold; padding: 8px;\">Check-out:</td><td style=\"padding: 8px;\">{booking.CheckOut:dddd, MMMM dd, yyyy}</td></tr>");
            html.AppendLine($"                    <tr><td style=\"font-weight: bold; padding: 8px;\">Total Amount:</td><td style=\"padding: 8px;\">₱ {booking.TotalAmount:F2}</td></tr>");
            html.AppendLine($"                    <tr><td style=\"font-weight: bold; padding: 8px;\">Status:</td><td style=\"padding: 8px;\">{booking.Status}</td></tr>");
            html.AppendLine("                </table>");
            html.AppendLine("            </div>");

            if (!string.IsNullOrWhiteSpace(booking.SpecialRequests))
            {
                html.AppendLine("            <div class=\"special-request\" style=\"margin-top: 20px;\">");
                html.AppendLine("                <h4>Special Requests:</h4>");
                html.AppendLine($"                <p>{booking.SpecialRequests}</p>");
                html.AppendLine("            </div>");
            }

            html.AppendLine("    </div>");
            html.AppendLine("</body>");

        

            html.AppendLine("            <div class=\"summary-section\">");
            html.AppendLine("                <h3>View Your Booking Summary</h3>");
            html.AppendLine("                <p>Click the button below to view your complete booking details online:</p>");
            html.AppendLine($"                <a href=\"{_emailSettings.BaseUrl}/view-booking-summary?bookingtoken={booking.BookingToken}\" class=\"view-summary-btn\">View Booking Summary</a>");
            html.AppendLine("            </div>");

            html.AppendLine("            <p>We look forward to welcoming you to our hotel!</p>");
            html.AppendLine("            <p>If you have any questions, please don't hesitate to contact us.</p>");
            html.AppendLine("        </div>");
            html.AppendLine("        <div class=\"footer\">");
            html.AppendLine($"            <p>{_emailSettings.HotelInfo.Name}</p>");
            if (!string.IsNullOrWhiteSpace(_emailSettings.HotelInfo.Address))
            {
                html.AppendLine($"            <p>{_emailSettings.HotelInfo.Address}</p>");
            }
            if (!string.IsNullOrWhiteSpace(_emailSettings.HotelInfo.Phone))
            {
                html.AppendLine($"            <p>Phone: {_emailSettings.HotelInfo.Phone}</p>");
            }
            if (!string.IsNullOrWhiteSpace(_emailSettings.HotelInfo.Email))
            {
                html.AppendLine($"            <p>Email: {_emailSettings.HotelInfo.Email}</p>");
            }
            if (!string.IsNullOrWhiteSpace(_emailSettings.HotelInfo.Website))
            {
                html.AppendLine($"            <p>Website: {_emailSettings.HotelInfo.Website}</p>");
            }
            html.AppendLine("        </div>");
            html.AppendLine("    </div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }

        /// <summary>
        /// Generates HTML content for booking update email.
        /// </summary>
        private string GenerateBookingUpdateHtml(BookingDto booking, string guestName, string updateType)
        {
            var html = new StringBuilder();
            
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang=\"en\">");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset=\"UTF-8\">");
            html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            html.AppendLine("    <title>Booking Update</title>");
            html.AppendLine("    <style>");
            html.AppendLine("        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; }");
            html.AppendLine("        .container { max-width: 600px; margin: 0 auto; padding: 20px; }");
            html.AppendLine("        .header { background-color: #3498db; color: white; padding: 20px; text-align: center; }");
            html.AppendLine("        .content { background-color: #f8f9fa; padding: 30px; }");
            html.AppendLine("        .booking-details { background-color: white; padding: 20px; margin: 20px 0; border-radius: 5px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }");
            html.AppendLine("        .detail-row { display: flex; justify-content: space-between; margin: 10px 0; padding: 8px 0; border-bottom: 1px solid #eee; }");
            html.AppendLine("        .detail-label { font-weight: bold; color: #2c3e50; }");
            html.AppendLine("        .detail-value { color: #555; }");
            html.AppendLine("        .footer { background-color: #34495e; color: white; padding: 20px; text-align: center; font-size: 14px; }");
            html.AppendLine("        .update-notice { background-color: #d1ecf1; border: 1px solid #bee5eb; padding: 15px; margin: 15px 0; border-radius: 5px; }");
            html.AppendLine("        .view-summary-btn { display: inline-block; background-color: #27ae60; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; margin: 20px 0; font-weight: bold; }");
            html.AppendLine("        .view-summary-btn:hover { background-color: #229954; }");
            html.AppendLine("        .summary-section { text-align: center; margin: 30px 0; }");
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("    <div class=\"container\">");
            html.AppendLine("        <div class=\"header\">");
            html.AppendLine($"            <h1>{_emailSettings.HotelInfo.Name}</h1>");
            html.AppendLine("            <p>Booking Update</p>");
            html.AppendLine("        </div>");
            html.AppendLine("        <div class=\"content\">");
            html.AppendLine($"            <h2>Dear {guestName},</h2>");
            html.AppendLine("            <div class=\"update-notice\">");
            html.AppendLine($"                <h3>Your booking has been updated: {updateType}</h3>");
            html.AppendLine("            </div>");
            html.AppendLine("            <div class=\"booking-details\">");
            html.AppendLine("                <h3>Updated Booking Details</h3>");
            html.AppendLine("                <div class=\"detail-row\">");
            html.AppendLine("                    <span class=\"detail-label\">Booking Reference:</span>");
            html.AppendLine($"                    <span class=\"detail-value\">#{booking.Id:N}</span>");
            html.AppendLine("                </div>");
            html.AppendLine("                <div class=\"detail-row\">");
            html.AppendLine("                    <span class=\"detail-label\">Guest Name:</span>");
            html.AppendLine($"                    <span class=\"detail-value\">{guestName}</span>");
            html.AppendLine("                </div>");
            html.AppendLine("                <div class=\"detail-row\">");
            html.AppendLine("                    <span class=\"detail-label\">Room:</span>");
            html.AppendLine($"                    <span class=\"detail-value\">{booking.RoomName}</span>");
            html.AppendLine("                </div>");
            html.AppendLine("                <div class=\"detail-row\">");
            html.AppendLine("                    <span class=\"detail-label\">Number of Guests:</span>");
            html.AppendLine($"                    <span class=\"detail-value\">{booking.Guests}</span>");
            html.AppendLine("                </div>");
            html.AppendLine("                <div class=\"detail-row\">");
            html.AppendLine("                    <span class=\"detail-label\">Check-in:</span>");
            html.AppendLine($"                    <span class=\"detail-value\">{booking.CheckIn:dddd, MMMM dd, yyyy}</span>");
            html.AppendLine("                </div>");
            html.AppendLine("                <div class=\"detail-row\">");
            html.AppendLine("                    <span class=\"detail-label\">Check-out:</span>");
            html.AppendLine($"                    <span class=\"detail-value\">{booking.CheckOut:dddd, MMMM dd, yyyy}</span>");
            html.AppendLine("                </div>");
            html.AppendLine("                <div class=\"detail-row\">");
            html.AppendLine("                    <span class=\"detail-label\">Total Amount:</span>");
            html.AppendLine($"                    <span class=\"detail-value\">${booking.TotalAmount:F2}</span>");
            html.AppendLine("                </div>");
            html.AppendLine("                <div class=\"detail-row\">");
            html.AppendLine("                    <span class=\"detail-label\">Status:</span>");
            html.AppendLine($"                    <span class=\"detail-value\">{booking.Status}</span>");
            html.AppendLine("                </div>");
            html.AppendLine("            </div>");

            html.AppendLine("            <div class=\"summary-section\">");
            html.AppendLine("                <h3>View Your Updated Booking Summary</h3>");
            html.AppendLine("                <p>Click the button below to view your complete booking details online:</p>");
            html.AppendLine($"                <a href=\"{_emailSettings.BaseUrl}/view-booking-summary?bookingtoken={booking.BookingToken}\" class=\"view-summary-btn\">View Booking Summary</a>");
            html.AppendLine("            </div>");

            html.AppendLine("            <p>If you have any questions about this update, please contact us.</p>");
            html.AppendLine("        </div>");
            html.AppendLine("        <div class=\"footer\">");
            html.AppendLine($"            <p>{_emailSettings.HotelInfo.Name}</p>");
            if (!string.IsNullOrWhiteSpace(_emailSettings.HotelInfo.Address))
            {
                html.AppendLine($"            <p>{_emailSettings.HotelInfo.Address}</p>");
            }
            if (!string.IsNullOrWhiteSpace(_emailSettings.HotelInfo.Phone))
            {
                html.AppendLine($"            <p>Phone: {_emailSettings.HotelInfo.Phone}</p>");
            }
            if (!string.IsNullOrWhiteSpace(_emailSettings.HotelInfo.Email))
            {
                html.AppendLine($"            <p>Email: {_emailSettings.HotelInfo.Email}</p>");
            }
            html.AppendLine("        </div>");
            html.AppendLine("    </div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }

        /// <summary>
        /// Generates HTML content for booking cancellation email.
        /// </summary>
        private string GenerateBookingCancellationHtml(BookingDto booking, string guestName)
        {
            var html = new StringBuilder();
            
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang=\"en\">");
            html.AppendLine("<head>");
            html.AppendLine("    <meta charset=\"UTF-8\">");
            html.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            html.AppendLine("    <title>Booking Cancellation</title>");
            html.AppendLine("    <style>");
            html.AppendLine("        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; }");
            html.AppendLine("        .container { max-width: 600px; margin: 0 auto; padding: 20px; }");
            html.AppendLine("        .header { background-color: #e74c3c; color: white; padding: 20px; text-align: center; }");
            html.AppendLine("        .content { background-color: #f8f9fa; padding: 30px; }");
            html.AppendLine("        .booking-details { background-color: white; padding: 20px; margin: 20px 0; border-radius: 5px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }");
            html.AppendLine("        .detail-row { display: flex; justify-content: space-between; margin: 10px 0; padding: 8px 0; border-bottom: 1px solid #eee; }");
            html.AppendLine("        .detail-label { font-weight: bold; color: #2c3e50; }");
            html.AppendLine("        .detail-value { color: #555; }");
            html.AppendLine("        .footer { background-color: #34495e; color: white; padding: 20px; text-align: center; font-size: 14px; }");
            html.AppendLine("        .cancellation-notice { background-color: #f8d7da; border: 1px solid #f5c6cb; padding: 15px; margin: 15px 0; border-radius: 5px; color: #721c24; }");
            html.AppendLine("        .view-summary-btn { display: inline-block; background-color: #27ae60; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; margin: 20px 0; font-weight: bold; }");
            html.AppendLine("        .view-summary-btn:hover { background-color: #229954; }");
            html.AppendLine("        .summary-section { text-align: center; margin: 30px 0; }");
            html.AppendLine("    </style>");
            html.AppendLine("</head>");
            html.AppendLine("<body>");
            html.AppendLine("    <div class=\"container\">");
            html.AppendLine("        <div class=\"header\">");
            html.AppendLine($"            <h1>{_emailSettings.HotelInfo.Name}</h1>");
            html.AppendLine("            <p>Booking Cancellation</p>");
            html.AppendLine("        </div>");
            html.AppendLine("        <div class=\"content\">");
            html.AppendLine($"            <h2>Dear {guestName},</h2>");
            html.AppendLine("            <div class=\"cancellation-notice\">");
            html.AppendLine("                <h3>Your booking has been cancelled</h3>");
            html.AppendLine("            </div>");
            html.AppendLine("            <div class=\"booking-details\">");
            html.AppendLine("                <h3>Cancelled Booking Details</h3>");
            html.AppendLine("                <div class=\"detail-row\">");
            html.AppendLine("                    <span class=\"detail-label\">Booking Reference:</span>");
            html.AppendLine($"                    <span class=\"detail-value\">#{booking.Id:N}</span>");
            html.AppendLine("                </div>");
            html.AppendLine("                <div class=\"detail-row\">");
            html.AppendLine("                    <span class=\"detail-label\">Guest Name:</span>");
            html.AppendLine($"                    <span class=\"detail-value\">{guestName}</span>");
            html.AppendLine("                </div>");
            html.AppendLine("                <div class=\"detail-row\">");
            html.AppendLine("                    <span class=\"detail-label\">Room:</span>");
            html.AppendLine($"                    <span class=\"detail-value\">{booking.RoomName}</span>");
            html.AppendLine("                </div>");
            html.AppendLine("                <div class=\"detail-row\">");
            html.AppendLine("                    <span class=\"detail-label\">Check-in:</span>");
            html.AppendLine($"                    <span class=\"detail-value\">{booking.CheckIn:dddd, MMMM dd, yyyy}</span>");
            html.AppendLine("                </div>");
            html.AppendLine("                <div class=\"detail-row\">");
            html.AppendLine("                    <span class=\"detail-label\">Check-out:</span>");
            html.AppendLine($"                    <span class=\"detail-value\">{booking.CheckOut:dddd, MMMM dd, yyyy}</span>");
            html.AppendLine("                </div>");
            html.AppendLine("                <div class=\"detail-row\">");
            html.AppendLine("                    <span class=\"detail-label\">Status:</span>");
            html.AppendLine($"                    <span class=\"detail-value\">{booking.Status}</span>");
            html.AppendLine("                </div>");
            html.AppendLine("            </div>");

            html.AppendLine("            <div class=\"summary-section\">");
            html.AppendLine("                <h3>View Your Cancelled Booking Summary</h3>");
            html.AppendLine("                <p>Click the button below to view your cancelled booking details online:</p>");
            html.AppendLine($"                <a href=\"{_emailSettings.BaseUrl}/view-booking-summary?bookingtoken={booking.BookingToken}\" class=\"view-summary-btn\">View Booking Summary</a>");
            html.AppendLine("            </div>");

            html.AppendLine("            <p>We hope to welcome you back in the future!</p>");
            html.AppendLine("        </div>");
            html.AppendLine("        <div class=\"footer\">");
            html.AppendLine($"            <p>{_emailSettings.HotelInfo.Name}</p>");
            if (!string.IsNullOrWhiteSpace(_emailSettings.HotelInfo.Address))
            {
                html.AppendLine($"            <p>{_emailSettings.HotelInfo.Address}</p>");
            }
            if (!string.IsNullOrWhiteSpace(_emailSettings.HotelInfo.Phone))
            {
                html.AppendLine($"            <p>Phone: {_emailSettings.HotelInfo.Phone}</p>");
            }
            if (!string.IsNullOrWhiteSpace(_emailSettings.HotelInfo.Email))
            {
                html.AppendLine($"            <p>Email: {_emailSettings.HotelInfo.Email}</p>");
            }
            html.AppendLine("        </div>");
            html.AppendLine("    </div>");
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }
    }
} 