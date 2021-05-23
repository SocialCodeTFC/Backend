# SocialCode

**Requirements**

* Docker
* Docker-compose
* Yarn
* Android SDK
* JAVA
* Npm 16.1.0

**Set up**  
```bash
git clone https://github.com/martinvarelaa/SocialCode.git
````

```bash
cd SocialCode
````
_To start SocialCode.API & MongoDB:_  
_(If you are facing problems with docker-compose up, try executing with/without sudo)_

```bash
sudo docker-compose -f docker-compose.yml up
````  
_To start SocialCode React native:_

```bash
cd ./SocialCodeRN
````
```bash
yarn
````
```bash
yarn android
````

```bash
yarn start
````
