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
    
    public async Task<IActionResult> Summary(DateTime? startDate, DateTime? endDate)
    {
        var expensesQuery = _context.Expenses.Include(e => e.Category).AsQueryable();

        if (startDate.HasValue)
        {
            expensesQuery = expensesQuery.Where(e => e.Date >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            expensesQuery = expensesQuery.Where(e => e.Date <= endDate.Value);
        }

        var categoryExpenses = await expensesQuery
            .GroupBy(e => new { e.CategoryId, e.Category.Name })
            .Select(group => new SummaryDto
            {
                CategoryId = group.Key.CategoryId,
                CategoryName = group.Key.Name,
                Score = (float)Math.Round(group.Sum(e => e.Amount), 2)
            })
            .ToListAsync();

        if (!categoryExpenses.Any())
        {
            return View();
        }

        var countRecords = await expensesQuery.Select(e => e.Date.Date).Distinct().CountAsync();
        var totalExpenses = await expensesQuery.SumAsync(e => e.Amount);

        var averageDays = totalExpenses / (countRecords > 0 ? countRecords : 1);
        var averageWeeks = totalExpenses / (decimal)(countRecords > 0 ? countRecords / 7.0 : 1);
        var averageMonths = totalExpenses / (decimal)(countRecords > 0 ? countRecords / 30.0 : 1);

        ViewData["Sum"] = (float)Math.Round(totalExpenses, 2);
        ViewData["Highest"] = (float)Math.Round(categoryExpenses.Max(e => e.Score), 2);
        ViewData["categoryExpenses"] = categoryExpenses;
        ViewData["AverageDays"] = (float)Math.Round(averageDays, 2);
        ViewData["AverageWeeks"] = (float)Math.Round(averageWeeks, 2);
        ViewData["AverageMonths"] = (float)Math.Round(averageMonths, 2);

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
            _limitController.AddAmount(expense.CategoryId, expense.Amount);
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