using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MUserMailConfigration : X_AD_UserMailConfigration
    {

        public MUserMailConfigration(Ctx ctx, int AD_UserMailConfigration_ID, Trx trxName)
            : base(ctx, AD_UserMailConfigration_ID, trxName)
        {
            if (AD_UserMailConfigration_ID == 0)
            {
               
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
         public MUserMailConfigration(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="idr">idatareader</param>
        /// <param name="trxName">trasaction</param>
         public MUserMailConfigration(Ctx ctx, IDataReader idr, Trx trxName)
             : base(ctx, idr, trxName)
         {
         }
    }
}
