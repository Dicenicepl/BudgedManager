using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BudgedManager.Models;
using BudgedManager.Models.Entity;

namespace BudgedManager.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly AppDbContext _context;

        public ExpenseController(AppDbContext context)
        {
            _context = context;
        }


        // GET: Expense

        public async Task<IActionResult> Index(string? orderBy, string? date, string? category, string? amount)
        {
            var expenses = await _context.Expenses.Include(
                    e => e.Category)
                .ToListAsync();
            //todo try to optimize that process 
            if (date != null)
            {
                /*
                 * OPTIONAL: Date.ToString("d") creates example: "11/12/2024"
                 */
                expenses = new List<Expense>(expenses.Where(e => e.Date.Date == DateTime.Parse(date)));
            }
            if (category != null)
            {
                expenses = new List<Expense>(expenses.Where(s => s.Category.Name.Equals(category)));
            }
            if (amount != null)
            {
                expenses = new List<Expense>(expenses.Where(s => s.Amount.ToString() == amount));
            }
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
            
            
            var categoryExpenses 
                = _context.Expenses.GroupBy(e => e.CategoryId).Select(
                    group => new
                    {
                        CategoryId = group.Key,
                        CategoryName = group.First().Category.Name,
                        Amount = group.Sum(e => e.Amount)
                    }).ToList();

            var countRecords = await _context.Expenses.Select(e => e.Date.Date).Distinct().CountAsync();
            
            var totalExpenses = await _context.Expenses.SumAsync(e => e.Amount);

            var averageDays = totalExpenses / countRecords;
            var averageWeeks = totalExpenses / (countRecords / 7.0);
            var averageMonths = totalExpenses / (countRecords / 30.0);


            ViewData["Sum"] = totalExpenses;
            ViewData["Highest"] = categoryExpenses.Max(e => e.Amount);
            ViewData["categoryExpenses"] = categoryExpenses;
            ViewData["AverageDays"] = averageDays;
            ViewData["AverageWeeks"] = averageWeeks;
            ViewData["AverageMonths"] = averageMonths;
            
            return View();
        }

        // GET: Expense/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expenses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (expense == null)
            {
                return NotFound();
            }

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
                _context.Add(expense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            if (expense.Amount <= 0.00)
            {
                return BadRequest();
            }
            return View(expense);
        }

        // GET: Expense/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
            {
                return NotFound();
            }
            return View(expense);
        }

        // POST: Expense/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,Category,Date,Comment")] Expense expense)
        {
            if (id != expense.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseExists(expense.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(expense);
        }

        // GET: Expense/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expense = await _context.Expenses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        // POST: Expense/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExpenseExists(int id)
        {
            return _context.Expenses.Any(e => e.Id == id);
        }
    }
}
