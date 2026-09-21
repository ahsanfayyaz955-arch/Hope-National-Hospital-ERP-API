using System.ComponentModel.DataAnnotations;

namespace Hope_National_Hospital.Application.Dtos.Treatment
{
    public class CreateTreatmentDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int PatientId { get; set; }


        [Required]
        [Range(1, int.MaxValue)]
        public int DoctorId { get; set; }


        [Range(1, int.MaxValue)]
        public int? AppointmentId { get; set; }


        [Required]
        [StringLength(500)]
        public string Diagnosis { get; set; } = string.Empty;


        [Required]
        [StringLength(2000)]
        public string Description { get; set; } = string.Empty;


        [Range(0, double.MaxValue)]
        public decimal Cost { get; set; }


        [Required]
        public DateTime TreatmentDate { get; set; }


        [StringLength(2000)]
        public string? Notes { get; set; }


        [Required]
        [StringLength(2000)]
        public string medicationPrescribed { get; set; } = string.Empty;
    }
}