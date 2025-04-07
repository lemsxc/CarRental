using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Models
{
    public class Logs
    {
        public int Id { get; set; }
        public string ActionType { get; set; }
        public string Description { get; set; }
        public string AdminName { get; set; } // Full name or username
        public int AdminId { get; set; }
        public DateTime ActionDate { get; set; }

        // Navigation property
        [ForeignKey("AdminId")]
        public User User { get; set; }
    }
}