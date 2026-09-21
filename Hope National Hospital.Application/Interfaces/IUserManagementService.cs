using Hope_National_Hospital.Application.Dtos.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Interfaces
{
    public interface IUserManagementService
    {
        Task<List<UserDto>> GetAllUserAsync();
        Task<UserDto?> GetUserByIdAsync(string id);

        Task<UserDto> CreateUserAsync(CreateUserDto dto);

        Task<UserDto> UpdateUserAsync(string id, UpdateUserDto dto);

        Task<bool> ChangeRoleAsync(string id, ChangeRoleDto dto);

        Task<bool> ToggleStatusAsync(string id);
    }
}
