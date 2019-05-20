/********************************************************
 * Module Name    : 
 * Purpose        : Movement Material Allocation
 * Class Used     : X_M_MovementLineMA
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
    public class MMovementLineMA : X_M_MovementLineMA
    {
        /**	Logger	*/
	    private static VLogger _log	= VLogger.GetVLogger(typeof(MMovementLineMA).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_MovementLineMA_ID">id (ignored)</param>
        /// <param name="trxName">transaction</param>
	    public MMovementLineMA (Ctx ctx, int M_MovementLineMA_ID, Trx trxName)
            : base (ctx, M_MovementLineMA_ID, trxName)
	    {
            if (M_MovementLineMA_ID != 0)
                throw new ArgumentException("Multi-Key");
	    }

	    /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transation</param>
        public MMovementLineMA(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="M_AttributeSetInstance_ID">asi</param>
        /// <param name="movementQty">qty</param>
        public MMovementLineMA(MMovementLine parent, int M_AttributeSetInstance_ID, Decimal movementQty)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetM_MovementLine_ID(parent.GetM_MovementLine_ID());
            //
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            SetMovementQty(movementQty);
        }

        /// <summary>
        /// Get Material Allocations for Line
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_MovementLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        /// <returns>allocations</returns>
        public static MMovementLineMA[] Get(Ctx ctx, int M_MovementLine_ID, Trx trxName)
        {
            List<MMovementLineMA> list = new List<MMovementLineMA>();
            String sql = "SELECT * FROM M_MovementLineMA WHERE M_MovementLine_ID=@mlid";
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@mlid", M_MovementLine_ID);

                DataSet ds = DataBase.DB.ExecuteDataset(sql, param, trxName);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MMovementLineMA(ctx, dr, trxName));
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            MMovementLineMA[] retValue = new MMovementLineMA[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Delete all Material Allocation for Movement
        /// </summary>
        /// <param name="M_Movement_ID">movement id</param>
        /// <param name="trxName">transaction</param>
        /// <returns>number of rows deleted or -1 for error</returns>
        public static int DeleteMovementMA(int M_Movement_ID, Trx trxName)
        {
            String sql = "DELETE FROM M_MovementLineMA ma WHERE EXISTS "
                + "(SELECT * FROM M_MovementLine l WHERE l.M_MovementLine_ID=ma.M_MovementLine_ID"
                + " AND M_Movement_ID=" + M_Movement_ID + ")";
            return DataBase.DB.ExecuteQuery(sql, null, trxName);
        }

        /// <summary>
        /// Delete all Material Allocation for Movement Line
        /// </summary>
        /// <param name="M_MovementLine_ID">movement line id</param>
        /// <param name="trxName">transaction</param>
        /// <returns>number of rows deleted or -1 for error</returns>
        public static int DeleteMovementLineMA(int M_MovementLine_ID, Trx trxName)
        {
            String sql = "DELETE FROM M_MovementLineMA ma WHERE EXISTS "
                + "(SELECT * FROM M_MovementLine l WHERE l.M_MovementLine_ID=ma.M_MovementLine_ID"
                + " AND M_MovementLine_ID=" + M_MovementLine_ID + ")";
            return DataBase.DB.ExecuteQuery(sql, null, trxName);
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MMovementLineMA[");
            sb.Append("M_MovementLine_ID=").Append(GetM_MovementLine_ID())
                .Append(",M_AttributeSetInstance_ID=").Append(GetM_AttributeSetInstance_ID())
                .Append(", Qty=").Append(GetMovementQty())
                .Append("]");
            return sb.ToString();
        }
    }
}
