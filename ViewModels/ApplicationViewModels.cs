using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace CandidateRegistrationPortal.ViewModels;

public class ApplicationViewModel
{
    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public int? CourseId { get; set; }

    [Required]
    public int? StateId { get; set; }

    [Required]
    public int? DistrictId { get; set; }

    public IFormFile? Photograph { get; set; }
    public IFormFile? Signature { get; set; }
    public IFormFile? PdfDocument { get; set; }
}
