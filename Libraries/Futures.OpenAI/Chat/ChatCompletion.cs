using OAI = OpenAI.Chat;

namespace Futures.OpenAI.Chat;

public class ChatCompletion : Future<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), OAI.ChatCompletion>, IFuture<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), OAI.ChatCompletion>
{
    public readonly OAI.ChatClient Client;

    public ChatCompletion(OAI.ChatClient client, CancellationToken cancellation = default) : base(cancellation)
    {
        Client = client;
        Resolver = (args) =>
        {
            var (messages, options) = args;
            var res = Client.CompleteChat(args.Item1, args.Item2 ?? options, Token);
            return res.Value;
        };
    }

    ~ChatCompletion()
    {
        Dispose();
    }

    public IFuture<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), OAI.AssistantChatMessage> Stream(Action<OAI.StreamingChatCompletionUpdate> onChunk)
    {
        return new Future<(IEnumerable<OAI.ChatMessage>, OAI.ChatCompletionOptions?), OAI.AssistantChatMessage>(args =>
        {
            var (messages, options) = args;
            var stream = Client.CompleteChatStreaming(args.Item1, args.Item2 ?? options, Token);
            var content = new List<OAI.ChatMessageContentPart>();

            foreach (var chunk in stream)
            {
                onChunk(chunk);

                foreach (var update in chunk.ContentUpdate)
                {
                    content.Add(update);
                }
            }

            return OAI.ChatMessage.CreateAssistantMessage(content);
        });
    }
}