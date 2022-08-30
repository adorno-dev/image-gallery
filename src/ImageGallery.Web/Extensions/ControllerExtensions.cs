using System.Text.Json;
using ImageGallery.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ImageGallery.Web.Extensions
{
    public static class ControllerExtensions
    {
        public static void Show(this Controller @this, string text, bool error = false)
        {
            @this.TempData.Add("message", JsonSerializer.Serialize(new MessageVM(error ? MessageType.Error : MessageType.Information, text)));
        }        
    }
}