using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RealEstateWebsite.Models
{
    public class Property
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [Display(Name = "Slug")]
        public string? Slug { get; set; }

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

        [Display(Name = "Ngày cập nhật")]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Trạng thái")]
        public string? Status { get; set; } = "Pending";
        // Accept, Pending, Rejected

        [Display(Name = "Người đăng")]
        public string? PostedBy { get; set; }

        // Navigation (nếu cần dùng thêm sau)
        public ApplicationUser? User { get; set; }

        public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
    }
}
