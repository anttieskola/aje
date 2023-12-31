# Development (zeus) Everything locally with GTX 3060 (12GB)

## Hosts
```
127.0.0.1       localhost
127.0.0.1       www.zeus.com
127.0.0.1       news.zeus.com
127.0.0.1       antai.zeus.com
127.0.0.1       promptstudio.zeus.com
192.168.1.20    zeus

# The following lines are desirable for IPv6 capable hosts
::1     localhost ip6-localhost ip6-loopback
ff02::1 ip6-allnodes
ff02::2 ip6-allrouters
```

## Nginx
```
server {
        listen 80;
        root /var/www/html;
        index index.html;
        server_name www.zeus.com;
        location / {
                # proxy to asp.net app
                proxy_pass http://127.0.0.1:5001;
                proxy_http_version 1.1;
                proxy_set_header Upgrade $http_upgrade;
                proxy_set_header Connection "Upgrade";
                proxy_set_header Host $host;
                proxy_cache_bypass $http_upgrade;
        }
}

server {
        listen 80;
        server_name news.zeus.com;
        root /var/www/news;
        index index.html;
        location / {
                # proxy to asp.net app
                proxy_pass http://127.0.0.1:5002;
                proxy_http_version 1.1;
                proxy_set_header Upgrade $http_upgrade;
                proxy_set_header Connection "Upgrade";
                proxy_set_header Host $host;
                proxy_cache_bypass $http_upgrade;
        }
}

server {
        listen 80;
        server_name antai.zeus.com;
        root /var/www/antai;
        index index.html;
        location / {
                # proxy to asp.net app
                proxy_pass http://127.0.0.1:5003;
                proxy_http_version 1.1;
                proxy_set_header Upgrade $http_upgrade;
                proxy_set_header Connection "Upgrade";
                proxy_set_header Host $host;
                proxy_cache_bypass $http_upgrade;
        }
}

server {
        listen 80;
        server_name promptstudio.zeus.com;
        root /var/www/promptstudio;
        index index.html;
        location / {
                # proxy to asp.net app
                proxy_pass http://127.0.0.1:5006;
                proxy_http_version 1.1;
                proxy_set_header Upgrade $http_upgrade;
                proxy_set_header Connection "Upgrade";
                proxy_set_header Host $host;
                proxy_cache_bypass $http_upgrade;
        }
}
```

# Production (ares) Websites + Redis

## Hosts
```
127.0.0.1       localhost
127.0.0.1       www.anttieskola.com
127.0.0.1       news.anttieskola.com
127.0.0.1       antai.anttieskola.com
192.168.1.2     ares

# The following lines are desirable for IPv6 capable hosts
::1     localhost ip6-localhost ip6-loopback
ff02::1 ip6-allnodes
ff02::2 ip6-allrouters
```

## Nginx
```
server {
        listen 80;
        server_name www.anttieskola.com;
        root /var/www/www;
        index index.html;
        location / {
                return 301 https://$host$request_uri;
        }
}
server {
        listen 443 ssl;
        include snippets/www.anttieskola.conf;
        root /var/www/www;
        index index.html;
        server_name www.anttieskola.com;
#        location / {
#                # proxy to asp.net app
#                proxy_pass http://127.0.0.1:5001;
#                proxy_http_version 1.1;
#                proxy_set_header Upgrade $http_upgrade;
#                proxy_set_header Connection "Upgrade";
#                proxy_set_header Host $host;
#                proxy_cache_bypass $http_upgrade;
#        }
}

server {
        listen 80;
        server_name news.anttieskola.com;
        root /var/www/news;
        index index.html;
        location / {
                return 301 https://$host$request_uri;
        }
}
server {
        listen 443 ssl;
        include snippets/news.anttieskola.conf;
        server_name news.anttieskola.com;
        root /var/www/news;
        index index.html;
        location / {
                # proxy to asp.net app
                proxy_pass http://127.0.0.1:5002;
                proxy_http_version 1.1;
                proxy_set_header Upgrade $http_upgrade;
                proxy_set_header Connection "Upgrade";
                proxy_set_header Host $host;
                proxy_cache_bypass $http_upgrade;
        }
}
server {
        listen 80;
        server_name antai.anttieskola.com;
        root /var/www/antai;
        index index.html;
        location / {
                return 301 https://$host$request_uri;
        }
}
server {
        listen 443 ssl;
        include snippets/antai.anttieskola.conf;
        server_name antai.anttieskola.com;
        root /var/www/antai;
        index index.html;
        auth_basic "Access denied";
        auth_basic_user_file /etc/nginx/antai.htpasswd;
        location / {
                # proxy to asp.net app
                proxy_pass http://127.0.0.1:5003;
                proxy_http_version 1.1;
                proxy_set_header Upgrade $http_upgrade;
                proxy_set_header Connection "Upgrade";
                proxy_set_header Host $host;
                proxy_cache_bypass $http_upgrade;
       }
}
```

# Production (god) - Llama.cpp with GTX 3080 (10GB)

## Hosts
```
127.0.0.1       localhost
192.168.1.11    god

# The following lines are desirable for IPv6 capable hosts
::1     localhost ip6-localhost ip6-loopback
ff02::1 ip6-allnodes
ff02::2 ip6-allrouters
```

## Nginx
```
server {
        listen 80;
        root /var/www/html;
        index index.html;
        location / {
                proxy_pass http://localhost:5999;
                proxy_http_version 1.1;
        }
}
```
