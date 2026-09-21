using Hope_National_Hospital.Application.Dtos.Department;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Interfaces
{
    public interface IDepartmentService
    {
        Task<DepartmentResponseDto>CreateAsync(DepartmentCreateDto dto);

        Task<IEnumerable<DepartmentResponseDto>>GetAllAsync();

        Task<DepartmentResponseDto?>GetByIdAsync(int Id);

        Task<bool>UpdateAsync(int Id,DepartmentUpdateDto dto);

        Task<bool>DeleteAsync(int Id);
    }
}
