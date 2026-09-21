using Hope_National_Hospital.Application.Dtos.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Interfaces
{
    public interface IPatientService
    {
        Task<PatientResponseDto>CreateAsync(CreatePatientDto dto);
        Task<IEnumerable<PatientResponseDto>>GetAllAsync();
        Task<PatientResponseDto?>GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, UpdatePatientDto dto);
        Task<bool> DeleteAsync(int id);

    }
}
