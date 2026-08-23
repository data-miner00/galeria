

namespace WebApi.Models
{
    public class SecuritySettings
    {
        public bool IsTotpEnabled { get; set; }

        public static SecuritySettings FromInternal(Core.Models.SecuritySettings settings)
        {
            return new SecuritySettings
            {
                IsTotpEnabled = settings.IsTotpEnabled,
            };
        }
    }
}
