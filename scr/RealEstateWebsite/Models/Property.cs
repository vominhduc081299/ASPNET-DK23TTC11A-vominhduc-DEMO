
using System;
using System.ComponentModel.DataAnnotations;

public class Property
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Ti�u ??")]
    public string Title { get; set; }

    [Required]
    [Display(Name = "M� t?")]
    public string Description { get; set; }

    [Required]
    [Display(Name = "Gi�")]
    public decimal Price { get; set; }

    [Display(Name = "??a ch?")]
    public string Address { get; set; }

    [Display(Name = "Lo?i h�nh")]
    public string Type { get; set; } // V� d?: "Nh�", "C?n h?", "??t"

    [Display(Name = "Ng�y ??ng")]
    public DateTime PostedDate { get; set; } = DateTime.Now;
}
