/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : UserPricingVO
 * Purpose        : 
 * Class Used     : 
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
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;

namespace VAdvantage.Model
{
    public class UserPricingVO
    {
        #region Private Variables
        //	Std	(Actual)	
        private Decimal? _PriceStd = null;
        // List		
        private Decimal? _PriceList = null;
        // Limit		
        private Decimal? _PriceLimit = null;
        // Currency	
        private int _C_Currency_ID = 0;
        // Enforce Price Limit	
        private bool _enforcePriceLimit = false;
        // Product UOM		
        private int _C_UOM_ID = 0;
        #endregion

        /// <summary>
        /// 	User Pricing VO
        /// </summary>
        /// <param name="price">one price</param>
        public UserPricingVO(Decimal price)
        {
            SetPriceList(price);
        }

        /// <summary>
        /// User Pricing VO
        /// </summary>
        /// <param name="priceList">list</param>
        /// <param name="priceStd">std</param>
        /// <param name="priceLimit">limit</param>
        public UserPricingVO(Decimal priceList, Decimal priceStd, Decimal priceLimit)
        {
            SetPriceList(priceList);
            SetPriceStd(priceStd);
            SetPriceLimit(priceLimit);
        }

        /// <summary>
        /// Do we have valid Price Info
        /// </summary>
        /// <returns>true if there is a list price</returns>
        public bool IsValid()
        {
            return _PriceList != null;
        }

        /// <summary>
        /// Get Price Std
        /// </summary>
        /// <returns>Std or list</returns>
        public Decimal GetPriceStd()
        {
            if (_PriceStd != null)
            {
                return (decimal)_PriceStd;
            }
            return (decimal)_PriceList;
        }

        /// <summary>
        /// Set Price Std
        /// </summary>
        /// <param name="priceStd">priceStd</param>
        public void SetPriceStd(Decimal priceStd)
        {
            _PriceStd = priceStd;
        }

        /// <summary>
        /// Get Price Limit
        /// </summary>
        /// <returns>limit or Std or list</returns>
        public Decimal GetPriceLimit()
        {
            if (_PriceLimit != null)
            {
                return (decimal)_PriceLimit;
            }
            if (_PriceStd != null)
            {
                return (decimal)_PriceStd;
            }
            return (decimal)_PriceList;
        }

        /// <summary>
        /// Set Price Limit
        /// </summary>
        /// <param name="priceLimit">priceLimit</param>
        public void SetPriceLimit(Decimal priceLimit)
        {
            _PriceLimit = priceLimit;
        }

        /// <summary>
        /// Get Price List
        /// </summary>
        /// <returns>list</returns>
        public Decimal GetPriceList()
        {
            return (decimal)_PriceList;
        }

        /// <summary>
        /// Set Price List
        /// </summary>
        /// <param name="priceList">priceList</param>
        public void SetPriceList(Decimal priceList)
        {
            _PriceList = priceList;
        }

        /// <summary>
        /// get country
        /// </summary>
        /// <returns>id</returns>
        public int GetC_Currency_ID()
        {
            return _C_Currency_ID;
        }

        /// <summary>
        /// set id
        /// </summary>
        /// <param name="currency_ID"></param>
        public void SetC_Currency_ID(int currency_ID)
        {
            _C_Currency_ID = currency_ID;
        }

        /// <summary>
        /// get UMO id
        /// </summary>
        /// <returns></returns>
        public int GetC_UOM_ID()
        {
            return _C_UOM_ID;
        }

        /// <summary>
        /// set Umo
        /// </summary>
        /// <param name="uom_id"></param>
        public void SetC_UOM_ID(int uom_id)
        {
            _C_UOM_ID = uom_id;
        }

        /// <summary>
        /// Prise limit
        /// </summary>
        /// <returns></returns>
        public bool IsEnforcePriceLimit()
        {
            return _enforcePriceLimit;
        }

        /// <summary>
        /// set limit
        /// </summary>
        /// <param name="priceLimit"></param>
        public void SetEnforcePriceLimit(bool priceLimit)
        {
            _enforcePriceLimit = priceLimit;
        }

    }
}
