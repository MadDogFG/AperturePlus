using AperturePlus.IdentityService.Application.Commands;
using AperturePlus.IdentityService.Application.Interfaces;
using AperturePlus.IdentityService.Domain.Entities;
using Contracts;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Application.Handlers
{
    public class UpdateUserRolesCommandHandler : IRequestHandler<UpdateUserRolesCommand, IdentityResult>
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMessageBus messageBus;

        public UpdateUserRolesCommandHandler(UserManager<ApplicationUser> userManager, IMessageBus messageBus)
        {
            this.userManager = userManager;
            this.messageBus = messageBus;
        }

        public async Task<IdentityResult> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId.ToString());
            if(user == null)
            {
                return IdentityResult.Failed(new IdentityError { Code = "UpdateUserRolesCommandHandler", Description = "用户未找到" });
            }
            var currentRoles = await userManager.GetRolesAsync(user);
            var newRoles = request.Roles.Distinct().Where(r=>r!="Admin").ToList();
            if (!newRoles.Contains("User")) 
            {
                newRoles.Insert(0, "User");
            }
            var rolestoAdd = newRoles.Except(currentRoles).ToList();
            var result = await userManager.AddToRolesAsync(user, rolestoAdd);
            if (!result.Succeeded)
            {
                return result;
            }
            var rolestoRemove = currentRoles.Except(newRoles).ToList();
            result = await userManager.RemoveFromRolesAsync(user, rolestoRemove);
            
            if (result.Succeeded)
            {
                // 获取更新后的用户角色
                var updatedRoles = await userManager.GetRolesAsync(user);
                var userRolesUpdatedEvent = new UserRolesUpdatedIntegrationEvent(user.Id, user.UserName, updatedRoles.ToList());
                await messageBus.Publish("user_events", "user.roles.updated", userRolesUpdatedEvent);
            }
            
            return result;
        }
    }
}
