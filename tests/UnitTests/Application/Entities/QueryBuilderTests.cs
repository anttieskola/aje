using AJE.Application.Entities;

namespace AJE.UnitTests;

public class QueryBuilderTests
{
    [Fact]
    public void Empty()
    {
        var builder = new QueryBuilder();
        var query = builder.Build();
        Assert.Equal("*", query);
    }

    [Fact]
    public void Single()
    {
        var builder = new QueryBuilder();
        builder.Conditions.Add(new QueryCondition { Expression = "foo" });
        var query = builder.Build();
        Assert.Equal("foo", query);

        builder = new QueryBuilder();
        builder.Conditions.Add(new QueryCondition { Operator = QueryConditionOperator.Or, Expression = "foo" });
        query = builder.Build();
        Assert.Equal("foo", query);
    }

    [Fact]
    public void AndPair()
    {
        var builder = new QueryBuilder();
        builder.Conditions.Add(new QueryCondition { Expression = "foo" });
        builder.Conditions.Add(new QueryCondition { Expression = "bar" });
        var query = builder.Build();
        Assert.Equal("foo & bar", query);
    }

    [Fact]
    public void OrPair()
    {
        var builder = new QueryBuilder();
        builder.Conditions.Add(new QueryCondition { Operator = QueryConditionOperator.Or, Expression = "foo" });
        builder.Conditions.Add(new QueryCondition { Operator = QueryConditionOperator.Or, Expression = "bar" });
        var query = builder.Build();
        Assert.Equal("foo | bar", query);
    }

    [Fact]
    public void AndsOrs()
    {
        var builder = new QueryBuilder();
        builder.Conditions.Add(new QueryCondition { Expression = "@field1:{v1}" });
        builder.Conditions.Add(new QueryCondition { Operator = QueryConditionOperator.Or, Expression = "@field2:{v2}" });
        builder.Conditions.Add(new QueryCondition { Operator = QueryConditionOperator.Or, Expression = "@field3:{v3}" });
        builder.Conditions.Add(new QueryCondition { Expression = "@field4:{v4}" });
        var query = builder.Build();
        Assert.Equal("@field1:{v1} | @field2:{v2} | @field3:{v3} & @field4:{v4}", query);
    }
}
