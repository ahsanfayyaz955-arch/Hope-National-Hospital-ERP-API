using Hope_National_Hospital.Application.Constants;
using Hope_National_Hospital.Application.Dtos.Department;
using Hope_National_Hospital.Application.Interfaces;
using Hope_National_Hospital.Domain.Entities;
using Hope_National_Hospital.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Hope_National_Hospital.Infrastructure.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly AppDbContext _context;
        private readonly IAuditService _auditService;
        private readonly IMemoryCache _cache;


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public DepartmentService(
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

        public async Task<DepartmentResponseDto> CreateAsync(
            DepartmentCreateDto dto)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException(
                    "Department name is required.");
            }

            var departmentName = dto.Name.Trim();


            // Duplicate check
            var exists = await _context.Departments
                .AnyAsync(d =>
                    d.Name.ToLower() ==
                    departmentName.ToLower());

            if (exists)
            {
                throw new ArgumentException(
                    "Department already exists.");
            }


            // Create department
            var department = new Department
            {
                Name = departmentName,
                Description = dto.Description?.Trim()
            };


            _context.Departments.Add(department);

            await _context.SaveChangesAsync();


            // =====================================================
            // CACHE INVALIDATION
            // =====================================================

            _cache.Remove(CacheKeys.Departments);


            // Audit
            await _auditService.LogAsync(
                "CREATE",
                "Department",
                department.Id.ToString(),
                $"Department '{department.Name}' was created."
            );


            // Response
            return new DepartmentResponseDto
            {
                Id = department.Id,

                Name = department.Name,

                Description = department.Description,

                DoctorCount = 0,

                ReceptionistCount = 0
            };
        }


        // =========================================================
        // GET ALL
        // =========================================================

        public async Task<IEnumerable<DepartmentResponseDto>>
            GetAllAsync()
        {
            // =====================================================
            // CHECK CACHE
            // =====================================================

            if (_cache.TryGetValue(
                CacheKeys.Departments,
                out List<DepartmentResponseDto>? cachedDepartments))
            {
                return cachedDepartments!;
            }


            // =====================================================
            // DATABASE
            // =====================================================

            var departments = await _context.Departments
                .AsNoTracking()
                .Select(d => new DepartmentResponseDto
                {
                    Id = d.Id,

                    Name = d.Name,

                    Description = d.Description,

                    DoctorCount =
                        d.Doctors.Count(),

                    ReceptionistCount =
                        d.receptionists.Count()
                })
                .ToListAsync();


            // =====================================================
            // SAVE TO CACHE
            // =====================================================

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(10),

                SlidingExpiration =
                    TimeSpan.FromMinutes(5)
            };


            _cache.Set(
                CacheKeys.Departments,
                departments,
                cacheOptions);


            return departments;
        }


        // =========================================================
        // GET BY ID
        // =========================================================

        public async Task<DepartmentResponseDto?>
            GetByIdAsync(int id)
        {
            // =====================================================
            // CHECK CACHE
            // =====================================================

            var cacheKey = CacheKeys.Department(id);

            if (_cache.TryGetValue(
                cacheKey,
                out DepartmentResponseDto? cachedDepartment))
            {
                return cachedDepartment;
            }


            // =====================================================
            // DATABASE
            // =====================================================

            var department = await _context.Departments
                .AsNoTracking()
                .Where(d => d.Id == id)
                .Select(d => new DepartmentResponseDto
                {
                    Id = d.Id,

                    Name = d.Name,

                    Description = d.Description,

                    DoctorCount =
                        d.Doctors.Count(),

                    ReceptionistCount =
                        d.receptionists.Count()
                })
                .FirstOrDefaultAsync();


            if (department == null)
            {
                return null;
            }


            // =====================================================
            // SAVE TO CACHE
            // =====================================================

            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    TimeSpan.FromMinutes(10),

                SlidingExpiration =
                    TimeSpan.FromMinutes(5)
            };


            _cache.Set(
                cacheKey,
                department,
                cacheOptions);


            return department;
        }


        // =========================================================
        // UPDATE
        // =========================================================

        public async Task<bool> UpdateAsync(
            int id,
            DepartmentUpdateDto dto)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ArgumentException(
                    "Department name is required.");
            }


            // Find department
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == id);


            if (department == null)
            {
                return false;
            }


            var departmentName = dto.Name.Trim();


            // Duplicate name check
            var duplicate = await _context.Departments
                .AnyAsync(d =>
                    d.Id != id &&
                    d.Name.ToLower() ==
                    departmentName.ToLower());


            if (duplicate)
            {
                throw new ArgumentException(
                    "Another department with this name already exists.");
            }


            // Update
            department.Name =
                departmentName;

            department.Description =
                dto.Description?.Trim();


            await _context.SaveChangesAsync();


            // =====================================================
            // CACHE INVALIDATION
            // =====================================================

            _cache.Remove(CacheKeys.Departments);

            _cache.Remove(
                CacheKeys.Department(id));


            // Audit
            await _auditService.LogAsync(
                "UPDATE",
                "Department",
                department.Id.ToString(),
                $"Department '{department.Name}' was updated."
            );


            return true;
        }


        // =========================================================
        // DELETE
        // =========================================================

        public async Task<bool> DeleteAsync(int id)
        {
            var department =
                await _context.Departments
                    .FirstOrDefaultAsync(d =>
                        d.Id == id);


            if (department == null)
            {
                return false;
            }


            // =====================================================
            // CHECK ASSIGNED DOCTORS
            // =====================================================

            var hasDoctor =
                await _context.Doctor
                    .AnyAsync(d =>
                        d.DepartmentId == id);


            // =====================================================
            // CHECK ASSIGNED RECEPTIONISTS
            // =====================================================

            var hasReceptionist =
                await _context.Receptionist
                    .AnyAsync(r =>
                        r.DepartmentId == id);


            if (hasDoctor || hasReceptionist)
            {
                throw new InvalidOperationException(
                    "Department cannot be deleted because doctors or receptionists are assigned to it.");
            }


            // Delete
            _context.Departments.Remove(department);

            await _context.SaveChangesAsync();


            // =====================================================
            // CACHE INVALIDATION
            // =====================================================

            _cache.Remove(CacheKeys.Departments);

            _cache.Remove(
                CacheKeys.Department(id));


            // Audit
            await _auditService.LogAsync(
                "DELETE",
                "Department",
                department.Id.ToString(),
                $"Department '{department.Name}' was deleted."
            );


            return true;
        }
    }
}