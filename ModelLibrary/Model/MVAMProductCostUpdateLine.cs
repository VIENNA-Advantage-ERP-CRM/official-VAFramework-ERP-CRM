using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Logging;
//using VAdvantage.VOS;


namespace VAdvantage.Model
{
    public class MVAMVAMProductCostUpdateLine : X_VAM_ProductCostUpdateLine
    {
        #region Private Variables
        //Logger for class MVAMVAMProductCostUpdate 
       
        //private static long serialVersionUID = 1L;
        #endregion
        /**************************************************************************
	 *  Default Constructor
	 *  @param ctx context
	 *  @param  VAB_Order_ID    order to load, (0 create new order)
	 *  @param trx p_trx name
	 */
        public MVAMVAMProductCostUpdateLine(Ctx ctx, int VAM_ProductCostUpdateLine_ID, Trx trx)
            : base(ctx, VAM_ProductCostUpdateLine_ID, trx)
        {

        }


        /**
         *  Load Constructor
         *  @param ctx context
         *  @param rs result set record
         *  @param trx transaction
         */
        public MVAMVAMProductCostUpdateLine(Ctx ctx, DataRow dr, Trx trx)
            : base(ctx, dr, trx)
        {

        }

        public MVAMVAMProductCostUpdateLine(Ctx ctx, MVAMVAMProductCostUpdate costupdate, Trx trx)
            : base(ctx, 0, trx)
        {
            SetVAM_ProductCostUpdate_ID(costupdate.GetVAM_ProductCostUpdate_ID());
        }

    }

}
