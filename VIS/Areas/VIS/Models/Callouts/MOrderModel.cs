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
            int C_Order_ID;

            //Assign parameter value
            C_Order_ID = Util.GetValueOfInt(paramValue[0].ToString());
            MOrder order = new MOrder(ctx, C_Order_ID, null);
            // End Assign parameter value

            Dictionary<String, object> retDic = new Dictionary<string, object>();
            // Reset Orig Shipment
            retDic["C_BPartner_ID"] = Util.GetValueOfString(order.GetC_BPartner_ID());
            retDic["C_BPartner_Location_ID"] = Util.GetValueOfString(order.GetC_BPartner_Location_ID());
            retDic["Bill_BPartner_ID"] = Util.GetValueOfString(order.GetBill_BPartner_ID());
            retDic["Bill_Location_ID"] = Util.GetValueOfString(order.GetBill_Location_ID());
            if (order.GetAD_User_ID() != 0)
                retDic["AD_User_ID"] = Util.GetValueOfString(order.GetAD_User_ID());
            if (order.GetBill_User_ID() != 0)
                retDic["Bill_User_ID"] = Util.GetValueOfString(order.GetBill_User_ID());
            retDic["M_PriceList_ID"] = Util.GetValueOfString(order.GetM_PriceList_ID());
            retDic["PaymentRule"] = order.GetPaymentRule();
            retDic["C_PaymentTerm_ID"] = Util.GetValueOfString(order.GetC_PaymentTerm_ID());
            //mTab.setValue ("DeliveryRule", X_C_Order.DELIVERYRULE_Manual);
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
            retDic["AD_Org_ID"] = Util.GetValueOfString(order.GetAD_Org_ID());
            retDic["DeliveryRule"] = order.GetDeliveryRule();
            retDic["DeliveryViaRule"] = order.GetDeliveryViaRule();
            retDic["M_Shipper_ID"] = Util.GetValueOfString(order.GetM_Shipper_ID());
            retDic["FreightAmt"] = Util.GetValueOfString(order.GetFreightAmt());
            retDic["AD_OrgTrx_ID"] = Util.GetValueOfString(order.GetAD_OrgTrx_ID());
            retDic["C_Activity_ID"] = Util.GetValueOfString(order.GetC_Activity_ID());
            retDic["C_Campaign_ID"] = Util.GetValueOfString(order.GetC_Campaign_ID());
            retDic["C_Project_ID"] = Util.GetValueOfString(order.GetC_Project_ID());
            retDic["User1_ID"] = Util.GetValueOfString(order.GetUser1_ID());
            retDic["User2_ID"] = Util.GetValueOfString(order.GetUser2_ID());
            retDic["M_Warehouse_ID"] = Util.GetValueOfString(order.GetM_Warehouse_ID());
            retDic["Orig_Order_ID"] = Util.GetValueOfString(order.GetOrig_Order_ID());
            retDic["Orig_InOut_ID"] = Util.GetValueOfString(order.GetOrig_InOut_ID());
            //Added By Amit
            retDic["IsSOTrx"] = Util.GetValueOfString(order.IsSOTrx());
            retDic["IsReturnTrx"] = Util.GetValueOfString(order.IsReturnTrx());
            retDic["C_Payment_ID"] = Util.GetValueOfString(order.GetPaymentMethod());
            retDic["VA009_PaymentMethod_ID"] = Util.GetValueOfString(order.GetVA009_PaymentMethod_ID());
            retDic["SalesRep_ID"] = Util.GetValueOfString(order.GetSalesRep_ID());
            retDic["C_ProjectRef_ID"] = Util.GetValueOfString(order.GetC_ProjectRef_ID());
            retDic["PriorityRule"] = Util.GetValueOfString(order.GetPriorityRule());


            if (order.GetC_Currency_ID() != 0)
            {
                retDic["C_Currency_ID"] = Util.GetValueOfString(order.GetC_Currency_ID());
            }
            else
            {
                retDic["C_Currency_ID"] = "0";
            }
            //End
            // added by vivek on 09/10/2017 advised by pradeep to set drop ship checkbox value
            retDic["IsDropShip"] = Util.GetValueOfBool(order.IsDropShip()) ? "Y" : "N";

            // Added by Bharat on 30 Jan 2018 to set Inco Term from Order

            if (order.Get_ColumnIndex("C_IncoTerm_ID") > 0)
            {
                retDic["C_IncoTerm_ID"] = Util.GetValueOfString(order.GetC_IncoTerm_ID());
            }

            if (Env.IsModuleInstalled("VA077_"))
            {
                retDic["VA077_HistoricContractDate"] = Util.GetValueOfString(order.Get_Value("VA077_HistoricContractDate"));
                retDic["VA077_ChangeStartDate"] = Util.GetValueOfString(order.Get_Value("VA077_ChangeStartDate"));
                retDic["VA077_ContractCPStartDate"] = Util.GetValueOfString(order.Get_Value("VA077_ContractCPStartDate"));
                retDic["VA077_ContractCPEndDate"] = Util.GetValueOfString(order.Get_Value("VA077_ContractCPEndDate"));
                retDic["VA077_PartialAmtCatchUp"] = Util.GetValueOfString(order.Get_Value("VA077_PartialAmtCatchUp"));
                retDic["VA077_OldAnnualContractTotal"] = Util.GetValueOfString(order.Get_Value("VA077_OldAnnualContractTotal"));
                retDic["VA077_AdditionalAnnualCharge"] = Util.GetValueOfString(order.Get_Value("VA077_AdditionalAnnualCharge"));
                retDic["VA077_NewAnnualContractTotal"] = Util.GetValueOfString(order.Get_Value("VA077_NewAnnualContractTotal"));
                retDic["VA077_SalesCoWorker"] = Util.GetValueOfString(order.Get_Value("VA077_SalesCoWorker"));
                retDic["VA077_SalesCoWorkerPer"] = Util.GetValueOfString(order.Get_Value("VA077_SalesCoWorkerPer"));
                retDic["VA077_TotalMarginAmt"] = Util.GetValueOfString(order.Get_Value("VA077_TotalMarginAmt"));
                retDic["VA077_TotalPurchaseAmt"] = Util.GetValueOfString(order.Get_Value("VA077_TotalPurchaseAmt"));
                retDic["VA077_TotalSalesAmt"] = Util.GetValueOfString(order.Get_Value("VA077_TotalSalesAmt"));
                retDic["VA077_MarginPercent"] = Util.GetValueOfString(order.Get_Value("VA077_MarginPercent"));
             }

            return retDic;
        }

        //Added By Amit
        public int GetM_PriceList(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int C_Order_ID;
            C_Order_ID = Util.GetValueOfInt(paramValue[0].ToString());
            MOrder order = new MOrder(ctx, C_Order_ID, null);
            return order.GetM_PriceList_ID();
        }

        public int GetC_Currency_ID(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int C_Order_ID;
            C_Order_ID = Util.GetValueOfInt(paramValue[0].ToString());
            MOrder order = new MOrder(ctx, C_Order_ID, null);
            return order.GetC_Currency_ID();
        }
        //End

        // Added by Bharat on 16 May 2017        
        public Dictionary<String, object> GetPaymentMethod(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            bool countVA009 = false;
            int C_Order_ID = 0;
            countVA009 = Util.GetValueOfBool(paramValue[0]);
            C_Order_ID = Util.GetValueOfInt(paramValue[1]);
            Dictionary<String, object> retDir = null; ;
            if (countVA009)
            {
                string sql = "SELECT p.VA009_PaymentMethod_ID, p.VA009_PaymentBaseType FROM C_Order o INNER JOIN VA009_PaymentMethod p "
                + " ON o.VA009_PaymentMethod_ID = p.VA009_PaymentMethod_ID WHERE o.C_Order_ID = " + C_Order_ID;
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
            if (tableName == "C_Order")
            {
                if (referenceID > 0)
                {
                    MOrder ord = new MOrder(ctx, referenceID, null);
                    incoTerm_ID = ord.GetC_IncoTerm_ID();
                }
            }
            else if (tableName == "C_Invoice")
            {
                if (isSOTrx && referenceID > 0)
                {
                    MOrder ord = new MOrder(ctx, referenceID, null);
                    incoTerm_ID = ord.GetC_IncoTerm_ID();
                }
                else if (!isSOTrx && referenceID > 0 && refColumn == "C_Order_ID")
                {
                    MOrder ord = new MOrder(ctx, referenceID, null);
                    incoTerm_ID = ord.GetC_IncoTerm_ID();
                }
                else if (!isSOTrx && referenceID > 0 && refColumn == "M_InOut_ID")
                {
                    MInOut inOut = new MInOut(ctx, referenceID, null);
                    incoTerm_ID = inOut.GetC_IncoTerm_ID();
                }
            }
            else if (tableName == "M_InOut")
            {
                if (isSOTrx && referenceID > 0)
                {
                    MOrder ord = new MOrder(ctx, referenceID, null);
                    incoTerm_ID = ord.GetC_IncoTerm_ID();
                }
                else if (!isSOTrx && referenceID > 0 && refColumn == "C_Order_ID")
                {
                    MOrder ord = new MOrder(ctx, referenceID, null);
                    incoTerm_ID = ord.GetC_IncoTerm_ID();
                }
                else if (!isSOTrx && referenceID > 0 && refColumn == "C_Invoice_ID")
                {
                    MInvoice inv = new MInvoice(ctx, referenceID, null);
                    incoTerm_ID = inv.GetC_IncoTerm_ID();
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
            MDocType doctype = MDocType.Get(ctx, documnetType_Id);
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
                                            WHERE C_PaymentTerm_ID = " + PaymentTerm_Id, null, null)).Equals("Y"))
            {
                isAdvancePayTerm = false;
            }
            // check any payment term schedule is Advance, then return False
            else if (Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(*) FROM C_PaySchedule 
                                            WHERE VA009_Advance = 'Y' AND C_PaymentTerm_ID = " + PaymentTerm_Id, null, null)) > 0)
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
                sql.Append("SELECT AD_CLIENT_ID, AD_ORG_ID, DOCUMENTNO, POREFERENCE, DESCRIPTION, C_DOCTYPETARGET_ID, DATEORDERED, DATEPROMISED, ORDERVALIDFROM, ORDERVALIDTO, C_BPARTNER_ID, BILL_BPARTNER_ID, C_BPARTNER_LOCATION_ID, BILL_LOCATION_ID, AD_USER_ID, BILL_USER_ID, M_WAREHOUSE_ID, PRIORITYRULE, M_PRICELIST_ID, C_INCOTERM_ID, C_CURRENCY_ID, C_CONVERSIONTYPE_ID, SALESREP_ID,");
                if (_PrefixVA009)
                {
                    sql.Append("VA009_PAYMENTMETHOD_ID,");
                }
                sql.Append("C_PAYMENTTERM_ID, C_CAMPAIGN_ID, C_ACTIVITY_ID, AD_ORGTRX_ID, USER1_ID, USER2_ID,");
                sql.Append("TOTALLINES, GRANDTOTAL FROM C_ORDER WHERE C_ORDER_ID= " + _Order_ID);
                DataSet ds = DB.ExecuteDataset(sql.ToString(), null, null);
                sql.Clear();
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    retDir = new Dictionary<string, object>();
                    retDir["AD_Client_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_Client_ID"]);
                    retDir["AD_Org_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_Org_ID"]);
                    retDir["Description"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["Description"]);
                    retDir["C_DocTypeTarget_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_DocTypeTarget_ID"]);
                    retDir["DateOrdered"] = Util.GetValueOfDateTime(ds.Tables[0].Rows[0]["DateOrdered"]);
                    retDir["DatePromised"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["DatePromised"]);
                    retDir["OrderValidFrom"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["OrderValidFrom"]);
                    retDir["OrderValidTo"] = Util.GetValueOfDateTime(ds.Tables[0].Rows[0]["OrderValidTo"]);
                    retDir["C_BPartner_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_BPartner_ID"]);
                    retDir["Bill_BPartner_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["Bill_BPartner_ID"]);
                    retDir["C_BPartner_Location_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_BPartner_Location_ID"]);
                    retDir["Bill_Location_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["Bill_Location_ID"]);
                    retDir["AD_User_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_User_ID"]);
                    retDir["Bill_User_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["Bill_User_ID"]);
                    retDir["M_Warehouse_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_Warehouse_ID"]);
                    retDir["PriorityRule"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["PriorityRule"]);
                    retDir["M_PriceList_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["M_PriceList_ID"]);
                    retDir["C_IncoTerm_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_IncoTerm_ID"]);
                    retDir["C_Currency_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_Currency_ID"]);
                    retDir["C_ConversionType_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_ConversionType_ID"]);
                    retDir["SalesRep_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["SalesRep_ID"]);
                    if (_PrefixVA009)
                    {
                        retDir["VA009_PaymentMethod_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VA009_PaymentMethod_ID"]);
                    }
                    retDir["C_PaymentTerm_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_PaymentTerm_ID"]);
                    retDir["C_Campaign_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_Campaign_ID"]);
                    retDir["C_Activity_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_Activity_ID"]);
                    retDir["AD_OrgTrx_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["AD_OrgTrx_ID"]);
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
            string sql = "SELECT CC.StdPrecision FROM C_Order CO INNER JOIN C_Currency CC on CC.C_Currency_Id = Co.C_Currency_Id where CO.C_Order_Id= " + Util.GetValueOfInt(fields);
            var stdPrecision = Util.GetValueOfInt(DB.ExecuteScalar(sql, null,null));
            return stdPrecision;
        }
    }
}