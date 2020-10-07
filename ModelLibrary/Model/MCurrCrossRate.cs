using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Collections;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Windows.Forms;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    class MCurrCrossRate : X_C_CurrCrossRate
    {
        /// <summary>
        ///Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_CurrCrossRate_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MCurrCrossRate(Ctx ctx, int C_CurrCrossRate_ID, Trx trxName)
            : base(ctx, C_CurrCrossRate_ID, trxName)
        {
            
        }

        /// <summary>
        ///	Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MCurrCrossRate(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
           
        }
        /// <summary>
        /// Before Save logic
        /// </summary>
        /// <param name="newRecord">new Record</param>
        /// <returns>If OK return true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	Common - From is the same
            if (GetC_Currency_ID() == GetC_Currency_From_ID())
            {
                log.SaveError("VIS_ComFromCurr","");
                return false;
            }
            //	Common - To is the same
            if (GetC_Currency_ID() == GetC_Currency_To_ID())
            {
                log.SaveError("VIS_ComToCurr","");
                return false;
            }
            //	From - To is the same
            if (GetC_Currency_From_ID() == GetC_Currency_To_ID())
            {
                log.SaveError("VIS_FromToCurr","");
                return false;
            }
            return true;
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            return true;
        }
    }
}
