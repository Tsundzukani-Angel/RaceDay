
if DB_ID('RaceDayDB') is not null
begin 
   drop database RaceDayDB;
end
go

create database RaceDayDB;
go

use RaceDayDB;
go
-- Role table--
create table Role
(
  RoleID int identity(1,1) primary key,
  RoleName varchar(20) not null unique
);
go

--User table--
create table [User]
(
  UserID int identity(1,1) primary key,
  RoleID int not null,
  FirstName varchar(50) not null,
  LastName varchar(50) not null,
  Email varchar(100) not null,
  PasswordHash varchar(255) not null,
  PhoneNumber varchar(20),
  CreatedDate datetime default getdate(),
  constraint fk_User_Role
     foreign key(RoleID)
     references Role(RoleID)
);
go

alter table [User]
add constraint UQ_User_Email unique (Email);
go

--Profile table--
create table Profile
(
  ProfileID int identity(1,1) primary key,
  UserID int not null unique, --unique becuase a user should only have one profile. (1:1)--
  DateOfBirth Date,
  Gender varchar(20),
  EmergencyContact varchar(100),
  MedicalNotes varchar(225),
  constraint fk_Profile_User
      foreign key (UserID)
      references [User](UserID)
);
go

--Event table--
create table Event
(
  EventID int identity(1,1) primary key,
  OrganiserID int not null,
  Name varchar(100) not null,
  Description varchar(500),
  EventDate date not null,
  Location varchar(100) not null,
  Distance decimal(5,2) not null,
  EventType varchar(20) not null,
  MaxParticipants int not null,
  RegistrationDeadline date not null,
  Status varchar(20) not null default 'Open',
  constraint fk_Event_Organiser
     foreign key (OrganiserID)
     references [User](UserID),
  
  constraint chk_EventType
     check (EventType in ('Run','Walk','Cycle')),

  constraint chk_EventStatus
     check (Status in ('Open','Closed','Completed'))
);
go

alter table Event
add constraint CHK_Event_Distance
check (Distance > 0);
go

alter table Event
add constraint CHK_Event_MaxParticipants
check (MaxParticipants > 0);
go

--Category table--
create table Category
(
  CategoryID int identity(1,1) primary key,
  EventID int not null,
  CategoryName varchar(50) not null,
  CategoryType varchar(20) not null,
  EntryFee decimal(10,2) not null default 0,
  constraint fk_Category_Event
     foreign key (EventID)
     references Event(EventID)
);
go

alter table Category 
add constraint CHK_Category_EntryFee
check (EntryFee >= 0);
go

--Enrolment table--
create table Enrolment
(
  EnrolmentID int identity(1,1) primary key,
  ParticipantID int not null,
  EventID int not null,
  CategoryID int not null,
  RegistrationDate datetime not null default getdate(),
  RaceNumber int not null,
  PaymentStatus varchar(20) not null default 'Pending',
  constraint fk_Enrolment_User
     foreign key (ParticipantID)
     references [User](UserID),

  constraint fk_Enrolment_Category
      foreign key (CategoryID)
      references Category(CategoryID),

   constraint chk_PaymentStatus
       check (PaymentStatus in ('Pending','Paid'))
);
go

alter table Enrolment
add constraint fk_Enrolment_Event
  foreign key (EventID)
  references Event (EventID);
go

alter table Enrolment 
add constraint UQ_Enrolment_Participant_Event
unique (ParticipantID, EventID);

--Result Table--
create table Result
(
  ResultID int identity(1,1) primary key,
  EnrolmentID int not null,
  FinishTime time,
  Position int,
  Status varchar(20),
  constraint fk_Result_Enrolment
      foreign key (EnrolmentID)
      references Enrolment(EnrolmentID),

   constraint chk_ResultStatus
       check (Status in ('Completed','DNF','Disqualified'))
);
go

alter table Result
add constraint CHK_Result_Position
check (POsition > 0);
go

--inserting roles into role table--
insert into Role(RoleName)
values('Organiser'), ('Participant');
go
select * from Role;

