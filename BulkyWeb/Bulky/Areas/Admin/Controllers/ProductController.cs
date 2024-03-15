using System.Collections;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Bulky.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        return View(_unitOfWork.Product.GetAll().ToList());
    }

    public IActionResult Upsert(string? title)
    {
        return View(new ProductViewModel
        {
            Product = _unitOfWork.Product.Get(e => e.Title == title) ?? new Product(),
            CategoryList = _unitOfWork.Category.GetAll()
                .Select(e => new SelectListItem
                {
                    Text = e.Name,
                    Value = e.Id.ToString()
                })
        });
    }

    [HttpPost]
    public IActionResult Upsert(ProductViewModel vm, IFormFile? file)
    {
        if (_unitOfWork.Product.Get(e => e.ISBN == vm.Product.ISBN) != null)
            ModelState.AddModelError("ISBN", "Product cannot contain the same ISBN!");
        
        if (_unitOfWork.Product.Get(e => e.Title == vm.Product.Title) != null)
            ModelState.AddModelError("Title", "Product cannot have the same Title!");
        
        if (file == null)
            ModelState.AddModelError("ImageUrl", "You have to upload a file!");

        if (!ModelState.IsValid)
        {
            return View(new ProductViewModel
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll()
                    .Select(e => new SelectListItem
                    {
                        Text = e.Name,
                        Value = e.Id.ToString()
                    })
            });
        }

        var wwwRootPath = _webHostEnvironment.WebRootPath;
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file?.FileName);
        var path = Path.Combine(wwwRootPath, @"images\product");
        
        using (var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
        {
            file?.CopyTo(fileStream);
        }

        vm.Product.ImageUrl = @"\images\product\" + fileName;
        
        _unitOfWork.Product.Add(vm.Product);
        _unitOfWork.Save();
        TempData["success"] = "Product added!";

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