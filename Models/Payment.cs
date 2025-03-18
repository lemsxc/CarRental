using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public int ReservationId { get; set; }
        public float Amount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; }

        public Reservation Reservation { get; set; }
    }
}