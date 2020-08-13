/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ReplenishReport
 * Purpose        : Copy Accounts from one Acct Schema to another
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           13-Jan-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;


using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class ReplenishReport : ProcessEngine.SvrProcess
    {
        /** Warehouse				*/
        private int _M_Warehouse_ID = 0;
        /**	Optional BPartner		*/
        private int _C_BPartner_ID = 0;
        /** Create (POO)Purchse Order or (POR)Requisition or (MMM)Movements */
        private String _ReplenishmentCreate = null;
        /** Document Type			*/
        private int _C_DocType_ID = 0;
        string _DocNo = null;
        /** Return Info				*/
        private String _info = "";
        private string _M_WareSource = "";
        /// <summary>
        /// Prepare - e.g., get Parameters.
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
                else if (name.Equals("M_Warehouse_ID"))
                {
                    _M_Warehouse_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_BPartner_ID"))
                {
                    _C_BPartner_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("ReplenishmentCreate"))
                {
                    _ReplenishmentCreate = Utility.Util.GetValueOfString(para[i].GetParameter());
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
        }	//	prepare

        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>Message </returns>
        protected override String DoIt()
        {
            log.Info("M_Warehouse_ID=" + _M_Warehouse_ID
                + ", C_BPartner_ID=" + _C_BPartner_ID
                + " - ReplenishmentCreate=" + _ReplenishmentCreate
                + ", C_DocType_ID=" + _C_DocType_ID);
            if (_ReplenishmentCreate != null && _C_DocType_ID == 0 || _C_DocType_ID == -1)
            {

                throw new Exception("@FillMandatory@ @C_DocType_ID@");
            }
            

            MWarehouse wh = MWarehouse.Get(GetCtx(), _M_Warehouse_ID);
            if (wh.Get_ID() == 0)
            {
                throw new Exception("@FillMandatory@ @M_Warehouse_ID@");
            }
            if (wh.GetM_WarehouseSource_ID() > 0)
            {
                _M_WareSource = "M_WarehouseSource_ID = " + Util.GetValueOfString(wh.GetM_WarehouseSource_ID());
            }
            else
            {
                _M_WareSource = null;
            }
            //
            PrepareTable();
            FillTable(wh);
            //
            if (_ReplenishmentCreate == null)
            {
                return "OK";
            }
            //
            MDocType dt = MDocType.Get(GetCtx(), _C_DocType_ID);
            if (!dt.GetDocBaseType().Equals(_ReplenishmentCreate))
            {
                throw new Exception("@C_DocType_ID@=" + dt.GetName() + " <> " + _ReplenishmentCreate);
            }
            //
           
            if (_ReplenishmentCreate.Equals("POO"))
            {
                CreatePO();
            }
            else if (_ReplenishmentCreate.Equals("POR"))
            {
                CreateRequisition();
            }
            else if (_ReplenishmentCreate.Equals("MMM"))
            {
                CreateMovements();
            }
            return _info;
           
            
        }	//	doIt

        /// <summary>
        ///	Prepare/Check Replenishment Table
        /// </summary>
        private void PrepareTable()
        {
            //	Level_Max must be >= Level_Max
            String sql = "UPDATE M_Replenish"
                + " SET Level_Max = Level_Min "
                + "WHERE Level_Max < Level_Min";
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("Corrected Max_Level=" + no);
            }

            //	Minimum Order should be 1
            sql = "UPDATE M_Product_PO"
                + " SET Order_Min = 1 "
                + "WHERE Order_Min IS NULL OR Order_Min < 1";
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("Corrected Order Min=" + no);
            }

            //	Pack should be 1
            sql = "UPDATE M_Product_PO"
                + " SET Order_Pack = 1 "
                + "WHERE Order_Pack IS NULL OR Order_Pack < 1";
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("Corrected Order Pack=" + no);
            }

            //	Set Current Vendor where only one vendor
            sql = "UPDATE M_Product_PO p"
                + " SET IsCurrentVendor='Y' "
                + "WHERE IsCurrentVendor<>'Y'"
                //jz groupby problem + " AND EXISTS (SELECT * FROM M_Product_PO pp "
                + " AND EXISTS (SELECT 1 FROM M_Product_PO pp "
                    + "WHERE p.M_Product_ID=pp.M_Product_ID "
                    + "GROUP BY pp.M_Product_ID "
                    + "HAVING COUNT(*) = 1)";
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("Corrected CurrentVendor(Y)=" + no);
            }

            //	More then one current vendor
            sql = "UPDATE M_Product_PO p"
                + " SET IsCurrentVendor='N' "
                + "WHERE IsCurrentVendor = 'Y'"
                //jz + " AND EXISTS (SELECT * FROM M_Product_PO pp "
                + " AND EXISTS (SELECT 1 FROM M_Product_PO pp "
                    + "WHERE p.M_Product_ID=pp.M_Product_ID AND pp.IsCurrentVendor='Y' "
                    + "GROUP BY pp.M_Product_ID "
                    + "HAVING COUNT(*) > 1)";
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("Corrected CurrentVendor(N)=" + no);
            }

            //	Just to be sure
            sql = "DELETE FROM T_Replenish WHERE AD_PInstance_ID=" + GetAD_PInstance_ID();
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("Delete Existing Temp=" + no);
            }
        }	//	prepareTable

        /// <summary>
        /// Fill Table
        /// </summary>
        /// <param name="wh">warehouse</param>
        private void FillTable(MWarehouse wh)
        {
            String sql = "INSERT INTO T_Replenish "
                + "(AD_PInstance_ID, M_Warehouse_ID, M_Product_ID, AD_Client_ID, AD_Org_ID,"
                + " ReplenishType, Level_Min, Level_Max, QtyOnHand,QtyReserved,QtyOrdered,"
                + " C_BPartner_ID, Order_Min, Order_Pack, QtyToOrder, ReplenishmentCreate) "
                + "SELECT " + GetAD_PInstance_ID()
                    + ", r.M_Warehouse_ID, r.M_Product_ID, r.AD_Client_ID, r.AD_Org_ID,"
                + " r.ReplenishType, r.Level_Min, r.Level_Max, 0,0,0,"
                + " po.C_BPartner_ID, po.Order_Min, po.Order_Pack, 0, ";
            if (_ReplenishmentCreate == null)
            {
                sql += "null";
            }
            else
            {
                sql += "'" + _ReplenishmentCreate + "'";
            }
            sql += " FROM M_Replenish r"
                + " INNER JOIN M_Product_PO po ON (r.M_Product_ID=po.M_Product_ID) "
                + "WHERE po.IsCurrentVendor='Y'"	//	Only Current Vendor
                + " AND r.ReplenishType<>'0'"
                + " AND po.IsActive='Y' AND r.IsActive='Y'"
                + " AND r.M_Warehouse_ID=" + _M_Warehouse_ID;
            if (_C_BPartner_ID != 0 && _C_BPartner_ID != -1)
            {
                sql += " AND po.C_BPartner_ID=" + _C_BPartner_ID;
            }
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            log.Finest(sql);
            log.Fine("Insert (1) #" + no);

            if (_C_BPartner_ID == 0 || _C_BPartner_ID == -1)
            {
                sql = "INSERT INTO T_Replenish "
                    + "(AD_PInstance_ID, M_Warehouse_ID, M_Product_ID, AD_Client_ID, AD_Org_ID,"
                    + " ReplenishType, Level_Min, Level_Max,"
                    + " C_BPartner_ID, Order_Min, Order_Pack, QtyToOrder, ReplenishmentCreate) "
                    + "SELECT " + GetAD_PInstance_ID()
                    + ", r.M_Warehouse_ID, r.M_Product_ID, r.AD_Client_ID, r.AD_Org_ID,"
                    + " r.ReplenishType, r.Level_Min, r.Level_Max,"
                    //jz + " null, 1, 1, 0, ";
                    + DataBase.DB.NULL("I", Types.VARCHAR)
                    + " , 1, 1, 0, ";
                if (_ReplenishmentCreate == null)
                {
                    sql += "null";
                }
                else
                {
                    sql += "'" + _ReplenishmentCreate + "'";
                }
                sql += " FROM M_Replenish r "
                    + "WHERE r.ReplenishType<>'0' AND r.IsActive='Y'"
                    + " AND r.M_Warehouse_ID=" + _M_Warehouse_ID
                    + " AND NOT EXISTS (SELECT * FROM T_Replenish t "
                        + "WHERE r.M_Product_ID=t.M_Product_ID"
                        + " AND AD_PInstance_ID=" + GetAD_PInstance_ID() + ")";

                no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
                log.Fine("Insert (BP) #" + no);
            }

            sql = "UPDATE T_Replenish t SET "
                + "QtyOnHand = (SELECT COALESCE(SUM(QtyOnHand),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID),"
                + "QtyReserved = (SELECT COALESCE(SUM(QtyReserved),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID),"
                + "QtyOrdered = (SELECT COALESCE(SUM(QtyOrdered),0) FROM M_Storage s, M_Locator l WHERE t.M_Product_ID=s.M_Product_ID"
                    + " AND l.M_Locator_ID=s.M_Locator_ID AND l.M_Warehouse_ID=t.M_Warehouse_ID)";
            if (_C_DocType_ID != 0 && _C_DocType_ID != -1)
            {
                sql += ", C_DocType_ID=" + _C_DocType_ID;
            }
            sql += " WHERE AD_PInstance_ID=" + GetAD_PInstance_ID();
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("Update #" + no);
            }

            //	Delete inactive products and replenishments
            sql = "DELETE FROM T_Replenish r "
                + "WHERE (EXISTS (SELECT * FROM M_Product p "
                    + "WHERE p.M_Product_ID=r.M_Product_ID AND p.IsActive='N')"
                + " OR EXISTS (SELECT * FROM M_Replenish rr "
                    + " WHERE rr.M_Product_ID=r.M_Product_ID AND rr.M_Warehouse_ID=r.M_Warehouse_ID AND rr.IsActive='N'))"
                + " AND AD_PInstance_ID=" + GetAD_PInstance_ID();
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("Delete Inactive=" + no);
            }

            //	Ensure Data consistency
            sql = "UPDATE T_Replenish SET QtyOnHand = 0 WHERE QtyOnHand IS NULL";
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            sql = "UPDATE T_Replenish SET QtyReserved = 0 WHERE QtyReserved IS NULL";
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            sql = "UPDATE T_Replenish SET QtyOrdered = 0 WHERE QtyOrdered IS NULL";
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());

            //	Set Minimum / Maximum Maintain Level
            //	X_M_Replenish.REPLENISHTYPE_ReorderBelowMinimumLevel
            sql = "UPDATE T_Replenish"
                + " SET QtyToOrder = Level_Min - QtyOnHand + QtyReserved - QtyOrdered "
                + "WHERE ReplenishType='1'"
                + " AND AD_PInstance_ID=" + GetAD_PInstance_ID();
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("Update Type-1=" + no);
            }
            //
            //	X_M_Replenish.REPLENISHTYPE_MaintainMaximumLevel

            sql = "UPDATE T_Replenish"
                + " SET QtyToOrder = Level_Max - QtyOnHand + QtyReserved - QtyOrdered "
                + "WHERE ReplenishType='2'"
                + " AND AD_PInstance_ID=" + GetAD_PInstance_ID();
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("Update Type-2=" + no);
            }  //dtd

            //	Delete rows where nothing to order
            sql = "DELETE FROM T_Replenish "
                + "WHERE QtyToOrder < 1"
                + " AND AD_PInstance_ID=" + GetAD_PInstance_ID();
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("Delete No QtyToOrder=" + no);
            }

            //dtd//	Minimum Order Quantity
            //sql = "UPDATE T_Replenish"
            //    + " SET QtyToOrder = Order_Min "
            //    + "WHERE QtyToOrder < Order_Min"
            //    + " AND AD_PInstance_ID=" + GetAD_PInstance_ID();
            //no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            //if (no != 0)
            //{
            //    log.Fine("Set MinOrderQty=" + no);
            //}

            //	Even dividable by Pack
            sql = "UPDATE T_Replenish"
                + " SET QtyToOrder = QtyToOrder - MOD(QtyToOrder, Order_Pack) + Order_Pack "
                + "WHERE MOD(QtyToOrder, Order_Pack) <> 0"
                + " AND AD_PInstance_ID=" + GetAD_PInstance_ID();
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("Set OrderPackQty=" + no);
            }

            //	Source from other warehouse
            if (wh.GetM_WarehouseSource_ID() != 0)
            {
                sql = "UPDATE T_Replenish"
                    + " SET M_WarehouseSource_ID=" + wh.GetM_WarehouseSource_ID()
                    + " WHERE AD_PInstance_ID=" + GetAD_PInstance_ID();
                no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
                if (no != 0)
                {
                    log.Fine("Set Warehouse Source Warehouse=" + no);
                }
            }
            //	Replenishment on Product level overwrites 
            sql = "UPDATE T_Replenish "
                + "SET M_WarehouseSource_ID=(SELECT M_WarehouseSource_ID FROM M_Replenish r "
                    + "WHERE r.M_Product_ID=T_Replenish.M_Product_ID"
                    + " AND r.M_Warehouse_ID=" + _M_Warehouse_ID + ")"
                + "WHERE AD_PInstance_ID=" + GetAD_PInstance_ID()
                + " AND EXISTS (SELECT * FROM M_Replenish r "
                    + "WHERE r.M_Product_ID=T_Replenish.M_Product_ID"
                    + " AND r.M_Warehouse_ID=" + _M_Warehouse_ID
                    + " AND r.M_WarehouseSource_ID > 0)";
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("Set Product Source Warehouse=" + no);
            }

            //	Check Source Warehouse
            sql = "UPDATE T_Replenish"
                + " SET M_WarehouseSource_ID = NULL "
                + "WHERE M_Warehouse_ID=M_WarehouseSource_ID"
                + " AND AD_PInstance_ID=" + GetAD_PInstance_ID();
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 0)
            {
                log.Fine("Set same Source Warehouse=" + no);
            }

            //	Custom Replenishment
            String className = wh.GetReplenishmentClass();
            if (className == null || className.Length == 0)
            {
                return;
            }
            //	Get Replenishment Class
            ReplenishInterface custom = null;
            try
            {
                //Class<?> clazz = Class.forName(className);
                Type clazz = Type.GetType(className);
                custom = (ReplenishInterface)Activator.CreateInstance(clazz);//.newInstance();
            }
            catch (Exception e)
            {
                throw new Exception("No custom Replenishment class "
                    + className + " - " + e.ToString());
            }

            X_T_Replenish[] replenishs = GetReplenish("ReplenishType='9'");
            for (int i = 0; i < replenishs.Length; i++)
            {
                X_T_Replenish replenish = replenishs[i];
                if (replenish.GetReplenishType().Equals(X_T_Replenish.REPLENISHTYPE_Custom))
                {
                    Decimal? qto = null;
                    try
                    {
                        qto = custom.GetQtyToOrder(wh, replenish);
                    }
                    catch (Exception e)
                    {
                        log.Log(Level.SEVERE, custom.ToString(), e);
                    }
                    if (qto == null)
                    {
                        qto = Env.ZERO;
                    }
                    replenish.SetQtyToOrder(qto);
                    replenish.Save();
                }
            }
            //	fillTable
        }
        /// <summary>
        /// Create PO's
        /// </summary>
        private void CreatePO()
        {
            int noOrders = 0;
            String info = "";
            //
            MOrder order = null;
            MWarehouse wh = null;
            X_T_Replenish[] replenishs = GetReplenish(_M_WareSource);
            for (int i = 0; i < replenishs.Length; i++)
            {
                X_T_Replenish replenish = replenishs[i];
                if (wh == null || wh.GetM_Warehouse_ID() != replenish.GetM_Warehouse_ID())
                {
                    wh = MWarehouse.Get(GetCtx(), replenish.GetM_Warehouse_ID());
                }
                //
                if (order == null
                    || order.GetC_BPartner_ID() != replenish.GetC_BPartner_ID()
                    || order.GetM_Warehouse_ID() != replenish.GetM_Warehouse_ID())
                {
                    order = new MOrder(GetCtx(), 0, Get_TrxName());
                    order.SetIsSOTrx(false);
                    order.SetC_DocTypeTarget_ID(_C_DocType_ID);
                    MBPartner bp = new MBPartner(GetCtx(), replenish.GetC_BPartner_ID(), Get_TrxName());
                    order.SetBPartner(bp);
                    order.SetSalesRep_ID(GetAD_User_ID());
                    order.SetDescription(Msg.GetMsg(GetCtx(), "Replenishment"));
                    //	Set Org/WH
                    order.SetAD_Org_ID(wh.GetAD_Org_ID());
                    order.SetM_Warehouse_ID(wh.GetM_Warehouse_ID());
                   
                    if (!order.Save())
                    {
                        return;
                    }
                    log.Fine(order.ToString());
                    noOrders++;
                    info += " - " + order.GetDocumentNo();
                }
                MOrderLine line = new MOrderLine(order);
                line.SetM_Product_ID(replenish.GetM_Product_ID());
                line.SetQty(replenish.GetQtyToOrder());
                line.SetPrice();
                line.Save();
            }
            _info = "#" + noOrders + info;
            log.Info(_info);
        }	//	createPO

        /// <summary>
        /// Create Requisition
        /// </summary>
        private void CreateRequisition()
        {
            int noReqs = 0;
            String info = "";
            //
            MRequisition requisition = null;
            MWarehouse wh = null;
            X_T_Replenish[] replenishs = GetReplenish(_M_WareSource);
            for (int i = 0; i < replenishs.Length; i++)
            {
                X_T_Replenish replenish = replenishs[i];
                if (wh == null || wh.GetM_Warehouse_ID() != replenish.GetM_Warehouse_ID())
                {
                    wh = MWarehouse.Get(GetCtx(), replenish.GetM_Warehouse_ID());
                }
                //
                if (requisition == null
                    || requisition.GetM_Warehouse_ID() != replenish.GetM_Warehouse_ID())
                {
                    requisition = new MRequisition(GetCtx(), 0, Get_TrxName());
                    requisition.SetAD_User_ID(GetAD_User_ID());
                    requisition.SetC_DocType_ID(_C_DocType_ID);
                    requisition.SetDescription(Msg.GetMsg(GetCtx(), "Replenishment"));
                    //	Set Org/WH
                    int _CountDTD001 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='DTD001_'"));
                    if (_CountDTD001 > 0)
                    {
                        requisition.SetDTD001_MWarehouseSource_ID(wh.GetM_WarehouseSource_ID());
                    }
                    requisition.SetAD_Org_ID(wh.GetAD_Org_ID());
                    requisition.SetM_Warehouse_ID(wh.GetM_Warehouse_ID());
                    
                    if (!requisition.Save())
                    {
                        return;
                    }
                    _DocNo = requisition.GetDocumentNo() + ","; //dtd
                    log.Fine(requisition.ToString());
                    noReqs++;
                    info += " - " + requisition.GetDocumentNo();
                }
                //
                MRequisitionLine line = new MRequisitionLine(requisition);
                line.SetM_Product_ID(replenish.GetM_Product_ID());
                line.SetC_BPartner_ID(replenish.GetC_BPartner_ID());
                line.SetQty(replenish.GetQtyToOrder());
                line.SetPrice();
                line.Save();
            }
            _DocNo = _DocNo.Substring(0, _DocNo.Length - 1);//dtd
            _info = "#" + noReqs + info;
            log.Info(_info);
        }	//	createRequisition

        /// <summary>
        /// Create Inventory Movements
        /// </summary>
        private void CreateMovements()
        {
            int noMoves = 0;
            String info = "";
            //
            MClient client = null;
            MMovement move = null;
            int M_Warehouse_ID = 0;
            int M_WarehouseSource_ID = 0;
            MWarehouse whSource = null;
            MWarehouse whTarget = null;

            string param = "";
            if (_M_WareSource != null)
            {
                param = _M_WareSource;
            }
            else
            {
                param = "M_WarehouseSource_ID IS NOT NULL";
            }
            X_T_Replenish[] replenishs = GetReplenish(param); ;
            for (int i = 0; i < replenishs.Length; i++)
            {
                X_T_Replenish replenish = replenishs[i];
                if (whSource == null || whSource.GetM_WarehouseSource_ID() != replenish.GetM_WarehouseSource_ID())
                {
                    whSource = MWarehouse.Get(GetCtx(), replenish.GetM_WarehouseSource_ID());
                }
                if (whTarget == null || whTarget.GetM_Warehouse_ID() != replenish.GetM_Warehouse_ID())
                {
                    whTarget = MWarehouse.Get(GetCtx(), replenish.GetM_Warehouse_ID());
                }
                if (client == null || client.GetAD_Client_ID() != whSource.GetAD_Client_ID())
                {
                    client = MClient.Get(GetCtx(), whSource.GetAD_Client_ID());
                }
                //
                if (move == null
                    || M_WarehouseSource_ID != replenish.GetM_WarehouseSource_ID()
                    || M_Warehouse_ID != replenish.GetM_Warehouse_ID())
                {
                    M_WarehouseSource_ID = replenish.GetM_WarehouseSource_ID();
                    M_Warehouse_ID = replenish.GetM_Warehouse_ID();

                    move = new MMovement(GetCtx(), 0, Get_TrxName());
                    move.SetC_DocType_ID(_C_DocType_ID);
                    move.SetDescription(Msg.GetMsg(GetCtx(), "Replenishment")
                        + ": " + whSource.GetName() + "->" + whTarget.GetName());
                    //	Set Org
                    move.SetAD_Org_ID(whSource.GetAD_Org_ID());
                    if (!move.Save())
                    {
                        return;
                    }
                    log.Fine(move.ToString());
                    noMoves++;
                    info += " - " + move.GetDocumentNo();
                }
                MProduct product = MProduct.Get(GetCtx(), replenish.GetM_Product_ID());
                //	To
                int M_LocatorTo_ID = GetLocator_ID(product, whTarget);

                //	From: Look-up Storage
                MProductCategory pc = MProductCategory.Get(GetCtx(), product.GetM_Product_Category_ID());
                String MMPolicy = pc.GetMMPolicy();
                if (MMPolicy == null || MMPolicy.Length == 0)
                {
                    MMPolicy = client.GetMMPolicy();
                }
                //
                MStorage[] storages = MStorage.GetWarehouse(GetCtx(),
                    whSource.GetM_Warehouse_ID(), replenish.GetM_Product_ID(), 0, 0,
                    true, null,
                    MClient.MMPOLICY_FiFo.Equals(MMPolicy), Get_TrxName());
                if (storages == null || storages.Length == 0)
                {
                    AddLog("No Inventory in " + whSource.GetName()
                        + " for " + product.GetName());
                    continue;
                }
                //
                Decimal target = replenish.GetQtyToOrder();
                for (int j = 0; j < storages.Length; j++)
                {
                    MStorage storage = storages[j];
                    //if (storage.GetQtyOnHand().signum() <= 0)
                    if (Env.Signum(storage.GetQtyOnHand()) <= 0)
                    {
                        continue;
                    }
                    Decimal moveQty = target;
                    if (storage.GetQtyOnHand().CompareTo(moveQty) < 0)
                    {
                        moveQty = storage.GetQtyOnHand();
                    }
                    //
                    MMovementLine line = new MMovementLine(move);
                    line.SetM_Product_ID(replenish.GetM_Product_ID());
                    line.SetMovementQty(moveQty);
                    if (replenish.GetQtyToOrder().CompareTo(moveQty) != 0)
                    {
                        line.SetDescription("Total: " + replenish.GetQtyToOrder());
                    }
                    line.SetM_Locator_ID(storage.GetM_Locator_ID());		//	from
                    line.SetM_AttributeSetInstance_ID(storage.GetM_AttributeSetInstance_ID());
                    line.SetM_LocatorTo_ID(M_LocatorTo_ID);					//	to
                    line.SetM_AttributeSetInstanceTo_ID(storage.GetM_AttributeSetInstance_ID());
                    line.Save();
                    //
                    //target = target.subtract(moveQty);
                    target = Decimal.Subtract(target, moveQty);
                    //if (target.signum() == 0)
                    if (Env.Signum(target) == 0)
                    {
                        break;
                    }
                }
                if (Env.Signum(target) != 0)
                {
                    AddLog("Insufficient Inventory in " + whSource.GetName()
                        + " for " + product.GetName() + " Qty=" + target);
                }
            }
            if (replenishs.Length == 0)
            {
                _info = "No Source Warehouse";
                log.Warning(_info);
            }
            else
            {
                _info = "#" + noMoves + info;
                log.Info(_info);
            }
        }	//	createRequisition

        /// <summary>
        /// Get Locator_ID
        /// </summary>
        /// <param name="product"> product </param>
        /// <param name="wh">warehouse</param>
        /// <returns>locator with highest priority</returns>
        private int GetLocator_ID(MProduct product, MWarehouse wh)
        {
            int M_Locator_ID = MProductLocator.GetFirstM_Locator_ID(product, wh.GetM_Warehouse_ID());
            /**	
            MLocator[] locators = MProductLocator.getLocators (product, wh.getM_Warehouse_ID());
            for (int i = 0; i < locators.length; i++)
            {
                MLocator locator = locators[i];
                //	Storage/capacity restrictions come here
                return locator.getM_Locator_ID();
            }
            //	default
            **/
            if (M_Locator_ID == 0)
            {
                M_Locator_ID = wh.GetDefaultM_Locator_ID();
            }
            return M_Locator_ID;
        }	//	getLocator_ID


        /// <summary>
        /// Get Replenish Records
        /// </summary>
        /// <param name="where"></param>
        /// <returns>replenish</returns>
        private X_T_Replenish[] GetReplenish(String where)
        {
            String sql = "SELECT * FROM T_Replenish "
                + "WHERE AD_PInstance_ID=@param AND C_BPartner_ID > 0 ";
            if (where != null && where.Length > 0)
            {
                sql += " AND " + where;
            }
            sql += " ORDER BY M_Warehouse_ID, M_WarehouseSource_ID, C_BPartner_ID";
            List<X_T_Replenish> list = new List<X_T_Replenish>();
            SqlParameter[] param = new SqlParameter[1];
            IDataReader idr = null;
            try
            {
                //pstmt = DataBase.prepareStatement (sql, get_TrxName());
                //pstmt.setInt (1, getAD_PInstance_ID());
                param[0] = new SqlParameter("@param", GetAD_PInstance_ID());
                //ResultSet rs = pstmt.executeQuery ();
                idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
                while (idr.Read())
                {
                    list.Add(new X_T_Replenish(GetCtx(), idr, Get_TrxName()));
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            X_T_Replenish[] retValue = new X_T_Replenish[list.Count];
            //list.toArray (retValue);
            retValue = list.ToArray();
            return retValue;
        }	//	getReplenish

    }	//	Replenish

}
