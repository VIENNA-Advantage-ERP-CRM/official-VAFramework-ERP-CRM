using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VIS.Classes
{
    public class VChargeModelClass
    {

    }

    public class VChargeLodGrideData
    {
        public List<VchargeMCElementTaxCategoryID> VchargeMCElemTaxCatID { get; set; }
        public List<VchargeCElementValue> VchargeCElementValue { get; set; }
        public int meID { get; set; }
    }

    public class VchargeMCElementTaxCategoryID
    {
        public int mCElementID { get; set; } 
        public int mCTaxCategoryID { get; set; } 
    }

    public class VchargeCElementValue
    {
        public int C_ElementValue_ID { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
        public string Expense { get; set; }        
    }
}