
using System.Xml.Serialization;
using BudgedManager.Models.Entity;
using Microsoft.AspNetCore.Mvc;

public class TestController : Controller
{

    public void Index()
    {
        var serializer = new XmlSerializer(typeof(ExpenseList));
        ExpenseList expenseList = new ExpenseList();
        expenseList.Items = new List<Expense>();

        Expense expense1 = new Expense();
        expense1.Amount = 11;
        expense1.CategoryId = 1;
        expense1.Date = DateTime.Now;
        expense1.Comment = "expense1";

        Expense expense2 = new Expense();
        expense2.Id = 1;
        expense2.Amount = 12;
        expense2.CategoryId = 1;
        expense2.Date = DateTime.Now;
        expense2.Comment = "expense2";

        Expense expense3 = new Expense();
        expense3.Amount = 13;
        expense3.CategoryId = 1;
        expense3.Date = DateTime.Now;
        expense3.Comment = "expense3";

        Expense expense4 = new Expense();
        expense4.Amount = 14;
        expense4.CategoryId = 1;
        expense4.Date = DateTime.Now;
        expense4.Comment = "expense4";

        expenseList.Items.Add(expense1);
        expenseList.Items.Add(expense2);
        expenseList.Items.Add(expense3);
        expenseList.Items.Add(expense4);


        serializer.Serialize(Console.Out, expenseList);
        Console.WriteLine();
        Console.ReadLine();
    }
}