using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Domain.Entities
{
    public class Patient
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? CNIC { get; set; } 

        public DateTime? Dob { get; set; }

        [MaxLength(20)]
        public string? Gender { get; set; }

        [MaxLength(20)]
        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? PhoneNumber { get; set; } 

        public string? MedicalHistory { get; set; }

        [MaxLength(50)]
        public string? BloodGroup { get; set; }

        [MaxLength(150)]
        public string? EmergencyContactName { get; set; }
        [MaxLength(150)]
        public string? EmergencyContactPhone { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

 

        public ICollection<Appointment>Appointment{ get; set; }=new List<Appointment>();

        public ICollection<Treatment> Treatment { get; set; } = new List<Treatment>();
        public ICollection<Admission> Admissions { get; set; }  = new List<Admission>();

    }
}
