using System;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class Driver
    {
        [Key]
        public int DriverId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } // Driver's full name

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } // Driver's full name

        [Required]
        [StringLength(15)]
        public string ContactNumber { get; set; } // Driver’s phone number

        [Required]
        [StringLength(100)]
        public string Address { get; set; } // Driver's address

        [Required]
        [StringLength(20)]
        public string LicenseNumber { get; set; } // Unique License Number

        [Required]
        public DateTime LicenseExpiry { get; set; } // License expiration date

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Available"; // "Available", "Hired", "Inactive"
    }
}