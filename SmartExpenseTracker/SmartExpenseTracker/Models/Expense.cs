using System;
using System.ComponentModel.DataAnnotations;

namespace SmartExpenseTracker.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 10000)]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;
    }
}
