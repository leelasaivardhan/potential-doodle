using CandidateRegistrationPortal.Services;
using CandidateRegistrationPortal.Data;
using CandidateRegistrationPortal.Models;
using CandidateRegistrationPortal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CandidateRegistrationPortal.Controllers;

public class ApplicationController(AppDbContext db, IWebHostEnvironment environment) : Controller
{
    private const string UserIdKey = "candidate-user-id";

    private int? CurrentUserId => SessionValueHelper.GetInt32(HttpContext.Session, UserIdKey);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (CurrentUserId is null) return RedirectToAction("Login", "Account");
        var user = await db.CandidateUsers.FindAsync(CurrentUserId.Value);
        if (user is null) return RedirectToAction("Logout", "Account");

        await LoadListsAsync();
        ViewBag.Name = user.Name;
        ViewBag.Mobile = "XXXXXXXXXX";
        ViewBag.DateOfBirth = user.DateOfBirth.ToString("dd-MM-yyyy");
        return View(new ApplicationViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(ApplicationViewModel model)
    {
        if (CurrentUserId is null) return RedirectToAction("Login", "Account");
        if (!ModelState.IsValid)
        {
            await LoadListsAsync();
            var user = await db.CandidateUsers.FindAsync(CurrentUserId.Value);
            ViewBag.Name = user?.Name;
            ViewBag.Mobile = "XXXXXXXXXX";
            ViewBag.DateOfBirth = user?.DateOfBirth.ToString("dd-MM-yyyy");
            return View("Index", model);
        }

        var photoPath = await SaveFileAsync(model.Photograph, new[] { ".jpg", ".jpeg", ".png" }, 2 * 1024 * 1024, "photos");
        var signPath = await SaveFileAsync(model.Signature, new[] { ".jpg", ".jpeg", ".png" }, 2 * 1024 * 1024, "signatures");
        var pdfPath = await SaveFileAsync(model.PdfDocument, new[] { ".pdf" }, 5 * 1024 * 1024, "pdfs");

        var app = new CandidateApplication
        {
            UserId = CurrentUserId.Value,
            Gender = model.Gender,
            CourseId = model.CourseId!.Value,
            StateId = model.StateId!.Value,
            DistrictId = model.DistrictId!.Value,
            PhotoPath = photoPath,
            SignaturePath = signPath,
            PdfPath = pdfPath,
            SubmittedAt = DateTime.Now
        };
        db.CandidateApplications.Add(app);
        await db.SaveChangesAsync();

        TempData["Message"] = "Application submitted successfully.";
        return RedirectToAction(nameof(Submissions));
    }

    [HttpGet]
    public async Task<IActionResult> Submissions()
    {
        if (CurrentUserId is null) return RedirectToAction("Login", "Account");
        var rows = await db.CandidateApplications
            .Where(x => x.UserId == CurrentUserId.Value)
            .Include(x => x.Course)
            .Include(x => x.State)
            .Include(x => x.District)
            .OrderByDescending(x => x.ApplicationId)
            .ToListAsync();
        return View(rows);
    }

    [HttpGet]
    public async Task<IActionResult> HallTicket(int id)
    {
        if (CurrentUserId is null) return RedirectToAction("Login", "Account");
        var row = await db.CandidateApplications
            .Include(x => x.User)
            .Include(x => x.Course)
            .Include(x => x.State)
            .Include(x => x.District)
            .FirstOrDefaultAsync(x => x.ApplicationId == id && x.UserId == CurrentUserId.Value);
        if (row is null) return NotFound();
        return View(row);
    }

    [HttpGet]
    public async Task<IActionResult> Districts(int stateId)
    {
        var rows = await db.CandidateDistricts
            .Where(x => x.StateId == stateId)
            .OrderBy(x => x.Name)
            .Select(x => new { districtId = x.DistrictId, name = x.Name })
            .ToListAsync();
        return Json(rows);
    }

    private async Task LoadListsAsync()
    {
        ViewBag.Courses = await db.CandidateCourses.OrderBy(x => x.Name).ToListAsync();
        ViewBag.States = await db.CandidateStates.OrderBy(x => x.Name).ToListAsync();
        ViewBag.Districts = Array.Empty<CandidateDistrict>();
    }

    private async Task<string?> SaveFileAsync(IFormFile? file, string[] allowedExtensions, long maxBytes, string folder)
    {
        if (file is null || file.Length == 0) return null;
        if (file.Length > maxBytes) throw new InvalidOperationException($"{file.FileName} exceeds the maximum size.");
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext)) throw new InvalidOperationException($"Invalid file type for {folder}.");

        var relativeFolder = Path.Combine("uploads", folder);
        var physicalFolder = Path.Combine(environment.WebRootPath, relativeFolder);
        Directory.CreateDirectory(physicalFolder);
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var physicalPath = Path.Combine(physicalFolder, fileName);
        await using var stream = System.IO.File.Create(physicalPath);
        await file.CopyToAsync(stream);
        return "/" + relativeFolder.Replace('\\', '/') + "/" + fileName;
    }
}
