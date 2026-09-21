using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Domain.Entities
{
    public class Treatment
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;

        public string medicationPrescribed { get; set; } = string.Empty;

        public decimal Cost { get; set; } 
        public string? Diagnosis { get; set; }
        public int? AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        public int PatinetId { get; set; }
        public Patient? Patient { get; set; }

        public DateTime TreatmentDate { get; set; }
        public string? Notes { get; set; }




    }
}
