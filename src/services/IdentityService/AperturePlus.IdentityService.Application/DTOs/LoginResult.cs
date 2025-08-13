using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AperturePlus.IdentityService.Application.DTOs
{
    public record class LoginResult
    {
        public bool Succeeded { get; init; }
        public string? Token { get; init; } //Token可能不存在
        public IEnumerable<string> Errors { get; init; }

        public LoginResult(bool succeeded, string? token, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Token = token;
            Errors = errors;
        }

        public static LoginResult Success(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("成功的LoginResult必须提供Token", nameof(token));
            }
            return new LoginResult(true, token, Enumerable.Empty<string>());
        }

        public static LoginResult Failure(IEnumerable<string> errors)
        {
            var errorList = errors.ToList();
            if (!errorList.Any())
            {
                throw new ArgumentException("失败的LoginResult必须提供错误信息。", nameof(errors));
            }
            return new LoginResult(false, null, errorList);
        }
    }
}

