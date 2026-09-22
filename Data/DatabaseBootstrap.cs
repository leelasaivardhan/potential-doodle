using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using CandidateRegistrationPortal.Services;
using CandidateRegistrationPortal.Models;

namespace CandidateRegistrationPortal.Data;

public sealed class DatabaseBootstrap(IConfiguration configuration, IWebHostEnvironment environment, SecurityService security)
{
    public async Task InitializeAsync()
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is missing.");

        var csb = new SqlConnectionStringBuilder(connectionString);
        var dbName = csb.InitialCatalog;
        if (string.IsNullOrWhiteSpace(dbName))
            throw new InvalidOperationException("Database name is missing from DefaultConnection.");

        await EnsureDatabaseExistsAsync(csb, dbName);

        var scripts = new[]
        {
            "02_CreateTables.sql",
            "03_SeedData.sql",
            "04_StoredProcedures.sql"
        };

        foreach (var script in scripts)
        {
            var path = Path.Combine(environment.ContentRootPath, "Database", script);
            if (!File.Exists(path))
                throw new FileNotFoundException($"Database script not found: {path}");

            await ExecuteBatchesAsync(connectionString, await File.ReadAllTextAsync(path));
        }

        await EnsureAdminAsync(connectionString);
    }

    private async Task EnsureAdminAsync(string connectionString)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        await using var db = new AppDbContext(options);
        var exists = await db.CandidateUsers.AnyAsync(x => x.UserName == "admin");
        if (exists) return;

        var admin = new CandidateUser
        {
            UserName = "admin",
            Name = "System Administrator",
            MobileEncrypted = security.ProtectMobile("9999999999"),
            MobileHash = security.HashMobile("9999999999"),
            DateOfBirth = new DateTime(1990, 1, 1),
            PasswordHash = security.HashPassword("Admin@123"),
            Role = "Admin",
            CreatedAt = DateTime.Now
        };
        db.CandidateUsers.Add(admin);
        await db.SaveChangesAsync();
    }

    private static async Task EnsureDatabaseExistsAsync(SqlConnectionStringBuilder source, string dbName)
    {
        var masterBuilder = new SqlConnectionStringBuilder(source.ConnectionString)
        {
            InitialCatalog = "master"
        };

        await using var connection = new SqlConnection(masterBuilder.ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        var safeDbName = dbName.Replace("]", "]]", StringComparison.Ordinal);
        command.CommandText = $"IF DB_ID(@dbName) IS NULL CREATE DATABASE [{safeDbName}];";
        command.Parameters.Add(new SqlParameter("@dbName", SqlDbType.NVarChar, 128) { Value = dbName });
        await command.ExecuteNonQueryAsync();
    }

    private static async Task ExecuteBatchesAsync(string connectionString, string script)
    {
        var batches = Regex.Split(script, @"^\s*GO\s*;?\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)
            .Where(x => !string.IsNullOrWhiteSpace(x));

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        foreach (var batch in batches)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = batch;
            command.CommandTimeout = 60;
            await command.ExecuteNonQueryAsync();
        }
    }
}
