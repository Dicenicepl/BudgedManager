using System.Security;
using System.Text.Json;
using System.Xml.Serialization;
using BudgedManager.Models;
using BudgedManager.Models.Entity;

namespace BudgedManager.Services;

public class Export
{
    private AppDbContext _db;
    private string exportFilesFolder = Environment.CurrentDirectory + "/wwwroot/files/";

    public Export(AppDbContext db)
    {
        _db = db;
    }

    public void Start(string type)
    {
        switch (type)
        {
            case "json":
                JsonFormat();
                break;
            case "txt":
                TxtFormat();
                break;
            case "xml":
                XmlFormat();
                break;
            default:
                Console.Error.WriteLine("Unknown export type: " + type);
                break;
        }
    }
    private void JsonFormat()
    {
        List<Expense> test = _db.Expenses.ToList();
        FileStream file = new FileStream(exportFilesFolder + "export.json", FileMode.Create);
        JsonSerializer.Serialize(file, test);
        file.Close();
    }

    private void TxtFormat()
    {
        List<Expense> test = _db.Expenses.ToList();
        FileStream file = new FileStream(exportFilesFolder + "export.txt", FileMode.Create);
        using (StreamWriter writer = new StreamWriter(file))
        {
            foreach (var expense in test)
            {
                writer.WriteLine(expense.ToString());
            }
            writer.Close();
        }
        file.Close();
    }
    private void XmlFormat()
    {
        List<Expense> expenses = _db.Expenses.ToList();
        ExpenseList expenseList = new ExpenseList
        {
            Items = expenses
        };

        var serializer = new XmlSerializer(typeof(ExpenseList));

        using var stream = new FileStream(exportFilesFolder + "export.xml", FileMode.Create);
        serializer.Serialize(stream, expenseList);
        stream.Close();
    }
}