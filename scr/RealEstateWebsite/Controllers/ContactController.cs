namespace RealEstateWebsite.Controllers;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstateWebsite.Models;
using RealEstateWebsite.Models.ViewModels;

public class ContactController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Contact(string Name, string Email, string Subject, string Message)
    {
        ViewBag.Message = "Thank you for your message!";
        return View("Index");
    }
}
