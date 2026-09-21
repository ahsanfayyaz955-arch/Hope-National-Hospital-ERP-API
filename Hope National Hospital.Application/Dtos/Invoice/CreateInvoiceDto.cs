using System.ComponentModel.DataAnnotations;

namespace Hope_National_Hospital.Application.Dtos.Invoice
{
    public class CreateInvoiceDto
    {
        [Range(1, int.MaxValue,
            ErrorMessage = "Patient is required.")]
        public int PatientId { get; set; }


        [Range(0, double.MaxValue,
            ErrorMessage = "Discount amount cannot be negative.")]
        public decimal DiscountAmount { get; set; }


        [StringLength(50,
            ErrorMessage = "Payment method cannot exceed 50 characters.")]
        public string? PaymentMethod { get; set; }


        [Required(ErrorMessage = "Invoice must contain at least one item.")]
        [MinLength(1,
            ErrorMessage = "Invoice must contain at least one item.")]
        public List<CreateInvoiceItemDto> Items { get; set; }
            = new List<CreateInvoiceItemDto>();
    }
}