/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MInvoiceBatchLine
 * Purpose        : Invoice batch line setting
 * Class Used     : X_C_InvoiceBatchLine
 * Chronological    Development
 * Raghunandan     17-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Data.SqlClient;
using VAdvantage.Logging;
//using java.math;
namespace VAdvantage.Model
{
    public class MInvoiceBatchLine : X_C_InvoiceBatchLine
    {
        /**
	 * 	Standard Constructor
	 *	@param ctx context
	 *	@param C_InvoiceBatchLine_ID id
	 *	@param trxName trx
	 */
        public MInvoiceBatchLine(Ctx ctx, int C_InvoiceBatchLine_ID,
            Trx trxName) :
            base(ctx, C_InvoiceBatchLine_ID, trxName)
        {
            if (C_InvoiceBatchLine_ID == 0)
            {
                //	setC_InvoiceBatch_ID (0);
                /**
                setC_BPartner_ID (0);
                setC_BPartner_Location_ID (0);
                setC_Charge_ID (0);
                setC_DocType_ID (0);	// @C_DocType_ID@
                setC_Tax_ID (0);
                setDocumentNo (null);
                setLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM C_InvoiceBatchLine WHERE C_InvoiceBatch_ID=@C_InvoiceBatch_ID@
                **/
                //SetDateAcct (new Timestamp(System.currentTimeMillis()));	// @DateDoc@
                SetDateAcct(DateTime.Now);	// @DateDoc@
                //SetDateInvoiced (new Timestamp(System.currentTimeMillis()));	// @DateDoc@
                SetDateInvoiced(DateTime.Now);	// @DateDoc@
                SetIsTaxIncluded(false);
                SetLineNetAmt(Env.ZERO);
                SetLineTotalAmt(Env.ZERO);
                SetPriceEntered(Env.ZERO);
                SetQtyEntered(Env.ONE);	// 1
                SetTaxAmt(Env.ZERO);
                SetProcessed(false);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param rs result set
         *	@param trxName trx
         */
        public MInvoiceBatchLine(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
        }

        /**
         * 	Set Document Type - Callout.
         * 	@param oldC_DocType_ID old ID
         * 	@param newC_DocType_ID new ID
         * 	@param windowNo window
         */
        //@UICallout 
        public void SetC_DocType_ID(String oldC_DocType_ID,
               String newC_DocType_ID, int windowNo)
        {
            try
            {
                if (newC_DocType_ID == null || newC_DocType_ID.Length == 0)
                    return;
                int C_DocType_ID = int.Parse(newC_DocType_ID);
                SetC_DocType_ID(C_DocType_ID);
                SetDocumentNo();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /**
         * 	Set DateInvoiced - Callout
         *	@param oldDateInvoiced old
         *	@param newDateInvoiced new
         *	@param windowNo window no
         */
        //@UICallout 
        public void SetDateInvoiced(String oldDateInvoiced,
               String newDateInvoiced, int windowNo)
        {
            try
            {
                if (newDateInvoiced == null || newDateInvoiced.Length == 0)
                    return;
                DateTime dateInvoiced = DateTime.Parse(newDateInvoiced);
                if (dateInvoiced == null)
                    return;
                SetDateInvoiced(dateInvoiced);
                SetDocumentNo();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Set Date Invoiced and Acct Date
        /// </summary>
        /// <param name="dateOrdered"></param>
        public new void SetDateInvoiced(DateTime? dateOrdered)
        {
            base.SetDateInvoiced(dateOrdered);
            base.SetDateAcct(dateOrdered);
        }

        /// <summary>
        /// Set Document No (increase existing)
        /// </summary>
        private void SetDocumentNo()
        {
            //	Get last line
            int C_InvoiceBatch_ID = GetC_InvoiceBatch_ID();
            String sql = "SELECT COALESCE(MAX(C_InvoiceBatchLine_ID),0) FROM C_InvoiceBatchLine WHERE C_InvoiceBatch_ID=" + C_InvoiceBatch_ID;
            int C_InvoiceBatchLine_ID = DataBase.DB.GetSQLValue(null, sql);
            if (C_InvoiceBatchLine_ID == 0)
                return;
            MInvoiceBatchLine last = new MInvoiceBatchLine(GetCtx(), C_InvoiceBatchLine_ID, null);

            //	Need to Increase when different DocType or BP
            int C_DocType_ID = GetC_DocType_ID();
            int C_BPartner_ID = GetC_BPartner_ID();
            if (C_DocType_ID == last.GetC_DocType_ID()
                && C_BPartner_ID == last.GetC_BPartner_ID())
                return;

            //	New Number
            String oldDocNo = last.GetDocumentNo();
            if (oldDocNo == null)
                return;
            int docNo = 0;
            try
            {
                docNo = int.Parse(oldDocNo);
            }
            catch
            {
            }
            if (docNo == 0)
                return;
            //String newDocNo = String.valueOf(docNo+1);
            String newDocNo = (docNo + 1).ToString();
            SetDocumentNo(newDocNo);
        }

        /**
         * 	Set Business Partner - Callout
         *	@param oldC_BPartner_ID old BP
         *	@param newC_BPartner_ID new BP
         *	@param windowNo window no
         */
        //@UICallout
        public void SetC_BPartner_ID(String oldC_BPartner_ID,
            String newC_BPartner_ID, int windowNo)
        {
            if (newC_BPartner_ID == null || newC_BPartner_ID.Length == 0)
                return;
            int C_BPartner_ID = int.Parse(newC_BPartner_ID);
            if (C_BPartner_ID == 0)
                return;

            String sql = "SELECT p.AD_Language,p.C_PaymentTerm_ID,"
                + " COALESCE(p.M_PriceList_ID,g.M_PriceList_ID) AS M_PriceList_ID, p.PaymentRule,p.POReference,"
                + " p.SO_Description,p.IsDiscountPrinted,"
                + " p.SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
                + " l.C_BPartner_Location_ID,c.AD_User_ID,"
                + " COALESCE(p.PO_PriceList_ID,g.PO_PriceList_ID) AS PO_PriceList_ID, p.PaymentRulePO,p.PO_PaymentTerm_ID "
                + "FROM C_BPartner p"
                + " INNER JOIN C_BP_Group g ON (p.C_BP_Group_ID=g.C_BP_Group_ID)"
                + " LEFT OUTER JOIN C_BPartner_Location l ON (p.C_BPartner_ID=l.C_BPartner_ID AND l.IsBillTo='Y' AND l.IsActive='Y')"
                + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
                + "WHERE p.C_BPartner_ID=@C_BPartner_ID AND p.IsActive='Y'";		//	#1

            Boolean isSOTrx = GetCtx().IsSOTrx(windowNo);
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@C_BPartner_ID", C_BPartner_ID);
                dr = DataBase.DB.ExecuteReader(sql, param);
                if (dr.Read())
                {
                    //	Location
                    int C_BPartner_Location_ID = Utility.Util.GetValueOfInt(dr["C_BPartner_Location_ID"]);
                    //	overwritten by InfoBP selection - works only if InfoWindow
                    //	was used otherwise creates error (uses last value, may belong to differnt BP)
                    if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "C_BPartner_ID") == C_BPartner_ID)
                        C_BPartner_Location_ID = GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "C_BPartner_Location_ID");
                    if (C_BPartner_Location_ID != 0)
                        SetC_BPartner_Location_ID(C_BPartner_Location_ID);
                    //	Contact - overwritten by InfoBP selection
                    int AD_User_ID = int.Parse(dr["AD_User_ID"].ToString());
                    if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "C_BPartner_ID") == C_BPartner_ID)
                        AD_User_ID = GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "AD_User_ID");
                    SetAD_User_ID(AD_User_ID);

                    //	CreditAvailable
                    if (isSOTrx)
                    {
                        Decimal CreditLimit = Utility.Util.GetValueOfDecimal(dr["SO_CreditLimit"].ToString());
                        //	String SOCreditStatus = rs.getString("SOCreditStatus");
                        if (int.Parse(CreditLimit.ToString()) != 0)
                        {
                            Decimal creditAvailable = Utility.Util.GetValueOfDecimal(dr["CreditAvailable"].ToString());
                            //if (p_changeVO != null && CreditAvailable != null && creditAvailable < 0)
                            //{
                            //    String msg = Msg.GetMsg(GetCtx(),"CreditLimitOver",
                            //        creditAvailable.ToString(DisplayType.AMOUNT));
                            //    //po_changeVO.addError(msg);
                            //}
                        }
                    }
                }
                dr.Close();

            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
                
            }
            
