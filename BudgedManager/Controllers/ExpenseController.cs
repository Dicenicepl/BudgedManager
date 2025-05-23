using BudgedManager.Models;
using BudgedManager.Models.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BudgedManager.Controllers;

public class ExpenseController : Controller
{
    private readonly AppDbContext _context;
    LimitController _limitController;
    public ExpenseController(AppDbContext context)
    {
        _context = context;
        _limitController = new LimitController(_context);
    }

    // GET: Expense
    public async Task<IActionResult> Index(string? orderBy, string? date, string? category, string? amount)
    {
        var expenses = await _context.Expenses.Include(
                e => e.Category)
            .ToListAsync();
        if (date != null) expenses = new List<Expense>(expenses.Where(e => e.Date.Date == DateTime.Parse(date)));
        if (category != null) expenses = new List<Expense>(expenses.Where(s => s.Category.Name.Equals(category)));
        if (amount != null) expenses = new List<Expense>(expenses.Where(s => s.Amount.ToString() == amount));
        switch (orderBy)
        {
            case "Amount":
                expenses = new List<Expense>(expenses.OrderBy(e => e.Amount));
                break;
            case "Category":
                expenses = new List<Expense>(expenses.OrderBy(e => e.Category.Name));
                break;
            case "Date":
                expenses = new List<Expense>(expenses.OrderBy(e => e.Date));
                break;
        }

        return View(expenses);
    }


    // GET: Expense/Summary/
    public async Task<IActionResult> Summary(DateTime? startDate, DateTime? endDate)
    {
        var categoryExpenses = _context.Expenses
            .GroupBy(e => e.CategoryId)
            .Select(group => new SummaryDto
            {
                CategoryId = group.Key,
                CategoryName = group.First().Category.Name,
                Score = (float)Math.Round(group.Sum(e => e.Amount))
            }).ToList();

        var countRecords = await _context.Expenses.Select(e => e.Date.Date).Distinct().CountAsync();

        var totalExpenses = await _context.Expenses.SumAsync(e => e.Amount);

        var averageDays = totalExpenses / countRecords;
        // var averageWeeks = totalExpenses / (countRecords / 7);
        // var averageMonths = totalExpenses / (countRecords / 30);

        ViewData["Sum"] = (float)Math.Round(totalExpenses, 2);
        ViewData["Highest"] = (float)Math.Round(categoryExpenses.Max(e => e.Score), 2);
        ViewData["categoryExpenses"] = categoryExpenses;
        ViewData["AverageDays"] = (float)Math.Round(averageDays, 2);
        // ViewData["AverageWeeks"] = (float)Math.Round(averageWeeks,2) ;
        // ViewData["AverageMonths"] = (float)Math.Round(averageMonths,2) ;

        return View();
    }

    // GET: Expense/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var expense = await _context.Expenses
            .FirstOrDefaultAsync(m => m.Id == id);
        if (expense == null) return NotFound();

        return View(expense);
    }

    // GET: Expense/Create
    public IActionResult Create()
    {
        ViewData["Category"] = new SelectList(_context.Categories, "Id", "Name");
        return View();
    }

    // POST: Expense/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Amount,CategoryId,Date,Comment")] Expense expense)
    {
        if (expense.CategoryId.Equals(null)) return BadRequest();
        
        if (!_limitController.IsLimitIsBigger(expense.CategoryId, expense.Amount))
        {
            return BadRequest();
        }
        
        if (ModelState.IsValid)
        {
            expense.Amount = decimal.Parse(string.Format("{0:0.00}", expense.Amount));
            _context.Add(expense);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(expense);
    }

    // GET: Expense/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        ViewData["Category"] = new SelectList(_context.Categories, "Id", "Name");

        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null) return NotFound();
        return View(expense);
    }

    // POST: Expense/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,Category,Date,Comment")] Expense expense)
    {
        if (id != expense.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(expense);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ExpenseExists(expense.Id)) return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(expense);
    }

    // GET: Expense/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var expense = await _context.Expenses
            .FirstOrDefaultAsync(m => m.Id == id);
        if (expense == null) return NotFound();

        return View(expense);
    }

    // GET: Expense/History
    public async Task<IActionResult> History()
    {
        var expense = _context.Expenses.Select(
                group =>
                    new HistoryDto
                    {
                        Category = _context.Categories.FirstOrDefault(c => c.Id == group.CategoryId).Name,
                        Amount = group.Amount,
                        Date = group.Date,
                        Comment = group.Comment
                    })
            .OrderByDescending(a => a.Date)
            .ToList();
        ViewData["expenses"] = expense;
        return View();
    }

    // POST: Expense/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense != null) _context.Expenses.Remove(expense);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ExpenseExists(int id)
    {
        return _context.Expenses.Any(e => e.Id == id);
    }
}