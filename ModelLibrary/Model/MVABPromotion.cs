/********************************************************
 * Module Name    : 
 * Purpose        : Campaign model
 * Class Used     : X_VAB_Promotion
 * Chronological    Development
 * Veena        09-Nov-2009
**********************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.Logging;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    /// <summary>
    /// Campaign model
    /// </summary>
    public class MVABPromotion : X_VAB_Promotion
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_Promotion_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVABPromotion(Ctx ctx, int VAB_Promotion_ID, Trx trxName)
            : base(ctx, VAB_Promotion_ID, trxName)
        {
            if (VAB_Promotion_ID == 0)
            {
                SetCosts(Env.ZERO);
                SetIsSummary(false);
            }
        }




        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MVABPromotion(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// After Save
        /// Insert
        /// - Create Tree
        /// </summary>
        /// <param name="newRecord">insert</param>
        /// <param name="success">save success</param>
        /// <returns>success</returns>
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            if (!success)
                return success;
            //	Value/Name change
            if (!newRecord && (Is_ValueChanged("Value") || Is_ValueChanged("Name")))
                MVABAccount.UpdateValueDescription(GetCtx(), "VAB_Promotion_ID=" + GetVAB_Promotion_ID(), Get_TrxName());

            return true;
        }
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetEndDate() < GetStartDate())
            {
                log.SaveError("Error", Msg.GetMsg(GetCtx(), "EnddategrtrthnStartdate"));
                return false;
            }
            return true;
        }
    }
}
