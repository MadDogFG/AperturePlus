using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Domain.ValueObjects
{
    public record class LoginIdentifier
    {
        public string Value { get; init; }
        public LoginIdentifierType Type { get; init; }

        private LoginIdentifier(string value)
        {
            this.Value = value;
            this.Type = DetermineType(value);
        }

        public static LoginIdentifier Create(string value)
        {
            return new LoginIdentifier(value);
        }

        private LoginIdentifierType DetermineType(string value)
        {
            MailAddress mailAddress;
            if (Regex.IsMatch(value, "^1[3-9]\\d{9}$"))//手机号正则，开头1，第二位3-9，后面9位非负数字
            {
                return LoginIdentifierType.PhoneNumber;
            }
            else if (MailAddress.TryCreate(value,out mailAddress))//邮箱正则
            {
                return LoginIdentifierType.Email;
            }
            else
            {
                return LoginIdentifierType.Username;
            }
        }

        public enum LoginIdentifierType
        {
            Username,
            Email,
            PhoneNumber //未来可能会有手机号登录的需求，所以预留了这个选项
        }
    }
}
