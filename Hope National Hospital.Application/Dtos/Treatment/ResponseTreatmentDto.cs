using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Dtos.Treatment
{
    public class ResponseTreatmentDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;

        public string Diagnosis { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Cost { get; set; }
        public DateTime TreatmentDate { get; set; }
        public string? Notes { get; set; }
        public string medicationPrescribed { get; set; } = string.Empty;


    }
}
