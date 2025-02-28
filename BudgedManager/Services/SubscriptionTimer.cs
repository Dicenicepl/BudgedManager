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
        Console.WriteLine("Starting subscription timer");
        SetTimer();
        _timer.Start();
    }

    private void SetTimer()
    {
        _timer = new System.Timers.Timer(GetNextSubscriptionTime());
        _timer.Elapsed += ExecuteSubscription;
        _timer.AutoReset = false;
    }

    private double GetNextSubscriptionTime()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
        //     var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        //     var today = DateTime.Now;
        //     TimeSpan timeBetween;
        //     try
        //     {
        //         var subscription = context.Subscriptions.Where(s => s.SubscriptionStartDate > today).OrderBy(s => s.SubscriptionStartDate).First();
        //         timeBetween = subscription.SubscriptionStartDate.Subtract(today);
        //         if (timeBetween.TotalMilliseconds > 0)
        //         {            
        //             return timeBetween.TotalMilliseconds;
        //         }
        //     }
        //     catch (InvalidOperationException e)
        //     {
        //         Console.WriteLine("Error method: 'GetNextSubscriptionTime': \n" + e + "\n--");
        //     }
            return 5000.0; //5s
        }
        
    }

    public void ExecuteSubscription(object? sender, System.Timers.ElapsedEventArgs? e)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var subscriptions = context.Subscriptions.Where(x => x.SubscriptionStartDate <= DateTime.Now).ToList();
            foreach (var subscription in subscriptions)
            {
                Expense expense = new Expense
                {
                    Amount = subscription.SubscriptionPrice,
                    Date = subscription.SubscriptionStartDate,
                    Comment = subscription.SubscriptionName + " - " + subscription.SubscriptionDescription
                };
                context.Expenses.Add(expense);
                
                subscription.SubscriptionStartDate = DateTime.Now.AddDays(subscription.SubscriptionPaymentPeriod);
                context.Subscriptions.Update(subscription);
            }
            context.SaveChanges();

        }
        Start();
    }
    
    private void LogMessage(System.Timers.ElapsedEventArgs e)
    {
        Console.WriteLine("Logging message, TIME: {0:HH:mm:ss.ffff}", e.SignalTime);
    }
}