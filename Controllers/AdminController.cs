using CandidateRegistrationPortal.Services;
using CandidateRegistrationPortal.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CandidateRegistrationPortal.Controllers;

public class AdminController(AppDbContext db, IWebHostEnvironment environment) : Controller
{
    private const string UserIdKey = "candidate-user-id";

    private async Task<Models.CandidateUser?> GetAdminAsync()
    {
        var id = SessionValueHelper.GetInt32(HttpContext.Session, UserIdKey);
        if (id is null) return null;
        var user = await db.CandidateUsers.FindAsync(id.Value);
        return user?.Role == "Admin" ? user : null;
    }

    public async Task<IActionResult> Index()
    {
        if (await GetAdminAsync() is null) return RedirectToAction("Login", "Account");
        var rows = await db.CandidateApplications
            .Include(x => x.User).Include(x => x.Course).Include(x => x.State).Include(x => x.District)
            .OrderByDescending(x => x.ApplicationId).ToListAsync();
        return View(rows);
    }

    public async Task<IActionResult> Documents()
    {
        if (await GetAdminAsync() is null) return RedirectToAction("Login", "Account");
        var rows = await db.CandidateApplications.Include(x => x.User).Where(x => x.PdfPath != null).OrderByDescending(x => x.ApplicationId).ToListAsync();
        return View(rows);
    }

    public async Task<IActionResult> AuditLogs()
    {
        if (await GetAdminAsync() is null) return RedirectToAction("Login", "Account");
        return View(await db.CandidateLoginAudits.OrderByDescending(x => x.AuditId).Take(500).ToListAsync());
    }

    public async Task<IActionResult> Pdf(int id)
    {
        if (await GetAdminAsync() is null) return RedirectToAction("Login", "Account");
        var row = await db.CandidateApplications.FindAsync(id);
        if (row?.PdfPath is null) return NotFound();
        var path = Path.Combine(environment.WebRootPath, row.PdfPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (!System.IO.File.Exists(path)) return NotFound();
        return PhysicalFile(path, "application/pdf");
    }
}
