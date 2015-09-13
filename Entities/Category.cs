
using FluentNHibernate.Mapping;

namespace ivNet.Listing.Entities
{
    public class Category : BaseEntity
    {
        public virtual string Name { get; set; }
    }

    public class CategoryMap : ClassMap<Category>
    {
        public CategoryMap()
        {
            Id(x => x.Id);

            Map(x => x.Name).Not.Nullable().Length(50);
          
        }
    }
}