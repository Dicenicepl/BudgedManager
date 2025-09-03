using BudgedManager.Models.Entity;
using System.Text;

namespace BudgedManager.Services;

public class Printer
{
    public byte[] GenerateTextFile(List<Expense> records)
    {
        var sb = new StringBuilder();
        foreach (var record in records)
        {
            sb.AppendLine(record.ToString());
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}
