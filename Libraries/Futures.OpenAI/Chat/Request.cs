using Futures.Operators;

using OAI = OpenAI.Chat;

namespace Futures.OpenAI.Chat;

public partial class Request
{
    private readonly OAI.ChatClient _client;
    private OAI.ChatCompletionOptions _options;
    private IList<OAI.ChatMessage>? _storage;

    public Request(OAI.ChatClient client)
    {
        _client = client;
        _options = new();
        _handlers = [];
    }

    public Request(OAI.ChatClient client, OAI.ChatCompletionOptions? options = null)
    {
        _client = client;
        _options = options ?? new();
        _handlers = [];
    }

    public Request(Request other)
    {
        _client = other._client;
        _options = other._options;
        _storage = other._storage;
        _handlers = other._handlers;
    }

    public Request Options(OAI.ChatCompletionOptions options)
    {
        var req = new Request(this);
        req._options = options;
        return req;
    }

    public Request Storage(IList<OAI.ChatMessage> storage)
    {
        var req = new Request(this);
        req._storage = storage;
        return req;
    }

    public Future<OAI.ChatCompletion> Build()
    {

    }

    public Future<OAI.ChatCompletion> Send(params OAI.ChatMessage[] messages)
    {
        return Send(messages.AsEnumerable());
    }

    public Future<OAI.ChatCompletion> Send(params string[] messages)
    {
        return Send(messages.Select(m => new OAI.UserChatMessage(m)).ToArray());
    }

    public Future<OAI.ChatCompletion> Send(IEnumerable<OAI.ChatMessage> messages, OAI.ChatCompletionOptions? options = null, CancellationToken cancellation = default)
    {
        var storage = _storage ?? [];

        foreach (var message in messages)
        {
            storage.Add(message);
        }

        return new Future<OAI.ChatCompletion>().Run(self =>
        {
            OAI.ChatCompletion completion = _client.CompleteChat(storage, options ?? _options, cancellation);

            while (completion.FinishReason == OAI.ChatFinishReason.ToolCalls)
            {
                List<OAI.ChatMessage> messages = [.. storage, OAI.ChatMessage.CreateAssistantMessage(completion)];
                messages.AddRange(Call([.. completion.ToolCalls]));
                completion = _client.CompleteChat(messages, options ?? _options, cancellation);
            }

            self.Next(completion);
            self.Complete();
        });
    }

    public Future<OAI.StreamingChatCompletionUpdate> SendStream(params OAI.ChatMessage[] messages)
    {
        return SendStream(messages.AsEnumerable());
    }

    public Future<OAI.StreamingChatCompletionUpdate> SendStream(params string[] messages)
    {
        return SendStream(messages.Select(m => new OAI.UserChatMessage(m)).ToArray());
    }

    public Future<OAI.StreamingChatCompletionUpdate> SendStream(IEnumerable<OAI.ChatMessage> messages, OAI.ChatCompletionOptions? options = null, CancellationToken cancellation = default)
    {
        var storage = _storage ?? [];

        foreach (var message in messages)
        {
            storage.Add(message);
        }

        return new Future<OAI.StreamingChatCompletionUpdate>().Run(self =>
        {
            var res = _client.CompleteChatStreaming(messages, options ?? _options, cancellation);
            var builder = new Streaming.CompletionBuilder();

            foreach (var update in res)
            {
                builder = builder.Append(update);
                var completion = builder.Build();

                while (update.FinishReason == OAI.ChatFinishReason.ToolCalls)
                {
                    List<OAI.ChatMessage> messages = [.. storage, OAI.ChatMessage.CreateAssistantMessage(completion)];
                    messages.AddRange(Call([.. completion.ToolCalls]));
                    completion = _client.CompleteChat(messages, options ?? _options, cancellation);
                }

                self.Next(update);
            }

            self.Complete();
        });
    }
}