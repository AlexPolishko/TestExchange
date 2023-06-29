# TestExchange

This repository contains a solution which gives a user the best possible price
 if he/she is buying or selling a certain amount of BTC in different cryptoexchanges.

## Console App

The console app project provides a command-line interface for select buy or sell transaction.
Add some money or coins to wallet and create plan to buy/sell some amount of BTC by best price

### Usage

To run the console app, navigate to the `TestExchange.Console` directory and execute the following command:
```shell
dotnet run
```
## Unit Test
The `TestExchange.Application.Tests` project contains unit tests to verify the functionality of the TestExchange application.
 These tests cover various scenarios and help ensure the reliability of the code.
 
### Running the Tests
To run the unit tests, navigate to the `TestExchange.Tests` directory and execute the following command:
```shell
dotnet test
```

### Getting Started
To run the API project, navigate to the `TestExchange.API` directory and execute the following command:
```shell
dotnet run
```

## Prerequisites for run Docker app

Before you begin, please ensure that you have the following prerequisites installed:

- Docker: You should have Docker installed on your machine. You can download and install Docker from the official Docker website: [https://www.docker.com/get-started](https://www.docker.com/get-started)

## Building the Docker Image

To run your ASP.NET Visual Studio application in a Docker container, you need to build a Docker image. Follow these steps to build the Docker image:

1. Open a terminal or command prompt.

2. Navigate to the root directory of your ASP.NET Visual Studio project.

3. Run the following command to build the Docker image:
   
   ```shell
   docker build -t testexchangeapi:dev .
   ```
4. Run the following command to start a Docker container from the previously built image:
   ```shell
	docker run -dt -e "ASPNETCORE_ENVIRONMENT=Development" -e "ASPNETCORE_URLS=https://+:443;http://+:80" -e "ASPNETCORE_HTTPS_PORT=64961" -w "C:\app" -p 64961:64961 testexchangeapi:dev
   ```
5. Start proccess in container
   ```shell
	docker exec 
	-i 
	-e ASPNETCORE_HTTPS_PORT="64961" 
	-w "C:\app" "C:\Program Files\dotnet\dotnet.exe" 
   ```