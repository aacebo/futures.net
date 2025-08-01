using System.ClientModel;
using System.ClientModel.Primitives;

using Futures.OpenAI.Chat;

using Moq;

using OAI = OpenAI.Chat;

namespace Futures.OpenAI.Tests.Chat;

public class ChatCompletionTests
{
    private readonly Mock<OAI.ChatClient> _client;

    public ChatCompletionTests()
    {
        _client = new Mock<OAI.ChatClient>();
        _client.Setup(client =>
            client.CompleteChat(
                It.IsAny<IEnumerable<OAI.ChatMessage>>(),
                It.IsAny<OAI.ChatCompletionOptions?>(),
                It.IsAny<CancellationToken>()
            )
        ).Returns(ClientResult.FromValue(
            OAI.OpenAIChatModelFactory.ChatCompletion(
                role: OAI.ChatMessageRole.Assistant,
                content: new OAI.ChatMessageContent("hi!")
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
        var chat = new ChatCompletion(_client.Object).Storage();

        chat.Next("hi");
        var (messages, message, _) = chat.Complete();
        Assert.Equal(2, messages.Count());
        Assert.Equal("hi!", message.Content[0].Text);
    }
}