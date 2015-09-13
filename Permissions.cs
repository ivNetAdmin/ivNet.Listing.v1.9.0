
using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace ivNet.Listing
{
    public class Permissions : IPermissionProvider
    {

        public static readonly Permission ivAdminTab = new Permission
        {
            Description = "Access admin website tab",
            Name = "ivAdminTab"
        };

        public static readonly Permission ivOwnerTab = new Permission
        {
            Description = "Access owner website tab",
            Name = "ivOwnerTab"
        };

        public Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ivAdminTab,    
                ivOwnerTab
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return null;
        }
    }
}