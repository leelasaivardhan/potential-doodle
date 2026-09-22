namespace CandidateRegistrationPortal.Models;

public class CandidateUser
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string MobileEncrypted { get; set; } = string.Empty;
    public string MobileHash { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Candidate";
    public DateTime CreatedAt { get; set; }
    public ICollection<CandidateApplication> Applications { get; set; } = new List<CandidateApplication>();
}

public class CandidateCourse
{
    public int CourseId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CandidateState
{
    public int StateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<CandidateDistrict> Districts { get; set; } = new List<CandidateDistrict>();
}

public class CandidateDistrict
{
    public int DistrictId { get; set; }
    public int StateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public CandidateState? State { get; set; }
}

public class CandidateApplication
{
    public int ApplicationId { get; set; }
    public int UserId { get; set; }
    public string Gender { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public int StateId { get; set; }
    public int DistrictId { get; set; }
    public string? PhotoPath { get; set; }
    public string? SignaturePath { get; set; }
    public string? PdfPath { get; set; }
    public DateTime SubmittedAt { get; set; }

    public CandidateUser? User { get; set; }
    public CandidateCourse? Course { get; set; }
    public CandidateState? State { get; set; }
    public CandidateDistrict? District { get; set; }
}

public class CandidateLoginAudit
{
    public long AuditId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime LoginDateTime { get; set; }
    public DateTime? LogoutDateTime { get; set; }
    public string LoginStatus { get; set; } = string.Empty;
}
