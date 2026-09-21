using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Dtos.Patient
{
    public class PatientResponseDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string? CNIC { get; set; }

        public string? Gender { get; set; }
        public DateTime? Dob { get; set;}

        public string? Email { get; set;}

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? BloodGroup { get; set; }
        public string? EmergencyContactName { get; set; }

        public string? EmergencyContactPhone { get; set; }

        public string? MedicalHistory { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set;}

        public int AppointmentCount { get; set; }
        public int TreatmentCount { get; set; }


    }
}
