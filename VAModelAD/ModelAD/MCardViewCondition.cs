using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using ViennaAdvantage.Model;
namespace VAdvantage.Model
{
    public class MCardViewCondition : X_AD_CardView_Condition
    {
        public MCardViewCondition(Ctx ctx, int AD_CardViewColumn_ID, Trx trxName)
            : base(ctx, AD_CardViewColumn_ID, trxName)
        {
        }

        /// <summary>
        ///Load Constructor 
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set(data row)</param>
        /// <param name="trxName">transaction</param>
        public MCardViewCondition(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
    }
}
