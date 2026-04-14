using WebApi.Services;

namespace WebApi.Clients
{
    public interface IGenAiClient
    {
        Task<CaptionResponse> GenerateCaptionAsync(MemoryStream imageStream, string mimeType);
    }
}
