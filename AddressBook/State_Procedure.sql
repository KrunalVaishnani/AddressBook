--Insert
USE Addressbook;

Create Proc [dbo].[PR_State_Insert]
@CountryID int,
@StateName varchar(100),
@StateCode varchar(50),
@UserID int
As
Begin
	Insert Into [dbo].[State]
	(
		[dbo].[State].[CountryID],
		[dbo].[State].[StateName],
		[dbo].[State].[StateCode],
		[dbo].[State].[UserID] 
	)
	Values
	(
		@CountryID,
		@StateName,
		@StateCode,
		@UserID
	)
End

--Exec PR_State_Insert 2, 'Gujarat', '24', 2
--Select * From [dbo].[State]


--Update

Create Proc [dbo].[PR_State_Update]
@StateID int,
@CountryID int,
@StateName varchar(100),
@StateCode varchar(50),
@UserID int
As
Begin
	Update [dbo].[State]
	Set
		[dbo].[State].[CountryID] = @CountryID,
		[dbo].[State].[StateName] = @StateName,
		[dbo].[State].[StateCode] = @StateCode,
		[dbo].[State].[UserID]    = @UserID
	Where
		[dbo].[State].[StateID] = @StateID
End

--Exec PR_State_Update 2,2, 'GUJARAT', '24', 2
--Select * From [dbo].[State]


--Delete

Create Proc [dbo].[PR_State_Delete]
@StateID int
As
Begin
	Delete From [dbo].[State]
	Where [dbo].[State].[StateID] = @StateID
End

--Exec PR_State_Delete 2
--Select * From [dbo].[State]


--Select_All

Create or alter Proc [dbo].[PR_State_SelectAll]
As
Begin
	Select 
		[dbo].[State].[StateID],
		[dbo].[State].[StateName],
		[dbo].[State].[StateCode],
		Format([dbo].[State].[CreationDate],'dd-MM-yyyy') as CreationDate,
		[dbo].[Country].[CountryName],
		[dbo].[User].[UserName]
	From [dbo].[State]

	inner join [dbo].[User]
	on [dbo].[User].[UserID] = [dbo].[State].[UserID]

	inner join [dbo].[Country]
	on [dbo].[Country].[CountryID] = [dbo].[State].[CountryID]

End

--Exec PR_State_SelectAll


--Select_By_PK

Create or alter Proc [dbo].[PR_State_SelectByPK]
@StateID int
As
Begin
	Select 
		[dbo].[State].[StateID],
		[dbo].[State].[StateName],
		[dbo].[State].[StateCode],
		[dbo].[State].[CreationDate],
		[dbo].[State].[CountryID],
		[dbo].[Country].[CountryName],
		[dbo].[State].[UserID],
		[dbo].[User].[UserName]
	From [dbo].[State]

	inner join [dbo].[User]
	on [dbo].[User].[UserID] = [dbo].[State].[UserID]

	inner join [dbo].[Country]
	on [dbo].[Country].[CountryID] = [dbo].[State].[CountryID]

	Where StateID = @StateID
End

--Exec PR_State_SelectByPK 3


---DropDown
CREATE or ALTER PROCEDURE [dbo].[PR_State_DropDown]
AS
BEGIN
	SELECT 
		[dbo].[State].[StateID],
		[dbo].[State].[StateName]
	FROM
		[dbo].[State]
	ORDER BY
		[dbo].[State].[StateID]
END	

EXEC PR_State_DropDown
			