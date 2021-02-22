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
//using System.Windows.Forms;
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
        private int _VAB_DocTypes_ID = 0;

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
                else if (name.Equals("VAB_DocTypes_ID"))
                {
                    _VAB_DocTypes_ID = para[i].GetParameterAsInt();
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
            String sql = "DELETE FROM VAM_Storage "
                + "WHERE QtyOnHand = 0 AND QtyReserved = 0 AND QtyOrdered = 0"
                //jz + " AND Created < SysDate-3";
                + " AND Created < addDays(SysDate,-3)";
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
            log.Info("Delete Empty #" + no);

            //
            sql = "SELECT * "
                + "FROM VAM_Storage s "
                + "WHERE VAF_Client_ID =" + GetCtx().GetVAF_Client_ID()
                + " AND QtyOnHand < 0"
                //	Instance Attribute
                + " AND EXISTS (SELECT * FROM VAM_Product p"
                    + " INNER JOIN VAM_PFeature_Set mas ON (p.VAM_PFeature_Set_ID=mas.VAM_PFeature_Set_ID) "
                    + "WHERE s.VAM_Product_ID=p.VAM_Product_ID AND mas.IsInstanceAttribute='Y')"
                //	Stock in same location
                //	+ " AND EXISTS (SELECT * FROM VAM_Storage sl "
                //		+ "WHERE sl.QtyOnHand > 0"
                //		+ " AND s.VAM_Product_ID=sl.VAM_Product_ID"
                //		+ " AND s.VAM_Locator_ID=sl.VAM_Locator_ID)"
                //	Stock in same Warehouse
                + " AND EXISTS (SELECT * FROM VAM_Storage sw"
                    + " INNER JOIN VAM_Locator swl ON (sw.VAM_Locator_ID=swl.VAM_Locator_ID), VAM_Locator sl "
                    + "WHERE sw.QtyOnHand > 0"
                    + " AND s.VAM_Product_ID=sw.VAM_Product_ID"
                    + " AND s.VAM_Locator_ID=sl.VAM_Locator_ID"
                    + " AND sl.VAM_Warehouse_ID=swl.VAM_Warehouse_ID)";
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
                    lines += Move(new MVAMStorage(GetCtx(), dr, Get_Trx()));
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
        private int Move(MVAMStorage target)
        {
            log.Info(target.ToString());
            Decimal qty = Decimal.Negate(target.GetQtyOnHand());//.negate();

            //	Create Movement
            MVAMInventoryTransfer mh = new MVAMInventoryTransfer(GetCtx(), 0, Get_Trx());
            mh.SetVAB_DocTypes_ID(_VAB_DocTypes_ID);
            mh.SetDescription(GetName());
            if (!mh.Save())
            {
                return 0;
            }

            int lines = 0;
            MVAMStorage[] sources = GetSources(target.GetVAM_Product_ID(), target.GetVAM_Locator_ID());
            for (int i = 0; i < sources.Length; i++)
            {
                MVAMStorage source = sources[i];

                //	Movement Line
                MVAMInvTrfLine ml = new MVAMInvTrfLine(mh);
                ml.SetVAM_Product_ID(target.GetVAM_Product_ID());
                ml.SetVAM_LocatorTo_ID(target.GetVAM_Locator_ID());
                ml.SetVAM_PFeature_SetInstanceTo_ID(target.GetVAM_PFeature_SetInstance_ID());
                //	From
                ml.SetVAM_Locator_ID(source.GetVAM_Locator_ID());
                ml.SetVAM_PFeature_SetInstance_ID(source.GetVAM_PFeature_SetInstance_ID());

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
            //mh.ProcessIt(MVAMInventoryTransfer.ACTION_Complete);
            mh.ProcessIt(DocActionVariables.ACTION_COMPLETE);
            mh.Save();

            AddLog(0, null, new Decimal(lines), "@VAM_InventoryTransfer_ID@ " + mh.GetDocumentNo() + " ("
                + MVAFCtrlRefList.Get(GetCtx(), MVAMInventoryTransfer.DOCSTATUS_VAF_Control_Ref_ID,
                    mh.GetDocStatus(), Get_Trx()) + ")");

            EliminateReservation(target);
            return lines;
        }

        /// <summary>
        /// Eliminate Reserved/Ordered
        /// </summary>
        /// <param name="target">target Storage</param>
        private void EliminateReservation(MVAMStorage target)
        {
            //	Negative Ordered / Reserved Qty
            if (Env.Signum(target.GetQtyReserved()) != 0 || Env.Signum(target.GetQtyOrdered()) != 0)
            {
                int VAM_Locator_ID = target.GetVAM_Locator_ID();
                MVAMStorage storage0 = MVAMStorage.Get(GetCtx(), VAM_Locator_ID,
                    target.GetVAM_Product_ID(), 0, Get_Trx());
                if (storage0 == null)
                {
                    MVAMLocator defaultLoc = MVAMLocator.GetDefault(GetCtx(), VAM_Locator_ID);
                    if (VAM_Locator_ID != defaultLoc.GetVAM_Locator_ID())
                    {
                        VAM_Locator_ID = defaultLoc.GetVAM_Locator_ID();
                        storage0 = MVAMStorage.Get(GetCtx(), VAM_Locator_ID,
                            target.GetVAM_Product_ID(), 0, Get_Trx());
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
                        if (MVAMStorage.Add(GetCtx(), target.GetVAM_Warehouse_ID(), target.GetVAM_Locator_ID(),
                            target.GetVAM_Product_ID(),
                            target.GetVAM_PFeature_SetInstance_ID(), target.GetVAM_PFeature_SetInstance_ID(),
                            Env.ZERO, Decimal.Negate(reserved), Decimal.Negate(ordered), Get_Trx()))
                        {
                            if (MVAMStorage.Add(GetCtx(), storage0.GetVAM_Warehouse_ID(), storage0.GetVAM_Locator_ID(),
                                storage0.GetVAM_Product_ID(),
                                storage0.GetVAM_PFeature_SetInstance_ID(), storage0.GetVAM_PFeature_SetInstance_ID(),
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
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_Locator_ID">locator</param>
        /// <returns>sources</returns>
        private MVAMStorage[] GetSources(int VAM_Product_ID, int VAM_Locator_ID)
        {
            List<MVAMStorage> list = new List<MVAMStorage>();
            String sql = "SELECT * "
                + "FROM VAM_Storage s "
                + "WHERE QtyOnHand > 0"
                + " AND VAM_Product_ID=" + VAM_Product_ID
                //	Empty ASI
                + " AND (VAM_PFeature_SetInstance_ID=0"
                + " OR EXISTS (SELECT * FROM VAM_PFeature_SetInstance asi "
                    + "WHERE s.VAM_PFeature_SetInstance_ID=asi.VAM_PFeature_SetInstance_ID"
                    + " AND asi.Description IS NULL) )"
                //	Stock in same Warehouse
                + " AND EXISTS (SELECT * FROM VAM_Locator sl, VAM_Locator x "
                    + "WHERE s.VAM_Locator_ID=sl.VAM_Locator_ID"
                    + " AND x.VAM_Locator_ID=" + VAM_Locator_ID
                    + " AND sl.VAM_Warehouse_ID=x.VAM_Warehouse_ID) "
                + "ORDER BY VAM_PFeature_SetInstance_ID";
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
                    list.Add(new MVAMStorage(GetCtx(), dr, Get_Trx()));
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

            MVAMStorage[] retValue = new MVAMStorage[list.Count];
            retValue = list.ToArray();
            return retValue;
        }
    }
}
