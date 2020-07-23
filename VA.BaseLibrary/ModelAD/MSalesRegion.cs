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
using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
namespace VAdvantage.Model
{
    public class MSalesRegion : X_C_SalesRegion
    {
        //Cache						
        private static CCache<int, MSalesRegion> s_cache = new CCache<int, MSalesRegion>("C_SalesRegion", 10);

        /// <summary>
        /// Get SalesRegion from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_SalesRegion_ID">id</param>
        /// <returns>MSalesRegion</returns>
        public static MSalesRegion Get(Ctx ctx, int C_SalesRegion_ID)
        {
            int key = C_SalesRegion_ID;
            MSalesRegion retValue = (MSalesRegion)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MSalesRegion(ctx, C_SalesRegion_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_SalesRegion_ID"></param>
        /// <param name="trxName">transaction</param>
        public MSalesRegion(Ctx ctx, int C_SalesRegion_ID, Trx trxName)
            : base(ctx, C_SalesRegion_ID, trxName)
        {
            if (C_SalesRegion_ID == 0)
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
        public MSalesRegion(Ctx ctx, DataRow rs, Trx trxName)
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
            if (GetAD_Org_ID() != 0)
                SetAD_Org_ID(0);
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
                MAccount.UpdateValueDescription(GetCtx(), "C_SalesRegion_ID=" + GetC_SalesRegion_ID(), Get_TrxName());
            return true;
        }
    }
}
