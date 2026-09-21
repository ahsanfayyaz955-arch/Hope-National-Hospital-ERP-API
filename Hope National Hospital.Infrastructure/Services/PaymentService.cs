using Hope_National_Hospital.Application.Dtos.Payment;
using Hope_National_Hospital.Application.Exceptions;
using Hope_National_Hospital.Application.Interfaces;
using Hope_National_Hospital.Domain.Entities;
using Hope_National_Hospital.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Hope_National_Hospoital.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;

        private readonly IAuditService _auditService;

        public PaymentService(AppDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }



        // =========================================================
        // CREATE PAYMENT
        // =========================================================
        public async Task<PaymentResponseDto> CreateAsync(
            CreatePaymentDto dto)
        {
            // Basic validation
            if (dto == null)
            {
                throw new BadRequestException(
                    "Payment data is required.");
            }

            if (dto.Amount <= 0)
            {
                throw new BadRequestException(
                    "Payment amount must be greater than zero.");
            }

            if (string.IsNullOrWhiteSpace(dto.PaymentMethod))
            {
                throw new BadRequestException(
                    "Payment method is required.");
            }

            // Invoice check
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.Id == dto.InvoiceId);

            if (invoice == null)
            {
                throw new NotFoundException(
                    "Invoice not found.");
            }

            // Calculate already paid
            var totalPaid = await _context.Payments
                .Where(p =>
                    p.InvoiceId == dto.InvoiceId &&
                    p.Status == "Completed")
                .SumAsync(p => (decimal?)p.Amount) ?? 0m;

            // Remaining amount
            var remainingAmount =
                invoice.NetAmount - totalPaid;

            if (remainingAmount <= 0)
            {
                throw new ConflictException(
                    "This invoice has already been fully paid.");
            }

            // Payment cannot exceed remaining
            if (dto.Amount > remainingAmount)
            {
                throw new BadRequestException(
                    $"Payment cannot be greater than remaining amount. " +
                    $"Remaining amount is {remainingAmount}.");
            }

            // Create payment
            var payment = new Payment
            {
                InvoiceId = invoice.Id,

                PatientId = invoice.PatientId,

                Amount = dto.Amount,

                PaymentMethod =
                    dto.PaymentMethod.Trim(),

                TransectionRefrence =
                    string.IsNullOrWhiteSpace(dto.TransactionRefrence)
                        ? null
                        : dto.TransactionRefrence.Trim(),

                Notes = dto.Notes,

                PaymentDate = DateTime.UtcNow,

                Status = "Completed",

                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);

            // New totals
            var newTotalPaid =
                totalPaid + dto.Amount;

            var newRemaining =
                invoice.NetAmount - newTotalPaid;

            // Update invoice status
            if (newRemaining == 0)
            {
                invoice.Status = "Paid";
            }
            else
            {
                invoice.Status = "PartiallyPaid";
            }

            invoice.PaymentMethod =
                dto.PaymentMethod.Trim();

            await _context.SaveChangesAsync();

             await _auditService.LogAsync(
              "CREATE",
                 "Payment",
                   payment.Id.ToString(),
                 $"Payment of {payment.Amount} was created for invoice '{invoice.InvoiceNumber}'."
              );

            return await GetByIdAsync(payment.Id)
                ?? throw new InvalidOperationException(
                    "Unable to create payment.");
        }


        // =========================================================
        // GET ALL PAYMENTS
        // =========================================================
        public async Task<IEnumerable<PaymentResponseDto>>
            GetAllAsync()
        {
            var payments = await _context.Payments
                .AsNoTracking()
                .Include(p => p.Invoice)
                .ThenInclude(i => i!.Patient)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            var result = new List<PaymentResponseDto>();

            foreach (var payment in payments)
            {
                result.Add(
                    await MapToDtoAsync(payment));
            }

            return result;
        }


        // =========================================================
        // GET PAYMENT BY ID
        // =========================================================
        public async Task<PaymentResponseDto?>
            GetByIdAsync(int id)
        {
            var payment = await _context.Payments
                .AsNoTracking()
                .Include(p => p.Invoice)
                .ThenInclude(i => i!.Patient)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
            {
                return null;
            }

            return await MapToDtoAsync(payment);
        }


        // =========================================================
        // GET INVOICE PAYMENTS
        // =========================================================
        public async Task<PaymentSummeryDto?>
            GetInvoicePaymentAsync(int invoiceId)
        {
            var invoice = await _context.Invoices
                .AsNoTracking()
                .Include(i => i.Patient)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);

            if (invoice == null)
            {
                return null;
            }

            var payments = await _context.Payments
                .AsNoTracking()
                .Where(p => p.InvoiceId == invoiceId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            var totalPaid = payments
                .Where(p => p.Status == "Completed")
                .Sum(p => p.Amount);

            var remainingAmount =
                invoice.NetAmount - totalPaid;

            if (remainingAmount < 0)
            {
                remainingAmount = 0;
            }

            var paymentDtos = payments
                .Select(p => new PaymentResponseDto
                {
                    Id = p.Id,

                    InvoiceId = p.InvoiceId,

                    InvoiceNumber =
                        invoice.InvoiceNumber,

                    PatientId =
                        invoice.PatientId,

                    PatientName =
                        invoice.Patient?.FullName
                        ?? string.Empty,

                    Amount = p.Amount,

                    InvoiceTotal =
                        invoice.NetAmount,

                    TotalPaid =
                        totalPaid,

                    RemainAmount =
                        remainingAmount,

                    PaymentMethod =
                        p.PaymentMethod,

                    TransectionRefrence =
                        p.TransectionRefrence,

                    PaymentDate =
                        p.PaymentDate,

                    Status =
                        p.Status,

                    notes =
                        p.Notes,

                    PaymentAmount =
                        p.Amount,

                    PaidAmount =
                        totalPaid,

                    InvoiceAmount =
                        invoice.NetAmount,

                    CreatedAt =
                        p.CreatedAt
                })
                .ToList();

            return new PaymentSummeryDto
            {
                InvoiceId =
                    invoice.Id,

                InvoiceNumber =
                    invoice.InvoiceNumber,

                PatientId =
                    invoice.PatientId,

                PatientName =
                    invoice.Patient?.FullName
                    ?? string.Empty,

                InvoiceTotal =
                    invoice.NetAmount,

                TotalPaid =
                    totalPaid,

                RemainingAmount =
                    remainingAmount,

                InvoiceStatus =
                    invoice.Status,

                Payments =
                    paymentDtos
            };
        }


        // =========================================================
        // DELETE PAYMENT
        // =========================================================
        public async Task<bool> DeleteAsync(int id)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
            {
                return false;
            }

            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(
                    i => i.Id == payment.InvoiceId);

            if (invoice == null)
            {
                throw new NotFoundException(
                    "Related invoice not found.");
            }

            _context.Payments.Remove(payment);

            var remainingPayments =
                await _context.Payments
                    .Where(p =>
                        p.InvoiceId == payment.InvoiceId &&
                        p.Id != payment.Id &&
                        p.Status == "Completed")
                    .SumAsync(p => (decimal?)p.Amount)
                    ?? 0m;

            var remainingAmount =
                invoice.NetAmount - remainingPayments;

            if (remainingPayments == 0)
            {
                invoice.Status = "Unpaid";

                invoice.PaymentMethod =
                    string.Empty;
            }
            else if (remainingAmount > 0)
            {
                invoice.Status = "PartiallyPaid";
            }
            else
            {
                invoice.Status = "Paid";
            }

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
               "DELETE",
                "Payment",
                payment.Id.ToString(),
                $"Payment of {payment.Amount} for invoice '{invoice.InvoiceNumber}' was deleted."
            );

            return true;
        }


        // =========================================================
        // MAPPING
        // =========================================================
        private async Task<PaymentResponseDto>
            MapToDtoAsync(Payment payment)
        {
            var totalPaid =
                await _context.Payments
                    .Where(p =>
                        p.InvoiceId == payment.InvoiceId &&
                        p.Status == "Completed")
                    .SumAsync(p => (decimal?)p.Amount)
                    ?? 0m;

            var invoiceTotal =
                payment.Invoice?.NetAmount
                ?? 0m;

            var remainingAmount =
                invoiceTotal - totalPaid;

            if (remainingAmount < 0)
            {
                remainingAmount = 0;
            }

            return new PaymentResponseDto
            {
                Id =
                    payment.Id,

                InvoiceId =
                    payment.InvoiceId,

                InvoiceNumber =
                    payment.Invoice?.InvoiceNumber
                    ?? string.Empty,

                PatientId =
                    payment.Invoice?.PatientId
                    ?? 0,

                PatientName =
                    payment.Invoice?.Patient?.FullName
                    ?? string.Empty,

                InvoiceAmount =
                    invoiceTotal,

                PaidAmount =
                    totalPaid,

                PaymentAmount =
                    payment.Amount,

                Amount =
                    payment.Amount,

                InvoiceTotal =
                    invoiceTotal,

                TotalPaid =
                    totalPaid,

                RemainAmount =
                    remainingAmount,

                PaymentMethod =
                    payment.PaymentMethod,

                TransectionRefrence =
                    payment.TransectionRefrence,

                PaymentDate =
                    payment.PaymentDate,

                Status =
                    payment.Status,

                notes =
                    payment.Notes,

                CreatedAt =
                    payment.CreatedAt
            };
        }
    }
}