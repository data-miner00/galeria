namespace WebApi.Dtos
{
    public class SecuritySettings
    {
        public bool IsTotpEnabled { get; set; }

        public SecuritySettings FromInternal(Models.SecuritySettings settings)
        {
            return new SecuritySettings
            {
                IsTotpEnabled = settings.IsTotpEnabled,
            };
        }
    }
}
