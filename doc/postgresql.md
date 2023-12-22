# Installation
[Installation](https://www.postgresql.org/download/linux/debian/)

```bash
sudo sh -c 'echo "deb https://apt.postgresql.org/pub/repos/apt $(lsb_release -cs)-pgdg main" > /etc/apt/sources.list.d/pgdg.list'

wget --quiet -O - https://www.postgresql.org/media/keys/ACCC4CF8.asc | sudo apt-key add -

sudo apt-get update

sudo apt-get -y install postgresql
```

Current postgresql 16 configure file location `/etc/postgresql/16/main/`

These default settings (not sure did I add them or not) in `pg_hba.conf` seems to allow local system users to login without password (peer authentication).
```
# TYPE  DATABASE        USER            ADDRESS                 METHOD
local   all             all                                     peer
```

Think this needs to be done once (to add user into postgres)
```bash
sudo -u postgres createuser -l antti
```

Creating database
```bash
sudo -u postgres createdb -O antti newsanalyzer
```


## SqlTools extension for vscode
- [Homepage](https://github.com/mtxr/vscode-sqltools/)
- [Driver](https://github.com/mtxr/vscode-sqltools/tree/dev/packages/driver.pg)

Configuration to connect using unix socket
```json
{
    "sqltools.connections": [
        {
            "database": "newsanalyzer",
            "driver": "PostgreSQL",
            "name": "newsanalyzer@localhost",
            "previewLimit": 1000,
            "server": "/run/postgresql",
            "username": "antti",
            "password": ""
        }
    ]
}
```

# Usage
Follow these steps

## EF Tools
Required to install for development environment

- [Guide](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

```bash
# install tools globally
dotnet tool install --global dotnet-ef

# update it if need
dotnet tool update --global dotnet-ef

# add to .bash_aliases, globally installed dotnet tools
export PATH=$PATH:$HOME/.dotnet/tools
```

## Nuget packages
For the component using EF & Database
- [Homepage](https://www.npgsql.org/)
- [Type mapping](https://www.npgsql.org/doc/types/basic.html)

Postgresql package (required)
```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
```

Tools (not required now, as using globally installed tools)
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
```

## Create DbContext
Example

```csharp
public class NewsAnalyzerContext : DbContext
{
    public NewsAnalyzerContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
    {
    }

    public NewsAnalyzerContext()
        : base()
    {
    }

    public DbSet<ArticleClassifiedEvent> ArticleClassifiedEvents { get; set; } = null!;
}
```
## Connection string
In the component's appsettings.json
```json
{
  "ConnectionStrings": {
    "NewsAnalyzer": "Host=/run/postgresql;Database=newsanalyzer;"
  }
}
```

## Register DbContext
In the component startup
```csharp
services.AddDbContext<NewsAnalyzerContext>(options =>
{
    options.UseNpgsql(config.GetConnectionString("NewsAnalyzer"));
});
```

## Create migration
In the component root, in console
```bash
dotnet ef migrations add InitialCreate --context NewsAnalyzerContext

# Should create Migrations folder and migration script
```

# Backups
```bash
#!/bin/bash
pg_dump -U antti -F t newsanalyzer | bzip2 > db_newsanalyzer.tar.bz2
```

# Restore
```bash
#!/bin/bash

# drop (EF migration history fails currently with backups so have to remake it)
sudo -u postgres dropdb newsanalyzer

# create
sudo -u postgres createdb -O antti newsanalyzer

# restore
bzip2 -d -c db_newsanalyzer.tar.bz2 | pg_restore -U antti -d newsanalyzer
```
