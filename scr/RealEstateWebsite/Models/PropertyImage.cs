using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace RealEstateWebsite.Models
{
    public class PropertyImage
    {
        public int Id { get; set; }
        public string ImagePath { get; set; } = string.Empty;

        // Quan hệ với Property
        public int PropertyId { get; set; }
        public Property Property { get; set; } = null!;
    }
}
