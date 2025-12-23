# Soki's PH Tech Assessment

__*Overview: Technical Assessment for Propellerhead.*__  

Sample application for REST Endpoints that include CRUD operations for Costumers, Contacts, Notes. Also includes retrieve operation for Country Catalog and simple User Authentication. Current implementation should run out of the box with included SQLite DB file at root level on Propeller.API


## Table of Contents
* [Quickstart](#quickstart)  
* [Repository Contents](#repository-contents)  
* [How to Run](#how-to-run)  
* [How to Test](#how-to-test)  
* [API Documentation](#api-documentation)
* [Endpoints](#endpoints)
* [Features](#features) 
* [Features I wanted to implement but ran out of time](#features-i-wanted-to-implement-but-ran-out-of-time)
* [Things I wish I would have done different](#things-i-wish-i-would-have-done-different)
* [Full Disclosure](#full-disclosure)
* [Final Words to the Reviewer](#final-words-to-the-reviewer)

## Quickstart  
### Requirements
To run this service you need to have .Net Core 6 or Higher installed

## Repository Contents

* Propeller.API - Contains the main WebAPI application
* Propeller.DALC - Data Access Layer
* Propeller.DALC.Sqlite - Data Implementation for SQLite
* Propeller.Entities - Data Entities
* Propeller.Integration.Tests - Integration Tests
* Propeller.Mappers - Model/Entities Mapper configuration for AutoMapper
* Propeller.Models - DTO Models
* Propeller.Shared - Shared Functionality

__*PropellerHeadTechAssessment.sln:*__ Contains the main Application  
__*PropellerHeadTechAssessment.Tests.sln:*__ Contains Integration Tests  

## How to Run

After cloning the GitHub repository:

1.- Open an Administrator Power Shell Console  
2.- Navigate to the location of the Propeller.API.csproj (Main Application)  
3.- Run the following command: dotnet run  

Alternative:  
1.- Open the PropellerHeadTechAssessment.sln file on Visual Studio 2022  
2.- Verify the Propeller.API is set to run as Startup project  
3.- Press F5  

4.- Be amazed :)  

## How to Test  
In the code base you can find a Folder named "Postman", here you will find sample calls for all endpoints and it's usage. Import the PropellerH.postman_collection file into PostMan. Variables can be used to setup the Auth Tokens across the calls, just remember 3 roles exist, so make sure to use the proper authorization (Admin has access to everything, Power has limited write access -no deletes-, Regular has only read access)

You can also open the PropellerHeadTechAssessment.Tests.sln solution file on Visual Studio and build. This will generate a set of tests to be run from the Test Explorer. It is suggested you install the SpecFlow extension to debug and inspect the Gherkin code easier.

Make sure you specify the proper path for BASEAPIURL to your local instance on the [specflow.json](PropellerHeadTechAssessment/Propeller.Integration.Tests/specflow.json) configuration file.

If done correctly, your tests will run successfully

![image](https://user-images.githubusercontent.com/119035054/205457742-4efbdf7f-94d2-47b9-917b-e2e440efcaa2.png)

## API Documentation

A basic documentation for the API endpoints and it's usage can be found at the following address (when run locally):

https://localhost:port/swagger/index.html

You will need to set port number to whatever port your application is running in
.
## Endpoints  
### Authentication
* [POST] *api/auth/authenticate* : __Authenticates a User__

### Contacts
* [POST] *api/contacts* : __Creates a New Contact__
* [GET] *api/contacts* : __Retrieves Multiple Contacts__
* [GET] *api/contacts/{contactId}* : __Retrieves Single Contact__
* [PUT] *api/contacts/{contactID}* : __Updates a Contact__
* [DELETE] *api/contacts/{contactID}* : __Deletes a Contact__

### Countries
* [GET] *api/countries* : __Retrieves All Countries__

### Customer
* [POST] *api/customers* : __Creates a New Customer__
* [GET] *api/customers* : __Retrieves Multiple Customers__
* [GET] *api/customers/{customerId}* : __Retrieves Single Customer__
* [PUT] *api/customers/{customerId}* : __Updates a Customer__
* [PUT] *api/customers/{customerId}/status/{statusId}* : __Changes a Customer's Status__
* [PATCH] *api/customers/{customerId}* : __Partially Updates a Customer__
* [DELETE] *api/customers/{customerId}* : __Deletes a Contact__

### CustomerStatus
* [GET] *api/status* : __Retrieves All Available Customer Statuses__

### Notes
* [POST] *api/notes/{customerId}* : __Creates a New Note for a given Customer__
* [GET] *api/notes/{customerId}* : __Retrieves All Notes for a Given Customer__
* [GET] *api/notes/{customerId}/{noteId}* : __Retrieves Single Note for a given Customer__
* [PUT] *api/contacts/{customerId}/{noteId}* : __Updates a Customer's Note__
* [DELETE] *api/contacts/{customerId}/{noteId}* : __Deletes a Customer's Note__


## Features
In this solution you will notice pretty interesting implementations / features

* __Roles__  
There exist 3 different Roles for users in the ecosystem: Admin, Power and Regular User

* __Authentication using Bearer Token__  
A users Table was created with sample seed data for each Role type, the Role and Country of the User is embedded into the Bearer Token to be used in the application  

* __Localization__    
Three locales supported (en-NZ, es-MX, fr-FR). The User's token contains a locale claim which is used to set the locale of the API (via a Request Culture Provider: TokenBasedRequestCultureProvider), if the token doesn't contain locale, it can be injected as an Accept-Language header  

* __Role Access Policy__  
This is implemented using Net Core's intrinsic RBAC approach. Regular User only has ReadOnly access, Power User has limited write access and full read access but deletions are restricted, Admin User has full access including deletion

* __Attribute Access Policy__    
Also an implementation for attribute based policies was made to demonstrate the use of middleware, a IsNZLUser policy was created and can restrict access to certain endpoints to users whose Country code is set to NZL

* __Integration Tests__  
A set of integration tests was created using Specflow and Gherkin, I chose this technology because I like the Scenario approach since it allows for non developers (Product) to create their test scenarios and test them in an easily readable format.

* __DB Cardinality__  
The DB contains approaches to use one-to-many and many-to-many relations. The Customer/Contacts association works in a many-to-many relation to demonstrate it's use with EF and how 

* __Multiple Request/Response approaches__  
There are several ways to achieve the same result when it comes to sending/receiving data from the server. For example some people prefer to use routes to handle all Id's while others prefer to send the data via the body (either json for complex objects or string for single values) and yet others might choose to use headers or the Bearer Token, for this demo I implemented several approaches to demonstrate. Pagination data for example will be returned as a custom MetaData header to keep the body of the request containin only domain objects, some places use generic POCOs while other might have a more complex Model Binding (Custom Requests), others use values from the route, querystring, body, headers and Bearer Token

* __StatusCodes__  
I tried to adhere to standard practices when it comes to error codes, as you'll see I implement 201's for successful create operations, 404's for values not found on the DB, 500's for General errors, 204's for successful deletes and so on. 

* __Data Obfuscation__  
You'll notice Exceptions are logged and response for the errors never carry the details of the exception itself but rather ambiguous and generic error descriptions to avoid giving the caller more detail than needed and enhance security. You'll also notice some of the routes use obfuscated strings as parameters even 'tho the Id's n the DB are numeric or Guids, the obfuscation implementation is very simple here, but the concept to demonstrate is the user of hashed values instead of integers to reduce the vulnerability on the endpoints, similar to how sites like youtube handle url's.

* __Minimal use of 3rd party packages__  
I try to stick as much as possible to Native libraries and custom code whenever possible. You'll notice the application uses only a few 3rd party packages and they are used on the Integration Tests project (Specflow, FluentAssertions and NUnit) and one on the main app (AutoMapper). I prefer to use 3rd party libraries only when they provide a clear advantage (Security, Performance) over custom implementations, exploits on 3rd party libraries are very risky and I've seen many companies don't have a proper culture of active monitoring and updating of libraries to diminish risk


## Features I wanted to implement but ran out of time
- OAuth2
- Unit Tests
- DALC Factory: I wanted to implement a SQL Server alternative to the SQLite approach, if you see the project structure you'll notice I use Entity Framework (Code First) and the Schemas are separated from the actual DB Context, this was created so I could generate the migrations for other DB providers and my intent was to implement a factory or strategy pattern to allow switching DB's for development and production environments.
- One way user/pwd encryption
- User lost password / recovery email feature
- Custom Model Binding for Contact setup to validate on binding for Email and/or Phone existance, currently this is performed at implementation level, I wanted to change this to binding level.
- Refactoring: Most code adheres to the Single Responsibility principle, so code duplication should be minimal, still I think some code can be moved over to Base classes and shared across derived classes.
- More Integration Tests

## Things I wish I would have done different
- AutoMapper, I wanted to give this library a try since I had it recommended by a colleage, I got to say I'm not quite convinced, it seems it's prone to errors and I belive it might have some security issues. Setting it up seemed a bit complicated for complex types and debugging mapping errors is quite hard. For a small application like this one it might not be the best idea
- You'll also notice a few TODO's left on the code, I addressed as many as I could but I ran out of time to address them all
- Start this earlier. We had a pretty busy last week at my current job and couldn't dedicate time until last saturday.  

## Full Disclosure  
I had to refresh my knowledge of Entity Frameworks because for the last 4 years I've only worked on NonSQL environments (Mongo) and we use the MongoDriver approach to query the collections so I wasn't familiar with the current .Net Core EF implementation, the last time I used it was back on .Net FW 4.5

## Final Words to the Reviewer  
Please reach out to me if you need any further info regarding the solution, It was really fun to work on this project and I appreciate you dedicating time to review it, if you have any input on what could be performed better please feel free to leave comments on the code or the repo I'd love to get feedback on this. I hope you have as much fun reviewing this as I had coding it, have a good one and Thank you!


