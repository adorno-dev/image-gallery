using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImageGallery.Web.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }        

        public int GalleryId { get; set; }

        [Required]
        [Display(Name = "Image Title")]
        public string? Title { get; set; }

        [NotMapped]
        public string? FilePath { get => Path.Combine($"/img/", Id.ToString("D6") + ".webp"); }

        [ForeignKey("GalleryId")]
        public Gallery? Gallery { get; set; }

        [Required(ErrorMessage = "Image wasn't send.")]
        [NotMapped]
        [Display(Name = "File Image")]
        public IFormFile? FileImage { get; set; }
    }
}