using System.ComponentModel.DataAnnotations;

namespace BudgedManager.Models.Entity;

public class Expense
{
    public int Id { get; set; }

    [Range(1, int.MaxValue)]
    public decimal Amount { get;set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public DateTime Date { get; set; }
    public string? Comment { get; set; }
    
    
}