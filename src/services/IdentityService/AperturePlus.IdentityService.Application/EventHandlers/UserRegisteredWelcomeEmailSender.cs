using AperturePlus.IdentityService.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Application.EventHandlers
{
    public class UserRegisteredWelcomeEmailSender : INotificationHandler<UserRegisteredEvent>
    {
        private readonly ILogger<UserRegisteredWelcomeEmailSender> logger;
        public UserRegisteredWelcomeEmailSender(ILogger<UserRegisteredWelcomeEmailSender> logger)
        {
            this.logger = logger;
        }
        public Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
        {
            logger.LogWarning($"用户：{notification.UserId}已注册，发送邮箱欢迎");//未来待补
            return Task.CompletedTask;
        }
    }
}
