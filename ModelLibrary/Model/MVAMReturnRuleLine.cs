/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAM_ReturnRuleLine
 * Chronological Development
 * Veena Pandey     19-June-2009
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
    public class MVAMReturnRuleLine : X_VAM_ReturnRuleLine
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_ReturnRuleLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAMReturnRuleLine(Ctx ctx, int VAM_ReturnRuleLine_ID, Trx trxName)
            : base(ctx, VAM_ReturnRuleLine_ID, trxName)
        {
        }

        /// <summary>
        /// Load Construor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MVAMReturnRuleLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
    }
}