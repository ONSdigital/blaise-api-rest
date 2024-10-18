# Blaise RESTful Web API

This service provides a thin RESTful (representational state transfer) wrapper around our custom Blaise API library.

## Blaise API library
https://github.com/ONSdigital/blaise-nuget-api

The Blaise API library offers an abstraction of the interfaces provided by the official Blaise API DLL library to offer a high level CRUD style interface that is easier to work with. This library is available internally to the ONS via a NuGet repository and uses .Net framework.

## Usage
Where the API library is .Net framework specific, the RESTful web API provides a language and infrastructure agnostic approach to accessing Blaise resources.

Blaise resources can get accessed via standard GET, POST, PUT, PATCH and DELETE requests over HTTPS, and the web API offers a swagger (https://swagger.io) UI interface which allows for easy testing of endpoints.

## Available resources

### Cases 
This endpoint offers the ability to create, update and delete cases for a questionnaire on a server park in Blaise. You may also retrieve a list of Case identifiers for a questionnaire and a status of a case.

### Cati
This endpoint offers the ability to retrieve a list of all questionnaires installed on a server park in Blaise, as well as details for a specific questionnaire. You can also retrieve details of daybatches and survey days configured for a questionnaire and the ability to create daybatches and survey days.

### Questionnaire
This endpoint offers the ability to install and uninstall an questionnaire on a server park in Blaise, as well as retrieving a list of questionnaires installed.

### Server park
This endpoint offers the ability to retrieve a list of server parks configured for a Blaise environment.

### User
This endpoint offers the ability to create, update or delete a user in a Blaise environment, as well as retrieving a list of existing users.

### User Role
This endpoint offers the ability to create, update or delete roles for a user in a Blaise environment, as well as retrieving a list of existing user roles.

## Local setup

Populate the App.config file accordingly, **never commit a populated App.config file!**
Run the application in admin mode.
Swagger should now be available locally at http://localhost:90/swagger

## Tests

Behaviour and unit tests are in a separate top level "Tests" folder.

Behaviour tests need to be run in debug mode so that they use the stubs.

### Note: the behaviour tests will fail until BLAIS5-4046 is completed as the stubs are not working