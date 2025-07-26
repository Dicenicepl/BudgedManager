using Microsoft.AspNetCore.Mvc;

namespace BudgedManager.Views.DataTransfer;

public class TransferMenu_cshtml : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}