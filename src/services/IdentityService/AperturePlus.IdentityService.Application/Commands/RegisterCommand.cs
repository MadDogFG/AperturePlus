using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Application.Commands
{
    public record class RegisterCommand//因为API层是引用应用层，而不是反过来，所以我们不能直接使用DTO，而是用Command来传递数据，而且有些时候DTO和Command是有差异的，DTO是为了适配前端的，而Command是为了适配应用层的业务逻辑，有些参数
    {
        public string username { get; init; }
        public string password { get; init; }
        public string? email { get; init; }//可空是因为未来可能会有手机号注册的需求

        public RegisterCommand(string username, string password, string? email = null)
        {
            this.username = username;
            this.password = password;
            this.email = email;
        }
    }
}
