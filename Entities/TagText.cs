
using FluentNHibernate.Mapping;

namespace ivNet.Listing.Entities
{
    public class TagText : BaseEntity
    {
        public virtual string Name { get; set; }

    }

    public class TagTextMap : ClassMap<TagText>
    {
        public TagTextMap()
        {
            Id(x => x.Id);

            Map(x => x.Name).Not.Nullable().Length(50);          
        }
    }
}