/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MInventoryLineMA
 * Purpose        : Inventory Material Allocation
 * Class Used     : X_M_InventoryLineMA
 * Chronological    Development
 * Raghunandan     22-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;


namespace VAdvantage.Model
{
    public class MInventoryLineMA : X_M_InventoryLineMA
    {

        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MInventoryLineMA).FullName);

        /**
         * 	Get Material Allocations for Line
         *	@param ctx context
         *	@param M_InventoryLine_ID line
         *	@param trxName trx
         *	@return allocations
         */
        public static MInventoryLineMA[] Get(Ctx ctx, int M_InventoryLine_ID, Trx trxName)
        {
            List<MInventoryLineMA> list = new List<MInventoryLineMA>();
            String sql = "SELECT * FROM M_InventoryLineMA WHERE M_InventoryLine_ID=" + M_InventoryLine_ID;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                 idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MInventoryLineMA(ctx, dr, trxName));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }
            MInventoryLineMA[] retValue = new MInventoryLineMA[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
         * 	Delete all Material Allocation for Inventory
         *	@param M_Inventory_ID inventory
         *	@param trxName transaction
         *	@return number of rows deleted or -1 for error
         */
        public static int DeleteInventoryMA(int M_Inventory_ID, Trx trxName)
        {
            String sql = "DELETE FROM M_InventoryLineMA ma WHERE EXISTS "
                + "(SELECT * FROM M_InventoryLine l WHERE l.M_InventoryLine_ID=ma.M_InventoryLine_ID"
                + " AND M_Inventory_ID=" + M_Inventory_ID + ")";
            return DataBase.DB.ExecuteQuery(sql, null, trxName);
        }

        /**
         * 	Delete all Material Allocation for Inventory
         *	@param M_InventoryLine_ID inventory
         *	@param trxName transaction
         *	@return number of rows deleted or -1 for error
         */
        public static int DeleteInventoryLineMA(int M_InventoryLine_ID, Trx trxName)
        {
            String sql = "DELETE FROM M_InventoryLineMA ma WHERE EXISTS "
                + "(SELECT * FROM M_InventoryLine l WHERE l.M_InventoryLine_ID=ma.M_InventoryLine_ID"
                + " AND M_InventoryLine_ID=" + M_InventoryLine_ID + ")";
            //return DataBase.executeUpdate(sql, trxName);
            return DataBase.DB.ExecuteQuery(sql, null, trxName);
        }

        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param M_InventoryLineMA_ID ignored
         *	@param trxName trx
         */
        public MInventoryLineMA(Ctx ctx, int M_InventoryLineMA_ID, Trx trxName)
            : base(ctx, M_InventoryLineMA_ID, trxName)
        {
            if (M_InventoryLineMA_ID != 0)
                throw new ArgumentException("Multi-Key");
        }

        /**
         * 	Load Cosntructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName trx
         */
        public MInventoryLineMA(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	Parent Constructor
         *	@param parent parent
         *	@param M_AttributeSetInstance_ID asi
         *	@param MovementQty qty
         */
        public MInventoryLineMA(MInventoryLine parent, int M_AttributeSetInstance_ID, Decimal MovementQty)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {

            SetClientOrg(parent);
            SetM_InventoryLine_ID(parent.GetM_InventoryLine_ID());
            //
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            SetMovementQty(MovementQty);
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MInventoryLineMA[");
            sb.Append("M_InventoryLine_ID=").Append(GetM_InventoryLine_ID())
                .Append(",M_AttributeSetInstance_ID=").Append(GetM_AttributeSetInstance_ID())
                .Append(", Qty=").Append(GetMovementQty())
                .Append("]");
            return sb.ToString();
        }

    }
}
