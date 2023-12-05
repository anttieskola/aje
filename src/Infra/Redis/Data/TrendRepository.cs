using AJE.Domain;

namespace AJE.Infra.Redis.Data;

public class TrendRepository : ITrendRepository
{
    private readonly IRedisIndex _index = new ArticleIndex();
    private readonly ILogger<TrendRepository> _logger;
    private readonly IConnectionMultiplexer _connection;

    public TrendRepository(
        ILogger<TrendRepository> logger,
        IConnectionMultiplexer connection)
    {
        _logger = logger;
        _connection = connection;
    }

    public async Task<NewsPolarityTrendSegment[]> GetArticleSentimentPolarityTrendsAsync(GetArticleSentimentPolarityTrendsQuery query, CancellationToken cancellationToken)
    {
        // Adjust start time
        var queryStart = query.Start;
        if (query.TimePeriod != TimePeriod.Hour)
        {
            // reset time to 00:00
            queryStart = queryStart.Date;
        }
        else
        {
            // reset time to the current hour
            queryStart = new DateTimeOffset(queryStart.Year, queryStart.Month, queryStart.Day, queryStart.Hour, 0, 0, TimeSpan.Zero);
        }

        // query items
        var items = await GetNewsPolarityTrendItemsForPeriod(query.ArticleCategory, queryStart.Ticks, query.End.Ticks);

        // create segments and split items into them
        var segments = new List<NewsPolarityTrendSegment>();
        var start = queryStart;
        var end = queryStart.AddPeriod(query.TimePeriod);
        while (true)
        {
            var segment = new NewsPolarityTrendSegment
            {
                TimePeriod = query.TimePeriod,
                Start = start,
                End = end,
                Items = new EquatableList<NewsPolarityTrendItem>()
            };

            foreach (var item in items.Where(x => x.Published >= start && x.Published < end))
                segment.Items.Add(item);

            segments.Add(segment);

            // note we want to build segments to the defined end of the query
            if (end >= query.End)
                break;
            else
            {
                start = end;
                end = end.AddPeriod(query.TimePeriod);
            }
        }
        return segments.ToArray();
    }


    private async Task<List<NewsPolarityTrendItem>> GetNewsPolarityTrendItemsForPeriod(ArticleCategory category, long start, long end)
    {
        var db = _connection.GetDatabase();
        var items = new List<NewsPolarityTrendItem>();
        var offset = 0;
        while (true)
        {
            var arguments = new string[]
            {
                _index.Name,
                $"@category:[{(int)category} {(int)category}]",
                "FILTER",
                "modified",
                start.ToString(),
                end.ToString(),
                "SORTBY",
                "modified",
                "ASC",
                "RETURN",
                "5",
                "$.id",
                "$.title",
                "$.modified",
                "$.polarity",
                "$.polarityVersion",
                "LIMIT",
                offset.ToString(),
                "1000"
            };
            var result = await db.ExecuteAsync("FT.SEARCH", arguments);
            // first item is total count (integer)
            // then pairs of key (bulk string) and value (multibulk)
            // inside value we have: modified, modified-value, json path, json-value
            var rows = (RedisResult[])result!;
            var totalCount = (long)rows[0];
            for (long i = 1; i < rows.LongLength; i += 2)
            {
                var data = (RedisResult[])rows[i + 1]!;
                Guid id;
                string title;
                DateTimeOffset published;
                Polarity polarity;
                int polarityVersion;
                if (data.Length == 10)
                {
                    var idString = data[1]! ?? throw new DataException($"invalid data value in key {rows[i]}");
                    id = Guid.Parse(idString.ToString()!);
                    title = data[3]!?.ToString() ?? throw new DataException($"invalid data value in key {rows[i]}");
                    var utcTicks = long.Parse(data[5]!?.ToString() ?? throw new DataException($"invalid data value in key {rows[i]}"));
                    published = new DateTimeOffset(new DateTime(utcTicks, DateTimeKind.Utc));
                    var idPolarity = (int?)data[7]! ?? throw new DataException($"invalid data value in key {rows[i]}");
                    polarity = (Polarity)idPolarity;
                    polarityVersion = (int?)data[9]! ?? throw new DataException($"invalid data value in key {rows[i]}");
                    items.Add(new NewsPolarityTrendItem { Id = id, Title = title, Published = published, Polarity = polarity, PolarityVersion = polarityVersion });
                }
                else
                {
                    _logger.LogError("invalid data value in key:{}", rows[i]);
                    _logger.LogError("invalid data Length:{}", data.Length);
                    for (var e = 0; e < data.Length; e++)
                        _logger.LogError("invalid data Index:{} Item:{}", e, data[e]);

                    throw new DataException($"invalid data value in key {rows[i]}"); // here it crashes
                }
            }
            if (items.Count == totalCount)
                break;
            else
                offset += 1000;
        }
        return items;
    }
}
