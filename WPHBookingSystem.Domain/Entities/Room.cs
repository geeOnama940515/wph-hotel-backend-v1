using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Domain.Entities.Common;
using WPHBookingSystem.Domain.Enums;

namespace WPHBookingSystem.Domain.Entities
{
    public class Room : BaseAuditable
    {
        public Guid Id { get; set; }  // PK

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Capacity { get; set; }

        public string Image { get; set; } = string.Empty;

        public RoomStatus Status { get; set; } = RoomStatus.Available;
    }
}
