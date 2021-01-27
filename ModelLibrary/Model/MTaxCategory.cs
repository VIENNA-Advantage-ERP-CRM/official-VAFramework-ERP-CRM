/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MTaxCategory
 * Purpose        : Tax Category Model
 * Class Used     : X_VAB_TaxCategory class
 * Chronological    Development
 * Deepak           20-Nov-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MTaxCategory : X_VAB_TaxCategory
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_TaxCategory_ID">id</param>
        /// <param name="trxName">trx</param>
        public MTaxCategory(Ctx ctx, int VAB_TaxCategory_ID, Trx trxName)
            : base(ctx, VAB_TaxCategory_ID, trxName)
        {
            //super(ctx, VAB_TaxCategory_ID, trxName);
            if (VAB_TaxCategory_ID == 0)
            {
                //	setName (null);
                SetIsDefault(false);
            }
        }	//	MTaxCategory

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">trx</param>
        public MTaxCategory(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }	//	MTaxCategory

        protected override bool BeforeSave(bool newRecord)
        {
            Tuple<String, String, String> mInfo = null;
            if (Env.HasModulePrefix("VATAX_", out mInfo))
            {
                if ((!String.IsNullOrEmpty(GetVATAX_Preference1()) || !String.IsNullOrEmpty(GetVATAX_Preference2())) && GetVATAX_Preference1() == GetVATAX_Preference2() ||
                    (!String.IsNullOrEmpty(GetVATAX_Preference1()) || !String.IsNullOrEmpty(GetVATAX_Preference3())) && GetVATAX_Preference1() == GetVATAX_Preference3() ||
                    (!String.IsNullOrEmpty(GetVATAX_Preference2()) || !String.IsNullOrEmpty(GetVATAX_Preference3())) && GetVATAX_Preference2() == GetVATAX_Preference3())
                {
                    return false;
                }
            }
            return true;
        }

    }	//	MTaxCategory

}
