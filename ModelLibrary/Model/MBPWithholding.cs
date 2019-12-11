/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MBPWithholding
 * Purpose        : 
 * Chronological    Development
 * Amit     19-Mar-2016
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using java.math;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MBPWithholding : X_C_BP_Withholding
    {

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_BP_Withholding_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MBPWithholding(Ctx ctx, int C_BP_Withholding_ID, Trx trxName)
            : base(ctx, C_BP_Withholding_ID, trxName)
        {

        }

         /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MBPWithholding(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        protected override bool BeforeSave(bool newRecord)
        {
            return true;
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {

            return true;
        }
    }
}
