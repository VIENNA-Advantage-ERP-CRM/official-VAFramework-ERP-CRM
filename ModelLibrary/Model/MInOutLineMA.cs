/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MInOutLineMA
 * Purpose        : Deletion of records from inout table
 * Class Used     : X_M_InOutLineMA
 * Chronological    Development
 * Raghunandan     08-Jun-2009  
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
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MInOutLineMA : X_M_InOutLineMA
    {
        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MInOutLineMA).FullName);

        /**
        * 	Get Material Allocations for Line
        *	@param ctx context
        *	@param M_InOutLine_ID line
        *	@param trxName trx
        *	@return allocations
        */
        public static MInOutLineMA[] Get(Ctx ctx, int M_InOutLine_ID, Trx trxName)
        {
            List<MInOutLineMA> list = new List<MInOutLineMA>();
            String sql = "SELECT * FROM M_InOutLineMA WHERE M_InOutLine_ID=" + M_InOutLine_ID;
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MInOutLineMA(ctx, dr, trxName));
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            MInOutLineMA[] retValue = new MInOutLineMA[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /*	Delete all Material Allocation for InOut
        *	@param M_InOut_ID shipment
        *	@param trxName transaction
        *	@return number of rows deleted or -1 for error
        */
        public static int DeleteInOutMA(int M_InOut_ID, Trx trxName)
        {
            String sql = "DELETE FROM M_InOutLineMA ma WHERE EXISTS "
                + "(SELECT * FROM M_InOutLine l WHERE l.M_InOutLine_ID=ma.M_InOutLine_ID"
                + " AND M_InOut_ID=" + M_InOut_ID + ")";
            //return DataBase.executeUpdate(sql, trxName);
            return DataBase.DB.ExecuteQuery(sql, null, trxName);
        }

        /*	Delete all Material Allocation for InOutLine
        *	@param M_InOutLine_ID Shipment Line
        *	@param trxName transaction
        *	@return number of rows deleted or -1 for error
        */
        public static int DeleteInOutLineMA(int M_InOutLine_ID, Trx trxName)
        {
            String sql = "DELETE FROM M_InOutLineMA ma WHERE EXISTS "
                + "(SELECT * FROM M_InOutLine l WHERE l.M_InOutLine_ID=ma.M_InOutLine_ID"
                + " AND M_InOutLine_ID=" + M_InOutLine_ID + ")";
            return DataBase.DB.ExecuteQuery(sql, null, trxName);
        }

        /*	Standard Constructor
         *	@param ctx context
         *	@param M_InOutLineMA_ID ignored
         *	@param trxName trx
         */
        public MInOutLineMA(Ctx ctx, int M_InOutLineMA_ID, Trx trxName)
            : base(ctx, M_InOutLineMA_ID, trxName)
        {
            if (M_InOutLineMA_ID != 0)
            {
                throw new ArgumentException("Multi-Key");
            }
        }

        /**
         * 	Load Cosntructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName trx
         */
        public MInOutLineMA(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
        * 	Parent Constructor
        *	@param parent parent
        *	@param M_AttributeSetInstance_ID asi
        *	@param MovementQty qty
        */
        public MInOutLineMA(MInOutLine parent, int M_AttributeSetInstance_ID, Decimal MovementQty)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetM_InOutLine_ID(parent.GetM_InOutLine_ID());
            //
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            SetMovementQty(MovementQty);
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="M_AttributeSetInstance_ID"></param>
        /// <param name="movementQty"></param>
        /// <param name="MMPloicyDate"></param>
        public MInOutLineMA(MInOutLine parent, int M_AttributeSetInstance_ID, Decimal movementQty, DateTime? MMPloicyDate)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetM_InOutLine_ID(parent.GetM_InOutLine_ID());
            //
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            SetMovementQty(movementQty);
            if (MMPloicyDate == null)
            {
                MMPloicyDate = parent.GetParent().GetMovementDate();
            }
            SetMMPolicyDate(MMPloicyDate);
        }

        /// <summary>
        /// Is Used to Get or Create  Instance of MInoutLineMA (Attribute)
        /// </summary>
        /// <param name="line"></param>
        /// <param name="M_AttributeSetInstance_ID"></param>
        /// <param name="MovementQty"></param>
        /// <param name="DateMaterialPolicy"></param>
        /// <returns></returns>
        public static MInOutLineMA GetOrCreate(MInOutLine line, int M_AttributeSetInstance_ID, Decimal MovementQty, DateTime? DateMaterialPolicy)
        {
            MInOutLineMA retValue = null;
            String sql = "SELECT * FROM M_InoutLineMA " +
                         @" WHERE  M_InOutLine_ID = " + line.GetM_InOutLine_ID() +
                         @" AND MMPolicyDate = " + GlobalVariable.TO_DATE(DateMaterialPolicy, true) + @" AND ";
            if (M_AttributeSetInstance_ID == 0)
                sql += "(M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID + " OR M_AttributeSetInstance_ID IS NULL)";
            else
                sql += "M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
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
                    retValue = new MInOutLineMA(line.GetCtx(), dr, line.Get_Trx());
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
                retValue = new MInOutLineMA(line, M_AttributeSetInstance_ID, MovementQty, DateMaterialPolicy);
            else
                retValue.SetMovementQty(Decimal.Add(retValue.GetMovementQty(), MovementQty));
            return retValue;
        }

        /**
	 * 	Get Material Allocations from shipment which is not returned
	 *	@param ctx context
	 *	@param M_InOutLine_ID line
	 *	@param trxName trx
	 *	@return allocations
	 */
        public static MInOutLineMA[] getNonReturned(Ctx ctx, int M_InOutLine_ID, Trx trxName)
        {
            List<MInOutLineMA> list = new List<MInOutLineMA>();
            String sql = "SELECT * FROM M_InOutLineMA WHERE M_InOutLine_ID=" + M_InOutLine_ID + " ORDER BY MMPolicyDate ASC";
            DataSet ds = null;
            try
            {
                ds = DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MInOutLineMA(ctx, dr, trxName));
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            MInOutLineMA[] retValue = new MInOutLineMA[list.Count];
            retValue = list.ToArray();
            return retValue;
        }	
	
         // Mohit 20-8-2015 VAWMS
           /***  Parent Constructor
           * @param parent parent
           * @param M_AttributeSetInstance_ID asi
           * @param MovementQty qty
           *  @param QtyAllocated qty*/
        public MInOutLineMA(MInOutLine parent, int M_AttributeSetInstance_ID,
                                Decimal MovementQty, Decimal QtyAllocated)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetM_InOutLine_ID(parent.GetM_InOutLine_ID());
            //
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            SetMovementQty(MovementQty);
            SetQtyAllocated(QtyAllocated);
        } // MInOutLineMA
        //END

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MInOutLineMA[");
            sb.Append("M_InOutLine_ID=").Append(GetM_InOutLine_ID())
                .Append(",M_AttributeSetInstance_ID=").Append(GetM_AttributeSetInstance_ID())
                .Append(", Qty=").Append(GetMovementQty())
                .Append("]");
            return sb.ToString();
        }
    }
}
