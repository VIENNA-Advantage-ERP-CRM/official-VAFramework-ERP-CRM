/********************************************************
 * ModuleName     : 
 * Purpose        : 
 * Class Used     : X_M_ProductStockSummary
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
    class MProductStockSummary : X_M_ProductStockSummary
    {

         /**
  * 	Standard Constructor
  *	@param ctx context
  *	@param M_TransactionSummary_ID id
  *	@param trxName transaction
  */
        public MProductStockSummary(Ctx ctx, int M_ProductStockSummary_ID, Trx trxName)
            : base(ctx, M_ProductStockSummary_ID, trxName)
        {
            
        }

        /**
	 * 	Load Constructor
	 *	@param ctx context
	 *	@param dr result set
	 *	@param trxName transaction
	 */
        public MProductStockSummary(Ctx ctx, DataRow dr, Trx trxName)
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
        public MProductStockSummary(Ctx ctx, int AD_Org_ID, int M_Product_ID, Decimal OpeningStock, Decimal ClosingStock, DateTime? MovementFromDate, Trx trxName)
            : base(ctx, 0, trxName)
        {
            SetAD_Org_ID(AD_Org_ID);
            if (M_Product_ID == 0)
                throw new ArgumentException("No Product");
            SetM_Product_ID(M_Product_ID);
            SetQtyOpenStockOrg(OpeningStock);
            SetQtyCloseStockOrg(ClosingStock);
            if (MovementFromDate == null)
                SetMovementFromDate(DateTime.Now);
            else
                SetMovementFromDate(MovementFromDate);
        }
    }
}
