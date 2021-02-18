/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MTaxDeclarationAcct
 * Purpose        : Tax Tax Declaration Accounting Model
 * Class Used     : X_VAB_TaxComputationAcct
 * Chronological    Development
 * Deepak           20-Nov-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MVABTaxComputationAcct : X_VAB_TaxComputationAcct
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="VAB_TaxComputationAcct_ID">id</param>
        /// <param name="trxName">trx</param>
        public MVABTaxComputationAcct(Context ctx, int VAB_TaxComputationAcct_ID, Trx trxName):base(ctx, VAB_TaxComputationAcct_ID, trxName)
        {
           // super(ctx, VAB_TaxComputationAcct_ID, trxName);
        }	//	MTaxDeclarationAcct

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">trx</param>
        public MVABTaxComputationAcct(Context ctx, DataRow dr, Trx trxName): base(ctx, dr, trxName)
        {
           // super(ctx, rs, trxName);
        }	//	MTaxDeclarationAcct

        /**
         * 	Parent Constructor
         *	@param parent parent
         *	@param fact fact
         */
        public MVABTaxComputationAcct(MVABTaxRateComputation parent, MActualAcctDetail fact):base(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            //super(parent.getCtx(), 0, parent.get_TrxName());
            SetClientOrg(fact);
            SetVAB_TaxRateComputation_ID(parent.GetVAB_TaxRateComputation_ID());
            //
            SetActual_Acct_Detail_ID(fact.GetActual_Acct_Detail_ID());
            SetVAB_AccountBook_ID(fact.GetVAB_AccountBook_ID());
        }	//	MTaxDeclarationAcct

    }	//	MTaxDeclarationAcct

}
