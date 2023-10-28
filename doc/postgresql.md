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
