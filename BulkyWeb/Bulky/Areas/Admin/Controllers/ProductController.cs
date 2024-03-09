using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Bulky.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        return View(_unitOfWork.Product.GetAll().ToList());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Product product)
    {
        if (_unitOfWork.Product.Get(e => e.ISBN == product.ISBN) != null)
            ModelState.AddModelError("ISBN", "Product cannot contain the same ISBN!");

        //if (!ModelState.IsValid) return View();
        
        _unitOfWork.Product.Add(product);
        _unitOfWork.Save();
        TempData["success"] = "Product added!";
        
        return RedirectToAction("Index");
    }

    public IActionResult Edit(string title)
    {
        return View(_unitOfWork.Product.Get(e => e.Title == title));
    }

    [HttpPost]
    public IActionResult Edit(Product product)
    {
        //if (!ModelState.IsValid) return View();
        
        _unitOfWork.Product.Update(product);
        _unitOfWork.Save();
        TempData["success"] = "Product edited.";
        
        return RedirectToAction("Index");
    }
    
    public IActionResult Delete(Product product)
    {
        _unitOfWork.Product.Remove(product);
        _unitOfWork.Save();
        TempData["success"] = "Product deleted.";
        
        return RedirectToAction("Index");
    }
}