
using AutoMapper;
using ivNet.Listing.Entities;
using ivNet.Listing.Models;

namespace ivNet.Listing.Helpers
{
    public class MapperHelper
    {
        #region models->entities

        public static Owner Map(Owner entity, RegistrationViewModel viewModel)
        {
            return Mapper.Map(viewModel, entity);
        }
        public static AddressDetail Map(AddressDetail entity, RegistrationViewModel viewModel)
        {
            return Mapper.Map(viewModel, entity);
        }
      
        public static ContactDetail Map(ContactDetail entity, RegistrationViewModel viewModel)
        {
            return Mapper.Map(viewModel, entity);
        }
        
        #endregion

        #region entities->models
        public static ListingDetailViewModel Map(ListingDetailViewModel viewModel, ListingDetail entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static ListingCategoryViewModel Map(ListingCategoryViewModel viewModel, Category entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static ListingTransportViewModel Map(ListingTransportViewModel viewModel, Transport entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static ListingRoomTypeViewModel Map(ListingRoomTypeViewModel viewModel, RoomType entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static ListingTagTextViewModel Map(ListingTagTextViewModel viewModel, TagText entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static ListingPackageViewModel Map(ListingPackageViewModel viewModel, PaymentPackage entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static EditListingViewModel Map(EditListingViewModel viewModel, ListingDetail entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static RoomViewModel Map(RoomViewModel viewModel, Room entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static TheatreViewModel Map(TheatreViewModel viewModel, Theatre entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static ImageViewModel Map(ImageViewModel viewModel, Image entity)
        {
            return Mapper.Map(entity, viewModel);
        }

        public static TagViewModel Map(TagViewModel viewModel, Tag entity)
        {
            return Mapper.Map(entity, viewModel);
        }
      
        #endregion
       
    }
}