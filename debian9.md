Ok same deal but with debian on fresh install
1. Check for updates:``sudo apt-get update``
2. Do the updates: ``sudo apt upgrade``
3. Install [mono](https://www.mono-project.com/download/stable/#download-lin-debian)
4. Install [msbuild](https://www.microsoft.com/net/learn/get-started-with-dotnet-tutorial)
5. Install [git](https://gist.github.com/derhuerst/1b15ff4652a867391f03)
6. Install gninx: ``sudo apt-get install nginx``
7. Install [certbot]:(https://certbot.eff.org/lets-encrypt/debianstretch-nginx)

Certbot instructions
1. Run: ``sudo certbot --authenticator webroot --installer nginx``
2. Follow onscreen instructions and set passphrase as: ``miner``
3. As you should already have openssl installed: ``openssl pkcs12 -export -out certificate.pfx -inkey privkey.pem -in cert.pem -certfile chain.pem``

Optional
1. Install fuse: ``sudo apt-get install fuse``
