/********************************************************
 * Module Name    :    VA Framework
 * Class Name     :    MProvisionalInvoicetax
 * Purpose        :    Provisional Invoice Tax Model
 * Employee Code  :    209
 * Date           :    14-July-2021
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VAdvanatge.Model
{
    public class MProvisionalInvoiceTax: X_C_ProvisionalInvoiceTax
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="C_ProvisionalInvoiceTax_ID">ProvisionalInvoiceTax</param>
        /// <param name="trxName">Transaction</param>
        /// <writer>209</writer>
        public MProvisionalInvoiceTax(Ctx ctx, int C_ProvisionalInvoiceTax_ID, Trx trxName) :
         base(ctx, C_ProvisionalInvoiceTax_ID, null)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="dr">DataRow</param>
        /// <param name="trxName">Transaction</param>
        /// <writer>209</writer>
        public MProvisionalInvoiceTax(Ctx ctx, DataRow dr, Trx trxName) :
           base(ctx, dr, trxName)
        {

        }
    }
}
