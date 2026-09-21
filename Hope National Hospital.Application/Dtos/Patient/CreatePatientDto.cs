using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Dtos.Patient
{
    public class CreatePatientDto
    {
        [Required(ErrorMessage = "Patient Name IS Required.")]
        [StringLength(150,MinimumLength =2 ,ErrorMessage ="Patient Name Must Be Between 2 And 150 Characters.")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(20,ErrorMessage = "CNIC Cannot Exceed 20 Characters.")]
        public string? CNIC { get; set; }

        [StringLength(20,ErrorMessage = "Gender Cannot Exceed 20 Charcters,")]
        public string? Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Dob { get; set; }


        [Phone(ErrorMessage = "Invalid Phone Number.")]
        [StringLength(20 ,ErrorMessage = "Phone Number Cannot Exceed 20 Characters.")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string? Email { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
        public string? Address { get; set; }

        [RegularExpression(@"^(A\+|A-|B\+|B-|AB\+|AB-|O\+|O-)$", ErrorMessage = "Invalid blood group.")]
        public string? BloodGroup { get; set; }


        [Phone(ErrorMessage = "Invalid emergency contact phone number.")]
        [StringLength(20,ErrorMessage = "Emergency contact phone cannot exceed 20 characters.")]
        public string? EmergencyContactPhone { get; set; }


        [StringLength(150, ErrorMessage = "Emergency contact name cannot exceed 150 characters.")]
        public string? EmergenecyContactName { get; set; }

        [StringLength(2000, ErrorMessage = "Medical history cannot exceed 2000 characters.")]
        public string? MedicalHistory { get; set; }

    }
}
