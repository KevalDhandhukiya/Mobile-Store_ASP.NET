using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace E_commerce_MobileStore.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Contact { get; set; }

    public string? Email { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsAdmin { get; set; } = false;

    public DateTime Created { get; set; }

    public DateTime Modified { get; set; }

    [JsonIgnore]
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
    
    [JsonIgnore]
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    
    [JsonIgnore]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    
    [JsonIgnore]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
