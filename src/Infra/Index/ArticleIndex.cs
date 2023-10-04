namespace AJE.Infra.Index;

public class ArticleIndex
{
    private readonly ILogger<ArticleIndex> _logger;
    private readonly IConnectionMultiplexer _connection;

    public ArticleIndex(
        ILogger<ArticleIndex> logger,
        IConnectionMultiplexer connection)
    {
        _logger = logger;
        _connection = connection;
    }

    public async Task Initialize()
    {
        var ft = _connection.GetDatabase().FT();
        var arIndexes = await ft._ListAsync();

        if (arIndexes.Any(i => i.ToString() == ArticleConstants.INDEX_NAME))
        {
            _logger.LogDebug("Index already exists");
            return;
        }

        var cp = new FTCreateParams()
            .On(IndexDataType.JSON)
            .Prefix(ArticleConstants.INDEX_PREFIX);

        var schema = new Schema()
            .AddTagField(new FieldName("$.id", "id"))
            .AddTextField(new FieldName("$.title", "title"))
            .AddTagField(new FieldName("$.source", "source"))
            .AddTagField(new FieldName("$.published", "published"));

        await ft.CreateAsync(ArticleConstants.INDEX_NAME, cp, schema);
        _logger.LogDebug("Index {indexName} created", ArticleConstants.INDEX_NAME);
    }
}
