using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace BudgedManager.Models.Entity;

public class Expense
{
    [XmlIgnore]
    public int Id { get; set; }

    [Range(0.01, Double.MaxValue)] public decimal Amount { get; set; }

    public int CategoryId { get; set; }
    
    [XmlIgnore]
    public Category? Category { get; set; }
    public DateTime Date { get; set; }

    [StringLength(255)]
    public string? Comment { get; set; }

    public override string ToString()
    {
        return Amount + "; " + CategoryId + "; " + Date + "; " + Comment;
    }
}
[XmlRoot("Expenses")]
public class ExpenseList
{
    [XmlElement("Expense")]
    public List<Expense> Items { get; set; }
}