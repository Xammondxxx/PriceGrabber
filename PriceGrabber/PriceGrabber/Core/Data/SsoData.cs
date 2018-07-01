using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceGrabber.Core.Data
{
    public class SsoData
    {
        public string FirstName { get; set; }
        public string LocationId { get; set; }
        public string deviceid { get; set; }
        public string MobilePhone { get; set; }
        public string HPRole { get; set; }
        public string Language { get; set; }
        public string PhysAdLine1 { get; set; }
        public string PhysAdLine2 { get; set; }
        public string PhysAdLine3 { get; set; }
        public string Programs { get; set; }
        public string PostalCode { get; set; }
        public string UserRights { get; set; }
        public string PartnerName { get; set; }
        public string Country { get; set; }
        public string PartnerPhone { get; set; }
        public string Campaigns { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Accreditations { get; set; }
        public string CountryCode { get; set; }
        public string HPOrg { get; set; }
        public string PartnerProIdHQ { get; set; }
        public string PartnerNameHQ { get; set; }
        public string ContactId { get; set; }
        public string LiferayUserId { get; set; }
        public string Email { get; set; }
        public string UserCountryOfBusiness { get; set; }
        public string WorkPhone { get; set; }
        public string PartnerProId { get; set; }
        public string LastName { get; set; }

        public string FullName => FirstName ?? "" + " " + LastName ?? "";
        public string CompanyFullName => PartnerName ?? "" + " (" + LocationId ?? "" + ")";
        public string CompanyAddress => PhysAdLine1 ?? "" + " " + PhysAdLine2 ?? "" + " " + PhysAdLine3 ?? "" + " " + PostalCode ?? "";
    }
}
