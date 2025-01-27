namespace BudgedManager.Models.Entity;

public class Subscription
{
    public int id { get; set; }
    public string subscriptionName { get; set; }
    public string subscriptionDescription { get; set; }
    public DateTime subscriptionStartDate { get; set; }
    public int subscriptionPaymentPeriod { get; set; }
    public decimal subscriptionPrice { get; set; }
}