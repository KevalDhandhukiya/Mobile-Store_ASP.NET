using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce_MobileStore.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTime AddedDate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Total { get; set; }

    public DateTime Created { get; set; }

    public DateTime Modified { get; set; }

    public virtual Product? Product { get; set; } = null!;

    public virtual User? User { get; set; } = null!;
}
