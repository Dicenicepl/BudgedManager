using BudgedManager.Models;
using BudgedManager.Models.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgedManager.Controllers;

public class CategoryController : Controller
{
    private readonly AppDbContext _context;

    public CategoryController(AppDbContext context)
    {
        _context = context;
       
    }

    // GET: Category
    public async Task<IActionResult> Index()
    {
        if (!_context.Expenses.Any()) return View(await _context.Categories.ToListAsync());
        
        var groupBy = _context.Expenses
            .GroupBy(exp => exp.CategoryId).Select(group =>
                new
                {
                    CategoryName = _context.Categories.FirstOrDefault(category => category.Id == group.Key).Name,
                    Count = group.Count()
                })
            .OrderByDescending(x => x.Count)
            .FirstOrDefault();

        ViewData["CategoryName"] = groupBy.CategoryName;
        ViewData["Count"] = groupBy.Count;

        return View(await _context.Categories.ToListAsync());
            
    }

    // GET: Category/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var category = await _context.Categories
            .FirstOrDefaultAsync(m => m.Id == id);
        if (category == null) return NotFound();

        return View(category);
    }

    // GET: Category/Create
    public IActionResult Create()
    {
        return View();
    }


    // POST: Category/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Description")] Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(category);
    }

    // GET: Category/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();
        return View(category);
    }

    // POST: Category/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Category category)
    {
        if (id != category.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(category.Id)) return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(category);
    }

    // GET: Category/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var category = await _context.Categories
            .FirstOrDefaultAsync(m => m.Id == id);
        if (category == null) return NotFound();

        return View(category);
    }

    // POST: Category/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        //todo better security for default category, for now there is no guarantise for a empty slot on id = 1
        var category = await _context.Categories.FindAsync(id);
        if (category != null && id != 1) _context.Categories.Remove(category);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

  
    
    private bool CategoryExists(int id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
    
}