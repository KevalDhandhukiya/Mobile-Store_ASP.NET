namespace E_commerce_Mobile.Models
{
    public class CartModel
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedDate { get; set; }
        public decimal Total { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public virtual ProductModel? Product { get; set; } = null;
        public virtual UserModel? User { get; set; } = null;
    }
}
