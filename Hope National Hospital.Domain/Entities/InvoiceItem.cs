using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Domain.Entities
{
    public class InvoiceItem
    {
        public int Id { get; set; }

        public int TreatmentId { get; set; }

        public int InvoiceId { get; set; }

        public Invoice? Invoice { get; set; }

        public string ItemName { get; set; } = string.Empty;

        public int Quantity { get; set; } = 1;

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice { get; set; }
    }

}
