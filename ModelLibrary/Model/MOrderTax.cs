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

            if (retValue != null)
            {
                retValue.SetPrecision(precision);
                retValue.Set_TrxName(trxName);
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
        /// Calculate/Set Tax Amt from Order Lines
        /// </summary>
        /// <returns>true if aclculated</returns>
        public bool CalculateTaxFromLines()
        {
            Decimal taxBaseAmt = Env.ZERO;
            Decimal taxAmt = Env.ZERO;
            //
            bool documentLevel = GetTax().IsDocumentLevel();
            MTax tax = GetTax();
            //
            String sql = "SELECT LineNetAmt FROM C_OrderLine WHERE C_Order_ID=" + GetC_Order_ID() + " AND C_Tax_ID=" + GetC_Tax_ID();
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
                        taxAmt = Decimal.Add(taxAmt, tax.CalculateTax(baseAmt, IsTaxIncluded(), GetPrecision()));
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
                taxBaseAmt = Utility.Util.GetValueOfDecimal(null);
            }
           
            ////
            //if (taxBaseAmt == null)
            //{
            //    return false;
            //}

            //	Calculate Tax
            if (documentLevel)		//	document level
                taxAmt = tax.CalculateTax(taxBaseAmt, IsTaxIncluded(), GetPrecision());
            SetTaxAmt(taxAmt);

            //	Set Base
            if (IsTaxIncluded())
                SetTaxBaseAmt(Decimal.Subtract(taxBaseAmt, taxAmt));
            else
                SetTaxBaseAmt(taxBaseAmt);
            //log.fine(toString());
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
