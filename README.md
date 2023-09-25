# notes/planning
Project started with learning to create an application to modify my notes that are
stored in Github as markdown and same time create homepage for myself.

There basically already is one thats Github's UI. But I wanted to write application
which would support hundreds of people modify content in real time with each other
and same create a new working homepage for myself.

To achieve this I will use redis to store data and connect blazor servers. Then I can have
many more servers running the application. Pub/Sub message patterns from using signal-r are
boosted by into redis.

## Short list of hopes and dreams (features)
- dotnet... i wanted to use 8 but not ready so 7 it is
	- Experienced issues with 8.0 preview
	- Using .net 7.0, because tried two hours to run [sample codes](https://github.com/dotnet/blazor-samples.git)
	with the 8.0 preview template. Not sure did I break template but all worked with 7.
- command/query split
	- commands -> events
- runs on debian 12 [my nodes setup](https://github.com/anttieskola/setup)
- redis used as data store and messaging
	- no sql in beginning to keeping it open
	- using atleast redis modules like [RediSearch](https://github.com/RediSearch/RediSearch) and [RedisJSON](https://github.com/RedisJSON/RedisJSON)
- want to keep an grpc server in the project if I need to expand the interface outside redis
- supports multiple front end servers (nginx)
- nginx proxies to .net app
- server rendered blazor app
- templates used as model, thanks a bunch \o/
	- [CleanArchitecture](https://github.com/ardalis/CleanArchitecture)
	- [CleanArchitecture](https://github.com/jasontaylordev/CleanArchitecture)
- using the lovely [MediatR](https://github.com/jbogard/MediatR)
- learning to use [tailwind css](https://github.com/tailwindlabs/tailwindcss)


## UI-Components
- TableTemplate
	- Planned
- Icon
	- Took [bootstrap icons](https://github.com/twbs/icons) and made a component that creates the svg element directly
	- So basically all icons are written directly to responses which is a very
      stupid idea as you can see if you list them all the pagesize is huge and
	  client can't cache it, but you can control the drawing better and was fun
	  thing to make. Sizes are small still but you don't want to write those into
	  each line for example.
	- Will make use of the small font size and load css so font is cached on client side.
	- This version I wanna keep the fun mess I made
		 
#  links
- [this repository](https://github.com/anttieskola/aje)
- [ui-lifecycle](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/lifecycle?view=aspnetcore-7.0)

## Misc
### making C# solution in linux

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


### llama.cpp
- how to combine llama.cpp (running on one node with the GTX)
	- works already on one machine
	- there must be already made server
	- https://github.com/ggerganov/llama.cpp/blob/master/examples/server/README.md

### database ???
- [installation](https://wiki.debian.org/PostgreSql)

```bash
sudo -u postgres psql
createdb -O antti aje
```