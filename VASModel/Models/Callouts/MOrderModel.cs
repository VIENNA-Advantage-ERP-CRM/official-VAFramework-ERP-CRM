using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DBase;

namespace VIS.Models
{
    public class MOrderModel
    {
        /// <summary>
        /// GetOrder
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<String, object> GetOrder(Ctx ctx, string fields)
        {

            string[] paramValue = fields.ToString().Split(',');
            int VAB_Order_ID;

            //Assign parameter value
            VAB_Order_ID = Util.GetValueOfInt(paramValue[0].ToString());
            MVABOrder order = new MVABOrder(ctx, VAB_Order_ID, null);
            // End Assign parameter value

            Dictionary<String, object> retDic = new Dictionary<string, object>();
            // Reset Orig Shipment
            retDic["VAB_BusinessPartner_ID"] = Util.GetValueOfString(order.GetVAB_BusinessPartner_ID());
            retDic["VAB_BPart_Location_ID"] = Util.GetValueOfString(order.GetVAB_BPart_Location_ID());
            retDic["Bill_BPartner_ID"] = Util.GetValueOfString(order.GetBill_BPartner_ID());
            retDic["Bill_Location_ID"] = Util.GetValueOfString(order.GetBill_Location_ID());
            if (order.GetVAF_UserContact_ID() != 0)
                retDic["VAF_UserContact_ID"] = Util.GetValueOfString(order.GetVAF_UserContact_ID());
            if (order.GetBill_User_ID() != 0)
                retDic["Bill_User_ID"] = Util.GetValueOfString(order.GetBill_User_ID());
            retDic["VAM_PriceList_ID"] = Util.GetValueOfString(order.GetVAM_PriceList_ID());
            retDic["PaymentRule"] = order.GetPaymentRule();
            retDic["VAB_PaymentTerm_ID"] = Util.GetValueOfString(order.GetVAB_PaymentTerm_ID());
            //mTab.setValue ("DeliveryRule", X_VAB_Order.DELIVERYRULE_Manual);
            retDic["Bill_Location_ID"] = Util.GetValueOfString(order.GetBill_Location_ID());
            retDic["InvoiceRule"] = order.GetInvoiceRule();
            retDic["PaymentRule"] = order.GetPaymentRule();
            retDic["DeliveryViaRule"] = order.GetDeliveryViaRule();
            retDic["FreightCostRule"] = order.GetFreightCostRule();
            retDic["ID"] = Util.GetValueOfString(order.Get_ID());
            //retDic["DateOrdered"] = Convert.ToDateTime(order.GetDateOrdered()).ToLocalTime().ToUniversalTime();
            retDic["DateOrdered"] = DateTime.SpecifyKind(order.GetDateOrdered().Value, DateTimeKind.Utc);
            retDic["DateAcct"] = Util.GetValueOfString(order.GetDateAcct());
            retDic["POReference"] = order.GetPOReference();
            retDic["VAF_Org_ID"] = Util.GetValueOfString(order.GetVAF_Org_ID());
            retDic["DeliveryRule"] = order.GetDeliveryRule();
            retDic["DeliveryViaRule"] = order.GetDeliveryViaRule();
            retDic["VAM_ShippingMethod_ID"] = Util.GetValueOfString(order.GetVAM_ShippingMethod_ID());
            retDic["FreightAmt"] = Util.GetValueOfString(order.GetFreightAmt());
            retDic["VAF_OrgTrx_ID"] = Util.GetValueOfString(order.GetVAF_OrgTrx_ID());
            retDic["VAB_BillingCode_ID"] = Util.GetValueOfString(order.GetVAB_BillingCode_ID());
            retDic["VAB_Promotion_ID"] = Util.GetValueOfString(order.GetVAB_Promotion_ID());
            retDic["VAB_Project_ID"] = Util.GetValueOfString(order.GetVAB_Project_ID());
            retDic["User1_ID"] = Util.GetValueOfString(order.GetUser1_ID());
            retDic["User2_ID"] = Util.GetValueOfString(order.GetUser2_ID());
            retDic["VAM_Warehouse_ID"] = Util.GetValueOfString(order.GetVAM_Warehouse_ID());
            retDic["Orig_Order_ID"] = Util.GetValueOfString(order.GetOrig_Order_ID());
            retDic["Orig_InOut_ID"] = Util.GetValueOfString(order.GetOrig_InOut_ID());
            //Added By Amit
            retDic["IsSOTrx"] = Util.GetValueOfString(order.IsSOTrx());
            retDic["IsReturnTrx"] = Util.GetValueOfString(order.IsReturnTrx());
            retDic["VAB_Payment_ID"] = Util.GetValueOfString(order.GetPaymentMethod());
            retDic["VA009_PaymentMethod_ID"] = Util.GetValueOfString(order.GetVA009_PaymentMethod_ID());
            retDic["SalesRep_ID"] = Util.GetValueOfString(order.GetSalesRep_ID());
            retDic["VAB_ProjectRef_ID"] = Util.GetValueOfString(order.GetVAB_ProjectRef_ID());
            retDic["PriorityRule"] = Util.GetValueOfString(order.GetPriorityRule());


            if (order.GetVAB_Currency_ID() != 0)
            {
                retDic["VAB_Currency_ID"] = Util.GetValueOfString(order.GetVAB_Currency_ID());
            }
            else
            {
                retDic["VAB_Currency_ID"] = "0";
            }
            //End
            // added by vivek on 09/10/2017 advised by pradeep to set drop ship checkbox value
            retDic["IsDropShip"] = Util.GetValueOfBool(order.IsDropShip()) ? "Y" : "N";

            // Added by Bharat on 30 Jan 2018 to set Inco Term from Order

            if (order.Get_ColumnIndex("VAB_IncoTerm_ID") > 0)
            {
                retDic["VAB_IncoTerm_ID"] = Util.GetValueOfString(order.GetVAB_IncoTerm_ID());
            }
            return retDic;
        }

