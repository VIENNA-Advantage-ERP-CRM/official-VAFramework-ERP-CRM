using System;
using System.Net;
using System.Windows;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MGenAttributeSearch : X_C_GenAttributeSearch
    {
        public MGenAttributeSearch(Ctx ctx, int C_GenAttributeSearch_ID, Trx trxName)
            : base(ctx, C_GenAttributeSearch_ID, trxName)
        {


        }

         /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public MGenAttributeSearch(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }


    }
}
