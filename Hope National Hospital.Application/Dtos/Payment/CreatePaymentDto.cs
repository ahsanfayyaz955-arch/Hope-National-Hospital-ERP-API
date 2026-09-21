using System.ComponentModel.DataAnnotations;

namespace Hope_National_Hospital.Application.Dtos.Payment
{
    public class CreatePaymentDto
    {
        [Range(1, int.MaxValue,
            ErrorMessage = "Invoice ID must be greater than zero.")]
        public int InvoiceId { get; set; }

        [Range(0.01, 100000000,
            ErrorMessage = "Payment amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment method is required.")]
        [StringLength(50,
            ErrorMessage = "Payment method cannot exceed 50 characters.")]
        public string PaymentMethod { get; set; } = string.Empty;

        [StringLength(200,
            ErrorMessage = "Transaction reference cannot exceed 200 characters.")]
        public string? TransactionRefrence { get; set; }

        [StringLength(1000,
            ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? Notes { get; set; }
    }
}