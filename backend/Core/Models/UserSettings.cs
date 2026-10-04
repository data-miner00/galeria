using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class UserSettings : Entity
    {
        [Range(4, 6)]
        public int NoOfColumns { get; set; }

        public string? Watermark { get; set; }

        public string? LayoutType { get; set; }

        // 0 keeps recycled images until the recycle bin is emptied manually.
        [Range(0, 365)]
        public int RecycleBinRetentionDays { get; set; }
    }
}
