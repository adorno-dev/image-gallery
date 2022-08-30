namespace ImageGallery.Web.Services.Contracts
{
    public interface IImageProcessorService
    {
        Task<bool> SaveUploadImageAsync(string path, IFormFile image);
        Task<bool> DeleteImageAsync(string path);
        Task<bool> ApplyEffectAsync(string path, ImageEffect effect);   
    }
}