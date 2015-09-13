
using System;

namespace ivNet.Listing.Models
{
    public class ListingDetailViewModel
    {
        public int Id { get; set; }
        public string PostCode { get; set; }
        public string PaymentPackageName { get; set; }
        public DateTime PaymentPackageExpiraryDate { get; set; }
        public string Owner { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public bool IsVetted { get; set; }
    }
}