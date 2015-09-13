
using System.Collections.Generic;

namespace ivNet.Listing.Models
{
    public class EditListingViewModel
    {
        public int Id { get; set; }
        public int PackageId { get; set; }
        public int CategoryId { get; set; }

        public string ListingKey { get; set; }

        public string StrapLine { get; set; }
        public string Price { get; set; }
        public virtual string OwnerName { get; set; }

        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual string WebsiteUrl { get; set; }

        public string Description { get; set; }
        public string DescriptionHtml { get; set; }        

        public string Package { get; set; }        
        public string Category { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }

        public string Rooms { get; set; }
        public string Theatres { get; set; }
        public string Tags { get; set; }
        public string Images { get; set; }

        public string Notes { get; set; }
        public string SearchTerm { get; set; }

        public virtual decimal Longitude { get; set; }
        public virtual decimal Latitude { get; set; }

    }
}