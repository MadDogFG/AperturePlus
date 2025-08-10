namespace AperturePlus.IdentityService.Api.DTOs
{
    public record class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string? Email { get; set; }//可空是因为未来可能会有手机号注册的需求

    }
}
