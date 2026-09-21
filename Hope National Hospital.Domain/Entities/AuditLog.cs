using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }

        //user who perform the action 
        public string? UserId { get; set; }

        //SnapShot of user information 
        public string? UserEmail { get; set; }

        public string? Role { get; set; }

        // Create / update / delete / login etc.
        public string Action { get; set; } = string.Empty;

        // Patient / doctor / invoice  etc.
        public string EntityName { get; set; } = string.Empty;

        // Id of affected record 
        public string? EntityId { get; set; }

        //Human readable description 
        public string? Description { get; set; }

        // Client IP Address
        public string? IpAddress { get; set; }

        // when action happend 
        public DateTime CreatedAt { get; set; }


    }
}
