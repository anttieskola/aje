﻿namespace AJE.Infra.Indexes;

public interface IRedisIndex
{
    string Name { get; }
    string Prefix { get; }
    string RedisId(string itemId);
    string Schema { get; }
    RedisChannel Channel { get; }
}