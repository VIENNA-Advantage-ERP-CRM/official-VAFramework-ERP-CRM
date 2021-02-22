/********************************************************
 * ModuleName     : 
 * Purpose        : 
 * Class Used     : X_VAM_Prod_StockSummary
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
    class MVAMProdStockSummary : X_VAM_Prod_StockSummary
    {

         /**
  * 	Standard Constructor
  *	@param ctx context
  *	@param VAM_Inv_TrxSummary_ID id
  *	@param trxName transaction
  */
        public MVAMProdStockSummary(Ctx ctx, int VAM_Prod_StockSummary_ID, Trx trxName)
            : base(ctx, VAM_Prod_StockSummary_ID, trxName)
        {
            
        }

        /**
	 * 	Load Constructor
	 *	@param ctx context
	 *	@param dr result set
	 *	@param trxName transaction
	 */
        public MVAMProdStockSummary(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /**
        * 	Detail Constructor
        *	@param ctx context
        *	@param VAF_Org_ID org
        * 	@param VAM_Locator_ID locator
        * 	@param VAM_Product_ID product
        * 	@param VAM_PFeature_SetInstance_ID attribute
        * 	@param Opening Stock
         * 	@param Opening Stock
        * 	@param MovementDate optional date
        *	@param trxName transaction
        */
        public MVAMProdStockSummary(Ctx ctx, int VAF_Org_ID, int VAM_Product_ID, Decimal OpeningStock, Decimal ClosingStock, DateTime? MovementFromDate, Trx trxName)
            : base(ctx, 0, trxName)
        {
            SetVAF_Org_ID(VAF_Org_ID);
            if (VAM_Product_ID == 0)
                throw new ArgumentException("No Product");
            SetVAM_Product_ID(VAM_Product_ID);
            SetQtyOpenStockOrg(OpeningStock);
            SetQtyCloseStockOrg(ClosingStock);
            if (MovementFromDate == null)
                SetMovementFromDate(DateTime.Now);
            else
                SetMovementFromDate(MovementFromDate);
        }
    }
}
