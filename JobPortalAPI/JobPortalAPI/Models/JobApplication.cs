using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class JobApplication
{
    public int Id { get; set; }

    [Required]
    [ForeignKey("JobOffer")]
    public int JobId { get; set; }
    public JobOffer Job { get; set; }

    [Required]
    [ForeignKey("Applicant")]
    public string ApplicantId { get; set; }
    public CustomUser Applicant { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "pending";

    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string CoverLetter { get; set; }

    public string Notes { get; set; }
}
