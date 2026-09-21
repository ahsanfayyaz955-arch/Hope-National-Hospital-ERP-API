using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Domain.Entities
{
    public class Receptionist
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;
        public string? emailAddress { get; set; }

        public int DepartmentId { get; set; }

        public Department? Department { get; set; }

        public bool IsActive { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    }
}
