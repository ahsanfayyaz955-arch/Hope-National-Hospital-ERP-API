using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Domain.Entities
{
    public class Appointment
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        public int? ReceptionistId { get; set; }
        public Receptionist? BookedByReceptionist { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string AppointmentTitle { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string? Reason { get; set; }
        public string? Notes { get; set; }


        public ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();





    }
}