/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAMInventoryLineMP
 * Purpose        : Inventory Material Allocation
 * Class Used     : X_VAM_InventoryLineMP
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
//////using System.Windows.Forms;
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
    public class MVAMInventoryLineMP : X_VAM_InventoryLineMP
    {

        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMInventoryLineMP).FullName);

        /**
         * 	Get Material Allocations for Line
         *	@param ctx context
         *	@param VAM_InventoryLine_ID line
         *	@param trxName trx
         *	@return allocations
         */
        public static MVAMInventoryLineMP[] Get(Ctx ctx, int VAM_InventoryLine_ID, Trx trxName)
        {
            List<MVAMInventoryLineMP> list = new List<MVAMInventoryLineMP>();
            String sql = "SELECT * FROM VAM_InventoryLineMP WHERE VAM_InventoryLine_ID=" + VAM_InventoryLine_ID;
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
                    list.Add(new MVAMInventoryLineMP(ctx, dr, trxName));
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
            MVAMInventoryLineMP[] retValue = new MVAMInventoryLineMP[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
         * 	Delete all Material Allocation for Inventory
         *	@param VAM_Inventory_ID inventory
         *	@param trxName transaction
         *	@return number of rows deleted or -1 for error
         */
        public static int DeleteInventoryMA(int VAM_Inventory_ID, Trx trxName)
        {
            String sql = "DELETE FROM VAM_InventoryLineMP ma WHERE EXISTS "
                + "(SELECT * FROM VAM_InventoryLine l WHERE l.VAM_InventoryLine_ID=ma.VAM_InventoryLine_ID"
                + " AND VAM_Inventory_ID=" + VAM_Inventory_ID + ")";
            return DataBase.DB.ExecuteQuery(sql, null, trxName);
        }

        /**
         * 	Delete all Material Allocation for Inventory
         *	@param VAM_InventoryLine_ID inventory
         *	@param trxName transaction
         *	@return number of rows deleted or -1 for error
         */
        public static int DeleteInventoryLineMA(int VAM_InventoryLine_ID, Trx trxName)
        {
            String sql = "DELETE FROM VAM_InventoryLineMP ma WHERE EXISTS "
                + "(SELECT * FROM VAM_InventoryLine l WHERE l.VAM_InventoryLine_ID=ma.VAM_InventoryLine_ID"
                + " AND VAM_InventoryLine_ID=" + VAM_InventoryLine_ID + ")";
            //return DataBase.executeUpdate(sql, trxName);
            return DataBase.DB.ExecuteQuery(sql, null, trxName);
        }

        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param VAM_InventoryLineMP_ID ignored
         *	@param trxName trx
         */
        public MVAMInventoryLineMP(Ctx ctx, int VAM_InventoryLineMP_ID, Trx trxName)
            : base(ctx, VAM_InventoryLineMP_ID, trxName)
        {
            if (VAM_InventoryLineMP_ID != 0)
                throw new ArgumentException("Multi-Key");
        }

        /**
         * 	Load Cosntructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName trx
         */
        public MVAMInventoryLineMP(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	Parent Constructor
         *	@param parent parent
         *	@param VAM_PFeature_SetInstance_ID asi
         *	@param MovementQty qty
         */
        public MVAMInventoryLineMP(MVAMInventoryLine parent, int VAM_PFeature_SetInstance_ID, Decimal MovementQty)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {

            SetClientOrg(parent);
            SetVAM_InventoryLine_ID(parent.GetVAM_InventoryLine_ID());
            //
            SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            SetMovementQty(MovementQty);
        }

             /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="VAM_PFeature_SetInstance_ID"></param>
        /// <param name="movementQty"></param>
        /// <param name="MMPloicyDate"></param>
        public MVAMInventoryLineMP(MVAMInventoryLine parent, int VAM_PFeature_SetInstance_ID, Decimal movementQty, DateTime? MMPloicyDate)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetVAM_InventoryLine_ID(parent.GetVAM_InventoryLine_ID());
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
        /// Is Used to Get or Create  Instance of MVAMInventoryLineMP (Attribute)
        /// </summary>
        /// <param name="line"></param>
        /// <param name="VAM_PFeature_SetInstance_ID"></param>
        /// <param name="MovementQty"></param>
        /// <param name="DateMaterialPolicy"></param>
        /// <returns></returns>
        public static MVAMInventoryLineMP GetOrCreate(MVAMInventoryLine line, int VAM_PFeature_SetInstance_ID, Decimal MovementQty, DateTime? DateMaterialPolicy)
        {
            MVAMInventoryLineMP retValue = null;
            String sql = "SELECT * FROM VAM_InventoryLineMP " +
                         @" WHERE  VAM_InventoryLine_ID = " + line.GetVAM_InventoryLine_ID() +
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
                    retValue = new MVAMInventoryLineMP(line.GetCtx(), dr, line.Get_Trx());
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
                retValue = new MVAMInventoryLineMP(line, VAM_PFeature_SetInstance_ID, MovementQty, DateMaterialPolicy);
            else
                retValue.SetMovementQty(Decimal.Add(retValue.GetMovementQty(), MovementQty));
            return retValue;
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVAMInventoryLineMP[");
            sb.Append("VAM_InventoryLine_ID=").Append(GetVAM_InventoryLine_ID())
                .Append(",VAM_PFeature_SetInstance_ID=").Append(GetVAM_PFeature_SetInstance_ID())
                .Append(", Qty=").Append(GetMovementQty())
                .Append("]");
            return sb.ToString();
        }

    }
}
