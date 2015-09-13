
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Formatting;
using ivNet.Listing.Service;
using Orchard;
using Orchard.Logging;

namespace ivNet.Listing.Controllers.api
{
    public class AdminListingController : ApiController
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IListingServices _listingServices;

        public AdminListingController(
            IOrchardServices orchardServices,
            IListingServices listingServices
            )
        {
            _orchardServices = orchardServices;
            _listingServices = listingServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public HttpResponseMessage Get(string id)
        {

            if (!_orchardServices.Authorizer.Authorize(Permissions.ivAdminTab))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            if (id == "package")
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                    _listingServices.GetPackageListings());
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK,
                   _listingServices.GetAdminListings());
            }

        }

        [HttpPut]
        public HttpResponseMessage Put(int id, dynamic item)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivAdminTab))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            try
            {
                switch ((string)item.Action)
                {
                    case "authorise":
                        _listingServices.AuthoriseListing(id);
                        break;
                    case "cancel":
                        _listingServices.CancelListing(id);
                        break;
                    case "delete":
                        _listingServices.DeleteListing(id);
                        break;
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}