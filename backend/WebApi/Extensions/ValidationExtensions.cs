namespace WebApi.Extensions
{
    public static class ValidationExtensions
    {
        private static readonly HashSet<string> ValidImageExtensions = [
            ".jpg",    
            ".jpeg",    
            ".bmp",    
            ".png",    
            ".webp",    
            ".gif",    
            ".avif",    
            ".heif",    
            ".heic",    
            ".svg",    
        ];

        public static bool IsValidImageFile(this IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);

            return ValidImageExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }
    }
}
