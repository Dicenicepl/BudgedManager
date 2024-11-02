namespace BudgedManager.Models.Entity;

public class Expense
{
    public int Id { get; set; }
    public string Title { get; set;}
    public double Amount { get; set; }
    public DateTime Date { get; set; }
}