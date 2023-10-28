namespace AJE.Infra.Redis;

public class QueryBuilder
{
    public List<QueryCondition> Conditions = new();
    public string Build()
    {
        if (Conditions.Count == 0)
            return "*";

        var sb = new StringBuilder();
        foreach (var c in Conditions)
        {
            if (sb.Length > 0)
                sb.Append($" {c.Operator.ToRedisString()} ");
            sb.Append(c.Expression);
        }
        return sb.ToString();
    }
}

public enum QueryConditionOperator
{
    And = 1,
    Or = 2,
}

public static class QueryConditionOperatorExtension
{
    public static string ToRedisString(this QueryConditionOperator qco) => qco switch
    {
        QueryConditionOperator.And => "&",
        QueryConditionOperator.Or => "|",
        _ => throw new ArgumentException("Unknown condition operator {}", qco.ToString()),
    };
}

public record QueryCondition
{
    public QueryConditionOperator Operator { get; set; } = QueryConditionOperator.And;
    public required string Expression { get; set; }
}