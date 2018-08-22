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
4. sudo semanage port -m -t http_port_t -p tcp 8081
5. sudo systemctl start nginx
6. sudo systemctl status nginx.service

