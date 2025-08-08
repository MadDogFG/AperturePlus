using AperturePlus.IdentityService.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Infrastructure.Persistence
{
    public class IdentityServiceDbContext:IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public IdentityServiceDbContext(DbContextOptions<IdentityServiceDbContext> options) : base(options)
        {
        }
        
    }
}
