using System.ComponentModel.DataAnnotations;

namespace CarRental.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }
        public int UsersId { get; set; }
        public int CarId { get; set; }
        public int Rating { get; set; }
        public string Review { get; set; }
        public DateTime DateReview { get; set; }

        public User User { get; set; }
        public Car Car { get; set; }
    }
}