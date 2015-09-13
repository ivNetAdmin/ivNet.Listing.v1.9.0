
using FluentNHibernate.Mapping;

namespace ivNet.Listing.Entities
{
    public class AddressDetail : BaseEntity
    {
        public virtual string AddressDetailKey { get; set; }

        public virtual string Address1 { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string Town { get; set; }
        public virtual string Postcode { get; set; }
    }

    public class AddressDetailMap : ClassMap<AddressDetail>
    {
        public AddressDetailMap()
        {
            Id(x => x.Id);
            Map(x => x.AddressDetailKey).Not.Nullable().Length(120).UniqueKey("ix_AddressDetail_Unique");

            Map(x => x.Address1).Not.Nullable().Length(100);
            Map(x => x.Address2).Nullable().Length(100);
            Map(x => x.Postcode).Not.Nullable().Length(12);
            Map(x => x.Town).Not.Nullable().Length(50);
            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}
