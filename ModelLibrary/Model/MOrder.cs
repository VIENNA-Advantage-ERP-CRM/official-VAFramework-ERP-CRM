/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_Order, DocAction(Interface)
 * Chronological Development
 * Veena Pandey     18-May-2009
 * Raghunandan      17-june-2009 
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
using System.IO;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Print;
using System.Reflection;
using ModelLibrary.Classes;
using VAdvantage.ProcessEngine;

namespace VAdvantage.Model
{
    /// <summary>
    /// Order model.
    /// Please do not set DocStatus and C_DocType_ID directly. 
    /// They are set in the Process() method. 
    /// Use DocAction and C_DocTypeTarget_ID instead.
    /// </summary>
    public class MOrder : X_C_Order, DocAction
    {
        #region Variables
        /**	Process Message 			*/
        private String _processMsg = null;

        /**	Order Lines					*/
        private MOrderLine[] _lines = null;
        /**	Tax Lines					*/
        private MOrderTax[] _taxes = null;
        /** Force Creation of order		*/
        private bool _forceCreation = false;
        /**	Just Prepared Flag			*/
        private bool _justPrepared = false;

        /**Create Counter Document **/
        //private int havingPriceList;
        private MBPartner counterBPartner = null;
        private int counterOrgId = 0;
        private int counterWarehouseId = 0;
        /** Sales Order Sub Type - SO	*/
        public static String DocSubTypeSO_Standard = "SO";
        /** Sales Order Sub Type - OB	*/
        public static String DocSubTypeSO_Quotation = "OB";
        /** Sales Order Sub Type - ON	*/
        public static String DocSubTypeSO_Proposal = "ON";
        /** Sales Order Sub Type - PR	*/
        public static String DocSubTypeSO_Prepay = "PR";
        /** Sales Order Sub Type - WR	*/
        public static String DocSubTypeSO_POS = "WR";
        /** Sales Order Sub Type - WP	*/
        public static String DocSubTypeSO_Warehouse = "WP";
        /** Sales Order Sub Type - WI	*/
        public static String DocSubTypeSO_OnCredit = "WI";
        /** Sales Order Sub Type - RM	*/
        public static String DocSubTypeSO_RMA = "RM";
        String DocSubTypeSO = "";
        public Decimal? OnHandQty = 0;
        /**is container applicable */
        private bool isContainerApplicable = false;

        private String _budgetMessage = String.Empty;
        private string _budgetNotDefined = string.Empty;

        #endregion

        /* 	Create new Order by copying
         * 	@param from order
         * 	@param dateDoc date of the document date
         * 	@param C_DocTypeTarget_ID target document type
         * 	@param isSOTrx sales order 
         * 	@param counter create counter links
         *	@param copyASI copy line attributes Attribute Set Instance, Resource Assignment
         * 	@param trxName trx
         *	@return Order
         */
        public static MOrder CopyFrom(MOrder from, DateTime? dateDoc, int C_DocTypeTarget_ID, bool counter, bool copyASI, Trx trxName, bool fromCreateSO = false)//added optional parameter which indicates from where this function is being called(If this fuction is called from Create SO Process on Sales Quotation window then fromCreateSO is true otherwise it will be false)----Neha
        {
            MOrder to = new MOrder(from.GetCtx(), 0, trxName);


            to.Set_TrxName(trxName);
            PO.CopyValues(from, to, from.GetAD_Client_ID(), from.GetAD_Org_ID());
            to.Set_ValueNoCheck("C_Order_ID", I_ZERO);
            to.Set_ValueNoCheck("DocumentNo", null);
            //
            to.SetDocStatus(DOCSTATUS_Drafted);		//	Draft
            to.SetDocAction(DOCACTION_Complete);
            //
            to.SetC_DocType_ID(0);
            to.SetC_DocTypeTarget_ID(C_DocTypeTarget_ID, true);
            //
            to.SetIsSelected(false);
            to.SetDateOrdered(dateDoc);
            to.SetDateAcct(dateDoc);
            to.SetDatePromised(dateDoc);	//	assumption
            to.SetDatePrinted(null);
            to.SetIsPrinted(false);
            //
            to.SetIsApproved(false);
            to.SetIsCreditApproved(false);
            to.SetC_Payment_ID(0);
            to.SetC_CashLine_ID(0);
            //	Amounts are updated  when adding lines
            to.SetGrandTotal(Env.ZERO);
            to.SetTotalLines(Env.ZERO);
            //
            to.SetIsDelivered(false);
            to.SetIsInvoiced(false);
            to.SetIsSelfService(false);
            to.SetIsTransferred(false);
            to.SetPosted(false);
            to.SetProcessed(false);
            if (counter)
            {
                to.SetRef_Order_ID(from.GetC_Order_ID());

                //SI_0625 : Link Organization Functionality
                // set counter BP Org
                if (from.GetCounterOrgID() > 0)
                    to.SetAD_Org_ID(from.GetCounterOrgID());

                //set warehouse
                if (from.GetCounterWarehouseID() > 0)
                    to.SetM_Warehouse_ID(from.GetCounterWarehouseID());

                // set Counter BP Details
                if (from.GetCounterBPartner() != null)
                    to.SetBPartner(from.GetCounterBPartner());

                MPriceList pl = MPriceList.Get(from.GetCtx(), to.GetM_PriceList_ID(), trxName);
                //when record is of SO then price list must be Sale price list and vice versa
                if (from.GetCounterBPartner() != null && ((to.IsSOTrx() && !pl.IsSOPriceList()) || (!to.IsSOTrx() && pl.IsSOPriceList())))
                {
                    /* 1. check first with same currency , same Org , same client and IsDefault as True
                     * 2. check 2nd with same currency , same Org , same client and IsDefault as False
                     * 3. check 3rd with same currency , (*) Org , same client and IsDefault as True
                     * 4. check 3rd with same currency , (*) Org , same client and IsDefault as False */
                    string sql = @"SELECT M_PriceList_ID FROM M_PriceList 
                               WHERE IsActive = 'Y' AND AD_Client_ID IN ( " + to.GetAD_Client_ID() + @" , 0 ) " +
                                    @" AND C_Currency_ID = " + to.GetC_Currency_ID() +
                                    @" AND AD_Org_ID IN ( " + to.GetAD_Org_ID() + @" , 0 ) " +
                                    @" AND IsSOPriceList = '" + (to.IsSOTrx() ? "Y" : "N") + "' " +
                                    @" ORDER BY AD_Org_ID DESC , IsDefault DESC,  M_PriceList_ID DESC , AD_Client_ID DESC";
                    int priceListId = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, trxName));
                    if (priceListId > 0)
                    {
                        to.SetM_PriceList_ID(priceListId);
                    }
                    else
                    {
                        //Could not create Order. Price List not avialable
                        from.SetProcessMsg(Msg.GetMsg(from.GetCtx(), "VIS_PriceListNotFound"));
                        throw new Exception("Could not create Order. Price List not avialable");
                    }
                }
            }
            else
            {
                to.SetRef_Order_ID(0);
            }

            if (to.Get_ColumnIndex("ConditionalFlag") > -1)
            {
                to.SetConditionalFlag(MOrder.CONDITIONALFLAG_PrepareIt);
            }
            //
            if (!to.Save(trxName))
            {
                ValueNamePair pp = VLogger.RetrieveError();
                if (pp != null)
                {
                    from.SetProcessMsg("Could not create Order. " + pp.GetName());
                }
                else
                {
                    from.SetProcessMsg("Could not create Order.");
                }
                throw new Exception("Could not create Order. " + (pp != null && pp.GetName() != null ? pp.GetName() : ""));
            }
            if (counter)
            {
                from.SetRef_Order_ID(to.GetC_Order_ID());
            }

            if (to.CopyLinesFrom(from, counter, copyASI, fromCreateSO) == 0)//added optional parameter which indicates from where this function is being called(If this fuction is called from Create SO Process on Sales Quotation window then fromCreateSO is true otherwise it will be false)----Neha
            {
                ValueNamePair pp = VLogger.RetrieveError();
                if (pp != null)
                {
                    from.SetProcessMsg("Could not create Order Lines. " + pp.GetName());
                }
                else
                {
                    from.SetProcessMsg("Could not create Order Lines.");
                }
                throw new Exception("Could not create Order Lines. " + (pp != null && pp.GetName() != null ? pp.GetName() : ""));
            }

            if (to.Get_ColumnIndex("ConditionalFlag") > -1)
            {
                DB.ExecuteQuery("UPDATE C_Order SET ConditionalFlag = null WHERE C_Order_ID = " + to.GetC_Order_ID(), null, trxName);
            }
            return to;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_Order_ID"></param>
        /// <param name="trxName"></param>
        public MOrder(Ctx ctx, int C_Order_ID, Trx trxName)
            : base(ctx, C_Order_ID, trxName)
        {

            //  New
            if (C_Order_ID == 0)
            {
                SetDocStatus(DOCSTATUS_Drafted);
                SetDocAction(DOCACTION_Prepare);
                //
                SetDeliveryRule(DELIVERYRULE_Force);
                SetFreightCostRule(FREIGHTCOSTRULE_FreightIncluded);
                SetInvoiceRule(INVOICERULE_Immediate);
                SetPaymentRule(PAYMENTRULE_OnCredit);
                SetPriorityRule(PRIORITYRULE_Medium);
                SetDeliveryViaRule(DELIVERYVIARULE_Pickup);
                //
                SetIsDiscountPrinted(false);
                SetIsSelected(false);
                SetIsTaxIncluded(false);
                SetIsSOTrx(true);
                SetIsDropShip(false);
                SetSendEMail(false);
                //
                SetIsApproved(false);
                SetIsPrinted(false);
                SetIsCreditApproved(false);
                SetIsDelivered(false);
                SetIsInvoiced(false);
                SetIsTransferred(false);
                SetIsSelfService(false);
                SetIsReturnTrx(false);
                //
                base.SetProcessed(false);
                SetProcessing(false);
                SetPosted(false);

                SetDateAcct(Convert.ToDateTime(DateTime.Now));// CommonFunctions.CurrentTimeMillis()));
                SetDatePromised(Convert.ToDateTime(DateTime.Now));// CommonFunctions.CurrentTimeMillis()));
                SetDateOrdered(Convert.ToDateTime(DateTime.Now));// CommonFunctions.CurrentTimeMillis()));

                SetFreightAmt(Env.ZERO);
                SetChargeAmt(Env.ZERO);
                SetTotalLines(Env.ZERO);
                SetGrandTotal(Env.ZERO);
            }


        }

        /*  Project Constructor
        *  @param  project Project to create Order from
        *  @param IsSOTrx sales order
        * 	@param	DocSubTypeSO if SO DocType Target (default DocSubTypeSO_OnCredit)
        */
        public MOrder(MProject project, bool IsSOTrx, String DocSubTypeSO)
            : this(project.GetCtx(), 0, project.Get_TrxName())
        {


            SetAD_Client_ID(project.GetAD_Client_ID());
            SetAD_Org_ID(project.GetAD_Org_ID());
            SetC_Campaign_ID(project.GetC_Campaign_ID());
            SetSalesRep_ID(project.GetSalesRep_ID());
            //
            SetC_Project_ID(project.GetC_Project_ID());
            SetDescription(project.GetName());
            DateTime? ts = project.GetDateContract();
            if (ts != null)
                SetDateOrdered(ts);
            ts = project.GetDateFinish();
            if (ts != null)
                SetDatePromised(ts);
            //
            SetC_BPartner_ID(project.GetC_BPartner_ID());
            SetC_BPartner_Location_ID(project.GetC_BPartner_Location_ID());
            SetAD_User_ID(project.GetAD_User_ID());
            //
            SetM_Warehouse_ID(project.GetM_Warehouse_ID());
            SetM_PriceList_ID(project.GetM_PriceList_ID());
            SetC_PaymentTerm_ID(project.GetC_PaymentTerm_ID());
            //
            SetIsSOTrx(IsSOTrx);
            if (IsSOTrx)
            {
                if (DocSubTypeSO == null || DocSubTypeSO.Length == 0)
                    SetC_DocTypeTarget_ID(DocSubTypeSO_OnCredit);
                else
                    SetC_DocTypeTarget_ID(DocSubTypeSO);
            }
            else
            {
                SetC_DocTypeTarget_ID();
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MOrder(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /*	Overwrite Client/Org if required
        * 	@param AD_Client_ID client
        * 	@param AD_Org_ID org
        */
        public new void SetClientOrg(int AD_Client_ID, int AD_Org_ID)
        {
            base.SetClientOrg(AD_Client_ID, AD_Org_ID);
        }

        /// <summary>
        /// Add to Description
        /// </summary>
        /// <param name="description">text</param>
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
            {
                SetDescription(description);
            }
            else
            {
                SetDescription(desc + " | " + description);
            }
        }

        /**
         * 	Set Business Partner (Ship+Bill)
         *	@param C_BPartner_ID bpartner
         */
        public new void SetC_BPartner_ID(int C_BPartner_ID)
        {
            base.SetC_BPartner_ID(C_BPartner_ID);
            base.SetBill_BPartner_ID(C_BPartner_ID);
        }

        /**
         * 	Set Business Partner Defaults & Details.
         * 	SOTrx should be set.
         * 	@param bp business partner
         */
        public void SetBPartner(MBPartner bp)
        {
            try
            {
                if (bp == null || !bp.IsActive())
                    return;

                SetC_BPartner_ID(bp.GetC_BPartner_ID());
                //	Defaults Payment Term
                int ii = 0;
                if (IsSOTrx())
                    ii = bp.GetC_PaymentTerm_ID();
                else
                    ii = bp.GetPO_PaymentTerm_ID();
                if (ii != 0)
                    SetC_PaymentTerm_ID(ii);
                //	Default Price List
                if (IsSOTrx())
                    ii = bp.GetM_PriceList_ID();
                else
                    ii = bp.GetPO_PriceList_ID();
                if (ii != 0)
                    SetM_PriceList_ID(ii);
                //	Default Delivery/Via Rule
                String ss = bp.GetDeliveryRule();
                if (ss != null)
                    SetDeliveryRule(ss);
                ss = bp.GetDeliveryViaRule();
                if (ss != null)
                    SetDeliveryViaRule(ss);
                //	Default Invoice/Payment Rule
                ss = bp.GetInvoiceRule();
                if (ss != null)
                    SetInvoiceRule(ss);
                if (IsSOTrx())
                    ss = bp.GetPaymentRule();
                else
                    ss = bp.GetPaymentRulePO();
                if (ss != null)
                    SetPaymentRule(ss);
                //	Sales Rep
                ii = bp.GetSalesRep_ID();
                if (ii != 0)
                    SetSalesRep_ID(ii);


                //	Set Locations
                MBPartnerLocation[] locs = bp.GetLocations(false);
                if (locs != null)
                {
                    for (int i = 0; i < locs.Length; i++)
                    {
                        if (locs[i].IsShipTo())
                            base.SetC_BPartner_Location_ID(locs[i].GetC_BPartner_Location_ID());
                        if (locs[i].IsBillTo())
                            SetBill_Location_ID(locs[i].GetC_BPartner_Location_ID());
                    }
                    //	set to first
                    if (GetC_BPartner_Location_ID() == 0 && locs.Length > 0)
                        base.SetC_BPartner_Location_ID(locs[0].GetC_BPartner_Location_ID());
                    if (GetBill_Location_ID() == 0 && locs.Length > 0)
                        SetBill_Location_ID(locs[0].GetC_BPartner_Location_ID());
                }
                if (GetC_BPartner_Location_ID() == 0)
                {
                    log.Log(Level.SEVERE, "MOrder.setBPartner - Has no Ship To Address: " + bp);
                }
                if (GetBill_Location_ID() == 0)
                {
                    log.Log(Level.SEVERE, "MOrder.setBPartner - Has no Bill To Address: " + bp);
                }

                //	Set Contact
                MUser[] contacts = bp.GetContacts(false);
                if (contacts != null && contacts.Length == 1)
                {
                    SetAD_User_ID(contacts[0].GetAD_User_ID());
                }
            }
            catch
            {
                //ShowMessage.Error("MOrder", null, "SetBPartner");
            }
        }

        /**
         * 	Set Business Partner - Callout
         *	@param oldC_BPartner_ID old BP
         *	@param newC_BPartner_ID new BP
         *	@param windowNo window no
         */
        //@UICallout 
        public void SetC_BPartner_ID(String oldC_BPartner_ID, String newC_BPartner_ID, int windowNo)
        {
            if (newC_BPartner_ID == null || newC_BPartner_ID.Length == 0)
                return;
            int C_BPartner_ID = Convert.ToInt32(newC_BPartner_ID);
            if (C_BPartner_ID == 0)
                return;

            // Skip these steps for RMA. These fields are copied over from the orignal order instead.
            if (IsReturnTrx())
                return;

            String sql = "SELECT p.AD_Language,p.C_PaymentTerm_ID,"
                + " COALESCE(p.M_PriceList_ID,g.M_PriceList_ID) AS M_PriceList_ID, p.PaymentRule,p.POReference,"
                + " p.SO_Description,p.IsDiscountPrinted,"
                + " p.InvoiceRule,p.DeliveryRule,p.FreightCostRule,DeliveryViaRule,"
                + " p.SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
                + " lship.C_BPartner_Location_ID,c.AD_User_ID,"
                + " COALESCE(p.PO_PriceList_ID,g.PO_PriceList_ID) AS PO_PriceList_ID, p.PaymentRulePO,p.PO_PaymentTerm_ID,"
                + " lbill.C_BPartner_Location_ID AS Bill_Location_ID, p.SOCreditStatus, lbill.IsShipTo "
                + "FROM C_BPartner p"
                + " INNER JOIN C_BP_Group g ON (p.C_BP_Group_ID=g.C_BP_Group_ID)"
                + " LEFT OUTER JOIN C_BPartner_Location lbill ON (p.C_BPartner_ID=lbill.C_BPartner_ID AND lbill.IsBillTo='Y' AND lbill.IsActive='Y')"
                + " LEFT OUTER JOIN C_BPartner_Location lship ON (p.C_BPartner_ID=lship.C_BPartner_ID AND lship.IsShipTo='Y' AND lship.IsActive='Y')"
                + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
                + "WHERE p.C_BPartner_ID=" + C_BPartner_ID + " AND p.IsActive='Y'";		//	#1

            bool isSOTrx = IsSOTrx();

            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {

                    base.SetC_BPartner_ID(C_BPartner_ID);

                    //	PriceList (indirect: IsTaxIncluded & Currency)
                    int ii = Utility.Util.GetValueOfInt(dr[isSOTrx ? "M_PriceList_ID" : "PO_PriceList_ID"].ToString());
                    if (ii != 0)
                        SetM_PriceList_ID(null, ii.ToString(), windowNo);
                    else
                    {	//	get default PriceList
                        ii = GetCtx().GetContextAsInt("#M_PriceList_ID");
                        if (ii != 0)
                            SetM_PriceList_ID(null, ii.ToString(), windowNo);
                    }

                    //	Bill-To BPartner
                    SetBill_BPartner_ID(C_BPartner_ID);
                    int bill_Location_ID = Utility.Util.GetValueOfInt(dr["Bill_Location_ID"].ToString());
                    if (bill_Location_ID == 0)
                    {
                        //   p_changeVO.addChangedValue("Bill_Location_ID", (String)null);
                    }
                    else
                    {
                        SetBill_Location_ID(bill_Location_ID);
                    }

                    // Ship-To Location
                    int shipTo_ID = Utility.Util.GetValueOfInt(dr["C_BPartner_Location_ID"].ToString());
                    //	overwritten by InfoBP selection - works only if InfoWindow
                    //	was used otherwise creates error (uses last value, may belong to differnt BP)
                    if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "C_BPartner_ID") == C_BPartner_ID)
                    {
                        String loc = GetCtx().GetContext(Env.WINDOW_INFO, Env.TAB_INFO, "C_BPartner_Location_ID");
                        if (loc.Length > 0)
                            shipTo_ID = int.Parse(loc);
                    }
                    if (shipTo_ID == 0)
                    {
                        // p_changeVO.addChangedValue("C_BPartner_Location_ID", (String)null);
                    }
                    else
                    {
                        SetC_BPartner_Location_ID(shipTo_ID);
                    }
                    if ("Y".Equals(dr["IsShipTo"].ToString()))	//	set the same
                        SetBill_Location_ID(shipTo_ID);

                    //	Contact - overwritten by InfoBP selection
                    int contID = Utility.Util.GetValueOfInt(dr["AD_User_ID"].ToString());
                    if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "C_BPartner_ID") == C_BPartner_ID)
                    {
                        String cont = GetCtx().GetContext(Env.WINDOW_INFO, Env.TAB_INFO, "AD_User_ID");
                        if (cont.Length > 0)
                            contID = int.Parse(cont);
                    }
                    SetAD_User_ID(contID);
                    SetBill_User_ID(contID);

                    //	CreditAvailable 
                    if (isSOTrx)
                    {
                        Decimal CreditLimit = Utility.Util.GetValueOfDecimal(dr["SO_CreditLimit"].ToString());
                        //	String SOCreditStatus = dr.getString("SOCreditStatus");
                        if (CreditLimit != null && Env.Signum(CreditLimit) != 0)
                        {
                            Decimal CreditAvailable = Utility.Util.GetValueOfDecimal(dr["CreditAvailable"].ToString());
                            //if (p_changeVO != null && CreditAvailable != null && CreditAvailable.signum() < 0)
                            //{
                            //    String msg = Msg.getMsg(GetCtx(), "CreditLimitOver",DisplayType.getNumberFormat(DisplayType.Amount).format(CreditAvailable));
                            //    p_changeVO.addError(msg);
                            //}
                        }
                    }

                    //	PO Reference
                    String s = dr["POReference"].ToString();
                    if (s != null && s.Length != 0)
                        SetPOReference(s);

                    //	SO Description
                    s = dr["SO_Description"].ToString();
                    if (s != null && s.Trim().Length != 0)
                        SetDescription(s);
                    //	IsDiscountPrinted
                    s = dr["IsDiscountPrinted"].ToString();
                    SetIsDiscountPrinted("Y".Equals(s));

