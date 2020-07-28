/********************************************************
 * Module Name    : Workflow
 * Purpose        : 
 * Class Used     : X_M_LotCtl
 * Chronological Development
 * Veena Pandey     16-June-2009
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
    public class MLotCtl : X_M_LotCtl
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_LotCtl_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MLotCtl(Ctx ctx, int M_LotCtl_ID, Trx trxName)
            : base(ctx, M_LotCtl_ID, trxName)
        {
            if (M_LotCtl_ID == 0)
            {
                //	setM_LotCtl_ID (0);
                SetStartNo(1);
                SetCurrentNext(1);
                SetIncrementNo(1);
                //	setName (null);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MLotCtl(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Create new Lot.
        /// Increments Current Next and Commits
        /// </summary>
        /// <param name="M_Product_ID">product</param>
        /// <returns>saved Lot</returns>
        public MLot CreateLot(int M_Product_ID)
        {
            StringBuilder name = new StringBuilder();
            if (GetPrefix() != null)
                name.Append(GetPrefix());
            int no = GetCurrentNext();
            name.Append(no);
            if (GetSuffix() != null)
                name.Append(GetSuffix());
            //
            no += GetIncrementNo();
            SetCurrentNext(no);
            Save();
            //
            MLot retValue = new MLot(this, M_Product_ID, name.ToString());
            retValue.Save();
            return retValue;
        }
    }
}
