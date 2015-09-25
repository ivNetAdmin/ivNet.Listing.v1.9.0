using System.Globalization;
using ivNet.Listing.Helpers;
using ivNet.Listing.Models;
using ivNet.Listing.Service;
using ivNet.Mail.Services;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Search.ViewModels;
using Orchard.Security;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Orchard.Themes;

namespace ivNet.Listing.Controllers
{
    public class SecureSiteController : BaseController
    {
        private readonly IOrchardServices _orchardServices;
        //private readonly IAuthenticationService _authenticationService;
        private readonly IOwnerServices _ownerServices;
        private readonly IEmailServices _emailServices;
        
        public SecureSiteController(
            IOrchardServices orchardServices, 
            IAuthenticationService authenticationService,
            IOwnerServices ownerServices,
            IEmailServices emailServices)
        {
            _orchardServices = orchardServices;
            _ownerServices = ownerServices;
            _emailServices = emailServices;
            //_authenticationService = authenticationService;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            CurrentUser = authenticationService.GetAuthenticatedUser();
        }

        private IUser CurrentUser { get; set; }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }


        [HttpPost]
        [Themed]
        public ActionResult RegistrationUpdate(RegistrationUpdateViewModel model)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivOwnerTab, T("You are not authorized")))
                Response.Redirect("/Users/Account/AccessDenied?ReturnUrl=/");

            if (!ModelState.IsValid) return View("Site/Registration/Update/Index", model);

            _ownerServices.UpdateOwnerRegistration(model);

            const string returnUrl = "/owner/my-listings";

            return Redirect(returnUrl);
        }

        [HttpPost]
        public ActionResult AddListing(EditListingViewModel model)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivOwnerTab, T("You are not authorized")))
                Response.Redirect("/Users/Account/AccessDenied?ReturnUrl=/");

            var ownerKey = CustomStringHelper.BuildKey(new[] { CurrentUser.Email });
            if(_orchardServices.Authorizer.Authorize(Permissions.ivAdminTab))
            {
                ownerKey = CustomStringHelper.BuildKey(new[] { model.Email });
            }

            var ownerId = _ownerServices.GetOwnerIdByKey(ownerKey);
       
            _ownerServices.AddListing(ownerId, model);
            var package = "Free";
            switch (model.Package)
            {
                case "2":
                    package = "Full";
                    break;
                case "3":
                    package = "Featured";
                    break;
            }

            _emailServices.SendNotificationEmail(package, CurrentUser.Email);

            string returnUrl = "/owner/my-listings";
            if (_orchardServices.Authorizer.Authorize(Permissions.ivAdminTab))
            {
                returnUrl = "/admin/listings";
            }
            
            return Redirect(returnUrl);
        }

        [HttpPost]
        public ContentResult UploadFiles(string ownerEmail)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivOwnerTab, T("You are not authorized")))
                Response.Redirect("/Users/Account/AccessDenied?ReturnUrl=/");
            
            var r = new List<UploadFilesResult>();

            var ownerKey = CustomStringHelper.BuildKey(new[] { CurrentUser.Email });
            if (_orchardServices.Authorizer.Authorize(Permissions.ivAdminTab))
            {
                ownerKey = CustomStringHelper.BuildKey(new[] { System.Web.HttpContext.Current.Session["OwnerEmail"].ToString() });
            }

            var ownerId = _ownerServices.GetOwnerIdByKey(ownerKey);
            var filePath = Server.MapPath(string.Format("~/Media/Default/ListingImages/{0}", ownerId));                  

            foreach (string file in Request.Files)
            {
                var hpf = Request.Files[file] as HttpPostedFileBase;

                if (hpf == null || hpf.ContentLength == 0)
                    continue;

                var now = DateTime.Now;
                var fileName = string.Format("{0}{1}{2}{3}{4}{5}.jpg",
                now.Year,
                now.Month.ToString(CultureInfo.InvariantCulture).Length==1?string.Format("0{0}",now.Month.ToString(CultureInfo.InvariantCulture)):now.Month.ToString(CultureInfo.InvariantCulture),
                now.Day.ToString(CultureInfo.InvariantCulture).Length == 1 ? string.Format("0{0}", now.Day.ToString(CultureInfo.InvariantCulture)) : now.Day.ToString(CultureInfo.InvariantCulture),
                now.Hour.ToString(CultureInfo.InvariantCulture).Length == 1 ? string.Format("0{0}", now.Hour.ToString(CultureInfo.InvariantCulture)) : now.Hour.ToString(CultureInfo.InvariantCulture),
                now.Minute.ToString(CultureInfo.InvariantCulture).Length == 1 ? string.Format("0{0}", now.Minute.ToString(CultureInfo.InvariantCulture)) : now.Minute.ToString(CultureInfo.InvariantCulture),
                now.Second.ToString(CultureInfo.InvariantCulture).Length == 1 ? string.Format("0{0}", now.Second.ToString(CultureInfo.InvariantCulture)) : now.Second.ToString(CultureInfo.InvariantCulture)
                );

                ImageHelper.UploadImage(hpf, fileName, filePath, 560, 303);
                ImageHelper.UploadImage(hpf, fileName, filePath + "/thumbnails", 60, 55);

                //var fileName = ImageHelper.UploadOwnerImage(hpf, filePath);
                //ImageHelper.UploadImageThumbnail(hpf, fileName, filePath);

                r.Add(new UploadFilesResult()
                {
                    Name = string.Format("/Media/Default/ListingImages/{0}/thumbnails/{1}",ownerId,fileName),
                    Length = hpf.ContentLength,
                    Type = hpf.ContentType
                });
            }

            // Returns json
            return Content("{\"name\":\"" + r[0].Name + "\",\"type\":\"" + r[0].Type + "\",\"size\":\"" + string.Format("{0} bytes", r[0].Length) + "\"}", "application/json");
        }
    }
}