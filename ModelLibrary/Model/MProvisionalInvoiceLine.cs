using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MProvisionalInvoiceLine : X_C_ProvisionalInvoiceLine
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="C_ProvisionalInvoiceLine_ID">ProvisionalInvoiceLine</param>
        /// <param name="trxName">Transaction</param>
        /// <writer>209</writer>
        public MProvisionalInvoiceLine(Ctx ctx, int C_ProvisionalInvoiceLine_ID, Trx trxName) :
        base(ctx, C_ProvisionalInvoiceLine_ID, null)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="dr">DataRow</param>
        /// <param name="trxName">Transaction</param>
        /// <writer>209</writer>
        public MProvisionalInvoiceLine(Ctx ctx, DataRow dr, Trx trxName) :
        base(ctx, dr, trxName)
        {

        }
    }
}
