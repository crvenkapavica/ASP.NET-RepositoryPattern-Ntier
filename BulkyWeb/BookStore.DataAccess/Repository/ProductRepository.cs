using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;

namespace BookStore.DataAccess.Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly ApplicationDbContext _db;
    
    public ProductRepository(ApplicationDbContext db) 
        : base(db)
    {
        _db = db;
    }

    public void Update(Product product)
    {
        var selectedProduct = _db.Products.FirstOrDefault(e => e.Id == product.Id);
        if (selectedProduct == null) return;

        selectedProduct.CategoryId = product.CategoryId;
        selectedProduct.Title = product.Title;
        selectedProduct.Author = product.Author;
        selectedProduct.Description = product.Description;
        selectedProduct.ISBN = product.ISBN;
        selectedProduct.ListPrice = product.ListPrice;
        selectedProduct.Price = product.Price;
        selectedProduct.Price50 = product.Price50;
        selectedProduct.Price100 = product.Price100;
        if (product.ImageUrl != null)
        {
            selectedProduct.ImageUrl = product.ImageUrl;
        }
    }
}