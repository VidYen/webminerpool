Ok same deal but with debian on fresh install
1. Check for updates:``sudo apt-get update``
2. Do the updates: ``sudo apt upgrade``
3. Install [mono](https://www.mono-project.com/download/stable/#download-lin-debian)
4. Install [msbuild](https://www.microsoft.com/net/learn/get-started-with-dotnet-tutorial)
5. Install [git](https://gist.github.com/derhuerst/1b15ff4652a867391f03)
6. Install gninx: ``sudo apt-get install nginx``
7. Install [certbot]:(https://certbot.eff.org/lets-encrypt/debianstretch-nginx)

Certbot instructions
1. Run: ``sudo certbot certonly --authenticator standalone --pre-hook "nginx -s stop" --post-hook "nginx"``
2. NOTE: You need to own the domain that you are certining (or IP etc but the cert will only work for it).
3. I copy the files out of the root area to make it easier to create the pfx file and move it around (``sudo su`` and then back again)
3. As you should already have openssl installed: ``openssl pkcs12 -export -out certificate.pfx -inkey privkey.pem -in cert.pem -certfile chain.pem``
4 Follow onscreen instructions and set export password as: ``miner``

Optional
1. Install fuse: ``sudo apt-get install fuse``
