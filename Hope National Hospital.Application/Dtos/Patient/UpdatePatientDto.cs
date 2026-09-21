using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Dtos.Patient
{
    public class UpdatePatientDto
    {
        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? CNIC { get; set; }

        [MaxLength(20)]
        public string? Gender { get; set; }

        public DateTime? Dob { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(150)]
        public string? EmergencyContactName { get; set; }

        [MaxLength(20)]
        public string? EmergencyContactPhone { get; set; }

        [MaxLength(1000)]
        public string? MedicalHistory { get; set; }

        [MaxLength(50)]
        public string? BloodGroup { get; set; }

        public bool IsActive { get; set; }


    }
}
