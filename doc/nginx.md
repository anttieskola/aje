# Host file hacking
```
127.0.0.1       www.zeus.com
127.0.0.1       news.zeus.com
```

# Current configuration on zeus
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
```