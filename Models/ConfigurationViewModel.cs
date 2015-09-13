
using System.Collections.Generic;

namespace ivNet.Listing.Models
{
    public class ConfigurationViewModel
    {
        public IEnumerable<ListingCategoryViewModel> Categories { get; set; }
        public IEnumerable<ListingTransportViewModel> TransportList { get; set; }
        public IEnumerable<ListingRoomTypeViewModel> RoomTypes { get; set; }
        public IEnumerable<ListingTagTextViewModel> TagTextList { get; set; }
    }
}