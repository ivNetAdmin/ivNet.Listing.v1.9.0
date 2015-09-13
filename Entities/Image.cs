
using FluentNHibernate.Mapping;

namespace ivNet.Listing.Entities
{
    public class Image : BaseEntity
    {
        public virtual string Alt { get; set; }
        public virtual string ThumbUrl { get; set; }
        public virtual string LargeUrl { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual ListingDetail ListingDetail { get; set; }
      
    }

    public class ImageMap : ClassMap<Image>
    {
        public ImageMap()
        {
            Id(x => x.Id);

            Map(x => x.Alt).Not.Nullable().Length(50);
            Map(x => x.ThumbUrl).Not.Nullable().Length(255);
            Map(x => x.LargeUrl).Not.Nullable().Length(255);
            Map(x => x.DisplayOrder).Not.Nullable();

            References(x => x.ListingDetail);

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }

    }
}