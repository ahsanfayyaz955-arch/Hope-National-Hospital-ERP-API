using Hope_National_Hospital.Application.Dtos.Treatment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Interfaces
{
    public interface ITreatmentService
    {
        Task<ResponseTreatmentDto> CreateAsync(CreateTreatmentDto dto);
        Task<IEnumerable<ResponseTreatmentDto>> GetAllAsync();
        Task<ResponseTreatmentDto?> GetByIdAsync(int id);

        Task<bool> UpdateAsync(int id, UpdateTreatmentDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
