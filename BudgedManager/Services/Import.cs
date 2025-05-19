using System.Text.Json;
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
        string json = File.ReadAllText(_path);
        List<Expense> expenses = JsonSerializer.Deserialize<List<Expense>>(json);

        foreach (var VARIABLE in expenses)
        {
            Console.WriteLine(VARIABLE);
        }
    }
    private void TxtFormat()
    {
        
    }
    private void XmlFormat()
    {
        
    }
}