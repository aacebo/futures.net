using System.ClientModel;
using System.ClientModel.Primitives;

using Moq;

using Chat = OpenAI.Chat;

namespace Futures.OpenAI.Tests;

public class ChatCompletionTests
{
    private readonly Mock<Chat.ChatClient> _client;

    public ChatCompletionTests()
    {
        _client = new Mock<Chat.ChatClient>();
        _client.Setup(client =>
            client.CompleteChatAsync(
                It.IsAny<IEnumerable<Chat.ChatMessage>>(),
                It.IsAny<Chat.ChatCompletionOptions?>(),
                It.IsAny<CancellationToken>()
            )
        ).ReturnsAsync(ClientResult.FromValue(
            Chat.OpenAIChatModelFactory.ChatCompletion(
                role: Chat.ChatMessageRole.Assistant,
                content: new Chat.ChatMessageContent("hi!")
            ),
            Mock.Of<PipelineResponse>()
        ));
    }

    [Fact]
    public void Should_CreateFuture()
    {
        var completion = new ChatCompletion(_client.Object)
            .Pipe(res => res.Content.FirstOrDefault()?.Text);

        Assert.Equal("hi!", completion.Next("hi"));
        completion.Complete();
    }

    [Fact]
    public void Should_StoreMessages()
    {
        var chat = new ChatCompletion(_client.Object)
            .AutoMessageStorage();

        chat.Next("hi");
        var (messages, message, _) = chat.Complete();
        Assert.Equal(2, messages.Count());
        Assert.Equal("hi!", message.Content[0].Text);
    }
}