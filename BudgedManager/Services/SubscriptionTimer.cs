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
        _milliseconds = GetNextSubscriptionTime();
        SetTimer();
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
                var subscription = context.Subscriptions.Where(s => s.StartDate > today)
                    .OrderBy(s => s.StartDate).First();
                var timeBetween = subscription.StartDate.Subtract(today);
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

            var subscriptions = context.Subscriptions.Where(x => x.StartDate <= DateTime.Now).ToList();
            foreach (var subscription in subscriptions)
            {
                var expense = new Expense
                {
                    Amount = subscription.Price,
                    Date = subscription.StartDate,
                    Comment = subscription.Name + " - " + subscription.Description
                };
                context.Expenses.Add(expense);

                subscription.StartDate = DateTime.Now.AddDays(subscription.PaymentPeriod);
                context.Subscriptions.Update(subscription);
            }

            context.SaveChanges();
        }
    }
}