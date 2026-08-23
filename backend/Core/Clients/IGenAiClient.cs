using Core.Services;

namespace Core.Clients
{
    public interface IGenAiClient
    {
        Task<CaptionResponse?> GenerateCaptionAsync(MemoryStream imageStream, string mimeType);
    }
}
