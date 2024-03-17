using System.Collections;
using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BookStore.Areas.Admin.Controllers;

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
        return View(_unitOfWork.Product.GetAll(includeProperties:"Category").ToList());
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
        if (vm.Product.Id == 0 && _unitOfWork.Product.Get(e => e.ISBN == vm.Product.ISBN) != null)
            ModelState.AddModelError("Product.ISBN", "Product cannot contain the same ISBN!");
        
        if (vm.Product.Id == 0 && _unitOfWork.Product.Get(e => e.Title == vm.Product.Title) != null)
            ModelState.AddModelError("Product.Title", "Product cannot have the same Title!");
        
        if (vm.Product.Id == 0 && file == null)
            ModelState.AddModelError("Product.ImageUrl", "You have to upload a file!");

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

        if (file != null)
        {
            var wwwRootPath = _webHostEnvironment.WebRootPath;
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file?.FileName);
            var path = Path.Combine(wwwRootPath, @"images\product");

            // delete the old image if it exists (if we are updating)
            if (!string.IsNullOrEmpty(vm.Product.ImageUrl))
            {
                var oldPath = Path.Combine(wwwRootPath, vm.Product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }
            
            using (var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
            {
                file?.CopyTo(fileStream);
            }

            vm.Product.ImageUrl = @"\images\product\" + fileName;
        }

        if (vm.Product.Id == 0)
        {
            _unitOfWork.Product.Add(vm.Product);
            TempData["success"] = "Product added!";
        }
        else
        {
            _unitOfWork.Product.Update(vm.Product);
            TempData["success"] = "Product updated!";
        }
        _unitOfWork.Save();

        return RedirectToAction("Index");
    }
    
    public IActionResult Delete(Product product)
    {
        var selectedProduct = _unitOfWork.Product.Get(e => e.Id == product.Id);
        if (selectedProduct == null) return RedirectToAction("Index");
        
        var path = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, selectedProduct.ImageUrl.TrimStart('\\'));
        
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }
        
        _unitOfWork.Product.Remove(selectedProduct);
        _unitOfWork.Save();
        
        TempData["success"] = "Product deleted.";

        return RedirectToAction("Index");
    }

    #region API CALLS

    [HttpGet]
    public IActionResult GetAll()
    {
        return Json(new { data = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList() });
    }

    #endregion
}