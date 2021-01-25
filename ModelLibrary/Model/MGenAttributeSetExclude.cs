using System;
using System.Net;
using System.Windows;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MGenAttributeSetExclude: X_VAB_GenFeatureSetExclude
    {
        public MGenAttributeSetExclude(Ctx ctx, int VAB_GenFeatureSetExclude_ID, Trx trxName)
            : base(ctx, VAB_GenFeatureSetExclude_ID, trxName)
        {


        }

         /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public MGenAttributeSetExclude(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }


    }
}
