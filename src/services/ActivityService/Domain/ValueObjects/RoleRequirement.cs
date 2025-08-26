using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.ActivityService.Domain.ValueObjects
{
    public record class RoleRequirement
    {
        public RoleType Role { get; private set; }
        public int Quantity { get; private set; }
        public RoleRequirement(RoleType role, int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(quantity), "数量必须大于0");
            }
            Role = role;
            Quantity = quantity;
        }
    }
}
