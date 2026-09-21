using Hope_National_Hospital.Application.Dtos.Room;
using Hope_National_Hospital.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Asp.Versioning;

namespace Hope_National_Hospital.Controllers
{
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [EnableRateLimiting("ApiPolicy")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        // =========================================================
        // GET: api/v1/Room
        // =========================================================
        [HttpGet]
        [Authorize(Policy = "AllMedicalStaff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _roomService.GetAllAsync();

            return Ok(result);
        }

        // =========================================================
        // GET: api/v1/Room/1
        // =========================================================
        [HttpGet("{id:int}", Name = "GetRoomById")]
        [Authorize(Policy = "AllMedicalStaff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid room ID."
                });
            }

            var result = await _roomService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Room not found."
                });
            }

            return Ok(result);
        }

        // =========================================================
        // POST: api/v1/Room
        // =========================================================
        [HttpPost]
        [Authorize(Policy = "AdminOrSuperAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create(
          [FromBody] CreateRoomDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    message = "Room data is required."
                });
            }

            var result = await _roomService.CreateAsync(dto);

            return CreatedAtRoute(
                "GetRoomById",
                new
                {
                    id = result.Id,
                    version = "1.0"
                },
                result);
        }

        // =========================================================
        // PUT: api/v1/Room/1
        // =========================================================
        [HttpPut("{id:int}")]
        [Authorize(Policy = "AdminOrSuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateRoomDto dto)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid room ID."
                });
            }

            if (dto == null)
            {
                return BadRequest(new
                {
                    message = "Room data is required."
                });
            }

            var result =
                await _roomService.UpdateAsync(id, dto);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Room not found."
                });
            }

            return Ok(new
            {
                message = "Room updated successfully."
            });
        }

        // =========================================================
        // DELETE: api/v1/Room/1
        // =========================================================
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOrSuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid room ID."
                });
            }

            var result =
                await _roomService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Room not found."
                });
            }

            return Ok(new
            {
                message = "Room deleted successfully."
            });
        }
    }
}