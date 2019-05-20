using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MPaymentModel
    {
        /// <summary>
        /// GetPayment
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetPayment(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int C_Payment_ID;
            //Assign parameter value
            C_Payment_ID = Util.GetValueOfInt(paramValue[0].ToString());
            MPayment payment = new MPayment(ctx, C_Payment_ID, null);
            Dictionary<string, string> result = new Dictionary<string, string>();
            result["C_Charge_ID"] = payment.GetC_Charge_ID().ToString();
            result["C_Invoice_ID"] = payment.GetC_Invoice_ID().ToString();
            result["C_Order_ID"] = payment.GetC_Order_ID().ToString();
            return result;
        }

        //Added by Bharat on 12/May/2017
        /// <summary>
        /// GetPayment Amount
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public decimal GetPayAmt(Ctx ctx, string fields)
        {
            int C_Payment_ID;
            //Assign parameter value
            C_Payment_ID = Util.GetValueOfInt(fields);
            MPayment payment = new MPayment(ctx, C_Payment_ID, null);
            return payment.GetPayAmt();
        }

        //Added by Bharat on 17/May/2017
        /// <summary>
        /// Get Invoice Data
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<String, Object> GetInvoiceData(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int C_Invoice_ID = Util.GetValueOfInt(paramValue[0]);
            int C_PaySchedule_ID = Util.GetValueOfInt(paramValue[1]);
            DateTime? trxDate = Util.GetValueOfDateTime(paramValue[2]);
            Dictionary<String, Object> retDic = null;
            string sql = "";
            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA009_'  AND IsActive = 'Y'", null, null)) > 0)
            {
                sql = "SELECT i.C_BPartner_ID, i.C_Currency_ID,i.C_Bpartner_Location_Id,"
                    //+ " invoiceOpen(C_Invoice_ID, @param1) as invoiceOpen,"
                     + " NVL(p.DueAmt , 0) - NVL(p.VA009_PaidAmntInvce , 0) as invoiceOpen,"
                     + " invoiceDiscount(" + C_Invoice_ID + ",@param1,"+ C_PaySchedule_ID +") as invoiceDiscount,"
                     + " i.IsSOTrx, i.IsInDispute, i.IsReturnTrx"
                     + " FROM C_Invoice i INNER JOIN C_InvoicePaySchedule p ON p.C_Invoice_ID = i.C_Invoice_ID"
                     + " WHERE i.C_Invoice_ID=" + C_Invoice_ID + " AND p.C_InvoicePaySchedule_ID=" + C_PaySchedule_ID;
            }
            else
            {
                sql = "SELECT i.C_BPartner_ID, i.C_Currency_ID,i.C_Bpartner_Location_Id,"
                    //+ " invoiceOpen(C_Invoice_ID, @param1) as invoiceOpen,"
                    + " p.DueAmt as invoiceOpen,"
                    + " invoiceDiscount(" + C_Invoice_ID + ",@param1," + C_PaySchedule_ID + ") as invoiceDiscount,"
                    + " i.IsSOTrx, i.IsInDispute, i.IsReturnTrx"
                    + " FROM C_Invoice i INNER JOIN C_InvoicePaySchedule p ON p.C_Invoice_ID = i.C_Invoice_ID"
                    + " WHERE i.C_Invoice_ID=" + C_Invoice_ID + " AND p.C_InvoicePaySchedule_ID=" + C_PaySchedule_ID;
            }
            SqlParameter[] param = new SqlParameter[1];
            //param[0] = new SqlParameter("@param1", C_PaySchedule_ID);
            param[0] = new SqlParameter("@param1", trxDate);
            //param[2] = new SqlParameter("@param3", C_PaySchedule_ID);
            //param[3] = new SqlParameter("@param4", C_Invoice_ID);
            DataSet ds = DB.ExecuteDataset(sql, param, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new Dictionary<string, object>();
                retDic["C_BPartner_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_BPartner_ID"]);
                retDic["C_Bpartner_Location_Id"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_Bpartner_Location_Id"]);
                retDic["C_Currency_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_Currency_ID"]);
                retDic["invoiceOpen"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["invoiceOpen"]);
                retDic["invoiceDiscount"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["invoiceDiscount"]);
                retDic["IsSOTrx"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["IsSOTrx"]);
                retDic["IsInDispute"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["IsInDispute"]);
                retDic["IsReturnTrx"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["IsReturnTrx"]);
            }
            return retDic;
        }

        //Added by Bharat on 17/May/2017
        /// <summary>
        /// Get Business Partner OutStanding Amount
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public string GetOutStandingAmt(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            bool countVA027 = Util.GetValueOfBool(paramValue[0]);
            int bp_BusinessPartner = Util.GetValueOfInt(paramValue[1]);
            DateTime? asOnDate = Util.GetValueOfDateTime(paramValue[2]);
            string Amounts = "";
            string sql = "SELECT LTRIM(MAX(SYS_CONNECT_BY_PATH( ConvertPrice, ',')),',') amounts FROM " +
                "(SELECT ConvertPrice, ROW_NUMBER () OVER (ORDER BY ConvertPrice ) RN, COUNT (*) OVER () CNT FROM " +
                "(SELECT iso_code || ':' || SUM(OpenAmt) AS ConvertPrice " +
                "FROM (SELECT c.iso_code,  invoiceOpen (i.C_Invoice_ID,i.C_InvoicePaySchedule_ID)*MultiplierAP AS OpenAmt FROM C_Invoice_V i " +
                "LEFT JOIN C_Currency C ON C.C_Currency_ID=i.C_Currency_ID " +
                "LEFT JOIN C_InvoicePaySchedule IPS ON IPS.c_invoice_ID = i.c_invoice_ID " +
                "WHERE i.docstatus IN ('CO','CL') AND i.IsActive ='Y' AND i.ispaid ='N' " +
                "AND ips.duedate IS NOT NULL AND NVL(ips.dueamt,0)!=0 AND i.c_bpartner_id = " + bp_BusinessPartner +
                " AND ips.duedate <= (CASE WHEN @param1 > sysdate THEN sysdate ELSE @param2 END ) " +
                "UNION SELECT c.iso_code, paymentAvailable(p.C_Payment_ID)*p.MultiplierAP*-1 AS OpenAmt " +
                "FROM C_Payment_v p LEFT JOIN C_Currency C ON C.C_Currency_ID=p.C_Currency_ID " +
                "LEFT JOIN c_payment pay ON (p.c_payment_id   =pay.c_payment_ID) WHERE p.IsAllocated  ='N' " +
                "AND p.C_BPARTNER_ID = " + bp_BusinessPartner + " AND p.DocStatus     IN ('CO','CL') " +
                "AND pay.DateTrx <= ( CASE WHEN @param3 > sysdate THEN sysdate ELSE @param4 END) " +
                ") GROUP BY iso_code ) ) WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1 ";
            SqlParameter[] param = new SqlParameter[4];
            param[0] = new SqlParameter("@param1", asOnDate);
            param[1] = new SqlParameter("@param2", asOnDate);
            param[2] = new SqlParameter("@param3", asOnDate);
            param[3] = new SqlParameter("@param4", asOnDate);
            Amounts = Util.GetValueOfString(DB.ExecuteScalar(sql, param, null));
            if (countVA027)
            {
                sql = @"SELECT LTRIM(MAX(SYS_CONNECT_BY_PATH( ConvertPrice, ',')),',') amounts
                    FROM (SELECT ConvertPrice, ROW_NUMBER () OVER (ORDER BY ConvertPrice ) RN,
                    COUNT (*) OVER () CNT FROM  
                    (SELECT ' PDC-'||iso_code|| ':'|| ROUND(SUM(PdcDue),2) AS ConvertPrice 
                    FROM (SELECT c.iso_code, CASE WHEN (pdc.VA027_MultiCheque = 'Y') THEN chk.VA027_ChequeAmount 
                    ELSE pdc.VA027_PayAmt END AS PdcDue 
                    FROM VA027_PostDatedCheck pdc LEFT JOIN VA027_ChequeDetails chk 
                    ON chk.VA027_PostDatedCheck_ID = pdc.VA027_PostDatedCheck_ID 
                    INNER JOIN C_Currency C ON C.C_Currency_ID=PDC.C_Currency_ID 
                    INNER JOIN C_DocType doc ON doc.C_DocType_ID = pdc.C_DocType_ID
                    WHERE pdc.IsActive ='Y' AND doc.DocBaseType = 'PDR'
                    AND pdc.DocStatus = 'CO' AND pdc.VA027_PAYMENTGENERATED ='N' AND pdc.VA027_PaymentStatus !=3
                    AND (CASE WHEN pdc.VA027_MultiCheque = 'Y' THEN chk.VA027_CheckDate 
                    ELSE pdc.VA027_CheckDate END) <= @param1 AND PDC.c_bpartner_id = " + bp_BusinessPartner +
                    @" UNION SELECT c.iso_code, CASE WHEN (pdc.VA027_MultiCheque = 'Y') THEN chk.VA027_ChequeAmount*-1 
                    ELSE pdc.VA027_PayAmt*-1 END AS PdcDue 
                    FROM VA027_PostDatedCheck pdc LEFT JOIN VA027_ChequeDetails chk 
                    ON chk.VA027_PostDatedCheck_ID = pdc.VA027_PostDatedCheck_ID 
                    INNER JOIN C_Currency C ON C.C_Currency_ID=pdc.C_Currency_ID
                    INNER JOIN C_DocType doc ON doc.C_DocType_ID = pdc.C_DocType_ID 
                    WHERE pdc.IsActive ='Y' AND doc.DocBaseType = 'PDP' 
                    AND pdc.VA027_PAYMENTGENERATED ='N' AND pdc.VA027_PaymentStatus !=3 
                    AND (CASE WHEN pdc.VA027_MultiCheque = 'Y' THEN chk.VA027_CheckDate 
                    ELSE pdc.VA027_CheckDate END) <= @param2 AND pdc.c_bpartner_id = " + bp_BusinessPartner +
                    @") GROUP BY iso_code )) WHERE RN = CNT START WITH RN = 1 CONNECT BY RN = PRIOR RN + 1";
                SqlParameter[] param1 = new SqlParameter[2];
                param1[0] = new SqlParameter("@param1", asOnDate);
                param1[1] = new SqlParameter("@param2", asOnDate);
                Amounts += Util.GetValueOfString(DB.ExecuteScalar(sql, param1, null));
            }
            return Amounts;
        }

        //Added by Bharat on 18/May/2017
        /// <summary>
        /// Get Order Data
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<String, Object> GetOrderData(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            bool countVA009 = Util.GetValueOfBool(paramValue[0]);
            int C_Order_ID = Util.GetValueOfInt(paramValue[1]);
            Dictionary<String, Object> retDic = null;
            string sql = "SELECT";
            if (countVA009)
            {
                sql += " VA009_PaymentMethod_ID,";
            }
            sql += " C_BPartner_ID, C_Currency_ID, GrandTotal, C_Bpartner_Location_ID"
                + " FROM C_Order WHERE C_Order_ID = " + C_Order_ID;

            DataSet ds = DB.ExecuteDataset(sql, null, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                retDic = new Dictionary<string, object>();
                if (countVA009)
                {
                    retDic["VA009_PaymentMethod_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["VA009_PaymentMethod_ID"]);
                }
                retDic["C_BPartner_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_BPartner_ID"]);
                retDic["C_Currency_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_Currency_ID"]);
                retDic["GrandTotal"] = Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["GrandTotal"]);
                retDic["C_BPartner_Location_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_BPartner_Location_ID"]);
            }
            return retDic;
        }
    }
}