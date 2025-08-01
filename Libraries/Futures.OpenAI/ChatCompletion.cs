using System.Text;

using Chat = OpenAI.Chat;

namespace Futures.OpenAI;

public class ChatCompletion : Future<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), Chat.ChatCompletion>, IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), Chat.ChatCompletion>
{
    public readonly Chat.ChatClient Client;

    public ChatCompletion(Chat.ChatClient client, CancellationToken cancellation = default) : base(cancellation)
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

    public IFuture<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), Chat.AssistantChatMessage> Stream(Action<Chat.StreamingChatCompletionUpdate> onChunk)
    {
        return new Future<(IEnumerable<Chat.ChatMessage>, Chat.ChatCompletionOptions?), Chat.AssistantChatMessage>(args =>
        {
            var (messages, options) = args;
            var stream = Client.CompleteChatStreaming(args.Item1, args.Item2 ?? options, Token);
            var content = new List<Chat.ChatMessageContentPart>();

            foreach (var chunk in stream)
            {
                onChunk(chunk);

                foreach (var update in chunk.ContentUpdate)
                {
                    content.Add(update);
                }
            }

            return Chat.ChatMessage.CreateAssistantMessage(content);
        });
    }
}