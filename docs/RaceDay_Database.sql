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
create table [user]
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