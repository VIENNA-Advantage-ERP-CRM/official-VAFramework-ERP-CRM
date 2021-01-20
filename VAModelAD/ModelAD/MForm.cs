/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAF_Page
 * Chronological Development
 * Veena Pandey     29-Aug-09
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MForm : X_VAF_Page
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Page_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MForm(Context ctx, int VAF_Page_ID, Trx trxName)
            : base(ctx, VAF_Page_ID, trxName)
        {
        }

        public MForm(Ctx ctx, int VAF_Page_ID, Trx trxName)
            : base(ctx, VAF_Page_ID, trxName)
        {
        }


        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MForm(Context ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            if (newRecord)
            {
                int AD_Role_ID = GetCtx().GetAD_Role_ID();
                MFormAccess pa = new MFormAccess(this, AD_Role_ID);
                pa.Save();
            }
            return success;
        }
    }
}
