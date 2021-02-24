using System;
using System.Net;
using System.Windows;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MGenAttributeUse : X_C_GenAttributeUse
    {
        public MGenAttributeUse(Ctx ctx, int C_GenAttributeValue_ID, Trx trxName)
            : base(ctx, C_GenAttributeValue_ID, trxName)
        {
            if (C_GenAttributeValue_ID != 0)
            {
                throw new Exception("Multi-Key");
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public MGenAttributeUse(Ctx ctx, DataRow idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }


    }

}