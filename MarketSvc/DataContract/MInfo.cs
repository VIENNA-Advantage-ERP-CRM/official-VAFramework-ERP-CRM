using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MarketSvc
{
    /// <summary>
    /// Hold Client-Server[Market] Parameter
    /// </summary>
    [DataContract]
    public class MInfo
    {

        //Server To client Data
        [DataMember]
        public String TokenKey
        {
            get;
            set;
        }
        [DataMember]
        public bool IsProfessionalEdition
        {
            get;
            set;
        }
        [DataMember]
        public bool IsRegistered
        {
            get;
            set;
        }

        [DataMember]
        public bool IsKeyExpired
        {
            get;
            set;
        }

        [DataMember]
        public bool IsMarketExpired
        {
            get;
            set;
        }


        [DataMember]
        public List<MService.MarketModuleInfo> lstMarketModuleInfo
        {
            get;
            set;
        }

        //Client to Server Data

        [DataMember]
        public String HostURL
        {
            get;
            set;
        }

        [DataMember]
        public String ModType
        {
            get;
            set;
        }

        [DataMember]
        public String SqlString
        {
            get;
            set;
        }

        [DataMember]
        public int PageSize
        {
            get;
            set;
        }

        [DataMember]
        public int PageIndex
        {
            get;
            set;
        }

    }
}
