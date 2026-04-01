namespace WebApi.Models
{
    public class UserProfileDocument : Document
    {
        public const string PartitionKeyValue = "default";

        public string? AvatarImage { get; set; }

        public string? Username { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Bio { get; set; }

        public string? WebsiteUrl { get; set; }

        public UserProfile ToEntity()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(Timestamp ?? 0);
            DateTime updatedAt = dateTimeOffset.UtcDateTime;

            return new UserProfile
            {
                Id = Id,
                AvatarImage = AvatarImage,
                Username = Username,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Bio = Bio,
                WebsiteUrl = WebsiteUrl,
                CreatedAt = CreatedAt,
                UpdatedAt = updatedAt,
                ETag = ETag,
            };
        }

        public static UserProfileDocument FromEntity(UserProfile entity)
        {
            return new UserProfileDocument
            {
                Id = entity.Id,
                AvatarImage = entity.AvatarImage,
                Username = entity.Username,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                Bio = entity.Bio,
                WebsiteUrl = entity.WebsiteUrl,
                CreatedAt = entity.CreatedAt,
                ETag = entity.ETag,
                DocumentType = DocumentType.UserProfile,
                PartitionKey = PartitionKeyValue,
            };
        }
    }
}
