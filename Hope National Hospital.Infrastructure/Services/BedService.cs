using Hope_National_Hospital.Application.Dtos.Bed;
using Hope_National_Hospital.Application.Interfaces;
using Hope_National_Hospital.Domain.Entities;
using Hope_National_Hospital.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Hope_National_Hospital.Infrastructure.Services
{
    public class BedService : IBedService
    {
        private readonly AppDbContext _context;
        private readonly IAuditService _auditService;
        private readonly IMemoryCache _cache;

        private const string AllBedsCacheKey = "beds_all";
        private const string AvailableBedsCacheKey = "beds_available";
        private const string BedByIdCachePrefix = "bed_";

        private const string AllRoomsCacheKey = "rooms_all";
        private const string RoomByIdCachePrefix = "room_";

        private static readonly string[] AllowedStatuses =
        {
            "Available",
            "Occupied",
            "Reserved",
            "Maintenance"
        };

        public BedService(
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
        public async Task<IEnumerable<BedDto>> GetAllAsync()
        {
            if (_cache.TryGetValue(
                AllBedsCacheKey,
                out IEnumerable<BedDto>? cachedBeds))
            {
                return cachedBeds!;
            }

            var beds = await _context.Beds
                .AsNoTracking()
                .Select(b => new BedDto
                {
                    Id = b.Id,
                    BedNumber = b.BedNumber,
                    RoomId = b.RoomId,
                    RoomNumber = b.Room != null
                        ? b.Room.RoomNumber
                        : null,
                    Status = b.Status,
                    IsActive = b.IsActive
                })
                .ToListAsync();

            _cache.Set(
                AllBedsCacheKey,
                beds,
                TimeSpan.FromMinutes(10));

            return beds;
        }

        // =========================================================
        // GET BY ID
        // =========================================================
        public async Task<BedDto?> GetByIdAsync(int id)
        {
            var cacheKey =
                $"{BedByIdCachePrefix}{id}";

            if (_cache.TryGetValue(
                cacheKey,
                out BedDto? cachedBed))
            {
                return cachedBed;
            }

            var bed = await _context.Beds
                .AsNoTracking()
                .Where(b => b.Id == id)
                .Select(b => new BedDto
                {
                    Id = b.Id,
                    BedNumber = b.BedNumber,
                    RoomId = b.RoomId,
                    RoomNumber = b.Room != null
                        ? b.Room.RoomNumber
                        : null,
                    Status = b.Status,
                    IsActive = b.IsActive
                })
                .FirstOrDefaultAsync();

            if (bed != null)
            {
                _cache.Set(
                    cacheKey,
                    bed,
                    TimeSpan.FromMinutes(10));
            }

            return bed;
        }

        // =========================================================
        // GET AVAILABLE
        // =========================================================
        public async Task<IEnumerable<BedDto>> GetAvailableAsync()
        {
            if (_cache.TryGetValue(
                AvailableBedsCacheKey,
                out IEnumerable<BedDto>? cachedBeds))
            {
                return cachedBeds!;
            }

            var beds = await _context.Beds
                .AsNoTracking()
                .Where(b =>
                    b.IsActive &&
                    b.Status == "Available" &&
                    b.Room != null &&
                    b.Room.IsActive)
                .Select(b => new BedDto
                {
                    Id = b.Id,
                    BedNumber = b.BedNumber,
                    RoomId = b.RoomId,
                    RoomNumber = b.Room!.RoomNumber,
                    Status = b.Status,
                    IsActive = b.IsActive
                })
                .ToListAsync();

            _cache.Set(
                AvailableBedsCacheKey,
                beds,
                TimeSpan.FromMinutes(5));

            return beds;
        }

        // =========================================================
        // CREATE
        // =========================================================
        public async Task<BedDto> CreateAsync(
            CreateBedDto dto)
        {
            if (dto.RoomId <= 0)
            {
                throw new ArgumentException(
                    "Valid room is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.BedNumber))
            {
                throw new ArgumentException(
                    "Bed number is required.");
            }

            var room = await _context.Rooms
                .FirstOrDefaultAsync(r =>
                    r.Id == dto.RoomId);

            if (room == null)
            {
                throw new ArgumentException(
                    "Room not found.");
            }

            if (!room.IsActive)
            {
                throw new ArgumentException(
                    "Cannot add a bed to an inactive room.");
            }

            var bedNumber = dto.BedNumber.Trim();

            var exists = await _context.Beds
                .AnyAsync(b =>
                    b.RoomId == dto.RoomId &&
                    b.BedNumber == bedNumber);

            if (exists)
            {
                throw new ArgumentException(
                    "Bed number already exists in this room.");
            }

            var bed = new Bed
            {
                BedNumber = bedNumber,
                RoomId = dto.RoomId,
                Status = "Available",
                IsActive = true
            };

            _context.Beds.Add(bed);

            await _context.SaveChangesAsync();

            ClearBedCache(bed.Id);
            ClearRoomCache(room.Id);

            await _auditService.LogAsync(
                "CREATE",
                "Bed",
                bed.Id.ToString(),
                $"Bed '{bed.BedNumber}' created in room '{room.RoomNumber}'."
            );

            return await GetByIdAsync(bed.Id)
                ?? throw new Exception(
                    "Unable to retrieve created bed.");
        }

        // =========================================================
        // UPDATE
        // =========================================================
        public async Task<bool> UpdateAsync(
            int id,
            UpdateBedDto dto)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Invalid bed ID.");
            }

            if (dto.RoomId <= 0)
            {
                throw new ArgumentException(
                    "Valid room is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.BedNumber))
            {
                throw new ArgumentException(
                    "Bed number is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Status))
            {
                throw new ArgumentException(
                    "Bed status is required.");
            }

            var bed = await _context.Beds
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bed == null)
            {
                return false;
            }

            var oldRoomId = bed.RoomId;

            var room = await _context.Rooms
                .FirstOrDefaultAsync(r =>
                    r.Id == dto.RoomId);

            if (room == null)
            {
                throw new ArgumentException(
                    "Room not found.");
            }

            if (!room.IsActive)
            {
                throw new ArgumentException(
                    "Cannot assign bed to an inactive room.");
            }

            if (!AllowedStatuses.Contains(
                dto.Status,
                StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Invalid bed status.");
            }

            var normalizedStatus =
                AllowedStatuses.First(s =>
                    s.Equals(
                        dto.Status.Trim(),
                        StringComparison.OrdinalIgnoreCase));

            if (bed.Status == "Occupied" &&
                normalizedStatus != "Occupied")
            {
                throw new InvalidOperationException(
                    "Occupied bed can only be released through the discharge process.");
            }

            var bedNumber = dto.BedNumber.Trim();

            var duplicate = await _context.Beds
                .AnyAsync(b =>
                    b.Id != id &&
                    b.RoomId == dto.RoomId &&
                    b.BedNumber == bedNumber);

            if (duplicate)
            {
                throw new ArgumentException(
                    "Bed number already exists in this room.");
            }

            bed.BedNumber = bedNumber;
            bed.RoomId = dto.RoomId;
            bed.Status = normalizedStatus;
            bed.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            ClearBedCache(bed.Id);
            ClearRoomCache(oldRoomId);
            ClearRoomCache(room.Id);

            await _auditService.LogAsync(
                "UPDATE",
                "Bed",
                bed.Id.ToString(),
                $"Bed '{bed.BedNumber}' updated."
            );

            return true;
        }

        // =========================================================
        // DELETE / DEACTIVATE
        // =========================================================
        public async Task<bool> DeleteAsync(int id)
        {
            var bed = await _context.Beds
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bed == null)
            {
                return false;
            }

            if (bed.Status == "Occupied")
            {
                throw new InvalidOperationException(
                    "Cannot delete an occupied bed.");
            }

            bed.IsActive = false;

            await _context.SaveChangesAsync();

            ClearBedCache(bed.Id);
            ClearRoomCache(bed.RoomId);

            await _auditService.LogAsync(
                "DELETE",
                "Bed",
                bed.Id.ToString(),
                $"Bed '{bed.BedNumber}' deactivated."
            );

            return true;
        }

        // =========================================================
        // CACHE HELPERS
        // =========================================================
        private void ClearBedCache(int bedId)
        {
            _cache.Remove(AllBedsCacheKey);
            _cache.Remove(AvailableBedsCacheKey);
            _cache.Remove(
                $"{BedByIdCachePrefix}{bedId}");
        }

        private void ClearRoomCache(int roomId)
        {
            _cache.Remove(AllRoomsCacheKey);
            _cache.Remove(
                $"{RoomByIdCachePrefix}{roomId}");
        }
    }
}