/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : InOutGenerate
 * Purpose        : Generate Shipments.
 *	                Manual or Automatic
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     21-Sep-2009
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
using VAdvantage.ProcessEngine;


namespace ViennaAdvantage.Process
{
    public class InOutGenerate : VAdvantage.ProcessEngine.SvrProcess
    {
        #region Private Variable
        //Manual Selection		
        private bool _selection = false;
        // Warehouse				
        private int _M_Warehouse_ID = 0;
        // BPartner				
        private int _C_BPartner_ID = 0;
        //Promise Date		
        private DateTime? _datePromised = null;
        //Include Orders w. unconfirmed Shipments	
        private bool _isUnconfirmedInOut = false;
        // DocAction				
        private String _docAction = DocActionVariables.ACTION_COMPLETE;
        // Consolidate				
        private bool _consolidateDocument = true;
        //	The current Shipment	
        private MInOut _shipment = null;
        private MOrder order = null;
        // Numner of Shipments		
        private int _created = 0;
        //	Line Number				
        private int _line = 0;
        // Movement Date		
        private DateTime? _movementDate = null;
        //Last BP Location	
        private int _lastC_BPartner_Location_ID = -1;
        //The Query sql		
        StringBuilder _sql = new StringBuilder();
        // Storages temp space				
        //private HashMap<SParameter,MStorage[]> _map = new HashMap<SParameter,MStorage[]>();
        Dictionary<SParameter, MStorage[]> _map = new Dictionary<SParameter, MStorage[]>();
        Dictionary<SParameter, X_M_ContainerStorage[]> _mapContainer = new Dictionary<SParameter, X_M_ContainerStorage[]>();
        // Last Parameter					
        private SParameter _lastPP = null;
        // Last Storage					
        private MStorage[] _lastStorages = null;
        private X_M_ContainerStorage[] _lastContainerStorages = null;
        //StringBuilder
        private StringBuilder _msg = new StringBuilder();
        // LogVariable
        ValueNamePair pp = null;
        // container applicable
        private bool isContainerApplicable = false;

        // Allow Non Item type Product also
        private bool isAllowNonItem = false;

