using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RealEstateWebsite.Models
{
    public class Property
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tiêu Đề")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Giá")]
        [Precision(18, 2)] // Specify precision and scale
        public decimal Price { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Loại hình")]
        public string Type { get; set; }

        [Display(Name = "Ngày đăng")]
        public DateTime PostedDate { get; set; } = DateTime.Now;

        public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
    }
}
