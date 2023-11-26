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
- Sonarqube analysis fixes should be done
- Chat with me feature (Antai)
	- UI skeleton is done (no integration to backend yeat)
	- Backend working in integration tests
	- Should add service to listen to chat events to save them into database?
		- Events contain all information
		- Chats are saved into Redis atm
- Statistic from the news, I want to see graph showing me for many positive news articles
are released daily. Vision to see history graph and some way to dig thru the graph aswell
into actual articles
- Create Eduskunta RSS feed downloader and analyzer
- Buy subscriptions to Helsinki Sanomat, Kaleva, Kauppalehti, YCombinator... any that
  provide RSS feeds or similar ways to track new articles

# Code architecture
All is done using clean architecture / "domain based" programming. Most of all I want to
thank [Jason Taylor](https://github.com/jasontaylordev/CleanArchitecture) for really great
template from which I have copied most of structure for this project.

Using Command/Query separation pattern with [MediatR](https://github.com/jbogard/MediatR).
Even my domain has dependency on MediatR as it is essential for the architecture.

MediatR has serious flaw that it is impossible to select which handlers you want to register
this has lead me to write dummies into web application that I don't want any direct use to
my GPU's. I might have to fork/modify MediaRT to support selecting specific handlers a good
idea I found on forums is to use attributes. Current author is not interested at all to allow
any kind of shenanigans like this. He will reject any propositions.

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
	- [NewsDownloader](./doc/News.md) Downloads news from internet, convert and store them into Redis
	- [NewsAnalyzer](./doc/NewsAnalyzer.md) Classifies news articles with large language models
- [Ui.Public](./doc/UiPublic.md) User interface for the application
	- Using server rendered blazor as I want to learn to use it better and I think it is
	fits my needs best
- [Ui.News](./doc/todo.md) Currently optimized for news reading, development in progress
- [Ui.Antai](./doc/antai.md) UI for chat with me feature
- Utilities (under Tests currently)
	- MChatter: Chat application with large language model
	- Populator: Utility to fill Redis with bogus data
- All code I write in C# (might do some in rust as went thrue the [book](https://doc.rust-lang.org/book/) a while ago)
	- Currently using .Net 7.0 as had issues with 8.0 RC with blazor
- [Llama.cpp](https://github.com/ggerganov/llama.cpp) server
	- Used to run large language models locally
	- Models I use I get from [Hugginface](https://huggingface.co/)
- [Nginx](https://nginx.org/en/) as front-end/proxy for Asp.Net applications
- [Redis](./doc/redis.md)
	- [Redis homepage](https://redis.io/) for data storage and messaging. Modules I use:
		- [RedisJSON](https://github.com/RedisJSON/RedisJSON) JSON support
		- [RediSearch](https://github.com/RediSearch/RediSearch) Search/indexing
- [Postgresql](https://www.postgresql.org/) for databases
- [Tailwind](https://github.com/tailwindlabs/tailwindcss) for UI styling
- [sonarqube.md](./doc/sonarqube.md) for code analysis
- [Debian](https://www.debian.org/)
	- Platform for everything
	- Currently running version 12 (bookworm)
	- [My servers setup](https://github.com/anttieskola/setup)

## Deployment locations
- Ui.Public published on server into path `/usr/local/bin/AJE.Ui.Public/`
- Ui.News published on server into path `/usr/local/bin/AJE.Ui.News/`
- News published on server into path `/usr/local/bin/AJE.Service.NewsDownloader/`
- NewsAnalyzer published on server into path `/usr/local/bin/AJE.Service.NewsAnalyzer/`
- Llama.cpp is installed manually on server into path `/usr/local/bin/Llama/`
- Redis is installed manually on server into path `/usr/local/bin/Redis/`
	- Modules have own folders under `/usr/local/bin`
- Sonarqube is installed manually on server into path `/usr/local/bin/sonarqube/`
	- I made a [Github repo of this](https://github.com/anttieskola/sonarqube)
	- Only run server locally and on-demand only
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
- [Documentation](./doc/nginx.md)
- Asp.net process is running in ports
	- Ui.Public 5001
	- Ui.News 5002
- [WebSocket documentation](https://www.nginx.com/blog/websocket-nginx/)
- TODO: Need separate certificate for news.anttieskola.com
- TODO: How to configure server to support both sites

# Links
- [this repository](https://github.com/anttieskola/aje)
- [redis commands](https://redis.io/commands/)
- [ui-lifecycle](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/lifecycle?view=aspnetcore-7.0)
- [blazor forms](https://learn.microsoft.com/en-us/aspnet/core/blazor/forms-and-input-components?view=aspnetcore-3.1)
- [signal-r](https://learn.microsoft.com/en-us/aspnet/core/signalr/hubs?view=aspnetcore-7.0)
- [IHttpClientFactory](https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory)

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
