/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : MOrderTax
 * Chronological Development
 * Raghunandan sharma     10-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Common;
using VAdvantage.Utility;
using System.Data;
using System.Windows.Forms;
using VAdvantage.SqlExec;
using VAdvantage.DataBase;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MOrderTax : X_C_OrderTax
    {
        /**	Static Logger	*/
        private static VLogger s_log = VLogger.GetVLogger(typeof(MOrderTax).FullName);

        /** Tax							*/
        private MTax _tax = null;
        /** Cached Precision			*/
        private int? _precision = null;

        /// <summary>
        /// Get Tax Line for Order Line
        /// </summary>
        /// <param name="line">line</param>
        /// <param name="precision">currenct precision</param>
        /// <param name="oldTax">get old tax</param>
        /// <param name="trxName">transaction</param>
        /// <returns>existing or new tax</returns>
        public static MOrderTax Get(MOrderLine line, int precision, bool oldTax, Trx trxName)
        {
            MOrderTax retValue = null;
            if (line == null || line.GetC_Order_ID() == 0)
            {
                //s_log.fine("No Order");
                return null;
            }
            int C_Tax_ID = line.GetC_Tax_ID();
            if (oldTax && line.Is_ValueChanged("C_Tax_ID"))
            {
                Object old = line.Get_ValueOld("C_Tax_ID");
                if (old == null)
                {
                    //s_log.fine("No Old Tax");
                    return null;
                }
                C_Tax_ID = int.Parse(old.ToString());
            }
            if (C_Tax_ID == 0)
            {
                s_log.Fine("No Tax");
                return null;
            }

            String sql = "SELECT * FROM C_OrderTax WHERE C_Order_ID=" + line.GetC_Order_ID() + " AND C_Tax_ID=" + C_Tax_ID;
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue = new MOrderTax(line.GetCtx(), dr, trxName);
                }
                ds = null;
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);
            }

            // JID_1303: On Order tax calculate tax according to selected pricelist. If user delete lines and change pricelist, it should check IsTaxIncluded on selected Pricelist.
            bool isTaxIncluded = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsTaxIncluded FROM M_PriceList WHERE M_PriceList_ID = (SELECT M_PriceList_ID FROM C_Order WHERE C_Order_ID = "
                + line.GetC_Order_ID() + ")", null, trxName)) == "Y";

            if (retValue != null)
            {
                retValue.SetPrecision(precision);
                retValue.Set_TrxName(trxName);

                retValue.SetIsTaxIncluded(isTaxIncluded);

                s_log.Fine("(old=" + oldTax + ") " + retValue);
                return retValue;
            }

            //	Create New
            retValue = new MOrderTax(line.GetCtx(), 0, trxName);
            retValue.Set_TrxName(trxName);
            retValue.SetClientOrg(line);
            retValue.SetC_Order_ID(line.GetC_Order_ID());
            retValue.SetC_Tax_ID(line.GetC_Tax_ID());
            retValue.SetPrecision(precision);
            retValue.SetIsTaxIncluded(line.IsTaxIncluded());
            s_log.Fine("(new) " + retValue);
            return retValue;
        }

        /// <summary>
        /// Get Surcharge Tax Line for Order Line
        /// </summary>
        /// <param name="line">line</param>
        /// <param name="precision">currenct precision</param>
        /// <param name="oldTax">get old tax</param>
        /// <param name="trxName">transaction</param>
        /// <returns>existing or new tax</returns>
        public static MOrderTax GetSurcharge(MOrderLine line, int precision, bool oldTax, Trx trxName)
        {
            MOrderTax retValue = null;
            if (line == null || line.GetC_Order_ID() == 0)
            {
                //s_log.fine("No Order");
                return null;
            }
            int C_Tax_ID = line.GetC_Tax_ID();
            if (oldTax && line.Is_ValueChanged("C_Tax_ID"))
            {
                Object old = line.Get_ValueOld("C_Tax_ID");
                if (old == null)
                {
                    //s_log.fine("No Old Tax");
                    return null;
                }
                C_Tax_ID = int.Parse(old.ToString());
            }

            // Get Surcharge Tax ID from Tax selected on Line
            C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Surcharge_Tax_ID FROM C_Tax WHERE C_Tax_ID = " + C_Tax_ID, null, trxName));

            if (C_Tax_ID == 0)
            {
                s_log.Fine("No Tax");
                return null;
            }

            String sql = "SELECT * FROM C_OrderTax WHERE C_Order_ID=" + line.GetC_Order_ID() + " AND C_Tax_ID=" + C_Tax_ID;
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        retValue = new MOrderTax(line.GetCtx(), dr, trxName);
                    }
                    ds = null;
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);
            }

            // Get IsTaxincluded from selected PriceList on header
            bool isTaxIncluded = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsTaxIncluded FROM M_PriceList WHERE M_PriceList_ID = (SELECT M_PriceList_ID FROM C_Order WHERE C_Order_ID = "
                + line.GetC_Order_ID() + ")", null, trxName)) == "Y";

            if (retValue != null)
            {
                retValue.SetPrecision(precision);
                retValue.SetIsTaxIncluded(isTaxIncluded);
                retValue.Set_TrxName(trxName);
                s_log.Fine("(old=" + oldTax + ") " + retValue);
                return retValue;
            }

            //	Create New
            retValue = new MOrderTax(line.GetCtx(), 0, trxName);
            retValue.Set_TrxName(trxName);
            retValue.SetClientOrg(line);
            retValue.SetC_Order_ID(line.GetC_Order_ID());
            retValue.SetC_Tax_ID(C_Tax_ID);
            retValue.SetPrecision(precision);
            retValue.SetIsTaxIncluded(isTaxIncluded);
            s_log.Fine("(new) " + retValue);
            return retValue;
        }

        /// <summary>
        /// Calculate/Set Tax Amt from Order Lines
        /// </summary>
        /// <returns>true if aclculated</returns>
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
            Decimal taxBaseAmt = Env.ZERO;
            Decimal taxAmt = Env.ZERO;
            //
            bool documentLevel = GetTax().IsDocumentLevel();
            MTax tax = GetTax();

            // Calculate Tax on TaxAble Amount
            if (idr == null)
            {
                sql = "SELECT LineNetAmt, TaxAbleAmt, C_Order_ID, C_Tax_ID FROM C_OrderLine WHERE C_Order_ID=" + GetC_Order_ID() + " AND C_Tax_ID=" + GetC_Tax_ID();
                idr = DB.ExecuteDataset(sql, null, Get_TrxName());
                if (idr != null && idr.Tables.Count > 0 && idr.Tables[0].Rows.Count > 0)
                {
                    dr = idr.Tables[0].Select(" C_Order_ID = " + GetC_Order_ID() + " AND C_Tax_ID = " + GetC_Tax_ID());
                }
            }
            else
            {
                dr = idr.Tables[0].Select(" C_Order_ID = " + GetC_Order_ID() + " AND C_Tax_ID = " + GetC_Tax_ID());
            }
            try
            {
                if (dr != null && dr.Length > 0)
                {
                    for (int i = 0; i < dr.Length; i++)
                    {
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

                        taxBaseAmt = Decimal.Add(taxBaseAmt, baseAmt);
                        //
                        if (!documentLevel)     // calculate line tax
                            taxAmt = Decimal.Add(taxAmt, tax.CalculateTax(TaxableAmt,
                                (tax.GetSurcharge_Tax_ID() > 0) ? false :
                                IsTaxIncluded(), GetPrecision()));
                    }
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Dispose();
                }
                log.Log(Level.SEVERE, "CalculateTaxFromLines", e);
                log.Log(Level.SEVERE, sql, e);
                taxBaseAmt = Utility.Util.GetValueOfDecimal(null);
            }

            //	Calculate Tax
            if (documentLevel)		//	document level
                taxAmt = tax.CalculateTax(taxBaseAmt,false, GetPrecision());
            SetTaxAmt(taxAmt);

            //	Set Base
            //if (IsTaxIncluded())
            //    SetTaxBaseAmt(Decimal.Subtract(taxBaseAmt, taxAmt));
            //else
            SetTaxBaseAmt(Decimal.Round(taxBaseAmt, GetPrecision()));
            //log.fine(toString());
            return true;
        }

        /// <summary>
        /// Calculate/Set Surcharge Tax Amt from Order Lines
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
            String sql = "SELECT  ol.TaxAbleAmt, ol.TaxAmt, tax.SurchargeType, ol.LineNetAmt, tax.Rate FROM C_OrderLine ol"
                + " INNER JOIN C_Tax tax ON ol.C_Tax_ID=tax.C_Tax_ID WHERE ol.C_Order_ID=" + GetC_Order_ID()
                + " AND tax.Surcharge_Tax_ID=" + GetC_Tax_ID();
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                while (idr.Read())
                {

                    Decimal baseAmt = Util.GetValueOfDecimal(idr["LineNetAmt"]);
                    Decimal taxAmt = Util.GetValueOfDecimal(idr[1]);
                    string surchargeType = Util.GetValueOfString(idr[2]);
                    Decimal orignalTaxRate = Util.GetValueOfDecimal(idr["Rate"]);

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
                    //
                    if (!documentLevel)		// calculate Surcharge tax
                        surTaxAmt = Decimal.Add(surTaxAmt, surTax.CalculateTax(baseAmt, false, GetPrecision()));
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "CalculateSurchargeFromLines", e);
                taxBaseAmt = Utility.Util.GetValueOfDecimal(null);
            }

            //	Calculate Tax
            if (documentLevel)      //	document level
            {
                surTaxAmt = surTax.CalculateTax(taxBaseAmt, false, GetPrecision());
            }
            SetTaxAmt(surTaxAmt);

            SetTaxBaseAmt(Decimal.Round(taxBaseAmt, GetPrecision()));
            return true;
        }

        /**
        * 	Load Constructor.
        * 	Set Precision and TaxIncluded for tax calculations!
        *	@param ctx context
        *	@param dr result set
        *	@param trxName transaction
        */
        public MOrderTax(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**************************************************************************
	 * 	Persistency Constructor
	 *	@param ctx context
	 *	@param ignored ignored
	 *	@param trxName transaction
	 */
        public MOrderTax(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, 0, trxName)
        {

            if (ignored != 0)
                throw new ArgumentException("Multi-Key");
            SetTaxAmt(Env.ZERO);
            SetTaxBaseAmt(Env.ZERO);
            SetIsTaxIncluded(false);
        }

        /**
	 * 	Get Precision
	 * 	@return Returns the precision or 2
	 */
        private int GetPrecision()
        {
            if (_precision == null)
                return 2;
            return (int)_precision;
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


    }
}
