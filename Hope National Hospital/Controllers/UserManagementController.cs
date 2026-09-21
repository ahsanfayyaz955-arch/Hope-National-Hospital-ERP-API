
using Asp.Versioning;
using Hope_National_Hospital.Application.Dtos.UserManagement;
using Hope_National_Hospital.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.VisualBasic;

namespace Hope_National_Hospital.Controllers
{
    [ApiController]
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Policy = "SuperAdminOnly")]
    [EnableRateLimiting("ApiPolicy")]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;

        public UserManagementController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        // GetAllUser

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userManagementService.GetAllUserAsync();
            return Ok(users);
        }

        // Get By Id 

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult>GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new
                {
                    message = "user Id is required "
                });
            }

            var user = await _userManagementService.GetUserByIdAsync(id);
            if(user == null)
            {
                return NotFound(new
                {
                    message = "user not found"
                });
            }

            return Ok(user);
        }

        // create user 
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            if(dto == null)
            {
                return BadRequest(new
                {
                    message = "user data is required"
                });
            }

            var user = await _userManagementService.CreateUserAsync(dto);

            return CreatedAtAction(nameof(GetById),new
            {
                id = user.Id,
                version = "1.0"
            },user);
                 
                
        }

        // update uer 
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult>Update(string id, [FromBody] UpdateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new
                {
                    message = "User Ide Is Required."
                });
            }

            if(dto== null)
            {
                return BadRequest(new
                {
                    message = "User Data Is Required"
                });
            }

            var user = await _userManagementService.UpdateUserAsync(id, dto);

            return Ok(new
            {
                message = " User Roles Changes Succefully"
            });
        }

        // Activate or  Deactivate users 
        [HttpPatch("{id}/toggle-status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        public async Task<IActionResult>ToggleStatus(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new
                {
                    message = " User Id Is Required."
                });    
                
            }

            var isActive = await _userManagementService.ToggleStatusAsync(id);
            return Ok(new
            {
                message = isActive ? "User Activated Successfully." :
                "User Deactivated Successfully.",
                isActive
            });
        }




    }
}
