using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateToken(string UserId, string Email, string FullName,
            IList<string> roles);
    }
}
