using System;
using System.Net;
using System.Windows;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MVABBankAcctLine : X_VAB_Bank_AcctLine
    {
        #region variables
        
        // Parent				
        private MVABBankAcct _parent = null;
        
        #endregion

        /* Standard Constructor
         * @param ctx context
	     *	@param VAB_Bank_AcctLine_ID id
	     *	@param trxName transaction
	    */
        public MVABBankAcctLine(Ctx ctx, int VAB_Bank_AcctLine_ID, Trx trxName)
            : base(ctx, VAB_Bank_AcctLine_ID, trxName)
        {
            if (VAB_Bank_AcctLine_ID == 0)
            {
                SetStatementDifference(Env.ZERO);
            }
        }

        /**
         * 	Load Cosntructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MVABBankAcctLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /**
         * 	Parent Cosntructor
         *	@param bank account parent
         */
        public MVABBankAcctLine(MVABBankAcct bankAccount)
            : this(bankAccount.GetCtx(), 0, bankAccount.Get_TrxName())
        {
            SetClientOrg(bankAccount);
            SetVAB_Bank_Acct_ID(bankAccount.GetVAB_Bank_Acct_ID());
            _parent = bankAccount;
        }
    }
}
