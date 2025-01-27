namespace BudgedManager.Models.Entity;

public class Subscription
{
    public int id { get; set; }
    public string subscriptionName { get; set; }
    public string subscriptionDescription { get; set; }
    public DateTime subscriptionStartDate { get; set; }
    public DateTime subscriptionPaymentPeriod { get; set; }
    public decimal subsriptionPrice { get; set; }
}