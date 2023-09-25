namespace AJE.Infra.Index;

public class ArticleIndex
{
    private readonly IConnectionMultiplexer _connection;

    public ArticleIndex(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public async Task Initialize()
    {
        var db = _connection.GetDatabase();
        var ft = db.FT();
        var arIndexes = await ft._ListAsync();

        // index
        if (arIndexes.Any(i => i.ToString() == ArticleConstants.IndexName))
        {
            return;
        }

        // This works
        // FT.DROPINDEX idx:Test
        // FT.CREATE idx:Test ON JSON PREFIX 1 test: SCORE 1.0 SCHEMA $.id AS id TEXT WEIGHT 1.0 $.title AS title TEXT WEIGHT 1.0 $.published AS published TAG
        // note to self: Using text field with boolean breaks index from working

        // create
        var cp = new FTCreateParams().On(IndexDataType.JSON).Prefix(ArticleConstants.IndexPrefix);
        var schema = new Schema()
            .AddTextField(new FieldName("$.id", "id"))
            .AddTextField(new FieldName("$.title", "title"))
            .AddTagField(new FieldName("$.published", "published"))
            .AddTextField(new FieldName("$.content", "content"));

        await ft.CreateAsync(ArticleConstants.IndexName, cp, schema);
    }
}
