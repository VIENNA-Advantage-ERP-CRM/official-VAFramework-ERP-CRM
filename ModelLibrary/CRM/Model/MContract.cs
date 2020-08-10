using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
//using VAdvantage.Common;
//using System.Windows.Forms;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Process;
//using java.io;
//using java.util.zip;

namespace VAdvantage.Model
{
    public class MContract : X_C_Contract, DocAction
    {
        #region Variables
        /**	Process Message 			*/
        private String _processMsg = null;
        /**	Order Lines					*/
        // private MOrderLine[] _lines = null;
        /**	Tax Lines					*/
        // private MOrderTax[] _taxes = null;
        /** Force Creation of order		*/
        private bool _forceCreation = false;
        /**	Just Prepared Flag			*/
        private bool _justPrepared = false;

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
        public static MContract CopyFrom(MContract from, DateTime? dateDoc, int C_DocTypeTarget_ID, bool counter, bool copyASI, Trx trxName)
        {
            MContract to = new MContract(from.GetCtx(), 0, trxName);

            //to.Set_TrxName(trxName);
            //VAdvantage.Model.PO.CopyValues(from, to, from.GetAD_Client_ID(), from.GetAD_Org_ID());
            //to.Set_ValueNoCheck("C_Order_ID", I_ZERO);
            //to.Set_ValueNoCheck("DocumentNo", null);
            ////
            //to.SetDocStatus(DOCSTATUS_Drafted);		//	Draft
            //to.SetDocAction(DOCACTION_Complete);
            ////
            //to.SetC_DocType_ID(0);
            //to.SetC_DocTypeTarget_ID(C_DocTypeTarget_ID, true);
            ////
            //to.SetIsSelected(false);
            //to.SetDateOrdered(dateDoc);
            //to.SetDateAcct(dateDoc);
            //to.SetDatePromised(dateDoc);	//	assumption
            //to.SetDatePrinted(null);
            //to.SetIsPrinted(false);
            ////
            //to.SetIsApproved(false);
            //to.SetIsCreditApproved(false);
            //to.SetC_Payment_ID(0);
            //to.SetC_CashLine_ID(0);
            ////	Amounts are updated  when adding lines
            //to.SetGrandTotal(Env.ZERO);
            //to.SetTotalLines(Env.ZERO);
            ////
            //to.SetIsDelivered(false);
            //to.SetIsInvoiced(false);
            //to.SetIsSelfService(false);
            //to.SetIsTransferred(false);
            //to.SetPosted(false);
            //to.SetProcessed(false);
            //if (counter)
            //{
            //    to.SetRef_Order_ID(from.GetC_Order_ID());
            //}
            //else
            //{
            //    to.SetRef_Order_ID(0);
            //}
            ////
            //if (!to.Save(trxName))
            //{
            //    throw new Exception("Could not create Order");
            //}
            //if (counter)
            //{
            //    from.SetRef_Order_ID(to.GetC_Order_ID());
            //}

            //if (to.CopyLinesFrom(from, counter, copyASI) == 0)
            //{
            //    throw new Exception("Could not create Order Lines");
            //}

            return to;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_Order_ID"></param>
        /// <param name="trxName"></param>
        public MContract(Ctx ctx, int C_Contract_ID, Trx trxName)
            : base(ctx, C_Contract_ID, trxName)
        {


        }

        /*  Project Constructor
        *  @param  project Project to create Order from
        *  @param IsSOTrx sales order
        * 	@param	DocSubTypeSO if SO DocType Target (default DocSubTypeSO_OnCredit)
        */
        //public MContract(MProject project, bool IsSOTrx, String DocSubTypeSO)
        //    : this(project.GetCtx(), 0, project.Get_TrxName())
        //{


        //    SetAD_Client_ID(project.GetAD_Client_ID());
        //    SetAD_Org_ID(project.GetAD_Org_ID());
        //    SetC_Campaign_ID(project.GetC_Campaign_ID());
        //    SetSalesRep_ID(project.GetSalesRep_ID());
        //    //
        //    SetC_Project_ID(project.GetC_Project_ID());
        //    SetDescription(project.GetName());
        //    DateTime? ts = project.GetDateContract();
        //    if (ts != null)
        //        SetDateOrdered(ts);
        //    ts = project.GetDateFinish();
        //    if (ts != null)
        //        SetDatePromised(ts);
        //    //
        //    SetC_BPartner_ID(project.GetC_BPartner_ID());
        //    SetC_BPartner_Location_ID(project.GetC_BPartner_Location_ID());
        //    SetAD_User_ID(project.GetAD_User_ID());
        //    //
        //    SetM_Warehouse_ID(project.GetM_Warehouse_ID());
        //    SetM_PriceList_ID(project.GetM_PriceList_ID());
        //    SetC_PaymentTerm_ID(project.GetC_PaymentTerm_ID());
        //    //
        //    SetIsSOTrx(IsSOTrx);
        //    if (IsSOTrx)
        //    {
        //        if (DocSubTypeSO == null || DocSubTypeSO.Length == 0)
        //            SetC_DocTypeTarget_ID(DocSubTypeSO_OnCredit);
        //        else
        //            SetC_DocTypeTarget_ID(DocSubTypeSO);
        //    }
        //    else
        //    {
        //        SetC_DocTypeTarget_ID();
        //    }

        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MContract(Ctx ctx, DataRow dr, Trx trxName)
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
            ////try
            ////{
            ////    if (bp == null || !bp.IsActive())
            ////        return;

            ////    SetC_BPartner_ID(bp.GetC_BPartner_ID());
            ////    //	Defaults Payment Term
            ////    int ii = 0;
            ////    if (IsSOTrx())
            ////        ii = bp.GetC_PaymentTerm_ID();
            ////    else
            ////        ii = bp.GetPO_PaymentTerm_ID();
            ////    if (ii != 0)
            ////        SetC_PaymentTerm_ID(ii);
            ////    //	Default Price List
            ////    if (IsSOTrx())
            ////        ii = bp.GetM_PriceList_ID();
            ////    else
            ////        ii = bp.GetPO_PriceList_ID();
            ////    if (ii != 0)
            ////        SetM_PriceList_ID(ii);
            ////    //	Default Delivery/Via Rule
            ////    String ss = bp.GetDeliveryRule();
            ////    if (ss != null)
            ////        SetDeliveryRule(ss);
            ////    ss = bp.GetDeliveryViaRule();
            ////    if (ss != null)
            ////        SetDeliveryViaRule(ss);
            ////    //	Default Invoice/Payment Rule
            ////    ss = bp.GetInvoiceRule();
            ////    if (ss != null)
            ////        SetInvoiceRule(ss);
            ////    if (IsSOTrx())
            ////        ss = bp.GetPaymentRule();
            ////    else
            ////        ss = bp.GetPaymentRulePO();
            ////    if (ss != null)
            ////        SetPaymentRule(ss);
            ////    //	Sales Rep
            ////    ii = bp.GetSalesRep_ID();
            ////    if (ii != 0)
            ////        SetSalesRep_ID(ii);


            ////    //	Set Locations
            ////    MBPartnerLocation[] locs = bp.GetLocations(false);
            ////    if (locs != null)
            ////    {
            ////        for (int i = 0; i < locs.Length; i++)
            ////        {
            ////            if (locs[i].IsShipTo())
            ////                base.SetC_BPartner_Location_ID(locs[i].GetC_BPartner_Location_ID());
            ////            if (locs[i].IsBillTo())
            ////                SetBill_Location_ID(locs[i].GetC_BPartner_Location_ID());
            ////        }
            ////        //	set to first
            ////        if (GetC_BPartner_Location_ID() == 0 && locs.Length > 0)
            ////            base.SetC_BPartner_Location_ID(locs[0].GetC_BPartner_Location_ID());
            ////        if (GetBill_Location_ID() == 0 && locs.Length > 0)
            ////            SetBill_Location_ID(locs[0].GetC_BPartner_Location_ID());
            ////    }
            ////    if (GetC_BPartner_Location_ID() == 0)
            ////    {
            ////        log.Log(Level.SEVERE, "MOrder.setBPartner - Has no Ship To Address: " + bp);
            ////    }
            ////    if (GetBill_Location_ID() == 0)
            ////    {
            ////        log.Log(Level.SEVERE, "MOrder.setBPartner - Has no Bill To Address: " + bp);
            ////    }

            ////    //	Set Contact
            ////    VAdvantage.Model.MUser[] contacts = bp.GetContacts(false);
            ////    if (contacts != null && contacts.Length == 1)
            ////    {
            ////        SetAD_User_ID(contacts[0].GetAD_User_ID());
            ////    }
            ////}
            ////catch (Exception ex)
            ////{
            ////    //// MessageBox.Show("MOrder--SetBPartner");
            ////}
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
            //if (newC_BPartner_ID == null || newC_BPartner_ID.Length == 0)
            //    return;
            //int C_BPartner_ID = Convert.ToInt32(newC_BPartner_ID);
            //if (C_BPartner_ID == 0)
            //    return;

            //// Skip these steps for RMA. These fields are copied over from the orignal order instead.
            //if (IsReturnTrx())
            //    return;

            //String sql = "SELECT p.AD_Language,p.C_PaymentTerm_ID,"
            //    + " COALESCE(p.M_PriceList_ID,g.M_PriceList_ID) AS M_PriceList_ID, p.PaymentRule,p.POReference,"
            //    + " p.SO_Description,p.IsDiscountPrinted,"
            //    + " p.InvoiceRule,p.DeliveryRule,p.FreightCostRule,DeliveryViaRule,"
            //    + " p.SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
            //    + " lship.C_BPartner_Location_ID,c.AD_User_ID,"
            //    + " COALESCE(p.PO_PriceList_ID,g.PO_PriceList_ID) AS PO_PriceList_ID, p.PaymentRulePO,p.PO_PaymentTerm_ID,"
            //    + " lbill.C_BPartner_Location_ID AS Bill_Location_ID, p.SOCreditStatus, lbill.IsShipTo "
            //    + "FROM C_BPartner p"
            //    + " INNER JOIN C_BP_Group g ON (p.C_BP_Group_ID=g.C_BP_Group_ID)"
            //    + " LEFT OUTER JOIN C_BPartner_Location lbill ON (p.C_BPartner_ID=lbill.C_BPartner_ID AND lbill.IsBillTo='Y' AND lbill.IsActive='Y')"
            //    + " LEFT OUTER JOIN C_BPartner_Location lship ON (p.C_BPartner_ID=lship.C_BPartner_ID AND lship.IsShipTo='Y' AND lship.IsActive='Y')"
            //    + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
            //    + "WHERE p.C_BPartner_ID=" + C_BPartner_ID + " AND p.IsActive='Y'";		//	#1

            //bool isSOTrx = IsSOTrx();

            //DataTable dt = null;

            //try
            //{

            //    IDataReader idr = DB.ExecuteReader(sql, null, null);
            //    dt = new DataTable();
            //    dt.Load(idr);
            //    idr.Close();
            //    foreach (DataRow dr in dt.Rows)
            //    {

            //        base.SetC_BPartner_ID(C_BPartner_ID);

            //        //	PriceList (indirect: IsTaxIncluded & Currency)
            //        int ii = Util.GetValueOfInt(dr[isSOTrx ? "M_PriceList_ID" : "PO_PriceList_ID"].ToString());
            //        if (ii != 0)
            //            SetM_PriceList_ID(null, ii.ToString(), windowNo);
            //        else
            //        {	//	get default PriceList
            //            ii = GetCtx().GetContextAsInt("#M_PriceList_ID");
            //            if (ii != 0)
            //                SetM_PriceList_ID(null, ii.ToString(), windowNo);
            //        }

            //        //	Bill-To BPartner
            //        SetBill_BPartner_ID(C_BPartner_ID);
            //        int bill_Location_ID = Util.GetValueOfInt(dr["Bill_Location_ID"].ToString());
            //        if (bill_Location_ID == 0)
            //        {
            //            //   p_changeVO.addChangedValue("Bill_Location_ID", (String)null);
            //        }
            //        else
            //        {
            //            SetBill_Location_ID(bill_Location_ID);
            //        }

            //        // Ship-To Location
            //        int shipTo_ID = Util.GetValueOfInt(dr["C_BPartner_Location_ID"].ToString());
            //        //	overwritten by InfoBP selection - works only if InfoWindow
            //        //	was used otherwise creates error (uses last value, may belong to differnt BP)
            //        if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "C_BPartner_ID") == C_BPartner_ID)
            //        {
            //            String loc = GetCtx().GetContext(Env.WINDOW_INFO, Env.TAB_INFO, "C_BPartner_Location_ID");
            //            if (loc.Length > 0)
            //                shipTo_ID = int.Parse(loc);
            //        }
            //        if (shipTo_ID == 0)
            //        {
            //            // p_changeVO.addChangedValue("C_BPartner_Location_ID", (String)null);
            //        }
            //        else
            //        {
            //            SetC_BPartner_Location_ID(shipTo_ID);
            //        }
            //        if ("Y".Equals(dr["IsShipTo"].ToString()))	//	set the same
            //            SetBill_Location_ID(shipTo_ID);

            //        //	Contact - overwritten by InfoBP selection
            //        int contID = Util.GetValueOfInt(dr["AD_User_ID"].ToString());
            //        if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "C_BPartner_ID") == C_BPartner_ID)
            //        {
            //            String cont = GetCtx().GetContext(Env.WINDOW_INFO, Env.TAB_INFO, "AD_User_ID");
            //            if (cont.Length > 0)
            //                contID = int.Parse(cont);
            //        }
            //        SetAD_User_ID(contID);
            //        SetBill_User_ID(contID);

            //        //	CreditAvailable 
            //        if (isSOTrx)
            //        {
            //            Decimal CreditLimit = Util.GetValueOfDecimal(dr["SO_CreditLimit"].ToString());
            //            //	String SOCreditStatus = dr.getString("SOCreditStatus");
            //            if (CreditLimit != null && Env.Signum(CreditLimit) != 0)
            //            {
            //                Decimal CreditAvailable = Util.GetValueOfDecimal(dr["CreditAvailable"].ToString());
            //                //if (p_changeVO != null && CreditAvailable != null && CreditAvailable.signum() < 0)
            //                //{
            //                //    String msg = Msg.getMsg(GetCtx(), "CreditLimitOver",DisplayType.getNumberFormat(DisplayType.Amount).format(CreditAvailable));
            //                //    p_changeVO.addError(msg);
            //                //}
            //            }
            //        }

            //        //	VAdvantage.Model.PO Reference
            //        String s = dr["POReference"].ToString();
            //        if (s != null && s.Length != 0)
            //            SetPOReference(s);

            //        //	SO Description
            //        s = dr["SO_Description"].ToString();
            //        if (s != null && s.Trim().Length != 0)
            //            SetDescription(s);
            //        //	IsDiscountPrinted
            //        s = dr["IsDiscountPrinted"].ToString();
            //        SetIsDiscountPrinted("Y".Equals(s));

            //        //	Defaults, if not Walk-in Receipt or Walk-in Invoice
            //        String OrderType = GetCtx().GetContext(windowNo, "OrderType");
            //        SetInvoiceRule(INVOICERULE_AfterDelivery);
            //        SetDeliveryRule(DELIVERYRULE_Availability);
            //        SetPaymentRule(PAYMENTRULE_OnCredit);
            //        if (OrderType.Equals(DocSubTypeSO_Prepay))
            //        {
            //            SetInvoiceRule(INVOICERULE_Immediate);
            //            SetDeliveryRule(DELIVERYRULE_AfterReceipt);
            //        }
            //        else if (OrderType.Equals(MOrder.DocSubTypeSO_POS))	//  for POS
            //            SetPaymentRule(PAYMENTRULE_Cash);
            //        else
            //        {
            //            //	PaymentRule
            //            s = dr[isSOTrx ? "PaymentRule" : "PaymentRulePO"].ToString();
            //            if (s != null && s.Length != 0)
            //            {
            //                if (s.Equals("B"))				//	No Cache in Non POS
            //                    s = PAYMENTRULE_OnCredit;	//  Payment Term
            //                if (isSOTrx && (s.Equals("S") || s.Equals("U")))	//	No Check/Transfer for SO_Trx
            //                    s = PAYMENTRULE_OnCredit;	//  Payment Term
            //                SetPaymentRule(s);
            //            }
            //            //	Payment Term
            //            ii = Util.GetValueOfInt(dr[isSOTrx ? "C_PaymentTerm_ID" : "PO_PaymentTerm_ID"].ToString());
            //            if (ii != 0)
            //                SetC_PaymentTerm_ID(ii);
            //            //	InvoiceRule
            //            s = dr["InvoiceRule"].ToString();
            //            if (s != null && s.Length != 0)
            //                SetInvoiceRule(s);
            //            //	DeliveryRule
            //            s = dr["DeliveryRule"].ToString();
            //            if (s != null && s.Length != 0)
            //                SetDeliveryRule(s);
            //            //	FreightCostRule
            //            s = dr["FreightCostRule"].ToString();
            //            if (s != null && s.Length != 0)
            //                SetFreightCostRule(s);
            //            //	DeliveryViaRule
            //            s = dr["DeliveryViaRule"].ToString();
            //            if (s != null && s.Length != 0)
            //                SetDeliveryViaRule(s);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // MessageBox.Show("MOrder--SetC_BPartner_ID");
            //}
            //finally { dt = null; }
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
            //if (newBill_BPartner_ID == null || newBill_BPartner_ID.Length == 0)
            //    return;
            //int bill_BPartner_ID = int.Parse(newBill_BPartner_ID);
            //if (bill_BPartner_ID == 0)
            //    return;

            //// Skip these steps for RMA. These fields are copied over from the orignal order instead.
            //if (IsReturnTrx())
            //    return;

            //String sql = "SELECT p.AD_Language,p.C_PaymentTerm_ID,"
            //    + "p.M_PriceList_ID,p.PaymentRule,p.POReference,"
            //    + "p.SO_Description,p.IsDiscountPrinted,"
            //    + "p.InvoiceRule,p.DeliveryRule,p.FreightCostRule,DeliveryViaRule,"
            //    + "p.SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
            //    + "c.AD_User_ID,"
            //    + "p.PO_PriceList_ID, p.PaymentRulePO, p.PO_PaymentTerm_ID,"
            //    + "lbill.C_BPartner_Location_ID AS Bill_Location_ID "
            //    + "FROM C_BPartner p"
            //    + " LEFT OUTER JOIN C_BPartner_Location lbill ON (p.C_BPartner_ID=lbill.C_BPartner_ID AND lbill.IsBillTo='Y' AND lbill.IsActive='Y')"
            //    + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
            //    + "WHERE p.C_BPartner_ID=" + bill_BPartner_ID + " AND p.IsActive='Y'";		//	#1

            //bool isSOTrx = IsSOTrx();
            //DataTable dt = null;
            //try
            //{

            //    IDataReader idr = DB.ExecuteReader(sql, null, null);
            //    dt = new DataTable();
            //    dt.Load(idr);
            //    idr.Close();
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        base.SetBill_BPartner_ID(bill_BPartner_ID);
            //        //	PriceList (indirect: IsTaxIncluded & Currency)
            //        int ii = Util.GetValueOfInt(dr[isSOTrx ? "M_PriceList_ID" : "PO_PriceList_ID"].ToString());
            //        if (ii != 0)
            //            SetM_PriceList_ID(null, ii.ToString(), windowNo);
            //        else
            //        {	//	get default PriceList
            //            ii = GetCtx().GetContextAsInt("#M_PriceList_ID");
            //            if (ii != 0)
            //                SetM_PriceList_ID(null, ii.ToString(), windowNo);
            //        }

            //        int bill_Location_ID = Util.GetValueOfInt(dr["Bill_Location_ID"].ToString());
            //        //	overwritten by InfoBP selection - works only if InfoWindow
            //        //	was used otherwise creates error (uses last value, may belong to differnt BP)
            //        if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "C_BPartner_ID") == bill_BPartner_ID)
            //        {
            //            String loc = GetCtx().GetContext(Env.WINDOW_INFO, Env.TAB_INFO, "C_BPartner_Location_ID");
            //            if (loc.Length > 0)
            //                bill_Location_ID = int.Parse(loc);
            //        }
            //        if (bill_Location_ID != 0)
            //            SetBill_Location_ID(bill_Location_ID);

            //        //	Contact - overwritten by InfoBP selection
            //        int contID = Util.GetValueOfInt(dr["AD_User_ID"].ToString());
            //        if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "C_BPartner_ID") == bill_BPartner_ID)
            //        {
            //            String cont = GetCtx().GetContext(Env.WINDOW_INFO, Env.TAB_INFO, "AD_User_ID");
            //            if (cont.Length > 0)
            //                contID = int.Parse(cont);
            //        }
            //        SetBill_User_ID(contID);

            //        //	CreditAvailable 
            //        if (isSOTrx)
            //        {
            //            Decimal CreditLimit = Util.GetValueOfDecimal(dr["SO_CreditLimit"].ToString());
            //            //	String SOCreditStatus = dr.getString("SOCreditStatus");
            //            if (CreditLimit != null && Env.Signum(CreditLimit) != 0)
            //            {
            //                Decimal CreditAvailable = Util.GetValueOfDecimal(dr["CreditAvailable"].ToString());
            //                //if (p_changeVO != null && CreditAvailable != null && Env.Signum(CreditAvailable) < 0)
            //                //{
            //                //    String msg = Msg.getMsg(GetCtx(), "CreditLimitOver",DisplayType.getNumberFormat(DisplayType.Amount).format(CreditAvailable));
            //                //    p_changeVO.addError(msg);
            //                //}
            //            }
            //        }

            //        //	VAdvantage.Model.PO Reference
            //        String s = dr["POReference"].ToString();

            //        // Order Reference should not be reset by Bill To BPartner; only by BPartner 
            //        /*if (s != null && s.Length != 0)
            //            setPOReference(s); */
            //        //	SO Description
            //        s = dr["SO_Description"].ToString();
            //        if (s != null && s.Trim().Length != 0)
            //            SetDescription(s);
            //        //	IsDiscountPrinted
            //        s = dr["IsDiscountPrinted"].ToString();
            //        SetIsDiscountPrinted("Y".Equals(s));

            //        //	Defaults, if not Walk-in Receipt or Walk-in Invoice
            //        //	Defaults, if not Walk-in Receipt or Walk-in Invoice
            //        String OrderType = GetCtx().GetContext(windowNo, "OrderType");
            //        SetInvoiceRule(INVOICERULE_AfterDelivery);
            //        SetPaymentRule(PAYMENTRULE_OnCredit);
            //        if (OrderType.Equals(DocSubTypeSO_Prepay))
            //            SetInvoiceRule(INVOICERULE_Immediate);
            //        else if (OrderType.Equals(MOrder.DocSubTypeSO_POS))	//  for POS
            //            SetPaymentRule(PAYMENTRULE_Cash);
            //        else
            //        {
            //            //	PaymentRule
            //            s = dr[isSOTrx ? "PaymentRule" : "PaymentRulePO"].ToString();
            //            if (s != null && s.Length != 0)
            //            {
            //                if (s.Equals("B"))				//	No Cache in Non POS
            //                    s = PAYMENTRULE_OnCredit;	//  Payment Term
            //                if (isSOTrx && (s.Equals("S") || s.Equals("U")))	//	No Check/Transfer for SO_Trx
            //                    s = PAYMENTRULE_OnCredit;	//  Payment Term
            //                SetPaymentRule(s);
            //            }
            //            //	Payment Term
            //            ii = Util.GetValueOfInt(dr[isSOTrx ? "C_PaymentTerm_ID" : "PO_PaymentTerm_ID"].ToString());
            //            if (ii != 0)
            //                SetC_PaymentTerm_ID(ii);
            //            //	InvoiceRule
            //            s = dr["InvoiceRule"].ToString();
            //            if (s != null && s.Length != 0)
            //                SetInvoiceRule(s);
            //        }
            //    }

            //    //dt.Dispose();
            //}
            //catch (Exception ex)
            //{
            //    // MessageBox.Show("MOrder--SetC_BPartner_ID-CallOut");
            //}
            //finally
            //{
            //    dt = null;
            //}
        }


