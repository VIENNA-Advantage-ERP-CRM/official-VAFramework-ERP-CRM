/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MSalesRegion
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     11-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
//////using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
namespace VAdvantage.Model
{
    public class MVABSalesRegionState : X_VAB_SalesRegionState
    {
        //Cache						
        private static CCache<int, MVABSalesRegionState> s_cache = new CCache<int, MVABSalesRegionState>("VAB_SalesRegionState", 10);

        /// <summary>
        /// Get SalesRegion from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_SalesRegionState_ID">id</param>
        /// <returns>MSalesRegion</returns>
        public static MVABSalesRegionState Get(Ctx ctx, int VAB_SalesRegionState_ID)
        {
            int key = VAB_SalesRegionState_ID;
            MVABSalesRegionState retValue = (MVABSalesRegionState)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVABSalesRegionState(ctx, VAB_SalesRegionState_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAB_SalesRegionState_ID"></param>
        /// <param name="trxName">transaction</param>
        public MVABSalesRegionState(Ctx ctx, int VAB_SalesRegionState_ID, Trx trxName)
            : base(ctx, VAB_SalesRegionState_ID, trxName)
        {
            if (VAB_SalesRegionState_ID == 0)
            {
                SetIsDefault(false);
                SetIsSummary(false);
            }
        }

        /// <summary>
        /// 	Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName">transaction</param>
        public MVABSalesRegionState(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetVAF_Org_ID() != 0)
                SetVAF_Org_ID(0);
            return true;
        }

        /// <summary>
        /// After Save.
        /// Insert
        /// - create tree
        /// </summary>
        /// <param name="newRecord">insert</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return success;
            //	Value/Name change
            if (!newRecord && (Is_ValueChanged("Value") || Is_ValueChanged("Name")))
                MVABAccount.UpdateValueDescription(GetCtx(), "VAB_SalesRegionState_ID=" + GetVAB_SalesRegionState_ID(), Get_TrxName());
            return true;
        }
    }
}
