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

namespace VAdvantage.Model
{
    public class MCostUpdate : X_M_CostUpdate
    {
        #region Private Variables
        //Logger for class MCostUpdate 
       // private static long serialVersionUID = 1L;
        #endregion

        /**************************************************************************
	 *  Default Constructor
	 *  @param ctx context
	 *  @param  C_Order_ID    order to load, (0 create new order)
	 *  @param trx p_trx name
	 */
        public MCostUpdate(Ctx ctx, int M_CostUpdate_ID, Trx trx)
            : base(ctx, M_CostUpdate_ID, trx)
        {

        }


        /**
         *  Load Constructor
         *  @param ctx context
         *  @param rs result set record
         *  @param trx transaction
         */
        public MCostUpdate(Ctx ctx, DataRow dr, Trx trx)
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
            if (GetAD_Org_ID() != 0)
            {
                SetAD_Org_ID(0);
            }
            SetProcessed(true);
            SetDocStatus(DocActionConstants.STATUS_Completed);
            SetDocAction(DocActionConstants.ACTION_Close);
            return true;
        }

        public MCostUpdateLine[] GetLines()
        {
            List<MCostUpdateLine> list = new List<MCostUpdateLine>();
            StringBuilder sql = new StringBuilder("SELECT * FROM M_CostUpdateline WHERE M_CostUpdate_ID=" + this.GetM_CostUpdate_ID());
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql.ToString(), null, Get_Trx());
                DataTable dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    MCostUpdateLine ol = new MCostUpdateLine(GetCtx(), dt.Rows[i], Get_Trx());
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
            MCostUpdateLine[] lines = new MCostUpdateLine[list.Count];
            lines = list.ToArray();
            return lines;
        }

    }

}
