
using System.Net.Configuration;
using ivNet.Listing.Entities;
using ivNet.Listing.Helpers;
using ivNet.Listing.Models;
using Newtonsoft.Json;
using NHibernate;
using Orchard;
using Orchard.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ivNet.Listing.Service
{
    public interface IOwnerServices : IDependency
    {
        int GetOwnerIdByKey(string ownerKey);
        void AddListing(int ownerId, EditListingViewModel model);
        void UpdateOwnerRegistration(RegistrationUpdateViewModel model);
    }

    public class OwnerServices : BaseService, IOwnerServices
    {
        public OwnerServices(IAuthenticationService authenticationService) 
            : base(authenticationService)
        {
        }

        public int GetOwnerIdByKey(string ownerKey)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var firstOrDefault = session.CreateCriteria(typeof (Owner))
                    .List<Owner>().FirstOrDefault(x => x.OwnerKey.Equals(ownerKey));
                if (firstOrDefault != null)
                    return firstOrDefault.Id;
            }

            return 0;
        }

        public void AddListing(int ownerId, EditListingViewModel model)
        {            
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {                   
                    // get existing entities
                    var owner = session.CreateCriteria(typeof (Owner))
                        .List<Owner>().FirstOrDefault(x => x.Id.Equals(ownerId));

                    var categoryId = model.CategoryId;
                    if (categoryId==0) int.TryParse(model.Category, out categoryId);
                    var category = session.CreateCriteria(typeof(Category))
                       .List<Category>().FirstOrDefault(x => x.Id.Equals(categoryId));

                    var packageId = model.PackageId;
                    if (packageId == 0) int.TryParse(model.Package, out packageId);
                    var package = session.CreateCriteria(typeof(PaymentPackage))
                       .List<PaymentPackage>().FirstOrDefault(x => x.Id.Equals(packageId));

                    var addressDetailKey = CustomStringHelper.BuildKey(new[] { model.Address1, model.Postcode });
                   // if (!string.IsNullOrEmpty(model.ListingKey))
                   // {
                   //     addressDetailKey = model.ListingKey;
                   // }

                    // get potentially new entities and save them
                    // address
                    var address = session.CreateCriteria(typeof(AddressDetail))
                        .List<AddressDetail>().FirstOrDefault(x => x.AddressDetailKey.Equals(addressDetailKey)) ??
                                  new AddressDetail();

                    address.Address1 = model.Address1;
                    address.Address2 = model.Address2;
                    address.Town = model.Town;
                    address.Postcode = model.Postcode.ToUpperInvariant();
                    address.IsActive = 1;
                    address.AddressDetailKey = CustomStringHelper.BuildKey(new[] { model.Address1, model.Postcode });

                    addressDetailKey = address.AddressDetailKey;

                    SetAudit(address);
                    session.SaveOrUpdate(address);

                    // contact
                    var contactDetail = session.CreateCriteria(typeof(ContactDetail))
                        .List<ContactDetail>().FirstOrDefault(x => x.ContactDetailKey.Equals(owner.OwnerKey)) ??
                                  new ContactDetail();

                    contactDetail.Website = model.WebsiteUrl;
                    contactDetail.IsActive = 1;
                    contactDetail.ContactDetailKey = owner.OwnerKey;

                    SetAudit(contactDetail);
                    session.SaveOrUpdate(contactDetail); 

                    // location
                    var location = session.CreateCriteria(typeof(Location))
                       .List<Location>().FirstOrDefault(x => 
                           x.Postcode.Equals(model.Postcode)) ??
                                 new Location();

                    // get geolocation from postcode
                    decimal lat = 0;
                    decimal lng = 0;
                    try
                    {
                        var requestUri =
                            string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false",
                                Uri.EscapeDataString(model.Postcode));
                        var request = WebRequest.Create(requestUri);
                        var response = request.GetResponse();
                        var xdoc = XDocument.Load(response.GetResponseStream());
                        decimal.TryParse(xdoc.XPathSelectElement("GeocodeResponse/result/geometry/location/lat").Value,
                            out lat);
                        decimal.TryParse(xdoc.XPathSelectElement("GeocodeResponse/result/geometry/location/lng").Value,
                            out lng);
                    }
                    catch{ }

                    location.Postcode = model.Postcode.ToUpperInvariant();
                    location.Latitude = lat;
                    location.Longitude = lng;
                    location.IsActive = 1;
                    
                    SetAudit(location);
                    session.SaveOrUpdate(location);

                    //if (!string.IsNullOrEmpty(model.ListingKey))
                    //{
                    //    addressDetailKey = model.ListingKey;
                    //}

                    // listing
                    var listing = session.CreateCriteria(typeof(ListingDetail))
                       .List<ListingDetail>().FirstOrDefault(x =>
                           x.ListingKey.Equals(addressDetailKey)) ??
                                 new ListingDetail();

                    listing.Init();                    

                    listing.ExpiraryDate = DateTime.Now;

                    listing.Description = package==null || package.Id == 1
                        ? model.Description
                        : model.DescriptionHtml;

                    listing.StrapLine = model.StrapLine;
                    listing.Price = model.Price;
                    listing.AddressDetail = address;
                    listing.Owner = owner;
                    listing.PaymentPackage = package;
                    listing.Category = category;
                    listing.Location = location;
                    listing.Notes = model.Notes;

                    listing.ListingKey = CustomStringHelper.BuildKey(new[] { model.Address1, model.Postcode });;

                    listing.Notes = model.Notes;

                    SetAudit(listing);
                    listing.IsActive = 1;
                    session.SaveOrUpdate(listing); 

                    // rooms
                    DeleteRooms(session, listing.Id);
                    if (!string.IsNullOrEmpty(model.Rooms))
                    {
                        var rooms = JsonConvert.DeserializeObject<List<RoomViewModel>>(model.Rooms);
                        foreach (var roomViewModel in rooms)
                        {
                            var room = new Room {Description = roomViewModel.Description, Type = roomViewModel.RoomType};
                            room.ListingDetail = listing;
                            SetAudit(room);
                            room.IsActive = 1;
                            session.SaveOrUpdate(room);
                            listing.Rooms.Add(room);
                        }
                    }

                    // theatres
                    DeleteTheatres(session, listing.Id);
                    if (!string.IsNullOrEmpty(model.Theatres))
                    {
                        var theatres = JsonConvert.DeserializeObject<List<TheatreViewModel>>(model.Theatres);
                        foreach (var theatreViewModel in theatres)
                        {
                            var theatre = new Theatre
                            {
                                Name = theatreViewModel.Name,
                                Town = theatreViewModel.Town,
                                Distance = theatreViewModel.Distance,
                                Transport = theatreViewModel.Transport,
                                ListingDetail = listing
                            };
                            SetAudit(theatre);
                            theatre.IsActive = 1;
                            session.SaveOrUpdate(theatre);
                            listing.Theatres.Add(theatre);
                        }
                    }

                    // images
                    DeleteImages(session, listing.Id);
                    if (!string.IsNullOrEmpty(model.Images))
                    {
                        var images = JsonConvert.DeserializeObject<List<ImageViewModel>>(model.Images);
                        foreach (var imageViewModel in images)
                        {
                            var image = new Image
                            {
                                Alt = "Owner Image",
                                ThumbUrl = imageViewModel.File,
                                LargeUrl = imageViewModel.File.Replace("thumbnails/", ""),
                                DisplayOrder = images.IndexOf(imageViewModel),
                                ListingDetail = listing
                            };
                            SetAudit(image);
                            image.IsActive = 1;
                            session.SaveOrUpdate(image);
                            listing.Images.Add(image);
                        }
                    }

                    // tags
                    DeleteTags(session, listing.Id);
                    if (!string.IsNullOrEmpty(model.Tags))
                    {
                        var taglist = model.Tags.Split(',');
                        foreach (var strTag in taglist)
                        {
                            var tag = new Tag {Name = strTag};
                            SetAudit(tag);
                            tag.ListingDetail = listing;
                            tag.IsActive = 1;
                            session.SaveOrUpdate(tag);
                            listing.Tags.Add(tag);
                        }
                    }
                    SetAudit(listing);
                    listing.IsActive = 1;
                    session.SaveOrUpdate(listing); 

                    transaction.Commit();
                }
            }                                                
        }

        public void UpdateOwnerRegistration(RegistrationUpdateViewModel model)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    // get existing entities
                    var owner = session.CreateCriteria(typeof (Owner))
                        .List<Owner>().FirstOrDefault(x => x.UserId.Equals(model.UserId));
                    if (owner != null)
                    {
                        owner.Surname = model.Surname;
                        owner.Firstname = model.Firstname;

                        var address = owner.AddressDetail;
                        address.Address1 = model.Address1;
                        address.Address2 = model.Address2;
                        address.Postcode = model.Postcode;
                        address.Town = model.Town;
                      //  address.AddressDetailKey = CustomStringHelper.BuildKey(new[] { model.Address1, model.Postcode });

                        var contact = owner.ContactDetail;
                        contact.Phone = model.Phone;
                        contact.Website = model.Website;

                        SetAudit(address);
                        session.SaveOrUpdate(address);

                        SetAudit(contact);
                        session.SaveOrUpdate(contact);

                        SetAudit(owner);
                        session.SaveOrUpdate(owner);

                        transaction.Commit();
                    }
                }
            }
        }

        private void DeleteTags(ISession session, int id)
        {
            var tagList = session.CreateCriteria(typeof(Tag))
                    .List<Tag>().Where(x => x.ListingDetail.Id.Equals(id));

            foreach (var tag in tagList)
            {
                session.Delete(tag);
            }
        }

        private void DeleteImages(ISession session, int id)
        {
            var imageList = session.CreateCriteria(typeof(Image))
                    .List<Image>().Where(x => x.ListingDetail.Id.Equals(id));

            foreach (var image in imageList)
            {
                session.Delete(image);
            }
        }

        private void DeleteTheatres(ISession session, int id)
        {
            var theatreList = session.CreateCriteria(typeof(Theatre))
                 .List<Theatre>().Where(x => x.ListingDetail.Id.Equals(id));

            foreach (var theatre in theatreList)
            {
                session.Delete(theatre);
            } 
        }

        private void DeleteRooms(ISession session, int id)
        {
            var roomList = session.CreateCriteria(typeof(Room))
                    .List<Room>().Where(x => x.ListingDetail.Id.Equals(id));

            foreach (var room in roomList)
            {
                session.Delete(room);
            }
        }
    }
}