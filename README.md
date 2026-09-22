# Candidate Registration Portal - SQL Server / Unique CRP Table Names

This version intentionally DOES NOT use a table named `Users`.
All application tables use a `CRP_` prefix, for example:

- CRP_CandidateUsers
- CRP_CandidateCourses
- CRP_CandidateStates
- CRP_CandidateDistricts
- CRP_CandidateApplications
- CRP_CandidateLoginAudits

This avoids the old `PK_Users` / stale-schema conflicts in MovieTicketBookingDb.

## Connection string

Server=(localdb)\\MSSQLLocalDB;Database=MovieTicketBookingDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True

## Run

1. Ensure LocalDB is available:

`\n sqllocaldb start MSSQLLocalDB\n`

2. Open PowerShell in this `app` folder.
3. Run:

`\n dotnet restore\n dotnet build\n dotnet run\n`

The application automatically creates the database if necessary and runs the CRP-specific scripts. It does NOT rename or touch old `Users` tables.

## Manual SQL setup

Use SQL Server Management Studio connected to `(localdb)\\MSSQLLocalDB` and execute, in order:

1. Database/01_CreateDatabase.sql
2. Database/02_CreateTables.sql
3. Database/03_SeedData.sql
4. Database/04_StoredProcedures.sql
5. Database/05_VerifyDatabase.sql

`00_FullSetup.sql` uses SQLCMD `:r` commands and therefore requires SQLCMD mode in SSMS.

## Demo accounts

Admin: `admin` / `Admin@123`
Candidate password for newly registered users: `Welcome@123`

A fresh offline OTP and captcha are displayed on the login screen for demonstration purposes.

### Session compatibility fix
This version uses `Services/SessionValueHelper.cs` and the underlying `ISession.Set` / `TryGetValue` methods directly, avoiding the ASP.NET Core session extension methods that can produce `SetInt64`/`GetInt64` compile errors in some setups.
