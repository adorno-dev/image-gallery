using ImageGallery.Web.Data;
using ImageGallery.Web.Models;
using ImageGallery.Web.Services;
using ImageGallery.Web.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImageGallery.Web.Controllers
{
    public class ImageController : Controller
    {
        private readonly IImageProcessorService imageProcessorService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly DatabaseContext context;

        private string GetImagePath(string imagesFolder, int imageId, string extension)
        {
            string root = webHostEnvironment.WebRootPath + imagesFolder;
            string file = $"{imageId:D6}{extension}";

            return Path.Combine(root, file).Replace('/', Path.DirectorySeparatorChar);
        }

        public ImageController(
            IImageProcessorService imageProcessorService, 
            IWebHostEnvironment webHostEnvironment, 
            DatabaseContext context)
        {
            this.imageProcessorService = imageProcessorService;
            this.webHostEnvironment = webHostEnvironment;
            this.context = context;
        }

        public IActionResult Index(int? id)
        {
            Gallery? gallery;

            if (id is null)
                return NotFound();
            
            gallery = context.Galleries.Find(id);

            if (gallery is null)
                return NotFound();
            
            context.Entry<Gallery>(gallery).Collection<Image>(o => o.Images).Load();

            this.ViewBag.Id = id.Value;
            this.ViewBag.Title = gallery.Title;

            return View(gallery.Images?.ToList());
        }

        public IActionResult Create(int? id)
        {
            Gallery? gallery;

            if (id is null)
                return NotFound();
            
            gallery = context.Galleries.Find(id);

            if (gallery is null)
                return NotFound();
            
            return View(new Image { GalleryId = gallery.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Image request)
        {
            if (ModelState.IsValid)
            {
                context.Images.Add(request);

                if (context.SaveChanges() > 0)
                {
                    string filePath = GetImagePath("/img/", request.Id, ".webp");

                    if (request.FileImage is not null)
                        await imageProcessorService.SaveUploadImageAsync(filePath, request.FileImage);
                }

                return RedirectToAction(nameof(Index), new { id = request.GalleryId });
            }

            return View(request);
        }

        public IActionResult Edit(int? id)
        {
            Image? image;

            if (id is null)
                return NotFound();
            
            image = context.Images.Find(id);

            if (image is null)
                return NotFound();
            
            return View(image);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Image request)
        {
            ModelState.Remove("FileImage");

            if (ModelState.IsValid)
            {
                context.Entry<Image>(request).State = EntityState.Modified;

                if (context.SaveChanges() > 0)
                {
                    string filePath = GetImagePath("/img/", request.Id, ".webp");

                    if (request.FileImage is not null)
                        await imageProcessorService.SaveUploadImageAsync(filePath, request.FileImage);
                }

                return RedirectToAction(nameof(Index), new { id = request.GalleryId });
            }

            return View(request);
        }

        public IActionResult Delete(int? id)
        {
            Image? image;

            if (id is null)
                return NotFound();
            
            image = context.Images.Find(id);

            if (image is null)
                return NotFound();
            
            return View(image);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var image = context.Images.Find(id);

            if (image is not null)
            {
                context.Images.Remove(image);

                if (context.SaveChanges() > 0)
                {
                    string filePath = GetImagePath("/img/", image.Id, ".webp");

                    if (image.FilePath is not null)
                        await imageProcessorService.DeleteImageAsync(image.FilePath);
                }
            }

            return RedirectToAction(nameof(Index), new { id = image?.GalleryId });
        }

        public IActionResult Effects(int? id)
        {
            Image? image;

            if (id is null)
                return NotFound();
            
            image = context.Images.Find(id);

            if (image is null)
                return NotFound();
            
            return View(image);
        }

        [HttpPost]
        public IActionResult ApplyEffects(int id, string effect)
        {
            string filePath = GetImagePath("/img/", id, ".webp");

            switch (effect)
            {
                case "rl": imageProcessorService.ApplyEffectAsync(filePath, ImageEffect.LeftRotation).Wait(); break;
                case "rr": imageProcessorService.ApplyEffectAsync(filePath, ImageEffect.RightRotation).Wait(); break;
                case "fh": imageProcessorService.ApplyEffectAsync(filePath, ImageEffect.HorizontalFlip).Wait(); break;
                case "fv": imageProcessorService.ApplyEffectAsync(filePath, ImageEffect.VerticalFlip).Wait(); break;
                case "gs": imageProcessorService.ApplyEffectAsync(filePath, ImageEffect.GrayScale).Wait(); break;
                case "sp": imageProcessorService.ApplyEffectAsync(filePath, ImageEffect.Sepia).Wait(); break;
                case "iv": imageProcessorService.ApplyEffectAsync(filePath, ImageEffect.Invert).Wait(); break;
                case "gb": imageProcessorService.ApplyEffectAsync(filePath, ImageEffect.GaussianBlur).Wait(); break;
            }

            return RedirectToAction(nameof(Effects), new { id });
        }
    }
}