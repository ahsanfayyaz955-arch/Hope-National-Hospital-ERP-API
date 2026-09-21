using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? UserEmail { get; }
        string? Role { get; }
        string? IpAddress { get;}
        bool IsAuthenticated { get; }
    }
}
