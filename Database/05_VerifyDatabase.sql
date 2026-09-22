USE [MovieTicketBookingDb];
GO
SELECT name FROM sys.tables WHERE name LIKE 'CRP_%' ORDER BY name;
SELECT TOP 20 * FROM dbo.CRP_CandidateCourses ORDER BY CourseId;
SELECT TOP 20 * FROM dbo.CRP_CandidateStates ORDER BY StateId;
SELECT TOP 50 * FROM dbo.CRP_CandidateDistricts ORDER BY StateId,DistrictId;
