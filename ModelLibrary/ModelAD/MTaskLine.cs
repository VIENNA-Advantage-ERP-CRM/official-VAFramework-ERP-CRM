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
     public class MTaskLine: X_C_TaskLine
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_TaskLine_ID">id</param>
        /// <param name="trxName">trx</param>
        public MTaskLine(Ctx ctx, int C_TaskLine_ID, Trx trxName)
            : base(ctx, C_TaskLine_ID, trxName)
        {

        }   //	MTaskLine

        /// <summary>
        /// Load Cosntructor 
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">transaction</param>
        public MTaskLine(Ctx ctx, DataRow dr, Trx trxName) : base(ctx, dr, trxName)
        {

        }	//	MTaskLine
    }
}
