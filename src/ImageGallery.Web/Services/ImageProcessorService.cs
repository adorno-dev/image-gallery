using ImageGallery.Web.Services.Contracts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageGallery.Web.Services
{
    public class ImageProcessorService : IImageProcessorService
    {
        private static async Task<bool> SaveImageAsWebpAsync(string path, MemoryStream ms, bool square = true)
        {
            var img = await Image.LoadAsync(ms);
            var ext = path.Substring(path.LastIndexOf('.')).ToLower();

            if (square)
            {
                var length = img.Size();
                var minorSide = (length.Height < length.Width) ? length.Height : length.Width;

                img.Mutate(options =>
                {
                    options.Resize(new ResizeOptions
                    {
                        Size = new Size(minorSide, minorSide),
                        Mode = ResizeMode.Crop
                    });

                    options.BackgroundColor(new Rgba32(255, 255, 255, 0));
                });
            }

            path = path.Replace(ext, ".webp");

            await img.SaveAsWebpAsync(path);

            return true;
        }

        private void LeftRotation(Image image) => image.Mutate(o => o.Rotate(-90));
        private void RightRotation(Image image) => image.Mutate(o => o.Rotate(90));
        private void HorizontalFlip(Image image) => image.Mutate(o => o.Flip(FlipMode.Horizontal));
        private void VerticalFlip(Image image) => image.Mutate(o => o.Flip(FlipMode.Vertical));
        private void GrayScale(Image image) => image.Mutate(o => o.Grayscale());
        private void Sepia(Image image) => image.Mutate(o => o.Sepia());
        private void GaussianBlur(Image image) => image.Mutate(o => o.GaussianBlur());
        private void Invert(Image image) => image.Mutate(o => o.Invert());

        public async Task<bool> ApplyEffectAsync(string path, ImageEffect effect)
        {
            Image image;

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                image = await Image.LoadAsync(fs);
            
            switch (effect)
            {
                case ImageEffect.LeftRotation: LeftRotation(image); break;
                case ImageEffect.RightRotation: RightRotation(image); break;
                case ImageEffect.HorizontalFlip: HorizontalFlip(image); break;
                case ImageEffect.VerticalFlip: VerticalFlip(image); break;
                case ImageEffect.GrayScale: GrayScale(image); break;
                case ImageEffect.Sepia: Sepia(image); break;
                case ImageEffect.GaussianBlur: GaussianBlur(image); break;
                case ImageEffect.Invert: Invert(image); break;
            }

            await image.SaveAsync(path);

            return true;
        }

        public async Task<bool> DeleteImageAsync(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);

                    return true;
                }
                catch (IOException)
                {
                    return await Task.FromResult<bool>(false);
                }
            }

            return await Task.FromResult<bool>(false);
        }

        public async Task<bool> SaveUploadImageAsync(string path, IFormFile image)
        {
            if (image is null)
                return false;
            
            using (var ms = new MemoryStream())
            {
                await image.CopyToAsync(ms);
                ms.Position = 0;
                return await SaveImageAsWebpAsync(path, ms, true);
            }
        }
    }
}