using System.Text.Json;

using OpenAI;

namespace Futures.OpenAI.Tests;

public class ChatCompletionTests
{
    private readonly OpenAIClient _client;

    public ChatCompletionTests()
    {
        _client = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
    }

    [Fact]
    public void Should_CreateFuture()
    {
        var completion = new ChatCompletion(_client.GetChatClient("gpt-3.5-turbo"))
            .Pipe(res => res.Content.FirstOrDefault()?.Text);

        completion.Next("hi");
        completion.Complete();
    }

    [Fact]
    public void Should_StoreMessages()
    {
        var chat = new ChatCompletion(_client.GetChatClient("gpt-3.5-turbo"))
            .AutoMessageStorage();

        chat.Next("hi");
        var (messages, _, _) = chat.Complete();
        Assert.Equal(2, messages.Count());
    }
}