/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MInvoiceTax
 * Purpose        : Invoice tex calculation
 * Class Used     : X_C_InvoiceTax
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
using System.Windows.Forms;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MInvoiceTax : X_C_InvoiceTax
    {
        /**
	     * 	Get Tax Line for Invoice Line
	     *	@param line invoice line
	     *	@param precision currency precision
	     *	@param oldTax if true old tax is returned
	     *	@param trxName transaction name
	     *	@return existing or new tax
	     */
        public static MInvoiceTax Get(MInvoiceLine line, int precision,
            Boolean oldTax, Trx trxName)
        {
            MInvoiceTax retValue = null;
            try
            {
                if (line == null || line.GetC_Invoice_ID() == 0 || line.IsDescription())
                    return null;
                int C_Tax_ID = line.GetC_Tax_ID();
                if (oldTax && line.Is_ValueChanged("C_Tax_ID"))
                {
                    Object old = line.Get_ValueOld("C_Tax_ID");
                    if (old == null)
                        return null;
                    C_Tax_ID = int.Parse(old.ToString());
                }
                if (C_Tax_ID == 0)
                {
                    _log.Warning("C_Tax_ID=0");
                    return null;
                }

                String sql = "SELECT * FROM C_InvoiceTax WHERE C_Invoice_ID=" + line.GetC_Invoice_ID() + " AND C_Tax_ID=" + C_Tax_ID;
                try
                {
                    DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                    if (ds.Tables.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            retValue = new MInvoiceTax(line.GetCtx(), dr, trxName);
                        }
                    }
                }
                catch (Exception e)
                {
                    _log.Log(Level.SEVERE, sql, e);
                }

                // Get IsTaxincluded from selected PriceList on header
                bool isTaxIncluded = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsTaxIncluded FROM M_PriceList WHERE M_PriceList_ID = (SELECT M_PriceList_ID FROM C_Invoice WHERE C_Invoice_ID = "
                    + line.GetC_Invoice_ID() + ")", null, trxName)) == "Y";

                if (retValue != null)
                {
                    retValue.Set_TrxName(trxName);
                    retValue.SetPrecision(precision);

                    retValue.SetIsTaxIncluded(isTaxIncluded);

                    _log.Fine("(old=" + oldTax + ") " + retValue);
                    return retValue;
                }

                //	Create New
                retValue = new MInvoiceTax(line.GetCtx(), 0, trxName);
                retValue.Set_TrxName(trxName);
                retValue.SetClientOrg(line);
                retValue.SetC_Invoice_ID(line.GetC_Invoice_ID());
                retValue.SetC_Tax_ID(line.GetC_Tax_ID());
                retValue.SetPrecision(precision);
                // Change here to set tax Inclusive or not based on the pricelist set on Invoice
                retValue.SetIsTaxIncluded(isTaxIncluded);
                //retValue.SetIsTaxIncluded(line.IsTaxIncluded());
                _log.Fine("(new) " + retValue);
            }
            catch
            {
                // MessageBox.Show("MInvoiceTax--Get");
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
        public static MInvoiceTax GetSurcharge(MInvoiceLine line, int precision, bool oldTax, Trx trxName)
        {
            MInvoiceTax retValue = null;
            try
            {
                if (line == null || line.GetC_Invoice_ID() == 0 || line.IsDescription())
                    return null;
                int C_Tax_ID = line.GetC_Tax_ID();
                if (oldTax && line.Is_ValueChanged("C_Tax_ID"))
                {
                    Object old = line.Get_ValueOld("C_Tax_ID");
                    if (old == null)
                        return null;
                    C_Tax_ID = int.Parse(old.ToString());
                }

                // Get Surcharge Tax ID from Tax selected on Line
                C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Surcharge_Tax_ID FROM C_Tax WHERE C_Tax_ID = " + C_Tax_ID, null, trxName));

                if (C_Tax_ID == 0)
                {
                    _log.Warning("C_Tax_ID=0");
                    return null;
                }

                String sql = "SELECT * FROM C_InvoiceTax WHERE C_Invoice_ID=" + line.GetC_Invoice_ID() + " AND C_Tax_ID=" + C_Tax_ID;
                try
                {
                    DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                    if (ds.Tables.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            retValue = new MInvoiceTax(line.GetCtx(), dr, trxName);
                        }
                    }
                }
                catch (Exception e)
                {
                    _log.Log(Level.SEVERE, sql, e);
                }

                // Get IsTaxincluded from selected PriceList on header
                bool isTaxIncluded = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsTaxIncluded FROM M_PriceList WHERE M_PriceList_ID = (SELECT M_PriceList_ID FROM C_Invoice WHERE C_Invoice_ID = "
                    + line.GetC_Invoice_ID() + ")", null, trxName)) == "Y";

                if (retValue != null)
                {
                    retValue.Set_TrxName(trxName);
                    retValue.SetPrecision(precision);
                    retValue.SetIsTaxIncluded(isTaxIncluded);
                    _log.Fine("(old=" + oldTax + ") " + retValue);
                    return retValue;
                }

                //	Create New
                retValue = new MInvoiceTax(line.GetCtx(), 0, trxName);
                retValue.Set_TrxName(trxName);
                retValue.SetClientOrg(line);
                retValue.SetC_Invoice_ID(line.GetC_Invoice_ID());
                retValue.SetC_Tax_ID(C_Tax_ID);
                retValue.SetPrecision(precision);
                retValue.SetIsTaxIncluded(isTaxIncluded);
                _log.Fine("(new) " + retValue);
            }
            catch
            {
                // MessageBox.Show("MInvoiceTax--Get");
            }
            return retValue;
        }

        //	Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MInvoiceTax).FullName);


        /**************************************************************************
         * 	Persistency Constructor
         *	@param ctx context
         *	@param ignored ignored
         *	@param trxName transaction
         */
        public MInvoiceTax(Ctx ctx, int ignored, Trx trxName)
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
        public MInvoiceTax(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /** Tax							*/
        private MTax _tax = null;
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
        public MTax GetTax()
        {
            if (_tax == null)
                _tax = MTax.Get(GetCtx(), GetC_Tax_ID());
            return _tax;
        }


        /// <summary>
        /// Calculate/Set Tax Base Amt from Invoice Lines
        /// </summary>
        /// <returns>true if tax calculated</returns>
        public bool CalculateTaxFromLines()
        {
            return CalculateTaxFromLines(null);
        }

        /// <summary>
        /// Calculate/Set Tax Base Amt from Invoice Lines
        /// </summary>
        /// <param name="idr">dataset</param>
        /// <returns>true if tax calculated</returns>
        public bool CalculateTaxFromLines(DataSet idr)
        {
            String sql = string.Empty;
            DataRow[] dr = null;
            Decimal? taxBaseAmt = Env.ZERO;
            Decimal taxAmt = Env.ZERO;
            //
            bool documentLevel = GetTax().IsDocumentLevel();
            MTax tax = GetTax();
            // Calculate Tax on TaxAble Amount
            if (idr == null)
            {
                sql = "SELECT il.TaxBaseAmt, COALESCE(il.TaxAmt,0), i.IsSOTrx  , i.C_Currency_ID , i.DateAcct , i.C_ConversionType_ID, "
                   + " il.C_Invoice_ID, il.C_Tax_ID, il.LineNetAmt FROM C_InvoiceLine il"
                   + " INNER JOIN C_Invoice i ON (il.C_Invoice_ID=i.C_Invoice_ID) "
                   + "WHERE il.C_Invoice_ID=" + GetC_Invoice_ID() + " AND il.C_Tax_ID=" + GetC_Tax_ID();
                idr = DB.ExecuteDataset(sql, null, Get_TrxName());
                if (idr != null && idr.Tables.Count > 0 && idr.Tables[0].Rows.Count > 0)
                {
                    dr = idr.Tables[0].Select(" C_Invoice_ID = " + GetC_Invoice_ID() + " AND C_Tax_ID = " + GetC_Tax_ID());
                }
            }
            else
            {
                dr = idr.Tables[0].Select(" C_Invoice_ID = " + GetC_Invoice_ID() + " AND C_Tax_ID = " + GetC_Tax_ID());
            }
            int c_Currency_ID = 0;
            DateTime? dateAcct = null;
            int c_ConversionType_ID = 0;
            try
            {
                if (dr != null && dr.Length > 0)
                {
                    for (int i = 0; i < dr.Length; i++)
                    {
                        //Get References from invoiice header
                        c_Currency_ID = Utility.Util.GetValueOfInt(dr[i]["C_Currency_ID"]);
                        dateAcct = Utility.Util.GetValueOfDateTime(dr[i]["DateAcct"]);
                        c_ConversionType_ID = Utility.Util.GetValueOfInt(dr[i]["C_ConversionType_ID"]);
                        //	BaseAmt
                        Decimal baseAmt = Utility.Util.GetValueOfDecimal(dr[i]["LineNetAmt"]);
                        Decimal TaxableAmt = baseAmt;
                        if (IsTaxIncluded())
                        {
                            Decimal surRate = 0;
                            Decimal multiplier = 0;
                            if (tax.GetSurcharge_Tax_ID() > 0)
                            {
                                MTax surTax = MTax.Get(GetCtx(), tax.GetSurcharge_Tax_ID());
                                surRate = surTax.GetRate();
                                if (tax.GetSurchargeType() == MTax.SURCHARGETYPE_LineAmountPlusTax)
                                {
                                    multiplier = Decimal.Round(Decimal.Divide(surRate, 100), 12, MidpointRounding.AwayFromZero);
                                    multiplier = Decimal.Add(multiplier, Env.ONE);
                                    baseAmt = Decimal.Divide(baseAmt, multiplier);
                                    baseAmt = Decimal.Round(baseAmt, 12, MidpointRounding.AwayFromZero);

                                    multiplier = Decimal.Round(Decimal.Divide(tax.GetRate(), 100), 12, MidpointRounding.AwayFromZero);
                                    multiplier = Decimal.Add(multiplier, Env.ONE);
                                    baseAmt = Decimal.Divide(baseAmt, multiplier);
                                    baseAmt = Decimal.Round(baseAmt, 12, MidpointRounding.AwayFromZero);
                                    TaxableAmt = baseAmt;
                                }
                                else if (tax.GetSurchargeType() == MTax.SURCHARGETYPE_LineAmount)
                                {
                                    multiplier = Decimal.Round(Decimal.Divide(Decimal.Add(tax.GetRate(), surRate), 100),
                                                           12, MidpointRounding.AwayFromZero);
                                    multiplier = Decimal.Add(multiplier, Env.ONE);
                                    baseAmt = Decimal.Divide(baseAmt, multiplier);
                                    baseAmt = Decimal.Round(baseAmt, 12, MidpointRounding.AwayFromZero);
                                    TaxableAmt = baseAmt;
                                }
                                else if (tax.GetSurchargeType() == MTax.SURCHARGETYPE_TaxAmount)
                                {
                                    multiplier = Decimal.Round(Decimal.Divide(surRate, 100), 12, MidpointRounding.AwayFromZero);
                                    multiplier = Decimal.Multiply(tax.GetRate(), multiplier);
                                    multiplier = Decimal.Add(tax.GetRate(), multiplier);
                                    multiplier = Decimal.Round(Decimal.Divide(multiplier, 100), 12, MidpointRounding.AwayFromZero);
                                    multiplier = Decimal.Add(multiplier, Env.ONE);

                                    baseAmt = Decimal.Divide(baseAmt, multiplier);
                                    baseAmt = Decimal.Round(baseAmt, 12, MidpointRounding.AwayFromZero);
                                    TaxableAmt = baseAmt;
                                }
                            }
                            else
                            {
                                multiplier = Decimal.Round(Decimal.Divide(tax.GetRate(), 100), 12, MidpointRounding.AwayFromZero);
                                multiplier = Decimal.Add(multiplier, Env.ONE);
                                baseAmt = Decimal.Divide(baseAmt, multiplier);
                            }
                        }
                        taxBaseAmt = Decimal.Add((Decimal)taxBaseAmt, baseAmt);

                        //	TaxAmt
                        Decimal amt = Utility.Util.GetValueOfDecimal(dr[i][1]);

                        bool isSOTrx = "Y".Equals(dr[i][2].ToString());
                        //
                        if (documentLevel || Env.Signum(baseAmt) == 0)
                        {
                            amt = Env.ZERO;
                        }
                        else if (Env.Signum(amt) != 0 && !isSOTrx)  //	manually entered
                        {
                            ;
                        }
                        else    // calculate line tax
                        {
                            amt = tax.CalculateTax(TaxableAmt,
                                (tax.GetSurcharge_Tax_ID() > 0) ? false :
                                IsTaxIncluded(), GetPrecision());
                        }
                        //
                        taxAmt = Decimal.Add(taxAmt, amt);
                    }
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Dispose();
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
                int primaryAcctSchemaCurrency = GetCtx().GetContextAsInt("$C_Currency_ID");
                if (c_Currency_ID != primaryAcctSchemaCurrency)
                {
                    taxAmtBaseCurrency = MConversionRate.Convert(GetCtx(), GetTaxAmt(), primaryAcctSchemaCurrency, c_Currency_ID,
                                                                               dateAcct, c_ConversionType_ID, GetAD_Client_ID(), GetAD_Org_ID());
                }
                SetTaxBaseCurrencyAmt(taxAmtBaseCurrency);
            }

            //	Set Base
            //if (IsTaxIncluded())
            //    SetTaxBaseAmt(Decimal.Subtract((Decimal)taxBaseAmt, taxAmt));
            //else
            SetTaxBaseAmt(Decimal.Round((Decimal)taxBaseAmt, GetPrecision()));
            return true;
        }

        /// <summary>
        /// Calculate/Set Surcharge Tax Amt from Invoice Lines
        /// </summary>        
        /// <returns>true if calculated</returns>
        public bool CalculateSurchargeFromLines()
        {
            Decimal multiplier = 0;
            Decimal taxBaseAmt = Env.ZERO;
            Decimal surTaxAmt = Env.ZERO;
            //
            MTax surTax = new MTax(GetCtx(), GetC_Tax_ID(), Get_TrxName());
            bool documentLevel = surTax.IsDocumentLevel();
            //
            String sql = @"SELECT il.TaxBaseAmt, COALESCE(il.TaxAmt,0), i.IsSOTrx  , i.C_Currency_ID ,
                            i.DateAcct , i.C_ConversionType_ID, tax.SurchargeType, il.LineNetAmt, tax.Rate "
                + "FROM C_InvoiceLine il"
                + " INNER JOIN C_Invoice i ON (il.C_Invoice_ID=i.C_Invoice_ID) "
                + " INNER JOIN C_Tax tax ON il.C_Tax_ID=tax.C_Tax_ID "
                + "WHERE il.C_Invoice_ID=" + GetC_Invoice_ID() + " AND tax.Surcharge_Tax_ID=" + GetC_Tax_ID();
            IDataReader idr = null;
            int c_Currency_ID = 0;
            DateTime? dateAcct = null;
            int c_ConversionType_ID = 0;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                while (idr.Read())
                {
                    //Get References from invoiice header
                    c_Currency_ID = Utility.Util.GetValueOfInt(idr[3]);
                    dateAcct = Utility.Util.GetValueOfDateTime(idr[4]);
                    c_ConversionType_ID = Utility.Util.GetValueOfInt(idr[5]);
                    //	BaseAmt
                    Decimal baseAmt = Utility.Util.GetValueOfDecimal(idr["LineNetAmt"]);
                    Decimal orignalTaxRate = Util.GetValueOfDecimal(idr["Rate"]);
                    //	TaxAmt
                    Decimal taxAmt = Utility.Util.GetValueOfDecimal(idr[1]);
                    string surchargeType = Util.GetValueOfString(idr[6]);

                    // for Surcharge Calculation type - Line Amount + Tax Amount
                    if (surchargeType.Equals(MTax.SURCHARGETYPE_LineAmountPlusTax))
                    {
                        if (IsTaxIncluded())
                        {
                            // calculate base amount based on Surcharge Rate
                            multiplier = Decimal.Round(Decimal.Divide(surTax.GetRate(), 100), 12, MidpointRounding.AwayFromZero);
                            multiplier = Decimal.Add(multiplier, Env.ONE);
                            baseAmt = Decimal.Divide(baseAmt, multiplier);
                            baseAmt = Decimal.Round(baseAmt, 12, MidpointRounding.AwayFromZero);

                            // calculate base amount on above calculate baseamt based on original tax rate
                            multiplier = Decimal.Round(Decimal.Divide(orignalTaxRate, 100), 12, MidpointRounding.AwayFromZero);
                            multiplier = Decimal.Add(multiplier, Env.ONE);
                            baseAmt = Decimal.Divide(baseAmt, multiplier);
                            baseAmt = Decimal.Round(baseAmt, 12, MidpointRounding.AwayFromZero);
                        }
                        baseAmt = Decimal.Add(baseAmt, taxAmt);
                        taxBaseAmt = Decimal.Add(taxBaseAmt, baseAmt);
                    }
                    // for Surcharge Calculation type - Line Amount 
                    else if (surchargeType.Equals(MTax.SURCHARGETYPE_LineAmount))
                    {
                        if (IsTaxIncluded())
                        {
                            multiplier = Decimal.Round(Decimal.Divide(Decimal.Add(surTax.GetRate(), orignalTaxRate), 100), 12, MidpointRounding.AwayFromZero);
                            multiplier = Decimal.Add(multiplier, Env.ONE);

                            baseAmt = Decimal.Divide(baseAmt, multiplier);
                            baseAmt = Decimal.Round(baseAmt, 12, MidpointRounding.AwayFromZero);
                        }
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
                int primaryAcctSchemaCurrency = GetCtx().GetContextAsInt("$C_Currency_ID");
                if (c_Currency_ID != primaryAcctSchemaCurrency)
                {
                    taxAmtBaseCurrency = MConversionRate.Convert(GetCtx(), GetTaxAmt(), primaryAcctSchemaCurrency, c_Currency_ID,
                                                                               dateAcct, c_ConversionType_ID, GetAD_Client_ID(), GetAD_Org_ID());
                }
                SetTaxBaseCurrencyAmt(taxAmtBaseCurrency);
            }

            //	Set Base
            //if (IsTaxIncluded())
            //    SetTaxBaseAmt(Decimal.Subtract((Decimal)taxBaseAmt, surTaxAmt));
            //else
            SetTaxBaseAmt(Decimal.Round(taxBaseAmt, GetPrecision()));
            return true;
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MInvoiceTax[");
            sb.Append("C_Invoice_ID=").Append(GetC_Invoice_ID())
                .Append(",C_Tax_ID=").Append(GetC_Tax_ID())
                .Append(", Base=").Append(GetTaxBaseAmt()).Append(",Tax=").Append(GetTaxAmt())
                .Append("]");
            return sb.ToString();
        }
    }
}