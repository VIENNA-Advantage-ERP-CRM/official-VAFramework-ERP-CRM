/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_AD_CtxArea
 * Chronological Development
 * Veena Pandey     31-Aug-09
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
    public class MCtxArea : X_AD_CtxArea
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_CtxArea_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MCtxArea(Context ctx, int AD_CtxArea_ID, Trx trxName)
            : base(ctx, AD_CtxArea_ID, trxName)
        {
            if (AD_CtxArea_ID == 0)
            {
                SetEntityType(ENTITYTYPE_UserMaintained); // U
                SetIsSOTrx(false);
            }
        }


        public MCtxArea(Ctx ctx, int AD_CtxArea_ID, Trx trxName)
            : base(ctx, AD_CtxArea_ID, trxName)
        {
            if (AD_CtxArea_ID == 0)
            {
                SetEntityType(ENTITYTYPE_UserMaintained); // U
                SetIsSOTrx(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MCtxArea(Context ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <param name="trxName">transaction</param>
        public MCtxArea(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MCtxArea[")
                .Append(Get_ID()).Append("-").Append(GetName()).Append("]");
            return sb.ToString();
        }
    }
}
