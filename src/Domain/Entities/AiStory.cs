namespace AJE.Domain.Entities;

public record AiStory
{
    [JsonPropertyName("storyId")]
    public Guid StoryId { get; set; }

    [JsonPropertyName("settings")]
    public AiSettings Settings { get; set; } = new();

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("systemInstructions")]
    public EquatableList<string> SystemInstructions { get; set; } = [];

    [JsonPropertyName("chapters")]
    public EquatableList<AiStoryChapter> Chapters { get; set; } = [];
}

public record AiStoryChapter
{
    [JsonPropertyName("chapterId")]
    public Guid ChapterId { get; set; }

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("entries")]
    public EquatableList<AiStoryEntry> Entries { get; set; } = [];
}

public record AiStoryEntry
{
    [JsonPropertyName("entryId")]
    public Guid EntryId { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}
