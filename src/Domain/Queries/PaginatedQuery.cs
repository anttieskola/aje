﻿namespace AJE.Domain.Queries;

public record PaginatedQuery
{
    public int Offset { get; init; }
    public int PageSize { get; init; }
}
