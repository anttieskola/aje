namespace AJE.Domain.Events;

[JsonDerivedType(typeof(AiStoryStartedEvent), "start")]
[JsonDerivedType(typeof(AiStoryTokenEvent), "token")]
[JsonDerivedType(typeof(AiStoryTitleUpdatedEvent), "title")]
[JsonDerivedType(typeof(AiStoryChapterAddedEvent), "chapterAdded")]
[JsonDerivedType(typeof(AiStoryChapterUpdateTitleEvent), "chapterTitleUpdated")]
public record AiStoryEvent
{
    [JsonPropertyName("isTest")]
    public bool IsTest { get; init; } = false;

    [JsonPropertyName("storyId")]
    public required Guid StoryId { get; init; }

    [JsonPropertyName("eventTimestamp")]
    public required DateTimeOffset EventTimeStamp { get; init; }
}

#pragma warning disable S2094
public record AiStoryStartedEvent : AiStoryEvent
{
}
#pragma warning restore S2094

public record AiStoryTokenEvent : AiStoryEvent
{
    [JsonPropertyName("token")]
    public required string Token { get; init; }
}

public record AiStoryTitleUpdatedEvent : AiStoryEvent
{
    [JsonPropertyName("title")]
    public required string Title { get; init; }
}

public record AiStoryChapterAddedEvent : AiStoryEvent
{
    [JsonPropertyName("chapterId")]
    public required Guid ChapterId { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }
}

public record AiStoryChapterUpdateTitleEvent : AiStoryEvent
{
    [JsonPropertyName("chapterId")]
    public required Guid ChapterId { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }
}

public record AiStoryChapterUpdateSummaryEvent : AiStoryEvent
{
    [JsonPropertyName("chapterId")]
    public required Guid ChapterId { get; init; }

    [JsonPropertyName("summary")]
    public required string Summary { get; init; }
}

public record AiStoryEntryAddedEvent : AiStoryEvent
{
    [JsonPropertyName("chapterId")]
    public required Guid ChapterId { get; init; }

    [JsonPropertyName("entryId")]
    public required Guid EntryId { get; init; }
}