
using AutoMapper;
using ivNet.Listing.Entities;
using ivNet.Listing.Models;

using Orchard.Environment;

namespace ivNet.Listing
{
    public class StartupShellEvents : IOrchardShellEvents
    {
        public void Activated()
        {
            #region models->entities

            Mapper.CreateMap<RegistrationViewModel, Owner>();
            Mapper.CreateMap<RegistrationViewModel, AddressDetail>();
            Mapper.CreateMap<RegistrationViewModel, ContactDetail>();  

            #endregion

            #region entities->models

            Mapper.CreateMap<ListingDetail, ListingDetailViewModel>()
                .ForMember(v => v.Id, m => m.MapFrom(e => e.Id))
                .ForMember(v => v.Owner,
                    m => m.MapFrom(e => string.Format("{0}, {1}", e.Owner.Surname, e.Owner.Firstname)))
                .ForMember(v => v.PostCode, m => m.MapFrom(e => e.AddressDetail.Postcode))
                .ForMember(v => v.Email, m => m.MapFrom(e => e.Owner.ContactDetail.Email))
                .ForMember(v => v.PaymentPackageName, m => m.MapFrom(e => e.PaymentPackage.Name))
                .ForMember(v => v.PaymentPackageExpiraryDate, m => m.MapFrom(e => e.ExpiraryDate))
                .ForMember(v => v.Address, m => m.MapFrom(e =>
                    string.Format("{0}{1} {2}",
                        e.AddressDetail.Address1,
                        string.IsNullOrEmpty(e.AddressDetail.Address2)
                            ? string.Empty
                            : string.Format(" {0}", e.AddressDetail.Address2),
                        e.AddressDetail.Town
                        )));

            Mapper.CreateMap<ListingDetail, EditListingViewModel>()
                .ForMember(v => v.Id, m => m.MapFrom(e => e.Id))
                .ForMember(v => v.PackageId, m => m.MapFrom(e => e.PaymentPackage.Id))
                .ForMember(v => v.CategoryId, m => m.MapFrom(e => e.Category.Id))
                .ForMember(v => v.Category, m => m.MapFrom(e => e.Category.Name))
                .ForMember(v => v.Address1, m => m.MapFrom(e => e.AddressDetail.Address1))
                .ForMember(v => v.Address2, m => m.MapFrom(e => e.AddressDetail.Address2))
                .ForMember(v => v.Town, m => m.MapFrom(e => e.AddressDetail.Town))
                .ForMember(v => v.Postcode, m => m.MapFrom(e => e.AddressDetail.Postcode));

            Mapper.CreateMap<Category, ListingCategoryViewModel>();
            Mapper.CreateMap<PaymentPackage, ListingPackageViewModel>();
            Mapper.CreateMap<Transport, ListingTransportViewModel>();
            Mapper.CreateMap<RoomType, ListingRoomTypeViewModel>();
            Mapper.CreateMap<TagText, ListingTagTextViewModel>();

            Mapper.CreateMap<Room, RoomViewModel>()
                .ForMember(v => v.RoomType, m => m.MapFrom(e => e.Type));

            Mapper.CreateMap<Image, ImageViewModel>()
                .ForMember(v => v.Thumb, m => m.MapFrom(e => e.ThumbUrl))
                .ForMember(v => v.File, m => m.MapFrom(e => e.LargeUrl));

            Mapper.CreateMap<Theatre, TheatreViewModel>();
            Mapper.CreateMap<Tag, TagViewModel>();
            
            #endregion
        }

        public void Terminating()
        {

        }
    }
}