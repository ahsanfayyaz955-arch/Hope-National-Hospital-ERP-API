using Hope_National_Hospital.Application.Dtos.Receptionist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Interfaces
{
    public interface IReceptionistService
    {
        Task<ResponseReceptionistDto> CreateAsync(CreateReceptionistDto dto);
        Task<IEnumerable<ResponseReceptionistDto>> GetAllAsync();

        Task<ResponseReceptionistDto?> GetByIdAsync(int id );

        Task<bool> UpdateAsync(int id, UpdateReceptionistDto dto);
        Task<bool> DeleteAsync(int id);

    }
}
