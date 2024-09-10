using System;
using System.Text.Json.Serialization;

namespace VendorBiddingApp.Models
{
    public class Bid
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public required string VendorId { get; set; }
        public required string ProjectId { get; set; }
        public required decimal Amount { get; set; }
        public string Status { get; set; } = "Pending";
    }

    public class BidDto
    {
        public required string VendorId { get; set; }
        public required string ProjectId { get; set; }
        public required decimal Amount { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
