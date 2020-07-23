using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace VAdvantage.Classes
{
    [DataContract]
    public class ContactsInfo
    {
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string EmailAddress { get; set; }
        [DataMember]
        public string ContactName { get; set; }
        [DataMember]
        public string Content { get; set;}
        [DataMember]
        public string PhoneNumber { get; set; }
        [DataMember]
        public string Organization { get; set; }
        [DataMember]
        public string Groupmembership { get; set; }
        [DataMember]
        public string IMAddress { get; set; }
        [DataMember]
        public bool IsCustomer { get; set; }
        [DataMember]
        public bool IsVendor { get; set; }
        [DataMember]
        public bool IsEmployee { get; set; }
        [DataMember]
        public bool IsSelected { get; set; }
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public List<ContactAddressInfo> Address { get; set; }
        [DataMember]
        public string Error { get; set; }
        [DataMember]
        public string BPID { get; set; }
    }

    [DataContract]
    public class ContactAddressInfo
    {
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public string Housename { get; set; }
        [DataMember]
        public string Region { get; set; }
        [DataMember]
        public string StreetNo { get; set; }
        [DataMember]
        public string Postcode { get; set; }
        [DataMember]
        public bool IsHomeAddress { get; set; }
    }
}
