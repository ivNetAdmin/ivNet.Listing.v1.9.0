
using FluentNHibernate.Mapping;

namespace ivNet.Listing.Entities
{
    public class Transport : BaseEntity
    {
        public virtual string Name { get; set; }

    }

    public class TransportMap : ClassMap<Transport>
    {
        public TransportMap()
        {
            Id(x => x.Id);

            Map(x => x.Name).Not.Nullable().Length(50);          
        }
    }
}