--populating user table--
insert into [User]
(RoleID, FirstName, LastName, Email, PasswordHash, PhoneNumber)
values
(1, 'Sarah', 'Mokeana',
 'sarah@raceday.co.za', 'Password123', '0829653012'),
(1, 'David', 'Naidoo',
 'david@raceday.co.za', 'Password123', '0836124569'),
(2, 'Lerato', 'Dlamini',
 'lerato@gmail.com', 'Password123', '0739154020'),
(2, 'Sipho', 'Nkosi',
 'sipho@gmail.com', 'Password123', '0631057820');
go
select * from [User];

 --populating profile table--
insert into Profile(UserID, DateOfBirth, Gender, EmergencyContact, MedicalNotes)
values
(1, '1985-03-10', 'Female',
 '0829991111', 'None'),
(2, '1982-11-12', 'Male',
 '0838882222', 'High Blood Pressure'),
(3, '2002-06-15', 'Female',
 '0725554444', 'Asthma'),
(4, '1999-09-01', 'Male', 
 '0713332222', 'None');
go
select * from Profile;

--populating the event table--
insert into Event(
 OrganiserID, Name, Description, EventDate, Location,
 Distance, EventType,MaxParticipants, RegistrationDeadline, Status)
values(
 1, 'Comrades Marathon',
 'Ultra marathon between Pietermaritzburg and Durban',
 '2027-06-13', 'KwaZulu-Natal', 89.00, 'Run', 25000, 
 '2027-05-01', 'Open'),

 (2, 'Cape Town Cycle Tour', 'Annual cycling event',
 '2027-03-14', 'Cape Town', 109.00, 'Cycle', 35000, '2027-02-15', 'Open'),

 (1, 'Soweto Charity Walk', 'Community fundraising walk',
 '2027-09-04', 'Soweto', 10.00, 'Walk', 5000, '2027-08-20', 'Open');
 go
 select * from Event;
 
--populating the category table--
insert into Category(
  EventID, 
  CategoryName,
  CategoryType, 
  EntryFee)
values(2, 'Senior', 'Age', 450),
 (2, 'Veteran', 'Age', 450),
 (3, '109 km', 'Distance', 650),
 (4, '10 km', 'Distance', 150);
 go
 select * from Category;

 --Populating Enrolment--
insert into Enrolment
( ParticipantID,
  EventID,
  CategoryID,
  RaceNumber,
  PaymentStatus
)
values 
 (3, 2, 3, 1001, 'Paid'),
 (4, 3, 5, 2050, 'Paid'),
 (3, 4, 6, 3010, 'Pending');
go
select * from Enrolment;

--verifying the relationships--
select 
en.EnrolmentID,
u.FirstName + ' ' + u.LastName as Participant,
e.Name as Event,
c.CategoryName as Category,
en.Racenumber,
en.PaymentStatus
from Enrolment en
inner join [User] u
 on en.ParticipantID = u.UserID
inner join Event e
 on en.EventID = e.EventID
inner join Category c
 on en.CategoryID = c.CategoryID;

insert into Result
 ( EnrolmentID, 
   FinishTime,
   Position,
   Status
 )
values 
 ( 3, '06:45:18', 120, 'Completed'),
 (4, '03:22:40', 58, 'Completed');
go
select * from Result;

select
 r.ResultID,
 u.FirstName + ' ' + u.LastName as Participant,
 e.Name as Event,
 c.CategoryName as Category,
 r.FinishTime,
 r.Position,
 r.Status
from Result r
inner join Enrolment en
 on r.EnrolmentID = en.EnrolmentID
inner join [User] u
 on en.ParticipantID = u.UserID
inner join Event e
 on en.EventID = e.EventID
inner join Category c
 on en.CategoryID = c.CategoryID
order by r.Position;

select 'Role' as TableName, count(*) as RecordCount from Role
union all
select 'User', count(*) from [User]
union all
select 'Profile', count(*) from Profile
union all
select 'Event', count(*) from Event
union all
select 'Category', count(*) from Category
union all
select 'Enrolment', count(*) from Enrolment
union all
select 'Result', count(*) from Result;

select * from Role;
select * from [User];
select * from Profile;
select * from Event;
select * from Category;
select * from Enrolment;
select * from Result;