/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAMDiscountCalculation
 * Purpose        : Discount Schema Model
 * Class Used     : X_VAM_DiscountCalculation 
 * Chronological    Development
 * Raghunandan     10-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Print;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVAMDiscountCalculation : X_VAM_DiscountCalculation
    {
        #region Privatevariables
        //	Cache						
        private static CCache<int, MVAMDiscountCalculation> s_cache = new CCache<int, MVAMDiscountCalculation>("VAM_DiscountCalculation", 20);
        //	Breaks						
        private MVAMBreakDiscount[] _breaks = null;
        //Lines							
        private MVAMPriceDiscount[] _lines = null;
        #endregion

        /// <summary>
        /// Get Discount Schema from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAM_DiscountCalculation_ID">id</param>
        /// <returns><MVAMDiscountCalculation/returns>
        public static MVAMDiscountCalculation Get(Ctx ctx, int VAM_DiscountCalculation_ID)
        {
            int key = VAM_DiscountCalculation_ID;
            MVAMDiscountCalculation retValue = (MVAMDiscountCalculation)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MVAMDiscountCalculation(ctx, VAM_DiscountCalculation_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAM_DiscountCalculation_ID"></param>
        /// <param name="trxName"></param>
        public MVAMDiscountCalculation(Ctx ctx, int VAM_DiscountCalculation_ID, Trx trxName)
            : base(ctx, VAM_DiscountCalculation_ID, trxName)
        {
            if (VAM_DiscountCalculation_ID == 0)
            {
                //	setName();
                SetDiscountType(DISCOUNTTYPE_FlatPercent);
                SetFlatDiscount(Env.ZERO);
                SetIsBPartnerFlatDiscount(false);
                SetIsQuantityBased(true);	// Y
                SetCumulativeLevel(CUMULATIVELEVEL_Line);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MVAMDiscountCalculation(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Get Breaks
        /// </summary>
        /// <param name="reload">reload</param>
        /// <returns>breaks</returns>
        public MVAMBreakDiscount[] GetBreaks(bool reload)
        {
            if (_breaks != null && !reload)
                return _breaks;

            String sql = "SELECT * FROM VAM_BreakDiscount WHERE VAM_DiscountCalculation_ID=" + GetVAM_DiscountCalculation_ID() + " ORDER BY SeqNo";
            List<MVAMBreakDiscount> list = new List<MVAMBreakDiscount>();
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MVAMBreakDiscount(GetCtx(), dr, Get_TrxName()));
                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            _breaks = new MVAMBreakDiscount[list.Count];
            _breaks = list.ToArray();
            return _breaks;
        }

        /// <summary>
        /// Get Lines
        /// </summary>
        /// <param name="reload">reload</param>
        /// <returns>lines</returns>
        public MVAMPriceDiscount[] GetLines(bool reload)
        {
            if (_lines != null && !reload)
                return _lines;

            String sql = "SELECT * FROM VAM_PriceDiscount WHERE VAM_DiscountCalculation_ID=" + GetVAM_DiscountCalculation_ID() + " ORDER BY SeqNo";
            List<MVAMPriceDiscount> list = new List<MVAMPriceDiscount>();
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MVAMPriceDiscount(GetCtx(), dr, Get_TrxName()));
                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            _lines = new MVAMPriceDiscount[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

        /// <summary>
        /// Calculate Discounted Price
        /// </summary>
        /// <param name="Qty">quantity</param>
        /// <param name="Price">price</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_ProductCategory_ID">category</param>
        /// <param name="BPartnerFlatDiscount">flat discount</param>
        /// <returns>discount or zero</returns>
        public Decimal CalculatePrice(Decimal Qty, Decimal Price, int VAM_Product_ID, int VAM_ProductCategory_ID,
            Decimal BPartnerFlatDiscount)
        {
            log.Fine("Price=" + Price + ",Qty=" + Qty);
            if (Price == null || Env.ZERO.CompareTo(Price) == 0)
            {
                return Price;
            }
            //get discount accordinf to Bpartner
            Decimal discount = CalculateDiscount(Qty, Price, VAM_Product_ID, VAM_ProductCategory_ID, BPartnerFlatDiscount);
            //	nothing to calculate
            if (discount == null || discount == 0)
            {
                return Price;
            }
            //
            Decimal onehundred = Env.ONEHUNDRED;// new Decimal(100);
            Decimal multiplier = Decimal.Subtract(onehundred, discount);
            multiplier = Decimal.Round(Decimal.Divide(multiplier, onehundred), 6, MidpointRounding.AwayFromZero);
            Decimal newPrice = Decimal.Multiply(Price, multiplier);
            log.Fine("=>" + newPrice);
            return newPrice;
        }

        /// <summary>
        /// Calculate Discount Percentage
        /// </summary>
        /// <param name="Qty">quantity</param>
        /// <param name="Price">price</param>
        /// <param name="VAM_Product_ID">product</param>
        /// <param name="VAM_ProductCategory_ID">category</param>
        /// <param name="BPartnerFlatDiscount">flat discount</param>
        /// <returns>discount or zero</returns>
        public Decimal CalculateDiscount(Decimal Qty, Decimal Price, int VAM_Product_ID, int VAM_ProductCategory_ID,
            Decimal BPartnerFlatDiscount)
        {
            //if (BPartnerFlatDiscount == null)
            //{
            //    BPartnerFlatDiscount = Env.ZERO;
            //}

            //
            if (DISCOUNTTYPE_FlatPercent.Equals(GetDiscountType()))
            {
                if (IsBPartnerFlatDiscount())
                {
                    return BPartnerFlatDiscount;
                }
                return GetFlatDiscount();
            }
            //	Not supported
            else if (DISCOUNTTYPE_Formula.Equals(GetDiscountType())
                || DISCOUNTTYPE_Pricelist.Equals(GetDiscountType()))
            {
                log.Info("Not supported (yet) DiscountType=" + GetDiscountType());
                return Env.ZERO;
            }

            //	Price Breaks
            GetBreaks(false);
           /// bool found = false;
            Decimal Amt = Decimal.Multiply(Price, Qty);
            if (IsQuantityBased())
            {
                log.Finer("Qty=" + Qty + ",VAM_Product_ID=" + VAM_Product_ID + ",VAM_ProductCategory_ID=" + VAM_ProductCategory_ID);
            }
            else
            {
                log.Finer("Amt=" + Amt + ",VAM_Product_ID=" + VAM_Product_ID + ",VAM_ProductCategory_ID=" + VAM_ProductCategory_ID);
            }
            for (int i = 0; i < _breaks.Length; i++)
            {
                MVAMBreakDiscount br = _breaks[i];
                if (!br.IsActive())
                {
                    continue;
                }
                if (IsQuantityBased())
                {
                    if (!br.Applies(Qty, VAM_Product_ID, VAM_ProductCategory_ID))
                    {
                        log.Finer("No: " + br);
                        continue;
                    }
                    log.Finer("Yes: " + br);
                }
                else
                {
                    if (!br.Applies(Amt, VAM_Product_ID, VAM_ProductCategory_ID))
                    {
                        log.Finer("No: " + br);
                        continue;
                    }
                    log.Finer("Yes: " + br);
                }

                //	Line applies
                Decimal? discount = null;
                if (br.IsBPartnerFlatDiscount())
                {
                    discount = BPartnerFlatDiscount;
                }
                else
                {
                    discount = br.GetBreakDiscount();
                }
                log.Fine("Discount=>" + discount);
                return Convert.ToDecimal(discount);
            }	//	for all breaks

            return Env.ZERO;
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns> true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetValidFrom() == null)
            {
                SetValidFrom(TimeUtil.GetDay(null));
            }

            return true;
        }

        /// <summary>
        /// Renumber
        /// </summary>
        /// <returns>lines updated</returns>
        public int ReSeq()
        {
            int count = 0;
            //	Lines
            MVAMPriceDiscount[] lines = GetLines(true);
            for (int i = 0; i < lines.Length; i++)
            {
                int line = (i + 1) * 10;
                if (line != lines[i].GetSeqNo())
                {
                    lines[i].SetSeqNo(line);
                    if (lines[i].Save())
                    {
                        count++;
                    }
                }
            }
            _lines = null;

            //	Breaks
            MVAMBreakDiscount[] breaks = GetBreaks(true);
            for (int i = 0; i < breaks.Length; i++)
            {
                int line = (i + 1) * 10;
                if (line != breaks[i].GetSeqNo())
                {
                    breaks[i].SetSeqNo(line);
                    if (breaks[i].Save())
                    {
                        count++;
                    }
                }
            }
            _breaks = null;
            return count;
        }
    }
}
