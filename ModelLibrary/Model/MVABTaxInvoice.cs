/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVABTaxInvoice
 * Purpose        : Invoice tex calculation
 * Class Used     : X_VAB_Tax_Invoice
 * Chronological    Development
 * Raghunandan     22-Jun-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
//////using System.Windows.Forms;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVABTaxInvoice : X_VAB_Tax_Invoice
    {
        /**
	     * 	Get Tax Line for Invoice Line
	     *	@param line invoice line
	     *	@param precision currency precision
	     *	@param oldTax if true old tax is returned
	     *	@param trxName transaction name
	     *	@return existing or new tax
	     */
        public static MVABTaxInvoice Get(MVABInvoiceLine line, int precision,
            Boolean oldTax, Trx trxName)
        {
            MVABTaxInvoice retValue = null;
            try
            {
                if (line == null || line.GetVAB_Invoice_ID() == 0 || line.IsDescription())
                    return null;
                int VAB_TaxRate_ID = line.GetVAB_TaxRate_ID();
                if (oldTax && line.Is_ValueChanged("VAB_TaxRate_ID"))
                {
                    Object old = line.Get_ValueOld("VAB_TaxRate_ID");
                    if (old == null)
                        return null;
                    VAB_TaxRate_ID = int.Parse(old.ToString());
                }
                if (VAB_TaxRate_ID == 0)
                {
                    _log.Warning("VAB_TaxRate_ID=0");
                    return null;
                }

                String sql = "SELECT * FROM VAB_Tax_Invoice WHERE VAB_Invoice_ID=" + line.GetVAB_Invoice_ID() + " AND VAB_TaxRate_ID=" + VAB_TaxRate_ID;
                try
                {
                    DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                    if (ds.Tables.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            retValue = new MVABTaxInvoice(line.GetCtx(), dr, trxName);
                        }
                    }
                }
                catch (Exception e)
                {
                    _log.Log(Level.SEVERE, sql, e);
                }

                // Get IsTaxincluded from selected PriceList on header
                bool isTaxIncluded = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsTaxIncluded FROM VAM_PriceList WHERE VAM_PriceList_ID = (SELECT VAM_PriceList_ID FROM VAB_Invoice WHERE VAB_Invoice_ID = " 
                    + line.GetVAB_Invoice_ID() + ")", null, trxName)) == "Y";

                if (retValue != null)
                {
                    retValue.Set_TrxName(trxName);
                    retValue.SetPrecision(precision);

                    retValue.SetIsTaxIncluded(isTaxIncluded);

                    _log.Fine("(old=" + oldTax + ") " + retValue);
                    return retValue;
                }

                //	Create New
                retValue = new MVABTaxInvoice(line.GetCtx(), 0, trxName);
                retValue.Set_TrxName(trxName);
                retValue.SetClientOrg(line);
                retValue.SetVAB_Invoice_ID(line.GetVAB_Invoice_ID());
                retValue.SetVAB_TaxRate_ID(line.GetVAB_TaxRate_ID());
                retValue.SetPrecision(precision);
                // Change here to set tax Inclusive or not based on the pricelist set on Invoice
                retValue.SetIsTaxIncluded(isTaxIncluded);
                //retValue.SetIsTaxIncluded(line.IsTaxIncluded());
                _log.Fine("(new) " + retValue);
            }
            catch
            {
                // MessageBox.Show("MVABTaxInvoice--Get");
            }
            return retValue;
        }

        /// <summary>
        /// Get Surcharge Tax Line for Invoice Line
        /// </summary>
        /// <param name="line">line</param>
        /// <param name="precision">currenct precision</param>
        /// <param name="oldTax">get old tax</param>
        /// <param name="trxName">transaction</param>
        /// <returns>existing or new tax</returns>
        public static MVABTaxInvoice GetSurcharge(MVABInvoiceLine line, int precision, bool oldTax, Trx trxName)
        {
            MVABTaxInvoice retValue = null;
            try
            {
                if (line == null || line.GetVAB_Invoice_ID() == 0 || line.IsDescription())
                    return null;
                int VAB_TaxRate_ID = line.GetVAB_TaxRate_ID();
                if (oldTax && line.Is_ValueChanged("VAB_TaxRate_ID"))
                {
                    Object old = line.Get_ValueOld("VAB_TaxRate_ID");
                    if (old == null)
                        return null;
                    VAB_TaxRate_ID = int.Parse(old.ToString());
                }

                // Get Surcharge Tax ID from Tax selected on Line
                VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Surcharge_Tax_ID FROM VAB_TaxRate WHERE VAB_TaxRate_ID = " + VAB_TaxRate_ID, null, trxName));

                if (VAB_TaxRate_ID == 0)
                {
                    _log.Warning("VAB_TaxRate_ID=0");
                    return null;
                }

                String sql = "SELECT * FROM VAB_Tax_Invoice WHERE VAB_Invoice_ID=" + line.GetVAB_Invoice_ID() + " AND VAB_TaxRate_ID=" + VAB_TaxRate_ID;
                try
                {
                    DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                    if (ds.Tables.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            retValue = new MVABTaxInvoice(line.GetCtx(), dr, trxName);
                        }
                    }
                }
                catch (Exception e)
                {
                    _log.Log(Level.SEVERE, sql, e);
                }

                // Get IsTaxincluded from selected PriceList on header
                bool isTaxIncluded = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsTaxIncluded FROM VAM_PriceList WHERE VAM_PriceList_ID = (SELECT VAM_PriceList_ID FROM VAB_Invoice WHERE VAB_Invoice_ID = " 
                    + line.GetVAB_Invoice_ID() + ")", null, trxName)) == "Y";

                if (retValue != null)
                {
                    retValue.Set_TrxName(trxName);
                    retValue.SetPrecision(precision);
                    retValue.SetIsTaxIncluded(isTaxIncluded);
                    _log.Fine("(old=" + oldTax + ") " + retValue);
                    return retValue;
                }

                //	Create New
                retValue = new MVABTaxInvoice(line.GetCtx(), 0, trxName);
                retValue.Set_TrxName(trxName);
                retValue.SetClientOrg(line);
                retValue.SetVAB_Invoice_ID(line.GetVAB_Invoice_ID());
                retValue.SetVAB_TaxRate_ID(VAB_TaxRate_ID);
                retValue.SetPrecision(precision);
                retValue.SetIsTaxIncluded(isTaxIncluded);
                _log.Fine("(new) " + retValue);
            }
            catch
            {
                // MessageBox.Show("MVABTaxInvoice--Get");
            }
            return retValue;
        }

        //	Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MVABTaxInvoice).FullName);


        /**************************************************************************
         * 	Persistency Constructor
         *	@param ctx context
         *	@param ignored ignored
         *	@param trxName transaction
         */
        public MVABTaxInvoice(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, 0, trxName)
        {
            if (ignored != 0)
                throw new ArgumentException("Multi-Key");
            SetTaxAmt(Env.ZERO);
            SetTaxBaseAmt(Env.ZERO);
            SetIsTaxIncluded(false);
        }

        /**
         * 	Load Constructor.
         * 	Set Precision and TaxIncluded for tax calculations!
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MVABTaxInvoice(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /** Tax							*/
        private MVABTaxRate _tax = null;
        /** Cached Precision			*/
        private int _precision = -1;


        /**
         * 	Get Precision
         * 	@return Returns the precision or 2
         */
        private int GetPrecision()
        {
            if (_precision == -1)
                return 2;
            return _precision;
        }

        /**
         * 	Set Precision
         *	@param precision The precision to set.
         */
        public void SetPrecision(int precision)
        {
            _precision = precision;
        }

        /**
         * 	Get Tax
         *	@return tax
         */
        public MVABTaxRate GetTax()
        {
            if (_tax == null)
                _tax = MVABTaxRate.Get(GetCtx(), GetVAB_TaxRate_ID());
            return _tax;
        }


        /**************************************************************************
         * 	Calculate/Set Tax Base Amt from Invoice Lines
         * 	@return true if tax calculated
         */
        public bool CalculateTaxFromLines()
        {
            Decimal? taxBaseAmt = Env.ZERO;
            Decimal taxAmt = Env.ZERO;
            //
            bool documentLevel = GetTax().IsDocumentLevel();
            MVABTaxRate tax = GetTax();
            // Calculate Tax on TaxAble Amount
            String sql = "SELECT il.TaxBaseAmt, COALESCE(il.TaxAmt,0), i.IsSOTrx  , i.VAB_Currency_ID , i.DateAcct , i.VAB_CurrencyType_ID "
                + "FROM VAB_InvoiceLine il"
                + " INNER JOIN VAB_Invoice i ON (il.VAB_Invoice_ID=i.VAB_Invoice_ID) "
                + "WHERE il.VAB_Invoice_ID=" + GetVAB_Invoice_ID() + " AND il.VAB_TaxRate_ID=" + GetVAB_TaxRate_ID();
            IDataReader idr = null;
            int VAB_Currency_ID = 0;
            DateTime? dateAcct = null;
            int VAB_CurrencyType_ID = 0;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                while (idr.Read())
                {
                    //Get References from invoiice header
                    VAB_Currency_ID = Utility.Util.GetValueOfInt(idr[3]);
                    dateAcct = Utility.Util.GetValueOfDateTime(idr[4]);
                    VAB_CurrencyType_ID = Utility.Util.GetValueOfInt(idr[5]);
                    //	BaseAmt
                    Decimal baseAmt = Utility.Util.GetValueOfDecimal(idr[0]);
                    taxBaseAmt = Decimal.Add((Decimal)taxBaseAmt, baseAmt);
                    //	TaxAmt
                    Decimal amt = Utility.Util.GetValueOfDecimal(idr[1]);
                    //if (amt == null)
                    //    amt = Env.ZERO;
                    bool isSOTrx = "Y".Equals(idr[2].ToString());
                    //
                    if (documentLevel || Env.Signum(baseAmt) == 0)
                    {
                        amt = Env.ZERO;
                    }
                    else if (Env.Signum(amt) != 0 && !isSOTrx)	//	manually entered
                    {
                        ;
                    }
                    else	// calculate line tax
                    {
                        amt = tax.CalculateTax(baseAmt, false, GetPrecision());
                    }
                    //
                    taxAmt = Decimal.Add(taxAmt, amt);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "setTaxBaseAmt", e);
                log.Log(Level.SEVERE, sql, e);
                taxBaseAmt = null;
            }
            if (taxBaseAmt == null)
                return false;

            //	Calculate Tax
            if (documentLevel || Env.Signum(taxAmt) == 0)
                taxAmt = tax.CalculateTax((Decimal)taxBaseAmt, false, GetPrecision());
            SetTaxAmt(taxAmt);

            // set Tax Amount in base currency 
            if (Get_ColumnIndex("TaxBaseCurrencyAmt") >= 0)
            {
                decimal taxAmtBaseCurrency = GetTaxAmt();
                int primaryAcctSchemaCurrency = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT VAB_Currency_ID FROM VAB_AccountBook WHERE VAB_AccountBook_ID = 
                                            (SELECT VAB_AccountBook1_id FROM VAF_ClientDetail WHERE vaf_client_id = " + GetVAF_Client_ID() + ")", null, Get_Trx()));
                if (VAB_Currency_ID != primaryAcctSchemaCurrency)
                {
                    taxAmtBaseCurrency = MVABExchangeRate.Convert(GetCtx(), GetTaxAmt(), primaryAcctSchemaCurrency, VAB_Currency_ID,
                                                                               dateAcct, VAB_CurrencyType_ID, GetVAF_Client_ID(), GetVAF_Org_ID());
                }
                SetTaxBaseCurrencyAmt(taxAmtBaseCurrency);
            }

            //	Set Base
            //if (IsTaxIncluded())
            //    SetTaxBaseAmt(Decimal.Subtract((Decimal)taxBaseAmt, taxAmt));
            //else
            SetTaxBaseAmt((Decimal)taxBaseAmt);
            return true;
        }

        /// <summary>
        /// Calculate/Set Surcharge Tax Amt from Invoice Lines
        /// </summary>        
        /// <returns>true if calculated</returns>
        public bool CalculateSurchargeFromLines()
        {
            Decimal taxBaseAmt = Env.ZERO;
            Decimal surTaxAmt = Env.ZERO;
            //
            MVABTaxRate surTax = new MVABTaxRate(GetCtx(), GetVAB_TaxRate_ID(), Get_TrxName());
            bool documentLevel = surTax.IsDocumentLevel();
            //
            String sql = "SELECT il.TaxBaseAmt, COALESCE(il.TaxAmt,0), i.IsSOTrx  , i.VAB_Currency_ID , i.DateAcct , i.VAB_CurrencyType_ID, tax.SurchargeType "
                + "FROM VAB_InvoiceLine il"
                + " INNER JOIN VAB_Invoice i ON (il.VAB_Invoice_ID=i.VAB_Invoice_ID) "
                + " INNER JOIN VAB_TaxRate tax ON il.VAB_TaxRate_ID=tax.VAB_TaxRate_ID "
                + "WHERE il.VAB_Invoice_ID=" + GetVAB_Invoice_ID() + " AND tax.Surcharge_Tax_ID=" + GetVAB_TaxRate_ID();
            IDataReader idr = null;
            int VAB_Currency_ID = 0;
            DateTime? dateAcct = null;
            int VAB_CurrencyType_ID = 0;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                while (idr.Read())
                {
                    //Get References from invoiice header
                    VAB_Currency_ID = Utility.Util.GetValueOfInt(idr[3]);
                    dateAcct = Utility.Util.GetValueOfDateTime(idr[4]);
                    VAB_CurrencyType_ID = Utility.Util.GetValueOfInt(idr[5]);
                    //	BaseAmt
                    Decimal baseAmt = Utility.Util.GetValueOfDecimal(idr[0]);
                    //	TaxAmt
                    Decimal taxAmt = Utility.Util.GetValueOfDecimal(idr[1]);
                    string surchargeType = Util.GetValueOfString(idr[6]);

                    // for Surcharge Calculation type - Line Amount + Tax Amount
                    if (surchargeType.Equals(MVABTaxRate.SURCHARGETYPE_LineAmountPlusTax))
                    {
                        baseAmt = Decimal.Add(baseAmt, taxAmt);
                        taxBaseAmt = Decimal.Add(taxBaseAmt, baseAmt);
                    }
                    // for Surcharge Calculation type - Line Amount 
                    else if (surchargeType.Equals(MVABTaxRate.SURCHARGETYPE_LineAmount))
                    {
                        taxBaseAmt = Decimal.Add(taxBaseAmt, baseAmt);
                    }
                    // for Surcharge Calculation type - Tax Amount
                    else
                    {
                        baseAmt = taxAmt;
                        taxBaseAmt = Decimal.Add(taxBaseAmt, baseAmt);
                    }

                    taxAmt = Env.ZERO;

                    bool isSOTrx = "Y".Equals(idr[2].ToString());
                    //
                    if (documentLevel || Env.Signum(baseAmt) == 0)
                    {

                    }
                    else if (Env.Signum(taxAmt) != 0 && !isSOTrx)	//	manually entered
                    {
                        ;
                    }
                    else	// calculate line tax
                    {
                        taxAmt = surTax.CalculateTax(baseAmt, false, GetPrecision());
                    }
                    //
                    surTaxAmt = Decimal.Add(surTaxAmt, taxAmt);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "setTaxBaseAmt", e);
                taxBaseAmt = Util.GetValueOfDecimal(null);
            }

            //	Calculate Tax
            if (documentLevel || Env.Signum(surTaxAmt) == 0)
                surTaxAmt = surTax.CalculateTax(taxBaseAmt, false, GetPrecision());
            SetTaxAmt(surTaxAmt);

            // set Tax Amount in base currency 
            if (Get_ColumnIndex("TaxBaseCurrencyAmt") >= 0)
            {
                decimal taxAmtBaseCurrency = GetTaxAmt();
                int primaryAcctSchemaCurrency = GetCtx().GetContextAsInt("$VAB_Currency_ID");
                if (VAB_Currency_ID != primaryAcctSchemaCurrency)
                {
                    taxAmtBaseCurrency = MVABExchangeRate.Convert(GetCtx(), GetTaxAmt(), primaryAcctSchemaCurrency, VAB_Currency_ID,
                                                                               dateAcct, VAB_CurrencyType_ID, GetVAF_Client_ID(), GetVAF_Org_ID());
                }
                SetTaxBaseCurrencyAmt(taxAmtBaseCurrency);
            }

            //	Set Base            
            SetTaxBaseAmt(taxBaseAmt);
            return true;
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVABTaxInvoice[");
            sb.Append("VAB_Invoice_ID=").Append(GetVAB_Invoice_ID())
                .Append(",VAB_TaxRate_ID=").Append(GetVAB_TaxRate_ID())
                .Append(", Base=").Append(GetTaxBaseAmt()).Append(",Tax=").Append(GetTaxAmt())
                .Append("]");
            return sb.ToString();
        }
    }
}