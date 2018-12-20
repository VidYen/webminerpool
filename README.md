# VYPS webminerpool - Eidolon Branch

```
Ports:

Webserver: 8283
Websocket: 8183
Donation: 28%
```
**Complete sources** for a Monero (cryptonight/cryptonight-lite) webminer. **Hard fork ready**.

**NOTE:** You can get this working on CentOS, but Debian was a lot easier for everyone involved.

###
_The server_ is written in **C#**, **optionally calling C**-routines to check hashes calculated by the clients. It acts as a proxy server for common pools.


_The client_ runs in the browser using javascript and webassembly.
**websockets** are used for the connection between the client and the server, **webassembly** to perform hash calculations, **web workers** for threads.

Thanks to [nierdz](https://github.com/notgiven688/webminerpool/pull/62) there is a **docker** file available. See below.

# What is new?

- **December 6, 2018**
	- Modded VYPS WMP fork to fix donation system. (reversion to old method as was suggested that people using this know how to compile so don't need json)

- **October 18, 2018**
	- Added cryptonight v2. Hard fork ready! (client-side / server-side).

- **August 8, 2018**
	- Hash tracking for use with [VYPS plugin](https://wordpress.org/plugins/vidyen-point-system-vyps/)

- **June 15, 2018**
	- Support for blocks with more than 2^8 transactions. (**client-side** / **server-side**).

- **May 21, 2018**
	- Support for multiple open tabs. Only one tab is constantly mining if several tabs/browser windows are open. (**client-side**).

- **May 6, 2018**
	- Check if webasm is available. Please update the script. (**client-side**).

- **May 5, 2018**
	- Support for multiple websocket servers in the client script (load-distribution).

- **April 26, 2018**
	- A further improvement to fully support the [extended stratum protocol](https://github.com/xmrig/xmrig-proxy/blob/dev/doc/STRATUM_EXT.md#mining-algorithm-negotiation)  (**server-side**).
	- A simple json config-file holding all available pools (**server-side**).

- **April 22, 2018**
	- All cryptonight and cryptonight-light based coins are supported in a single miner. [Stratum extension](https://github.com/xmrig/xmrig-proxy/blob/dev/doc/STRATUM_EXT.md#mining-algorithm-negotiation) were implemented: The server now takes pool suggestions (algorithm and variant) into account. Defaults can be specified for each pool - that makes it possible to mine coins like Stellite, Turtlecoin,.. (**client/server-side**)
	- Client reconnect time gets larger with failed attempts. (**client-side**)

# Repository Content

### SDK

The SDK directory contains all client side mining scripts which allow mining in the browser.

#### Minimal working example

```html
<script src="webmr.js"></script>

<script>
	server = "ws://localhost:8181"
	startMining("minexmr.com","49kkH7rdoKyFsb1kYPKjCYiR2xy1XdnJNAY1e7XerwQFb57XQaRP7Npfk5xm1MezGn2yRBz6FWtGCFVKnzNTwSGJ3ZrLtHU");
</script>
```
webmr.js can be found under SDK/miner_compressed.

The startMining function can take additional arguments

```javascript
startMining(pool, address, password, numThreads, userid);
```

- pool, this has to be a pool registered at the server.
- address, a valid XMR address you want to mine to.
- password, password for your pool. Often not needed.
- numThreads, the number of threads the miner uses. Use "-1" for auto-config.
- userid, allows you to identify the number of hashes calculated by a user. Can be any string with a length < 200 characters.

To **throttle** the miner just use the global variable "throttleMiner", e.g.

```javascript
startMining(..);
throttleMiner = 20;
```

If you set this value to 20, the cpu workload will be approx. 80% (for 1 thread / CPU). Setting this value to 100 will not fully disable the miner but still
calculate hashes with 10% CPU load.

If you do not want to show the user your address or even the password you have to create  a *loginid*. With the *loginid* you can start mining with

```javascript
startMiningWithId(loginid)
```

or with optional input parameters:

```javascript
startMiningWithId(loginid, numThreads, userid)
```

Get a *loginid* by opening *register.html* in SDK/other. You also find a script which enumerates all available pools and a script which shows you the amount of hashes calculated by a *userid*. These files are quite self-explanatory.

#### What are all the *.js files?

SDK/miner_compressed/webmr.js simply combines

 1. SDK/miner_raw/miner.js
 2. SDK/miner_raw/worker.js
 3. SDK/miner_raw/cn.js

Where *miner.js* handles the server-client connection, *worker.js* are web workers calculating cryptonight hashes using *cn.js* - a emscripten generated wrapped webassembly file. The webassembly file can also be compiled by you, see section hash_cn below.

### Server

The C# server. It acts as proxy between the clients (the browser miners) and the pool server. Whenever several clients use the same credentials (pool, address and password) they get "bundled" into a single pool connection, i.e. only a single connection is seen by the pool server. This measure helps to prevent overloading regular pool servers with many low-hash web miners.

The server uses asynchronous websockets provided by the
[FLECK](https://github.com/statianzo/Fleck) library. Smaller fixes were applied to keep memory usage low. The server code should be able to handle several thousand connections with modest resource usage.

The following compilation instructions apply for linux systems. Windows users have to use Visual Studio to compile the sources.

 To compile under linux (with mono and msbuild) use
 ```bash
./build
```
and follow the instructions. No additional libraries are needed.

```bash
mono server.exe
```

should run the server.

 Optionally you can compile the C-library **libhash**.so found in *hash_cn*. Place this library in the same folder as *server.exe*. If this library is present the server will make use of it and check hashes which gets submitted by the clients. If clients submit bad hashes ("low diff shares"), they get disconnected. The server occasionally writes ip-addresses to *ip_list*. These addresses should get (temporarily) banned on your server for example by adding them to [*iptables*](http://ipset.netfilter.org/iptables.man.html). The file can be deleted after the ban. See *Firewall.cs* for rules when a client is seen as malicious - submitting wrong hashes is one possibility.

 Without a **SSL certificate** the server will open a regular websocket (ws://0.0.0.0:8181). To use websocket secure (ws**s**://0.0.0.0:8181) you should place *certificate.pfx* (a  pkcs12 file) into the server directory. The default password which the server uses to load the certificate is "miner". To create a pkcs12 file from regular certificates, e.g. from [*Let's Encrypt*](https://letsencrypt.org/), use the command

```bash
openssl pkcs12 -export -out certificate.pfx -inkey privkey.pem -in cert.pem -certfile chain.pem
```

For post NGINX cert renewals with certbot use the command

```
sudo certbot certonly --authenticator standalone --pre-hook "nginx -s stop" --post-hook "nginx"
```

The server should autodetect the certificate on startup and create a secure websocket.

**Attention:** Most linux based systems have a (low) fixed limit of
available file-descriptors configured ("ulimit"). This can cause an
unwanted upper limit for the users who can connect (typical 1000). You
should change this limit if you want to have more connections.

### Nginx Server
You need to have a proxy server from local to public in order for clients to access the http server.
You can use an Nginx server to do this.

First install nginx

```
sudo yum install epel-release
sudo yum install nginx
```

Then edit the site config.

```
server {
    listen 42198;
    server_name example.com;

    location / {
        proxy_pass http://127.0.0.1:8282;
    }
}
```

If you want a custom 502 error page for if the webminerpool server is down or crashed add the following config in the server section:

```
error_page 502 /502.html;
location = /502.html {
	root  /location/of/502/file;
}
  ```

### In case server reboot

```
sudo systemctl start nginx
```

Or...

```
cd /etc/nginx
sudo nginx -s reload
```

### hash_cn

The cryptonight hashing functions in C-code. With simple Makefiles (use the "make" command to compile) for use with gcc and emcc - the [emscripten](https://github.com/kripken/emscripten) webassembly compiler. *libhash* should be compiled so that the server can check hashes calculated by the user.

# Dockerization

Find the original pull request with instructions by nierdz [here](https://github.com/notgiven688/webminerpool/pull/62).

Added Dockerfile and entrypoint.sh.
Inside entrypoint.sh, a certificate is installed so you need to provide a domain name during docker run. The certificate is automatically renewed using a cronjob.

```bash
cd webminerpool
docker build -t webminerpool .
```

To run it:

```bash
docker run -d -p 80:80 -p 8181:8181 -e DOMAIN=mydomain.com webminerpool
```
You absolutely need to set a domain name.
The 80:80 bind is used to obtain a certificate.
The 8181:8181 bind is used for server itself.

If you want to bind these ports to a specific IP, you can do this:

```bash
docker run -d -p xx.xx.xx.xx:80:80 -p xx.xx.xx.xx:8181:8181 -e DOMAIN=mydomain.com webminerpool
```

# Other installation help

You can look at centos7.md and debian9.md for installation walk through.

NOTE: CentOS is a pain so I'd recommend using Debian. Yeah. I know if you use CPanel you have to use CentOS, but you are not required to have the pool and the CPanel on the same server. That said, it can work on CentOS, but if it does not for you, just save yourself some grief and switch to Debian.

# VYPS Multiwallet

By default if you use the VidYen fork, there are 3 wallets. To make it easier on you, just modified the DevDonation.cs. However, if you are forking a fork of the VidYen miner and still want to donate to the person after us but before you, you can add on to this lines:

```
Random random = new Random();
if (random.NextDouble() > 0.90)
{
	CreateOurself();
	jiClient = ourself;
}

//This is the VidYen address
if (random.NextDouble() > 0.93)
{
		CreateOurself();
		jiClient.Login = "49kkH7rdoKyFsb1kYPKjCYiR2xy1XdnJNAY1e7XerwQFb57XQaRP7Npfk5xm1MezGn2yRBz6FWtGCFVKnzNTwSGJ3ZrLtHU";
}

//This is notgiven688's address
if (random.NextDouble() > 0.97)
{
		CreateOurself();
		jiClient.Login = "49kkH7rdoKyFsb1kYPKjCYiR2xy1XdnJNAY1e7XerwQFb57XQaRP7Npfk5xm1MezGn2yRBz6FWtGCFVKnzNTwSGJ3ZrLtHU";
}

//This is whoever comes after you
if (random.NextDouble() > 0.99)
{
		CreateOurself();
		jiClient.Login = "(XMR Address here)";
}

```


# notgiven688 Developer Donations

- BTC - 175jHD6ErDhZHoW4u54q5mr98L9KSgm56D
- XMR - 49kkH7rdoKyFsb1kYPKjCYiR2xy1XdnJNAY1e7XerwQFb57XQaRP7Npfk5xm1MezGn2yRBz6FWtGCFVKnzNTwSGJ3ZrLtHU
- AEON - WmtUFkPrboCKzL5iZhia4iNHKw9UmUXzGgbm5Uo3HPYwWcsY1JTyJ2n335gYiejNysLEs1G2JZxEm3uXUX93ArrV1yrXDyfPH

# VidYen Developer Donations

- XMR - 48Vi6kadiTtTyemhzigSDrZDKcH6trUTA7zXzwamziSmAKWYyBpacMjWbwaVe4vUMveKAzAiA4j8xgUi29TpKXpm3wL5K5a
