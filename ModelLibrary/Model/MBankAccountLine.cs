using System;
using System.Net;
using System.Windows;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MBankAccountLine : X_C_BankAccountLine
    {
        #region variables
        
        // Parent				
        private MBankAccount _parent = null;
        
        #endregion

        /* Standard Constructor
         * @param ctx context
	     *	@param C_BankAccountLine_ID id
	     *	@param trxName transaction
	    */
        public MBankAccountLine(Ctx ctx, int C_BankAccountLine_ID, Trx trxName)
            : base(ctx, C_BankAccountLine_ID, trxName)
        {
            if (C_BankAccountLine_ID == 0)
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
        public MBankAccountLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /**
         * 	Parent Cosntructor
         *	@param bank account parent
         */
        public MBankAccountLine(MBankAccount bankAccount)
            : this(bankAccount.GetCtx(), 0, bankAccount.Get_TrxName())
        {
            SetClientOrg(bankAccount);
            SetC_BankAccount_ID(bankAccount.GetC_BankAccount_ID());
            _parent = bankAccount;
        }
    }
}
