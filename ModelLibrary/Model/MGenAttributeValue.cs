using System;
using System.Net;
using System.Windows;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MGenAttributeValue: X_C_GenAttributeValue
    {
        public MGenAttributeValue(Ctx ctx, int C_GenAttributeValue_ID, Trx trxName)
            : base(ctx, C_GenAttributeValue_ID, trxName)
        {


        }

         /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public MGenAttributeValue(Ctx ctx, DataRow idr, Trx trxName)
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
