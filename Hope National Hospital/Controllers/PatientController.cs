
using Asp.Versioning;
using Hope_National_Hospital.Application.Constants;
using Hope_National_Hospital.Application.Dtos.Patient;
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
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        // =========================================================
        // GET: api/Patient
        // =========================================================
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _patientService.GetAllAsync();

            return Ok(result);
        }

        // =========================================================
        // GET: api/Patient/1
        // =========================================================
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid patient ID."
                });
            }

            var result = await _patientService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Patient not found."
                });
            }

            return Ok(result);
        }

        // =========================================================
        // POST: api/Patient
        // =========================================================
        [HttpPost]
        [Authorize(Policy = "ReceptionistOrAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create(
            [FromBody] CreatePatientDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    message = "Patient data is required."
                });
            }

            var result = await _patientService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        // =========================================================
        // PUT: api/Patient/1
        // =========================================================
        [HttpPut("{id:int}")]
        [Authorize(Policy = "ReceptionistOrAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdatePatientDto dto)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid patient ID."
                });
            }

            if (dto == null)
            {
                return BadRequest(new
                {
                    message = "Patient data is required."
                });
            }

            var result =
                await _patientService.UpdateAsync(id, dto);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Patient not found."
                });
            }

            return Ok(new
            {
                message = "Patient updated successfully."
            });
        }

        // =========================================================
        // DELETE: api/Patient/1
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
                    message = "Invalid patient ID."
                });
            }

            var result =
                await _patientService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Patient not found."
                });
            }

            return Ok(new
            {
                message = "Patient deleted successfully."
            });
        }
    }
}

