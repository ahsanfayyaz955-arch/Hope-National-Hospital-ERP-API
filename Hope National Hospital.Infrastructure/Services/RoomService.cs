using Hope_National_Hospital.Application.Dtos.Room;
using Hope_National_Hospital.Application.Interfaces;
using Hope_National_Hospital.Domain.Entities;
using Hope_National_Hospital.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Hope_National_Hospital.Infrastructure.Services
{
    public class RoomService : IRoomService
    {
        private readonly AppDbContext _context;
        private readonly IAuditService _auditService;
        private readonly IMemoryCache _cache;

        private const string AllRoomsCacheKey = "rooms_all";
        private const string RoomByIdCachePrefix = "room_";

        private const string AllBedsCacheKey = "beds_all";
        private const string AvailableBedsCacheKey = "beds_available";

        public RoomService(
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
        public async Task<IEnumerable<RoomDto>> GetAllAsync()
        {
            if (_cache.TryGetValue(
                AllRoomsCacheKey,
                out IEnumerable<RoomDto>? cachedRooms))
            {
                return cachedRooms!;
            }

            var rooms = await _context.Rooms
                .AsNoTracking()
                .Select(r => new RoomDto
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    RoomType = r.RoomType,
                    DepartmentId = r.DepartmentId,

                    DepartmentName =
                        r.Department != null
                            ? r.Department.Name
                            : null,

                    IsActive = r.IsActive,

                    TotalBeds =
                        r.Beds.Count,

                    AvailableBeds =
                        r.Beds.Count(b =>
                            b.Status == "Available" &&
                            b.IsActive)
                })
                .ToListAsync();

            _cache.Set(
                AllRoomsCacheKey,
                rooms,
                TimeSpan.FromMinutes(10));

            return rooms;
        }

        // =========================================================
        // GET BY ID
        // =========================================================
        public async Task<RoomDto?> GetByIdAsync(int id)
        {
            var cacheKey =
                $"{RoomByIdCachePrefix}{id}";

            if (_cache.TryGetValue(
                cacheKey,
                out RoomDto? cachedRoom))
            {
                return cachedRoom;
            }

            var room = await _context.Rooms
                .AsNoTracking()
                .Where(r => r.Id == id)
                .Select(r => new RoomDto
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    RoomType = r.RoomType,
                    DepartmentId = r.DepartmentId,

                    DepartmentName =
                        r.Department != null
                            ? r.Department.Name
                            : null,

                    IsActive = r.IsActive,

                    TotalBeds =
                        r.Beds.Count,

                    AvailableBeds =
                        r.Beds.Count(b =>
                            b.Status == "Available" &&
                            b.IsActive)
                })
                .FirstOrDefaultAsync();

            if (room != null)
            {
                _cache.Set(
                    cacheKey,
                    room,
                    TimeSpan.FromMinutes(10));
            }

            return room;
        }

        // =========================================================
        // CREATE
        // =========================================================
        public async Task<RoomDto> CreateAsync(
            CreateRoomDto dto)
        {
            if (string.IsNullOrWhiteSpace(
                dto.RoomNumber))
            {
                throw new ArgumentException(
                    "Room number is required.");
            }

            if (string.IsNullOrWhiteSpace(
                dto.RoomType))
            {
                throw new ArgumentException(
                    "Room type is required.");
            }

            if (dto.DepartmentId <= 0)
            {
                throw new ArgumentException(
                    "Valid department is required.");
            }

            var roomNumber =
                dto.RoomNumber.Trim();

            var exists = await _context.Rooms
                .AnyAsync(r =>
                    r.RoomNumber == roomNumber);

            if (exists)
            {
                throw new ArgumentException(
                    "Room number already exists.");
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(d =>
                    d.Id == dto.DepartmentId);

            if (department == null)
            {
                throw new ArgumentException(
                    "Department not found.");
            }

            var room = new Room
            {
                RoomNumber = roomNumber,
                RoomType = dto.RoomType.Trim(),
                DepartmentId = dto.DepartmentId,
                IsActive = true
            };

            _context.Rooms.Add(room);

            await _context.SaveChangesAsync();

            ClearRoomCache(room.Id);

            await _auditService.LogAsync(
                "CREATE",
              "Room",
              room.Id.ToString(),
             $"Room '{room.RoomNumber}' created."
             );

            return await GetByIdAsync(room.Id)
                ?? throw new Exception(
                    "Unable to retrieve created room.");
        }

        // =========================================================
        // UPDATE
        // =========================================================
        public async Task<bool> UpdateAsync(
            int id,
            UpdateRoomDto dto)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Invalid room ID.");
            }

            if (string.IsNullOrWhiteSpace(
                dto.RoomNumber))
            {
                throw new ArgumentException(
                    "Room number is required.");
            }

            if (string.IsNullOrWhiteSpace(
                dto.RoomType))
            {
                throw new ArgumentException(
                    "Room type is required.");
            }

            if (dto.DepartmentId <= 0)
            {
                throw new ArgumentException(
                    "Valid department is required.");
            }

            var room = await _context.Rooms
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return false;
            }

            var roomNumber =
                dto.RoomNumber.Trim();

            var duplicate = await _context.Rooms
                .AnyAsync(r =>
                    r.Id != id &&
                    r.RoomNumber == roomNumber);

            if (duplicate)
            {
                throw new ArgumentException(
                    "Room number already exists.");
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(d =>
                    d.Id == dto.DepartmentId);

            if (department == null)
            {
                throw new ArgumentException(
                    "Department not found.");
            }

            room.RoomNumber = roomNumber;
            room.RoomType = dto.RoomType.Trim();
            room.DepartmentId = dto.DepartmentId;
            room.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            ClearRoomCache(room.Id);

            await _auditService.LogAsync(
                "UPDATE",
                "Room",
                room.Id.ToString(),
                $"Room '{room.RoomNumber}' updated."
            );

            return true;
        }

        // =========================================================
        // DELETE / DEACTIVATE
        // =========================================================
        public async Task<bool> DeleteAsync(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Beds)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return false;
            }

            if (room.Beds.Any(b =>
                b.Status == "Occupied"))
            {
                throw new InvalidOperationException(
                    "Cannot delete a room containing occupied beds.");
            }

            room.IsActive = false;

            foreach (var bed in room.Beds)
            {
                bed.IsActive = false;
            }

            await _context.SaveChangesAsync();

            ClearRoomCache(room.Id);

            // Room deactivate hone se beds ki
            // available/all lists bhi change hongi
            _cache.Remove(AllBedsCacheKey);
            _cache.Remove(AvailableBedsCacheKey);

            foreach (var bed in room.Beds)
            {
                _cache.Remove($"bed_{bed.Id}");
            }

            await _auditService.LogAsync(
                "DELETE",
                "Room",
                room.Id.ToString(),
                $"Room '{room.RoomNumber}' deactivated."
            );

            return true;
        }

        // =========================================================
        // CACHE HELPER
        // =========================================================
        private void ClearRoomCache(int roomId)
        {
            _cache.Remove(AllRoomsCacheKey);
            _cache.Remove(
                $"{RoomByIdCachePrefix}{roomId}");
        }
    }
}