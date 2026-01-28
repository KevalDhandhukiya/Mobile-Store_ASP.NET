using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace E_commerce_Mobile.Models
{

    // View Model for MVC views
    public class UserModel
    {
        [JsonProperty("userId")]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        [JsonProperty("userName")]
        public string UserName { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [JsonProperty("password")]
        public string Password { get; set; } = null!;

        [StringLength(50)]
        [JsonProperty("contact")]
        public string? Contact { get; set; }

        [EmailAddress]
        [StringLength(100)]
        [JsonProperty("email")]
        public string? Email { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; } = true;
        
        [JsonProperty("isAdmin")]
        public bool IsAdmin { get; set; } = false;
        
        [JsonProperty("created")]
        public DateTime Created { get; set; }
        
        [JsonProperty("modified")]
        public DateTime Modified { get; set; }
    }
}
