
using Asp.Versioning;
using Hope_National_Hospital.Application.Constants;
using Hope_National_Hospital.Application.Dtos.Payment;
using Hope_National_Hospital.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Hope_National_Hospital.Controllers
{

    [ApiController]
    [Authorize]
    [ApiVersion(1.0)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [EnableRateLimiting("ApiPolicy")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // =========================================================
        // GET: api/Payment
        // =========================================================
        [HttpGet]
        [Authorize(Policy = "ReceptionistOrAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _paymentService.GetAllAsync();

            return Ok(result);
        }

        // =========================================================
        // GET: api/Payment/1
        // =========================================================
        [HttpGet("{id:int}")]
        [Authorize(Policy = "ReceptionistOrAdmin")]
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
                    message = "Invalid payment ID."
                });
            }

            var result = await _paymentService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Payment not found."
                });
            }

            return Ok(result);
        }

        // =========================================================
        // GET: api/Payment/invoice/1
        // =========================================================
        [HttpGet("invoice/{invoiceId:int}")]
        [Authorize(Policy = "ReceptionistOrAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetInvoicePayments(
            int invoiceId)
        {
            if (invoiceId <= 0)
            {
                return BadRequest(new
                {
                    message = "Invalid invoice ID."
                });
            }

            var result =
                await _paymentService.GetInvoicePaymentAsync(invoiceId);

            if (result == null)
            {
                return NotFound(new
                {
                    message =
                        "No payment information found for this invoice."
                });
            }

            return Ok(result);
        }

        // =========================================================
        // POST: api/Payment
        // =========================================================
        [HttpPost]
        [Authorize(Policy = "ReceptionistOrAdmin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create(
            [FromBody] CreatePaymentDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new
                {
                    message = "Payment data is required."
                });
            }

            var result =
                await _paymentService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result);
        }

        // =========================================================
        // DELETE: api/Payment/1
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
                    message = "Invalid payment ID."
                });
            }

            var result =
                await _paymentService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Payment not found."
                });
            }

            return Ok(new
            {
                message = "Payment deleted successfully."
            });
        }
    }
}

