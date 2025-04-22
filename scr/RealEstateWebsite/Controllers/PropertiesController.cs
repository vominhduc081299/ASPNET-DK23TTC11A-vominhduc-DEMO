namespace RealEstateWebsite.Controllers;

using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealEstateWebsite.Data;
using RealEstateWebsite.Models;

[Authorize]
public class PropertiesController : Controller
{
    private readonly ILogger<PropertiesController> _logger;
    private readonly ApplicationDbContext _context;

    public PropertiesController(ILogger<PropertiesController> logger, ApplicationDbContext context)
    {
        _context = context;
        _logger = logger;
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
                            "wwwroot/images",
                            fileName
                        );

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var image = new PropertyImage
                        {
                            PropertyId = property.Id,
                            ImagePath = "/images/" + fileName,
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
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var image = new PropertyImage
                        {
                            PropertyId = property.Id,
                            ImagePath = "/images/" + fileName
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
        var properties = await _context.Properties.ToListAsync();
        return View(properties);
    }
}
