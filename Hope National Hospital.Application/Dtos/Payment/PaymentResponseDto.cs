using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Dtos.Payment
{
    public class PaymentResponseDto
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;

        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;

        public decimal InvoiceAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal PaymentAmount { get; set; }

        public decimal Amount { get; set; }
        public decimal TotalPaid { get; set; }

        public decimal InvoiceTotal { get; set; }
        public decimal RemainAmount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? TransectionRefrence {get;set;}
        public string? notes { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime CreatedAt { get; set; }


    }
}
