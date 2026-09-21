using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Domain.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;

        public string RoomType { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public bool IsActive { get; set; } = true;

        //Navigation 
        public Department? Department { get; set; }
        public ICollection<Bed> Beds { get; set; } = new List<Bed>();
    }
}
