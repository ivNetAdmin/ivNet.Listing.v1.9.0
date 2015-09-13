using FluentNHibernate.Mapping;

namespace ivNet.Listing.Entities
{
    public class Room : BaseEntity
    {
        public virtual string Type { get; set; }
        public virtual string Description { get; set; }
        public virtual ListingDetail ListingDetail { get; set; }
    }

    public class RoomMap : ClassMap<Room>
    {
        public RoomMap()
        {
            Id(x => x.Id);

            Map(x => x.Type).Not.Nullable().Length(50);
            Map(x => x.Description).Not.Nullable().Length(2000);

            References(x => x.ListingDetail);

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}