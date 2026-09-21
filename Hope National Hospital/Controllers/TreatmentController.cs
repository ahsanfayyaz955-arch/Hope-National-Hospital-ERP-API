
using Asp.Versioning;
using Hope_National_Hospital.Application.Constants;
using Hope_National_Hospital.Application.Dtos.Treatment;
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
    public class TreatmentController : ControllerBase
    {
        private readonly ITreatmentService _treatmentService;

        public TreatmentController(
            ITreatmentService treatmentService)
        {
            _treatmentService = treatmentService;
        }

        // =========================================================
        // GET: api/Treatment
        // =========================================================
        [HttpGet]
        [Authorize(Policy = "AllMedicalStaff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _treatmentService.GetAllAsync();

            return Ok(result);
        }

        // =========================================================
        // GET: api/Treatment/1
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
                    message = "Invalid treatment ID."
                });
            }

            var result =
                await _treatmentService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Treatment not found."
                });
            }

            return Ok(result);
        }

        // =========================================================
        // POST: api/Treatment
        // =========================================================
        [HttpPost]
        [Authorize(Policy = "DoctorOrAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create(
            [FromBody] CreateTreatmentDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    message = "Treatment data is required."
                });
            }

            var result =
                await _treatmentService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        // =========================================================
        // PUT: api/Treatment/1
        // =========================================================
        [HttpPut("{id:int}")]
        [Authorize(Policy = "DoctorOrAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateTreatmentDto dto)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid treatment ID."
                });
            }

            if (dto == null)
            {
                return BadRequest(new
                {
                    message = "Treatment data is required."
                });
            }

            var result =
                await _treatmentService.UpdateAsync(id, dto);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Treatment not found."
                });
            }

            return Ok(new
            {
                message = "Treatment updated successfully."
            });
        }

        // =========================================================
        // DELETE: api/Treatment/1
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
                    message = "Invalid treatment ID."
                });
            }

            var result =
                await _treatmentService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Treatment not found."
                });
            }

            return Ok(new
            {
                message = "Treatment deleted successfully."
            });
        }
    }
}

