using BudgedManager.Models.Entity;
using Humanizer;

namespace BudgedManager.Services;

public class Import
{
    private string _path;
    public void Start(string type, string path)
    {
        _path = path;
        switch (type)
        {
            case "json": JsonFormat();
                break;
            case "txt": TxtFormat();
                break;
            case "xml": XmlFormat();
                break;
            default: Console.Error.WriteLine("Unknown export type: " + type);
                break;
        }
        Console.WriteLine("Import complete.");
    }

    private void JsonFormat()
    {
        string test = "";
        foreach (var VARIABLE in File.ReadAllLines(_path))
        {
            Expense expense = new Expense();
            for (int i = 0; i < 6; i++)
            {
                test += VARIABLE.Substring(VARIABLE.IndexOf(':') + 2).Replace(",", "");
                switch (i)
                {
                    case 0: expense.Id = int.Parse(test); break;
                    case 1: expense.Amount = int.Parse(test); break;
                    case 2: expense.CategoryId = int.Parse(test); break;
                    case 3: expense.Category = null; break;
                    case 4: expense.Date = DateTime.Parse(test); break;
                    case 5: expense.Comment = test; break;
                }
                Console.WriteLine(expense.ToString());
            }
            
        }
        Console.WriteLine(test);
    }
    private void TxtFormat()
    {
        
    }
    private void XmlFormat()
    {
        
    }
}