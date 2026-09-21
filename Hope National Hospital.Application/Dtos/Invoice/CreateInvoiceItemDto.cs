using System.ComponentModel.DataAnnotations;

namespace Hope_National_Hospital.Application.Dtos.Invoice
{
    public class CreateInvoiceItemDto
    {
        [Range(1, int.MaxValue,
            ErrorMessage = "Treatment is required.")]
        public int TreatmentId { get; set; }


        [Required(ErrorMessage = "Item name is required.")]
        [StringLength(200,
            ErrorMessage = "Item name cannot exceed 200 characters.")]
        public string ItemName { get; set; } = string.Empty;


        [Range(1, int.MaxValue,
            ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;


        [Range(0, double.MaxValue,
            ErrorMessage = "Unit price cannot be negative.")]
        public decimal UnitPrice { get; set; }
    }
}