        /**
         * 	Set Business Partner Location (Ship+Bill)
         *	@param C_BPartner_Location_ID bp location
         */
        public void SetC_BPartner_Location_ID(int C_BPartner_Location_ID)
        {
            SetC_BPartner_Location_ID(C_BPartner_Location_ID);
            SetBill_Location_ID(C_BPartner_Location_ID);
        }


        /// <summary>
        /// Set Business Partner Contact (Ship+Bill)
        /// </summary>
        /// <param name="AD_User_ID">user</param>
        public void SetAD_User_ID(int AD_User_ID)
        {
            SetAD_User_ID(AD_User_ID);
            SetBill_User_ID(AD_User_ID);
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
            SetC_BPartner_Location_ID(C_BPartner_Location_ID);
        }

        /**
         * 	Set Ship Business Partner Contact
         *	@param AD_User_ID user
         */
        public void SetShip_User_ID(int AD_User_ID)
        {
            SetAD_User_ID(AD_User_ID);
        }


        /**
         * 	Set Warehouse
         *	@param M_Warehouse_ID warehouse
         */
        public void SetM_Warehouse_ID(int M_Warehouse_ID)
        {
            // base.SetM_Warehouse_ID(M_Warehouse_ID);
        }

        /**
         * 	Set Drop Ship
         *	@param IsDropShip drop ship
         */
        public void SetIsDropShip(bool IsDropShip)
        {
            // base.SetIsDropShip(IsDropShip);
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
                DateTime? dateOrdered = (DateTime?)VAdvantage.Model.PO.ConvertToTimestamp(newDateOrdered);
                if (dateOrdered == null)
                {
                    return;
                }
                SetDateOrdered(dateOrdered);
            }
            catch
            {
                //  // MessageBox.Show("MOrder--SetDateOrdered");
            }
        }

