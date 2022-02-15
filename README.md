# Blaise RESTful Web API

This service provides a thin RESTful (representational state trasfer) wrapper around our custom Blaise API library.

## Blaise API library
(https://github.com/ONSdigital/blaise-nuget-api)

The Blaise API library offers an abstraction of the interfaces provided by the official Blaise API DLL library to offer a high level CRUD style interface that is 
easier to work with. This library is available internally to the ONS via a NUGet repository and uses .Net framework.

## Usage
Where the API library is .Net framework specific, the RESTful web API provides a language and infrastructure agnostic approach to accessing Blaise resources.

Blaise resources can get accessed via standard GET, POST, PUT, PATCH requests over HTTPS, and the web API offers a swagger (https://swagger.io) UI interface which 
allows for easy testing of endpoints.

## Available resources

# Cases 
This endpoint offers the ability to create, update and delete cases for an instrument on a server park in Blaise. You may also retrieve a list of Case identifiers 
for an instrument and a status of a case.

# Cati
This endpoint offers the ability to retrieve a list of all instruments installed on a server park in Blaise, as well as details for a specific instrument. You
can also retrieve details of daybatches and survey days configured for an instrument and the ability to create daybatches and survey days.

# Instrument
This endpoint offers the ability to install and uninstall an instrument on a server park in Blaise, as well as retrieving a list of instruments installed.

# Server park
This endpoint offers the ability to retrieve a list of server parks configured for a Blaise environment.

# User
This endpoint offers the ability to create, update or delete a user in a Blaise envronment, as well as retrieving a list of exisiting users.

# User Role
This endpoint offers the ability to create, update or delete roles for a user in a Blaise envronment, as well as retrieving a list of exisiting user roles.