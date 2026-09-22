USE [MovieTicketBookingDb];
GO

CREATE OR ALTER PROCEDURE dbo.sp_CRP_LoginAudit_Insert
    @UserName NVARCHAR(50),
    @LoginDateTime DATETIME2,
    @LoginStatus NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.CRP_CandidateLoginAudits(UserName,LoginDateTime,LoginStatus)
    VALUES(@UserName,@LoginDateTime,@LoginStatus);
    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS AuditId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_CRP_LoginAudit_Logout
    @AuditId BIGINT,
    @LogoutDateTime DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.CRP_CandidateLoginAudits SET LogoutDateTime=@LogoutDateTime WHERE AuditId=@AuditId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_CRP_Applications_SelectAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT a.ApplicationId,u.UserName,u.Name,c.Name AS Course,s.Name AS StateName,d.Name AS DistrictName,a.SubmittedAt
    FROM dbo.CRP_CandidateApplications a
    INNER JOIN dbo.CRP_CandidateUsers u ON u.UserId=a.UserId
    INNER JOIN dbo.CRP_CandidateCourses c ON c.CourseId=a.CourseId
    INNER JOIN dbo.CRP_CandidateStates s ON s.StateId=a.StateId
    INNER JOIN dbo.CRP_CandidateDistricts d ON d.DistrictId=a.DistrictId
    ORDER BY a.ApplicationId DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_CRP_LoginAudit_SelectAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT AuditId,UserName,LoginDateTime,LogoutDateTime,LoginStatus
    FROM dbo.CRP_CandidateLoginAudits
    ORDER BY AuditId DESC;
END
GO
