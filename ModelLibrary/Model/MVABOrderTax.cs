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
//////using System.Windows.Forms;
using VAdvantage.SqlExec;
using VAdvantage.DataBase;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVABOrderTax : X_VAB_OrderTax
    {
        /**	Static Logger	*/
        private static VLogger s_log = VLogger.GetVLogger(typeof(MVABOrderTax).FullName);

        /** Tax							*/
        private MVABTaxRate _tax = null;
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
        public static MVABOrderTax Get(MVABOrderLine line, int precision, bool oldTax, Trx trxName)
        {
            MVABOrderTax retValue = null;
            if (line == null || line.GetVAB_Order_ID() == 0)
            {
                //s_log.fine("No Order");
                return null;
            }
            int VAB_TaxRate_ID = line.GetVAB_TaxRate_ID();
            if (oldTax && line.Is_ValueChanged("VAB_TaxRate_ID"))
            {
                Object old = line.Get_ValueOld("VAB_TaxRate_ID");
                if (old == null)
                {
                    //s_log.fine("No Old Tax");
                    return null;
                }
                VAB_TaxRate_ID = int.Parse(old.ToString());
            }
            if (VAB_TaxRate_ID == 0)
            {
                s_log.Fine("No Tax");
                return null;
            }

            String sql = "SELECT * FROM VAB_OrderTax WHERE VAB_Order_ID=" + line.GetVAB_Order_ID() + " AND VAB_TaxRate_ID=" + VAB_TaxRate_ID;
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue = new MVABOrderTax(line.GetCtx(), dr, trxName);
                }
                ds = null;
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);
            }

            // JID_1303: On Order tax calculate tax according to selected pricelist. If user delete lines and change pricelist, it should check IsTaxIncluded on selected Pricelist.
            bool isTaxIncluded = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsTaxIncluded FROM VAM_PriceList WHERE VAM_PriceList_ID = (SELECT VAM_PriceList_ID FROM VAB_Order WHERE VAB_Order_ID = "
                + line.GetVAB_Order_ID() + ")", null, trxName)) == "Y";

            if (retValue != null)
            {
                retValue.SetPrecision(precision);
                retValue.Set_TrxName(trxName);

                retValue.SetIsTaxIncluded(isTaxIncluded);

                s_log.Fine("(old=" + oldTax + ") " + retValue);
                return retValue;
            }

            //	Create New
            retValue = new MVABOrderTax(line.GetCtx(), 0, trxName);
            retValue.Set_TrxName(trxName);
            retValue.SetClientOrg(line);
            retValue.SetVAB_Order_ID(line.GetVAB_Order_ID());
            retValue.SetVAB_TaxRate_ID(line.GetVAB_TaxRate_ID());
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
        public static MVABOrderTax GetSurcharge(MVABOrderLine line, int precision, bool oldTax, Trx trxName)
        {
            MVABOrderTax retValue = null;
            if (line == null || line.GetVAB_Order_ID() == 0)
            {
                //s_log.fine("No Order");
                return null;
            }
            int VAB_TaxRate_ID = line.GetVAB_TaxRate_ID();
            if (oldTax && line.Is_ValueChanged("VAB_TaxRate_ID"))
            {
                Object old = line.Get_ValueOld("VAB_TaxRate_ID");
                if (old == null)
                {
                    //s_log.fine("No Old Tax");
                    return null;
                }
                VAB_TaxRate_ID = int.Parse(old.ToString());
            }

            // Get Surcharge Tax ID from Tax selected on Line
            VAB_TaxRate_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Surcharge_Tax_ID FROM VAB_TaxRate WHERE VAB_TaxRate_ID = " + VAB_TaxRate_ID, null, trxName));

            if (VAB_TaxRate_ID == 0)
            {
                s_log.Fine("No Tax");
                return null;
            }

            String sql = "SELECT * FROM VAB_OrderTax WHERE VAB_Order_ID=" + line.GetVAB_Order_ID() + " AND VAB_TaxRate_ID=" + VAB_TaxRate_ID;
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        retValue = new MVABOrderTax(line.GetCtx(), dr, trxName);
                    }
                    ds = null;
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);
            }

            // Get IsTaxincluded from selected PriceList on header
            bool isTaxIncluded = Util.GetValueOfString(DB.ExecuteScalar("SELECT IsTaxIncluded FROM VAM_PriceList WHERE VAM_PriceList_ID = (SELECT VAM_PriceList_ID FROM VAB_Order WHERE VAB_Order_ID = "
                + line.GetVAB_Order_ID() + ")", null, trxName)) == "Y";

            if (retValue != null)
            {
                retValue.SetPrecision(precision);
                retValue.SetIsTaxIncluded(isTaxIncluded);
                retValue.Set_TrxName(trxName);
                s_log.Fine("(old=" + oldTax + ") " + retValue);
                return retValue;
            }

            //	Create New
            retValue = new MVABOrderTax(line.GetCtx(), 0, trxName);
            retValue.Set_TrxName(trxName);
            retValue.SetClientOrg(line);
            retValue.SetVAB_Order_ID(line.GetVAB_Order_ID());
            retValue.SetVAB_TaxRate_ID(VAB_TaxRate_ID);
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
            Decimal taxBaseAmt = Env.ZERO;
            Decimal taxAmt = Env.ZERO;
            //
            bool documentLevel = GetTax().IsDocumentLevel();
            MVABTaxRate tax = GetTax();
            
            // Calculate Tax on TaxAble Amount
            String sql = "SELECT TaxAbleAmt FROM VAB_OrderLine WHERE VAB_Order_ID=" + GetVAB_Order_ID() + " AND VAB_TaxRate_ID=" + GetVAB_TaxRate_ID();
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                while (idr.Read())
                {
                    Decimal baseAmt = Utility.Util.GetValueOfDecimal(idr[0]);
                    taxBaseAmt = Decimal.Add(taxBaseAmt, baseAmt);
                    //
                    if (!documentLevel)		// calculate line tax
                        taxAmt = Decimal.Add(taxAmt, tax.CalculateTax(baseAmt, false, GetPrecision()));
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "CalculateTaxFromLines", e);
                log.Log(Level.SEVERE, sql, e);
                taxBaseAmt = Utility.Util.GetValueOfDecimal(null);
            }            

            //	Calculate Tax
            if (documentLevel)		//	document level
                taxAmt = tax.CalculateTax(taxBaseAmt, false, GetPrecision());
            SetTaxAmt(taxAmt);

            //	Set Base
            //if (IsTaxIncluded())
            //    SetTaxBaseAmt(Decimal.Subtract(taxBaseAmt, taxAmt));
            //else
            SetTaxBaseAmt(taxBaseAmt);
            //log.fine(toString());
            return true;
        }

        /// <summary>
        /// Calculate/Set Surcharge Tax Amt from Order Lines
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
            String sql = "SELECT ol.TaxAbleAmt, ol.TaxAmt, tax.SurchargeType FROM VAB_OrderLine ol"
                + " INNER JOIN VAB_TaxRate tax ON ol.VAB_TaxRate_ID=tax.VAB_TaxRate_ID WHERE ol.VAB_Order_ID=" + GetVAB_Order_ID() 
                + " AND tax.Surcharge_Tax_ID=" + GetVAB_TaxRate_ID();
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                while (idr.Read())
                {
                    Decimal baseAmt = Util.GetValueOfDecimal(idr[0]);
                    Decimal taxAmt = Util.GetValueOfDecimal(idr[1]);
                    string surchargeType = Util.GetValueOfString(idr[2]);

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
            SetTaxBaseAmt(taxBaseAmt);
            return true;
        }

        /**
        * 	Load Constructor.
        * 	Set Precision and TaxIncluded for tax calculations!
        *	@param ctx context
        *	@param dr result set
        *	@param trxName transaction
        */
        public MVABOrderTax(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**************************************************************************
	 * 	Persistency Constructor
	 *	@param ctx context
	 *	@param ignored ignored
	 *	@param trxName transaction
	 */
        public MVABOrderTax(Ctx ctx, int ignored, Trx trxName)
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
        public MVABTaxRate GetTax()
        {
            if (_tax == null)
                _tax = MVABTaxRate.Get(GetCtx(), GetVAB_TaxRate_ID());
            return _tax;
        }


    }
}
