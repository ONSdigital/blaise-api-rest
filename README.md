# Blaise RESTful API

The Blaise RESTful API provides a lightweight RESTful wrapper around our custom Blaise API NuGet library. It enables developers to interact with Blaise through standard HTTP methods, offering a more accessible and platform-agnostic integration.

This service exposes Blaise functionality as RESTful endpoints, making it easy to perform CRUD operations using common HTTP methods such as `GET`, `POST`, `PUT`, `PATCH`, and `DELETE`.

The API includes an interactive Swagger UI for exploring and testing endpoints directly in your browser.

The RESTful API is powered by our [Blaise NuGet API library](https://github.com/ONSdigital/blaise-nuget-api
), which provides a simplified and intuitive abstraction over the official Blaise API NuGet package.

## Available Endpoints

### `cases`

This endpoint offers the ability to create, update and delete cases for a questionnaire on a server park in Blaise. You may also retrieve a list of case identifiers for a questionnaire and a status of a case.

### `cati`

This endpoint offers the ability to retrieve a list of all questionnaires installed on a server park in Blaise, as well as details for a specific questionnaire. You can also retrieve details of daybatches and survey days configured for a questionnaire and the ability to create daybatches and survey days.

### `health`

This endpoint offers the ability to check the health status of Blaise.

### `ingest`

This endpoint offers the ability to ingest questionnaire data and append it to our existing questionnaire data.

### `questionnaires`

This endpoint offers the ability to install and uninstall a questionnaire on a server park in Blaise, as well as retrieving a list of questionnaires installed.

### `serverparks`

This endpoint offers the ability to retrieve a list of server parks configured for a Blaise environment.

### `users`

This endpoint offers the ability to create, update or delete a user in a Blaise environment, as well as retrieving a list of existing users.

### `userroles`

This endpoint offers the ability to create, update or delete roles for a user in a Blaise environment, as well as retrieving a list of existing user roles and their permissions.

## Local Setup

Run Visual Studio in administrator mode.

To run the service locally, you must provide the necessary connection details for a Blaise environment. You can achieve this in two ways:

- **Populate `App.config`:** Update the `App.config` file with the required Blaise connection details.
- **Use Environment Variables:** Alternatively, you can use `setx` commands to set environment variables. This is a safer way to handle sensitive data. For example: `setx ENV_BLAISE_SERVER_HOST_NAME=blah /m`.

⚠️ **Important:** Never commit `App.config` files with populated secrets or credentials to source control. To safely commit your changes without including the `App.config` file, you can use the command: `git add . ':!app.config'`.

**Connecting to a Blaise Environment:** The service needs to communicate with Blaise on two specific ports which are defined in the `App.config` file. To connect to a Blaise environment deployed on Google Cloud Platform (GCP), you can open IAP tunnels to the virtual machines.

```bash
gcloud auth login

gcloud config set project ons-blaise-v2-<env>

gcloud compute start-iap-tunnel blaise-gusty-mgmt 8031 --local-host-port=localhost:8031

gcloud compute start-iap-tunnel blaise-gusty-mgmt 8033 --local-host-port=localhost:8033
```

Run the service.

Swagger should now be available locally at http://localhost/swagger.

## Tests

Behaviour and unit tests are in a separate top level "Tests" folder.

Tests can be run via the Visual Studio IDE or via the `dotnet test` command.

## Coding Standards

The project enforces a strict set of coding and formatting rules via an .editorconfig file, which is used by StyleCop. Builds may error or issue warnings if these standards are not followed. You can use dotnet format to automatically fix some formatting issues.