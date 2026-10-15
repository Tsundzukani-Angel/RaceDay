# RaceDay
## PART 1
## 1. Project Overview
RaceDay is a full-stack web-based event management system designed for the South African road running, walking, and cycling community. The system is designed to help organisers manage events, categories, participant enrolments and race results, while allowing participants to browse events, enter events and tack their personal performance history. The project is being developed across three parts, covering system planning and database developments, API development, and an MVC-based user interface.

## 2. User Roles
RaceDay supports two different user roles.

### Organiser
Organisers can:
- Create events
- Update events
- Delete events
- Manage event categories
- View event enrolments
- Capture particpant results
- Manage their own profile

### Participant
Participants can:
- Register to availlable events
- Browse available events
- Enter events by selecting a category
- View their own enrolements
- View their personal results
- Manage their own profile
Role-base access control will be enforced at the API level in Part 2 and reflected in the MVC interface in Part 3.

## 3. Part 1 technologies
- Microsoft SQL Server
- SQL Server Management Studio(SSMS)
- Git
- GitHub
- GitHub Actions
- Markdown

## 4. Database
The RaceDay database contains the following entities:
- Role
- User
- Profile
- Event
- Catgory
- Enrolment
- Result
The database includes primary keys, foreign keys, constraints and sample data for testing. The database script is located in: '/docs/RaceDay_Database.sql'

## 5. Project Documentation
The Part 1 documentation is located in the '/docs' folder.
| Documentation | Description |
| --- | --- |
| 'RaceDayERD.drawio_1.png' | Entity Relationship Diagram for the RaceDay database |
| 'API_Endpoint_Plan.md' | Planned API endpoints for Part 2 |
| 'RaceDay_Database.sql' | SQL Server database schema and sample data |

## 6. CI/CD
GitHub Actions is used to validate the RaceDay repository structure. The workflow checks that the required documentation and project files are present in the repository. The workflow file is located at: '.github/workflows/ci.yml'
### Successful CI Build
<img width="644" height="487" alt="Screenshot 2026-08-31 000157" src="https://github.com/user-attachments/assets/01b9ea00-b5b4-4d50-8dee-10b984fa575c" />

## 7. Setup Instructions
### Database Setup
1. Open the SQL Server Management Studio(SSMS)
2. Open '/docs/RaceDay_Database.sql'.
3. Connect to a SQL Server instance.
4. Execute the script.
5. The script creates the 'RaceDayDB' database.
6. The script creates all required tables and constraints.
7. The script inserts sample data for testing.

YouTube Link: https://youtu.be/KajMtqlY5C4

## Part 2: RESTful API

## Overview
RaceDay is a web-based event management platform designed for South African road running, walking, and cycling events.

Part 2 focuses on developing the backend RESTful API using ASP.NET Core Web API and .NET 10. The API allows organisers to manage events, categories, enrolments, and race results, while participants can browse events, register for races, and access their personal race information. The API uses Entity Framework Core to interact with SQL Server and implements authentication, session management, and role-based access control. Automated tests are written using xUnit, WebApplicationFactory, and Entity Framework Core InMemory. A GitHub Actions workflow is used to build the project and run the automated tests.

## Technologies Used

| Component | Technology |
|---|---|
| Framework | ASP.NET Core Web API |
| Runtime | .NET 10 |
| Programming Language | C# |
| Database | SQL Server LocalDB |
| ORM | Entity Framework Core 10 |
| Authentication | Session-based authentication |
| Password Hashing | SHA-256 (current implementation) |
| API Documentation | Swagger / OpenAPI |
| Testing Framework | xUnit |
| API Integration Testing | WebApplicationFactory |
| Test Database | Entity Framework Core InMemory |
| CI/CD | GitHub Actions |
| Version Control | Git and GitHub |

## User Roles

RaceDay supports two user roles.

### 1. Organiser

Organisers can:

- Register and log in.
- Create, update, and delete their own events.
- Create and manage event categories.
- View enrolments for their events.
- Update enrolment payment statuses.
- Assign race numbers where supported.
- Record race results, including finish times, positions, and result statuses.

### 2. Participant

Participants can:

- Register and log in.
- Browse available events.
- View event details and categories.
- Enrol in events by selecting a category.
- View their own enrolments.
- View their race history and results.
- Update their personal profile.

The API is designed to restrict access to operations according to the user's role and ownership of the relevant resources.

## Project Structure

```text
RaceDay/
├── .github/
│   └── workflows/
│       └── ci.yml
├── docs/
│   ├── RaceDay_ERD.png
│   ├── API_Endpoint_Plan.md
│   └── RaceDay_Database.sql
├── RaceDay.API/
│   ├── RaceDay.API/
│   │   ├── Controllers/
│   │   ├── Data/
│   │   │   └── RaceDayDbContext.cs
│   │   ├── DTOs/
│   │   ├── Models/
│   │   ├── Services/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   └── RaceDay.API.Tests/
├── .gitignore
└── README.md
```

The API project contains the controllers, database context, entity models, DTOs, and services. The test project contains automated tests for API functionality and access control. The `docs` directory contains the database design, endpoint plan, and SQL script developed during Part 1.

## Prerequisites

Before running the project, install:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server LocalDB
- Visual Studio with ASP.NET and web development support, or another compatible .NET development environment
- Git

The Entity Framework Core CLI tool is required if you want to manage migrations using `dotnet ef`.

Install it with:
```bash
dotnet tool install --global dotnet-ef
```

If it is already installed, update it instead using:

```bash
dotnet tool update --global dotnet-ef
```

