using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Dtos.Bed
{
    public class UpdateBedDto
    {
        [Required(ErrorMessage = "Bed number is required.")]
        [StringLength(20, ErrorMessage = "Bed number cannot exceed 20 characters.")]
        public string BedNumber { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Room is required.")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Bed status is required.")]
        public string Status { get; set; } = "Available";

        public bool IsActive { get; set; }
    }
}
