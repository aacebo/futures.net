using System.Text.Json;

using OAI = OpenAI.Chat;

namespace Futures.OpenAI.Chat;

public partial class Request
{
    private readonly Dictionary<string, Func<OAI.ChatToolCall, object?>> _handlers;

    public Request Function<TParams>
    (
        string name,
        string? description = null,
        BinaryData? parameters = null,
        Func<TParams, object?>? handler = null,
        bool strict = false
    )
    {
        var req = new Request(this);
        var tool = OAI.ChatTool.CreateFunctionTool(
            name,
            description,
            parameters,
            strict
        );

        req._options.Tools.Add(tool);

        if (handler is not null)
        {
            _handlers[name] = call =>
            {

                var @params = JsonSerializer.Deserialize<TParams>(call.FunctionArguments) ?? throw new ArgumentException("could not deserialize params");
                return handler(@params);
            };
        }

        return req;
    }

    public Request Function<TParams>
    (
        string name,
        string? description = null,
        BinaryData? parameters = null,
        Func<TParams, Task<object?>>? handler = null,
        bool strict = false
    )
    {
        return Function<TParams>(
            name,
            description,
            parameters,
            handler is null ? null : call => handler(call).ConfigureAwait(false).GetAwaiter().GetResult(),
            strict
        );
    }
}