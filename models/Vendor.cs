using System;
using System.Text.Json.Serialization;

namespace VendorBiddingApp.Models
{
    public class Vendor
    {
        [JsonIgnore]
        public required Guid Id { get; set; }

        public required string Name { get; set; }

        public required string Email { get; set; }

        [JsonIgnore]
        public required string PasswordHash { get; set; }
    }

    public class VendorLoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class VendorRegistrationDto
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
