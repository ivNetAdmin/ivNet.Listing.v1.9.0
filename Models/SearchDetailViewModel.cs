
using System.Collections.Generic;

namespace ivNet.Listing.Models
{
    public class SearchDetailViewModel
    {
        public int ListingId { get; set; }
        public string StrapLine { get; set; }
        public string Owner { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string ImageUrl { get; set; }
        public IList<string> Tags { get; set; }
    }
}