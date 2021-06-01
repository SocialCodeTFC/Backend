# SocialCode

**Requirements**

* Docker
* Docker-compose
* Docker-engine
* Dotnet SDK 3.1

**Set up**  
```bash
git clone https://github.com/SocialCodeTFC/Backend
````

```bash
cd Backend
````
_To start SocialCode.API & MongoDB:_  
_(If you are facing problems with docker-compose up, try executing with/without sudo)_

```bash
sudo docker-compose -f docker-compose.yml up  //If you want to restore with any local change in the project, run with --build --remove-orphans 
````  
_If you want to run db locally:_

```bash
sudo docker-compose -f docker-compose.mongo.yml up 
````  

_To see db active ports: ( dbuser: **socialCode** & password :**password**)_
```bashd
sudo docker ps 
````  