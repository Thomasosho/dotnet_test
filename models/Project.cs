using System;
using System.Text.Json.Serialization;

namespace VendorBiddingApp.Models
{
    public class Project
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
    }

    public class ProjectDto
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
    }
}
