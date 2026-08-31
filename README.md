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

## Part 2


