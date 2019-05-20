/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_AD_Reference
 * Chronological Development
 * Veena Pandey     29-Aug-09
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
    public class MReference : X_AD_Reference
    {
        // Cache
        private static CCache<int, MReference> _cache = new CCache<int, MReference>("AD_Reference", 20);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Reference_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MReference(Context ctx, int AD_Reference_ID, Trx trxName)
            : base(ctx, AD_Reference_ID, trxName)
        {
        }

        public MReference(Ctx ctx, int AD_Reference_ID, Trx trxName)
            : base(ctx, AD_Reference_ID, trxName)
        {
        }
        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MReference(Context ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        public MReference(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Get Reference from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Reference_ID">id</param>
        /// <returns>MReference</returns>
        public static MReference Get(Ctx ctx, int AD_Reference_ID)
        {
            int key = AD_Reference_ID;
            MReference retValue = _cache[key];
            if (retValue == null)
                return new MReference(ctx, AD_Reference_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }


        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MReference[")
                .Append(Get_ID()).Append("-").Append(GetName()).Append("]");
            return sb.ToString();
        }
    }
}
