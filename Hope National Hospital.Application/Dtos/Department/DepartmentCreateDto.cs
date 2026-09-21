using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Dtos.Department
{
    public class DepartmentCreateDto
    {
        [Required(ErrorMessage = "Department name is required.")]
        [StringLength(100, MinimumLength = 2,
            ErrorMessage = "Department name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500,
            ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }


    }
}
