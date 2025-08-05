using BudgedManager.Models;
using BudgedManager.Services;
using Microsoft.AspNetCore.Mvc;

public class DataTransferController : Controller
{
    private Import import;
    private Export export;
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
    public void Import(IFormFile FormFile)
    {
        using var File = new FileStream(FormFile.FileName, FileMode.Create);
        FormFile.CopyTo(File);
        File.Close();
        import = new Import(_db);
        import.Start(FormFile.ContentType.Remove(0,FormFile.ContentType.IndexOf('/') + 1), File.Name);
    }
    public void Export(string? type)
    {
        export = new Export(_db);
        export.Start(type);
    }
}