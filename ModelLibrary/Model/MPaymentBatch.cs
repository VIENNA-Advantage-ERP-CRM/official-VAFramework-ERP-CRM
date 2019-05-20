/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_PaymentBatch
 * Chronological Development
 * Veena Pandey     24-June-2009
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
    public class MPaymentBatch : X_C_PaymentBatch
    {
       /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_PaymentBatch_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MPaymentBatch(Ctx ctx, int C_PaymentBatch_ID, Trx trxName)
            : base(ctx, C_PaymentBatch_ID, trxName)
        {
            if (C_PaymentBatch_ID == 0)
            {
                //	setName (null);
                SetProcessed(false);
                SetProcessing(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MPaymentBatch(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// New Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="name">name</param>
        /// <param name="trxName">transaction</param>
        public MPaymentBatch(Ctx ctx, String name, Trx trxName)
            : this(ctx, 0, trxName)
        {
            SetName(name);
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="ps">Pay Selection</param>
        public MPaymentBatch(MPaySelection ps)
            : this(ps.GetCtx(), 0, ps.Get_TrxName())
        {
            SetClientOrg(ps);
            SetName(ps.GetName());
        }

        /// <summary>
        /// Get Payment Batch for PaySelection
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_PaySelection_ID">id</param>
        /// <param name="trxName">transaction</param>
        /// <returns>payment batch</returns>
        public static MPaymentBatch GetForPaySelection(Ctx ctx, int C_PaySelection_ID, Trx trxName)
        {
            MPaySelection ps = new MPaySelection(ctx, C_PaySelection_ID, trxName);
            MPaymentBatch retValue = new MPaymentBatch(ps);
            return retValue;
        }
    }
}
