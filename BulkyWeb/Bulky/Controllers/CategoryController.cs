using Bulky.Data;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bulky.Controllers;

public class CategoryController : Controller
{
    private readonly ApplicationDbContext _db;
    public CategoryController(ApplicationDbContext db)
    {
        _db = db;
    }
    public IActionResult Index()
    {
        var categories = _db.Categories.ToList();
        return View(categories);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Category category)
    {
        if (category.Name == category.DisplayOrder.ToString())
        {
            ModelState.AddModelError("Name", "Display Order cannot match the name.");
        }

        if (string.IsNullOrWhiteSpace(category.DisplayOrder.ToString()))
        {
            ModelState.AddModelError("DisplayOrder", "Value cannot be empty.");
        }
        
        if (!ModelState.IsValid) return View();
        
        _db.Categories.Add(category);   
        _db.SaveChanges();

        return RedirectToAction("Index");
    }
}