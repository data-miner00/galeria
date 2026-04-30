namespace WebApi.Models
{
    public class UserSettingsDocument : Document
    {
        public const string PartitionKeyValue = "default";

        public int NoOfColumns { get; set; }

        public string? Watermark { get; set; }

        public UserSettings ToEntity()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(Timestamp ?? 0);
            DateTime updatedAt = dateTimeOffset.UtcDateTime;

            return new UserSettings
            { 
                Id = Id,
                NoOfColumns = NoOfColumns,
                Watermark = Watermark,
                CreatedAt = CreatedAt,
                UpdatedAt = updatedAt,
                ETag = ETag,
            };
        }

        public static UserSettingsDocument FromEntity(UserSettings userSettings)
        {
            return new UserSettingsDocument
            {
                PartitionKey = PartitionKeyValue,
                Id = "FixedId",
                DocumentType = DocumentType.UserSettings,
                CreatedAt = userSettings.CreatedAt,
                NoOfColumns = userSettings.NoOfColumns,
                Watermark = userSettings.Watermark,
                ETag = userSettings.ETag,
            };
        }
    }
}
