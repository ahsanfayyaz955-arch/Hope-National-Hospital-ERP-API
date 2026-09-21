using Asp.Versioning;
using Hope_National_Hospital.Application.Dtos.Admission;
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
    public class AdmissionController : ControllerBase
    {
        private readonly IAdmissionService _admissionService;

        public AdmissionController(
            IAdmissionService admissionService)
        {
            _admissionService = admissionService;
        }

        // =========================================================
        // GET: api/v1/Admission
        // =========================================================

        [HttpGet]
        [Authorize(Policy = "AllMedicalStaff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _admissionService.GetAllAsync();

            return Ok(result);
        }

        // =========================================================
        // GET: api/v1/Admission/1
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
                    message = "Invalid admission ID."
                });
            }

            var result =
                await _admissionService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Admission not found."
                });
            }

            return Ok(result);
        }

        // =========================================================
        // POST: api/v1/Admission
        // =========================================================

        [HttpPost]
        [Authorize(Policy = "ReceptionistOrAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create(
            [FromBody] CreateAdmissionDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    message = "Admission data is required."
                });
            }

            var result =
                await _admissionService.CreateAsync(dto);

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
        // PUT: api/v1/Admission/1
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
            [FromBody] UpdateAdmissionDto dto)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid admission ID."
                });
            }

            if (dto == null)
            {
                return BadRequest(new
                {
                    message = "Admission data is required."
                });
            }

            var result =
                await _admissionService.UpdateAsync(
                    id,
                    dto);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Admission not found."
                });
            }

            return Ok(new
            {
                message = "Admission updated successfully."
            });
        }

        // =========================================================
        // POST: api/v1/Admission/1/discharge
        // =========================================================

        [HttpPost("{id:int}/discharge")]
        [Authorize(Policy = "ReceptionistOrAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Discharge(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid admission ID."
                });
            }

            var result =
                await _admissionService.DischargeAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Admission not found."
                });
            }

            return Ok(new
            {
                message =
                    "Patient discharged successfully and bed is now available."
            });
        }
    }
}