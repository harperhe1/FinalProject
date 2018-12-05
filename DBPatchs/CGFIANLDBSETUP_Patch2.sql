drop table GameAndChillDB.dbo.Questions
GO
create table GameAndChillDB.dbo.Questions(
	ID int primary key,
	Name nvarchar(100) not null
)
GO
create table GameAndChillDB.dbo.Answers(
	QuestionID int not null references GameAndChillDB.dbo.Questions(ID),
	UserID int foreign key references GameAndChillDB.dbo.Users(ID) not null,
	Answer int,
	Constraint PK_Question primary key(QuestionId,UserID)
)
GO
create table GameAndChillDB.dbo.Question_Genre(
	QuestionID int foreign key references GameAndChillDB.dbo.Questions(ID) not null,
	GenreID int foreign key references GameAndChillDB.dbo.Genres(ID) not null,
	Constraint PK_Question_Genre primary key(QuestionID,GenreID)
)
GO

insert into GameAndChillDB.dbo.Questions(ID,Name) values(1,'What Disney Princess are you');
insert into GameAndChillDB.dbo.Questions(ID,Name) values(2,'What Super Power do you have?');
insert into GameAndChillDB.dbo.Questions(ID,Name) values(3,'Favorite Day of the week?');
insert into GameAndChillDB.dbo.Questions(ID,Name) values(4,'Favorite Sport?');
insert into GameAndChillDB.dbo.Questions(ID,Name) values(5,'What He-Man villian would you be?');
