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

---

## Coding Standard Rules (C#)

This project uses a standardized set of formatting and naming rules to ensure consistency and maintainability in the codebase. These rules are enforced via the `.editorconfig` file.

The Nuget package StyleCop.Analyzers is responsible for auto code-fixing when the 'dotnet format' command is run in terminal. The extensive list of rules which this package can enforce be found here: https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/DOCUMENTATION.md

The editor config contains a mix of rules which only DotNet Format can understand (which the server pipeline relies on) and StyleCop.Analyzers rules which help auto code fix locally (these will have the prefix 'SA' with a number code).

### Formatting Rules (`*.cs`)

#### Indentation & Spacing

* **Spaces, not tabs**: `indent_style = space`
* **Indent size**: `indent_size = 4`
* **Tab width**: `tab_width = 4`
* **Final newline**: Required (`insert_final_newline = true`)
* **Trim trailing whitespace**: `trim_trailing_whitespace = true`

#### Line Endings

* **Windows-style line endings**: `end_of_line = crlf`

#### Curly Braces & Parentheses

* **Brace spacing**: Ignored (`csharp_space_between_braces = ignore`)
* **No space inside parentheses**: `csharp_space_between_parentheses = false`

#### Empty Lines

* **No multiple blank lines allowed**: `dotnet_style_allow_multiple_blank_lines = false`

#### Single-Line Statements

* **Preserve single-line formatting**: `csharp_preserve_single_line_statements = true`

#### Comma Spacing

* **Space after commas**: Yes (`dotnet_style_spacing_after_comma = true`)
* **Space before commas**: No (`dotnet_style_spacing_before_comma = false`)

---

### Miscellaneous C# Formatting

* **Label indentation**: Flush left (`csharp_indent_labels = flush_left`)
* **`using` directive placement**: Outside namespace (`csharp_using_directive_placement = outside_namespace:silent`)
* **Prefer simple `using` statements**: Enabled (`csharp_prefer_simple_using_statement = true:suggestion`)
* **Require braces for blocks**: Yes (`csharp_prefer_braces = true:silent`)
* **Namespace style**: Block scoped (`csharp_style_namespace_declarations = block_scoped:silent`)
* **Prefer method group conversions**: Yes (`csharp_style_prefer_method_group_conversion = true:silent`)
* **Prefer top-level statements**: Yes (`csharp_style_prefer_top_level_statements = true:silent`)
* **Prefer primary constructors**: Yes (`csharp_style_prefer_primary_constructors = true:suggestion`)
* **Prefer `System.Threading.Monitor` lock**: Yes (`csharp_prefer_system_threading_lock = true:suggestion`)
* **Expression-bodied methods**: Disabled (`csharp_style_expression_bodied_methods = false:silent`)

---

### Naming Rules (`*.{cs,vb}`)

#### Interfaces

* **Must begin with "I"**
  Rule: `interface_should_be_begins_with_i`
  Style: `IName` (PascalCase with "I" prefix)

#### Types (classes, structs, interfaces, enums)

* **Must use PascalCase**
  Rule: `types_should_be_pascal_case`
  Style: `TypeName`

#### Non-field Members (methods, properties, events)

* **Must use PascalCase**
  Rule: `non_field_members_should_be_pascal_case`
  Style: `MemberName`

#### Operator Placement

* **Operators placed at the beginning of the line when wrapping**:
  `dotnet_style_operator_placement_when_wrapping = beginning_of_line`

---

### Character Encoding

* **Charset**: UTF-8 (`charset = utf-8`)

---

This configuration promotes a consistent and readable codebase, aligned with modern C# conventions. All contributors should ensure their editors respect this `.editorconfig` file.

---

