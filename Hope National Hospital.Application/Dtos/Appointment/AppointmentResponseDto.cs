using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Dtos.Appointment
{
    public class AppointmentResponseDto
    {
        public int Id { get; set; }

        public int DoctorId { get; set; }

        public string? DoctorName { get; set; }

        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public int? ReceptionistId { get; set; }
        public string? ReceptionistName { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string AppointmentTitle { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }

        public string? Notes { get; set; }

    }
}
