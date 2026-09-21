
using Hope_National_Hospital.Application.Constants;
using Hope_National_Hospital.Application.Dtos.Doctor;
using Hope_National_Hospital.Application.Interfaces;
using Hope_National_Hospital.Domain.Entities;
using Hope_National_Hospital.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Hope_National_Hospital.Infrastructure.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly AppDbContext _context;
        private readonly IAuditService _auditService;
        private readonly IMemoryCache _cache;


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public DoctorService(
            AppDbContext context,
            IAuditService auditService,
            IMemoryCache cache)
        {
            _context = context;
            _auditService = auditService;
            _cache = cache;
        }


        // =========================================================
        // CREATE
        // =========================================================

        public async Task<DoctorResponseDto> CreateAsync(
            CreateDoctorDto dto)
        {
            // -----------------------------------------------------
            // Required field validation
            // -----------------------------------------------------

            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                throw new ArgumentException(
                    "Doctor name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Specialization))
            {
                throw new ArgumentException(
                    "Doctor specialization is required.");
            }

            if (dto.DepartmentId <= 0)
            {
                throw new ArgumentException(
                    "Valid department is required.");
            }

            if (dto.ConsultationFee < 0)
            {
                throw new ArgumentException(
                    "Consultation fee cannot be negative.");
            }


            // -----------------------------------------------------
            // Check Department
            // -----------------------------------------------------

            var departmentExists =
                await _context.Departments
                    .AnyAsync(d =>
                        d.Id == dto.DepartmentId);

            if (!departmentExists)
            {
                throw new KeyNotFoundException(
                    "Department not found.");
            }


            // -----------------------------------------------------
            // Check duplicate email
            // -----------------------------------------------------

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var email = dto.Email.Trim();

                var emailExists =
                    await _context.Doctor
                        .AnyAsync(d =>
                            d.EmailAddress != null &&
                            d.EmailAddress.ToLower() ==
                            email.ToLower());

                if (emailExists)
                {
                    throw new ArgumentException(
                        "Doctor with this email already exists.");
                }
            }


            // -----------------------------------------------------
            // Create Doctor
            // -----------------------------------------------------

            var doctor = new Doctor
            {
                Name =
                    dto.FullName.Trim(),

                Specialization =
                    dto.Specialization.Trim(),

                PhoneNumber =
                    string.IsNullOrWhiteSpace(dto.Phone)
                        ? null
                        : dto.Phone.Trim(),

                EmailAddress =
                    string.IsNullOrWhiteSpace(dto.Email)
                        ? null
                        : dto.Email.Trim(),

                ConsultationFee =
                    dto.ConsultationFee,

                DepartmentId =
                    dto.DepartmentId,

                IsActive = true
            };


            _context.Doctor.Add(doctor);

            await _context.SaveChangesAsync();


            // -----------------------------------------------------
            // CACHE INVALIDATION
            // -----------------------------------------------------

            // Doctor list changed
            _cache.Remove(CacheKeys.Doctors);

            // Department DoctorCount changed
            _cache.Remove(CacheKeys.Departments);

            // Audit
            await _auditService.LogAsync(
                "CREATE",
                "Doctor",
                doctor.Id.ToString(),
                $"Doctor '{doctor.Name}' was created."
            );


            // -----------------------------------------------------
            // Return created doctor
            // -----------------------------------------------------

            return await GetByIdAsync(doctor.Id)
                ?? throw new InvalidOperationException(
                    "Unable to create doctor.");
        }


        // =========================================================
        // GET ALL
        // =========================================================

        public async Task<IEnumerable<DoctorResponseDto>>
            GetAllAsync()
        {
            // -----------------------------------------------------
            // Check Cache
            // -----------------------------------------------------

            if (_cache.TryGetValue(
                CacheKeys.Doctors,
                out List<DoctorResponseDto>? cachedDoctors))
            {
                return cachedDoctors!;
            }


            // -----------------------------------------------------
            // Database Query
            // -----------------------------------------------------

            var doctors = await _context.Doctor
                .AsNoTracking()
                .Select(d => new DoctorResponseDto
                {
                    Id =
                        d.Id,

                    FullName =
                        d.Name,

                    Specialization =
                        d.Specialization,

                    Phone =
                        d.PhoneNumber,

                    Email =
                        d.EmailAddress,

                    ConsultationFee =
                        d.ConsultationFee,

                    IsActive =
                        d.IsActive,

                    DepartmentId =
                        d.DepartmentId,

                    DepartmentName =
                        d.Department != null
                            ? d.Department.Name
                            : string.Empty
                })
                .ToListAsync();


            // -----------------------------------------------------
            // Cache Options
            // -----------------------------------------------------

            var cacheOptions =
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        TimeSpan.FromMinutes(10),

                    SlidingExpiration =
                        TimeSpan.FromMinutes(5)
                };


            // -----------------------------------------------------
            // Save to Cache
            // -----------------------------------------------------

            _cache.Set(
                CacheKeys.Doctors,
                doctors,
                cacheOptions);


            return doctors;
        }


        // =========================================================
        // GET BY ID
        // =========================================================

        public async Task<DoctorResponseDto?>
            GetByIdAsync(int id)
        {
            // -----------------------------------------------------
            // Validate ID
            // -----------------------------------------------------

            if (id <= 0)
            {
                throw new ArgumentException(
                    "Invalid doctor ID.");
            }


            // -----------------------------------------------------
            // Individual Cache Key
            // -----------------------------------------------------

            var cacheKey =
                CacheKeys.Doctor(id);


            // -----------------------------------------------------
            // Check Cache
            // -----------------------------------------------------

            if (_cache.TryGetValue(
                cacheKey,
                out DoctorResponseDto? cachedDoctor))
            {
                return cachedDoctor;
            }


            // -----------------------------------------------------
            // Database Query
            // -----------------------------------------------------

            var doctor = await _context.Doctor
                .AsNoTracking()
                .Where(d =>
                    d.Id == id)
                .Select(d => new DoctorResponseDto
                {
                    Id =
                        d.Id,

                    FullName =
                        d.Name,

                    Specialization =
                        d.Specialization,

                    Phone =
                        d.PhoneNumber,

                    Email =
                        d.EmailAddress,

                    ConsultationFee =
                        d.ConsultationFee,

                    IsActive =
                        d.IsActive,

                    DepartmentId =
                        d.DepartmentId,

                    DepartmentName =
                        d.Department != null
                            ? d.Department.Name
                            : string.Empty
                })
                .FirstOrDefaultAsync();


            // -----------------------------------------------------
            // Not Found
            // -----------------------------------------------------

            if (doctor == null)
            {
                return null;
            }


            // -----------------------------------------------------
            // Cache Options
            // -----------------------------------------------------

            var cacheOptions =
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        TimeSpan.FromMinutes(10),

                    SlidingExpiration =
                        TimeSpan.FromMinutes(5)
                };


            // -----------------------------------------------------
            // Save to Cache
            // -----------------------------------------------------

            _cache.Set(
                cacheKey,
                doctor,
                cacheOptions);


            return doctor;
        }


        // =========================================================
        // UPDATE
        // =========================================================

        public async Task<bool> UpdateAsync(
            int id,
            UpdateDoctorDto dto)
        {
            // -----------------------------------------------------
            // Required validation
            // -----------------------------------------------------

            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                throw new ArgumentException(
                    "Doctor name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Specialization))
            {
                throw new ArgumentException(
                    "Doctor specialization is required.");
            }

            if (dto.DepartmentId <= 0)
            {
                throw new ArgumentException(
                    "Valid department is required.");
            }

            if (dto.ConsultationFee < 0)
            {
                throw new ArgumentException(
                    "Consultation fee cannot be negative.");
            }


            // -----------------------------------------------------
            // Find Doctor
            // -----------------------------------------------------

            var doctor =
                await _context.Doctor
                    .FirstOrDefaultAsync(d =>
                        d.Id == id);

            if (doctor == null)
            {
                return false;
            }


            // Save old department for cache invalidation
            var oldDepartmentId =
                doctor.DepartmentId;


            // -----------------------------------------------------
            // Check Department
            // -----------------------------------------------------

            var departmentExists =
                await _context.Departments
                    .AnyAsync(d =>
                        d.Id == dto.DepartmentId);

            if (!departmentExists)
            {
                throw new KeyNotFoundException(
                    "Department not found.");
            }


            // -----------------------------------------------------
            // Check duplicate email
            // -----------------------------------------------------

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                var email =
                    dto.Email.Trim();

                var emailExists =
                    await _context.Doctor
                        .AnyAsync(d =>
                            d.Id != id &&
                            d.EmailAddress != null &&
                            d.EmailAddress.ToLower() ==
                            email.ToLower());

                if (emailExists)
                {
                    throw new ArgumentException(
                        "Another doctor with this email already exists.");
                }
            }


            // -----------------------------------------------------
            // Update Doctor
            // -----------------------------------------------------

            doctor.Name =
                dto.FullName.Trim();

            doctor.Specialization =
                dto.Specialization.Trim();

            doctor.PhoneNumber =
                string.IsNullOrWhiteSpace(dto.Phone)
                    ? null
                    : dto.Phone.Trim();

            doctor.EmailAddress =
                string.IsNullOrWhiteSpace(dto.Email)
                    ? null
                    : dto.Email.Trim();

            doctor.ConsultationFee =
                dto.ConsultationFee;

            doctor.DepartmentId =
                dto.DepartmentId;

            doctor.IsActive =
                dto.IsActive;


            await _context.SaveChangesAsync();


            // -----------------------------------------------------
            // CACHE INVALIDATION
            // -----------------------------------------------------

            // Doctor list changed
            _cache.Remove(CacheKeys.Doctors);

            // Individual doctor cache changed
            _cache.Remove(
                CacheKeys.Doctor(id));

            // Department counts may have changed
            _cache.Remove(CacheKeys.Departments);

            // If doctor moved from one department to another,
            // both department representations may be affected.
            if (oldDepartmentId != dto.DepartmentId)
            {
                _cache.Remove(CacheKeys.Departments);
            }


            // -----------------------------------------------------
            // Audit
            // -----------------------------------------------------

            await _auditService.LogAsync(
                "UPDATE",
                "Doctor",
                doctor.Id.ToString(),
                $"Doctor '{doctor.Name}' was updated."
            );


            return true;
        }


        // =========================================================
        // DELETE
        // =========================================================

        public async Task<bool> DeleteAsync(int id)
        {
            // -----------------------------------------------------
            // Find Doctor
            // -----------------------------------------------------

            var doctor =
                await _context.Doctor
                    .FirstOrDefaultAsync(d =>
                        d.Id == id);

            if (doctor == null)
            {
                return false;
            }


            // Save department for cache invalidation
            var departmentId =
                doctor.DepartmentId;


            // -----------------------------------------------------
            // Check Appointments
            // -----------------------------------------------------

            var hasAppointments =
                await _context.Appointments
                    .AnyAsync(a =>
                        a.DoctorId == id);

            if (hasAppointments)
            {
                throw new InvalidOperationException(
                    "Doctor cannot be deleted because appointments exist.");
            }


            // -----------------------------------------------------
            // Delete Doctor
            // -----------------------------------------------------

            _context.Doctor.Remove(doctor);

            await _context.SaveChangesAsync();


            // -----------------------------------------------------
            // CACHE INVALIDATION
            // -----------------------------------------------------

            // Doctor list changed
            _cache.Remove(CacheKeys.Doctors);

            // Individual doctor cache
            _cache.Remove(
                CacheKeys.Doctor(id));

            // Department DoctorCount changed
            _cache.Remove(CacheKeys.Departments);


            // -----------------------------------------------------
            // Audit
            // -----------------------------------------------------

            await _auditService.LogAsync(
                "DELETE",
                "Doctor",
                doctor.Id.ToString(),
                $"Doctor '{doctor.Name}' was deleted."
            );


            return true;
        }
    }
}

