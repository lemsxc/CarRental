using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class User
    {
        [Key]
        public int UsersId { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        public string? ContactNumber { get; set; }

        public string? Address { get; set; }

        public string Role { get; set; }
        
        public bool IsVerified { get; set; }

        public ICollection<Reservation> Reservations { get; set; }
    }

}