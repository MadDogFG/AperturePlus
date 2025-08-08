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
        public DateTime CreatedAt { get; init; }
        public string? Description { get; private set; }

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
