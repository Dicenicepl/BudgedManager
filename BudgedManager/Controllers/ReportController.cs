using BudgedManager.Models;
using BudgedManager.Models.Entity;
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
    
    public async Task<IActionResult> Index(string? period)
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

        // var records = await _context.Expenses.Where(x => x.Date > date).ToListAsync();
        var records = _context.Expenses
            .Include(e => e.Category)
            .Where(e => e.Date >= date)
            .ToList();
        return View(records);
    }
    
}