using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Domain.Entities
{
    public class Bed
    {
        public int Id { get; set; }
        public string BedNumber { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public string Status { get; set; } = "Available";
        public bool IsActive { get; set; } = true;
        //Navigation 
        public Room? Room { get; set; }
    }
}