        //Added By Amit
        public int GetVAM_PriceList(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int VAB_Order_ID;
            VAB_Order_ID = Util.GetValueOfInt(paramValue[0].ToString());
            MVABOrder order = new MVABOrder(ctx, VAB_Order_ID, null);
            return order.GetVAM_PriceList_ID();
        }

        public int GetVAB_Currency_ID(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int VAB_Order_ID;
            VAB_Order_ID = Util.GetValueOfInt(paramValue[0].ToString());
            MVABOrder order = new MVABOrder(ctx, VAB_Order_ID, null);
            return order.GetVAB_Currency_ID();
        }
        //End

        // Added by Bharat on 16 May 2017        
        public Dictionary<String, object> GetPaymentMethod(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            bool countVA009 = false;
            int VAB_Order_ID = 0;
            countVA009 = Util.GetValueOfBool(paramValue[0]);
            VAB_Order_ID = Util.GetValueOfInt(paramValue[1]);
            Dictionary<String, object> retDir = null; ;
            if (countVA009)
            {
                string sql = "SELECT p.VA009_PaymentMethod_ID, p.VA009_PaymentBaseType FROM VAB_Order o INNER JOIN VA009_PaymentMethod p "
                + " ON o.VA009_PaymentMethod_ID = p.VA009_PaymentMethod_ID WHERE o.VAB_Order_ID = " + VAB_Order_ID;
                DataSet ds = DB.ExecuteDataset(sql, null, null);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    retDir = new Dictionary<string, object>();
                    retDir["VA009_PaymentMethod_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VA009_PaymentMethod_ID"]);
                    retDir["VA009_PaymentBaseType"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["VA009_PaymentBaseType"]);
                }
            }
            return retDir;
        }

        // Added by Bharat on 30 Jan 2018 to Get Inco Term   
        public int GetIncoTerm(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            bool isSOTrx = false;
            int incoTerm_ID = 0, referenceID = 0;
            string tableName = "", refColumn = "", qry = "";
            isSOTrx = Util.GetValueOfBool(paramValue[0]);
            tableName = Util.GetValueOfString(paramValue[1]);
            refColumn = Util.GetValueOfString(paramValue[2]);
            referenceID = Util.GetValueOfInt(paramValue[3]);
            if (tableName == "VAB_Order")
            {
                if (referenceID > 0)
                {
                    MVABOrder ord = new MVABOrder(ctx, referenceID, null);
                    incoTerm_ID = ord.GetVAB_IncoTerm_ID();
                }
            }
            else if (tableName == "VAB_Invoice")
            {
                if (isSOTrx && referenceID > 0)
                {
                    MVABOrder ord = new MVABOrder(ctx, referenceID, null);
                    incoTerm_ID = ord.GetVAB_IncoTerm_ID();
                }
                else if (!isSOTrx && referenceID > 0 && refColumn == "VAB_Order_ID")
                {
                    MVABOrder ord = new MVABOrder(ctx, referenceID, null);
                    incoTerm_ID = ord.GetVAB_IncoTerm_ID();
                }
                else if (!isSOTrx && referenceID > 0 && refColumn == "VAM_Inv_InOut_ID")
                {
                    MVAMInvInOut inOut = new MVAMInvInOut(ctx, referenceID, null);
                    incoTerm_ID = inOut.GetVAB_IncoTerm_ID();
                }
            }
            else if (tableName == "VAM_Inv_InOut")
            {
                if (isSOTrx && referenceID > 0)
                {
                    MVABOrder ord = new MVABOrder(ctx, referenceID, null);
                    incoTerm_ID = ord.GetVAB_IncoTerm_ID();
                }
                else if (!isSOTrx && referenceID > 0 && refColumn == "VAB_Order_ID")
                {
                    MVABOrder ord = new MVABOrder(ctx, referenceID, null);
                    incoTerm_ID = ord.GetVAB_IncoTerm_ID();
                }
                else if (!isSOTrx && referenceID > 0 && refColumn == "VAB_Invoice_ID")
                {
                    MVABInvoice inv = new MVABInvoice(ctx, referenceID, null);
                    incoTerm_ID = inv.GetVAB_IncoTerm_ID();
                }
            }
            return incoTerm_ID;

        }

        // when doc type = Warehouse Order / Credit Order / POS Order / Prepay order --- and payment term is advance -- then system return false
        // Payment term can't be advance for Customer RMA / Vendor RMA
        public bool checkAdvancePaymentTerm(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            bool isAdvancePayTerm = true;
            int documnetType_Id = 0, PaymentTerm_Id = 0;
            documnetType_Id = Util.GetValueOfInt(paramValue[0]);
            PaymentTerm_Id = Util.GetValueOfInt(paramValue[1]);

            // when document type is not --  Warehouse Order / Credit Order / POS Order / Prepay order, then true
            // Payment term can't be advance for Customer RMA / Vendor RMA
            MVABDocTypes doctype = MVABDocTypes.Get(ctx, documnetType_Id);
            if (!(doctype.GetDocSubTypeSO() == X_VAB_DocTypes.DOCSUBTYPESO_PrepayOrder ||
                doctype.GetDocSubTypeSO() == X_VAB_DocTypes.DOCSUBTYPESO_OnCreditOrder ||
                doctype.GetDocSubTypeSO() == X_VAB_DocTypes.DOCSUBTYPESO_WarehouseOrder ||
                doctype.GetDocSubTypeSO() == X_VAB_DocTypes.DOCSUBTYPESO_POSOrder ||
                (doctype.GetDocSubTypeSO() == X_VAB_DocTypes.DOCSUBTYPESO_StandardOrder && doctype.IsReturnTrx()) ||
                (doctype.GetDocBaseType() == "POO" && doctype.IsReturnTrx())))
            {
                isAdvancePayTerm = true;
            }
            // check payment term is Advance, then return False
            else if (Util.GetValueOfString(DB.ExecuteScalar(@"SELECT VA009_Advance FROM VAB_PaymentTerm
                                            WHERE VAB_PaymentTerm_ID = " + PaymentTerm_Id, null, null)).Equals("Y"))
            {
                isAdvancePayTerm = false;
            }
            // check any payment term schedule is Advance, then return False
            else if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(*) FROM VAB_PaymentSchedule 
                                            WHERE VA009_Advance = 'Y' AND VAB_PaymentTerm_ID = " + PaymentTerm_Id, null, null)) > 0)
            {
                isAdvancePayTerm = false;
            }

            return isAdvancePayTerm;
        }

        // Added by Neha Thakur on 20 June 2018 to copy Blnaket PO ,Blanket So header as per selected Blanket Order Reference      
        public Dictionary<String, object> GetOrderHeader(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            StringBuilder sql = new StringBuilder();
            int _Order_ID = 0;
            bool _PrefixVA009;
            _Order_ID = Util.GetValueOfInt(paramValue[0]);
            _PrefixVA009 = Util.GetValueOfBool(paramValue[1]);
            Dictionary<String, object> retDir = null;
            if (_Order_ID > 0)
            {
                sql.Append("SELECT VAF_CLIENT_ID, VAF_ORG_ID, DOCUMENTNO, POREFERENCE, DESCRIPTION, VAB_DocTypesTARGET_ID, DATEORDERED, DATEPROMISED, ORDERVALIDFROM, ORDERVALIDTO, VAB_BUSINESSPARTNER_ID, BILL_BPARTNER_ID, VAB_BPart_Location_ID, BILL_LOCATION_ID, VAF_USERCONTACT_ID, BILL_USER_ID, VAM_Warehouse_ID, PRIORITYRULE, VAM_PriceList_ID, VAB_INCOTERM_ID, VAB_CURRENCY_ID, VAB_CurrencyType_ID, SALESREP_ID,");
                if (_PrefixVA009)
                {
                    sql.Append("VA009_PAYMENTMETHOD_ID,");
                }
                sql.Append("VAB_PAYMENTTERM_ID, VAB_PROMOTION_ID, VAB_BILLINGCODE_ID, VAF_ORGTRX_ID, USER1_ID, USER2_ID,");
                sql.Append("TOTALLINES, GRANDTOTAL FROM VAB_ORDER WHERE VAB_ORDER_ID= " + _Order_ID);
                DataSet ds = DB.ExecuteDataset(sql.ToString(), null, null);
                sql.Clear();
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    retDir = new Dictionary<string, object>();
                    retDir["VAF_Client_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAF_Client_ID"]);
                    retDir["VAF_Org_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAF_Org_ID"]);
                    retDir["Description"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Description"]);
                    retDir["VAB_DocTypesTarget_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAB_DocTypesTarget_ID"]);
                    retDir["DateOrdered"] = Util.GetValueOfDateTime(ds.Tables[0].Rows[0]["DateOrdered"]);
                    retDir["DatePromised"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["DatePromised"]);
                    retDir["OrderValidFrom"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["OrderValidFrom"]);
                    retDir["OrderValidTo"] = Util.GetValueOfDateTime(ds.Tables[0].Rows[0]["OrderValidTo"]);
                    retDir["VAB_BusinessPartner_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAB_BusinessPartner_ID"]);
                    retDir["Bill_BPartner_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["Bill_BPartner_ID"]);
                    retDir["VAB_BPart_Location_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAB_BPart_Location_ID"]);
                    retDir["Bill_Location_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["Bill_Location_ID"]);
                    retDir["VAF_UserContact_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAF_UserContact_ID"]);
                    retDir["Bill_User_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["Bill_User_ID"]);
                    retDir["VAM_Warehouse_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAM_Warehouse_ID"]);
                    retDir["PriorityRule"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["PriorityRule"]);
                    retDir["VAM_PriceList_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAM_PriceList_ID"]);
                    retDir["VAB_IncoTerm_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAB_IncoTerm_ID"]);
                    retDir["VAB_Currency_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAB_Currency_ID"]);
                    retDir["VAB_CurrencyType_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAB_CurrencyType_ID"]);
                    retDir["SalesRep_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["SalesRep_ID"]);
                    if (_PrefixVA009)
                    {
                        retDir["VA009_PaymentMethod_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VA009_PaymentMethod_ID"]);
                    }
                    retDir["VAB_PaymentTerm_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAB_PaymentTerm_ID"]);
                    retDir["VAB_Promotion_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAB_Promotion_ID"]);
                    retDir["VAB_BillingCode_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAB_BillingCode_ID"]);
                    retDir["VAF_OrgTrx_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VAF_OrgTrx_ID"]);
                    retDir["User1_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["User1_ID"]);
                    retDir["User2_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["User2_ID"]);
                    retDir["TotalLines"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["TotalLines"]);
                    retDir["GrandTotal"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["GrandTotal"]);

                }
            }
            return retDir;
        }

        /// <summary>
        /// Getting the percision values
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns>get Percision value</returns>

        public int GetPrecision(Ctx ctx,string fields)
        {
            string sql = "SELECT CC.StdPrecision FROM VAB_Order CO INNER JOIN VAB_Currency CC on CC.VAB_Currency_Id = Co.VAB_Currency_Id where CO.VAB_Order_Id= " + Util.GetValueOfInt(fields);
            var stdPrecision = Util.GetValueOfInt(DB.ExecuteScalar(sql, null,null));
            return stdPrecision;
        }
    }
}