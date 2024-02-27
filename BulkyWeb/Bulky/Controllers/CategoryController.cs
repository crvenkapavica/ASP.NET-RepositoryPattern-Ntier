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
        if (string.IsNullOrEmpty(category.DisplayOrder?.ToString()))
        {
            ModelState.AddModelError("", "Display Order is required!");
        }
        
        if (category.Name == category.DisplayOrder.ToString())
        {
            ModelState.AddModelError("Name", "Display Order cannot match the name.");
        }

        if (string.IsNullOrEmpty(category.DisplayOrder?.ToString()))
        {
            ModelState.AddModelError("DisplayOrder", "Value cannot be empty.");
        }
        
        if (!ModelState.IsValid) return View();
        
        _db.Categories.Add(category);   
        _db.SaveChanges();
        TempData["success"] = "Category created successfully!";
        return RedirectToAction("Index");
    }
    
    public IActionResult Edit(string name)
    {
        return View(_db.Categories.FirstOrDefault(c => c.Name == name) ?? throw new InvalidOperationException("Category ID not found!"));
    }

    [HttpPost]
    public IActionResult Edit(Category category)
    {
        if (!ModelState.IsValid) return View();
        
        _db.Categories.Update(category);   
        _db.SaveChanges();
        TempData["success"] = "Category updated!";
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id) 
    {
        _db.Categories.Remove(_db.Categories.Find(id) ?? throw new InvalidOperationException("Category Not Found!"));
        _db.SaveChanges();
        TempData["success"] = "Category deleted!";
        return RedirectToAction("Index");
    }
}

