using Futures.Operators;

namespace OpenAI.Futures.Tests;

public class ChatCompletionTests
{
    [Fact]
    public void Should_CreateFuture()
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        var client = new OpenAIClient(apiKey);
        var completion = new ChatCompletion(client.GetChatClient("gpt-3.5-turbo"))
            .Pipe(res => res.Content.FirstOrDefault()?.Text);

        completion.Next("hi");
        completion.Complete();
        Console.WriteLine(completion.Resolve());

        // var response = new Response(client.GetOpenAIResponseClient(""))
        //     .Function("b", "b function", BinaryData.FromString("{}"))
        //     .Pipe(res => res.OutputItems.FirstOrDefault()?.ToString());

        // completion.Pipe(text => response.Next(new()));
    }
}