using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace E_commerce_MobileStore.Models
{
    public class State
    {
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string StateName { get; set; } = null!;
        public string? StateCode { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; }
        [JsonIgnore]
        public DateTime ModifiedDate { get; set; }
        [JsonIgnore]
        public string? FilePath { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
        [JsonIgnore]
        public virtual ICollection<City> Cities { get; set; } = new List<City>();
        [JsonIgnore]
        public virtual Country? Country { get; set; } = null!;
    }
}
