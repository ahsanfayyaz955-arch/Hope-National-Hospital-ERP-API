using Hope_National_Hospital.Application.Dtos.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomDto>> GetAllAsync();

        Task<RoomDto?> GetByIdAsync(int id);

        Task<RoomDto> CreateAsync(CreateRoomDto dto);
        Task<bool> UpdateAsync(int id, UpdateRoomDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
