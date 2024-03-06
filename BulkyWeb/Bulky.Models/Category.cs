using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bulky.Models;

public class Category
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "This field is required!")]
    [MaxLength(30, ErrorMessage = "Value must be inside the range 1-30")]
    [DisplayName("Category Name")]
    public string Name { get; set; }
    
    [DisplayName("Display Order")]
    [Range(1, 100, ErrorMessage = "Value must be inside the range 1-100")]
    public int? DisplayOrder { get; set; }
}