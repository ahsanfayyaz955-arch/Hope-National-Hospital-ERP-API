using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Dtos.Receptionist
{
    public class ResponseReceptionistDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }

        public int DepartmentId { get; set; } 
        public string DepartmentName { get; set; } = string.Empty;

       public bool IsActive { get; set; }
    }
}
