namespace WebApi.Dtos
{
    public class DeleteByIdsRequest
    {
        public bool IsSoftDelete { get; set; } = true;
        public List<string> RequestedIds { get; set; } = [];
    }
}
