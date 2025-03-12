CREATE DATABASE Addressbook;

use Addressbook;


-- Table: User
CREATE TABLE [User] (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    UserName VARCHAR(100) NOT NULL,
    MobileNo VARCHAR(50) NOT NULL,
    EmailID VARCHAR(100) NOT NULL,
    [Password] VARCHAR(12) NOT NULL ,
    CreationDate DATETIME DEFAULT GETDATE()
);



-- Table: Country
CREATE TABLE Country (
    CountryID INT PRIMARY KEY IDENTITY(1,1),
    CountryName VARCHAR(100) UNIQUE NOT NULL,
    CountryCode VARCHAR(50) NOT NULL,
    CreationDate DATETIME DEFAULT GETDATE(),
    UserID INT NOT NULL,
    FOREIGN KEY (UserID) REFERENCES [User](UserID)
);

-- Table: State
CREATE TABLE State (
    StateID INT PRIMARY KEY IDENTITY(1,1),
    CountryID INT NOT NULL,
    StateName VARCHAR(100) UNIQUE NOT NULL,
    StateCode VARCHAR(50) NOT NULL,
    CreationDate DATETIME DEFAULT GETDATE(),
    UserID INT NOT NULL,
    FOREIGN KEY (CountryID) REFERENCES Country(CountryID),
    FOREIGN KEY (UserID) REFERENCES [User](UserID)
);

-- Table: City
CREATE TABLE City (
    CityID INT PRIMARY KEY IDENTITY(1,1),
    StateID INT NOT NULL,
    CountryID INT NOT NULL,
    CityName VARCHAR(100) UNIQUE NOT NULL,
    STDCode VARCHAR(50),
    PinCode VARCHAR(6),
    CreationDate DATETIME DEFAULT GETDATE(),
    UserID INT NOT NULL,
    FOREIGN KEY (StateID) REFERENCES State(StateID),
    FOREIGN KEY (CountryID) REFERENCES Country(CountryID),
    FOREIGN KEY (UserID) REFERENCES [User](UserID)
);