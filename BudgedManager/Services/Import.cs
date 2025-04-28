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
        var test = File.ReadAllLines(_path);
        foreach (var VARIABLE in test)
        {
            if (VARIABLE.IndexOf(':') > 0)
            {
                Console.WriteLine(VARIABLE.Substring(VARIABLE.IndexOf(':') + 2).Replace(",", ""));
            }
            
        }
    }
    private void TxtFormat()
    {
        
    }
    private void XmlFormat()
    {
        
    }
}