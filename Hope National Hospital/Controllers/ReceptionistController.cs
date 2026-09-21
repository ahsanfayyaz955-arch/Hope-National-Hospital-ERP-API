
using Asp.Versioning;
using Hope_National_Hospital.Application.Constants;
using Hope_National_Hospital.Application.Dtos.Receptionist;
using Hope_National_Hospital.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Hope_National_Hospital.Controllers
{

    [ApiController]
    [Authorize]
    [EnableRateLimiting("ApiPolicy")]
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ReceptionistController : ControllerBase
    {
        private readonly IReceptionistService _receptionistService;

        public ReceptionistController(
            IReceptionistService receptionistService)
        {
            _receptionistService = receptionistService;
        }

        // =========================================================
        // GET: api/Receptionist
        // =========================================================
        [HttpGet]
        [Authorize(Policy = "AdminOrSuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _receptionistService.GetAllAsync();

            return Ok(result);
        }

        // =========================================================
        // GET: api/Receptionist/1
        // =========================================================
        [HttpGet("{id:int}")]
        [Authorize(Policy = "AdminOrSuperAdmin")]
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
                    message = "Invalid receptionist ID."
                });
            }

            var result =
                await _receptionistService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Receptionist not found."
                });
            }

            return Ok(result);
        }

        // =========================================================
        // POST: api/Receptionist
        // =========================================================
        [HttpPost]
        [Authorize(Policy = "AdminOrSuperAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create(
            [FromBody] CreateReceptionistDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    message = "Receptionist data is required."
                });
            }

            var result =
                await _receptionistService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        // =========================================================
        // PUT: api/Receptionist/1
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
            [FromBody] UpdateReceptionistDto dto)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid receptionist ID."
                });
            }

            if (dto == null)
            {
                return BadRequest(new
                {
                    message = "Receptionist data is required."
                });
            }

            var result =
                await _receptionistService.UpdateAsync(id, dto);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Receptionist not found."
                });
            }

            return Ok(new
            {
                message = "Receptionist updated successfully."
            });
        }

        // =========================================================
        // DELETE: api/Receptionist/1
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
                    message = "Invalid receptionist ID."
                });
            }

            var result =
                await _receptionistService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Receptionist not found."
                });
            }

            return Ok(new
            {
                message = "Receptionist deleted successfully."
            });
        }
    }
}

