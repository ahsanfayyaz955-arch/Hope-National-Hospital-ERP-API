
using Hope_National_Hospital.Application.Dtos.Invoice;
using Hope_National_Hospital.Application.Interfaces;
using Hope_National_Hospital.Domain.Entities;
using Hope_National_Hospital.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Hope_National_Hospital.Infrastructure.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly AppDbContext _context;

        private readonly IAuditService _auditService;

        public InvoiceService(AppDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }



        // =========================================================
        // CREATE
        // =========================================================
        public async Task<InvoiceResponseDto> CreateAsync(
            CreateInvoiceDto dto)
        {
            // Validate request
            if (dto.Items == null || !dto.Items.Any())
            {
                throw new ArgumentException(
                    "Invoice must contain at least one item.");
            }

            if (dto.DiscountAmount < 0)
            {
                throw new ArgumentException(
                    "Discount amount cannot be negative.");
            }

            // Patient check
            var patientExists = await _context.Patients
                .AnyAsync(p => p.Id == dto.PatientId);

            if (!patientExists)
            {
                throw new KeyNotFoundException(
                    "Patient not found.");
            }

            // Validate invoice items
            foreach (var item in dto.Items)
            {
                if (item.Quantity <= 0)
                {
                    throw new ArgumentException(
                        "Item quantity must be greater than zero.");
                }

                if (item.UnitPrice < 0)
                {
                    throw new ArgumentException(
                        "Unit price cannot be negative.");
                }

                // Treatment must belong to the selected patient
                var treatmentExists = await _context.Treatments
                    .AnyAsync(t =>
                        t.Id == item.TreatmentId &&
                        t.PatinetId == dto.PatientId);

                if (!treatmentExists)
                {
                    throw new KeyNotFoundException(
                        $"Treatment {item.TreatmentId} not found for this patient.");
                }
            }

            // Calculate gross amount
            decimal grossAmount = dto.Items.Sum(item =>
                item.Quantity * item.UnitPrice);

            if (dto.DiscountAmount > grossAmount)
            {
                throw new ArgumentException(
                    "Discount cannot be greater than gross amount.");
            }

            // Calculate net amount
            decimal netAmount =
                grossAmount - dto.DiscountAmount;

            // Generate unique invoice number
            string invoiceNumber =
                $"INV-{DateTime.UtcNow:yyyyMMddHHmmssfff}";

            var invoice = new Invoice
            {
                PatientId = dto.PatientId,

                InvoiceNumber = invoiceNumber,

                InvoiceDate = DateTime.UtcNow,

                GrossAmount = grossAmount,

                DiscountAmount = dto.DiscountAmount,

                NetAmount = netAmount,

                Status = "Unpaid",

                PaymentMethod =
                    dto.PaymentMethod ?? string.Empty,

                CreatedAt = DateTime.UtcNow
            };

            // Add invoice items
            foreach (var item in dto.Items)
            {
                invoice.InvoiceItems.Add(
                    new InvoiceItem
                    {
                        TreatmentId = item.TreatmentId,

                        ItemName = item.ItemName,

                        Quantity = item.Quantity,

                        UnitPrice = item.UnitPrice,

                        TotalPrice =
                            item.Quantity * item.UnitPrice
                    });
            }

            _context.Invoices.Add(invoice);

            await _context.SaveChangesAsync();

             await _auditService.LogAsync(
              "CREATE",
                "Invoice",
                  invoice.Id.ToString(),
                 $"Invoice '{invoice.InvoiceNumber}' was created for patient '{invoice.PatientId}'."
              );

            return await GetByIdAsync(invoice.Id)
                ?? throw new InvalidOperationException(
                    "Unable to create invoice.");
        }


        // =========================================================
        // GET ALL
        // =========================================================
        public async Task<IEnumerable<InvoiceResponseDto>>
            GetAllAsync()
        {
            var invoices = await _context.Invoices
                .AsNoTracking()
                .Include(i => i.Patient)
                .Include(i => i.InvoiceItems)
                .OrderByDescending(i => i.Id)
                .ToListAsync();

            return invoices.Select(MapToDto);
        }


        // =========================================================
        // GET BY ID
        // =========================================================
        public async Task<InvoiceResponseDto?>
            GetByIdAsync(int id)
        {
            var invoice = await _context.Invoices
                .AsNoTracking()
                .Include(i => i.Patient)
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
            {
                return null;
            }

            return MapToDto(invoice);
        }


        // =========================================================
        // UPDATE
        // =========================================================
        public async Task<bool> UpdateAsync(
            int id,
            UpdateInvoiceDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
            {
                throw new ArgumentException(
                    "Invoice must contain at least one item.");
            }

            if (dto.DiscountAmount < 0)
            {
                throw new ArgumentException(
                    "Discount amount cannot be negative.");
            }

            var invoice = await _context.Invoices
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
            {
                return false;
            }

            // =====================================================
            // IMPORTANT:
            // Don't allow modifying a fully paid invoice
            // =====================================================

            if (invoice.Status == "Paid")
            {
                throw new InvalidOperationException(
                    "A fully paid invoice cannot be modified.");
            }

            // Validate items
            foreach (var item in dto.Items)
            {
                if (item.Quantity <= 0)
                {
                    throw new ArgumentException(
                        "Item quantity must be greater than zero.");
                }

                if (item.UnitPrice < 0)
                {
                    throw new ArgumentException(
                        "Unit price cannot be negative.");
                }

                var treatmentExists =
                    await _context.Treatments.AnyAsync(t =>
                        t.Id == item.TreatmentId &&
                        t.PatinetId == invoice.PatientId);

                if (!treatmentExists)
                {
                    throw new KeyNotFoundException(
                        $"Treatment {item.TreatmentId} not found for this patient.");
                }
            }

            // Calculate gross amount
            decimal grossAmount = dto.Items.Sum(item =>
                item.Quantity * item.UnitPrice);

            if (dto.DiscountAmount > grossAmount)
            {
                throw new ArgumentException(
                    "Discount cannot be greater than gross amount.");
            }

            decimal netAmount =
                grossAmount - dto.DiscountAmount;

            // =====================================================
            // PAYMENT SAFETY
            // =====================================================

            var totalPaid = await _context.Payments
                .Where(p =>
                    p.InvoiceId == invoice.Id &&
                    p.Status == "Completed")
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            if (netAmount < totalPaid)
            {
                throw new InvalidOperationException(
                    $"Invoice total cannot be less than the amount already paid. " +
                    $"Already paid: {totalPaid}.");
            }

            // Update invoice
            invoice.GrossAmount = grossAmount;

            invoice.DiscountAmount =
                dto.DiscountAmount;

            invoice.NetAmount =
                netAmount;

            invoice.PaymentMethod =
                dto.PaymentMethod ?? invoice.PaymentMethod;

            // Update status according to payment
            if (totalPaid == 0)
            {
                invoice.Status = "Unpaid";
            }
            else if (totalPaid < netAmount)
            {
                invoice.Status = "PartiallyPaid";
            }
            else
            {
                invoice.Status = "Paid";
            }

            // =====================================================
            // REMOVE OLD ITEMS
            // =====================================================

            _context.InvoiceItems.RemoveRange(
                invoice.InvoiceItems);

            invoice.InvoiceItems.Clear();

            // =====================================================
            // ADD NEW ITEMS
            // =====================================================

            foreach (var item in dto.Items)
            {
                invoice.InvoiceItems.Add(
                    new InvoiceItem
                    {
                        InvoiceId = invoice.Id,

                        TreatmentId = item.TreatmentId,

                        ItemName = item.ItemName,

                        Quantity = item.Quantity,

                        UnitPrice = item.UnitPrice,

                        TotalPrice =
                            item.Quantity * item.UnitPrice
                    });
            }

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                "UPDATE",
                 "Invoice",
                 invoice.Id.ToString(),
                 $"Invoice '{invoice.InvoiceNumber}' was updated."
             );

            return true;
        }


        // =========================================================
        // DELETE
        // =========================================================
        public async Task<bool> DeleteAsync(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
            {
                return false;
            }

            // Don't delete invoice if payments exist
            var hasPayments = await _context.Payments
                .AnyAsync(p => p.InvoiceId == id);

            if (hasPayments)
            {
                throw new InvalidOperationException(
                    "Invoice cannot be deleted because payments exist.");
            }

            _context.Invoices.Remove(invoice);

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
               "DELETE",
                "Invoice",
                invoice.Id.ToString(),
                   $"Invoice '{invoice.InvoiceNumber}' was deleted."
             );

            return true;
        }


        // =========================================================
        // MAPPING
        // =========================================================
        private static InvoiceResponseDto MapToDto(
            Invoice invoice)
        {
            return new InvoiceResponseDto
            {
                Id = invoice.Id,

                PatientId =
                    invoice.PatientId,

                PatientName =
                    invoice.Patient != null
                        ? invoice.Patient.FullName
                        : string.Empty,

                InvoiceNumber =
                    invoice.InvoiceNumber,

                InvoiceDate =
                    invoice.InvoiceDate,

                GrossAmount =
                    invoice.GrossAmount,

                DiscountAmount =
                    invoice.DiscountAmount,

                NetAmount =
                    invoice.NetAmount,

                Status =
                    invoice.Status,

                PaymentMethod =
                    invoice.PaymentMethod,

                CreatedAt =
                    invoice.CreatedAt,

                Items = invoice.InvoiceItems
                    .Select(item =>
                        new InvoiceItemResponseDto
                        {
                            Id =
                                item.Id,

                            TreatmentId =
                                item.TreatmentId,

                            ItemName =
                                item.ItemName,

                            Quantity =
                                item.Quantity,

                            UnitPrice =
                                item.UnitPrice,

                            TotalPrice =
                                item.TotalPrice
                        })
                    .ToList()
            };
        }
    }
}

