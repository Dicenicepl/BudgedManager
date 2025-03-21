using BudgedManager.Models;
using BudgedManager.Models.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgedManager.Controllers;

public class SubscriptionController : Controller
{
    private readonly AppDbContext _context;

    public SubscriptionController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Subscription
    public async Task<IActionResult> Index()
    {
        return View(await _context.Subscriptions.ToListAsync());
    }

    // GET: Subscription/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(m => m.Id == id);
        if (subscription == null) return NotFound();

        return View(subscription);
    }

    // GET: Subscription/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Subscription/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind(
            "Id,SubscriptionName,SubscriptionDescription,SubscriptionStartDate,SubscriptionPaymentPeriod,SubscriptionPrice")]
        Subscription subscription)
    {
        if (ModelState.IsValid)
        {
            _context.Add(subscription);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(subscription);
    }


    // GET: Subscription/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var subscription = await _context.Subscriptions.FindAsync(id);
        if (subscription == null) return NotFound();
        return View(subscription);
    }

    // POST: Subscription/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind(
            "id,subscriptionName,subscriptionDescription,subscriptionStartDate,subscriptionPaymentPeriod,subsriptionPrice")]
        Subscription subscription)
    {
        if (id != subscription.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(subscription);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubscriptionExists(subscription.Id)) return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(subscription);
    }

    // GET: Subscription/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(m => m.Id == id);
        if (subscription == null) return NotFound();

        return View(subscription);
    }

    // POST: Subscription/Delete/5
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var subscription = await _context.Subscriptions.FindAsync(id);
        if (subscription != null) _context.Subscriptions.Remove(subscription);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool SubscriptionExists(int id)
    {
        return _context.Subscriptions.Any(e => e.Id == id);
    }
}