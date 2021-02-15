using System;
using System.Net;
using System.Windows;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MVABGenFeatureSearch : X_VAB_GenFeatureSearch
    {
        public MVABGenFeatureSearch(Ctx ctx, int VAB_GenFeatureSearch_ID, Trx trxName)
            : base(ctx, VAB_GenFeatureSearch_ID, trxName)
        {


        }

         /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public MVABGenFeatureSearch(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }


    }
}
