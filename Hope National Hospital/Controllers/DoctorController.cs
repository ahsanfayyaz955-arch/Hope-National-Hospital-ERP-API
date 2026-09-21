
using Asp.Versioning;
using Hope_National_Hospital.Application.Constants;
using Hope_National_Hospital.Application.Dtos.Doctor;
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
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(
            IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }


        // =========================================================
        // GET ALL DOCTORS
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _doctorService.GetAllAsync();

            return Ok(result);
        }


        // =========================================================
        // GET DOCTOR BY ID
        // =========================================================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid doctor ID."
                });
            }

            var result =
                await _doctorService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Doctor not found."
                });
            }

            return Ok(result);
        }


        // =========================================================
        // CREATE DOCTOR
        // =========================================================
        [HttpPost]
        [Authorize(Policy = "AdminOrSuperAdmin")]
        public async Task<IActionResult> Create(
            [FromBody] CreateDoctorDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Doctor data is required."
                });
            }

            var result =
                await _doctorService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }


        // =========================================================
        // UPDATE DOCTOR
        // =========================================================
        [HttpPut("{id:int}")]
        [Authorize(Policy = "AdminOrSuperAdmin")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateDoctorDto dto)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid doctor ID."
                });
            }

            if (dto == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Doctor data is required."
                });
            }

            var result =
                await _doctorService.UpdateAsync(id, dto);

            if (!result)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Doctor not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Doctor updated successfully."
            });
        }


        // =========================================================
        // DELETE DOCTOR
        // =========================================================
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "AdminOrSuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid doctor ID."
                });
            }

            var result =
                await _doctorService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Doctor not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Doctor deleted successfully."
            });
        }
    }
}

