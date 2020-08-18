/********************************************************
 * Module Name    : 
 * Purpose        : Campaign model
 * Class Used     : X_C_Campaign
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
    public class MCampaign : X_C_Campaign
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Campaign_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MCampaign(Ctx ctx, int C_Campaign_ID, Trx trxName)
            : base(ctx, C_Campaign_ID, trxName)
        {
            if (C_Campaign_ID == 0)
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
        public MCampaign(Ctx ctx, DataRow dr, Trx trxName)
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
                MAccount.UpdateValueDescription(GetCtx(), "C_Campaign_ID=" + GetC_Campaign_ID(), Get_TrxName());

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
