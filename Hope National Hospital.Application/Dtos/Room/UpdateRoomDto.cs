using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Dtos.Room
{
    public class UpdateRoomDto
    {
        [Required(ErrorMessage = "Room number is required.")]
        [StringLength(20, ErrorMessage = "Room number cannot exceed 20 characters.")]
        public string RoomNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Room type is required.")]
        [StringLength(50, ErrorMessage = "Room type cannot exceed 50 characters.")]
        public string RoomType { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Department is required.")]
        public int DepartmentId { get; set; }

        public bool IsActive { get; set; }
    }
}
