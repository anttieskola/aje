# Background / Goal
Project started with learning to create an application to modify my notes that are
stored in Github as markdown and same time create homepage for myself. Idea was to
basically copy functionality what Github already offers and learning to use few selected
technologies better. The editing of articles is suppose to support up to one thousand users
editing same time by leveraging signal-r communication that is extended to support multiple
host front end nodes with Redis.

This has turned now to become a simple news downloader (News microservice). So currently
It download all YLE news articles as they release them, convert them little bit into simple
markdown article format. Then NewsAnalyze microservice feeds them to Mistral AI model
running with llama.cpp for classification. I am running the models with my Nvidia
GTX 3060 & 3080 gaming cards.

Idea is to just classify articles so I can browse only positive news :)

Other thing I am currently working is to write very good instructions for model to
mimic myself. It seems to be really fun to interract this way. Maybe I get the instructions
good enough that I can use models with instructions to write some job applications.

After I learn to create good enough instructions I want to learn to fine-tune models
and start creating a data-set to use in fine-tuning these awesome open source models.

## Current todo list
- Improve UI for news browsing
- Improve the AI instructions for news classification and could try more different models
	- Maybe use multiple sets of instructions for different classifications?
- Statistic from the news, I want to see graph showing me for many positive news articles
are released daily. Vision to see history graph and some way to dig thru the graph aswell
into actual articles.
- Chat with me feature using large set of instructions and good model like Mistral that I
currently use

# Code architecture
All is done using clean architecture / "domain based" programming. Most of all I want to
thank [Jason Taylor](https://github.com/jasontaylordev/CleanArchitecture) for really great
template from which I have copied most of structure for this project.

Using Command/Query separation pattern with [MediatR](https://github.com/jbogard/MediatR).
Even my domain has dependency on MediatR as it is essential for the architecture.

## Libraries
- Domain the "holy grail"
	- This has dependency on MediaRt & Dependency injection to be able to use the command &
	query architecture
	- ***No other dependencies should be added***
- Application higher level implementations... kinda empty still as many things implemented
features are placed currently in the component that is kinda wrong way and should be fixed
- Infra, folder that contains shared infrastructure implementations
	- AI api to llama server running models
	- Redis our shared data storage and messaging
- Each component should have their own infrastructure library when they need (database as an example)

# Components
Each component is true microservice so they work on their own. What they do share is
Redis so that data is shared between them, but note that ***Redis data is not permanent*** and
components need to be able to build up Redis data from scratch. Communication between components
happens using events send in Redis channels.

List of current components and used software
- Microservices
	- [News](./doc/News.md) Downloads news from internet, convert and store them into Redis
	- [NewsAnalyzer](./doc/NewsAnalyzer.md) Classifies news articles with large language models
- [Ui.Public](./doc/UiPublic.md) User interface for the application
	- Using server rendered blazor as I want to learn to use it better and I think it is
	fits my needs best
- Utilities (under Tests currently)
	- MChatter: Chat application with large language model
	- Populator: Utility to fill Redis with bogus data
- All code I write in C# (might do some in rust as went thrue the [book](https://doc.rust-lang.org/book/) a while ago)
	- Currently using .Net 7.0 as had issues with 8.0 RC with blazor
- [Llama.cpp](https://github.com/ggerganov/llama.cpp) server
	- Used to run large language models locally
	- Models I use I get from [Hugginface](https://huggingface.co/)
- [Nginx](https://nginx.org/en/) as front-end/proxy for Asp.Net applications
- [Redis](https://redis.io/) for data storage and messaging. Modules I use:
	- [RedisJSON](https://github.com/RedisJSON/RedisJSON) JSON support
	- [RediSearch](https://github.com/RediSearch/RediSearch) Search/indexing
- [Postgresql](https://www.postgresql.org/) for databases
- [Tailwind](https://github.com/tailwindlabs/tailwindcss) for UI styling
- [Debian](https://www.debian.org/)
	- Platform for everything
	- Currently running version 12 (bookworm)
	- [My servers setup](https://github.com/anttieskola/setup)

## Deployment locations
- Web.Ui published on server into path `/usr/local/bin/AJE.Ui.Public/`
- News published on server into path `/usr/local/bin/AJE.Service.News/`
- NewsAnalyzer published on server into path `/usr/local/bin/AJE.Service.NewsAnalyzer/`
- Llama.cpp is installed manually on server into path `/usr/local/bin/Llama/`
- Redis is installed manually on server into path `/usr/local/bin/Redis/`
	- Modules have own folders under `/usr/local/bin`
- Postgresql is installed using their apt package repository

Could someday learn to create docker containers and make one for each component.

## Folder paths
- `/var/aje/yle` is used to store raw YLE news downloads
- `/var/aje/ai` is used to dump AI model input & outputs that require inspection

## Databases (postgresql)
- [installation](https://wiki.debian.org/PostgreSql)

## Systemd
- Each component is their own service
- Service installation instructions are found in [service definition](./aje.service)
	- [Debian systemd](https://wiki.debian.org/systemd/Services)
	- Running as my normal user for now

## Nginx configuration
- Asp.net process is running in the default port 5000
	- Change to some specific?
- Here is the nginx proxy configuration
	- It is running in root path, if path changed need to fix in code too as resource paths will not work
- These settings are important as blazor runs using websockets
	- Works only partially without
- [WebSocket documentation](https://www.nginx.com/blog/websocket-nginx/)

Configuration (sniplet just is the ssl configuration)
```
server {
        listen 443 ssl default_server;
        listen [::]:443 ssl default_server;
        include snippets/anttieskola.conf;
        root /var/www/html;
        index index.html;
        server_name _;
        location / {
                proxy_pass http://127.0.0.1:5000;
                proxy_http_version 1.1;
                proxy_set_header Upgrade $http_upgrade;
                proxy_set_header Connection "Upgrade";
                proxy_set_header Host $host;
        }
		...
}
```
# Links
- [this repository](https://github.com/anttieskola/aje)
- [redis commands](https://redis.io/commands/)
- [ui-lifecycle](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/lifecycle?view=aspnetcore-7.0)
- [blazor forms](https://learn.microsoft.com/en-us/aspnet/core/blazor/forms-and-input-components?view=aspnetcore-3.1)
- [signal-r](https://learn.microsoft.com/en-us/aspnet/core/signalr/hubs?view=aspnetcore-7.0)
- [IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory)

# Redis
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
***it won't return missing columns at all*** so returned data size can change depending on what columns exists
on object

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

# making C# solution in linux

```bash
# creates new component with the name of directory
dotnet new [template]

# creates solution
dotnet new sln
# add project to solution
dotnet sln add
```

using templates:
- classlib
- blazorserver
- worker
- grpc
- sln

```xml
  <PropertyGroup>
    <RootNamespace>AJE.Application</RootNamespace>
    <AssemblyName>AJE.Application</AssemblyName>
  </PropertyGroup>
```
