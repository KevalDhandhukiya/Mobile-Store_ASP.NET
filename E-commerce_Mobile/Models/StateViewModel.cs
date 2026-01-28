namespace E_commerce_Mobile.Models
{
    public class StateViewModel
    {
        public int StateId { get; set; }
        public string StateName { get; set; } = string.Empty;
        public string? StateCode { get; set; }
        public int CountryId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public IFormFile? File { get; set; }
        public List<CountryViewModel>? CountryList { get; set; }
    }

    public class CountryViewModel
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;
    }
}
