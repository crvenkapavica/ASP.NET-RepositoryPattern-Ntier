using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    public CategoryController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public IActionResult Index()
    {
        return View(_unitOfWork.Category.GetAll().ToList());
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
        
        _unitOfWork.Category.Add(category);   
        _unitOfWork.Save();
        TempData["success"] = "Category created!";
        
        return RedirectToAction("Index");
    }
    
    public IActionResult Edit(string name)
    {
        return View(_unitOfWork.Category.Get(c => c.Name == name) 
                    ?? throw new InvalidOperationException("Category ID not found!"));
    }

    [HttpPost]
    public IActionResult Edit(Category category)
    {
        if (!ModelState.IsValid) return View();
        
        _unitOfWork.Category.Update(category);
        _unitOfWork.Save();
        TempData["success"] = "Category updated.";
        
        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id) 
    {
        _unitOfWork.Category.Remove(_unitOfWork.Category.Get(c => c.Id == id) 
                             ?? throw new InvalidOperationException("Category Not Found!"));
        _unitOfWork.Save();
        TempData["success"] = "Category deleted.";
        
        return RedirectToAction("Index");
    }
}

