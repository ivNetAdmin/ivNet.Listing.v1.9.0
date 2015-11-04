
using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace ivNet.Listing.Entities
{
    public class Owner : BaseEntity
    {
        public virtual string OwnerKey { get; set; }

        public virtual string Surname { get; set; }        
        public virtual string Firstname { get; set; }
        public virtual int UserId { get; set; }        

        public virtual AddressDetail AddressDetail { get; set; }
        public virtual ContactDetail ContactDetail { get; set; }
        public virtual IList<ListingDetail> Listings { get; set; }

        public virtual void Init()
        {          
            Listings = new List<ListingDetail>();
        }
    }

    public class OwnerMap : ClassMap<Owner>
    {
        public OwnerMap()
        {
            Id(x => x.Id);
            Map(x => x.OwnerKey).Not.Nullable().Length(120).UniqueKey("ix_OwnerKey_Unique");

            Map(x => x.UserId);

            Map(x => x.Surname).Not.Nullable().Length(50).Index("ix_Surname_Unique");
            Map(x => x.Firstname).Not.Nullable().Length(50);

            
            Map(x => x.IsActive);

            References(x => x.AddressDetail);
            References(x => x.ContactDetail);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();

            HasMany(x => x.Listings)
              .Inverse()
              .Cascade.All();
        }
    }
}