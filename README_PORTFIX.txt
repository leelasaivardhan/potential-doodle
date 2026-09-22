Run the application on ports 7050/5050.

PowerShell:
  dotnet clean
  dotnet restore
  dotnet build
  dotnet run

Open:
  https://localhost:7050

If port 7050 is also busy, run:
  dotnet run --urls "https://localhost:7060;http://localhost:5060"
