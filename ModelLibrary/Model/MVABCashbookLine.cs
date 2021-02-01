using System;
using System.Net;
using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Ink;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Shapes;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MVABCashbookLine : X_VAB_CashbookLine
    {
        #region variables
        
        // Parent				
        private MVABCashBook _parent = null;
        
        #endregion

        /* Standard Constructor
         * @param ctx context
	     *	@param VAB_CashbookLine_ID id
	     *	@param trxName transaction
	    */
        public MVABCashbookLine(Ctx ctx, int VAB_CashbookLine_ID, Trx trxName)
            : base(ctx, VAB_CashbookLine_ID, trxName)
        {
            if (VAB_CashbookLine_ID == 0)
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
        public MVABCashbookLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /**
         * 	Parent Cosntructor
         *	@param cashbook parent
         */
        public MVABCashbookLine(MVABCashBook cashbook)
            : this(cashbook.GetCtx(), 0, cashbook.Get_TrxName())
        {
            SetClientOrg(cashbook);
            SetVAB_CashBook_ID(cashbook.GetVAB_CashBook_ID());
            _parent = cashbook;
        }
    }
}
