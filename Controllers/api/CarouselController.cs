
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Listing.Service;
using Orchard.Logging;

namespace ivNet.Listing.Controllers.api
{
    public class CarouselController : ApiController
    {
        private readonly IListingServices _listingServices;

        public CarouselController(IListingServices listingServices)
        {
            _listingServices = listingServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK,
                _listingServices.GetCarouselImages());
        }
    }
}