
using System.Xml.Serialization;
using BudgedManager.Models.Entity;
using Microsoft.AspNetCore.Mvc;

public class TestController : Controller
{
    public IActionResult Index()
    {
        return Content(@"<html><body><a href=""http://localhost:5062/Test/download/"" download>test</a></body></html>", "text/html");

    }
    public IActionResult Download()
    {
        var content = System.IO.File.ReadAllBytes("C:\\Users\\Dicenice\\RiderProjects\\BudgedManager\\BudgedManager\\wwwroot\\files\\export.xml");
        return File(content, "application/xml", "export.xml");
    }


}