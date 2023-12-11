
# Redis

## Funky
- When reloading now over 7k articles...
- Afterwards starting like token counter it finds few articles then stops
- Have to drop index, remake to make it work again
- Should investigate more [from module homepage](https://github.com/RediSearch)
- Lots of time I been looking issues that are just by fixed by re-creating index...

We could program functionality that drops index after reloading articles, recreate it
and query info until it shows its 100% ready.

## Helpful commands

```bash
# Delete all keys with pattern "article:*"
./redis-cli --scan --pattern "article:*" | xargs ./redis-cli del
```

- KEYS List all keys
- FT._LIST List all search indexes
- FT.EXPLAIN -> explains query, very good for learning

## Executing commands with StackExchange.Redis
This took an hour to figure out...

```csharp
// When executing command with arguments in List
var arguments = new List<string> { "idx:article", "*", "NOCONTENT" };
var resException = await db.ExecuteAsync("FT.SEARCH", arguments);
// Will result in RedisTimeoutException

// But if you execute command with array of arguments
var resOk = await db.ExecuteAsync("FT.SEARCH", arguments.ToArray());
// All will work fine
```

## indexes
- Numeric data fields can't be made tag, index breaks and nothing is found
- When we add fields to model's/indexe's redis data has to be updated or reloaded even if json has default value
but the field is missing in redis data of key and query won't work
- When querying index and ***even when defining return column*** and that column does not exists on object
***it won't return missing columns at all*** so returned data size can change depending on what columns exists on object
- When creating index, it is usable right away but using it before it is fully created will result in partial results

## Guid/Url type of columns in index
So I had added stuff with bogus to redis and was trying to search does any article have source
url `https://rosetta.net/decentralized/human/refined-granite-shoes`. So the source field was added
to the index as text field.

Studied [tokenization](https://redis.io/docs/interact/search-and-query/advanced-concepts/escaping/) on characters
that gotta be escaped. This let me to a point where I thought it should find the one article having that source value
but always end up getting 0 results. The explain from the query looked fine:
- `"@source:https://rosetta.net/decentralized/human/refined-granite-shoes\n"`

Then I Tried the same using the GUID values and that end up with same conclusion, search looks fine:
But it won't match...
- `"@id:b1653517-25f2-4c82-87b5-f610e7fd6dbc\n"`

But when I switched the field types to tag I could match the exact value. But all could been easier if I had read the
documentation better :) But this was good practice

- TEXT - Allows full-text search queries against the value in this attribute.
- TAG - Allows ***exact-match queries***, such as categories or primary keys, against the value in this attribute. For more information, see Tag Fields.

As Text -> naah
```
FT.DROPINDEX idx:article
FT.CREATE "idx:article" ON JSON PREFIX 1 article: SCORE 1.0 SCHEMA $.id AS id TEXT WEIGHT 1.0
FT.SEARCH "idx:article" "*"
FT.SEARCH "idx:article" "@id:\"b1653517\\-25f2\\-4c82\\-87b5\\-f610e7fd6dbc\""
# explain: "@id:b1653517-25f2-4c82-87b5-f610e7fd6dbc\n"
```

As Tag -> works
```
FT.DROPINDEX idx:article
FT.CREATE "idx:article" ON JSON PREFIX 1 article: SCORE 1.0 SCHEMA $.id AS id TAG
FT.SEARCH "idx:article" "*"
FT.SEARCH "idx:article" "@id:{b1653517\\-25f2\\-4c82\\-87b5\\-f610e7fd6dbc}"
# explain: "TAG:@id {\n  b1653517-25f2-4c82-87b5-f610e7fd6dbc\n}\n"
```

So after reading documentation better and practicing yeah tag is the correct type for these.
Maybe if some point I would like to search articles where source is some specific domain I need to add it as text...

Can I add same field twice? Yes :)

```
FT.DROPINDEX idx:article
FT.CREATE "idx:article" ON JSON PREFIX 1 article: SCORE 1.0 SCHEMA $.id AS id TAG $.source as source TAG $.source as sourcetext TEXT WEIGHT 1.0

# ok... maybe not sure really as '.' means
FT.SEARCH "idx:article" "@sourcetext:rosetta.net"
# explain: "@sourcetext:INTERSECT {\n  @sourcetext:UNION {\n    @sourcetext:rosetta\n    @sourcetext:+rosetta(expanded)\n  }\n  @sourcetext:UNION {\n    @sourcetext:net\n    @sourcetext:+net(expanded)\n  }\n}\n"

# ok... correct way?
FT.SEARCH "idx:article" "@sourcetext:rosetta\.net"
# explain: "@sourcetext:INTERSECT {\n  @sourcetext:UNION {\n    @sourcetext:rosetta\n    @sourcetext:+rosetta(expanded)\n  }\n  @sourcetext:UNION {\n    @sourcetext:net\n    @sourcetext:+net(expanded)\n  }\n}\n"

# not finding
FT.SEARCH "idx:article" "@sourcetext:rosetta\\.net"
# explain: "@sourcetext:UNION {\n  @sourcetext:rosetta.net\n  @sourcetext:+rosetta.net(expanded)\n}\n"

```

## Index
Example
```
FT.CREATE idxArticle ON JSON PREFIX 1 article: SCORE 1.0 SCHEMA $.id AS id TEXT WEIGHT 1.0 $.title AS title TEXT WEIGHT 1.0 $.source AS source TEXT WEIGHT 1.0 $.published AS published TAG
```

# Random things learned along the way
- It really is impossible to run any blazor application in subfolder/virtual folder
	- Issue is that I found now way to configure the dll's generated blazor.server.js to be found
	- I exhausted myself using AI and internet for around 10 hours to find a solution
- dotnet... i wanted to use 8 but not ready so 7 it is
	- Experienced issues with 8.0 preview
	- Using .net 7.0, because tried two hours to run [sample codes](https://github.com/dotnet/blazor-samples.git)
	with the 8.0 preview template. Not sure did I break template but all worked with 7.
- redis can be integrated directly [read the documentation](https://learn.microsoft.com/en-us/aspnet/core/signalr/redis-backplane?view=aspnetcore-7.0)
	- package [Microsoft.AspNetCore.SignalR.StackExchangeRedis](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.signalr.stackexchangeredis?view=aspnetcore-7.0)
	- package kinda does same thing I had in mind to relay messages
	- still if I want to create let's say some rust app that produces data don't think I can send data directly to these channels
	  as they are generated as needed and follow own naming conventions and message formats
		- so I can send to my custom channel data from the rust app that something like the ChatRelayService then send into signal-r
		- ofc the setup using redis with signal-r I can reach all servers directly
- each signal-r hub has its own websocket connection (pre .net 4.7 there was only one connection for all hubs)
	- there is limitation on number of users single server can handle
- azure signal-r service moves the socket load from server to service (the selling point of the product)
