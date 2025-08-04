using OpenAI.Chat;

namespace Futures.OpenAI.Chat.Streaming;

public class CompletionBuilder
{
    public string? Id;
    public string? Model;
    public ChatMessageRole? Role;
    public string? Refusal;
    public ChatFinishReason? FinishReason;
    public DateTimeOffset? CreatedAt;
    public ChatTokenUsage? Usage;
    public readonly List<ChatMessageContentPart> Content = [];
    public readonly ToolCallsBuilder ToolCalls = new();
    public readonly List<ChatTokenLogProbabilityDetails> ContentTokenLogProbabilities = [];
    public readonly List<ChatTokenLogProbabilityDetails> RefusalTokenLogProbabilities = [];

    public void Append(StreamingChatCompletionUpdate update)
    {
        Id ??= update.CompletionId;
        Model ??= update.Model;
        Role ??= update.Role;
        Refusal ??= update.RefusalUpdate;
        FinishReason ??= update.FinishReason;
        CreatedAt ??= update.CreatedAt;
        Usage ??= update.Usage;
        ContentTokenLogProbabilities.AddRange(update.ContentTokenLogProbabilities);
        RefusalTokenLogProbabilities.AddRange(update.RefusalTokenLogProbabilities);
        Content.AddRange(update.ContentUpdate);
        ToolCalls.AddRange(update.ToolCallUpdates);
    }

    public void AddRange(IEnumerable<StreamingChatCompletionUpdate> updates)
    {
        foreach (var update in updates)
        {
            Append(update);
        }
    }

    public ChatCompletion Build()
    {
        return OpenAIChatModelFactory.ChatCompletion(
            id: Id,
            model: Model,
            role: Role ?? throw new InvalidDataException("Role not found"),
            refusal: Refusal,
            finishReason: FinishReason ?? throw new InvalidDataException("FinishReason not found"),
            createdAt: CreatedAt ?? DateTimeOffset.UtcNow,
            usage: Usage,
            contentTokenLogProbabilities: ContentTokenLogProbabilities,
            refusalTokenLogProbabilities: RefusalTokenLogProbabilities,
            content: new(Content),
            toolCalls: ToolCalls.Build()
        );
    }
}