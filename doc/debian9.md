## Debian Server Setup Instructions

1. Check for updates:``sudo apt-get update``
2. Do the updates: ``sudo apt upgrade``
3. Install [mono](https://www.mono-project.com/download/stable/#download-lin-debian) NOTE: You may have to install with ``mono-complete``
4. Install [msbuild](https://www.microsoft.com/net/learn/get-started-with-dotnet-tutorial) NOTE: If you run ``sudo apt-get install msbuild`` you do not have to install the whole dot net framework. (Note You still need to add the repos in the dotnet part) NOTE: Actually I think we just need this [mono version](https://www.mono-project.com/download/vs/#download-lin-debian) according to the install command line.
5. Install [git](https://gist.github.com/derhuerst/1b15ff4652a867391f03)
6. Install gninx: ``sudo apt-get install nginx``
7. Install [certbot](https://certbot.eff.org/lets-encrypt/debianstretch-nginx)
8. On a bare bones server (which I just got) you may have to `sudo apt-get install build-essential`
9. Also `sudo apt-get install libssl-dev` is needed as well.
10. And lastly had to install the `libssl1.0.0 in jessie` deb and had to resintall mono.
11. Also these may need to be installed:

```
liblttng-ust0
libcurl3
libssl1.0.0
libkrb5-3
zlib1g
libicu52 (for 14.x)
libicu55 (for 16.x)
libicu57 (for 17.x)
libicu60 (for 18.x)
```

## Certbot instructions
1. Run: ``sudo certbot certonly --authenticator standalone --pre-hook "nginx -s stop" --post-hook "nginx"``
2. NOTE: You need to own the domain that you are certining (or IP etc but the cert will only work for it).
3. I copy the files out of the root area to make it easier to create the pfx file and move it around (``sudo su`` and then back again)
3. As you should already have openssl installed: ``openssl pkcs12 -export -out certificate.pfx -inkey privkey.pem -in cert.pem -certfile chain.pem``
4 Follow onscreen instructions and set export password as: ``miner``

## Install the miner
1. Install the repo ``git clone https://github.com/VidYen/webminerpool.git``
2. Build the repo

## Optional
1. Install fuse: ``sudo apt-get install fuse``

### Setup Nginx (also optional)
1. Add the following in the http section of the `/etc/nginx/nginx.conf` file:

These have been made mostly redundant with the JS of VYPS now switching servers rather than the php checking

To do pure port 80 talking:

Comment out this...
```/etc/nginx/sites-enable/default```

And `sudo vi /etc/nginx/sites-enabled/default` and turn off the default server listen and proxy pass in the nginx.conf

Add:

```
server {
        listen       80 default_server;
        listen       [::]:80 default_server;
        server_name  _;

    location / {
        proxy_pass http://127.0.0.1:8282;
    }

}

```




By default this seems to be friendly with most firewalls:

```
server {
        listen       8081 default_server;
        listen       [::]:8081 default_server;
        server_name  _;

    location / {
        proxy_pass http://127.0.0.1:8282;
    }

}

```

If that fails you can try an obscure port:

```
server {
        listen       42198 default_server;
        listen       [::]:42198 default_server;
        server_name  _;

    location / {
        proxy_pass http://127.0.0.1:8282;
    }

}
```
**NOTE** Sometimes you just have to reboot Debian to get port 80 freed up even if you commented everything out and stopped and started the service.

2. You may have to disable things running on port 80
3. Run `sudo systemctl stop nginx`
4. Run `sudo systemctl start nginx`
5. If error for more info: `sudo systemctl status nginx.service`



Do not forget:
``sudo apt install mono-devel``
``sudo apt install msbuild``

Create an sh bash script to keep servers up (say in /home/bootup/ as server.sh and don't forget to make executable with chmod)

```
#!/bin/bash
cd /home/webminerpool/server/Server/bin/Release_Server
mono server.exe > websocket.log
```

Setup a crontab for the user (you do not want to run the webscokcet as root)

```
@reboot /home/bootup/server.sh
```

Crontab setup for root:

```
* * * * * /usr/bin/pgrep nginx > /dev/null || /etc/init.d/nginx restart >> /var/log/messages
*/5 * * * * /usr/bin/pgrep mono > /dev/null || /sbin/shutdown -r
```

This will check that nginx is running and restart the service is not.
And will check every 5 minutes if mono is running and reboot the server if not.

For the servers themselves:
```bash
#! /bin/bash

if lsof -Pi :2053 -sTCP:LISTEN -t >/dev/null ; then
    cd /home/fabius/mshare/trazyn/webminerpool/server/Server/bin/Release_Server
    current_date_time="`date +%Y%m%d%H%M%S`";    
    echo "running" >> uptimecheck.log
    echo $current_date_time >> uptimecheck.log
else
    cd /home/fabius/mshare/trazyn/webminerpool/server/Server/bin/Release_Server
    current_date_time="`date +%Y%m%d%H%M%S`";    
    echo "not running" >> uptimecheck.log
    echo $current_date_time >> uptimecheck.log
    mono server.exe > server.log
fi
```
