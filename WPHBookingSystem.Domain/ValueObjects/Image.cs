using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPHBookingSystem.Domain.ValueObjects
{
    /// <summary>
    /// Represents an image in the hotel room gallery.
    /// This is a value object that encapsulates image information for display purposes.
    /// 
    /// Images are used to showcase room amenities and provide visual information
    /// to potential guests during the booking process.
    /// </summary>
    public class GalleryImage
    {
        /// <summary>
        /// Gets or sets the filename of the image.
        /// This should be the complete filename including extension (e.g., "room-101.jpg").
        /// </summary>
        public string FileName { get; set; } = string.Empty;
    }
}
