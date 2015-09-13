using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ivNet.Listing.Entities;
using ivNet.Listing.Helpers;
using ivNet.Listing.Models;
using Newtonsoft.Json;
using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Mapping;
using NHibernate.Util;
using Orchard;
using Orchard.Security;

namespace ivNet.Listing.Service
{
    public interface IListingServices : IDependency
    {
        IEnumerable<ListingDetailViewModel> GetListings(string eMail);
        IEnumerable<ListingDetailViewModel> GetAdminListings();
        IEnumerable<ListingDetailViewModel> GetPackageListings();
        
        IEnumerable<ListingCategoryViewModel> GetListingCategories();
        IEnumerable<ListingPackageViewModel> GetListingPackages();
        EditListingViewModel GetListing(int id);

        void AuthoriseListing(int id);
        void CancelListing(int id);
        void DeleteListing(int id);

        List<CarouselImageViewModel> GetCarouselImages();
        IEnumerable<CarouselImageViewModel> GetFeaturedListings(); 
        IEnumerable<SearchDetailViewModel> Search(string searchTerm);  
    }

    public class ListingServices : BaseService, IListingServices
    {
        public ListingServices(IAuthenticationService authenticationService)
            : base(authenticationService)
        {
        }

        public IEnumerable<ListingDetailViewModel> GetListings(string eMail)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var ownerKey = CustomStringHelper.BuildKey(new[] { eMail });

                var listingDetailList = session.CreateCriteria(typeof (ListingDetail))
                    .List<ListingDetail>().Where(x => x.Owner.OwnerKey.Equals(ownerKey) && x.IsVetted.Equals(1) && x.IsActive.Equals(1));

