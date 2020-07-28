/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MDiscountSchema
 * Purpose        : Discount Schema Model
 * Class Used     : X_M_DiscountSchema 
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
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MDiscountSchema : X_M_DiscountSchema
    {
        #region Privatevariables
        //	Cache						
        private static CCache<int, MDiscountSchema> s_cache = new CCache<int, MDiscountSchema>("M_DiscountSchema", 20);
        //	Breaks						
        private MDiscountSchemaBreak[] _breaks = null;
        //Lines							
        private MDiscountSchemaLine[] _lines = null;
        #endregion

        /// <summary>
        /// Get Discount Schema from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_DiscountSchema_ID">id</param>
        /// <returns><MDiscountSchema/returns>
        public static MDiscountSchema Get(Ctx ctx, int M_DiscountSchema_ID)
        {
            int key = M_DiscountSchema_ID;
            MDiscountSchema retValue = (MDiscountSchema)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MDiscountSchema(ctx, M_DiscountSchema_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="M_DiscountSchema_ID"></param>
        /// <param name="trxName"></param>
        public MDiscountSchema(Ctx ctx, int M_DiscountSchema_ID, Trx trxName)
            : base(ctx, M_DiscountSchema_ID, trxName)
        {
            if (M_DiscountSchema_ID == 0)
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
        public MDiscountSchema(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Get Breaks
        /// </summary>
        /// <param name="reload">reload</param>
        /// <returns>breaks</returns>
        public MDiscountSchemaBreak[] GetBreaks(bool reload)
        {
            if (_breaks != null && !reload)
                return _breaks;

            String sql = "SELECT * FROM M_DiscountSchemaBreak WHERE M_DiscountSchema_ID=" + GetM_DiscountSchema_ID() + " ORDER BY SeqNo";
            List<MDiscountSchemaBreak> list = new List<MDiscountSchemaBreak>();
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MDiscountSchemaBreak(GetCtx(), dr, Get_TrxName()));
                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            _breaks = new MDiscountSchemaBreak[list.Count];
            _breaks = list.ToArray();
            return _breaks;
        }

        /// <summary>
        /// Get Lines
        /// </summary>
        /// <param name="reload">reload</param>
        /// <returns>lines</returns>
        public MDiscountSchemaLine[] GetLines(bool reload)
        {
            if (_lines != null && !reload)
                return _lines;

            String sql = "SELECT * FROM M_DiscountSchemaLine WHERE M_DiscountSchema_ID=" + GetM_DiscountSchema_ID() + " ORDER BY SeqNo";
            List<MDiscountSchemaLine> list = new List<MDiscountSchemaLine>();
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MDiscountSchemaLine(GetCtx(), dr, Get_TrxName()));
                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            _lines = new MDiscountSchemaLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

        /// <summary>
        /// Calculate Discounted Price
        /// </summary>
        /// <param name="Qty">quantity</param>
        /// <param name="Price">price</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_Product_Category_ID">category</param>
        /// <param name="BPartnerFlatDiscount">flat discount</param>
        /// <returns>discount or zero</returns>
        public Decimal CalculatePrice(Decimal Qty, Decimal Price, int M_Product_ID, int M_Product_Category_ID,
            Decimal BPartnerFlatDiscount)
        {
            log.Fine("Price=" + Price + ",Qty=" + Qty);
            if (Price == null || Env.ZERO.CompareTo(Price) == 0)
            {
                return Price;
            }
            //get discount accordinf to Bpartner
            Decimal discount = CalculateDiscount(Qty, Price, M_Product_ID, M_Product_Category_ID, BPartnerFlatDiscount);
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
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_Product_Category_ID">category</param>
        /// <param name="BPartnerFlatDiscount">flat discount</param>
        /// <returns>discount or zero</returns>
        public Decimal CalculateDiscount(Decimal Qty, Decimal Price, int M_Product_ID, int M_Product_Category_ID,
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
                log.Finer("Qty=" + Qty + ",M_Product_ID=" + M_Product_ID + ",M_Product_Category_ID=" + M_Product_Category_ID);
            }
            else
            {
                log.Finer("Amt=" + Amt + ",M_Product_ID=" + M_Product_ID + ",M_Product_Category_ID=" + M_Product_Category_ID);
            }
            for (int i = 0; i < _breaks.Length; i++)
            {
                MDiscountSchemaBreak br = _breaks[i];
                if (!br.IsActive())
                {
                    continue;
                }
                if (IsQuantityBased())
                {
                    if (!br.Applies(Qty, M_Product_ID, M_Product_Category_ID))
                    {
                        log.Finer("No: " + br);
                        continue;
                    }
                    log.Finer("Yes: " + br);
                }
                else
                {
                    if (!br.Applies(Amt, M_Product_ID, M_Product_Category_ID))
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
            MDiscountSchemaLine[] lines = GetLines(true);
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
            MDiscountSchemaBreak[] breaks = GetBreaks(true);
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