        #endregion

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
                    _M_Warehouse_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("C_BPartner_ID"))
                {
                    _C_BPartner_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("DatePromised"))
                {
                    _datePromised = Util.GetValueOfDateTime(para[i].GetParameter());
                }
                else if (name.Equals("Selection"))
                {
                    _selection = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("IsUnconfirmedInOut"))
                {
                    _isUnconfirmedInOut = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("ConsolidateDocument"))
                {
                    _consolidateDocument = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("DocAction"))
                {
                    _docAction = (String)para[i].GetParameter();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
                //	Login Date
                //_movementDate = new DateTime(GetCtx().GetContextAsTime("#Date"));
                _movementDate = CommonFunctions.CovertMilliToDate(GetCtx().GetContextAsTime("#Date"));
                //	DocAction check
                if (!DocActionVariables.ACTION_COMPLETE.Equals(_docAction))
                {
                    _docAction = DocActionVariables.ACTION_PREPARE;
                }
            }
        }

        /// <summary>
        /// Generate Shipments
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            // check container functionality applicable into system or not
            isContainerApplicable = MTransaction.ProductContainerApplicable(GetCtx());

            // check Allow Non Item type Product setting on Tenant
            isAllowNonItem = Util.GetValueOfString(GetCtx().GetContext("$AllowNonItem")).Equals("Y");

            log.Info("Selection=" + _selection
                + ", M_Warehouse_ID=" + _M_Warehouse_ID
                + ", C_BPartner_ID=" + _C_BPartner_ID
                + ", Consolidate=" + _consolidateDocument
                + ", IsUnconfirmed=" + _isUnconfirmedInOut
                + ", Movement=" + _movementDate);

            if (_M_Warehouse_ID == 0)
            {
                throw new Exception("@NotFound@ @M_Warehouse_ID@");
            }

            if (_selection)	//	VInOutGen
            {
                _sql.Append("SELECT * FROM C_Order "
                    + "WHERE IsSelected='Y' AND DocStatus='CO' AND IsSOTrx='Y' AND AD_Client_ID=" + GetCtx().GetAD_Client_ID()
                   //JID_0444_1 : If there are orders with Advance payment and not paid, system will not create the shipments for that orders but will create the shipment for other orders.
                   + @" AND C_order_ID NOT IN
                      (SELECT C_order_ID FROM VA009_OrderPaySchedule WHERE va009_orderpayschedule.C_Order_ID =C_Order.c_order_id
                      AND va009_orderpayschedule.va009_ispaid = 'N' AND va009_orderpayschedule.isactive = 'Y')");
            }
            else
            {
                _sql.Append("SELECT * FROM C_Order o "
                    + "WHERE DocStatus='CO' AND IsSOTrx='Y'"
                    //	No Offer,POS
                    + " AND o.C_DocType_ID IN (SELECT C_DocType_ID FROM C_DocType "
                        + "WHERE DocBaseType='SOO' AND DocSubTypeSO NOT IN ('ON','OB','WR')) AND AD_Client_ID=" + GetCtx().GetAD_Client_ID()
                    + "	AND o.IsDropShip='N'"
                    //	No Manual
                    + " AND o.DeliveryRule<>'M'"
                   //JID_0444_1 : If there are orders with Advance payment and not paid, system will not create the shipments for that orders but will create the shipment for other orders.
                   + @" AND o.C_order_ID NOT IN
                      (SELECT C_order_ID FROM VA009_OrderPaySchedule WHERE va009_orderpayschedule.C_Order_ID =o.c_order_id
                      AND va009_orderpayschedule.va009_ispaid = 'N' AND va009_orderpayschedule.isactive = 'Y' ) "
                    //	Open Order Lines with Warehouse
                    // JID_0685: If there is sales order with Charge and product that are other than items type that order should not be available at Gemerate Shipment process.
                    + " AND EXISTS (SELECT C_OrderLine_ID FROM C_OrderLine ol ");

                // Get the lines of Order based on the setting taken on Tenant to allow non item Product
                if (!isAllowNonItem)
                {
                    _sql.Append("INNER JOIN M_Product prd ON (ol.M_Product_ID = prd.M_Product_ID AND prd.ProductType = 'I')");
                }

                _sql.Append("WHERE ol.M_Warehouse_ID=" + _M_Warehouse_ID);					//	#1

                if (_datePromised != null)
                {
                    _sql.Append(" AND TRUNC(ol.DatePromised,'DD')<='" + String.Format("{0:dd-MMM-yy}", _datePromised) + "'");		//	#2
                }
                _sql.Append(" AND o.C_Order_ID=ol.C_Order_ID AND ol.QtyOrdered<>ol.QtyDelivered)");
                //
                if (_C_BPartner_ID != 0)
                {
                    _sql.Append(" AND o.C_BPartner_ID=" + _C_BPartner_ID);					//	#3
                }
            }
            _sql.Append(" ORDER BY M_Warehouse_ID, PriorityRule, M_Shipper_ID, C_BPartner_ID, C_BPartner_Location_ID, C_PaymentTerm_ID , C_Order_ID");
            //	_sql += " FOR UPDATE";

            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(_sql.ToString(), null, Get_TrxName());
                DataTable dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                return Generate(dt);
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, _sql.ToString(), e);
                AddLog(e.Message);
                throw new ArgumentException("@Created@ = " + _created);
            }
        }

        /// <summary>
        /// Generate Shipments
        /// </summary>
        /// <param name="idr">Order Query</param>
        /// <returns>info</returns>
        private String Generate(DataTable dt)
        {
            MClient client = MClient.Get(GetCtx());

            foreach (DataRow dr in dt.Rows)		//	Order
            {
                order = new MOrder(GetCtx(), dr, Get_TrxName());

                // Credit Limit check 
                MBPartner bp = MBPartner.Get(GetCtx(), order.GetC_BPartner_ID());
                if (bp.GetCreditStatusSettingOn() == "CH")
                {
                    decimal creditLimit = bp.GetSO_CreditLimit();
                    string creditVal = bp.GetCreditValidation();
                    if (creditLimit != 0)
                    {
                        decimal creditAvlb = creditLimit - bp.GetSO_CreditUsed();
                        if (creditAvlb <= 0)
                        {
                            if (creditVal == "B" || creditVal == "D" || creditVal == "E" || creditVal == "F")
                            {
                                AddLog(Msg.GetMsg(GetCtx(), "StopShipment") + bp.GetName());
                                continue;
                            }
                            else if (creditVal == "H" || creditVal == "J" || creditVal == "K" || creditVal == "L")
                            {
                                if (_msg != null)
                                {
                                    _msg.Clear();
                                }
                                _msg.Append(Msg.GetMsg(GetCtx(), "WarningShipment") + bp.GetName());
                                //AddLog(Msg.GetMsg(GetCtx(), "WarningShipment") + bp.GetName());
                            }
                        }
                    }
                }
                // JID_0161 // change here now will check credit settings on field only on Business Partner Header // Lokesh Chauhan 15 July 2019
                else if (bp.GetCreditStatusSettingOn() == X_C_BPartner.CREDITSTATUSSETTINGON_CustomerLocation)
                {
                    MBPartnerLocation bpl = new MBPartnerLocation(GetCtx(), order.GetC_BPartner_Location_ID(), null);
                    //MBPartner bpartner = MBPartner.Get(GetCtx(), order.GetC_BPartner_ID());
                    //if (bpl.GetCreditStatusSettingOn() == "CL")
                    //{
                    decimal creditLimit = bpl.GetSO_CreditLimit();
                    string creditVal = bpl.GetCreditValidation();
                    if (creditLimit != 0)
                    {
                        decimal creditAvlb = creditLimit - bpl.GetSO_CreditUsed();
                        if (creditAvlb <= 0)
                        {
                            if (creditVal == "B" || creditVal == "D" || creditVal == "E" || creditVal == "F")
                            {
                                AddLog(Msg.GetMsg(GetCtx(), "StopShipment") + bp.GetName() + " " + bpl.GetName());
                                continue;
                            }
                            else if (creditVal == "H" || creditVal == "J" || creditVal == "K" || creditVal == "L")
                            {
                                if (_msg != null)
                                {
                                    _msg.Clear();
                                }
                                _msg.Append(Msg.GetMsg(GetCtx(), "WarningShipment") + bp.GetName() + " " + bpl.GetName());
                                //AddLog(Msg.GetMsg(GetCtx(), "WarningShipment") + bp.GetName() + " " + bpl.GetName());
                            }
                        }
                    }
                    //}
                }
                // Credit Limit End

                //	New Header different Shipper, Shipment Location
                if (!_consolidateDocument
                    || (_shipment != null
                    && (_shipment.GetC_BPartner_Location_ID() != order.GetC_BPartner_Location_ID()
                        || _shipment.GetM_Shipper_ID() != order.GetM_Shipper_ID())))
                {
                    CompleteShipment();
                }
                log.Fine("check: " + order + " - DeliveryRule=" + order.GetDeliveryRule());
                //
                DateTime? minGuaranteeDate = _movementDate;
                bool completeOrder = MOrder.DELIVERYRULE_CompleteOrder.Equals(order.GetDeliveryRule());
                //	OrderLine WHERE
                StringBuilder where = new StringBuilder(" AND M_Warehouse_ID=" + _M_Warehouse_ID);
                if (_datePromised != null)
                {
                    where.Append(" AND (TRUNC(DatePromised,'DD')<=" + DB.TO_DATE((DateTime?)_datePromised, true)
                        + " OR DatePromised IS NULL)");
                    //where += " AND (TRUNC(DatePromised,'DD')<='" + String.Format("{0:dd-MMM-yy}", _datePromised)
                    //    + "' OR DatePromised IS NULL)";
                }
                //	Exclude Auto Delivery if not Force
                if (!MOrder.DELIVERYRULE_Force.Equals(order.GetDeliveryRule()))
                {
                    where.Append(" AND (C_OrderLine.M_Product_ID IS NULL"
                        + " OR EXISTS (SELECT * FROM M_Product p "
                        + "WHERE C_OrderLine.M_Product_ID=p.M_Product_ID"
                        + " AND IsExcludeAutoDelivery='N'))");
                }
                //	Exclude Unconfirmed
                if (!_isUnconfirmedInOut)
                {
                    where.Append(" AND NOT EXISTS (SELECT * FROM M_InOutLine iol"
                            + " INNER JOIN M_InOut io ON (iol.M_InOut_ID=io.M_InOut_ID) "
                                + "WHERE iol.C_OrderLine_ID=C_OrderLine.C_OrderLine_ID AND io.DocStatus IN ('IP','WC'))");
                }

                // Get the lines of Order based on the setting taken on Tenant to allow non item Product
                if (!isAllowNonItem)
                {
                    // JID_1307: Shipment is not generating against the Sales Order which includes combination of ItemType & Service/Expense/Resource type of Products at SO lines
                    where.Append(" AND C_OrderLine_ID IN (SELECT ol.C_OrderLine_ID FROM C_OrderLine ol INNER JOIN M_Product p ON ol.M_Product_ID = p.M_Product_ID WHERE ol.C_Order_ID = "
                    + order.GetC_Order_ID() + " AND p.ProductType = 'I')");
                }

                //	Deadlock Prevention - Order by M_Product_ID
                MOrderLine[] lines = order.GetLines(where.ToString(), "ORDER BY C_BPartner_Location_ID, M_Product_ID");
                for (int i = 0; i < lines.Length; i++)
                {
                    MOrderLine line = lines[i];
                    // if order line is not drop ship type
                    if (!line.IsDropShip())
                    {
                        if (line.GetM_Warehouse_ID() != _M_Warehouse_ID)
                        {
                            continue;
                        }
                        log.Fine("check: " + line);
                        Decimal onHand = Env.ZERO;
                        Decimal toDeliver = Decimal.Subtract(line.GetQtyOrdered(),
                            line.GetQtyDelivered());
                        Decimal QtyNotDelivered = Util.GetValueOfDecimal(DB.ExecuteScalar(@"SELECT SUM(MovementQty) FROM M_Inout i INNER JOIN M_InoutLine il ON i.M_Inout_ID = il.M_Inout_ID
                            WHERE il.C_OrderLine_ID = " + line.GetC_OrderLine_ID() + @" AND il.Isactive = 'Y' AND i.docstatus NOT IN ('RE' , 'VO' , 'CL' , 'CO')", null, Get_Trx()));
                        toDeliver -= QtyNotDelivered;
                        MProduct product = line.GetProduct();
                        //	Nothing to Deliver
                        if (product != null && Env.Signum(toDeliver) == 0)
                        {
                            continue;
                        }

                        // Get the lines of Order based on the setting taken on Tenant to allow non item Product                        
                        if (line.GetC_Charge_ID() != 0 && (!isAllowNonItem || Env.Signum(toDeliver) == 0))        // Nothing to Deliver
                        {
                            continue;
                        }

                        //	Check / adjust for confirmations
                        Decimal unconfirmedShippedQty = Env.ZERO;
                        if (_isUnconfirmedInOut && product != null && Env.Signum(toDeliver) != 0)
                        {
                            String where2 = "EXISTS (SELECT * FROM M_InOut io WHERE io.M_InOut_ID=M_InOutLine.M_InOut_ID AND io.DocStatus IN ('IP','WC'))";
                            MInOutLine[] iols = MInOutLine.GetOfOrderLine(GetCtx(),
                                line.GetC_OrderLine_ID(), where2, null);
                            for (int j = 0; j < iols.Length; j++)
                            {
                                unconfirmedShippedQty = Decimal.Add(unconfirmedShippedQty, iols[j].GetMovementQty());
                            }
                            String logInfo = "Unconfirmed Qty=" + unconfirmedShippedQty
                                + " - ToDeliver=" + toDeliver + "->";
                            toDeliver = Decimal.Subtract(toDeliver, unconfirmedShippedQty);
                            logInfo += toDeliver;
                            if (Env.Signum(toDeliver) < 0)
                            {
                                toDeliver = Env.ZERO;
                                logInfo += " (set to 0)";
                            }
                            //	Adjust On Hand
                            onHand = Decimal.Subtract(onHand, unconfirmedShippedQty);
                            log.Fine(logInfo);
                        }

                        //	Comments & lines w/o product & services
                        if ((product == null || !product.IsStocked())
                            && (Env.Signum(line.GetQtyOrdered()) == 0 	//	comments
                                || Env.Signum(toDeliver) != 0))		//	lines w/o product
                        {
                            if (!MOrder.DELIVERYRULE_CompleteOrder.Equals(order.GetDeliveryRule()))	//	printed later
                            {
                                CreateLine(order, line, toDeliver, null, false, false);
                            }
                            continue;
                        }

                        //	Stored Product
                        MProductCategory pc = MProductCategory.Get(order.GetCtx(), product.GetM_Product_Category_ID());
                        String MMPolicy = pc.GetMMPolicy();
                        if (MMPolicy == null || MMPolicy.Length == 0)
                        {
                            MMPolicy = client.GetMMPolicy();
                        }
                        //
                        dynamic[] storages = null;
                        if (!isContainerApplicable)
                        {
                            storages = GetStorages(line.GetM_Warehouse_ID(),
                               line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
                               product.GetM_AttributeSet_ID(),
                               line.GetM_AttributeSetInstance_ID() == 0,
                               (DateTime?)minGuaranteeDate,
                               MClient.MMPOLICY_FiFo.Equals(MMPolicy));
                        }
                        else
                        {
                            storages = GetContainerStorages(line.GetM_Warehouse_ID(),
                              line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
                              product.GetM_AttributeSet_ID(),
                              line.GetM_AttributeSetInstance_ID() == 0,
                              (DateTime?)minGuaranteeDate,
                              MClient.MMPOLICY_FiFo.Equals(MMPolicy), false, false);
                        }

                        for (int j = 0; j < storages.Length; j++)
                        {
                            dynamic storage = storages[j];
                            onHand = Decimal.Add(onHand, isContainerApplicable ? storage.GetQty() : storage.GetQtyOnHand());
                        }

                        bool fullLine = onHand.CompareTo(toDeliver) >= 0
                            || Env.Signum(toDeliver) < 0;

                        //	Complete Order
                        if (completeOrder && !fullLine)
                        {
                            log.Fine("Failed CompleteOrder - OnHand=" + onHand
                                + " (Unconfirmed=" + unconfirmedShippedQty
                                + "), ToDeliver=" + toDeliver + " - " + line);
                            completeOrder = false;
                            break;
                        }
                        //	Complete Line
                        else if (fullLine && MOrder.DELIVERYRULE_CompleteLine.Equals(order.GetDeliveryRule()))
                        {
                            log.Fine("CompleteLine - OnHand=" + onHand
                                + " (Unconfirmed=" + unconfirmedShippedQty
                                + ", ToDeliver=" + toDeliver + " - " + line);
                            //	
                            CreateLine(order, line, toDeliver, storages, false, MClient.MMPOLICY_FiFo.Equals(MMPolicy));
                        }
                        //	Availability
                        else if (MOrder.DELIVERYRULE_Availability.Equals(order.GetDeliveryRule())
                            && (Env.Signum(onHand) > 0
                                || Env.Signum(toDeliver) < 0))
                        {
                            Decimal deliver = toDeliver;
                            if (deliver.CompareTo(onHand) > 0)
                                deliver = onHand;
                            log.Fine("Available - OnHand=" + onHand
                                + " (Unconfirmed=" + unconfirmedShippedQty
                                + "), ToDeliver=" + toDeliver
                                + ", Delivering=" + deliver + " - " + line);
                            //	
                            CreateLine(order, line, deliver, storages, false, MClient.MMPOLICY_FiFo.Equals(MMPolicy));
                        }
                        //	Force
                        else if (MOrder.DELIVERYRULE_Force.Equals(order.GetDeliveryRule()))
                        {
                            Decimal deliver = toDeliver;
                            log.Fine("Force - OnHand=" + onHand
                                + " (Unconfirmed=" + unconfirmedShippedQty
                                + "), ToDeliver=" + toDeliver
                                + ", Delivering=" + deliver + " - " + line);
                            //	
                            CreateLine(order, line, deliver, storages, true, MClient.MMPOLICY_FiFo.Equals(MMPolicy));
                        }
                        //	Manual
                        else if (MOrder.DELIVERYRULE_Manual.Equals(order.GetDeliveryRule()))
                            log.Fine("Manual - OnHand=" + onHand
                                + " (Unconfirmed=" + unconfirmedShippedQty
                                + ") - " + line);
                        else
                            log.Fine("Failed: " + order.GetDeliveryRule() + " - OnHand=" + onHand
                                + " (Unconfirmed=" + unconfirmedShippedQty
                                + "), ToDeliver=" + toDeliver + " - " + line);
                    }
                }//	for all order lines

                //	Complete Order successful
                if (completeOrder && MOrder.DELIVERYRULE_CompleteOrder.Equals(order.GetDeliveryRule()))
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        MOrderLine line = lines[i];
                        // if order line is not drop ship type
                        if (!line.IsDropShip())
                        {
                            if (line.GetM_Warehouse_ID() != _M_Warehouse_ID)
                                continue;
                            MProduct product = line.GetProduct();
                            Decimal toDeliver = Decimal.Subtract(line.GetQtyOrdered(), line.GetQtyDelivered());
                            //
                            dynamic[] storages = null;
                            String MMPolicy = null;
                            if (product != null && product.IsStocked())
                            {
                                MProductCategory pc = MProductCategory.Get(order.GetCtx(), product.GetM_Product_Category_ID());
                                MMPolicy = pc.GetMMPolicy();
                                if (MMPolicy == null || MMPolicy.Length == 0)
                                {
                                    MMPolicy = client.GetMMPolicy();
                                }
                                //
                                if (!isContainerApplicable)
                                {
                                    storages = GetStorages(line.GetM_Warehouse_ID(),
                                        line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
                                        product.GetM_AttributeSet_ID(),
                                        line.GetM_AttributeSetInstance_ID() == 0, (DateTime?)minGuaranteeDate,
                                        MClient.MMPOLICY_FiFo.Equals(MMPolicy));
                                }
                                else
                                {
                                    storages = GetContainerStorages(line.GetM_Warehouse_ID(),
                                      line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
                                      product.GetM_AttributeSet_ID(),
                                      line.GetM_AttributeSetInstance_ID() == 0,
                                      (DateTime?)minGuaranteeDate,
                                      MClient.MMPOLICY_FiFo.Equals(MMPolicy), false, false);
                                }
                            }
                            //	
                            CreateLine(order, line, toDeliver, storages, false, MClient.MMPOLICY_FiFo.Equals(MMPolicy));
                        }
                    }
                }
                _line += 10;
            }	//	while order
            CompleteShipment();
            return "@Created@ = " + _created;
        }

        /// <summary>
        /// Create Line
        /// </summary>
        /// <param name="order">order</param>
        /// <param name="orderLine">line</param>
        /// <param name="qty">Quantity</param>
        /// <param name="storages">storage info</param>
        /// <param name="force">force delivery</param>
        private void CreateLine(MOrder order, MOrderLine orderLine, Decimal qty,
            dynamic[] storages, bool force, bool FiFo)
        {
            //	Complete last Shipment - can have multiple shipments
            if (_lastC_BPartner_Location_ID != orderLine.GetC_BPartner_Location_ID())
            {
                CompleteShipment();
            }
            _lastC_BPartner_Location_ID = orderLine.GetC_BPartner_Location_ID();
            //	Create New Shipment
            if (_shipment == null)
            {
                _shipment = new MInOut(order, 0, _movementDate);
                _shipment.SetM_Warehouse_ID(orderLine.GetM_Warehouse_ID());	//	sets Org too
                if (order.GetC_BPartner_ID() != orderLine.GetC_BPartner_ID())
                {
                    _shipment.SetC_BPartner_ID(orderLine.GetC_BPartner_ID());
                }
                if (order.GetC_BPartner_Location_ID() != orderLine.GetC_BPartner_Location_ID())
                {
                    _shipment.SetC_BPartner_Location_ID(orderLine.GetC_BPartner_Location_ID());
                }

                // Added by Bharat on 29 Jan 2018 to set Inco Term from Order
                if (_shipment.Get_ColumnIndex("C_IncoTerm_ID") > 0)
                {
                    _shipment.SetC_IncoTerm_ID(order.GetC_IncoTerm_ID());
                }

                if (Env.IsModuleInstalled("VA077_"))
                {
                    //shipment  fields copied
                    _shipment.Set_Value("VA077_HistoricContractDate", order.Get_Value("VA077_HistoricContractDate"));
                    _shipment.Set_Value("VA077_ChangeStartDate", order.Get_Value("VA077_ChangeStartDate"));
                    _shipment.Set_Value("VA077_ContractCPStartDate", order.Get_Value("VA077_ContractCPStartDate"));
                    _shipment.Set_Value("VA077_ContractCPEndDate", order.Get_Value("VA077_ContractCPEndDate"));
                    _shipment.Set_Value("VA077_PartialAmtCatchUp", order.Get_Value("VA077_PartialAmtCatchUp"));

                    _shipment.Set_Value("VA077_OldAnnualContractTotal", order.Get_Value("VA077_OldAnnualContractTotal"));
                    _shipment.Set_Value("VA077_AdditionalAnnualCharge", order.Get_Value("VA077_AdditionalAnnualCharge"));
                    _shipment.Set_Value("VA077_NewAnnualContractTotal", order.Get_Value("VA077_NewAnnualContractTotal"));
                    _shipment.Set_Value("VA077_SalesCoWorker", order.Get_Value("VA077_SalesCoWorker"));

                    _shipment.Set_Value("VA077_SalesCoWorkerPer", order.Get_Value("VA077_SalesCoWorkerPer"));
                    _shipment.Set_Value("VA077_TotalMarginAmt", order.Get_Value("VA077_TotalMarginAmt"));
                    _shipment.Set_Value("VA077_TotalPurchaseAmt", order.Get_Value("VA077_TotalPurchaseAmt"));
                    _shipment.Set_Value("VA077_TotalSalesAmt", order.Get_Value("VA077_TotalSalesAmt"));

                    _shipment.Set_Value("VA077_MarginPercent", order.Get_Value("VA077_MarginPercent"));
                    _shipment.Set_Value("VA077_IsLegalEntity", order.Get_Value("VA077_IsLegalEntity"));

                }




                if (!_shipment.Save())
                {
                    //throw new Exception("Could not create Shipment");
                    log.Fine("Failed: Could not create Shipment against Order No : " + order.GetDocumentNo());

                    // JID_0405: In the error message it should show the Name of the Product for which qty is not available.
                    pp = VLogger.RetrieveError();
                    if (pp != null)
                    {
                        _msg.Append(!string.IsNullOrEmpty(pp.GetName()) ? pp.GetName() : Msg.GetMsg(GetCtx(), pp.GetValue()));
                    }
                    return;
                }
            }
            //	Non Inventory Lines
            if (storages == null || storages.Count() == 0)
            {
                #region Non Inventory Lines
                MInOutLine line = new MInOutLine(_shipment);
                line.SetOrderLine(orderLine, 0, Env.ZERO);
                line.SetQty(qty);	//	Correct UOM
                if (orderLine.GetQtyEntered().CompareTo(orderLine.GetQtyOrdered()) != 0)
                {
                    line.SetQtyEntered(Decimal.Round(Decimal.Divide(Decimal.Multiply(qty,
                        orderLine.GetQtyEntered()), orderLine.GetQtyOrdered()),
                        12, MidpointRounding.AwayFromZero));
                }
                line.SetLine(_line + orderLine.GetLine());

                if (Env.IsModuleInstalled("DTD001_"))
                {
                    line.SetDTD001_IsAttributeNo(true);
                }

                //190 - Set Print Description
                if (line.Get_ColumnIndex("PrintDescription") >= 0)
                    line.Set_Value("PrintDescription", orderLine.Get_Value("PrintDescription"));

                if (Env.IsModuleInstalled("VA077_"))
                {
                    line.Set_Value("VA077_SerialNo", orderLine.Get_Value("VA077_SerialNo"));
                    line.Set_Value("VA077_OldSN", orderLine.Get_Value("VA077_OldSN"));
                    line.Set_Value("VA077_ServiceContract_ID", orderLine.Get_Value("VA077_ServiceContract_ID"));
                    line.Set_Value("VA077_UserRef_ID", orderLine.Get_Value("VA077_UserRef_ID"));
                    line.Set_Value("VA077_Duration", orderLine.Get_Value("VA077_Duration"));
                    line.Set_Value("VA077_CNAutodesk", orderLine.Get_Value("VA077_CNAutodesk"));
                    line.Set_Value("VA077_RegEmail", orderLine.Get_Value("VA077_RegEmail"));
                    line.Set_Value("VA077_UpdateFromVersn", orderLine.Get_Value("VA077_UpdateFromVersn"));
                    line.Set_Value("VA077_ProductInfo", orderLine.Get_Value("VA077_ProductInfo"));
                    line.Set_Value("VA077_MarginPercent", orderLine.Get_Value("VA077_MarginPercent"));
                    line.Set_Value("VA077_MarginAmt", orderLine.Get_Value("VA077_MarginAmt"));
                    line.Set_Value("VA077_PurchasePrice", orderLine.Get_Value("VA077_PurchasePrice"));
                    line.Set_Value("VA077_StartDate", orderLine.Get_Value("VA077_StartDate"));
                    line.Set_Value("VA077_EndDate", orderLine.Get_Value("VA077_EndDate"));

                }

                if (!line.Save())
                {
                    //throw new Exception("Could not create Shipment Line");
                    log.Fine("Failed: Could not create Shipment Line against Order No : " + order.GetDocumentNo() + " for orderline id : " + orderLine.GetC_OrderLine_ID());

                    // JID_0405: In the error message it should show the Name of the Product for which qty is not available.
                    pp = VLogger.RetrieveError();
                    if (pp != null)
                    {
                        _msg.Append(!string.IsNullOrEmpty(pp.GetName()) ? pp.GetName() : pp.GetValue());
                    }
                    return;
                }
                log.Fine(line.ToString());
                return;
                #endregion
            }

            //	Product
            MProduct product = orderLine.GetProduct();
            bool linePerASI = false;

            // need to consolidate record every time when locator and product container matched
            //if (product.GetM_AttributeSet_ID() != 0)
            //{
            //    MAttributeSet mas = MAttributeSet.Get(GetCtx(), product.GetM_AttributeSet_ID());
            //    linePerASI = mas.IsInstanceAttribute();
            //}

            //	Inventory Lines
            if (isContainerApplicable)
            {
                #region Container applicable
                List<MInOutLine> list = new List<MInOutLine>();

                // qty to be delivered
                Decimal toDeliver = qty;

                for (int i = 0; i < storages.Length; i++)
                {
                    if (Env.Signum(toDeliver) == 0)	//	zero deliver
                        continue;

                    dynamic storage = storages[i];

                    #region when data found on container storage
                    Decimal deliver = toDeliver;
                    Decimal containerQty = Util.GetValueOfDecimal(storage.GetQty());
                    int M_ProductContainer_ID = Util.GetValueOfInt(storage.GetM_ProductContainer_ID());
                    // when container qty is less than ZERO, then not to consider that line
                    if (containerQty < 0) continue;

                    //	when deliver qty > container qty, and if system traverse last record of storage loop then make deliver = containerQty
                    if (deliver.CompareTo(containerQty) > 0)
                    {
                        if (!force	//	Adjust to OnHand Qty  
                           || (i + 1 != storages.Length))	//	if force don't adjust last location
                            deliver = containerQty;
                    }

                    if (Env.Signum(deliver) == 0)	//	zero deliver
                    {
                        continue;
                    }

                    int M_Locator_ID = storage.GetM_Locator_ID();

                    //
                    MInOutLine line = null;
                    if (!linePerASI)	//	find line with Locator, AttributeSetInsatnce and ProductContainer
                    {
                        for (int n = 0; n < list.Count; n++)
                        {
                            MInOutLine test = (MInOutLine)list[n];
                            if (test.GetM_Locator_ID() == M_Locator_ID
                                && test.GetM_ProductContainer_ID() == M_ProductContainer_ID
                                && test.GetM_AttributeSetInstance_ID() == storage.GetM_AttributeSetInstance_ID())
                            {
                                line = test;
                                break;
                            }
                        }
                    }
                    if (line == null)	//	new line
                    {
                        line = new MInOutLine(_shipment);
                        line.SetOrderLine(orderLine, M_Locator_ID, order.IsSOTrx() ? deliver : Env.ZERO);
                        line.SetQty(deliver);
                        line.SetM_ProductContainer_ID(M_ProductContainer_ID);
                        list.Add(line);
                    }
                    else
                    {
                        //	existing line
                        line.SetQty(Decimal.Add(line.GetMovementQty(), deliver));
                    }
                    if (orderLine.GetQtyEntered().CompareTo(orderLine.GetQtyOrdered()) != 0)
                    {
                        line.SetQtyEntered(Decimal.Round(Decimal.Divide(Decimal.Multiply(line.GetMovementQty(), orderLine.GetQtyEntered()),
                            orderLine.GetQtyOrdered()), 12, MidpointRounding.AwayFromZero));
                    }

                    line.SetLine(_line + orderLine.GetLine());
                    line.SetM_AttributeSetInstance_ID(storage.GetM_AttributeSetInstance_ID());

                    if (Env.IsModuleInstalled("DTD001_"))
                    {
                        line.SetDTD001_IsAttributeNo(true);
                    }

                    //190 - Set Print Description
                    if (line.Get_ColumnIndex("PrintDescription") >= 0)
                        line.Set_Value("PrintDescription", orderLine.Get_Value("PrintDescription"));

                    if (Env.IsModuleInstalled("VA077_"))
                    {
                        line.Set_Value("VA077_SerialNo", orderLine.Get_Value("VA077_SerialNo"));
                        line.Set_Value("VA077_OldSN", orderLine.Get_Value("VA077_OldSN"));
                        line.Set_Value("VA077_ServiceContract_ID", orderLine.Get_Value("VA077_ServiceContract_ID"));
                        line.Set_Value("VA077_UserRef_ID", orderLine.Get_Value("VA077_UserRef_ID"));
                        line.Set_Value("VA077_Duration", orderLine.Get_Value("VA077_Duration"));

                        line.Set_Value("VA077_CNAutodesk", orderLine.Get_Value("VA077_CNAutodesk"));
                        line.Set_Value("VA077_RegEmail", orderLine.Get_Value("VA077_RegEmail"));
                        line.Set_Value("VA077_UpdateFromVersn", orderLine.Get_Value("VA077_UpdateFromVersn"));
                        line.Set_Value("VA077_ProductInfo", orderLine.Get_Value("VA077_ProductInfo"));
                        line.Set_Value("VA077_MarginPercent", orderLine.Get_Value("VA077_MarginPercent"));

                        line.Set_Value("VA077_MarginAmt", orderLine.Get_Value("VA077_MarginAmt"));
                        line.Set_Value("VA077_PurchasePrice", orderLine.Get_Value("VA077_PurchasePrice"));
                        line.Set_Value("VA077_StartDate", orderLine.Get_Value("VA077_StartDate"));
                        line.Set_Value("VA077_EndDate", orderLine.Get_Value("VA077_EndDate"));

                    }

                    if (!line.Save())
                    {
                        log.Fine("Failed: Could not create Shipment Line against Order No : " + order.GetDocumentNo() + " for orderline id : " + orderLine.GetC_OrderLine_ID());
                        pp = VLogger.RetrieveError();
                        if (pp != null)
                        {
                            _msg.Append(!string.IsNullOrEmpty(pp.GetName()) ? pp.GetName() : pp.GetValue());
                        }
                        continue;
                    }
                    log.Fine("ToDeliver=" + qty + "/" + deliver + " - " + line);
                    toDeliver = Decimal.Subtract(toDeliver, deliver);
                    //	Temp adjustment
                    //storage.SetQtyOnHand(Decimal.Subtract(storage.GetQtyOnHand(), deliver));
                    storage.SetQty(Decimal.Subtract(storage.GetQty(), deliver));
                    //
                    if (Env.Signum(toDeliver) == 0)
                    {
                        break;
                    }
                    #endregion
                }
                if (Env.Signum(toDeliver) != 0)
                {
                    throw new Exception("Not All Delivered - Remainder=" + toDeliver);
                }
                #endregion
            }
            else
            {
                #region normal Flow
                List<MInOutLine> list = new List<MInOutLine>();
                Decimal toDeliver = qty;
                for (int i = 0; i < storages.Length; i++)
                {
                    MStorage storage = storages[i];
                    Decimal deliver = toDeliver;
                    //	Not enough On Hand
                    if (deliver.CompareTo(storage.GetQtyOnHand()) > 0
                        && Env.Signum(storage.GetQtyOnHand()) >= 0)		//	positive storage
                    {
                        if (!force	//	Adjust to OnHand Qty  
                            || (i + 1 != storages.Length))	//	if force don't adjust last location
                            deliver = storage.GetQtyOnHand();
                    }
                    if (Env.Signum(deliver) == 0)	//	zero deliver
                    {
                        continue;
                    }

                    int M_Locator_ID = storage.GetM_Locator_ID();
                    //
                    MInOutLine line = null;
                    if (!linePerASI)	//	find line with Locator
                    {
                        for (int n = 0; n < list.Count; n++)
                        {
                            MInOutLine test = (MInOutLine)list[n];
                            if (test.GetM_Locator_ID() == M_Locator_ID)
                            {
                                line = test;
                                break;
                            }
                        }
                    }
                    if (line == null)	//	new line
                    {
                        line = new MInOutLine(_shipment);
                        line.SetOrderLine(orderLine, M_Locator_ID, order.IsSOTrx() ? deliver : Env.ZERO);
                        line.SetQty(deliver);
                        list.Add(line);
                    }
                    else
                    {
                        //	existing line
                        line.SetQty(Decimal.Add(line.GetMovementQty(), deliver));
                    }
                    if (orderLine.GetQtyEntered().CompareTo(orderLine.GetQtyOrdered()) != 0)
                    {
                        line.SetQtyEntered(Decimal.Round(Decimal.Divide(Decimal.Multiply(line.GetMovementQty(), orderLine.GetQtyEntered()),
                            orderLine.GetQtyOrdered()), 12, MidpointRounding.AwayFromZero));
                    }

                    line.SetLine(_line + orderLine.GetLine());
                    //if (linePerASI)
                    //{
                    line.SetM_AttributeSetInstance_ID(storage.GetM_AttributeSetInstance_ID());
                    //}

                    if (Env.IsModuleInstalled("DTD001_"))
                    {
                        line.SetDTD001_IsAttributeNo(true);
                    }
                    //190 - Set Print Description
                    if (line.Get_ColumnIndex("PrintDescription") >= 0)
                        line.Set_Value("PrintDescription", orderLine.Get_Value("PrintDescription"));

                    if (Env.IsModuleInstalled("VA077_"))
                    {
                        line.Set_Value("VA077_SerialNo", orderLine.Get_Value("VA077_SerialNo"));
                        line.Set_Value("VA077_OldSN", orderLine.Get_Value("VA077_OldSN"));
                        line.Set_Value("VA077_ServiceContract_ID", orderLine.Get_Value("VA077_ServiceContract_ID"));
                        line.Set_Value("VA077_UserRef_ID", orderLine.Get_Value("VA077_UserRef_ID"));
                        line.Set_Value("VA077_Duration", orderLine.Get_Value("VA077_Duration"));

                        line.Set_Value("VA077_CNAutodesk", orderLine.Get_Value("VA077_CNAutodesk"));
                        line.Set_Value("VA077_RegEmail", orderLine.Get_Value("VA077_RegEmail"));
                        line.Set_Value("VA077_UpdateFromVersn", orderLine.Get_Value("VA077_UpdateFromVersn"));
                        line.Set_Value("VA077_ProductInfo", orderLine.Get_Value("VA077_ProductInfo"));
                        line.Set_Value("VA077_MarginPercent", orderLine.Get_Value("VA077_MarginPercent"));

                        line.Set_Value("VA077_MarginAmt", orderLine.Get_Value("VA077_MarginAmt"));
                        line.Set_Value("VA077_PurchasePrice", orderLine.Get_Value("VA077_PurchasePrice"));
                        line.Set_Value("VA077_StartDate", orderLine.Get_Value("VA077_StartDate"));
                        line.Set_Value("VA077_EndDate", orderLine.Get_Value("VA077_EndDate"));

                    }

                    if (!line.Save())
                    {
                        //throw new Exception("Could not create Shipment Line");
                        log.Fine("Failed: Could not create Shipment Line against Order No : " + order.GetDocumentNo() + " for orderline id : " + orderLine.GetC_OrderLine_ID());

                        // JID_0405: In the error message it should show the Name of the Product for which qty is not available.
                        pp = VLogger.RetrieveError();
                        if (pp != null)
                        {
                            _msg.Append(!string.IsNullOrEmpty(pp.GetName()) ? pp.GetName() : pp.GetValue());
                        }
                    }
                    log.Fine("ToDeliver=" + qty + "/" + deliver + " - " + line);
                    toDeliver = Decimal.Subtract(toDeliver, deliver);
                    //	Temp adjustment
                    storage.SetQtyOnHand(Decimal.Subtract(storage.GetQtyOnHand(), deliver));
                    //
                    if (Env.Signum(toDeliver) == 0)
                    {
                        break;
                    }
                }
                if (Env.Signum(toDeliver) != 0)
                {
                    throw new Exception("Not All Delivered - Remainder=" + toDeliver);
                }
                #endregion
            }
        }

        /// <summary>
        /// Get Storages
        /// </summary>
        /// <param name="M_Warehouse_ID">Ware House ID</param>
        /// <param name="M_Product_ID">Product id</param>
        /// <param name="M_AttributeSetInstance_ID">Attribute Set instance</param>
        /// <param name="M_AttributeSet_ID">attribute id</param>
        /// <param name="allAttributeInstances">all atribute</param>
        /// <param name="minGuaranteeDate"></param>
        /// <param name="FiFo"></param>
        /// <returns>storages</returns>
        private MStorage[] GetStorages(int M_Warehouse_ID,
            int M_Product_ID, int M_AttributeSetInstance_ID, int M_AttributeSet_ID,
            bool allAttributeInstances, DateTime? minGuaranteeDate,
            bool FiFo)
        {

            _lastPP = new SParameter(M_Warehouse_ID,
                M_Product_ID, M_AttributeSetInstance_ID, M_AttributeSet_ID,
                allAttributeInstances, minGuaranteeDate, FiFo);
            //m_lastStorages = m_map.get(m_lastPP);
            if (_map.ContainsKey(_lastPP))
            {
                _lastStorages = _map[_lastPP];
            }
            else
            {
                _lastStorages = null;
            }

            if (_lastStorages == null)
            {
                _lastStorages = MStorage.GetWarehouse(GetCtx(),
                    M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID,
                    M_AttributeSet_ID, allAttributeInstances, minGuaranteeDate,
                    FiFo, Get_TrxName());

                try
                {
                    _map.Add(_lastPP, _lastStorages);
                }
                catch (Exception exp)
                {
                    //// MessageBox.Show(exp.ToString());
                    log.Severe(exp.ToString());
                }
            }

            return _lastStorages;
        }

        /// <summary>
        /// Get Container Storages
        /// </summary>
        /// <param name="M_Warehouse_ID">WareHouse ID</param>
        /// <param name="M_Product_ID">Product id</param>
        /// <param name="M_AttributeSetInstance_ID">Attribute Set instance</param>
        /// <param name="M_AttributeSet_ID">attribute id</param>
        /// <param name="allAttributeInstances">all atribute</param>
        /// <param name="minGuaranteeDate">current date</param>
        /// <param name="FiFo">material policy</param>
        /// <param name="greater">if false, then pick record upto current date</param>
        /// <param name="isContainerConsider">record to be filtered based on Container or not</param>
        /// <returns>container storages</returns>
        private X_M_ContainerStorage[] GetContainerStorages(int M_Warehouse_ID, int M_Product_ID, int M_AttributeSetInstance_ID, int M_AttributeSet_ID,
                                                              bool allAttributeInstances, DateTime? minGuaranteeDate, bool FiFo, bool greater, bool isContainerConsider)
        {

            _lastPP = new SParameter(M_Warehouse_ID,
                M_Product_ID, M_AttributeSetInstance_ID, M_AttributeSet_ID,
                allAttributeInstances, minGuaranteeDate, FiFo);
            //m_lastStorages = m_map.get(m_lastPP);
            if (_mapContainer.ContainsKey(_lastPP))
            {
                _lastContainerStorages = _mapContainer[_lastPP];
            }
            else
            {
                _lastContainerStorages = null;
            }

            if (_lastContainerStorages == null)
            {
                _lastContainerStorages = MProductContainer.GetContainerStorage(GetCtx(), M_Warehouse_ID, 0, 0,
                              M_Product_ID, M_AttributeSetInstance_ID,
                              M_AttributeSet_ID,
                              allAttributeInstances,
                              (DateTime?)minGuaranteeDate,
                              FiFo, false, Get_Trx(), false);

                try
                {
                    _mapContainer.Add(_lastPP, _lastContainerStorages);
                }
                catch (Exception exp)
                {
                    //// MessageBox.Show(exp.ToString());
                    log.Severe(exp.ToString());
                }
            }

            return _lastContainerStorages;
        }

        /// <summary>
        /// Complete Shipment
        /// </summary>
        private void CompleteShipment()
        {
            if (_shipment != null)
            {
                //	Fails if there is a confirmation
                if (!_shipment.ProcessIt(_docAction))
                {
                    _shipment.Get_TrxName().Rollback();
                    log.Warning("Failed: " + _shipment);
                }
                _shipment.Save();
                //
                if ((_msg != null && _msg.Length > 0) || !String.IsNullOrEmpty(_shipment.GetProcessMsg()))
                {
                    MOrder OrderDoc = new MOrder(GetCtx(), _shipment.GetC_Order_ID(), _shipment.Get_TrxName());
                    AddLog(_shipment.GetM_InOut_ID(), _shipment.GetMovementDate(), null,
                        (_msg + "  " +
                        (_shipment.GetProcessMsg() != "" ? (_shipment.GetProcessMsg() + " " + (!_consolidateDocument ? OrderDoc.GetDocumentNo() : " ")) : _shipment.GetDocumentNo())));
                }
                else
                {
                    AddLog(_shipment.GetM_InOut_ID(), _shipment.GetMovementDate(), null, _shipment.GetDocumentNo());
                    _created++;
                }

                if (isContainerApplicable)
                {
                    _mapContainer.Clear();
                    _mapContainer = null;
                    _mapContainer = new Dictionary<SParameter, X_M_ContainerStorage[]>();
                    if (_lastPP != null && _lastContainerStorages != null)
                    {
                        _mapContainer.Add(_lastPP, _lastContainerStorages);
                    }
                }
                else
                {
                    _map = new Dictionary<SParameter, MStorage[]>();
                    if (_lastPP != null && _lastStorages != null)
                    {
                        _map.Add(_lastPP, _lastStorages);
                    }
                }
                _shipment.Get_TrxName().Commit();
            }
            _shipment = null;
            _line = 0;
        }

        /// <summary>
        /// InOutGenerate Parameter
        /// </summary>
        public class SParameter
        {
            #region Private Variables
            //Warehouse		
            public int M_Warehouse_ID;
            // Product			
            public int M_Product_ID;
            // ASI				
            public int M_AttributeSetInstance_ID;
            // AS				
            public int M_AttributeSet_ID;
            // All instances	
            public bool allAttributeInstances;
            //Mon Guarantee Date	
            public DateTime? minGuaranteeDate;
            // FiFo
            public bool FiFo;
            #endregion

            /// <summary>
            /// Parameter
            /// </summary>
            /// <param name="p_Warehouse_ID">warehouse</param>
            /// <param name="p_Product_ID">Product_ID</param>
            /// <param name="p_AttributeSetInstance_ID">AttributeSetInstance_ID</param>
            /// <param name="p_AttributeSet_ID">AttributeSet_ID</param>
            /// <param name="p_allAttributeInstances">allAttributeInstances</param>
            /// <param name="p_minGuaranteeDate">minGuaranteeDate</param>
            /// <param name="p_FiFo">FiFo</param>
            public SParameter(int p_Warehouse_ID,
                int p_Product_ID, int p_AttributeSetInstance_ID, int p_AttributeSet_ID,
                bool p_allAttributeInstances, DateTime? p_minGuaranteeDate,
                bool p_FiFo)
            {
                this.M_Warehouse_ID = p_Warehouse_ID;
                this.M_Product_ID = p_Product_ID;
                this.M_AttributeSetInstance_ID = p_AttributeSetInstance_ID;
                this.M_AttributeSet_ID = p_AttributeSet_ID;
                this.allAttributeInstances = p_allAttributeInstances;
                this.minGuaranteeDate = p_minGuaranteeDate;
                this.FiFo = p_FiFo;
            }

            /// <summary>
            /// Equals
            /// </summary>
            /// <param name="obj">Object</param>
            /// <returns>true if equal</returns>
            public override bool Equals(Object obj)
            {
                if (obj != null && obj is SParameter)
                {
                    SParameter cmp = (SParameter)obj;
                    bool eq = cmp.M_Warehouse_ID == M_Warehouse_ID
                        && cmp.M_Product_ID == M_Product_ID
                        && cmp.M_AttributeSetInstance_ID == M_AttributeSetInstance_ID
                        && cmp.M_AttributeSet_ID == M_AttributeSet_ID
                        && cmp.allAttributeInstances == allAttributeInstances
                        && cmp.FiFo == FiFo;
                    if (eq)
                    {
                        if (cmp.minGuaranteeDate == null && minGuaranteeDate == null)
                        {
                            ;
                        }
                        else if (cmp.minGuaranteeDate != null && minGuaranteeDate != null
                            && cmp.minGuaranteeDate.Equals(minGuaranteeDate))
                        {
                            ;
                        }
                        else
                        {
                            eq = false;
                        }
                    }
                    return eq;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode();
            }

            /// <summary>
            /// hashCode
            /// </summary>
            /// <returns>hash code</returns>
            public int HashCode()
            {
                long hash = M_Warehouse_ID
                    + (M_Product_ID * 2)
                    + (M_AttributeSetInstance_ID * 3)
                    + (M_AttributeSet_ID * 4);

                if (allAttributeInstances)
                {
                    hash *= -1;
                }
                if (FiFo)
                {
                    ;
                }
                hash *= -2;
                if (hash < 0)
                {
                    hash = -hash + 7;
                }
                while (hash > int.MaxValue)
                {
                    hash -= int.MaxValue;
                }
                //
                if (minGuaranteeDate != null)
                {
                    hash += minGuaranteeDate.GetHashCode();//.hashCode();
                    while (hash > int.MaxValue)
                    {
                        hash -= int.MaxValue;
                    }
                }

                return (int)hash;
            }

        }
    }
}
