using Hope_National_Hospital.Application.Dtos.Bed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Interfaces
{
    public interface IBedService
    {
        Task<IEnumerable<BedDto>> GetAllAsync();

        Task<BedDto?> GetByIdAsync(int id);

        Task<IEnumerable<BedDto>> GetAvailableAsync();

        Task<BedDto> CreateAsync(CreateBedDto dto);

        Task<bool> UpdateAsync(int id, UpdateBedDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
