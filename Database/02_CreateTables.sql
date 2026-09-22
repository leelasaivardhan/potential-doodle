USE [MovieTicketBookingDb];
GO

IF OBJECT_ID(N'dbo.CRP_CandidateUsers', N'U') IS NULL
BEGIN
CREATE TABLE dbo.CRP_CandidateUsers
(
    UserId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_CRP_CandidateUsers PRIMARY KEY,
    UserName NVARCHAR(50) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    MobileEncrypted NVARCHAR(1000) NOT NULL,
    MobileHash VARCHAR(64) NOT NULL,
    DateOfBirth DATE NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    Role NVARCHAR(20) NOT NULL CONSTRAINT DF_CRP_CandidateUsers_Role DEFAULT ('Candidate'),
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_CRP_CandidateUsers_CreatedAt DEFAULT (SYSDATETIME()),
    CONSTRAINT UQ_CRP_CandidateUsers_UserName UNIQUE(UserName),
    CONSTRAINT UQ_CRP_CandidateUsers_MobileHash UNIQUE(MobileHash)
);
END
GO

IF OBJECT_ID(N'dbo.CRP_CandidateCourses', N'U') IS NULL
BEGIN
CREATE TABLE dbo.CRP_CandidateCourses
(
    CourseId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_CRP_CandidateCourses PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL CONSTRAINT UQ_CRP_CandidateCourses_Name UNIQUE
);
END
GO

IF OBJECT_ID(N'dbo.CRP_CandidateStates', N'U') IS NULL
BEGIN
CREATE TABLE dbo.CRP_CandidateStates
(
    StateId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_CRP_CandidateStates PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL CONSTRAINT UQ_CRP_CandidateStates_Name UNIQUE
);
END
GO

IF OBJECT_ID(N'dbo.CRP_CandidateDistricts', N'U') IS NULL
BEGIN
CREATE TABLE dbo.CRP_CandidateDistricts
(
    DistrictId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_CRP_CandidateDistricts PRIMARY KEY,
    StateId INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    CONSTRAINT FK_CRP_CandidateDistricts_State FOREIGN KEY(StateId) REFERENCES dbo.CRP_CandidateStates(StateId),
    CONSTRAINT UQ_CRP_CandidateDistricts_State_Name UNIQUE(StateId,Name)
);
END
GO

IF OBJECT_ID(N'dbo.CRP_CandidateApplications', N'U') IS NULL
BEGIN
CREATE TABLE dbo.CRP_CandidateApplications
(
    ApplicationId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_CRP_CandidateApplications PRIMARY KEY,
    UserId INT NOT NULL,
    Gender NVARCHAR(20) NOT NULL,
    CourseId INT NOT NULL,
    StateId INT NOT NULL,
    DistrictId INT NOT NULL,
    PhotoPath NVARCHAR(500) NULL,
    SignaturePath NVARCHAR(500) NULL,
    PdfPath NVARCHAR(500) NULL,
    SubmittedAt DATETIME2 NOT NULL CONSTRAINT DF_CRP_CandidateApplications_SubmittedAt DEFAULT(SYSDATETIME()),
    CONSTRAINT FK_CRP_CandidateApplications_User FOREIGN KEY(UserId) REFERENCES dbo.CRP_CandidateUsers(UserId),
    CONSTRAINT FK_CRP_CandidateApplications_Course FOREIGN KEY(CourseId) REFERENCES dbo.CRP_CandidateCourses(CourseId),
    CONSTRAINT FK_CRP_CandidateApplications_State FOREIGN KEY(StateId) REFERENCES dbo.CRP_CandidateStates(StateId),
    CONSTRAINT FK_CRP_CandidateApplications_District FOREIGN KEY(DistrictId) REFERENCES dbo.CRP_CandidateDistricts(DistrictId)
);
END
GO

IF OBJECT_ID(N'dbo.CRP_CandidateLoginAudits', N'U') IS NULL
BEGIN
CREATE TABLE dbo.CRP_CandidateLoginAudits
(
    AuditId BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_CRP_CandidateLoginAudits PRIMARY KEY,
    UserName NVARCHAR(50) NOT NULL,
    LoginDateTime DATETIME2 NOT NULL,
    LogoutDateTime DATETIME2 NULL,
    LoginStatus NVARCHAR(20) NOT NULL
);
END
GO
