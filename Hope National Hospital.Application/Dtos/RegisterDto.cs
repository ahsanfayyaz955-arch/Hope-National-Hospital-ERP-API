using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100,MinimumLength =8,ErrorMessage ="Password Is Too Short")]
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "Patient";

    }
}
