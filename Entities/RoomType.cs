
using FluentNHibernate.Mapping;

namespace ivNet.Listing.Entities
{
    public class RoomType : BaseEntity
    {
        public virtual string Name { get; set; }

    }

    public class RoomTypetMap : ClassMap<RoomType>
    {
        public RoomTypetMap()
        {
            Id(x => x.Id);

            Map(x => x.Name).Not.Nullable().Length(50);
        }
    }
}