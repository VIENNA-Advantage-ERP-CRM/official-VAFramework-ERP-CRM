using System;
using System.Net;
using System.Windows;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MGenAttributeUse: X_VAB_GenFeatureUse
    {
        public MGenAttributeUse(Ctx ctx, int VAB_GenFeatureValue_ID, Trx trxName)
            : base(ctx, VAB_GenFeatureValue_ID, trxName)
        {

        }

         /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public MGenAttributeUse(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }


    }
    
}
