using Futures;
using Futures.OpenAI.Chat;

using OpenAI;

using Samples.Functions;

List<OpenAI.Chat.ChatMessage> messages = [];
var client = new OpenAIClient(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
var future = Providers.From
    .Chat(client.GetChatClient("gpt-3.5-turbo"))
    .Function<Params>(
        "increment",
        "increment a given value",
        BinaryData.FromString(@"{
            ""type"": ""object"",
            ""properties"": {
                ""value"": { ""type"": ""integer"" }
            },
            ""required"": [""value""]
        }"),
        handler: (args) => (args.Value + 1).ToString()
    )
    .Storage(messages)
    .Pipe(completion => OpenAI.Chat.ChatMessage.CreateAssistantMessage(completion))
    .Pipe(message => string.Join("", message.Content.Where(c => c.Text != string.Empty).Select(c => c.Text)));

Console.WriteLine(future.Send("hi, please increment the value 203"));
Console.WriteLine(future.Send("hi, please increment the value 500"));
Console.WriteLine($"done! {messages.Count}");