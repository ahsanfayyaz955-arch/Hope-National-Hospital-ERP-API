using Hope_National_Hospital.Application.Dtos.Admission;

namespace Hope_National_Hospital.Application.Interfaces
{
    public interface IAdmissionService
    {
        Task<IEnumerable<AdmissionDto>> GetAllAsync();

        Task<AdmissionDto?> GetByIdAsync(int id);

        Task<AdmissionDto> CreateAsync(CreateAdmissionDto dto);

        Task<bool> UpdateAsync(
            int id,
            UpdateAdmissionDto dto);

        Task<bool> DischargeAsync(int id);
    }
}