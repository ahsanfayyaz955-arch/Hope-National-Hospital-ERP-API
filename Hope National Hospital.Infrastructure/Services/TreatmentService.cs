using Hope_National_Hospital.Application.Dtos.Treatment;
using Hope_National_Hospital.Application.Interfaces;
using Hope_National_Hospital.Domain.Entities;
using Hope_National_Hospital.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Hope_National_Hospital.Infrastructure.Services
{
    public class TreatmentService : ITreatmentService
    {
        private readonly AppDbContext _context;

        private readonly IAuditService _auditService;

        public TreatmentService(AppDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }



        // =========================================================
        // CREATE
        // =========================================================
        public async Task<ResponseTreatmentDto> CreateAsync(
            CreateTreatmentDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentException(
                    "Treatment data is required.");
            }

            if (dto.PatientId <= 0)
            {
                throw new ArgumentException(
                    "Valid patient ID is required.");
            }

            if (dto.DoctorId <= 0)
            {
                throw new ArgumentException(
                    "Valid doctor ID is required.");
            }

            if (dto.AppointmentId <= 0)
            {
                throw new ArgumentException(
                    "Valid appointment ID is required.");
            }

            // =====================================================
            // PATIENT CHECK
            // =====================================================
            var patientExists = await _context.Patients
                .AnyAsync(p => p.Id == dto.PatientId);

            if (!patientExists)
            {
                throw new KeyNotFoundException(
                    "Patient not found.");
            }

            // =====================================================
            // DOCTOR CHECK
            // =====================================================
            var doctorExists = await _context.Doctor
                .AnyAsync(d => d.Id == dto.DoctorId);

            if (!doctorExists)
            {
                throw new KeyNotFoundException(
                    "Doctor not found.");
            }

            // =====================================================
            // APPOINTMENT CHECK
            // =====================================================
            var appointmentExists = await _context.Appointments
                .AnyAsync(a =>
                    a.Id == dto.AppointmentId &&
                    a.PatientId == dto.PatientId &&
                    a.DoctorId == dto.DoctorId);

            if (!appointmentExists)
            {
                throw new KeyNotFoundException(
                    "Appointment not found or it does not belong to the selected patient and doctor.");
            }

            // =====================================================
            // COST VALIDATION
            // =====================================================
            if (dto.Cost < 0)
            {
                throw new ArgumentException(
                    "Treatment cost cannot be negative.");
            }

            // =====================================================
            // CREATE TREATMENT
            // =====================================================
            var treatment = new Treatment
            {
                PatinetId = dto.PatientId,
                DoctorId = dto.DoctorId,
                AppointmentId = dto.AppointmentId,

                Diagnosis = dto.Diagnosis,
                Description = dto.Description,
                Cost = dto.Cost,
                TreatmentDate = dto.TreatmentDate,
                Notes = dto.Notes,
                medicationPrescribed = dto.medicationPrescribed
            };

            _context.Treatments.Add(treatment);

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
               "CREATE",
                 "Treatment",
                    treatment.Id.ToString(),
                   $"Treatment for patient '{treatment.PatinetId}' was created."
             );

            // =====================================================
            // RETURN CREATED TREATMENT
            // =====================================================
            return await GetByIdAsync(treatment.Id)
                ?? throw new InvalidOperationException(
                    "Unable to retrieve created treatment.");
        }


        // =========================================================
        // GET ALL
        // =========================================================
        public async Task<IEnumerable<ResponseTreatmentDto>>
            GetAllAsync()
        {
            return await _context.Treatments
                .AsNoTracking()
                .Select(t => new ResponseTreatmentDto
                {
                    Id = t.Id,

                    PatientId = t.PatinetId,

                    PatientName = t.Patient != null
                        ? t.Patient.FullName
                        : string.Empty,

                    DoctorId = t.DoctorId,

                    DoctorName = t.Doctor != null
                        ? t.Doctor.Name
                        : string.Empty,

                    Diagnosis = t.Diagnosis,

                    Description = t.Description,

                    Cost = t.Cost,

                    TreatmentDate = t.TreatmentDate,

                    Notes = t.Notes,

                    medicationPrescribed =
                        t.medicationPrescribed

                })
                .ToListAsync();
        }


        // =========================================================
        // GET BY ID
        // =========================================================
        public async Task<ResponseTreatmentDto?>
            GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Invalid treatment ID.");
            }

            return await _context.Treatments
                .AsNoTracking()
                .Where(t => t.Id == id)
                .Select(t => new ResponseTreatmentDto
                {
                    Id = t.Id,

                    PatientId = t.PatinetId,

                    PatientName = t.Patient != null
                        ? t.Patient.FullName
                        : string.Empty,

                    DoctorId = t.DoctorId,

                    DoctorName = t.Doctor != null
                        ? t.Doctor.Name
                        : string.Empty,

                    Diagnosis = t.Diagnosis,

                    Description = t.Description,

                    Cost = t.Cost,

                    TreatmentDate = t.TreatmentDate,

                    Notes = t.Notes,

                    medicationPrescribed =
                        t.medicationPrescribed

                })
                .FirstOrDefaultAsync();
        }


        // =========================================================
        // UPDATE
        // =========================================================
        public async Task<bool> UpdateAsync(
            int id,
            UpdateTreatmentDto dto)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Invalid treatment ID.");
            }

            if (dto == null)
            {
                throw new ArgumentException(
                    "Treatment data is required.");
            }

            // =====================================================
            // FIND TREATMENT
            // =====================================================
            var treatment = await _context.Treatments
                .FirstOrDefaultAsync(t => t.Id == id);

            if (treatment == null)
            {
                throw new KeyNotFoundException(
                    "Treatment not found.");
            }

            // =====================================================
            // COST VALIDATION
            // =====================================================
            if (dto.Cost < 0)
            {
                throw new ArgumentException(
                    "Treatment cost cannot be negative.");
            }

            // =====================================================
            // UPDATE
            // =====================================================
            treatment.Diagnosis =
                dto.Diagnosis;

            treatment.Description =
                dto.Description;

            treatment.Cost =
                dto.Cost;

            treatment.TreatmentDate =
                dto.TreatmentDate;

            treatment.Notes =
                dto.Notes;

            treatment.medicationPrescribed =
                dto.medicationPrescribed;

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                "UPDATE",
                 "Treatment",
                  treatment.Id.ToString(),
                    $"Treatment '{treatment.Id}' was updated."
            );

            return true;
        }


        // =========================================================
        // DELETE
        // =========================================================
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Invalid treatment ID.");
            }

            // =====================================================
            // FIND TREATMENT
            // =====================================================
            var treatment = await _context.Treatments
                .FirstOrDefaultAsync(t => t.Id == id);

            if (treatment == null)
            {
                throw new KeyNotFoundException(
                    "Treatment not found.");
            }

            // =====================================================
            // DELETE
            // =====================================================
            _context.Treatments.Remove(treatment);

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
             "DELETE",
              "Treatment",
               treatment.Id.ToString(),
                $"Treatment '{treatment.Id}' was deleted."
            );

            return true;
        }
    }
}