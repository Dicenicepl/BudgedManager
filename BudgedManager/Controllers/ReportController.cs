using BudgedManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgedManager.Controllers;

public class ReportController : Controller
{
    private readonly AppDbContext _context;
    
    public ReportController(AppDbContext context)
    {
        _context = context;
    }
 
    // GET
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(string? period)
    {
        DateTime date = DateTime.Now;
        switch (period)
        {
            case "year":
                date = date.AddYears(-1);
                break;
            case "quarter":
                date = date.AddMonths(-6);
                break;
            case "month":
                date = date.AddMonths(-1);
                break;
        }

        var records = _context.Expenses.Where(x => x.Date > date).ToList();
        return View(records);
    }
}