using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Domain.Entities
{
    public class Doctor
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public int DepartmentId { get; set; }

        public Department? Department { get; set; } 

        public decimal ConsultationFee { get; set; }

        public bool IsActive { get; set;}

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