                    //	Defaults, if not Walk-in Receipt or Walk-in Invoice
                    String OrderType = GetCtx().GetContext(windowNo, "OrderType");
                    SetInvoiceRule(INVOICERULE_AfterDelivery);
                    SetDeliveryRule(DELIVERYRULE_Availability);
                    SetPaymentRule(PAYMENTRULE_OnCredit);
                    if (OrderType.Equals(DocSubTypeSO_Prepay))
                    {
                        SetInvoiceRule(INVOICERULE_Immediate);
                        SetDeliveryRule(DELIVERYRULE_AfterReceipt);
                    }
                    else if (OrderType.Equals(MOrder.DocSubTypeSO_POS))	//  for POS
                        SetPaymentRule(PAYMENTRULE_Cash);
                    else
                    {
                        //	PaymentRule
                        s = dr[isSOTrx ? "PaymentRule" : "PaymentRulePO"].ToString();
                        if (s != null && s.Length != 0)
                        {
                            if (s.Equals("B"))				//	No Cache in Non POS
                                s = PAYMENTRULE_OnCredit;	//  Payment Term
                            if (isSOTrx && (s.Equals("S") || s.Equals("U")))	//	No Check/Transfer for SO_Trx
                                s = PAYMENTRULE_OnCredit;	//  Payment Term
                            SetPaymentRule(s);
                        }
                        //	Payment Term
                        ii = Utility.Util.GetValueOfInt(dr[isSOTrx ? "C_PaymentTerm_ID" : "PO_PaymentTerm_ID"].ToString());
                        if (ii != 0)
                            SetC_PaymentTerm_ID(ii);
                        //	InvoiceRule
                        s = dr["InvoiceRule"].ToString();
                        if (s != null && s.Length != 0)
                            SetInvoiceRule(s);
                        //	DeliveryRule
                        s = dr["DeliveryRule"].ToString();
                        if (s != null && s.Length != 0)
                            SetDeliveryRule(s);
                        //	FreightCostRule
                        s = dr["FreightCostRule"].ToString();
                        if (s != null && s.Length != 0)
                            SetFreightCostRule(s);
                        //	DeliveryViaRule
                        s = dr["DeliveryViaRule"].ToString();
                        if (s != null && s.Length != 0)
                            SetDeliveryViaRule(s);
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "MOrder" + e.Message, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                dt = null;
            }
        }


        /**
         * 	Set Bill Business Partner - Callout
         *	@param oldBill_BPartner_ID old BP
         *	@param newBill_BPartner_ID new BP
         *	@param windowNo window no
         */
        //@UICallout
        public void SetBill_BPartner_ID(String oldBill_BPartner_ID, String newBill_BPartner_ID, int windowNo)
        {
            if (newBill_BPartner_ID == null || newBill_BPartner_ID.Length == 0)
                return;
            int bill_BPartner_ID = int.Parse(newBill_BPartner_ID);
            if (bill_BPartner_ID == 0)
                return;

            // Skip these steps for RMA. These fields are copied over from the orignal order instead.
            if (IsReturnTrx())
                return;

            String sql = "SELECT p.AD_Language,p.C_PaymentTerm_ID,"
                + "p.M_PriceList_ID,p.PaymentRule,p.POReference,"
                + "p.SO_Description,p.IsDiscountPrinted,"
                + "p.InvoiceRule,p.DeliveryRule,p.FreightCostRule,DeliveryViaRule,"
                + "p.SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
                + "c.AD_User_ID,"
                + "p.PO_PriceList_ID, p.PaymentRulePO, p.PO_PaymentTerm_ID,"
                + "lbill.C_BPartner_Location_ID AS Bill_Location_ID "
                + "FROM C_BPartner p"
                + " LEFT OUTER JOIN C_BPartner_Location lbill ON (p.C_BPartner_ID=lbill.C_BPartner_ID AND lbill.IsBillTo='Y' AND lbill.IsActive='Y')"
                + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
                + "WHERE p.C_BPartner_ID=" + bill_BPartner_ID + " AND p.IsActive='Y'";		//	#1

            bool isSOTrx = IsSOTrx();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {

                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    base.SetBill_BPartner_ID(bill_BPartner_ID);
                    //	PriceList (indirect: IsTaxIncluded & Currency)
                    int ii = Utility.Util.GetValueOfInt(dr[isSOTrx ? "M_PriceList_ID" : "PO_PriceList_ID"].ToString());
                    if (ii != 0)
                        SetM_PriceList_ID(null, ii.ToString(), windowNo);
                    else
                    {	//	get default PriceList
                        ii = GetCtx().GetContextAsInt("#M_PriceList_ID");
                        if (ii != 0)
                            SetM_PriceList_ID(null, ii.ToString(), windowNo);
                    }

                    int bill_Location_ID = Utility.Util.GetValueOfInt(dr["Bill_Location_ID"].ToString());
                    //	overwritten by InfoBP selection - works only if InfoWindow
                    //	was used otherwise creates error (uses last value, may belong to differnt BP)
                    if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "C_BPartner_ID") == bill_BPartner_ID)
                    {
                        String loc = GetCtx().GetContext(Env.WINDOW_INFO, Env.TAB_INFO, "C_BPartner_Location_ID");
                        if (loc.Length > 0)
                            bill_Location_ID = int.Parse(loc);
                    }
                    if (bill_Location_ID != 0)
                        SetBill_Location_ID(bill_Location_ID);

                    //	Contact - overwritten by InfoBP selection
                    int contID = Utility.Util.GetValueOfInt(dr["AD_User_ID"].ToString());
                    if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "C_BPartner_ID") == bill_BPartner_ID)
                    {
                        String cont = GetCtx().GetContext(Env.WINDOW_INFO, Env.TAB_INFO, "AD_User_ID");
                        if (cont.Length > 0)
                            contID = int.Parse(cont);
                    }
                    SetBill_User_ID(contID);

                    //	CreditAvailable 
                    if (isSOTrx)
                    {
                        Decimal CreditLimit = Utility.Util.GetValueOfDecimal(dr["SO_CreditLimit"].ToString());
                        //	String SOCreditStatus = dr.getString("SOCreditStatus");
                        if (CreditLimit != null && Env.Signum(CreditLimit) != 0)
                        {
                            Decimal CreditAvailable = Utility.Util.GetValueOfDecimal(dr["CreditAvailable"].ToString());
                            //if (p_changeVO != null && CreditAvailable != null && Env.Signum(CreditAvailable) < 0)
                            //{
                            //    String msg = Msg.getMsg(GetCtx(), "CreditLimitOver",DisplayType.getNumberFormat(DisplayType.Amount).format(CreditAvailable));
                            //    p_changeVO.addError(msg);
                            //}
                        }
                    }

                    //	PO Reference
                    String s = dr["POReference"].ToString();

                    // Order Reference should not be reset by Bill To BPartner; only by BPartner 
                    /*if (s != null && s.Length != 0)
                        setPOReference(s); */
                    //	SO Description
                    s = dr["SO_Description"].ToString();
                    if (s != null && s.Trim().Length != 0)
                        SetDescription(s);
                    //	IsDiscountPrinted
                    s = dr["IsDiscountPrinted"].ToString();
                    SetIsDiscountPrinted("Y".Equals(s));

                    //	Defaults, if not Walk-in Receipt or Walk-in Invoice
                    //	Defaults, if not Walk-in Receipt or Walk-in Invoice
                    String OrderType = GetCtx().GetContext(windowNo, "OrderType");
                    SetInvoiceRule(INVOICERULE_AfterDelivery);
                    SetPaymentRule(PAYMENTRULE_OnCredit);
                    if (OrderType.Equals(DocSubTypeSO_Prepay))
                        SetInvoiceRule(INVOICERULE_Immediate);
                    else if (OrderType.Equals(MOrder.DocSubTypeSO_POS))	//  for POS
                        SetPaymentRule(PAYMENTRULE_Cash);
                    else
                    {
                        //	PaymentRule
                        s = dr[isSOTrx ? "PaymentRule" : "PaymentRulePO"].ToString();
                        if (s != null && s.Length != 0)
                        {
                            if (s.Equals("B"))				//	No Cache in Non POS
                                s = PAYMENTRULE_OnCredit;	//  Payment Term
                            if (isSOTrx && (s.Equals("S") || s.Equals("U")))	//	No Check/Transfer for SO_Trx
                                s = PAYMENTRULE_OnCredit;	//  Payment Term
                            SetPaymentRule(s);
                        }
                        //	Payment Term
                        ii = Utility.Util.GetValueOfInt(dr[isSOTrx ? "C_PaymentTerm_ID" : "PO_PaymentTerm_ID"].ToString());
                        if (ii != 0)
                            SetC_PaymentTerm_ID(ii);
                        //	InvoiceRule
                        s = dr["InvoiceRule"].ToString();
                        if (s != null && s.Length != 0)
                            SetInvoiceRule(s);
                    }
                }

                //dt.Dispose();
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "MOrder" + sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                dt = null;
            }
        }


        /**
         * 	Set Business Partner Location (Ship+Bill)
         *	@param C_BPartner_Location_ID bp location
         */
        public new void SetC_BPartner_Location_ID(int C_BPartner_Location_ID)
        {
            base.SetC_BPartner_Location_ID(C_BPartner_Location_ID);
            base.SetBill_Location_ID(C_BPartner_Location_ID);
        }


        /// <summary>
        /// Set Business Partner Contact (Ship+Bill)
        /// </summary>
        /// <param name="AD_User_ID">user</param>
        public new void SetAD_User_ID(int AD_User_ID)
        {
            base.SetAD_User_ID(AD_User_ID);
            base.SetBill_User_ID(AD_User_ID);
        }

        /*	Set Ship Business Partner
        *	@param C_BPartner_ID bpartner
        */
        public void SetShip_BPartner_ID(int C_BPartner_ID)
        {
            base.SetC_BPartner_ID(C_BPartner_ID);
        }

        /**
         * 	Set Ship Business Partner Location
         *	@param C_BPartner_Location_ID bp location
         */
        public void SetShip_Location_ID(int C_BPartner_Location_ID)
        {
            base.SetC_BPartner_Location_ID(C_BPartner_Location_ID);
        }

        /**
         * 	Set Ship Business Partner Contact
         *	@param AD_User_ID user
         */
        public void SetShip_User_ID(int AD_User_ID)
        {
            base.SetAD_User_ID(AD_User_ID);
        }


        /**
         * 	Set Warehouse
         *	@param M_Warehouse_ID warehouse
         */
        public new void SetM_Warehouse_ID(int M_Warehouse_ID)
        {
            base.SetM_Warehouse_ID(M_Warehouse_ID);
        }

        /**
         * 	Set Drop Ship
         *	@param IsDropShip drop ship
         */
        public new void SetIsDropShip(bool IsDropShip)
        {
            base.SetIsDropShip(IsDropShip);
        }

        /**
         * 	Set DateOrdered - Callout
         *	@param oldDateOrdered old
         *	@param newDateOrdered new
         *	@param windowNo window no
         */
        //@UICallout 
        public void SetDateOrdered(String oldDateOrdered, String newDateOrdered, int windowNo)
        {
            try
            {
                if (newDateOrdered == null || newDateOrdered.Length == 0)
                {
                    return;
                }
                DateTime? dateOrdered = (DateTime?)PO.ConvertToTimestamp(newDateOrdered);
                if (dateOrdered == null)
                {
                    return;
                }
                SetDateOrdered(dateOrdered);
            }
            catch
            {
                //ShowMessage.Error("MOrder", null, "SetDateOrdered");
            }
        }

        /**
         *	Set Date Ordered and Acct Date
         */
        public new void SetDateOrdered(DateTime? dateOrdered)
        {
            base.SetDateOrdered(dateOrdered);
            base.SetDateAcct(dateOrdered);
        }


        /*	Set Target Sales Document Type - Callout.
        * 	Sets OrderType (=DocSubTypeSO), HasCharges [ctx only]
        * 	IsDropShip, DeliveryRule, InvoiceRule, PaymentRule, IsSOTrx, DocumentNo
        * 	If BP is changed: PaymentRule, C_PaymentTerm_ID, InvoiceRule, DeliveryRule,
        * 	FreightCostRule, DeliveryViaRule
        * 	@param oldC_DocTypeTarget_ID old ID
        * 	@param newC_DocTypeTarget_ID new ID
        * 	@param windowNo window
        */
        //@UICallout
        public void SetC_DocTypeTarget_ID(String oldC_DocTypeTarget_ID, String newC_DocTypeTarget_ID, int windowNo)
        {
            if (newC_DocTypeTarget_ID == null || newC_DocTypeTarget_ID.Length == 0)
                return;
            int C_DocTypeTarget_ID = int.Parse(newC_DocTypeTarget_ID);
            if (C_DocTypeTarget_ID == 0)
                return;

            //	Re-Create new DocNo, if there is a doc number already
            //	and the existing source used a different Sequence number
            String oldDocNo = GetDocumentNo();
            bool newDocNo = (oldDocNo == null);
            if (!newDocNo && oldDocNo.StartsWith("<") && oldDocNo.EndsWith(">"))
                newDocNo = true;
            int oldC_DocType_ID = GetC_DocType_ID();

            String sql = "SELECT d.DocSubTypeSO,d.HasCharges,'N',"			//	1..3
                + "d.IsDocNoControlled,s.CurrentNext,s.CurrentNextSys,"     //  4..6
                + "s.AD_Sequence_ID,d.IsSOTrx,d.IsReturnTrx "               //	7..9
                + "FROM C_DocType d "
                + "LEFT OUTER JOIN AD_Sequence s ON (d.DocNoSequence_ID=s.AD_Sequence_ID) "
                + "WHERE C_DocType_ID=";	//	#1
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                int AD_Sequence_ID = 0;

                //	Get old AD_SeqNo for comparison
                if (!newDocNo && oldC_DocType_ID != 0)
                {
                    sql = sql + oldC_DocType_ID;
                    idr = DataBase.DB.ExecuteReader(sql, null, null);
                    dt = new DataTable();
                    dt.Load(idr);
                    idr.Close();
                    foreach (DataRow dr in dt.Rows)
                    {
                        AD_Sequence_ID = Utility.Util.GetValueOfInt(dr[5].ToString());
                    }
                    dt = null;
                }
                sql = sql + C_DocTypeTarget_ID;
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                String DocSubTypeSO = "";
                bool isSOTrx = true;
                bool isReturnTrx = false;
                foreach (DataRow dr in dt.Rows)		//	we found document type
                {
                    base.SetC_DocTypeTarget_ID(C_DocTypeTarget_ID);
                    //	Set Ctx:	Document Sub Type for Sales Orders
                    DocSubTypeSO = dr[0].ToString();
                    if (DocSubTypeSO == null)
                        DocSubTypeSO = "--";
                    //if (p_changeVO != null)
                    //    p_changeVO.setContext(GetCtx(), windowNo, "OrderType", DocSubTypeSO);
                    //	No Drop Ship other than Standard
                    if (!DocSubTypeSO.Equals(DocSubTypeSO_Standard))
                        SetIsDropShip(false);

                    //	IsSOTrx
                    if ("N".Equals(dr[7].ToString()))
                        isSOTrx = false;
                    SetIsSOTrx(isSOTrx);

                    // IsReturnTrx
                    isReturnTrx = "Y".Equals(dr[8].ToString());
                    SetIsReturnTrx(isReturnTrx);

                    if (!isReturnTrx)
                    {
                        //	Delivery Rule
                        if (DocSubTypeSO.Equals(MOrder.DocSubTypeSO_POS))
                            SetDeliveryRule(DELIVERYRULE_Force);
                        else if (DocSubTypeSO.Equals(MOrder.DocSubTypeSO_Prepay))
                            SetDeliveryRule(DELIVERYRULE_AfterReceipt);
                        else
                            SetDeliveryRule(DELIVERYRULE_Availability);

                        //	Invoice Rule
                        if (DocSubTypeSO.Equals(DocSubTypeSO_POS)
                            || DocSubTypeSO.Equals(DocSubTypeSO_Prepay)
                            || DocSubTypeSO.Equals(DocSubTypeSO_OnCredit))
                            SetInvoiceRule(INVOICERULE_Immediate);
                        else
                            SetInvoiceRule(INVOICERULE_AfterDelivery);


                        //	Payment Rule - POS Order
                        if (DocSubTypeSO.Equals(DocSubTypeSO_POS))
                            SetPaymentRule(PAYMENTRULE_Cash);
                        else
                            SetPaymentRule(PAYMENTRULE_OnCredit);

                        //	Set Ctx: Charges
                        //if (p_changeVO != null)
                        //    p_changeVO.setContext(GetCtx(), windowNo, "HasCharges", dr.getString(2));
                    }
                    else
                    {
                        if (DocSubTypeSO.Equals(MOrder.DocSubTypeSO_POS))
                            SetDeliveryRule(DELIVERYRULE_Force);
                        else
                            SetDeliveryRule(DELIVERYRULE_Manual);
                    }

                    //	DocumentNo
                    if (dr[3].ToString().Equals("Y"))			//	IsDocNoControlled
                    {
                        if (!newDocNo && AD_Sequence_ID != Utility.Util.GetValueOfInt(dr[6].ToString()))
                            newDocNo = true;
                        if (newDocNo)
                            if (Ini.IsPropertyBool(Ini._VIENNASYS) && Env.GetContext().GetAD_Client_ID() < 1000000)
                            {
                                SetDocumentNo("<" + dr[5].ToString() + ">");
                            }
                            else
                            {
                                SetDocumentNo("<" + dr[4].ToString() + ">");
                            }
                    }
                }

                // Skip remaining steps for RMA. These are copied over from original order.
                if (isReturnTrx)
                    return;

                //  When BPartner is changed, the Rules are not set if
                //  it is a POS or Credit Order (i.e. defaults from Standard BPartner)
                //  This re-reads the Rules and applies them.
                if (DocSubTypeSO.Equals(DocSubTypeSO_POS)
                    || DocSubTypeSO.Equals(DocSubTypeSO_Prepay))    //  not for POS/PrePay
                {
                    ;
                }
                else
                {
                    int C_BPartner_ID = GetC_BPartner_ID();
                    sql = "SELECT PaymentRule,C_PaymentTerm_ID,"            //  1..2
                        + "InvoiceRule,DeliveryRule,"                       //  3..4
                        + "FreightCostRule,DeliveryViaRule, "               //  5..6
                        + "PaymentRulePO,PO_PaymentTerm_ID "
                        + "FROM C_BPartner "
                        + "WHERE C_BPartner_ID=" + C_BPartner_ID;		//	#1
                    dt = null;
                    idr = DataBase.DB.ExecuteReader(sql, null, null);
                    dt = new DataTable();
                    dt.Load(idr);
                    idr.Close();
                    foreach (DataRow dr in dt.Rows)
                    {
                        //	PaymentRule
                        String paymentRule = dr[isSOTrx ? "PaymentRule" : "PaymentRulePO"].ToString();
                        if (paymentRule != null && paymentRule.Length != 0)
                        {
                            if (isSOTrx 	//	No Cash/Check/Transfer for SO_Trx
                                && (paymentRule.Equals(PAYMENTRULE_Cash)
                                    || paymentRule.Equals(PAYMENTRULE_Check)
                                    || paymentRule.Equals(PAYMENTRULE_DirectDeposit)))
                                paymentRule = PAYMENTRULE_OnCredit;				//  Payment Term
                            if (!isSOTrx 	//	No Cash for PO_Trx
                                    && (paymentRule.Equals(PAYMENTRULE_Cash)))
                                paymentRule = PAYMENTRULE_OnCredit;				//  Payment Term
                            SetPaymentRule(paymentRule);
                        }
                        //	Payment Term
                        int C_PaymentTerm_ID = Utility.Util.GetValueOfInt(dr[isSOTrx ? "C_PaymentTerm_ID" : "PO_PaymentTerm_ID"].ToString());
                        if (C_PaymentTerm_ID != 0)
                            SetC_PaymentTerm_ID(C_PaymentTerm_ID);
                        //	InvoiceRule
                        String invoiceRule = dr[2].ToString();
                        if (invoiceRule != null && invoiceRule.Length != 0)
                            SetInvoiceRule(invoiceRule);
                        //	DeliveryRule
                        String deliveryRule = dr[3].ToString();
                        if (deliveryRule != null && deliveryRule.Length != 0)
                            SetDeliveryRule(deliveryRule);
                        //	FreightCostRule
                        String freightCostRule = dr[4].ToString();
                        if (freightCostRule != null && freightCostRule.Length != 0)
                            SetFreightCostRule(freightCostRule);
                        //	DeliveryViaRule
                        String deliveryViaRule = dr[5].ToString();
                        if (deliveryViaRule != null && deliveryViaRule.Length != 0)
                            SetDeliveryViaRule(deliveryViaRule);
                    }
                }   //  re-read customer rules

            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                dt = null;

            }
        }

        /**
         * 	Set Target Sales Document Type
         * 	@param DocSubTypeSO_x SO sub type - see DocSubTypeSO_*
         */
        public void SetC_DocTypeTarget_ID(String DocSubTypeSO_x)
        {
            try
            {
                String sql = "SELECT C_DocType_ID FROM C_DocType "
                    + "WHERE AD_Client_ID=" + GetAD_Client_ID() + " AND AD_Org_ID IN (0," + GetAD_Org_ID()
                    + ") AND DocSubTypeSO='" + DocSubTypeSO_x + "' AND IsReturnTrx='N' "
                    + "ORDER BY AD_Org_ID DESC, IsDefault DESC";
                int C_DocType_ID = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql, null, null));
                if (C_DocType_ID <= 0)
                {
                    log.Severe("Not found for AD_Client_ID=" + GetAD_Client_ID() + ", SubType=" + DocSubTypeSO_x);
                }
                else
                {
                    log.Fine("(SO) - " + DocSubTypeSO_x);
                    SetC_DocTypeTarget_ID(C_DocType_ID);
                    SetIsSOTrx(true);
                    SetIsReturnTrx(false);
                }
            }
            catch
            {
                //ShowMessage.Error("MOrder", null, "SetC_DocTypeTarget_ID");
            }
        }

        /**
         * 	Set Target Document Type
         *	@param C_DocTypeTarget_ID id
         *	@param setReturnTrx if true set ReturnTrx and SOTrx
         */
        public void SetC_DocTypeTarget_ID(int C_DocTypeTarget_ID, bool setReturnTrx)
        {
            try
            {
                base.SetC_DocTypeTarget_ID(C_DocTypeTarget_ID);
                if (setReturnTrx)
                {
                    MDocType dt = MDocType.Get(GetCtx(), C_DocTypeTarget_ID);
                    SetIsSOTrx(dt.IsSOTrx());
                    SetIsReturnTrx(dt.IsReturnTrx());
                }
            }
            catch
            {
                //ShowMessage.Error("MOrder", null, "SetC_DocTypeTarget_ID(int C_DocTypeTarget_ID, bool setReturnTrx)");
            }
        }

        /**
         * 	Set Target Document Type.
         * 	Standard Order or PO
         */
        public void SetC_DocTypeTarget_ID()
        {
            try
            {
                if (IsSOTrx())		//	SO = Std Order
                {
                    SetC_DocTypeTarget_ID(DocSubTypeSO_Standard);
                    return;
                }
                //	PO
                String sql = "SELECT C_DocType_ID FROM C_DocType "
                    + "WHERE AD_Client_ID=" + GetAD_Client_ID() + " AND AD_Org_ID IN (0," + GetAD_Org_ID()
                    + ") AND DocBaseType='POO' AND IsReturnTrx='N' "
                    + "ORDER BY AD_Org_ID DESC, IsDefault DESC";
                int C_DocType_ID = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql, null, null));
                if (C_DocType_ID <= 0)
                {
                    log.Severe("No POO found for AD_Client_ID=" + GetAD_Client_ID());
                }
                else
                {
                    log.Fine("(PO) - " + C_DocType_ID);
                    SetC_DocTypeTarget_ID(C_DocType_ID);
                    SetIsReturnTrx(false);
                }
            }
            catch
            {
                //ShowMessage.Error("MOrder", null, "SetC_DocTypeTarget_ID()");
            }
        }

        // New function added to set document target type according relseased order
        /**
         * 	Set Target Document Type.
         * 	@params bool released order @params
         * 	Standard Order or PO which doesn't have blanket order
         */
        public void SetC_DocTypeTarget_ID(bool ReleaseOrder)
        {
            string Released = "Y";
            try
            {
                if (IsSOTrx())		//	SO = Std Order
                {
                    SetC_DocTypeTarget_ID(DocSubTypeSO_Standard);
                    return;
                }
                if (!ReleaseOrder)
                {
                    Released = "N";
                }
                //	PO
                String sql = "SELECT C_DocType_ID FROM C_DocType "
                    + "WHERE AD_Client_ID=" + GetAD_Client_ID() + " AND AD_Org_ID IN (0," + GetAD_Org_ID()
                    + ") AND DocBaseType='POO' AND IsReturnTrx='N' AND IsReleaseDocument='" + Released + "'"
                    + " ORDER BY AD_Org_ID DESC, IsDefault DESC";
                int C_DocType_ID = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql, null, null));
                if (C_DocType_ID <= 0)
                {
                    log.Severe("No POO found for AD_Client_ID=" + GetAD_Client_ID());
                }
                else
                {
                    log.Fine("(PO) - " + C_DocType_ID);
                    SetC_DocTypeTarget_ID(C_DocType_ID);
                    SetIsReturnTrx(false);
                }
            }
            catch
            {
                //ShowMessage.Error("MOrder", null, "SetC_DocTypeTarget_ID()");
            }
        }

        /* 	Copy Lines From other Order
        *	@param otherOrder order
        *	@param counter set counter Info
        *	@param copyASI copy line attributes Attribute Set Instance, Resaouce Assignment
        *	@return number of lines copied
        */
        public int CopyLinesFrom(MOrder otherOrder, bool counter, bool copyASI, bool fromCreateSO = false)//added optional parameter which indicates from where this function is being called(If this fuction is called from Create SO Process on Sales Quotation window then fromCreateSO is true otherwise it will be false)----Neha
        {
            int count = 0;
            try
            {
                if (IsProcessed() || IsPosted() || otherOrder == null)
                    return 0;
                MOrderLine[] fromLines = otherOrder.GetLines(false, null);

                // Added by Bharat on 05 Jan 2018 to set Values for Blanket Sales Order from Sales Quotation.
                MDocType docType = new MDocType(GetCtx(), GetC_DocTypeTarget_ID(), Get_TrxName());
                string docBaseType = docType.GetDocBaseType();
                for (int i = 0; i < fromLines.Length; i++)
                {
                    //issue JID_1474 If full quantity of any line is released from blanket order then system will not create that line in Release order
                    if (docType.IsReleaseDocument())
                    {
                        if (docBaseType == MDocBaseType.DOCBASETYPE_PURCHASEORDER || docBaseType == MDocBaseType.DOCBASETYPE_SALESORDER)
                        {
                            if (fromLines[i].GetQtyEntered() == 0)
                            {
                                continue;
                            }
                        }
                    }

                    MOrderLine line = new MOrderLine(this);
                    PO.CopyValues(fromLines[i], line, GetAD_Client_ID(), GetAD_Org_ID());

                    line.SetC_Order_ID(GetC_Order_ID());
                    line.SetOrder(this);
                    line.Set_ValueNoCheck("C_OrderLine_ID", I_ZERO);	//	new
                    line.Set_ValueNoCheck("C_Contract_ID", I_ZERO);
                    line.SetCreateServiceContract("N");
                    //	References
                    if (!copyASI)
                    {
                        line.SetM_AttributeSetInstance_ID(0);
                        line.SetS_ResourceAssignment_ID(0);
                    }
                    if (counter)
                        line.SetRef_OrderLine_ID(fromLines[i].GetC_OrderLine_ID());
                    else
                        line.SetRef_OrderLine_ID(0);
                    //
                    if (docBaseType == "BOO")
                    {
                        //Changes done by Neha---10 August 2018---Set QtyEstimation =QtyEntered when we create Blanket Sales Order from Create Sales Order Process
                        if (fromCreateSO)
                            line.Set_ValueNoCheck("QtyEstimation", fromLines[i].GetQtyEntered());
                        else
                            //Changes done by Neha Thakur--2 July,2018--Wrong Qty Estimation was updated.Set QtyEstimation at the place of QtyEntered when Copy Order Line from Copy From Process on header tab--Asked by Vineet/Pradeep
                            line.Set_ValueNoCheck("QtyEstimation", fromLines[i].GetQtyEstimation());
                    }

                    // Set Reference of Blanket Order Line on Release Order Line.
                    if (docType.IsReleaseDocument())
                    {
                        line.SetC_OrderLine_Blanket_ID(fromLines[i].GetC_OrderLine_ID());
                        // Blanket order qty not updated correctly by Release order process
                        line.SetQtyBlanket(fromLines[i].GetQtyOrdered());
                    }

                    // Added by Bharat on 06 Jan 2018 to set Values on Sales Order from Sales Quotation.
                    if (line.Get_ColumnIndex("C_Quotation_Line_ID") >= 0)
                        line.Set_Value("C_Quotation_Line_ID", fromLines[i].GetC_OrderLine_ID());
                    // Added by Bharat on 06 Jan 2018 to set Values on Sales Order from Sales Quotation.
                    if (line.Get_ColumnIndex("C_Order_Quotation") >= 0)
                        line.Set_Value("C_Order_Quotation", fromLines[i].GetC_Order_ID());

                    line.SetQtyDelivered(Env.ZERO);
                    line.SetQtyInvoiced(Env.ZERO);
                    line.SetQtyReserved(Env.ZERO);
                    line.SetQtyReleased(Env.ZERO);      // set Qty Released to Zero.
                    line.SetDateDelivered(null);
                    line.SetDateInvoiced(null);
                    //	Tax
                    if (GetC_BPartner_ID() != otherOrder.GetC_BPartner_ID())
                    {
                        line.SetTax();                 //	recalculate
                        //1052-- set tax exempt reason null if business partner is different
                        line.SetIsTaxExempt(false);
                        line.SetC_TaxExemptReason_ID(0);
                    }
                    //

                    //	Tax Amount
                    // JID_1319: System should not copy Tax Amount, Line Total Amount and Taxable Amount field. System Should Auto Calculate thease field On save of lines.
                    if (GetM_PriceList_ID() != otherOrder.GetM_PriceList_ID())
                        line.SetTaxAmt();		//	recalculate Tax Amount

                    // ReCalculate Surcharge Amount
                    if (line.Get_ColumnIndex("SurchargeAmt") >= 0)
                    {
                        line.SetSurchargeAmt(Env.ZERO);
                    }

                    //
                    line.SetProcessed(false);
                    if (line.Save(Get_TrxName()))
                        count++;
                    //	Cross Link
                    if (counter)
                    {
                        fromLines[i].SetRef_OrderLine_ID(line.GetC_OrderLine_ID());
                        fromLines[i].Save(Get_TrxName());
                    }
                }
                if (fromLines.Length != count)
                {
                    log.Log(Level.SEVERE, "Line difference - From=" + fromLines.Length + " <> Saved=" + count);
                }

                if (!CalculateTaxTotal())   //	setTotals
                {
                    log.Info(Msg.GetMsg(GetCtx(), "ErrorCalculateTax") + ": " + GetDocumentNo().ToString());
                }

                // Update order header
                UpdateHeader();
            }
            catch
            {
                //ShowMessage.Error("MOrder", null, "CopyLinesFrom");
            }
            return count;
        }

        /// <summary>
        /// Update Order Header
        /// </summary>
        /// <returns>true if header updated</returns>
        public void UpdateHeader()
        {
            //	Update Order Header
            String sql = "UPDATE C_Order i"
                + " SET TotalLines="
                    + "(SELECT COALESCE(SUM(LineNetAmt),0) FROM C_OrderLine il WHERE i.C_Order_ID=il.C_Order_ID) "
                    + ", AmtDimSubTotal = null "
                    + ", AmtDimGrandTotal = null "
                + "WHERE C_Order_ID=" + GetC_Order_ID();
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 1)
            {
                log.Warning("(1) #" + no);
            }

            if (IsTaxIncluded())
                sql = "UPDATE C_Order i "
                    + "SET GrandTotal=TotalLines "
                    + "WHERE C_Order_ID=" + GetC_Order_ID();
            else
                sql = "UPDATE C_Order i "
                    + "SET GrandTotal=TotalLines+"
                        + "(SELECT COALESCE(SUM(TaxAmt),0) FROM C_OrderTax it WHERE i.C_Order_ID=it.C_Order_ID) "
                        + "WHERE C_Order_ID=" + GetC_Order_ID();
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 1)
            {
                log.Warning("(2) #" + no);
            }
        }

        /*	String Representation
        *	@return Info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MOrder[")
                .Append(Get_ID()).Append("-").Append(GetDocumentNo())
                .Append(",IsSOTrx=").Append(IsSOTrx())
                .Append(",C_DocType_ID=").Append(GetC_DocType_ID())
                .Append(", GrandTotal=").Append(GetGrandTotal())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Get Document Info
        /// </summary>
        /// <returns>document Info (untranslated)</returns>
        public String GetDocumentInfo()
        {
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            return dt.GetName() + " " + GetDocumentNo();
        }

        /// <summary>
        /// Create PDF
        /// </summary>
        /// <returns>File or null</returns>
        public FileInfo CreatePDF()
        {
            try
            {
                //string fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo()
                String fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo() + ".pdf";
                string filePath = Path.Combine(GlobalVariable.PhysicalPath, "TempDownload", fileName);


                ReportEngine_N re = ReportEngine_N.Get(GetCtx(), ReportEngine_N.ORDER, GetC_Order_ID());
                if (re == null)
                    return null;

                re.GetView();
                bool b = re.CreatePDF(filePath);

                //File temp = File.createTempFile(Get_TableName() + Get_ID() + "_", ".pdf");
                //FileStream fOutStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

                FileInfo temp = new FileInfo(filePath);
                if (!temp.Exists)
                {
                    b = re.CreatePDF(filePath);
                    if (b)
                    {
                        return new FileInfo(filePath);
                    }
                    return null;
                }
                else
                    return temp;
            }
            catch (Exception e)
            {
                log.Severe("Could not create PDF - " + e.Message);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public FileInfo CreatePDF(FileInfo file)
        {

            return null;

        }

        /*	Set Price List (and Currency, TaxIncluded) when valid
        * 	@param M_PriceList_ID price list
        */
        public new void SetM_PriceList_ID(int M_PriceList_ID)
        {
            MPriceList pl = MPriceList.Get(GetCtx(), M_PriceList_ID, null);
            if (pl.Get_ID() == M_PriceList_ID)
            {
                base.SetM_PriceList_ID(M_PriceList_ID);
                SetC_Currency_ID(pl.GetC_Currency_ID());
                SetIsTaxIncluded(pl.IsTaxIncluded());
            }
        }

        /*	Set Price List - Callout
        *	@param oldM_PriceList_ID old value
        *	@param newM_PriceList_ID new value
        *	@param windowNo window
        *	@throws Exception
        */
        //@UICallout
        public void SetM_PriceList_ID(String oldM_PriceList_ID, String newM_PriceList_ID, int windowNo)
        {
            if (newM_PriceList_ID == null || newM_PriceList_ID.Length == 0)
                return;
            int M_PriceList_ID = int.Parse(newM_PriceList_ID);
            if (M_PriceList_ID == 0)
                return;

            String sql = "SELECT pl.IsTaxIncluded,pl.EnforcePriceLimit,pl.C_Currency_ID,c.StdPrecision,"
                + "plv.M_PriceList_Version_ID,plv.ValidFrom "
                + "FROM M_PriceList pl,C_Currency c,M_PriceList_Version plv "
                + "WHERE pl.C_Currency_ID=c.C_Currency_ID"
                + " AND pl.M_PriceList_ID=plv.M_PriceList_ID"
                + " AND pl.M_PriceList_ID=" + M_PriceList_ID						//	1
                + "ORDER BY plv.ValidFrom DESC";

            //	Use newest price list - may not be future
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    base.SetM_PriceList_ID(M_PriceList_ID);
                    //	Tax Included
                    SetIsTaxIncluded("Y".Equals(dr[0].ToString()));
                    //	Price Limit Enforce
                    //if (p_changeVO != null)
                    //    p_changeVO.setContext(GetCtx(), windowNo, "EnforcePriceLimit", dr.getString(2));
                    //	Currency
                    int ii = Utility.Util.GetValueOfInt(dr[2].ToString());
                    SetC_Currency_ID(ii);
                    //	PriceList Version
                    //if (p_changeVO != null)
                    //    p_changeVO.setContext(GetCtx(), windowNo, "M_PriceList_Version_ID", dr.getInt(5));
                }

            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "MOrder" + sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                dt = null;
            }
        }

        /// <summary>
        /// Set Return Policy
        /// </summary>
        public void SetM_ReturnPolicy_ID()
        {
            try
            {
                MBPartner bpartner = new MBPartner(GetCtx(), GetC_BPartner_ID(), null);
                if (bpartner.Get_ID() != 0)
                {
                    if (IsSOTrx())
                    {
                        base.SetM_ReturnPolicy_ID(bpartner.GetM_ReturnPolicy_ID());
                    }
                    else
                    {
                        base.SetM_ReturnPolicy_ID(bpartner.GetPO_ReturnPolicy_ID());
                    }
                }
            }
            catch
            {
                //ShowMessage.Error("MOrder", null, "SetM_ReturnPolicy_ID");
            }

        }

        /* 	Set Original Order for RMA
         * 	SOTrx should be set.
         * 	@param origOrder MOrder
         */
        public void SetOrigOrder(MOrder origOrder)
        {
            try
            {
                if (origOrder == null || origOrder.Get_ID() == 0)
                    return;

                SetOrig_Order_ID(origOrder.GetC_Order_ID());
                //	Get Details from Original Order
                MBPartner bpartner = new MBPartner(GetCtx(), origOrder.GetC_BPartner_ID(), null);

                // Reset Original Shipment
                SetOrig_InOut_ID(-1);
                SetC_BPartner_ID(origOrder.GetC_BPartner_ID());
                SetC_BPartner_Location_ID(origOrder.GetC_BPartner_Location_ID());
                SetAD_User_ID(origOrder.GetAD_User_ID());
                SetBill_BPartner_ID(origOrder.GetBill_BPartner_ID());
                SetBill_Location_ID(origOrder.GetBill_Location_ID());
                SetBill_User_ID(origOrder.GetBill_User_ID());

                SetM_ReturnPolicy_ID();

                SetM_PriceList_ID(origOrder.GetM_PriceList_ID());
                SetPaymentRule(origOrder.GetPaymentRule());
                SetC_PaymentTerm_ID(origOrder.GetC_PaymentTerm_ID());
                //setDeliveryRule(X_C_Order.DELIVERYRULE_Manual);

                SetBill_Location_ID(origOrder.GetBill_Location_ID());
                SetInvoiceRule(origOrder.GetInvoiceRule());
                SetPaymentRule(origOrder.GetPaymentRule());
                SetDeliveryViaRule(origOrder.GetDeliveryViaRule());
                SetFreightCostRule(origOrder.GetFreightCostRule());
            }
            catch
            {
                //ShowMessage.Error("MOrder", null, "SetOrigOrder");
            }
            return;

        }

        /*	Set Original Order - Callout
        *	@param oldOrig_Order_ID old Orig Order
        *	@param newOrig_Order_ID new Orig Order
        *	@param windowNo window no
        */
        //@UICallout
        public void SetOrig_Order_ID(String oldOrig_Order_ID, String newOrig_Order_ID, int windowNo)
        {
            try
            {
                if (newOrig_Order_ID == null || newOrig_Order_ID.Length == 0)
                    return;
                int Orig_Order_ID = int.Parse(newOrig_Order_ID);
                if (Orig_Order_ID == 0)
                {
                    return;
                }

                //		Get Details
                MOrder origOrder = new MOrder(GetCtx(), Orig_Order_ID, null);
                if (origOrder.Get_ID() != 0)
                {
                    SetOrigOrder(origOrder);
                }
            }
            catch
            {
                //ShowMessage.Error("MOrder", null, "SetOrig_Order_ID-callout");
            }

        }

        /*	Set Original Shipment for RMA
        * 	SOTrx should be set.
        * 	@param origInOut MInOut
        */
        public void SetOrigInOut(MInOut origInOut)
        {
            try
            {
                if (origInOut == null || origInOut.Get_ID() == 0)
                {
                    return;
                }
                SetOrig_InOut_ID(origInOut.GetM_InOut_ID());
                SetC_Project_ID(origInOut.GetC_Project_ID());
                SetC_Campaign_ID(origInOut.GetC_Campaign_ID());
                SetC_Activity_ID(origInOut.GetC_Activity_ID());
                SetAD_OrgTrx_ID(origInOut.GetAD_OrgTrx_ID());
                SetUser1_ID(origInOut.GetUser1_ID());
                SetUser2_ID(origInOut.GetUser2_ID());
            }
            catch
            {
                //ShowMessage.Error("MOrder", null, "SetOrigInOut");
            }

            return;

        }

        /*	Set Original Shipment - Callout
        *	@param oldOrig_InOut_ID old Orig Order
        *	@param newOrig_InOut_ID new Orig Order
        *	@param windowNo window no
        */
        //@UICallout
        public void SetOrig_InOut_ID(String oldOrig_InOut_ID, String newOrig_InOut_ID, int windowNo)
        {
            try
            {
                if (newOrig_InOut_ID == null || newOrig_InOut_ID.Length == 0)
                    return;
                int Orig_InOut_ID = int.Parse(newOrig_InOut_ID);
                if (Orig_InOut_ID == 0)
                    return;
                //		Get Details
                MInOut origInOut = new MInOut(GetCtx(), Orig_InOut_ID, null);
                if (origInOut.Get_ID() != 0)
                    SetOrigInOut(origInOut);
            }
            catch
            {

                //ShowMessage.Error("MOrder", null, "SetOrig_InOut_ID");
            }

        }

        /// <summary>
        /// Get Lines of Order
        /// </summary>
        /// <param name="whereClause">where clause or null (starting with AND)</param>
        /// <param name="orderClause">order clause</param>
        /// <returns>lines</returns>
        public MOrderLine[] GetLines(String whereClause, String orderClause)
        {
            List<MOrderLine> list = new List<MOrderLine>();
            StringBuilder sql = new StringBuilder("SELECT * FROM C_OrderLine WHERE C_Order_ID=" + GetC_Order_ID() + "");
            if (whereClause != null)
                sql.Append(whereClause);
            if (orderClause != null)
                sql.Append(" ").Append(orderClause);
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql.ToString(), null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        MOrderLine ol = new MOrderLine(GetCtx(), dr, Get_TrxName());
                        ol.SetHeaderInfo(this);
                        //JID_1673 Quantity entered should not be zero
                        //if ((Utility.Util.GetValueOfDecimal(dr["QtyEntered"])) > 0)
                        list.Add(ol);
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql.ToString(), e);
            }
            //
            MOrderLine[] lines = new MOrderLine[list.Count];
            lines = list.ToArray();
            return lines;
        }

        /// <summary>
        /// Get Lines of Order
        /// </summary>
        /// <param name="requery">requery</param>
        /// <param name="orderBy">optional order by column</param>
        /// <returns>lines</returns>
        public MOrderLine[] GetLines(bool requery, String orderBy)
        {
            try
            {
                if (_lines != null && !requery)
                {
                    return _lines;
                }
                //
                String orderClause = "ORDER BY ";
                if (orderBy != null && orderBy.Length > 0)
                {
                    orderClause += orderBy;
                }
                else
                {
                    orderClause += "Line";
                }
                _lines = GetLines(null, orderClause);

            }
            catch
            {

                //ShowMessage.Error("MOrder", null, "GetLines");
            }
            return _lines;
        }

        /// <summary>
        /// Get Lines of Order.
        /// </summary>
        /// <returns>lines</returns>
        public MOrderLine[] GetLines()
        {
            return GetLines(false, null);
        }

        /// <summary>
        /// Get Lines of Order for a given product
        /// </summary>
        /// <param name="M_Product_ID"></param>
        /// <param name="whereClause"></param>
        /// <param name="orderClause">order clause</param>
        /// <returns>lines</returns>
        /// <date>10-March-2011</date>
        /// <writer>raghu</writer>
        public MOrderLine[] GetLines(int M_Product_ID, String whereClause, String orderClause)
        {
            List<MOrderLine> list = new List<MOrderLine>();
            StringBuilder sql = new StringBuilder("SELECT * FROM C_OrderLine WHERE C_Order_ID=" + GetC_Order_ID() + " AND M_Product_ID=" + M_Product_ID);

            if (whereClause != null)
                sql.Append(" AND ").Append(whereClause);

            if (orderClause != null)
                sql.Append(" ORDER BY ").Append(orderClause);

            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                DataTable dt = new DataTable();
                dt.Load(idr);
                idr.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    MOrderLine ol = new MOrderLine(GetCtx(), dr, Get_TrxName());
                    ol.SetHeaderInfo(this);
                    list.Add(ol);
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql.ToString(), e);
            }
            finally
            {
                if (idr != null)
                {

                    idr.Close();
                    idr = null;
                }
            }
            //
            MOrderLine[] lines = new MOrderLine[list.Count]; ;
            lines = list.ToArray();
            return lines;
        }

        /// <summary>
        /// Get Lines of Order
        /// </summary>
        /// <param name="orderBy">optional order by column</param>
        /// <returns>lines</returns>
        public MOrderLine[] GetLines(String orderBy)
        {
            String orderClause = "ORDER BY ";
            if ((orderBy != null) && (orderBy.Length > 0))
            {
                orderClause += orderBy;
            }
            else
            {
                orderClause += "Line";
            }
            return GetLines(null, orderClause);
        }

        /// <summary>
        /// Is Used to get all orderline except those where (Product is of ITEM type)
        /// </summary>
        /// <returns>lines</returns>
        /// <writer>Amit</writer>
        public MOrderLine[] GetLinesOtherthanProduct()
        {
            List<MOrderLine> list = new List<MOrderLine>();
            StringBuilder sql = new StringBuilder(@"SELECT * FROM C_OrderLine ol
                                                        LEFT JOIN m_product p ON p.m_product_id = ol.m_product_id
                                                        WHERE ol.C_Order_ID =" + GetC_Order_ID() + @" AND ol.isactive = 'Y' 
                                                        AND (ol.M_Product_ID IS NULL OR p.ProductType     != 'I')");
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                DataTable dt = new DataTable();
                dt.Load(idr);
                idr.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    MOrderLine ol = new MOrderLine(GetCtx(), dr, Get_TrxName());
                    ol.SetHeaderInfo(this);
                    list.Add(ol);
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql.ToString(), e);
            }
            finally
            {
                if (idr != null)
                {

                    idr.Close();
                    idr = null;
                }
            }
            //
            MOrderLine[] lines = new MOrderLine[list.Count]; ;
            lines = list.ToArray();
            return lines;
        }

        /*	Renumber Lines
        *	@param step start and step
        */
        public void RenumberLines(int step)
        {
            int number = step;
            MOrderLine[] lines = GetLines(true, null);	//	Line is default
            for (int i = 0; i < lines.Length; i++)
            {
                MOrderLine line = lines[i];
                line.SetLine(number);
                line.Save(Get_TrxName());
                number += step;
            }
            _lines = null;
        }

        /* 	Does the Order Line belong to this Order
         *	@param C_OrderLine_ID line
         *	@return true if part of the order
         */
        public bool IsOrderLine(int C_OrderLine_ID)
        {
            if (_lines == null)
                GetLines();
            for (int i = 0; i < _lines.Length; i++)
                if (_lines[i].GetC_OrderLine_ID() == C_OrderLine_ID)
                    return true;
            return false;
        }

        /* 	Get Taxes of Order
         *	@param requery requery
         *	@return array of taxes
         */
        public MOrderTax[] GetTaxes(bool requery)
        {
            if (_taxes != null && !requery)
                return _taxes;
            //
            List<MOrderTax> list = new List<MOrderTax>();
            String sql = "SELECT * FROM C_OrderTax WHERE C_Order_ID=" + GetC_Order_ID();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);

                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MOrderTax(GetCtx(), dr, Get_TrxName()));
                }
                dt = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                dt = null;
            }
            _taxes = new MOrderTax[list.Count];
            _taxes = list.ToArray();
            return _taxes;
        }

        /*	Get Invoices of Order
        * 	@param hearderLinkOnly shipments based on header only
        * 	@return invoices
        */
        public MInvoice[] GetInvoices(bool hearderLinkOnly)
        {
            //	TODO get invoiced which are linked on line level
            List<MInvoice> list = new List<MInvoice>();
            String sql = "SELECT * FROM C_Invoice WHERE C_Order_ID=" + GetC_Order_ID() + " ORDER BY Created DESC";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MInvoice(GetCtx(), dr, Get_TrxName()));
                }
                dt = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                dt = null;
            }

            MInvoice[] retValue = new MInvoice[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /*	Get latest Invoice of Order
        * 	@return invoice id or 0
        */
        public int GetC_Invoice_ID()
        {
            int C_Invoice_ID = 0;
            String sql = "SELECT C_Invoice_ID FROM C_Invoice "
                + "WHERE C_Order_ID=" + GetC_Order_ID() + " AND DocStatus IN ('CO','CL') "
                + "ORDER BY Created DESC";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    //C_Invoice_ID =Convert.ToInt32(dr[0]);
                    C_Invoice_ID = Utility.Util.GetValueOfInt(dr[0].ToString());
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "getC_Invoice_ID", e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                dt = null;
            }
            return C_Invoice_ID;
        }

        /* 	Get Shipments of Order
         * 	@param hearderLinkOnly shipments based on header only
         * 	@return shipments
         */
        public MInOut[] GetShipments(bool hearderLinkOnly)
        {
            //	TODO: getShipment if linked on line
            List<MInOut> list = new List<MInOut>();
            String sql = "SELECT * FROM M_InOut WHERE C_Order_ID=" + GetC_Order_ID() + " ORDER BY Created DESC";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MInOut(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                dt = null;
            }

            MInOut[] retValue = new MInOut[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /*	Get RMAs of Order
        * 	@return RMAs
        */
        public MOrder[] GetRMAs()
        {
            List<MOrder> list = new List<MOrder>();
            String sql = "SELECT * FROM C_Order WHERE Orig_Order_ID=" + GetC_Order_ID() + " ORDER BY Created DESC";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MOrder(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                dt = null;
            }


            MOrder[] retValue = new MOrder[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /*	Get Shipment Lines of Order
        * 	@return shipments newest first
        */
        public MInOutLine[] GetShipmentLines()
        {
            List<MInOutLine> list = new List<MInOutLine>();
            String sql = "SELECT * FROM M_InOutLine iol "
                + "WHERE iol.C_OrderLine_ID IN "
                    + "(SELECT C_OrderLine_ID FROM C_OrderLine WHERE C_Order_ID=@C_Order_ID) "
                + "ORDER BY M_InOutLine_ID";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@C_Order_ID", GetC_Order_ID());
                idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MInOutLine(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch(Exception e)
            {

                log.Log(Level.SEVERE, "MOrder" + sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                dt = null;
            }

            MInOutLine[] retValue = new MInOutLine[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /*	Get ISO Code of Currency
        *	@return Currency ISO
        */
        public String GetCurrencyISO()
        {
            return MCurrency.GetISO_Code(GetCtx(), GetC_Currency_ID());
        }

        /// <summary>
        /// Get Currency Precision
        /// </summary>
        /// <returns>precision</returns>
        public int GetPrecision()
        {
            return MCurrency.GetStdPrecision(GetCtx(), GetC_Currency_ID());
        }

        /*	Get Document Status
        *	@return Document Status Clear Text
        */
        public String GetDocStatusName()
        {
            return MRefList.GetListName(GetCtx(), 131, GetDocStatus());
        }

        /// <summary>
        /// Set DocAction
        /// </summary>
        /// <param name="docAction">doc action</param>
        public new void SetDocAction(String docAction)
        {
            SetDocAction(docAction, false);
        }

        /// <summary>
        /// Set DocAction
        /// </summary>
        /// <param name="docAction">doc action</param>
        /// <param name="forceCreation">force creation</param>
        public void SetDocAction(String docAction, bool forceCreation)
        {
            base.SetDocAction(docAction);
            _forceCreation = forceCreation;
        }

        /*	Set Processed.
        * 	Propergate to Lines/Taxes
        *	@param processed processed
        */
        public new void SetProcessed(bool processed)
        {
            base.SetProcessed(processed);
            if (Get_ID() == 0)
                return;
            String set = "SET Processed='"
                + (processed ? "Y" : "N")
                + "' WHERE C_Order_ID=" + GetC_Order_ID();
            int noLine = DataBase.DB.ExecuteQuery("UPDATE C_OrderLine " + set, null, Get_TrxName());
            int noTax = DataBase.DB.ExecuteQuery("UPDATE C_OrderTax " + set, null, Get_TrxName());
            _lines = null;
            _taxes = null;
            log.Fine(processed + " - Lines=" + noLine + ", Tax=" + noTax);
        }

        /* 	Before Save
        *	@param newRecord new
        *	@return save
        */
        protected override bool BeforeSave(bool newRecord)
        {
            MBPartner bp = null;
            try
            {
                //	Client/Org Check
                if (GetAD_Org_ID() == 0)
                {
                    int context_AD_Org_ID = GetCtx().GetAD_Org_ID();
                    if (context_AD_Org_ID != 0)
                    {
                        SetAD_Org_ID(context_AD_Org_ID);
                        log.Warning("Changed Org to Ctx=" + context_AD_Org_ID);
                    }
                }
                if (GetAD_Client_ID() == 0)
                {
                    _processMsg = "AD_Client_ID = 0";
                    return false;
                }

                //	New Record Doc Type - make sure DocType set to 0
                if (newRecord && GetC_DocType_ID() == 0)
                    SetC_DocType_ID(0);

                //	Default Warehouse
                if (GetM_Warehouse_ID() == 0)
                {
                    MOrg org = MOrg.Get(GetCtx(), GetAD_Org_ID());
                    SetM_Warehouse_ID(org.GetM_Warehouse_ID());
                }

                //	Warehouse Org
                MWarehouse wh = null;
                if (newRecord
                    || Is_ValueChanged("AD_Org_ID") || Is_ValueChanged("M_Warehouse_ID"))
                {
                    wh = MWarehouse.Get(GetCtx(), GetM_Warehouse_ID());
                    if (wh.GetAD_Org_ID() != GetAD_Org_ID())
                    {
                        //Arpit 20th Nov,2017 issue No.115 -Not to save record if WareHouse is conflictiong with Organization
                        log.SaveWarning("WarehouseOrgConflict", "");
                        return false;
                    }
                }

                // JID_1366 : in case of disallow : True on warhouse, and when user try to save SO with shipping Rule as Force,
                // then system will save record with Availability as shipping Rule and give message to user.
                if (GetDeliveryRule() == X_C_Order.DELIVERYRULE_Force)
                {
                    if (wh == null)
                    {
                        wh = MWarehouse.Get(GetCtx(), GetM_Warehouse_ID());
                    }
                    if (wh.IsDisallowNegativeInv())
                    {
                        SetDeliveryRule(X_C_Order.DELIVERYRULE_Availability);
                        log.Info("JID_1366 : in case of disallow : True on warhouse, and user try to save Order with shipping Rule as Force, then system will save record with Availability.");
                    }
                }

                //	Reservations in Warehouse
                if (!newRecord && Is_ValueChanged("M_Warehouse_ID"))
                {
                    //MOrderLine[] lines = GetLines(false, null);
                    //for (int i = 0; i < lines.Length; i++)
                    //{
                    //    if (!lines[i].CanChangeWarehouse())		// saves Error	
                    //        return false;
                    //}

                    string sql = "SELECT COUNT(C_OrderLine_ID) AS Count, QtyDelivered, QtyInvoiced, QtyReserved FROM C_OrderLine WHERE C_Order_ID = " + GetC_Order_ID()
                        + " AND IsActive = 'Y' AND (QtyDelivered != 0 OR QtyInvoiced != 0 OR QtyReserved != 0) GROUP BY QtyDelivered, QtyInvoiced, QtyReserved, Line ORDER BY Line";
                    DataSet ds = DB.ExecuteDataset(sql, null, Get_Trx());
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        if (Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["QtyDelivered"]) != 0)
                        {
                            log.SaveError("Error", Msg.Translate(GetCtx(), "QtyDelivered") + "=" + Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["QtyDelivered"]));
                            return false;
                        }
                        if (Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["QtyInvoiced"]) != 0)
                        {
                            log.SaveError("Error", Msg.Translate(GetCtx(), "QtyInvoiced") + "=" + Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["QtyInvoiced"]));
                            return false;
                        }
                        if (Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["QtyReserved"]) != 0)
                        {
                            log.SaveError("Error", Msg.Translate(GetCtx(), "QtyReserved") + "=" + Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["QtyReserved"]));
                            return false;
                        }
                    }
                }

                // If lines are available and user is changing the pricelist, Order or Ship/Receipt on header than we have to restrict it because
                // JID_0399_1: After change the receipt or order system will give the error message
                if (!newRecord && (Is_ValueChanged("M_PriceList_ID") || Is_ValueChanged("Orig_Order_ID") || Is_ValueChanged("Orig_InOut_ID")))
                {
                    //MOrderLine[] lines = GetLines(false, null);
                    //if (lines.Length > 0)
                    //{
                    //    log.SaveWarning("pleaseDeleteLinesFirst", "");
                    //    return false;
                    //}

                    string sql = "SELECT COUNT(C_OrderLine_ID) FROM C_OrderLine WHERE C_Order_ID = " + GetC_Order_ID() + " AND IsActive = 'Y'";
                    if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) > 0)
                    {
                        log.SaveWarning("pleaseDeleteLinesFirst", "");
                        return false;
                    }
                }
                //End
                if (!newRecord && Is_ValueChanged("C_DocTypeTarget_ID"))
                {
                    //1052--if doctype is changed from release order to another then blanket order line reference
                    //should not be preset at order line if present order should not be updated  
                    string sql = "SELECT COUNT(C_OrderLine_ID) FROM C_OrderLine WHERE C_Order_ID = " + GetC_Order_ID() + " AND IsActive = 'Y' " +
                    "AND C_OrderLine_Blanket_ID IS NOT NULL ";
                    if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) > 0)
                    {
                        log.SaveWarning("pleaseDeleteLinesFirst", "");
                        return false;
                    }
                }
                //	No Partner Info - set Template
                if (GetC_BPartner_ID() == 0)
                    SetBPartner(MBPartner.GetTemplate(GetCtx(), GetAD_Client_ID()));
                if (GetC_BPartner_Location_ID() == 0)
                    SetBPartner(MBPartner.Get(GetCtx(), GetC_BPartner_ID()));
                //	No Bill - get from Ship
                if (GetBill_BPartner_ID() == 0)
                {
                    SetBill_BPartner_ID(GetC_BPartner_ID());
                    SetBill_Location_ID(GetC_BPartner_Location_ID());
                }
                if (GetBill_Location_ID() == 0)
                    SetBill_Location_ID(GetC_BPartner_Location_ID());

                //	BP Active check
                if (newRecord || Is_ValueChanged("C_BPartner_ID"))
                {
                    bp = MBPartner.Get(GetCtx(), GetC_BPartner_ID());
                    if (!bp.IsActive())
                    {
                        log.SaveError("NotActive", Msg.GetMsg(GetCtx(), "C_BPartner_ID"));
                        return false;
                    }
                }
                if ((newRecord || Is_ValueChanged("Bill_BPartner_ID"))
                        && GetBill_BPartner_ID() != GetC_BPartner_ID())
                {
                    bp = MBPartner.Get(GetCtx(), GetBill_BPartner_ID());
                    if (!bp.IsActive())
                    {
                        log.SaveError("NotActive", Msg.GetMsg(GetCtx(), "Bill_BPartner_ID"));
                        return false;
                    }
                }

                //	Default Price List
                if (GetM_PriceList_ID() == 0)
                {
                    string test = IsSOTrx() ? "Y" : "N";
                    int ii = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar("SELECT M_PriceList_ID FROM M_PriceList "
                        + "WHERE AD_Client_ID=" + GetAD_Client_ID() + " AND IsSOPriceList='" + test
                        + "' ORDER BY IsDefault DESC", null, null));
                    if (ii != 0)
                        SetM_PriceList_ID(ii);
                }
                //	Default Currency
                if (GetC_Currency_ID() == 0)
                {
                    String sql = "SELECT C_Currency_ID FROM M_PriceList WHERE M_PriceList_ID=" + GetM_PriceList_ID();
                    int ii = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql, null, null));
                    if (ii != 0)
                        SetC_Currency_ID(ii);
                    else
                        SetC_Currency_ID(GetCtx().GetContextAsInt("#C_Currency_ID"));
                }

                //	Default Sales Rep
                if (GetSalesRep_ID() == 0)
                {
                    int ii = GetCtx().GetContextAsInt("#SalesRep_ID");
                    if (ii != 0)
                        SetSalesRep_ID(ii);
                }

                //	Default Document Type
                if (GetC_DocTypeTarget_ID() == 0)
                    SetC_DocTypeTarget_ID(DocSubTypeSO_Standard);

                //	Default Payment Term
                if (GetC_PaymentTerm_ID() == 0)
                {
                    int ii = GetCtx().GetContextAsInt("#C_PaymentTerm_ID");
                    if (ii != 0)
                        SetC_PaymentTerm_ID(ii);
                    else
                    {
                        String sql = "SELECT C_PaymentTerm_ID FROM C_PaymentTerm WHERE AD_Client_ID=" + GetAD_Client_ID() + " AND IsDefault='Y'";
                        ii = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql, null, null));
                        if (ii != 0)
                            SetC_PaymentTerm_ID(ii);
                    }
                }

                //Do not save if "valid to" date is less than "valid from" date in case of blanket order.  By SUkhwinder on 31/07/2017
                MDocType dt = MDocType.Get(GetCtx(), GetC_DocTypeTarget_ID());
                if (dt.GetDocBaseType() == "BOO") ///dt.GetValue() == "BSO" || dt.GetValue() == "BPO")
                {
                    if (GetOrderValidFrom() != null && GetOrderValidTo() != null)
                    {
                        if (GetOrderValidFrom().Value.Date > GetOrderValidTo().Value.Date)
                        {
                            log.SaveError(Msg.Translate(GetCtx(), "VIS_ValidFromDateGrtrThanValidToDate"), "");
                            return false;
                        }
                    }
                }

                //JID_0211: System is allowing to save promised date smaller than order date on header as wll as on order lines.. There should be a validation.
                if (!IsReturnTrx() && GetDateOrdered() != null && GetDatePromised() != null)
                {
                    if (GetDateOrdered().Value.Date > GetDatePromised().Value.Date)
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "VIS_OrderDateGrtrThanPromisedDate"));
                        return false;
                    }
                }

                //SI_0648_2 : payment rule and payment method should be same as on payment base type of payment method window
                if (Env.IsModuleInstalled("VA009_") && GetVA009_PaymentMethod_ID() > 0)
                {
                    string paymentRule = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT VA009_PAYMENTBASETYPE FROM VA009_PAYMENTMETHOD 
                                                                                   WHERE VA009_PAYMENTMETHOD_ID=" + GetVA009_PaymentMethod_ID(), null, Get_Trx()));
                    //if Docbase is Sales Order and Sub Type SO Is POS Order then check the payment method must not be letter of credit and check.
                    if ((dt.GetDocBaseType().Equals("SOO")) && (dt.GetDocSubTypeSO().Equals(DocSubTypeSO_POS)))
                    {
                        if (paymentRule.Equals("L") || paymentRule.Equals("S")) // "L"-- Letter of credit, "S"-- Check
                        {
                            _processMsg = Msg.GetMsg(GetCtx(), "VIS_PleaseChangePaymentMethod");
                            log.SaveError("VIS_PleaseChangePaymentMethod", "");
                            return false;
                        }
                    }

                    if (!String.IsNullOrEmpty(paymentRule))
                    {
                        SetPaymentMethod(paymentRule);
                        SetPaymentRule(paymentRule);
                    }
                }

                //	Default Conversion Type
                if (GetC_ConversionType_ID() == 0)
                    SetC_ConversionType_ID(MConversionType.GetDefault(GetAD_Client_ID()));

                //Added by Bharat for Credit Limit on 24/08/2016
                //if (IsSOTrx())
                //{
                //    MBPartner bp = MBPartner.Get(GetCtx(), GetC_BPartner_ID());
                //    if (bp.GetCreditStatusSettingOn() == "CH")
                //    {
                //        decimal creditLimit = bp.GetSO_CreditLimit();
                //        string creditVal = bp.GetCreditValidation();
                //        if (creditLimit != 0)
                //        {
                //            decimal creditAvlb = creditLimit - bp.GetSO_CreditUsed();
                //            if (creditAvlb <= 0)
                //            {
                //                //if (creditVal == "A" || creditVal == "D" || creditVal == "E")
                //                //{
                //                //    log.SaveError("Error", Msg.GetMsg(GetCtx(), "CreditUsedSalesOrder"));
                //                //    return false;
                //                //}
                //                //else if (creditVal == "G" || creditVal == "J" || creditVal == "K")
                //                //{
                //                    log.SaveWarning("Warning", Msg.GetMsg(GetCtx(), "CreditOver"));
                //                //}
                //            }
                //        }
                //    }
                //    // JID_0161 // change here now will check credit settings on field only on Business Partner Header // Lokesh Chauhan 15 July 2019
                //    else if (bp.GetCreditStatusSettingOn() == X_C_BPartner.CREDITSTATUSSETTINGON_CustomerLocation)
                //    {
                //        MBPartnerLocation bpl = new MBPartnerLocation(GetCtx(), GetC_BPartner_Location_ID(), null);
                //        //if (bpl.GetCreditStatusSettingOn() == "CL")
                //        //{
                //            decimal creditLimit = bpl.GetSO_CreditLimit();
                //            string creditVal = bpl.GetCreditValidation();
                //            if (creditLimit != 0)
                //            {
                //                decimal creditAvlb = creditLimit - bpl.GetSO_CreditUsed();
                //                if (creditAvlb <= 0)
                //                {
                //                    //if (creditVal == "A" || creditVal == "D" || creditVal == "E")
                //                    //{
                //                    //    log.SaveError("Error", Msg.GetMsg(GetCtx(), "CreditUsedSalesOrder"));
                //                    //    return false;
                //                    //}
                //                    //else if (creditVal == "G" || creditVal == "J" || creditVal == "K")
                //                    //{
                //                        //log.Warning(Msg.GetMsg(GetCtx(), "CreditOver"));
                //                        log.SaveWarning("Warning", Msg.GetMsg(GetCtx(), "CreditOver"));
                //                    //}
                //                }
                //            }
                //        //}
                //    }
                //}

                if (IsReturnTrx())
                {
                    bool withinPolicy = true;

                    if (GetM_ReturnPolicy_ID() == 0)
                        SetM_ReturnPolicy_ID();

                    if (GetM_ReturnPolicy_ID() != 0)
                    {
                        DateTime? movementdate = Util.GetValueOfDateTime(DB.ExecuteScalar("SELECT MovementDate FROM M_InOut WHERE " +
                                                                                        "M_InOut_ID=" + GetOrig_InOut_ID()));
                        // MInOut origInOut = new MInOut(GetCtx(), GetOrig_InOut_ID(), null);
                        MReturnPolicy rpolicy = new MReturnPolicy(GetCtx(), GetM_ReturnPolicy_ID(), null);
                        log.Fine("RMA Date : " + GetDateOrdered() + " Shipment Date : " + movementdate);
                        withinPolicy = rpolicy.CheckReturnPolicy(movementdate, GetDateOrdered());
                    }
                    else
                        withinPolicy = false;

                    if (!withinPolicy)
                    {
                        if (!MRole.GetDefault(GetCtx()).IsOverrideReturnPolicy())
                        {
                            log.SaveError("Error", Msg.GetMsg(GetCtx(), "ReturnPolicyExceeded"));
                            return false;
                        }
                        else
                        {
                            log.SaveWarning("Warning", "ReturnPolicyExceeded");
                        }
                    }
                }
            }
            catch
            {
                //ShowMessage.Error("MOrder", null, "Before Save");
                return false;
            }
            return true;
        }

        /* 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return true if can be saved
         */
        protected override bool AfterSave(bool newRecord, bool success)
        {
            try
            {
                //if (!success || newRecord)
                if (!success)
                    return success;

                if (!newRecord)
                {
                    //	Propagate Description changes
                    if (Is_ValueChanged("Description") || Is_ValueChanged("POReference"))
                    {
                        String sql = "UPDATE C_Invoice i"
                            + " SET (Description,POReference)="
                                + "(SELECT Description, POReference "
                                + "FROM C_Order o WHERE i.C_Order_ID=o.C_Order_ID) "
                            + "WHERE DocStatus NOT IN ('RE','CL') AND C_Order_ID=" + GetC_Order_ID();

                        int no = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql, null, Get_TrxName()));
                        log.Fine("Description -> #" + no);
                    }

                    //	Propagate Changes of Payment Info to existing (not reversed/closed) invoices
                    if (Is_ValueChanged("PaymentRule") || Is_ValueChanged("C_PaymentTerm_ID")
                        || Is_ValueChanged("DateAcct") || Is_ValueChanged("C_Payment_ID")
                        || Is_ValueChanged("C_CashLine_ID"))
                    {
                        String sql = "UPDATE C_Invoice i "
                            + "SET (PaymentRule,C_PaymentTerm_ID,DateAcct,C_Payment_ID,C_CashLine_ID)="
                                + "(SELECT PaymentRule,C_PaymentTerm_ID,DateAcct,C_Payment_ID,C_CashLine_ID "
                                + "FROM C_Order o WHERE i.C_Order_ID=o.C_Order_ID)"
                            + "WHERE DocStatus NOT IN ('RE','CL') AND C_Order_ID=" + GetC_Order_ID();
                        //	Don't touch Closed/Reversed entries
                        int no = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql, null, Get_TrxName()));
                        log.Fine("Payment -> #" + no);
                    }

                    //	Sync Lines
                    AfterSaveSync("AD_Org_ID");
                    AfterSaveSync("C_BPartner_ID");
                    AfterSaveSync("C_BPartner_Location_ID");
                    AfterSaveSync("DateOrdered");
                    AfterSaveSync("DatePromised");
                    AfterSaveSync("M_Warehouse_ID");
                    AfterSaveSync("M_Shipper_ID");
                    AfterSaveSync("C_Currency_ID");
                }

                // Applied check for warning message on credit limit for Business Partner
                if ((IsSOTrx() && !IsReturnTrx()) || (!IsSOTrx() && IsReturnTrx()))
                {
                    string docSubType = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT DocSubtypeSO FROM C_DocType WHERE 
                                                C_DocType_ID = " + GetC_DocTypeTarget_ID() + " AND DocBaseType = 'SOO'", null, Get_TrxName()));
                    if (!(docSubType == "ON" || docSubType == "OB"))
                    {
                        Decimal grandTotal = MConversionRate.ConvertBase(GetCtx(),
                            GetGrandTotal(), GetC_Currency_ID(), GetDateOrdered(),
                            GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID());

                        MBPartner bp = new MBPartner(GetCtx(), GetC_BPartner_ID(), Get_Trx());
                        string retMsg = "";

                        bool crdAll = bp.IsCreditAllowed(GetC_BPartner_Location_ID(), grandTotal, out retMsg);
                        if (!crdAll)
                            log.SaveWarning("Warning", retMsg);
                        else if (bp.IsCreditWatch(GetC_BPartner_Location_ID()))
                        {
                            log.SaveWarning("Warning", Msg.GetMsg(GetCtx(), "VIS_BPCreditWatch"));
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                log.Severe(ex.ToString());
                //MessageBox.Show("Error in MOrder--AfterSave");
                return false;
            }
            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        private void AfterSaveSync(String columnName)
        {
            if (Is_ValueChanged(columnName))
            {
                String sql = "UPDATE C_OrderLine ol"
                    + " SET " + columnName + " ="
                        + "(SELECT " + columnName
                        + " FROM C_Order o WHERE ol.C_Order_ID=o.C_Order_ID) "
                    + "WHERE C_Order_ID=" + GetC_Order_ID();
                int no = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql, null, Get_TrxName()));
                log.Fine(columnName + " Lines -> #" + no);
            }
        }

        /// <summary>
        /// when doc type = Warehouse Order / Credit Order / POS Order / Prepay order --- and payment term is advance -- then system return false
        /// </summary>
        /// <param name="documnetType_Id"></param>
        /// <param name="PaymentTerm_Id"></param>
        /// <returns></returns>
        public bool checkAdvancePaymentTerm(int documnetType_Id, int PaymentTerm_Id)
        {
            bool isAdvancePayTerm = true;

            // when document type is not --  Warehouse Order / Credit Order / POS Order / Prepay order , then true
            // Payment term can't be advance for Customer RMA / Vendor RMA
            MDocType doctype = MDocType.Get(GetCtx(), documnetType_Id);
            if (!(doctype.GetDocSubTypeSO() == X_C_DocType.DOCSUBTYPESO_PrepayOrder ||
                doctype.GetDocSubTypeSO() == X_C_DocType.DOCSUBTYPESO_OnCreditOrder ||
                doctype.GetDocSubTypeSO() == X_C_DocType.DOCSUBTYPESO_WarehouseOrder ||
                doctype.GetDocSubTypeSO() == X_C_DocType.DOCSUBTYPESO_POSOrder ||
                (doctype.GetDocSubTypeSO() == X_C_DocType.DOCSUBTYPESO_StandardOrder && doctype.IsReturnTrx()) ||
                (doctype.GetDocBaseType() == "POO" && doctype.IsReturnTrx())))
            {
                isAdvancePayTerm = true;
            }
            // check payment term is Advance, then return False
            else if (Util.GetValueOfString(DB.ExecuteScalar(@"SELECT VA009_Advance FROM C_PaymentTerm
                                            WHERE C_PaymentTerm_ID = " + PaymentTerm_Id, null, Get_TrxName())).Equals("Y"))
            {
                isAdvancePayTerm = false;
            }
            // check any payment term schedule is Advance, then return False
            // JID_1193: If Payment term header is valid but having lines with advance and Inactive. System should consider that as 100% immedate. However, system is creating schedule of advance on order.
            else if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(C_PaySchedule_ID) FROM C_PaySchedule WHERE IsActive = 'Y' AND IsValid = 'Y' 
                                                            AND VA009_Advance = 'Y' AND C_PaymentTerm_ID = " + PaymentTerm_Id, null, Get_TrxName())) > 0)
            {
                isAdvancePayTerm = false;
            }

            return isAdvancePayTerm;
        }


        /* 	Before Delete
         *	@return true of it can be deleted
         */
        protected override bool BeforeDelete()
        {
            try
            {
                if (IsProcessed())
                    return false;

                GetLines();
                for (int i = 0; i < _lines.Length; i++)
                {
                    if (!_lines[i].DeleteCheck())
                    {
                        return false;
                    }
                }
            }
            catch
            {
                //ShowMessage.Error("MOrder", null, "BeforeDelete");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Process document
        /// </summary>
        /// <param name="processAction">document action</param>
        /// <returns>true if performed</returns>
        public bool ProcessIt(String processAction)
        {
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }

        /// <summary>
        /// Unlock Document.
        /// </summary>
        /// <returns>true if success</returns>
        public bool UnlockIt()
        {
            log.Info("unlockIt - " + ToString());
            SetProcessing(false);
            return true;
        }

        /// <summary>
        /// Invalidate Document
        /// </summary>
        /// <returns>true if success</returns>
        public bool InvalidateIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_Prepare);
            return true;
        }

        /// <summary>
        /// Prepare Document
        /// </summary>
        /// <returns>new status (In Progress or Invalid)</returns>
        public String PrepareIt()
        {
            log.Info(ToString());
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (_processMsg != null)
                return DocActionVariables.STATUS_INVALID;
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocTypeTarget_ID());
            SetIsReturnTrx(dt.IsReturnTrx());
            SetIsSOTrx(dt.IsSOTrx());

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), dt.GetDocBaseType(), GetAD_Org_ID()))
            {
                _processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }

            // is Non Business Day?
            // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateAcct(), GetAD_Org_ID()))
            {
                _processMsg = Common.Common.NONBUSINESSDAY;
                return DocActionVariables.STATUS_INVALID;
            }


            //	Lines
            MOrderLine[] lines = GetLines(true, "M_Product_ID");
            if (lines.Length == 0)
            {
                _processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }

            // In case of blanket transaction or sales quotation no need to  check curency conversion.
            if (!Util.GetValueOfBool(Get_Value("IsBlanketTrx")) && !IsSalesQuotation())
            {
                // GNZ_1027: if conversion not found system will give message Message: Could not convert currency to base currency - Conversion type: XXXX
                Decimal ordAmt = GetGrandTotal();
                // If Amount is ZERO then no need to check currency conversion
                if (!ordAmt.Equals(Env.ZERO))
                {
                    ordAmt = MConversionRate.ConvertBase(GetCtx(), GetGrandTotal(), //	CM adjusted 
                 GetC_Currency_ID(), GetDateOrdered(), GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID());

                    if (ordAmt == 0)
                    {
                        MConversionType conv = MConversionType.Get(GetCtx(), GetC_ConversionType_ID());
                        _processMsg = Msg.GetMsg(GetCtx(), "NoConversion") + MCurrency.GetISO_Code(GetCtx(), GetC_Currency_ID()) + Msg.GetMsg(GetCtx(), "ToBaseCurrency")
                            + MCurrency.GetISO_Code(GetCtx(), MClient.Get(GetCtx()).GetC_Currency_ID()) + " - " + Msg.GetMsg(GetCtx(), "ConversionType") + conv.GetName();

                        return DocActionVariables.STATUS_INVALID;
                    }
                }
            }

            //	Convert DocType to Target
            if (GetC_DocType_ID() != GetC_DocTypeTarget_ID())
            {
                //	Cannot change Std to anything else if different warehouses
                if (GetC_DocType_ID() != 0)
                {
                    MDocType dtOld = MDocType.Get(GetCtx(), GetC_DocType_ID());
                    if (MDocType.DOCSUBTYPESO_StandardOrder.Equals(dtOld.GetDocSubTypeSO())		//	From SO
                        && !MDocType.DOCSUBTYPESO_StandardOrder.Equals(dt.GetDocSubTypeSO()))	//	To !SO
                    {
                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (lines[i].GetM_Warehouse_ID() != GetM_Warehouse_ID())
                            {
                                log.Warning("different Warehouse " + lines[i]);
                                _processMsg = "@CannotChangeDocType@";
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }
                    }
                }

                //	New or in Progress/Invalid
                if (DOCSTATUS_Drafted.Equals(GetDocStatus())
                    || DOCSTATUS_InProgress.Equals(GetDocStatus())
                    || DOCSTATUS_Invalid.Equals(GetDocStatus())
                    || GetC_DocType_ID() == 0)
                {
                    SetC_DocType_ID(GetC_DocTypeTarget_ID());
                }
                else	//	convert only if offer
                {
                    if (dt.IsOffer())
                        SetC_DocType_ID(GetC_DocTypeTarget_ID());
                    else
                    {
                        _processMsg = "@CannotChangeDocType@";
                        return DocActionVariables.STATUS_INVALID;
                    }
                }
            }	//	convert DocType

            //	Mandatory Product Attribute Set Instance
            String mandatoryType = "='Y'";	//	IN ('Y','S')
            String sql = "SELECT COUNT(C_OrderLine_ID) "
                + "FROM C_OrderLine ol"
                + " INNER JOIN M_Product p ON (ol.M_Product_ID=p.M_Product_ID)"
                + " INNER JOIN M_AttributeSet pas ON (p.M_AttributeSet_ID=pas.M_AttributeSet_ID) "
                + "WHERE pas.MandatoryType" + mandatoryType
                + " AND ol.M_AttributeSetInstance_ID IS NULL"
                + " AND ol.C_Order_ID=" + GetC_Order_ID();
            int no = DataBase.DB.GetSQLValue(Get_TrxName(), sql);
            if (no != 0)
            {
                _processMsg = "@LinesWithoutProductAttribute@ (" + no + ")";
                return DocActionVariables.STATUS_INVALID;
            }

            // stop completing of RMA when original order and shipment/receipt are not completed or closed            
            if (IsReturnTrx())   // Added by Vivek on 20/01/2018 assigned by Mukesh sir
            {
                MOrder OrigOrder = new MOrder(GetCtx(), GetOrig_Order_ID(), Get_Trx());
                MInOut OrigInout = new MInOut(GetCtx(), GetOrig_InOut_ID(), Get_Trx());
                if (OrigInout.GetDocStatus() == "RE" || OrigInout.GetDocStatus() == "VO")
                {
                    _processMsg = Msg.GetMsg(GetCtx(), "Order/ShipmentNotCompleted");
                    return DocActionVariables.STATUS_INVALID;
                }
                if (OrigOrder.GetDocStatus() == "RE" || OrigOrder.GetDocStatus() == "VO")
                {
                    _processMsg = Msg.GetMsg(GetCtx(), "Order/ShipmentNotCompleted");
                    return DocActionVariables.STATUS_INVALID;
                }

            }




            //	Lines
            if (ExplodeBOM())
                lines = GetLines(true, "M_Product_ID");

            //check Payment term is valid or Not (SI_0018)
            if (Util.GetValueOfString(DB.ExecuteScalar("SELECT IsValid FROM C_PaymentTerm WHERE C_PaymentTerm_ID = " + GetC_PaymentTerm_ID())) == "N")
            {
                _processMsg = Msg.GetMsg(GetCtx(), "VIS_PaymentTermIsInValid");
                return DocActionVariables.STATUS_INVALID;
            }

            // SI_0646_1 : when doc type = Warehouse Order / Credit Order / POS Order / Prepay order --- and payment term is advance -- then system return false
            if (Env.IsModuleInstalled("VA009_") && !checkAdvancePaymentTerm(GetC_DocTypeTarget_ID(), GetC_PaymentTerm_ID()))
            {
                _processMsg = Msg.GetMsg(GetCtx(), "VIS_NotToBeAdvance");
                return DocActionVariables.STATUS_INVALID;
            }

            if (!ReserveStock(dt, lines))
            {
                _processMsg = "Cannot reserve Stock";
                return DocActionVariables.STATUS_INVALID;
            }

            if (!CalculateTaxTotal())
            {
                _processMsg = "Error calculating tax";
                return DocActionVariables.STATUS_INVALID;
            }

            // Changed check to handle Vendor RMA Cases also
            if ((IsSOTrx() && !IsReturnTrx()) || (!IsSOTrx() && IsReturnTrx()))
            {
                // added by Bharat to avoid completion if Payment Method is not selected
                // Tuple<String, String, String> aInfo = null;
                if (Env.IsModuleInstalled("VA009_") && GetVA009_PaymentMethod_ID() == 0 && !Util.GetValueOfBool(Get_Value("IsSalesQuotation")))
                {
                    _processMsg = "@MandatoryPaymentMethod@";
                    return DocActionVariables.STATUS_INVALID;
                }

                if (Env.IsModuleInstalled("VAPOS_") && GetVAPOS_POSTerminal_ID() > 0)
                {
                    if (Util.GetValueOfDecimal(GetVAPOS_CreditAmt()) > 0)
                    {
                        MBPartner bp = MBPartner.Get(GetCtx(), GetC_BPartner_ID());
                        if (MBPartner.SOCREDITSTATUS_CreditStop.Equals(bp.GetSOCreditStatus()))
                        {
                            _processMsg = "@BPartnerCreditStop@ - @TotalOpenBalance@="
                                + bp.GetTotalOpenBalance()
                                + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
                            return DocActionVariables.STATUS_INVALID;
                        }
                        if (MBPartner.SOCREDITSTATUS_CreditHold.Equals(bp.GetSOCreditStatus()))
                        {
                            _processMsg = "@BPartnerCreditHold@ - @TotalOpenBalance@="
                                + bp.GetTotalOpenBalance()
                                + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
                            return DocActionVariables.STATUS_INVALID;
                        }
                        Decimal grandTotal = MConversionRate.ConvertBase(GetCtx(),
                            GetVAPOS_CreditAmt(), GetC_Currency_ID(), GetDateOrdered(),
                            GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID());

                        if (MBPartner.SOCREDITSTATUS_CreditHold.Equals(bp.GetSOCreditStatus(grandTotal)))
                        {
                            _processMsg = "@BPartnerOverOCreditHold@ - @TotalOpenBalance@="
                                + bp.GetTotalOpenBalance() + ", @GrandTotal@=" + grandTotal
                                + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                }
                else
                {
                    string docSubType = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT DocSubtypeSO FROM C_DocType WHERE 
                                                C_DocType_ID = " + GetC_DocTypeTarget_ID() + " AND DocBaseType = 'SOO'", null, Get_TrxName()));
                    if (!(docSubType == "ON" || docSubType == "OB"))
                    {
                        MBPartner bp = MBPartner.Get(GetCtx(), GetC_BPartner_ID());
                        //if (MBPartner.SOCREDITSTATUS_CreditStop.Equals(bp.GetSOCreditStatus()))
                        //{
                        //    _processMsg = "@BPartnerCreditStop@ - @TotalOpenBalance@="
                        //        + bp.GetTotalOpenBalance()
                        //        + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
                        //    return DocActionVariables.STATUS_INVALID;
                        //}
                        //if (MBPartner.SOCREDITSTATUS_CreditHold.Equals(bp.GetSOCreditStatus()))
                        //{
                        //    _processMsg = "@BPartnerCreditHold@ - @TotalOpenBalance@="
                        //        + bp.GetTotalOpenBalance()
                        //        + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
                        //    return DocActionVariables.STATUS_INVALID;
                        //}
                        Decimal grandTotal = MConversionRate.ConvertBase(GetCtx(),
                           GetGrandTotal(), GetC_Currency_ID(), GetDateOrdered(),
                            GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID());
                        //if (MBPartner.SOCREDITSTATUS_CreditHold.Equals(bp.GetSOCreditStatus(grandTotal)))
                        //{
                        //    _processMsg = "@BPartnerOverOCreditHold@ - @TotalOpenBalance@="
                        //        + bp.GetTotalOpenBalance() + ", @GrandTotal@=" + grandTotal
                        //        + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
                        //    return DocActionVariables.STATUS_INVALID;
                        //}

                        string retMsg = "";
                        bool crdAll = true;
                        //written by sandeep

                        if (Env.IsModuleInstalled("VA077_"))
                        {
                            // Skip Credit Check validation in case of Advance Order
                            if (Util.GetValueOfString(DB.ExecuteScalar(@"SELECT VA009_Advance FROM C_PaymentTerm
                                            WHERE C_PaymentTerm_ID = " + GetC_PaymentTerm_ID(), null, Get_TrxName())).Equals("N"))
                            {
                                DateTime validate = new DateTime();
                                string CreditStatusSettingOn = bp.GetCreditStatusSettingOn();
                                MBPartnerLocation bpl = new MBPartnerLocation(GetCtx(), GetC_BPartner_Location_ID(), null);
                                if (CreditStatusSettingOn.Contains("CL"))
                                {
                                    validate = Util.GetValueOfDateTime(bpl.Get_Value("VA077_ValidityDate")).Value;
                                }
                                else
                                {
                                    validate = Util.GetValueOfDateTime(bp.Get_Value("VA077_ValidityDate")).Value;
                                }

                                if (bp.Get_Value("VA077_ValidityDate") != null && validate.Date < DateTime.Now.Date)
                                {
                                    int RecCount = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(C_Invoice_ID) FROM C_Invoice WHERE IsSOTrx='Y' AND IsReturnTrx='N' AND C_BPartner_ID =" + GetC_BPartner_ID() + " and DocStatus in('CO','CL') and DateInvoiced BETWEEN " + GlobalVariable.TO_DATE(DateTime.Now.Date.AddDays(-730), true) + " AND " + GlobalVariable.TO_DATE(DateTime.Now.Date, true) + ""));

                                    if (RecCount > 0)
                                    {
                                        crdAll = bp.IsCreditAllowed(GetC_BPartner_Location_ID(), grandTotal, out retMsg);
                                    }
                                    else
                                    {
                                        _processMsg = Msg.GetMsg(GetCtx(), "VA077_CrChkExpired");
                                        return DocActionVariables.STATUS_INVALID;
                                    }
                                }
                                else
                                {
                                    crdAll = bp.IsCreditAllowed(GetC_BPartner_Location_ID(), grandTotal, out retMsg);
                                    if (!crdAll)
                                    {
                                        _processMsg = Msg.GetMsg(GetCtx(), "VA077_CrChkExpired");
                                        return DocActionVariables.STATUS_INVALID;
                                    }
                                }
                            }
                        }
                        else
                        {
                            crdAll = bp.IsCreditAllowed(GetC_BPartner_Location_ID(), grandTotal, out retMsg);
                        }
                        if (!crdAll)
                        {
                            if (bp.ValidateCreditValidation("A,D,E", GetC_BPartner_Location_ID()))
                            {
                                //to set credit fail and credit fail notice true in case of sales order
                                if ((IsSOTrx() && !IsReturnTrx() && !IsSalesQuotation() && !Util.GetValueOfBool(Get_Value("IsBlanketTrx"))))
                                {
                                    if ((Get_ColumnIndex("IsCreditFail") >= 0) && (Get_ColumnIndex("IsCreditFailNotice") >= 0))
                                    {
                                        /* rolback transaction because we ned to set credit fail checkbox true if credit not alowded if we set status invalid 
                                        then it wil rollback the update query as wel that's why we rollback before update query */
                                        if (Get_Trx() != null)
                                            Get_Trx().Rollback();
                                        int res = DB.ExecuteQuery(" UPDATE C_Order SET IsCreditFail='Y', IsCreditFailNotice='Y' WHERE C_Order_ID=" + GetC_Order_ID(), null, Get_Trx());
                                        if (res <= 0)
                                        {
                                            log.Info("Credit fail or notice  checkbox not updated ");
                                        }
                                        else
                                            Get_Trx().Commit();
                                    }
                                }
                                _processMsg = retMsg;
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }
                        else
                        {
                            //to set credit fail and credit fail notice false in case of sales order
                            if ((IsSOTrx() && !IsReturnTrx() && !IsSalesQuotation() && !Util.GetValueOfBool(Get_Value("IsBlanketTrx"))))
                            {
                                if ((Get_ColumnIndex("IsCreditFail") >= 0) && (Get_ColumnIndex("IsCreditFailNotice") >= 0))
                                {

                                    int res = Util.GetValueOfInt(DB.ExecuteQuery(" UPDATE C_Order SET IsCreditFail='N', IsCreditFailNotice='N' WHERE C_Order_ID=" + GetC_Order_ID(), null, Get_Trx()));
                                    if (res <= 0)
                                    {
                                        log.Info("Credit fail or notice  checkbox not updated ");
                                    }
                                }
                            }
                        }
                    }
                }
            }

            _justPrepared = true;
            // dont uncomment
            //if (!DOCACTION_Complete.Equals(getDocAction()))		don't set for just prepare 
            //		setDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /* 	Explode non stocked BOM.
         * 	@return true if bom exploded
         */
        private bool ExplodeBOM()
        {
            bool retValue = false;
            String where = "AND IsActive='Y' AND EXISTS "
                + "(SELECT * FROM M_Product p WHERE C_OrderLine.M_Product_ID=p.M_Product_ID"
                + " AND	p.IsBOM='Y' AND p.IsVerified='Y' AND p.IsStocked='N')";
            //
            String sql = "SELECT COUNT(C_OrderLine_ID) FROM C_OrderLine "
                + "WHERE C_Order_ID=" + GetC_Order_ID() + where;
            int count = DataBase.DB.GetSQLValue(Get_TrxName(), sql); //Convert.ToInt32(DataBase.DB.ExecuteScalar(sql, null, Get_TrxName()));

            StringBuilder sbSQL = new StringBuilder("");

            while (count != 0)
            {
                retValue = true;
                RenumberLines(1000);		//	max 999 bom items	

                //	Order Lines with non-stocked BOMs
                MOrderLine[] lines = GetLines(where, "ORDER BY Line");
                for (int i = 0; i < lines.Length; i++)
                {
                    MOrderLine line = lines[i];
                    MProduct product = MProduct.Get(GetCtx(), line.GetM_Product_ID());
                    log.Fine(product.GetName());
                    //	New Lines
                    int lineNo = line.GetLine();
                    MProductBOM[] boms = MProductBOM.GetBOMLines(product);
                    for (int j = 0; j < boms.Length; j++)
                    {
                        MProductBOM bom = boms[j];
                        MOrderLine newLine = new MOrderLine(this);
                        newLine.SetLine(++lineNo);
                        newLine.SetM_Product_ID(bom.GetProduct()
                            .GetM_Product_ID());
                        newLine.SetC_UOM_ID(bom.GetProduct().GetC_UOM_ID());
                        newLine.SetQty(Decimal.Multiply(line.GetQtyOrdered(), bom.GetBOMQty()));
                        if (bom.GetDescription() != null)
                            newLine.SetDescription(bom.GetDescription());
                        //
                        newLine.SetPrice();
                        newLine.Save(Get_TrxName());
                    }
                    //	Convert into Comment Line
                    //line.SetM_Product_ID(0);
                    //line.SetM_AttributeSetInstance_ID(0);
                    //line.SetPrice(Env.ZERO);
                    //line.SetPriceLimit(Env.ZERO);
                    //line.SetPriceList(Env.ZERO);
                    //line.SetLineNetAmt(Env.ZERO);
                    //line.SetFreightAmt(Env.ZERO);
                    //
                    String description = product.GetName();
                    if (product.GetDescription() != null)
                        description += " " + product.GetDescription();
                    if (line.GetDescription() != null)
                        description += " " + line.GetDescription();
                    //line.SetDescription(description);
                    //line.Save(Get_TrxName());

                    // change here to set product and other related information through query as in orderline before save you can not set both product or charge to ZERO
                    // Lokesh 10 july 2019
                    sbSQL.Clear();
                    sbSQL.Append(@"UPDATE C_OrderLine SET M_Product_ID = null, M_AttributeSetInstance_ID = null, PriceEntered = 0, PriceLimit = 0, PriceList = 0, LineNetAmt = 0, 
                                    FreightAmt = 0, Description = '" + description + "' WHERE C_OrderLine_ID = " + line.GetC_OrderLine_ID());
                    int res = Util.GetValueOfInt(DB.ExecuteQuery(sbSQL.ToString(), null, Get_TrxName()));
                    if (res <= 0)
                    {
                        log.Info("line not updated for BOM type product");
                        break;
                    }

                }	//	for all lines with BOM

                _lines = null;		//	force requery
                count = DataBase.DB.GetSQLValue(Get_TrxName(), sql, GetC_Invoice_ID());
                RenumberLines(10);
            }	//	while count != 0
            return retValue;
        }

        /* Reserve Inventory.
        * 	Counterpart: MInOut.completeIt()
        * 	@param dt document type or null
        * 	@param lines order lines (ordered by M_Product_ID for deadlock prevention)
        * 	@return true if (un) reserved
        */
        private bool ReserveStock(MDocType dt, MOrderLine[] lines)
        {
            try
            {
                if (dt == null)
                    dt = MDocType.Get(GetCtx(), GetC_DocType_ID());

                // Reserved quantity and ordered quantity should not be updated for returns
                if (dt.IsReturnTrx())
                    return true;

                //	Binding
                bool binding = !dt.IsProposal();
                //	Not binding - i.e. Target=0
                if (DOCACTION_Void.Equals(GetDocAction())
                //Closing Binding Quotation
                || (MDocType.DOCSUBTYPESO_Quotation.Equals(dt.GetDocSubTypeSO())
                    && DOCACTION_Close.Equals(GetDocAction())))
                    //Commented this check for get binding by Vivek on 27/09/2017
                    //|| IsDropShip())
                    //if (DOCACTION_Void.Equals(GetDocAction())
                    //    //	Closing Binding Quotation
                    //|| (MDocType.DOCSUBTYPESO_Quotation.Equals(dt.GetDocSubTypeSO())
                    //    && DOCACTION_Close.Equals(GetDocAction())) || IsDropShip())
                    binding = false;
                bool isSOTrx = IsSOTrx();
                log.Fine("Binding=" + binding + " - IsSOTrx=" + isSOTrx);
                //	Force same WH for all but SO/PO
                int header_M_Warehouse_ID = GetM_Warehouse_ID();
                if (MDocType.DOCSUBTYPESO_StandardOrder.Equals(dt.GetDocSubTypeSO())
                    || MDocBaseType.DOCBASETYPE_PURCHASEORDER.Equals(dt.GetDocBaseType()))
                    header_M_Warehouse_ID = 0;		//	don't enforce

                Decimal Volume = Env.ZERO;
                Decimal Weight = Env.ZERO;

                //	Always check and (un) Reserve Inventory		
                for (int i = 0; i < lines.Length; i++)
                {
                    MOrderLine line = lines[i];
                    int M_Locator_ID = 0;
                    MWarehouse wh = MWarehouse.Get(GetCtx(), line.GetM_Warehouse_ID());
                    //	Check/set WH/Org
                    if (header_M_Warehouse_ID != 0)	//	enforce WH
                    {
                        if (header_M_Warehouse_ID != line.GetM_Warehouse_ID())
                            line.SetM_Warehouse_ID(header_M_Warehouse_ID);
                        if (GetAD_Org_ID() != line.GetAD_Org_ID())
                            line.SetAD_Org_ID(GetAD_Org_ID());
                    }
                    //	Binding
                    Decimal target = binding ? line.GetQtyOrdered() : Env.ZERO;

                    Decimal difference = 0;

                    if (dt.GetDocBaseType() == "BOO")      //if (dt.GetValue() == "BSO")  IF is is BSO or BPO
                    {
                        difference = Decimal.Subtract(Decimal.Add(target, line.GetQtyReleased()), line.GetQtyReserved());
                    }
                    else
                    {
                        difference = Decimal.Subtract(Decimal.Subtract(target, line.GetQtyReserved()), line.GetQtyDelivered());
                    }



                    if (Env.Signum(difference) == 0)
                    {
                        MProduct product = line.GetProduct();
                        if (product != null)
                        {
                            Volume = Decimal.Add(Volume, (Decimal.Multiply((Decimal)product.GetVolume(), line.GetQtyOrdered())));
                            Weight = Decimal.Add(Weight, (Decimal.Multiply(product.GetWeight(), line.GetQtyOrdered())));
                        }

                        //JID_1686,JID_1687 only Items are updated in storage tab
                        if (product.IsStocked())
                        {
                            // Work done by Vivek on 13/11/2017 assigned by Mukesh sir
                            // Work done to update qtyordered at storage and qtyreserved at order line
                            // when document is processed in closing state
                            if (DOCACTION_Close.Equals(GetDocAction()))
                            {
                                Decimal ordered = isSOTrx ? Env.ZERO : line.GetQtyReserved();
                                Decimal reserved = isSOTrx ? line.GetQtyReserved() : Env.ZERO;
                                M_Locator_ID = wh.GetDefaultM_Locator_ID();
                                if (!MStorage.Add(GetCtx(), line.GetM_Warehouse_ID(), M_Locator_ID,
                                            line.GetM_Product_ID(),
                                            line.GetM_AttributeSetInstance_ID(), reserved,
                                            ordered, Get_TrxName()))
                                    return false;
                                line.SetQtyReserved(Env.ZERO);
                                if (!line.Save(Get_TrxName()))
                                    return false;
                            }
                        }
                        continue;
                    }

                    log.Fine("Line=" + line.GetLine()
                        + " - Target=" + target + ",Difference=" + difference
                        + " - Ordered=" + line.GetQtyOrdered()
                        + ",Reserved=" + line.GetQtyReserved() + ",Delivered=" + line.GetQtyDelivered());

                    //	Check Product - Stocked and Item
                    MProduct product1 = line.GetProduct();
                    if (product1 != null)
                    {
                        if (product1.IsStocked())
                        {
                            Decimal ordered = isSOTrx ? Env.ZERO : difference;

                            //if (dt.GetValue() == "BSO" && ordered == 0 && difference > 0)  // In case of Blanket Sales order set quantity order at Storage.
                            //{
                            //    ordered = difference;
                            //}

                            Decimal reserved = isSOTrx ? difference : Env.ZERO;
                            //	Get default Location'
                            //MWarehouse wh = MWarehouse.Get(GetCtx(), line.GetM_Warehouse_ID());

                            if (Env.IsModuleInstalled("VAPOS_") && Util.GetValueOfInt(GetVAPOS_POSTerminal_ID()) > 0)
                            {
                                //	Get Locator to reserve
                                if (line.GetM_AttributeSetInstance_ID() != 0)	//	Get existing Location
                                    M_Locator_ID = MStorage.GetM_Locator_ID(line.GetM_Warehouse_ID(),
                                        line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
                                        ordered, Get_TrxName());
                                //	Get default Location
                                if (M_Locator_ID == 0)
                                {
                                    M_Locator_ID = wh.GetDefaultM_Locator_ID();
                                }
                            }
                            else
                            {
                                M_Locator_ID = wh.GetDefaultM_Locator_ID();

                                //	Get Locator to reserve
                                if (M_Locator_ID == 0)
                                {
                                    if (line.GetM_AttributeSetInstance_ID() != 0)	//	Get existing Location
                                        M_Locator_ID = MStorage.GetM_Locator_ID(line.GetM_Warehouse_ID(),
                                            line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
                                            ordered, Get_TrxName());
                                }
                                if (M_Locator_ID == 0)
                                {
                                    String sql = "SELECT M_Locator_ID FROM M_Locator WHERE M_Warehouse_ID=" + line.GetM_Warehouse_ID();
                                    M_Locator_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                                }
                            }


                            if (dt.IsReleaseDocument() && (dt.GetDocBaseType() == "SOO" || dt.GetDocBaseType() == "POO"))  //if (dt.GetValue() == "RSO" || dt.GetValue() == "RPO") // if (dt.IsSOTrx() && dt.GetDocBaseType() == "SOO" && dt.GetDocSubTypeSO() == "BO")   
                            {
                                // if it is Release Sales Order (RSO) Or Release Purchase Order (RPO) then donot reserve stock as it is already reserved during Blanket Sales Order Completion.
                            }
                            else
                            {
                                //	Update Storage
                                if (!MStorage.Add(GetCtx(), line.GetM_Warehouse_ID(), M_Locator_ID,
                                    line.GetM_Product_ID(),
                                    line.GetM_AttributeSetInstance_ID(), line.GetM_AttributeSetInstance_ID(),
                                    Env.ZERO, reserved, ordered, Get_TrxName()))
                                    return false;
                            }
                        }	//	stockec
                        //	update line                             

                        Decimal? qtyRel = MUOMConversion.ConvertProductTo(GetCtx(), line.GetM_Product_ID(), line.GetC_UOM_ID(), difference);

                        // Added by Bharat on 03 April 2018 to handle issue to set Blanket Order Quantity only in case of Blanket Order
                        if (dt.GetDocBaseType() == "BOO")
                        {
                            if (qtyRel != null)
                            {
                                line.SetQtyBlanket(Decimal.Add(line.GetQtyBlanket(), Convert.ToDecimal(qtyRel)));
                            }
                            else
                            {
                                line.SetQtyBlanket(Decimal.Add(line.GetQtyBlanket(), difference));
                            }
                        }
                        line.SetQtyReserved(Decimal.Add(line.GetQtyReserved(), difference));

                        if (!line.Save(Get_TrxName()))
                            return false;



                        if (dt.IsReleaseDocument() && (dt.GetDocBaseType() == "SOO" || dt.GetDocBaseType() == "POO"))  //if (dt.GetValue() == "RSO" || dt.GetValue() == "RPO") // if (dt.IsSOTrx() && dt.GetDocBaseType() == "SOO" && dt.GetDocSubTypeSO() == "BO")
                        {
                            MOrderLine lineBlanket = new MOrderLine(GetCtx(), line.GetC_OrderLine_Blanket_ID(), Get_TrxName());

                            if (qtyRel != null)
                            {
                                lineBlanket.SetQty(Decimal.Subtract(lineBlanket.GetQtyEntered(), Convert.ToDecimal(qtyRel)));
                            }
                            else
                            {
                                lineBlanket.SetQty(Decimal.Subtract(lineBlanket.GetQtyEntered(), difference));
                            }

                            lineBlanket.SetQtyReleased(Decimal.Add(lineBlanket.GetQtyReleased(), difference));

                            line.SetQtyBlanket(lineBlanket.GetQtyEntered());

                            lineBlanket.Save();
                            line.Save();
                        }


                        Volume = Decimal.Add(Volume, (Decimal.Multiply((Decimal)product1.GetVolume(), line.GetQtyOrdered())));
                        Weight = Decimal.Add(Weight, (Decimal.Multiply(product1.GetWeight(), line.GetQtyOrdered())));
                    }	//	product
                }	//	reverse inventory

                SetVolume(Volume);
                SetWeight(Weight);
            }
            catch (Exception ex)
            {
                //ShowMessage.Error("MOrder", null, "ReserveStock");
            }
            return true;
        }

        /// <summary>
        /// Calculate Tax and Total
        /// </summary>
        /// <returns>true if tax total calculated</returns>
        public bool CalculateTaxTotal()
        {
            try
            {
                log.Fine("");
                //	Delete Taxes
                DataBase.DB.ExecuteQuery("DELETE FROM C_OrderTax WHERE C_Order_ID=" + GetC_Order_ID(), null, Get_TrxName());
                _taxes = null;

                DataSet dsOdrLine = DB.ExecuteDataset("SELECT LineNetAmt, TaxAbleAmt, C_Order_ID, C_Tax_ID FROM C_OrderLine WHERE C_Order_ID=" + GetC_Order_ID(), null, Get_TrxName());

                //	Lines
                Decimal totalLines = Env.ZERO;
                List<int> taxList = new List<int>();
                MOrderLine[] lines = GetLines();
                for (int i = 0; i < lines.Length; i++)
                {
                    MOrderLine line = lines[i];
                    int taxID = line.GetC_Tax_ID();
                    if (!taxList.Contains(taxID))
                    {
                        MOrderTax oTax = MOrderTax.Get(line, GetPrecision(),
                            false, Get_TrxName());	//	current Tax
                        //oTax.SetIsTaxIncluded(IsTaxIncluded());
                        if (!oTax.CalculateTaxFromLines(dsOdrLine))
                            return false;
                        if (!oTax.Save(Get_TrxName()))
                            return false;
                        taxList.Add(taxID);

                        // if Surcharge Tax is selected then calculate Tax for this Surcharge Tax.
                        if (line.Get_ColumnIndex("SurchargeAmt") >= 0)
                        {
                            oTax = MOrderTax.GetSurcharge(line, GetPrecision(), false, Get_TrxName());  //	current Tax
                            if (oTax != null)
                            {
                                if (!oTax.CalculateSurchargeFromLines())
                                    return false;
                                if (!oTax.Save(Get_TrxName()))
                                    return false;
                            }
                        }
                    }
                    totalLines = Decimal.Add(totalLines, line.GetLineNetAmt());
                }

                //	Taxes
                Decimal grandTotal = totalLines;
                MOrderTax[] taxes = GetTaxes(true);
                for (int i = 0; i < taxes.Length; i++)
                {
                    MOrderTax oTax = taxes[i];
                    MTax tax = oTax.GetTax();
                    if (tax.IsSummary())
                    {
                        MTax[] cTaxes = tax.GetChildTaxes(false);
                        for (int j = 0; j < cTaxes.Length; j++)
                        {
                            MTax cTax = cTaxes[j];
                            Decimal taxAmt = cTax.CalculateTax(oTax.GetTaxBaseAmt(), false, GetPrecision());

                            // JID_0430: if we add 2 lines with different Taxes. one is Parent and other is child. System showing error on completion that "Error Calculating Tax"
                            if (taxList.Contains(cTax.GetC_Tax_ID()))
                            {
                                String sql = "SELECT * FROM C_OrderTax WHERE C_Order_ID=" + GetC_Order_ID() + " AND C_Tax_ID=" + cTax.GetC_Tax_ID();
                                DataSet ds = DB.ExecuteDataset(sql, null, Get_TrxName());
                                if (ds != null && ds.Tables[0].Rows.Count > 0)
                                {
                                    DataRow dr = ds.Tables[0].Rows[0];
                                    MOrderTax newOTax = new MOrderTax(GetCtx(), dr, Get_TrxName());
                                    newOTax.SetTaxAmt(Decimal.Add(newOTax.GetTaxAmt(), taxAmt));
                                    newOTax.SetTaxBaseAmt(Decimal.Add(newOTax.GetTaxBaseAmt(), oTax.GetTaxBaseAmt()));
                                    if (!newOTax.Save(Get_TrxName()))
                                    {
                                        return false;
                                    }
                                }
                                ds = null;
                            }
                            else
                            {
                                //
                                MOrderTax newOTax = new MOrderTax(GetCtx(), 0, Get_TrxName());
                                newOTax.SetClientOrg(this);
                                newOTax.SetC_Order_ID(GetC_Order_ID());
                                newOTax.SetC_Tax_ID(cTax.GetC_Tax_ID());
                                newOTax.SetPrecision(GetPrecision());
                                newOTax.SetIsTaxIncluded(IsTaxIncluded());
                                newOTax.SetTaxBaseAmt(oTax.GetTaxBaseAmt());
                                newOTax.SetTaxAmt(taxAmt);
                                if (!newOTax.Save(Get_TrxName()))
                                    return false;
                            }
                            //
                            if (!IsTaxIncluded())
                                grandTotal = Decimal.Add(grandTotal, taxAmt);
                        }
                        if (!oTax.Delete(true, Get_TrxName()))
                            return false;
                        _taxes = null;
                    }
                    else
                    {
                        if (!IsTaxIncluded())
                            grandTotal = Decimal.Add(grandTotal, oTax.GetTaxAmt());
                    }
                }
                //
                SetTotalLines(totalLines);
                SetGrandTotal(Decimal.Round(grandTotal, GetPrecision()));
            }
            catch
            {
                //ShowMessage.Error("MOrder",null,"CalculateTaxTotal");
            }
            return true;
        }

        /// <summary>
        /// Approve Document
        /// </summary>
        /// <returns>true if success</returns>
        public bool ApproveIt()
        {
            log.Info("approveIt - " + ToString());
            SetIsApproved(true);
            return true;
        }

        /// <summary>
        /// Reject Approval
        /// </summary>
        /// <returns>true if success</returns>
        public bool RejectIt()
        {
            log.Info("rejectIt - " + ToString());
            SetIsApproved(false);
            return true;
        }

        /// <summary>
        /// Complete Document
        /// </summary>
        /// <returns>new status (Complete, In Progress, Invalid, Waiting ..)</returns>
        /****************************************************************************************************/
        public String CompleteIt()
        {
            // chck pallet Functionality applicable or not
            isContainerApplicable = MTransaction.ProductContainerApplicable(GetCtx());

            try
            {
                MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
                DocSubTypeSO = dt.GetDocSubTypeSO();

                //to check document type if it is pos then we need to check the document type on selected document types
                if (MDocType.DOCSUBTYPESO_POSOrder.Equals(DocSubTypeSO))
                {
                    string paymentbaseType = Util.GetValueOfString(DB.ExecuteScalar("SELECT VA009_PaymentBaseType FROM VA009_PaymentMethod WHERE VA009_PaymentMethod_ID = " + GetVA009_PaymentMethod_ID(), null, Get_Trx()));
                    if (!paymentbaseType.Equals(PAYMENTRULEPO_Check) && !paymentbaseType.Equals(PAYMENTRULEPO_LetterOfCredit) && !paymentbaseType.Equals(PAYMENTRULEPO_Cash) && !paymentbaseType.Equals(PAYMENTRULEPO_CashPlusCard))
                    {
                        if ((dt.Get_ColumnIndex("C_DocTypePayment_ID") >= 0) && (dt.Get_ColumnIndex("C_BankAccount_ID") >= 0))
                        {
                            if (dt.GetC_DocTypeShipment_ID() == 0 || dt.GetC_DocTypeInvoice_ID() == 0 || dt.GetC_DocTypePayment_ID() == 0 || dt.GetC_BankAccount_ID() == 0)
                            {
                                _processMsg = Msg.GetMsg(GetCtx(), "VIS_PosDocTypeConfig");
                                return DOCSTATUS_Invalid;
                            }
                        }
                    }
                    else
                    {
                        if (paymentbaseType.Equals(PAYMENTRULEPO_Cash) || paymentbaseType.Equals(PAYMENTRULEPO_CashPlusCard))
                        {
                            if (dt.GetC_DocTypeShipment_ID() == 0 || dt.GetC_DocTypeInvoice_ID() == 0)
                            {
                                _processMsg = Msg.GetMsg(GetCtx(), "VIS_PosDocTypeConfig");
                                return DOCSTATUS_Invalid;
                            }
                        }
                    }
                }

                //	Just prepare
                if (DOCACTION_Prepare.Equals(GetDocAction()))
                {
                    SetProcessed(false);
                    return DocActionVariables.STATUS_INPROGRESS;
                }

                // Set Document Date based on setting on Document Type
                SetCompletedDocumentDate();

                if (!IsReturnTrx())
                {
                    //	Offers
                    if (MDocType.DOCSUBTYPESO_Proposal.Equals(DocSubTypeSO)
                        || MDocType.DOCSUBTYPESO_Quotation.Equals(DocSubTypeSO))
                    {
                        //	Binding
                        if (MDocType.DOCSUBTYPESO_Quotation.Equals(DocSubTypeSO))
                            ReserveStock(dt, GetLines(true, "M_Product_ID"));

                        // Added by Bharat on 22 August 2017 to copy lines to new table Order Line History in case of Quotation
                        if (PO.Get_Table_ID("C_OrderlineHistory") > 0)
                        {
                            #region C_OrderlineHistory
                            MOrderlineHistory lHist = null;
                            GetLines(true, null);
                            if (_lines.Length > 0)
                            {
                                for (int i = 0; i < _lines.Length; i++)
                                {
                                    lHist = new MOrderlineHistory(GetCtx(), 0, Get_TrxName());
                                    lHist.SetClientOrg(_lines[i]);
                                    lHist.SetC_OrderLine_ID(_lines[i].Get_ID());
                                    lHist.SetC_Charge_ID(_lines[i].GetC_Charge_ID());
                                    lHist.SetC_Frequency_ID(_lines[i].GetC_Frequency_ID());
                                    lHist.SetC_Tax_ID(_lines[i].GetC_Tax_ID());
                                    lHist.SetDateOrdered(_lines[i].GetDateOrdered());
                                    lHist.SetDatePromised(_lines[i].GetDatePromised());
                                    lHist.SetDescription(_lines[i].GetDescription());
                                    lHist.SetDiscount(_lines[i].GetDiscount());
                                    lHist.SetEndDate(_lines[i].GetEndDate());
                                    lHist.SetLineNetAmt(_lines[i].GetLineNetAmt());
                                    lHist.SetM_Product_ID(_lines[i].GetM_Product_ID());
                                    lHist.SetC_UOM_ID(_lines[i].GetC_UOM_ID());
                                    lHist.SetM_Shipper_ID(_lines[i].GetM_Shipper_ID());
                                    lHist.SetNoofCycle(_lines[i].GetNoofCycle());
                                    lHist.SetPriceActual(_lines[i].GetPriceActual());
                                    lHist.SetPriceCost(_lines[i].GetPriceCost());
                                    lHist.SetPriceEntered(_lines[i].GetPriceEntered());
                                    lHist.SetPriceList(_lines[i].GetPriceList());
                                    lHist.SetProcessed(true);
                                    lHist.SetQtyEntered(_lines[i].GetQtyEntered());
                                    lHist.SetQtyOrdered(_lines[i].GetQtyOrdered());
                                    lHist.SetQtyPerCycle(_lines[i].GetQtyPerCycle());
                                    lHist.SetStartDate(_lines[i].GetStartDate());
                                    if (!lHist.Save(Get_TrxName()))
                                    {
                                        _processMsg = "Could not Create Order Line History";
                                        return DocActionVariables.STATUS_INVALID;
                                    }
                                }
                            }
                            #endregion
                        }
                        SetProcessed(true);
                        SetDocAction(DOCACTION_Close);
                        return DocActionVariables.STATUS_COMPLETED;
                    }
                    //	Waiting Payment - until we have a payment
                    if (!_forceCreation
                        && MDocType.DOCSUBTYPESO_PrepayOrder.Equals(DocSubTypeSO)
                        && GetC_Payment_ID() == 0 && GetC_CashLine_ID() == 0)
                    {
                        SetProcessed(true);
                        return DocActionVariables.STATUS_WAITINGPAYMENT;
                    }


                    //by Sukhwinder on 28 July for Release Sales/Purchase order completion
                    if (dt.IsReleaseDocument() && (dt.GetDocBaseType() == "SOO" || dt.GetDocBaseType() == "POO"))// if (dt.GetValue() == "RSO" || dt.GetValue() == "RPO")  ///if (dt.IsSOTrx() && dt.GetDocBaseType() == "SOO" && dt.GetDocSubTypeSO() == "BO")     //if (dt.GetValue() == "RSO")
                    {
                        MOrderLine[] lines = GetLines(true, "M_Product_ID");
                        //MOrder mo = new MOrder(GetCtx(), GetC_Order_ID(), null);
                        //MOrder moBlanket = new MOrder(GetCtx(), mo.GetC_Order_Blanket(), null);

                        if (lines.Length == 0)
                        {
                            _processMsg = "@NoLines@";
                            return DocActionVariables.STATUS_INVALID;
                        }

                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (lines[i].GetM_Warehouse_ID() != GetM_Warehouse_ID())
                            {
                                log.Warning("different Warehouse " + lines[i]);
                                _processMsg = "@CannotChangeDocType@";
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }

                        #region commented
                        //for (int i = 0; i < lines.Length; i++)
                        //{
                        //    bool binding = !dt.IsProposal();

                        //    Decimal target = binding ? lines[i].GetQtyOrdered() : Env.ZERO;

                        //    Decimal difference = 0;

                        //    difference = Decimal.Subtract(lines[i].GetQtyReserved(), lines[i].GetQtyDelivered());

                        //    MOrderLine lineBlanket = new MOrderLine(GetCtx(), lines[i].GetC_OrderLine_Blanket_ID(), null);

                        //    lineBlanket.SetQty(Decimal.Subtract(lineBlanket.GetQtyEntered(), difference));
                        //    lineBlanket.SetQtyReleased(Decimal.Add(lineBlanket.GetQtyReleased(), difference));

                        //    lines[i].SetQtyBlanket(lineBlanket.GetQtyEntered());

                        //    lineBlanket.Save();
                        //    lines[i].Save();
                        //}                      


                        //for (int i = 0; i < lines.Length; i++)
                        //{
                        //    MOrderLine lineBlanket = new MOrderLine(GetCtx(), lines[i].GetC_OrderLine_Blanket_ID(), null);

                        //    if (lineBlanket.GetQtyEntered() > 0
                        //        && Decimal.Subtract(lines[i].GetQtyBlanket(), lines[i].GetQtyEntered()) > 0
                        //        )
                        //    {
                        //        if (lines[i].GetQtyEntered() > lines[i].GetQtyReleased())
                        //        {
                        //            lineBlanket.SetQty(Decimal.Subtract(lineBlanket.GetQtyEntered(), Decimal.Subtract(lines[i].GetQtyEntered(), lines[i].GetQtyReleased())));
                        //            lineBlanket.SetQtyReleased(Decimal.Add(Decimal.Subtract(lines[i].GetQtyEntered(), lines[i].GetQtyReleased()), lineBlanket.GetQtyReleased()));

                        //        }
                        //        else if (lines[i].GetQtyEntered() == lines[i].GetQtyReleased())
                        //        {
                        //            if (Decimal.Subtract(lineBlanket.GetQtyEntered(), lineBlanket.GetQtyReleased()) == lineBlanket.GetQtyEntered())
                        //            {
                        //                lineBlanket.SetQty(Decimal.Subtract(lineBlanket.GetQtyEntered(), lines[i].GetQtyEntered()));
                        //                lineBlanket.SetQtyReleased(Decimal.Add(lineBlanket.GetQtyReleased(), lines[i].GetQtyEntered()));
                        //            }
                        //        }
                        //        else
                        //        {
                        //            lineBlanket.SetQty(Decimal.Add(lineBlanket.GetQtyEntered(), Decimal.Subtract(lines[i].GetQtyReleased(), lines[i].GetQtyEntered())));
                        //            lineBlanket.SetQtyReleased(Decimal.Subtract(lineBlanket.GetQtyReleased(), Decimal.Subtract(lines[i].GetQtyReleased(), lines[i].GetQtyEntered())));
                        //        }

                        //        // lineBlanket.SetQty(lineBlanket.GetQtyEntered() - lines[i].GetQtyEntered());
                        //        //lineBlanket.SetQtyReleased(Decimal.Subtract(Decimal.Add(lineBlanket.GetQtyReleased(), lines[i].GetQtyEntered()), lines[i].GetQtyReleased()));

                        //        // lines[i].SetQtyReserved(Decimal.Add(lines[i].GetQtyReserved(), lines[i].GetQtyEntered()));
                        //        lines[i].SetQtyBlanket(lineBlanket.GetQtyEntered());
                        //    }

                        //    lineBlanket.Save();
                        //    lines[i].Save();
                        //}
                        #endregion

                    }

                    // Enabled Order History Tab
                    //if (dt.GetDocBaseType() == "BOO") ///dt.GetValue() == "BSO" || dt.GetValue() == "BPO")
                    //{
                    if (PO.Get_Table_ID("C_OrderlineHistory") > 0)
                    {
                        #region C_OrderlineHistory
                        MOrderlineHistory lHist = null;
                        GetLines(true, null);
                        if (_lines.Length > 0)
                        {
                            for (int i = 0; i < _lines.Length; i++)
                            {
                                lHist = new MOrderlineHistory(GetCtx(), 0, Get_TrxName());
                                lHist.SetClientOrg(_lines[i]);
                                lHist.SetC_OrderLine_ID(_lines[i].Get_ID());
                                lHist.SetC_Charge_ID(_lines[i].GetC_Charge_ID());
                                lHist.SetC_Frequency_ID(_lines[i].GetC_Frequency_ID());
                                lHist.SetC_Tax_ID(_lines[i].GetC_Tax_ID());
                                lHist.SetDateOrdered(_lines[i].GetDateOrdered());
                                lHist.SetDatePromised(_lines[i].GetDatePromised());
                                lHist.SetDescription(_lines[i].GetDescription());
                                lHist.SetDiscount(_lines[i].GetDiscount());
                                lHist.SetEndDate(_lines[i].GetEndDate());
                                lHist.SetLineNetAmt(_lines[i].GetLineNetAmt());
                                lHist.SetM_Product_ID(_lines[i].GetM_Product_ID());
                                lHist.SetC_UOM_ID(_lines[i].GetC_UOM_ID());
                                lHist.SetM_Shipper_ID(_lines[i].GetM_Shipper_ID());
                                lHist.SetNoofCycle(_lines[i].GetNoofCycle());
                                lHist.SetPriceActual(_lines[i].GetPriceActual());
                                lHist.SetPriceCost(_lines[i].GetPriceCost());
                                lHist.SetPriceEntered(_lines[i].GetPriceEntered());
                                lHist.SetPriceList(_lines[i].GetPriceList());
                                lHist.SetProcessed(true);
                                lHist.SetQtyEntered(_lines[i].GetQtyEntered());
                                lHist.SetQtyOrdered(_lines[i].GetQtyOrdered());
                                lHist.SetQtyPerCycle(_lines[i].GetQtyPerCycle());
                                lHist.SetStartDate(_lines[i].GetStartDate());
                                if (!lHist.Save(Get_TrxName()))
                                {
                                    _processMsg = "Could not Create Order Line History";
                                    return DocActionVariables.STATUS_INVALID;
                                }
                            }
                        }
                        #endregion
                        //}
                    }

                    //	Re-Check
                    if (!_justPrepared)
                    {
                        String status = PrepareIt();
                        if (!DocActionVariables.STATUS_INPROGRESS.Equals(status))
                            return status;
                    }
                }

                // Handle Budget Control
                if (Env.IsModuleInstalled("FRPT_") && !IsSOTrx() && !IsReturnTrx())
                {
                    // Once budget breach approved do not check budget breach functionality again
                    if (!IsBudgetBreachApproved())
                    {
                        // budget control functionality work when Financial Managemt Module Available
                        try
                        {

                            log.Info("Budget Control Start for PO Document No  " + GetDocumentNo());
                            EvaluateBudgetControlData();
                            // If Budget Exceeded or Not Defined By Rakesh Kumar 29/Apr/2021
                            if (_budgetMessage.Length > 0 || _budgetNotDefined.Length > 0)
                            {
                                // Done by Rakesh Kumar On 29/Apr/2021
                                // When budget exceeded
                                if (_budgetMessage.Length > 0)
                                {
                                    _processMsg = Msg.GetMsg(GetCtx(), "BudgetExceedFor") + _budgetMessage;
                                }
                                if (_budgetNotDefined.Length > 0)
                                {
                                    _processMsg = _processMsg + "" + Msg.GetMsg(GetCtx(), "BudgetNotDefinedFor") + _budgetNotDefined;
                                }
                                SetProcessed(false);
                                // Set Budget Breach only in case when budget exceeded
                                // Done by Rakesh 29/Apr/2021
                                if (_budgetMessage.Length > 0 && _budgetNotDefined.Length == 0)
                                {
                                    // Done by rakesh 18/Feb/2020
                                    SetIsBudgetBreach(true);
                                    SetIsBudgetBreachApproved(false);
                                }
                                return DocActionVariables.STATUS_INPROGRESS;
                            }
                            SetIsBudgetBreach(false);
                            SetIsBudgetBreachApproved(true);
                            log.Info("Budget Control Completed for PO Document No  " + GetDocumentNo());
                        }
                        catch (Exception ex)
                        {
                            log.Severe("Budget Control Issue " + ex.Message);
                            SetProcessed(false);
                            SetIsBudgetBreach(false);
                            return DocActionVariables.STATUS_INPROGRESS;
                        }
                    }
                }

                //	Implicit Approval
                if (!IsApproved())
                    ApproveIt();
                GetLines(false, null);

                //JID_1126: System will check the selected Vendor and product to update the price on Purchasing tab.
                if (!IsSOTrx() && !IsReturnTrx() && dt.GetDocBaseType() == "POO")
                {
                    MProductPO po = null;

                    for (int i = 0; i < _lines.Length; i++)
                    {
                        if (_lines[i].GetC_Charge_ID() > 0)
                        {
                            continue;
                        }
                        po = MProductPO.GetOfVendorProduct(GetCtx(), GetC_BPartner_ID(), _lines[i].GetM_Product_ID(), Get_Trx());
                        if (po != null)
                        {
                            po.SetPriceLastPO(_lines[i].GetPriceEntered());
                            if (!po.Save())
                            {
                                _processMsg = Msg.GetMsg(GetCtx(), "NotUpdatePOPrice");
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }
                    }
                }

                log.Info(ToString());
                StringBuilder Info = new StringBuilder();

                /* nnayak - Bug 1720003 - We need to set the processed flag so the Tax Summary Line
                does not get recreated in the afterSave procedure of the MOrderLine class */
                SetProcessed(true);

                bool realTimePOS = false;

                try
                {
                    //	Counter Documents
                    MOrder counter = CreateCounterDoc();
                    if (counter != null)
                    {
                        Info.Append(" - @CounterDoc@: @Order@=").Append(counter.GetDocumentNo());
                    }
                }
                catch (Exception e)
                {
                    //if get any exception while creating counter document then return the message to user 
                    // and not to complete the main record
                    //Info.Append(" - @CounterDoc@: ").Append(e.Message.ToString());
                    _processMsg = e.Message.ToString();
                    return DocActionVariables.STATUS_INPROGRESS;
                }

                ////	Create SO Shipment - Force Shipment
                MInOut shipment = null;
                // Shipment not created in case of Resturant               

                if (Util.GetValueOfString(dt.GetVAPOS_POSMode()) != "RS")
                {
                    if (MDocType.DOCSUBTYPESO_OnCreditOrder.Equals(DocSubTypeSO)		//	(W)illCall(I)nvoice
                        || MDocType.DOCSUBTYPESO_WarehouseOrder.Equals(DocSubTypeSO)	//	(W)illCall(P)ickup	
                        || MDocType.DOCSUBTYPESO_POSOrder.Equals(DocSubTypeSO)			//	(W)alkIn(R)eceipt
                        || MDocType.DOCSUBTYPESO_PrepayOrder.Equals(DocSubTypeSO))
                    {
                        if (!DELIVERYRULE_Force.Equals(GetDeliveryRule()))
                            SetDeliveryRule(DELIVERYRULE_Force);
                        //
                        shipment = CreateShipment(dt, realTimePOS ? null : GetDateOrdered());
                        if (shipment == null)
                            return DocActionVariables.STATUS_INVALID;

                        //(1052)correct process message
                        Info.Append(Msg.GetMsg(GetCtx(), "SucessfullyCreatedMInout")).Append(shipment.GetDocumentNo());
                        _processMsg = Info.ToString();
                        if (shipment.GetDocStatus() == "DR")
                        {
                            if (String.IsNullOrEmpty(_processMsg))
                            {
                                _processMsg = " Could Not Complete because Reserved qty is greater than Onhand qty, Available Qty Is : " + OnHandQty;
                            }
                            shipment.SetProcessMsg(_processMsg);
                            // Info.Append(" " + _processMsg);
                        }


                        String msg = shipment.GetProcessMsg();
                        if (msg != null && msg.Length > 0)
                            Info.Append(" (").Append(msg).Append(")");
                    }
                }


                //	Create SO Invoice - Always invoice complete Order
                if (MDocType.DOCSUBTYPESO_POSOrder.Equals(DocSubTypeSO)
                    || MDocType.DOCSUBTYPESO_OnCreditOrder.Equals(DocSubTypeSO)
                    || MDocType.DOCSUBTYPESO_PrepayOrder.Equals(DocSubTypeSO))
                {
                    try
                    {
                        DateTime? tSet = realTimePOS ? null : GetDateOrdered();
                        MInvoice invoice = CreateInvoice(dt, shipment, tSet);
                        if (invoice == null)
                        {
                            Get_Trx().Rollback();
                            return DocActionVariables.STATUS_INVALID;
                        }

                        //Info.Append(" - @C_Invoice_ID@: ").Append(invoice.GetDocumentNo());
                        //Info.Append(" & @C_Invoice_ID@ No: ").Append(invoice.GetDocumentNo()).Append(" generated successfully");
                        //(1052)correct process message
                        Info.Append(Msg.GetMsg(GetCtx(), "InvNo")).Append(invoice.GetDocumentNo());
                        _processMsg += Info.ToString();

                        String msg = invoice.GetProcessMsg();
                        if (msg != null && msg.Length > 0)
                            Info.Append(" (").Append(msg).Append(")");

                        // for POS Doctype we need to create payment with invoice
                        if (invoice != null)
                        {
                            if (MDocType.DOCSUBTYPESO_POSOrder.Equals(DocSubTypeSO))
                            {
                                if (!X_C_Invoice.PAYMENTRULE_Cash.Equals(invoice.GetPaymentRule()) && !X_C_Invoice.PAYMENTRULE_CashAndCredit.Equals(invoice.GetPaymentRule()))
                                {
                                    MPayment _pay = null;
                                    _pay = CreatePaymentAgainstPOSDocType(Info, invoice);
                                    if (_pay == null)
                                    {
                                        if (_processMsg != null && _processMsg.Length > 0)
                                            Info.Append(" (").Append(_processMsg).Append(")");
                                        Get_Trx().Rollback();
                                        return DocActionVariables.STATUS_INVALID;
                                    }

                                }
                            }
                        }
                    }
                    catch (NullReferenceException ex)
                    {
                        //ShowMessage.Error("Moder",null,"Completeit");
                    }
                }

                ////	Counter Documents
                //MOrder counter = CreateCounterDoc();
                //if (counter != null)
                //    Info.Append(" - @CounterDoc@: @Order@=").Append(counter.GetDocumentNo());
                //if (havingPriceList == 0)
                //{
                //    _processMsg = Msg.GetMsg(GetCtx(), "VIS_PriceListNotAvilable");
                //    return DocActionVariables.STATUS_INVALID;
                //}

                //User Validation
                String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
                if (valid != null)
                {
                    if (Info.Length > 0)
                        Info.Append(" - ");
                    Info.Append(valid);
                    _processMsg = Info.ToString();
                    return DocActionVariables.STATUS_INVALID;
                }
                /******************/
                String Qry = "SELECT * FROM C_OrderLine WHERE C_Order_ID=" + GetC_Order_ID();
                DataSet orderlines = DB.ExecuteDataset(Qry);
                // Set IsContract to true if IsContract selected on order line
                if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(IsContract) FROM C_OrderLine WHERE C_Order_ID = " + GetC_Order_ID() +
                                                         " AND IsContract = 'Y' AND IsActive = 'Y' ")) > 0)
                {
                    Qry = @"UPDATE C_Order SET IsContract='Y' WHERE C_Order_ID=" + GetC_Order_ID();
                    DB.ExecuteQuery(Qry, null, Get_TrxName());
                }
                if (orderlines.Tables[0].Rows.Count > 0)
                {
                    //for (int i = 0; i < orderlines.Tables[0].Rows.Count; i++)
                    //{
                    //Char IsCont = Convert.ToChar(orderlines.Tables[0].Rows[i]["IsContract"]);
                    //if (IsCont == 'Y')
                    //{
                    //    MOrder mo = new MOrder(GetCtx(), GetC_Order_ID(), Get_Trx());
                    //    mo.SetIsContract(true);
                    //    mo.Save();
                    //}

                    ////Set Values on unit window if Unit is seleced on Line in case of POC Construction Module installed only for demo perpose
                    if (Env.IsModuleInstalled("VA052_"))
                    {
                        for (int i = 0; i < orderlines.Tables[0].Rows.Count; i++)
                        {
                            int UnitID = Util.GetValueOfInt(orderlines.Tables[0].Rows[i]["A_Asset_ID"]); //AssetID is UnitID
                            if (UnitID > 0)
                            {
                                MAsset asset = new MAsset(GetCtx(), UnitID, Get_Trx());
                                asset.Set_Value("VA052_Status", "SO");
                                asset.Set_Value("VA052_LoanAmount", Util.GetValueOfDecimal(orderlines.Tables[0].Rows[i]["VA052_LoanAmount"]));
                                asset.Set_Value("C_BPartner_ID", GetC_BPartner_ID());
                                asset.Set_Value("VA052_BuyersCont", Util.GetValueOfDecimal(orderlines.Tables[0].Rows[i]["LineNetAmt"]) - Util.GetValueOfDecimal(orderlines.Tables[0].Rows[i]["VA052_LoanAmount"]));
                                if (!asset.Save())
                                    log.SaveError("Error", "WhileSavingDataOnAssetWindow_VA052");

                            }
                        }
                    }

                    //Set VA077_IsContract check if VA077 module installed and VA077_IsContract selected on order line
                    if (Env.IsModuleInstalled("VA077_"))
                    {
                        if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(VA077_IsContract) FROM 
                                                                  C_OrderLine WHERE C_Order_ID = " + GetC_Order_ID() + @" AND 
                                                                  VA077_IsContract = 'Y' AND IsActive = 'Y' ")) > 0)
                        {

                            Qry = @"UPDATE C_Order SET VA077_IsContract='Y', VA077_CreateServiceContract='N'                            
                                    WHERE C_Order_ID=" + GetC_Order_ID();
                            int no = DB.ExecuteQuery(Qry, null, Get_TrxName());
                            if (no <= 0)
                            {
                                log.SaveWarning("", Msg.GetMsg(GetCtx(), "VIS_NotUpdated"));
                            }
                        }
                    }
                }

                /******************/

                //landed cost distribution
                if (!IsSOTrx())
                {
                    String error = ExpectedlandedCostDistribution();
                    if (!Util.IsEmpty(error))
                    {
                        _processMsg = error;
                        return DocActionVariables.STATUS_INVALID;
                    }
                }

                SetProcessed(true);
                _processMsg = Info.ToString();
                //
                SetDocAction(DOCACTION_Close);
                //Changes by abhishek suggested by lokesh on 7/1/2016
                //try
                //{
                //    int countVAPOS = Util.GetValueOfInt(DB.ExecuteScalar("Select count(*) from AD_ModuleInfo Where Prefix='VAPOS_'"));
                //    if (countVAPOS > 0)
                //    {
                //        MPriceList priceLst = new MPriceList(GetCtx(), GetM_PriceList_ID(), null);
                //        bool taxInclusive = priceLst.IsTaxIncluded();
                //        int VAPOS_POSTertminal_ID = Util.GetValueOfInt(DB.ExecuteScalar("Select VAPOS_POSTerminal_ID from c_Order Where C_Order_ID=" + GetC_Order_ID()));
                //        if (VAPOS_POSTertminal_ID > 0)
                //        {
                //            string cAmount = Util.GetValueOfString(DB.ExecuteScalar("Select VAPOS_CashPaid from c_Order Where C_Order_ID=" + GetC_Order_ID()));
                //            string pAmount = Util.GetValueOfString(DB.ExecuteScalar("Select VAPOS_PayAmt from c_Order Where C_Order_ID=" + GetC_Order_ID()));
                //            List<string> tax_IDLst = new List<string>();
                //            List<string> OLTaxAmtLst = new List<string>();
                //            List<string> DscLineLst = new List<string>();

                //            DataSet dsDE = DB.ExecuteDataset("select ol.C_Tax_ID, ol.VAPOS_DiscountAmount, ol.LINENETAMT, tx.rate from C_OrderLine ol inner join C_Tax tx on(ol.C_Tax_ID=tx.C_Tax_ID)  where C_Order_ID=" + GetC_Order_ID());
                //            try
                //            {
                //                if (dsDE != null)
                //                {
                //                    if (dsDE.Tables[0].Rows.Count > 0)
                //                    {
                //                        for (int i = 0; i < dsDE.Tables[0].Rows.Count; i++)
                //                        {
                //                            tax_IDLst.Add(Util.GetValueOfString(dsDE.Tables[0].Rows[i]["C_Tax_ID"]));
                //                            DscLineLst.Add(Util.GetValueOfString(dsDE.Tables[0].Rows[i]["VAPOS_DiscountAmount"]));
                //                            decimal taxRate = Util.GetValueOfDecimal(dsDE.Tables[0].Rows[i]["rate"]);
                //                            decimal LINENETAMT = Util.GetValueOfDecimal(dsDE.Tables[0].Rows[i]["LINENETAMT"]);
                //                            if (taxInclusive)
                //                            {
                //                                OLTaxAmtLst.Add(Convert.ToString(((LINENETAMT / (100 + taxRate)) * (taxRate / 100))));
                //                            }
                //                            else
                //                            {
                //                                OLTaxAmtLst.Add(Convert.ToString(taxRate * LINENETAMT / 100));

                //                            }

                //                        }
                //                    }
                //                    dsDE.Dispose();
                //                }
                //            }
                //            catch
                //            {
                //                if (dsDE != null) { dsDE.Dispose(); }
                //            }
                //            string[] tax_ID = tax_IDLst.ToArray();
                //            string[] OLTaxAmt = OLTaxAmtLst.ToArray();
                //            string[] DscLine = DscLineLst.ToArray();
                //            SaveDayEndRecord(GetCtx(), VAPOS_POSTertminal_ID, cAmount, pAmount, GetC_DocType_ID(), tax_ID, OLTaxAmt, GetGrandTotal().ToString(), DscLine);
                //        }
                //    }
                //}

                //catch
                //{
                //    //ShowMessage.Error("MOrder",null,"CompleteIt");
                //}
            }

            catch
            {
                //ShowMessage.Error("MOrder",null,"CompleteIt");
                _processMsg = GetProcessMsg();
                return DocActionVariables.STATUS_INVALID;
            }

            // Set the document number from completed document sequence after completed (if needed)
            SetCompletedDocumentNo();

            return DocActionVariables.STATUS_COMPLETED;
        }

        /// <summary>
        /// This function is used to check, document is budget control or not.
        /// </summary>
        /// <returns>True, when budget controlled or not applicable</returns>
        private bool EvaluateBudgetControlData()
        {
            DataSet dsRecordData;
            DataRow[] drRecordData = null;
            DataRow[] drBudgetControl = null;
            DataSet dsBudgetControlDimension;
            DataRow[] drBudgetControlDimension = null;
            List<BudgetControl> _budgetControl = new List<BudgetControl>();
            StringBuilder sql = new StringBuilder();
            BudgetCheck budget = new BudgetCheck();

            sql.Clear();
            sql.Append(@"SELECT GL_Budget.GL_Budget_ID , GL_Budget.BudgetControlBasis, GL_Budget.C_Year_ID , GL_Budget.C_Period_ID,GL_Budget.Name As BudgetName, 
                  GL_BudgetControl.C_AcctSchema_ID, GL_BudgetControl.CommitmentType, GL_BudgetControl.BudgetControlScope,  GL_BudgetControl.GL_BudgetControl_ID, GL_BudgetControl.Name AS ControlName,GL_BudgetControl.BudgetBreachPercent
                FROM GL_Budget 
                INNER JOIN GL_BudgetControl ON GL_Budget.GL_Budget_ID = GL_BudgetControl.GL_Budget_ID
                INNER JOIN Ad_ClientInfo ON Ad_ClientInfo.AD_Client_ID = GL_Budget.AD_Client_ID
                WHERE GL_BudgetControl.IsActive = 'Y' AND GL_Budget.IsActive = 'Y' AND GL_BudgetControl.AD_Org_ID IN (0 , " + GetAD_Org_ID() + @")
                   AND GL_BudgetControl.CommitmentType IN('B', 'C') AND
                  ((GL_Budget.BudgetControlBasis = 'P' AND GL_Budget.C_Period_ID =
                  (SELECT C_Period.C_Period_ID FROM C_Period INNER JOIN C_Year ON C_Year.C_Year_ID = C_Period.C_Year_ID
                  WHERE C_Period.IsActive = 'Y'  AND C_Year.C_Calendar_ID = Ad_ClientInfo.C_Calendar_ID
                  AND " + GlobalVariable.TO_DATE(GetDateAcct(), true) + @" BETWEEN C_Period.StartDate AND C_Period.EndDate))
                OR(GL_Budget.BudgetControlBasis = 'A' AND GL_Budget.C_Year_ID =
                  (SELECT C_Period.C_Year_ID FROM C_Period INNER JOIN C_Year ON C_Year.C_Year_ID = C_Period.C_Year_ID
                  WHERE C_Period.IsActive = 'Y'   AND C_Year.C_Calendar_ID = AD_ClientInfo.C_Calendar_ID
                AND " + GlobalVariable.TO_DATE(GetDateAcct(), true) + @" BETWEEN C_Period.StartDate AND C_Period.EndDate) ) ) 
                AND(SELECT COUNT(Fact_Acct_ID) FROM Fact_Acct
                WHERE GL_Budget_ID = GL_Budget.GL_Budget_ID
                AND(C_Period_ID  IN (NVL(GL_Budget.C_Period_ID, 0))
                OR C_Period_ID    IN (SELECT C_Period_ID FROM C_Period   WHERE C_Year_ID = NVL(GL_Budget.C_Year_ID, 0)))) > 0");
            DataSet dsBudgetControl = DB.ExecuteDataset(sql.ToString(), null, Get_Trx());
            if (dsBudgetControl != null && dsBudgetControl.Tables.Count > 0 && dsBudgetControl.Tables[0].Rows.Count > 0)
            {
                // get budget control ids (TODO for postgre)
                object[] budgetControlIds = dsBudgetControl.Tables[0].AsEnumerable().Select(r => r.Field<object>("GL_BUDGETCONTROL_ID")).ToArray();
                string result = string.Join(",", budgetControlIds);
                dsBudgetControlDimension = budget.GetBudgetDimension(result);

                // get record posting data 
                dsRecordData = BudgetControlling();
                if (dsRecordData != null && dsRecordData.Tables.Count > 0 && dsRecordData.Tables[0].Rows.Count > 0)
                {
                    // datarows of Debit values which to be controlled
                    drRecordData = dsRecordData.Tables[0].Select("Debit > 0 ", " Account_ID ASC");
                    if (drRecordData != null)
                    {
                        // loop on PO record data which is to be debited only 
                        for (int i = 0; i < drRecordData.Length; i++)
                        {
                            // datarows of Budget, of selected accouting schema
                            drBudgetControl = dsBudgetControl.Tables[0].Select("C_AcctSchema_ID  = " + Util.GetValueOfInt(drRecordData[i]["C_AcctSchema_ID"]));

                            // loop on Budget which to be controlled 
                            if (drBudgetControl != null)
                            {
                                for (int j = 0; j < drBudgetControl.Length; j++)
                                {
                                    // get budget Dimension datarow 
                                    drBudgetControlDimension = dsBudgetControlDimension.Tables[0].Select("GL_BudgetControl_ID  = "
                                                                + Util.GetValueOfInt(drBudgetControl[j]["GL_BudgetControl_ID"]));

                                    // get BUdgeted Controlled Value based on dimension
                                    _budgetControl = budget.GetBudgetControlValue(drRecordData[i], drBudgetControl[j], drBudgetControlDimension, GetDateAcct(),
                                        _budgetControl, Get_Trx(), 'O', GetC_Order_ID());

                                    // Reduce amount from Budget controlled value
                                    _budgetControl = ReduceAmountFromBudget(drRecordData[i], drBudgetControl[j], drBudgetControlDimension, _budgetControl);

                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //  no recod found for budget control 
                log.Info("Budget control not found" + sql.ToString());
                return true;
            }
            return true;
        }

        /// <summary>
        /// This Function is used to get data based on Posting Logic, which is to be posted after completion.
        /// </summary>
        /// <returns>DataSet of Posting Records</returns>
        private DataSet BudgetControlling()
        {
            int ad_window_id = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Window_ID FROM AD_Window WHERE  Export_ID = 'VIS_181'")); // Purchase Order
            DataSet result = new DataSet();
            Type type = null;
            MethodInfo methodInfo = null;
            string className = "FRPTSvc.Controllers.PostAccLocalizationVO";
            type = ClassTypeContainer.GetClassType(className, "FRPTSvc");
            if (type != null)
            {
                methodInfo = type.GetMethod("BudgetControlled");
                if (methodInfo != null)
                {
                    ParameterInfo[] parameters = methodInfo.GetParameters();
                    if (parameters.Length == 8)
                    {
                        object[] parametersArray = new object[] { GetCtx(),
                                                                Util.GetValueOfInt(GetAD_Client_ID()),
                                                                Util.GetValueOfInt(X_C_Order.Table_ID),//MTable.Get(GetCtx() , "C_Order").GetAD_Table_ID()
                                                                Util.GetValueOfInt(GetC_Order_ID()),
                                                                true,
                                                                Util.GetValueOfInt(GetAD_Org_ID()),
                                                                ad_window_id,
                                                                Util.GetValueOfInt(GetC_DocTypeTarget_ID()) };
                        result = (DataSet)methodInfo.Invoke(null, parametersArray);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// This Function is used to Reduce From Budget controlled amount
        /// </summary>
        /// <param name="drDataRecord">document Posting Record</param>
        /// <param name="drBUdgetControl">BUdget Control information</param>
        /// <param name="drBudgetComtrolDimension">Budget Control dimension which is applicable</param>
        /// <param name="_listBudgetControl">list of budget controls</param>
        /// <returns>modified list Budget Control</returns>
        public List<BudgetControl> ReduceAmountFromBudget(DataRow drDataRecord, DataRow drBUdgetControl, DataRow[] drBudgetComtrolDimension, List<BudgetControl> _listBudgetControl)
        {
            BudgetControl _budgetControl = null;
            List<String> selectedDimension = new List<string>();
            if (drBudgetComtrolDimension != null)
            {
                for (int i = 0; i < drBudgetComtrolDimension.Length; i++)
                {
                    selectedDimension.Add(Util.GetValueOfString(drBudgetComtrolDimension[i]["ElementType"]));
                }
            }

            if (_listBudgetControl.Exists(x => (x.GL_Budget_ID == Util.GetValueOfInt(drBUdgetControl["GL_Budget_ID"])) &&
                                              (x.GL_BudgetControl_ID == Util.GetValueOfInt(drBUdgetControl["GL_BudgetControl_ID"])) &&
                                              (x.Account_ID == Util.GetValueOfInt(drDataRecord["Account_ID"])) &&
                                              (x.AD_Org_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Organization) ? Util.GetValueOfInt(drDataRecord["AD_Org_ID"]) : 0)) &&
                                              (x.C_BPartner_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_BPartner) ? Util.GetValueOfInt(drDataRecord["C_BPartner_ID"]) : 0)) &&
                                              (x.M_Product_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Product) ? Util.GetValueOfInt(drDataRecord["M_Product_ID"]) : 0)) &&
                                              (x.C_Activity_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Activity) ? Util.GetValueOfInt(drDataRecord["C_Activity_ID"]) : 0)) &&
                                              (x.C_LocationFrom_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_LocationFrom) ? Util.GetValueOfInt(drDataRecord["C_LocationFrom_ID"]) : 0)) &&
                                              (x.C_LocationTo_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_LocationTo) ? Util.GetValueOfInt(drDataRecord["C_LocationTo_ID"]) : 0)) &&
                                              (x.C_Campaign_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Campaign) ? Util.GetValueOfInt(drDataRecord["C_Campaign_ID"]) : 0)) &&
                                              (x.AD_OrgTrx_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_OrgTrx) ? Util.GetValueOfInt(drDataRecord["AD_OrgTrx_ID"]) : 0)) &&
                                              (x.C_Project_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Project) ? Util.GetValueOfInt(drDataRecord["C_Project_ID"]) : 0)) &&
                                              (x.C_SalesRegion_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_SalesRegion) ? Util.GetValueOfInt(drDataRecord["C_SalesRegion_ID"]) : 0)) &&
                                              (x.UserList1_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserList1) ? Util.GetValueOfInt(drDataRecord["UserList1_ID"]) : 0)) &&
                                              (x.UserList2_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserList2) ? Util.GetValueOfInt(drDataRecord["UserList2_ID"]) : 0)) &&
                                              (x.UserElement1_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement1) ? Util.GetValueOfInt(drDataRecord["UserElement1_ID"]) : 0)) &&
                                              (x.UserElement2_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement2) ? Util.GetValueOfInt(drDataRecord["UserElement2_ID"]) : 0)) &&
                                              (x.UserElement3_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement3) ? Util.GetValueOfInt(drDataRecord["UserElement3_ID"]) : 0)) &&
                                              (x.UserElement4_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement4) ? Util.GetValueOfInt(drDataRecord["UserElement4_ID"]) : 0)) &&
                                              (x.UserElement5_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement5) ? Util.GetValueOfInt(drDataRecord["UserElement5_ID"]) : 0)) &&
                                              (x.UserElement6_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement6) ? Util.GetValueOfInt(drDataRecord["UserElement6_ID"]) : 0)) &&
                                              (x.UserElement7_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement7) ? Util.GetValueOfInt(drDataRecord["UserElement7_ID"]) : 0)) &&
                                              (x.UserElement8_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement8) ? Util.GetValueOfInt(drDataRecord["UserElement8_ID"]) : 0)) &&
                                              (x.UserElement9_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement9) ? Util.GetValueOfInt(drDataRecord["UserElement9_ID"]) : 0))
                                             ))
            {
                _budgetControl = _listBudgetControl.Find(x => (x.GL_Budget_ID == Util.GetValueOfInt(drBUdgetControl["GL_Budget_ID"])) &&
                                              (x.GL_BudgetControl_ID == Util.GetValueOfInt(drBUdgetControl["GL_BudgetControl_ID"])) &&
                                              (x.Account_ID == Util.GetValueOfInt(drDataRecord["Account_ID"])) &&
                                              (x.AD_Org_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Organization) ? Util.GetValueOfInt(drDataRecord["AD_Org_ID"]) : 0)) &&
                                              (x.C_BPartner_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_BPartner) ? Util.GetValueOfInt(drDataRecord["C_BPartner_ID"]) : 0)) &&
                                              (x.M_Product_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Product) ? Util.GetValueOfInt(drDataRecord["M_Product_ID"]) : 0)) &&
                                              (x.C_Activity_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Activity) ? Util.GetValueOfInt(drDataRecord["C_Activity_ID"]) : 0)) &&
                                              (x.C_LocationFrom_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_LocationFrom) ? Util.GetValueOfInt(drDataRecord["C_LocationFrom_ID"]) : 0)) &&
                                              (x.C_LocationTo_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_LocationTo) ? Util.GetValueOfInt(drDataRecord["C_LocationTo_ID"]) : 0)) &&
                                              (x.C_Campaign_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Campaign) ? Util.GetValueOfInt(drDataRecord["C_Campaign_ID"]) : 0)) &&
                                              (x.AD_OrgTrx_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_OrgTrx) ? Util.GetValueOfInt(drDataRecord["AD_OrgTrx_ID"]) : 0)) &&
                                              (x.C_Project_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Project) ? Util.GetValueOfInt(drDataRecord["C_Project_ID"]) : 0)) &&
                                              (x.C_SalesRegion_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_SalesRegion) ? Util.GetValueOfInt(drDataRecord["C_SalesRegion_ID"]) : 0)) &&
                                              (x.UserList1_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserList1) ? Util.GetValueOfInt(drDataRecord["UserList1_ID"]) : 0)) &&
                                              (x.UserList2_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserList2) ? Util.GetValueOfInt(drDataRecord["UserList2_ID"]) : 0)) &&
                                              (x.UserElement1_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement1) ? Util.GetValueOfInt(drDataRecord["UserElement1_ID"]) : 0)) &&
                                              (x.UserElement2_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement2) ? Util.GetValueOfInt(drDataRecord["UserElement2_ID"]) : 0)) &&
                                              (x.UserElement3_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement3) ? Util.GetValueOfInt(drDataRecord["UserElement3_ID"]) : 0)) &&
                                              (x.UserElement4_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement4) ? Util.GetValueOfInt(drDataRecord["UserElement4_ID"]) : 0)) &&
                                              (x.UserElement5_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement5) ? Util.GetValueOfInt(drDataRecord["UserElement5_ID"]) : 0)) &&
                                              (x.UserElement6_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement6) ? Util.GetValueOfInt(drDataRecord["UserElement6_ID"]) : 0)) &&
                                              (x.UserElement7_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement7) ? Util.GetValueOfInt(drDataRecord["UserElement7_ID"]) : 0)) &&
                                              (x.UserElement8_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement8) ? Util.GetValueOfInt(drDataRecord["UserElement8_ID"]) : 0)) &&
                                              (x.UserElement9_ID == (selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement9) ? Util.GetValueOfInt(drDataRecord["UserElement9_ID"]) : 0))
                                             );
                _budgetControl.ControlledAmount = Decimal.Subtract(_budgetControl.ControlledAmount, Util.GetValueOfDecimal(drDataRecord["Debit"]));
                if (_budgetControl.ControlledAmount < 0)
                {
                    if (!_budgetMessage.Contains(Util.GetValueOfString(drBUdgetControl["BudgetName"])))
                    {
                        _budgetMessage += Util.GetValueOfString(drBUdgetControl["BudgetName"]) + " - "
                                            + Util.GetValueOfString(drBUdgetControl["ControlName"]) + ", ";
                    }
                    log.Info("Budget Exceed - " + Util.GetValueOfString(drBUdgetControl["BudgetName"]) + " - "
                                        + Util.GetValueOfString(drBUdgetControl["ControlName"]) + " - (" + _budgetControl.ControlledAmount + ") - Table ID : " +
                                        Util.GetValueOfInt(drDataRecord["LineTable_ID"]) + " - Record ID : " + Util.GetValueOfInt(drDataRecord["Line_ID"]));
                }
            }
            else
            {
                if (_listBudgetControl.Exists(x => (x.GL_Budget_ID == Util.GetValueOfInt(drBUdgetControl["GL_Budget_ID"])) &&
                                             (x.GL_BudgetControl_ID == Util.GetValueOfInt(drBUdgetControl["GL_BudgetControl_ID"])) &&
                                             (x.Account_ID == Util.GetValueOfInt(drDataRecord["Account_ID"]))
                                            ))
                {
                    // If budget not defined then add error message in _budgetNotDefined message variable
                    // Done by rakesh on 29/Apr/2021 Messsage Variable changed from _budgetMessage to _budgetNotDefined
                    if (!_budgetNotDefined.Contains(Util.GetValueOfString(drBUdgetControl["BudgetName"])))
                    {
                        _budgetNotDefined += Util.GetValueOfString(drBUdgetControl["BudgetName"]) + " - "
                                            + Util.GetValueOfString(drBUdgetControl["ControlName"]) + ", ";
                    }
                    log.Info("Budget not defined for - " + Util.GetValueOfString(drBUdgetControl["BudgetName"]) + " - "
                                        + Util.GetValueOfString(drBUdgetControl["ControlName"]) + " - Table ID : " +
                                        Util.GetValueOfInt(drDataRecord["LineTable_ID"]) + " - Record ID : " + Util.GetValueOfInt(drDataRecord["Line_ID"]) +
                                        " - Account ID : " + Util.GetValueOfInt(drDataRecord["Account_ID"]));
                }
            }

            return _listBudgetControl;
        }

        /// <summary>
        /// This Function is used to get those dimension which is not to controlled
        /// </summary>
        /// <param name="selectedDimension">controlled dimension</param>
        /// <returns>where Condition</returns>
        public String GetNonSelectedDimension(List<String> selectedDimension)
        {
            String where = "";
            String sql = @" SELECT AD_Ref_List.Value FROM AD_Reference INNER JOIN AD_Ref_List ON AD_Ref_List.AD_Reference_ID = AD_Reference.AD_Reference_ID 
                            WHERE  AD_Reference.AD_Reference_ID=181 AND AD_Ref_List.Value NOT IN ('AC' , 'SA')";
            DataSet dsElementType = DB.ExecuteDataset(sql, null, null);
            if (dsElementType != null && dsElementType.Tables.Count > 0)
            {
                for (int i = 0; i < dsElementType.Tables[0].Rows.Count; i++)
                {
                    if (Util.GetValueOfString(dsElementType.Tables[0].Rows[i]["Value"]).Equals(X_C_AcctSchema_Element.ELEMENTTYPE_BPartner)
                        && !selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_BPartner))
                    {
                        where += " AND NVL(C_BPartner_ID, 0) = 0 ";
                    }
                    else if (Util.GetValueOfString(dsElementType.Tables[0].Rows[i]["Value"]).Equals(X_C_AcctSchema_Element.ELEMENTTYPE_Product)
                        && !selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Product))
                    {
                        where += " AND NVL(M_Product_ID, 0) = 0 ";
                    }
                    else if (Util.GetValueOfString(dsElementType.Tables[0].Rows[i]["Value"]).Equals(X_C_AcctSchema_Element.ELEMENTTYPE_Activity)
                        && !selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Activity))
                    {
                        where += " AND NVL(C_Activity_ID, 0) = 0 ";
                    }
                    else if (Util.GetValueOfString(dsElementType.Tables[0].Rows[i]["Value"]).Equals(X_C_AcctSchema_Element.ELEMENTTYPE_LocationFrom)
                        && !selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_LocationFrom))
                    {
                        where += " AND NVL(C_LocFrom_ID, 0) = 0 ";
                    }
                    else if (Util.GetValueOfString(dsElementType.Tables[0].Rows[i]["Value"]).Equals(X_C_AcctSchema_Element.ELEMENTTYPE_LocationTo)
                       && !selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_LocationTo))
                    {
                        where += " AND NVL(C_LocTo_ID, 0) = 0 ";
                    }
                    else if (Util.GetValueOfString(dsElementType.Tables[0].Rows[i]["Value"]).Equals(X_C_AcctSchema_Element.ELEMENTTYPE_Campaign)
                      && !selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Campaign))
                    {
                        where += " AND NVL(C_Campaign_ID, 0) = 0 ";
                    }
                    else if (Util.GetValueOfString(dsElementType.Tables[0].Rows[i]["Value"]).Equals(X_C_AcctSchema_Element.ELEMENTTYPE_OrgTrx)
                      && !selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_OrgTrx))
                    {
                        where += " AND NVL(AD_OrgTrx_ID, 0) = 0 ";
                    }
                    else if (Util.GetValueOfString(dsElementType.Tables[0].Rows[i]["Value"]).Equals(X_C_AcctSchema_Element.ELEMENTTYPE_Project)
                       && !selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_Project))
                    {
                        where += " AND NVL(C_Project_ID, 0) = 0 ";
                    }
                    else if (Util.GetValueOfString(dsElementType.Tables[0].Rows[i]["Value"]).Equals(X_C_AcctSchema_Element.ELEMENTTYPE_SalesRegion)
                       && !selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_SalesRegion))
                    {
                        where += " AND NVL(C_SalesRegion_ID, 0) = 0 ";
                    }
                    else if (Util.GetValueOfString(dsElementType.Tables[0].Rows[i]["Value"]).Equals(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement1)
                       && !selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement1))
                    {
                        where += " AND NVL(UserElement1_ID, 0) = 0 ";
                    }
                    else if (Util.GetValueOfString(dsElementType.Tables[0].Rows[i]["Value"]).Equals(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement2)
                       && !selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement2))
                    {
                        where += " AND NVL(UserElement2_ID, 0) = 0 ";
                    }
                    else if (Util.GetValueOfString(dsElementType.Tables[0].Rows[i]["Value"]).Equals(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement3)
                       && !selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement3))
                    {
                        where += " AND NVL(UserElement3_ID, 0) = 0 ";
                    }
                    else if (Util.GetValueOfString(dsElementType.Tables[0].Rows[i]["Value"]).Equals(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement4)
                       && !selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement4))
                    {
                        where += " AND NVL(UserElement4_ID, 0) = 0 ";
                    }
                    else if (Util.GetValueOfString(dsElementType.Tables[0].Rows[i]["Value"]).Equals(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement5)
                       && !selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement5))
                    {
                        where += " AND NVL(UserElement5_ID, 0) = 0 ";
                    }
                    else if (Util.GetValueOfString(dsElementType.Tables[0].Rows[i]["Value"]).Equals(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement6)
                       && !selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement6))
                    {
                        where += " AND NVL(UserElement6_ID, 0) = 0 ";
                    }
                    else if (Util.GetValueOfString(dsElementType.Tables[0].Rows[i]["Value"]).Equals(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement7)
                       && !selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement7))
                    {
                        where += " AND NVL(UserElement7_ID, 0) = 0 ";
                    }
                    else if (Util.GetValueOfString(dsElementType.Tables[0].Rows[i]["Value"]).Equals(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement8)
                       && !selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement8))
                    {
                        where += " AND NVL(UserElement8_ID, 0) = 0 ";
                    }
                    else if (Util.GetValueOfString(dsElementType.Tables[0].Rows[i]["Value"]).Equals(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement9)
                       && !selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserElement9))
                    {
                        where += " AND NVL(UserElement9_ID, 0) = 0 ";
                    }
                    else if (Util.GetValueOfString(dsElementType.Tables[0].Rows[i]["Value"]).Equals(X_C_AcctSchema_Element.ELEMENTTYPE_UserList1)
                       && !selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserList1))
                    {
                        where += " AND NVL(User1_ID, 0) = 0 ";
                    }
                    else if (Util.GetValueOfString(dsElementType.Tables[0].Rows[i]["Value"]).Equals(X_C_AcctSchema_Element.ELEMENTTYPE_UserList2)
                       && !selectedDimension.Contains(X_C_AcctSchema_Element.ELEMENTTYPE_UserList2))
                    {
                        where += " AND NVL(User2_ID, 0) = 0 ";
                    }
                }
            }
            return where;
        }

        /// <summary>
        /// This function is used to create default record
        /// </summary>
        /// <param name="budget_id"></param>
        /// <param name="budgetControl_Id"></param>
        /// <param name="acctSchema_ID"></param>
        /// <param name="account_id"></param>
        public void CheckOrCreateDefault(int budget_id, int budgetControl_Id, int acctSchema_ID, int account_id, List<BudgetControl> _listBudgetControl)
        {
            BudgetControl budgetControl = null;
            if (!_listBudgetControl.Exists(x => (x.GL_Budget_ID == budget_id) &&
                                             (x.GL_BudgetControl_ID == budgetControl_Id) &&
                                             (x.Account_ID == account_id) &&
                                             (x.AD_Org_ID == 0) &&
                                             (x.C_BPartner_ID == 0) &&
                                             (x.M_Product_ID == 0) &&
                                             (x.C_Activity_ID == 0) &&
                                             (x.C_LocationFrom_ID == 0) &&
                                             (x.C_LocationTo_ID == 0) &&
                                             (x.C_Campaign_ID == 0) &&
                                             (x.AD_OrgTrx_ID == 0) &&
                                             (x.C_Project_ID == 0) &&
                                             (x.C_SalesRegion_ID == 0) &&
                                             (x.UserList1_ID == 0) &&
                                             (x.UserList2_ID == 0) &&
                                             (x.UserElement1_ID == 0) &&
                                             (x.UserElement2_ID == 0) &&
                                             (x.UserElement3_ID == 0) &&
                                             (x.UserElement4_ID == 0) &&
                                             (x.UserElement5_ID == 0) &&
                                             (x.UserElement6_ID == 0) &&
                                             (x.UserElement7_ID == 0) &&
                                             (x.UserElement8_ID == 0) &&
                                             (x.UserElement9_ID == 0)
                                            ))
            {
                budgetControl = new BudgetControl();
                budgetControl.GL_Budget_ID = budget_id;
                budgetControl.GL_BudgetControl_ID = budgetControl_Id;
                budgetControl.C_AcctSchema_ID = acctSchema_ID;
                budgetControl.Account_ID = account_id;
                budgetControl.AD_Org_ID = 0;
                budgetControl.M_Product_ID = 0;
                budgetControl.C_BPartner_ID = 0;
                budgetControl.C_Activity_ID = 0;
                budgetControl.C_LocationFrom_ID = 0;
                budgetControl.C_LocationTo_ID = 0;
                budgetControl.C_Campaign_ID = 0;
                budgetControl.AD_OrgTrx_ID = 0;
                budgetControl.C_Project_ID = 0;
                budgetControl.C_SalesRegion_ID = 0;
                budgetControl.UserList1_ID = 0;
                budgetControl.UserList2_ID = 0;
                budgetControl.UserElement1_ID = 0;
                budgetControl.UserElement2_ID = 0;
                budgetControl.UserElement3_ID = 0;
                budgetControl.UserElement4_ID = 0;
                budgetControl.UserElement5_ID = 0;
                budgetControl.UserElement6_ID = 0;
                budgetControl.UserElement7_ID = 0;
                budgetControl.UserElement8_ID = 0;
                budgetControl.UserElement9_ID = 0;
                budgetControl.ControlledAmount = 0;
                _listBudgetControl.Add(budgetControl);
            }
        }

        /// <summary>
        /// Set the document number from Completed Document Sequence after completed
        /// </summary>
        protected void SetCompletedDocumentNo()
        {
            // if Re-Activated document then no need to get Document no from Completed sequence
            if (Get_ColumnIndex("IsReActivated") >= 0 && IsReActivated())
            {
                return;
            }

            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());

            // if Overwrite Sequence on Complete checkbox is true.
            if (dt.IsOverwriteSeqOnComplete())
            {
                // Set Drafted Document No into Temp Document No.
                if (Get_ColumnIndex("TempDocumentNo") >= 0)
                {
                    SetTempDocumentNo(GetDocumentNo());
                }

                // Get current next from Completed document sequence defined on Document type
                String value = MSequence.GetDocumentNo(GetC_DocType_ID(), Get_TrxName(), GetCtx(), true, this);
                if (value != null)
                {
                    SetDocumentNo(value);
                }
            }
        }

        /// <summary>
        /// Overwrite the document date based on setting on Document Type
        /// </summary>
        private void SetCompletedDocumentDate()
        {
            // if Re-Activated document then no need to get Document no from Completed sequence
            if (Get_ColumnIndex("IsReActivated") >= 0 && IsReActivated())
            {
                return;
            }

            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());

            // if Overwrite Date on Complete checkbox is true.
            if (dt.IsOverwriteDateOnComplete())
            {
                SetDateOrdered(DateTime.Now.Date);
                if (GetDateAcct().Value.Date < GetDateOrdered().Value.Date)
                {
                    SetDateAcct(GetDateOrdered());

                    //	Std Period open?
                    if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), dt.GetDocBaseType(), GetAD_Org_ID()))
                    {
                        throw new Exception("@PeriodClosed@");
                    }
                }
            }
        }

        /// <summary>
        /// Create Exepected Landed cost distribution lines
        /// </summary>
        /// <returns>if success then empty string else message</returns>
        protected String ExpectedlandedCostDistribution()
        {
            MExpectedCost[] expectedlandedCosts = MExpectedCost.GetLines(GetCtx(), GetC_Order_ID(), Get_Trx());
            if (expectedlandedCosts != null && expectedlandedCosts.Length > 0)
            {
                for (int i = 0; i < expectedlandedCosts.Length; i++)
                {
                    String error = expectedlandedCosts[i].DistributeLandedCost();
                    if (!Util.IsEmpty(error))
                        return error;
                }
            }
            return "";
        }

        //Changes by abhishek suggested by lokesh on 7/1/2016
        //public void SaveDayEndRecord(Ctx ctx, int Terminal_ID, string CashAmt, string CreditAmt, int DocType_ID, string[] Tax_ID, string[] TaxAmts, string OrderTotal, string[] DiscountLine)
        //{
        //    int DayEnd_ID = Util.GetValueOfInt(DB.ExecuteScalar("select vapos_dayendreport_id from vapos_dayendreport where to_date(vapos_trxdate, 'DD-MM-YYYY') =to_date(sysdate, 'DD-MM-YYYY') and vapos_posterminal_id=" + Terminal_ID));
        //    if (DayEnd_ID > 0)
        //    {
        //        // Addition in existing record
        //        ViennaAdvantage.Model.MVAPOSDayEndReport DayEndRep = new ViennaAdvantage.Model.MVAPOSDayEndReport(ctx, DayEnd_ID, null);
        //        if (Util.GetValueOfDecimal(CashAmt) > 0)
        //        {
        //            DayEndRep.SetVAPOS_CashPaymrnt(Decimal.Add(DayEndRep.GetVAPOS_CashPaymrnt(), Util.GetValueOfDecimal(CashAmt)));
        //            DayEndRep.SetVAPOS_PayGrandTotal(Decimal.Add(DayEndRep.GetVAPOS_PayGrandTotal(), Util.GetValueOfDecimal(CashAmt)));
        //        }
        //        if (Util.GetValueOfDecimal(CreditAmt) > 0)
        //        {
        //            DayEndRep.SetVAPOS_CreditCardPay(Decimal.Add(DayEndRep.GetVAPOS_CreditCardPay(), Util.GetValueOfDecimal(CreditAmt)));
        //            DayEndRep.SetVAPOS_PayGrandTotal(Decimal.Add(DayEndRep.GetVAPOS_PayGrandTotal(), Util.GetValueOfDecimal(CreditAmt)));
        //        }
        //        string OrderType = Util.GetValueOfString(DB.ExecuteScalar("select vapos_ordertype from c_doctype where c_doctype_id=" + DocType_ID));
        //        if (OrderType == "H")
        //        {
        //            DayEndRep.SetVAPOS_HmeDelivery(Decimal.Add(DayEndRep.GetVAPOS_HmeDelivery(), Util.GetValueOfDecimal(OrderTotal)));

        //        }
        //        else if (OrderType == "P")
        //        {
        //            DayEndRep.SetVAPOS_PickOrder(Decimal.Add(DayEndRep.GetVAPOS_PickOrder(), Util.GetValueOfDecimal(OrderTotal)));
        //        }
        //        else if (OrderType == "R")
        //        {
        //            DayEndRep.SetVAPOS_Return(Decimal.Add(DayEndRep.GetVAPOS_Return(), Util.GetValueOfDecimal(OrderTotal)));
        //        }
        //        else if (OrderType == "W")
        //        {
        //            DayEndRep.SetVAPOS_WHOrder(Decimal.Add(DayEndRep.GetVAPOS_WHOrder(), Util.GetValueOfDecimal(OrderTotal)));
        //        }
        //        DayEndRep.SetVAPOS_OrderGrandTotal(Decimal.Add(DayEndRep.GetVAPOS_OrderGrandTotal(), Util.GetValueOfDecimal(OrderTotal)));
        //        DayEndRep.SetVAPOS_SalesOSubTotal(Decimal.Add(DayEndRep.GetVAPOS_SalesOSubTotal(), Util.GetValueOfDecimal(OrderTotal)));
        //        DayEndRep.SetVAPOS_OSummaryGrandTot(Decimal.Add(DayEndRep.GetVAPOS_OSummaryGrandTot(), Util.GetValueOfDecimal(OrderTotal)));

        //        for (int i = 0; i < DiscountLine.Length; i++)
        //        {
        //            DayEndRep.SetVAPOS_Discount(Decimal.Add(DayEndRep.GetVAPOS_Discount(), Util.GetValueOfDecimal(DiscountLine[i])));
        //            DayEndRep.SetVAPOS_OSummaryGrandTot(Decimal.Add(DayEndRep.GetVAPOS_OSummaryGrandTot(), Util.GetValueOfDecimal(DiscountLine[i])));
        //        }
        //        if (DayEndRep.Save())
        //        {
        //            for (int j = 0; j < Tax_ID.Length; j++)
        //            {
        //                int DayEndTax_ID = Util.GetValueOfInt(DB.ExecuteScalar("select vapos_dayendreporttax_ID from vapos_dayendreporttax where vapos_dayendreport_id=" + DayEndRep.GetVAPOS_DayEndReport_ID() + " and c_tax_id=" + Util.GetValueOfInt(Tax_ID[j])));
        //                if (DayEndTax_ID > 0)
        //                {
        //                    ViennaAdvantage.Model.X_VAPOS_DayEndReportTax EndDayTax = new ViennaAdvantage.Model.X_VAPOS_DayEndReportTax(ctx, DayEndTax_ID, null);
        //                    EndDayTax.SetVAPOS_TaxAmount(Decimal.Add(EndDayTax.GetVAPOS_TaxAmount(), Util.GetValueOfDecimal(TaxAmts[j])));
        //                    if (EndDayTax.Save())
        //                    {

        //                    }
        //                }
        //                else
        //                {
        //                    ViennaAdvantage.Model.X_VAPOS_DayEndReportTax EndDayTax = new ViennaAdvantage.Model.X_VAPOS_DayEndReportTax(ctx, 0, null);
        //                    EndDayTax.SetVAPOS_DayEndReport_ID(DayEndRep.GetVAPOS_DayEndReport_ID());
        //                    EndDayTax.SetC_Tax_ID(Util.GetValueOfInt(Tax_ID[j]));
        //                    EndDayTax.SetVAPOS_TaxAmount(Util.GetValueOfDecimal(TaxAmts[j]));
        //                    if (EndDayTax.Save())
        //                    {

        //                    }
        //                }
        //            }

        //        }
        //    }
        //    else
        //    {
        //        //New Records On First Tab Of Day End
        //        ViennaAdvantage.Model.MVAPOSDayEndReport DayEndRep = new ViennaAdvantage.Model.MVAPOSDayEndReport(ctx, 0, null);
        //        DayEndRep.SetVAPOS_POSTerminal_ID(Util.GetValueOfInt(Terminal_ID));
        //        DayEndRep.SetVAPOS_TrxDate((DateTime.Now.ToLocalTime()));
        //        if (Util.GetValueOfDecimal(CashAmt) > 0)
        //        {
        //            DayEndRep.SetVAPOS_CashPaymrnt(Util.GetValueOfDecimal(CashAmt));
        //            DayEndRep.SetVAPOS_PayGrandTotal(Decimal.Add(DayEndRep.GetVAPOS_PayGrandTotal(), Util.GetValueOfDecimal(CashAmt)));
        //        }
        //        if (Util.GetValueOfDecimal(CreditAmt) > 0)
        //        {
        //            DayEndRep.SetVAPOS_CreditCardPay(Util.GetValueOfDecimal(CreditAmt));
        //            DayEndRep.SetVAPOS_PayGrandTotal(Decimal.Add(DayEndRep.GetVAPOS_PayGrandTotal(), Util.GetValueOfDecimal(CreditAmt)));
        //        }
        //        string OrderType = Util.GetValueOfString(DB.ExecuteScalar("select vapos_ordertype from c_doctype where c_doctype_id=" + DocType_ID));
        //        if (OrderType == "H")
        //        {
        //            DayEndRep.SetVAPOS_HmeDelivery(Util.GetValueOfDecimal(OrderTotal));
        //        }
        //        else if (OrderType == "P")
        //        {
        //            DayEndRep.SetVAPOS_PickOrder(Util.GetValueOfDecimal(OrderTotal));
        //        }
        //        else if (OrderType == "R")
        //        {
        //            DayEndRep.SetVAPOS_Return(Util.GetValueOfDecimal(OrderTotal));
        //        }
        //        else if (OrderType == "W")
        //        {
        //            DayEndRep.SetVAPOS_WHOrder(Util.GetValueOfDecimal(OrderTotal));
        //        }
        //        DayEndRep.SetVAPOS_OrderGrandTotal(Decimal.Add(DayEndRep.GetVAPOS_OrderGrandTotal(), Util.GetValueOfDecimal(OrderTotal)));
        //        DayEndRep.SetVAPOS_SalesOSubTotal(Util.GetValueOfDecimal(OrderTotal));
        //        DayEndRep.SetVAPOS_OSummaryGrandTot(Decimal.Add(DayEndRep.GetVAPOS_OSummaryGrandTot(), Util.GetValueOfDecimal(OrderTotal)));
        //        for (int i = 0; i < DiscountLine.Length; i++)
        //        {
        //            DayEndRep.SetVAPOS_Discount(Decimal.Add(DayEndRep.GetVAPOS_Discount(), Util.GetValueOfDecimal(DiscountLine[i])));
        //            DayEndRep.SetVAPOS_OSummaryGrandTot(Decimal.Add(DayEndRep.GetVAPOS_OSummaryGrandTot(), Util.GetValueOfDecimal(DiscountLine[i])));
        //        }
        //        if (DayEndRep.Save())
        //        {
        //            for (int j = 0; j < Tax_ID.Length; j++)
        //            {
        //                int DayEndTax_ID = Util.GetValueOfInt(DB.ExecuteScalar("select vapos_dayendreporttax_ID from vapos_dayendreporttax where vapos_dayendreport_id=" + DayEndRep.GetVAPOS_DayEndReport_ID() + " and c_tax_id=" + Tax_ID[j]));
        //                if (DayEndTax_ID > 0)
        //                {
        //                    ViennaAdvantage.Model.X_VAPOS_DayEndReportTax EndDayTax = new ViennaAdvantage.Model.X_VAPOS_DayEndReportTax(ctx, DayEndTax_ID, null);
        //                    EndDayTax.SetVAPOS_TaxAmount(Decimal.Add(EndDayTax.GetVAPOS_TaxAmount(), Util.GetValueOfDecimal(TaxAmts[j])));
        //                    if (EndDayTax.Save())
        //                    {
        //                    }
        //                }
        //                else
        //                {
        //                    ViennaAdvantage.Model.X_VAPOS_DayEndReportTax EndDayTax = new ViennaAdvantage.Model.X_VAPOS_DayEndReportTax(ctx, 0, null);
        //                    EndDayTax.SetVAPOS_DayEndReport_ID(DayEndRep.GetVAPOS_DayEndReport_ID());
        //                    EndDayTax.SetC_Tax_ID(Util.GetValueOfInt(Tax_ID[j]));
        //                    EndDayTax.SetVAPOS_TaxAmount(Util.GetValueOfDecimal(TaxAmts[j]));
        //                    if (EndDayTax.Save())
        //                    {
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        //Changes by abhishek suggested by lokesh on 7/1/2016
        //public void SaveDayEndRecord(Ctx ctx, int Terminal_ID, string CashAmt, string CreditAmt, int DocType_ID, string[] Tax_ID, string[] TaxAmts, string OrderTotal, string[] DiscountLine)
        //{
        //    MTable tblDayEnd = new MTable(ctx, Util.GetValueOfInt(DB.ExecuteScalar("select AD_Table_ID from ad_table where  tablename = 'VAPOS_DayEndReport'")), null);

        //    int DayEnd_ID = Util.GetValueOfInt(DB.ExecuteScalar("select vapos_dayendreport_id from vapos_dayendreport where to_date(vapos_trxdate, 'DD-MM-YYYY') =to_date(sysdate, 'DD-MM-YYYY') and vapos_posterminal_id=" + Terminal_ID));
        //    if (DayEnd_ID > 0)
        //    {
        //        PO poDayEnd = tblDayEnd.GetPO(ctx, DayEnd_ID, null);
        //        if (Math.Abs(Util.GetValueOfDecimal(CashAmt)) > 0)
        //        {
        //            //DayEndRep.SetVAPOS_CashPaymrnt(Decimal.Add(DayEndRep.GetVAPOS_CashPaymrnt(), Util.GetValueOfDecimal(CashAmt)));
        //            //DayEndRep.SetVAPOS_PayGrandTotal(Decimal.Add(DayEndRep.GetVAPOS_PayGrandTotal(), Util.GetValueOfDecimal(CashAmt)));
        //            poDayEnd.Set_Value("VAPOS_CashPaymrnt", Decimal.Add(Util.GetValueOfDecimal(poDayEnd.Get_Value("VAPOS_CashPaymrnt")), Util.GetValueOfDecimal(CashAmt)));
        //            poDayEnd.Set_Value("VAPOS_PayGrandTotal", Decimal.Add(Util.GetValueOfDecimal(poDayEnd.Get_Value("VAPOS_PayGrandTotal")), Util.GetValueOfDecimal(CashAmt)));
        //        }
        //        if (Util.GetValueOfDecimal(CreditAmt) > 0)
        //        {
        //            //DayEndRep.SetVAPOS_CreditCardPay(Decimal.Add(DayEndRep.GetVAPOS_CreditCardPay(), Util.GetValueOfDecimal(CreditAmt)));
        //            //DayEndRep.SetVAPOS_PayGrandTotal(Decimal.Add(DayEndRep.GetVAPOS_PayGrandTotal(), Util.GetValueOfDecimal(CreditAmt)));
        //            poDayEnd.Set_Value("VAPOS_CreditCardPay", Decimal.Add(Util.GetValueOfDecimal(poDayEnd.Get_Value("VAPOS_CreditCardPay")), Util.GetValueOfDecimal(CreditAmt)));
        //            poDayEnd.Set_Value("VAPOS_PayGrandTotal", Decimal.Add(Util.GetValueOfDecimal(poDayEnd.Get_Value("VAPOS_PayGrandTotal")), Util.GetValueOfDecimal(CreditAmt)));
        //        }
        //        string OrderType = Util.GetValueOfString(DB.ExecuteScalar("select vapos_ordertype from c_doctype where c_doctype_id=" + DocType_ID));
        //        if (OrderType == "H")
        //        {
        //            //DayEndRep.SetVAPOS_HmeDelivery(Decimal.Add(DayEndRep.GetVAPOS_HmeDelivery(), Util.GetValueOfDecimal(OrderTotal)));
        //            poDayEnd.Set_Value("VAPOS_HmeDelivery", Decimal.Add(Util.GetValueOfDecimal(poDayEnd.Get_Value("VAPOS_HmeDelivery")), Util.GetValueOfDecimal(OrderTotal)));

        //        }
        //        else if (OrderType == "P")
        //        {
        //            //DayEndRep.SetVAPOS_PickOrder(Decimal.Add(DayEndRep.GetVAPOS_PickOrder(), Util.GetValueOfDecimal(OrderTotal)));
        //            poDayEnd.Set_Value("VAPOS_PickOrder", Decimal.Add(Util.GetValueOfDecimal(poDayEnd.Get_Value("VAPOS_PickOrder")), Util.GetValueOfDecimal(OrderTotal)));
        //        }
        //        else if (OrderType == "R")
        //        {
        //            //DayEndRep.SetVAPOS_Return(Decimal.Add(DayEndRep.GetVAPOS_Return(), Util.GetValueOfDecimal(OrderTotal)));
        //            poDayEnd.Set_Value("VAPOS_Return", Decimal.Add(Util.GetValueOfDecimal(poDayEnd.Get_Value("VAPOS_Return")), Util.GetValueOfDecimal(OrderTotal)));
        //        }
        //        else if (OrderType == "W")
        //        {
        //            //DayEndRep.SetVAPOS_WHOrder(Decimal.Add(DayEndRep.GetVAPOS_WHOrder(), Util.GetValueOfDecimal(OrderTotal)));
        //            poDayEnd.Set_Value("VAPOS_WHOrder", Decimal.Add(Util.GetValueOfDecimal(poDayEnd.Get_Value("VAPOS_WHOrder")), Util.GetValueOfDecimal(OrderTotal)));
        //        }
        //        //DayEndRep.SetVAPOS_OrderGrandTotal(Decimal.Add(DayEndRep.GetVAPOS_OrderGrandTotal(), Util.GetValueOfDecimal(OrderTotal)));
        //        //DayEndRep.SetVAPOS_SalesOSubTotal(Decimal.Add(DayEndRep.GetVAPOS_SalesOSubTotal(), Util.GetValueOfDecimal(OrderTotal)));
        //        //DayEndRep.SetVAPOS_OSummaryGrandTot(Decimal.Add(DayEndRep.GetVAPOS_OSummaryGrandTot(), Util.GetValueOfDecimal(OrderTotal)));
        //        poDayEnd.Set_Value("VAPOS_OrderGrandTotal", Decimal.Add(Util.GetValueOfDecimal(poDayEnd.Get_Value("VAPOS_OrderGrandTotal")), Util.GetValueOfDecimal(OrderTotal)));
        //        poDayEnd.Set_Value("VAPOS_SalesOSubTotal", Decimal.Add(Util.GetValueOfDecimal(poDayEnd.Get_Value("VAPOS_SalesOSubTotal")), Util.GetValueOfDecimal(OrderTotal)));
        //        poDayEnd.Set_Value("VAPOS_OSummaryGrandTot", Decimal.Add(Util.GetValueOfDecimal(poDayEnd.Get_Value("VAPOS_OSummaryGrandTot")), Util.GetValueOfDecimal(OrderTotal)));

        //        for (int i = 0; i < DiscountLine.Length; i++)
        //        {
        //            //DayEndRep.SetVAPOS_Discount(Decimal.Add(DayEndRep.GetVAPOS_Discount(), Util.GetValueOfDecimal(DiscountLine[i])));
        //            //DayEndRep.SetVAPOS_OSummaryGrandTot(Decimal.Add(DayEndRep.GetVAPOS_OSummaryGrandTot(), Util.GetValueOfDecimal(DiscountLine[i])));
        //            poDayEnd.Set_Value("VAPOS_Discount", Decimal.Add(Util.GetValueOfDecimal(poDayEnd.Get_Value("VAPOS_Discount")), Util.GetValueOfDecimal(DiscountLine[i])));
        //            poDayEnd.Set_Value("VAPOS_OSummaryGrandTot", Decimal.Add(Util.GetValueOfDecimal(poDayEnd.Get_Value("VAPOS_OSummaryGrandTot")), Util.GetValueOfDecimal(DiscountLine[i])));
        //        }
        //        if (poDayEnd.Save())
        //        {
        //            for (int j = 0; j < Tax_ID.Length; j++)
        //            {
        //                MTable tblTax = new MTable(ctx, Util.GetValueOfInt(DB.ExecuteScalar("select AD_Table_ID from ad_table where  tablename = 'VAPOS_DayEndReportTax'")), null);
        //                int DayEndTax_ID = Util.GetValueOfInt(DB.ExecuteScalar("select vapos_dayendreporttax_ID from vapos_dayendreporttax where vapos_dayendreport_id=" + Util.GetValueOfInt(poDayEnd.Get_Value("VAPOS_DayEndReport_ID")) + " and c_tax_id=" + Util.GetValueOfInt(Tax_ID[j])));
        //                if (DayEndTax_ID > 0)
        //                {
        //                    //ViennaAdvantage.Model.X_VAPOS_DayEndReportTax EndDayTax = new ViennaAdvantage.Model.X_VAPOS_DayEndReportTax(ctx, DayEndTax_ID, null);
        //                    //EndDayTax.SetVAPOS_TaxAmount(Decimal.Add(EndDayTax.GetVAPOS_TaxAmount(), Util.GetValueOfDecimal(TaxAmts[j])));
        //                    PO poDayEndTax = tblDayEnd.GetPO(ctx, DayEndTax_ID, null);
        //                    poDayEndTax.Set_Value("VAPOS_TaxAmount", Decimal.Add(Util.GetValueOfDecimal(poDayEndTax.Get_Value("VAPOS_TaxAmount")), Util.GetValueOfDecimal(TaxAmts[j])));
        //                    if (poDayEndTax.Save())
        //                    {

        //                    }
        //                }
        //                else
        //                {
        //                    //ViennaAdvantage.Model.X_VAPOS_DayEndReportTax EndDayTax = new ViennaAdvantage.Model.X_VAPOS_DayEndReportTax(ctx, 0, null);
        //                    //EndDayTax.SetVAPOS_DayEndReport_ID(DayEndRep.GetVAPOS_DayEndReport_ID());
        //                    //EndDayTax.SetC_Tax_ID(Util.GetValueOfInt(Tax_ID[j]));
        //                    //EndDayTax.SetVAPOS_TaxAmount(Util.GetValueOfDecimal(TaxAmts[j]));
        //                    PO poDayEndTax = tblDayEnd.GetPO(ctx, 0, null);
        //                    poDayEndTax.Set_Value("VAPOS_TaxAmount", Util.GetValueOfDecimal(TaxAmts[j]));
        //                    poDayEndTax.Set_Value("VAPOS_DayEndReport_ID", poDayEnd.Get_Value("VAPOS_DayEndReport_ID"));
        //                    poDayEndTax.Set_Value("C_Tax_ID", Util.GetValueOfInt(Tax_ID[j]));

        //                    if (poDayEndTax.Save())
        //                    {

        //                    }
        //                }
        //            }

        //        }
        //        else
        //        {
        //            poDayEnd = tblDayEnd.GetPO(ctx, 0, null);
        //            poDayEnd.Set_Value("VAPOS_POSTerminal_ID", Util.GetValueOfInt(Terminal_ID));
        //            poDayEnd.Set_Value("VAPOS_TrxDate", (DateTime.Now.ToLocalTime()));
        //            if (Math.Abs(Util.GetValueOfDecimal(CashAmt)) > 0)
        //            {
        //                //DayEndRep.SetVAPOS_CashPaymrnt(Decimal.Add(DayEndRep.GetVAPOS_CashPaymrnt(), Util.GetValueOfDecimal(CashAmt)));
        //                //DayEndRep.SetVAPOS_PayGrandTotal(Decimal.Add(DayEndRep.GetVAPOS_PayGrandTotal(), Util.GetValueOfDecimal(CashAmt)));
        //                poDayEnd.Set_Value("VAPOS_CashPaymrnt", Util.GetValueOfDecimal(CashAmt));
        //                poDayEnd.Set_Value("VAPOS_PayGrandTotal", Decimal.Add(Util.GetValueOfDecimal(poDayEnd.Get_Value("VAPOS_PayGrandTotal")), Util.GetValueOfDecimal(CashAmt)));
        //            }
        //            if (Util.GetValueOfDecimal(CreditAmt) > 0)
        //            {
        //                //DayEndRep.SetVAPOS_CreditCardPay(Decimal.Add(DayEndRep.GetVAPOS_CreditCardPay(), Util.GetValueOfDecimal(CreditAmt)));
        //                //DayEndRep.SetVAPOS_PayGrandTotal(Decimal.Add(DayEndRep.GetVAPOS_PayGrandTotal(), Util.GetValueOfDecimal(CreditAmt)));
        //                poDayEnd.Set_Value("VAPOS_CreditCardPay", Util.GetValueOfDecimal(CreditAmt));
        //                poDayEnd.Set_Value("VAPOS_PayGrandTotal", Decimal.Add(Util.GetValueOfDecimal(poDayEnd.Get_Value("VAPOS_PayGrandTotal")), Util.GetValueOfDecimal(CreditAmt)));
        //            }
        //            OrderType = Util.GetValueOfString(DB.ExecuteScalar("select vapos_ordertype from c_doctype where c_doctype_id=" + DocType_ID));
        //            if (OrderType == "H")
        //            {
        //                //DayEndRep.SetVAPOS_HmeDelivery(Decimal.Add(DayEndRep.GetVAPOS_HmeDelivery(), Util.GetValueOfDecimal(OrderTotal)));
        //                poDayEnd.Set_Value("VAPOS_HmeDelivery", Util.GetValueOfDecimal(OrderTotal));

        //            }
        //            else if (OrderType == "P")
        //            {
        //                //DayEndRep.SetVAPOS_PickOrder(Decimal.Add(DayEndRep.GetVAPOS_PickOrder(), Util.GetValueOfDecimal(OrderTotal)));
        //                poDayEnd.Set_Value("VAPOS_PickOrder", Util.GetValueOfDecimal(OrderTotal));
        //            }
        //            else if (OrderType == "R")
        //            {
        //                //DayEndRep.SetVAPOS_Return(Decimal.Add(DayEndRep.GetVAPOS_Return(), Util.GetValueOfDecimal(OrderTotal)));
        //                poDayEnd.Set_Value("VAPOS_Return", Util.GetValueOfDecimal(OrderTotal));
        //            }
        //            else if (OrderType == "W")
        //            {
        //                //DayEndRep.SetVAPOS_WHOrder(Decimal.Add(DayEndRep.GetVAPOS_WHOrder(), Util.GetValueOfDecimal(OrderTotal)));
        //                poDayEnd.Set_Value("VAPOS_WHOrder", Decimal.Add(Util.GetValueOfDecimal(poDayEnd.Get_Value("VAPOS_WHOrder")), Util.GetValueOfDecimal(OrderTotal)));
        //            }
        //            //DayEndRep.SetVAPOS_OrderGrandTotal(Decimal.Add(DayEndRep.GetVAPOS_OrderGrandTotal(), Util.GetValueOfDecimal(OrderTotal)));
        //            //DayEndRep.SetVAPOS_SalesOSubTotal(Decimal.Add(DayEndRep.GetVAPOS_SalesOSubTotal(), Util.GetValueOfDecimal(OrderTotal)));
        //            //DayEndRep.SetVAPOS_OSummaryGrandTot(Decimal.Add(DayEndRep.GetVAPOS_OSummaryGrandTot(), Util.GetValueOfDecimal(OrderTotal)));
        //            poDayEnd.Set_Value("VAPOS_OrderGrandTotal", Decimal.Add(Util.GetValueOfDecimal(poDayEnd.Get_Value("VAPOS_OrderGrandTotal")), Util.GetValueOfDecimal(OrderTotal)));
        //            poDayEnd.Set_Value("VAPOS_SalesOSubTotal", Decimal.Add(Util.GetValueOfDecimal(poDayEnd.Get_Value("VAPOS_SalesOSubTotal")), Util.GetValueOfDecimal(OrderTotal)));
        //            poDayEnd.Set_Value("VAPOS_OSummaryGrandTot", Decimal.Add(Util.GetValueOfDecimal(poDayEnd.Get_Value("VAPOS_OSummaryGrandTot")), Util.GetValueOfDecimal(OrderTotal)));

        //            for (int i = 0; i < DiscountLine.Length; i++)
        //            {
        //                //DayEndRep.SetVAPOS_Discount(Decimal.Add(DayEndRep.GetVAPOS_Discount(), Util.GetValueOfDecimal(DiscountLine[i])));
        //                //DayEndRep.SetVAPOS_OSummaryGrandTot(Decimal.Add(DayEndRep.GetVAPOS_OSummaryGrandTot(), Util.GetValueOfDecimal(DiscountLine[i])));
        //                poDayEnd.Set_Value("VAPOS_Discount", Decimal.Add(Util.GetValueOfDecimal(poDayEnd.Get_Value("VAPOS_Discount")), Util.GetValueOfDecimal(DiscountLine[i])));
        //                poDayEnd.Set_Value("VAPOS_OSummaryGrandTot", Decimal.Add(Util.GetValueOfDecimal(poDayEnd.Get_Value("VAPOS_OSummaryGrandTot")), Util.GetValueOfDecimal(DiscountLine[i])));
        //            }
        //            if (poDayEnd.Save())
        //            {
        //                for (int j = 0; j < Tax_ID.Length; j++)
        //                {
        //                    MTable tblTax = new MTable(ctx, Util.GetValueOfInt(DB.ExecuteScalar("select AD_Table_ID from ad_table where  tablename = 'VAPOS_DayEndReportTax'")), null);
        //                    int DayEndTax_ID = Util.GetValueOfInt(DB.ExecuteScalar("select vapos_dayendreporttax_ID from vapos_dayendreporttax where vapos_dayendreport_id=" + Util.GetValueOfInt(poDayEnd.Get_Value("VAPOS_DayEndReport_ID")) + " and c_tax_id=" + Util.GetValueOfInt(Tax_ID[j])));
        //                    if (DayEndTax_ID > 0)
        //                    {
        //                        //ViennaAdvantage.Model.X_VAPOS_DayEndReportTax EndDayTax = new ViennaAdvantage.Model.X_VAPOS_DayEndReportTax(ctx, DayEndTax_ID, null);
        //                        //EndDayTax.SetVAPOS_TaxAmount(Decimal.Add(EndDayTax.GetVAPOS_TaxAmount(), Util.GetValueOfDecimal(TaxAmts[j])));
        //                        PO poDayEndTax = tblDayEnd.GetPO(ctx, DayEndTax_ID, null);
        //                        poDayEndTax.Set_Value("VAPOS_TaxAmount", Decimal.Add(Util.GetValueOfDecimal(poDayEndTax.Get_Value("VAPOS_TaxAmount")), Util.GetValueOfDecimal(TaxAmts[j])));
        //                        if (poDayEndTax.Save())
        //                        {

        //                        }
        //                    }
        //                    else
        //                    {
        //                        //ViennaAdvantage.Model.X_VAPOS_DayEndReportTax EndDayTax = new ViennaAdvantage.Model.X_VAPOS_DayEndReportTax(ctx, 0, null);
        //                        //EndDayTax.SetVAPOS_DayEndReport_ID(DayEndRep.GetVAPOS_DayEndReport_ID());
        //                        //EndDayTax.SetC_Tax_ID(Util.GetValueOfInt(Tax_ID[j]));
        //                        //EndDayTax.SetVAPOS_TaxAmount(Util.GetValueOfDecimal(TaxAmts[j]));
        //                        PO poDayEndTax = tblDayEnd.GetPO(ctx, 0, null);
        //                        poDayEndTax.Set_Value("VAPOS_TaxAmount", Util.GetValueOfDecimal(TaxAmts[j]));
        //                        poDayEndTax.Set_Value("VAPOS_DayEndReport_ID", poDayEnd.Get_Value("VAPOS_DayEndReport_ID"));
        //                        poDayEndTax.Set_Value("C_Tax_ID", Util.GetValueOfInt(Tax_ID[j]));

        //                        if (poDayEndTax.Save())
        //                        {

        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        /* 	Create Shipment
        *	@param dt order document type
        *	@param movementDate optional movement date (default today)
        *	@return shipment or null
        */
        private MInOut CreateShipment(MDocType dt, DateTime? movementDate)
        {
            MInOut shipment = new MInOut(this, (int)dt.GetC_DocTypeShipment_ID(), (DateTime?)movementDate);
            String DocSubTypeSO = dt.GetDocSubTypeSO();
            if (MDocType.DOCSUBTYPESO_POSOrder.Equals(DocSubTypeSO))
            {
                if (Util.GetValueOfInt(GetVAPOS_POSTerminal_ID()) != 0)
                {
                    shipment.SetDocumentNo(GetDocumentNo());
                }
            }
            log.Info("For " + dt);
            bool IsDrafted = false;
            try
            {
                //	shipment.setDateAcct(getDateAcct());
                if (!shipment.Save(Get_TrxName()))
                {
                    _processMsg = "Could not create Shipment";
                    return null;
                }
                //
                String posStatus = "";
                MWarehouse wh = null;
                MOrderLine[] oLines = GetLines(true, null);
                for (int i = 0; i < oLines.Length; i++)
                {

                    MOrderLine oLine = oLines[i];
                    if (Util.GetValueOfInt(GetVAPOS_POSTerminal_ID()) > 0)
                    {
                        #region POS Terminal > 0
                        MInOutLine ioLine = new MInOutLine(shipment);
                        //	Qty = Ordered - Delivered
                        Decimal MovementQty = Decimal.Subtract(oLine.GetQtyOrdered(), oLine.GetQtyDelivered());
                        //	Location
                        int M_Locator_ID = MStorage.GetM_Locator_ID(oLine.GetM_Warehouse_ID(),
                                oLine.GetM_Product_ID(), oLine.GetM_AttributeSetInstance_ID(),
                                MovementQty, Get_TrxName());
                        if (M_Locator_ID == 0)      //	Get default Location
                        {
                            MProduct product = ioLine.GetProduct();
                            int M_Warehouse_ID = oLine.GetM_Warehouse_ID();
                            M_Locator_ID = MProductLocator.GetFirstM_Locator_ID(product, M_Warehouse_ID);
                            if (M_Locator_ID == 0)
                            {
                                wh = MWarehouse.Get(GetCtx(), M_Warehouse_ID);
                                M_Locator_ID = wh.GetDefaultM_Locator_ID();
                            }
                        }
                        //
                        ioLine.SetOrderLine(oLine, M_Locator_ID, MovementQty);
                        ioLine.SetQty(MovementQty);
                        if (oLine.GetQtyEntered().CompareTo(oLine.GetQtyOrdered()) != 0)
                        {
                            ioLine.SetQtyEntered(Decimal.Multiply(MovementQty, (Decimal.Divide(oLine.GetQtyEntered(), (oLine.GetQtyOrdered())))));
                        }
                        if (!ioLine.Save(Get_TrxName()))
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null && !string.IsNullOrEmpty(pp.GetName()))
                                _processMsg = "Could not create Shipment Line. " + pp.GetName();
                            else
                                _processMsg = "Could not create Shipment Line";
                            return null;
                        }
                        #endregion
                    }
                    else
                    {
                        // when order line created with charge OR with Product which is not of "item type" then not to create shipment line against this.
                        MProduct oproduct = oLine.GetProduct();
                        string allowNonItem = Util.GetValueOfString(GetCtx().GetContext("$AllowNonItem"));

                        if (String.IsNullOrEmpty(allowNonItem))
                        {
                            allowNonItem = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsAllowNonItem FROM AD_Client WHERE AD_Client_ID = " + GetAD_Client_ID(), null, Get_Trx()));
                        }

                        //Create Lines for Charge / (Resource - Service - Expense) type product based on setting on Tenant to "Allow Non Item type".
                        if ((oproduct == null || !(oproduct != null && oproduct.GetProductType() == MProduct.PRODUCTTYPE_Item))
                            && (allowNonItem.Equals("N")))
                            continue;

                        //
                        int M_Warehouse_ID = oLine.GetM_Warehouse_ID();
                        wh = MWarehouse.Get(GetCtx(), M_Warehouse_ID);

                        MInOutLine ioLine = new MInOutLine(shipment);

                        //	Qty = Ordered - Delivered
                        Decimal MovementQty = Decimal.Subtract(oLine.GetQtyOrdered(), oLine.GetQtyDelivered());

                        //	Location
                        int M_Locator_ID = MStorage.GetM_Locator_ID(oLine.GetM_Warehouse_ID(),
                                oLine.GetM_Product_ID(), oLine.GetM_AttributeSetInstance_ID(),
                                MovementQty, Get_TrxName());
                        if (M_Locator_ID == 0)      //	Get default Location
                        {
                            MProduct product = ioLine.GetProduct();
                            M_Locator_ID = MProductLocator.GetFirstM_Locator_ID(product, M_Warehouse_ID);
                            if (M_Locator_ID == 0)
                            {
                                //wh = MWarehouse.Get(GetCtx(), M_Warehouse_ID);
                                M_Locator_ID = wh.GetDefaultM_Locator_ID();
                            }
                        }

                        Decimal? QtyAvail = MStorage.GetQtyAvailable(M_Warehouse_ID, oLine.GetM_Product_ID(), oLine.GetM_AttributeSetInstance_ID(), Get_Trx());
                        if (MovementQty > 0)
                            QtyAvail += MovementQty;

                        String sql = "SELECT SUM(QtyOnHand) FROM M_Storage s INNER JOIN M_Locator l ON (s.M_Locator_ID=l.M_Locator_ID) WHERE s.M_Product_ID=" + oLine.GetM_Product_ID() + " AND l.M_Warehouse_ID=" + M_Warehouse_ID;
                        if (oLine.GetM_AttributeSetInstance_ID() != 0)
                        {
                            sql += " AND M_AttributeSetInstance_ID=" + oLine.GetM_AttributeSetInstance_ID();
                        }
                        // check onhand qty on specified locator
                        if (M_Locator_ID > 0)
                        {
                            sql += " AND l.M_Locator_ID = " + M_Locator_ID;
                        }
                        OnHandQty = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_Trx()));
                        if (wh.IsDisallowNegativeInv())
                        {
                            if (oLine.GetQtyOrdered() > QtyAvail && (DocSubTypeSO == "WR" || DocSubTypeSO == "WP"))
                            {
                                #region In Case of -- WR (WareHouse Order) / WP (POS Order)
                                // when qty avialble on warehouse is less than qty ordered, at that time we can create shipment in Drafetd stage, to be completed mnually
                                _processMsg = CreateShipmentLineContainer(shipment, ioLine, oLine, M_Locator_ID, MovementQty, wh.IsDisallowNegativeInv(), oproduct);

                                // when OnHand qty is less than qtyOrdered then not to create shipment (need to be rollback)
                                if (!String.IsNullOrEmpty(_processMsg) && OnHandQty < oLine.GetQtyOrdered())
                                {
                                    return null;
                                }

                                //ioLine.SetOrderLine(oLine, M_Locator_ID, MovementQty);
                                //ioLine.SetQty(MovementQty);
                                //if (oLine.GetQtyEntered().CompareTo(oLine.GetQtyOrdered()) != 0)
                                //{
                                //    //ioLine.SetQtyEntered(Decimal.Multiply(MovementQty,(oLine.getQtyEntered()).divide(oLine.getQtyOrdered(), 6, Decimal.ROUND_HALF_UP));
                                //    ioLine.SetQtyEntered(Decimal.Multiply(MovementQty, (Decimal.Divide(oLine.GetQtyEntered(), (oLine.GetQtyOrdered())))));
                                //}
                                //if (!ioLine.Save(Get_TrxName()))
                                //{
                                //    //_processMsg = "Could not create Shipment Line";
                                //    //return null;
                                //}
                                //	Manually Process Shipment
                                IsDrafted = true;
                                #endregion
                            }
                            else
                            {
                                #region In Case of -- Credit Order / PePay Order / WareHouse Order / POS Order

                                _processMsg = CreateShipmentLineContainer(shipment, ioLine, oLine, M_Locator_ID, MovementQty, wh.IsDisallowNegativeInv(), oproduct);
                                if (!String.IsNullOrEmpty(_processMsg))
                                {
                                    return null;
                                }

                                //ioLine.SetOrderLine(oLine, M_Locator_ID, MovementQty);
                                //ioLine.SetQty(MovementQty);
                                //if (oLine.GetQtyEntered().CompareTo(oLine.GetQtyOrdered()) != 0)
                                //{
                                //    //ioLine.SetQtyEntered(Decimal.Multiply(MovementQty,(oLine.getQtyEntered()).divide(oLine.getQtyOrdered(), 6, Decimal.ROUND_HALF_UP));
                                //    ioLine.SetQtyEntered(Decimal.Multiply(MovementQty, (Decimal.Divide(oLine.GetQtyEntered(), (oLine.GetQtyOrdered())))));
                                //}
                                //if (!ioLine.Save(Get_TrxName()))
                                //{
                                //    ValueNamePair pp = VLogger.RetrieveError();
                                //    if (pp != null && !string.IsNullOrEmpty(pp.GetName()))
                                //        _processMsg = "Could not create Shipment Line. " + pp.GetName();
                                //    else
                                //        _processMsg = "Could not create Shipment Line";
                                //    return null;
                                //}
                                #endregion
                            }
                        }
                        else
                        {
                            #region when disallow is FALSE
                            _processMsg = CreateShipmentLineContainer(shipment, ioLine, oLine, M_Locator_ID, MovementQty, wh.IsDisallowNegativeInv(), oproduct);
                            if (!String.IsNullOrEmpty(_processMsg))
                            {
                                return null;
                            }

                            //ioLine.SetOrderLine(oLine, M_Locator_ID, MovementQty);
                            //ioLine.SetQty(MovementQty);
                            //if (oLine.GetQtyEntered().CompareTo(oLine.GetQtyOrdered()) != 0)
                            //{
                            //    //ioLine.SetQtyEntered(Decimal.Multiply(MovementQty,(oLine.getQtyEntered()).divide(oLine.getQtyOrdered(), 6, Decimal.ROUND_HALF_UP));
                            //    ioLine.SetQtyEntered(Decimal.Multiply(MovementQty, (Decimal.Divide(oLine.GetQtyEntered(), (oLine.GetQtyOrdered())))));
                            //}
                            //if (!ioLine.Save(Get_TrxName()))
                            //{
                            //    ValueNamePair pp = VLogger.RetrieveError();
                            //    if (pp != null && !string.IsNullOrEmpty(pp.GetName()))
                            //        _processMsg = "Could not create Shipment Line. " + pp.GetName();
                            //    else
                            //        _processMsg = "Could not create Shipment Line";
                            //    return null;
                            //}
                            #endregion
                        }
                    }
                }

                /// Change Here for Warehouse and Home Delivery Orders In case of POS Orders
                if (Util.GetValueOfInt(GetVAPOS_POSTerminal_ID()) > 0)
                {
                    if (MDocType.DOCSUBTYPESO_POSOrder.Equals(DocSubTypeSO))
                    {
                        //string sql = "SELECT COUNT(AD_Column_ID) FROM AD_Column WHERE AD_Table_ID = (SELECT AD_Table_ID FROM AD_Table WHERE TableName = 'C_DocType') AND ColumnName = 'VAPOS_OrderType'";
                        //int val = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                        //if (val > 0)
                        //sql = "SELECT VAPOS_OrderType FROM C_DocType WHERE C_DocType_ID = " + dt.GetC_DocType_ID();
                        //string oType = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_TrxName()));
                        if (dt.Get_ColumnIndex("VAPOS_OrderType") > -1)
                        {
                            string oType = Util.GetValueOfString(dt.Get_Value("VAPOS_OrderType"));
                            if (oType.Equals("H") || oType.Equals("W"))
                            {
                                posStatus = shipment.PrepareIt();
                                shipment.SetDocStatus(posStatus);
                                shipment.Save(Get_TrxName());
                                return shipment;
                            }
                        }
                    }
                }

                if (IsDrafted)
                {
                    String status = "DR";
                    shipment.SetDocStatus(status);
                    shipment.Save(Get_TrxName());
                    if (!DOCSTATUS_Drafted.Equals(status))
                    {
                        _processMsg = "@M_InOut_ID@: " + shipment.GetProcessMsg();
                        return null;
                    }
                }
                //	Manually Process Shipment
                else
                {
                    String statuss = shipment.CompleteIt();
                    shipment.SetDocStatus(statuss);
                    shipment.Save(Get_TrxName());
                    if (!DOCSTATUS_Completed.Equals(statuss))
                    {
                        _processMsg = "@M_InOut_ID@: " + shipment.GetProcessMsg();
                        return null;
                    }
                }
            }
            catch
            {
                return null;
                //ShowMessage.Error("MOrder",null,"CreateShipment");
            }
            return shipment;
        }

        /// <summary>
        /// Create line with the reference of Container
        /// </summary>
        /// <param name="inout"></param>
        /// <param name="ioLine"></param>
        /// <param name="oLine"></param>
        /// <param name="M_Locator_ID"></param>
        /// <param name="Qty"></param>
        /// <param name="oproduct"></param>
        /// <returns></returns>
        private String CreateShipmentLineContainer(MInOut inout, MInOutLine ioLine, MOrderLine oLine, int M_Locator_ID, Decimal Qty, bool disalowNegativeInventory, MProduct oproduct)
        {
            String pMsg = null;
            List<RecordContainer> shipLine = new List<RecordContainer>();

            // JID_1746: Create Lines for Charge / (Resource - Service - Expense) type product based on setting on Tenant to "Allow Non Item type".
            if (oproduct != null && oproduct.GetProductType() == MProduct.PRODUCTTYPE_Item)
            {
                MProductCategory productCategory = MProductCategory.GetOfProduct(GetCtx(), oLine.GetM_Product_ID());

                RecordContainer recordContainer = null;
                bool existingRecord = false;
                String sql = "";
                if (isContainerApplicable)
                {
                    // Pick data from Container Storage based on Material Policy
                    sql = @"SELECT s.M_AttributeSetInstance_ID ,s.M_ProductContainer_ID, s.Qty
                           FROM M_ContainerStorage s 
                           LEFT OUTER JOIN M_AttributeSetInstance asi ON (s.M_AttributeSetInstance_ID=asi.M_AttributeSetInstance_ID)
                           WHERE NOT EXISTS (SELECT * FROM M_ProductContainer p WHERE IsActive = 'N' AND p.M_ProductContainer_ID = NVL(s.M_ProductContainer_ID , 0)) 
                           AND s.AD_Client_ID= " + oLine.GetAD_Client_ID() + @"
                           AND s.AD_Org_ID=" + oLine.GetAD_Org_ID() + @"
                           AND s.M_Locator_ID = " + M_Locator_ID + @" 
                           AND s.M_Product_ID=" + oLine.GetM_Product_ID() + @"
                           AND s.Qty > 0 ";
                    if (oLine.GetM_AttributeSetInstance_ID() != 0)
                    {
                        sql += " AND NVL(s.M_AttributeSetInstance_ID , 0)=" + oLine.GetM_AttributeSetInstance_ID();
                    }
                    if (productCategory.GetMMPolicy() == X_M_Product_Category.MMPOLICY_LiFo)
                        sql += " ORDER BY asi.GuaranteeDate ASC, s.MMPolicyDate DESC, s.M_ContainerStorage_ID DESC";
                    else if (productCategory.GetMMPolicy() == X_M_Product_Category.MMPOLICY_FiFo)
                        sql += " ORDER BY asi.GuaranteeDate ASC, s.MMPolicyDate ASC , s.M_ContainerStorage_ID ASC";
                }
                else
                {
                    sql = @"SELECT s.M_AttributeSetInstance_ID ,0 AS M_ProductContainer_ID, s.QtyOnHand AS Qty
                        FROM M_Storage s WHERE s.M_Locator_ID = " + M_Locator_ID + @" 
                           AND s.M_Product_ID=" + oLine.GetM_Product_ID() + @"
                           AND s.QtyOnHand > 0 ";
                    if (oLine.GetM_AttributeSetInstance_ID() != 0)
                    {
                        sql += " AND NVL(s.M_AttributeSetInstance_ID , 0)=" + oLine.GetM_AttributeSetInstance_ID();
                    }

                    // VIS0060: Handle case of Material Policy on Shipment and Order without Attribute.
                    if (productCategory.GetMMPolicy() == X_M_Product_Category.MMPOLICY_LiFo)
                        sql += " ORDER BY l.PriorityNo DESC, s.M_AttributeSetInstance_ID DESC";
                    else if (productCategory.GetMMPolicy() == X_M_Product_Category.MMPOLICY_FiFo)
                        sql += " ORDER BY l.PriorityNo DESC, s.M_AttributeSetInstance_ID ASC";
                }
                DataSet ds = DB.ExecuteDataset(sql, null, Get_Trx());
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    int containerASI = 0;
                    int containerId = 0;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        existingRecord = false;
                        if (i > 0)
                        {
                            #region  Find existing record based on respective parameter
                            if (shipLine.Count > 0)
                            {
                                // Find existing record based on respective parameter
                                RecordContainer iRecord = null;
                                containerASI = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                                containerId = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_ProductContainer_ID"]);

                                if (shipLine.Exists(x => (x.M_Locator_ID == M_Locator_ID) &&
                                                         (x.M_Product_ID == oLine.GetM_Product_ID()) &&
                                                         (x.M_ASI_ID == containerASI) &&
                                                         (x.M_ProductContainer_ID == containerId)
                                                    ))
                                {
                                    iRecord = shipLine.Find(x => (x.M_Locator_ID == M_Locator_ID) &&
                                                                 (x.M_Product_ID == oLine.GetM_Product_ID()) &&
                                                                 (x.M_ASI_ID == containerASI) &&
                                                                 (x.M_ProductContainer_ID == containerId)
                                                           );
                                    if (iRecord != null)
                                    {
                                        // create object of existing record
                                        existingRecord = true;
                                        ioLine = new VAdvantage.Model.MInOutLine(GetCtx(), iRecord.M_InoutLine_ID, Get_Trx());
                                    }
                                }
                            }
                            #endregion
                        }

                        if (!existingRecord && i != 0)
                        {
                            // Create new object of shipline
                            ioLine = new MInOutLine(inout);
                        }

                        Decimal containerQty = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Qty"]);
                        Decimal lineCreatedQty = (Decimal.Subtract(Qty, containerQty) >= 0 ? ((i + 1) != ds.Tables[0].Rows.Count ? containerQty : Qty) : Qty);
                        if (!existingRecord)
                        {
                            ioLine.SetOrderLine(oLine, M_Locator_ID, lineCreatedQty);
                            ioLine.SetM_ProductContainer_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_ProductContainer_ID"]));
                        }
                        ioLine.SetQty(Decimal.Add(ioLine.GetQtyEntered(), lineCreatedQty));
                        if (oLine.GetQtyEntered().CompareTo(oLine.GetQtyOrdered()) != 0)
                        {
                            ioLine.SetQtyEntered(Decimal.Multiply(lineCreatedQty, (Decimal.Divide(oLine.GetQtyEntered(), (oLine.GetQtyOrdered())))));
                        }
                        ioLine.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]));

                        //190 - Get Print description and set
                        if (ioLine.Get_ColumnIndex("PrintDescription") >= 0 && oLine.Get_ColumnIndex("PrintDescription") >= 0)
                            ioLine.Set_Value("PrintDescription", oLine.Get_Value("PrintDescription"));

                        if (!ioLine.Save(Get_TrxName()))
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            if (pp != null && !string.IsNullOrEmpty(pp.GetName()))
                                pMsg = "Could not create Shipment Line. " + pp.GetName();
                            else
                                pMsg = "Could not create Shipment Line";
                            return pMsg;
                        }
                        else
                        {
                            #region  created list - for updating on same record
                            // created list - for updating on same record
                            if (!shipLine.Exists(x => (x.M_Locator_ID == ioLine.GetM_Locator_ID()) &&
                                                     (x.M_Product_ID == ioLine.GetM_Product_ID()) &&
                                                     (x.M_ASI_ID == ioLine.GetM_AttributeSetInstance_ID()) &&
                                                     (x.M_ProductContainer_ID == ioLine.GetM_ProductContainer_ID())
                                                     ))
                            {
                                recordContainer = new RecordContainer();
                                recordContainer.M_InoutLine_ID = ioLine.GetM_InOutLine_ID();
                                recordContainer.M_Locator_ID = ioLine.GetM_Locator_ID();
                                recordContainer.M_Product_ID = ioLine.GetM_Product_ID();
                                recordContainer.M_ASI_ID = ioLine.GetM_AttributeSetInstance_ID();
                                recordContainer.M_ProductContainer_ID = ioLine.GetM_ProductContainer_ID();
                                shipLine.Add(recordContainer);
                            }
                            #endregion

                            // Qty Represent "Remaining Qty" whose shipline line to be created
                            Qty -= lineCreatedQty;
                            if (Qty <= 0)
                                break;
                        }
                    }
                }
                ds.Dispose();
            }

            // When Disallow Negative Inventory is FALSE - then create new line with remainning qty
            if (Qty != 0)
            {
                ioLine = null;
                RecordContainer iRecord = null;
                if (shipLine.Exists(x => (x.M_Locator_ID == M_Locator_ID) &&
                                                    (x.M_Product_ID == oLine.GetM_Product_ID()) &&
                                                    (x.M_ASI_ID == oLine.GetM_AttributeSetInstance_ID()) &&
                                                    (x.M_ProductContainer_ID == 0)))
                {
                    iRecord = shipLine.Find(x => (x.M_Locator_ID == M_Locator_ID) &&
                                                 (x.M_Product_ID == oLine.GetM_Product_ID()) &&
                                                 (x.M_ASI_ID == oLine.GetM_AttributeSetInstance_ID()) &&
                                                 (x.M_ProductContainer_ID == 0));
                    if (iRecord != null)
                    {
                        // create object of existing record
                        ioLine = new VAdvantage.Model.MInOutLine(GetCtx(), iRecord.M_InoutLine_ID, Get_Trx());
                    }
                }

                // when no recod found on above filteation - then need to create new object
                if (ioLine == null)
                {
                    // Create new object of shipline
                    ioLine = new MInOutLine(inout);
                }
                ioLine.SetOrderLine(oLine, M_Locator_ID, Qty);
                ioLine.SetM_ProductContainer_ID(0);
                ioLine.SetQty(Decimal.Add(ioLine.GetQtyEntered(), Qty));
                if (oLine.GetQtyEntered().CompareTo(oLine.GetQtyOrdered()) != 0)
                {
                    ioLine.SetQtyEntered(Decimal.Multiply(Qty, (Decimal.Divide(oLine.GetQtyEntered(), (oLine.GetQtyOrdered())))));
                }
                //190 - Get Print description and set
                if (ioLine.Get_ColumnIndex("PrintDescription") >= 0 && oLine.Get_ColumnIndex("PrintDescription") >= 0)
                    ioLine.Set_Value("PrintDescription", oLine.Get_Value("PrintDescription"));
                if (!ioLine.Save(Get_TrxName()))
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    if (pp != null && !string.IsNullOrEmpty(pp.GetName()))
                        pMsg = "Could not create Shipment Line. " + pp.GetName();
                    else
                        pMsg = "Could not create Shipment Line";
                    return pMsg;
                }
            }
            return pMsg;
        }


        /****************************************************************************************************/
        /* 	Create Invoice
            *	@param dt order document type
            *	@param shipment optional shipment
            *	@param invoiceDate invoice date
            *	@return invoice or null
            */
        private MInvoice CreateInvoice(MDocType dt, MInOut shipment, DateTime? invoiceDate)
        {
            MInvoice invoice = new MInvoice(this, dt.GetC_DocTypeInvoice_ID(), invoiceDate);
            if (Util.GetValueOfInt(GetVAPOS_POSTerminal_ID()) > 0)
            {
                #region VAPOS_POSTerminal_ID > 0
                String DocSubTypeSO = dt.GetDocSubTypeSO();
                if (MDocType.DOCSUBTYPESO_POSOrder.Equals(DocSubTypeSO))
                {
                    if (GetVAPOS_POSTerminal_ID() != 0)
                    {
                        invoice.SetDocumentNo(GetDocumentNo());
                        try
                        {
                            int ConversionTypeId = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT C_ConversionType_ID FROM VAPOS_POSTerminal WHERE 
                                                VAPOS_POSTerminal_ID=" + GetVAPOS_POSTerminal_ID()));
                            invoice.SetC_ConversionType_ID(ConversionTypeId);

                            //MOrder ord = new MOrder(GetCtx(), GetC_Order_ID(), null);

                            if (GetVAPOS_CreditAmt() > 0)
                            {
                                invoice.SetIsPaid(false);
                                invoice.SetVAPOS_IsPaid(false);
                            }
                            else
                            {
                                invoice.SetIsPaid(true);
                                invoice.SetVAPOS_IsPaid(true);
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Info("Paid Or ConversionType not Marked ====>>>> " + ex.Message);
                        }
                    }
                }

                if (Env.IsModuleInstalled("VA009_"))
                {
                    invoice.SetVA009_PaymentMethod_ID(GetVA009_PaymentMethod_ID());
                }
                #endregion
            }
            log.Info(dt.ToString());
            try
            {
                //SI_0181 : set Payment Method from order to invoice
                if (Env.IsModuleInstalled("VA009_"))
                {
                    invoice.SetVA009_PaymentMethod_ID(GetVA009_PaymentMethod_ID());
                }
                if (!invoice.Save(Get_TrxName()))
                {
                    _processMsg = "Could not create Invoice";
                    return null;
                }

                //	If we have a Shipment - use that as a base
                if (shipment != null)
                {
                    if (!INVOICERULE_AfterDelivery.Equals(GetInvoiceRule()))
                        SetInvoiceRule(INVOICERULE_AfterDelivery);
                    //
                    MInOutLine[] sLines = shipment.GetLines(false);
                    for (int i = 0; i < sLines.Length; i++)
                    {
                        MInOutLine sLine = sLines[i];
                        //
                        MInvoiceLine iLine = new MInvoiceLine(invoice);
                        iLine.SetShipLine(sLine);
                        //	Qty = Delivered	
                        iLine.SetQtyEntered(sLine.GetQtyEntered());
                        iLine.SetQtyInvoiced(sLine.GetMovementQty());
                        //190 - Get Print description and set
                        if (iLine.Get_ColumnIndex("PrintDescription") >= 0 && sLine.Get_ColumnIndex("PrintDescription") >= 0)
                            iLine.Set_Value("PrintDescription", sLine.Get_Value("PrintDescription"));
                        if (!iLine.Save(Get_TrxName()))
                        {
                            _processMsg = "Could not create Invoice Line from Shipment Line";
                            return null;
                        }
                        //
                        sLine.SetIsInvoiced(true);
                        if (!sLine.Save(Get_TrxName()))
                        {
                            log.Warning("Could not update Shipment line: " + sLine);
                        }
                    }


                    // Create Lines for Charge / (Resource - Service - Expense) type product based on setting on Tenant to "Allow Non Item type".
                    string allowNonItem = Util.GetValueOfString(GetCtx().GetContext("$AllowNonItem"));

                    if (String.IsNullOrEmpty(allowNonItem))
                    {
                        allowNonItem = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsAllowNonItem FROM AD_Client WHERE AD_Client_ID = " + GetAD_Client_ID(), null, Get_Trx()));
                    }
                    if (allowNonItem.Equals("N"))
                    {
                        // Create Invoice Line for Charge / (Resource - Service - Expense) type product 
                        MOrderLine[] oLines = GetLinesOtherthanProduct();
                        for (int i = 0; i < oLines.Length; i++)
                        {
                            MOrderLine oLine = oLines[i];
                            //
                            MInvoiceLine iLine = new MInvoiceLine(invoice);
                            iLine.SetOrderLine(oLine);
                            //	Qty = Ordered - Invoiced	
                            iLine.SetQtyInvoiced(Decimal.Subtract(oLine.GetQtyOrdered(), oLine.GetQtyInvoiced()));
                            if (oLine.GetQtyOrdered().CompareTo(oLine.GetQtyEntered()) == 0)
                                iLine.SetQtyEntered(iLine.GetQtyInvoiced());
                            else
                                iLine.SetQtyEntered(Decimal.Multiply(iLine.GetQtyInvoiced(), (Decimal.Divide(oLine.GetQtyEntered(), oLine.GetQtyOrdered()))));
                            
                            //190 - Get Print description and set
                            if (iLine.Get_ColumnIndex("PrintDescription") >= 0 && oLine.Get_ColumnIndex("PrintDescription") >= 0)
                                iLine.Set_Value("PrintDescription", oLine.Get_Value("PrintDescription"));
                            if (!iLine.Save(Get_TrxName()))
                            {
                                _processMsg = "Could not create Invoice Line from Order Line";
                                return null;
                            }
                        }
                    }
                }
                else    //	Create Invoice from Order
                {
                    if (!INVOICERULE_Immediate.Equals(GetInvoiceRule()))
                        SetInvoiceRule(INVOICERULE_Immediate);
                    //
                    MOrderLine[] oLines = GetLines();
                    for (int i = 0; i < oLines.Length; i++)
                    {
                        MOrderLine oLine = oLines[i];
                        //
                        MInvoiceLine iLine = new MInvoiceLine(invoice);
                        iLine.SetOrderLine(oLine);
                        //	Qty = Ordered - Invoiced	
                        iLine.SetQtyInvoiced(Decimal.Subtract(oLine.GetQtyOrdered(), oLine.GetQtyInvoiced()));
                        if (oLine.GetQtyOrdered().CompareTo(oLine.GetQtyEntered()) == 0)
                            iLine.SetQtyEntered(iLine.GetQtyInvoiced());
                        else
                            iLine.SetQtyEntered(Decimal.Multiply(iLine.GetQtyInvoiced(), (Decimal.Divide(oLine.GetQtyEntered(), oLine.GetQtyOrdered()))));

                        //190 - Get Print description and set
                        if (iLine.Get_ColumnIndex("PrintDescription") >= 0 && oLine.Get_ColumnIndex("PrintDescription") >= 0)
                            iLine.Set_Value("PrintDescription", oLine.Get_Value("PrintDescription"));
                        if (!iLine.Save(Get_TrxName()))
                        {
                            _processMsg = "Could not create Invoice Line from Order Line";
                            return null;
                        }
                    }
                }
                //	Manually Process Invoice
                String status = invoice.CompleteIt();
                invoice.SetDocStatus(status);
                invoice.Save(Get_TrxName());

                SetC_CashLine_ID(invoice.GetC_CashLine_ID());
                if (!DOCSTATUS_Completed.Equals(status))
                {
                    _processMsg = "@C_Invoice_ID@: " + invoice.GetProcessMsg();
                    return null;
                }
            }
            catch
            {
                //ShowMessage.Error("MOrder",null,"CreateInvoice");
            }
            return invoice;
        }

        /* 	Create Counter Document
         * 	@return counter order
         */
        private MOrder CreateCounterDoc()
        {
            //	Is this itself a counter doc ?
            if (GetRef_Order_ID() != 0)
            {
                return null;
            }

            //	Document Type
            //check weather Counter Document created & Acitve or not 
            int C_DocTypeTarget_ID = 0;
            MDocTypeCounter counterDT = MDocTypeCounter.GetCounterDocType(GetCtx(), GetC_DocType_ID());
            if (counterDT != null)
            {
                log.Fine(counterDT.ToString());
                if (!counterDT.IsCreateCounter() || !counterDT.IsValid())
                {
                    //erro save into the log the messge if couter DocType is not valid
                    log.Info("Counter Document Type is not Valid one!");
                    return null;
                }
                C_DocTypeTarget_ID = counterDT.GetCounter_C_DocType_ID();
                if (C_DocTypeTarget_ID <= 0)
                {
                    //Info save into the log the messge if couter DocType is not found
                    log.Info("Counter Document Type not found on Inter Company Document window.");
                    return null;
                }
            }
            else
            {
                return null;
            }


            //	Org Must be linked to BPartner
            MOrg org = MOrg.Get(GetCtx(), GetAD_Org_ID());
            //jz int counterC_BPartner_ID = org.getLinkedC_BPartner_ID(Get_TrxName()); 
            int counterC_BPartner_ID = org.GetLinkedC_BPartner_ID(Get_TrxName());
            if (counterC_BPartner_ID == 0)
            {
                //Info save into the log the messge if Counter BP not found
                log.Info("Business Partner is not found on Customer/Vendor master window to create the Counter Document.");
                return null;
            }
            //	Business Partner needs to be linked to Org
            //jz MBPartner bp = MBPartner.get (GetCtx(), getC_BPartner_ID());
            MBPartner bp = new MBPartner(GetCtx(), GetC_BPartner_ID(), Get_TrxName());
            int counterAD_Org_ID = bp.GetAD_OrgBP_ID_Int();
            if (counterAD_Org_ID == 0)
            {
                //Info save into the log the messge if Link Organization not found
                log.Info("Linked Organization is not found on Customer/Vendor master window to create the Counter Document.");
                return null;
            }

            //System should not allow to create counter document with same BP and organization.
            if (counterAD_Org_ID == GetAD_Org_ID() || counterC_BPartner_ID == GetC_BPartner_ID())
            {
                //erro save into the log
                log.Info("On Counter Document Organization or Business Partner should not allow the same with the Document.");
                return null;
            }

            //jz MBPartner counterBP = MBPartner.get (GetCtx(), counterC_BPartner_ID);
            MBPartner counterBP = new MBPartner(GetCtx(), counterC_BPartner_ID, Get_TrxName());
            MOrgInfo counterOrgInfo = MOrgInfo.Get(GetCtx(), counterAD_Org_ID, null);
            log.Info("Counter BP=" + counterBP.GetName());

            // set counter Businees partner 
            SetCounterBPartner(counterBP, counterAD_Org_ID, counterOrgInfo.GetM_Warehouse_ID());
            //	Deep Copy
            // JID_1300: If the PO is created with lines includes Product with attribute set instance, once the counter document is created on completion of PO i.e. SO, 
            // Attribute Set Instance values are not getting fetched into SO lines.
            MOrder counter = CopyFrom(this, GetDateOrdered(),
                C_DocTypeTarget_ID, true, true, Get_TrxName());
            //
            counter.SetDatePromised(GetDatePromised());     // default is date ordered 
                                                            //	Refernces (Should not be required
            counter.SetSalesRep_ID(GetSalesRep_ID());
            //
            counter.SetProcessing(false);
            counter.Save(Get_TrxName());

            //	Update copied lines
            MOrderLine[] counterLines = counter.GetLines(true, null);
            for (int i = 0; i < counterLines.Length; i++)
            {
                MOrderLine counterLine = counterLines[i];
                counterLine.SetOrder(counter);  //	copies header values (BP, etc.)
                counterLine.SetPrice();
                counterLine.SetTax();
                counterLine.Save(Get_TrxName());
            }
            log.Fine(counter.ToString());

            //	Document Action
            if (counterDT != null)
            {
                if (counterDT.GetDocAction() != null)
                {
                    counter.SetDocAction(counterDT.GetDocAction());
                    counter.ProcessIt(counterDT.GetDocAction());
                    counter.Save(Get_TrxName());
                }
            }
            return counter;
        }

        /// <summary>
        /// Void Document.
        ///	Set Qtys to 0 - Sales: reverse all documents
        /// </summary>
        /// <returns>true if success</returns>
        public bool VoidIt()
        {
            MOrderLine[] lines = GetLines(true, "M_Product_ID");
            log.Info(ToString());

            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            String DocSubTypeSO = dt.GetDocSubTypeSO();

            //JID_1474 If quantity released is greater than 0, then system will not allow to void blanket order record and give message: 'Please Void/Reverse its dependent transactions first'
            if (dt.GetDocBaseType() == MDocBaseType.DOCBASETYPE_BLANKETSALESORDER)
            {
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT SUM(QtyReleased) FROM C_OrderLine WHERE C_Order_ID = " + GetC_Order_ID(), null, Get_Trx())) > 0)
                {
                    _processMsg = "Please Void/Reverse its dependent transactions first";
                    return false;
                }
            }

            // Added by Vivek on 08/11/2017 assigned by Mukesh sir
            // return false if linked document is in completed or closed stage
            // when we void SO then system void all transaction which is linked with that SO
            if (MDocType.DOCSUBTYPESO_OnCreditOrder.Equals(DocSubTypeSO)    //	(W)illCall(I)nvoice
                    || MDocType.DOCSUBTYPESO_WarehouseOrder.Equals(DocSubTypeSO)    //	(W)illCall(P)ickup	
                    || MDocType.DOCSUBTYPESO_POSOrder.Equals(DocSubTypeSO))         //	(W)alkIn(R)eceipt
            {
                // when we void SO then system void all transaction which is linked with that SO
            }
            else
            {
                if (!linkedDocument(GetC_Order_ID()))
                {
                    _processMsg = Msg.GetMsg(GetCtx(), "LinkedDocStatus");
                    return false;
                }
            }

            // Added by Vivek on 27/09/2017 assigned by Pradeep 
            // To check if associated PO is exist but not reversed in case of drop shipment
            if (IsSOTrx() && !IsReturnTrx())
            {
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(C_OrderLine_ID) FROM C_OrderLine ol INNER JOIN C_Order o ON o.C_Order_ID=ol.C_Order_ID WHERE O.C_Order_ID=" + GetC_Order_ID() + " AND ol.IsDropShip='Y' AND O.IsSoTrx='Y' AND O.IsReturnTrx='N'")) > 0)
                {
                    if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(C_Order_ID) FROM C_Order WHERE Ref_Order_ID=" + GetC_Order_ID() + " AND IsDropShip='Y' AND IsSoTrx='N' AND IsReturnTrx='N' AND DocStatus NOT IN ('VO','RE')")) > 0)
                    {
                        _processMsg = "Associated purchase order must be voided or reversed first";
                        return false;
                    }
                }
            }
            for (int i = 0; i < lines.Length; i++)
            {
                MOrderLine line = lines[i];
                Decimal old = line.GetQtyOrdered();
                if (System.Math.Sign(old) != 0)
                {
                    line.AddDescription(Msg.GetMsg(Env.GetContext(), "Voided", true) + " (" + old + ")");
                    line.SetQtyLostSales(old);
                    line.SetQty(Env.ZERO);
                    line.SetLineNetAmt(Env.ZERO);

                    // Remove Reference of Requisition from PO line after Void.
                    line.Set_Value("M_RequisitionLine_ID", 0);
                    line.Save(Get_TrxName());
                }
            }
            AddDescription(Msg.GetMsg(Env.GetContext(), "Voided", true));
            //	Clear Reservations
            if (!ReserveStock(null, lines))
            {
                _processMsg = "Cannot unreserve Stock (void)";
                return false;
            }

            if (!CreateReversals())
                return false;

            //************* Changed ***************************
            // Set Status at Order to Rejected if it is Sales Order 
            MOrder ord = new MOrder(Env.GetCtx(), GetC_Order_ID(), Get_TrxName());
            if (IsSOTrx())
            {
                ord.SetStatusCode("R");
                ord.Save();
            }

            // JID_0658: After Creating PO from Open Requisition, & the PO record is Void, PO line reference is not getting removed from Requisition Line.
            DB.ExecuteQuery("UPDATE M_RequisitionLine SET C_OrderLine_ID = NULL WHERE C_OrderLine_ID IN (SELECT C_OrderLine_ID FROM C_OrderLine WHERE C_Order_ID = " + GetC_Order_ID() + ")", null, Get_TrxName());

            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        /* Create Shipment/Invoice Reversals
        * 	@return true if success
        */
        private bool CreateReversals()
        {
            try
            {
                //	Cancel only Sales 
                if (!IsSOTrx() || Util.GetValueOfBool(Get_Value("IsSalesQuotation")))
                    return true;

                log.Info("");
                StringBuilder Info = new StringBuilder();
                // JID_0216: System void/reverse the Shipment and invoice related to SO and voided document number will be displayed with the document name. 
                //	Reverse All *Shipments*
                //Info.Append("@M_InOut_ID@:");
                Info.Append(Msg.GetMsg(GetCtx(), "Shipment") + ":");
                MInOut[] shipments = GetShipments(false);   //	get all (line based)
                for (int i = 0; i < shipments.Length; i++)
                {
                    MInOut ship = shipments[i];
                    //	if closed - ignore
                    if (MInOut.DOCSTATUS_Closed.Equals(ship.GetDocStatus())
                        || MInOut.DOCSTATUS_Reversed.Equals(ship.GetDocStatus())
                        || MInOut.DOCSTATUS_Voided.Equals(ship.GetDocStatus()))
                        continue;
                    ship.Set_TrxName(Get_TrxName());

                    //	If not completed - void - otherwise reverse it
                    if (!MInOut.DOCSTATUS_Completed.Equals(ship.GetDocStatus()))
                    {
                        if (ship.VoidIt())
                            ship.SetDocStatus(MInOut.DOCSTATUS_Voided);
                    }
                    //	Create new Reversal with only that order
                    else if (!ship.IsOnlyForOrder(this))
                    {
                        ship.ReverseCorrectIt(this);
                        //	shipLine.setDocStatus(MInOut.DOCSTATUS_Reversed);
                        Info.Append(" Parial ").Append(ship.GetDocumentNo());
                    }
                    else if (ship.ReverseCorrectIt()) //	completed shipment
                    {
                        ship.SetDocStatus(MInOut.DOCSTATUS_Reversed);
                        Info.Append(" ").Append(ship.GetDocumentNo());
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(ship.GetProcessMsg()))
                        {
                            Info.Append(" ").Append(ship.GetProcessMsg());
                            //_processMsg = ship.GetProcessMsg() + " - " + ship;
                        }
                        else
                        {
                            Info.Append(" ").Append(Msg.GetMsg(GetCtx(), "ErrorReverse"));
                            //_processMsg = "Could not reverse Shipment " + ship;
                        }
                        _processMsg = Info.ToString();
                        return false;
                    }
                    ship.SetDocAction(MInOut.DOCACTION_None);
                    ship.Save(Get_TrxName());
                }   //	for all shipments

                //	Reverse All *Invoices*
                Info.Append(" - @C_Invoice_ID@:");
                //Info.Append(Msg.GetMsg(GetCtx(), "SalesOrder"));
                MInvoice[] invoices = GetInvoices(false);   //	get all (line based)
                for (int i = 0; i < invoices.Length; i++)
                {
                    MInvoice invoice = invoices[i];
                    //	if closed - ignore
                    if (MInvoice.DOCSTATUS_Closed.Equals(invoice.GetDocStatus())
                        || MInvoice.DOCSTATUS_Reversed.Equals(invoice.GetDocStatus())
                        || MInvoice.DOCSTATUS_Voided.Equals(invoice.GetDocStatus()))
                        continue;
                    invoice.Set_TrxName(Get_TrxName());

                    //	If not completed - void - otherwise reverse it
                    if (!MInvoice.DOCSTATUS_Completed.Equals(invoice.GetDocStatus()))
                    {
                        if (invoice.VoidIt())
                            invoice.SetDocStatus(MInvoice.DOCSTATUS_Voided);
                    }
                    else if (invoice.ReverseCorrectIt())    //	completed invoice
                    {
                        invoice.SetDocStatus(MInvoice.DOCSTATUS_Reversed);
                        Info.Append(" ").Append(invoice.GetDocumentNo());
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(invoice.GetProcessMsg()))
                        {
                            Info.Append(" ").Append(invoice.GetProcessMsg());
                            //_processMsg = invoice.GetProcessMsg() + " - " + invoice;
                        }
                        else
                        {
                            Info.Append(" ").Append(Msg.GetMsg(GetCtx(), "ErrorReverse"));
                            //_processMsg = "Could not reverse Invoice " + invoice;
                        }
                        _processMsg = Info.ToString();
                        return false;
                    }
                    invoice.SetDocAction(MInvoice.DOCACTION_None);
                    invoice.Save(Get_TrxName());
                }   //	for all shipments

                //	Reverse All *RMAs*
                //Info.Append("@C_Order_ID@:");
                Info.Append(" - " + Msg.GetMsg(GetCtx(), "RMA") + ":");
                MOrder[] rmas = GetRMAs();
                for (int i = 0; i < rmas.Length; i++)
                {
                    MOrder rma = rmas[i];
                    //	if closed - ignore
                    if (MOrder.DOCSTATUS_Closed.Equals(rma.GetDocStatus())
                        || MOrder.DOCSTATUS_Reversed.Equals(rma.GetDocStatus())
                        || MOrder.DOCSTATUS_Voided.Equals(rma.GetDocStatus()))
                        continue;
                    rma.Set_TrxName(Get_TrxName());

                    //	If not completed - void - otherwise reverse it
                    if (!MOrder.DOCSTATUS_Completed.Equals(rma.GetDocStatus()))
                    {
                        if (rma.VoidIt())
                            rma.SetDocStatus(MInOut.DOCSTATUS_Voided);
                    }
                    //	Create new Reversal with only that order
                    else if (rma.ReverseCorrectIt()) //	completed shipment
                    {
                        rma.SetDocStatus(MOrder.DOCSTATUS_Reversed);
                        Info.Append(" ").Append(rma.GetDocumentNo());
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(rma.GetProcessMsg()))
                        {
                            Info.Append(" ").Append(rma.GetProcessMsg());
                            //_processMsg = rma.GetProcessMsg() + " - " + rma;
                        }
                        else
                        {
                            Info.Append(" ").Append(Msg.GetMsg(GetCtx(), "ErrorReverse"));
                            //_processMsg = "Could not reverse RMA " + rma;
                        }
                        _processMsg = Info.ToString();
                        return false;
                    }
                    rma.SetDocAction(MInOut.DOCACTION_None);
                    rma.Save(Get_TrxName());
                }   //	for all shipments

                _processMsg = Info.ToString();
            }
            catch
            {
                //ShowMessage.Error("MOrder",null,"CreateReversals");
                ValueNamePair pp = VLogger.RetrieveError();
                if (pp != null && !String.IsNullOrEmpty(pp.GetName()))
                    _processMsg = "Could not CreateReversals. " + pp.GetName();
                else
                    _processMsg = "Could not CreateReversals.";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Close Document. Cancel not delivered Quantities
        /// </summary>
        /// <returns>true if success</returns>
        public bool CloseIt()
        {
            log.Info(ToString());
            //	Close Not delivered Qty - SO/PO
            MOrderLine[] lines = GetLines(true, "M_Product_ID");
            for (int i = 0; i < lines.Length; i++)
            {
                MOrderLine line = lines[i];
                Decimal old = line.GetQtyOrdered();
                if (old.CompareTo(line.GetQtyDelivered()) != 0)
                {
                    line.SetQtyLostSales(Decimal.Subtract(line.GetQtyOrdered(), line.GetQtyDelivered()));
                    line.SetQtyOrdered(line.GetQtyDelivered());
                    //Set property to true because close event is called
                    line.SetIsClosedDocument(true);
                    //	QtyEntered unchanged
                    line.AddDescription("Close (" + old + ")");
                    line.Save(Get_TrxName());
                }
            }
            //	Clear Reservations
            if (!ReserveStock(null, lines))
            {
                _processMsg = "Cannot unreserve Stock (close)";
                return false;
            }
            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Reverse Correction - same void
        /// </summary>
        /// <returns>true if success</returns>
        public bool ReverseCorrectIt()
        {
            log.Info(ToString());
            return VoidIt();
        }

        /// <summary>
        /// Reverse Accrual - none
        /// </summary>
        /// <returns>false</returns>
        public bool ReverseAccrualIt()
        {
            log.Info(ToString());
            return false;
        }

        /// <summary>
        /// Re-activate.
        /// </summary>
        /// <returns>true if success</returns>
        public bool ReActivateIt()
        {
            try
            {
                log.Info(ToString());

                if (GetVAPOS_POSTerminal_ID() > 0 && (GetVA018_VoucherAmount() > 0 || GetVA204_RewardAmt() > 0))
                {
                    _processMsg = "Voucher Or Reward amount redeemed against this order, can-not reactivate";
                    log.Warning("Voucher  Or Reward amount redeemed against this order, can-not reactivate");
                    return false;
                }

                // JID_1035 before reactivating the order user need to void the payment first if orderschedule  exist against current order
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT( VA009_OrderPaySchedule_ID ) FROM VA009_OrderPaySchedule WHERE C_Order_ID =" + GetC_Order_ID() + " AND (C_Payment_ID !=0 OR C_CashLine_ID!=0)")) > 0)
                {
                    _processMsg = Msg.GetMsg(GetCtx(), "PaymentmustvoidedFirst");
                    return false;
                }

                MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
                String DocSubTypeSO = dt.GetDocSubTypeSO();
                MOrderLine[] lines = null;
                // Added by Vivek on 08/11/2017 assigned by Mukesh sir
                // return false if linked document is in completed or closed stage
                if (MDocType.DOCSUBTYPESO_OnCreditOrder.Equals(DocSubTypeSO)    //	(W)illCall(I)nvoice
                    || MDocType.DOCSUBTYPESO_WarehouseOrder.Equals(DocSubTypeSO)    //	(W)illCall(P)ickup	
                    || MDocType.DOCSUBTYPESO_POSOrder.Equals(DocSubTypeSO))         //	(W)alkIn(R)eceipt
                {
                    // when we void SO then system void all transaction which is linked with that SO
                }
                else
                {
                    // JID_1362 - User can reactivate record even the depenedent transaction available into system
                    ////if (!linkedDocument(GetC_Order_ID()))
                    ////{
                    ////    _processMsg = Msg.GetMsg(GetCtx(), "LinkedDocStatus");
                    ////    return false;
                    ////}
                }

                // Added by Vivek on 27/09/2017 assigned by Pradeep 
                // To check if associated PO is exist but not reversed  in case of drop shipment
                if (IsSOTrx() && !IsReturnTrx())
                {
                    if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(C_OrderLine_ID) FROM C_OrderLine ol INNER JOIN C_Order o ON o.C_Order_ID=ol.C_Order_ID Where O.C_Order_ID=" + GetC_Order_ID() + " AND ol.IsDropShip='Y' AND O.IsSoTrx='Y' AND O.IsReturnTrx='N'")) > 0)
                    {
                        if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(C_Order_ID) FROM C_Order WHERE Ref_Order_Id=" + GetC_Order_ID() + " AND IsDropShip='Y' AND IsSoTrx='N' AND IsReturnTrx='N' AND DocStatus NOT IN ('VO','RE')")) > 0)
                        {
                            _processMsg = "Associated purchase order must be voided or reversed first";
                            return false;
                        }
                    }
                }

                // set reserved qty to 0 when order is reactivating
                // Added by Vivek on 27/02/2018 assigned by mukesh sir
                /* SI_0561 : when we Re-Activate -- we can not set as "0" to QtyReserved because we are not updating qtyeserved on storage.
                             Otherwise system reserving again the same qty which define on the same record*/
                //MOrderLine line = null;
                //lines = GetLines(true, "M_Product_ID");
                //for (int i = 0; i < lines.Length; i++)
                //{
                //    line = new MOrderLine(GetCtx(), _lines[i].GetC_OrderLine_ID(), Get_Trx());
                //    line.SetQtyReserved(0);
                //    line.Save(Get_Trx());
                //}

                //	Replace Prepay with POS to revert all doc
                if (MDocType.DOCSUBTYPESO_PrepayOrder.Equals(DocSubTypeSO))
                {
                    MDocType newDT = null;
                    MDocType[] dts = MDocType.GetOfClient(GetCtx());
                    for (int i = 0; i < dts.Length; i++)
                    {
                        MDocType type = dts[i];
                        if (MDocType.DOCSUBTYPESO_PrepayOrder.Equals(type.GetDocSubTypeSO()))
                        {
                            if (type.IsDefault() || newDT == null)
                                newDT = type;
                        }
                    }
                    if (newDT == null)
                        return false;
                    else
                    {
                        SetC_DocType_ID(newDT.GetC_DocType_ID());
                        SetIsReturnTrx(newDT.IsReturnTrx());
                    }
                }

                if (dt.GetDocBaseType() == "BOO")  // if (dt.GetValue() == "BPO" || dt.GetValue() == "BSO")
                {
                    MOrder mo = new MOrder(GetCtx(), GetC_Order_ID(), null);
                    if (DateTime.Now.Date > mo.GetOrderValidTo().Value.Date)
                    {
                        _processMsg = "Validity of Order has been finished";
                        log.Info("Validity of Order has been finished");
                        return false;
                    }
                    lines = GetLines(true, "M_Product_ID");
                    int count = 0;

                    for (int i = 0; i < lines.Length; i++)
                    {
                        Decimal? qtyRel = MUOMConversion.ConvertProductTo(GetCtx(), lines[i].GetM_Product_ID(), lines[i].GetC_UOM_ID(), lines[i].GetQtyReleased());
                        if (qtyRel != null)
                        {
                            if (qtyRel >= lines[i].GetQtyEstimation())
                                count++;
                        }
                        else
                        {
                            if (lines[i].GetQtyReleased() >= lines[i].GetQtyEstimation())
                                count++;
                        }
                    }

                    if (count == lines.Length)
                    {
                        _processMsg = "All Estimated Quantites has been released.";
                        log.Warning("All Estimated Quantites has been released.");
                        return false;
                    }
                }


                //	PO - just re-open
                if (!IsSOTrx())
                {
                    log.Info("Existing documents not modified - " + dt);
                }
                //	Reverse Direct Documents
                else if (MDocType.DOCSUBTYPESO_OnCreditOrder.Equals(DocSubTypeSO)   //	(W)illCall(I)nvoice
                    || MDocType.DOCSUBTYPESO_WarehouseOrder.Equals(DocSubTypeSO)    //	(W)illCall(P)ickup	
                    || MDocType.DOCSUBTYPESO_POSOrder.Equals(DocSubTypeSO))         //	(W)alkIn(R)eceipt
                {
                    if (!CreateReversals())
                        return false;
                    else if (GetVAPOS_POSTerminal_ID() > 0) //POS Terminal Order
                    {
                        if (GetVAPOS_RefPOSTerminal_ID() < 1)
                        {
                            SetVAPOS_RefPOSTerminal_ID(GetVAPOS_POSTerminal_ID());
                        }

                        SetVAPOS_POSTerminal_ID(0);
                        //SetVAPOS_RefPOSTerminal_ID
                        //Remove data from POS specific fields
                        SetVAPOS_CashPaid(0);
                        SetVAPOS_CreditAmt(0);
                        SetVAPOS_PayAmt(0);
                        SetVA205_Amounts("");
                        SetVA205_Currencies("");
                        SetVA205_RetAmounts("0");
                        SetVA205_Amounts("");
                        SetVAPOS_ReturnAmt(0);
                        SetVAPOS_TPPAmt(0);
                        SetVAPOS_TPPInfo("");
                        Save(Get_Trx());
                    }
                }
                else
                {
                    log.Info("Existing documents not modified - SubType=" + DocSubTypeSO);
                }

                // Set Value in Re-Activated when record is reactivated
                if (Get_ColumnIndex("IsReActivated") >= 0)
                {
                    SetIsReActivated(true);
                }

                SetDocAction(DOCACTION_Complete);
                SetProcessed(false);
                // In case of purchase order reverse budget breach
                // Done by Rakesh Kumar 18/Feb/2020
                if (Env.IsModuleInstalled("FRPT_") && !IsSOTrx() && !IsReturnTrx() && !IsSalesQuotation() && !Util.GetValueOfBool(Get_Value("IsBlanketTrx")))
                {
                    SetIsBudgetBreach(false);
                    SetIsBudgetBreachApproved(false);
                }
            }
            catch
            {
                //ShowMessage.Error("MOrder", null, "SetBPartner");
            }
            return true;
        }

        /// <summary>
        /// Get Summary
        /// </summary>
        /// <returns>Summary of Document</returns>
        public String GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetDocumentNo());
            //	: Grand Total = 123.00 (#1)
            sb.Append(": ").
                Append(Msg.Translate(GetCtx(), "GrandTotal")).Append("=").Append(GetGrandTotal());
            if (_lines != null)
                sb.Append(" (#").Append(_lines.Length).Append(")");
            //	 - Description
            if (GetDescription() != null && GetDescription().Length > 0)
                sb.Append(" - ").Append(GetDescription());
            return sb.ToString();
        }

        /// <summary>
        /// Get Process Message
        /// </summary>
        /// <returns>clear text error message</returns>
        public String GetProcessMsg()
        {
            return _processMsg;
        }

        /// <summary>
        /// Get Document Owner (Responsible)
        /// </summary>
        /// <returns>AD_User_ID</returns>
        public int GetDoc_User_ID()
        {
            return GetSalesRep_ID();
        }

        /// <summary>
        /// Get Document Approval Amount
        /// </summary>
        /// <returns>amount</returns>
        public Decimal GetApprovalAmt()
        {
            return GetGrandTotal();
        }

        /// <summary>
        /// Get Latest Shipment for the Order
        /// </summary>
        /// <param name="C_DocType_ID"></param>
        /// <param name="M_Warehouse_ID"></param>
        /// <param name="C_BPartner_ID"></param>
        /// <param name="C_BPartner_Location_ID"></param>
        /// <returns>latest shipment</returns>
        public MInOut GetOpenInOut(int C_DocType_ID, int M_Warehouse_ID, int C_BPartner_ID, int C_BPartner_Location_ID)
        {
            //	TODO: getShipment if linked on line
            MInOut inout = null;
            String sql = "SELECT M_InOut_ID " +
            "FROM M_InOut WHERE C_Order_ID=" + GetC_Order_ID()
           + " AND M_Warehouse_ID=" + M_Warehouse_ID
           + " AND C_BPartner_ID=" + C_BPartner_ID
           + " AND C_BPartner_Location_ID= " + C_BPartner_Location_ID
           + " AND C_DocType_ID= " + C_DocType_ID
            + " AND DocStatus IN ('DR','IP') " +
            " ORDER BY Created DESC";
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, Get_TrxName());
                if (idr.Read())
                {
                    inout = new MInOut(GetCtx(), Util.GetValueOfInt(idr[0]), Get_TrxName());
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            //
            return inout;
        }

        /// <summary>
        /// Get Linekd Document for the Order
        /// </summary>
        /// <param name="C_Order_ID"></param>
        /// <returns>True/False</returns>
        private bool linkedDocument(int C_Order_ID)
        {
            // SI_0595_3 : check orderline id exist on invoiceline or inoutline. if exist then not able to reverse the current order.
            // JID_1953 : by Amit -> when payment exist against selected Order, then not able to void transaction
            string sql = @"SELECT SUM(Result) FROM (
                           SELECT COUNT(il.C_OrderLine_ID) AS Result FROM M_InOut i INNER JOIN M_InOutLine il ON i.M_InOut_ID = il.M_InOut_ID
                           INNER JOIN C_OrderLine ol ON ol.C_OrderLine_ID = il.C_OrderLine_ID
                           WHERE ol.C_Order_ID  = " + C_Order_ID + @" AND i.DocStatus NOT IN ('RE' , 'VO')
                         UNION ALL
                           SELECT COUNT(C_Order_ID) AS Result FROM C_Payment WHERE 
                                DocStatus NOT IN ('RE' , 'VO') AND C_Order_ID = " + C_Order_ID + @"
                         UNION ALL
                          SELECT COUNT(il.C_OrderLine_ID) AS Result FROM C_Invoice i INNER JOIN C_InvoiceLine il ON i.C_Invoice_id = il.C_Invoice_id
                          INNER JOIN C_OrderLine ol ON ol.C_OrderLine_ID = il.C_OrderLine_ID
                          WHERE ol.C_Order_ID  = " + C_Order_ID + @" AND i.DocStatus NOT IN ('RE' , 'VO')) t";
            int _countOrder = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));

            //check order id exist on LCDetail or PODetail or SODetail. if exist then not able to reverse the current order 
            if (_countOrder == 0 && Env.IsModuleInstalled("VA026_"))
            {
                sql = @"SELECT SUM(Result) FROM (                                                  
                                SELECT COUNT(o.C_Order_ID) AS Result FROM VA026_LCDetail lc INNER JOIN C_Order o ON lc.C_Order_ID=o.C_Order_ID WHERE 
                                lc.DocStatus NOT IN ('RE' , 'VO') AND o.C_Order_ID=" + C_Order_ID + @"
                                UNION ALL
                                SELECT COUNT(o.C_Order_ID) AS Result FROM VA026_LCDetail lc INNER JOIN C_Order o ON lc.VA026_Order_ID=o.C_Order_ID WHERE 
                                lc.DocStatus NOT IN ('RE' , 'VO') AND o.C_Order_ID=" + C_Order_ID + @"
                                UNION ALL
                                SELECT COUNT(o.C_Order_ID) AS Result FROM VA026_PODetail po INNER JOIN VA026_LCDetail lc ON po.VA026_LCDetail_ID=lc.VA026_LCDetail_ID
                                INNER JOIN C_Order o ON po.C_Order_ID=o.C_Order_ID 
                                WHERE lc.DocStatus NOT IN ('RE' , 'VO') AND  o.C_Order_ID=" + C_Order_ID + @"
                                UNION ALL
                                SELECT COUNT(o.C_Order_ID) AS Result FROM VA026_SODetail so INNER JOIN VA026_LCDetail lc ON so.VA026_LCDetail_ID=lc.VA026_LCDetail_ID
                                INNER JOIN C_Order o ON so.C_Order_ID=o.C_Order_ID 
                                WHERE lc.DocStatus NOT IN ('RE' , 'VO') AND  o.C_Order_ID=" + C_Order_ID + @") t";

                _countOrder = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));
            }

            if (_countOrder > 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// To create payments when POS doc type 
        /// </summary>
        /// <param name="info">to save log or append msg</param>
        /// <returns>payment object or null</returns>
        public MPayment CreatePaymentAgainstPOSDocType(StringBuilder info, MInvoice inv)
        {
            DataSet invSch = DB.ExecuteDataset(@"SELECT ci.C_InvoicePaySchedule_ID, ci.VA009_OpnAmntInvce, d.C_BankAccount_ID,
                                d.C_DocTypePayment_ID FROM C_InvoicePaySchedule ci 
                                INNER JOIN C_Invoice  i ON i.C_Invoice_ID = ci.C_Invoice_ID
                                INNER JOIN C_Order ord ON ord.C_Order_ID=i.C_Order_ID
                                INNER JOIN C_DocType  d ON d.C_DocType_ID = ord.C_DocType_ID 
                                WHERE i.C_Invoice_ID= " + GetC_Invoice_ID(), null, Get_Trx());
            MPayment _payment = new MPayment(GetCtx(), 0, Get_Trx());
            _payment.SetAD_Org_ID(inv.GetAD_Org_ID());
            _payment.SetAD_Client_ID(inv.GetAD_Client_ID());
            _payment.SetDateTrx(inv.GetDateAcct());
            _payment.SetDateAcct(inv.GetDateAcct());
            _payment.SetC_BPartner_ID(inv.GetC_BPartner_ID());
            _payment.SetC_BPartner_Location_ID(inv.GetC_BPartner_Location_ID());
            _payment.SetC_Invoice_ID(inv.GetC_Invoice_ID());
            _payment.SetC_Currency_ID(inv.GetC_Currency_ID());
            _payment.SetC_ConversionType_ID(inv.GetC_ConversionType_ID());
            _payment.SetVA009_PaymentMethod_ID(inv.GetVA009_PaymentMethod_ID());
            _payment.SetAD_OrgTrx_ID(inv.GetAD_OrgTrx_ID());
            _payment.SetDocStatus(DOCSTATUS_Drafted);
            if (invSch != null && invSch.Tables[0].Rows.Count > 0)
            {
                _payment.SetC_InvoicePaySchedule_ID(Util.GetValueOfInt(invSch.Tables[0].Rows[0]["C_InvoicePaySchedule_ID"]));
                _payment.SetPayAmt(Util.GetValueOfDecimal(invSch.Tables[0].Rows[0]["VA009_OpnAmntInvce"]));
                _payment.SetC_BankAccount_ID(Util.GetValueOfInt(invSch.Tables[0].Rows[0]["C_BankAccount_ID"]));
                _payment.SetC_DocType_ID(Util.GetValueOfInt(invSch.Tables[0].Rows[0]["C_DocTypePayment_ID"]));
            }
            if (!_payment.Save())
            {
                string msg = string.Empty;
                ValueNamePair pp = VLogger.RetrieveError();
                if (pp != null)
                {
                    msg = pp.GetName();
                    //if GetName is Empty then it will check GetValue
                    if (string.IsNullOrEmpty(msg))
                        msg = Msg.GetMsg("", pp.GetValue());
                }
                if (string.IsNullOrEmpty(msg))
                    msg = Msg.GetMsg(GetCtx(), "VIS_PaymentnotSaved");
                else
                    msg = Msg.GetMsg(GetCtx(), "VIS_PaymentnotSaved") + "," + msg;

                log.Info("Error occured while saving payment." + msg);
                _processMsg = msg;
                return null;
            }
            else
            {
                //to save the payment which is created by POS DocType Order because we need to save the payment in Drafted stage.
                if (Get_Trx() != null)
                {
                    Get_Trx().Commit();
                }

                if (_payment.CompleteIt().Equals(DOCACTION_Complete))
                {
                    _payment.SetProcessed(true);
                    _payment.SetDocStatus(DOCSTATUS_Completed);
                    _payment.SetDocAction(DOCACTION_Close);
                    if (_payment.Save())
                    {
                        info.Append(" & @C_Payment_ID@: " + _payment.GetDocumentNo());
                    }
                }
                else
                {
                    info.Append(" & @C_Payment_ID@: " + _payment.GetDocumentNo() + " (" + _payment.GetProcessMsg() + ")");
                    //TO reverse the eftects of completion  of payment because Alocation is generating always if we don't rolback it will set invoice as Paid
                    Get_Trx().Rollback();
                }
                _processMsg = "";
            }
            return _payment;
        }

        #region DocAction Members


        public Env.QueryParams GetLineOrgsQueryInfo()
        {
            return null;
        }

        public DateTime? GetDocumentDate()
        {
            return null;
        }

        public string GetDocBaseType()
        {
            return null;
        }

        public void SetProcessMsg(string processMsg)
        {
            _processMsg = processMsg;
        }

        private void SetCounterBPartner(MBPartner BPartner, int counterAdOrgId, int counterMWarehouseId)
        {
            counterBPartner = BPartner;
            counterOrgId = counterAdOrgId;
            counterWarehouseId = counterMWarehouseId;
        }

        private MBPartner GetCounterBPartner()
        {
            return counterBPartner;
        }

        private int GetCounterOrgID()
        {
            return counterOrgId;
        }

        private int GetCounterWarehouseID()
        {
            return counterWarehouseId;
        }

        #endregion
    }

    public class RecordContainer
    {
        public int M_InoutLine_ID { get; set; }
        public int M_Locator_ID { get; set; }
        public int M_Product_ID { get; set; }
        public int M_ASI_ID { get; set; }
        public int M_ProductContainer_ID { get; set; }
    }


    public class BudgetControl
    {
        public int GL_Budget_ID { get; set; }
        public int GL_BudgetControl_ID { get; set; }
        public int C_AcctSchema_ID { get; set; }
        public int Account_ID { get; set; }
        public int AD_Org_ID { get; set; }
        public int M_Product_ID { get; set; }
        public int C_BPartner_ID { get; set; }
        public int C_Activity_ID { get; set; }
        public int C_LocationFrom_ID { get; set; }
        public int C_LocationTo_ID { get; set; }
        public int C_Campaign_ID { get; set; }
        public int AD_OrgTrx_ID { get; set; }
        public int C_Project_ID { get; set; }
        public int C_SalesRegion_ID { get; set; }
        public int UserElement1_ID { get; set; }
        public int UserElement2_ID { get; set; }
        public int UserElement3_ID { get; set; }
        public int UserElement4_ID { get; set; }
        public int UserElement5_ID { get; set; }
        public int UserElement6_ID { get; set; }
        public int UserElement7_ID { get; set; }
        public int UserElement8_ID { get; set; }
        public int UserElement9_ID { get; set; }
        public int UserList1_ID { get; set; }
        public int UserList2_ID { get; set; }
        public Decimal ControlledAmount { get; set; }
        public String WhereClause { get; set; }
    }

}
