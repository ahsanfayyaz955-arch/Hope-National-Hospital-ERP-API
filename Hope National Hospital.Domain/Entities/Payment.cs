using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        //invoice relation 
        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }

        //patient RelationShip 
        public int PatientId { get; set; }
        public Patient? Patient { get; set; }
        
        //payment information 
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = "Completed";

        public string? TransectionRefrence { get; set; }
        public string? Notes { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        //Audit 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        
    }
}
