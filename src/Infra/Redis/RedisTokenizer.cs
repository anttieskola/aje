namespace AJE.Infra.Redis;

public static class RedisTokenizer
{
    // tested with ft.search / ft.index
    // https://redis.io/docs/interact/search-and-query/advanced-concepts/escaping/
    // + '/' + '?'
    private const string _escapeCharacters = ",.<>{}[]\"':;!@#$%^&*()-+=~/?";

    public static string RedisEscape(this string str)
    {
        var sb = new StringBuilder();
        foreach (var c in str)
        {
            if (_escapeCharacters.Contains(c))
            {
                sb.Append('\\');
            }
            sb.Append(c);
        }
        return sb.ToString();
    }
}