## Getting Started

### 1. Configure the Database Connection

Configure the `RaceDayConnection` connection string in:

`RaceDay.API/RaceDay.API/appsettings.json`

Example for SQL Server LocalDB:

```json
{
  "ConnectionStrings": {
    "RaceDayConnection": "Server=(localdb)\\MSSQLLocalDB;Database=RaceDayDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```
This example is for local development. Do not commit passwords, API keys, or other sensitive configuration values to GitHub.

### 2. Set Up the Database

The project uses Entity Framework Core to manage database access.

If setting up a fresh database through migrations, open a terminal in the API project directory:

```bash
cd RaceDay.API/RaceDay.API
dotnet ef database update
```

This applies the migrations included in the project to the database specified by the connection string.

**Important:** If you already created `RaceDayDB` using the Part 1 SQL script, check that the migration history and database schema are compatible before applying migrations. Do not assume that migrations can safely recreate tables that already exist.

If you are using the original Part 1 database, run the SQL script in `docs/RaceDay_Database.sql` when appropriate for your setup.

### 3. Run the API
From the API project directory, run:

```bash
dotnet run
```
Use the local URL displayed in the terminal.

For example, the application may run at:

- HTTPS: `https://localhost:7112`
- HTTP: `http://localhost:5067`

The actual ports depend on your launch configuration.

### 4. Explore the API with Swagger

Open the Swagger interface using the HTTPS URL displayed by the application, followed by `/swagger`.

For example:

```text
https://localhost:7112/swagger
```
Swagger provides an interactive interface for viewing and testing the available API endpoints. During local development, your browser may display a certificate warning for the development HTTPS certificate.

## API Endpoints

The API is organised into the following endpoint groups.
| Group | Example Endpoints | Purpose |
|---|---|---|
| Authentication | `POST /api/auth/register` | Register a user |
| Authentication | `POST /api/auth/login` | Log in |
| Authentication | `POST /api/auth/logout` | Log out |
| Authentication | `GET /api/auth/me` | Retrieve the current session's user information |
| Users / Profile | `GET /api/users/profile` | Retrieve the user's profile |
| Users / Profile | `PUT /api/users/profile` | Update the user's profile |
| Events | `GET /api/events` | Browse events |
| Events | `GET /api/events/{id}` | Retrieve an event |
| Events | `GET /api/events/my-events` | Retrieve the organiser's events |
| Events | `POST /api/events` | Create an event |
| Events | `PUT /api/events/{id}` | Update an event |
| Events | `DELETE /api/events/{id}` | Delete an event |
| Categories | `GET /api/categories/event/{eventId}` | Retrieve categories for an event |
| Categories | `POST /api/categories` | Create a category |
| Categories | `DELETE /api/categories/{id}` | Delete a category |
| Enrolments | `POST /api/enrolments` | Enrol in an event |
| Enrolments | `GET /api/enrolments/my` | Retrieve the participant's enrolments |
| Enrolments | `GET /api/enrolments/event/{eventId}` | Retrieve enrolments for an event |
| Enrolments | `PUT /api/enrolments/{id}/status` | Update enrolment status |
| Results | `POST /api/results` | Record a race result |
| Results | `GET /api/results/my` | Retrieve the participant's results |
| Results | `GET /api/results/event/{eventId}` | Retrieve results for an event |

The table summarises the planned endpoint structure. Consult Swagger for the actual routes and HTTP methods implemented in the project.

For more detailed endpoint planning, refer to the API endpoint documentation in the `docs` directory.

## Testing

The project includes a separate xUnit test project:

`RaceDay.API/RaceDay.API.Tests`

The tests use WebApplicationFactory to exercise the API and Entity Framework Core InMemory to provide an isolated test database.

To run the tests, open a terminal at the repository root and execute:

```bash
dotnet test RaceDay.API/RaceDay.API.Tests/RaceDay.API.Tests.csproj
```
The test suite is intended to verify functionality such as:

- User registration and login.
- Validation of registration details.
- Event retrieval.
- Event creation and role restrictions.
- Participant enrolments.
- Access to user-specific information.
- Responses to unauthenticated or unauthorised requests.
## Continuous Integration

RaceDay uses GitHub Actions to automate the build and test process.

The workflow is located at:

`.github/workflows/ci_Part2.yml`

The workflow can be configured to:

1. Set up the required .NET SDK.
2. Restore NuGet packages.
3. Build the solution.
4. Run automated tests.
5. Publish test results or artifacts when configured.

### CI Build Status

<img width="662" height="585" alt="Screenshot 2026-10-13 000418" src="https://github.com/user-attachments/assets/72cd0a4c-9f0d-4cdc-93aa-ee90fc770696" />

<img width="651" height="621" alt="Screenshot 2026-10-12 235859" src="https://github.com/user-attachments/assets/851d3d45-b0c2-4cc4-8f40-badd11343720" />

## Security Considerations

The API uses session-based authentication and role-based access checks.

The current password-hashing implementation uses SHA-256 with a fixed salt. This is suitable only as a description of the current implementation, not as a recommendation for production password storage. A password-specific hashing algorithm, such as ASP.NET Core Identity's password hasher, should be used before deploying a real application. For production deployment, additional security measures should include HTTPS, secure session-cookie settings, appropriate session expiration, input validation, and secure configuration management. 

## Version Control

Git was used to track the development of the RaceDay API throughout Part 2. The commit history records changes to the database models, DbContext, migrations, services, DTOs, controllers, tests, and CI workflow.

To view the commit history, run:

```bash
git log --oneline
```

## Part 3
