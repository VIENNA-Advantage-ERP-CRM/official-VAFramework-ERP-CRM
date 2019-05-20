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
        private String _sql = null;
        // Storages temp space				
        //private HashMap<SParameter,MStorage[]> _map = new HashMap<SParameter,MStorage[]>();
        Dictionary<SParameter, MStorage[]> _map = new Dictionary<SParameter, MStorage[]>();
        // Last Parameter					
        private SParameter _lastPP = null;
        // Last Storage					
        private MStorage[] _lastStorages = null;
        //StringBuilder
        private StringBuilder _msg = new StringBuilder();
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
                _sql = "SELECT * FROM C_Order "
                    + "WHERE IsSelected='Y' AND DocStatus='CO' AND IsSOTrx='Y' AND AD_Client_ID=" + GetCtx().GetAD_Client_ID();
            }
            else
            {
                _sql = "SELECT * FROM C_Order o "
                    + "WHERE DocStatus='CO' AND IsSOTrx='Y'"
                    //	No Offer,POS
                    + " AND o.C_DocType_ID IN (SELECT C_DocType_ID FROM C_DocType "
                        + "WHERE DocBaseType='SOO' AND DocSubTypeSO NOT IN ('ON','OB','WR')) AND AD_Client_ID=" + GetCtx().GetAD_Client_ID()
                    + "	AND o.IsDropShip='N'"
                    //	No Manual
                    + " AND o.DeliveryRule<>'M'"
                    //	Open Order Lines with Warehouse
                    + " AND EXISTS (SELECT * FROM C_OrderLine ol "
                        + "WHERE ol.M_Warehouse_ID=" + _M_Warehouse_ID;					//	#1
                if (_datePromised != null)
                {
                    _sql += " AND TRUNC(ol.DatePromised,'DD')<='" + String.Format("{0:dd-MMM-yy}", _datePromised) + "'";		//	#2
                }
                _sql += " AND o.C_Order_ID=ol.C_Order_ID AND ol.QtyOrdered<>ol.QtyDelivered)";
                //
                if (_C_BPartner_ID != 0)
                {
                    _sql += " AND o.C_BPartner_ID=" + _C_BPartner_ID;					//	#3
                }
            }
            _sql += " ORDER BY M_Warehouse_ID, PriorityRule, M_Shipper_ID, C_BPartner_ID, C_BPartner_Location_ID, C_PaymentTerm_ID , C_Order_ID";
            //	_sql += " FOR UPDATE";

            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(_sql, null, Get_TrxName());
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, _sql, e);
            }
            return Generate(idr);
        }

        /// <summary>
        /// Generate Shipments
        /// </summary>
        /// <param name="idr">Order Query</param>
        /// <returns>info</returns>
        private String Generate(IDataReader idr)
        {
            DataTable dt = new DataTable();
            MClient client = MClient.Get(GetCtx());
            try
            {
                dt.Load(idr);
                idr.Close();
                //ResultSet dr = pstmt.executeQuery();
                foreach (DataRow dr in dt.Rows)//  while (dr.next ())		//	Order
                {
                    order = new MOrder(GetCtx(), dr, Get_TrxName());

                    // Credit Limit check added by Vivek on 24/08/2016
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
                    else
                    {
                        MBPartnerLocation bpl = new MBPartnerLocation(GetCtx(), order.GetC_BPartner_Location_ID(), null);
                        MBPartner bpartner = MBPartner.Get(GetCtx(), order.GetC_BPartner_ID());
                        if (bpl.GetCreditStatusSettingOn() == "CL")
                        {
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
                        }
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
                    String where = " AND M_Warehouse_ID=" + _M_Warehouse_ID;
                    if (_datePromised != null)
                    {
                        where += " AND (TRUNC(DatePromised,'DD')<=" + DB.TO_DATE((DateTime?)_datePromised, true)
                            + " OR DatePromised IS NULL)";
                        //where += " AND (TRUNC(DatePromised,'DD')<='" + String.Format("{0:dd-MMM-yy}", _datePromised)
                        //    + "' OR DatePromised IS NULL)";
                    }
                    //	Exclude Auto Delivery if not Force
                    if (!MOrder.DELIVERYRULE_Force.Equals(order.GetDeliveryRule()))
                    {
                        where += " AND (C_OrderLine.M_Product_ID IS NULL"
                            + " OR EXISTS (SELECT * FROM M_Product p "
                            + "WHERE C_OrderLine.M_Product_ID=p.M_Product_ID"
                            + " AND IsExcludeAutoDelivery='N'))";
                    }
                    //	Exclude Unconfirmed
                    if (!_isUnconfirmedInOut)
                    {
                        where += " AND NOT EXISTS (SELECT * FROM M_InOutLine iol"
                                + " INNER JOIN M_InOut io ON (iol.M_InOut_ID=io.M_InOut_ID) "
                                    + "WHERE iol.C_OrderLine_ID=C_OrderLine.C_OrderLine_ID AND io.DocStatus IN ('IP','WC'))";
                    }
                    //	Deadlock Prevention - Order by M_Product_ID
                    MOrderLine[] lines = order.GetLines(where, "ORDER BY C_BPartner_Location_ID, M_Product_ID");
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

                            // Added by Bharat on 07 April 2017 as code already on Admpiere but deleted here.
                            if (line.GetC_Charge_ID() != 0)
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
                                    CreateLine(order, line, toDeliver, null, false);
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
                            MStorage[] storages = GetStorages(line.GetM_Warehouse_ID(),
                                line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
                                product.GetM_AttributeSet_ID(),
                                line.GetM_AttributeSetInstance_ID() == 0,
                                (DateTime?)minGuaranteeDate,
                                MClient.MMPOLICY_FiFo.Equals(MMPolicy));

                            for (int j = 0; j < storages.Length; j++)
                            {
                                MStorage storage = storages[j];
                                onHand = Decimal.Add(onHand, storage.GetQtyOnHand());
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
                                CreateLine(order, line, toDeliver, storages, false);
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
                                CreateLine(order, line, deliver, storages, false);
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
                                CreateLine(order, line, deliver, storages, true);
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
                                MStorage[] storages = null;
                                if (product != null && product.IsStocked())
                                {
                                    MProductCategory pc = MProductCategory.Get(order.GetCtx(), product.GetM_Product_Category_ID());
                                    String MMPolicy = pc.GetMMPolicy();
                                    if (MMPolicy == null || MMPolicy.Length == 0)
                                    {
                                        MMPolicy = client.GetMMPolicy();
                                    }
                                    //
                                    storages = GetStorages(line.GetM_Warehouse_ID(),
                                        line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
                                        product.GetM_AttributeSet_ID(),
                                        line.GetM_AttributeSetInstance_ID() == 0, (DateTime?)minGuaranteeDate,
                                        MClient.MMPOLICY_FiFo.Equals(MMPolicy));
                                }
                                //	
                                CreateLine(order, line, toDeliver, storages, false);
                            }
                        }
                    }
                    _line += 1000;
                }	//	while order
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, _sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
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
            MStorage[] storages, bool force)
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

                if (!_shipment.Save())
                {
                    //throw new Exception("Could not create Shipment");
                    log.Fine("Failed: Could not create Shipment against Order No : " + order.GetDocumentNo());
                    return;
                }
            }
            //	Non Inventory Lines
            if (storages == null)
            {
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
                //Amit 27-jan-2014
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='DTD001_'")) > 0)
                {
                    line.SetDTD001_IsAttributeNo(true);
                }
                if (!line.Save())
                {
                    //throw new Exception("Could not create Shipment Line");
                    log.Fine("Failed: Could not create Shipment Line against Order No : " + order.GetDocumentNo() + " for orderline id : " + orderLine.GetC_OrderLine_ID());
                    return;
                }
                log.Fine(line.ToString());
                return;
            }

            //	Product
            MProduct product = orderLine.GetProduct();
            bool linePerASI = false;
            if (product.GetM_AttributeSet_ID() != 0)
            {
                MAttributeSet mas = MAttributeSet.Get(GetCtx(), product.GetM_AttributeSet_ID());
                linePerASI = mas.IsInstanceAttribute();
            }

            //	Inventory Lines
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
                if (linePerASI)
                {
                    line.SetM_AttributeSetInstance_ID(storage.GetM_AttributeSetInstance_ID());
                }

                //Amit 27-jan-2014
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='DTD001_'")) > 0)
                {
                    line.SetDTD001_IsAttributeNo(true);
                }

                if (!line.Save())
                {
                    //throw new Exception("Could not create Shipment Line");
                    log.Fine("Failed: Could not create Shipment Line against Order No : " + order.GetDocumentNo() + " for orderline id : " + orderLine.GetC_OrderLine_ID());
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
                if (_msg != null)
                {
                    MOrder OrderDoc = new MOrder(GetCtx(), _shipment.GetC_Order_ID(), _shipment.Get_TrxName());
                    AddLog(_shipment.GetM_InOut_ID(), _shipment.GetMovementDate(), null,
                        (_msg + "  " +
                        (_shipment.GetProcessMsg() != "" ? (_shipment.GetProcessMsg() + " " + (!_consolidateDocument ? OrderDoc.GetDocumentNo() : " ")) : _shipment.GetDocumentNo())));
                }
                else
                {
                    AddLog(_shipment.GetM_InOut_ID(), _shipment.GetMovementDate(), null, _shipment.GetDocumentNo());
                }
                _created++;
                _map = new Dictionary<SParameter, MStorage[]>();
                if (_lastPP != null && _lastStorages != null)
                {
                    _map.Add(_lastPP, _lastStorages);
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
