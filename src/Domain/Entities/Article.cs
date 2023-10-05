﻿namespace AJE.Domain.Entities;

public static class ArticleConstants
{
    public static readonly string INDEX_NAME = "idx:article";
    public static readonly string INDEX_PREFIX = "article:";
    public static readonly string CHANNEL = "articles";
}

public enum ArticleCategory
{
    BOGUS = 1,
    NEWS = 2,
};

public class ArticleHeader
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("category")]
    public ArticleCategory Category { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("modified")]
    public long Modified { get; set; }

    [JsonPropertyName("published")]
    public bool Published { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    [JsonPropertyName("language")]
    public string Language { get; set; } = string.Empty;
}

public class Article : ArticleHeader
{
    [JsonPropertyName("content")]
    public IEnumerable<MarkdownElement> Content { get; set; } = Array.Empty<MarkdownElement>();

    [JsonPropertyName("chat")]
    public IEnumerable<ChatMessage> Chat { get; set; } = Array.Empty<ChatMessage>();
}
