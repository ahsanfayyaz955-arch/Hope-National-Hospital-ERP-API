using Hope_National_Hospital.Domain.Entities;

public class Invoice
{
    public int Id { get; set; }

    public int PatientId { get; set; }
    public Patient? Patient { get; set; }

    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    public string InvoiceNumber { get; set; } = string.Empty;

    public decimal GrossAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal NetAmount { get; set; }

    public string Status { get; set; } = "Unpaid";

    public string PaymentMethod { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<InvoiceItem> InvoiceItems { get; set; }
        = new List<InvoiceItem>();
}