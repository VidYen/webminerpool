File in progress on how to install on CentOS on Google Cloud platform
1. sudo yum update
2. sudo yum install git
3. install mono
4. install nginx
5. install msbuild
6. install cerbot (only cert... don't let it mess with your nginx)

Fix the damn ports
sudo firewall-cmd --permanent --zone=public --add-service=http
sudo firewall-cmd --permanent --zone=public --add-service=https
sudo firewall-cmd --reload
sudo semanage port -m -t http_port_t -p tcp 8081
sudo systemctl start nginx
sudo systemctl status nginx.service

