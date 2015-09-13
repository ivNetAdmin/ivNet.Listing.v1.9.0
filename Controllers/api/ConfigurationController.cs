
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Listing.Service;
using Orchard;
using Orchard.Logging;

namespace ivNet.Listing.Controllers.api
{
    public class ConfigurationController : ApiController
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IConfigurationServices _configurationServices;

        public ConfigurationController(
            IOrchardServices orchardServices,
            IConfigurationServices configurationServices
            )
        {
            _orchardServices = orchardServices;
            _configurationServices = configurationServices;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        [HttpPost]
        public HttpResponseMessage Post(dynamic item)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivAdminTab))
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            object rtnItem = null;
            try
            {
                switch ((string) item.Type)
                {
                    case "category":
                        rtnItem = _configurationServices.CreateCategory((string)item.Name);
                        break;
                    case "transport":
                        rtnItem = _configurationServices.CreateTransport((string)item.Name);
                        break;
                    case "roomType":
                        rtnItem = _configurationServices.CreateRoomType((string)item.Name);
                        break;
                    case "tag":
                        rtnItem = _configurationServices.CreateTag((string)item.Name);
                        break;
                }

                return Request.CreateResponse(HttpStatusCode.OK, rtnItem);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpGet]
        public HttpResponseMessage Get()
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivOwnerTab)
                && !_orchardServices.Authorizer.Authorize(Permissions.ivAdminTab))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            return Request.CreateResponse(HttpStatusCode.OK,
                _configurationServices.GetConfiguration());
        }

        [HttpDelete]
        public HttpResponseMessage Delete(int id, string type)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivAdminTab))
                return Request.CreateResponse(HttpStatusCode.Forbidden);

            try
            {
                _configurationServices.Delete(id, type);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError,ex.Message);
            }
        }
    }
}