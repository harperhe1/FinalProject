create database GameAndChillDB
Go
create table GameAndChillDB.dbo.Users(
	ID int primary key IDENTITY(1,1),
	Name nvarchar(50) not null

)
create table GameAndChillDB.dbo.Games(
	ID int primary key,
	Name nvarchar(MAX) not null,
	Summary nvarchar(MAX),
	URL nvarchar(MAX),
	ImageURL nvarchar(MAX)
	)
create table GameAndChillDB.dbo.Genres(
	ID int primary key,
	Name nvarchar(50) not null
)
create table GameAndChillDB.dbo.Keywords(
	ID int primary key,
	Name nvarchar(50) not null
)
create table GameAndChillDB.dbo.Platforms(
	ID int primary key,
	Name nvarchar(50) not null
)
GO
create table GameAndChillDB.dbo.Questions(
	ID int not null,
	UserID int foreign key references GameAndChillDB.dbo.Users(ID) not null,
	Answer int,
	Constraint PK_Question primary key(Id,UserID)
)
create table GameAndChillDB.dbo.User_Game(
	UserID int foreign key references GameAndCHillDB.dbo.Users(ID) not null,
	GameID int foreign key references GameAndCHillDB.dbo.Games(ID) not null,
	IsLike bit not null,
	Constraint PK_User_Game primary key(UserID,GameID)
)
create table GameAndChillDB.dbo.Genre_Game(
	GenreID int foreign key references GameAndCHillDB.dbo.Genres(ID) not null,
	GameID int foreign key references GameAndCHillDB.dbo.Games(ID) not null,
	Constraint PK_Genre_Game primary key(GenreID,GameID)
)
create table GameAndChillDB.dbo.Keyword_Game(
	KeywordID int foreign key references GameAndCHillDB.dbo.Keywords(ID) not null,
	GameID int foreign key references GameAndCHillDB.dbo.Games(ID) not null,
	Constraint PK_Keyword_Game primary key(KeywordID,GameID)
)
create table GameAndChillDB.dbo.Platform_Game(
	PlatformID int foreign key references GameAndCHillDB.dbo.Platforms(ID) not null,
	GameID int foreign key references GameAndCHillDB.dbo.Games(ID) not null,
	Constraint PK_Platform_Game primary key(PlatformID,GameID)
)
Go