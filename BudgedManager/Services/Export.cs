using System.Security;
using BudgedManager.Models.Entity;

namespace BudgedManager.Services;

public class Export
{

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

    }
    private void TxtFormat()
    {

    }
    private void XmlFormat()
    {
        
    }
}