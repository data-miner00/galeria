using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class UpdateUserSettingsRequest
    {
        [Range(4, 6)]
        public int? NoOfColumns { get; set; }

        public string? Watermark { get; set; }

        [RegularExpression("^(masonry|grid)$", ErrorMessage = "{0} must be either 'masonry' or 'grid'.")]
        public string? LayoutType { get; set; }

        [RegularExpression("^(compact|comfortable|spacious)$", ErrorMessage = "{0} must be either 'compact', 'comfortable' or 'spacious'.")]
        public string? ThumbnailGap { get; set; }

        [RegularExpression("^(none|small|medium|large)$", ErrorMessage = "{0} must be either 'none', 'small', 'medium' or 'large'.")]
        public string? ThumbnailCornerRadius { get; set; }

        [Range(0, 365)]
        public int? RecycleBinRetentionDays { get; set; }
    }
}
