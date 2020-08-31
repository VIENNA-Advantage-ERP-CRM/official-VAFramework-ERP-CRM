using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace MarketSvc
{
    [DataContract]
    public class InitLoginInfo
    {
        [DataMember]
        public bool IsValid
        { get; set; }
        [DataMember]
        public String Message
        {
            get;
            set;
        }
        [DataMember]
        public bool IsAllowWork
        {
            get;
            set;
        }
        [DataMember]
        public bool IsULExceeded
        {
            get;
            set;
        }
        [DataMember]
        public Object Constraint
        {
            get;
            set;
        }
        [DataMember]
        public int Client_ID
        {
            get;
            set;
        }
        [DataMember]
        public String ClientName
        {
            get;
            set;
        }
        [DataMember]
        public String UserName
        {
            get;
            set;
        }
        [DataMember]
        public String RoleName
        {
            get;
            set;
        }
        [DataMember]
        public String Token
        {
            get;
            set;
        }
        [DataMember]
        public String Url
        {
            get;
            set;
        }
        [DataMember]
        public bool IsDemoCheck
        {
            get;
            set;
        }

        [DataMember]
        public bool IsEntCheck
        {
            get;
            set;
        }

        [DataMember]
        public String IP
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
        public bool IsExpired
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
        public string  KeyEdition
        {
            get;
            set;
        }


        internal static InitLoginInfo InitLogin(InitLoginInfo info,string auth)
        {
            EvaluationCheck.Init(auth);
            info.Url = GetKey(info.Url);
            InitLoginInfo result = new InitLoginInfo();
            if (info.IsDemoCheck)
            {
                if (!info.IsAllowWork) // on cloud 
                    result = EvaluationCheck.DemoCheck(info);
                result.KeyEdition = EvaluationCheck.GetKeyEdition(info.Url);
            }
            else
            {
                result = EvaluationCheck.ProcessUpdateLicense(info);
                result.KeyEdition = EvaluationCheck.GetKeyEdition(info.Url);
            }
            return result;
        }
        private static String GetKey(string url)
        {
            url = url.ToLower().Replace("http://", "");
            url = url.Replace("https://", "");
            url = url.Replace("/viennaadvantage.aspx", "");
            return url;

        }
    }
}