            //
            SetDocumentNo();
            SetTax(windowNo, "C_BPartner_ID");
        }

        /**
         * 	Set Partner Location - Callout
         *	@param oldC_BPartner_Location_ID old value
         *	@param newC_BPartner_Location_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetC_BPartner_Location_ID(String oldC_BPartner_Location_ID,
            String newC_BPartner_Location_ID, int windowNo)
        {
            if (newC_BPartner_Location_ID == null || newC_BPartner_Location_ID.Length == 0)
                return;
            int C_BPartner_Location_ID = int.Parse(newC_BPartner_Location_ID);
            if (C_BPartner_Location_ID == 0)
                return;
            //
            base.SetC_BPartner_Location_ID(C_BPartner_Location_ID);
            SetTax(windowNo, "C_BPartner_Location_ID");
        }

        /**
         * 	Set Charge - Callout
         *	@param oldC_Charge_ID old value
         *	@param newC_Charge_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout 
        public void SetC_Charge_ID(String oldC_Charge_ID,
            String newC_Charge_ID, int windowNo)
        {
            if (newC_Charge_ID == null || newC_Charge_ID.Length == 0)
                return;
            int C_Charge_ID = int.Parse(newC_Charge_ID);
            base.SetC_Charge_ID(C_Charge_ID);

            MCharge charge = MCharge.Get(GetCtx(), C_Charge_ID);
            SetPriceEntered(charge.GetChargeAmt());
            SetTax(windowNo, "C_Charge_ID");
        }	//	setC_Charge_ID

        /// <summary>
        /// Calculate Tax
        /// </summary>
        /// <param name="windowNo"></param>
        /// <param name="columnName"></param>
        private void SetTax(int windowNo, String columnName)
        {
            int C_Charge_ID = GetC_Charge_ID();
            log.Fine("C_Charge_ID=" + C_Charge_ID);
            if (C_Charge_ID == 0)
            {
                SetAmt(windowNo, columnName);
                return;
            }
            //	Check Partner Location
            int C_BPartner_Location_ID = GetC_BPartner_Location_ID();
            log.Fine("BP_Location=" + C_BPartner_Location_ID);
            if (C_BPartner_Location_ID == 0)
            {
                SetAmt(windowNo, columnName);
                return;
            }
            //	Dates
            DateTime? billDate = GetDateInvoiced();
            log.Fine("Bill Date=" + billDate);
            DateTime? shipDate = billDate;
            log.Fine("Ship Date=" + shipDate);

            int AD_Org_ID = GetAD_Org_ID();
            log.Fine("Org=" + AD_Org_ID);
            MOrg org = MOrg.Get(GetCtx(), AD_Org_ID);
            int M_Warehouse_ID = org.GetM_Warehouse_ID();
            log.Fine("Warehouse=" + M_Warehouse_ID);

            Boolean isSOTrx = GetCtx().IsSOTrx(windowNo);
            //
            int C_Tax_ID = Tax.Get(GetCtx(), 0, C_Charge_ID, billDate, shipDate,
                AD_Org_ID, M_Warehouse_ID, C_BPartner_Location_ID, C_BPartner_Location_ID,
                isSOTrx);
            log.Info("Tax ID=" + C_Tax_ID + " - SOTrx=" + isSOTrx);

            if (C_Tax_ID == 0)
            {
                //ValueNamePair pp = CLogger.retrieveError();
                //if (pp != null)
                //{
                //p_changeVO.addError(pp.getValue());
                //}
                //else
                //{
                //p_changeVO.addError("Tax Error");
                //}
            }
            else
            {
                base.SetC_Tax_ID(C_Tax_ID);
            }
            SetAmt(windowNo, columnName);
        }

        /**
         * 	Set Tax - Callout
         *	@param oldC_Tax_ID old value
         *	@param newC_Tax_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout 
        public void SetC_Tax_ID(String oldC_Tax_ID,
            String newC_Tax_ID, int windowNo)
        {
            try
            {
                if (newC_Tax_ID == null || newC_Tax_ID.Length == 0)
                    return;
                int C_Tax_ID = int.Parse(newC_Tax_ID);
                SetC_Tax_ID(C_Tax_ID);
                SetAmt(windowNo, "C_Tax_ID");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /**
         * 	Set Tax Included - Callout
         *	@param oldIsTaxIncluded old value
         *	@param newIsTaxIncluded new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetIsTaxIncluded(String oldIsTaxIncluded,
            String newIsTaxIncluded, int windowNo)
        {
            try
            {
                Boolean IsTaxIncluded = "Y".Equals(newIsTaxIncluded);
                SetIsTaxIncluded(IsTaxIncluded);
                SetAmt(windowNo, "IsTaxIncluded");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        /**
         * 	Set PriceEntered - Callout
         *	@param oldPriceEntered old value
         *	@param newPriceEntered new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout 
        public void SetPriceEntered(String oldPriceEntered,
            String newPriceEntered, int windowNo)
        {
            try
            {
                if (newPriceEntered == null || newPriceEntered.Length == 0)
                    return;
                decimal PriceEntered = decimal.Parse(newPriceEntered);
                base.SetPriceEntered(PriceEntered);
                SetAmt(windowNo, "PriceEntered");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /**
         * 	Set QtyEntered - Callout
         *	@param oldQtyEntered old value
         *	@param newQtyEntered new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetQtyEntered(String oldQtyEntered,
            String newQtyEntered, int windowNo)
        {
            try
            {
                if (newQtyEntered == null || newQtyEntered.Length == 0)
                    return;
                decimal QtyEntered = decimal.Parse(newQtyEntered);
                base.SetQtyEntered(QtyEntered);
                SetAmt(windowNo, "QtyEntered");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Set Amount (Callout)
        /// </summary>
        /// <param name="windowNo">window</param>
        /// <param name="columnName">changed column</param>
        private void SetAmt(int windowNo, String columnName)
        {
            //	get values
            Decimal qtyEntered = GetQtyEntered();
            Decimal priceEntered = GetPriceEntered();
            log.Fine("QtyEntered=" + qtyEntered + ", PriceEntered=" + priceEntered);
            //if (qtyEntered == null)
            //{
            //    qtyEntered = Env.ZERO;
            //}
            //if (priceEntered == null)
            //{
            //    priceEntered = Env.ZERO;
            //}

            //	Line Net Amt
            Decimal lineNetAmt = Decimal.Multiply(qtyEntered, priceEntered);
            int stdPrecision = GetCtx().GetStdPrecision();
            if (Env.Scale(lineNetAmt) > stdPrecision)
            {
                lineNetAmt = Decimal.Round(lineNetAmt, stdPrecision, MidpointRounding.AwayFromZero);
            }
            //	Calculate Tax Amount
            Boolean isTaxIncluded = IsTaxIncluded();
            Decimal? taxAmt = null;
            if (columnName.Equals("TaxAmt"))
            {
                taxAmt = GetTaxAmt();
            }
            else
            {
                int C_Tax_ID = GetC_Tax_ID();
                if (C_Tax_ID != 0)
                {
                    MTax tax = new MTax(GetCtx(), C_Tax_ID, null);
                    taxAmt = tax.CalculateTax(lineNetAmt, isTaxIncluded, stdPrecision);
                    SetTaxAmt(Convert.ToDecimal(taxAmt));
                }
            }
            if (isTaxIncluded)
            {
                SetLineTotalAmt(lineNetAmt);
                SetLineNetAmt(Decimal.Subtract(lineNetAmt, Convert.ToDecimal(taxAmt)));
            }
            else
            {
                SetLineNetAmt(lineNetAmt);
                SetLineTotalAmt(Decimal.Subtract(lineNetAmt, Convert.ToDecimal(taxAmt)));
            }
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns>true</returns>
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            // Amount
            //if (int.Parse(GetPriceEntered().ToString()) == 0)
            if (Env.Signum(GetPriceEntered()) == 0)
            {
                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "PriceEntered"));
                return false;
            }
            //return true;
            //if (GetPriceEntered().signum() == 0)
            //if (Env.Signum(GetPriceEntered()) == 0)
            //{
            //    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "PriceEntered"));
            //    return false;
            //}
            return true;
        }

        /**
         * 	After Save.
         * 	Update Header
         *	@param newRecord new
         *	@param success success
         *	@return success
         */
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            if (success)
            {
                String sql = "UPDATE C_InvoiceBatch h "
                    + "SET DocumentAmt = NVL((SELECT SUM(LineTotalAmt) FROM C_InvoiceBatchLine l "
                        + "WHERE h.C_InvoiceBatch_ID=l.C_InvoiceBatch_ID AND l.IsActive='Y'),0) "
                    + "WHERE C_InvoiceBatch_ID=" + GetC_InvoiceBatch_ID();
                DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            }
            return success;
        }
    }
}
