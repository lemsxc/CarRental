using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    // Users Model
    public class Car
    {
        [Key]
        public int CarId { get; set; }

        [Required, StringLength(50)]
        public string Brand { get; set; }

        [Required, StringLength(50)]
        public string Model { get; set; }

        [Required]
        public string Image { get; set; } // Store image URL or path

        [Required]
        public float RentPrice { get; set; } // Changed to decimal for precision

        [Required, StringLength(50)]
        public string Category { get; set; } // E.g., SUV, Sedan, Sports

        [Required, StringLength(20)]
        public string Transmission { get; set; } // Manual / Automatic

        [Required, StringLength(20)]
        public string FuelType { get; set; } // Petrol / Diesel / Electric

        [Range(0, 100)]
        public int? FuelLevel { get; set; } // Optional (0-100%)

        [Required, StringLength(20)]
        public string PlateNumber { get; set; }

        [Range(0, int.MaxValue)]
        public int Mileage { get; set; } // Assuming in kilometers/miles

        [Required, StringLength(50)]
        public string Condition { get; set; } // E.g., Excellent, Good, Needs Maintenance

        [Required, StringLength(50)]
        public string Status { get; set; } // E.g., Available & Unavailable

        public ICollection<Reservation> Reservations { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
    }
}
