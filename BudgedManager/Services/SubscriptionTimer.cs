using System.Timers;
using BudgedManager.Models;
using BudgedManager.Models.Entity;
using Timer = System.Timers.Timer;

namespace BudgedManager.Services;

public class SubscriptionTimer
{
    private static Timer _timer;
    private readonly IServiceProvider _serviceProvider;
    private double _milliseconds = 2000; //2s

    public SubscriptionTimer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }


    public void Start()
    {
        SetTimer();
        _milliseconds = GetNextSubscriptionTime();
        _timer.Start();
    }

    private void SetTimer()
    {
        _timer = new Timer(_milliseconds);
        _timer.Elapsed += ExecuteTimerTask;
        _timer.AutoReset = false;
    }

    private void ExecuteTimerTask(object? sender, ElapsedEventArgs e)
    {
        UpdateSubscriptionStartDate();
        Start();
    }

    private double GetNextSubscriptionTime()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var today = DateTime.Now;
            try
            {
                var subscription = context.Subscriptions.Where(s => s.SubscriptionStartDate > today)
                    .OrderBy(s => s.SubscriptionStartDate).First();
                var timeBetween = subscription.SubscriptionStartDate.Subtract(today);
                if (timeBetween.TotalMilliseconds > 0) return timeBetween.TotalMilliseconds;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("Error method: 'GetNextSubscriptionTime': \n" + e + "\n--");
            }

            return 5000.0; //5s
        }
    }

    private void UpdateSubscriptionStartDate()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var subscriptions = context.Subscriptions.Where(x => x.SubscriptionStartDate <= DateTime.Now).ToList();
            foreach (var subscription in subscriptions)
            {
                var expense = new Expense
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
    }

    // private void LogMessage(System.Timers.ElapsedEventArgs e)
    // {
    //     Console.WriteLine("Logging message, TIME: {0:HH:mm:ss.ffff}", e.SignalTime);
    // }
}