using System.ComponentModel.DataAnnotations;

namespace Hope_National_Hospital.Application.Dtos.Invoice
{
    public class UpdateInvoiceItemDto
    {
        [Range(1, int.MaxValue,
            ErrorMessage = "Treatment is required.")]
        public int TreatmentId { get; set; }

        [Required(ErrorMessage = "Item name is required.")]
        [StringLength(200, MinimumLength = 2,
            ErrorMessage = "Item name must be between 2 and 200 characters.")]
        public string ItemName { get; set; } = string.Empty;

        [Range(1, 10000,
            ErrorMessage = "Quantity must be between 1 and 10,000.")]
        public int Quantity { get; set; } = 1;

        [Range(0, 100000000,
            ErrorMessage = "Unit price must be between 0 and 100,000,000.")]
        public decimal UnitPrice { get; set; }
    }
}