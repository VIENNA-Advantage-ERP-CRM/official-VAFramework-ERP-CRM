using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.DataBase;
namespace VAdvantage.Model
{
  public  class MTaxPostal : X_VAB_TaxZIP
    {
        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param VAB_TaxZIP_ID id
         *	@param trxName transaction
         */
        public MTaxPostal(Ctx ctx, int VAB_TaxZIP_ID, Trx trxName):
            base(ctx, VAB_TaxZIP_ID, trxName)
        {
            //super(ctx, VAB_TaxZIP_ID, trxName);
        }	//	MTaxPostal

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param rs result set
         *	@param trxName transaction
         */
        public MTaxPostal(Ctx ctx, DataRow dr, Trx trxName):
            base(ctx,dr,trxName)
        {
            
        }	//	MTaxPostal
    }
}
