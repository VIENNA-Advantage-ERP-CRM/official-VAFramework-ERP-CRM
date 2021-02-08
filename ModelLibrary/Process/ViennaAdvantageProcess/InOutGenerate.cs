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
//using System.Windows.Forms;
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
        private int _VAM_Warehouse_ID = 0;
        // BPartner				
        private int _VAB_BusinessPartner_ID = 0;
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
        private MVABOrder order = null;
        // Numner of Shipments		
        private int _created = 0;
        //	Line Number				
        private int _line = 0;
        // Movement Date		
        private DateTime? _movementDate = null;
        //Last BP Location	
        private int _lastVAB_BPart_Location_ID = -1;
        //The Query sql		
        StringBuilder _sql = new StringBuilder();
        // Storages temp space				
        //private HashMap<SParameter,MStorage[]> _map = new HashMap<SParameter,MStorage[]>();
        Dictionary<SParameter, MStorage[]> _map = new Dictionary<SParameter, MStorage[]>();
        Dictionary<SParameter, X_VAM_ContainerStorage[]> _mapContainer = new Dictionary<SParameter, X_VAM_ContainerStorage[]>();
        // Last Parameter					
        private SParameter _lastPP = null;
        // Last Storage					
        private MStorage[] _lastStorages = null;
        private X_VAM_ContainerStorage[] _lastContainerStorages = null;
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
                else if (name.Equals("VAM_Warehouse_ID"))
                {
                    _VAM_Warehouse_ID = Util.GetValueOfInt(para[i].GetParameter());
                }
                else if (name.Equals("VAB_BusinessPartner_ID"))
                {
                    _VAB_BusinessPartner_ID = Util.GetValueOfInt(para[i].GetParameter());
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
                + ", VAM_Warehouse_ID=" + _VAM_Warehouse_ID
                + ", VAB_BusinessPartner_ID=" + _VAB_BusinessPartner_ID
                + ", Consolidate=" + _consolidateDocument
                + ", IsUnconfirmed=" + _isUnconfirmedInOut
                + ", Movement=" + _movementDate);

            if (_VAM_Warehouse_ID == 0)
            {
                throw new Exception("@NotFound@ @VAM_Warehouse_ID@");
            }

            if (_selection)	//	VInOutGen
            {
                _sql.Append("SELECT * FROM VAB_Order "
                    + "WHERE IsSelected='Y' AND DocStatus='CO' AND IsSOTrx='Y' AND VAF_Client_ID=" + GetCtx().GetVAF_Client_ID()
                   //JID_0444_1 : If there are orders with Advance payment and not paid, system will not create the shipments for that orders but will create the shipment for other orders.
                   + @" AND VAB_Order_ID NOT IN
                      (SELECT VAB_Order_ID FROM VA009_OrderPaySchedule WHERE va009_orderpayschedule.VAB_Order_ID =VAB_Order.VAB_Order_id
                      AND va009_orderpayschedule.va009_ispaid = 'N' AND va009_orderpayschedule.isactive = 'Y')");
            }
            else
            {
                _sql.Append("SELECT * FROM VAB_Order o "
                    + "WHERE DocStatus='CO' AND IsSOTrx='Y'"
                    //	No Offer,POS
                    + " AND o.VAB_DocTypes_ID IN (SELECT VAB_DocTypes_ID FROM VAB_DocTypes "
                        + "WHERE DocBaseType='SOO' AND DocSubTypeSO NOT IN ('ON','OB','WR')) AND VAF_Client_ID=" + GetCtx().GetVAF_Client_ID()
                    + "	AND o.IsDropShip='N'"
                    //	No Manual
                    + " AND o.DeliveryRule<>'M'"
                   //JID_0444_1 : If there are orders with Advance payment and not paid, system will not create the shipments for that orders but will create the shipment for other orders.
                   + @" AND o.VAB_Order_ID NOT IN
                      (SELECT VAB_Order_ID FROM VA009_OrderPaySchedule WHERE va009_orderpayschedule.VAB_Order_ID =o.VAB_Order_id
                      AND va009_orderpayschedule.va009_ispaid = 'N' AND va009_orderpayschedule.isactive = 'Y' ) "
                    //	Open Order Lines with Warehouse
                    // JID_0685: If there is sales order with Charge and product that are other than items type that order should not be available at Gemerate Shipment process.
                    + " AND EXISTS (SELECT VAB_OrderLine_ID FROM VAB_OrderLine ol ");

                // Get the lines of Order based on the setting taken on Tenant to allow non item Product
                if (!isAllowNonItem)
                {
                    _sql.Append("INNER JOIN VAM_Product prd ON (ol.VAM_Product_ID = prd.VAM_Product_ID AND prd.ProductType = 'I')");
                }

                _sql.Append("WHERE ol.VAM_Warehouse_ID=" + _VAM_Warehouse_ID);					//	#1

                if (_datePromised != null)
                {
                    _sql.Append(" AND TRUNC(ol.DatePromised,'DD')<='" + String.Format("{0:dd-MMM-yy}", _datePromised) + "'");		//	#2
                }
                _sql.Append(" AND o.VAB_Order_ID=ol.VAB_Order_ID AND ol.QtyOrdered<>ol.QtyDelivered)");
                //
                if (_VAB_BusinessPartner_ID != 0)
                {
                    _sql.Append(" AND o.VAB_BusinessPartner_ID=" + _VAB_BusinessPartner_ID);					//	#3
                }
            }
            _sql.Append(" ORDER BY VAM_Warehouse_ID, PriorityRule, VAM_ShippingMethod_ID, VAB_BusinessPartner_ID, VAB_BPart_Location_ID, VAB_PaymentTerm_ID , VAB_Order_ID");
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
            MVAFClient client = MVAFClient.Get(GetCtx());

            foreach (DataRow dr in dt.Rows)		//	Order
            {
                order = new MVABOrder(GetCtx(), dr, Get_TrxName());

                // Credit Limit check 
                MVABBusinessPartner bp = MVABBusinessPartner.Get(GetCtx(), order.GetVAB_BusinessPartner_ID());
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
                else if (bp.GetCreditStatusSettingOn() == X_VAB_BusinessPartner.CREDITSTATUSSETTINGON_CustomerLocation)
                {
                    MVABBPartLocation bpl = new MVABBPartLocation(GetCtx(), order.GetVAB_BPart_Location_ID(), null);
                    //MBPartner bpartner = MBPartner.Get(GetCtx(), order.GetVAB_BusinessPartner_ID());
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
                    && (_shipment.GetVAB_BPart_Location_ID() != order.GetVAB_BPart_Location_ID()
                        || _shipment.GetVAM_ShippingMethod_ID() != order.GetVAM_ShippingMethod_ID())))
                {
                    CompleteShipment();
                }
                log.Fine("check: " + order + " - DeliveryRule=" + order.GetDeliveryRule());
                //
                DateTime? minGuaranteeDate = _movementDate;
                bool completeOrder = MVABOrder.DELIVERYRULE_CompleteOrder.Equals(order.GetDeliveryRule());
                //	OrderLine WHERE
                StringBuilder where = new StringBuilder(" AND VAM_Warehouse_ID=" + _VAM_Warehouse_ID);
                if (_datePromised != null)
                {
                    where.Append(" AND (TRUNC(DatePromised,'DD')<=" + DB.TO_DATE((DateTime?)_datePromised, true)
                        + " OR DatePromised IS NULL)");
                    //where += " AND (TRUNC(DatePromised,'DD')<='" + String.Format("{0:dd-MMM-yy}", _datePromised)
                    //    + "' OR DatePromised IS NULL)";
                }
                //	Exclude Auto Delivery if not Force
                if (!MVABOrder.DELIVERYRULE_Force.Equals(order.GetDeliveryRule()))
                {
                    where.Append(" AND (VAB_OrderLine.VAM_Product_ID IS NULL"
                        + " OR EXISTS (SELECT * FROM VAM_Product p "
                        + "WHERE VAB_OrderLine.VAM_Product_ID=p.VAM_Product_ID"
                        + " AND IsExcludeAutoDelivery='N'))");
                }
                //	Exclude Unconfirmed
                if (!_isUnconfirmedInOut)
                {
                    where.Append(" AND NOT EXISTS (SELECT * FROM VAM_Inv_InOutLine iol"
                            + " INNER JOIN VAM_Inv_InOut io ON (iol.VAM_Inv_InOut_ID=io.VAM_Inv_InOut_ID) "
                                + "WHERE iol.VAB_OrderLine_ID=VAB_OrderLine.VAB_OrderLine_ID AND io.DocStatus IN ('IP','WC'))");
                }

                // Get the lines of Order based on the setting taken on Tenant to allow non item Product
                if (!isAllowNonItem)
                {
                    // JID_1307: Shipment is not generating against the Sales Order which includes combination of ItemType & Service/Expense/Resource type of Products at SO lines
                    where.Append(" AND VAB_OrderLine_ID IN (SELECT ol.VAB_OrderLine_ID FROM VAB_OrderLine ol INNER JOIN VAM_Product p ON ol.VAM_Product_ID = p.VAM_Product_ID WHERE ol.VAB_Order_ID = "
                    + order.GetVAB_Order_ID() + " AND p.ProductType = 'I')");
                }

                //	Deadlock Prevention - Order by M_Product_ID
                MVABOrderLine[] lines = order.GetLines(where.ToString(), "ORDER BY VAB_BPart_Location_ID, VAM_Product_ID");
                for (int i = 0; i < lines.Length; i++)
                {
                    MVABOrderLine line = lines[i];
                    // if order line is not drop ship type
                    if (!line.IsDropShip())
                    {
                        if (line.GetVAM_Warehouse_ID() != _VAM_Warehouse_ID)
                        {
                            continue;
                        }
                        log.Fine("check: " + line);
                        Decimal onHand = Env.ZERO;
                        Decimal toDeliver = Decimal.Subtract(line.GetQtyOrdered(),
                            line.GetQtyDelivered());
                        Decimal QtyNotDelivered = Util.GetValueOfDecimal(DB.ExecuteScalar(@"SELECT SUM(MovementQty) FROM VAM_Inv_InOut i INNER JOIN VAM_Inv_InOutLine il ON i.VAM_Inv_InOut_ID = il.VAM_Inv_InOut_ID
                            WHERE il.VAB_OrderLine_ID = " + line.GetVAB_OrderLine_ID() + @" AND il.Isactive = 'Y' AND i.docstatus NOT IN ('RE' , 'VO' , 'CL' , 'CO')", null, Get_Trx()));
                        toDeliver -= QtyNotDelivered;
                        MProduct product = line.GetProduct();
                        //	Nothing to Deliver
                        if (product != null && Env.Signum(toDeliver) == 0)
                        {
                            continue;
                        }

                        // Get the lines of Order based on the setting taken on Tenant to allow non item Product                        
                        if (line.GetVAB_Charge_ID() != 0 && (!isAllowNonItem || Env.Signum(toDeliver) == 0))        // Nothing to Deliver
                        {
                            continue;
                        }

                        //	Check / adjust for confirmations
                        Decimal unconfirmedShippedQty = Env.ZERO;
                        if (_isUnconfirmedInOut && product != null && Env.Signum(toDeliver) != 0)
                        {
                            String where2 = "EXISTS (SELECT * FROM VAM_Inv_InOut io WHERE io.VAM_Inv_InOut_ID=VAM_Inv_InOutLine.VAM_Inv_InOut_ID AND io.DocStatus IN ('IP','WC'))";
                            MInOutLine[] iols = MInOutLine.GetOfOrderLine(GetCtx(),
                                line.GetVAB_OrderLine_ID(), where2, null);
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
                            if (!MVABOrder.DELIVERYRULE_CompleteOrder.Equals(order.GetDeliveryRule()))	//	printed later
                            {
                                CreateLine(order, line, toDeliver, null, false, false);
                            }
                            continue;
                        }

                        //	Stored Product
                        MProductCategory pc = MProductCategory.Get(order.GetCtx(), product.GetVAM_ProductCategory_ID());
                        String MMPolicy = pc.GetMMPolicy();
                        if (MMPolicy == null || MMPolicy.Length == 0)
                        {
                            MMPolicy = client.GetMMPolicy();
                        }
                        //
                        dynamic[] storages = null;
                        if (!isContainerApplicable)
                        {
                            storages = GetStorages(line.GetVAM_Warehouse_ID(),
                               line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(),
                               product.GetVAM_PFeature_Set_ID(),
                               line.GetVAM_PFeature_SetInstance_ID() == 0,
                               (DateTime?)minGuaranteeDate,
                               MVAFClient.MMPOLICY_FiFo.Equals(MMPolicy));
                        }
                        else
                        {
                            storages = GetContainerStorages(line.GetVAM_Warehouse_ID(),
                              line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(),
                              product.GetVAM_PFeature_Set_ID(),
                              line.GetVAM_PFeature_SetInstance_ID() == 0,
                              (DateTime?)minGuaranteeDate,
                              MVAFClient.MMPOLICY_FiFo.Equals(MMPolicy), false, false);
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
                        else if (fullLine && MVABOrder.DELIVERYRULE_CompleteLine.Equals(order.GetDeliveryRule()))
                        {
                            log.Fine("CompleteLine - OnHand=" + onHand
                                + " (Unconfirmed=" + unconfirmedShippedQty
                                + ", ToDeliver=" + toDeliver + " - " + line);
                            //	
                            CreateLine(order, line, toDeliver, storages, false, MVAFClient.MMPOLICY_FiFo.Equals(MMPolicy));
                        }
                        //	Availability
                        else if (MVABOrder.DELIVERYRULE_Availability.Equals(order.GetDeliveryRule())
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
                            CreateLine(order, line, deliver, storages, false, MVAFClient.MMPOLICY_FiFo.Equals(MMPolicy));
                        }
                        //	Force
                        else if (MVABOrder.DELIVERYRULE_Force.Equals(order.GetDeliveryRule()))
                        {
                            Decimal deliver = toDeliver;
                            log.Fine("Force - OnHand=" + onHand
                                + " (Unconfirmed=" + unconfirmedShippedQty
                                + "), ToDeliver=" + toDeliver
                                + ", Delivering=" + deliver + " - " + line);
                            //	
                            CreateLine(order, line, deliver, storages, true, MVAFClient.MMPOLICY_FiFo.Equals(MMPolicy));
                        }
                        //	Manual
                        else if (MVABOrder.DELIVERYRULE_Manual.Equals(order.GetDeliveryRule()))
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
                if (completeOrder && MVABOrder.DELIVERYRULE_CompleteOrder.Equals(order.GetDeliveryRule()))
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        MVABOrderLine line = lines[i];
                        // if order line is not drop ship type
                        if (!line.IsDropShip())
                        {
                            if (line.GetVAM_Warehouse_ID() != _VAM_Warehouse_ID)
                                continue;
                            MProduct product = line.GetProduct();
                            Decimal toDeliver = Decimal.Subtract(line.GetQtyOrdered(), line.GetQtyDelivered());
                            //
                            dynamic[] storages = null;
                            String MMPolicy = null;
                            if (product != null && product.IsStocked())
                            {
                                MProductCategory pc = MProductCategory.Get(order.GetCtx(), product.GetVAM_ProductCategory_ID());
                                MMPolicy = pc.GetMMPolicy();
                                if (MMPolicy == null || MMPolicy.Length == 0)
                                {
                                    MMPolicy = client.GetMMPolicy();
                                }
                                //
                                if (!isContainerApplicable)
                                {
                                    storages = GetStorages(line.GetVAM_Warehouse_ID(),
                                        line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(),
                                        product.GetVAM_PFeature_Set_ID(),
                                        line.GetVAM_PFeature_SetInstance_ID() == 0, (DateTime?)minGuaranteeDate,
                                        MVAFClient.MMPOLICY_FiFo.Equals(MMPolicy));
                                }
                                else
                                {
                                    storages = GetContainerStorages(line.GetVAM_Warehouse_ID(),
                                      line.GetVAM_Product_ID(), line.GetVAM_PFeature_SetInstance_ID(),
                                      product.GetVAM_PFeature_Set_ID(),
                                      line.GetVAM_PFeature_SetInstance_ID() == 0,
                                      (DateTime?)minGuaranteeDate,
                                      MVAFClient.MMPOLICY_FiFo.Equals(MMPolicy), false, false);
                                }
                            }
                            //	
                            CreateLine(order, line, toDeliver, storages, false, MVAFClient.MMPOLICY_FiFo.Equals(MMPolicy));
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
        private void CreateLine(MVABOrder order, MVABOrderLine orderLine, Decimal qty,
            dynamic[] storages, bool force, bool FiFo)
        {
            //	Complete last Shipment - can have multiple shipments
            if (_lastVAB_BPart_Location_ID != orderLine.GetVAB_BPart_Location_ID())
            {
                CompleteShipment();
            }
            _lastVAB_BPart_Location_ID = orderLine.GetVAB_BPart_Location_ID();
            //	Create New Shipment
            if (_shipment == null)
            {
                _shipment = new MInOut(order, 0, _movementDate);
                _shipment.SetVAM_Warehouse_ID(orderLine.GetVAM_Warehouse_ID());	//	sets Org too
                if (order.GetVAB_BusinessPartner_ID() != orderLine.GetVAB_BusinessPartner_ID())
                {
                    _shipment.SetVAB_BusinessPartner_ID(orderLine.GetVAB_BusinessPartner_ID());
                }
                if (order.GetVAB_BPart_Location_ID() != orderLine.GetVAB_BPart_Location_ID())
                {
                    _shipment.SetVAB_BPart_Location_ID(orderLine.GetVAB_BPart_Location_ID());
                }

                // Added by Bharat on 29 Jan 2018 to set Inco Term from Order
                if (_shipment.Get_ColumnIndex("VAB_IncoTerm_ID") > 0)
                {
                    _shipment.SetVAB_IncoTerm_ID(order.GetVAB_IncoTerm_ID());
                }

                if (!_shipment.Save())
                {
                    //throw new Exception("Could not create Shipment");
                    log.Fine("Failed: Could not create Shipment against Order No : " + order.GetDocumentNo());

                    // JID_0405: In the error message it should show the Name of the Product for which qty is not available.
                    pp = VLogger.RetrieveError();
                    if (pp != null)
                    {
                        _msg.Append(!string.IsNullOrEmpty(pp.GetName()) ? pp.GetName() : pp.GetValue());
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
                if (!line.Save())
                {
                    //throw new Exception("Could not create Shipment Line");
                    log.Fine("Failed: Could not create Shipment Line against Order No : " + order.GetDocumentNo() + " for orderline id : " + orderLine.GetVAB_OrderLine_ID());

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
            //if (product.GetVAM_PFeature_Set_ID() != 0)
            //{
            //    MAttributeSet mas = MAttributeSet.Get(GetCtx(), product.GetVAM_PFeature_Set_ID());
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
                    int VAM_ProductContainer_ID = Util.GetValueOfInt(storage.GetVAM_ProductContainer_ID());
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

                    int VAM_Locator_ID = storage.GetVAM_Locator_ID();

                    //
                    MInOutLine line = null;
                    if (!linePerASI)	//	find line with Locator, AttributeSetInsatnce and ProductContainer
                    {
                        for (int n = 0; n < list.Count; n++)
                        {
                            MInOutLine test = (MInOutLine)list[n];
                            if (test.GetVAM_Locator_ID() == VAM_Locator_ID
                                && test.GetVAM_ProductContainer_ID() == VAM_ProductContainer_ID
                                && test.GetVAM_PFeature_SetInstance_ID() == storage.GetVAM_PFeature_SetInstance_ID())
                            {
                                line = test;
                                break;
                            }
                        }
                    }
                    if (line == null)	//	new line
                    {
                        line = new MInOutLine(_shipment);
                        line.SetOrderLine(orderLine, VAM_Locator_ID, order.IsSOTrx() ? deliver : Env.ZERO);
                        line.SetQty(deliver);
                        line.SetVAM_ProductContainer_ID(VAM_ProductContainer_ID);
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
                    line.SetVAM_PFeature_SetInstance_ID(storage.GetVAM_PFeature_SetInstance_ID());

                    if (Env.IsModuleInstalled("DTD001_"))
                    {
                        line.SetDTD001_IsAttributeNo(true);
                    }

                    if (!line.Save())
                    {
                        log.Fine("Failed: Could not create Shipment Line against Order No : " + order.GetDocumentNo() + " for orderline id : " + orderLine.GetVAB_OrderLine_ID());
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

                    int VAM_Locator_ID = storage.GetVAM_Locator_ID();
                    //
                    MInOutLine line = null;
                    if (!linePerASI)	//	find line with Locator
                    {
                        for (int n = 0; n < list.Count; n++)
                        {
                            MInOutLine test = (MInOutLine)list[n];
                            if (test.GetVAM_Locator_ID() == VAM_Locator_ID)
                            {
                                line = test;
                                break;
                            }
                        }
                    }
                    if (line == null)	//	new line
                    {
                        line = new MInOutLine(_shipment);
                        line.SetOrderLine(orderLine, VAM_Locator_ID, order.IsSOTrx() ? deliver : Env.ZERO);
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
                    line.SetVAM_PFeature_SetInstance_ID(storage.GetVAM_PFeature_SetInstance_ID());
                    //}

                    if (Env.IsModuleInstalled("DTD001_"))
                    {
                        line.SetDTD001_IsAttributeNo(true);
                    }

                    if (!line.Save())
                    {
                        //throw new Exception("Could not create Shipment Line");
                        log.Fine("Failed: Could not create Shipment Line against Order No : " + order.GetDocumentNo() + " for orderline id : " + orderLine.GetVAB_OrderLine_ID());

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
        /// <param name="VAM_Warehouse_ID">Ware House ID</param>
        /// <param name="VAM_Product_ID">Product id</param>
        /// <param name="VAM_PFeature_SetInstance_ID">Attribute Set instance</param>
        /// <param name="VAM_PFeature_Set_ID">attribute id</param>
        /// <param name="allAttributeInstances">all atribute</param>
        /// <param name="minGuaranteeDate"></param>
        /// <param name="FiFo"></param>
        /// <returns>storages</returns>
        private MStorage[] GetStorages(int VAM_Warehouse_ID,
            int VAM_Product_ID, int VAM_PFeature_SetInstance_ID, int VAM_PFeature_Set_ID,
            bool allAttributeInstances, DateTime? minGuaranteeDate,
            bool FiFo)
        {

            _lastPP = new SParameter(VAM_Warehouse_ID,
                VAM_Product_ID, VAM_PFeature_SetInstance_ID, VAM_PFeature_Set_ID,
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
                    VAM_Warehouse_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                    VAM_PFeature_Set_ID, allAttributeInstances, minGuaranteeDate,
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
        /// <param name="VAM_Warehouse_ID">WareHouse ID</param>
        /// <param name="VAM_Product_ID">Product id</param>
        /// <param name="VAM_PFeature_SetInstance_ID">Attribute Set instance</param>
        /// <param name="VAM_PFeature_Set_ID">attribute id</param>
        /// <param name="allAttributeInstances">all atribute</param>
        /// <param name="minGuaranteeDate">current date</param>
        /// <param name="FiFo">material policy</param>
        /// <param name="greater">if false, then pick record upto current date</param>
        /// <param name="isContainerConsider">record to be filtered based on Container or not</param>
        /// <returns>container storages</returns>
        private X_VAM_ContainerStorage[] GetContainerStorages(int VAM_Warehouse_ID, int VAM_Product_ID, int VAM_PFeature_SetInstance_ID, int VAM_PFeature_Set_ID,
                                                              bool allAttributeInstances, DateTime? minGuaranteeDate, bool FiFo, bool greater, bool isContainerConsider)
        {

            _lastPP = new SParameter(VAM_Warehouse_ID,
                VAM_Product_ID, VAM_PFeature_SetInstance_ID, VAM_PFeature_Set_ID,
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
                _lastContainerStorages = MProductContainer.GetContainerStorage(GetCtx(), VAM_Warehouse_ID, 0, 0,
                              VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                              VAM_PFeature_Set_ID,
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
                    MVABOrder OrderDoc = new MVABOrder(GetCtx(), _shipment.GetVAB_Order_ID(), _shipment.Get_TrxName());
                    AddLog(_shipment.GetVAM_Inv_InOut_ID(), _shipment.GetMovementDate(), null,
                        (_msg + "  " +
                        (_shipment.GetProcessMsg() != "" ? (_shipment.GetProcessMsg() + " " + (!_consolidateDocument ? OrderDoc.GetDocumentNo() : " ")) : _shipment.GetDocumentNo())));
                }
                else
                {
                    AddLog(_shipment.GetVAM_Inv_InOut_ID(), _shipment.GetMovementDate(), null, _shipment.GetDocumentNo());
                    _created++;
                }

                if (isContainerApplicable)
                {
                    _mapContainer.Clear();
                    _mapContainer = null;
                    _mapContainer = new Dictionary<SParameter, X_VAM_ContainerStorage[]>();
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
            public int VAM_Warehouse_ID;
            // Product			
            public int VAM_Product_ID;
            // ASI				
            public int VAM_PFeature_SetInstance_ID;
            // AS				
            public int VAM_PFeature_Set_ID;
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
                this.VAM_Warehouse_ID = p_Warehouse_ID;
                this.VAM_Product_ID = p_Product_ID;
                this.VAM_PFeature_SetInstance_ID = p_AttributeSetInstance_ID;
                this.VAM_PFeature_Set_ID = p_AttributeSet_ID;
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
                    bool eq = cmp.VAM_Warehouse_ID == VAM_Warehouse_ID
                        && cmp.VAM_Product_ID == VAM_Product_ID
                        && cmp.VAM_PFeature_SetInstance_ID == VAM_PFeature_SetInstance_ID
                        && cmp.VAM_PFeature_Set_ID == VAM_PFeature_Set_ID
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
                long hash = VAM_Warehouse_ID
                    + (VAM_Product_ID * 2)
                    + (VAM_PFeature_SetInstance_ID * 3)
                    + (VAM_PFeature_Set_ID * 4);

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
