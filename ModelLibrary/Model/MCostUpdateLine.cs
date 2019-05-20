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
    public class MCostUpdateLine : X_M_CostUpdateLine
    {
        #region Private Variables
        //Logger for class MCostUpdate 
       
        //private static long serialVersionUID = 1L;
        #endregion
        /**************************************************************************
	 *  Default Constructor
	 *  @param ctx context
	 *  @param  C_Order_ID    order to load, (0 create new order)
	 *  @param trx p_trx name
	 */
        public MCostUpdateLine(Ctx ctx, int M_CostUpdateline_ID, Trx trx)
            : base(ctx, M_CostUpdateline_ID, trx)
        {

        }


        /**
         *  Load Constructor
         *  @param ctx context
         *  @param rs result set record
         *  @param trx transaction
         */
        public MCostUpdateLine(Ctx ctx, DataRow dr, Trx trx)
            : base(ctx, dr, trx)
        {

        }

        public MCostUpdateLine(Ctx ctx, MCostUpdate costupdate, Trx trx)
            : base(ctx, 0, trx)
        {
            SetM_CostUpdate_ID(costupdate.GetM_CostUpdate_ID());
        }

    }

}
