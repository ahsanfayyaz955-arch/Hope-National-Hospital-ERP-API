using Hope_National_Hospital.Application.Dtos.Receptionist;
using Hope_National_Hospital.Application.Interfaces;
using Hope_National_Hospital.Domain.Entities;
using Hope_National_Hospital.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Hope_National_Hospital.Infrastructure.Services
{
    public class ReceptionistService : IReceptionistService
    {
        private readonly AppDbContext _context;
        private readonly IAuditService _auditService;

        public ReceptionistService(AppDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        // =========================================================
        // CREATE
        // =========================================================
        public async Task<ResponseReceptionistDto> CreateAsync(
            CreateReceptionistDto dto)
        {
            // Basic validation
            if (dto == null)
            {
                throw new ArgumentException(
                    "Receptionist data is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                throw new ArgumentException(
                    "Receptionist full name is required.");
            }

            if (dto.DepartmentId <= 0)
            {
                throw new ArgumentException(
                    "Valid department is required.");
            }

            // =====================================================
            // CHECK DEPARTMENT
            // =====================================================
            var departmentExists = await _context.Departments
                .AnyAsync(d => d.Id == dto.DepartmentId);

            if (!departmentExists)
            {
                throw new KeyNotFoundException(
                    "Department not found.");
            }

            // =====================================================
            // DUPLICATE EMAIL CHECK
            // =====================================================
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var normalizedEmail = dto.Email.Trim();

                var emailExists = await _context.Receptionist
                    .AnyAsync(r =>
                        r.emailAddress == normalizedEmail);

                if (emailExists)
                {
                    throw new InvalidOperationException(
                        "Receptionist with this email already exists.");
                }
            }

            // =====================================================
            // CREATE RECEPTIONIST
            // =====================================================
            var receptionist = new Receptionist
            {
                FullName = dto.FullName.Trim(),
                PhoneNumber = dto.Phone?.Trim(),
                emailAddress = string.IsNullOrWhiteSpace(dto.Email)
                    ? null
                    : dto.Email.Trim(),
                DepartmentId = dto.DepartmentId,
                IsActive = true
            };

            _context.Receptionist.Add(receptionist);

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
             "CREATE",
              "Receptionist",
                receptionist.Id.ToString(),
                $"Receptionist '{receptionist.FullName}' was created."
            );

            // =====================================================
            // RETURN CREATED RECEPTIONIST
            // =====================================================
            return await GetByIdAsync(receptionist.Id)
                ?? throw new InvalidOperationException(
                    "Unable to create receptionist.");
        }


        // =========================================================
        // GET ALL
        // =========================================================
        public async Task<IEnumerable<ResponseReceptionistDto>>
            GetAllAsync()
        {
            return await _context.Receptionist
                .AsNoTracking()
                .Select(r => new ResponseReceptionistDto
                {
                    Id = r.Id,

                    FullName = r.FullName,

                    Phone = r.PhoneNumber,

                    Email = r.emailAddress,

                    DepartmentId = r.DepartmentId,

                    DepartmentName = r.Department != null
                        ? r.Department.Name
                        : string.Empty,

                    IsActive = r.IsActive

                })
                .ToListAsync();
        }


        // =========================================================
        // GET BY ID
        // =========================================================
        public async Task<ResponseReceptionistDto?>
            GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Invalid receptionist ID.");
            }

            return await _context.Receptionist
                .AsNoTracking()
                .Where(r => r.Id == id)
                .Select(r => new ResponseReceptionistDto
                {
                    Id = r.Id,

                    FullName = r.FullName,

                    Phone = r.PhoneNumber,

                    Email = r.emailAddress,

                    DepartmentId = r.DepartmentId,

                    DepartmentName = r.Department != null
                        ? r.Department.Name
                        : string.Empty,

                    IsActive = r.IsActive

                })
                .FirstOrDefaultAsync();
        }


        // =========================================================
        // UPDATE
        // =========================================================
        public async Task<bool> UpdateAsync(
            int id,
            UpdateReceptionistDto dto)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Invalid receptionist ID.");
            }

            if (dto == null)
            {
                throw new ArgumentException(
                    "Receptionist data is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                throw new ArgumentException(
                    "Receptionist full name is required.");
            }

            if (dto.DepartmentId <= 0)
            {
                throw new ArgumentException(
                    "Valid department is required.");
            }

            // =====================================================
            // FIND RECEPTIONIST
            // =====================================================
            var receptionist = await _context.Receptionist
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receptionist == null)
            {
                throw new KeyNotFoundException(
                    "Receptionist not found.");
            }

            // =====================================================
            // CHECK DEPARTMENT
            // =====================================================
            var departmentExists = await _context.Departments
                .AnyAsync(d => d.Id == dto.DepartmentId);

            if (!departmentExists)
            {
                throw new KeyNotFoundException(
                    "Department not found.");
            }

            // =====================================================
            // DUPLICATE EMAIL CHECK
            // =====================================================
            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var normalizedEmail = dto.Email.Trim();

                var emailExists = await _context.Receptionist
                    .AnyAsync(r =>
                        r.Id != id &&
                        r.emailAddress == normalizedEmail);

                if (emailExists)
                {
                    throw new InvalidOperationException(
                        "Another receptionist with this email already exists.");
                }
            }

            // =====================================================
            // UPDATE
            // =====================================================
            receptionist.FullName =
                dto.FullName.Trim();

            receptionist.PhoneNumber =
                dto.Phone?.Trim();

            receptionist.emailAddress =
                string.IsNullOrWhiteSpace(dto.Email)
                    ? null
                    : dto.Email.Trim();

            receptionist.DepartmentId =
                dto.DepartmentId;

            receptionist.IsActive =
                dto.IsActive;

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
              "UPDATE",
                "Receptionist",
                receptionist.Id.ToString(),
               $"Receptionist '{receptionist.FullName}' was updated."
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
                    "Invalid receptionist ID.");
            }

            // =====================================================
            // FIND RECEPTIONIST
            // =====================================================
            var receptionist = await _context.Receptionist
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receptionist == null)
            {
                throw new KeyNotFoundException(
                    "Receptionist not found.");
            }

            // =====================================================
            // CHECK APPOINTMENTS
            // =====================================================
            var hasAppointments =
                await _context.Appointments
                    .AnyAsync(a =>
                        a.ReceptionistId == id);

            if (hasAppointments)
            {
                throw new InvalidOperationException(
                    "Receptionist cannot be deleted because appointments exist.");
            }

            // =====================================================
            // DELETE
            // =====================================================
            _context.Receptionist.Remove(receptionist);

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
             "DELETE",
              "Receptionist",
               receptionist.Id.ToString(),
               $"Receptionist '{receptionist}' was deleted."
             );

            return true;
        }
    }
}