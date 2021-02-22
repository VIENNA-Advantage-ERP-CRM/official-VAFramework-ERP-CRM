/********************************************************
 * ModuleName     : 
 * Purpose        : 
 * Class Used     : X_VAM_Inv_TrxSummary
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
    class MVAMInvTrxSummary : X_VAM_Inv_TrxSummary
    {
        /**
  * 	Standard Constructor
  *	@param ctx context
  *	@param VAM_Inv_TrxSummary_ID id
  *	@param trxName transaction
  */
        public MVAMInvTrxSummary(Ctx ctx, int VAM_Inv_TrxSummary_ID, Trx trxName)
            : base(ctx, VAM_Inv_TrxSummary_ID, trxName)
        {
            
        }

        /**
	 * 	Load Constructor
	 *	@param ctx context
	 *	@param dr result set
	 *	@param trxName transaction
	 */
        public MVAMInvTrxSummary(Ctx ctx, DataRow dr, Trx trxName)
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
        public MVAMInvTrxSummary(Ctx ctx, int VAF_Org_ID, int VAM_Locator_ID, int VAM_Product_ID, int VAM_PFeature_SetInstance_ID,
            Decimal OpeningStock, Decimal ClosingStock, DateTime? MovementDate, Trx trxName)
            : base(ctx, 0, trxName)
        {

            SetVAF_Org_ID(VAF_Org_ID);
            if (VAM_Locator_ID == 0)
                throw new ArgumentException("No Locator");
            SetVAM_Locator_ID(VAM_Locator_ID);
            if (VAM_Product_ID == 0)
                throw new ArgumentException("No Product");
            SetVAM_Product_ID(VAM_Product_ID);
            SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            SetOpeningStock(OpeningStock);
            SetClosingStock(ClosingStock);
            if (MovementDate == null)
                SetMovementDate(DateTime.Now);
            else
                SetMovementDate(MovementDate);
        }
    }
}
