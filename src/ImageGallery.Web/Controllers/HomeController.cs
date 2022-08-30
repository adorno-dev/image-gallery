using ImageGallery.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImageGallery.Web.Controllers;

public class HomeController : Controller
{
    private readonly DatabaseContext context;

    public HomeController(DatabaseContext context)
    {
        this.context = context;
    }

    public IActionResult Index()
    {
        var galleries = context.Galleries.Include(g => g.Images).AsNoTracking().ToList();

        return View(galleries);
    }
}
