using System.ComponentModel.DataAnnotations;

namespace BudgedManager.Models.Entity;

public class Subscription
{
    public int Id { get; set; }
    public string SubscriptionName { get; set; }
    public string SubscriptionDescription { get; set; }
    public DateTime SubscriptionStartDate { get; set; }
    [Range(1, int.MaxValue)]
    public int SubscriptionPaymentPeriod { get; set; }
    // [Range(1, int.MaxValue)]
    public decimal SubscriptionPrice { get; set; }
}