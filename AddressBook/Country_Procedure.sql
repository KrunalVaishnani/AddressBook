--Insert
USE Addressbook;

Create Proc [dbo].[PR_Country_Insert]
@CountryName varchar(100),
@CountryCode varchar(50),
@UserID int
As
Begin
    Insert Into [dbo].[Country] 
	(
		[dbo].[Country].[CountryName],
		[dbo].[Country].[CountryCode],
		[dbo].[Country].[UserID]
	)
    Values
	(
		@CountryName,
		@CountryCode,
		@UserID
	)
End

--Exec PR_Country_Insert 'India', '+91', 2
--Select * From [dbo].[Country]


--Update

Create Proc [dbo].[PR_Country_Update]
@CountryID int,
@CountryName varchar(100),
@CountryCode varchar(50),
@UserID int
As
Begin
    Update [dbo].[Country]
    Set
		[dbo].[Country].[CountryName] = @CountryName,
        [dbo].[Country].[CountryCode] = @CountryCode,
		[dbo].[Country].[UserID] = @UserID
    Where
		[dbo].[Country].[CountryID] = @CountryID
End

--Exec PR_Country_Update 1, 'INDIA', '+91', 2
--Select * From [dbo].[Country]


--Delete

Create Proc [dbo].[PR_Country_Delete]
@CountryID int
As
Begin
    Delete From [dbo].[Country]
    Where [dbo].[Country].[CountryID] = @CountryID
End

--Exec PR_Country_Delete 1
--Select * From [dbo].[Country]


--Select_All

Create or alter Proc [dbo].[PR_Country_SelectAll]
As
Begin
	Select 
		[dbo].[Country].[CountryID],
		[dbo].[Country].[CountryName],
		[dbo].[Country].[CountryCode],
		Format([dbo].[Country].[CreationDate],'dd-MM-yyyy') as CreationDate,
		[dbo].[User].[UserName]
		from [dbo].[Country]

		Inner Join [dbo].[User]
		on [dbo].[User].UserID = [dbo].[Country].[UserID]
End

--Exec PR_Country_SelectAll


--Select_By_PK

Create or alter Proc [dbo].[PR_Country_SelectByPK]
@CountryID int
As
Begin
	Select 
		[dbo].[Country].[CountryID],
		[dbo].[Country].[CountryName],
		[dbo].[Country].[CountryCode],
		[dbo].[Country].[CreationDate],
		[dbo].[Country].[UserID],
		[dbo].[User].[UserName]
		from [dbo].[Country]

		Inner Join [dbo].[User]
		on [dbo].[User].UserID = [dbo].[Country].[UserID]

		Where [dbo].[Country].CountryID = @CountryID
End

--Exec PR_Country_SelectByPK 2


---Dropdown
CREATE or ALTER PROCEDURE [dbo].[PR_Country_DropDown]
AS
BEGIN
	SELECT 
		[dbo].[Country].[CountryID],
		[dbo].[Country].[CountryName]
	FROM
		[dbo].[Country]
	ORDER BY
		[dbo].[Country].[CountryID]		
END			

exec PR_Country_DropDown
