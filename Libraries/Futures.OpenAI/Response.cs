using Responses = OpenAI.Responses;

namespace Futures.OpenAI;

public class Response : Future<Responses.ResponseCreationOptions, Responses.OpenAIResponse>, IFuture<Responses.ResponseCreationOptions, Responses.OpenAIResponse>
{
    public readonly Responses.OpenAIResponseClient Client;

    public Response(Responses.OpenAIResponseClient client, CancellationToken cancellation = default) : base(cancellation)
    {
        Client = client;
        Resolver = options => client.CreateResponse([], options);
    }

    public IFuture<Responses.ResponseCreationOptions, Responses.OpenAIResponse> Function(string name, string description, BinaryData parameters, bool strict = false)
    {
        var tool = Responses.ResponseTool.CreateFunctionTool(
            name,
            description,
            parameters,
            strict
        );

        return new Future<Responses.ResponseCreationOptions, Responses.OpenAIResponse>(
            options =>
            {
                options.Tools.Add(tool);
                return Client.CreateResponse([], options);
            }
        );
    }

    public IFuture<Responses.ResponseCreationOptions, Responses.OpenAIResponse> FileSearch(IEnumerable<string> vectorStoreIds, int? maxResultCount = null, Responses.FileSearchToolRankingOptions? rankingOptions = null, BinaryData? filters = null)
    {
        var tool = Responses.ResponseTool.CreateFileSearchTool(
            vectorStoreIds,
            maxResultCount,
            rankingOptions,
            filters
        );

        return new Future<Responses.ResponseCreationOptions, Responses.OpenAIResponse>(
            options =>
            {
                options.Tools.Add(tool);
                return Client.CreateResponse([], options);
            }
        );
    }

    public IFuture<Responses.ResponseCreationOptions, Responses.OpenAIResponse> WebSearch(Responses.WebSearchUserLocation? userLocation = null, Responses.WebSearchContextSize? searchContextSize = null)
    {
        var tool = Responses.ResponseTool.CreateWebSearchTool(
            userLocation,
            searchContextSize
        );

        return new Future<Responses.ResponseCreationOptions, Responses.OpenAIResponse>(
            options =>
            {
                options.Tools.Add(tool);
                return Client.CreateResponse([], options);
            }
        );
    }
}