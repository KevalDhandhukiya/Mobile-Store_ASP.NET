namespace E_commerce_Mobile.Models
{
    public class ProductModel
    {
        public int ProductId { get; set; }
        public string ModelName { get; set; }
        public string CompanyName { get; set; }
        public string? Description { get; set; }
        public DateTime LaunchDate { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public int Stock { get; set; }
        public int SpecificationId { get; set; }
        public int UserId { get; set; }
        public bool IsRemoved { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        // Navigation properties
        public CategoryModel? Category { get; set; }

        // Dropdown helpers
        public List<CategoryDropdownModel>? CategoryList { get; set; }
    }
}