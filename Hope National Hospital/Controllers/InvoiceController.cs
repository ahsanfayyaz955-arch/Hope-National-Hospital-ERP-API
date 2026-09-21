using Asp.Versioning;
using Hope_National_Hospital.Application.Constants;
using Hope_National_Hospital.Application.Dtos.Invoice;
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
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        // =========================================================
        // GET: api/Invoice
        // =========================================================
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _invoiceService.GetAllAsync();

            return Ok(result);
        }

        // =========================================================
        // GET: api/Invoice/1
        // =========================================================
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid invoice ID."
                });
            }

            var result = await _invoiceService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Invoice not found."
                });
            }

            return Ok(result);
        }

        // =========================================================
        // POST: api/Invoice
        // =========================================================
        [HttpPost]
        [Authorize(Policy = "ReceptionistOrAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create(
            [FromBody] CreateInvoiceDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    message = "Invoice data is required."
                });
            }

            var result = await _invoiceService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        // =========================================================
        // PUT: api/Invoice/1
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
            [FromBody] UpdateInvoiceDto dto)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid invoice ID."
                });
            }

            if (dto == null)
            {
                return BadRequest(new
                {
                    message = "Invoice data is required."
                });
            }

            var result = await _invoiceService.UpdateAsync(id, dto);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Invoice not found."
                });
            }

            return Ok(new
            {
                message = "Invoice updated successfully."
            });
        }

        // =========================================================
        // DELETE: api/Invoice/1
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
                    message = "Invalid invoice ID."
                });
            }

            var result = await _invoiceService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Invoice not found."
                });
            }

            return Ok(new
            {
                message = "Invoice deleted successfully."
            });
        }
    }
}