using Google.GenAI;
using Google.GenAI.Types;
using System.Text.Json;
using WebApi.Options;
using WebApi.Services;
using GoogleType = Google.GenAI.Types.Type;

namespace WebApi.Clients;

public class GeminiClient : IGenAiClient
{
    private const string CaptionPrompt = """Caption the image and generate at least 3 relevant tags. Return the response strictly in parsable plain JSON format. Do not use markdown fenced block. Format: {"description": "<caption>", "tags": ["tag1", "tag2"]}""";
    private static JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };
    private readonly Client client;
    private readonly GeminiOptions options;
    private readonly ILogger<GeminiClient> logger;

    public GeminiClient(Client client, GeminiOptions options, ILogger<GeminiClient> logger)
    {
        this.client = client;
        this.options = options;
        this.logger = logger;
    }

    public async Task<CaptionResponse?> GenerateCaptionAsync(MemoryStream imageStream, string mimeType)
    {
        var generationOption = GetGenerateContentConfig();

        try
        {
            var response = await client.Models.GenerateContentAsync(
                model: options.Model,
                contents:
                [
                    new Content { Parts = [ new Part { Text = CaptionPrompt }]},
                    new Content { Parts = [ Part.FromBytes(imageStream.ToArray(), mimeType) ]}
                ],
                config: generationOption);

            imageStream.Position = 0;

            var parsedResponse = JsonSerializer.Deserialize<CaptionResponse>(response.Text, SerializerOptions);

            return parsedResponse;
        }
        catch (ServerError ex)
        {
            // Optional
            this.logger.LogError(ex, "Failed to caption image: {Message}", ex.Message);
            return null;
        }
    }

    private static Schema GetResponseSchema()
    {
        return new Schema
        {
            Properties = new Dictionary<string, Schema>
            {
                {
                    "description", new Schema { Type = GoogleType.String, Title = "Description" }
                },
                {
                    "tags", new Schema { Type = GoogleType.Array, Title = "Tags" }
                },
            },
            PropertyOrdering = ["description", "tags"],
            Required = ["description", "tags"],
            Title = "Response",
            Type = GoogleType.Object,
        };
    }

    private static GenerateContentConfig GetGenerateContentConfig()
    {
        return new GenerateContentConfig
        {
            ResponseSchema = GetResponseSchema(),
            ResponseMimeType = "application/json",
        };
    }
}
