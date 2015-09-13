
using FluentNHibernate.Mapping;

namespace ivNet.Listing.Entities
{
    public class ContactDetail : BaseEntity
    {
        public virtual string ContactDetailKey { get; set; }

        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Website { get; set; }
    }

    public class ContactDetailMap : ClassMap<ContactDetail>
    {
        public ContactDetailMap()
        {
            Id(x => x.Id);
            Map(x => x.ContactDetailKey).Not.Nullable().Length(120).UniqueKey("ix_ContactDetails_Unique");

            Map(x => x.Email).Not.Nullable().Length(50);
            Map(x => x.Phone).Not.Nullable().Length(50);
            Map(x => x.Website).Nullable().Length(255);

            Map(x => x.IsActive);

            Map(x => x.CreatedBy).Not.Nullable().Length(50);
            Map(x => x.CreateDate).Not.Nullable();
            Map(x => x.ModifiedBy).Not.Nullable().Length(50);
            Map(x => x.ModifiedDate).Not.Nullable();
        }
    }
}
