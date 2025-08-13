using System.ComponentModel.DataAnnotations;

namespace AperturePlus.IdentityService.Api.DTOs
{
    public record class RegisterRequest
    {
        [Required(ErrorMessage = "用户名不能为空")]
        [StringLength(16, MinimumLength = 1, ErrorMessage = "用户名长度需在1-16字符之间")]
        public string Username { get; set; }
        [Required(ErrorMessage = "密码不能为空")]
        [StringLength(32, MinimumLength = 6, ErrorMessage = "密码长度需在6-32字符之间")]
        public string Password { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        public string? Email { get; set; }//可空是因为未来可能会有手机号注册的需求

    }
}
