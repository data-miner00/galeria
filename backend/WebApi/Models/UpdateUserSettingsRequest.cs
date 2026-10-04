using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class UpdateUserSettingsRequest
    {
        [Range(4, 6)]
        public int? NoOfColumns { get; set; }

        public string? Watermark { get; set; }

        [RegularExpression("^(masonry|grid)$", ErrorMessage = "LayoutType must be either 'masonry' or 'grid'.")]
        public string? LayoutType { get; set; }
    }
}
