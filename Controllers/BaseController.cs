
using System.Web.Mvc;
using System.Web.Routing;

namespace ivNet.Listing.Controllers
{
    public class BaseController : Controller
    {
        protected string ActionName;
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            ActionName = requestContext.RouteData.Values["action"].ToString();
        }
    }
}