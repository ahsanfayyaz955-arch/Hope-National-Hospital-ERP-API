using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Dtos.Payment
{
    public class PaymentSummeryDto
    {
        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;

        public int PatientId { get; set; }

        public string PatientName { get; set; } = string.Empty;
        public decimal InvoiceAmount { get; set; }

        public decimal InvoiceTotal { get; set; }
        public decimal TotalPaid { get; set; }

        public decimal RemainingAmount { get; set; }

        public string InvoiceStatus { get; set; } = string.Empty;

        public List<PaymentResponseDto> Payments { get; set; }
        = new List<PaymentResponseDto>();


    }
}
