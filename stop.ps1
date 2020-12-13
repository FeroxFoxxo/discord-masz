docker ps -a

Write-Host "Killing old containers"
docker-compose stop
Write-Host "Killed old containers"

docker ps -a

Write-Host "Removing old containers/images/volumes"
docker container rm masz_nginx
docker container rm masz_backend
docker container rm masz_sf4_apache
docker container rm masz_sf4_php
docker container rm masz_discordbot

docker image rm discord-masz_nginx
docker image rm discord-masz_sf4_apache
docker image rm discord-masz_php
docker image rm discord-masz_backend
docker image rm discord-masz_discordbot

docker volume rm discord-masz_php_share
Write-Host "Removed old containers/images/volumes"

docker ps -a
