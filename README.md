# SocialCode

**Requirements**  

* [Docker](https://docs.docker.com/get-docker/)
* [Docker-compose](https://docs.docker.com/compose/install/)
* [Docker-engine](https://docs.docker.com/engine/install/)
* [.Net SDK 3.1](https://dotnet.microsoft.com/download)  (If you want to debug API or run unit tests)
* [JetBrains Rider](https://www.jetbrains.com/rider/download/) (Only if you want to debug the API)

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
sudo docker-compose -f docker-compose.yml up  //If you want to run with any local change in the project, run with --build --remove-orphans 
````

_In order to run the SocialCode api with Rider, open SocialCode.sln & replace "mongo.SocialCode" with "localhost" in the Host section of the appSettings.json file, then run:_

```bash
sudo docker-compose -f docker-compose.mongo.yml up (Start DB)
dotnet restore 
dotnet build
dotnet run
````  

**This previous step requires .Net Sdk 3.1**

_To see db active ports: ( dbuser: **socialCode** & password: **password**)_

```bash
sudo docker ps 
````  
_To run unit tests:_

```bashd
dotnet test
````  