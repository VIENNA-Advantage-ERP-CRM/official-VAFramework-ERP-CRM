/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : UserPricingInterface
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     10-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;


namespace VAdvantage.Model
{
    public interface UserPricingInterface
    {
        /// <summary>
        /// User Pricing
        /// </summary>
        /// <param name="VAF_Org_ID">org of document</param>
        /// <param name="isSOTrx">sales order</param>
        /// <param name="VAM_PriceList_ID">price list selected</param>
        /// <param name="VAB_BusinessPartner_ID">business partner</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="Qty">quantity</param>
        /// <param name="PriceDate">date for pricing</param>
        /// <returns>null if Vienna should price it or pricing info</returns>
        UserPricingVO Price(int VAF_Org_ID, bool isSOTrx, int VAM_PriceList_ID, int VAB_BusinessPartner_ID,
            int VAM_Product_ID, Decimal Qty, DateTime? PriceDate);
    }
}
