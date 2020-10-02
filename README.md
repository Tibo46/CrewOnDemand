# PERSON API

This API allows you to get the first available pilot for a given departure and return date on a specific location.
Once you have a pilot id, you can book a flight.
You can find a hexagonal diagram of the project in this repository.
The swagger page is accessible from /swagger/index.html
It is build in dotnet core 3.1 and uses Entity Framework Core.
The tests are using xUnit.
Follow this readme to setup the project and run it from your machine.
Note: this was only tested on a Windows machine, but it should work the same way on Mac.

## PREREQUISITE

- [ASP.NET Core Runtime 3.1.x](https://dotnet.microsoft.com/download/dotnet-core/3.1)
- dotnet ef installed globally: `dotnet tool install --global dotnet-ef`
- [Docker](https://www.docker.com/products/docker-desktop) or [SSMS](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15)

## DB SETUP

The DB can be setup in 2 different ways:

- via docker,
- using your own SQL server and running the migrations

### Via Docker

As an example password I am using "TestP@ssW0rd", don't forget to replace it with your desired strong password. You will also have to update it in appsettings.json

- Start Docker Desktop in your machine

- Pull the SQL server image

  `docker pull mcr.microsoft.com/mssql/server:2019-CU3-ubuntu-18.04`

- Make sure to stop any container running in port 1433 as it would conflict with the following command

  `docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=TestP@ssW0rd" -p 1433:1433 --name test-db -d mcr.microsoft.com/mssql/server:2019-CU3-ubuntu-18.04`

- Connect to the container

  `docker exec -it test-db "bash"`

- Connect locally to the DB

  `/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "TestP@ssW0rd"`

- Create the DB

  `CREATE DATABASE CrewOnDemand GO`

- Close the terminal

- Open a terminal from <path to repository>/src

  `dotnet ef database update`

### Using your own DB and migrations

If you do not want to use docker, you can run the migrations against your own database to create the tables

- In your favorite SQL management tool, create a new DB

  `CREATE DATABASE CrewOnDemand`

- Change the connection string from appsettings.json to point to your SqlServer with your credentials

- Open a terminal from <path to repository>/src

  `dotnet ef database update`

## RUN THE PROJECT

- Open a terminal from <path to repository>/src

  `dotnet run`

  The API will be accessible at the following address: https://localhost:5001/
  To test that the API is able to access the DB correctly, navigate to https://localhost:5001//status/health you should see "Healthy"
  You can view the swagger page here: https://localhost:5001/swagger/index.html

## RUN THE UNIT TESTS

- Open a terminal from <path to repository>/tests/CrewOnDemand-UnitTests

  `dotnet test`

## RUN THE INTEGRATION TESTS

- Open a terminal from <path to repository>/tests/CrewOnDemand-IntegrationTests

  `dotnet test`

- Note: the tests are running against a in memory database to avoid polluting the local db
