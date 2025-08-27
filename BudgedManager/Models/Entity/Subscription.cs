using System.ComponentModel.DataAnnotations;

namespace BudgedManager.Models.Entity;

public class Subscription
{
    public int Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    [MinLength(2)]
    [MaxLength(20)]
    public string Name { get; set; }

    [Required(AllowEmptyStrings = true)]
    [MaxLength(240)]
    public string Description { get; set; }


    public DateTime StartDate { get; set; }

    [Range(1, int.MaxValue)] public int PaymentPeriod { get; set; }

    [Range(0.01, double.MaxValue)] public decimal Price { get; set; }
}