using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Domain.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<Receptionist> receptionists { get; set; } = new List<Receptionist>();

        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();


    }
}
