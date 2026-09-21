
using Hope_National_Hospital.Application.Dtos.Patient;
using Hope_National_Hospital.Application.Interfaces;
using Hope_National_Hospital.Domain.Entities;
using Hope_National_Hospital.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Hope_National_Hospital.Infrastructure.Services
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _context;
        private readonly IAuditService _auditService;

        public PatientService(AppDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }




        // =========================================================
        // CREATE
        // =========================================================
        public async Task<PatientResponseDto> CreateAsync(
            CreatePatientDto dto)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                throw new ArgumentException(
                    "Patient full name is required.");
            }

            // CNIC duplicate check
            if (!string.IsNullOrWhiteSpace(dto.CNIC))
            {
                var cnicExists = await _context.Patients
                    .AnyAsync(p => p.CNIC == dto.CNIC);

                if (cnicExists)
                {
                    throw new InvalidOperationException(
                        "Patient with this CNIC already exists.");
                }
            }

            // Email duplicate check
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var emailExists = await _context.Patients
                    .AnyAsync(p => p.Email == dto.Email);

                if (emailExists)
                {
                    throw new InvalidOperationException(
                        "Patient with this email already exists.");
                }
            }

            var patient = new Patient
            {
                FullName = dto.FullName,
                CNIC = dto.CNIC,
                Gender = dto.Gender,
                Dob = dto.Dob,
                PhoneNumber = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                BloodGroup = dto.BloodGroup,
                EmergencyContactName =
                    dto.EmergenecyContactName,
                EmergencyContactPhone =
                    dto.EmergencyContactPhone,
                MedicalHistory = dto.MedicalHistory,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Patients.Add(patient);

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
           "CREATE",
           "Patient",
            patient.Id.ToString(),
            $"Patient '{patient.FullName}' was created."
            );

            return await GetByIdAsync(patient.Id)
                ?? throw new InvalidOperationException(
                    "Unable to create patient.");
        }


        // =========================================================
        // GET ALL
        // =========================================================
        public async Task<IEnumerable<PatientResponseDto>>
            GetAllAsync()
        {
            return await _context.Patients
                .AsNoTracking()
                .Select(p => new PatientResponseDto
                {
                    Id = p.Id,

                    FullName =
                        p.FullName,

                    CNIC =
                        p.CNIC,

                    Gender =
                        p.Gender,

                    Dob =
                        p.Dob,

                    Phone =
                        p.PhoneNumber,

                    Email =
                        p.Email,

                    Address =
                        p.Address,

                    BloodGroup =
                        p.BloodGroup,

                    EmergencyContactName =
                        p.EmergencyContactName,

                    EmergencyContactPhone =
                        p.EmergencyContactPhone,

                    MedicalHistory =
                        p.MedicalHistory,

                    IsActive =
                        p.IsActive,

                    CreatedAt =
                        p.CreatedAt,

                    AppointmentCount =
                        p.Appointment.Count(),

                    TreatmentCount =
                        p.Treatment.Count()
                })
                .ToListAsync();
        }


        // =========================================================
        // GET BY ID
        // =========================================================
        public async Task<PatientResponseDto?> GetByIdAsync(
            int id)
        {
            return await _context.Patients
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new PatientResponseDto
                {
                    Id =
                        p.Id,

                    FullName =
                        p.FullName,

                    CNIC =
                        p.CNIC,

                    Gender =
                        p.Gender,

                    Dob =
                        p.Dob,

                    Phone =
                        p.PhoneNumber,

                    Email =
                        p.Email,

                    Address =
                        p.Address,

                    BloodGroup =
                        p.BloodGroup,

                    EmergencyContactName =
                        p.EmergencyContactName,

                    EmergencyContactPhone =
                        p.EmergencyContactPhone,

                    MedicalHistory =
                        p.MedicalHistory,

                    IsActive =
                        p.IsActive,

                    CreatedAt =
                        p.CreatedAt,

                    // FIXED
                    AppointmentCount =
                        p.Appointment.Count(),

                    TreatmentCount =
                        p.Treatment.Count()
                })
                .FirstOrDefaultAsync();
        }


        // =========================================================
        // UPDATE
        // =========================================================
        public async Task<bool> UpdateAsync(
            int id,
            UpdatePatientDto dto)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
            {
                return false;
            }

            // Don't allow editing inactive patient
            // unless your business logic requires it.
            // Remove this block if inactive patients
            // should still be editable.
            if (!patient.IsActive)
            {
                throw new InvalidOperationException(
                    "Inactive patient cannot be updated.");
            }

            // CNIC duplicate check
            if (!string.IsNullOrWhiteSpace(dto.CNIC))
            {
                var cnicExists = await _context.Patients
                    .AnyAsync(p =>
                        p.Id != id &&
                        p.CNIC == dto.CNIC);

                if (cnicExists)
                {
                    throw new InvalidOperationException(
                        "Another patient with this CNIC already exists.");
                }
            }

            // Email duplicate check
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var emailExists = await _context.Patients
                    .AnyAsync(p =>
                        p.Id != id &&
                        p.Email == dto.Email);

                if (emailExists)
                {
                    throw new InvalidOperationException(
                        "Another patient with this email already exists.");
                }
            }

            // Update patient
            patient.FullName =
                dto.FullName;

            patient.CNIC =
                dto.CNIC;

            patient.Gender =
                dto.Gender;

            patient.Dob =
                dto.Dob;

            patient.PhoneNumber =
                dto.Phone;

            patient.Email =
                dto.Email;

            patient.Address =
                dto.Address;

            patient.BloodGroup =
                dto.BloodGroup;

            patient.EmergencyContactName =
                dto.EmergencyContactName;

            patient.EmergencyContactPhone =
                dto.EmergencyContactPhone;

            patient.MedicalHistory =
                dto.MedicalHistory;

            patient.IsActive =
                dto.IsActive;

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
              "UPDATE",
              "Patient",
               patient.Id.ToString(),
              $"Patient '{patient.FullName}' was updated."
             );

            return true;
        }


        // =========================================================
        // DELETE
        // =========================================================
        public async Task<bool> DeleteAsync(int id)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
            {
                return false;
            }

            // =====================================================
            // CHECK APPOINTMENTS
            // =====================================================

            var hasAppointments =
                await _context.Appointments
                    .AnyAsync(a => a.PatientId == id);

            if (hasAppointments)
            {
                throw new InvalidOperationException(
                    "Patient cannot be deleted because appointments exist.");
            }

            // =====================================================
            // CHECK TREATMENTS
            // =====================================================

            var hasTreatments =
                await _context.Treatments
                    .AnyAsync(t => t.PatinetId == id);

            if (hasTreatments)
            {
                throw new InvalidOperationException(
                    "Patient cannot be deleted because treatments exist.");
            }

            // =====================================================
            // CHECK INVOICES
            // =====================================================

            var hasInvoices =
                await _context.Invoices
                    .AnyAsync(i => i.PatientId == id);

            if (hasInvoices)
            {
                throw new InvalidOperationException(
                    "Patient cannot be deleted because invoices exist.");
            }

            _context.Patients.Remove(patient);

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
              "DELETE",
               "Patient",
              patient.Id.ToString(),
              $"Patient '{patient}' was deleted."
               );

            return true;
        }
    }
}

