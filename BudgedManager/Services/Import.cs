using System.Text.Json;
using BudgedManager.Controllers;
using BudgedManager.Models;
using BudgedManager.Models.Entity;
using Humanizer;

namespace BudgedManager.Services;

public class Import
{
    private string _path;
    private AppDbContext _db;

    public Import(AppDbContext db)
    {
        _db = db;
    }
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
        try
        {
            string json = File.ReadAllText(_path);
            List<Expense> expenses = JsonSerializer.Deserialize<List<Expense>>(json);
            foreach (var VARIABLE in expenses)
            {
                _db.Expenses.Add(VARIABLE);
            }
            _db.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error while importing data from json, LINE 36 returned:" + e);
            throw;
        }
    }
    private void TxtFormat()
    {
        
    }
    private void XmlFormat()
    {
        
    }
}