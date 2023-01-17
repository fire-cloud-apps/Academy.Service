# Academy.API
An Academy API Service, is an web application service which is used for School management system/Institute/College Management portal.

## Deploy
1. This will be deployed as a Azure Web Apps.
2. App will be containerized and pushed to Docker Hub.
3. Azure Web App, will be enabled with continuous deployment

### Development Rules

1. Define the Entity/Model carefully - Academy.Entity
2. Create Data Access for the Model - Academy.DataAccess
3. Implement Bogus for the Model - Academy.Entity.DataBogus
4. Test the Data Access - Academy.Test.DataAccess
5. Create Web API Controller for the Defined Model. - Academy.Service 

#### Docker Deploy Rules
1. Build the Docker file, which exists near to solution file.

```docker build --tag academy-service-api:v1.0.0 .```

2. Tag the Docker image.

```docker tag 72dab714f095 ganeshramsr/academy-service-api:v1.0.0```
```docker tag 72dab714f095 ganeshramsr/academy-service-api:latest```

3. Push to Docker Hub

```docker push ganeshramsr/academy-service-api:v1.0.0```

4. To make sure this is the latest one
```docker push ganeshramsr/academy-service-api:latest```

5. To run as docker Image

```docker run --rm -it -p 7181:443/tcp -p 7282:80/tcp academy-service-api:v1.0.0```

6. Optional
-- To remove all unnecessary images we can use the below command

```docker image prune --filter="dangling=true"```

----- Removes all unused containers

```docker container prune```
