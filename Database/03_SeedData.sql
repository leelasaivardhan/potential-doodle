USE [MovieTicketBookingDb];
GO

INSERT INTO dbo.CRP_CandidateCourses(Name)
SELECT v.Name FROM (VALUES (N'B.Tech'),(N'M.Tech'),(N'MCA'),(N'MBA'),(N'B.Sc'),(N'B.Com')) v(Name)
WHERE NOT EXISTS (SELECT 1 FROM dbo.CRP_CandidateCourses c WHERE c.Name=v.Name);
GO

INSERT INTO dbo.CRP_CandidateStates(Name)
SELECT v.Name FROM (VALUES (N'Andhra Pradesh'),(N'Telangana'),(N'Tamil Nadu'),(N'Karnataka'),(N'Maharashtra')) v(Name)
WHERE NOT EXISTS (SELECT 1 FROM dbo.CRP_CandidateStates s WHERE s.Name=v.Name);
GO

DECLARE @AP INT=(SELECT StateId FROM dbo.CRP_CandidateStates WHERE Name=N'Andhra Pradesh');
DECLARE @TG INT=(SELECT StateId FROM dbo.CRP_CandidateStates WHERE Name=N'Telangana');
DECLARE @TN INT=(SELECT StateId FROM dbo.CRP_CandidateStates WHERE Name=N'Tamil Nadu');
DECLARE @KA INT=(SELECT StateId FROM dbo.CRP_CandidateStates WHERE Name=N'Karnataka');
DECLARE @MH INT=(SELECT StateId FROM dbo.CRP_CandidateStates WHERE Name=N'Maharashtra');

INSERT INTO dbo.CRP_CandidateDistricts(StateId,Name)
SELECT s.StateId,v.Name FROM (VALUES (@AP,N'Vijayawada'),(@AP,N'Visakhapatnam'),(@AP,N'Guntur'),(@TG,N'Hyderabad'),(@TG,N'Warangal'),(@TN,N'Chennai'),(@TN,N'Coimbatore'),(@KA,N'Bengaluru'),(@MH,N'Mumbai')) v(StateId,Name)
JOIN dbo.CRP_CandidateStates s ON s.StateId=v.StateId
WHERE NOT EXISTS (SELECT 1 FROM dbo.CRP_CandidateDistricts d WHERE d.StateId=v.StateId AND d.Name=v.Name);
GO
