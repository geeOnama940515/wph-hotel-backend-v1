using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPHBookingSystem.Application.DTOs
{
    public class FileUploadDto
    {
        [Required]
        public IFormFile File { get; set; }
    }
    public class RoomImageUploadDto
    {
        [Required]
        public List<IFormFile> Files { get; set; }
    }

    public class RoomImageDto
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
