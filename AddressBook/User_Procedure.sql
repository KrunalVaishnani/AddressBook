--Insert
use Addressbook;


Create Proc [dbo].[PR_User_Insert]
	@UserName	varchar(100),
	@MobileNo	varchar(50),
	@EmailID	varchar(100)
As
	Insert Into [dbo].[User]
	(
		[dbo].[User].[UserName], 
		[dbo].[User].[MobileNo], 
		[dbo].[User].[EmailID]
	)
	Values
	(
		@UserName, 
		@MobileNo, 
		@EmailID
	)

--Exec PR_User_Insert 'Nikunj', 8849760510, 'nikunj.rathod.3748@gmail.com'
--Select * From [dbo].[User]


--Update

Create Proc [dbo].[PR_User_Update]
	@UserID		int,
	@UserName	varchar(100),
	@MobileNo	varchar(50),
	@EmailID	varchar(100)
As
	Update [dbo].[User]
	Set
		[dbo].[User].[UserName] = @UserName,
		[dbo].[User].[MobileNo] = @MobileNo,
		[dbo].[User].[EmailID] = @EmailID
	Where
		[dbo].[User].[UserID] = @UserID

--Exec PR_User_Update 2, 'NIKUNJ', '8849760510', 'nik@gmail.com'
--Select * From [dbo].[User]


--Delete

Create Proc [dbo].[PR_User_Delete]
	@UserID		int
As
	Delete From 
		[dbo].[User]
	Where 
		[dbo].[User].[UserID] = @UserID

--Exec PR_User_Delete 1
--Select * From [dbo].[User]


--Select_All

Create Proc [dbo].[PR_User_SelectAll]
As
	Select
		[dbo].[User].[UserID],
		[dbo].[User].[UserName], 
		[dbo].[User].[MobileNo], 
		[dbo].[User].[EmailID],
		[dbo].[User].[CreationDate]
	From 
		[dbo].[User]
	Order By 
		[dbo].[User].[UserName]

--Exec PR_User_SelectAll


--Select_By_PK

Create Proc [dbo].[PR_User_SelectByPK]
@UserID int
As
	Select
		[dbo].[User].[UserID],
		[dbo].[User].[UserName], 
		[dbo].[User].[MobileNo], 
		[dbo].[User].[EmailID],
		[dbo].[User].[CreationDate]
	From 
		[dbo].[User]
	Where 
		[dbo].[User].[UserID] = @UserID

--Exec PR_User_SelectByPK 2

--DropDown
Create Proc [dbo].[PR_User_DropDown]
AS
	Select
		[dbo].[User].[UserID],
		[dbo].[User].[UserName]
	FROM
		[dbo].[User]
	ORDER BY
		[dbo].[User].[UserID]

-- EXEC PR_User_DropDown
					
---User Registration

CREATE PROCEDURE [dbo].[PR_User_Register]
    @UserName NVARCHAR(50),
    @Password NVARCHAR(50),
    @Email NVARCHAR(500),
    @MobileNo VARCHAR(50)
AS
BEGIN
    INSERT INTO [dbo].[User]
    (
        [UserName],
        [Password],
        [EmailID],
        [MobileNo]
    )
    VALUES
    (
        @UserName,
        @Password,
        @Email,
        @MobileNo
    );
END

select * from [User]


---Login SP

CREATE PROCEDURE [dbo].[PR_User_Login]
    @Credential NVARCHAR(50),
    @Password NVARCHAR(50)
AS
BEGIN
    SELECT 
        [dbo].[User].[UserID], 
        [dbo].[User].[UserName], 
        [dbo].[User].[MobileNo], 
        [dbo].[User].[EmailID], 
        [dbo].[User].[Password]
    FROM 
        [dbo].[User] 
    WHERE 
        ([dbo].[User].[UserName] = @Credential OR
		 [dbo].[User].[EmailID] = @Credential OR
		 [dbo].[User].[MobileNo] = @Credential	
		 )
        AND [dbo].[User].[Password] = @Password;
END

EXEC PR_User_Login '9023410087','Keval@9206';