using System.Security;
using BudgedManager.Models.Entity;

namespace BudgedManager.Services;

public class Export
{
    private string _exportPath = "./DataTransfer/Export/";
    private FileStream fileStream;

    public void ActiveExport(List<Expense> expenses)
    {
        Console.WriteLine("Exposing expenses...");
        if (expenses.Count != 0)
        {
            
        }

        Console.WriteLine("Exporting competed");
    }
    
    
    private void LoadData()
    {
        
    }

    private void CreateFile()
    {
        var name = DateTime.Now.ToString("yyyyMMddHHmmss");
        try
        {
            fileStream = new FileStream(_exportPath + name, FileMode.Create);
        }
        catch (SecurityException e)
        {
            Console.WriteLine("Application does not have permission to create file");
        }
    }
}