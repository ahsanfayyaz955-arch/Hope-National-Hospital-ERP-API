using Hope_National_Hospital.Application.Interfaces;
using Hope_National_Hospital.Domain.Entities;
using Hope_National_Hospital.Infrastructure.Data;

namespace Hope_National_Hospital.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public AuditService(
            AppDbContext context,
            ICurrentUserService currentUser)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));

            _currentUser = currentUser
                ?? throw new ArgumentNullException(nameof(currentUser));
        }

        public async Task LogAsync(
            string action,
            string entityName,
            string? entityId = null,
            string? description = null)
        {
            var auditLog = new AuditLog
            {
                UserId = _currentUser.UserId,
                UserEmail = _currentUser.UserEmail,
                Role = _currentUser.Role,

                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Description = description,

                IpAddress = _currentUser.IpAddress,

                CreatedAt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);

            await _context.SaveChangesAsync();
        }
    }
}