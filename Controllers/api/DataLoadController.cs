
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Xml;
using ivNet.Listing.Entities;
using ivNet.Listing.Helpers;
using ivNet.Listing.Models;
using ivNet.Listing.Service;

namespace ivNet.Listing.Controllers.api
{
    public class DataLoadController : ApiController
    {
        private readonly IConfigurationServices _configurationServices;
        private readonly IRegistrationServices _registrationServices;
        private readonly IOwnerServices _ownerServices;

        public DataLoadController(
            IConfigurationServices configurationServices,
            IRegistrationServices registrationServices,
            IOwnerServices ownerServices)
        {
            _configurationServices = configurationServices;
            _registrationServices = registrationServices;
            _ownerServices = ownerServices;
        }

        public HttpResponseMessage Get()
        {
            var errorList = new List<string>();

            var siteDataXml = new XmlDocument();

            siteDataXml.Load(HttpContext.Current.Server.MapPath("~/Modules/ivNet.Listing/App_Data/SiteData.xml"));

            //siteDataXml.Load(HttpContext.Current.Server.MapPath("~/Modules/ivNet.Listing/App_Data/TestData.xml"));

            if (siteDataXml.DocumentElement != null)
            {
                foreach (XmlNode listing in siteDataXml.DocumentElement.SelectNodes("listing"))
                {
                    // create category
                    var categoryId = 0;
                    var categoryName = listing.SelectSingleNode("category") == null
                        ? "unknown"
                        : listing.SelectSingleNode("category").InnerText;
                    try
                    {
                        categoryId = _configurationServices.CreateCategory(categoryName).Id;
                    }
                    catch (Exception ex)
                    {
                        errorList.Add(getErrorMessage("category", categoryName, ex));
                    }

                    var name = listing.SelectSingleNode("key[@name]") == null
                        ? string.Empty
                        : listing.SelectSingleNode("key[@name]").InnerText;

                    var nameParts = name.Split(' ');

                    var pw = CustomStringHelper.GenerateInitialPassword(new Owner {Firstname = nameParts[0]});

                    // create owner
                    var registrationModel = new RegistrationViewModel
                    {
                        Firstname = nameParts[0],

                        Surname = nameParts.Length == 1
                            ? "unknown"
                            : nameParts[1],

                        Email = listing.SelectSingleNode("post_content") == null
                            ? "unknown@showdigs.co.uk"
                            : listing.SelectSingleNode("email").InnerXml,

                        Address1 = listing.SelectSingleNode("map_location") == null
                            ? string.Empty
                            : listing.SelectSingleNode("map_location").InnerText,

                        Town = listing.SelectSingleNode("key[@town]") == null
                            ? string.Empty
                            : listing.SelectSingleNode("key[@town]").InnerText,

                        Postcode = listing.SelectSingleNode("key[@postcode]") == null
                            ? string.Empty
                            : listing.SelectSingleNode("key[@postcode]").InnerText,

                        Phone = listing.SelectSingleNode("key[@telephone]") == null
                            ? string.Empty
                            : listing.SelectSingleNode("key[@telephone]").InnerText,

                        Website = listing.SelectSingleNode("key[@url]") == null
                            ? string.Empty
                            : listing.SelectSingleNode("key[@url]").InnerText,

                        Password = pw,
                        ConfirmPassword = pw
                    };

                    // try and sort out address
                    var addressParts = registrationModel.Address1.Split(',');
                    if (addressParts.Length > 1)
                    {
                        registrationModel.Address1 = addressParts[0];
                        for (var i = 1; i < addressParts.Length; i++)
                        {
                            registrationModel.Address2 = string.Format("{0} {1}", registrationModel.Address2,
                                addressParts[i]).Trim();
                        }
                    }

                    try
                    {
                        _registrationServices.UpdateOwner(registrationModel);
                        var user = _registrationServices.CreateOwnerUser(
                            new ActivationViewModel
                            {
                                ConfirmPassword = registrationModel.ConfirmPassword,
                                Email = registrationModel.Email,
                                Password = registrationModel.Password,
                                Message = ""
                            });
                        _registrationServices.UpdateOwnerUserId(user.Email, user.Id);
                    }
                    catch (Exception ex)
                    {
                        errorList.Add(getErrorMessage("owner", registrationModel.Email, ex));
                    }

                    // create listing
                    var description = listing.SelectSingleNode("post_content") == null
                        ? string.Empty
                        : listing.SelectSingleNode("post_content").InnerXml;

                    var strapLine = listing.SelectSingleNode("post_content") == null
                        ? string.Empty
                        : listing.SelectSingleNode("post_title").InnerXml;

                    // mop up some data just for admin view
                    var notes = string.Format("[Featured]:{0} - [Hits]:{1} - [Key 3]:{2} - [Key 4]:{3} - [Key 5]:{4} - [Package Access]:{5} - [Tagline]{6}",
                       listing.SelectSingleNode("featured") == null
                        ? string.Empty
                        : listing.SelectSingleNode("featured").InnerXml,
                        listing.SelectSingleNode("hits") == null
                        ? string.Empty
                        : listing.SelectSingleNode("hits").InnerXml,
                         listing.SelectSingleNode("key_3") == null
                        ? string.Empty
                        : listing.SelectSingleNode("key_3").InnerXml,
                         listing.SelectSingleNode("key_4") == null
                        ? string.Empty
                        : listing.SelectSingleNode("key_4").InnerXml,
                         listing.SelectSingleNode("key_5") == null
                        ? string.Empty
                        : listing.SelectSingleNode("key_5").InnerXml,
                         listing.SelectSingleNode("package_access") == null
                        ? string.Empty
                        : listing.SelectSingleNode("package_access").InnerXml,
                        listing.SelectSingleNode("tagline") == null
                        ? string.Empty
                        : listing.SelectSingleNode("tagline").InnerXml
                        
                        );

                    var listingModel = new EditListingViewModel
                    {
                        StrapLine = strapLine,
                        
                        Price ="",

                        Description = description,

                        DescriptionHtml = description,

                        Address1 = registrationModel.Address1,

                        Address2 = registrationModel.Address2,

                        Town = registrationModel.Town,

                        CategoryId = categoryId,

                        Category = listing.SelectSingleNode("category") == null
                            ? string.Empty
                            : listing.SelectSingleNode("category").InnerText,

                        PackageId = listing.SelectSingleNode("packageid") == null
                            ? 0
                            : Convert.ToInt32(listing.SelectSingleNode("packageid").InnerText),

                        Postcode = listing.SelectSingleNode("key[@postcode]") == null
                            ? string.Empty
                            : listing.SelectSingleNode("key[@postcode]").InnerText,

                        Notes = notes

                    };

                    try
                    {
                        var ownerKey = CustomStringHelper.BuildKey(new[] {registrationModel.Email});
                        var ownerId = _ownerServices.GetOwnerIdByKey(ownerKey);
                        _ownerServices.AddListing(ownerId, listingModel);
                    }
                    catch (Exception ex)
                    {
                        errorList.Add(getErrorMessage("listing", listingModel.Address1, ex));
                    }

                }
            }

            return Request.CreateResponse(HttpStatusCode.OK,
                errorList);
        }

        private string getErrorMessage(string type, string name, Exception ex)
        {
            var innerErrorMessage = ex.Message;

            while (ex.InnerException != null)
            {
                innerErrorMessage = string.Format("{0}, {1}", innerErrorMessage, ex.InnerException.Message);
                ex = ex.InnerException;
            }

            return string.Format("{0} - {1} : {2}", type, name, innerErrorMessage);
        }
    }
}