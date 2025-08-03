using System.Security;
using System.Text.Json;
using System.Xml.Serialization;
using BudgedManager.Models;
using BudgedManager.Models.Entity;

namespace BudgedManager.Services;

public class Export
{
    private AppDbContext _db;

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
            case "plain":
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
    // testowanie na 3 obiekty typu Expense
    private void JsonFormat()
    {
        /*
        [
            {
                "Id": 1,
                "Amount": 123.45,
                "CategoryId": 2,
                "Date": "2025-07-29T00:00:00",
                "Comment": "Lunch w restauracji"
            },
            {
                "Id": 2,
                "Amount": 89.99,
                "CategoryId": 1,
                "Date": "2025-07-28T00:00:00",
                "Comment": "Zakupy spożywcze"
            },
            {
                "Id": 3,
                "Amount": 300.00,
                "CategoryId": 5,
                "Date": "2025-07-27T00:00:00",
                "Comment": "Rachunek za prąd"
            }
        ]
        */
        // json serializer can write to stream 
        List<Expense> test = _db.Expenses.ToList();
        FileStream file = new FileStream("export.json", FileMode.Create);
        JsonSerializer.Serialize(file, test);
        file.Close();
    }

    // Method works
    private void TxtFormat()
    {
        /*
            12; 1; 27/07/2025 13:53:00; asdas
            59.99; 1; 24/07/2025 00:00:00; Paliwo do samochodu 
            123.45; 1; 25/07/2025 00:00:00; Obiad w restauracji
        */
        List<Expense> test = _db.Expenses.ToList();
        FileStream file = new FileStream("export.txt", FileMode.Create);
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

        using var stream = new FileStream("export.xml", FileMode.Create);
        serializer.Serialize(stream, expenseList);
        stream.Close();
    }
}