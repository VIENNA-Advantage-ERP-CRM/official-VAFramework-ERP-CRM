/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_OrderLineHistory
 * Chronological Development
 * Bharat Singla   22-Aug-2017
 ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Common;
using VAdvantage.Utility;
using System.Data;
using System.Windows.Forms;
using VAdvantage.SqlExec;
using VAdvantage.DataBase;
using VAdvantage.Login;
using VAdvantage.Model;
using VAdvantage.WF;
//using VAdvantage.Grid;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MOrderlineHistory : X_C_OrderlineHistory
    {
        public MOrderlineHistory(Ctx ctx, int C_OrderlineHistory_ID, Trx trxName)
            : base(ctx, C_OrderlineHistory_ID, trxName)
        {

        }

        public MOrderlineHistory(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }


        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">newRecord</param>
        /// <returns>true if it can be saved</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	Get Line No
            if (GetLine() == 0)
            {
                String sql = "SELECT COALESCE(MAX(Line),0)+10 FROM C_OrderlineHistory WHERE C_OrderLine_ID=" + GetC_OrderLine_ID();
                int ii = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql, null, Get_TrxName()));
                SetLine(ii);
            }
            return true;
        }
    }
}
