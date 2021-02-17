/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAMInvInOutLineMP
 * Purpose        : Deletion of records from inout table
 * Class Used     : X_VAM_Inv_InOutLineMP
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
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVAMInvInOutLineMP : X_VAM_Inv_InOutLineMP
    {
        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMInvInOutLineMP).FullName);

        /**
        * 	Get Material Allocations for Line
        *	@param ctx context
        *	@param VAM_Inv_InOutLine_ID line
        *	@param trxName trx
        *	@return allocations
        */
        public static MVAMInvInOutLineMP[] Get(Ctx ctx, int VAM_Inv_InOutLine_ID, Trx trxName)
        {
            List<MVAMInvInOutLineMP> list = new List<MVAMInvInOutLineMP>();
            String sql = "SELECT * FROM VAM_Inv_InOutLineMP WHERE VAM_Inv_InOutLine_ID=" + VAM_Inv_InOutLine_ID;
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MVAMInvInOutLineMP(ctx, dr, trxName));
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            MVAMInvInOutLineMP[] retValue = new MVAMInvInOutLineMP[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /*	Delete all Material Allocation for InOut
        *	@param VAM_Inv_InOut_ID shipment
        *	@param trxName transaction
        *	@return number of rows deleted or -1 for error
        */
        public static int DeleteInOutMA(int VAM_Inv_InOut_ID, Trx trxName)
        {
            String sql = "DELETE FROM VAM_Inv_InOutLineMP ma WHERE EXISTS "
                + "(SELECT * FROM VAM_Inv_InOutLine l WHERE l.VAM_Inv_InOutLine_ID=ma.VAM_Inv_InOutLine_ID"
                + " AND VAM_Inv_InOut_ID=" + VAM_Inv_InOut_ID + ")";
            //return DataBase.executeUpdate(sql, trxName);
            return DataBase.DB.ExecuteQuery(sql, null, trxName);
        }

        /*	Delete all Material Allocation for InOutLine
        *	@param VAM_Inv_InOutLine_ID Shipment Line
        *	@param trxName transaction
        *	@return number of rows deleted or -1 for error
        */
        public static int DeleteInOutLineMA(int VAM_Inv_InOutLine_ID, Trx trxName)
        {
            String sql = "DELETE FROM VAM_Inv_InOutLineMP ma WHERE EXISTS "
                + "(SELECT * FROM VAM_Inv_InOutLine l WHERE l.VAM_Inv_InOutLine_ID=ma.VAM_Inv_InOutLine_ID"
                + " AND VAM_Inv_InOutLine_ID=" + VAM_Inv_InOutLine_ID + ")";
            return DataBase.DB.ExecuteQuery(sql, null, trxName);
        }

        /*	Standard Constructor
         *	@param ctx context
         *	@param VAM_Inv_InOutLineMP_ID ignored
         *	@param trxName trx
         */
        public MVAMInvInOutLineMP(Ctx ctx, int VAM_Inv_InOutLineMP_ID, Trx trxName)
            : base(ctx, VAM_Inv_InOutLineMP_ID, trxName)
        {
            if (VAM_Inv_InOutLineMP_ID != 0)
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
        public MVAMInvInOutLineMP(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
        * 	Parent Constructor
        *	@param parent parent
        *	@param VAM_PFeature_SetInstance_ID asi
        *	@param MovementQty qty
        */
        public MVAMInvInOutLineMP(MVAMInvInOutLine parent, int VAM_PFeature_SetInstance_ID, Decimal MovementQty)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetVAM_Inv_InOutLine_ID(parent.GetVAM_Inv_InOutLine_ID());
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
        public MVAMInvInOutLineMP(MVAMInvInOutLine parent, int VAM_PFeature_SetInstance_ID, Decimal movementQty, DateTime? MMPloicyDate)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetVAM_Inv_InOutLine_ID(parent.GetVAM_Inv_InOutLine_ID());
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
        /// Is Used to Get or Create  Instance of MVAMInvInOutLineMP (Attribute)
        /// </summary>
        /// <param name="line"></param>
        /// <param name="VAM_PFeature_SetInstance_ID"></param>
        /// <param name="MovementQty"></param>
        /// <param name="DateMaterialPolicy"></param>
        /// <returns></returns>
        public static MVAMInvInOutLineMP GetOrCreate(MVAMInvInOutLine line, int VAM_PFeature_SetInstance_ID, Decimal MovementQty, DateTime? DateMaterialPolicy)
        {
            MVAMInvInOutLineMP retValue = null;
            String sql = "SELECT * FROM VAM_Inv_InOutLineMP " +
                         @" WHERE  VAM_Inv_InOutLine_ID = " + line.GetVAM_Inv_InOutLine_ID() +
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
                    retValue = new MVAMInvInOutLineMP(line.GetCtx(), dr, line.Get_Trx());
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
                retValue = new MVAMInvInOutLineMP(line, VAM_PFeature_SetInstance_ID, MovementQty, DateMaterialPolicy);
            else
                retValue.SetMovementQty(Decimal.Add(retValue.GetMovementQty(), MovementQty));
            return retValue;
        }

        /**
	 * 	Get Material Allocations from shipment which is not returned
	 *	@param ctx context
	 *	@param VAM_Inv_InOutLine_ID line
	 *	@param trxName trx
	 *	@return allocations
	 */
        public static MVAMInvInOutLineMP[] getNonReturned(Ctx ctx, int VAM_Inv_InOutLine_ID, Trx trxName)
        {
            List<MVAMInvInOutLineMP> list = new List<MVAMInvInOutLineMP>();
            String sql = "SELECT * FROM VAM_Inv_InOutLineMP WHERE VAM_Inv_InOutLine_ID=" + VAM_Inv_InOutLine_ID + " ORDER BY MMPolicyDate ASC";
            DataSet ds = null;
            try
            {
                ds = DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MVAMInvInOutLineMP(ctx, dr, trxName));
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            MVAMInvInOutLineMP[] retValue = new MVAMInvInOutLineMP[list.Count];
            retValue = list.ToArray();
            return retValue;
        }	
	
         // Mohit 20-8-2015 VAWMS
           /***  Parent Constructor
           * @param parent parent
           * @param VAM_PFeature_SetInstance_ID asi
           * @param MovementQty qty
           *  @param QtyAllocated qty*/
        public MVAMInvInOutLineMP(MVAMInvInOutLine parent, int VAM_PFeature_SetInstance_ID,
                                Decimal MovementQty, Decimal QtyAllocated)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetVAM_Inv_InOutLine_ID(parent.GetVAM_Inv_InOutLine_ID());
            //
            SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            SetMovementQty(MovementQty);
            SetQtyAllocated(QtyAllocated);
        } // MVAMInvInOutLineMP
        //END

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVAMInvInOutLineMP[");
            sb.Append("VAM_Inv_InOutLine_ID=").Append(GetVAM_Inv_InOutLine_ID())
                .Append(",VAM_PFeature_SetInstance_ID=").Append(GetVAM_PFeature_SetInstance_ID())
                .Append(", Qty=").Append(GetMovementQty())
                .Append("]");
            return sb.ToString();
        }
    }
}
