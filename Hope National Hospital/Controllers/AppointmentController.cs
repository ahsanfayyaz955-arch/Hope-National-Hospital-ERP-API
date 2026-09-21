
using Hope_National_Hospital.Application.Dtos.Appointment;
using Hope_National_Hospital.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hope_National_Hospital.Application.Constants;
using Microsoft.AspNetCore.RateLimiting;
using Asp.Versioning;

namespace Hope_National_Hospital.Controllers
{
   
    [ApiController]
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [EnableRateLimiting("ApiPolicy")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(
            IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        // =========================================================
        // GET ALL APPOINTMENTS
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _appointmentService.GetAllAsync();

            return Ok(result);
        }


        // =========================================================
        // GET APPOINTMENT BY ID
        // =========================================================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid appointment ID."
                });
            }

            var result =
                await _appointmentService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Appointment not found."
                });
            }

            return Ok(result);
        }


        // =========================================================
        // CREATE APPOINTMENT
        // =========================================================
        [HttpPost]
        [Authorize(Policy = "ReceptionistOrAdmin")]
        public async Task<IActionResult> Create(
            [FromBody] CreateAppointmentDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Appointment data is required."
                });
            }

            var result =
                await _appointmentService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }


        // =========================================================
        // UPDATE APPOINTMENT
        // =========================================================
        [HttpPut("{id:int}")]
        [Authorize(Policy = "ReceptionistOrAdmin")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateAppointmentDto dto)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid appointment ID."
                });
            }

            if (dto == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Appointment data is required."
                });
            }

            var result =
                await _appointmentService.UpdateAsync(id, dto);

            if (!result)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Appointment not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Appointment updated successfully."
            });
        }


        // =========================================================
        // DELETE APPOINTMENT
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
                    message = "Invalid appointment ID."
                });
            }

            var result =
                await _appointmentService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Appointment not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Appointment deleted successfully."
            });
        }
    }
}

