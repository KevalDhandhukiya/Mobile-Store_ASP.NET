namespace E_commerce_Mobile.Models
{
    public class OrderModel
    {
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public virtual UserModel? User { get; set; } = null;
    }
}
