
using Asp.Versioning;
using Hope_National_Hospital.Application.Constants;
using Hope_National_Hospital.Application.Dtos.Department;
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
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(
            IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }


        // =========================================================
        // GET ALL DEPARTMENTS
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _departmentService.GetAllAsync();

            return Ok(result);
        }


        // =========================================================
        // GET DEPARTMENT BY ID
        // =========================================================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid department ID."
                });
            }

            var result =
                await _departmentService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Department not found."
                });
            }

            return Ok(result);
        }


        // =========================================================
        // CREATE DEPARTMENT
        // =========================================================
        [HttpPost]
        [Authorize(Policy = "AdminOrSuperAdmin")]
        public async Task<IActionResult> Create(
            [FromBody] DepartmentCreateDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Department data is required."
                });
            }

            var result =
                await _departmentService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }


        // =========================================================
        // UPDATE DEPARTMENT
        // =========================================================
        [HttpPut("{id:int}")]
        [Authorize(Policy = "AdminOrSuperAdmin")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] DepartmentUpdateDto dto)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid department ID."
                });
            }

            if (dto == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Department data is required."
                });
            }

            var result =
                await _departmentService.UpdateAsync(id, dto);

            if (!result)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Department not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Department updated successfully."
            });
        }


        // =========================================================
        // DELETE DEPARTMENT
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
                    message = "Invalid department ID."
                });
            }

            var result =
                await _departmentService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Department not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Department deleted successfully."
            });
        }
    }
}

