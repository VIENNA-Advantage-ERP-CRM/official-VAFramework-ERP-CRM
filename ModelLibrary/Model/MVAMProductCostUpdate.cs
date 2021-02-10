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
using VAdvantage.VOS;
//using VAdvantage.VOS;
//using VAdvantage.VOS;

namespace VAdvantage.Model
{
    public class MVAMProductCostUpdate : X_VAM_ProductCostUpdate
    {
        #region Private Variables
        //Logger for class MVAMProductCostUpdate 
       // private static long serialVersionUID = 1L;
        #endregion

        /**************************************************************************
	 *  Default Constructor
	 *  @param ctx context
	 *  @param  VAB_Order_ID    order to load, (0 create new order)
	 *  @param trx p_trx name
	 */
        public MVAMProductCostUpdate(Ctx ctx, int VAM_ProductCostUpdate_ID, Trx trx)
            : base(ctx, VAM_ProductCostUpdate_ID, trx)
        {

        }


        /**
         *  Load Constructor
         *  @param ctx context
         *  @param rs result set record
         *  @param trx transaction
         */
        public MVAMProductCostUpdate(Ctx ctx, DataRow dr, Trx trx)
            : base(ctx, dr, trx)
        {

        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            if (GetVAF_Org_ID() != 0)
            {
                SetVAF_Org_ID(0);
            }
            SetProcessed(true);
            SetDocStatus(DocActionConstants.STATUS_Completed);
            SetDocAction(DocActionConstants.ACTION_Close);
            return true;
        }

        public MVAMProductCostUpdateLine[] GetLines()
        {
            List<MVAMProductCostUpdateLine> list = new List<MVAMProductCostUpdateLine>();
            StringBuilder sql = new StringBuilder("SELECT * FROM VAM_ProductCostUpdateLine WHERE VAM_ProductCostUpdate_ID=" + this.GetVAM_ProductCostUpdate_ID());
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql.ToString(), null, Get_Trx());
                DataTable dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    MVAMProductCostUpdateLine ol = new MVAMProductCostUpdateLine(GetCtx(), dt.Rows[i], Get_Trx());
                    list.Add(ol);
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql.ToString(), e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            //
            MVAMProductCostUpdateLine[] lines = new MVAMProductCostUpdateLine[list.Count];
            lines = list.ToArray();
            return lines;
        }

    }

}
