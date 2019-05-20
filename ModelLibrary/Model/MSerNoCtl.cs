/********************************************************
 * Module Name    : Workflow
 * Purpose        : 
 * Class Used     : X_M_SerNoCtl
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
  public  class MSerNoCtl : X_M_SerNoCtl
    {

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_SerNoCtl_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MSerNoCtl(Ctx ctx, int M_SerNoCtl_ID, Trx trxName)
            : base(ctx, M_SerNoCtl_ID, trxName)
        {
            if (M_SerNoCtl_ID == 0)
            {
                //	setM_SerNoCtl_ID (0);
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
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MSerNoCtl(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        /// Create new Lot.
        /// Increments Current Next and Commits
        /// </summary>
        /// <returns>saved Lot</returns>
        public String CreateSerNo()
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
            return name.ToString();
        }
    }
}
