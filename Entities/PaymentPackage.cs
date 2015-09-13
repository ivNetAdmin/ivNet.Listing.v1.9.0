
using System;
using FluentNHibernate.Mapping;

namespace ivNet.Listing.Entities
{
    public class PaymentPackage : BaseEntity
    {
        public virtual string Name { get; set; }        
        public virtual Decimal Cost { get; set; }
        public virtual int PhotoCount { get; set; }
        public virtual byte ShowWebsiteLink { get; set; }
        public virtual byte AllowRichText { get; set; }
        public virtual byte IsPriorityListing { get; set; }
    }

    public class PaymentPackageMap : ClassMap<PaymentPackage>
    {
        public PaymentPackageMap()
        {
            Id(x => x.Id);

            Map(x => x.Name).Not.Nullable().Length(50);            
            Map(x => x.Cost).Not.Nullable();
            Map(x => x.PhotoCount).Not.Nullable();
            Map(x => x.ShowWebsiteLink);
            Map(x => x.AllowRichText);
            Map(x => x.IsPriorityListing);
        }
    }
}