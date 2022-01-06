/********************************************************
 * Module Name    :    VA Framework
 * Class Name     :    MProvisionalInvoicetax
 * Purpose        :    Provisional Invoice Tax Model
 * Employee Code  :    209
 * Date           :    14-July-2021
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VAdvanatge.Model
{
    public class MProvisionalInvoiceTax : X_C_ProvisionalInvoiceTax
    {
        //	Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(X_C_ProvisionalInvoiceTax).FullName);
        /** Tax	*/
        private MTax _tax = null;
        /** Cached Precision */
        private int _precision = -1;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="C_ProvisionalInvoiceTax_ID">ProvisionalInvoiceTax</param>
        /// <param name="trxName">Transaction</param>
        /// <writer>209</writer>
        public MProvisionalInvoiceTax(Ctx ctx, int C_ProvisionalInvoiceTax_ID, Trx trxName) :
         base(ctx, C_ProvisionalInvoiceTax_ID, null)
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="dr">DataRow</param>
        /// <param name="trxName">Transaction</param>
        /// <writer>209</writer>
        public MProvisionalInvoiceTax(Ctx ctx, DataRow dr, Trx trxName) :
           base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Get Tax Line for Invoice Line
        /// </summary>
        /// <param name="line">line</param>
        /// <param name="precision">currenct precision</param>
        /// <param name="oldTax">get old tax</param>
        /// <param name="trxName">transaction</param>
        /// <returns>existing or new tax</returns>
        public static MProvisionalInvoiceTax Get(MProvisionalInvoiceLine line, int precision,
            Boolean oldTax, Trx trxName)
        {
            MProvisionalInvoiceTax retValue = null;
            try
            {
                if (line == null || line.GetC_ProvisionalInvoice_ID() == 0)
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

                String sql = "SELECT * FROM C_ProvisionalInvoiceTax WHERE C_ProvisionalInvoice_ID=" + line.GetC_ProvisionalInvoice_ID()
                    + " AND C_Tax_ID=" + C_Tax_ID;
                try
                {
                    DataSet ds = DB.ExecuteDataset(sql, null, trxName);
                    if (ds.Tables.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            retValue = new MProvisionalInvoiceTax(line.GetCtx(), dr, trxName);
                        }
                    }
                }
                catch (Exception e)
                {
                    _log.Log(Level.SEVERE, sql, e);
                }

                // Get IsTaxincluded from selected PriceList on header
                bool isTaxIncluded = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT IsTaxIncluded FROM M_PriceList 
                    WHERE M_PriceList_ID = (SELECT M_PriceList_ID FROM C_ProvisionalInvoice WHERE C_ProvisionalInvoice_ID = "
                    + line.GetC_ProvisionalInvoice_ID() + ")", null, trxName)) == "Y";

                if (retValue != null)
                {
                    retValue.Set_TrxName(trxName);
                    retValue.SetPrecision(precision);
                    retValue.SetIsTaxIncluded(isTaxIncluded);
                    _log.Fine("(old=" + oldTax + ") " + retValue);
                    return retValue;
                }

                //	Create New
                retValue = new MProvisionalInvoiceTax(line.GetCtx(), 0, trxName);
                retValue.Set_TrxName(trxName);
                retValue.SetClientOrg(line);
                retValue.SetC_ProvisionalInvoice_ID(line.GetC_ProvisionalInvoice_ID());
                retValue.SetC_Tax_ID(line.GetC_Tax_ID());
                retValue.SetPrecision(precision);
                retValue.SetIsTaxIncluded(isTaxIncluded);
                _log.Fine("(new) " + retValue);
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, e.Message);
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
        public static MProvisionalInvoiceTax GetSurcharge(MProvisionalInvoiceLine line, int precision, bool oldTax, Trx trxName)
        {
            MProvisionalInvoiceTax retValue = null;
            try
            {
                if (line == null || line.GetC_ProvisionalInvoice_ID() == 0)
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

                String sql = @"SELECT * FROM C_ProvisionalInvoiceTax WHERE C_ProvisionalInvoice_ID=" + line.GetC_ProvisionalInvoice_ID()
                    + " AND C_Tax_ID=" + C_Tax_ID;
                try
                {
                    DataSet ds = DB.ExecuteDataset(sql, null, trxName);
                    if (ds.Tables.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            retValue = new MProvisionalInvoiceTax(line.GetCtx(), dr, trxName);
                        }
                    }
                }
                catch (Exception e)
                {
                    _log.Log(Level.SEVERE, sql, e);
                }

                // Get IsTaxincluded from selected PriceList on header
                bool isTaxIncluded = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT IsTaxIncluded FROM M_PriceList 
                    WHERE M_PriceList_ID = (SELECT M_PriceList_ID FROM C_ProvisionalInvoice WHERE C_ProvisionalInvoice_ID = "
                    + line.GetC_ProvisionalInvoice_ID() + ")", null, trxName)) == "Y";

                if (retValue != null)
                {
                    retValue.Set_TrxName(trxName);
                    retValue.SetPrecision(precision);
                    retValue.SetIsTaxIncluded(isTaxIncluded);
                    _log.Fine("(old=" + oldTax + ") " + retValue);
                    return retValue;
                }

                //	Create New
                retValue = new MProvisionalInvoiceTax(line.GetCtx(), 0, trxName);
                retValue.Set_TrxName(trxName);
                retValue.SetClientOrg(line);
                retValue.SetC_ProvisionalInvoice_ID(line.GetC_ProvisionalInvoice_ID());
                retValue.SetC_Tax_ID(C_Tax_ID);
                retValue.SetPrecision(precision);
                retValue.SetIsTaxIncluded(isTaxIncluded);
                _log.Fine("(new) " + retValue);
            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, ex.Message);
            }
            return retValue;
        }

        /// <summary>
        /// Get Precision
        /// </summary>
        /// <returns>Returns the precision or 2</returns>
        private int GetPrecision()
        {
            if (_precision == -1)
                return 2;
            return _precision;
        }

        /// <summary>
        ///  Set Precision
        /// </summary>
        /// <param name="precision">The precision to set.</param>
        public void SetPrecision(int precision)
        {
            _precision = precision;
        }

        /// <summary>
        /// Get Tax
        /// </summary>
        /// <returns>tax</returns>
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
                   + " il.C_ProvisionalInvoice_ID, il.C_Tax_ID FROM C_ProvisionalInvoiceLine il"
                   + " INNER JOIN C_ProvisionalInvoice i ON (il.C_ProvisionalInvoice_ID=i.C_ProvisionalInvoice_ID) "
                   + "WHERE il.C_ProvisionalInvoice_ID=" + GetC_ProvisionalInvoice_ID() + " AND il.C_Tax_ID=" + GetC_Tax_ID();
                idr = DB.ExecuteDataset(sql, null, Get_Trx());
                if (idr != null && idr.Tables.Count > 0 && idr.Tables[0].Rows.Count > 0)
                {
                    dr = idr.Tables[0].Select(" C_ProvisionalInvoice_ID = " + GetC_ProvisionalInvoice_ID() + " AND C_Tax_ID = " + GetC_Tax_ID());
                }
            }
            else
            {
                dr = idr.Tables[0].Select(" C_ProvisionalInvoice_ID = " + GetC_ProvisionalInvoice_ID() + " AND C_Tax_ID = " + GetC_Tax_ID());
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
                        c_Currency_ID = Util.GetValueOfInt(dr[i]["C_Currency_ID"]);
                        dateAcct = Util.GetValueOfDateTime(dr[i]["DateAcct"]);
                        c_ConversionType_ID = Util.GetValueOfInt(dr[i]["C_ConversionType_ID"]);
                        //	BaseAmt
                        Decimal baseAmt = Util.GetValueOfDecimal(dr[i]["TaxBaseAmt"]);
                        taxBaseAmt = Decimal.Add((Decimal)taxBaseAmt, baseAmt);
                        //	TaxAmt
                        Decimal amt = Util.GetValueOfDecimal(dr[i][1]);
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
                            amt = tax.CalculateTax(baseAmt, false, GetPrecision());
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
                //decimal taxAmtBaseCurrency = GetTaxAmt();
                //int primaryAcctSchemaCurrency = GetCtx().GetContextAsInt("$C_Currency_ID");
                //if (c_Currency_ID != primaryAcctSchemaCurrency)
                //{
                //    taxAmtBaseCurrency = MConversionRate.Convert(GetCtx(), GetTaxAmt(), primaryAcctSchemaCurrency, c_Currency_ID,
                //                                                               dateAcct, c_ConversionType_ID, GetAD_Client_ID(), GetAD_Org_ID());
                //}
                //SetTaxBaseCurrencyAmt(taxAmtBaseCurrency);
            }

            //	Set Base
            SetTaxAbleAmt((Decimal)taxBaseAmt);
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
            MTax surTax = new MTax(GetCtx(), GetC_Tax_ID(), Get_Trx());
            bool documentLevel = surTax.IsDocumentLevel();
            //
            String sql = "SELECT il.TaxBaseAmt, COALESCE(il.TaxAmt,0), i.IsSOTrx  , i.C_Currency_ID , i.DateAcct , i.C_ConversionType_ID, tax.SurchargeType "
                + "FROM C_ProvisionalInvoiceLine il"
                + " INNER JOIN C_ProvisionalInvoice i ON (il.C_ProvisionalInvoice_ID=i.C_ProvisionalInvoice_ID) "
                + " INNER JOIN C_Tax tax ON il.C_Tax_ID=tax.C_Tax_ID "
                + "WHERE il.C_ProvisionalInvoice_ID=" + GetC_ProvisionalInvoice_ID() + " AND tax.Surcharge_Tax_ID=" + GetC_Tax_ID();
            IDataReader idr = null;
            int c_Currency_ID = 0;
            DateTime? dateAcct = null;
            int c_ConversionType_ID = 0;
            try
            {
                idr = DB.ExecuteReader(sql, null, Get_Trx());
                while (idr.Read())
                {
                    //Get References from invoiice header
                    c_Currency_ID = Util.GetValueOfInt(idr[3]);
                    dateAcct = Util.GetValueOfDateTime(idr[4]);
                    c_ConversionType_ID = Util.GetValueOfInt(idr[5]);
                    //	BaseAmt
                    Decimal baseAmt = Util.GetValueOfDecimal(idr[0]);
                    //	TaxAmt
                    Decimal taxAmt = Util.GetValueOfDecimal(idr[1]);
                    string surchargeType = Util.GetValueOfString(idr[6]);

                    // for Surcharge Calculation type - Line Amount + Tax Amount
                    if (surchargeType.Equals(MTax.SURCHARGETYPE_LineAmountPlusTax))
                    {
                        baseAmt = Decimal.Add(baseAmt, taxAmt);
                        taxBaseAmt = Decimal.Add(taxBaseAmt, baseAmt);
                    }
                    // for Surcharge Calculation type - Line Amount 
                    else if (surchargeType.Equals(MTax.SURCHARGETYPE_LineAmount))
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
                //decimal taxAmtBaseCurrency = GetTaxAmt();
                //int primaryAcctSchemaCurrency = GetCtx().GetContextAsInt("$C_Currency_ID");
                //if (c_Currency_ID != primaryAcctSchemaCurrency)
                //{
                //    taxAmtBaseCurrency = MConversionRate.Convert(GetCtx(), GetTaxAmt(), primaryAcctSchemaCurrency, c_Currency_ID,
                //                                                               dateAcct, c_ConversionType_ID, GetAD_Client_ID(), GetAD_Org_ID());
                //}
                //SetTaxBaseCurrencyAmt(taxAmtBaseCurrency);
            }

            //	Set Base            
            SetTaxAbleAmt(taxBaseAmt);
            return true;
        }
    }
}
