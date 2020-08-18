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
  public  class MTaxPostal : X_C_TaxPostal
    {
        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param C_TaxPostal_ID id
         *	@param trxName transaction
         */
        public MTaxPostal(Ctx ctx, int C_TaxPostal_ID, Trx trxName):
            base(ctx, C_TaxPostal_ID, trxName)
        {
            //super(ctx, C_TaxPostal_ID, trxName);
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
