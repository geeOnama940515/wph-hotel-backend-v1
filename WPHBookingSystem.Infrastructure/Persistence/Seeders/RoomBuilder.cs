using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPHBookingSystem.Domain.Entities;
using WPHBookingSystem.Domain.Enums;

namespace WPHBookingSystem.Infrastructure.Persistence.Seeders
{
    public class RoomBuilder
    {
        private readonly Room _room = (Room)Activator.CreateInstance(typeof(Room), true)!;

        public RoomBuilder WithId(Guid id) { typeof(Room).GetProperty("Id")!.SetValue(_room, id); return this; }
        public RoomBuilder WithName(string name) { typeof(Room).GetProperty("Name")!.SetValue(_room, name); return this; }
        public RoomBuilder WithDescription(string description) { typeof(Room).GetProperty("Description")!.SetValue(_room, description); return this; }
        public RoomBuilder WithPrice(decimal price) { typeof(Room).GetProperty("Price")!.SetValue(_room, price); return this; }
        public RoomBuilder WithCapacity(int capacity) { typeof(Room).GetProperty("Capacity")!.SetValue(_room, capacity); return this; }
        public RoomBuilder WithStatus(RoomStatus status) { typeof(Room).GetProperty("Status")!.SetValue(_room, status); return this; }
        public Room Build() => _room;
    }

}
