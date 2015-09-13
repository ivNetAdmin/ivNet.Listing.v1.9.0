
using ivNet.Listing.Service;
using Orchard.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ivNet.Listing.Controllers.api
{
    public class SearchController : ApiController
    {
        private readonly IListingServices _listingServices;

        public SearchController(IListingServices listingServices)
        {
            _listingServices = listingServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public HttpResponseMessage Get(string id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _listingServices.Search(id));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}