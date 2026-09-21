using Hope_National_Hospital.Application.Dtos.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceResponseDto> CreateAsync(CreateInvoiceDto dto);

        Task<IEnumerable<InvoiceResponseDto>> GetAllAsync();

        Task<InvoiceResponseDto?> GetByIdAsync(int id);

        Task<bool> UpdateAsync(int id, UpdateInvoiceDto dto);

        Task<bool> DeleteAsync(int id);
    }
}
