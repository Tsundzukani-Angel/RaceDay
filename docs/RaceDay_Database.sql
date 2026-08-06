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
