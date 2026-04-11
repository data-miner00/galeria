internal class MeilisearchProvisionOptions
{
    public string Host { get; set; }
    public string ApiKey { get; set; }
    public string IndexName { get; set; }
    public List<string> SearchableAttributes { get; set; } = [];
    public List<string> RankingRules { get; set; } = [];
    public List<string> FilterableAttributes { get; set; } = [];
    public List<string> SortableAttributes { get; set; } = [];
}


