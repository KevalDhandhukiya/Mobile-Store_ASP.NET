namespace E_commerce_Mobile.Models
{
    public class StateModel
    {
        public int StateId { get; set; }
        public string StateName { get; set; } = string.Empty;
        public string? StateCode { get; set; }
        public int CountryId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
