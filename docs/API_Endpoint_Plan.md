# RaceDay API Endpoint Plan

## 1. Introduction
This documents defines the API endpoints planned for the RaceDay event management system. The endpoint plan covers authentication, user profiles, events, category, event enrolments and results.

The API will support two user Roles:

- **Organiser**: Manages events, categories and participant results and views event enrolments.
- **Participant**: Browses events, enters events, views their enrolments and tracks their personal results.

Role based access control will be enforced at the API level in Part2.

## 2. Authentication
| HTTP Method | Route | Description| Role | Request Body | Expected Response |
| --- | --- | --- | --- | --- | --- |
| POST | /api/auth/register | Register a new user | Public | FirstName, LastName, Email, Password, PhoneNumber, RoleId | 201 created with user details| 
| POST | /api/auth/login | Authenticate a user | Public | Email, Password | 200 OK with authentication taken and user information |

## 3. User Profile
| HTTP Method | Route | Description| Role | Request Body | Expected Response |
| --- | --- | --- | --- | --- | --- |
| GET | /api/profile/me | view logged-in user's profile | Organiser/Participant | None | 200 OK with profile | 
| PUT | /api/profile/me | Update logged-in user's profile | Organiser/Participant | DateOfBirth, Gender, EmergencyContact, MedicalNotes | 200 OK with updated profile |

## 4. Events
| HTTP Method | Route | Description| Role | Request Body | Expected Response |
| --- | --- | --- | --- | --- | --- |
| GET | /api/events | View all events | Organiser/Participant | None | 200 OK list of events |
| GET | /api/events/{id} | View one event | Organiser/Participant | None |200 OK event |
| POST | /api/events | Create an event | Organiser | Event details | 201 Created |
| PUT | /api/events/{id} | Update an event | Organiser | Event details | 200 OK | 
| DELETE | /api/events/{id} | Delete an event | Organiser | None | 204 No Content |

## 5. Categories
| HTTP Method | Route | Description| Role | Request Body | Expected Response |
| --- | --- | --- | --- | --- | --- |
| GET | /api/events/{eventId}/categories |	View categories for an event | Organiser/Participant | None | 200 OK list |
| POST | /api/events/{eventId}/categories | Create category for event |	Organiser| CategoryName, CategoryType, EntryFee| 201 Created |
| PUT | /api/categories/{id} | Update category | Organiser | Category details | 200 OK |
| DELETE | /api/categories/{id} | Delete category | Organiser| None | 204 No Content |

## 6. Event Enrolments
| HTTP Method | Route | Description| Role | Request Body | Expected Response |
| --- | --- | --- | --- | --- | --- |
| POST | /api/enrolments | Enter an event | Participant | EventID, CategoryID | 201 Created |
| GET | /api/enrolments/me | View participant's own enrolments | Participant | None	| 200 OK list |
| GET | /api/events/{eventId}/enrolments | View enrolments for an event	| Organiser | None | 200 OK list |

## 7. Results
| HTTP Method | Route | Description| Role | Request Body | Expected Response |
| --- | --- | --- | --- | --- | --- |
| POST | /api/results | Capture participant result | Organiser | EnrolmentID, FinishTime, Position, Status | 201 Created |
| PUT | /api/results/{id} | Update result | Organiser | FinishTime, Position, Status | 200 OK |
| GET | /api/results/me	| View own results | Participant | None | 200 OK list |
| GET | /api/events/{eventId}/results | View results for an event | Organiser | None | 200 OK list |
