/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : StorageCleanup
 * Purpose        : StorageCleanup
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     28-Oct-2009
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
using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class StorageCleanup : ProcessEngine.SvrProcess
    {
        // Movement Document Type	
        private int _C_DocType_ID = 0;

        /// <summary>
        ///  Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("C_DocType_ID"))
                {
                    _C_DocType_ID = para[i].GetParameterAsInt();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            log.Info("");
            //	Clean up empty Storage
            String sql = "DELETE FROM M_Storage "
                + "WHERE QtyOnHand = 0 AND QtyReserved = 0 AND QtyOrdered = 0"
                //jz + " AND Created < SysDate-3";
                + " AND Created < addDays(SysDate,-3)";
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
            log.Info("Delete Empty #" + no);

            //
            sql = "SELECT * "
                + "FROM M_Storage s "
                + "WHERE AD_Client_ID =" + GetCtx().GetAD_Client_ID()
                + " AND QtyOnHand < 0"
                //	Instance Attribute
                + " AND EXISTS (SELECT * FROM M_Product p"
                    + " INNER JOIN M_AttributeSet mas ON (p.M_AttributeSet_ID=mas.M_AttributeSet_ID) "
                    + "WHERE s.M_Product_ID=p.M_Product_ID AND mas.IsInstanceAttribute='Y')"
                //	Stock in same location
                //	+ " AND EXISTS (SELECT * FROM M_Storage sl "
                //		+ "WHERE sl.QtyOnHand > 0"
                //		+ " AND s.M_Product_ID=sl.M_Product_ID"
                //		+ " AND s.M_Locator_ID=sl.M_Locator_ID)"
                //	Stock in same Warehouse
                + " AND EXISTS (SELECT * FROM M_Storage sw"
                    + " INNER JOIN M_Locator swl ON (sw.M_Locator_ID=swl.M_Locator_ID), M_Locator sl "
                    + "WHERE sw.QtyOnHand > 0"
                    + " AND s.M_Product_ID=sw.M_Product_ID"
                    + " AND s.M_Locator_ID=sl.M_Locator_ID"
                    + " AND sl.M_Warehouse_ID=swl.M_Warehouse_ID)";
            DataTable dt = null;
            IDataReader idr = null;
            int lines = 0;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_Trx());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    lines += Move(new MStorage(GetCtx(), dr, Get_Trx()));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
                if(idr!=null)
                {
                   idr.Close();
                }
            }

            return "#" + lines;
        }

        /// <summary>
        ///  	Move stock to location
        /// </summary>
        /// <param name="target">storage</param>
        /// <returns>no of movements</returns>
        private int Move(MStorage target)
        {
            log.Info(target.ToString());
            Decimal qty = Decimal.Negate(target.GetQtyOnHand());//.negate();

            //	Create Movement
            MMovement mh = new MMovement(GetCtx(), 0, Get_Trx());
            mh.SetC_DocType_ID(_C_DocType_ID);
            mh.SetDescription(GetName());
            if (!mh.Save())
            {
                return 0;
            }

            int lines = 0;
            MStorage[] sources = GetSources(target.GetM_Product_ID(), target.GetM_Locator_ID());
            for (int i = 0; i < sources.Length; i++)
            {
                MStorage source = sources[i];

                //	Movement Line
                MMovementLine ml = new MMovementLine(mh);
                ml.SetM_Product_ID(target.GetM_Product_ID());
                ml.SetM_LocatorTo_ID(target.GetM_Locator_ID());
                ml.SetM_AttributeSetInstanceTo_ID(target.GetM_AttributeSetInstance_ID());
                //	From
                ml.SetM_Locator_ID(source.GetM_Locator_ID());
                ml.SetM_AttributeSetInstance_ID(source.GetM_AttributeSetInstance_ID());

                Decimal qtyMove = qty;
                if (qtyMove.CompareTo(source.GetQtyOnHand()) > 0)
                {
                    qtyMove = source.GetQtyOnHand();
                }
                ml.SetMovementQty(qtyMove);
                //
                lines++;
                ml.SetLine(lines * 10);
                if (!ml.Save())
                {
                    return 0;
                }

                qty = Decimal.Subtract(qty, qtyMove);
                if (Env.Signum(qty) <= 0)
                {
                    break;
                }
            }	//	for all movements

            //	Process
            //mh.ProcessIt(MMovement.ACTION_Complete);
            mh.ProcessIt(DocActionVariables.ACTION_COMPLETE);
            mh.Save();

            AddLog(0, null, new Decimal(lines), "@M_Movement_ID@ " + mh.GetDocumentNo() + " ("
                + MRefList.Get(GetCtx(), MMovement.DOCSTATUS_AD_Reference_ID,
                    mh.GetDocStatus(), Get_Trx()) + ")");

            EliminateReservation(target);
            return lines;
        }

        /// <summary>
        /// Eliminate Reserved/Ordered
        /// </summary>
        /// <param name="target">target Storage</param>
        private void EliminateReservation(MStorage target)
        {
            //	Negative Ordered / Reserved Qty
            if (Env.Signum(target.GetQtyReserved()) != 0 || Env.Signum(target.GetQtyOrdered()) != 0)
            {
                int M_Locator_ID = target.GetM_Locator_ID();
                MStorage storage0 = MStorage.Get(GetCtx(), M_Locator_ID,
                    target.GetM_Product_ID(), 0, Get_Trx());
                if (storage0 == null)
                {
                    MLocator defaultLoc = MLocator.GetDefault(GetCtx(), M_Locator_ID);
                    if (M_Locator_ID != defaultLoc.GetM_Locator_ID())
                    {
                        M_Locator_ID = defaultLoc.GetM_Locator_ID();
                        storage0 = MStorage.Get(GetCtx(), M_Locator_ID,
                            target.GetM_Product_ID(), 0, Get_Trx());
                    }
                }
                if (storage0 != null)
                {
                    Decimal reserved = Env.ZERO;
                    Decimal ordered = Env.ZERO;
                    if (Env.Signum(Decimal.Add(target.GetQtyReserved(), storage0.GetQtyReserved())) >= 0)
                    {
                        reserved = target.GetQtyReserved();		//	negative
                    }
                    if (Env.Signum(Decimal.Add(target.GetQtyOrdered(), storage0.GetQtyOrdered())) >= 0)
                    {
                        ordered = target.GetQtyOrdered();		//	negative
                    }
                    //	Eliminate Reservation
                    if (Env.Signum(reserved) != 0 || Env.Signum(ordered) != 0)
                    {
                        if (MStorage.Add(GetCtx(), target.GetM_Warehouse_ID(), target.GetM_Locator_ID(),
                            target.GetM_Product_ID(),
                            target.GetM_AttributeSetInstance_ID(), target.GetM_AttributeSetInstance_ID(),
                            Env.ZERO, Decimal.Negate(reserved), Decimal.Negate(ordered), Get_Trx()))
                        {
                            if (MStorage.Add(GetCtx(), storage0.GetM_Warehouse_ID(), storage0.GetM_Locator_ID(),
                                storage0.GetM_Product_ID(),
                                storage0.GetM_AttributeSetInstance_ID(), storage0.GetM_AttributeSetInstance_ID(),
                                Env.ZERO, reserved, ordered, Get_Trx()))
                            {
                                log.Info("Reserved=" + reserved + ",Ordered=" + ordered);
                            }
                            else
                            {
                                log.Warning("Failed Storage0 Update");
                            }
                        }
                        else
                        {
                            log.Warning("Failed Target Update");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get Storage Sources
        /// </summary>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_Locator_ID">locator</param>
        /// <returns>sources</returns>
        private MStorage[] GetSources(int M_Product_ID, int M_Locator_ID)
        {
            List<MStorage> list = new List<MStorage>();
            String sql = "SELECT * "
                + "FROM M_Storage s "
                + "WHERE QtyOnHand > 0"
                + " AND M_Product_ID=" + M_Product_ID
                //	Empty ASI
                + " AND (M_AttributeSetInstance_ID=0"
                + " OR EXISTS (SELECT * FROM M_AttributeSetInstance asi "
                    + "WHERE s.M_AttributeSetInstance_ID=asi.M_AttributeSetInstance_ID"
                    + " AND asi.Description IS NULL) )"
                //	Stock in same Warehouse
                + " AND EXISTS (SELECT * FROM M_Locator sl, M_Locator x "
                    + "WHERE s.M_Locator_ID=sl.M_Locator_ID"
                    + " AND x.M_Locator_ID=" + M_Locator_ID
                    + " AND sl.M_Warehouse_ID=x.M_Warehouse_ID) "
                + "ORDER BY M_AttributeSetInstance_ID";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_Trx());

                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MStorage(GetCtx(), dr, Get_Trx()));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }

            MStorage[] retValue = new MStorage[list.Count];
            retValue = list.ToArray();
            return retValue;
        }
    }
}
