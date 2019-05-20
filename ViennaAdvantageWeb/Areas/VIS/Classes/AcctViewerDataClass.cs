using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIS.Classes
{
    public class AcctViewerData
    {

    }
    public class AcctViewerDatakeysParam
    {
        public int Key { get; set; } 
        public string Name { get; set; }
    }

    public class AcctViewerDataTabel
    {
        public int AD_Table_ID { get; set; }
        public string TableName { get; set; }
    }

    public class AcctViewerDataOrg
    {
        public int AD_Org_ID { get; set; }
        public string OrgName { get; set; }
    }

    public class AcctViewerDataPosting
    {
        public string PostingValue { get; set; }
        public string PostingName { get; set; }
    }

    public class AcctViewerDataAcctSchElements
    {
        public int C_AcctSchema_Element_ID { get; set; }
        public string ElementName { get; set; }
        public string ElementType { get; set; }
        public int C_ElementValue_ID { get; set; }
        public int SeqNo { get; set; }
        public string Detail { get; set; }
        public string C_Element_ID { get; set; }
    }

}