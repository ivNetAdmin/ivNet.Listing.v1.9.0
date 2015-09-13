
using System.Linq;
using ivNet.Listing.Entities;
using ivNet.Listing.Helpers;
using ivNet.Listing.Models;
using Orchard;
using Orchard.Security;

namespace ivNet.Listing.Service
{
    public interface IConfigurationServices : IDependency
    {
        ConfigurationViewModel GetConfiguration();
        void Delete(int id, string type);
        Category CreateCategory(string name);
        Transport CreateTransport(string name);
        TagText CreateTag(string name);
        RoomType CreateRoomType(string name);
    }

    public class ConfigurationServices : BaseService, IConfigurationServices
    {
        public ConfigurationServices(IAuthenticationService authenticationService)
            : base(authenticationService)
        {
        }

        public ConfigurationViewModel GetConfiguration()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var listingCategoryList = session.CreateCriteria(typeof (Category))
                    .List<Category>().OrderBy(x => x.Name);

                var listingTransportList = session.CreateCriteria(typeof(Transport))
                   .List<Transport>().OrderBy(x => x.Name);

                var roomTypeList = session.CreateCriteria(typeof(RoomType))
                  .List<RoomType>().OrderBy(x => x.Name);

                var tagTextList = session.CreateCriteria(typeof(TagText))
                 .List<TagText>().OrderBy(x => x.Name);

                var configurationViewModel = new ConfigurationViewModel
                {
                    Categories = (from listingCategory in listingCategoryList
                        let listingCategoryViewModel = new ListingCategoryViewModel()
                        select MapperHelper.Map(listingCategoryViewModel, listingCategory)).ToList(),

                    TransportList = (from listingTransport in listingTransportList
                                  let listingTransportViewModel = new ListingTransportViewModel()
                                     select MapperHelper.Map(listingTransportViewModel, listingTransport)).ToList(),

                    RoomTypes = (from roomType in roomTypeList
                                     let roomTypeViewModel = new ListingRoomTypeViewModel()
                                 select MapperHelper.Map(roomTypeViewModel, roomType)).ToList(),

                    TagTextList = (from tagText in tagTextList
                                 let tagTextViewModel = new ListingTagTextViewModel()
                                   select MapperHelper.Map(tagTextViewModel, tagText)).ToList()

                };

                return configurationViewModel;
            }
        }

        public void Delete(int id, string type)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    switch (type)
                    {
                        case "category":
                            var category = session.CreateCriteria(typeof (Category))
                                .List<Category>().FirstOrDefault(x => x.Id == id);

                            session.Delete(category);
                            transaction.Commit();
                            break;

                        case "transport":
                            var transport = session.CreateCriteria(typeof(Transport))
                                .List<Transport>().FirstOrDefault(x => x.Id == id);

                            session.Delete(transport);
                            transaction.Commit();
                            break;

                        case "tag":
                            var tag = session.CreateCriteria(typeof(TagText))
                                .List<TagText>().FirstOrDefault(x => x.Id == id);

                            session.Delete(tag);
                            transaction.Commit();
                            break;

                        case "roomType":
                            var roomType = session.CreateCriteria(typeof(RoomType))
                                .List<RoomType>().FirstOrDefault(x => x.Id == id);

                            session.Delete(roomType);
                            transaction.Commit();
                            break;
                        default:
                            transaction.Rollback();
                            break;
                    }
                }
            }
        }

        public Category CreateCategory(string name)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                      var category = session.CreateCriteria(typeof (Category))
                          .List<Category>().FirstOrDefault(x => x.Name.ToLowerInvariant() == name.ToLowerInvariant()) ??
                                     new Category {Name = name};

                    SetAudit(category);
                    session.SaveOrUpdate(category);
                    transaction.Commit();

                    return category;
                }
            }
        }

        public Transport CreateTransport(string name)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var transport = new Transport { Name = name };

                    SetAudit(transport);
                    session.SaveOrUpdate(transport);
                    transaction.Commit();

                    return transport;
                }
            }
        }

        public TagText CreateTag(string name)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var tag = new TagText { Name = name };

                    SetAudit(tag);
                    session.SaveOrUpdate(tag);
                    transaction.Commit();

                    return tag;
                }
            }
        }

        public RoomType CreateRoomType(string name)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var roomType = new RoomType { Name = name };

                    SetAudit(roomType);
                    session.SaveOrUpdate(roomType);
                    transaction.Commit();

                    return roomType;
                }
            }
        }
    }
}