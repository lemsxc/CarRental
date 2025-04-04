using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Models
{
    public class Verification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string LicenseFrontPath { get; set; } // Path for front image
        public string LicenseBackPath { get; set; } // Path for back image

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        public string? AdminRemarks { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }

        // Navigation property
        [ForeignKey("UserId")]
        public User User { get; set; }
    }

}