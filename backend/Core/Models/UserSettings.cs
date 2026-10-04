using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class UserSettings : Entity
    {
        [Range(4, 6)]
        public int NoOfColumns { get; set; }

        public string? Watermark { get; set; }

        public string? LayoutType { get; set; }
    }
}
