using Hope_National_Hospital.Application.Dtos.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreateAsync(CreatePaymentDto dto);

        Task<IEnumerable<PaymentResponseDto>> GetAllAsync();

        Task<PaymentResponseDto?> GetByIdAsync(int id);

        Task<PaymentSummeryDto?> GetInvoicePaymentAsync(
            int invoiceId);

        Task<bool> DeleteAsync(int id);
    }
}
