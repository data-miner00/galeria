using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WebApi.Models
{
    public abstract class Document
    {
        [JsonProperty("_etag")]
        public string? ETag { get; set; }

        [JsonProperty("_ts")]
        public long? Timestamp { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }

        public DocumentType DocumentType { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
