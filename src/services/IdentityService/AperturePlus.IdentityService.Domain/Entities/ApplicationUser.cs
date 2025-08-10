using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Domain.Entities
{
    public class ApplicationUser:IdentityUser<Guid>
    {
        public DateTime CreatedAt {  get; private set; }
        private ApplicationUser() : base()
        {
            //空构造函数，不然EF Core报错，好像是反射获取不到了
        }
        private ApplicationUser(string userName):base(userName)
        {
            this.CreatedAt = DateTime.UtcNow;
        }

        public static ApplicationUser CreateWithEmail(string userName,string email)//未来可能会有手机号注册的需求，所以把构造函数设为私有的，用静态方法来通过不同的注册方式创建用户
        {
            return new ApplicationUser(userName) { Email = email };

        }
    }
}
