using System.ComponentModel.DataAnnotations;

namespace BudgedManager.Models.Entity;

public class Subscription
{
    public int Id { get; set; }
    [Required (AllowEmptyStrings = false)]
    [MinLength (2)]
    [MaxLength(20)]
    public string SubscriptionName { get; set; }
    
    [Required (AllowEmptyStrings = true)]
    [MaxLength(240)]
    public string SubscriptionDescription { get; set; }
    
    
    public DateTime SubscriptionStartDate { get; set; }
    [Range(1, int.MaxValue)]
    public int SubscriptionPaymentPeriod { get; set; }
    [Range(1, int.MaxValue)]
    public decimal SubscriptionPrice { get; set; }
}