using System.ComponentModel.DataAnnotations;

namespace E_commerce_Mobile.Models
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public int UserId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public virtual UserModel? User { get; set; } = null;
    }

    public class CategoryDropdownModel
    {
        public int CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; } = null!;
    }
}
