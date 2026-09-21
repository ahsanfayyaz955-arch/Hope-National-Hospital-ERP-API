using Hope_National_Hospital.Application.Dtos.Admission;
using Hope_National_Hospital.Application.Interfaces;
using Hope_National_Hospital.Domain.Entities;
using Hope_National_Hospital.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Hope_National_Hospital.Infrastructure.Services
{
    public class AdmissionService : IAdmissionService
    {
        private readonly AppDbContext _context;
        private readonly IAuditService _auditService;
        private readonly IMemoryCache _cache;

        // =========================================================
        // CACHE KEYS
        // =========================================================

        private const string AllAdmissionsCacheKey = "admissions_all";

        private static string AdmissionByIdCacheKey(int id)
            => $"admission_{id}";

        private const string AllBedsCacheKey = "beds_all";
        private const string AvailableBedsCacheKey = "beds_available";

        private static string BedByIdCacheKey(int id)
            => $"bed_{id}";

        private const string AllRoomsCacheKey = "rooms_all";

        private static string RoomByIdCacheKey(int id)
            => $"room_{id}";

        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public AdmissionService(
            AppDbContext context,
            IAuditService auditService,
            IMemoryCache cache)
        {
            _context = context;
            _auditService = auditService;
            _cache = cache;
        }

        // =========================================================
        // GET ALL
        // =========================================================

        public async Task<IEnumerable<AdmissionDto>> GetAllAsync()
        {
            if (_cache.TryGetValue(
                    AllAdmissionsCacheKey,
                    out IEnumerable<AdmissionDto>? cachedAdmissions))
            {
                return cachedAdmissions!;
            }

            var admissions = await _context.Admissions
                .AsNoTracking()
                .OrderByDescending(x => x.AdmissionDate)
                .Select(x => new AdmissionDto
                {
                    Id = x.Id,

                    PatientId = x.PatientId,
                    PatientName = x.Patient != null
                        ? x.Patient.FullName
                        : string.Empty,

                    DoctorId = x.DoctorId,
                    DoctorName = x.Doctor != null
                        ? x.Doctor.Name
                        : string.Empty,

                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.Department != null
                        ? x.Department.Name
                        : string.Empty,

                    RoomId = x.RoomId,
                    RoomNumber = x.Room != null
                        ? x.Room.RoomNumber
                        : string.Empty,

                    BedId = x.BedId,
                    BedNumber = x.Bed != null
                        ? x.Bed.BedNumber
                        : string.Empty,

                    Reason = x.Reason,

                    AdmissionDate = x.AdmissionDate,

                    DischargeDate = x.DischargeDate,

                    Status = x.Status,

                    Notes = x.Notes,

                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(5),

                SlidingExpiration =
                    TimeSpan.FromMinutes(2)
            };

            _cache.Set(
                AllAdmissionsCacheKey,
                admissions,
                cacheOptions);

            return admissions;
        }

        // =========================================================
        // GET BY ID
        // =========================================================

        public async Task<AdmissionDto?> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            var cacheKey = AdmissionByIdCacheKey(id);

            if (_cache.TryGetValue(
                    cacheKey,
                    out AdmissionDto? cachedAdmission))
            {
                return cachedAdmission;
            }

            var admission = await _context.Admissions
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new AdmissionDto
                {
                    Id = x.Id,

                    PatientId = x.PatientId,
                    PatientName = x.Patient != null
                        ? x.Patient.FullName
                        : string.Empty,

                    DoctorId = x.DoctorId,
                    DoctorName = x.Doctor != null
                        ? x.Doctor.Name
                        : string.Empty,

                    DepartmentId = x.DepartmentId,
                    DepartmentName = x.Department != null
                        ? x.Department.Name
                        : string.Empty,

                    RoomId = x.RoomId,
                    RoomNumber = x.Room != null
                        ? x.Room.RoomNumber
                        : string.Empty,

                    BedId = x.BedId,
                    BedNumber = x.Bed != null
                        ? x.Bed.BedNumber
                        : string.Empty,

                    Reason = x.Reason,

                    AdmissionDate = x.AdmissionDate,

                    DischargeDate = x.DischargeDate,

                    Status = x.Status,

                    Notes = x.Notes,

                    CreatedAt = x.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (admission == null)
            {
                return null;
            }

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(5),

                SlidingExpiration =
                    TimeSpan.FromMinutes(2)
            };

            _cache.Set(
                cacheKey,
                admission,
                cacheOptions);

            return admission;
        }

        // =========================================================
        // CREATE ADMISSION
        // =========================================================

        public async Task<AdmissionDto> CreateAsync(
            CreateAdmissionDto dto)
        {
            // -----------------------------------------------------
            // Validate Patient
            // -----------------------------------------------------

            var patient = await _context.Patients
                .FirstOrDefaultAsync(x =>
                    x.Id == dto.PatientId &&
                    x.IsActive);

            if (patient == null)
            {
                throw new ArgumentException(
                    "Patient not found or inactive.");
            }

            // -----------------------------------------------------
            // Check Already Admitted
            // -----------------------------------------------------

            var alreadyAdmitted =
                await _context.Admissions.AnyAsync(x =>
                    x.PatientId == dto.PatientId &&
                    x.Status == "Active");

            if (alreadyAdmitted)
            {
                throw new InvalidOperationException(
                    "Patient is already admitted.");
            }

            // -----------------------------------------------------
            // Validate Doctor
            // -----------------------------------------------------

            var doctor = await _context.Doctor
                .FirstOrDefaultAsync(x =>
                    x.Id == dto.DoctorId &&
                    x.IsActive);

            if (doctor == null)
            {
                throw new ArgumentException(
                    "Doctor not found or inactive.");
            }

            // -----------------------------------------------------
            // Validate Department
            // -----------------------------------------------------

            var department = await _context.Departments
                .FirstOrDefaultAsync(x =>
                    x.Id == dto.DepartmentId);

            if (department == null)
            {
                throw new ArgumentException(
                    "Department not found or inactive.");
            }

            // -----------------------------------------------------
            // Validate Room
            // -----------------------------------------------------

            var room = await _context.Rooms
                .FirstOrDefaultAsync(x =>
                    x.Id == dto.RoomId &&
                    x.IsActive);

            if (room == null)
            {
                throw new ArgumentException(
                    "Room not found or inactive.");
            }

            // -----------------------------------------------------
            // Room must belong to Department
            // -----------------------------------------------------

            if (room.DepartmentId != dto.DepartmentId)
            {
                throw new ArgumentException(
                    "Selected room does not belong to the selected department.");
            }

            // -----------------------------------------------------
            // Validate Bed
            // -----------------------------------------------------

            var bed = await _context.Beds
                .FirstOrDefaultAsync(x =>
                    x.Id == dto.BedId &&
                    x.IsActive);

            if (bed == null)
            {
                throw new ArgumentException(
                    "Bed not found or inactive.");
            }

            // -----------------------------------------------------
            // Bed must belong to Room
            // -----------------------------------------------------

            if (bed.RoomId != dto.RoomId)
            {
                throw new ArgumentException(
                    "Selected bed does not belong to the selected room.");
            }

            // -----------------------------------------------------
            // Bed must be Available
            // -----------------------------------------------------

            if (!string.Equals(
                    bed.Status,
                    "Available",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Selected bed is not available.");
            }

            // -----------------------------------------------------
            // Create Admission
            // -----------------------------------------------------

            var admission = new Admission
            {
                PatientId = dto.PatientId,

                DoctorId = dto.DoctorId,

                DepartmentId = dto.DepartmentId,

                RoomId = dto.RoomId,

                BedId = dto.BedId,

                Reason = dto.Reason?.Trim(),

                AdmissionDate =
                    dto.AdmissionDate?.ToUniversalTime()
                    ?? DateTime.UtcNow,

                Status = "Active",

                Notes = dto.Notes?.Trim(),

                CreatedAt = DateTime.UtcNow
            };

            // -----------------------------------------------------
            // Occupy Bed
            // -----------------------------------------------------

            bed.Status = "Occupied";

            _context.Admissions.Add(admission);

            await _context.SaveChangesAsync();

            // -----------------------------------------------------
            // Clear Admission Cache
            // -----------------------------------------------------

            ClearAdmissionCache();

            // -----------------------------------------------------
            // Clear Bed Cache
            // -----------------------------------------------------

            ClearBedCache(bed.Id);

            // -----------------------------------------------------
            // Clear Room Cache
            // -----------------------------------------------------

            ClearRoomCache(room.Id);

            // -----------------------------------------------------
            // Audit
            // -----------------------------------------------------

            await _auditService.LogAsync(
                "CREATE",
                "Admission",
                admission.Id.ToString(),
                $"Patient '{patient.FullName}' admitted to Room '{room.RoomNumber}', Bed '{bed.BedNumber}'."
            );

            return (await GetByIdAsync(admission.Id))!;
        }

        // =========================================================
        // UPDATE
        // =========================================================

        public async Task<bool> UpdateAsync(
            int id,
            UpdateAdmissionDto dto)
        {
            var admission =
                await _context.Admissions
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (admission == null)
            {
                return false;
            }

            // -----------------------------------------------------
            // Only Active Admission can be updated
            // -----------------------------------------------------

            if (!string.Equals(
                    admission.Status,
                    "Active",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Only active admissions can be updated.");
            }

            // -----------------------------------------------------
            // Validate Doctor
            // -----------------------------------------------------

            var doctor = await _context.Doctor
                .FirstOrDefaultAsync(x =>
                    x.Id == dto.DoctorId &&
                    x.IsActive);

            if (doctor == null)
            {
                throw new ArgumentException(
                    "Doctor not found or inactive.");
            }

            // -----------------------------------------------------
            // Validate Department
            // -----------------------------------------------------

            var department = await _context.Departments
                .FirstOrDefaultAsync(x =>
                    x.Id == dto.DepartmentId);

            if (department == null)
            {
                throw new ArgumentException(
                    "Department not found or inactive.");
            }

            // -----------------------------------------------------
            // Update
            // -----------------------------------------------------

            admission.DoctorId = dto.DoctorId;

            admission.DepartmentId = dto.DepartmentId;

            admission.Reason = dto.Reason?.Trim();

            admission.Notes = dto.Notes?.Trim();

            await _context.SaveChangesAsync();

            // -----------------------------------------------------
            // Clear Admission Cache
            // -----------------------------------------------------

            ClearAdmissionCache(id);

            // -----------------------------------------------------
            // Clear Department Cache
            // -----------------------------------------------------

            ClearDepartmentCache();

            // -----------------------------------------------------
            // Audit
            // -----------------------------------------------------

            await _auditService.LogAsync(
                "UPDATE",
                "Admission",
                admission.Id.ToString(),
                $"Admission '{admission.Id}' updated."
            );

            return true;
        }

        // =========================================================
        // DISCHARGE
        // =========================================================

        public async Task<bool> DischargeAsync(int id)
        {
            var admission =
                await _context.Admissions
                    .Include(x => x.Patient)
                    .Include(x => x.Bed)
                    .Include(x => x.Room)
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (admission == null)
            {
                return false;
            }

            // -----------------------------------------------------
            // Check Active Admission
            // -----------------------------------------------------

            if (!string.Equals(
                    admission.Status,
                    "Active",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Patient is already discharged.");
            }

            // -----------------------------------------------------
            // Update Admission
            // -----------------------------------------------------

            admission.Status = "Discharged";

            admission.DischargeDate =
                DateTime.UtcNow;

            // -----------------------------------------------------
            // Free Bed
            // -----------------------------------------------------

            if (admission.Bed != null)
            {
                admission.Bed.Status = "Available";
            }

            await _context.SaveChangesAsync();

            // -----------------------------------------------------
            // Clear Admission Cache
            // -----------------------------------------------------

            ClearAdmissionCache(id);

            // -----------------------------------------------------
            // Clear Bed Cache
            // -----------------------------------------------------

            
                ClearBedCache(admission.BedId);
           
            // -----------------------------------------------------
            // Clear Room Cache
            // -----------------------------------------------------

            
            
                ClearRoomCache(admission.RoomId);
            

            // -----------------------------------------------------
            // Audit
            // -----------------------------------------------------

            await _auditService.LogAsync(
                "DISCHARGE",
                "Admission",
                admission.Id.ToString(),
                $"Patient '{admission.Patient?.FullName}' discharged from admission '{admission.Id}'."
            );

            return true;
        }

        // =========================================================
        // CACHE HELPERS
        // =========================================================

        private void ClearAdmissionCache(int? admissionId = null)
        {
            // Clear all admissions list
            _cache.Remove(AllAdmissionsCacheKey);

            // Clear specific admission
            if (admissionId.HasValue)
            {
                _cache.Remove(
                    AdmissionByIdCacheKey(admissionId.Value));
            }
        }

        // ---------------------------------------------------------
        // BED CACHE
        // ---------------------------------------------------------

        private void ClearBedCache(int bedId)
        {
            _cache.Remove(AllBedsCacheKey);

            _cache.Remove(AvailableBedsCacheKey);

            _cache.Remove(BedByIdCacheKey(bedId));
        }

        // ---------------------------------------------------------
        // ROOM CACHE
        // ---------------------------------------------------------

        private void ClearRoomCache(int roomId)
        {
            _cache.Remove(AllRoomsCacheKey);

            _cache.Remove(RoomByIdCacheKey(roomId));
        }

        // ---------------------------------------------------------
        // DEPARTMENT CACHE
        // ---------------------------------------------------------

        private void ClearDepartmentCache()
        {
            _cache.Remove("departments");
        }
    }
}