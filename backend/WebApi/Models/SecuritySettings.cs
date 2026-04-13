namespace WebApi.Models
{
    public class SecuritySettings : Entity
    {
        public bool IsTotpEnabled { get; set; }

        public string? OtpSecret { get; set; }
    }
}
