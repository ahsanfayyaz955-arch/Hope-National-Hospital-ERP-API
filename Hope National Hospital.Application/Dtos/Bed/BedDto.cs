using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Dtos.Bed
{
    public class BedDto
    {
        public int Id { get; set; }

        public string BedNumber { get; set; } = string.Empty;

        public int RoomId { get; set; }

        public string? RoomNumber { get; set; }

        public string Status { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