                return (from listingDetail in listingDetailList
                    let listingDetailViewModel = new ListingDetailViewModel()
                    select MapperHelper.Map(listingDetailViewModel, listingDetail)).ToList();

            }
        }

        public IEnumerable<ListingDetailViewModel> GetPackageListings()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var listingDetailList = session.CreateCriteria(typeof (ListingDetail))
                    .List<ListingDetail>().Where(x => x.IsVetted.Equals(0) && x.IsActive.Equals(1));

                return (from listingDetail in listingDetailList
                    let listingDetailViewModel = new ListingDetailViewModel()
                    select MapperHelper.Map(listingDetailViewModel, listingDetail)).ToList();

            }
        }

        public IEnumerable<ListingDetailViewModel> GetAdminListings()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var listingDetailList = session.CreateCriteria(typeof(ListingDetail))
                    .List<ListingDetail>().Where(x=> x.IsActive.Equals(1));

                return (from listingDetail in listingDetailList
                        let listingDetailViewModel = new ListingDetailViewModel()
                        select MapperHelper.Map(listingDetailViewModel, listingDetail)).ToList();

            }
        }

        public IEnumerable<ListingCategoryViewModel> GetListingCategories()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var listingCategoryList = session.CreateCriteria(typeof (Category))
                    .List<Category>().OrderBy(x => x.Name);

                return (from listingCategory in listingCategoryList
                    let listingCategoryViewModel = new ListingCategoryViewModel()
                    select MapperHelper.Map(listingCategoryViewModel, listingCategory)).ToList();

            }
        }

        public IEnumerable<ListingPackageViewModel> GetListingPackages()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var listingPackageList = session.CreateCriteria(typeof (PaymentPackage))
                    .List<PaymentPackage>().OrderBy(x => x.Name);

                return (from listingPackage in listingPackageList
                    let listingPackageViewModel = new ListingPackageViewModel()
                    select MapperHelper.Map(listingPackageViewModel, listingPackage)).ToList();

            }
        }

        public EditListingViewModel GetListing(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var listingDetail = session.CreateCriteria(typeof (ListingDetail))
                    .List<ListingDetail>().FirstOrDefault(x => x.Id.Equals(id) && x.IsActive.Equals(1));

                var listing = MapperHelper.Map(new EditListingViewModel(), listingDetail);

                listing.Rooms = JsonConvert.SerializeObject((from room in listingDetail.Rooms
                    let roomViewModel = new RoomViewModel()
                    select MapperHelper.Map(roomViewModel, room)).ToList());

                listing.Theatres = JsonConvert.SerializeObject((from theatre in listingDetail.Theatres
                    let theatreViewModel = new TheatreViewModel()
                    select MapperHelper.Map(theatreViewModel, theatre)).ToList());

                listing.Images = JsonConvert.SerializeObject((from image in listingDetail.Images
                    let imageViewModel = new ImageViewModel()
                    select MapperHelper.Map(imageViewModel, image)).ToList());

                listing.Tags = JsonConvert.SerializeObject((from tag in listingDetail.Tags
                    let tagViewModel = new TagViewModel()
                    select MapperHelper.Map(tagViewModel, tag)).ToList());

                listing.Package = listingDetail.PaymentPackage.Name;

               // var description = new StringWriter();
               // HttpUtility.HtmlDecode(listing.Description, description);
               // listing.Description = description.ToString();
               
                listing.Longitude = listingDetail.Location.Longitude;
                listing.Description = HttpUtility.UrlDecode(HttpUtility.HtmlDecode(listingDetail.Description));
                listing.OwnerName = string.Format("{0} {1}", listingDetail.Owner.Firstname, listingDetail.Owner.Surname);
                listing.StrapLine = HttpUtility.UrlDecode(HttpUtility.HtmlDecode(listingDetail.StrapLine));
                listing.Notes = listingDetail.Notes;
                listing.Email = listingDetail.Owner.ContactDetail.Email;
                listing.Phone = listingDetail.Owner.ContactDetail.Phone;
                listing.WebsiteUrl = listingDetail.Owner.ContactDetail.Website;

                listing.Longitude = listingDetail.Location.Longitude;
                listing.Latitude = listingDetail.Location.Latitude;

                HttpContext.Current.Session["OwnerEmail"] = listing.Email;

                return listing;
            }
        }

        public void AuthoriseListing(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var listingDetail = session.CreateCriteria(typeof(ListingDetail))
                   .List<ListingDetail>().FirstOrDefault(x => x.Id.Equals(id) && x.IsActive.Equals(1));

                    if (listingDetail != null)
                    {
                        listingDetail.IsVetted = 1;
                        SetAudit(listingDetail);
                        session.SaveOrUpdate(listingDetail);

                        transaction.Commit();    
                        return;
                    }
                    transaction.Rollback();
                }
            }
        }

        public void DeleteListing(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var listingDetail = session.CreateCriteria(typeof(ListingDetail))
                   .List<ListingDetail>().FirstOrDefault(x => x.Id.Equals(id));

                    if (listingDetail != null)
                    {
                       
                        session.Delete(listingDetail);

                        transaction.Commit();
                        return;
                    }
                    transaction.Rollback();
                }
            }
        }

        public void CancelListing(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var listingDetail = session.CreateCriteria(typeof(ListingDetail))
                   .List<ListingDetail>().FirstOrDefault(x => x.Id.Equals(id) && x.IsActive.Equals(1));

                    if (listingDetail != null)
                    {
                        listingDetail.IsVetted = 0;
                        SetAudit(listingDetail);
                        session.SaveOrUpdate(listingDetail);

                        transaction.Commit();    
                        return;
                    }
                    transaction.Rollback();
                }
            }
        }

        public List<CarouselImageViewModel> GetCarouselImages()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var imageList = session.CreateCriteria(typeof (Image))
                    .List<Image>().Where(x => x.ListingDetail.IsVetted.Equals(1) && x.DisplayOrder.Equals(0)).OrderBy(x => x.ListingDetail.Id);

                return CarouselImageList(imageList);
            }
        }

        public IEnumerable<CarouselImageViewModel> GetFeaturedListings()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var imageList = session.CreateCriteria(typeof(Image))
                    .List<Image>().Where(x => x.ListingDetail.IsVetted.Equals(1) && x.DisplayOrder.Equals(0) && x.ListingDetail.PaymentPackage.Id.Equals(3)).OrderBy(x => x.ListingDetail.Id);

                return CarouselImageList(imageList);
            }
        }

        public IEnumerable<SearchDetailViewModel> Search(string searchTerm)
        {
            searchTerm = searchTerm.ToLowerInvariant();
            using (var session = NHibernateHelper.OpenSession())
            {
                var listingDetailList = session.CreateCriteria<ListingDetail>()
                    .Add(Restrictions.Eq("IsVetted", (byte)1)).Add(Restrictions.Eq("IsActive", (byte)1)).List<ListingDetail>();

                listingDetailList = FilterResult(listingDetailList, searchTerm);

                return listingDetailList.Select(listingDetail => new SearchDetailViewModel
                {
                    ListingId = listingDetail.Id,
                    StrapLine = HttpUtility.UrlDecode(listingDetail.StrapLine),
                    Owner = string.Format("{0} {1}",
                        listingDetail.Owner.Firstname,
                        listingDetail.Owner.Surname),
                    Address = string.Format("{0}{1}, {2}, {3}",
                        listingDetail.AddressDetail.Address1,
                        string.IsNullOrEmpty(listingDetail.AddressDetail.Address2)
                            ? string.Empty
                            : string.Format(" {0}",
                                listingDetail.AddressDetail.Address2),
                        listingDetail.AddressDetail.Town,
                        listingDetail.AddressDetail.Postcode),
                    Description = HttpUtility.UrlDecode(HttpUtility.HtmlDecode(listingDetail.Description)),
                    Price = HttpUtility.UrlDecode(listingDetail.Price.Replace("%A3","£")),
                    ImageUrl = listingDetail.Images.FirstOrNull() == null
                        ? "/Media/Default/showdigs.jpg"
                        : ((Image) listingDetail.Images.First()).LargeUrl,
                    Tags = listingDetail.Tags.Select(tag => tag.Name).ToList()
                }).ToList();
            }
        }

        private IList<ListingDetail> FilterResult(IEnumerable<ListingDetail> listingDetailList, string searchTerm)
        {
            var rtnList = new List<ListingDetail>();
            searchTerm = searchTerm.ToLowerInvariant();
            foreach (var listingDetail in listingDetailList)
            {
                if (listingDetail.AddressDetail.Town.ToLowerInvariant().Contains(searchTerm)) rtnList.Add(listingDetail);
                else if (listingDetail.Theatres.Any(t => t.Name.ToLowerInvariant().Contains(searchTerm))) rtnList.Add(listingDetail);
                else if (listingDetail.Owner.Surname.ToLowerInvariant().Contains(searchTerm)) rtnList.Add(listingDetail);
                else if (listingDetail.Tags.Any(t=>t.Name.ToLowerInvariant().Contains(searchTerm))) rtnList.Add(listingDetail);
            }
          
            return rtnList;
        }

        private List<CarouselImageViewModel> CarouselImageList(IEnumerable<Image> imageList)
        {
            var carouselImageList = new List<CarouselImageViewModel>();
            var imageId = 0;
            foreach (var image in imageList)
            {
                if (imageId != image.Id)
                {
                    carouselImageList.Add(new CarouselImageViewModel
                    {
                        ListingId = image.ListingDetail.Id,
                        Url = image.LargeUrl,
                        Town = image.ListingDetail.Owner.AddressDetail.Town,
                        Postcode = image.ListingDetail.Owner.AddressDetail.Postcode,
                        StrapLine = image.ListingDetail.StrapLine

                    });
                    imageId = image.Id;
                }
            }

            return carouselImageList;
        }
    }
}