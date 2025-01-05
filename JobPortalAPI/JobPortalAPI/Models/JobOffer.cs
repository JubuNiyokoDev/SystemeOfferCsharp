using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class JobOffer
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    [StringLength(100)]
    public string Company { get; set; }

    [StringLength(100)]
    public string Location { get; set; }

    [ForeignKey("Publisher")]
    public string PublisherId { get; set; }
    public CustomUser Publisher { get; set; }

    [StringLength(100)]
    public string SalaryRange { get; set; }

    [Required]
    [StringLength(10)]
    public string Status { get; set; } = "active";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
}
