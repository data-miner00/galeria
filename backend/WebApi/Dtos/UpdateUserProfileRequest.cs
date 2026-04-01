namespace WebApi.Dtos
{
    public class UpdateUserProfileRequest
    {
        public string? AvatarImage { get; set; }

        public string? Username { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? Bio { get; set; }

        public string? WebsiteUrl { get; set; }
    }
}
