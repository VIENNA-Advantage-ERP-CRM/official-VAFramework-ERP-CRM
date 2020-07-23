/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MTaxDeclarationAcct
 * Purpose        : Tax Tax Declaration Accounting Model
 * Class Used     : X_C_TaxDeclarationAcct
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
    public class MTaxDeclarationAcct : X_C_TaxDeclarationAcct
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="C_TaxDeclarationAcct_ID">id</param>
        /// <param name="trxName">trx</param>
        public MTaxDeclarationAcct(Context ctx, int C_TaxDeclarationAcct_ID, Trx trxName):base(ctx, C_TaxDeclarationAcct_ID, trxName)
        {
           // super(ctx, C_TaxDeclarationAcct_ID, trxName);
        }	//	MTaxDeclarationAcct

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">trx</param>
        public MTaxDeclarationAcct(Context ctx, DataRow dr, Trx trxName): base(ctx, dr, trxName)
        {
           // super(ctx, rs, trxName);
        }	//	MTaxDeclarationAcct

        /**
         * 	Parent Constructor
         *	@param parent parent
         *	@param fact fact
         */
        public MTaxDeclarationAcct(MTaxDeclaration parent, MFactAcct fact):base(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            //super(parent.getCtx(), 0, parent.get_TrxName());
            SetClientOrg(fact);
            SetC_TaxDeclaration_ID(parent.GetC_TaxDeclaration_ID());
            //
            SetFact_Acct_ID(fact.GetFact_Acct_ID());
            SetC_AcctSchema_ID(fact.GetC_AcctSchema_ID());
        }	//	MTaxDeclarationAcct

    }	//	MTaxDeclarationAcct

}
