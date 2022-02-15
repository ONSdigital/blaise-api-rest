# Blaise RESTful Web API

This service provides a thin RESTful (representational state trasfer) wrapper around our custom Blaise API library.

## Blaise API library
(https://github.com/ONSdigital/blaise-nuget-api)

The Blaise API library offers an abstraction of the interfaces provided by the official Blaise API DLL library to offer a high level CRUD style interface that is 
easier to work with. This library is available internally to the ONS via a NUGet repository and uses .Net framework.

## Usage
Where the API library is .Net framework specific, the RESTful web API provides a language and infrastructure agnostic approach to accessing Blaise resources.

Blaise resources can get accessed via standard GET, POST, PUT, PATCH requests over HTTPS, and the web API offers a swagger (https://swagger.io/) UI interface which 
allows for easy testing of endpoints.

