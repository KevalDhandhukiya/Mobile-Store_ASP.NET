using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace E_commerce_MobileStore.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime Created { get; set; }

    public DateTime Modified { get; set; }

    public virtual User? User { get; set; } = null!;
}
