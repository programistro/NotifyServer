using Microsoft.AspNetCore.Identity;

namespace NotifyNet.Core.Models
{
    public class Permission : IdentityRole<Guid>
    {
        public virtual ICollection<EmployeePermission> EmployeePermissions { get; set; }

        public string RoleName { get; set; }

        public string Description { get; set; }
    }
}
