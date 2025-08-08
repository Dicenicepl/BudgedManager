using BudgedManager.Models;
using BudgedManager.Models.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BudgedManager.Controllers;

public class ExpenseController : Controller
{
    private readonly AppDbContext _context;
    private readonly LimitController _limitController;
    public ExpenseController(AppDbContext context)
    {
        _context = context;
        _limitController = new LimitController(_context);
    }

    // GET: Expense
    public async Task<IActionResult> Index(string? orderBy, string? date, string? category, string? amount)
    {
        var query = _context.Expenses.Include(e => e.Category).AsQueryable();

        try
        {
            if (!string.IsNullOrEmpty(date))
            {
                var dateTime = DateTime.Parse(date);
                query = query.Where(e => e.Date.Date == dateTime);
            }
        }
        catch (FormatException e)
        {
            query = query.Where(e => e.Date.Date == DateTime.Today);
        }
        if (!string.IsNullOrEmpty(category)) query = query.Where(e => e.Category.Name == category);
        if (!string.IsNullOrEmpty(amount)) query = query.Where(e => e.Amount.ToString() == amount);

        switch (orderBy)
        {
            case "Amount": query = query.OrderBy(e => e.Amount); break;
            case "Category": query = query.OrderBy(e => e.Category.Name); break;
            case "Date": query = query.OrderBy(e => e.Date); break;
        }

        var expenses = await query.ToListAsync();

        return View(expenses);
    }


    // GET: Expense/Summary/
    
    //todo repair errors when there is no Expenses in database
    public async Task<IActionResult> Summary(DateTime? startDate, DateTime? endDate)
    {
        var categoryExpenses = _context.Expenses
            .GroupBy(e => e.CategoryId)
            .Select(group => new SummaryDto
            {
                CategoryId = (int)group.Key,
                CategoryName = group.First().Category.Name,
                Score = (float)Math.Round(group.Sum(e => e.Amount))
            }).ToList();

        if (categoryExpenses.Count == 0)
        {
            return View();
        }
        var countRecords = await _context.Expenses.Select(e => e.Date.Date).Distinct().CountAsync();
        var totalExpenses = await _context.Expenses.SumAsync(e => e.Amount);

        //test\/
        var averageDays = totalExpenses / countRecords;
        ViewData["Sum"] = (float)Math.Round(totalExpenses, 2);
        ViewData["Highest"] = (float)Math.Round(categoryExpenses.Max(e => e.Score), 2);
        ViewData["categoryExpenses"] = categoryExpenses;
        ViewData["AverageDays"] = (float)Math.Round(averageDays, 2);
        ViewData["AverageWeeks"] = 100;
        ViewData["AverageMonths"] = 100;

        //test^
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
        

        if (ModelState.IsValid)
        {
            expense.Amount = Math.Round(expense.Amount, 2);
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
    public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,CategoryId,Date,Comment")] Expense expense)
    {
        if (id != expense.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                expense.Amount = Math.Round(expense.Amount, 2);
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
    public IActionResult History()
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