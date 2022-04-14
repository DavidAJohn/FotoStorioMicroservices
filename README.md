## Foto Storio Microservices
**Foto Storio Microservices** is a .NET e-commerce application built with a microservice architecture using Docker containers.

---

![Screenshot](https://github.com/DavidAJohn/FotoStorioMicroservices/blob/main/images/fotostorio_store_screenshot.jpg?raw=true "Screenshot")

## Features

- Blazor WebAssembly e-commerce store with a responsive layout created using Tailwind CSS 3
- Basket functionality using a Redis database
- PCI DSS-compliant payment integration with Stripe, using JSInterop
- Authentication and authorisation using .NET Core Identity
- API Gateways for the store and admin sites using Ocelot
- Discount pricing database using gRPC
- Async messaging using RabbitMQ and MassTransit
- Inventory management using PostgreSQL and async messaging

## Getting Started

To run the application locally, make sure you have Docker Desktop [installed](https://docs.docker.com/desktop/windows/install/) and running on your system.

After downloading or cloning the repository, open a terminal inside the application's source folder **'fotostorio-microservices'**  and run the following command:

`docker-compose --profile clientapps up -d`

This may take a few minutes, depending on whether or not you already have some or all of the Docker images downloaded. 

The application features a SQL Server with four databases which involves a 1.4Gb download just for the image itself.

With all application services running, Docker will need at least 4Gb RAM.

Once all of the containers are up and running, visit [http://localhost:8000](http://localhost:8000) in your browser to view the store.

## Application Architecture

The following diagram illustrates the structure of the application and gives an overview of how each of these microservices interact.

![Screenshot](https://github.com/DavidAJohn/FotoStorioMicroservices/blob/main/images/FotoStorio_architecture_diagram.png?raw=true "Screenshot")

On the left-hand side we have the Blazor client applications which provide the user interfaces for our customers and admins to interact with the application. These client applications in turn send requests to a particular gateway which is configured to communicate with a sub-set of the backend services in order to retrieve the data or perform the functions requested by its client application. These services communicate with each other by sending and receiving asynchronous messages to and from the Event Bus.