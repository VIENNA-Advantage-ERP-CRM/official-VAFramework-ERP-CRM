/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAB_PaymentBatch
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
    public class MVABPaymentBatch : X_VAB_PaymentBatch
    {
       /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_PaymentBatch_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVABPaymentBatch(Ctx ctx, int VAB_PaymentBatch_ID, Trx trxName)
            : base(ctx, VAB_PaymentBatch_ID, trxName)
        {
            if (VAB_PaymentBatch_ID == 0)
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
        public MVABPaymentBatch(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// New Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="name">name</param>
        /// <param name="trxName">transaction</param>
        public MVABPaymentBatch(Ctx ctx, String name, Trx trxName)
            : this(ctx, 0, trxName)
        {
            SetName(name);
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="ps">Pay Selection</param>
        public MVABPaymentBatch(MVABPaymentOption ps)
            : this(ps.GetCtx(), 0, ps.Get_TrxName())
        {
            SetClientOrg(ps);
            SetName(ps.GetName());
        }

        /// <summary>
        /// Get Payment Batch for PaySelection
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_PaymentOption_ID">id</param>
        /// <param name="trxName">transaction</param>
        /// <returns>payment batch</returns>
        public static MVABPaymentBatch GetForPaySelection(Ctx ctx, int VAB_PaymentOption_ID, Trx trxName)
        {
            MVABPaymentOption ps = new MVABPaymentOption(ctx, VAB_PaymentOption_ID, trxName);
            MVABPaymentBatch retValue = new MVABPaymentBatch(ps);
            return retValue;
        }
    }
}
