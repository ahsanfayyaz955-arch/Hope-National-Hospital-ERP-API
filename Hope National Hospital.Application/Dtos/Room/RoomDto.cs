using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Dtos.Room
{
    public class RoomDto
    {
        public int Id { get; set; }

        public string RoomNumber { get; set; } = string.Empty;

        public string RoomType { get; set; } = string.Empty;

        public int DepartmentId { get; set; }

        public string? DepartmentName { get; set; }

        public bool IsActive { get; set; }

        public int TotalBeds { get; set; }

        public int AvailableBeds { get; set; }
    }
}
