# Goal
Project started with learning to create an application to modify my notes that are
stored in Github as markdown and same time create homepage for myself.

There basically already is one thats Github's UI. But I wanted to write application
which would support hundreds of people modify content in real time with each other
and same create a new working homepage for myself.

To achieve this I will use redis to store data and connect blazor servers. Then I can have
many more servers running the application. Pub/Sub message patterns from using signal-r are
boosted by into redis.

# Learned things along the way
- redis can be integrated directly [read the documentation](https://learn.microsoft.com/en-us/aspnet/core/signalr/redis-backplane?view=aspnetcore-7.0)
	- package [Microsoft.AspNetCore.SignalR.StackExchangeRedis](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.signalr.stackexchangeredis?view=aspnetcore-7.0)
	- package kinda does same thing I had in mind to relay messages
	- still if I want to create let's say some rust app that produces data don't think I can send data directly to these channels
	  as they are generated as needed and follow own naming conventions and message formats
		- so I can send to my custom channel data from the rust app that something like the ChatRelayService then send into signal-r
		- ofc the setup using redis with signal-r I can reach all servers directly
- each signal-r hub has its own websocket connection (pre .net 4.7 there was only one connection for all hubs)
- so there is limitation on number of users single server can handle
- azure signal-r service moves the socket load from server to service (the selling point of the product)


## Short list of hopes and dreams (features)
- dotnet... i wanted to use 8 but not ready so 7 it is
	- Experienced issues with 8.0 preview
	- Using .net 7.0, because tried two hours to run [sample codes](https://github.com/dotnet/blazor-samples.git)
	with the 8.0 preview template. Not sure did I break template but all worked with 7.
- command/query split
	- commands -> events
- runs on debian 12 [my nodes setup](https://github.com/anttieskola/setup)
- redis used as data store and messaging
	- no sql at all in beginning
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
		 
##  Links
- [this repository](https://github.com/anttieskola/aje)
- [ui-lifecycle](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/lifecycle?view=aspnetcore-7.0)
- [blazor forms](https://learn.microsoft.com/en-us/aspnet/core/blazor/forms-and-input-components?view=aspnetcore-3.1)
- [signal-r](https://learn.microsoft.com/en-us/aspnet/core/signalr/hubs?view=aspnetcore-7.0)

# Deployment
- Currently published on server into path `/usr/local/bin/aje/`
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


# Misc

## making C# solution in linux

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


## llama.cpp
- how to combine llama.cpp (running on one node with the GTX)
	- works already on one machine
	- there must be already made server
	- https://github.com/ggerganov/llama.cpp/blob/master/examples/server/README.md

## database ???
- [installation](https://wiki.debian.org/PostgreSql)

```bash
sudo -u postgres psql
createdb -O antti aje
```