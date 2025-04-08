
using System;
using System.ComponentModel.DataAnnotations;

public class Property
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Tiêu ??")]
    public string Title { get; set; }

    [Required]
    [Display(Name = "Mô t?")]
    public string Description { get; set; }

    [Required]
    [Display(Name = "Giá")]
    public decimal Price { get; set; }

    [Display(Name = "??a ch?")]
    public string Address { get; set; }

    [Display(Name = "Lo?i hình")]
    public string Type { get; set; } // Ví d?: "Nhà", "C?n h?", "??t"

    [Display(Name = "Ngày ??ng")]
    public DateTime PostedDate { get; set; } = DateTime.Now;
}
