using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Models
{
    public abstract class Entity
    {
        public string? ETag { get; set; }

        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }

        //public DateTime UpdatedAt { get; set; }
    }
}
