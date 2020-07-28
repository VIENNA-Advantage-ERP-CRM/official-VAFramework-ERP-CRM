/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MPriceListVersion
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     09-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MPriceListVersion : X_M_PriceList_Version
    {
        // Product Prices			
        private MProductPrice[] _pp = null;
        // Price List				
        private MPriceList _pl = null;

        /// <summary>
        /// Standard Cinstructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_PriceList_Version_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MPriceListVersion(Ctx ctx, int M_PriceList_Version_ID, Trx trxName)
            : base(ctx, M_PriceList_Version_ID, trxName)
        {
            if (M_PriceList_Version_ID == 0)
            {
                //	setName (null);	// @#Date@
                //	setM_PriceList_ID (0);
                //	setValidFrom (TimeUtil.getDay(null));	// @#Date@
                //	setM_DiscountSchema_ID (0);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MPriceListVersion(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="pl">parent</param>
        public MPriceListVersion(MPriceList pl)
            : this(pl.GetCtx(), 0, pl.Get_TrxName())
        {
            SetClientOrg(pl);
            SetM_PriceList_ID(pl.GetM_PriceList_ID());
        }

        /// <summary>
        /// Get Parent PriceList
        /// </summary>
        /// <returns>v</returns>
        public MPriceList GetPriceList()
        {
            if (_pl == null && GetM_PriceList_ID() != 0)
            {
                _pl = MPriceList.Get(GetCtx(), GetM_PriceList_ID(), null);
            }
            return _pl;
        }

        /// <summary>
        /// Get Product Price
        /// </summary>
        /// <param name="refresh">true if refresh</param>
        /// <returns>product price</returns>
        public MProductPrice[] GetProductPrice(bool refresh)
        {
            if (_pp != null && !refresh)
                return _pp;
            _pp = GetProductPrice(null);
            return _pp;
        }

        /// <summary>
        /// Get Product Price
        /// </summary>
        /// <param name="whereClause">optional where clause</param>
        /// <returns>product price</returns>
        public MProductPrice[] GetProductPrice(String whereClause)
        {
            List<MProductPrice> list = new List<MProductPrice>();
            String sql = "SELECT * FROM M_ProductPrice WHERE M_PriceList_Version_ID=" + GetM_PriceList_Version_ID();
            if (whereClause != null)
                sql += " " + whereClause;
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MProductPrice(GetCtx(), dr, Get_TrxName()));
                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            //
            MProductPrice[] pp = new MProductPrice[list.Count];
            pp = list.ToArray();
            return pp;
        }

        /// <summary>
        /// Set Name to Valid From Date.
        /// If valid from not set, use today
        /// </summary>
        public void SetName()
        {
            if (GetValidFrom() == null)
            {
                SetValidFrom(TimeUtil.GetDay(null));
            }
            if (GetName() == null)
            {
                //String name = DisplayType.getDateFormat(DisplayType.Date).format(getValidFrom());
                String name = GetValidFrom().ToString();
                SetName(name);
            }
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            SetName();
            return true;
        }
    }
}
