using Hope_National_Hospital.Application.Dtos.Doctor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Interfaces
{
    public interface IDoctorService
    {
        Task<DoctorResponseDto>CreateAsync(CreateDoctorDto dto);
        Task<IEnumerable<DoctorResponseDto>>GetAllAsync();
        Task<DoctorResponseDto?>GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, UpdateDoctorDto dto);
        Task<bool> DeleteAsync(int id);

    }
}
