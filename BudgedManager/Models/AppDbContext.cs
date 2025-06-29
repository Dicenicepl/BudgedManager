using BudgedManager.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace BudgedManager.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    public DbSet<Limit> Limit { get; set; } = default!;
}