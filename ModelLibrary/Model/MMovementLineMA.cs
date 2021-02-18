/********************************************************
 * Module Name    : 
 * Purpose        : Movement Material Allocation
 * Class Used     : X_VAM_InvTrf_LineMP
 * Chronological Development
 * Veena         27-Oct-2009  
 ******************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    /// <summary>
    /// Movement Material Allocation
    /// </summary>
    public class MVAMInvTrfLineMA : X_VAM_InvTrf_LineMP
    {
        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMInvTrfLineMA).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_InvTrf_LineMP_ID">id (ignored)</param>
        /// <param name="trxName">transaction</param>
        public MVAMInvTrfLineMA(Ctx ctx, int VAM_InvTrf_LineMP_ID, Trx trxName)
            : base(ctx, VAM_InvTrf_LineMP_ID, trxName)
        {
            if (VAM_InvTrf_LineMP_ID != 0)
                throw new ArgumentException("Multi-Key");
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transation</param>
        public MVAMInvTrfLineMA(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="VAM_PFeature_SetInstance_ID">asi</param>
        /// <param name="movementQty">qty</param>
        public MVAMInvTrfLineMA(MVAMInvTrfLine parent, int VAM_PFeature_SetInstance_ID, Decimal movementQty)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetVAM_InvTrf_Line_ID(parent.GetVAM_InvTrf_Line_ID());
            //
            SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            SetMovementQty(movementQty);
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="VAM_PFeature_SetInstance_ID"></param>
        /// <param name="movementQty"></param>
        /// <param name="MMPloicyDate"></param>
        public MVAMInvTrfLineMA(MVAMInvTrfLine parent, int VAM_PFeature_SetInstance_ID, Decimal movementQty, DateTime? MMPloicyDate)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetVAM_InvTrf_Line_ID(parent.GetVAM_InvTrf_Line_ID());
            //
            SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            SetMovementQty(movementQty);
            if (MMPloicyDate == null)
            {

                MMPloicyDate = parent.GetParent().GetMovementDate();
            }
            SetMMPolicyDate(MMPloicyDate);
        }

        /// <summary>
        /// Is Used to Get or Create  Instance of MVAMInvTrfLineMA (Attribute)
        /// </summary>
        /// <param name="line"></param>
        /// <param name="VAM_PFeature_SetInstance_ID"></param>
        /// <param name="MovementQty"></param>
        /// <param name="DateMaterialPolicy"></param>
        /// <returns></returns>
        public static MVAMInvTrfLineMA GetOrCreate(MVAMInvTrfLine line, int VAM_PFeature_SetInstance_ID, Decimal MovementQty, DateTime? DateMaterialPolicy)
        {
            MVAMInvTrfLineMA retValue = null;
            String sql = "SELECT * FROM VAM_InvTrf_LineMP " +
                         @" WHERE  VAM_InvTrf_Line_ID = " + line.GetVAM_InvTrf_Line_ID() +
                         @" AND MMPolicyDate = " + GlobalVariable.TO_DATE(DateMaterialPolicy, true) + @" AND ";
            if (VAM_PFeature_SetInstance_ID == 0)
                sql += "(VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID + " OR VAM_PFeature_SetInstance_ID IS NULL)";
            else
                sql += "VAM_PFeature_SetInstance_ID=" + VAM_PFeature_SetInstance_ID;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, line.Get_Trx());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MVAMInvTrfLineMA(line.GetCtx(), dr, line.Get_Trx());
                }
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            if (retValue == null)
                retValue = new MVAMInvTrfLineMA(line, VAM_PFeature_SetInstance_ID, MovementQty, DateMaterialPolicy);
            else
                retValue.SetMovementQty(Decimal.Add(retValue.GetMovementQty(), MovementQty));
            return retValue;
        }

        /// <summary>
        /// Get Material Allocations for Line
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_InvTrf_Line_ID">id</param>
        /// <param name="trxName">transaction</param>
        /// <returns>allocations</returns>
        public static MVAMInvTrfLineMA[] Get(Ctx ctx, int VAM_InvTrf_Line_ID, Trx trxName)
        {
            List<MVAMInvTrfLineMA> list = new List<MVAMInvTrfLineMA>();
            String sql = "SELECT * FROM VAM_InvTrf_LineMP WHERE VAM_InvTrf_Line_ID=@mlid";
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@mlid", VAM_InvTrf_Line_ID);

                DataSet ds = DataBase.DB.ExecuteDataset(sql, param, trxName);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MVAMInvTrfLineMA(ctx, dr, trxName));
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            MVAMInvTrfLineMA[] retValue = new MVAMInvTrfLineMA[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Delete all Material Allocation for Movement
        /// </summary>
        /// <param name="VAM_InventoryTransfer_ID">movement id</param>
        /// <param name="trxName">transaction</param>
        /// <returns>number of rows deleted or -1 for error</returns>
        public static int DeleteMovementMA(int VAM_InventoryTransfer_ID, Trx trxName)
        {
            String sql = "DELETE FROM VAM_InvTrf_LineMP ma WHERE EXISTS "
                + "(SELECT * FROM VAM_InvTrf_Line l WHERE l.VAM_InvTrf_Line_ID=ma.VAM_InvTrf_Line_ID"
                + " AND VAM_InventoryTransfer_ID=" + VAM_InventoryTransfer_ID + ")";
            return DataBase.DB.ExecuteQuery(sql, null, trxName);
        }

        /// <summary>
        /// Delete all Material Allocation for Movement Line
        /// </summary>
        /// <param name="VAM_InvTrf_Line_ID">movement line id</param>
        /// <param name="trxName">transaction</param>
        /// <returns>number of rows deleted or -1 for error</returns>
        public static int DeleteMovementLineMA(int VAM_InvTrf_Line_ID, Trx trxName)
        {
            String sql = "DELETE FROM VAM_InvTrf_LineMP ma WHERE EXISTS "
                + "(SELECT * FROM VAM_InvTrf_Line l WHERE l.VAM_InvTrf_Line_ID=ma.VAM_InvTrf_Line_ID"
                + " AND VAM_InvTrf_Line_ID=" + VAM_InvTrf_Line_ID + ")";
            return DataBase.DB.ExecuteQuery(sql, null, trxName);
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVAMInvTrfLineMA[");
            sb.Append("VAM_InvTrf_Line_ID=").Append(GetVAM_InvTrf_Line_ID())
                .Append(",VAM_PFeature_SetInstance_ID=").Append(GetVAM_PFeature_SetInstance_ID())
                .Append(", Qty=").Append(GetMovementQty())
                .Append("]");
            return sb.ToString();
        }
    }
}
