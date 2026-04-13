namespace WebApi.Models
{
    public class SecuritySettingsDocument : Document
    {
        public const string PartitionKeyValue = "default";

        public bool IsTotpEnabled { get; set; }

        public string? OtpSecret { get; set; }

        public SecuritySettings ToEntity()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(Timestamp ?? 0);
            DateTime updatedAt = dateTimeOffset.UtcDateTime;

            return new SecuritySettings
            {
                Id = Id,
                CreatedAt = CreatedAt,
                UpdatedAt = updatedAt,
                ETag = ETag,
                IsTotpEnabled = IsTotpEnabled,
                OtpSecret = OtpSecret,
            };
        }

        public static SecuritySettingsDocument From(SecuritySettings settings)
        {
            return new SecuritySettingsDocument
            {
                PartitionKey = PartitionKeyValue,
                Id = "FixedId",
                DocumentType = DocumentType.SecuritySettings,
                CreatedAt = settings.CreatedAt,
                ETag = settings.ETag,
                IsTotpEnabled = settings.IsTotpEnabled,
                OtpSecret = settings.OtpSecret,
            };
        }
    }
}
