## Foto Storio Microservices
**Foto Storio Microservices** is a .NET e-commerce application built with a microservice architecture using Docker containers.

---

![Screenshot](https://i.ibb.co/02zsbbc/fotostorio-screenshot.jpg "Screenshot")

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

`docker-compose -f docker-compose.yml -f docker-compose.override.yml up --build -d`

This may take a few minutes, depending on whether or not you already have some or all of the Docker images downloaded. 

The application features a SQL Server with four databases which involves a 1.4Gb download just for the image itself.

With all application services running, Docker may need 4Gb RAM.