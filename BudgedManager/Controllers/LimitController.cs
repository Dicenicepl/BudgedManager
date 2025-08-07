using BudgedManager.Models;
using BudgedManager.Models.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace BudgedManager.Controllers;

public class LimitController : Controller
{
    private readonly AppDbContext _context;

    public LimitController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Limit
    public async Task<IActionResult> Index()
    {
        var limits = _context.Limit.Select(limit => new Limit
        {
            Id = limit.Id,
            CategoryId = limit.CategoryId,
            Category = limit.Category,
            LimitAlert = limit.LimitAlert,
            LimitWarning = limit.LimitWarning
        }).ToList();
        return View(limits);
    }

    // GET: Limit/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var limit = await _context.Limit
            .FirstOrDefaultAsync(m => m.Id == id);
        if (limit == null) return NotFound();

        return View(limit);
    }

    // GET: Limit/Create
    public IActionResult Create()
    {
        ViewData["Category"] = new SelectList(_context.Categories, "Id", "Name");
        return View();
    }

    // POST: Limit/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,CategoryId,LimitWarning,LimitAlert")] Limit limit)
    {
        if (ModelState.IsValid)
        {
            _context.Add(limit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(limit);
    }

    // GET: Limit/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var limit = await _context.Limit.FindAsync(id);
        if (limit == null) return NotFound();
        return View(limit);
    }

    // POST: Limit/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,LimitWarning,LimitAlert")] Limit limit)
    {
        if (id != limit.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(limit);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LimitExists(limit.Id)) return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(limit);
    }

    // GET: Limit/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var limit = await _context.Limit
            .FirstOrDefaultAsync(m => m.Id == id);
        if (limit == null) return NotFound();

        return View(limit);
    }

    // POST: Limit/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var limit = await _context.Limit.FindAsync(id);
        if (limit != null) _context.Limit.Remove(limit);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    // Function created for checking if new Expense is in limits
    public bool IsLimitIsBigger(int categoryId, decimal amount)
    {
        var limit = _context.Limit.FirstOrDefault(c => c.CategoryId == categoryId);
        
        if (limit == null)
        {
            return true;
        }
        
        return limit.LimitWarning > amount || limit.LimitAlert > amount;
    }

    [HttpGet]
    public String LimitWarning(int? categoryId)
    {
        var limit = _context.Limit.FirstOrDefault(m => m.CategoryId == categoryId);
        return limit.ToJson();
    }
    
    private bool LimitExists(int id)
    {
        return _context.Limit.Any(e => e.Id == id);
    }
}