using AperturePlus.IdentityService.Application.Commands;
using AperturePlus.IdentityService.Application.Interfaces;
using AperturePlus.IdentityService.Domain.Entities;
using AperturePlus.IdentityService.Domain.Events;
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
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, IdentityResult>
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IPublisher publisher;
        private readonly IMessageBus messageBus;
        public RegisterCommandHandler(UserManager<ApplicationUser> userManager, IPublisher publisher, IMessageBus messageBus)
        {
            this.userManager = userManager;
            this.publisher = publisher;
            this.messageBus = messageBus;
        }
        public async Task<IdentityResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.Email))
            {
                if(await userManager.FindByNameAsync(request.Username) != null)
                {
                    return IdentityResult.Failed(new IdentityError { Code = "RegisterCommandHandler", Description = "用户名已存在" });
                }
                if(await userManager.FindByEmailAsync(request.Email) != null)
                {
                    return IdentityResult.Failed(new IdentityError { Code = "RegisterCommandHandler", Description = "邮箱已存在" });
                }
                ApplicationUser user = ApplicationUser.CreateWithEmail(request.Username, request.Email);
                IdentityResult result = await userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                { 
                    result = await userManager.AddToRoleAsync(user, "User");//默认添加到User角色
                    await publisher.Publish(new UserRegisteredEvent(user.Id));//发布用户注册事件
                    var userRegisterIntegrationEvent = new UserRegisteredIntegrationEvent(user.Id, user.UserName);
                    await messageBus.Publish("user_events","user.register", userRegisterIntegrationEvent);//通过消息总线发布用户注册消息
                }
                return result;
            }
            //未来这里可以扩展为手机号注册
            return IdentityResult.Failed(new IdentityError { Code = "RegisterCommandHandler", Description = "注册失败，登录标识不能为空" });
        }
    }
}
