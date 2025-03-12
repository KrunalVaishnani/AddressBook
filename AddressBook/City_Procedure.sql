--Insert
USE Addressbook;

Create Proc [dbo].[PR_City_Insert]
@StateID int,
@CountryID int,
@CityName varchar(100),
@STDCode varchar(50),
@PinCode varchar(6),
@UserID int
As
Begin
	Insert Into [dbo].[City]
	(
		[dbo].[City].[StateID],
		[dbo].[City].[CountryID],
		[dbo].[City].[CityName],
		[dbo].[City].[STDCode],
		[dbo].[City].[PinCode],
		[dbo].[City].[UserID]
	)
	Values
	(
		@StateID,
		@CountryID,
		@CityName,
		@STDCode,
		@PinCode,
		@UserID
	)
End

--Exec PR_City_Insert 3, 2, 'Jamnagar', '0288', '361006', 2
--Select * From [dbo].[City]


--Update

Create or ALTER Proc [dbo].[PR_City_Update]
@CityID int,
@StateID int,
@CountryID int,
@CityName varchar(100),
@STDCode varchar(50),
@PinCode varchar(6),
@UserID int
As
Begin
	Update [dbo].[City]
	Set
		[dbo].[City].[StateID]   = @StateID,
		[dbo].[City].[CountryID] = @CountryID,
		[dbo].[City].[CityName]  = @CityName,
		[dbo].[City].[STDCode]   = @STDCode,
		[dbo].[City].[PinCode]   = @PinCode,
		[dbo].[City].[UserID]    = @UserID
	WHERE 
		[dbo].[City].[CityID] = @CityID	
End

--Exec PR_City_Update 1, 3, 2, 'JAMNAGAR', '0288', '361006', 2
--Select * From [dbo].[City]


--Delete

Create Proc [dbo].[PR_City_Delete]
@CityID int
As
Begin
	Delete From [dbo].[City]
	Where [dbo].[City].[CityID] = @CityID
End

--Exec PR_City_Delete 1
--Select * From [dbo].[City]


--Select_All

Create or alter Proc [dbo].[PR_City_SelectAll]
As
Begin
	Select 
		[dbo].[City].[CityID],
		[dbo].[City].[CityName],
		[dbo].[City].[STDCode],
		[dbo].[City].[PinCode],
		Format([dbo].[City].[CreationDate],'dd-MM-yyyy') as CreationDate,
		[dbo].[State].[StateName],
		[dbo].[Country].[CountryName],
		[dbo].[User].[UserName]
	From [dbo].[City]

	inner join [dbo].[User]
	on [dbo].[User].[UserID] = [dbo].[City].[UserID]

	inner join [dbo].[Country]
	on [dbo].[Country].[CountryID] = [dbo].[City].[CountryID]

	inner join [dbo].[State]
	on [dbo].[State].[StateID] = [dbo].[City].[StateID]
End

--Exec PR_City_SelectAll


--Select_By_PK

Create or ALTER Proc [dbo].[PR_City_SelectByPK]
@CityID int
As
Begin
	Select 
		[dbo].[City].[CityID],
		[dbo].[City].[CityName],
		[dbo].[City].[STDCode],
		[dbo].[City].[PinCode],
		[dbo].[City].[CreationDate],
		[dbo].[City].[StateID],
		[dbo].[State].[StateName],
		[dbo].[City].[CountryID],
		[dbo].[Country].[CountryName],
		[dbo].[City].[UserID],
		[dbo].[User].[UserName]
	From [dbo].[City]

	inner join [dbo].[User]
	on [dbo].[User].[UserID] = [dbo].[City].[UserID]

	inner join [dbo].[Country]
	on [dbo].[Country].[CountryID] = [dbo].[City].[CountryID]

	inner join [dbo].[State]
	on [dbo].[State].[StateID] = [dbo].[City].[StateID]

	Where [dbo].[City].[CityID] = @CityID
End

--Exec PR_City_SelectByPK 2



SELECT 
        c.CountryID,
        c.CountryName,
        c.CountryCode,
        c.CreationDate,
        c.UserID,
        COUNT(ci.CityID) AS CityCount
    FROM Country c
    LEFT JOIN City ci ON c.CountryID = ci.CountryID
    GROUP BY c.CountryID, c.CountryName, c.CountryCode, c.CreationDate, c.UserID
    ORDER BY c.CountryName;