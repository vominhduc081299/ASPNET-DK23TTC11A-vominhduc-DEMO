namespace RealEstateWebsite.Controllers;

using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealEstateWebsite.Data;
using RealEstateWebsite.Models;
using Microsoft.AspNetCore.Identity;

[Authorize]
public class PropertiesController : Controller
{
    private readonly ILogger<PropertiesController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PropertiesController(ILogger<PropertiesController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }

    // GET: PropertyList
    [AllowAnonymous]
    public async Task<IActionResult> PropertyList(string searchString, string propertyType, decimal? minPrice, decimal? maxPrice)
    {
        var propertiesQuery = _context.Properties.AsQueryable();
        // debug propertiesQuery
        _logger.LogInformation($"searchString: {searchString}");
        _logger.LogInformation($"propertyType: {propertyType}");
        _logger.LogInformation($"minPrice: {minPrice}");
        _logger.LogInformation($"maxPrice: {maxPrice}");


        // Apply filters if provided
        // if (!string.IsNullOrEmpty(searchString))
        // {
        //     propertiesQuery = propertiesQuery.Where(p => 
        //         p.Title.Contains(searchString) || 
        //         p.Description.Contains(searchString) ||
        //         p.Address.Contains(searchString));
        // }
        
        // if (!string.IsNullOrEmpty(propertyType))
        // {
        //     propertiesQuery = propertiesQuery.Where(p => p.PropertyType == propertyType);
        // }
        // 293636
        // if (minPrice.HasValue)
        // {
        //     propertiesQuery = propertiesQuery.Where(p => p.Price >= minPrice.Value);
        // }
        
        // if (maxPrice.HasValue)
        // {
        //     propertiesQuery = propertiesQuery.Where(p => p.Price <= maxPrice.Value);
        // }
        
        int pageSize = 30;
        int pageNumber = 2;
        var properties = await propertiesQuery
            .Include(p => p.Images)
            // .Include(p => p.User)
            // .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        _logger.LogInformation($"Retrieved {properties.Count} properties matching filter criteria");
        return View(properties);
    }

    // GET: Propertydetails
    [AllowAnonymous]
    [Route("properties/PropertyDetails/{slug}")]
    public async Task<IActionResult> PropertyDetails(string slug)
    {
        _logger.LogInformation($"slug: {slug}");
        var property = await _context.Properties
            .Include(p => p.Images)
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Slug == slug);
        return property == null ? NotFound() : View(property);
    }

    // GET: Create
    public IActionResult Create() => View();

    // POST: Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Property property, List<IFormFile> ImageFiles)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for property creation.");
            return View(property);
        }

        try
        {
            // Gán người đăng
            property.PostedBy = _userManager.GetUserId(User);
            property.Slug = GenerateSlug(property.Title);
            _context.Add(property);
            await _context.SaveChangesAsync();

            // var result = await _propertyService.CreateAsync(property);
            if (ImageFiles != null && ImageFiles.Count > 0)
            {
                foreach (var file in ImageFiles)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot/images/user",
                            fileName
                        );

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var image = new PropertyImage
                        {
                            PropertyId = property.Id,
                            ImagePath = "/images/user/" + fileName,
                        };

                        _context.PropertyImages.Add(image);
                    }
                }

                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("Property created successfully.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating property.");
            return View(property);
        }
    }

    // GET: Edit
    public async Task<IActionResult> Edit(int id)
    {
        var property = await _context.Properties
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (property == null)
            return NotFound();

        return View(property);
    }

    // POST: Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Property property, List<IFormFile> NewImages)
    {
        if (id != property.Id)
        {
            _logger.LogWarning("Property ID mismatch.");
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for property editing.");
            return View(property);
        }

        try
        {
            property.Slug = GenerateSlug(property.Title);
            // Update property details
            _context.Update(property);
            await _context.SaveChangesAsync();

            // process new images
            if (NewImages != null && NewImages.Count > 0)
            {
                foreach (var file in NewImages)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/user", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var image = new PropertyImage
                        {
                            PropertyId = property.Id,
                            ImagePath = "/images/user/" + fileName
                        };

                        _context.PropertyImages.Add(image);
                    }
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi sửa bài đăng ID {id}");
            return View(property);
        }
    }

    // GET: Details
    public async Task<IActionResult> Details(int id)
    {
        var property = await _context.Properties
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id);
        return property == null ? NotFound() : View(property);
    }

    // GET: Delete
    public async Task<IActionResult> Delete(int id)
    {
        var property = await GetPropertyByIdAsync(id);
        if (property == null)
        {
            _logger.LogWarning($"Property with ID {id} not found.");
            return NotFound();
        }

        return View(property);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var property = await _context.Properties
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
            {
                _logger.LogWarning($"Không tìm thấy Property ID: {id}");
                return NotFound();
            }

            foreach (var image in property.Images)
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            _context.PropertyImages.RemoveRange(property.Images);

            _context.Properties.Remove(property);

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Đã xoá Property + ảnh kèm theo (ID: {id})");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi xoá Property ID: {id}");
            return RedirectToAction(nameof(Delete), new { id });
        }
    }


    private async Task<Property?> GetPropertyByIdAsync(int id)
    {
        // Simulate fetching property by ID
        return await _context.Properties.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IActionResult> Index()
    {
        var properties = await _context.Properties.Include(p => p.User).ToListAsync();
        return View(properties);
    }

    private string GenerateSlug(string title)
    {
        // convert title to slug
        var slug = title.ToLower()
            .Replace(" ", "-")
            .Replace("đ", "d")
            .Replace("á", "a")
            .Replace("à", "a")
            .Replace("ạ", "a")
            .Replace("ả", "a")
            .Replace("ã", "a")
            .Replace("é", "e")
            .Replace("è", "e")
            .Replace("ẹ", "e")
            .Replace("ẻ", "e")
            .Replace("ẽ", "e")
            .Replace("ê", "e")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace(".", "")
            .Replace(",", "")
            .Replace("?", "")
            .Replace("/", "")
            .Replace("\\", "")
            .Replace("&", "and");
        return slug;
    }
}
