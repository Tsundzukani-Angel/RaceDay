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