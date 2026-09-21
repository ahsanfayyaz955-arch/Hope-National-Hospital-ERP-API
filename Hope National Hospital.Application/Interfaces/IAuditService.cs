using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Interfaces
{
    public interface IAuditService
    {
        Task LogAsync(string action, string entityName, string? entityId = null,
            string? description = null);
    }
}
