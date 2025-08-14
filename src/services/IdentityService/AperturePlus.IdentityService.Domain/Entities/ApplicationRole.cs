using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Domain.Entities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public DateTime CreatedAt { get; private set; }
        public string? Description { get; private set; }
        private ApplicationRole() : base()
        {

        }
        private ApplicationRole(string roleName) : base(roleName)
        {
            this.CreatedAt = DateTime.UtcNow;
        }

        public static ApplicationRole Create(string roleName)
        {
            return new ApplicationRole(roleName);
        }

        public void SetDescription(string? description)
        {
            if (description == null)
            {
                description = "";
            }
            this.Description = description;
        }
    }
}
