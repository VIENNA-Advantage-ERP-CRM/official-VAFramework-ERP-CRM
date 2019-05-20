/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MActivity
 * Purpose        : Activity model.
 * Class Used     : X_C_Activity class
 * Chronological    Development
 * Deepak           19-Nov-2009
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
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MActivity : X_C_Activity
    {

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Activity_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MActivity(Ctx ctx, int C_Activity_ID, Trx trxName):base(ctx, C_Activity_ID, trxName)
        {
            //super(ctx, C_Activity_ID, trxName);
        }	//	MActivity

        /// <summary>
        ///	Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">DataRow</param>
        /// <param name="trxName">transaction</param>
        public MActivity(Ctx ctx, DataRow dr, Trx trxName):base(ctx, dr, trxName)
        {
            //super(ctx, rs, trxName);
        }	//	MActivity


        /// <summary>
        ///	After Save.Insert - create tree
        /// </summary>
        /// <param name="newRecord">insert</param>
        /// <param name="success">save success</param>
        /// <returns>true if saved</returns>
        protected  override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            if (!success)
            {
                return success;
            }
            //	Value/Name change
            if (!newRecord && (Is_ValueChanged("Value") || Is_ValueChanged("Name")))
            {
                MAccount.UpdateValueDescription(GetCtx(), "C_Activity_ID=" + GetC_Activity_ID(), Get_TrxName());
            }
            return true;
        }	//	afterSave

    }	//	MActivity

}
