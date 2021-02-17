/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVABInvoiceBatchLine
 * Purpose        : Invoice batch line setting
 * Class Used     : X_VAB_BatchInvoiceLine
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
//////using System.Windows.Forms;
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
    public class MVABInvoiceBatchLine : X_VAB_BatchInvoiceLine
    {
        /**
	 * 	Standard Constructor
	 *	@param ctx context
	 *	@param VAB_BatchInvoiceLine_ID id
	 *	@param trxName trx
	 */
        public MVABInvoiceBatchLine(Ctx ctx, int VAB_BatchInvoiceLine_ID,
            Trx trxName) :
            base(ctx, VAB_BatchInvoiceLine_ID, trxName)
        {
            if (VAB_BatchInvoiceLine_ID == 0)
            {
                //	setVAB_BatchInvoice_ID (0);
                /**
                setVAB_BusinessPartner_ID (0);
                setVAB_BPart_Location_ID (0);
                setVAB_Charge_ID (0);
                setVAB_DocTypes_ID (0);	// @VAB_DocTypes_ID@
                setVAB_TaxRate_ID (0);
                setDocumentNo (null);
                setLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM VAB_BatchInvoiceLine WHERE VAB_BatchInvoice_ID=@VAB_BatchInvoice_ID@
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
        public MVABInvoiceBatchLine(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
        }

        /**
         * 	Set Document Type - Callout.
         * 	@param oldVAB_DocTypes_ID old ID
         * 	@param newVAB_DocTypes_ID new ID
         * 	@param windowNo window
         */
        //@UICallout 
        public void SetVAB_DocTypes_ID(String oldVAB_DocTypes_ID,
               String newVAB_DocTypes_ID, int windowNo)
        {
            try
            {
                if (newVAB_DocTypes_ID == null || newVAB_DocTypes_ID.Length == 0)
                    return;
                int VAB_DocTypes_ID = int.Parse(newVAB_DocTypes_ID);
                SetVAB_DocTypes_ID(VAB_DocTypes_ID);
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
            int VAB_BatchInvoice_ID = GetVAB_BatchInvoice_ID();
            String sql = "SELECT COALESCE(MAX(VAB_BatchInvoiceLine_ID),0) FROM VAB_BatchInvoiceLine WHERE VAB_BatchInvoice_ID=" + VAB_BatchInvoice_ID;
            int VAB_BatchInvoiceLine_ID = DataBase.DB.GetSQLValue(null, sql);
            if (VAB_BatchInvoiceLine_ID == 0)
                return;
            MVABInvoiceBatchLine last = new MVABInvoiceBatchLine(GetCtx(), VAB_BatchInvoiceLine_ID, null);

            //	Need to Increase when different DocType or BP
            int VAB_DocTypes_ID = GetVAB_DocTypes_ID();
            int VAB_BusinessPartner_ID = GetVAB_BusinessPartner_ID();
            if (VAB_DocTypes_ID == last.GetVAB_DocTypes_ID()
                && VAB_BusinessPartner_ID == last.GetVAB_BusinessPartner_ID())
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
         *	@param oldVAB_BusinessPartner_ID old BP
         *	@param newVAB_BusinessPartner_ID new BP
         *	@param windowNo window no
         */
        //@UICallout
        public void SetVAB_BusinessPartner_ID(String oldVAB_BusinessPartner_ID,
            String newVAB_BusinessPartner_ID, int windowNo)
        {
            if (newVAB_BusinessPartner_ID == null || newVAB_BusinessPartner_ID.Length == 0)
                return;
            int VAB_BusinessPartner_ID = int.Parse(newVAB_BusinessPartner_ID);
            if (VAB_BusinessPartner_ID == 0)
                return;

            String sql = "SELECT p.VAF_Language,p.VAB_PaymentTerm_ID,"
                + " COALESCE(p.VAM_PriceList_ID,g.VAM_PriceList_ID) AS VAM_PriceList_ID, p.PaymentRule,p.POReference,"
                + " p.SO_Description,p.IsDiscountPrinted,"
                + " p.SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
                + " l.VAB_BPart_Location_ID,c.VAF_UserContact_ID,"
                + " COALESCE(p.PO_PriceList_ID,g.PO_PriceList_ID) AS PO_PriceList_ID, p.PaymentRulePO,p.PO_PaymentTerm_ID "
                + "FROM VAB_BusinessPartner p"
                + " INNER JOIN VAB_BPart_Category g ON (p.VAB_BPart_Category_ID=g.VAB_BPart_Category_ID)"
                + " LEFT OUTER JOIN VAB_BPart_Location l ON (p.VAB_BusinessPartner_ID=l.VAB_BusinessPartner_ID AND l.IsBillTo='Y' AND l.IsActive='Y')"
                + " LEFT OUTER JOIN VAF_UserContact c ON (p.VAB_BusinessPartner_ID=c.VAB_BusinessPartner_ID) "
                + "WHERE p.VAB_BusinessPartner_ID=@VAB_BusinessPartner_ID AND p.IsActive='Y'";		//	#1

            Boolean isSOTrx = GetCtx().IsSOTrx(windowNo);
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);
                dr = DataBase.DB.ExecuteReader(sql, param);
                if (dr.Read())
                {
                    //	Location
                    int VAB_BPart_Location_ID = Utility.Util.GetValueOfInt(dr["VAB_BPart_Location_ID"]);
                    //	overwritten by InfoBP selection - works only if InfoWindow
                    //	was used otherwise creates error (uses last value, may belong to differnt BP)
                    if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAB_BusinessPartner_ID") == VAB_BusinessPartner_ID)
                        VAB_BPart_Location_ID = GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAB_BPart_Location_ID");
                    if (VAB_BPart_Location_ID != 0)
                        SetVAB_BPart_Location_ID(VAB_BPart_Location_ID);
                    //	Contact - overwritten by InfoBP selection
                    int VAF_UserContact_ID = int.Parse(dr["VAF_UserContact_ID"].ToString());
                    if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAB_BusinessPartner_ID") == VAB_BusinessPartner_ID)
                        VAF_UserContact_ID = GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAF_UserContact_ID");
                    SetVAF_UserContact_ID(VAF_UserContact_ID);

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
            SetTax(windowNo, "VAB_BusinessPartner_ID");
        }

        /**
         * 	Set Partner Location - Callout
         *	@param oldVAB_BPart_Location_ID old value
         *	@param newVAB_BPart_Location_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetVAB_BPart_Location_ID(String oldVAB_BPart_Location_ID,
            String newVAB_BPart_Location_ID, int windowNo)
        {
            if (newVAB_BPart_Location_ID == null || newVAB_BPart_Location_ID.Length == 0)
                return;
            int VAB_BPart_Location_ID = int.Parse(newVAB_BPart_Location_ID);
            if (VAB_BPart_Location_ID == 0)
                return;
            //
            base.SetVAB_BPart_Location_ID(VAB_BPart_Location_ID);
            SetTax(windowNo, "VAB_BPart_Location_ID");
        }

        /**
         * 	Set Charge - Callout
         *	@param oldVAB_Charge_ID old value
         *	@param newVAB_Charge_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout 
        public void SetVAB_Charge_ID(String oldVAB_Charge_ID,
            String newVAB_Charge_ID, int windowNo)
        {
            if (newVAB_Charge_ID == null || newVAB_Charge_ID.Length == 0)
                return;
            int VAB_Charge_ID = int.Parse(newVAB_Charge_ID);
            base.SetVAB_Charge_ID(VAB_Charge_ID);

            MVABCharge charge = MVABCharge.Get(GetCtx(), VAB_Charge_ID);
            SetPriceEntered(charge.GetChargeAmt());
            SetTax(windowNo, "VAB_Charge_ID");
        }	//	setVAB_Charge_ID

        /// <summary>
        /// Calculate Tax
        /// </summary>
        /// <param name="windowNo"></param>
        /// <param name="columnName"></param>
        private void SetTax(int windowNo, String columnName)
        {
            int VAB_Charge_ID = GetVAB_Charge_ID();
            log.Fine("VAB_Charge_ID=" + VAB_Charge_ID);
            if (VAB_Charge_ID == 0)
            {
                SetAmt(windowNo, columnName);
                return;
            }
            //	Check Partner Location
            int VAB_BPart_Location_ID = GetVAB_BPart_Location_ID();
            log.Fine("BP_Location=" + VAB_BPart_Location_ID);
            if (VAB_BPart_Location_ID == 0)
            {
                SetAmt(windowNo, columnName);
                return;
            }
            //	Dates
            DateTime? billDate = GetDateInvoiced();
            log.Fine("Bill Date=" + billDate);
            DateTime? shipDate = billDate;
            log.Fine("Ship Date=" + shipDate);

            int VAF_Org_ID = GetVAF_Org_ID();
            log.Fine("Org=" + VAF_Org_ID);
            MVAFOrg org = MVAFOrg.Get(GetCtx(), VAF_Org_ID);
            int VAM_Warehouse_ID = org.GetVAM_Warehouse_ID();
            log.Fine("Warehouse=" + VAM_Warehouse_ID);

            Boolean isSOTrx = GetCtx().IsSOTrx(windowNo);
            //
            int VAB_TaxRate_ID = Tax.Get(GetCtx(), 0, VAB_Charge_ID, billDate, shipDate,
                VAF_Org_ID, VAM_Warehouse_ID, VAB_BPart_Location_ID, VAB_BPart_Location_ID,
                isSOTrx);
            log.Info("Tax ID=" + VAB_TaxRate_ID + " - SOTrx=" + isSOTrx);

            if (VAB_TaxRate_ID == 0)
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
                base.SetVAB_TaxRate_ID(VAB_TaxRate_ID);
            }
            SetAmt(windowNo, columnName);
        }

        /**
         * 	Set Tax - Callout
         *	@param oldVAB_TaxRate_ID old value
         *	@param newVAB_TaxRate_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout 
        public void SetVAB_TaxRate_ID(String oldVAB_TaxRate_ID,
            String newVAB_TaxRate_ID, int windowNo)
        {
            try
            {
                if (newVAB_TaxRate_ID == null || newVAB_TaxRate_ID.Length == 0)
                    return;
                int VAB_TaxRate_ID = int.Parse(newVAB_TaxRate_ID);
                SetVAB_TaxRate_ID(VAB_TaxRate_ID);
                SetAmt(windowNo, "VAB_TaxRate_ID");
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
                int VAB_TaxRate_ID = GetVAB_TaxRate_ID();
                if (VAB_TaxRate_ID != 0)
                {
                    MTax tax = new MTax(GetCtx(), VAB_TaxRate_ID, null);
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
                String sql = "UPDATE VAB_BatchInvoice h "
                    + "SET DocumentAmt = NVL((SELECT SUM(LineTotalAmt) FROM VAB_BatchInvoiceLine l "
                        + "WHERE h.VAB_BatchInvoice_ID=l.VAB_BatchInvoice_ID AND l.IsActive='Y'),0) "
                    + "WHERE VAB_BatchInvoice_ID=" + GetVAB_BatchInvoice_ID();
                DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            }
            return success;
        }
    }
}
