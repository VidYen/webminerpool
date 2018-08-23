Ok same deal but with debian on fresh install
1. Check for updates:``sudo apt-get update``
2. Do the updates: ``sudo apt upgrade``https://github.com/VidYen/webminerpool/issues
3. Install [mono](https://www.mono-project.com/download/stable/#download-lin-debian)
4. Install [msbuild](https://www.microsoft.com/net/learn/get-started-with-dotnet-tutorial) NOTE: If you run ``sudo apt-get install msbuild`` you do not have to install the whole dot net framework.
5. Install [git](https://gist.github.com/derhuerst/1b15ff4652a867391f03)
6. Install gninx: ``sudo apt-get install nginx``
7. Install [certbot](https://certbot.eff.org/lets-encrypt/debianstretch-nginx)

Certbot instructions
1. Run: ``sudo certbot certonly --authenticator standalone --pre-hook "nginx -s stop" --post-hook "nginx"``
2. NOTE: You need to own the domain that you are certining (or IP etc but the cert will only work for it).
3. I copy the files out of the root area to make it easier to create the pfx file and move it around (``sudo su`` and then back again)
3. As you should already have openssl installed: ``openssl pkcs12 -export -out certificate.pfx -inkey privkey.pem -in cert.pem -certfile chain.pem``
4 Follow onscreen instructions and set export password as: ``miner``

Install the miner
1. Install the repo ``git clone https://github.com/VidYen/webminerpool.git``
2. Build the repo

Optional
1. Install fuse: ``sudo apt-get install fuse``

Setup Nginx
1. Add the following in the http section of the `/etc/nginx/nginx.conf` file:

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
2. `sudo systemctl start nginx`
3. `sudo systemctl status nginx.service`



Do not forget:
``sudo apt install mono-devel``
``sudo apt install msbuild``
