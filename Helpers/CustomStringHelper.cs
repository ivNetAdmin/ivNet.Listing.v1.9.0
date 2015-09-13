using System;
using System.Text.RegularExpressions;
using FluentNHibernate;
using ivNet.Listing.Entities;

namespace ivNet.Listing.Helpers
{
    public class CustomStringHelper
    {
        public static string BuildKey(string[] items)
        {
            var key = String.Join(string.Empty, items).ToLowerInvariant();
            return Regex.Replace(key, "[^0-9a-z]", string.Empty);
        }

        public static string GenerateInitialPassword(Owner owner)
        {
            return string.Format("{0}{1}1",
              owner.Firstname.ToLowerInvariant(),
              owner.Firstname.ToUpperInvariant())
              .Replace(" ", string.Empty);
        }
    }
}