using Hope_National_Hospital.Application.Dtos.UserManagement;
using Hope_National_Hospital.Application.Interfaces;
using Hope_National_Hospital.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Hope_National_Hospital.Infrastructure.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditService _auditService;

        public UserManagementService(
            UserManager<ApplicationUser> userManager,
            IAuditService auditService)
        {
            _userManager = userManager;
            _auditService = auditService;
        }

        // =========================================================
        // GET ALL USERS
        // =========================================================
        public async Task<List<UserDto>> GetAllUserAsync()
        {
            var users = _userManager.Users.ToList();

            var result = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    IsActive = user.IsActive,
                    Role = roles
                });
            }

            return result;
        }


        // =========================================================
        // GET USER BY ID
        // =========================================================
        public async Task<UserDto?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                IsActive = user.IsActive,
                Role = roles
            };
        }


        // =========================================================
        // CREATE USER
        // =========================================================
        public async Task<UserDto> CreateUserAsync(
            CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                throw new ArgumentException(
                    "Full name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ArgumentException(
                    "Email is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new ArgumentException(
                    "Password is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Role))
            {
                throw new ArgumentException(
                    "Role is required.");
            }


         

            var validRoles = new[]
            {
                "Admin",
                "Doctor",
                "Receptionist",
                "Patient"
            };

            if (!validRoles.Contains(
                dto.Role,
                StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Invalid role.");
            }


            // Check existing email
            var existingUser =
                await _userManager.FindByEmailAsync(
                    dto.Email.Trim());

            if (existingUser != null)
            {
                throw new ArgumentException(
                    "Email already exists.");
            }


            var user = new ApplicationUser
            {
                FullName = dto.FullName.Trim(),
                Email = dto.Email.Trim(),
                UserName = dto.Email.Trim(),
                EmailConfirmed = true,
                IsActive = true
            };


            var result =
                await _userManager.CreateAsync(
                    user,
                    dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(
                        x => x.Description));

                throw new ArgumentException(errors);
            }


            // Assign role
            var roleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    dto.Role);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(
                        x => x.Description));

                throw new InvalidOperationException(
                    $"Unable to assign role. {errors}");
            }


            await _auditService.LogAsync(
                "CREATE_USER",
                "User",
                user.Id,
                $"User '{user.Email}' created with role '{dto.Role}'."
            );


            return await GetUserByIdAsync(user.Id)
                   ?? throw new InvalidOperationException(
                       "User could not be retrieved after creation.");
        }


        // =========================================================
        // UPDATE USER
        // =========================================================
        public async Task<UserDto> UpdateUserAsync(
            string id,
            UpdateUserDto dto)
        {
            var user =
                await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException(
                    "User not found.");
            }

            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                throw new ArgumentException(
                    "Full name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                throw new ArgumentException(
                    "Email is required.");
            }


            var existingUser =
                await _userManager.FindByEmailAsync(
                    dto.Email.Trim());

            if (existingUser != null &&
                existingUser.Id != user.Id)
            {
                throw new ArgumentException(
                    "Email already exists.");
            }


            user.FullName = dto.FullName.Trim();
            user.Email = dto.Email.Trim();
            user.UserName = dto.Email.Trim();


            var result =
                await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(
                        x => x.Description));

                throw new ArgumentException(errors);
            }


            await _auditService.LogAsync(
                "UPDATE_USER",
                "User",
                user.Id,
                $"User '{user.Email}' updated."
            );


            return await GetUserByIdAsync(user.Id)
                   ?? throw new InvalidOperationException(
                       "User could not be retrieved after update.");
        }


        // =========================================================
        // CHANGE ROLE
        // =========================================================
        public async Task<bool> ChangeRoleAsync(
            string id,
            ChangeRoleDto dto)
        {
            var user =
                await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException(
                    "User not found.");
            }


            var validRoles = new[]
            {
                "Admin",
                "Doctor",
                "Receptionist",
                "Patient"
            };

            if (!validRoles.Contains(
                dto.Role,
                StringComparer.OrdinalIgnoreCase))
            {
                throw new ArgumentException(
                    "Invalid role.");
            }


            var currentRoles =
                await _userManager.GetRolesAsync(user);


            if (currentRoles.Any())
            {
                var removeResult =
                    await _userManager.RemoveFromRolesAsync(
                        user,
                        currentRoles);

                if (!removeResult.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        removeResult.Errors.Select(
                            x => x.Description));

                    throw new InvalidOperationException(errors);
                }
            }


            var addResult =
                await _userManager.AddToRoleAsync(
                    user,
                    dto.Role);

            if (!addResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    addResult.Errors.Select(
                        x => x.Description));

                throw new InvalidOperationException(errors);
            }


            await _auditService.LogAsync(
                "CHANGE_ROLE",
                "User",
                user.Id,
                $"User '{user.Email}' role changed to '{dto.Role}'."
            );

            return true;
        }


        // =========================================================
        // TOGGLE ACTIVE / INACTIVE
        // =========================================================
        public async Task<bool> ToggleStatusAsync(
            string id)
        {
            var user =
                await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                throw new KeyNotFoundException(
                    "User not found.");
            }


            user.IsActive = !user.IsActive;


            var result =
                await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(
                        x => x.Description));

                throw new InvalidOperationException(errors);
            }


            await _auditService.LogAsync(
                "TOGGLE_USER_STATUS",
                "User",
                user.Id,
                $"User '{user.Email}' status changed to '{user.IsActive}'."
            );


            return user.IsActive;
        }
    }
}