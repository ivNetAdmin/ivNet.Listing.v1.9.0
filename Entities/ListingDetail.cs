
using System;
using System.Collections.Generic;
using FluentNHibernate.Mapping;

namespace ivNet.Listing.Entities
{
    public class ListingDetail : BaseEntity
    {
        public virtual string ListingKey { get; set; }
        public virtual string StrapLine { get; set; }
        public virtual string Description { get; set; }
        public virtual string Price { get; set; }
        public virtual string Notes { get; set; }
        public virtual string WebsiteUrl { get; set; }
        public virtual byte IsPriority { get; set; }
        public virtual byte IsVetted { get; set; }
        public virtual DateTime ExpiraryDate { get; set; }

        public virtual Owner Owner { get; set; }
        public virtual AddressDetail AddressDetail { get; set; }
        public virtual Category Category { get; set; }
        public virtual Location Location { get; set; }
        public virtual PaymentPackage PaymentPackage { get; set; }
        public virtual IList<Tag> Tags { get; set; }
        public virtual IList<Image> Images { get; set; }
        public virtual IList<Theatre> Theatres { get; set; }
        public virtual IList<Room> Rooms { get; set; }

        public virtual void Init()
        {           
            Tags = new List<Tag>();
            Images = new List<Image>();
            Theatres = new List<Theatre>();
            Rooms = new List<Room>();
        }
    }

    public class ListingDetailMap : ClassMap<ListingDetail>
    {
        public ListingDetailMap()
        {
            Id(x => x.Id);

            Map(x => x.ListingKey).Not.Nullable().Length(50);
            Map(x => x.StrapLine).Nullable().Length(500);
            Map(x => x.Price).Nullable().Length(500);
            Map(x => x.Description).Nullable().Length(4500);
            Map(x => x.Notes).Nullable().Length(2000);
            Map(x => x.WebsiteUrl).Nullable().Length(255);
            Map(x => x.ExpiraryDate).Not.Nullable();

            Map(x => x.IsVetted);

            References(x => x.AddressDetail);
            References(x => x.Location);
            References(x => x.Category);
            References(x => x.PaymentPackage);
            References(x => x.Owner);

            HasMany(x => x.Tags)
               .Inverse()
               .Cascade.All();

            HasMany(x => x.Images)
              .Inverse()
              .Cascade.All(); 
            
            HasMany(x => x.Theatres)
              .Inverse()
              .Cascade.All();

            HasMany(x => x.Rooms)
             .Inverse()
             .Cascade.All();

            Map(x => x.IsPriority);

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}