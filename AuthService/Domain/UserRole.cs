using System;

namespace Domain
{
    public class UserRole
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public DateTime AssignedAt { get; set; }
        public DateTime? RevokedAt { get; set; }

        //Navigation Properties
        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}
