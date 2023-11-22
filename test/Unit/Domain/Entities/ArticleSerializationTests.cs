using System.Text.Json;
using AJE.Domain.Entities;
using AJE.Domain.Enums;

namespace AJE.Test.Unit.Domain.Entities;

public class ArticleSerializationTests
{
    #region article

    [Fact]
    public void ArticleWithContentTypes()
    {
        var original = new Article
        {
            Id = Guid.Parse("12300000-1200-1200-1200-000000000034"),
            Category = Category.BOGUS,
            Title = "AJE is born",
            Modified = new DateTime(1980, 9, 12, 12, 00, 12, DateTimeKind.Utc).Ticks,
            Published = true,
            Source = "https://www.anttieskola.com",
            Language = "en",
            Content = new EquatableList<MarkdownElement>
            {
                new MarkdownHeaderElement{
                    Level = 1,
                    Text = "This is header 1"
                },
                new MarkdownTextElement{
                    Text = "This is a paragraph"
                },
                new MarkdownHeaderElement{
                    Level = 2,
                    Text = "This is header 2"
                },
                new MarkdownTextElement{
                    Text = "This is another paragraph"
                },
            },
        };

        var json = JsonSerializer.Serialize(original);
        var copy = JsonSerializer.Deserialize<Article>(json);

        Assert.NotNull(copy);
        Assert.Equal(original.Id, copy.Id);
        Assert.Equal(original.Category, copy.Category);
        Assert.Equal(original.Title, copy.Title);
        Assert.Equal(original.Modified, copy.Modified);
        Assert.Equal(original.Published, copy.Published);
        Assert.Equal(original.Source, copy.Source);
        Assert.Equal(original.Language, copy.Language);
        Assert.Equal(original.Content.Count, copy.Content.Count);

        var h1 = copy.Content[0] as MarkdownHeaderElement;
        Assert.NotNull(h1);
        Assert.Equal(1, h1.Level);
        Assert.Equal("This is header 1", h1.Text);

        var p1 = copy.Content[1] as MarkdownTextElement;
        Assert.NotNull(p1);
        Assert.Equal("This is a paragraph", p1.Text);

        var h2 = copy.Content[2] as MarkdownHeaderElement;
        Assert.NotNull(h2);
        Assert.Equal(2, h2.Level);
        Assert.Equal("This is header 2", h2.Text);

        var p2 = copy.Content[3] as MarkdownTextElement;
        Assert.NotNull(p2);
        Assert.Equal("This is another paragraph", p2.Text);
    }

    #endregion article

    #region completion

    private readonly string _completionResponse = @"
{
    ""content"": ""RESPONSE_CONTENT"",
    ""generation_settings"": {
        ""frequency_penalty"": 0.0,
        ""grammar"": """",
        ""ignore_eos"": false,
        ""logit_bias"": [],
        ""mirostat"": 0,
        ""mirostat_eta"": 0.10000000149011612,
        ""mirostat_tau"": 5.0,
        ""model"": ""/home/models/Mistral-7B-OpenOrca-GGUF/mistral-7b-openorca.Q5_K_M.gguf"",
        ""n_ctx"": 16384,
        ""n_keep"": 0,
        ""n_predict"": 2048,
        ""n_probs"": 0,
        ""penalize_nl"": true,
        ""presence_penalty"": 0.0,
        ""repeat_last_n"": 64,
        ""repeat_penalty"": 1.100000023841858,
        ""seed"": 4294967295,
        ""stop"": [],
        ""stream"": false,
        ""temp"": 0.800000011920929,
        ""tfs_z"": 1.0,
        ""top_k"": 40,
        ""top_p"": 0.949999988079071,
        ""typical_p"": 1.0
    },
    ""model"": ""/home/models/Mistral-7B-OpenOrca-GGUF/mistral-7b-openorca.Q5_K_M.gguf"",
    ""prompt"": ""PROMPT_CONTENT"",
    ""stop"": true,
    ""stopped_eos"": true,
    ""stopped_limit"": false,
    ""stopped_word"": false,
    ""stopping_word"": """",
    ""timings"": {
        ""predicted_ms"": 5172.194,
        ""predicted_n"": 230,
        ""predicted_per_second"": 44.46855628385168,
        ""predicted_per_token_ms"": 22.487800000000004,
        ""prompt_ms"": 513.524,
        ""prompt_n"": 322,
        ""prompt_per_second"": 627.0398267656428,
        ""prompt_per_token_ms"": 1.5947950310559007
    },
    ""tokens_cached"": 552,
    ""tokens_evaluated"": 322,
    ""tokens_predicted"": 230,
    ""truncated"": false
}
    ";

    [Fact]
    public void CompletionResponse()
    {
        var response = JsonSerializer.Deserialize<CompletionResponse>(_completionResponse);
        Assert.NotNull(response);
    }

    #endregion completion
}
