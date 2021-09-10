#!/bin/bash

echo "Removing old containers and images..."
docker container prune -f --filter "label=name=certificate-password-cracker"
docker image prune -a -f --filter "label=rep=certificate-password-cracker"

echo "Building image"
docker build -t certificate-password-cracker .

echo "Stopping and removing old container..."
docker stop certificate-password-cracker
docker rm certificate-password-cracker

echo "Deploying container..."
docker run -d --name bcertificate-password-cracker --restart=always certificate-password-cracker:latest
