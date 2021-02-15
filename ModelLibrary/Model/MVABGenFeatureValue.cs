using System;
using System.Net;
using System.Windows;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MVABGenFeatureValue: X_VAB_GenFeatureValue
    {
        public MVABGenFeatureValue(Ctx ctx, int VAB_GenFeatureValue_ID, Trx trxName)
            : base(ctx, VAB_GenFeatureValue_ID, trxName)
        {


        }

         /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public MVABGenFeatureValue(Ctx ctx, DataRow idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            return GetName();
        }
    }
}
