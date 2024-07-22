## Foto Storio Microservices
**Foto Storio Microservices** is a .NET e-commerce application built with a microservice architecture using Docker containers.

A 'monolithic' version of the application can be found here: [https://github.com/DavidAJohn/FotoStorio](https://github.com/DavidAJohn/FotoStorio)

---

![Screenshot](https://github.com/DavidAJohn/FotoStorioMicroservices/blob/main/images/fotostorio_store_screenshot.jpg?raw=true "Screenshot")

## Features

- Blazor WebAssembly e-commerce store with a responsive layout created using Tailwind CSS 3
- Basket functionality using a Redis database
- PCI DSS-compliant payment integration using Stripe Elements and JSInterop
- Authentication and authorisation using .NET Core Identity
- API Gateways for the client applications using YARP and Ocelot
- Discount pricing database using gRPC
- Async messaging using RabbitMQ and MassTransit
- Inventory management using PostgreSQL and async messaging
- Centralised, structured logging using Serilog and Seq

## Getting Started

To run the application locally, make sure you have Docker Desktop [installed](https://docs.docker.com/desktop/windows/install/) and running on your system.

After downloading or cloning the repository, you'll need to rename the '.env.sample' file in the **'fotostorio-microservices'** folder to '.env' and update the placeholder values with your own values.

Once the .env file configuration is complete, open a terminal inside the application's source folder (**'fotostorio-microservices'**)  and run the following command:

`docker-compose --profile clientapps up -d`

This may take a few minutes, depending on whether or not you already have some or all of the Docker images downloaded. 

The application features a SQL Server with four databases which involves a 1.4Gb download just for the image itself.

With all application services running, Docker will need at least 4Gb RAM, preferably 6-8Gb if you can spare it.

Once all of the containers are up and running, visit [http://localhost:8000](http://localhost:8000) in your browser to view the store.

The Blazor Server Admin site will be available at [http://localhost:8020](http://localhost:8020).

The Health Checks/Application Status site will be available at [http://localhost:8100/hc-ui](http://localhost:8100/hc-ui).

The [Seq](https://datalust.co/seq) logging site will be available at [http://localhost:8200](http://localhost:8200).

## Application Architecture

The following diagram illustrates the structure of the application and gives an overview of how each of these microservices interact.

![Screenshot](https://github.com/DavidAJohn/FotoStorioMicroservices/blob/main/images/FotoStorio_architecture_diagram.png?raw=true "Screenshot")

On the left-hand side we have the Blazor client applications which provide the user interfaces for our customers and admins to interact with the application. These client applications in turn send requests to a particular gateway which is configured to communicate with a sub-set of the backend services in order to retrieve the data or perform the functions requested by its client application. These services communicate with each other by sending and receiving asynchronous messages to and from the Event Bus.

## Development Goals

The main aim of this application is to offer an example of how an ecommerce application could be split up using a microservice architecture, with each service running in its own Docker container. It is NOT intended to be a production-ready, turnkey enterprise solution that can immediately be deployed to Azure or AWS.

Primarily, my intention was to explore this type of architecture and to develop my knowledge of the different types of interaction required between the different elements. As an experienced .NET developer, I was familiar with many of the more common parts of the application such as Web API, Blazor, SQL Server and Entity Framework, but less familiar with others like minimal APIs, gRPC, Ocelot and async messaging with RabbitMq and Mass Transit. I've learned a great deal about the latter topics and almost as much about the former ones and how they need to be adapted to function well within this type of architecture.

Inevitably, there are structural similarities with other microservice applications that you may be familiar with, such as Microsoft’s eShopOnContainers reference application, although this is not a fork and there is no code taken directly from Microsoft’s application.

FotoStorio offers a significant divergence from Microsoft’s method of dealing with ordering and payment processing. Given the justifiably onerous burden that the payment card industry places on anyone handling card details, it seems unwise to promote storing those details locally, even within a reference application. Instead of doing this, FotoStorio uses Stripe to process card payments, therefore avoiding any issues with PCI DSS compliance.

Another key intention was to follow best practice as closely as possible, unless it imposed a significant barrier to entry, particularly for developers who may not have a great deal of experience using Docker or setting up SSL certificates, for example. One particular area where this will be apparent to experienced developers is in the use of ASP.NET Identity rather than an implementation of IdentityServer. This doesn't provide the typical security functions like OAuth2 or OpenIdConnect, but is simpler to set up and understand. This was a concession for the sake of developer convenience.

Emphasis has been placed on client applications built using single page application frameworks like Microsoft’s Blazor framework rather than MVC. The main store is built using standalone Blazor WebAssembly, whereas site admin functionality is provided by a separate Blazor Server app. The client applications have been styled using Tailwind CSS. While it may still be a controversial choice with some developers it has quickly become my preferred choice of CSS framework, particularly when building responsive layouts, where it becomes a huge time-saver. I can't remember another piece of development technology that moved so quickly from repulsive to near-essential in my estimations.

Most of the microservices also have sample data seeded into them when they first run, using a code-first approach with Entity Framework. This was to allow a developer to immediately have a functional store without having to add products manually. The images used throughout the application can be downloaded as a single (5Mb) zip file [here](https://1drv.ms/u/s!At8F4UY7yLDf-mOTqdyhqFn2KSFe?e=JboIZ2).