        /**
         *	Set Date Ordered and Acct Date
         */
        public void SetDateOrdered(DateTime? dateOrdered)
        {
            // base.SetDateOrdered(dateOrdered);
            //  base.SetDateAcct(dateOrdered);
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
            //  if (newC_DocTypeTarget_ID == null || newC_DocTypeTarget_ID.Length == 0)
            //      return;
            //  int C_DocTypeTarget_ID = int.Parse(newC_DocTypeTarget_ID);
            //  if (C_DocTypeTarget_ID == 0)
            //      return;

            //  //	Re-Create new DocNo, if there is a doc number already
            //  //	and the existing source used a different Sequence number
            //  String oldDocNo = GetDocumentNo();
            //  bool newDocNo = (oldDocNo == null);
            //  if (!newDocNo && oldDocNo.StartsWith("<") && oldDocNo.EndsWith(">"))
            //      newDocNo = true;
            ////  int oldC_DocType_ID = GetC_DocType_ID();

            //  String sql = "SELECT d.DocSubTypeSO,d.HasCharges,'N',"			//	1..3
            //      + "d.IsDocNoControlled,s.CurrentNext,s.CurrentNextSys,"     //  4..6
            //      + "s.AD_Sequence_ID,d.IsSOTrx,d.IsReturnTrx "               //	7..9
            //      + "FROM C_DocType d "
            //      + "LEFT OUTER JOIN AD_Sequence s ON (d.DocNoSequence_ID=s.AD_Sequence_ID) "
            //      + "WHERE C_DocType_ID=";	//	#1
            //  DataTable dt = null;
            //  try
            //  {
            //      int AD_Sequence_ID = 0;

            //      IDataReader idr = null;
            //      //	Get old AD_SeqNo for comparison
            //      if (!newDocNo)
            //      {

            //          idr = DB.ExecuteReader(sql, null, null);
            //          dt = new DataTable();
            //          dt.Load(idr);
            //          idr.Close();
            //          foreach (DataRow dr in dt.Rows)
            //          {
            //              AD_Sequence_ID = Util.GetValueOfInt(dr[5].ToString());
            //          }
            //          dt = null;
            //      }
            //      sql = sql + C_DocTypeTarget_ID;
            //      idr = DB.ExecuteReader(sql, null, null);
            //      dt = new DataTable();
            //      dt.Load(idr);
            //      idr.Close();
            //      String DocSubTypeSO = "";
            //      bool isSOTrx = true;
            //      bool isReturnTrx = false;
            //      foreach (DataRow dr in dt.Rows)		//	we found document type
            //      {
            //         // base.SetC_DocTypeTarget_ID(C_DocTypeTarget_ID);
            //          //	Set Ctx:	Document Sub Type for Sales Orders
            //          DocSubTypeSO = dr[0].ToString();
            //          if (DocSubTypeSO == null)
            //              DocSubTypeSO = "--";
            //          //if (p_changeVO != null)
            //          //    p_changeVO.setContext(GetCtx(), windowNo, "OrderType", DocSubTypeSO);
            //          //	No Drop Ship other than Standard
            //          if (!DocSubTypeSO.Equals(DocSubTypeSO_Standard))
            //              SetIsDropShip(false);

            //          //	IsSOTrx
            //          if ("N".Equals(dr[7].ToString()))
            //              isSOTrx = false;
            //         // SetIsSOTrx(isSOTrx);

            //          // IsReturnTrx
            //          isReturnTrx = "Y".Equals(dr[8].ToString());
            //          SetIsReturnTrx(isReturnTrx);

            //          if (!isReturnTrx)
            //          {
            //              //	Delivery Rule
            //              //if (DocSubTypeSO.Equals(MOrder.DocSubTypeSO_POS))
            //              //    SetDeliveryRule(DELIVERYRULE_Force);
            //              //else if (DocSubTypeSO.Equals(MOrder.DocSubTypeSO_Prepay))
            //              //    SetDeliveryRule(DELIVERYRULE_AfterReceipt);
            //              //else
            //              //    SetDeliveryRule(DELIVERYRULE_Availability);

            //              ////	Invoice Rule
            //              //if (DocSubTypeSO.Equals(DocSubTypeSO_POS)
            //              //    || DocSubTypeSO.Equals(DocSubTypeSO_Prepay)
            //              //    || DocSubTypeSO.Equals(DocSubTypeSO_OnCredit))
            //              //    SetInvoiceRule(INVOICERULE_Immediate);
            //              //else
            //              //    SetInvoiceRule(INVOICERULE_AfterDelivery);


            //              ////	Payment Rule - POS Order
            //              //if (DocSubTypeSO.Equals(DocSubTypeSO_POS))
            //              //    SetPaymentRule(PAYMENTRULE_Cash);
            //              //else
            //              //    SetPaymentRule(PAYMENTRULE_OnCredit);

            //              //	Set Ctx: Charges
            //              //if (p_changeVO != null)
            //              //    p_changeVO.setContext(GetCtx(), windowNo, "HasCharges", dr.getString(2));
            //          }
            //          else
            //          {
            //              //if (DocSubTypeSO.Equals(MOrder.DocSubTypeSO_POS))
            //              //    SetDeliveryRule(DELIVERYRULE_Force);
            //              //else
            //              //    SetDeliveryRule(DELIVERYRULE_Manual);
            //          }

            //          //	DocumentNo
            //          if (dr[3].ToString().Equals("Y"))			//	IsDocNoControlled
            //          {
            //              if (!newDocNo && AD_Sequence_ID != Util.GetValueOfInt(dr[6].ToString()))
            //                  newDocNo = true;
            //              if (newDocNo)
            //                  {
            //                      SetDocumentNo("<" + dr[5].ToString() + ">");
            //                  }
            //                  else
            //                  {
            //                      SetDocumentNo("<" + dr[4].ToString() + ">");
            //                  }
            //          }
            //      }

            //      // Skip remaining steps for RMA. These are copied over from original order.
            //      if (isReturnTrx)
            //          return;

            //      //  When BPartner is changed, the Rules are not set if
            //      //  it is a POS or Credit Order (i.e. defaults from Standard BPartner)
            //      //  This re-reads the Rules and applies them.
            //      if (DocSubTypeSO.Equals(DocSubTypeSO_POS)
            //          || DocSubTypeSO.Equals(DocSubTypeSO_Prepay))    //  not for POS/PrePay
            //      {
            //          ;
            //      }
            //      else
            //      {
            //          int C_BPartner_ID = GetC_BPartner_ID();
            //          sql = "SELECT PaymentRule,C_PaymentTerm_ID,"            //  1..2
            //              + "InvoiceRule,DeliveryRule,"                       //  3..4
            //              + "FreightCostRule,DeliveryViaRule, "               //  5..6
            //              + "PaymentRulePO,PO_PaymentTerm_ID "
            //              + "FROM C_BPartner "
            //              + "WHERE C_BPartner_ID=" + C_BPartner_ID;		//	#1
            //          dt = null;
            //          idr = DB.ExecuteReader(sql, null, null);
            //          dt = new DataTable();
            //          dt.Load(idr);
            //          idr.Close();
            //          foreach (DataRow dr in dt.Rows)
            //          {
            //              //	PaymentRule
            //              String paymentRule = dr[isSOTrx ? "PaymentRule" : "PaymentRulePO"].ToString();
            //              if (paymentRule != null && paymentRule.Length != 0)
            //              {
            //                  //if (isSOTrx 	//	No Cash/Check/Transfer for SO_Trx
            //                  //    && (paymentRule.Equals(PAYMENTRULE_Cash)
            //                  //        || paymentRule.Equals(PAYMENTRULE_Check)
            //                  //        || paymentRule.Equals(PAYMENTRULE_DirectDeposit)))
            //                  //    paymentRule = PAYMENTRULE_OnCredit;				//  Payment Term
            //                  //if (!isSOTrx 	//	No Cash for PO_Trx
            //                  //        && (paymentRule.Equals(PAYMENTRULE_Cash)))
            //                  //    paymentRule = PAYMENTRULE_OnCredit;				//  Payment Term
            //                  //SetPaymentRule(paymentRule);
            //              }
            //              //	Payment Term
            //              int C_PaymentTerm_ID = Util.GetValueOfInt(dr[isSOTrx ? "C_PaymentTerm_ID" : "PO_PaymentTerm_ID"].ToString());
            //              if (C_PaymentTerm_ID != 0)
            //                  SetC_PaymentTerm_ID(C_PaymentTerm_ID);
            //              //	InvoiceRule
            //              String invoiceRule = dr[2].ToString();
            //              //if (invoiceRule != null && invoiceRule.Length != 0)
            //              //    SetInvoiceRule(invoiceRule);
            //              ////	DeliveryRule
            //              //String deliveryRule = dr[3].ToString();
            //              //if (deliveryRule != null && deliveryRule.Length != 0)
            //              //    SetDeliveryRule(deliveryRule);
            //              ////	FreightCostRule
            //              //String freightCostRule = dr[4].ToString();
            //              //if (freightCostRule != null && freightCostRule.Length != 0)
            //              //    SetFreightCostRule(freightCostRule);
            //              ////	DeliveryViaRule
            //              //String deliveryViaRule = dr[5].ToString();
            //              //if (deliveryViaRule != null && deliveryViaRule.Length != 0)
            //              //    SetDeliveryViaRule(deliveryViaRule);
            //          }
            //      }   //  re-read customer rules

            //  }
            //  catch (Exception e)
            //  {
            //      log.Log(Level.SEVERE, sql, e);
            //  }
            //  finally
            //  {
            //      dt = null;

            //  }
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
                int C_DocType_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                if (C_DocType_ID <= 0)
                {
                    log.Severe("Not found for AD_Client_ID=" + GetAD_Client_ID() + ", SubType=" + DocSubTypeSO_x);
                }
                else
                {
                    log.Fine("(SO) - " + DocSubTypeSO_x);
                    // SetC_DocTypeTarget_ID(C_DocType_ID);
                    // SetIsSOTrx(true);
                    SetIsReturnTrx(false);
                }
            }
            catch
            {
                //// MessageBox.Show("MOrder--SetC_DocTypeTarget_ID");
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
                //base.SetC_DocTypeTarget_ID(C_DocTypeTarget_ID);
                if (setReturnTrx)
                {
                    VAdvantage.Model.MDocType dt = VAdvantage.Model.MDocType.Get(GetCtx(), C_DocTypeTarget_ID);
                    //  SetIsSOTrx(dt.IsSOTrx());
                    SetIsReturnTrx(dt.IsReturnTrx());
                }
            }
            catch
            {
                //// MessageBox.Show("MOrder--SetC_DocTypeTarget_ID(int C_DocTypeTarget_ID, bool setReturnTrx)");
            }
        }

        /**
         * 	Set Target Document Type.
         * 	Standard Order or VAdvantage.Model.PO
         */
        public void SetC_DocTypeTarget_ID()
        {
            try
            {
                //if (IsSOTrx())		//	SO = Std Order
                //{
                //    SetC_DocTypeTarget_ID(DocSubTypeSO_Standard);
                //    return;
                //}
                //	VAdvantage.Model.PO
                String sql = "SELECT C_DocType_ID FROM C_DocType "
                    + "WHERE AD_Client_ID=" + GetAD_Client_ID() + " AND AD_Org_ID IN (0," + GetAD_Org_ID()
                    + ") AND DocBaseType='POO' AND IsReturnTrx='N' "
                    + "ORDER BY AD_Org_ID DESC, IsDefault DESC";
                int C_DocType_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                if (C_DocType_ID <= 0)
                {
                    log.Severe("No POO found for AD_Client_ID=" + GetAD_Client_ID());
                }
                else
                {
                    log.Fine("(VAdvantage.Model.PO) - " + C_DocType_ID);
                    //SetC_DocTypeTarget_ID(C_DocType_ID);
                    SetIsReturnTrx(false);
                }
            }
            catch
            {
                // // MessageBox.Show("MOrder--SetC_DocTypeTarget_ID()");
            }
        }

        /* 	Copy Lines From other Order
        *	@param otherOrder order
        *	@param counter set counter Info
        *	@param copyASI copy line attributes Attribute Set Instance, Resaouce Assignment
        *	@return number of lines copied
        */
        public int CopyLinesFrom(MContract otherOrder, bool counter, bool copyASI)
        {
            int count = 0;
            try
            {
                if (IsProcessed() || otherOrder == null)
                    return 0;
                //MOrderLine[] fromLines = otherOrder.GetLines(false, null);

                //for (int i = 0; i < fromLines.Length; i++)
                //{
                //    MOrderLine line = new MOrderLine(this);
                //    VAdvantage.Model.PO.CopyValues(fromLines[i], line, GetAD_Client_ID(), GetAD_Org_ID());
                //    line.SetC_Order_ID(GetC_Order_ID());
                //    line.SetOrder(this);
                //    line.Set_ValueNoCheck("C_OrderLine_ID", I_ZERO);	//	new
                //    //	References
                //    if (!copyASI)
                //    {
                //        line.SetM_AttributeSetInstance_ID(0);
                //        line.SetS_ResourceAssignment_ID(0);
                //    }
                //    if (counter)
                //        line.SetRef_OrderLine_ID(fromLines[i].GetC_OrderLine_ID());
                //    else
                //        line.SetRef_OrderLine_ID(0);
                //    //
                //    line.SetQtyDelivered(Env.ZERO);
                //    line.SetQtyInvoiced(Env.ZERO);
                //    line.SetQtyReserved(Env.ZERO);
                //    line.SetDateDelivered(null);
                //    line.SetDateInvoiced(null);
                //    //	Tax
                //    if (GetC_BPartner_ID() != otherOrder.GetC_BPartner_ID())
                //        line.SetTax();		//	recalculate
                //    //
                //    //
                //    line.SetProcessed(false);
                //    if (line.Save(Get_TrxName()))
                //        count++;
                //    //	Cross Link
                //    if (counter)
                //    {
                //        fromLines[i].SetRef_OrderLine_ID(line.GetC_OrderLine_ID());
                //        fromLines[i].Save(Get_TrxName());
                //    }
                //}
                //if (fromLines.Length != count)
                //{
                //    log.Log(Level.SEVERE, "Line difference - From=" + fromLines.Length + " <> Saved=" + count);
                //}
            }
            catch
            {
                // // MessageBox.Show("MOrder--CopyLinesFrom");
            }
            return count;
        }

        /*	String Representation
        *	@return Info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MOrder[")
                .Append(Get_ID()).Append("-").Append(GetDocumentNo())
                //.Append(",IsSOTrx=").Append(IsSOTrx())
                // .Append(",C_DocType_ID=").Append(GetC_DocType_ID())
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
            // VAdvantage.Model.MDocType dt = VAdvantage.Model.MDocType.Get(GetCtx(), GetC_DocType_ID());
            return GetDocumentNo();
        }

        /// <summary>
        /// Create PDF
        /// </summary>
        /// <returns>File or null</returns>
        public FileInfo CreatePDF()
        {
            try
            {
                string fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo()
                                    + ".txt"; //.pdf
                string filePath = Path.GetTempPath() + fileName;

                //File temp = File.createTempFile(Get_TableName() + Get_ID() + "_", ".pdf");
                //FileStream fOutStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

                FileInfo temp = new FileInfo(filePath);
                if (!temp.Exists)
                {
                    return CreatePDF(temp);
                }
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
            //ReportEngine re = ReportEngine.get(GetCtx(), ReportEngine.ORDER, GetC_Order_ID());
            //if (re == null)
            //    return null;
            //return re.getPDF(file);

            //Create a file to write to.
            //using (StreamWriter sw = file.CreateText())
            //{
            //    sw.WriteLine("Hello");
            //    sw.WriteLine("And");
            //    sw.WriteLine("Welcome");
            //}

            return file;

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
                // SetIsTaxIncluded(pl.IsTaxIncluded());
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
            try
            {

                IDataReader idr = DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    base.SetM_PriceList_ID(M_PriceList_ID);
                    //	Tax Included
                    // SetIsTaxIncluded("Y".Equals(dr[0].ToString()));
                    //	Price Limit Enforce
                    //if (p_changeVO != null)
                    //    p_changeVO.setContext(GetCtx(), windowNo, "EnforcePriceLimit", dr.getString(2));
                    //	Currency
                    int ii = Util.GetValueOfInt(dr[2].ToString());
                    SetC_Currency_ID(ii);
                    //	PriceList Version
                    //if (p_changeVO != null)
                    //    p_changeVO.setContext(GetCtx(), windowNo, "M_PriceList_Version_ID", dr.getInt(5));
                }

            }
            catch
            {

            }
            finally
            {
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
                //MBPartner bpartner = new MBPartner(GetCtx(), GetC_BPartner_ID(), null);
                //if (bpartner.Get_ID() != 0)
                //{
                //    if (IsSOTrx())
                //    {
                //        base.SetM_ReturnPolicy_ID(bpartner.GetM_ReturnPolicy_ID());
                //    }
                //    else
                //    {
                //        base.SetM_ReturnPolicy_ID(bpartner.GetPO_ReturnPolicy_ID());
                //    }
                //}
            }
            catch
            {
                // // MessageBox.Show("MOrder--SetM_ReturnPolicy_ID()");
            }

        }

        /* 	Set Original Order for RMA
         * 	SOTrx should be set.
         * 	@param origOrder MOrder
         */
        public void SetOrigOrder(MContract origOrder)
        {
            try
            {
                if (origOrder == null || origOrder.Get_ID() == 0)
                    return;

                // SetOrig_Order_ID(origOrder.GetC_Order_ID());
                //	Get Details from Original Order
                MBPartner bpartner = new MBPartner(GetCtx(), origOrder.GetC_BPartner_ID(), null);

                // Reset Original Shipment
                //  SetOrig_InOut_ID(-1);
                SetC_BPartner_ID(origOrder.GetC_BPartner_ID());
                //   SetC_BPartner_Location_ID(origOrder.GetC_BPartner_Location_ID());
                //   SetAD_User_ID(origOrder.GetAD_User_ID());
                SetBill_BPartner_ID(origOrder.GetBill_BPartner_ID());
                SetBill_Location_ID(origOrder.GetBill_Location_ID());
                SetBill_User_ID(origOrder.GetBill_User_ID());

                SetM_ReturnPolicy_ID();

                SetM_PriceList_ID(origOrder.GetM_PriceList_ID());
                //SetPaymentRule(origOrder.GetPaymentRule());
                SetC_PaymentTerm_ID(origOrder.GetC_PaymentTerm_ID());
                //setDeliveryRule(X_C_Order.DELIVERYRULE_Manual);

                SetBill_Location_ID(origOrder.GetBill_Location_ID());
                //SetInvoiceRule(origOrder.GetInvoiceRule());
                //SetPaymentRule(origOrder.GetPaymentRule());
                // SetDeliveryViaRule(origOrder.GetDeliveryViaRule());
                // SetFreightCostRule(origOrder.GetFreightCostRule());
            }
            catch
            {
                // // MessageBox.Show("MOrder--SetOrigOrder");
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
                MContract origOrder = new MContract(GetCtx(), Orig_Order_ID, null);
                if (origOrder.Get_ID() != 0)
                {
                    SetOrigOrder(origOrder);
                }
            }
            catch
            {
                // // MessageBox.Show("MOrder--SetOrig_Order_ID-callout");
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
                //  SetOrig_InOut_ID(origInOut.GetM_InOut_ID());
                // SetC_Project_ID(origInOut.GetC_Project_ID());
                SetC_Campaign_ID(origInOut.GetC_Campaign_ID());
                // SetC_Activity_ID(origInOut.GetC_Activity_ID());
                // SetAD_OrgTrx_ID(origInOut.GetAD_OrgTrx_ID());
                //  SetUser1_ID(origInOut.GetUser1_ID());
                //  SetUser2_ID(origInOut.GetUser2_ID());
            }
            catch
            {
                //  // MessageBox.Show("MOrder--SetOrigInOut");
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
                //MInOut origInOut = new MInOut(GetCtx(), Orig_InOut_ID, null);
                //if (origInOut.Get_ID() != 0)
                //    SetOrigInOut(origInOut);
            }
            catch
            {
                //// MessageBox.Show("MOrder--SetOrig_InOut_ID");
            }

        }

        /// <summary>
        /// Get Lines of Order
        /// </summary>
        /// <param name="whereClause">where clause or null (starting with AND)</param>
        /// <param name="orderClause">order clause</param>
        /// <returns>lines</returns>
        //public MOrderLine[] GetLines(String whereClause, String orderClause)
        //{
        //    List<MOrderLine> list = new List<MOrderLine>();
        //    StringBuilder sql = new StringBuilder("SELECT * FROM C_OrderLine WHERE C_Order_ID=" + GetC_Order_ID() + "");
        //    if (whereClause != null)
        //        sql.Append(whereClause);
        //    if (orderClause != null)
        //        sql.Append(" ").Append(orderClause);
        //    try
        //    {
        //        DataSet ds = DB.ExecuteDataset(sql.ToString(), null, Get_TrxName());
        //        if (ds.Tables.Count > 0)
        //        {
        //            foreach (DataRow dr in ds.Tables[0].Rows)
        //            {
        //                MOrderLine ol = new MOrderLine(GetCtx(), dr, Get_TrxName());
        //                ol.SetHeaderInfo(this);
        //                list.Add(ol);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        log.Log(Level.SEVERE, sql.ToString(), e);
        //    }
        //    //
        //    MOrderLine[] lines = new MOrderLine[list.Count];
        //    lines = list.ToArray();
        //    return lines;
        //}

        /// <summary>
        /// Get Lines of Order
        /// </summary>
        /// <param name="requery">requery</param>
        /// <param name="orderBy">optional order by column</param>
        /// <returns>lines</returns>
        //public MOrderLine[] GetLines(bool requery, String orderBy)
        //{
        //    try
        //    {
        //        if (_lines != null && !requery)
        //        {
        //            return _lines;
        //        }
        //        //
        //        String orderClause = "ORDER BY ";
        //        if (orderBy != null && orderBy.Length > 0)
        //        {
        //            orderClause += orderBy;
        //        }
        //        else
        //        {
        //            orderClause += "Line";
        //        }
        //        _lines = GetLines(null, orderClause);

        //    }
        //    catch (Exception e)
        //    {
        //        //// MessageBox.Show("MOrder--GetLines");
        //    }
        //    return _lines;
        //}

        /// <summary>
        /// Get Lines of Order.
        /// </summary>
        /// <returns>lines</returns>
        //public MOrderLine[] GetLines()
        //{
        //    return GetLines(false, null);
        //}

        /// <summary>
        /// Get Lines of Order for a given product
        /// </summary>
        /// <param name="M_Product_ID"></param>
        /// <param name="whereClause"></param>
        /// <param name="orderClause">order clause</param>
        /// <returns>lines</returns>
        /// <date>10-March-2011</date>
        /// <writer>raghu</writer>
        //public MOrderLine[] GetLines(int M_Product_ID, String whereClause, String orderClause)
        //{
        //    List<MOrderLine> list = new List<MOrderLine>();
        //    StringBuilder sql = new StringBuilder("SELECT * FROM C_OrderLine WHERE C_Order_ID=" + GetC_Order_ID() + " AND M_Product_ID=" + M_Product_ID);

        //    if (whereClause != null)
        //        sql.Append(" AND ").Append(whereClause);

        //    if (orderClause != null)
        //        sql.Append(" ORDER BY ").Append(orderClause);

        //    IDataReader idr = null;
        //    try
        //    {
        //        idr = DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
        //        DataTable dt = new DataTable();
        //        dt.Load(idr);
        //        idr.Close();

        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            MOrderLine ol = new MOrderLine(GetCtx(), dr, Get_TrxName());
        //            ol.SetHeaderInfo(this);
        //            list.Add(ol);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        log.Log(Level.SEVERE, sql.ToString(), e);
        //    }
        //    finally
        //    {
        //        if (idr != null)
        //        {

        //            idr.Close();
        //            idr = null;
        //        }
        //    }
        //    //
        //    MOrderLine[] lines = new MOrderLine[list.Count]; ;
        //    lines = list.ToArray();
        //    return lines;
        //}

        /// <summary>
        /// Get Lines of Order
        /// </summary>
        /// <param name="orderBy">optional order by column</param>
        /// <returns>lines</returns>
        //public MOrderLine[] GetLines(String orderBy)
        //{
        //    String orderClause = "ORDER BY ";
        //    if ((orderBy != null) && (orderBy.Length > 0))
        //    {
        //        orderClause += orderBy;
        //    }
        //    else
        //    {
        //        orderClause += "Line";
        //    }
        //    return GetLines(null, orderClause);
        //}

        /*	Renumber Lines
        *	@param step start and step
        */
        //public void RenumberLines(int step)
        //{
        //    int number = step;
        //    MOrderLine[] lines = GetLines(true, null);	//	Line is default
        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        MOrderLine line = lines[i];
        //        line.SetLine(number);
        //        line.Save(Get_TrxName());
        //        number += step;
        //    }
        //    _lines = null;
        //}

        /* 	Does the Order Line belong to this Order
         *	@param C_OrderLine_ID line
         *	@return true if part of the order
         */
        //public bool IsOrderLine(int C_OrderLine_ID)
        //{
        //    if (_lines == null)
        //        GetLines();
        //    for (int i = 0; i < _lines.Length; i++)
        //        if (_lines[i].GetC_OrderLine_ID() == C_OrderLine_ID)
        //            return true;
        //    return false;
        //}

        /* 	Get Taxes of Order
         *	@param requery requery
         *	@return array of taxes
         */
        //public MOrderTax[] GetTaxes(bool requery)
        //{
        //    if (_taxes != null && !requery)
        //        return _taxes;
        //    //
        //    List<MOrderTax> list = new List<MOrderTax>();
        //    String sql = "SELECT * FROM C_OrderTax WHERE C_Order_ID=" + GetC_Order_ID();
        //    DataTable dt = null;
        //    try
        //    {
        //        IDataReader idr = DB.ExecuteReader(sql, null, Get_TrxName());
        //        dt = new DataTable();
        //        dt.Load(idr);

        //        idr.Close();
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            list.Add(new MOrderTax(GetCtx(), dr, Get_TrxName()));
        //        }
        //        dt = null;
        //    }
        //    catch (Exception e)
        //    {
        //        log.Log(Level.SEVERE, sql, e);
        //    }
        //    finally
        //    {
        //        dt = null;
        //    }
        //    _taxes = new MOrderTax[list.Count];
        //    _taxes = list.ToArray();
        //    return _taxes;
        //}

        /*	Get Invoices of Order
        * 	@param hearderLinkOnly shipments based on header only
        * 	@return invoices
        */
        //public MInvoice[] GetInvoices(bool hearderLinkOnly)
        //{
        //    //	TODO get invoiced which are linked on line level
        //    List<MInvoice> list = new List<MInvoice>();
        //    String sql = "SELECT * FROM C_Invoice WHERE C_Order_ID=" + GetC_Order_ID() + " ORDER BY Created DESC";
        //    DataTable dt = null;
        //    try
        //    {
        //        IDataReader idr = DB.ExecuteReader(sql, null, Get_TrxName());
        //        dt = new DataTable();
        //        dt.Load(idr);
        //        idr.Close();
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            list.Add(new MInvoice(GetCtx(), dr, Get_TrxName()));
        //        }
        //        dt = null;
        //    }
        //    catch (Exception e)
        //    {
        //        log.Log(Level.SEVERE, sql, e);
        //    }
        //    finally { dt = null; }

        //    MInvoice[] retValue = new MInvoice[list.Count];
        //    retValue = list.ToArray();
        //    return retValue;
        //}

        /*	Get latest Invoice of Order
        * 	@return invoice id or 0
        */
        //public int GetC_Invoice_ID()
        //{
        //    int C_Invoice_ID = 0;
        //    String sql = "SELECT C_Invoice_ID FROM C_Invoice "
        //        + "WHERE C_Order_ID=" + GetC_Order_ID() + " AND DocStatus IN ('CO','CL') "
        //        + "ORDER BY Created DESC";
        //    DataTable dt = null;
        //    try
        //    {
        //        IDataReader idr = DB.ExecuteReader(sql, null, Get_TrxName());
        //        dt = new DataTable();
        //        dt.Load(idr);
        //        idr.Close();
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            //C_Invoice_ID =Convert.ToInt32(dr[0]);
        //            C_Invoice_ID = Util.GetValueOfInt(dr[0].ToString());
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        log.Log(Level.SEVERE, "getC_Invoice_ID", e);
        //    }
        //    finally { dt = null; }
        //    return C_Invoice_ID;
        //}

        /* 	Get Shipments of Order
         * 	@param hearderLinkOnly shipments based on header only
         * 	@return shipments
         */
        //public MInOut[] GetShipments(bool hearderLinkOnly)
        //{
        //    //	TODO: getShipment if linked on line
        //    List<MInOut> list = new List<MInOut>();
        //    String sql = "SELECT * FROM M_InOut WHERE C_Order_ID=" + GetC_Order_ID() + " ORDER BY Created DESC";
        //    DataTable dt = null;
        //    try
        //    {
        //        IDataReader idr = DB.ExecuteReader(sql, null, Get_TrxName());
        //        dt = new DataTable();
        //        dt.Load(idr);
        //        idr.Close();
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            list.Add(new MInOut(GetCtx(), dr, Get_TrxName()));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        log.Log(Level.SEVERE, sql, e);
        //    }
        //    finally { dt = null; }

        //    MInOut[] retValue = new MInOut[list.Count];
        //    retValue = list.ToArray();
        //    return retValue;
        //}

        /*	Get RMAs of Order
        * 	@return RMAs
        */
        //public MOrder[] GetRMAs()
        //{
        //    List<MOrder> list = new List<MOrder>();
        //    String sql = "SELECT * FROM C_Order WHERE Orig_Order_ID=" + GetC_Order_ID() + " ORDER BY Created DESC";
        //    DataTable dt = null;
        //    try
        //    {
        //        IDataReader idr = DB.ExecuteReader(sql, null, Get_TrxName());
        //        dt = new DataTable();
        //        dt.Load(idr);
        //        idr.Close();
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            list.Add(new MOrder(GetCtx(), dr, Get_TrxName()));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        log.Log(Level.SEVERE, sql, e);
        //    }
        //    finally { dt = null; }


        //    MOrder[] retValue = new MOrder[list.Count];
        //    retValue = list.ToArray();
        //    return retValue;
        //}

        /*	Get Shipment Lines of Order
        * 	@return shipments newest first
        */
        //public MInOutLine[] GetShipmentLines()
        //{
        //    List<MInOutLine> list = new List<MInOutLine>();
        //    String sql = "SELECT * FROM M_InOutLine iol "
        //        + "WHERE iol.C_OrderLine_ID IN "
        //            + "(SELECT C_OrderLine_ID FROM C_OrderLine WHERE C_Order_ID=@C_Order_ID) "
        //        + "ORDER BY M_InOutLine_ID";
        //    DataTable dt = null;
        //    try
        //    {
        //        SqlParameter[] param = new SqlParameter[1];
        //        param[0] = new SqlParameter("@C_Order_ID", GetC_Order_ID());
        //        IDataReader idr = DB.ExecuteReader(sql, param, Get_TrxName());
        //        dt = new DataTable();
        //        dt.Load(idr);
        //        idr.Close();
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            list.Add(new MInOutLine(GetCtx(), dr, Get_TrxName()));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        // MessageBox.Show("MOrder--GetShipmentLines");
        //    }
        //    finally { dt = null; }

        //    MInOutLine[] retValue = new MInOutLine[list.Count];
        //    retValue = list.ToArray();
        //    return retValue;
        //}

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
                + "' WHERE C_Contract_ID=" + GetC_Contract_ID();
            // int noLine = DB.ExecuteQuery("UPDATE C_OrderLine " + set, null, Get_TrxName());
            int noLine = DB.ExecuteQuery("UPDATE C_ContractSchedule " + set, null, Get_TrxName());
            //  int noTax = DB.ExecuteQuery("UPDATE C_OrderTax " + set, null, Get_TrxName());
            //_lines = null;
            //_taxes = null;
            log.Fine(processed + " - Lines=" + noLine + ", Tax=");
        }

        /* 	Before Save
        *	@param newRecord new
        *	@return save
        */
        protected override bool BeforeSave(bool newRecord)
        {
            //Neha----Can not save Service Contract in * Organization---12 Sep,2018
            if (GetAD_Org_ID() == 0)
            {
                log.SaveWarning("ValidateOrg", "");
                return false;
            }

            // calculate tax amount and surcharge amount 
            if (newRecord || Is_ValueChanged("C_Tax_ID") || Is_ValueChanged("M_PriceList_ID")
                || Is_ValueChanged("QtyEntered") || Is_ValueChanged("PriceActual") || Is_ValueChanged("Discount"))
            {
                CalculateAndUpdateTaxes();
            }

            return true;
        }

        /// <summary>
        /// Calculat and update Tax Amounts
        /// </summary>
        private void CalculateAndUpdateTaxes()
        {
            // PriceList Object
            MPriceList priceList = MPriceList.Get(GetCtx(), GetM_PriceList_ID(), Get_Trx());
            // Currency Object
            MCurrency currency = MCurrency.Get(GetCtx(), priceList.GetC_Currency_ID());
            //Tax Object 
            MTax tax = new MTax(GetCtx(), GetC_Tax_ID(), Get_Trx());

            Decimal surchargeAmt = Env.ZERO;
            Decimal TaxAmt = Env.ZERO;
            Decimal LineNetAmt = Decimal.Multiply(GetPriceActual(), GetQtyEntered());

            // whn surchage field available then calculate surcharge also
            if (Get_ColumnIndex("SurchargeAmt") > 0 && tax.Get_ColumnIndex("Surcharge_Tax_ID") > 0 && tax.GetSurcharge_Tax_ID() > 0)
            {
                TaxAmt = tax.CalculateSurcharge(LineNetAmt, priceList.IsTaxIncluded(), currency.GetStdPrecision(), out surchargeAmt);
            }
            else
            {
                TaxAmt = tax.CalculateTax(LineNetAmt, priceList.IsTaxIncluded(), currency.GetStdPrecision());
            }

            // set tax amount
            SetTaxAmt(TaxAmt);
            // update Surcharge amount
            if (Get_ColumnIndex("SurchargeAmt") > 0)
            {
                SetSurchargeAmt(surchargeAmt);
            }

            if (!priceList.IsTaxIncluded())
            {
                SetGrandTotal(LineNetAmt + TaxAmt + surchargeAmt);
            }
            else
            {
                SetGrandTotal(LineNetAmt);
            }

        }

        /* 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return true if can be saved
         */
        protected override bool AfterSave(bool newRecord, bool success)
        {

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        private void AfterSaveSync(String columnName)
        {
            if (Is_ValueChanged(columnName))
            {
                //String sql = "UPDATE C_OrderLine ol"
                //    + " SET " + columnName + " ="
                //        + "(SELECT " + columnName
                //        + " FROM C_Order o WHERE ol.C_Order_ID=o.C_Order_ID) "
                //    + "WHERE C_Order_ID=" + GetC_Order_ID();
                //int no = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                //log.Fine(columnName + " Lines -> #" + no);
            }
        }

        /* 	Before Delete
         *	@return true of it can be deleted
         */
        protected override bool BeforeDelete()
        {
            try
            {
                //if (IsProcessed())
                //    return false;

                //GetLines();
                //for (int i = 0; i < _lines.Length; i++)
                //{
                //    if (!_lines[i].BeforeDelete())
                //    {
                //        return false;
                //    }
                //}
            }
            catch
            {
                //// MessageBox.Show("Error in MOrder--BeforeDelete");
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
            //SetProcessing(false);
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
            VAdvantage.Model.MDocType dt = VAdvantage.Model.MDocType.Get(GetCtx(), 0);
            SetIsReturnTrx(dt.IsReturnTrx());
            // SetIsSOTrx(dt.IsSOTrx());

            //	Std Period open?
            //if (!VAdvantage.Model.MPeriod.IsOpen(GetCtx(), GetStartDate(), dt.GetDocBaseType()))
            //{
            //    _processMsg = "@PeriodClosed@";
            //    return DocActionVariables.STATUS_INVALID;
            //}

            //	Lines
            //MOrderLine[] lines = GetLines(true, "M_Product_ID");
            //if (lines.Length == 0)
            //{
            //    _processMsg = "@NoLines@";
            //    return DocActionVariables.STATUS_INVALID;
            //}

            //	Convert DocType to Target
            //if (GetC_DocType_ID() != GetC_DocTypeTarget_ID())
            //{
            //    //	Cannot change Std to anything else if different warehouses
            //    if (GetC_DocType_ID() != 0)
            //    {
            //        VAdvantage.Model.MDocType dtOld = VAdvantage.Model.MDocType.Get(GetCtx(), GetC_DocType_ID());
            //        if (VAdvantage.Model.MDocType.DOCSUBTYPESO_StandardOrder.Equals(dtOld.GetDocSubTypeSO())		//	From SO
            //            && !VAdvantage.Model.MDocType.DOCSUBTYPESO_StandardOrder.Equals(dt.GetDocSubTypeSO()))	//	To !SO
            //        {
            //            for (int i = 0; i < lines.Length; i++)
            //            {
            //                if (lines[i].GetM_Warehouse_ID() != GetM_Warehouse_ID())
            //                {
            //                    log.Warning("different Warehouse " + lines[i]);
            //                    _processMsg = "@CannotChangeDocType@";
            //                    return DocActionVariables.STATUS_INVALID;
            //                }
            //            }
            //        }
            //    }

            //    //	New or in Progress/Invalid
            //    if (DOCSTATUS_Drafted.Equals(GetDocStatus())
            //        || DOCSTATUS_InProgress.Equals(GetDocStatus())
            //        || DOCSTATUS_Invalid.Equals(GetDocStatus())
            //        || GetC_DocType_ID() == 0)
            //    {
            //        SetC_DocType_ID(GetC_DocTypeTarget_ID());
            //    }
            //    else	//	convert only if offer
            //    {
            //        if (dt.IsOffer())
            //            SetC_DocType_ID(GetC_DocTypeTarget_ID());
            //        else
            //        {
            //            _processMsg = "@CannotChangeDocType@";
            //            return DocActionVariables.STATUS_INVALID;
            //        }
            //    }
            //}	//	convert DocType

            //	Mandatory Product Attribute Set Instance
            //String mandatoryType = "='Y'";	//	IN ('Y','S')
            //String sql = "SELECT COUNT(*) "
            //    + "FROM C_OrderLine ol"
            //    + " INNER JOIN M_Product p ON (ol.M_Product_ID=p.M_Product_ID)"
            //    + " INNER JOIN M_AttributeSet pas ON (p.M_AttributeSet_ID=pas.M_AttributeSet_ID) "
            //    + "WHERE pas.MandatoryType" + mandatoryType
            //    + " AND ol.M_AttributeSetInstance_ID IS NULL"
            //    + " AND ol.C_Order_ID=" + GetC_Order_ID();
            //int no = DB.GetSQLValue(Get_TrxName(), sql);
            //if (no != 0)
            //{
            //    _processMsg = "@LinesWithoutProductAttribute@ (" + no + ")";
            //    return DocActionVariables.STATUS_INVALID;
            //}

            //	Lines
            //if (ExplodeBOM())
            //    lines = GetLines(true, "M_Product_ID");
            //if (!ReserveStock(dt, lines))
            //{
            //    _processMsg = "Cannot reserve Stock";
            //    return DocActionVariables.STATUS_INVALID;
            //}
            //if (!CalculateTaxTotal())
            //{
            //    _processMsg = "Error calculating tax";
            //    return DocActionVariables.STATUS_INVALID;
            //}

            //	Credit Check
            //if (IsSOTrx() && !IsReturnTrx())
            //{
            //    MBPartner bp = MBPartner.Get(GetCtx(), GetC_BPartner_ID());
            //    if (MBPartner.SOCREDITSTATUS_CreditStop.Equals(bp.GetSOCreditStatus()))
            //    {
            //        _processMsg = "@BPartnerCreditStop@ - @TotalOpenBalance@="
            //            + bp.GetTotalOpenBalance()
            //            + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
            //        return DocActionVariables.STATUS_INVALID;
            //    }
            //    if (MBPartner.SOCREDITSTATUS_CreditHold.Equals(bp.GetSOCreditStatus()))
            //    {
            //        _processMsg = "@BPartnerCreditHold@ - @TotalOpenBalance@="
            //            + bp.GetTotalOpenBalance()
            //            + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
            //        return DocActionVariables.STATUS_INVALID;
            //    }
            //    Decimal grandTotal = VAdvantage.Model.MConversionRate.ConvertBase(GetCtx(),
            //        GetGrandTotal(), GetC_Currency_ID(), GetDateOrdered(),
            //        GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID());
            //    if (MBPartner.SOCREDITSTATUS_CreditHold.Equals(bp.GetSOCreditStatus(grandTotal)))
            //    {
            //        _processMsg = "@BPartnerOverOCreditHold@ - @TotalOpenBalance@="
            //            + bp.GetTotalOpenBalance() + ", @GrandTotal@=" + grandTotal
            //            + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
            //        return DocActionVariables.STATUS_INVALID;
            //    }
            //}

            _justPrepared = true;
            // dont uncomment
            //if (!DOCACTION_Complete.Equals(getDocAction()))		don't set for just prepare 
            //		setDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /* 	Explode non stocked BOM.
         * 	@return true if bom exploded
         */
        //private bool ExplodeBOM()
        //{
        //    bool retValue = false;
        //    String where = "AND IsActive='Y' AND EXISTS "
        //        + "(SELECT * FROM M_Product p WHERE C_OrderLine.M_Product_ID=p.M_Product_ID"
        //        + " AND	p.IsBOM='Y' AND p.IsVerified='Y' AND p.IsStocked='N')";
        //    //
        //    String sql = "SELECT COUNT(*) FROM C_OrderLine "
        //        + "WHERE C_Order_ID=" + GetC_Order_ID() + where;
        //    int count = DB.GetSQLValue(Get_TrxName(), sql); //Convert.ToInt32(DB.ExecuteScalar(sql, null, Get_TrxName()));
        //    while (count != 0)
        //    {
        //        retValue = true;
        //        RenumberLines(1000);		//	max 999 bom items	

        //        //	Order Lines with non-stocked BOMs
        //        MOrderLine[] lines = GetLines(where, "ORDER BY Line");
        //        for (int i = 0; i < lines.Length; i++)
        //        {
        //            MOrderLine line = lines[i];
        //            MProduct product = MProduct.Get(GetCtx(), line.GetM_Product_ID());
        //            log.Fine(product.GetName());
        //            //	New Lines
        //            int lineNo = line.GetLine();
        //            MProductBOM[] boms = MProductBOM.GetBOMLines(product);
        //            for (int j = 0; j < boms.Length; j++)
        //            {
        //                MProductBOM bom = boms[j];
        //                MOrderLine newLine = new MOrderLine(this);
        //                newLine.SetLine(++lineNo);
        //                newLine.SetM_Product_ID(bom.GetProduct()
        //                    .GetM_Product_ID());
        //                newLine.SetC_UOM_ID(bom.GetProduct().GetC_UOM_ID());
        //                newLine.SetQty(Decimal.Multiply(line.GetQtyOrdered(), bom.GetBOMQty()));
        //                if (bom.GetDescription() != null)
        //                    newLine.SetDescription(bom.GetDescription());
        //                //
        //                newLine.SetPrice();
        //                newLine.Save(Get_TrxName());
        //            }
        //            //	Convert into Comment Line
        //            line.SetM_Product_ID(0);
        //            line.SetM_AttributeSetInstance_ID(0);
        //            line.SetPrice(Env.ZERO);
        //            line.SetPriceLimit(Env.ZERO);
        //            line.SetPriceList(Env.ZERO);
        //            line.SetLineNetAmt(Env.ZERO);
        //            line.SetFreightAmt(Env.ZERO);
        //            //
        //            String description = product.GetName();
        //            if (product.GetDescription() != null)
        //                description += " " + product.GetDescription();
        //            if (line.GetDescription() != null)
        //                description += " " + line.GetDescription();
        //            line.SetDescription(description);
        //            line.Save(Get_TrxName());
        //        }	//	for all lines with BOM

        //        _lines = null;		//	force requery
        //        count = DB.GetSQLValue(Get_TrxName(), sql, GetC_Invoice_ID());
        //        RenumberLines(10);
        //    }	//	while count != 0
        //    return retValue;
        //}

        /* Reserve Inventory.
        * 	Counterpart: MInOut.completeIt()
        * 	@param dt document type or null
        * 	@param lines order lines (ordered by M_Product_ID for deadlock prevention)
        * 	@return true if (un) reserved
        */
        //private bool ReserveStock(VAdvantage.Model.MDocType dt, MOrderLine[] lines)
        //{
        //    try
        //    {
        //        if (dt == null)
        //            dt = VAdvantage.Model.MDocType.Get(GetCtx(), GetC_DocType_ID());

        //        // Reserved quantity and ordered quantity should not be updated for returns
        //        if (dt.IsReturnTrx())
        //            return true;

        //        //	Binding
        //        bool binding = !dt.IsProposal();
        //        //	Not binding - i.e. Target=0
        //        if (DOCACTION_Void.Equals(GetDocAction())
        //            //	Closing Binding Quotation
        //            || (VAdvantage.Model.MDocType.DOCSUBTYPESO_Quotation.Equals(dt.GetDocSubTypeSO())
        //                && DOCACTION_Close.Equals(GetDocAction()))
        //            || IsDropShip())
        //            binding = false;
        //        bool isSOTrx = IsSOTrx();
        //        log.Fine("Binding=" + binding + " - IsSOTrx=" + isSOTrx);
        //        //	Force same WH for all but SO/VAdvantage.Model.PO
        //        int header_M_Warehouse_ID = GetM_Warehouse_ID();
        //        if (VAdvantage.Model.MDocType.DOCSUBTYPESO_StandardOrder.Equals(dt.GetDocSubTypeSO())
        //            || VAdvantage.Model.MDocBaseType.DOCBASETYPE_PURCHASEORDER.Equals(dt.GetDocBaseType()))
        //            header_M_Warehouse_ID = 0;		//	don't enforce

        //        Decimal Volume = Env.ZERO;
        //        Decimal Weight = Env.ZERO;

        //        //	Always check and (un) Reserve Inventory		
        //        for (int i = 0; i < lines.Length; i++)
        //        {
        //            MOrderLine line = lines[i];
        //            //	Check/set WH/Org
        //            if (header_M_Warehouse_ID != 0)	//	enforce WH
        //            {
        //                if (header_M_Warehouse_ID != line.GetM_Warehouse_ID())
        //                    line.SetM_Warehouse_ID(header_M_Warehouse_ID);
        //                if (GetAD_Org_ID() != line.GetAD_Org_ID())
        //                    line.SetAD_Org_ID(GetAD_Org_ID());
        //            }
        //            //	Binding
        //            Decimal target = binding ? line.GetQtyOrdered() : Env.ZERO;
        //            Decimal difference = Decimal.Subtract(Decimal.Subtract(target, line.GetQtyReserved()), line.GetQtyDelivered());
        //            if (Env.Signum(difference) == 0)
        //            {
        //                MProduct product = line.GetProduct();
        //                if (product != null)
        //                {
        //                    Volume = Decimal.Add(Volume, (Decimal.Multiply((Decimal)product.GetVolume(), line.GetQtyOrdered())));
        //                    Weight = Decimal.Add(Weight, (Decimal.Multiply(product.GetWeight(), line.GetQtyOrdered())));
        //                }
        //                continue;
        //            }

        //            log.Fine("Line=" + line.GetLine()
        //                + " - Target=" + target + ",Difference=" + difference
        //                + " - Ordered=" + line.GetQtyOrdered()
        //                + ",Reserved=" + line.GetQtyReserved() + ",Delivered=" + line.GetQtyDelivered());

        //            //	Check Product - Stocked and Item
        //            MProduct product1 = line.GetProduct();
        //            if (product1 != null)
        //            {
        //                if (product1.IsStocked())
        //                {
        //                    Decimal ordered = isSOTrx ? Env.ZERO : difference;
        //                    Decimal reserved = isSOTrx ? difference : Env.ZERO;
        //                    int M_Locator_ID = 0;
        //                    //	Get Locator to reserve
        //                    if (line.GetM_AttributeSetInstance_ID() != 0)	//	Get existing Location
        //                        M_Locator_ID = MStorage.GetM_Locator_ID(line.GetM_Warehouse_ID(),
        //                            line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
        //                            ordered, Get_TrxName());
        //                    //	Get default Location
        //                    if (M_Locator_ID == 0)
        //                    {
        //                        MWarehouse wh = MWarehouse.Get(GetCtx(), line.GetM_Warehouse_ID());
        //                        M_Locator_ID = wh.GetDefaultM_Locator_ID();
        //                    }
        //                    //	Update Storage
        //                    if (!MStorage.Add(GetCtx(), line.GetM_Warehouse_ID(), M_Locator_ID,
        //                        line.GetM_Product_ID(),
        //                        line.GetM_AttributeSetInstance_ID(), line.GetM_AttributeSetInstance_ID(),
        //                        Env.ZERO, reserved, ordered, Get_TrxName()))
        //                        return false;
        //                }	//	stockec
        //                //	update line
        //                line.SetQtyReserved(Decimal.Add(line.GetQtyReserved(), difference));
        //                if (!line.Save(Get_TrxName()))
        //                    return false;
        //                //
        //                Volume = Decimal.Add(Volume, (Decimal.Multiply((Decimal)product1.GetVolume(), line.GetQtyOrdered())));
        //                Weight = Decimal.Add(Weight, (Decimal.Multiply(product1.GetWeight(), line.GetQtyOrdered())));
        //            }	//	product
        //        }	//	reverse inventory

        //        SetVolume(Volume);
        //        SetWeight(Weight);
        //    }
        //    catch (Exception ex)
        //    {
        //        // MessageBox.Show("MOrder--ReserveStock");
        //    }
        //    return true;
        //}

        /* 	Calculate Tax and Total
         * 	@return true if tax total calculated
         */
        //private bool CalculateTaxTotal()
        //{
        //    try
        //    {
        //        log.Fine("");
        //        //	Delete Taxes
        //        DB.ExecuteQuery("DELETE FROM C_OrderTax WHERE C_Order_ID=" + GetC_Order_ID(), null, Get_TrxName());
        //        _taxes = null;

        //        //	Lines
        //        Decimal totalLines = Env.ZERO;
        //        List<int> taxList = new List<int>();
        //        MOrderLine[] lines = GetLines();
        //        for (int i = 0; i < lines.Length; i++)
        //        {
        //            MOrderLine line = lines[i];
        //            int taxID = line.GetC_Tax_ID();
        //            if (!taxList.Contains(taxID))
        //            {
        //                MOrderTax oTax = MOrderTax.Get(line, GetPrecision(),
        //                    false, Get_TrxName());	//	current Tax
        //                oTax.SetIsTaxIncluded(IsTaxIncluded());
        //                if (!oTax.CalculateTaxFromLines())
        //                    return false;
        //                if (!oTax.Save(Get_TrxName()))
        //                    return false;
        //                taxList.Add(taxID);
        //            }
        //            totalLines = Decimal.Add(totalLines, line.GetLineNetAmt());
        //        }

        //        //	Taxes
        //        Decimal grandTotal = totalLines;
        //        MOrderTax[] taxes = GetTaxes(true);
        //        for (int i = 0; i < taxes.Length; i++)
        //        {
        //            MOrderTax oTax = taxes[i];
        //            MTax tax = oTax.GetTax();
        //            if (tax.IsSummary())
        //            {
        //                MTax[] cTaxes = tax.GetChildTaxes(false);
        //                for (int j = 0; j < cTaxes.Length; j++)
        //                {
        //                    MTax cTax = cTaxes[j];
        //                    Decimal taxAmt = cTax.CalculateTax(oTax.GetTaxBaseAmt(), IsTaxIncluded(), GetPrecision());
        //                    //
        //                    MOrderTax newOTax = new MOrderTax(GetCtx(), 0, Get_TrxName());
        //                    newOTax.SetClientOrg(this);
        //                    newOTax.SetC_Order_ID(GetC_Order_ID());
        //                    newOTax.SetC_Tax_ID(cTax.GetC_Tax_ID());
        //                    newOTax.SetPrecision(GetPrecision());
        //                    newOTax.SetIsTaxIncluded(IsTaxIncluded());
        //                    newOTax.SetTaxBaseAmt(oTax.GetTaxBaseAmt());
        //                    newOTax.SetTaxAmt(taxAmt);
        //                    if (!newOTax.Save(Get_TrxName()))
        //                        return false;
        //                    //
        //                    if (!IsTaxIncluded())
        //                        grandTotal = Decimal.Add(grandTotal, taxAmt);
        //                }
        //                if (!oTax.Delete(true, Get_TrxName()))
        //                    return false;
        //                _taxes = null;
        //            }
        //            else
        //            {
        //                if (!IsTaxIncluded())
        //                    grandTotal = Decimal.Add(grandTotal, oTax.GetTaxAmt());
        //            }
        //        }
        //        //
        //        SetTotalLines(totalLines);
        //        SetGrandTotal(grandTotal);
        //    }
        //    catch (Exception ex)
        //    {
        //       // // MessageBox.Show("MOrder--CalculateTaxTotal");
        //    }
        //    return true;
        //}

        /// <summary>
        /// Approve Document
        /// </summary>
        /// <returns>true if success</returns>
        public bool ApproveIt()
        {
            log.Info("approveIt - " + ToString());
            //  SetIsApproved(true);
            return true;
        }

        /// <summary>
        /// Reject Approval
        /// </summary>
        /// <returns>true if success</returns>
        public bool RejectIt()
        {
            log.Info("rejectIt - " + ToString());
            //SetIsApproved(false);
            return true;
        }

        /// <summary>
        /// Complete Document
        /// </summary>
        /// <returns>new status (Complete, In Progress, Invalid, Waiting ..)</returns>
        public String CompleteIt()
        {
            try
            {
                VAdvantage.Model.MDocType dt = VAdvantage.Model.MDocType.Get(GetCtx(), 0);
                String DocSubTypeSO = dt.GetDocSubTypeSO();

                //	Just prepare
                if (DOCACTION_Prepare.Equals(GetDocAction()))
                {
                    SetProcessed(false);
                    return DocActionVariables.STATUS_INPROGRESS;
                }

                if (!IsReturnTrx())
                {
                    //	Offers
                    if (VAdvantage.Model.MDocType.DOCSUBTYPESO_Proposal.Equals(DocSubTypeSO)
                        || VAdvantage.Model.MDocType.DOCSUBTYPESO_Quotation.Equals(DocSubTypeSO))
                    {
                        //	Binding

                        SetProcessed(true);
                        SetDocAction(DOCACTION_Close);
                        //email code
                        // SendingEmail();
                        // SendSMS();
                        return DocActionVariables.STATUS_COMPLETED;
                    }
                    //	Waiting Payment - until we have a payment
                    if (!_forceCreation
                        && VAdvantage.Model.MDocType.DOCSUBTYPESO_PrepayOrder.Equals(DocSubTypeSO)
                       )
                    {
                        SetProcessed(true);
                        return DocActionVariables.STATUS_WAITINGPAYMENT;
                    }

                    //	Re-Check
                    if (!_justPrepared)
                    {
                        String status = PrepareIt();
                        if (!DocActionVariables.STATUS_INPROGRESS.Equals(status))
                            return status;
                    }
                }

                //	Implicit Approval
                // if (!IsApproved())
                ApproveIt();
                // GetLines(true, null);
                log.Info(ToString());
                StringBuilder Info = new StringBuilder();

                /* nnayak - Bug 1720003 - We need to set the processed flag so the Tax Summary Line
                does not get recreated in the afterSave procedure of the MOrderLine class */
                SetProcessed(true);



                ////	Create SO Shipment - Force Shipment



                //	Create SO Invoice - Always invoice complete Order
                if (VAdvantage.Model.MDocType.DOCSUBTYPESO_POSOrder.Equals(DocSubTypeSO)
                    || VAdvantage.Model.MDocType.DOCSUBTYPESO_OnCreditOrder.Equals(DocSubTypeSO)
                    || VAdvantage.Model.MDocType.DOCSUBTYPESO_PrepayOrder.Equals(DocSubTypeSO))
                {
                    try
                    {

                    }
                    catch
                    {
                        // // MessageBox.Show("tSet not null");
                    }
                }

                //	Counter Documents


                SetProcessed(true);
                _processMsg = Info.ToString();
                //

                SetDocAction(DOCACTION_Close);

            }
            catch (Exception ex)
            {
                log.Severe("MOrder--CompleteIt" + ex.Message);
            }
            return DocActionVariables.STATUS_COMPLETED;
        }

        /* 	Create Shipment
        *	@param dt order document type
        *	@param movementDate optional movement date (default today)
        *	@return shipment or null
        */
        //private MInOut CreateShipment(VAdvantage.Model.MDocType dt, DateTime? movementDate)
        //{
        //    MInOut shipment = new MInOut(this, (int)dt.GetC_DocTypeShipment_ID(), (DateTime?)movementDate);
        //    log.Info("For " + dt);
        //    try
        //    {
        //        //	shipment.setDateAcct(getDateAcct());
        //        if (!shipment.Save(Get_TrxName()))
        //        {
        //            _processMsg = "Could not create Shipment";
        //            return null;
        //        }
        //        //
        //        MOrderLine[] oLines = GetLines(true, null);
        //        for (int i = 0; i < oLines.Length; i++)
        //        {
        //            MOrderLine oLine = oLines[i];
        //            //
        //            MInOutLine ioLine = new MInOutLine(shipment);
        //            //	Qty = Ordered - Delivered
        //            Decimal MovementQty = Decimal.Subtract(oLine.GetQtyOrdered(), oLine.GetQtyDelivered());
        //            //	Location
        //            int M_Locator_ID = MStorage.GetM_Locator_ID(oLine.GetM_Warehouse_ID(),
        //                    oLine.GetM_Product_ID(), oLine.GetM_AttributeSetInstance_ID(),
        //                    MovementQty, Get_TrxName());
        //            if (M_Locator_ID == 0)		//	Get default Location
        //            {
        //                MProduct product = ioLine.GetProduct();
        //                int M_Warehouse_ID = oLine.GetM_Warehouse_ID();
        //                M_Locator_ID = MProductLocator.GetFirstM_Locator_ID(product, M_Warehouse_ID);
        //                if (M_Locator_ID == 0)
        //                {
        //                    MWarehouse wh = MWarehouse.Get(GetCtx(), M_Warehouse_ID);
        //                    M_Locator_ID = wh.GetDefaultM_Locator_ID();
        //                }
        //            }
        //            //
        //            ioLine.SetOrderLine(oLine, M_Locator_ID, MovementQty);
        //            ioLine.SetQty(MovementQty);
        //            if (oLine.GetQtyEntered().CompareTo(oLine.GetQtyOrdered()) != 0)
        //            {
        //                //ioLine.SetQtyEntered(Decimal.Multiply(MovementQty,(oLine.getQtyEntered()).divide(oLine.getQtyOrdered(), 6, Decimal.ROUND_HALF_UP));
        //                ioLine.SetQtyEntered(Decimal.Multiply(MovementQty, (Decimal.Divide(oLine.GetQtyEntered(), (oLine.GetQtyOrdered())))));
        //            }
        //            if (!ioLine.Save(Get_TrxName()))
        //            {
        //                _processMsg = "Could not create Shipment Line";
        //                return null;
        //            }
        //        }
        //        //	Manually Process Shipment
        //        String status = shipment.CompleteIt();
        //        shipment.SetDocStatus(status);
        //        shipment.Save(Get_TrxName());
        //        if (!DOCSTATUS_Completed.Equals(status))
        //        {
        //            _processMsg = "@M_InOut_ID@: " + shipment.GetProcessMsg();
        //            return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //       // // MessageBox.Show("MOrder--CreateShipment");
        //    }
        //    return shipment;
        //}

        /* 	Create Invoice
            *	@param dt order document type
            *	@param shipment optional shipment
            *	@param invoiceDate invoice date
            *	@return invoice or null
            */
        //private MInvoice CreateInvoice(VAdvantage.Model.MDocType dt, MInOut shipment, DateTime? invoiceDate)
        //{
        //    MInvoice invoice = new MInvoice(this, dt.GetC_DocTypeInvoice_ID(), invoiceDate);
        //    log.Info(dt.ToString());
        //    try
        //    {
        //        if (!invoice.Save(Get_TrxName()))
        //        {
        //            _processMsg = "Could not create Invoice";
        //            return null;
        //        }

        //        //	If we have a Shipment - use that as a base
        //        if (shipment != null)
        //        {
        //            if (!INVOICERULE_AfterDelivery.Equals(GetInvoiceRule()))
        //                SetInvoiceRule(INVOICERULE_AfterDelivery);
        //            //
        //            MInOutLine[] sLines = shipment.GetLines(false);
        //            for (int i = 0; i < sLines.Length; i++)
        //            {
        //                MInOutLine sLine = sLines[i];
        //                //
        //                MInvoiceLine iLine = new MInvoiceLine(invoice);
        //                iLine.SetShipLine(sLine);
        //                //	Qty = Delivered	
        //                iLine.SetQtyEntered(sLine.GetQtyEntered());
        //                iLine.SetQtyInvoiced(sLine.GetMovementQty());
        //                if (!iLine.Save(Get_TrxName()))
        //                {
        //                    _processMsg = "Could not create Invoice Line from Shipment Line";
        //                    return null;
        //                }
        //                //
        //                sLine.SetIsInvoiced(true);
        //                if (!sLine.Save(Get_TrxName()))
        //                {
        //                    log.Warning("Could not update Shipment line: " + sLine);
        //                }
        //            }
        //        }
        //        else	//	Create Invoice from Order
        //        {
        //            if (!INVOICERULE_Immediate.Equals(GetInvoiceRule()))
        //                SetInvoiceRule(INVOICERULE_Immediate);
        //            //
        //            MOrderLine[] oLines = GetLines();
        //            for (int i = 0; i < oLines.Length; i++)
        //            {
        //                MOrderLine oLine = oLines[i];
        //                //
        //                MInvoiceLine iLine = new MInvoiceLine(invoice);
        //                iLine.SetOrderLine(oLine);
        //                //	Qty = Ordered - Invoiced	
        //                iLine.SetQtyInvoiced(Decimal.Subtract(oLine.GetQtyOrdered(), oLine.GetQtyInvoiced()));
        //                if (oLine.GetQtyOrdered().CompareTo(oLine.GetQtyEntered()) == 0)
        //                    iLine.SetQtyEntered(iLine.GetQtyInvoiced());
        //                else
        //                    iLine.SetQtyEntered(Decimal.Multiply(iLine.GetQtyInvoiced(), (Decimal.Divide(oLine.GetQtyEntered(), oLine.GetQtyOrdered()))));
        //                if (!iLine.Save(Get_TrxName()))
        //                {
        //                    _processMsg = "Could not create Invoice Line from Order Line";
        //                    return null;
        //                }
        //            }
        //        }
        //        //	Manually Process Invoice
        //        String status = invoice.CompleteIt();
        //        invoice.SetDocStatus(status);
        //        invoice.Save(Get_TrxName());
        //        SetC_CashLine_ID(invoice.GetC_CashLine_ID());
        //        if (!DOCSTATUS_Completed.Equals(status))
        //        {
        //            _processMsg = "@C_Invoice_ID@: " + invoice.GetProcessMsg();
        //            return null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // MessageBox.Show("MOrder--CreateInvoice");
        //    }
        //    return invoice;
        //}

        /* 	Create Counter Document
         * 	@return counter order
         */

        /// <summary>
        /// Void Document.
        ///	Set Qtys to 0 - Sales: reverse all documents
        /// </summary>
        /// <returns>true if success</returns>
        public bool VoidIt()
        {
            /***********************************/
            //MOrderLine[] lines = GetLines(true, "M_Product_ID");
            //log.Info(ToString());
            //for (int i = 0; i < lines.Length; i++)
            //{
            //    MOrderLine line = lines[i];
            //    Decimal old = line.GetQtyOrdered();
            //    if (System.Math.Sign(old) != 0)
            //    {
            //        line.AddDescription(Msg.GetMsg(GetCtx(), "Voided", true) + " (" + old + ")");
            //        line.SetQtyLostSales(old);
            //        line.SetQty(Env.ZERO);
            //        line.SetLineNetAmt(Env.ZERO);
            //        line.Save(Get_TrxName());
            //    }
            //}
            /***********************************/
            //AddDescription(Msg.GetMsg( "Voided", true));
            //	Clear Reservations
            //if (!ReserveStock(null, lines))
            //{
            //    _processMsg = "Cannot unreserve Stock (void)";
            //    return false;
            //}

            if (!CreateReversals())
                return false;

            //************* Changed ***************************
            // Set Status at Order to Rejected if it is Sales Order 

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

                log.Info("");
                StringBuilder Info = new StringBuilder();

                //	Reverse All *Shipments*
                Info.Append("@M_InOut_ID@:");
                //MInOut[] shipments = GetShipments(false);	//	get all (line based)
                //for (int i = 0; i < shipments.Length; i++)
                //{
                //    MInOut ship = shipments[i];
                //    //	if closed - ignore
                //    if (MInOut.DOCSTATUS_Closed.Equals(ship.GetDocStatus())
                //        || MInOut.DOCSTATUS_Reversed.Equals(ship.GetDocStatus())
                //        || MInOut.DOCSTATUS_Voided.Equals(ship.GetDocStatus()))
                //        continue;
                //    ship.Set_TrxName(Get_TrxName());

                //    //	If not completed - void - otherwise reverse it
                //    if (!MInOut.DOCSTATUS_Completed.Equals(ship.GetDocStatus()))
                //    {
                //        if (ship.VoidIt())
                //            ship.SetDocStatus(MInOut.DOCSTATUS_Voided);
                //    }
                //    //	Create new Reversal with only that order
                //    else if (!ship.IsOnlyForOrder(this))
                //    {
                //        ship.ReverseCorrectIt(this);
                //        //	shipLine.setDocStatus(MInOut.DOCSTATUS_Reversed);
                //        Info.Append(" Parial ").Append(ship.GetDocumentNo());
                //    }
                //    else if (ship.ReverseCorrectIt()) //	completed shipment
                //    {
                //        ship.SetDocStatus(MInOut.DOCSTATUS_Reversed);
                //        Info.Append(" ").Append(ship.GetDocumentNo());
                //    }
                //    else
                //    {
                //        _processMsg = "Could not reverse Shipment " + ship;
                //        return false;
                //    }
                //    ship.SetDocAction(MInOut.DOCACTION_None);
                //    ship.Save(Get_TrxName());
                //}	//	for all shipments

                //	Reverse All *Invoices*
                Info.Append(" - @C_Invoice_ID@:");
                //MInvoice[] invoices = GetInvoices(false);	//	get all (line based)
                //for (int i = 0; i < invoices.Length; i++)
                //{
                //    MInvoice invoice = invoices[i];
                //    //	if closed - ignore
                //    if (MInvoice.DOCSTATUS_Closed.Equals(invoice.GetDocStatus())
                //        || MInvoice.DOCSTATUS_Reversed.Equals(invoice.GetDocStatus())
                //        || MInvoice.DOCSTATUS_Voided.Equals(invoice.GetDocStatus()))
                //        continue;
                //    invoice.Set_TrxName(Get_TrxName());

                //    //	If not completed - void - otherwise reverse it
                //    if (!MInvoice.DOCSTATUS_Completed.Equals(invoice.GetDocStatus()))
                //    {
                //        if (invoice.VoidIt())
                //            invoice.SetDocStatus(MInvoice.DOCSTATUS_Voided);
                //    }
                //    else if (invoice.ReverseCorrectIt())	//	completed invoice
                //    {
                //        invoice.SetDocStatus(MInvoice.DOCSTATUS_Reversed);
                //        Info.Append(" ").Append(invoice.GetDocumentNo());
                //    }
                //    else
                //    {
                //        _processMsg = "Could not reverse Invoice " + invoice;
                //        return false;
                //    }
                //    invoice.SetDocAction(MInvoice.DOCACTION_None);
                //    invoice.Save(Get_TrxName());
                //}	//	for all shipments

                //	Reverse All *RMAs*
                Info.Append("@C_Order_ID@:");
                //MOrder[] rmas = GetRMAs();
                //for (int i = 0; i < rmas.Length; i++)
                //{
                //    MOrder rma = rmas[i];
                //    //	if closed - ignore
                //    if (MOrder.DOCSTATUS_Closed.Equals(rma.GetDocStatus())
                //        || MOrder.DOCSTATUS_Reversed.Equals(rma.GetDocStatus())
                //        || MOrder.DOCSTATUS_Voided.Equals(rma.GetDocStatus()))
                //        continue;
                //    rma.Set_TrxName(Get_TrxName());

                //    //	If not completed - void - otherwise reverse it
                //    if (!MOrder.DOCSTATUS_Completed.Equals(rma.GetDocStatus()))
                //    {
                //        if (rma.VoidIt())
                //            rma.SetDocStatus(MInOut.DOCSTATUS_Voided);
                //    }
                //    //	Create new Reversal with only that order
                //    else if (rma.ReverseCorrectIt()) //	completed shipment
                //    {
                //        rma.SetDocStatus(MOrder.DOCSTATUS_Reversed);
                //        Info.Append(" ").Append(rma.GetDocumentNo());
                //    }
                //    else
                //    {
                //        _processMsg = "Could not reverse RMA " + rma;
                //        return false;
                //    }
                //    rma.SetDocAction(MInOut.DOCACTION_None);
                //    rma.Save(Get_TrxName());
                //}	//	for all shipments


                _processMsg = Info.ToString();
            }
            catch
            {
                //// MessageBox.Show("MOrder--CreateReversals");
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

            //	Close Not delivered Qty - SO/VAdvantage.Model.PO
            /*********************************/
            //MOrderLine[] lines = GetLines(true, "M_Product_ID");
            //for (int i = 0; i < lines.Length; i++)
            //{
            //    MOrderLine line = lines[i];
            //    Decimal old = line.GetQtyOrdered();
            //    if (old.CompareTo(line.GetQtyDelivered()) != 0)
            //    {
            //        line.SetQtyLostSales(Decimal.Subtract(line.GetQtyOrdered(), line.GetQtyDelivered()));
            //        line.SetQtyOrdered(line.GetQtyDelivered());
            //        //	QtyEntered unchanged
            //        line.AddDescription("Close (" + old + ")");
            //        line.Save(Get_TrxName());
            //    }
            //}

            //	Clear Reservations
            //if (!ReserveStock(null, lines))
            //{
            //    _processMsg = "Cannot unreserve Stock (close)";
            //    return false;
            //}
            /*********************************/

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

                VAdvantage.Model.MDocType dt = VAdvantage.Model.MDocType.Get(GetCtx(), 0);
                String DocSubTypeSO = dt.GetDocSubTypeSO();

                //	Replace Prepay with POS to revert all doc
                if (VAdvantage.Model.MDocType.DOCSUBTYPESO_PrepayOrder.Equals(DocSubTypeSO))
                {
                    VAdvantage.Model.MDocType newDT = null;
                    VAdvantage.Model.MDocType[] dts = VAdvantage.Model.MDocType.GetOfClient(GetCtx());
                    for (int i = 0; i < dts.Length; i++)
                    {
                        VAdvantage.Model.MDocType type = dts[i];
                        if (VAdvantage.Model.MDocType.DOCSUBTYPESO_PrepayOrder.Equals(type.GetDocSubTypeSO()))
                        {
                            if (type.IsDefault() || newDT == null)
                                newDT = type;
                        }
                    }
                    if (newDT == null)
                        return false;
                    else
                    {
                        // SetC_DocType_ID(newDT.GetC_DocType_ID());
                        SetIsReturnTrx(newDT.IsReturnTrx());
                    }
                }

                //	VAdvantage.Model.PO - just re-open
                //if (!IsSOTrx())
                //{
                //    log.Info("Existing documents not modified - " + dt);
                //}
                //	Reverse Direct Documents
                else if (VAdvantage.Model.MDocType.DOCSUBTYPESO_OnCreditOrder.Equals(DocSubTypeSO)	//	(W)illCall(I)nvoice
                    || VAdvantage.Model.MDocType.DOCSUBTYPESO_WarehouseOrder.Equals(DocSubTypeSO)	//	(W)illCall(P)ickup	
                    || VAdvantage.Model.MDocType.DOCSUBTYPESO_POSOrder.Equals(DocSubTypeSO))			//	(W)alkIn(R)eceipt
                {
                    if (!CreateReversals())
                        return false;
                }
                else
                {
                    log.Info("Existing documents not modified - SubType=" + DocSubTypeSO);
                }

                SetDocAction(DOCACTION_Complete);
                SetProcessed(false);
            }
            catch
            {
                //// MessageBox.Show("MOrder--ReActivateIt");
            }
            return true;
        }

        ///// <summary>
        ///// Get Summary
        ///// </summary>
        ///// <returns>Summary of Document</returns>
        //public String GetSummary()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(GetDocumentNo());
        //    //	: Grand Total = 123.00 (#1)
        //    sb.Append(": ").
        //        Append(Msg.Translate(GetCtx(), "GrandTotal")).Append("=").Append(GetGrandTotal());
        //    if (_lines != null)
        //        sb.Append(" (#").Append(_lines.Length).Append(")");
        //    //	 - Description
        //    if (GetDescription() != null && GetDescription().Length > 0)
        //        sb.Append(" - ").Append(GetDescription());
        //    return sb.ToString();
        //}

        ///// <summary>
        ///// Get Process Message
        ///// </summary>
        ///// <returns>clear text error message</returns>
        //public String GetProcessMsg()
        //{
        //    return _processMsg;
        //}

        ///// <summary>
        ///// Get Document Owner (Responsible)
        ///// </summary>
        ///// <returns>AD_User_ID</returns>
        //public int GetDoc_User_ID()
        //{
        //    return GetSalesRep_ID();
        //}

        ///// <summary>
        ///// Get Document Approval Amount
        ///// </summary>
        ///// <returns>amount</returns>
        //public Decimal GetApprovalAmt()
        //{
        //    return GetGrandTotal();
        //}
        ///// <summary>
        ///// Get Latest Shipment for the Order
        ///// </summary>
        ///// <param name="C_DocType_ID"></param>
        ///// <param name="M_Warehouse_ID"></param>
        ///// <param name="C_BPartner_ID"></param>
        ///// <param name="C_BPartner_Location_ID"></param>
        ///// <returns>latest shipment</returns>
        //public MInOut GetOpenInOut(int C_DocType_ID, int M_Warehouse_ID, int C_BPartner_ID, int C_BPartner_Location_ID)
        //{
        //    //	TODO: getShipment if linked on line
        //    MInOut inout = null;
        //    String sql = "SELECT M_InOut_ID " +
        //    "FROM M_InOut WHERE C_Order_ID=" + GetC_Order_ID()
        //   + " AND M_Warehouse_ID=" + M_Warehouse_ID
        //   + " AND C_BPartner_ID=" + C_BPartner_ID
        //   + " AND C_BPartner_Location_ID= " + C_BPartner_Location_ID
        //   + " AND C_DocType_ID= " + C_DocType_ID
        //    + " AND DocStatus IN ('DR','IP') " +
        //    " ORDER BY Created DESC";
        //    IDataReader idr = null;
        //    try
        //    {
        //        idr = DB.ExecuteReader(sql, null, Get_TrxName());
        //        if (idr.Read())
        //        {
        //            inout = new MInOut(GetCtx(), Util.GetValueOfInt(idr[0]), Get_TrxName());
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        log.Log(Level.SEVERE, sql, e);
        //    }
        //    finally
        //    {
        //        if (idr != null)
        //        {
        //            idr.Close();
        //            idr = null;
        //        }
        //    }
        //    //
        //    return inout;
        //}


        #region DocAction Members


        public string GetDocBaseType()
        {
            return "";
        }

        public DateTime? GetDocumentDate()
        {
            return null;
        }

        public Env.QueryParams GetLineOrgsQueryInfo()
        {
            return null;
        }

        #endregion

        #region DocAction Members



        public void SetProcessMsg(string processMsg)
        {

        }

        #endregion


        #region DocAction Members


        public decimal GetApprovalAmt()
        {
            return Env.ZERO;
        }

        public int GetDoc_User_ID()
        {
            return 0;
        }

        public string GetProcessMsg()
        {
            return "No Message";
        }

        public string GetSummary()
        {
            return "Not Defind";
        }

        #endregion
    }
}
