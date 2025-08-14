using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Infrastructure.Settings
{
    public record class RoleSettings
    {
        public List<string> Roles { get; set; } = new List<string>();
    }
}
