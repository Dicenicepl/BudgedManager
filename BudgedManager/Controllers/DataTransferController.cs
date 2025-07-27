using BudgedManager.Models;
using BudgedManager.Services;
using Microsoft.AspNetCore.Mvc;

public class DataTransferController : Controller
{
    private Import import;
    private AppDbContext _db;
    public DataTransferController(AppDbContext db)
    {
        _db = db;
    }
    public IActionResult Menu()
    {
        return View();
    }
    [HttpPost]
    public void Menu(IFormFile FormFile)
    {
        using var File = new FileStream(FormFile.Name, FileMode.Create);
        FormFile.CopyTo(File);
        File.Close();
        import = new Import(_db);
        import.Start("xml", File.Name);

    }
}