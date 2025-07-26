using Microsoft.AspNetCore.Mvc;

public class DataTransferController : Controller
{
    public IActionResult Menu()
    {
        return View();
    }
    [HttpPost]
    public void Menu(IFormFile import)
    {
        using var File = new FileStream(import.Name, FileMode.Create);
        import.CopyTo(File);
        
    }
}