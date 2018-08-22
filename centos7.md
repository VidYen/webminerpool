File in progress on how to install on CentOS on Google Cloud platform
1. sudo yum update
2. sudo yum install git
3. install mono
4. install nginx
5. install msbuild
6. install cerbot (only cert... don't let it mess with your nginx)

Fix the damn ports
1. sudo firewall-cmd --permanent --zone=public --add-service=http
2. sudo firewall-cmd --permanent --zone=public --add-service=https
3. sudo firewall-cmd --reload
4. sudo semanage port -m -t http_port_t -p tcp 42198 (using this new port)
5. sudo systemctl start nginx
6. sudo systemctl status nginx.service
7. setsebool -P httpd_can_network_connect true (this allows httpd to talk with local)

Some useful commands and links 
1. sudo semanage port -l | grep 8081
2. https://certbot.eff.org/lets-encrypt/centosrhel7-nginx
3. nginx: [emerg] getpwnam("nginx") failed in /etc/nginx/nginx.conf:5
4. useradd -s /bin/false nginx
