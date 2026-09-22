using CandidateRegistrationPortal.Data;
using CandidateRegistrationPortal.Models;
using CandidateRegistrationPortal.Services;
using CandidateRegistrationPortal.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CandidateRegistrationPortal.Controllers;

public class AccountController(
    AppDbContext db,
    SecurityService security,
    CaptchaService captcha) : Controller
{
    private const string CaptchaKey = "register-captcha";
    private const string LoginCaptchaKey = "login-captcha";
    private const string OtpKey = "login-otp";
    private const string UserIdKey = "candidate-user-id";
    private const string LoginAuditKey = "candidate-login-audit";

    [HttpGet]
    public IActionResult Register()
    {
        var c = captcha.Generate();
        SessionValueHelper.SetInt32(HttpContext.Session, CaptchaKey, c.Answer);
        ViewBag.CaptchaQuestion = c.Question;
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        var expected = SessionValueHelper.GetInt32(HttpContext.Session, CaptchaKey);
        ViewBag.CaptchaQuestion = captcha.Generate().Question;
        if (!ModelState.IsValid || expected is null || !int.TryParse(model.Captcha, out var answer) || answer != expected)
        {
            ModelState.AddModelError(nameof(model.Captcha), "Invalid captcha.");
            var c = captcha.Generate();
            SessionValueHelper.SetInt32(HttpContext.Session, CaptchaKey, c.Answer);
            ViewBag.CaptchaQuestion = c.Question;
            return View(model);
        }

        if (model.DateOfBirth is null || CalculateAge(model.DateOfBirth.Value.Date) < 16)
        {
            ModelState.AddModelError(nameof(model.DateOfBirth), "Candidate must be at least 16 years old.");
            var c = captcha.Generate();
            SessionValueHelper.SetInt32(HttpContext.Session, CaptchaKey, c.Answer);
            ViewBag.CaptchaQuestion = c.Question;
            return View(model);
        }

        if (await db.CandidateUsers.AnyAsync(x => x.UserName == model.UserName.Trim()))
        {
            ModelState.AddModelError(nameof(model.UserName), "Username already exists.");
            var c = captcha.Generate();
            SessionValueHelper.SetInt32(HttpContext.Session, CaptchaKey, c.Answer);
            ViewBag.CaptchaQuestion = c.Question;
            return View(model);
        }

        var mobile = model.Mobile.Trim();
        var mobileHash = security.HashMobile(mobile);
        if (await db.CandidateUsers.AnyAsync(x => x.MobileHash == mobileHash))
        {
            ModelState.AddModelError(nameof(model.Mobile), "Mobile number is already registered.");
            var c = captcha.Generate();
            SessionValueHelper.SetInt32(HttpContext.Session, CaptchaKey, c.Answer);
            ViewBag.CaptchaQuestion = c.Question;
            return View(model);
        }

        var temporaryPassword = "Welcome@123";
        var user = new CandidateUser
        {
            UserName = model.UserName.Trim(),
            Name = model.Name.Trim(),
            MobileEncrypted = security.ProtectMobile(mobile),
            MobileHash = mobileHash,
            DateOfBirth = model.DateOfBirth.Value.Date,
            PasswordHash = security.HashPassword(temporaryPassword),
            Role = "Candidate",
            CreatedAt = DateTime.Now
        };

        db.CandidateUsers.Add(user);
        await db.SaveChangesAsync();

        TempData["Message"] = $"Registration successful. Demo password: {temporaryPassword}";
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult Login()
    {
        var c = captcha.Generate();
        SessionValueHelper.SetInt32(HttpContext.Session, LoginCaptchaKey, c.Answer);
        var otp = captcha.GenerateOtp();
        SessionValueHelper.SetString(HttpContext.Session, OtpKey, otp);
        ViewBag.CaptchaQuestion = c.Question;
        ViewBag.DemoOtp = otp;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var captchaExpected = SessionValueHelper.GetInt32(HttpContext.Session, LoginCaptchaKey);
        var otpExpected = SessionValueHelper.GetString(HttpContext.Session, OtpKey);
        var validCaptcha = int.TryParse(model.Captcha, out var captchaAnswer) && captchaExpected == captchaAnswer;
        var user = await db.CandidateUsers.FirstOrDefaultAsync(x => x.UserName == model.UserName.Trim());
        var valid = user != null && security.VerifyPassword(model.Password, user.PasswordHash) && model.OTP == otpExpected;

        db.CandidateLoginAudits.Add(new CandidateLoginAudit
        {
            UserName = model.UserName.Trim(),
            LoginDateTime = DateTime.Now,
            LoginStatus = valid && validCaptcha ? "Successful" : "Unsuccessful"
        });
        await db.SaveChangesAsync();

        if (!validCaptcha || !valid)
        {
            ModelState.AddModelError(string.Empty, "Invalid username, password, OTP, or captcha.");
            var c = captcha.Generate();
            SessionValueHelper.SetInt32(HttpContext.Session, LoginCaptchaKey, c.Answer);
            var otp = captcha.GenerateOtp();
            SessionValueHelper.SetString(HttpContext.Session, OtpKey, otp);
            ViewBag.CaptchaQuestion = c.Question;
            ViewBag.DemoOtp = otp;
            return View(model);
        }

        SessionValueHelper.SetInt32(HttpContext.Session, UserIdKey, user!.UserId);
        var audit = await db.CandidateLoginAudits
            .Where(x => x.UserName == user.UserName && x.LoginStatus == "Successful")
            .OrderByDescending(x => x.AuditId)
            .FirstAsync();
        SessionValueHelper.SetInt64(HttpContext.Session, LoginAuditKey, audit.AuditId);

        return user.Role == "Admin"
            ? RedirectToAction("Index", "Admin")
            : RedirectToAction("Index", "Application");
    }

    public async Task<IActionResult> Logout()
    {
        var auditId = SessionValueHelper.GetInt64(HttpContext.Session, LoginAuditKey);
        if (auditId.HasValue)
        {
            var audit = await db.CandidateLoginAudits.FindAsync(auditId.Value);
            if (audit != null)
            {
                audit.LogoutDateTime = DateTime.Now;
                await db.SaveChangesAsync();
            }
        }
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }

    private static int CalculateAge(DateTime dob)
    {
        var today = DateTime.Today;
        var age = today.Year - dob.Year;
        if (dob.Date > today.AddYears(-age)) age--;
        return age;
    }
}
