
using Hope_National_Hospital.Application.Dtos.Appointment;
using Hope_National_Hospital.Application.Interfaces;
using Hope_National_Hospital.Domain.Entities;
using Hope_National_Hospital.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Hope_National_Hospital.Infrastructure.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _context;
        private readonly IAuditService auditService;

        public AppointmentService(AppDbContext context, IAuditService auditService)
        {
            _context = context;
            this.auditService = auditService;
        }



        // =========================================================
        // CREATE
        // =========================================================
        public async Task<AppointmentResponseDto> CreateAsync(
            CreateAppointmentDto dto)
        {
            // Doctor check
            var doctor = await _context.Doctor
                .FirstOrDefaultAsync(d => d.Id == dto.DoctorId);

            if (doctor == null)
            {
                throw new KeyNotFoundException(
                    "Doctor not found.");
            }

            if (!doctor.IsActive)
            {
                throw new ArgumentException(
                    "Doctor is not active.");
            }

            // Patient check
            var patientExists = await _context.Patients
                .AnyAsync(p => p.Id == dto.PatientId);

            if (!patientExists)
            {
                throw new KeyNotFoundException(
                    "Patient not found.");
            }

            // Receptionist check
            if (dto.ReceptionistId.HasValue)
            {
                var receptionistExists =
                    await _context.Receptionist
                        .AnyAsync(r =>
                            r.Id == dto.ReceptionistId.Value);

                if (!receptionistExists)
                {
                    throw new KeyNotFoundException(
                        "Receptionist not found.");
                }
            }

            // Doctor double booking
            var doctorBusy = await _context.Appointments
                .AnyAsync(a =>
                    a.DoctorId == dto.DoctorId &&
                    a.AppointmentDate == dto.AppointmentDate &&
                    a.Status != "Cancelled");

            if (doctorBusy)
            {
                throw new ArgumentException(
                    "Doctor already has an appointment at this time.");
            }

            // Patient double booking
            var patientBusy = await _context.Appointments
                .AnyAsync(a =>
                    a.PatientId == dto.PatientId &&
                    a.AppointmentDate == dto.AppointmentDate &&
                    a.Status != "Cancelled");

            if (patientBusy)
            {
                throw new ArgumentException(
                    "Patient already has an appointment at this time.");
            }

            // Create appointment
            var appointment = new Appointment
            {
                DoctorId = dto.DoctorId,
                PatientId = dto.PatientId,
                ReceptionistId = dto.ReceptionistId,
                AppointmentDate = dto.AppointmentDate,
                AppointmentTitle = dto.AppointmentTitle,
                Status = dto.Status,
                Reason = dto.Reason,
                Notes = dto.Notes
            };

            _context.Appointments.Add(appointment);

            await _context.SaveChangesAsync();

            await auditService.LogAsync(
              "CREATE",
               "Appointment",
                appointment.Id.ToString(),
                $"Appointment for patient '{appointment.PatientId}' with doctor '{appointment.DoctorId}' was created."
             );





            return await GetByIdAsync(appointment.Id)
                ?? throw new Exception(
                    "Unable to create appointment.");
        }


        // =========================================================
        // GET ALL
        // =========================================================
        public async Task<IEnumerable<AppointmentResponseDto>>
            GetAllAsync()
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Include(a => a.BookedByReceptionist)
                .Select(a => new AppointmentResponseDto
                {
                    Id = a.Id,

                    DoctorId = a.DoctorId,

                    DoctorName = a.Doctor != null
                        ? a.Doctor.Name
                        : string.Empty,

                    PatientId = a.PatientId,

                    PatientName = a.Patient != null
                        ? a.Patient.FullName
                        : string.Empty,

                    ReceptionistId = a.ReceptionistId,

                    ReceptionistName =
                        a.BookedByReceptionist != null
                            ? a.BookedByReceptionist.FullName
                            : null,

                    AppointmentDate =
                        a.AppointmentDate,

                    AppointmentTitle =
                        a.AppointmentTitle,

                    Status =
                        a.Status,

                    Reason =
                        a.Reason,

                    Notes =
                        a.Notes

                })
                .ToListAsync();
        }


        // =========================================================
        // GET BY ID
        // =========================================================
        public async Task<AppointmentResponseDto?>
            GetByIdAsync(int id)
        {
            return await _context.Appointments
                .AsNoTracking()
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Include(a => a.BookedByReceptionist)
                .Where(a => a.Id == id)
                .Select(a => new AppointmentResponseDto
                {
                    Id = a.Id,

                    DoctorId = a.DoctorId,

                    DoctorName = a.Doctor != null
                        ? a.Doctor.Name
                        : string.Empty,

                    PatientId = a.PatientId,

                    PatientName = a.Patient != null
                        ? a.Patient.FullName
                        : string.Empty,

                    ReceptionistId = a.ReceptionistId,

                    ReceptionistName =
                        a.BookedByReceptionist != null
                            ? a.BookedByReceptionist.FullName
                            : null,

                    AppointmentDate =
                        a.AppointmentDate,

                    AppointmentTitle =
                        a.AppointmentTitle,

                    Status =
                        a.Status,

                    Reason =
                        a.Reason,

                    Notes =
                        a.Notes

                })
                .FirstOrDefaultAsync();
        }


        // =========================================================
        // UPDATE
        // =========================================================
        public async Task<bool> UpdateAsync(
            int id,
            UpdateAppointmentDto dto)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return false;
            }

            // Doctor check
            var doctor = await _context.Doctor
                .FirstOrDefaultAsync(d => d.Id == dto.DoctorId);

            if (doctor == null)
            {
                throw new KeyNotFoundException(
                    "Doctor not found.");
            }

            if (!doctor.IsActive)
            {
                throw new ArgumentException(
                    "Doctor is not active.");
            }

            // Patient check
            var patientExists = await _context.Patients
                .AnyAsync(p => p.Id == dto.PatientId);

            if (!patientExists)
            {
                throw new KeyNotFoundException(
                    "Patient not found.");
            }

            // Receptionist check
            if (dto.ReceptionistId.HasValue)
            {
                var receptionistExists =
                    await _context.Receptionist
                        .AnyAsync(r =>
                            r.Id == dto.ReceptionistId.Value);

                if (!receptionistExists)
                {
                    throw new KeyNotFoundException(
                        "Receptionist not found.");
                }
            }

            // Doctor double booking
            var doctorBusy = await _context.Appointments
                .AnyAsync(a =>
                    a.Id != id &&
                    a.DoctorId == dto.DoctorId &&
                    a.AppointmentDate == dto.AppointmentDate &&
                    a.Status != "Cancelled");

            if (doctorBusy)
            {
                throw new ArgumentException(
                    "Doctor already has an appointment at this time.");
            }

            // Patient double booking
            var patientBusy = await _context.Appointments
                .AnyAsync(a =>
                    a.Id != id &&
                    a.PatientId == dto.PatientId &&
                    a.AppointmentDate == dto.AppointmentDate &&
                    a.Status != "Cancelled");

            if (patientBusy)
            {
                throw new ArgumentException(
                    "Patient already has an appointment at this time.");
            }

            // Update
            appointment.DoctorId =
                dto.DoctorId;

            appointment.PatientId =
                dto.PatientId;

            appointment.ReceptionistId =
                dto.ReceptionistId;

            appointment.AppointmentDate =
                dto.AppointmentDate;

            appointment.AppointmentTitle =
                dto.AppointmentTitle;

            appointment.Status =
                dto.Status;

            appointment.Reason =
                dto.Reason;

            appointment.Notes =
                dto.Notes;

            await _context.SaveChangesAsync();

            await auditService.LogAsync(
               "UPDATE",
              "Appointment",
                appointment.Id.ToString(),
                   $"Appointment '{appointment.Id}' was updated."
             );

            return true;
        }


        // =========================================================
        // DELETE
        // =========================================================
        public async Task<bool> DeleteAsync(int id)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return false;
            }

            _context.Appointments.Remove(appointment);

            await _context.SaveChangesAsync();

            await auditService.LogAsync(
                  "DELETE",
                "Appointment",
                   appointment.Id.ToString(),
                  $"Appointment '{appointment.Id}' was deleted."
            );

            return true;
        }
    }
}

