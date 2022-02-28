/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MProductPricing
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     04-Jun-2009
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
    public class MProductPricing
    {
        #region Private variables
        private int _AD_Client_ID;
        private int _AD_Org_ID;
        private int _M_Product_ID;
        private int _C_BPartner_ID;
        private Decimal _qty = Env.ONE;
        private Boolean _isSOTrx = true;
        //
        private int _M_PriceList_ID = 0;
        private int _M_PriceList_Version_ID = 0;
        private int _M_AttributeSetInstance_ID = 0;
        private DateTime? _PriceDate;
        /** Precision -1 = no rounding		*/
        private int _precision = -1;


        private Boolean _calculated = false;
        private Boolean? _found = null;

        private Decimal _PriceList = Env.ZERO;
        private Decimal _PriceStd = Env.ZERO;
        private Decimal _PriceLimit = Env.ZERO;
        private int _C_Currency_ID = 0;
        private Boolean _enforcePriceLimit = false;
        private int _C_UOM_ID = 0;
        private int _M_Product_Category_ID;
        private Boolean _discountSchema = false;
        private Boolean _isTaxIncluded = false;

        private Boolean? _userPricing = null;
        private UserPricingInterface _api = null;

        /**	Logger			*/
        //logger
        protected VLogger log = null;
        //protected CLogger log = CLogger.getCLogger(getClass());

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="AD_Client_ID">product</param>
        /// <param name="AD_Org_ID">partner</param>
        /// <param name="M_Product_ID"></param>
        /// <param name="C_BPartner_ID"></param>
        /// <param name="Qty">quantity</param>
        /// <param name="isSOTrx">SO or PO</param>
        public MProductPricing(int AD_Client_ID, int AD_Org_ID,
            int M_Product_ID, int C_BPartner_ID,
            Decimal? qty, bool isSOTrx)
        {
            if (log == null)
            {
                log = VLogger.GetVLogger(this.GetType().FullName);
            }
            _AD_Client_ID = AD_Client_ID;
            _AD_Org_ID = AD_Org_ID;
            _M_Product_ID = M_Product_ID;
            _C_BPartner_ID = C_BPartner_ID;
            if (qty != null && Env.ZERO.CompareTo(qty) != 0)
                _qty = (Decimal)qty;
            _isSOTrx = isSOTrx;
        }

        /// <summary>
        /// Calculate Price
        /// </summary>
        /// <returns> true if calculated</returns>
        public bool CalculatePrice()
        {
            if (_M_Product_ID == 0 || (_found != null && !(Boolean)_found))	//	previously not found
                return false;
            //	Customer Pricing Engine
            if (!_calculated)
                _calculated = CalculateUser();
            //	Price List Version known
            if (!_calculated)
                _calculated = CalculatePLV();
            //	Price List known
            if (!_calculated)
                _calculated = CalculatePL();
            //	Base Price List used
            if (!_calculated)
                _calculated = CalculateBPL();
            //	Set UOM, Prod.Category
            if (!_calculated)
                SetBaseInfo();
            //	User based Discount
            if (_calculated)
                CalculateDiscount();
            SetPrecision();		//	from Price List
            _found = _calculated;
            return _calculated;
        }

        /// <summary>
        /// Calculate User Price
        /// </summary>
        /// <returns>true if calculated</returns>
        private bool CalculateUser()
        {
            if (_userPricing == null)
            {
                MClientInfo client = MClientInfo.Get(Env.GetContext(), _AD_Client_ID);
                String userClass = client.GetPricingEngineClass();
                try
                {
                    // class<?> clazz = null;
                    Type clazz = null;
                    if (userClass != null)
                    {
                        //clazz = Class.forName(userClass);
                        clazz = Type.GetType(userClass);
                    }
                    if (clazz != null)
                    {
                        _api = (UserPricingInterface)Activator.CreateInstance(clazz);
                    }
                }
                catch (Exception ex)
                {
                    log.Warning("No User Pricing Engine (" + userClass + ") " + ex.ToString());
                    _userPricing = false;
                    return false;
                }
                _userPricing = _api != null;
            }
            if (!(Boolean)_userPricing)
                return false;

            UserPricingVO vo = null;
            if (_api != null)
            {
                try
                {
                    vo = _api.Price(_AD_Org_ID, _isSOTrx, _M_PriceList_ID,
                        _C_BPartner_ID, _M_Product_ID, _qty, _PriceDate);
                }
                catch (Exception ex)
                {
                    log.Warning("Error User Pricing - " + ex.ToString());
                    return false;
                }
            }

            if (vo != null && vo.IsValid())
            {
                _PriceList = vo.GetPriceList();
                _PriceStd = vo.GetPriceStd();
                _PriceLimit = vo.GetPriceLimit();
                _found = true;
                //	Optional
                _C_UOM_ID = vo.GetC_UOM_ID();
                _C_Currency_ID = vo.GetC_Currency_ID();
                _enforcePriceLimit = vo.IsEnforcePriceLimit();
                if (_C_UOM_ID == 0 || _C_Currency_ID == 0)
                    SetBaseInfo();
            }
            return false;
        }

        /// <summary>
        /// Calculate Price based on Price List Version
        /// </summary>
        /// <returns>true if calculated</returns>
        private bool CalculatePLV()
        {
            String sql = "";
            if (_M_Product_ID == 0 || _M_PriceList_Version_ID == 0)
                return false;
            // Check For Advance Pricing Module
            if (Env.IsModuleInstalled("VAPRC_"))
            {
                if (Env.IsModuleInstalled("ED011_"))
                {
                    /** Price List - Ensuring valid Uom id ** Dt:01/02/2021 ** Modified By: Kumar **/
                    if (_C_UOM_ID <= 0)
                    {
                        //vikas  mantis Issue ( 0000517)
                        string _sql = null;
                        _sql = "SELECT C_UOM_ID FROM M_Product WHERE  M_Product_ID=" + _M_Product_ID;
                        _C_UOM_ID = Util.GetValueOfInt(DB.ExecuteScalar(_sql));
                    }
                    sql = "SELECT bomPriceStdUOM(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID , pp.C_UOM_ID) AS PriceStd,"	//	1
                       + " bomPriceListUOM(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID , pp.C_UOM_ID) AS PriceList,"		//	2
                       + " bomPriceLimitUOM(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID , pp.C_UOM_ID) AS PriceLimit,"	//	3
                       + " p.C_UOM_ID,pv.ValidFrom,pl.C_Currency_ID,p.M_Product_Category_ID,"	//	4..7
                       + " pl.EnforcePriceLimit, pl.IsTaxIncluded "	// 8..9
                       + "FROM M_Product p"
                       + " INNER JOIN M_ProductPrice pp ON (p.M_Product_ID=pp.M_Product_ID)"
                       + " INNER JOIN  M_PriceList_Version pv ON (pp.M_PriceList_Version_ID=pv.M_PriceList_Version_ID)"
                       + " INNER JOIN M_PriceList pl ON (pv.M_PriceList_ID=pl.M_PriceList_ID) "
                       + "WHERE pv.IsActive='Y'"
                       + " AND p.M_Product_ID=" + _M_Product_ID				//	#1
                       + " AND pv.M_PriceList_Version_ID=" + _M_PriceList_Version_ID	//	#2
                       + " AND pp.M_AttributeSetInstance_ID = " + _M_AttributeSetInstance_ID  //	#3
                       + " AND pp.C_UOM_ID = " + _C_UOM_ID  //    #4
                       + " AND pp.IsActive='Y'";
                }
                else
                {
                    sql = "SELECT bomPriceStdAttr(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID) AS PriceStd,"	//	1
                        + " bomPriceListAttr(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID) AS PriceList,"		//	2
                        + " bomPriceLimitAttr(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID) AS PriceLimit,"	//	3
                        + " p.C_UOM_ID,pv.ValidFrom,pl.C_Currency_ID,p.M_Product_Category_ID,"	//	4..7
                        + " pl.EnforcePriceLimit, pl.IsTaxIncluded "	// 8..9
                        + "FROM M_Product p"
                        + " INNER JOIN M_ProductPrice pp ON (p.M_Product_ID=pp.M_Product_ID)"
                        + " INNER JOIN  M_PriceList_Version pv ON (pp.M_PriceList_Version_ID=pv.M_PriceList_Version_ID)"
                        + " INNER JOIN M_PriceList pl ON (pv.M_PriceList_ID=pl.M_PriceList_ID) "
                        + "WHERE pv.IsActive='Y'"
                        + " AND p.M_Product_ID=" + _M_Product_ID				//	#1
                        + " AND pv.M_PriceList_Version_ID=" + _M_PriceList_Version_ID	//	#2
                        + " AND pp.M_AttributeSetInstance_ID = " + _M_AttributeSetInstance_ID;   //	#3
                }
            }
            else
            {
                sql = "SELECT bomPriceStd(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceStd,"	//	1
                    + " bomPriceList(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceList,"		//	2
                    + " bomPriceLimit(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceLimit,"	//	3
                    + " p.C_UOM_ID,pv.ValidFrom,pl.C_Currency_ID,p.M_Product_Category_ID,"	//	4..7
                    + " pl.EnforcePriceLimit, pl.IsTaxIncluded "	// 8..9
                    + "FROM M_Product p"
                    + " INNER JOIN M_ProductPrice pp ON (p.M_Product_ID=pp.M_Product_ID)"
                    + " INNER JOIN  M_PriceList_Version pv ON (pp.M_PriceList_Version_ID=pv.M_PriceList_Version_ID)"
                    + " INNER JOIN M_PriceList pl ON (pv.M_PriceList_ID=pl.M_PriceList_ID) "
                    + "WHERE pv.IsActive='Y'"
                    + " AND p.M_Product_ID=" + _M_Product_ID				//	#1
                    + " AND pv.M_PriceList_Version_ID=" + _M_PriceList_Version_ID;	//	#2
            }
            _calculated = false;
            try
            {
                DataSet ds = ExecuteQuery.ExecuteDataset(sql, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    //	Prices
                    _PriceStd = Utility.Util.GetValueOfDecimal(dr[0]);//.getBigDecimal(1);
                    if (dr[0] == null)
                        _PriceStd = Env.ZERO;
                    _PriceList = Utility.Util.GetValueOfDecimal(dr[1]);//.getBigDecimal(2);
                    if (dr[1] == null)
                        _PriceList = Env.ZERO;
                    _PriceLimit = Utility.Util.GetValueOfDecimal(dr[2]);//.getBigDecimal(3);
                    if (dr[2] == null)
                        _PriceLimit = Env.ZERO;
                    //
                    _C_UOM_ID = Utility.Util.GetValueOfInt(dr[3].ToString());//.getInt(4);
                    _C_Currency_ID = Utility.Util.GetValueOfInt(dr[5].ToString());//.getInt(6);
                    _M_Product_Category_ID = Utility.Util.GetValueOfInt(dr[6].ToString());//.getInt(7);
                    _enforcePriceLimit = "Y".Equals(dr[7].ToString());//.getString(8));
                    _isTaxIncluded = "Y".Equals(dr[8].ToString());//.getString(9));
                    //
                    log.Fine("M_PriceList_Version_ID=" + _M_PriceList_Version_ID + " - " + _PriceStd);
                    _calculated = true;
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
                _calculated = false;
            }
            return _calculated;
        }

        /// <summary>
        /// Calculate Price based on Price List
        /// </summary>
        /// <returns>true if calculated</returns>
        private bool CalculatePL()
        {
            String sql = "";
            if (_M_Product_ID == 0)
                return false;

            if (_M_PriceList_ID == 0)
            {
                log.Log(Level.SEVERE, "No PriceList");
                return false;
            }

            //	Get Prices for Price List
            // Check For Advance Pricing Module
            if (Env.IsModuleInstalled("VAPRC_"))
            {
                if (Env.IsModuleInstalled("ED011_"))
                {
                    if (_C_UOM_ID <= 0)
                    {
                        _C_UOM_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_UOM_ID FROM M_Product WHERE  M_Product_ID=" + _M_Product_ID));
                    }
                    sql = "SELECT bomPriceStdUOM(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID , pp.C_UOM_ID) AS PriceStd,"	//	1
                       + " bomPriceListUOM(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID , pp.C_UOM_ID) AS PriceList,"		//	2
                       + " bomPriceLimitUOM(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID , pp.C_UOM_ID) AS PriceLimit,"	//	3
                       + " p.C_UOM_ID,pv.ValidFrom,pl.C_Currency_ID,p.M_Product_Category_ID,pl.EnforcePriceLimit "	// 4..8
                       + "FROM M_Product p"
                       + " INNER JOIN M_ProductPrice pp ON (p.M_Product_ID=pp.M_Product_ID)"
                       + " INNER JOIN  M_PriceList_Version pv ON (pp.M_PriceList_Version_ID=pv.M_PriceList_Version_ID)"
                       + " INNER JOIN M_PriceList pl ON (pv.M_PriceList_ID=pl.M_PriceList_ID) "
                       + " WHERE pv.IsActive='Y'"
                       + " AND p.M_Product_ID=" + _M_Product_ID				//	#1
                       + " AND pv.M_PriceList_ID=" + _M_PriceList_ID			//	#2
                       + " AND pp.M_AttributeSetInstance_ID = " + _M_AttributeSetInstance_ID   //	#3
                       + " AND pp.C_UOM_ID = " + _C_UOM_ID  //    #4
                       + " AND pp.IsActive='Y'"
                       + " ORDER BY pv.ValidFrom DESC";
                }
                else
                {
                    sql = "SELECT bomPriceStdAttr(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID) AS PriceStd,"	//	1
                        + " bomPriceListAttr(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID) AS PriceList,"		//	2
                        + " bomPriceLimitAttr(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID) AS PriceLimit,"	//	3
                        + " p.C_UOM_ID,pv.ValidFrom,pl.C_Currency_ID,p.M_Product_Category_ID,pl.EnforcePriceLimit "	// 4..8
                        + "FROM M_Product p"
                        + " INNER JOIN M_ProductPrice pp ON (p.M_Product_ID=pp.M_Product_ID)"
                        + " INNER JOIN  M_PriceList_Version pv ON (pp.M_PriceList_Version_ID=pv.M_PriceList_Version_ID)"
                        + " INNER JOIN M_PriceList pl ON (pv.M_PriceList_ID=pl.M_PriceList_ID) "
                        + " WHERE pv.IsActive='Y'"
                        + " AND p.M_Product_ID=" + _M_Product_ID				//	#1
                        + " AND pv.M_PriceList_ID=" + _M_PriceList_ID			//	#2
                        + " AND pp.M_AttributeSetInstance_ID = " + _M_AttributeSetInstance_ID   //	#3
                        + " ORDER BY pv.ValidFrom DESC";
                }
            }
            else
            {
                sql = "SELECT bomPriceStd(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceStd,"	//	1
                    + " bomPriceList(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceList,"		//	2
                    + " bomPriceLimit(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceLimit,"  	//	3
                    + " p.C_UOM_ID,pv.ValidFrom,pl.C_Currency_ID,p.M_Product_Category_ID,pl.EnforcePriceLimit "	// 4..8
                    + "FROM M_Product p"
                    + " INNER JOIN M_ProductPrice pp ON (p.M_Product_ID=pp.M_Product_ID)"
                    + " INNER JOIN  M_PriceList_Version pv ON (pp.M_PriceList_Version_ID=pv.M_PriceList_Version_ID)"
                    + " INNER JOIN M_PriceList pl ON (pv.M_PriceList_ID=pl.M_PriceList_ID) "
                    + "WHERE pv.IsActive='Y'"
                    + " AND p.M_Product_ID=" + _M_Product_ID				//	#1
                    + " AND pv.M_PriceList_ID=" + _M_PriceList_ID			//	#2
                    + " ORDER BY pv.ValidFrom DESC";
            }
            _calculated = false;
            if (_PriceDate == null)
                _PriceDate = DateTime.Now;
            IDataReader dr = null;
            try
            {
                dr = ExecuteQuery.ExecuteReader(sql, null);
                while (!_calculated && dr.Read())
                {
                    DateTime? plDate = Utility.Util.GetValueOfDateTime(dr[4]);
                    //	we have the price list
                    //	if order date is after or equal PriceList validFrom
                    if (plDate == null || !(_PriceDate < plDate))
                    {
                        //	Prices
                        _PriceStd = Utility.Util.GetValueOfDecimal(dr[0]);
                        // if (dr.wasNull())
                        if (dr[0] == null)
                            _PriceStd = Env.ZERO;
                        _PriceList = Utility.Util.GetValueOfDecimal(dr[1]);
                        // if (dr.wasNull())
                        if (dr[1] == null)
                            _PriceList = Env.ZERO;
                        _PriceLimit = Utility.Util.GetValueOfDecimal(dr[2]);
                        //if (dr.wasNull())
                        if (dr[2] == null)
                            _PriceLimit = Env.ZERO;
                        //
                        _C_UOM_ID = Utility.Util.GetValueOfInt(dr[3].ToString());
                        _C_Currency_ID = Utility.Util.GetValueOfInt(dr[5].ToString());
                        _M_Product_Category_ID = Utility.Util.GetValueOfInt(dr[6].ToString());
                        _enforcePriceLimit = "Y".Equals(dr[7].ToString());
                        //
                        log.Fine("M_PriceList_ID=" + _M_PriceList_ID + "(" + plDate + ")" + " - " + _PriceStd);
                        _calculated = true;
                        break;
                    }
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
                _calculated = false;
            }
            if (!_calculated)
            {
                log.Finer("Not found (PL)");
            }
            return _calculated;
        }

        /// <summary>
        /// Calculate Price based on Base Price List
        /// </summary>
        /// <returns>true if calculated</returns>
        private bool CalculateBPL()
        {
            String sql = "";
            if (_M_Product_ID == 0 || _M_PriceList_ID == 0)
                return false;
            // Check For Advance Pricing Module
            if (Env.IsModuleInstalled("VAPRC_"))
            {
                if (Env.IsModuleInstalled("ED011_"))
                {
                    if (_C_UOM_ID <= 0)
                    {
                        _C_UOM_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_UOM_ID FROM M_Product WHERE  M_Product_ID=" + _M_Product_ID));
                    }
                    sql = "SELECT bomPriceStdUOM(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID , pp.C_UOM_ID) AS PriceStd,"	//	1
                        + " bomPriceListUOM(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID , pp.C_UOM_ID) AS PriceList,"		//	2
                        + " bomPriceLimitUOM(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID , pp.C_UOM_ID) AS PriceLimit,"	//	3
                        + " p.C_UOM_ID,pv.ValidFrom,pl.C_Currency_ID,p.M_Product_Category_ID,"	//	4..7
                        + " pl.EnforcePriceLimit, pl.IsTaxIncluded "	// 8..9
                        + "FROM M_Product p"
                        + " INNER JOIN M_ProductPrice pp ON (p.M_Product_ID=pp.M_Product_ID)"
                        + " INNER JOIN  M_PriceList_Version pv ON (pp.M_PriceList_Version_ID=pv.M_PriceList_Version_ID)"
                        + " INNER JOIN M_PriceList bpl ON (pv.M_PriceList_ID=bpl.M_PriceList_ID)"
                        + " INNER JOIN M_PriceList pl ON (bpl.M_PriceList_ID=pl.BasePriceList_ID) "
                        + "WHERE pv.IsActive='Y'"
                        + " AND p.M_Product_ID=" + _M_Product_ID				//	#1
                        + " AND pl.M_PriceList_ID= " + _M_PriceList_ID			//	#2
                        + " AND pp.M_AttributeSetInstance_ID = " + _M_AttributeSetInstance_ID   //	#3
                        + " AND pp.C_UOM_ID = " + _C_UOM_ID  //    #4
                        + " AND pp.IsActive='Y'"
                        + "ORDER BY pv.ValidFrom DESC";
                }
                else
                {
                    sql = "SELECT bomPriceStdAttr(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID) AS PriceStd,"	//	1
                        + " bomPriceListAttr(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID) AS PriceList,"		//	2
                        + " bomPriceLimitAttr(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID) AS PriceLimit,"	//	3
                        + " p.C_UOM_ID,pv.ValidFrom,pl.C_Currency_ID,p.M_Product_Category_ID,"	//	4..7
                        + " pl.EnforcePriceLimit, pl.IsTaxIncluded "	// 8..9
                        + "FROM M_Product p"
                        + " INNER JOIN M_ProductPrice pp ON (p.M_Product_ID=pp.M_Product_ID)"
                        + " INNER JOIN  M_PriceList_Version pv ON (pp.M_PriceList_Version_ID=pv.M_PriceList_Version_ID)"
                        + " INNER JOIN M_PriceList bpl ON (pv.M_PriceList_ID=bpl.M_PriceList_ID)"
                        + " INNER JOIN M_PriceList pl ON (bpl.M_PriceList_ID=pl.BasePriceList_ID) "
                        + "WHERE pv.IsActive='Y'"
                        + " AND p.M_Product_ID=" + _M_Product_ID				//	#1
                        + " AND pl.M_PriceList_ID= " + _M_PriceList_ID			//	#2
                        + " AND pp.M_AttributeSetInstance_ID = " + _M_AttributeSetInstance_ID   //	#3
                        + "ORDER BY pv.ValidFrom DESC";
                }
            }
            else
            {

                sql = "SELECT bomPriceStd(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceStd,"	//	1
                    + " bomPriceList(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceList,"		//	2
                    + " bomPriceLimit(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceLimit,"	//	3
                    + " p.C_UOM_ID,pv.ValidFrom,pl.C_Currency_ID,p.M_Product_Category_ID,"	//	4..7
                    + " pl.EnforcePriceLimit, pl.IsTaxIncluded "	// 8..9
                    + "FROM M_Product p"
                    + " INNER JOIN M_ProductPrice pp ON (p.M_Product_ID=pp.M_Product_ID)"
                    + " INNER JOIN  M_PriceList_Version pv ON (pp.M_PriceList_Version_ID=pv.M_PriceList_Version_ID)"
                    + " INNER JOIN M_PriceList bpl ON (pv.M_PriceList_ID=bpl.M_PriceList_ID)"
                    + " INNER JOIN M_PriceList pl ON (bpl.M_PriceList_ID=pl.BasePriceList_ID) "
                    + "WHERE pv.IsActive='Y'"
                    + " AND p.M_Product_ID=" + _M_Product_ID				//	#1
                    + " AND pl.M_PriceList_ID= " + _M_PriceList_ID			//	#2
                    + "ORDER BY pv.ValidFrom DESC";
            }
            _calculated = false;
            if (_PriceDate == null)
                _PriceDate = DateTime.Now;
            IDataReader dr = null;
            try
            {
                dr = ExecuteQuery.ExecuteReader(sql, null);
                while (!_calculated && dr.Read())
                {
                    DateTime? plDate = Utility.Util.GetValueOfDateTime(dr[4]);
                    //	we have the price list
                    //	if order date is after or equal PriceList validFrom
                    if (plDate == null || !(_PriceDate < plDate))
                    {
                        //	Prices
                        _PriceStd = Utility.Util.GetValueOfDecimal(dr[0]);
                        //if (dr.wasNull())
                        if (dr[0] == null)
                            _PriceStd = Env.ZERO;
                        _PriceList = Utility.Util.GetValueOfDecimal(dr[1]);
                        //if (dr.wasNull())
                        if (dr[1] == null)
                            _PriceList = Env.ZERO;
                        _PriceLimit = Utility.Util.GetValueOfDecimal(dr[2]);
                        //if (dr.wasNull())
                        if (dr[2] == null)
                            _PriceLimit = Env.ZERO;
                        //
                        _C_UOM_ID = Utility.Util.GetValueOfInt(dr[3].ToString());
                        _C_Currency_ID = Utility.Util.GetValueOfInt(dr[5].ToString());
                        _M_Product_Category_ID = Utility.Util.GetValueOfInt(dr[6].ToString());
                        _enforcePriceLimit = "Y".Equals(dr[7].ToString());
                        _isTaxIncluded = "Y".Equals(dr[8].ToString());
                        //
                        log.Fine("M_PriceList_ID=" + _M_PriceList_ID + "(" + plDate + ")" + " - " + _PriceStd);
                        _calculated = true;
                        break;
                    }
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
                _calculated = false;
            }
            if (!_calculated)
            {
                log.Finer("Not found (BPL)");
            }
            return _calculated;
        }

        /// <summary>
        /// Set Base Info (UOM)
        /// </summary>
        private void SetBaseInfo()
        {
            if (_M_Product_ID == 0)
                return;
            //
            String sql = "SELECT C_UOM_ID, M_Product_Category_ID FROM M_Product WHERE M_Product_ID=" + _M_Product_ID;
            IDataReader dr = null;
            try
            {
                dr = ExecuteQuery.ExecuteReader(sql, null);
                if (dr.Read())
                {
                    _C_UOM_ID = Utility.Util.GetValueOfInt(dr[0]);//.getInt(1);
                    _M_Product_Category_ID = Utility.Util.GetValueOfInt(dr[1]);//.getInt(2);
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }

        }

        /// <summary>
        /// Is Tax Included
        /// </summary>
        /// <returns>tax included</returns>
        public bool IsTaxIncluded()
        {
            return _isTaxIncluded;
        }

        /// <summary>
        /// Calculate (Business Partner) Discount
        /// </summary>
        private void CalculateDiscount()
        {
            try
            {
                _discountSchema = false;
                if (_C_BPartner_ID == 0 || _M_Product_ID == 0)
                    return;

                int M_DiscountSchema_ID = 0;
                Decimal? FlatDiscount = null;
                String sql = "SELECT COALESCE(p.M_DiscountSchema_ID,g.M_DiscountSchema_ID),"
                    + " COALESCE(p.PO_DiscountSchema_ID,g.PO_DiscountSchema_ID), p.FlatDiscount "
                    + "FROM C_BPartner p"
                    + " INNER JOIN C_BP_Group g ON (p.C_BP_Group_ID=g.C_BP_Group_ID) "
                    + "WHERE p.C_BPartner_ID=" + _C_BPartner_ID;
                DataTable dt = null;
                IDataReader idr = null;
                try
                {
                    idr = DataBase.DB.ExecuteReader(sql, null, null);
                    dt = new DataTable();
                    dt.Load(idr);
                    idr.Close();
                    //if (dr.Read())
                    foreach (DataRow dr in dt.Rows)
                    {
                        M_DiscountSchema_ID = Utility.Util.GetValueOfInt(dr[_isSOTrx ? 0 : 1]);
                        if (dr[2] == DBNull.Value)
                        {
                            FlatDiscount = Env.ZERO;
                        }
                        else
                        {
                            FlatDiscount = Utility.Util.GetValueOfDecimal(dr[2]);//.getBigDecimal(3);
                        }
                        if (FlatDiscount == null)
                        {
                            FlatDiscount = Env.ZERO;
                        }
                    }

                }
                catch (Exception e)
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    log.Log(Level.SEVERE, sql, e);
                }
                finally
                {
                    dt = null;
                    if (idr != null)
                    {
                        idr.Close();
                    }
                }
                //	No Discount Schema
                if (M_DiscountSchema_ID == 0)
                    return;

                MDiscountSchema sd = MDiscountSchema.Get(Env.GetContext(), M_DiscountSchema_ID);	//	not correct
                if (sd.Get_ID() == 0)
                    return;
                //
                _discountSchema = true;
                _PriceStd = sd.CalculatePrice(_qty, _PriceStd, _M_Product_ID, _M_Product_Category_ID, (decimal)FlatDiscount);
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MProductPricing--CalculateDiscount");
                log.Severe(ex.ToString());
            }

        }

        /// <summary>
        /// Calculate Discount Percentage based on Standard/List Price
        /// </summary>
        /// <returns>Discount</returns>
        public Decimal GetDiscount()
        {
            Decimal Discount = Env.ZERO;
            if (_PriceList != 0)
            {
                Discount = new Decimal((Decimal.ToDouble(_PriceList) - Decimal.ToDouble(_PriceStd)) / Decimal.ToDouble(_PriceList) * 100.0);
            }
            if (Env.Scale(Discount) > 2)
                Discount = Decimal.Round(Discount, 2);//, MidpointRounding.AwayFromZero);
            return Discount;
        }

        /// <summary>
        /// Get Line Amt
        /// </summary>
        /// <param name="currencyPrecision">precision -1 = ignore</param>
        /// <returns>Standard Price * Qty</returns>
        public Decimal GetLineAmt(int currencyPrecision)
        {
            Decimal amt = Decimal.Multiply(GetPriceStd(), _qty);
            //	Currency Precision
            if (currencyPrecision >= 0 && Env.Scale(amt) > currencyPrecision)
            {
                amt = Decimal.Round(amt, currencyPrecision);//, MidpointRounding.AwayFromZero);
            }
            return amt;
        }

        /// <summary>
        /// Get Product ID
        /// </summary>
        /// <returns>id</returns>
        public int GetM_Product_ID()
        {
            return _M_Product_ID;
        }

        /// <summary>
        /// Get PriceList ID
        /// </summary>
        /// <returns>pl</returns>
        public int GetM_PriceList_ID()
        {
            return _M_PriceList_ID;
        }

        /// <summary>
        /// Set PriceList
        /// </summary>
        /// <param name="M_PriceList_ID">pl</param>
        public void SetM_PriceList_ID(int M_PriceList_ID)
        {
            _M_PriceList_ID = M_PriceList_ID;
            _calculated = false;
        }

        /// <summary>
        /// Get Attribute Set Instance
        /// </summary>
        /// <returns>plv</returns>
        public int GetM_AttributeSetInstance_ID()
        {
            return _M_AttributeSetInstance_ID;
        }

        /// <summary>
        /// Set Attribute Set Instance
        /// </summary>
        /// <param name="M_AttributeSetInstance_ID">plv</param>
        public void SetM_AttributeSetInstance_ID(int M_AttributeSetInstance_ID)
        {
            _M_AttributeSetInstance_ID = M_AttributeSetInstance_ID;
            _calculated = false;
        }

        /// <summary>
        /// Get PriceList Version
        /// </summary>
        /// <returns>plv</returns>
        public int GetM_PriceList_Version_ID()
        {
            return _M_PriceList_Version_ID;
        }

        /// <summary>
        /// Set PriceList Version
        /// </summary>
        /// <param name="M_PriceList_Version_ID">plv</param>
        public void SetM_PriceList_Version_ID(int M_PriceList_Version_ID)
        {
            _M_PriceList_Version_ID = M_PriceList_Version_ID;
            _calculated = false;
        }

        /// <summary>
        /// Get Price Date
        /// </summary>
        /// <returns>date</returns>
        public DateTime? GetPriceDate()
        {
            return _PriceDate;
        }

        /// <summary>
        /// Set Price Date
        /// </summary>
        /// <param name="priceDate">date</param>
        public void SetPriceDate(DateTime? priceDate)
        {
            _PriceDate = (DateTime?)priceDate;
            _calculated = false;
        }

        /// <summary>
        /// Set Price Date
        /// </summary>
        /// <param name="priceTime">priceTime date</param>
        public void SetPriceDate(long priceTime)
        {
            SetPriceDate(Convert.ToDateTime(priceTime));
        }

        public void SetPriceDate1(DateTime? priceTime)
        {
            SetPriceDate(priceTime);
        }
        /// <summary>
        /// Set Price List Precision.
        /// </summary>
        private void SetPrecision()
        {
            if (_M_PriceList_ID != 0)
                _precision = MPriceList.GetPricePrecision(Env.GetContext(), GetM_PriceList_ID());
        }

        /// <summary>
        /// Get Price List Precision
        /// </summary>
        /// <returns>precision - -1 = no rounding</returns>
        public int GetPrecision()
        {
            return _precision;
        }

        /// <summary>
        /// Round
        /// </summary>
        /// <param name="bd">number</param>
        /// <returns>rounded number</returns>
        private Decimal Round(Decimal bd)
        {
            if (_precision >= 0	//	-1 = no rounding
                && Env.Scale(bd) > _precision)
            {
                return Decimal.Round(bd, _precision, MidpointRounding.AwayFromZero);
            }
            return bd;
        }

        /// <summary>
        /// Get C_UOM_ID
        /// </summary>
        /// <returns>uom</returns>
        public int GetC_UOM_ID()
        {
            if (!_calculated)
                CalculatePrice();
            return _C_UOM_ID;
        }


        /// <summary>
        /// Set C_UOM_ID
        /// </summary>
        /// <returns></returns>
        public void SetC_UOM_ID(int C_UOM_ID)
        {
            _C_UOM_ID = C_UOM_ID;
            _calculated = false;
        }


        /// <summary>
        /// Get Price List
        /// </summary>
        /// <returns>list</returns>
        public Decimal GetPriceList()
        {
            if (!_calculated)
                CalculatePrice();
            return Round(_PriceList);
        }

        /// <summary>
        /// Get Price Std
        /// </summary>
        /// <returns>std</returns>
        public Decimal GetPriceStd()
        {
            if (!_calculated)
                CalculatePrice();
            return Round(_PriceStd);
        }

        /// <summary>
        /// Get Price Limit
        /// </summary>
        /// <returns>limit</returns>
        public Decimal GetPriceLimit()
        {
            if (!_calculated)
                CalculatePrice();
            return Round(_PriceLimit);
        }

        /// <summary>
        /// Get Price List Currency
        /// </summary>
        /// <returns>currency</returns>
        public int GetC_Currency_ID()
        {
            if (!_calculated)
                CalculatePrice();
            return _C_Currency_ID;
        }

        /// <summary>
        /// Is Price List enforded?
        /// </summary>
        /// <returns>enforce limit</returns>
        public bool IsEnforcePriceLimit()
        {
            if (!_calculated)
                CalculatePrice();
            return _enforcePriceLimit;
        }

        /// <summary>
        /// Is a DiscountSchema active?
        /// </summary>
        /// <returns>active Discount Schema</returns>
        public bool IsDiscountSchema()
        {
            return _discountSchema;
        }

        /// <summary>
        /// Is the Price Calculated (i.e. found)?
        /// </summary>
        /// <returns>calculated</returns>
        public bool IsCalculated()
        {
            return _calculated;
        }
    }
}
