#pragma warning disable OPENAI001

namespace OpenAI.Futures.Tests;

public class ChatCompletionTests
{
    [Fact]
    public void Should_CreateFuture()
    {
        var client = new OpenAIClient("");
        var completion = new ChatCompletion(client.GetChatClient(""))
            .Function("test", "a test function", BinaryData.FromString("{}"))
            .Pipe(res => res.Content.FirstOrDefault()?.Text);

        var response = new Response(client.GetOpenAIResponseClient(""))
            .Function("b", "b function", BinaryData.FromString("{}"))
            .Pipe(res => res.OutputItems.FirstOrDefault()?.ToString());

        completion.Pipe(text => response.Next(new()));
    }
}