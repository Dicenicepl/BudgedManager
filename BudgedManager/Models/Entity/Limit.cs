using System.ComponentModel.DataAnnotations;

namespace BudgedManager.Models.Entity;

public class Limit
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    [Range(0, int.MaxValue)]
    public decimal LimitWarning { get; set; }

    [Range(0, int.MaxValue)]
    public decimal LimitAlert { get; set; }

    [Range(0, int.MaxValue)]
    public decimal Amount { get; set; }

    public int ResetPeriod { get; set; }
    
    public DateOnly PreviousReset{ get; set; }
}