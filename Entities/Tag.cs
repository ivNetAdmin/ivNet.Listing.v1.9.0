
using FluentNHibernate.Mapping;

namespace ivNet.Listing.Entities
{
    public class Tag : BaseEntity
    {
        public virtual string Name { get; set; }
        public virtual ListingDetail ListingDetail { get; set; }
      
    }

    public class TagMap : ClassMap<Tag>
    {
        public TagMap()
        {
            Id(x => x.Id);

            Map(x => x.Name).Not.Nullable().Length(50).Index("ix_TagName_Unique");

            References(x => x.ListingDetail);

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}