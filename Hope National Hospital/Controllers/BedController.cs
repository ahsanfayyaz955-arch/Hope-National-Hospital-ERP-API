using Asp.Versioning;
using Hope_National_Hospital.Application.Dtos.Bed;
using Hope_National_Hospital.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Hope_National_Hospital.Controllers
{
    [ApiController]
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [EnableRateLimiting("ApiPolicy")]
    public class BedController : ControllerBase
    {
        private readonly IBedService _bedService;

        public BedController(IBedService bedService)
        {
            _bedService = bedService;
        }

        // =========================================================
        // GET: api/v1/Bed
        // =========================================================
        [HttpGet]
        [Authorize(Policy = "AllMedicalStaff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _bedService.GetAllAsync();

            return Ok(result);
        }

        // =========================================================
        // GET: api/v1/Bed/available
        // =========================================================
        [HttpGet("available")]
        [Authorize(Policy = "AllMedicalStaff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAvailable()
        {
            var result =
                await _bedService.GetAvailableAsync();

            return Ok(result);
        }

        // =========================================================
        // GET: api/v1/Bed/1
        // =========================================================
        [HttpGet("{id:int}")]
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
                    message = "Invalid bed ID."
                });
            }

            var result =
                await _bedService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Bed not found."
                });
            }

            return Ok(result);
        }

        // =========================================================
        // POST: api/v1/Bed
        // =========================================================
        [HttpPost]
        [Authorize(Policy = "AdminOrSuperAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create(
            [FromBody] CreateBedDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    message = "Bed data is required."
                });
            }

            var result =
                await _bedService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    id = result.Id,
                    version = "1.0"
                },
                result);
        }

        // =========================================================
        // PUT: api/v1/Bed/1
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
            [FromBody] UpdateBedDto dto)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid bed ID."
                });
            }

            if (dto == null)
            {
                return BadRequest(new
                {
                    message = "Bed data is required."
                });
            }

            var result =
                await _bedService.UpdateAsync(id, dto);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Bed not found."
                });
            }

            return Ok(new
            {
                message = "Bed updated successfully."
            });
        }

        // =========================================================
        // DELETE: api/v1/Bed/1
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
                    message = "Invalid bed ID."
                });
            }

            var result =
                await _bedService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Bed not found."
                });
            }

            return Ok(new
            {
                message = "Bed deleted successfully."
            });
        }
    }
}