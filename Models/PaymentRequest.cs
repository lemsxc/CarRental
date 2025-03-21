using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class PaymentRequest
    {
        public int CarId { get; set; }
        public string CarName { get; set; }
        public string CarImage { get; set; }
        public decimal CarPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
