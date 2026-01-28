using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace E_commerce_MobileStore.Models
{
    public class Country
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; } = null!;
        public string? CountryCode { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        [JsonIgnore]
        public virtual ICollection<State> States { get; set; } = new List<State>();
    }
}
