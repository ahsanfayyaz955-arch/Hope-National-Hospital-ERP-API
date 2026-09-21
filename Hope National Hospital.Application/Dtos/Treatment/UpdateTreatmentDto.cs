using System.ComponentModel.DataAnnotations;

namespace Hope_National_Hospital.Application.Dtos.Treatment
{
    public class UpdateTreatmentDto
    {
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