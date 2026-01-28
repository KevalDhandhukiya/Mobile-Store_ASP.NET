using System;
using System.Text.Json.Serialization;

namespace E_commerce_MobileStore.Models
{
    public class City
    {
        public int CityId { get; set; }
        public int StateId { get; set; }
        public string CityName { get; set; } = null!;
        public string? CityCode { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        [JsonIgnore]
        public virtual State? State { get; set; } = null!;
    }
}
