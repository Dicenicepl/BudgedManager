using BudgedManager.Controllers;
using BudgedManager.Models;
using BudgedManager.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace BudgedManager.Services;


public class SubscriptionTimer
{
    private static System.Timers.Timer _timer;
    private readonly IServiceProvider _serviceProvider;

    public SubscriptionTimer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }


    public void Start()
    {
        SetTimer();
        _timer.Start();
    }

    private void SetTimer()
    {
        _timer = new System.Timers.Timer(GetNextSubscriptionTime());
        _timer.Elapsed += ExecuteSubscription;
    }

    private double GetNextSubscriptionTime()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var subscription = context.Subscriptions.FirstOrDefault(s => s.subscriptionStartDate > DateTime.Now);
            var timeBetween = subscription.subscriptionStartDate - DateTime.Now;
            if (timeBetween.TotalMilliseconds > 0)
            {            
                return timeBetween.TotalMilliseconds;
            }
            return 20000;
        }
        
    }

    private void ExecuteSubscription(object sender, System.Timers.ElapsedEventArgs e)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var test = context.Subscriptions.Where(x => x.subscriptionStartDate <= DateTime.Now.Date).ToList();
            foreach (var subscription in test)
            {
                subscription.subscriptionStartDate = DateTime.Now.Date.AddDays(subscription.subscriptionPaymentPeriod);
                context.Subscriptions.Update(subscription);
                Expense expense = new Expense
                {
                    Amount = subscription.subscriptionPrice,
                    Date = subscription.subscriptionStartDate,
                    Comment = subscription.subscriptionName + " - " + subscription.subscriptionDescription
                };
                context.Expenses.Add(expense);
                context.SaveChanges();
            }
            Console.WriteLine("--");

        }
        Start();
    }
    
    private void LogMessage(object sender, System.Timers.ElapsedEventArgs e)
    {
        Console.WriteLine("Logging message, TIME: {0:HH:mm:ss.ffff}", e.SignalTime);
    }
}