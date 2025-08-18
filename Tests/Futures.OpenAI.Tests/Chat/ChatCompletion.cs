using System.ClientModel;
using System.ClientModel.Primitives;

using Futures.OpenAI.Chat;
using Futures.Operators;

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
        var completion = Providers.From
            .Chat(_client.Object)
            .Map(res => res.Content.FirstOrDefault()?.Text);

        Assert.Equal("hi!", completion.Send("hi"));
        completion.Complete();
    }

    [Fact]
    public void Should_StoreMessages()
    {
        var messages = new List<OAI.ChatMessage>();
        var chat = Providers.From
            .Chat(_client.Object)
            .Storage(messages)
            .Map(completion => (OAI.ChatMessage.CreateAssistantMessage(completion), completion));

        var (message, completion) = chat.Send("hi");
        Assert.Equal(2, messages.Count());
        Assert.Equal("hi!", message.Content[0].Text);

        (message, completion) = chat.Complete();
        Assert.Equal(2, messages.Count());
        Assert.Equal("hi!", message.Content[0].Text);
    }
}