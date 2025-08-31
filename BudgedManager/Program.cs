using BudgedManager.Models;
using BudgedManager.Models.Entity;
using BudgedManager.Services;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Secured")));
builder.Services.AddSingleton<SubscriptionTimer>();
// builder.Logging.ClearProviders(); //Turned off logs
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!database.Categories.Any(c => c.Id == 1))
    {
        database.Categories.Add(new Category() { Id = 1, Name = "None", Description = "Created for non categorised" });
        database.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapControllerRoute(
    "default",
    "{controller=Home}/{action=Index}/{id?}");
// method will not work correctly
// app.Services.GetRequiredService<SubscriptionTimer>().Start();

app.Run();