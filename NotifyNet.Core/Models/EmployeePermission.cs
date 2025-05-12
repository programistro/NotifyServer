using Microsoft.AspNetCore.Identity;

namespace NotifyNet.Core.Models
{
    public class EmployeePermission : IdentityUserRole<Guid>
    {
        public virtual Employee? Employee { get; set; }

        public virtual Permission? Permission { get; set; }
    }
}
