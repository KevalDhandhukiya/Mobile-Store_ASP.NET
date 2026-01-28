using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace E_commerce_MobileStore.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ModelName { get; set; } = null!;

    public string CompanyName { get; set; } = null!;

    public DateTime LaunchDate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    public int CategoryId { get; set; }

    public int Stock { get; set; }

    public bool IsRemoved { get; set; } = false;

    public string? Image1 { get; set; }
    public string? Image2 { get; set; }
    public string? Image3 { get; set; }
    public string? Image4 { get; set; }

    public int SpecificationId { get; set; }

    public int UserId { get; set; }

    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }

    [JsonIgnore]  // ✅ Ignore in API binding
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    [JsonIgnore]  // ✅ Ignore navigation properties during API binding
    public virtual Category? Category { get; set; } = null!;

    [JsonIgnore]  // ✅ THIS FIXES YOUR ERROR
    public virtual User User { get; set; } = null!;
}
