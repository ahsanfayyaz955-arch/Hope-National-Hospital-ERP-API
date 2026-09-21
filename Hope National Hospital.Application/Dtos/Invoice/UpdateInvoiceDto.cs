using System.ComponentModel.DataAnnotations;

namespace Hope_National_Hospital.Application.Dtos.Invoice
{
    public class UpdateInvoiceDto
    {
        [Range(0, 100000000,
            ErrorMessage = "Discount amount must be between 0 and 100,000,000.")]
        public decimal DiscountAmount { get; set; }

        [StringLength(50,
            ErrorMessage = "Payment method cannot exceed 50 characters.")]
        public string? PaymentMethod { get; set; }

        [Required(ErrorMessage = "Invoice must contain at least one item.")]
        [MinLength(1,
            ErrorMessage = "Invoice must contain at least one item.")]
        public List<UpdateInvoiceItemDto> Items { get; set; }
            = new List<UpdateInvoiceItemDto>();
    }
}