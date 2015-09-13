
using FluentNHibernate.Mapping;

namespace ivNet.Listing.Entities
{
    public class Theatre : BaseEntity
    {
        public virtual string Name { get; set; }        
        public virtual string Distance { get; set; }
        public virtual string Town { get; set; }
        public virtual string Transport { get; set; }
        public virtual ListingDetail ListingDetail { get; set; }
    }

    public class TheatreMap : ClassMap<Theatre>
    {
        public TheatreMap()
        {
            Id(x => x.Id);

            Map(x => x.Name).Not.Nullable().Length(50);
            Map(x => x.Distance).Not.Nullable().Length(255); 
            Map(x => x.Town).Nullable().Length(50);            
            Map(x => x.Transport).Nullable().Length(50);

            References(x => x.ListingDetail);

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}