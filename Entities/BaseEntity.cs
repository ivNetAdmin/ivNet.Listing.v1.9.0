
using System;

namespace ivNet.Listing.Entities
{
    public class BaseEntity
    {
        public virtual int Id { get; protected set; }

        public virtual byte IsActive { get; set; }

        public virtual string CreatedBy { get; set; }
        public virtual DateTime? CreateDate { get; set; }
        public virtual string ModifiedBy { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
    }
}