using System.ComponentModel.DataAnnotations;

namespace ImageGallery.Web.Models
{
    public class Gallery
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }        

        [Required]
        [Display(Name = "Gallery Title")]
        public string? Title { get; set; }

        public ICollection<Image> Images { get; set; } = Array.Empty<Image>().ToList();
    }
}