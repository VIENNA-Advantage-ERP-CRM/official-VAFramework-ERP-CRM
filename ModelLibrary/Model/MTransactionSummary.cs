/********************************************************
 * ModuleName     : 
 * Purpose        : 
 * Class Used     : X_M_TransactionSummary
 * Chronological    Development
 * Bharat     28-Oct-2016
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Data;

using VAdvantage.Model;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    class MTransactionSummary : X_M_TransactionSummary
    {
        /**
  * 	Standard Constructor
  *	@param ctx context
  *	@param M_TransactionSummary_ID id
  *	@param trxName transaction
  */
        public MTransactionSummary(Ctx ctx, int M_TransactionSummary_ID, Trx trxName)
            : base(ctx, M_TransactionSummary_ID, trxName)
        {
            
        }

        /**
	 * 	Load Constructor
	 *	@param ctx context
	 *	@param dr result set
	 *	@param trxName transaction
	 */
        public MTransactionSummary(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /**
        * 	Detail Constructor
        *	@param ctx context
        *	@param AD_Org_ID org
        * 	@param M_Locator_ID locator
        * 	@param M_Product_ID product
        * 	@param M_AttributeSetInstance_ID attribute
        * 	@param Opening Stock
         * 	@param Opening Stock
        * 	@param MovementDate optional date
        *	@param trxName transaction
        */
        public MTransactionSummary(Ctx ctx, int AD_Org_ID, int M_Locator_ID, int M_Product_ID, int M_AttributeSetInstance_ID,
            Decimal OpeningStock, Decimal ClosingStock, DateTime? MovementDate, Trx trxName)
            : base(ctx, 0, trxName)
        {

            SetAD_Org_ID(AD_Org_ID);
            if (M_Locator_ID == 0)
                throw new ArgumentException("No Locator");
            SetM_Locator_ID(M_Locator_ID);
            if (M_Product_ID == 0)
                throw new ArgumentException("No Product");
            SetM_Product_ID(M_Product_ID);
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            SetOpeningStock(OpeningStock);
            SetClosingStock(ClosingStock);
            if (MovementDate == null)
                SetMovementDate(DateTime.Now);
            else
                SetMovementDate(MovementDate);
        }
    }
}
