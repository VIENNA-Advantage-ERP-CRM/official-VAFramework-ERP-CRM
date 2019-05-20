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
    public class MCashbookLine : X_C_CashbookLine
    {
        #region variables
        
        // Parent				
        private MCashBook _parent = null;
        
        #endregion

        /* Standard Constructor
         * @param ctx context
	     *	@param C_CashbookLine_ID id
	     *	@param trxName transaction
	    */
        public MCashbookLine(Ctx ctx, int C_CashbookLine_ID, Trx trxName)
            : base(ctx, C_CashbookLine_ID, trxName)
        {
            if (C_CashbookLine_ID == 0)
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
        public MCashbookLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /**
         * 	Parent Cosntructor
         *	@param cashbook parent
         */
        public MCashbookLine(MCashBook cashbook)
            : this(cashbook.GetCtx(), 0, cashbook.Get_TrxName())
        {
            SetClientOrg(cashbook);
            SetC_CashBook_ID(cashbook.GetC_CashBook_ID());
            _parent = cashbook;
        }
    }
}
