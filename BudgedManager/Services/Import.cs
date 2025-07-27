using System.IO.Pipelines;
using System.Text.Json;
using System.Xml.Serialization;
using BudgedManager.Models;
using BudgedManager.Models.Entity;

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
        // TODO - Create a temp category for Expenses or set them to default category
        foreach (var textLine in File.ReadAllLines(_path))
        {
            var item = textLine.Split(';');

            _db.Expenses.Add(new Expense
            {
                Amount = decimal.Parse(item[0]),
                CategoryId = int.Parse(item[1]),
                Date = DateTime.Parse(item[2]),
                Comment = item[3]
            }
            );
            _db.SaveChanges();
        }
    }
    private void XmlFormat()
    {
        var serializer = new XmlSerializer(typeof(ExpenseList));

        using var stream = new FileStream(_path, FileMode.Open);
        var expenseList = (ExpenseList)serializer.Deserialize(stream);
        
        foreach (var item in expenseList.Items)
        {
            _db.Expenses.Add(item);
        }

        _db.SaveChanges();
    }
}