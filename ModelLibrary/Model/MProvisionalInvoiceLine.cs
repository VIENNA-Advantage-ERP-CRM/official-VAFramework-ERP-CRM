/********************************************************
 * Module Name    : VA Framework
 * Purpose        : 
 * Class Used     : MProvisionalInvoiceLine
 * Chronological Development
 * Amit Bansal     15/July/2021
 ******************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvanatge.Model;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MProvisionalInvoiceLine : X_C_ProvisionalInvoiceLine
    {
        /** Cached Precision			*/
        private int? _precision = null;
        //	Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MProvisionalInvoiceLine).FullName);
        // Price List
        private int _M_PriceList_ID = 0;
        // String
        private StringBuilder sql = new StringBuilder();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="C_ProvisionalInvoiceLine_ID">Provisional Invoice Line</param>
        /// <param name="trxName">Transaction</param>
        /// <writer>209</writer>
        public MProvisionalInvoiceLine(Ctx ctx, int C_ProvisionalInvoiceLine_ID, Trx trxName) :
        base(ctx, C_ProvisionalInvoiceLine_ID, null)
        {
            if (C_ProvisionalInvoiceLine_ID == 0)
            {
                SetLineNetAmt(Env.ZERO);
                SetPriceEntered(Env.ZERO);
                SetPriceList(Env.ZERO);
                SetM_AttributeSetInstance_ID(0);
                SetTaxAmt(Env.ZERO);
                SetQtyEntered(Env.ZERO);
                SetQtyInvoiced(Env.ZERO);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="dr">DataRow</param>
        /// <param name="trxName">Transaction</param>
        /// <writer>209</writer>
        public MProvisionalInvoiceLine(Ctx ctx, DataRow dr, Trx trxName) :
        base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="invoice">Provisional Invoice</param>
        public MProvisionalInvoiceLine(MProvisionalInvoice invoice)
           : this(invoice.GetCtx(), 0, invoice.Get_Trx())
        {
            if (invoice.Get_ID() == 0)
                throw new ArgumentException("Header not saved");
            SetClientOrg(invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID());
            SetC_ProvisionalInvoice_ID(invoice.GetC_ProvisionalInvoice_ID());
        }

        /// <summary>
        /// Set values from Order Line, Does not Set quantity!
        /// </summary>
        /// <param name="oLine">order line</param>
        public void SetOrderLine(MOrderLine oLine)
        {
            SetC_OrderLine_ID(oLine.GetC_OrderLine_ID());
            SetLine(oLine.GetLine());
            SetC_Charge_ID(oLine.GetC_Charge_ID());
            SetM_Product_ID(oLine.GetM_Product_ID());
            SetM_AttributeSetInstance_ID(oLine.GetM_AttributeSetInstance_ID());
            SetC_UOM_ID(oLine.GetC_UOM_ID());
            SetUnitPrice(oLine.GetPriceEntered());
            SetPriceEntered(oLine.GetPriceEntered());
            SetPriceList(oLine.GetPriceList());
            SetC_Tax_ID(oLine.GetC_Tax_ID());
        }

        /// <summary>
        /// Implement Before Save
        /// </summary>
        /// <param name="newRecord">newRecord</param>
        /// <returns>true, when success</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            // Calculate Percentage (((Price List - Price Entered) / Price List) * 100)
            if (Util.GetValueOfDecimal(GetPriceList()) != 0)
            {
                SetDiscount(Decimal.Round(Decimal.Multiply(Decimal.Divide(
                    Decimal.Subtract(Util.GetValueOfDecimal(GetPriceList()), Util.GetValueOfDecimal(GetPriceEntered())),
                     Util.GetValueOfDecimal(GetPriceList())), 100), 2));
            }
            else
            {
                SetDiscount(0);
            }

            // Set Total difference between PO Price and Provisional Price 
            SetTotalDifference(Decimal.Round(Decimal.Multiply(GetPerUnitDifference(), GetQtyEntered()), GetPrecision()));

            // Set Line Net Amount
            SetLineNetAmt();

            // Set tax amount
            if (((Decimal)GetTaxAmt()).CompareTo(Env.ZERO) == 0 || GetSurchargeAmt().CompareTo(Env.ZERO) == 0)
                SetTaxAmt();

            // set Taxable Amount -- (Line Total-Tax Amount)
            SetTaxBaseAmt(Decimal.Subtract(Decimal.Subtract(GetLineTotalAmt(), GetTaxAmt()), GetSurchargeAmt()));

            // Set Available Stock
            if (!IsProcessed() && GetAvailableStock() == 0)
            {
                SetAvailableStock(GetAvailableStock(GetM_InOutLine_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(), 0));
            }

            return true;
        }

        /// <summary>
        /// Implement After Save
        /// </summary>
        /// <param name="newRecord">newRecord</param>
        /// <param name="success">Success</param>
        /// <returns>true, when sucess</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
            {
                return success;
            }

            // for reversal record, not to execute Update Header tax
            return (GetReversalDoc_ID() <= 0 ? UpdateHeaderTax() : true);
        }

        /// <summary>
        /// Implement After Delete
        /// </summary>
        /// <param name="success">Success</param>
        /// <returns>True, when success</returns>
        protected override bool AfterDelete(bool success)
        {
            return UpdateHeaderTax();
        }

        /// <summary>
        /// This function is used to calulate Tax and Update line total and grand total
        /// </summary>
        /// <returns>true, when updated</returns>
        private bool UpdateHeaderTax()
        {
            //	Recalculate Tax for this Tax
            MProvisionalInvoiceTax tax = MProvisionalInvoiceTax.Get(this, GetPrecision(),
                false, Get_Trx());  //	current Tax
            if (tax != null)
            {
                if (!tax.CalculateTaxFromLines(null))
                    return false;
                if (!tax.Save(Get_Trx()))
                    return false;
            }

            MTax taxRate = tax.GetTax();
            if (taxRate.IsSummary())
            {
                if (!CalculateChildTax(new MProvisionalInvoice(GetCtx(), GetC_ProvisionalInvoice_ID(), Get_Trx()), tax, taxRate, Get_Trx()))
                {
                    return false;
                }
            }

            // if Surcharge Tax is selected then calculate Tax for this Surcharge Tax.
            else if (taxRate.GetSurcharge_Tax_ID() > 0)
            {
                tax = MProvisionalInvoiceTax.GetSurcharge(this, GetPrecision(), false, Get_Trx());  //	current Tax
                if (!tax.CalculateSurchargeFromLines())
                    return false;
                if (!tax.Save(Get_Trx()))
                    return false;
            }

            //	Update Invoice Header
            String sql = "UPDATE C_ProvisionalInvoice i"
                 + " SET TotalLines="
                 + " (SELECT COALESCE(SUM(LineNetAmt),0) FROM C_ProvisionalInvoiceLine il WHERE i.C_ProvisionalInvoice_ID=il.C_ProvisionalInvoice_ID) "
                 + " WHERE C_ProvisionalInvoice_ID=" + GetC_ProvisionalInvoice_ID();
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
            if (no != 1)
            {
                log.Warning("(1) #" + no);
            }

            if (IsTaxIncluded())
                sql = "UPDATE C_ProvisionalInvoice i "
                    + "SET GrandTotal=TotalLines "
                    + "WHERE C_ProvisionalInvoice_ID=" + GetC_ProvisionalInvoice_ID();
            else
                sql = "UPDATE C_ProvisionalInvoice i "
                    + "SET GrandTotal=TotalLines+"
                        + "(SELECT COALESCE(SUM(TaxAmt),0) FROM C_ProvisionalInvoiceTax it WHERE i.C_ProvisionalInvoice_ID=it.C_ProvisionalInvoice_ID) "
                        + "WHERE C_ProvisionalInvoice_ID=" + GetC_ProvisionalInvoice_ID();
            no = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
            if (no != 1)
            {
                log.Warning("(2) #" + no);
            }

            return no == 1;
        }

        /// <summary>
        /// Calculate Child Tax
        /// </summary>
        /// <param name="invoice">Provisional Invoice</param>
        /// <param name="iTax">Proviosnal Invoice Tax</param>
        /// <param name="tax">Tax</param>
        /// <param name="trxName">Trx</param>
        /// <returns>true, when calculated</returns>
        private bool CalculateChildTax(MProvisionalInvoice invoice, MProvisionalInvoiceTax iTax, MTax tax, Trx trxName)
        {
            MTax[] cTaxes = tax.GetChildTaxes(false);	//	Multiple taxes
            for (int j = 0; j < cTaxes.Length; j++)
            {
                MProvisionalInvoiceTax newITax = null;
                MTax cTax = cTaxes[j];
                Decimal taxAmt = cTax.CalculateTax(iTax.GetTaxAbleAmt(), false, GetPrecision());

                // check child tax record is avialable or not 
                // if not then create new record
                String sql = "SELECT * FROM C_ProvisionalInvoiceTax WHERE C_Invoice_ID=" + invoice.GetC_ProvisionalInvoice_ID() + " AND C_Tax_ID=" + cTax.GetC_Tax_ID();
                try
                {
                    DataSet ds = DB.ExecuteDataset(sql, null, trxName);
                    if (ds.Tables.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            newITax = new MProvisionalInvoiceTax(GetCtx(), dr, trxName);
                        }
                    }
                }
                catch (Exception e)
                {
                    _log.Log(Level.SEVERE, sql, e);
                }

                if (newITax != null)
                {
                    newITax.Set_TrxName(trxName);
                }

                // Create New
                if (newITax == null)
                {
                    newITax = new MProvisionalInvoiceTax(GetCtx(), 0, Get_Trx());
                    newITax.SetClientOrg(this);
                    newITax.SetC_ProvisionalInvoice_ID(GetC_ProvisionalInvoice_ID());
                    newITax.SetC_Tax_ID(cTax.GetC_Tax_ID());
                }

                newITax.SetPrecision(GetPrecision());
                newITax.SetIsTaxIncluded(IsTaxIncluded());
                newITax.SetTaxAbleAmt(iTax.GetTaxAbleAmt());
                newITax.SetTaxAmt(taxAmt);
                //Set Tax Amount (Base Currency) on Invoice Tax Window 
                if (newITax.Get_ColumnIndex("TaxBaseCurrencyAmt") > 0)
                {
                    decimal? baseTaxAmt = taxAmt;
                    int primaryAcctSchemaCurrency = GetCtx().GetContextAsInt("$C_Currency_ID");
                    if (invoice.GetC_Currency_ID() != primaryAcctSchemaCurrency)
                    {
                        baseTaxAmt = MConversionRate.Convert(GetCtx(), taxAmt, primaryAcctSchemaCurrency, invoice.GetC_Currency_ID(),
                                                                                   invoice.GetDateAcct(), invoice.GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID());
                    }
                    newITax.Set_Value("TaxBaseCurrencyAmt", baseTaxAmt);
                }
                if (!newITax.Save(Get_Trx()))
                    return false;
            }
            // Delete Summary Level Tax Line
            if (!iTax.Delete(true, Get_Trx()))
                return false;

            return true;
        }

        /// <summary>
        /// Set LineNetAmount
        /// </summary>
        public void SetLineNetAmt()
        {
            //	Calculations & Rounding 
            Decimal LineNetAmt = Decimal.Multiply(GetPriceEntered(), GetQtyEntered());
            Decimal LNAmt = LineNetAmt;
            if (Env.Scale(LineNetAmt) > GetPrecision())
            {
                LineNetAmt = Decimal.Round(LineNetAmt, GetPrecision(), MidpointRounding.AwayFromZero);
                base.SetLineNetAmt(LineNetAmt);
            }
            else
            {
                base.SetLineNetAmt(LineNetAmt);
            }
        }

        /// <summary>
        /// Set Tax Amount
        /// </summary>
        public void SetTaxAmt()
        {
            try
            {
                Decimal TaxAmt = Env.ZERO;
                if (GetC_Tax_ID() == 0)
                    return;

                MTax tax = MTax.Get(GetCtx(), GetC_Tax_ID());
                if (tax.IsDocumentLevel())
                    return;

                // if Surcharge Tax is selected on Tax, then calculate Tax accordingly
                if (tax.GetSurcharge_Tax_ID() > 0)
                {
                    Decimal surchargeAmt = Env.ZERO;

                    // Calculate Surcharge Amount
                    TaxAmt = tax.CalculateSurcharge(GetLineNetAmt(), IsTaxIncluded(), GetPrecision(), out surchargeAmt);

                    if (IsTaxIncluded())
                        SetLineTotalAmt(GetLineNetAmt());
                    else
                        SetLineTotalAmt(Decimal.Add(Decimal.Add(GetLineNetAmt(), TaxAmt), surchargeAmt));
                    base.SetTaxAmt(TaxAmt);
                    SetSurchargeAmt(surchargeAmt);
                }
                else if (GetTaxAmt().Equals(0))
                {
                    TaxAmt = tax.CalculateTax(GetLineNetAmt(), IsTaxIncluded(), GetPrecision());
                    if (IsTaxIncluded())
                        SetLineTotalAmt(GetLineNetAmt());
                    else
                        SetLineTotalAmt(Decimal.Add(GetLineNetAmt(), TaxAmt));
                    base.SetTaxAmt(TaxAmt);
                }
            }
            catch (Exception ex)
            {
                _log.Severe("Error in Calculating Tax (SetTaxAmt) : " + ex.Message);
            }
        }

        /// <summary>
        /// This function is used to set On hand Qty of respective Locator
        /// </summary>
        /// <param name="M_InoutLine_ID"></param>
        /// <param name="M_Product_ID"></param>
        /// <param name="ASI"></param>
        /// <param name="M_ProductContainer_ID"></param>
        public static Decimal GetAvailableStock(int M_InoutLine_ID, int M_Product_ID, int ASI, int M_ProductContainer_ID)
        {
            if (M_InoutLine_ID <= 0)
            {
                return 0;
            }
            Decimal onhandStock = 0;
            String query = @"SELECT " + (M_ProductContainer_ID > 0 ? "SUM(QTY)" : "QtyOnHand") +
                        " FROM " + (M_ProductContainer_ID > 0 ? "M_ContainerStorage" : "M_Storage") + @"
                        WHERE M_Locator_ID = (SELECT M_Locator_ID FROM M_InOutLine WHERE M_InOutLine_ID = " + M_InoutLine_ID + @")
                        AND M_Product_ID=" + M_Product_ID + @" AND NVL(M_AttributeSetInstance_ID , 0) =  " + ASI;
            if (M_ProductContainer_ID > 0)
            {
                query += " AND M_ProductContainer_ID = " + M_ProductContainer_ID;
            }
            onhandStock = Util.GetValueOfDecimal(DB.ExecuteScalar(query.ToString(), null, null));
            return onhandStock;
        }

        /// <summary>
        /// Get Standard Precision
        /// </summary>
        /// <returns>Precision</returns>
        public int GetPrecision()
        {
            if (_precision != null)
            {
                return Convert.ToInt32(_precision);
            }

            String sql = "SELECT c.StdPrecision "
                + "FROM C_Currency c INNER JOIN C_ProvisionalInvoice x ON (x.C_Currency_ID=c.C_Currency_ID) "
                + "WHERE x.C_ProvisionalInvoice_ID=" + GetC_ProvisionalInvoice_ID();
            int i = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql, null, Get_Trx()));
            if (i < 0)
            {
                log.Warning("Precision=" + i + " - Set to 2");
                i = 2;
            }
            _precision = i;
            return (int)_precision;
        }

        /// <summary>
        /// Get Tax Included into price list or not
        /// </summary>
        /// <returns>true, if included</returns>
        public Boolean IsTaxIncluded()
        {
            if (_M_PriceList_ID == 0)
            {
                _M_PriceList_ID = DB.GetSQLValue(Get_Trx(),
                    "SELECT M_PriceList_ID FROM C_ProvisionalInvoice WHERE C_ProvisionalInvoice_ID=@param1",
                    GetC_ProvisionalInvoice_ID());
            }
            MPriceList pl = MPriceList.Get(GetCtx(), _M_PriceList_ID, Get_Trx());
            return pl.IsTaxIncluded();
        }

        /// <summary>
        /// This function is used to return diffreence amount (PO Cost - Provisional Cost) of line
        /// </summary>
        /// <param name="Invoiceline">Invoice Line</param>
        /// <param name="IsPOCostingMethod">Is PO CostingMethod</param>
        /// <returns>Amount</returns>
        public Decimal GetProductLineCost(MProvisionalInvoiceLine Invoiceline, bool IsPOCostingMethod)
        {
            bool isOrderRecordFound = false;
            int multiplier = 1;

            // when not found invoiceline object or OrderLine ID not available
            if ((Invoiceline == null || Invoiceline.Get_ID() <= 0) && (Invoiceline.Get_ID() > 0 && Invoiceline.GetC_OrderLine_ID() == 0))
            {
                return 0;
            }

            // Get Amount from PO
            DataSet ds = null;
            if (IsPOCostingMethod)
            {
                ds = DB.ExecuteDataset(@"SELECT ROUND((TaxableAmt/QtyOrdered) * " + Math.Abs(Invoiceline.GetQtyInvoiced()) + @", 10) AS TaxableAmt  
                                                    , ROUND((TaxAmt/QtyOrdered) * " + Math.Abs(Invoiceline.GetQtyInvoiced()) + @", 10) AS TaxAmt 
                                                    , ROUND((SurchargeAmt/QtyOrdered) * " + Math.Abs(Invoiceline.GetQtyInvoiced()) + @", 10) AS SurchargeAmt 
                                            FROM C_OrderLine 
                                            WHERE C_OrderLIne_ID = " + Invoiceline.GetC_OrderLine_ID(), null, Get_Trx());
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    isOrderRecordFound = true;
                }
            }
            if (Invoiceline.GetReversalDoc_ID() > 0)
            {
                multiplier = -1;
            }

            // Get Taxable amount from invoiceline
            Decimal amt = Invoiceline.GetTaxBaseAmt() - (isOrderRecordFound ? (multiplier * Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["TaxableAmt"])) : 0);

            // create object of tax - for checking tax to be include in cost or not
            MTax tax = MTax.Get(Invoiceline.GetCtx(), Invoiceline.GetC_Tax_ID());
            if (tax.Get_ColumnIndex("IsIncludeInCost") >= 0)
            {
                // add Tax amount in product cost
                if (tax.IsIncludeInCost())
                {
                    amt += (Invoiceline.GetTaxAmt() - (isOrderRecordFound ? (multiplier * Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["TaxAmt"])) : 0));
                }

                // add Surcharge amount in product cost
                if (tax.Get_ColumnIndex("Surcharge_Tax_ID") >= 0 && tax.GetSurcharge_Tax_ID() > 0)
                {
                    if (MTax.Get(Invoiceline.GetCtx(), tax.GetSurcharge_Tax_ID()).IsIncludeInCost())
                    {
                        amt += (Invoiceline.GetSurchargeAmt() - (isOrderRecordFound ? (multiplier * Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["SurchargeAmt"])) : 0));
                    }
                }
            }

            return amt;
        }

    }
}
