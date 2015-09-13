
using System;
using System.Linq;
using ivNet.Listing.Entities;
using NHibernate;
using Orchard.Security;

namespace ivNet.Listing.Service
{
    public class BaseService
    {
        protected readonly IUser CurrentUser;

        public BaseService(IAuthenticationService authenticationService)
        {
            CurrentUser = authenticationService.GetAuthenticatedUser();
        }

        // owner check
        protected Owner DuplicateCheck(ISession session, Owner owner, string key)
        {
            var entity = session.CreateCriteria(typeof(Owner))
               .List<Owner>().FirstOrDefault(x => x.OwnerKey.Equals(key));
            return entity ?? owner;
        }

        protected void SetAudit(BaseEntity entity, string merge = null)
        {
            var currentUser = CurrentUser != null ? CurrentUser.UserName : "Non-Authenticated";

            entity.ModifiedBy = currentUser;
            entity.ModifiedDate = DateTime.Now;

            if (entity.Id != 0) return;
            entity.CreatedBy = currentUser;
            entity.CreateDate = DateTime.Now;
        }        
    }
}