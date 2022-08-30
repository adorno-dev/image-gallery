using ImageGallery.Web.Data;
using ImageGallery.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImageGallery.Web.Controllers
{
    public class GalleryController : Controller
    {
        private readonly DatabaseContext context;

        public GalleryController(DatabaseContext context)
        {
            this.context = context;
        }

        public IActionResult Index() => View(context.Galleries.AsNoTracking().ToList());

        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(Gallery request)
        {
            if (ModelState.IsValid)
            {
                context.Galleries.Add(request);
                context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }

        public IActionResult Edit(int? id)
        {
            Gallery? gallery;

            if (id is null)
                return NotFound();
            
            gallery = context.Galleries.Find(id);

            return gallery is null ?
                NotFound():
                View(gallery);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(Gallery request)
        {
            if (ModelState.IsValid)
            {
                context.Entry<Gallery>(request).State = EntityState.Modified;
                context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            return View(request);
        }

        public IActionResult Delete(int? id)
        {
            Gallery? gallery;

            if (id is null)
                return NotFound();
            
            gallery = context.Galleries.Find(id);

            return gallery is null ?
                NotFound():
                View(gallery);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int? id)
        {
            Gallery? gallery;

            if (id is null)
                return NotFound();
            
            gallery = context.Galleries.Find(id);

            if (gallery is not null)
            {
                context.Galleries.Remove(gallery);
                context.SaveChanges();
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}