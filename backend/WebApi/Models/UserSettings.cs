using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class UserSettings : Entity
    {
        [Range(4, 6)]
        public int NoOfColumns { get; set; }

        public string? Watermark { get; set; }
    }
}
