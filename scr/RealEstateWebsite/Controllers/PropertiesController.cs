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
    // public async Task<IActionResult> Edit(int id)
    // {
    //     // Fetch property by id
    //     var property = await GetPropertyByIdAsync(id);
    //     if (property == null)
    //     {
    //         _logger.LogWarning($"Property with ID {id} not found.");
    //         return NotFound();
    //     }

    //     return View(property);
    // }

    // POST: Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Property property)
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
            _context.Update(property); // Update the property in the database
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Property with ID {id} updated successfully.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating property with ID {id}.");
            return View(property);
        }
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

    // POST: Delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            // Add property deletion logic here
            Console.WriteLine($"Deleting property with ID: {id}");
            // Console.WriteLine($"Property: {System.Text.Json.JsonSerializer.Serialize(property)}");
            var property = await GetPropertyByIdAsync(id);
            if (property == null)
            {
                _logger.LogWarning($"Property with ID {id} not found.");
                return NotFound();
            }
            _context.Properties.Remove(property);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Property with ID {id} deleted successfully.");
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while deleting property with ID {id}.");
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
