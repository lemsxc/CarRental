using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }
        public int UsersId { get; set; }
        public int CarId { get; set; }
        public DateTime ReservedDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public string Status { get; set; }

        public User User { get; set; }
        public Car Car { get; set; }
        public Payment Payment { get; set; }
    }
}