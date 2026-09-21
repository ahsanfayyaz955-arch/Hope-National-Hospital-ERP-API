namespace Hope_National_Hospital.Application.Dtos.Admission
{
    public class AdmissionDto
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;

        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;

        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;

        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;

        public int BedId { get; set; }
        public string BedNumber { get; set; } = string.Empty;

        public string? Reason { get; set; }

        public DateTime AdmissionDate { get; set; }

        public DateTime? DischargeDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}