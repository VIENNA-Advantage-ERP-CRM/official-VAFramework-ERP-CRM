/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_OrderLine
 * Chronological Development
 * Veena Pandey     19-May-2009
 * raghunandan      10-June-2009 (adding new functions in class)
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
using VAdvantage.Login;
using VAdvantage.Model;
using VAdvantage.WF;
//using VAdvantage.Grid;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    /// <summary>
    /// Order Line model.
    /// </summary>
    public class MOrderLine : X_C_OrderLine
    {
        #region Private variables
        private int _M_PriceList_ID = 0;
        private bool _IsSOTrx = true;
        private bool _IsReturnTrx = true;
        private static VLogger _log = VLogger.GetVLogger(typeof(MOrderLine).FullName);
        //	Product Pricing
        private MProductPricing _productPrice = null;
        //Cached Currency Precision	
        private int? _precision = null;
        //	Product					
        private MProduct _product = null;
        //Parent					
        private MOrder _parent = null;

        private int I_Order_ID = 0;

        private bool _fromProcess = false;

        /** is Closed Document*/
        private bool isClosed = false;

        private bool resetAmtDim = false;
        private bool resetTotalAmtDim = false;
        #endregion

        /// <summary>
        /// Get Order Unreserved Qty
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Warehouse_ID">wh</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">asi</param>
        /// <param name="excludeC_OrderLine_ID">exclude C_OrderLine_ID</param>
        /// <returns>Unreserved Qty</returns>
        public static Decimal GetNotReserved(Ctx ctx, int M_Warehouse_ID,
            int M_Product_ID, int M_AttributeSetInstance_ID, int excludeC_OrderLine_ID)
        {
            Decimal retValue = Env.ZERO;
            String sql = "SELECT SUM(qtyOrdered-QtyDelivered-QtyReserved) "
                + "FROM C_OrderLine ol"
                + " INNER JOIN C_Order o ON (ol.C_Order_ID=o.C_Order_ID) "
                + "WHERE ol.M_Warehouse_ID=" + M_Warehouse_ID	//	#1
                + " AND M_Product_ID=" + M_Product_ID			//	#2
                + " AND o.IsSOTrx='Y' AND o.DocStatus='DR'"
                + " AND qtyOrdered-QtyDelivered-QtyReserved<>0"
                + " AND ol.C_OrderLine_ID<>" + excludeC_OrderLine_ID;
            if (M_AttributeSetInstance_ID != 0)
                sql += " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;

            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                while (idr.Read())
                {
                    retValue = Utility.Util.GetValueOfDecimal(idr[0]);
                }
                if (idr != null)
                    idr.Close();
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                    idr.Close();
            }
            if (retValue == null)
            {
                _log.Fine("-");
            }
            else
            {
                _log.Fine(retValue.ToString());
            }
            return retValue;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_OrderLine_ID">order line to load</param>
        /// <param name="trxName">transction</param>
        public MOrderLine(Ctx ctx, int C_OrderLine_ID, Trx trxName)
            : base(ctx, C_OrderLine_ID, trxName)
        {
            if (C_OrderLine_ID == 0)
            {
                //	setC_Order_ID (0);
                //	setLine (0);
                //	setM_Warehouse_ID (0);	// @M_Warehouse_ID@
                //	setC_BPartner_ID(0);
                //	setC_BPartner_Location_ID (0);	// @C_BPartner_Location_ID@
                //	setC_Currency_ID (0);	// @C_Currency_ID@
                //	setDateOrdered (new Timestamp(System.currentTimeMillis()));	// @DateOrdered@
                //
                //	setC_Tax_ID (0);
                //	setC_UOM_ID (0);
                //
                SetFreightAmt(Env.ZERO);
                SetLineNetAmt(Env.ZERO);
                //
                SetPriceEntered(Env.ZERO);
                SetPriceActual(Env.ZERO);
                SetPriceLimit(Env.ZERO);
                SetPriceList(Env.ZERO);
                //
                SetM_AttributeSetInstance_ID(0);
                //
                SetQtyEntered(Env.ZERO);
                SetQtyOrdered(Env.ZERO);	// 1
                SetQtyDelivered(Env.ZERO);
                SetQtyInvoiced(Env.ZERO);
                SetQtyReserved(Env.ZERO);
                //
                SetIsDescription(false);	// N
                SetProcessed(false);
                SetLine(0);
            }
        }

        /// <summary>
        /// Parent Constructor.
        /// ol.setM_Product_ID(wbl.getM_Product_ID());
        /// ol.setQtyOrdered(wbl.getQuantity());
        /// ol.setPrice();
        /// ol.setPriceActual(wbl.getPrice());
        /// ol.setTax();
        /// ol.save();
        /// </summary>
        /// <param name="order">parent order</param>
        public MOrderLine(MOrder order)
            : this(order.GetCtx(), 0, order.Get_TrxName())
        {
            if (order.Get_ID() == 0)
                throw new ArgumentException("Header not saved");
            SetC_Order_ID(order.GetC_Order_ID());	//	parent
            SetOrder(order);
        }

        public MOrderLine(MOrder order, int p_I_Order_ID)
            : this(order.GetCtx(), 0, order.Get_TrxName())
        {
            if (order.GetC_Order_ID() != 0)
            {
                SetC_Order_ID(order.GetC_Order_ID());	//	parent
            }
            SetOrder(order);
            I_Order_ID = p_I_Order_ID;
        }

        public int GetI_Order_ID()
        {
            return I_Order_ID;
        }

        public MOrderLine GetRef_OrderLine()
        {
            String sql = "SELECT C_OrderLine_ID FROM C_OrderLine WHERE Ref_OrderLine_ID=@param1";
            MOrderLine line = null;

            int ii = DB.GetSQLValue(Get_TrxName(), sql, GetC_OrderLine_ID());
            if (ii > 0)
            {
                line = new MOrderLine(GetCtx(), ii, Get_TrxName());
            }
            return line;
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">set record</param>
        /// <param name="trxName">transaction</param>
        public MOrderLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        public MOrderLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Set Defaults from Order.
        /// Does not set Parent !!
        /// </summary>
        /// <param name="order">order</param>
        public void SetOrder(MOrder order)
        {
            SetClientOrg(order);
            SetC_BPartner_ID(order.GetC_BPartner_ID());
            SetC_BPartner_Location_ID(order.GetC_BPartner_Location_ID());
            SetM_Warehouse_ID(order.GetM_Warehouse_ID());
            SetDateOrdered(order.GetDateOrdered());
            SetDatePromised(order.GetDatePromised());
            SetC_Currency_ID(order.GetC_Currency_ID());
            SetHeaderInfo(order);	//	sets m_order
            //	Don't set Activity, etc as they are overwrites
        }

        /// <summary>
        /// Set Header Info
        /// </summary>
        /// <param name="order">order</param>
        public void SetHeaderInfo(MOrder order)
        {
            _parent = order;
            _precision = order.GetPrecision();
            _M_PriceList_ID = order.GetM_PriceList_ID();
            _IsSOTrx = order.IsSOTrx();
        }

        /// <summary>
        /// Get Parent
        /// </summary>
        /// <returns>parent</returns>
        public MOrder GetParent()
        {
            if (_parent == null)
                _parent = new MOrder(GetCtx(), GetC_Order_ID(), Get_TrxName());
            return _parent;
        }

        /// <summary>
        /// Set Price Entered/Actual.
        /// Use this Method if the Line UOM is the Product UOM 
        /// </summary>
        /// <param name="priceActual">price</param>
        public void SetPrice(Decimal priceActual)
        {
            SetPriceEntered(priceActual);
            SetPriceActual(priceActual);
        }

        /// <summary>
        /// Set Price for Product and PriceList.
        /// Use only if newly created.
        /// Uses standard price list of not set by order constructor
        /// </summary>
        public void SetPrice()
        {
            if (GetM_Product_ID() == 0)
                return;
            if (_M_PriceList_ID == 0)
                throw new Exception("PriceList unknown!");
            SetPrice(_M_PriceList_ID);
        }

        /// <summary>
        /// Set Price for Product and PriceList
        /// </summary>
        /// <param name="M_PriceList_ID">price list</param>
        public void SetPrice(int M_PriceList_ID)
        {
            if (GetM_Product_ID() == 0)
                return;
            log.Fine(ToString() + " - M_PriceList_ID=" + M_PriceList_ID);
            GetProductPricing(M_PriceList_ID);
            SetPriceActual(_productPrice.GetPriceStd());
            SetPriceList(_productPrice.GetPriceList());
            SetPriceLimit(_productPrice.GetPriceLimit());
            // Set price entered by rakesh kumar on 17/Mar/2021
            SetPriceEntered(GetPriceActual());

            // Commented by rakesh kumar on 17/Mar/2021 price handled in product pricing
            //if (GetQtyEntered().CompareTo(GetQtyOrdered()) == 0)
            //{
            //    SetPriceEntered(GetPriceActual());
            //}
            //else
            //{
            //    //SetPriceEntered(GetPriceActual().multiply(getQtyOrdered().divide(getQtyEntered(), 12, BigDecimal.ROUND_HALF_UP)));	//	recision
            //    //SetPriceEntered(Decimal.Multiply(GetPriceActual(), Decimal.Divide(GetQtyOrdered(), Decimal.Round(GetQtyEntered(), 12, MidpointRounding.AwayFromZero))));
            //}
            //	Calculate Discount
            SetDiscount(_productPrice.GetDiscount());

            //	Set UOM
            // gwu: only set UOM if not already set
            if (GetC_UOM_ID() == 0)
                SetC_UOM_ID(_productPrice.GetC_UOM_ID());
        }

        /// <summary>
        /// Get and calculate Product Pricing
        /// </summary>
        /// <param name="M_PriceList_ID">id</param>
        /// <returns>product pricing</returns>
        private MProductPricing GetProductPricing(int M_PriceList_ID)
        {
            _productPrice = new MProductPricing(GetAD_Client_ID(), GetAD_Org_ID(),
                GetM_Product_ID(), GetC_BPartner_ID(), GetQtyOrdered(), _IsSOTrx);
            _productPrice.SetM_PriceList_ID(M_PriceList_ID);
            //Amit 24-nov-2014
            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='ED011_'")) > 0)
            {
                _productPrice.SetC_UOM_ID(GetC_UOM_ID());
            }
            //Amit
            _productPrice.SetPriceDate(GetDateOrdered());
            _productPrice.SetC_UOM_ID(GetC_UOM_ID());
            //Set AttributeInstanceId by rakesh kumar 17/Mar/2021
            _productPrice.SetM_AttributeSetInstance_ID(GetM_AttributeSetInstance_ID());
            _productPrice.CalculatePrice();
            return _productPrice;
        }

        /// <summary>
        /// Get Purchase Price if exists
        /// </summary>
        /// <returns>limit</returns>
        public Decimal GetPurchasePrice(int M_PriceList_ID)
        {
            //VA077_PurchasePrice
            Decimal purchasePrice = 0;
            if (GetM_Product_ID() == 0)
                return purchasePrice;
            bool isSOTrx = false;
            MProduct prd = new MProduct(GetCtx(), GetM_Product_ID(), null);
            MProductPricing pp = new MProductPricing(GetAD_Client_ID(), GetAD_Org_ID(),
                GetM_Product_ID(), _parent.GetC_BPartner_ID(), GetQtyEntered(), isSOTrx);
            pp.SetM_PriceList_ID(M_PriceList_ID);

            DateTime? validFrom = GetDateOrdered();
            if (validFrom == null)
            {
                validFrom = DateTime.Now.Date;
            }

            int M_PriceList_Version_ID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT M_PriceList_Version_ID FROM M_PriceList_Version WHERE IsActive = 'Y' 
                        AND M_PriceList_ID = " + Util.GetValueOfInt(_parent.Get_Value("PO_PriceList_ID")) + @" AND ValidFrom <= "
                        + DB.TO_DATE(validFrom, true) + " ORDER BY ValidFrom DESC"));

            pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
            pp.SetM_AttributeSetInstance_ID(GetM_AttributeSetInstance_ID());
            pp.SetC_UOM_ID(GetC_UOM_ID());

            if (pp.CalculatePrice())
                purchasePrice = pp.GetPriceStd();
            return purchasePrice;
        }

        /// <summary>
        /// Set Tax
        /// </summary>
        /// <returns>true if tax is set</returns>        
        public bool SetTax()
        {
            // Change to Set Tax ID based on the VAT Engine Module
            // if (_IsSOTrx)
            // {           
            DataSet dsLoc = null;
            MOrder inv = new MOrder(Env.GetCtx(), Util.GetValueOfInt(Get_Value("C_Order_ID")), Get_TrxName());
            //11-nov-2014 Amit
            string taxRule = string.Empty;

            string sql = "SELECT VATAX_TaxRule FROM AD_OrgInfo WHERE AD_Org_ID=" + inv.GetAD_Org_ID() + " AND IsActive ='Y' AND AD_Client_ID =" + GetCtx().GetAD_Client_ID();
            if (Env.IsModuleInstalled("VATAX_"))
            {
                taxRule = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            // if (taxRule == "T" && _IsSOTrx)
            // if (taxRule == "T" && ((_IsSOTrx && !_IsReturnTrx) || (!_IsSOTrx && !_IsReturnTrx)))
            if (taxRule == "T")
            {
                int c_tax_ID = 0;
                int taxCategory = 0;
                MBPartner bp = new MBPartner(GetCtx(), inv.GetC_BPartner_ID(), Get_TrxName());
                if (bp.IsTaxExempt())
                {
                    c_tax_ID = GetExemptTax(GetCtx(), GetAD_Org_ID());
                    SetC_Tax_ID(c_tax_ID);
                    return true;
                }
                if (GetM_Product_ID() > 0)
                {
                    MProduct prod = new MProduct(Env.GetCtx(), GetM_Product_ID(), Get_TrxName());
                    taxCategory = Util.GetValueOfInt(prod.GetC_TaxCategory_ID());
                }
                if (GetC_Charge_ID() > 0)
                {
                    MCharge chrg = new MCharge(Env.GetCtx(), GetC_Charge_ID(), Get_TrxName());
                    taxCategory = Util.GetValueOfInt(chrg.GetC_TaxCategory_ID());
                }
                if (taxCategory > 0)
                {
                    MTaxCategory taxCat = new MTaxCategory(GetCtx(), taxCategory, Get_TrxName());
                    int Country_ID = 0, Region_ID = 0, orgCountry = 0, orgRegion = 0, taxRegion = 0;
                    string Postal = "", orgPostal = "";

                    if (taxCat.GetVATAX_Location() == "I")
                    {
                        sql = @"SELECT loc.C_Country_ID,loc.C_Region_ID,loc.Postal FROM C_Location loc INNER JOIN C_BPartner_Location bpl ON loc.C_Location_ID = bpl.C_Location_ID WHERE bpl.C_BPartner_Location_ID ="
                            + inv.GetBill_Location_ID() + " AND bpl.IsActive = 'Y'";
                    }
                    else
                    {
                        sql = @"SELECT loc.C_Country_ID,loc.C_Region_ID,loc.Postal FROM C_Location loc INNER JOIN C_BPartner_Location bpl ON loc.C_Location_ID = bpl.C_Location_ID WHERE bpl.C_BPartner_Location_ID ="
                            + inv.GetC_BPartner_Location_ID() + " AND bpl.IsActive = 'Y'";
                    }
                    dsLoc = DB.ExecuteDataset(sql, null, Get_TrxName());
                    if (dsLoc != null)
                    {
                        if (dsLoc.Tables[0].Rows.Count > 0)
                        {
                            for (int j = 0; j < dsLoc.Tables[0].Rows.Count; j++)
                            {
                                Country_ID = Util.GetValueOfInt(dsLoc.Tables[0].Rows[j][0]);
                                Region_ID = Util.GetValueOfInt(dsLoc.Tables[0].Rows[j][1]);
                                Postal = Util.GetValueOfString(dsLoc.Tables[0].Rows[j][2]);
                            }
                        }
                    }
                    dsLoc = null;
                    sql = @"SELECT loc.C_Country_ID,loc.C_Region_ID,loc.Postal FROM C_Location loc LEFT JOIN AD_OrgInfo org ON loc.C_Location_ID = org.C_Location_ID WHERE org.AD_Org_ID ="
                            + inv.GetAD_Org_ID() + " AND org.IsActive = 'Y'";
                    dsLoc = DB.ExecuteDataset(sql, null, Get_TrxName());
                    if (dsLoc != null)
                    {
                        if (dsLoc.Tables[0].Rows.Count > 0)
                        {
                            for (int j = 0; j < dsLoc.Tables[0].Rows.Count; j++)
                            {
                                orgCountry = Util.GetValueOfInt(dsLoc.Tables[0].Rows[j][0]);
                                orgRegion = Util.GetValueOfInt(dsLoc.Tables[0].Rows[j][1]);
                                orgPostal = Util.GetValueOfString(dsLoc.Tables[0].Rows[j][2]);
                            }
                        }
                    }

                    for (int i = 0; i < 3; i++)
                    {
                        string pref = taxCat.GetVATAX_Preference1();
                        if (i == 1)
                        {
                            pref = taxCat.GetVATAX_Preference2();
                        }
                        else if (i == 2)
                        {
                            pref = taxCat.GetVATAX_Preference3();
                        }

                        // if Tax Preference is Tax Class
                        if (pref == "T")
                        {
                            sql = @"SELECT VATAX_TaxType_ID FROM C_BPartner_Location WHERE C_BPartner_ID =" + inv.GetC_BPartner_ID() +
                                           " AND IsActive = 'Y'  AND C_BPartner_Location_ID = " + inv.GetC_BPartner_Location_ID();
                            int taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                            if (taxType == 0)
                            {
                                sql = @"SELECT VATAX_TaxType_ID FROM C_BPartner WHERE C_BPartner_ID =" + inv.GetC_BPartner_ID() + " AND IsActive = 'Y'";
                                taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                            }
                            if (taxType > 0)
                            {
                                sql = "SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID  WHERE tcr.C_TaxCategory_ID = " + taxCategory +
                                    " AND tcr.IsActive ='Y' AND tcr.VATAX_TaxBase = 'T' AND tcr.VATAX_TaxType_ID =" + taxType + " AND tx.SOPOType IN ('B','" + (inv.IsSOTrx() ? 'S' : 'P') + "')";
                                c_tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                                if (c_tax_ID > 0)
                                {
                                    SetC_Tax_ID(c_tax_ID);
                                    return true;
                                }
                            }
                        }
                        // if Tax Preference is Location
                        else if (pref == "L")
                        {
                            c_tax_ID = GetTaxFromLocation(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                            if (c_tax_ID > 0)
                            {
                                SetC_Tax_ID(c_tax_ID);
                                return true;
                            }
                        }
                        // if Tax Preference is Tax Region
                        else if (pref == "R")
                        {
                            if (Country_ID > 0)
                            {
                                dsLoc = null;
                                sql = @"SELECT VATAX_TaxRegion_ID FROM VATAX_TaxCatRate  WHERE C_TaxCategory_ID = " + taxCategory +
                                    " AND VATAX_TaxBase = 'R' AND VATAX_DiffCountry = 'Y' AND IsActive = 'Y' AND C_Country_ID = " + Country_ID;
                                dsLoc = DB.ExecuteDataset(sql, null, Get_TrxName());
                                if (dsLoc != null)
                                {
                                    if (dsLoc.Tables[0].Rows.Count > 0)
                                    {

                                    }
                                    else
                                    {
                                        c_tax_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                        if (c_tax_ID > 0)
                                        {
                                            SetC_Tax_ID(c_tax_ID);
                                            return true;
                                        }
                                    }
                                }
                                else
                                {
                                    c_tax_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                    if (c_tax_ID > 0)
                                    {
                                        SetC_Tax_ID(c_tax_ID);
                                        return true;
                                    }
                                }
                            }
                            else
                            {
                                c_tax_ID = GetTaxFromRegion(inv.IsSOTrx(), taxCategory, Country_ID, Region_ID, Postal);
                                if (c_tax_ID > 0)
                                {
                                    SetC_Tax_ID(c_tax_ID);
                                    return true;
                                }
                            }
                        }

                        // if Tax Preference is Document Type
                        else if (pref == "D")
                        {
                            sql = @"SELECT VATAX_TaxType_ID FROM C_DocType WHERE C_DocType_ID = " + inv.GetC_DocTypeTarget_ID();
                            int taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));

                            if (taxType > 0)
                            {
                                sql = "SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID  WHERE tcr.C_TaxCategory_ID = " + taxCategory +
                                    " AND tcr.IsActive ='Y' AND tcr.VATAX_TaxBase = 'T' AND tcr.VATAX_TaxType_ID = " + taxType + " AND tx.SOPOType IN ('B','" + (inv.IsSOTrx() ? 'S' : 'P') + "')";
                                c_tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                                if (c_tax_ID > 0)
                                {
                                    SetC_Tax_ID(c_tax_ID);
                                    return true;
                                }
                            }
                        }
                    }
                    if (taxCat.GetVATAX_Preference1() == "R" || taxCat.GetVATAX_Preference2() == "R" || taxCat.GetVATAX_Preference3() == "R")
                    {
                        sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxRegion tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.IsDefault = 'Y' AND tcr.IsActive = 'Y' 
                                AND tx.SOPOType IN ('B','" + (inv.IsSOTrx() ? 'S' : 'P') + "') ORDER BY tcr.Updated";
                        c_tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                        if (c_tax_ID > 0)
                        {
                            SetC_Tax_ID(c_tax_ID);
                            return true;
                        }
                    }
                    sql = @"SELECT tcr.C_Tax_ID FROM C_TaxCategory tcr WHERE tcr.C_TaxCategory_ID =" + taxCategory + " AND tcr.IsActive = 'Y'";
                    c_tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                    SetC_Tax_ID(c_tax_ID);
                    return true;
                }
                return false;
                //else
                //{
                //    sql = @"SELECT VATAX_TaxType_ID FROM C_BPartner_Location WHERE C_BPartner_ID =" + inv.GetC_BPartner_ID() +
                //                   " AND IsActive = 'Y'  AND C_BPartner_Location_ID = " + inv.GetC_BPartner_Location_ID();
                //    int taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                //    if (taxType == 0)
                //    {
                //        sql = @"SELECT VATAX_TaxType_ID FROM C_BPartner WHERE C_BPartner_ID =" + inv.GetC_BPartner_ID() + " AND IsActive = 'Y'";
                //        taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                //        #region If Business Partner Is Proposed then it'll select default Tax.   Done By Manjot
                //        if (taxType == 0)
                //        {
                //            int ii = Tax.Get(GetCtx(), GetM_Product_ID(), GetC_Charge_ID(),
                //                         GetDateOrdered(), GetDateOrdered(),
                //                         GetAD_Org_ID(), GetM_Warehouse_ID(),
                //                         GetC_BPartner_Location_ID(),  // should be bill to
                //                         GetC_BPartner_Location_ID(), _IsSOTrx);
                //            if (ii == 0)
                //            {
                //                log.Log(Level.SEVERE, "No Tax found");
                //                return false;
                //            }
                //            SetC_Tax_ID(ii);
                //            return true;
                //        }
                //        #endregion
                //    }
                //    MProduct prod = new MProduct(Env.GetCtx(), System.Convert.ToInt32(GetM_Product_ID()), Get_TrxName());
                //    sql = "SELECT C_Tax_ID FROM VATAX_TaxCatRate WHERE C_TaxCategory_ID = " + prod.GetC_TaxCategory_ID() + " AND IsActive ='Y' AND VATAX_TaxType_ID =" + taxType;
                //    int taxId = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                //    if (taxId > 0)
                //    {
                //        SetC_Tax_ID(taxId);
                //        return true;
                //    }
                //}
            }
            else
            {
                int ii = Tax.Get(GetCtx(), GetM_Product_ID(), GetC_Charge_ID(),
                    GetDateOrdered(), GetDateOrdered(),
                    GetAD_Org_ID(), GetM_Warehouse_ID(),
                    GetC_BPartner_Location_ID(),		//	should be bill to
                    GetC_BPartner_Location_ID(), _IsSOTrx);
                if (ii == 0)
                {
                    log.Log(Level.SEVERE, "No Tax found");
                    return false;
                }
                SetC_Tax_ID(ii);
            }
            return true;
        }
        // Return Exempted Tax From the Organization
        private int GetExemptTax(Ctx ctx, int AD_Org_ID)
        {
            int C_Tax_ID = 0;
            String sql = "SELECT t.C_Tax_ID "
                + "FROM C_Tax t"
                + " INNER JOIN AD_Org o ON (t.AD_Client_ID=o.AD_Client_ID) "
                + "WHERE t.IsTaxExempt='Y' AND o.AD_Org_ID= " + AD_Org_ID
                + "ORDER BY t.Rate DESC";
            bool found = false;
            try
            {
                DataSet pstmt = ExecuteQuery.ExecuteDataset(sql, null);
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = pstmt.Tables[0].Rows[i];
                    C_Tax_ID = Utility.Util.GetValueOfInt(dr[0]);
                    found = true;
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            log.Fine("TaxExempt=Y - C_Tax_ID=" + C_Tax_ID);
            if (C_Tax_ID == 0)
            {
                log.SaveError("TaxCriteriaNotFound", Msg.GetMsg(ctx, "TaxNoExemptFound")
                    + (found ? "" : " (Tax/Org=" + AD_Org_ID + " not found)"));
            }
            return C_Tax_ID;
        }

        private int GetTaxFromLocation(bool isSoTrx, int taxCategory, int Country_ID, int Region_ID, string Postal)
        {
            string sql = "";
            int C_Tax_ID = 0;
            if (String.IsNullOrEmpty(Postal))
            {
                sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID =" + taxCategory +
                    " AND tcr.IsActive = 'Y' AND tcr.VATAX_TaxBase = 'L' AND tcr.C_Country_ID = " + Country_ID + " AND NVL(tcr.C_Region_ID,0) = " + Region_ID +
                    " AND tcr.Postal IS NULL AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
            }
            else
            {
                sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID =" + taxCategory +
                    " AND tcr.IsActive = 'Y' AND tcr.VATAX_TaxBase = 'L' AND tcr.C_Country_ID = " + Country_ID + " AND NVL(tcr.C_Region_ID,0) = " + Region_ID +
                    " AND (CASE WHEN (tcr.VATAX_IsPostal = 'Y') THEN CASE WHEN tcr.Postal <= '" + Postal + "' AND tcr.Postal_To >= '" + Postal + "' THEN 1 ELSE 2" +
                    " END ELSE  CASE WHEN tcr.Postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
            }
            C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
            if (C_Tax_ID > 0)
            {
                return C_Tax_ID;
            }
            else
            {
                if (String.IsNullOrEmpty(Postal))
                {
                    sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID =" + taxCategory +
                        " AND tcr.IsActive = 'Y' AND tcr.VATAX_TaxBase = 'L' AND tcr.C_Country_ID = " + Country_ID + " AND tcr.C_Region_ID IS NULL AND tcr.Postal IS NULL AND tx.SOPOType IN ('B','"
                        + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                }
                else
                {
                    sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID =" + taxCategory +
                        " AND tcr.IsActive = 'Y' AND tcr.VATAX_TaxBase = 'L' AND tcr.C_Country_ID = " + Country_ID + " AND tcr.C_Region_ID IS NULL AND (CASE WHEN (tcr.VATAX_IsPostal = 'Y') THEN CASE WHEN tcr.Postal <= '" + Postal +
                        "' AND tcr.Postal_To >= '" + Postal + "' THEN 1 ELSE 2" + " END ELSE  CASE WHEN tcr.Postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','"
                        + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                }
                C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                if (C_Tax_ID > 0)
                {
                    return C_Tax_ID;
                }
                else
                {
                    if (!String.IsNullOrEmpty(Postal))
                    {
                        sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID =" + taxCategory +
                            " AND tcr.IsActive = 'Y' AND tcr.VATAX_TaxBase = 'L' AND tcr.C_Country_ID IS NULL " + " AND tcr.C_Region_ID IS NULL AND (CASE WHEN (tcr.VATAX_IsPostal = 'Y') THEN CASE WHEN tcr.Postal <= '"
                            + Postal + "' AND tcr.Postal_To >= '" + Postal + "' THEN 1 ELSE 2 END ELSE  CASE WHEN tcr.Postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','"
                            + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                        C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                    }
                    if (C_Tax_ID > 0)
                    {
                        return C_Tax_ID;
                    }
                }
            }
            return C_Tax_ID;
        }

        private int GetTaxFromRegion(bool isSoTrx, int taxCategory, int Country_ID, int Region_ID, string Postal)
        {
            string sql = "";
            int C_Tax_ID = 0;
            if (String.IsNullOrEmpty(Postal))
            {
                sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = "
                + taxCategory + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID = " + Country_ID + " AND NVL(trl.C_Region_ID,0) = " + Region_ID +
                " AND trl.Postal IS NULL AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
            }
            else
            {
                sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = "
                + taxCategory + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID = " + Country_ID + " AND NVL(trl.C_Region_ID,0) = " + Region_ID +
                " AND (CASE WHEN (trl.VATAX_IsPostal = 'Y') THEN CASE WHEN trl.Postal <= '" + Postal + "' AND trl.Postal_To >= '" + Postal + "' THEN 1 ELSE 2 END ELSE  CASE WHEN trl.Postal = '"
                + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
            }
            C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
            if (C_Tax_ID > 0)
            {
                return C_Tax_ID;
            }
            else
            {
                if (String.IsNullOrEmpty(Postal))
                {
                    sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = "
                    + taxCategory + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID = " + Country_ID +
                    " AND trl.C_Region_ID IS NULL AND trl.Postal IS NULL AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                }
                else
                {
                    sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = "
                    + taxCategory + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID = " + Country_ID + " AND trl.C_Region_ID IS NULL AND (CASE WHEN (trl.VATAX_IsPostal = 'Y') THEN CASE WHEN trl.Postal <= '"
                    + Postal + "' AND trl.Postal_To >= '" + Postal + "' THEN 1 ELSE 2 END ELSE  CASE WHEN trl.Postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','"
                    + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                }
                C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                if (C_Tax_ID > 0)
                {
                    return C_Tax_ID;
                }
                else
                {
                    if (!String.IsNullOrEmpty(Postal))
                    {
                        sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = "
                        + taxCategory + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID IS NULL AND trl.C_Region_ID IS NULL AND (CASE WHEN (trl.VATAX_IsPostal = 'Y') THEN CASE WHEN trl.Postal <= '"
                        + Postal + "' AND trl.Postal_To >= '" + Postal + "' THEN 1 ELSE 2 END ELSE  CASE WHEN trl.Postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','"
                        + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                        C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));

                        if (C_Tax_ID > 0)
                        {
                            return C_Tax_ID;
                        }
                    }
                }
            }
            return C_Tax_ID;
        }

        private int GetTaxFromRegion(bool isSoTrx, int taxCategory, int Country_ID, int Region_ID, string Postal, int taxRegion, int toCountry)
        {
            string sql = "";
            int C_Tax_ID = 0;
            if (String.IsNullOrEmpty(Postal))
            {
                sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = "
                + taxCategory + " AND tcr.VATAX_DiffCountry = 'Y' AND tcr.C_Country_ID = " + toCountry + " AND tcr.VATAX_TaxRegion_ID = " + taxRegion + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID = " + Country_ID +
                " AND NVL(trl.C_Region_ID,0) = " + Region_ID + " AND trl.Postal IS NULL AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
            }
            else
            {
                sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = "
                + taxCategory + " AND tcr.VATAX_DiffCountry = 'Y' AND tcr.C_Country_ID = " + toCountry + " AND tcr.VATAX_TaxRegion_ID = " + taxRegion + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID = " + Country_ID +
                " AND NVL(trl.C_Region_ID,0) = " + Region_ID + " AND (CASE WHEN (trl.VATAX_IsPostal = 'Y') THEN CASE WHEN trl.Postal <= '" + Postal + "' AND trl.Postal_To >= '" + Postal +
                "' THEN 1 ELSE 2 END ELSE  CASE WHEN trl.Postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
            }
            C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
            if (C_Tax_ID > 0)
            {
                return C_Tax_ID;
            }
            else
            {
                if (String.IsNullOrEmpty(Postal))
                {
                    sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = "
                    + taxCategory + " AND tcr.VATAX_DiffCountry = 'Y' AND tcr.C_Country_ID = " + toCountry + " AND tcr.VATAX_TaxRegion_ID = " + taxRegion + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID = " + Country_ID +
                    " AND trl.C_Region_ID IS NULL AND trl.Postal IS NULL AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                }
                else
                {
                    sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = "
                    + taxCategory + " AND tcr.VATAX_DiffCountry = 'Y' AND tcr.C_Country_ID = " + toCountry + " AND tcr.VATAX_TaxRegion_ID = " + taxRegion + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID = " + Country_ID + " AND trl.C_Region_ID IS NULL AND (CASE WHEN (trl.VATAX_IsPostal = 'Y') THEN CASE WHEN trl.Postal <= '"
                    + Postal + "' AND trl.Postal_To >= '" + Postal + "' THEN 1 ELSE 2 END ELSE  CASE WHEN trl.Postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                }
                C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                if (C_Tax_ID > 0)
                {
                    return C_Tax_ID;
                }
                else
                {
                    if (!String.IsNullOrEmpty(Postal))
                    {
                        sql = @"SELECT tcr.C_Tax_ID FROM VATAX_TaxCatRate tcr LEFT JOIN VATAX_TaxRegionLine trl ON tcr.VATAX_TaxRegion_ID = trl.VATAX_TaxRegion_ID LEFT JOIN C_Tax tx ON tcr.C_Tax_ID = tx.C_Tax_ID WHERE tcr.C_TaxCategory_ID = "
                        + taxCategory + " AND tcr.VATAX_DiffCountry = 'Y' AND tcr.C_Country_ID = " + toCountry + " AND tcr.VATAX_TaxRegion_ID = " + taxRegion + " AND tcr.VATAX_TaxBase = 'R' AND tcr.IsActive = 'Y' AND trl.C_Country_ID IS NULL AND trl.C_Region_ID IS NULL AND (CASE WHEN (trl.VATAX_IsPostal = 'Y') THEN CASE WHEN trl.Postal <= '"
                        + Postal + "' AND trl.Postal_To >= '" + Postal + "' THEN 1 ELSE 2 END ELSE  CASE WHEN trl.Postal = '" + Postal + "' THEN 1 ELSE 2 END END) = 1 AND tx.SOPOType IN ('B','" + (isSoTrx ? 'S' : 'P') + "') ORDER BY tx.SOPOType DESC";
                        C_Tax_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                        if (C_Tax_ID > 0)
                        {
                            return C_Tax_ID;
                        }
                    }
                }
            }
            return C_Tax_ID;
        }

        /// <summary>
        /// Set Tax - (Callout follow-up)
        /// </summary>
        /// <param name="windowNo">window</param>
        /// <param name="columnName">changed column</param>
        /// <returns>true</returns>
        private bool SetTax(int windowNo, String columnName)
        {
            //	Check Product
            int M_Product_ID = GetM_Product_ID();
            int C_Charge_ID = GetC_Charge_ID();
            log.Fine("Product=" + M_Product_ID + ", C_Charge_ID=" + C_Charge_ID);
            if (M_Product_ID == 0 && C_Charge_ID == 0)
            {
                return SetAmt(windowNo, columnName);		//	true
            }

            //	Check Partner Location
            int shipC_BPartner_Location_ID = GetC_BPartner_Location_ID();
            if (shipC_BPartner_Location_ID == 0)
                return SetAmt(windowNo, columnName);		//
            log.Fine("Ship BP_Location=" + shipC_BPartner_Location_ID);
            DateTime? billDate = GetDateOrdered();
            log.Fine("Bill Date=" + billDate);

            DateTime? shipDate = GetDatePromised();
            log.Fine("Ship Date=" + shipDate);

            int AD_Org_ID = GetAD_Org_ID();
            log.Fine("Org=" + AD_Org_ID);

            int M_Warehouse_ID = GetM_Warehouse_ID();
            log.Fine("Warehouse=" + M_Warehouse_ID);

            int billC_BPartner_Location_ID = GetCtx().GetContextAsInt(windowNo, "Bill_Location_ID");
            if (billC_BPartner_Location_ID == 0)
                billC_BPartner_Location_ID = shipC_BPartner_Location_ID;
            log.Fine("Bill BP_Location=" + billC_BPartner_Location_ID);
            //
            int C_Tax_ID = Tax.Get(GetCtx(), M_Product_ID, C_Charge_ID, billDate, shipDate,
                AD_Org_ID, M_Warehouse_ID, billC_BPartner_Location_ID, shipC_BPartner_Location_ID,
                GetCtx().IsSOTrx(windowNo));
            log.Info("Tax ID=" + C_Tax_ID);
            //
            if (C_Tax_ID == 0)
            {
                // MessageBox.Show("Tax Error--error return from CLogger class and add in p_changeVO");
                //ValueNamePair pp = CLogger.retrieveError();
                //if (pp != null)
                //{
                //    p_changeVO.addError(pp.getValue());
                //}
                //else
                //{
                //    p_changeVO.addError("Tax Error");
                //}
            }
            else
                base.SetC_Tax_ID(C_Tax_ID);
            return SetAmt(windowNo, columnName);
        }

        /**
         * 	Calculare Tax Amt.
         * 	Assumes Line Net is calculated
         */
        public void SetTaxAmt()
        {
            try
            {
                Decimal TaxAmt = Env.ZERO;
                if (GetC_Tax_ID() == 0)
                    return;
                //	SetLineNetAmt();
                MTax tax = MTax.Get(GetCtx(), GetC_Tax_ID());
                if (tax.IsDocumentLevel() && _IsSOTrx)		//	AR Inv Tax
                    return;

                // if Surcharge Tax is selected on Tax, then calculate Tax accordingly
                if (Get_ColumnIndex("SurchargeAmt") > 0 && tax.GetSurcharge_Tax_ID() > 0)
                {
                    Decimal surchargeAmt = Env.ZERO;

                    // Calculate Surcharge Amount
                    TaxAmt = tax.CalculateSurcharge(GetLineNetAmt(), IsTaxIncluded(), GetPrecision(), out surchargeAmt);

                    if (IsTaxIncluded())
                        SetLineTotalAmt(GetLineNetAmt());
                    else
                        SetLineTotalAmt(Decimal.Add(Decimal.Add(GetLineNetAmt(), TaxAmt), surchargeAmt));
                    base.SetTaxAmt(TaxAmt);
                    SetSurchargeAmt(surchargeAmt);
                }
                else if (GetTaxAmt().Equals(Env.ZERO))
                {
                    TaxAmt = tax.CalculateTax(GetLineNetAmt(), IsTaxIncluded(), GetPrecision());
                    if (IsTaxIncluded())
                        SetLineTotalAmt(GetLineNetAmt());
                    else
                        SetLineTotalAmt(Decimal.Add(GetLineNetAmt(), TaxAmt));
                    base.SetTaxAmt(TaxAmt);
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MInvoiceLine--SetTaxAmt");
            }
        }

        /// <summary>
        /// Calculate Extended Amt.
        /// May or may not include tax
        /// </summary>
        public void SetLineNetAmt()
        {
            MOrder Ord = new MOrder(Env.GetCtx(), GetC_Order_ID(), null);
            if (Util.GetValueOfInt(Ord.GetVAPOS_POSTerminal_ID()) > 0)
            {
                Decimal bd = Decimal.Multiply(GetPriceActual(), GetQtyEntered());
                if (Env.Scale(bd) > GetPrecision())
                {
                    bd = Decimal.Round(bd, GetPrecision(), MidpointRounding.AwayFromZero);
                }
                base.SetLineNetAmt(bd);
            }
            else
            {
                Decimal LineNetAmt = Decimal.Multiply(GetPriceActual(), GetQtyEntered());

                if (Env.IsModuleInstalled("ED007_"))
                {
                    #region Set Discount Values
                    MOrder order = new MOrder(GetCtx(), Util.GetValueOfInt(GetC_Order_ID()), null);
                    MProduct product = new MProduct(GetCtx(), Util.GetValueOfInt(GetM_Product_ID()), null);
                    MBPartner bPartner = new MBPartner(GetCtx(), order.GetC_BPartner_ID(), null);
                    MDiscountSchema discountSchema = new MDiscountSchema(GetCtx(), bPartner.GetM_DiscountSchema_ID(), null);
                    int precision = MCurrency.GetStdPrecision(GetCtx(), order.GetC_Currency_ID());
                    String epl = GetCtx().GetContext("EnforcePriceLimit");
                    bool enforce = order.IsSOTrx() && epl != null && epl.Equals("Y");
                    decimal valueBasedDiscount = 0;
                    #region set Value Based Discount Based on Discount Calculation
                    if (bPartner.GetED007_DiscountCalculation() == "C3")
                    {
                        SetED007_ValueBaseDiscount(0);
                    }
                    #endregion

                    if (Util.GetValueOfInt(GetM_Product_ID()) > 0)
                    {
                        if (bPartner.GetM_DiscountSchema_ID() > 0 && ((Util.GetValueOfString(order.IsSOTrx()) == "True" && Util.GetValueOfString(order.IsReturnTrx()) == "False")
                                                                   || (Util.GetValueOfString(order.IsSOTrx()) == "False" && Util.GetValueOfString(order.IsReturnTrx()) == "False")))
                        {
                            #region Combination
                            if (discountSchema.GetDiscountType() == "C" || discountSchema.GetDiscountType() == "T")
                            {
                                string sql = @"SELECT C_BPartner_ID , C_BP_Group_ID , M_Product_Category_ID , M_Product_ID , ED007_DiscountPercentage1 , ED007_DiscountPercentage2 , ED007_DiscountPercentage3 ,
                                          ED007_DiscountPercentage4 , ED007_DiscountPercentage5 , ED007_ValueBasedDiscount FROM ED007_DiscountCombination WHERE M_DiscountSchema_ID = " + bPartner.GetM_DiscountSchema_ID() +
                                                 " AND IsActive='Y' AND AD_Client_ID =" + GetCtx().GetAD_Client_ID();
                                DataSet dsDiscountCombination = new DataSet();
                                dsDiscountCombination = DB.ExecuteDataset(sql, null, null);
                                if (dsDiscountCombination != null)
                                {
                                    if (dsDiscountCombination.Tables.Count > 0)
                                    {
                                        if (dsDiscountCombination.Tables[0].Rows.Count > 0)
                                        {
                                            int i = 0, ProductValue = 0, BPValue = 0, BPAndProductValue = 0, BPGrpValue = 0, PCatValue = 0, PCatAndBpGrpValue = 0, noException = 0;
                                            decimal AmtProduct = 0, AmtBpartner = 0, AmtBpAndProduct = 0, AmtBpGroup = 0, AmtPCategory = 0, AmtPcatAndBpGrp = 0, AmtNoException = 0;

                                            for (i = 0; i < dsDiscountCombination.Tables[0].Rows.Count; i++)
                                            {
                                                #region Business Partner And Product
                                                if (Util.GetValueOfInt(dsDiscountCombination.Tables[0].Rows[i]["M_Product_ID"]) == product.GetM_Product_ID() &&
                                                    Util.GetValueOfInt(dsDiscountCombination.Tables[0].Rows[i]["C_BPartner_ID"]) == order.GetC_BPartner_ID())
                                                {
                                                    BPAndProductValue = i + 1;
                                                    AmtBpAndProduct = LineNetAmt;

                                                    #region set Value Based Discount Based on Discount Calculation
                                                    if (bPartner.GetED007_DiscountCalculation() == "C1" || bPartner.GetED007_DiscountCalculation() == "C2")
                                                    {
                                                        valueBasedDiscount = Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_ValueBasedDiscount"]);
                                                        SetED007_DiscountPerUnit(valueBasedDiscount);
                                                        //Amit 27-nov-2014
                                                        //mTab.SetValue("ED007_ValueBaseDiscount", Decimal.Round(Decimal.Multiply(valueBasedDiscount, QtyOrdered.Value), precision));
                                                        SetED007_ValueBaseDiscount(Decimal.Round(Decimal.Multiply(valueBasedDiscount, GetQtyEntered()), precision));
                                                        //Amit
                                                    }
                                                    #endregion

                                                    #region Value Based Discount
                                                    if (Util.GetValueOfString(bPartner.GetED007_DiscountCalculation()) == "C2")
                                                    {
                                                        //amit 27-nov-2014
                                                        //AmtBpAndProduct = Decimal.Subtract(AmtBpAndProduct, Decimal.Multiply(valueBasedDiscount, QtyOrdered.Value));
                                                        AmtBpAndProduct = Decimal.Subtract(AmtBpAndProduct, Decimal.Multiply(valueBasedDiscount, GetQtyEntered()));
                                                        //Amit
                                                    }
                                                    #endregion

                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage1"]) > 0)
                                                    {
                                                        AmtBpAndProduct = Decimal.Subtract(AmtBpAndProduct, Decimal.Divide(Decimal.Multiply(AmtBpAndProduct, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage1"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage2"]) > 0)
                                                    {
                                                        AmtBpAndProduct = Decimal.Subtract(AmtBpAndProduct, Decimal.Divide(Decimal.Multiply(AmtBpAndProduct, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage2"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage3"]) > 0)
                                                    {
                                                        AmtBpAndProduct = Decimal.Subtract(AmtBpAndProduct, Decimal.Divide(Decimal.Multiply(AmtBpAndProduct, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage3"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage4"]) > 0)
                                                    {
                                                        AmtBpAndProduct = Decimal.Subtract(AmtBpAndProduct, Decimal.Divide(Decimal.Multiply(AmtBpAndProduct, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage4"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage5"]) > 0)
                                                    {
                                                        AmtBpAndProduct = Decimal.Subtract(AmtBpAndProduct, Decimal.Divide(Decimal.Multiply(AmtBpAndProduct, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage5"])), 100));
                                                    }
                                                    break;
                                                }
                                                #endregion
                                                #region  Product
                                                else if (Util.GetValueOfInt(dsDiscountCombination.Tables[0].Rows[i]["M_Product_ID"]) == product.GetM_Product_ID() &&
                                                         Util.GetValueOfInt(dsDiscountCombination.Tables[0].Rows[i]["C_BPartner_ID"]) == 0)
                                                {
                                                    ProductValue = i + 1;
                                                    AmtProduct = LineNetAmt;
                                                    #region set Value Based Discount Based on Discount Calculation
                                                    if (bPartner.GetED007_DiscountCalculation() == "C1" || bPartner.GetED007_DiscountCalculation() == "C2")
                                                    {
                                                        valueBasedDiscount = Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_ValueBasedDiscount"]);
                                                        SetED007_DiscountPerUnit(valueBasedDiscount);
                                                        //Amit 27-nov-2014
                                                        //mTab.SetValue("ED007_ValueBaseDiscount", Decimal.Round(Decimal.Multiply(valueBasedDiscount, QtyOrdered.Value), precision));
                                                        SetED007_ValueBaseDiscount(Decimal.Round(Decimal.Multiply(valueBasedDiscount, GetQtyEntered()), precision));
                                                        //Amit
                                                    }
                                                    #endregion

                                                    #region Value Based Discount
                                                    if (Util.GetValueOfString(bPartner.GetED007_DiscountCalculation()) == "C2")
                                                    {
                                                        //Amit 27-nov-2014
                                                        // AmtProduct = Decimal.Subtract(AmtProduct, Decimal.Multiply(valueBasedDiscount, QtyOrdered.Value));
                                                        AmtProduct = Decimal.Subtract(AmtProduct, Decimal.Multiply(valueBasedDiscount, GetQtyEntered()));
                                                        //Amit
                                                    }
                                                    #endregion

                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage1"]) > 0)
                                                    {
                                                        AmtProduct = Decimal.Subtract(AmtProduct, Decimal.Divide(Decimal.Multiply(AmtProduct, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage1"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage2"]) > 0)
                                                    {
                                                        AmtProduct = Decimal.Subtract(AmtProduct, Decimal.Divide(Decimal.Multiply(AmtProduct, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage2"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage3"]) > 0)
                                                    {
                                                        AmtProduct = Decimal.Subtract(AmtProduct, Decimal.Divide(Decimal.Multiply(AmtProduct, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage3"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage4"]) > 0)
                                                    {
                                                        AmtProduct = Decimal.Subtract(AmtProduct, Decimal.Divide(Decimal.Multiply(AmtProduct, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage4"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage5"]) > 0)
                                                    {
                                                        AmtProduct = Decimal.Subtract(AmtProduct, Decimal.Divide(Decimal.Multiply(AmtProduct, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage5"])), 100));
                                                    }
                                                }
                                                #endregion
                                                #region Business Partner
                                                else if (Util.GetValueOfInt(dsDiscountCombination.Tables[0].Rows[i]["M_Product_ID"]) == 0 &&
                                                         Util.GetValueOfInt(dsDiscountCombination.Tables[0].Rows[i]["C_BPartner_ID"]) == order.GetC_BPartner_ID())
                                                {
                                                    BPValue = i + 1;
                                                    AmtBpartner = LineNetAmt;

                                                    #region set Value Based Discount Based on Discount Calculation
                                                    if (bPartner.GetED007_DiscountCalculation() == "C1" || bPartner.GetED007_DiscountCalculation() == "C2")
                                                    {
                                                        valueBasedDiscount = Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_ValueBasedDiscount"]);
                                                        SetED007_DiscountPerUnit(valueBasedDiscount);
                                                        //Amit 27-nov-2014
                                                        //mTab.SetValue("ED007_ValueBaseDiscount", Decimal.Round(Decimal.Multiply(valueBasedDiscount, QtyOrdered.Value), precision));
                                                        SetED007_ValueBaseDiscount(Decimal.Round(Decimal.Multiply(valueBasedDiscount, GetQtyEntered()), precision));
                                                        //Amit
                                                    }
                                                    #endregion

                                                    #region Value Based Discount
                                                    if (Util.GetValueOfString(bPartner.GetED007_DiscountCalculation()) == "C2")
                                                    {
                                                        //Amit 27-nov-2014
                                                        //AmtBpartner = Decimal.Subtract(AmtBpartner, Decimal.Multiply(valueBasedDiscount, QtyOrdered.Value));
                                                        AmtBpartner = Decimal.Subtract(AmtBpartner, Decimal.Multiply(valueBasedDiscount, GetQtyEntered()));
                                                        //amit
                                                    }
                                                    #endregion

                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage1"]) > 0)
                                                    {
                                                        AmtBpartner = Decimal.Subtract(AmtBpartner, Decimal.Divide(Decimal.Multiply(AmtBpartner, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage1"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage2"]) > 0)
                                                    {
                                                        AmtBpartner = Decimal.Subtract(AmtBpartner, Decimal.Divide(Decimal.Multiply(AmtBpartner, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage2"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage3"]) > 0)
                                                    {
                                                        AmtBpartner = Decimal.Subtract(AmtBpartner, Decimal.Divide(Decimal.Multiply(AmtBpartner, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage3"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage4"]) > 0)
                                                    {
                                                        AmtBpartner = Decimal.Subtract(AmtBpartner, Decimal.Divide(Decimal.Multiply(AmtBpartner, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage4"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage5"]) > 0)
                                                    {
                                                        AmtBpartner = Decimal.Subtract(AmtBpartner, Decimal.Divide(Decimal.Multiply(AmtBpartner, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage5"])), 100));
                                                    }
                                                }
                                                #endregion
                                                #region Business Partner Group And Product Category
                                                else if (Util.GetValueOfInt(dsDiscountCombination.Tables[0].Rows[i]["M_Product_Category_ID"]) == product.GetM_Product_Category_ID() &&
                                                        Util.GetValueOfInt(dsDiscountCombination.Tables[0].Rows[i]["C_BP_Group_ID"]) == bPartner.GetC_BP_Group_ID())
                                                {
                                                    PCatAndBpGrpValue = i + 1;
                                                    AmtPcatAndBpGrp = LineNetAmt;

                                                    #region set Value Based Discount Based on Discount Calculation
                                                    if (bPartner.GetED007_DiscountCalculation() == "C1" || bPartner.GetED007_DiscountCalculation() == "C2")
                                                    {
                                                        valueBasedDiscount = Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_ValueBasedDiscount"]);
                                                        SetED007_DiscountPerUnit(valueBasedDiscount);
                                                        //Amit 27-nov-2014
                                                        //mTab.SetValue("ED007_ValueBaseDiscount", Decimal.Round(Decimal.Multiply(valueBasedDiscount, QtyOrdered.Value), precision));
                                                        SetED007_ValueBaseDiscount(Decimal.Round(Decimal.Multiply(valueBasedDiscount, GetQtyEntered()), precision));
                                                        //Amit
                                                    }
                                                    #endregion

                                                    #region Value Based Discount
                                                    if (Util.GetValueOfString(bPartner.GetED007_DiscountCalculation()) == "C2")
                                                    {
                                                        //Amit 27-nov-2014
                                                        //AmtPcatAndBpGrp = Decimal.Subtract(AmtPcatAndBpGrp, Decimal.Multiply(valueBasedDiscount, QtyOrdered.Value));
                                                        AmtPcatAndBpGrp = Decimal.Subtract(AmtPcatAndBpGrp, Decimal.Multiply(valueBasedDiscount, GetQtyEntered()));
                                                        //amit
                                                    }
                                                    #endregion

                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage1"]) > 0)
                                                    {
                                                        AmtPcatAndBpGrp = Decimal.Subtract(AmtPcatAndBpGrp, Decimal.Divide(Decimal.Multiply(AmtPcatAndBpGrp, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage1"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage2"]) > 0)
                                                    {
                                                        AmtPcatAndBpGrp = Decimal.Subtract(AmtPcatAndBpGrp, Decimal.Divide(Decimal.Multiply(AmtPcatAndBpGrp, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage2"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage3"]) > 0)
                                                    {
                                                        AmtPcatAndBpGrp = Decimal.Subtract(AmtPcatAndBpGrp, Decimal.Divide(Decimal.Multiply(AmtPcatAndBpGrp, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage3"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage4"]) > 0)
                                                    {
                                                        AmtPcatAndBpGrp = Decimal.Subtract(AmtPcatAndBpGrp, Decimal.Divide(Decimal.Multiply(AmtPcatAndBpGrp, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage4"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage5"]) > 0)
                                                    {
                                                        AmtPcatAndBpGrp = Decimal.Subtract(AmtPcatAndBpGrp, Decimal.Divide(Decimal.Multiply(AmtPcatAndBpGrp, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage5"])), 100));
                                                    }
                                                }
                                                #endregion
                                                #region  Product Category
                                                else if (Util.GetValueOfInt(dsDiscountCombination.Tables[0].Rows[i]["M_Product_Category_ID"]) == product.GetM_Product_Category_ID() &&
                                                        Util.GetValueOfInt(dsDiscountCombination.Tables[0].Rows[i]["C_BP_Group_ID"]) == 0)
                                                {
                                                    PCatValue = i + 1;
                                                    AmtPCategory = LineNetAmt;

                                                    #region set Value Based Discount Based on Discount Calculation
                                                    if (bPartner.GetED007_DiscountCalculation() == "C1" || bPartner.GetED007_DiscountCalculation() == "C2")
                                                    {
                                                        valueBasedDiscount = Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_ValueBasedDiscount"]);
                                                        SetED007_DiscountPerUnit(valueBasedDiscount);
                                                        //Amit 27-nov-2014
                                                        //mTab.SetValue("ED007_ValueBaseDiscount", Decimal.Round(Decimal.Multiply(valueBasedDiscount, QtyOrdered.Value), precision));
                                                        SetED007_ValueBaseDiscount(Decimal.Round(Decimal.Multiply(valueBasedDiscount, GetQtyEntered()), precision));
                                                        //Amit
                                                    }
                                                    #endregion

                                                    #region Value Based Discount
                                                    if (Util.GetValueOfString(bPartner.GetED007_DiscountCalculation()) == "C2")
                                                    {
                                                        //Amit 27-nov-2014
                                                        //AmtPCategory = Decimal.Subtract(AmtPCategory, Decimal.Multiply(valueBasedDiscount, QtyOrdered.Value));
                                                        AmtPCategory = Decimal.Subtract(AmtPCategory, Decimal.Multiply(valueBasedDiscount, GetQtyEntered()));
                                                    }
                                                    #endregion

                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage1"]) > 0)
                                                    {
                                                        AmtPCategory = Decimal.Subtract(AmtPCategory, Decimal.Divide(Decimal.Multiply(AmtPCategory, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage1"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage2"]) > 0)
                                                    {
                                                        AmtPCategory = Decimal.Subtract(AmtPCategory, Decimal.Divide(Decimal.Multiply(AmtPCategory, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage2"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage3"]) > 0)
                                                    {
                                                        AmtPCategory = Decimal.Subtract(AmtPCategory, Decimal.Divide(Decimal.Multiply(AmtPCategory, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage3"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage4"]) > 0)
                                                    {
                                                        AmtPCategory = Decimal.Subtract(AmtPCategory, Decimal.Divide(Decimal.Multiply(AmtPCategory, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage4"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage5"]) > 0)
                                                    {
                                                        AmtPCategory = Decimal.Subtract(AmtPCategory, Decimal.Divide(Decimal.Multiply(AmtPCategory, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage5"])), 100));
                                                    }
                                                }
                                                #endregion
                                                #region Business Partner Group
                                                else if (Util.GetValueOfInt(dsDiscountCombination.Tables[0].Rows[i]["M_Product_Category_ID"]) == 0 &&
                                                       Util.GetValueOfInt(dsDiscountCombination.Tables[0].Rows[i]["C_BP_Group_ID"]) == bPartner.GetC_BP_Group_ID())
                                                {
                                                    BPGrpValue = i + 1;
                                                    AmtBpGroup = LineNetAmt;

                                                    #region set Value Based Discount Based on Discount Calculation
                                                    if (bPartner.GetED007_DiscountCalculation() == "C1" || bPartner.GetED007_DiscountCalculation() == "C2")
                                                    {
                                                        valueBasedDiscount = Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_ValueBasedDiscount"]);
                                                        SetED007_DiscountPerUnit(valueBasedDiscount);
                                                        //Amit 27-nov-2014
                                                        //mTab.SetValue("ED007_ValueBaseDiscount", Decimal.Round(Decimal.Multiply(valueBasedDiscount, QtyOrdered.Value), precision));
                                                        SetED007_ValueBaseDiscount(Decimal.Round(Decimal.Multiply(valueBasedDiscount, GetQtyEntered()), precision));
                                                        //Amit
                                                    }
                                                    #endregion

                                                    #region Value Based Discount
                                                    if (Util.GetValueOfString(bPartner.GetED007_DiscountCalculation()) == "C2")
                                                    {
                                                        //Amit 27-nov-2014
                                                        //AmtBpGroup = Decimal.Subtract(AmtBpGroup, Decimal.Multiply(valueBasedDiscount, QtyOrdered.Value));
                                                        AmtBpGroup = Decimal.Subtract(AmtBpGroup, Decimal.Multiply(valueBasedDiscount, GetQtyEntered()));
                                                        //Amit
                                                    }
                                                    #endregion

                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage1"]) > 0)
                                                    {
                                                        AmtBpGroup = Decimal.Subtract(AmtBpGroup, Decimal.Divide(Decimal.Multiply(AmtBpGroup, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage1"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage2"]) > 0)
                                                    {
                                                        AmtBpGroup = Decimal.Subtract(AmtBpGroup, Decimal.Divide(Decimal.Multiply(AmtBpGroup, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage2"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage3"]) > 0)
                                                    {
                                                        AmtBpGroup = Decimal.Subtract(AmtBpGroup, Decimal.Divide(Decimal.Multiply(AmtBpGroup, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage3"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage4"]) > 0)
                                                    {
                                                        AmtBpGroup = Decimal.Subtract(AmtBpGroup, Decimal.Divide(Decimal.Multiply(AmtBpGroup, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage4"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage5"]) > 0)
                                                    {
                                                        AmtBpGroup = Decimal.Subtract(AmtBpGroup, Decimal.Divide(Decimal.Multiply(AmtBpGroup, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage5"])), 100));
                                                    }
                                                }
                                                #endregion
                                                #region when no Exception
                                                if (Util.GetValueOfInt(dsDiscountCombination.Tables[0].Rows[i]["M_Product_ID"]) == 0 &&
                                                        Util.GetValueOfInt(dsDiscountCombination.Tables[0].Rows[i]["C_BPartner_ID"]) == 0 &&
                                                        Util.GetValueOfInt(dsDiscountCombination.Tables[0].Rows[i]["M_Product_Category_ID"]) == 0 &&
                                                        Util.GetValueOfInt(dsDiscountCombination.Tables[0].Rows[i]["C_BP_Group_ID"]) == 0)
                                                {
                                                    noException = i + 1;
                                                    AmtNoException = LineNetAmt;

                                                    #region set Value Based Discount Based on Discount Calculation
                                                    if (bPartner.GetED007_DiscountCalculation() == "C1" || bPartner.GetED007_DiscountCalculation() == "C2")
                                                    {
                                                        valueBasedDiscount = Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_ValueBasedDiscount"]);
                                                        SetED007_DiscountPerUnit(valueBasedDiscount);
                                                        //Amit 27-nov-2014
                                                        //mTab.SetValue("ED007_ValueBaseDiscount", Decimal.Round(Decimal.Multiply(valueBasedDiscount, QtyOrdered.Value), precision));
                                                        SetED007_ValueBaseDiscount(Decimal.Round(Decimal.Multiply(valueBasedDiscount, GetQtyEntered()), precision));
                                                        //Amit
                                                    }
                                                    #endregion

                                                    #region Value Based Discount
                                                    if (Util.GetValueOfString(bPartner.GetED007_DiscountCalculation()) == "C2")
                                                    {
                                                        //Amit 27-nov-2014
                                                        //AmtNoException = Decimal.Subtract(AmtNoException, Decimal.Multiply(valueBasedDiscount, QtyOrdered.Value));
                                                        AmtNoException = Decimal.Subtract(AmtNoException, Decimal.Multiply(valueBasedDiscount, GetQtyEntered()));
                                                        //Amit
                                                    }
                                                    #endregion

                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage1"]) > 0)
                                                    {
                                                        AmtNoException = Decimal.Subtract(AmtNoException, Decimal.Divide(Decimal.Multiply(AmtNoException, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage1"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage2"]) > 0)
                                                    {
                                                        AmtNoException = Decimal.Subtract(AmtNoException, Decimal.Divide(Decimal.Multiply(AmtNoException, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage2"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage3"]) > 0)
                                                    {
                                                        AmtNoException = Decimal.Subtract(AmtNoException, Decimal.Divide(Decimal.Multiply(AmtNoException, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage3"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage4"]) > 0)
                                                    {
                                                        AmtNoException = Decimal.Subtract(AmtNoException, Decimal.Divide(Decimal.Multiply(AmtNoException, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage4"])), 100));
                                                    }
                                                    if (Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage5"]) > 0)
                                                    {
                                                        AmtNoException = Decimal.Subtract(AmtNoException, Decimal.Divide(Decimal.Multiply(AmtNoException, Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[i]["ED007_DiscountPercentage5"])), 100));
                                                    }
                                                }
                                                #endregion
                                            }


                                            #region Discount Percent
                                            SetED007_DiscountPercent(Util.GetValueOfDecimal(order.GetED007_DiscountPercent()));
                                            #endregion

                                            #region Set value when record match for Business Partner And Product
                                            if (BPAndProductValue > 0)
                                            {
                                                SetED007_DiscountPercentage1(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[BPAndProductValue - 1]["ED007_DiscountPercentage1"]));
                                                SetED007_DiscountPercentage2(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[BPAndProductValue - 1]["ED007_DiscountPercentage2"]));
                                                SetED007_DiscountPercentage3(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[BPAndProductValue - 1]["ED007_DiscountPercentage3"]));
                                                SetED007_DiscountPercentage4(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[BPAndProductValue - 1]["ED007_DiscountPercentage4"]));
                                                SetED007_DiscountPercentage5(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[BPAndProductValue - 1]["ED007_DiscountPercentage5"]));
                                                AmtBpAndProduct = Decimal.Round(AmtBpAndProduct, precision);
                                                valueBasedDiscount = Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[BPAndProductValue - 1]["ED007_ValueBasedDiscount"]);
                                                SetED007_DiscountPerUnit(valueBasedDiscount);
                                                #region Break
                                                if (discountSchema.GetDiscountType() == "T")
                                                {
                                                    AmtBpAndProduct = BreakCalculation(Util.GetValueOfInt(GetM_Product_ID()), GetCtx().GetAD_Client_ID(),
                                                                 AmtBpAndProduct, bPartner.GetM_DiscountSchema_ID(), Util.GetValueOfDecimal(bPartner.GetFlatDiscount()), GetQtyEntered());
                                                }
                                                #endregion
                                                #region Value Based Discount
                                                if (Util.GetValueOfString(bPartner.GetED007_DiscountCalculation()) == "C1")
                                                {
                                                    valueBasedDiscount = Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[BPAndProductValue - 1]["ED007_ValueBasedDiscount"]);
                                                    SetED007_DiscountPerUnit(valueBasedDiscount);
                                                    //AmtBpAndProduct = Decimal.Subtract(AmtBpAndProduct, Decimal.Multiply(valueBasedDiscount, QtyOrdered.Value));
                                                    AmtBpAndProduct = Decimal.Subtract(AmtBpAndProduct, Decimal.Multiply(valueBasedDiscount, GetQtyEntered()));
                                                }
                                                #endregion
                                                SetED007_DiscountAmount(Decimal.Round(Decimal.Divide(Decimal.Multiply(Util.GetValueOfDecimal(order.GetED007_DiscountPercent()), AmtBpAndProduct), 100), precision));
                                                SetLineNetAmt(Decimal.Subtract(AmtBpAndProduct, Util.GetValueOfDecimal(GetED007_DiscountAmount())));

                                                //if (enforce && Decimal.ToDouble(PriceLimit.Value) != 0.0
                                                //      && Util.GetValueOfDecimal(mTab.GetValue("LineNetAmt")).CompareTo(Decimal.Round(Decimal.Multiply(PriceLimit.Value, QtyOrdered.Value), precision)) < 0)

                                                if (enforce && Decimal.ToDouble(GetPriceLimit()) != 0.0
                                                     && Util.GetValueOfDecimal(GetLineNetAmt()).CompareTo(Decimal.Round(Decimal.Multiply(GetPriceLimit(), GetQtyEntered()), precision)) < 0)
                                                {
                                                    SetPriceActual(GetPriceLimit());
                                                    SetPriceEntered(GetPriceLimit());

                                                    if (GetPriceList() != 0)
                                                    {
                                                        Decimal Discount = VAdvantage.Utility.Util.GetValueOfDecimal(((Decimal.ToDouble(GetPriceList()) - Decimal.ToDouble(GetPriceLimit())) / Decimal.ToDouble(GetPriceList()) * 100.0));
                                                        if (Env.Scale(Discount) > 2)
                                                        {
                                                            Discount = Decimal.Round(Discount, 2);
                                                        }
                                                        SetDiscount(Discount);
                                                    }
                                                    //Amit 27-nov-2014
                                                    //LineNetAmt = Decimal.Multiply(QtyOrdered.Value, PriceLimit.Value);
                                                    LineNetAmt = Decimal.Multiply(GetQtyEntered(), GetPriceLimit());
                                                    //Amit
                                                    if (Env.Scale(LineNetAmt) > precision)
                                                        LineNetAmt = Decimal.Round(LineNetAmt, precision);//, MidpointRounding.AwayFromZero);
                                                    SetED007_DscuntlineAmt(LineNetAmt);
                                                    SetLineNetAmt(LineNetAmt);
                                                    SetED007_DiscountAmount(0);
                                                    SetED007_DiscountPercentage1(0);
                                                    SetED007_DiscountPercentage2(0);
                                                    SetED007_DiscountPercentage3(0);
                                                    SetED007_DiscountPercentage4(0);
                                                    SetED007_DiscountPercentage5(0);
                                                    SetED007_DiscountPercent(0);
                                                }
                                                else
                                                {
                                                    // mTab.SetValue("LineNetAmt", AmtBpAndProduct);
                                                    SetED007_DscuntlineAmt(AmtBpAndProduct);
                                                    if (GetQtyOrdered() > 0 && GetPriceList() > 0 && GetQtyEntered() > 0)
                                                    {
                                                        //mTab.SetValue("Discount", Decimal.Round(Decimal.Multiply(Decimal.Divide(Decimal.Subtract(Decimal.Multiply(PriceList.Value, QtyOrdered.Value), AmtBpAndProduct), Decimal.Multiply(PriceList.Value, QtyOrdered.Value)), 100), precision));
                                                        SetDiscount(Decimal.Round(Decimal.Multiply(Decimal.Divide(Decimal.Subtract(Decimal.Multiply(GetPriceList(), GetQtyEntered()), AmtBpAndProduct), Decimal.Multiply(GetPriceList(), GetQtyEntered())), 100), precision));
                                                    }
                                                }
                                            }
                                            #endregion
                                            #region  Set value when record match for  Product
                                            else if (ProductValue > 0)
                                            {
                                                SetED007_DiscountPercentage1(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[ProductValue - 1]["ED007_DiscountPercentage1"]));
                                                SetED007_DiscountPercentage2(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[ProductValue - 1]["ED007_DiscountPercentage2"]));
                                                SetED007_DiscountPercentage3(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[ProductValue - 1]["ED007_DiscountPercentage3"]));
                                                SetED007_DiscountPercentage4(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[ProductValue - 1]["ED007_DiscountPercentage4"]));
                                                SetED007_DiscountPercentage5(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[ProductValue - 1]["ED007_DiscountPercentage5"]));
                                                AmtProduct = Decimal.Round(AmtProduct, precision);
                                                valueBasedDiscount = Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[ProductValue - 1]["ED007_ValueBasedDiscount"]);
                                                SetED007_DiscountPerUnit(valueBasedDiscount);
                                                #region Break
                                                if (discountSchema.GetDiscountType() == "T")
                                                {
                                                    AmtProduct = BreakCalculation(Util.GetValueOfInt(GetM_Product_ID()), GetCtx().GetAD_Client_ID(),
                                                                 AmtProduct, bPartner.GetM_DiscountSchema_ID(), Util.GetValueOfDecimal(bPartner.GetFlatDiscount()), GetQtyEntered());
                                                }
                                                #endregion
                                                #region Value Based Discount
                                                if (Util.GetValueOfString(bPartner.GetED007_DiscountCalculation()) == "C1")
                                                {
                                                    valueBasedDiscount = Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[ProductValue - 1]["ED007_ValueBasedDiscount"]);
                                                    SetED007_DiscountPerUnit(valueBasedDiscount);
                                                    //AmtProduct = Decimal.Subtract(AmtProduct, Decimal.Multiply(valueBasedDiscount, QtyOrdered.Value));
                                                    AmtProduct = Decimal.Subtract(AmtProduct, Decimal.Multiply(valueBasedDiscount, GetQtyEntered()));
                                                }
                                                #endregion
                                                SetED007_DiscountAmount(Decimal.Round(Decimal.Divide(Decimal.Multiply(Util.GetValueOfDecimal(order.GetED007_DiscountPercent()), AmtProduct), 100), precision));
                                                SetLineNetAmt(Decimal.Subtract(AmtProduct, Util.GetValueOfDecimal(GetED007_DiscountAmount())));

                                                //if (enforce && Decimal.ToDouble(PriceLimit.Value) != 0.0
                                                //      && Util.GetValueOfDecimal(mTab.GetValue("LineNetAmt")).CompareTo(Decimal.Round(Decimal.Multiply(PriceLimit.Value, QtyOrdered.Value), precision)) < 0)
                                                if (enforce && Decimal.ToDouble(GetPriceLimit()) != 0.0
                                                     && Util.GetValueOfDecimal(GetLineNetAmt()).CompareTo(Decimal.Round(Decimal.Multiply(GetPriceLimit(), GetQtyEntered()), precision)) < 0)
                                                {
                                                    SetPriceActual(GetPriceLimit());
                                                    SetPriceEntered(GetPriceLimit());

                                                    if (GetPriceList() != 0)
                                                    {
                                                        Decimal Discount = VAdvantage.Utility.Util.GetValueOfDecimal(((Decimal.ToDouble(GetPriceList()) - Decimal.ToDouble(GetPriceLimit())) / Decimal.ToDouble(GetPriceList()) * 100.0));
                                                        if (Env.Scale(Discount) > 2)
                                                        {
                                                            Discount = Decimal.Round(Discount, 2);
                                                        }
                                                        SetDiscount(Discount);
                                                    }
                                                    //Amit 27-nov-2014
                                                    //LineNetAmt = Decimal.Multiply(QtyOrdered.Value, PriceLimit.Value);
                                                    LineNetAmt = Decimal.Multiply(GetQtyEntered(), GetPriceLimit());
                                                    //Amit
                                                    if (Env.Scale(LineNetAmt) > precision)
                                                        LineNetAmt = Decimal.Round(LineNetAmt, precision);//, MidpointRounding.AwayFromZero);
                                                    SetLineNetAmt(LineNetAmt);
                                                    SetED007_DscuntlineAmt(LineNetAmt);
                                                    SetED007_DiscountAmount(0);
                                                    SetED007_DiscountPercentage1(0);
                                                    SetED007_DiscountPercentage2(0);
                                                    SetED007_DiscountPercentage3(0);
                                                    SetED007_DiscountPercentage4(0);
                                                    SetED007_DiscountPercentage5(0);
                                                    SetED007_DiscountPercent(0);
                                                }
                                                else
                                                {
                                                    // mTab.SetValue("LineNetAmt", AmtProduct);
                                                    SetED007_DscuntlineAmt(AmtProduct);
                                                    if (GetQtyOrdered() > 0 && GetPriceList() > 0 && GetQtyEntered() > 0)
                                                    {
                                                        //mTab.SetValue("Discount", Decimal.Round(Decimal.Multiply(Decimal.Divide(Decimal.Subtract(Decimal.Multiply(PriceList.Value, QtyOrdered.Value), AmtProduct), Decimal.Multiply(PriceList.Value, QtyOrdered.Value)), 100), precision));
                                                        SetDiscount(Decimal.Round(Decimal.Multiply(Decimal.Divide(Decimal.Subtract(Decimal.Multiply(GetPriceList(), GetQtyEntered()), AmtProduct), Decimal.Multiply(GetPriceList(), GetQtyEntered())), 100), precision));
                                                    }
                                                }
                                            }
                                            #endregion
                                            #region Set value when record match for Business Partner
                                            else if (BPValue > 0)
                                            {
                                                SetED007_DiscountPercentage1(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[BPValue - 1]["ED007_DiscountPercentage1"]));
                                                SetED007_DiscountPercentage2(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[BPValue - 1]["ED007_DiscountPercentage2"]));
                                                SetED007_DiscountPercentage3(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[BPValue - 1]["ED007_DiscountPercentage3"]));
                                                SetED007_DiscountPercentage4(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[BPValue - 1]["ED007_DiscountPercentage4"]));
                                                SetED007_DiscountPercentage5(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[BPValue - 1]["ED007_DiscountPercentage5"]));
                                                AmtBpartner = Decimal.Round(AmtBpartner, precision);
                                                valueBasedDiscount = Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[BPValue - 1]["ED007_ValueBasedDiscount"]);
                                                SetED007_DiscountPerUnit(valueBasedDiscount);
                                                #region Break
                                                if (discountSchema.GetDiscountType() == "T")
                                                {
                                                    AmtBpartner = BreakCalculation(Util.GetValueOfInt(GetM_Product_ID()), GetCtx().GetAD_Client_ID(),
                                                                 AmtBpartner, bPartner.GetM_DiscountSchema_ID(), Util.GetValueOfDecimal(bPartner.GetFlatDiscount()), GetQtyEntered());
                                                }
                                                #endregion
                                                #region Value Based Discount
                                                if (Util.GetValueOfString(bPartner.GetED007_DiscountCalculation()) == "C1")
                                                {
                                                    valueBasedDiscount = Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[BPValue - 1]["ED007_ValueBasedDiscount"]);
                                                    SetED007_DiscountPerUnit(valueBasedDiscount);
                                                    //AmtBpartner = Decimal.Subtract(AmtBpartner, Decimal.Multiply(valueBasedDiscount, QtyOrdered.Value));
                                                    AmtBpartner = Decimal.Subtract(AmtBpartner, Decimal.Multiply(valueBasedDiscount, GetQtyEntered()));
                                                }
                                                #endregion
                                                SetED007_DiscountAmount(Decimal.Round(Decimal.Divide(Decimal.Multiply(Util.GetValueOfDecimal(order.GetED007_DiscountPercent()), AmtBpartner), 100), precision));
                                                SetLineNetAmt(Decimal.Subtract(AmtBpartner, Util.GetValueOfDecimal(GetED007_DiscountAmount())));

                                                //if (enforce && Decimal.ToDouble(PriceLimit.Value) != 0.0
                                                //      && Util.GetValueOfDecimal(mTab.GetValue("LineNetAmt")).CompareTo(Decimal.Round(Decimal.Multiply(PriceLimit.Value, QtyOrdered.Value), precision)) < 0)
                                                if (enforce && Decimal.ToDouble(GetPriceLimit()) != 0.0
                                                     && Util.GetValueOfDecimal(GetLineNetAmt()).CompareTo(Decimal.Round(Decimal.Multiply(GetPriceLimit(), GetQtyEntered()), precision)) < 0)
                                                {
                                                    SetPriceActual(GetPriceLimit());
                                                    SetPriceEntered(GetPriceLimit());
                                                    if (GetPriceList() != 0)
                                                    {
                                                        Decimal Discount = VAdvantage.Utility.Util.GetValueOfDecimal(((Decimal.ToDouble(GetPriceList()) - Decimal.ToDouble(GetPriceLimit())) / Decimal.ToDouble(GetPriceList()) * 100.0));
                                                        if (Env.Scale(Discount) > 2)
                                                        {
                                                            Discount = Decimal.Round(Discount, 2);
                                                        }
                                                        SetDiscount(Discount);
                                                    }
                                                    //Amit 27-nov-2014
                                                    //LineNetAmt = Decimal.Multiply(QtyOrdered.Value, PriceLimit.Value);
                                                    LineNetAmt = Decimal.Multiply(GetQtyEntered(), GetPriceLimit());
                                                    //Amit
                                                    if (Env.Scale(LineNetAmt) > precision)
                                                        LineNetAmt = Decimal.Round(LineNetAmt, precision);//, MidpointRounding.AwayFromZero);
                                                    SetLineNetAmt(LineNetAmt);
                                                    SetED007_DscuntlineAmt(LineNetAmt);
                                                    SetED007_DiscountAmount(0);
                                                    SetED007_DiscountPercentage1(0);
                                                    SetED007_DiscountPercentage2(0);
                                                    SetED007_DiscountPercentage3(0);
                                                    SetED007_DiscountPercentage4(0);
                                                    SetED007_DiscountPercentage5(0);
                                                    SetED007_DiscountPercent(0);
                                                }
                                                else
                                                {
                                                    // mTab.SetValue("LineNetAmt", AmtBpartner);
                                                    SetED007_DscuntlineAmt(AmtBpartner);
                                                    if (GetQtyOrdered() > 0 && GetPriceList() > 0 && GetQtyEntered() > 0)
                                                    {
                                                        //mTab.SetValue("Discount", Decimal.Round(Decimal.Multiply(Decimal.Divide(Decimal.Subtract(Decimal.Multiply(PriceList.Value, QtyOrdered.Value), AmtBpartner), Decimal.Multiply(PriceList.Value, QtyOrdered.Value)), 100), precision));
                                                        SetDiscount(Decimal.Round(Decimal.Multiply(Decimal.Divide(Decimal.Subtract(Decimal.Multiply(GetPriceList(), GetQtyEntered()), AmtBpartner), Decimal.Multiply(GetPriceList(), GetQtyEntered())), 100), precision));
                                                    }
                                                }
                                            }
                                            #endregion
                                            #region Set value when record match for Business Partner Group And Product Category
                                            else if (PCatAndBpGrpValue > 0)
                                            {
                                                SetED007_DiscountPercentage1(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[PCatAndBpGrpValue - 1]["ED007_DiscountPercentage1"]));
                                                SetED007_DiscountPercentage2(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[PCatAndBpGrpValue - 1]["ED007_DiscountPercentage2"]));
                                                SetED007_DiscountPercentage3(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[PCatAndBpGrpValue - 1]["ED007_DiscountPercentage3"]));
                                                SetED007_DiscountPercentage4(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[PCatAndBpGrpValue - 1]["ED007_DiscountPercentage4"]));
                                                SetED007_DiscountPercentage5(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[PCatAndBpGrpValue - 1]["ED007_DiscountPercentage5"]));
                                                AmtPcatAndBpGrp = Decimal.Round(AmtPcatAndBpGrp, precision);
                                                valueBasedDiscount = Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[PCatAndBpGrpValue - 1]["ED007_ValueBasedDiscount"]);
                                                SetED007_DiscountPerUnit(valueBasedDiscount);
                                                #region Break
                                                if (discountSchema.GetDiscountType() == "T")
                                                {
                                                    AmtPcatAndBpGrp = BreakCalculation(Util.GetValueOfInt(GetM_Product_ID()), GetCtx().GetAD_Client_ID(),
                                                                 AmtPcatAndBpGrp, bPartner.GetM_DiscountSchema_ID(), Util.GetValueOfDecimal(bPartner.GetFlatDiscount()), GetQtyEntered());
                                                }
                                                #endregion
                                                #region Value Based Discount
                                                if (Util.GetValueOfString(bPartner.GetED007_DiscountCalculation()) == "C1")
                                                {
                                                    valueBasedDiscount = Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[PCatAndBpGrpValue - 1]["ED007_ValueBasedDiscount"]);
                                                    SetED007_DiscountPerUnit(valueBasedDiscount);
                                                    //AmtPcatAndBpGrp = Decimal.Subtract(AmtPcatAndBpGrp, Decimal.Multiply(valueBasedDiscount, QtyOrdered.Value));
                                                    AmtPcatAndBpGrp = Decimal.Subtract(AmtPcatAndBpGrp, Decimal.Multiply(valueBasedDiscount, GetQtyEntered()));
                                                }
                                                #endregion
                                                SetED007_DiscountAmount(Decimal.Round(Decimal.Divide(Decimal.Multiply(Util.GetValueOfDecimal(order.GetED007_DiscountPercent()), AmtPcatAndBpGrp), 100), precision));
                                                SetLineNetAmt(Decimal.Subtract(AmtPcatAndBpGrp, Util.GetValueOfDecimal(GetED007_DiscountAmount())));

                                                //if (enforce && Decimal.ToDouble(PriceLimit.Value) != 0.0
                                                //     && Util.GetValueOfDecimal(mTab.GetValue("LineNetAmt")).CompareTo(Decimal.Round(Decimal.Multiply(PriceLimit.Value, QtyOrdered.Value), precision)) < 0)
                                                if (enforce && Decimal.ToDouble(GetPriceLimit()) != 0.0
                                                    && Util.GetValueOfDecimal(GetLineNetAmt()).CompareTo(Decimal.Round(Decimal.Multiply(GetPriceLimit(), GetQtyEntered()), precision)) < 0)
                                                {
                                                    SetPriceActual(GetPriceLimit());
                                                    SetPriceEntered(GetPriceLimit());

                                                    if (GetPriceList() != 0)
                                                    {
                                                        Decimal Discount = VAdvantage.Utility.Util.GetValueOfDecimal(((Decimal.ToDouble(GetPriceList()) - Decimal.ToDouble(GetPriceLimit())) / Decimal.ToDouble(GetPriceList()) * 100.0));
                                                        if (Env.Scale(Discount) > 2)
                                                        {
                                                            Discount = Decimal.Round(Discount, 2);
                                                        }
                                                        SetDiscount(Discount);
                                                    }
                                                    //Amit 27-nov-2014
                                                    //LineNetAmt = Decimal.Multiply(QtyOrdered.Value, PriceLimit.Value);
                                                    LineNetAmt = Decimal.Multiply(GetQtyEntered(), GetPriceLimit());
                                                    //Amit
                                                    if (Env.Scale(LineNetAmt) > precision)
                                                        LineNetAmt = Decimal.Round(LineNetAmt, precision);//, MidpointRounding.AwayFromZero);
                                                    // log.Info("LineNetAmt=" + LineNetAmt);
                                                    SetLineNetAmt(LineNetAmt);
                                                    SetED007_DscuntlineAmt(LineNetAmt);
                                                    SetED007_DiscountAmount(0);
                                                    SetED007_DiscountPercentage1(0);
                                                    SetED007_DiscountPercentage2(0);
                                                    SetED007_DiscountPercentage3(0);
                                                    SetED007_DiscountPercentage4(0);
                                                    SetED007_DiscountPercentage5(0);
                                                    SetED007_DiscountPercent(0);
                                                }
                                                else
                                                {
                                                    // mTab.SetValue("LineNetAmt", AmtPcatAndBpGrp);
                                                    SetED007_DscuntlineAmt(AmtPcatAndBpGrp);
                                                    if (GetQtyOrdered() > 0 && GetPriceList() > 0 && GetQtyEntered() > 0)
                                                    {
                                                        //mTab.SetValue("Discount", Decimal.Round(Decimal.Multiply(Decimal.Divide(Decimal.Subtract(Decimal.Multiply(PriceList.Value, QtyOrdered.Value), AmtPcatAndBpGrp), Decimal.Multiply(PriceList.Value, QtyOrdered.Value)), 100), precision));
                                                        SetDiscount(Decimal.Round(Decimal.Multiply(Decimal.Divide(Decimal.Subtract(Decimal.Multiply(GetPriceList(), GetQtyEntered()), AmtPcatAndBpGrp), Decimal.Multiply(GetPriceList(), GetQtyEntered())), 100), precision));
                                                    }
                                                }
                                            }
                                            #endregion
                                            #region Set value when record match for  Product Category
                                            else if (PCatValue > 0)
                                            {
                                                SetED007_DiscountPercentage1(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[PCatValue - 1]["ED007_DiscountPercentage1"]));
                                                SetED007_DiscountPercentage2(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[PCatValue - 1]["ED007_DiscountPercentage2"]));
                                                SetED007_DiscountPercentage3(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[PCatValue - 1]["ED007_DiscountPercentage3"]));
                                                SetED007_DiscountPercentage4(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[PCatValue - 1]["ED007_DiscountPercentage4"]));
                                                SetED007_DiscountPercentage5(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[PCatValue - 1]["ED007_DiscountPercentage5"]));
                                                AmtPCategory = Decimal.Round(AmtPCategory, precision);
                                                valueBasedDiscount = Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[PCatValue - 1]["ED007_ValueBasedDiscount"]);
                                                SetED007_DiscountPerUnit(valueBasedDiscount);
                                                #region Break
                                                if (discountSchema.GetDiscountType() == "T")
                                                {
                                                    AmtPCategory = BreakCalculation(Util.GetValueOfInt(GetM_Product_ID()), GetCtx().GetAD_Client_ID(),
                                                                 AmtPCategory, bPartner.GetM_DiscountSchema_ID(), Util.GetValueOfDecimal(bPartner.GetFlatDiscount()), GetQtyEntered());
                                                }
                                                #endregion
                                                #region Value Based Discount
                                                if (Util.GetValueOfString(bPartner.GetED007_DiscountCalculation()) == "C1")
                                                {
                                                    valueBasedDiscount = Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[PCatValue - 1]["ED007_ValueBasedDiscount"]);
                                                    SetED007_DiscountPerUnit(valueBasedDiscount);
                                                    //AmtPCategory = Decimal.Subtract(AmtPCategory, Decimal.Multiply(valueBasedDiscount, QtyOrdered.Value));
                                                    AmtPCategory = Decimal.Subtract(AmtPCategory, Decimal.Multiply(valueBasedDiscount, GetQtyEntered()));
                                                }
                                                #endregion
                                                SetED007_DiscountAmount(Decimal.Round(Decimal.Divide(Decimal.Multiply(Util.GetValueOfDecimal(order.GetED007_DiscountPercent()), AmtPCategory), 100), precision));
                                                SetLineNetAmt(Decimal.Subtract(AmtPCategory, Util.GetValueOfDecimal(GetED007_DiscountAmount())));

                                                //if (enforce && Decimal.ToDouble(PriceLimit.Value) != 0.0
                                                //    && Util.GetValueOfDecimal(mTab.GetValue("LineNetAmt")).CompareTo(Decimal.Round(Decimal.Multiply(PriceLimit.Value, QtyOrdered.Value), precision)) < 0)
                                                if (enforce && Decimal.ToDouble(GetPriceLimit()) != 0.0
                                                 && Util.GetValueOfDecimal(GetLineNetAmt()).CompareTo(Decimal.Round(Decimal.Multiply(GetPriceLimit(), GetQtyEntered()), precision)) < 0)
                                                {
                                                    SetPriceActual(GetPriceLimit());
                                                    SetPriceEntered(GetPriceLimit());

                                                    if (GetPriceList() != 0)
                                                    {
                                                        Decimal Discount = VAdvantage.Utility.Util.GetValueOfDecimal(((Decimal.ToDouble(GetPriceList()) - Decimal.ToDouble(GetPriceLimit())) / Decimal.ToDouble(GetPriceList()) * 100.0));
                                                        if (Env.Scale(Discount) > 2)
                                                        {
                                                            Discount = Decimal.Round(Discount, 2);
                                                        }
                                                        SetDiscount(Discount);
                                                    }
                                                    //Amit 27-nov-2014
                                                    //LineNetAmt = Decimal.Multiply(QtyOrdered.Value, PriceLimit.Value);
                                                    LineNetAmt = Decimal.Multiply(GetQtyEntered(), GetPriceLimit());
                                                    //Amit
                                                    if (Env.Scale(LineNetAmt) > precision)
                                                        LineNetAmt = Decimal.Round(LineNetAmt, precision);//, MidpointRounding.AwayFromZero);
                                                    //  log.Info("LineNetAmt=" + LineNetAmt);
                                                    SetLineNetAmt(LineNetAmt);
                                                    SetED007_DscuntlineAmt(LineNetAmt);
                                                    SetED007_DiscountAmount(0);
                                                    SetED007_DiscountPercentage1(0);
                                                    SetED007_DiscountPercentage2(0);
                                                    SetED007_DiscountPercentage3(0);
                                                    SetED007_DiscountPercentage4(0);
                                                    SetED007_DiscountPercentage5(0);
                                                    SetED007_DiscountPercent(0);
                                                }
                                                else
                                                {
                                                    // mTab.SetValue("LineNetAmt", AmtPCategory);
                                                    SetED007_DscuntlineAmt(AmtPCategory);
                                                    if (GetQtyOrdered() > 0 && GetPriceList() > 0 && GetQtyEntered() > 0)
                                                    {
                                                        //mTab.SetValue("Discount", Decimal.Round(Decimal.Multiply(Decimal.Divide(Decimal.Subtract(Decimal.Multiply(PriceList.Value, QtyOrdered.Value), AmtPCategory), Decimal.Multiply(PriceList.Value, QtyOrdered.Value)), 100), precision));
                                                        SetDiscount(Decimal.Round(Decimal.Multiply(Decimal.Divide(Decimal.Subtract(Decimal.Multiply(GetPriceList(), GetQtyEntered()), AmtPCategory), Decimal.Multiply(GetPriceList(), GetQtyEntered())), 100), precision));
                                                    }
                                                }
                                            }
                                            #endregion
                                            #region Set value when record match for Business Partner Group
                                            else if (BPGrpValue > 0)
                                            {
                                                SetED007_DiscountPercentage1(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[BPGrpValue - 1]["ED007_DiscountPercentage1"]));
                                                SetED007_DiscountPercentage2(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[BPGrpValue - 1]["ED007_DiscountPercentage2"]));
                                                SetED007_DiscountPercentage3(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[BPGrpValue - 1]["ED007_DiscountPercentage3"]));
                                                SetED007_DiscountPercentage4(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[BPGrpValue - 1]["ED007_DiscountPercentage4"]));
                                                SetED007_DiscountPercentage5(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[BPGrpValue - 1]["ED007_DiscountPercentage5"]));
                                                AmtBpGroup = Decimal.Round(AmtBpGroup, precision);
                                                valueBasedDiscount = Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[BPGrpValue - 1]["ED007_ValueBasedDiscount"]);
                                                SetED007_DiscountPerUnit(valueBasedDiscount);
                                                #region Break
                                                if (discountSchema.GetDiscountType() == "T")
                                                {
                                                    AmtBpGroup = BreakCalculation(Util.GetValueOfInt(GetM_Product_ID()), GetCtx().GetAD_Client_ID(),
                                                                 AmtBpGroup, bPartner.GetM_DiscountSchema_ID(), Util.GetValueOfDecimal(bPartner.GetFlatDiscount()), GetQtyEntered());
                                                }
                                                #endregion
                                                #region Value Based Discount
                                                if (Util.GetValueOfString(bPartner.GetED007_DiscountCalculation()) == "C1")
                                                {
                                                    valueBasedDiscount = Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[BPGrpValue - 1]["ED007_ValueBasedDiscount"]);
                                                    SetED007_DiscountPerUnit(valueBasedDiscount);
                                                    //AmtBpGroup = Decimal.Subtract(AmtBpGroup, Decimal.Multiply(valueBasedDiscount, QtyOrdered.Value));
                                                    AmtBpGroup = Decimal.Subtract(AmtBpGroup, Decimal.Multiply(valueBasedDiscount, GetQtyEntered()));
                                                }
                                                #endregion
                                                SetED007_DiscountAmount(Decimal.Round(Decimal.Divide(Decimal.Multiply(Util.GetValueOfDecimal(order.GetED007_DiscountPercent()), AmtBpGroup), 100), precision));
                                                SetLineNetAmt(Decimal.Subtract(AmtBpGroup, Util.GetValueOfDecimal(GetED007_DiscountAmount())));

                                                //if (enforce && Decimal.ToDouble(PriceLimit.Value) != 0.0
                                                // && Util.GetValueOfDecimal(mTab.GetValue("LineNetAmt")).CompareTo(Decimal.Round(Decimal.Multiply(PriceLimit.Value, QtyOrdered.Value), precision)) < 0)
                                                if (enforce && Decimal.ToDouble(GetPriceLimit()) != 0.0
                                                 && Util.GetValueOfDecimal(GetLineNetAmt()).CompareTo(Decimal.Round(Decimal.Multiply(GetPriceLimit(), GetQtyEntered()), precision)) < 0)
                                                {
                                                    SetPriceActual(GetPriceLimit());
                                                    SetPriceEntered(GetPriceLimit());
                                                    if (GetPriceList() != 0)
                                                    {
                                                        Decimal Discount = VAdvantage.Utility.Util.GetValueOfDecimal(((Decimal.ToDouble(GetPriceList()) - Decimal.ToDouble(GetPriceLimit())) / Decimal.ToDouble(GetPriceList()) * 100.0));
                                                        if (Env.Scale(Discount) > 2)
                                                        {
                                                            Discount = Decimal.Round(Discount, 2);
                                                        }
                                                        SetDiscount(Discount);
                                                    }
                                                    //Amit 27-nov-2014
                                                    //LineNetAmt = Decimal.Multiply(QtyOrdered.Value, PriceLimit.Value);
                                                    LineNetAmt = Decimal.Multiply(GetQtyEntered(), GetPriceLimit());
                                                    //Amit
                                                    if (Env.Scale(LineNetAmt) > precision)
                                                        LineNetAmt = Decimal.Round(LineNetAmt, precision);//, MidpointRounding.AwayFromZero);
                                                    //log.Info("LineNetAmt=" + LineNetAmt);
                                                    SetLineNetAmt(LineNetAmt);
                                                    SetED007_DscuntlineAmt(LineNetAmt);
                                                    SetED007_DiscountAmount(0);
                                                    SetED007_DiscountPercentage1(0);
                                                    SetED007_DiscountPercentage2(0);
                                                    SetED007_DiscountPercentage3(0);
                                                    SetED007_DiscountPercentage4(0);
                                                    SetED007_DiscountPercentage5(0);
                                                    SetED007_DiscountPercent(0);
                                                }
                                                else
                                                {
                                                    // mTab.SetValue("LineNetAmt", AmtBpGroup);
                                                    SetED007_DscuntlineAmt(AmtBpGroup);
                                                    if (GetQtyOrdered() > 0 && GetPriceList() > 0 && GetQtyEntered() > 0)
                                                    {
                                                        //mTab.SetValue("Discount", Decimal.Round(Decimal.Multiply(Decimal.Divide(Decimal.Subtract(Decimal.Multiply(PriceList.Value, QtyOrdered.Value), AmtBpGroup), Decimal.Multiply(PriceList.Value, QtyOrdered.Value)), 100), precision));
                                                        SetDiscount(Decimal.Round(Decimal.Multiply(Decimal.Divide(Decimal.Subtract(Decimal.Multiply(GetPriceList(), GetQtyEntered()), AmtBpGroup), Decimal.Multiply(GetPriceList(), GetQtyEntered())), 100), precision));
                                                    }
                                                }
                                            }
                                            #endregion
                                            #region Set Value when No Exception
                                            else if (noException > 0)
                                            {
                                                SetED007_DiscountPercentage1(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[noException - 1]["ED007_DiscountPercentage1"]));
                                                SetED007_DiscountPercentage2(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[noException - 1]["ED007_DiscountPercentage2"]));
                                                SetED007_DiscountPercentage3(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[noException - 1]["ED007_DiscountPercentage3"]));
                                                SetED007_DiscountPercentage4(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[noException - 1]["ED007_DiscountPercentage4"]));
                                                SetED007_DiscountPercentage5(Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[noException - 1]["ED007_DiscountPercentage5"]));
                                                AmtNoException = Decimal.Round(AmtNoException, precision);
                                                valueBasedDiscount = Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[noException - 1]["ED007_ValueBasedDiscount"]);
                                                SetED007_DiscountPerUnit(valueBasedDiscount);
                                                #region Break
                                                if (discountSchema.GetDiscountType() == "T")
                                                {
                                                    AmtNoException = BreakCalculation(Util.GetValueOfInt(GetM_Product_ID()), GetCtx().GetAD_Client_ID(),
                                                                  AmtNoException, bPartner.GetM_DiscountSchema_ID(), Util.GetValueOfDecimal(bPartner.GetFlatDiscount()), GetQtyEntered());
                                                }
                                                #endregion
                                                #region Value Based Discount
                                                if (Util.GetValueOfString(bPartner.GetED007_DiscountCalculation()) == "C1")
                                                {
                                                    valueBasedDiscount = Util.GetValueOfDecimal(dsDiscountCombination.Tables[0].Rows[noException - 1]["ED007_ValueBasedDiscount"]);
                                                    SetED007_DiscountPerUnit(valueBasedDiscount);
                                                    //AmtNoException = Decimal.Subtract(AmtNoException, Decimal.Multiply(valueBasedDiscount, QtyOrdered.Value));
                                                    AmtNoException = Decimal.Subtract(AmtNoException, Decimal.Multiply(valueBasedDiscount, GetQtyEntered()));
                                                }
                                                #endregion
                                                SetED007_DiscountAmount(Decimal.Round(Decimal.Divide(Decimal.Multiply(Util.GetValueOfDecimal(order.GetED007_DiscountPercent()), AmtNoException), 100), precision));
                                                SetLineNetAmt(Decimal.Subtract(AmtNoException, Util.GetValueOfDecimal(GetED007_DiscountAmount())));

                                                //if (enforce && Decimal.ToDouble(PriceLimit.Value) != 0.0
                                                // && Util.GetValueOfDecimal(mTab.GetValue("LineNetAmt")).CompareTo(Decimal.Round(Decimal.Multiply(PriceLimit.Value, QtyOrdered.Value), precision)) < 0)
                                                if (enforce && Decimal.ToDouble(GetPriceLimit()) != 0.0
                                              && Util.GetValueOfDecimal(GetLineNetAmt()).CompareTo(Decimal.Round(Decimal.Multiply(GetPriceLimit(), GetQtyEntered()), precision)) < 0)
                                                {
                                                    SetPriceActual(GetPriceLimit());
                                                    SetPriceEntered(GetPriceLimit());

                                                    if (GetPriceList() != 0)
                                                    {
                                                        Decimal Discount = VAdvantage.Utility.Util.GetValueOfDecimal(((Decimal.ToDouble(GetPriceList()) - Decimal.ToDouble(GetPriceLimit())) / Decimal.ToDouble(GetPriceList()) * 100.0));
                                                        if (Env.Scale(Discount) > 2)
                                                        {
                                                            Discount = Decimal.Round(Discount, 2);
                                                        }
                                                        SetDiscount(Discount);
                                                    }
                                                    //Amit 27-nov-2014
                                                    //LineNetAmt = Decimal.Multiply(QtyOrdered.Value, PriceLimit.Value);
                                                    LineNetAmt = Decimal.Multiply(GetQtyEntered(), GetPriceLimit());
                                                    //Amit
                                                    if (Env.Scale(LineNetAmt) > precision)
                                                        LineNetAmt = Decimal.Round(LineNetAmt, precision);//, MidpointRounding.AwayFromZero);
                                                    //log.Info("LineNetAmt=" + LineNetAmt);
                                                    SetLineNetAmt(LineNetAmt);
                                                    SetED007_DscuntlineAmt(LineNetAmt);
                                                    SetED007_DiscountAmount(0);
                                                    SetED007_DiscountPercentage1(0);
                                                    SetED007_DiscountPercentage2(0);
                                                    SetED007_DiscountPercentage3(0);
                                                    SetED007_DiscountPercentage4(0);
                                                    SetED007_DiscountPercentage5(0);
                                                    SetED007_DiscountPercent(0);
                                                }
                                                else
                                                {
                                                    // mTab.SetValue("LineNetAmt", AmtBpGroup);
                                                    SetED007_DscuntlineAmt(AmtNoException);
                                                    if (GetQtyOrdered() > 0 && GetPriceList() > 0 && GetQtyEntered() > 0)
                                                    {
                                                        //mTab.SetValue("Discount", Decimal.Round(Decimal.Multiply(Decimal.Divide(Decimal.Subtract(Decimal.Multiply(PriceList.Value, QtyOrdered.Value), AmtNoException), Decimal.Multiply(PriceList.Value, QtyOrdered.Value)), 100), precision));
                                                        SetDiscount(Decimal.Round(Decimal.Multiply(Decimal.Divide(Decimal.Subtract(Decimal.Multiply(GetPriceList(), GetQtyEntered()), AmtNoException), Decimal.Multiply(GetPriceList(), GetQtyEntered())), 100), precision));
                                                    }
                                                }
                                            }
                                            #endregion
                                            #region Set Value Based Discount
                                            SetED007_ValueBaseDiscount(Decimal.Round(Decimal.Multiply(Util.GetValueOfDecimal(GetED007_DiscountPerUnit()), GetQtyEntered()), precision));
                                            #endregion
                                        }
                                    }
                                }
                                dsDiscountCombination.Dispose();
                            }
                            else
                            {
                                #region

                                SetED007_DscuntlineAmt(LineNetAmt);
                                SetED007_DiscountAmount(Decimal.Round(Decimal.Divide(Decimal.Multiply(Util.GetValueOfDecimal(order.GetED007_DiscountPercent()), LineNetAmt), 100), precision));
                                SetLineNetAmt(Decimal.Subtract(LineNetAmt, Util.GetValueOfDecimal(GetED007_DiscountAmount())));

                                //if (enforce && Decimal.ToDouble(PriceLimit.Value) != 0.0
                                //        && Util.GetValueOfDecimal(mTab.GetValue("LineNetAmt")).CompareTo(Decimal.Round(Decimal.Multiply(PriceLimit.Value, QtyOrdered.Value), precision)) < 0)
                                if (enforce && Decimal.ToDouble(GetPriceLimit()) != 0.0
                                        && Util.GetValueOfDecimal(GetLineNetAmt()).CompareTo(Decimal.Round(Decimal.Multiply(GetPriceLimit(), GetQtyEntered()), precision)) < 0)
                                {
                                    SetPriceActual(GetPriceLimit());
                                    SetPriceEntered(GetPriceLimit());

                                    if (GetPriceList() != 0)
                                    {
                                        Decimal Discount = VAdvantage.Utility.Util.GetValueOfDecimal(((Decimal.ToDouble(GetPriceList()) - Decimal.ToDouble(GetPriceLimit())) / Decimal.ToDouble(GetPriceList()) * 100.0));
                                        if (Env.Scale(Discount) > 2)
                                        {
                                            Discount = Decimal.Round(Discount, 2);
                                        }
                                        SetDiscount(Discount);
                                    }
                                    //Amit 27-nov-2014
                                    //LineNetAmt = Decimal.Multiply(QtyOrdered.Value, PriceLimit.Value);
                                    LineNetAmt = Decimal.Multiply(GetQtyEntered(), GetPriceLimit());
                                    //Amit
                                    if (Env.Scale(LineNetAmt) > precision)
                                        LineNetAmt = Decimal.Round(LineNetAmt, precision);//, MidpointRounding.AwayFromZero);
                                    // log.Info("LineNetAmt=" + LineNetAmt);
                                    SetLineNetAmt(LineNetAmt);
                                    SetED007_DscuntlineAmt(LineNetAmt);
                                    SetED007_DiscountAmount(0);
                                    SetED007_DiscountPercentage1(0);
                                    SetED007_DiscountPercentage2(0);
                                    SetED007_DiscountPercentage3(0);
                                    SetED007_DiscountPercentage4(0);
                                    SetED007_DiscountPercentage5(0);
                                    SetED007_DiscountPercent(0);
                                }
                                else
                                {
                                    if (GetQtyOrdered() > 0 && GetPriceList() > 0 && GetQtyEntered() > 0)
                                    {
                                        //mTab.SetValue("Discount", Decimal.Round(Decimal.Multiply(Decimal.Divide(Decimal.Subtract(Decimal.Multiply(PriceList.Value, QtyOrdered.Value), LineNetAmt), Decimal.Multiply(PriceList.Value, QtyOrdered.Value)), 100), precision));
                                        SetDiscount(Decimal.Round(Decimal.Multiply(Decimal.Divide(Decimal.Subtract(Decimal.Multiply(GetPriceList(), GetQtyEntered()), LineNetAmt), Decimal.Multiply(GetPriceList(), GetQtyEntered())), 100), precision));
                                    }
                                }
                                #endregion
                            }
                            #endregion
                        }
                        else if ((Util.GetValueOfString(order.IsSOTrx()) == "True" && Util.GetValueOfString(order.IsReturnTrx()) == "False")
                            || (Util.GetValueOfString(order.IsSOTrx()) == "False" && Util.GetValueOfString(order.IsReturnTrx()) == "False"))
                        {
                            #region

                            SetED007_DscuntlineAmt(LineNetAmt);
                            SetED007_DiscountAmount(Decimal.Round(Decimal.Divide(Decimal.Multiply(Util.GetValueOfDecimal(order.GetED007_DiscountPercent()), LineNetAmt), 100), precision));
                            SetLineNetAmt(Decimal.Subtract(LineNetAmt, Util.GetValueOfDecimal(GetED007_DiscountAmount())));

                            //if (enforce && Decimal.ToDouble(PriceLimit.Value) != 0.0
                            //        && Util.GetValueOfDecimal(mTab.GetValue("LineNetAmt")).CompareTo(Decimal.Round(Decimal.Multiply(PriceLimit.Value, QtyOrdered.Value), precision)) < 0)
                            if (enforce && Decimal.ToDouble(GetPriceLimit()) != 0.0
                                    && Util.GetValueOfDecimal(GetLineNetAmt()).CompareTo(Decimal.Round(Decimal.Multiply(GetPriceLimit(), GetQtyEntered()), precision)) < 0)
                            {
                                SetPriceActual(GetPriceLimit());
                                SetPriceEntered(GetPriceLimit());

                                if (GetPriceList() != 0)
                                {
                                    Decimal Discount = VAdvantage.Utility.Util.GetValueOfDecimal(((Decimal.ToDouble(GetPriceList()) - Decimal.ToDouble(GetPriceLimit())) / Decimal.ToDouble(GetPriceList()) * 100.0));
                                    if (Env.Scale(Discount) > 2)
                                    {
                                        Discount = Decimal.Round(Discount, 2);
                                    }
                                    SetDiscount(Discount);
                                }
                                //Amit 27-nov-2014
                                //LineNetAmt = Decimal.Multiply(QtyOrdered.Value, PriceLimit.Value);
                                LineNetAmt = Decimal.Multiply(GetQtyEntered(), GetPriceLimit());
                                //Amit
                                if (Env.Scale(LineNetAmt) > precision)
                                    LineNetAmt = Decimal.Round(LineNetAmt, precision);//, MidpointRounding.AwayFromZero);
                                // log.Info("LineNetAmt=" + LineNetAmt);
                                SetLineNetAmt(LineNetAmt);
                                SetED007_DscuntlineAmt(LineNetAmt);
                                SetED007_DiscountAmount(0);
                                SetED007_DiscountPercentage1(0);
                                SetED007_DiscountPercentage2(0);
                                SetED007_DiscountPercentage3(0);
                                SetED007_DiscountPercentage4(0);
                                SetED007_DiscountPercentage5(0);
                                SetED007_DiscountPercent(0);
                            }
                            else
                            {
                                if (GetQtyOrdered() > 0 && GetPriceList() > 0 && GetQtyEntered() > 0)
                                {
                                    //mTab.SetValue("Discount", Decimal.Round(Decimal.Multiply(Decimal.Divide(Decimal.Subtract(Decimal.Multiply(PriceList.Value, QtyOrdered.Value), LineNetAmt), Decimal.Multiply(PriceList.Value, QtyOrdered.Value)), 100), precision));
                                    SetDiscount(Decimal.Round(Decimal.Multiply(Decimal.Divide(Decimal.Subtract(Decimal.Multiply(GetPriceList(), GetQtyEntered()), LineNetAmt), Decimal.Multiply(GetPriceList(), GetQtyEntered())), 100), precision));
                                }
                            }
                            #endregion
                        }
                    }
                    else if ((Util.GetValueOfString(order.IsSOTrx()) == "True" && Util.GetValueOfString(order.IsReturnTrx()) == "False")
                          || (Util.GetValueOfString(order.IsSOTrx()) == "False" && Util.GetValueOfString(order.IsReturnTrx()) == "False"))
                    {
                        #region

                        #region Set Payment Term Discount Percent
                        SetED007_DiscountPercent(Util.GetValueOfDecimal(order.GetED007_DiscountPercent()));
                        #endregion

                        #region  #region set Value Based Discount Based on Discount Calculation
                        // mTab.SetValue("ED007_ValueBaseDiscount", Decimal.Round(Decimal.Multiply(Util.GetValueOfDecimal(mTab.GetValue("ED007_DiscountPerUnit")), QtyOrdered.Value), precision));
                        SetED007_ValueBaseDiscount(Decimal.Round(Decimal.Multiply(Util.GetValueOfDecimal(GetED007_DiscountPerUnit()), GetQtyEntered()), precision));
                        #endregion

                        Decimal ReLineNetAmount = 0;
                        //ReLineNetAmount = Decimal.Round(Decimal.Multiply(Util.GetValueOfDecimal(mTab.GetValue("PriceActual")), QtyOrdered.Value), precision);  // Total Line Net Amount
                        ReLineNetAmount = Decimal.Round(Decimal.Multiply(Util.GetValueOfDecimal(GetPriceEntered()), GetQtyEntered()), precision);  // Total Line Net Amount

                        #region Value Based Discount
                        if (Util.GetValueOfString(bPartner.GetED007_DiscountCalculation()) == "C2")
                        {
                            ReLineNetAmount = Decimal.Subtract(ReLineNetAmount, Util.GetValueOfDecimal(GetED007_ValueBaseDiscount()));
                        }
                        #endregion

                        if (Util.GetValueOfDecimal(GetED007_DiscountPercentage1()) > 0)
                        {
                            ReLineNetAmount = Decimal.Subtract(ReLineNetAmount, Decimal.Divide(Decimal.Multiply(ReLineNetAmount, Util.GetValueOfDecimal(GetED007_DiscountPercentage1())), 100));
                        }
                        if (Util.GetValueOfDecimal(GetED007_DiscountPercentage2()) > 0)
                        {
                            ReLineNetAmount = Decimal.Subtract(ReLineNetAmount, Decimal.Divide(Decimal.Multiply(ReLineNetAmount, Util.GetValueOfDecimal(GetED007_DiscountPercentage2())), 100));
                        }
                        if (Util.GetValueOfDecimal(GetED007_DiscountPercentage3()) > 0)
                        {
                            ReLineNetAmount = Decimal.Subtract(ReLineNetAmount, Decimal.Divide(Decimal.Multiply(ReLineNetAmount, Util.GetValueOfDecimal(GetED007_DiscountPercentage3())), 100));
                        }
                        if (Util.GetValueOfDecimal(GetED007_DiscountPercentage4()) > 0)
                        {
                            ReLineNetAmount = Decimal.Subtract(ReLineNetAmount, Decimal.Divide(Decimal.Multiply(ReLineNetAmount, Util.GetValueOfDecimal(GetED007_DiscountPercentage4())), 100));
                        }
                        if (Util.GetValueOfDecimal(GetED007_DiscountPercentage5()) > 0)
                        {
                            ReLineNetAmount = Decimal.Subtract(ReLineNetAmount, Decimal.Divide(Decimal.Multiply(ReLineNetAmount, Util.GetValueOfDecimal(GetED007_DiscountPercentage5())), 100));
                        }
                        ReLineNetAmount = Decimal.Round(ReLineNetAmount, precision);
                        #region Break
                        if (discountSchema.GetDiscountType() == "T")
                        {
                            ReLineNetAmount = BreakCalculation(Util.GetValueOfInt(GetM_Product_ID()), GetCtx().GetAD_Client_ID(),
                                                                 ReLineNetAmount, bPartner.GetM_DiscountSchema_ID(), Util.GetValueOfDecimal(bPartner.GetFlatDiscount()), GetQtyEntered());
                        }
                        #endregion

                        #region Value Based Discount
                        if (Util.GetValueOfString(bPartner.GetED007_DiscountCalculation()) == "C1")
                        {
                            ReLineNetAmount = Decimal.Subtract(ReLineNetAmount, Util.GetValueOfDecimal(GetED007_ValueBaseDiscount()));
                        }
                        #endregion

                        SetED007_DiscountAmount(Decimal.Round(Decimal.Divide(Decimal.Multiply(Util.GetValueOfDecimal(order.GetED007_DiscountPercent()), ReLineNetAmount), 100), precision));
                        SetLineNetAmt(Decimal.Subtract(ReLineNetAmount, Util.GetValueOfDecimal(GetED007_DiscountAmount())));

                        //if (enforce && Decimal.ToDouble(PriceLimit.Value) != 0.0
                        //            && Util.GetValueOfDecimal(mTab.GetValue("LineNetAmt")).CompareTo(Decimal.Round(Decimal.Multiply(PriceLimit.Value, QtyOrdered.Value), precision)) < 0)
                        if (enforce && Decimal.ToDouble(GetPriceLimit()) != 0.0
                                  && Util.GetValueOfDecimal(GetLineNetAmt()).CompareTo(Decimal.Round(Decimal.Multiply(GetPriceLimit(), GetQtyEntered()), precision)) < 0)
                        {
                            SetPriceActual(GetPriceLimit());
                            SetPriceEntered(GetPriceLimit());

                            if (GetPriceList() != 0)
                            {
                                Decimal Discount = VAdvantage.Utility.Util.GetValueOfDecimal(((Decimal.ToDouble(GetPriceList()) - Decimal.ToDouble(GetPriceLimit())) / Decimal.ToDouble(GetPriceList()) * 100.0));
                                if (Env.Scale(Discount) > 2)
                                {
                                    Discount = Decimal.Round(Discount, 2);
                                }
                                SetDiscount(Discount);
                            }
                            //Amit 27-nov-2014
                            //LineNetAmt = Decimal.Multiply(QtyOrdered.Value, PriceLimit.Value);
                            LineNetAmt = Decimal.Multiply(GetQtyEntered(), GetPriceLimit());
                            //Amit
                            if (Env.Scale(LineNetAmt) > precision)
                                LineNetAmt = Decimal.Round(LineNetAmt, precision);//, MidpointRounding.AwayFromZero);
                            // log.Info("LineNetAmt=" + LineNetAmt);
                            SetLineNetAmt(LineNetAmt);
                            SetED007_DscuntlineAmt(LineNetAmt);
                            SetED007_DiscountAmount(0);
                            SetED007_DiscountPercentage1(0);
                            SetED007_DiscountPercentage2(0);
                            SetED007_DiscountPercentage3(0);
                            SetED007_DiscountPercentage4(0);
                            SetED007_DiscountPercentage5(0);
                            SetED007_DiscountPercent(0);
                        }
                        else
                        {
                            // mTab.SetValue("LineNetAmt", ReLineNetAmount);
                            SetED007_DscuntlineAmt(ReLineNetAmount);
                            if (GetQtyOrdered() > 0 && GetPriceList() > 0 && GetQtyEntered() > 0)
                            {
                                //mTab.SetValue("Discount", Decimal.Round(Decimal.Multiply(Decimal.Divide(Decimal.Subtract(Decimal.Multiply(PriceList.Value, QtyOrdered.Value), ReLineNetAmount), Decimal.Multiply(PriceList.Value, QtyOrdered.Value)), 100), precision));
                                SetDiscount(Decimal.Round(Decimal.Multiply(Decimal.Divide(Decimal.Subtract(Decimal.Multiply(GetPriceList(), GetQtyEntered()), ReLineNetAmount), Decimal.Multiply(GetPriceList(), GetQtyEntered())), 100), precision));
                            }
                        }

                        #endregion

                        if (Util.GetValueOfInt(GetM_Product_ID()) <= 0)
                        {
                            SetLineNetAmt(0);
                            SetED007_DscuntlineAmt(0);
                            SetED007_DiscountPercentage1(0);
                            SetED007_DiscountPercentage2(0);
                            SetED007_DiscountPercentage3(0);
                            SetED007_DiscountPercentage4(0);
                            SetED007_DiscountPercentage5(0);
                            SetED007_DiscountPercent(0);
                        }
                    }
                    #endregion
                }
                else if (Env.Scale(LineNetAmt) > GetPrecision())
                {
                    LineNetAmt = Decimal.Round(LineNetAmt, GetPrecision(), MidpointRounding.AwayFromZero);
                    base.SetLineNetAmt(LineNetAmt);
                }
                else
                {
                    base.SetLineNetAmt(LineNetAmt);
                }
            }
        }

        private decimal BreakCalculation(int ProductId, int ClientId, decimal amount, int DiscountSchemaId, decimal FlatDiscount, decimal? QtyEntered)
        {
            StringBuilder query = new StringBuilder();
            decimal amountAfterBreak = amount;
            query.Append(@"SELECT DISTINCT M_Product_Category_ID FROM M_Product WHERE IsActive='Y' AND M_Product_ID = " + ProductId);
            int productCategoryId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
            bool isCalulate = false;

            #region Product Based
            query.Clear();
            query.Append(@"SELECT M_Product_Category_ID , M_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM M_DiscountSchemaBreak WHERE 
                                                                   M_DiscountSchema_ID = " + DiscountSchemaId + " AND M_Product_ID = " + ProductId
                                                                       + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId + "Order BY BreakValue DESC");
            DataSet dsDiscountBreak = new DataSet();
            dsDiscountBreak = DB.ExecuteDataset(query.ToString(), null, null);
            if (dsDiscountBreak != null)
            {
                if (dsDiscountBreak.Tables.Count > 0)
                {
                    if (dsDiscountBreak.Tables[0].Rows.Count > 0)
                    {
                        int m = 0;
                        decimal discountBreakValue = 0;

                        for (m = 0; m < dsDiscountBreak.Tables[0].Rows.Count; m++)
                        {
                            if (QtyEntered.Value.CompareTo(Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakValue"])) < 0)
                            {
                                continue;
                            }
                            if (Util.GetValueOfString(dsDiscountBreak.Tables[0].Rows[0]["IsBPartnerFlatDiscount"]) == "N")
                            {
                                isCalulate = true;
                                discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakDiscount"])), 100));
                                break;
                            }
                            else
                            {
                                isCalulate = true;
                                discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, FlatDiscount), 100));
                                break;
                            }
                        }
                        if (isCalulate)
                        {
                            amountAfterBreak = discountBreakValue;
                            return amountAfterBreak;
                        }
                    }
                }
            }
            #endregion

            #region Product Category Based
            query.Clear();
            query.Append(@"SELECT M_Product_Category_ID , M_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM M_DiscountSchemaBreak WHERE 
                                                                   M_DiscountSchema_ID = " + DiscountSchemaId + " AND M_Product_Category_ID = " + productCategoryId
                                                                       + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId + "Order BY BreakValue DESC");
            dsDiscountBreak.Clear();
            dsDiscountBreak = DB.ExecuteDataset(query.ToString(), null, null);
            if (dsDiscountBreak != null)
            {
                if (dsDiscountBreak.Tables.Count > 0)
                {
                    if (dsDiscountBreak.Tables[0].Rows.Count > 0)
                    {
                        int m = 0;
                        decimal discountBreakValue = 0;

                        for (m = 0; m < dsDiscountBreak.Tables[0].Rows.Count; m++)
                        {
                            if (QtyEntered.Value.CompareTo(Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakValue"])) < 0)
                            {
                                continue;
                            }
                            if (Util.GetValueOfString(dsDiscountBreak.Tables[0].Rows[0]["IsBPartnerFlatDiscount"]) == "N")
                            {
                                isCalulate = true;
                                discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakDiscount"])), 100));
                                break;
                            }
                            else
                            {
                                isCalulate = true;
                                discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, FlatDiscount), 100));
                                break;
                            }
                        }
                        if (isCalulate)
                        {
                            amountAfterBreak = discountBreakValue;
                            return amountAfterBreak;
                        }
                    }
                }
            }
            #endregion

            #region Otherwise
            query.Clear();
            query.Append(@"SELECT M_Product_Category_ID , M_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM M_DiscountSchemaBreak WHERE 
                                                                   M_DiscountSchema_ID = " + DiscountSchemaId + " AND M_Product_Category_ID IS NULL AND m_product_id IS NULL "
                                                                       + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId + "Order BY BreakValue DESC");
            dsDiscountBreak.Clear();
            dsDiscountBreak = DB.ExecuteDataset(query.ToString(), null, null);
            if (dsDiscountBreak != null)
            {
                if (dsDiscountBreak.Tables.Count > 0)
                {
                    if (dsDiscountBreak.Tables[0].Rows.Count > 0)
                    {
                        int m = 0;
                        decimal discountBreakValue = 0;

                        for (m = 0; m < dsDiscountBreak.Tables[0].Rows.Count; m++)
                        {
                            if (QtyEntered.Value.CompareTo(Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakValue"])) < 0)
                            {
                                continue;
                            }
                            if (Util.GetValueOfString(dsDiscountBreak.Tables[0].Rows[0]["IsBPartnerFlatDiscount"]) == "N")
                            {
                                isCalulate = true;
                                discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakDiscount"])), 100));
                                break;
                            }
                            else
                            {
                                isCalulate = true;
                                discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, FlatDiscount), 100));
                                break;
                            }
                        }
                        if (isCalulate)
                        {
                            amountAfterBreak = discountBreakValue;
                            return amountAfterBreak;
                        }
                    }
                }
            }
            #endregion

            return amountAfterBreak;
        }

        private decimal FlatDiscount(int ProductId, int ClientId, decimal amount, int DiscountSchemaId, decimal FlatDiscount, decimal? QtyEntered)
        {
            StringBuilder query = new StringBuilder();
            decimal amountAfterBreak = amount;
            query.Append(@"SELECT DISTINCT M_Product_Category_ID FROM M_Product WHERE IsActive='Y' AND M_Product_ID = " + ProductId);
            int productCategoryId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
            bool isCalulate = false;

            // Is flat Discount
            query.Clear();
            query.Append("SELECT  DiscountType  FROM M_DiscountSchema WHERE "
                      + "M_DiscountSchema_ID = " + DiscountSchemaId + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId);
            string discountType = Util.GetValueOfString(DB.ExecuteScalar(query.ToString()));

            if (discountType == "F")
            {
                isCalulate = true;
                decimal discountBreakValue = (amount - ((amount * FlatDiscount) / 100));
                amountAfterBreak = discountBreakValue;
                return amountAfterBreak;
            }
            else if (discountType == "B")
            {
                #region Product Based
                query.Clear();
                query.Append(@"SELECT M_Product_Category_ID , M_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM M_DiscountSchemaBreak WHERE 
                                                                   M_DiscountSchema_ID = " + DiscountSchemaId + " AND M_Product_ID = " + ProductId
                                                                           + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId + "Order BY BreakValue DESC");
                DataSet dsDiscountBreak = new DataSet();
                dsDiscountBreak = DB.ExecuteDataset(query.ToString(), null, null);
                if (dsDiscountBreak != null)
                {
                    if (dsDiscountBreak.Tables.Count > 0)
                    {
                        if (dsDiscountBreak.Tables[0].Rows.Count > 0)
                        {
                            int m = 0;
                            decimal discountBreakValue = 0;

                            for (m = 0; m < dsDiscountBreak.Tables[0].Rows.Count; m++)
                            {
                                if (QtyEntered.Value.CompareTo(Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakValue"])) < 0)
                                {
                                    continue;
                                }
                                if (Util.GetValueOfString(dsDiscountBreak.Tables[0].Rows[0]["IsBPartnerFlatDiscount"]) == "N")
                                {
                                    isCalulate = true;
                                    discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakDiscount"])), 100));
                                    break;
                                }
                                else
                                {
                                    isCalulate = true;
                                    discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, FlatDiscount), 100));
                                    break;
                                }
                            }
                            if (isCalulate)
                            {
                                amountAfterBreak = discountBreakValue;
                                return amountAfterBreak;
                            }
                        }
                    }
                }
                #endregion

                #region Product Category Based
                query.Clear();
                query.Append(@"SELECT M_Product_Category_ID , M_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM M_DiscountSchemaBreak WHERE 
                                                                   M_DiscountSchema_ID = " + DiscountSchemaId + " AND M_Product_Category_ID = " + productCategoryId
                                                                           + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId + "Order BY BreakValue DESC");
                dsDiscountBreak.Clear();
                dsDiscountBreak = DB.ExecuteDataset(query.ToString(), null, null);
                if (dsDiscountBreak != null)
                {
                    if (dsDiscountBreak.Tables.Count > 0)
                    {
                        if (dsDiscountBreak.Tables[0].Rows.Count > 0)
                        {
                            int m = 0;
                            decimal discountBreakValue = 0;

                            for (m = 0; m < dsDiscountBreak.Tables[0].Rows.Count; m++)
                            {
                                if (QtyEntered.Value.CompareTo(Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakValue"])) < 0)
                                {
                                    continue;
                                }
                                if (Util.GetValueOfString(dsDiscountBreak.Tables[0].Rows[0]["IsBPartnerFlatDiscount"]) == "N")
                                {
                                    isCalulate = true;
                                    discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakDiscount"])), 100));
                                    break;
                                }
                                else
                                {
                                    isCalulate = true;
                                    discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, FlatDiscount), 100));
                                    break;
                                }
                            }
                            if (isCalulate)
                            {
                                amountAfterBreak = discountBreakValue;
                                return amountAfterBreak;
                            }
                        }
                    }
                }
                #endregion

                #region Otherwise
                query.Clear();
                query.Append(@"SELECT M_Product_Category_ID , M_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM M_DiscountSchemaBreak WHERE 
                                                                   M_DiscountSchema_ID = " + DiscountSchemaId + " AND M_Product_Category_ID IS NULL AND m_product_id IS NULL "
                                                                           + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId + "Order BY BreakValue DESC");
                dsDiscountBreak.Clear();
                dsDiscountBreak = DB.ExecuteDataset(query.ToString(), null, null);
                if (dsDiscountBreak != null)
                {
                    if (dsDiscountBreak.Tables.Count > 0)
                    {
                        if (dsDiscountBreak.Tables[0].Rows.Count > 0)
                        {
                            int m = 0;
                            decimal discountBreakValue = 0;

                            for (m = 0; m < dsDiscountBreak.Tables[0].Rows.Count; m++)
                            {
                                if (QtyEntered.Value.CompareTo(Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakValue"])) < 0)
                                {
                                    continue;
                                }
                                if (Util.GetValueOfString(dsDiscountBreak.Tables[0].Rows[0]["IsBPartnerFlatDiscount"]) == "N")
                                {
                                    isCalulate = true;
                                    discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakDiscount"])), 100));
                                    break;
                                }
                                else
                                {
                                    isCalulate = true;
                                    discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, FlatDiscount), 100));
                                    break;
                                }
                            }
                            if (isCalulate)
                            {
                                amountAfterBreak = discountBreakValue;
                                return amountAfterBreak;
                            }
                        }
                    }
                }
                #endregion
            }

            return amountAfterBreak;
        }

        /// <summary>
        /// Get Currency Precision from Currency
        /// </summary>
        /// <returns>precision</returns>
        public int GetPrecision()
        {
            if (_precision != null)
                return (int)_precision;
            //
            if (GetC_Currency_ID() == 0)
            {
                SetOrder(GetParent());
                if (_precision != null)
                    return (int)_precision;
            }
            if (GetC_Currency_ID() != 0)
            {
                MCurrency cur = MCurrency.Get(GetCtx(), GetC_Currency_ID());
                if (cur.Get_ID() != 0)
                {
                    _precision = (int)(cur.GetStdPrecision());
                    return (int)_precision;
                }
            }
            //	Fallback
            String sql = "SELECT C.StdPrecision "
                + "FROM C_Currency c INNER JOIN C_Order x ON (x.C_Currency_ID=c.C_Currency_ID) "
                + "WHERE x.C_Order_ID=" + GetC_Order_ID();

            int i = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql, null, Get_TrxName()));
            _precision = i;
            return (int)_precision;
        }

        /// <summary>
        /// Set Product
        /// </summary>
        /// <param name="product">product</param>
        public void SetProduct(MProduct product)
        {
            _product = product;
            if (_product != null)
            {
                SetM_Product_ID(_product.GetM_Product_ID());
                SetC_UOM_ID(_product.GetC_UOM_ID());
            }
            else
            {
                SetM_Product_ID(0);
                Set_ValueNoCheck("C_UOM_ID", null);
            }
            SetM_AttributeSetInstance_ID(0);
        }

        /// <summary>
        /// Set M_Product_ID
        /// </summary>
        /// <param name="M_Product_ID">product</param>
        /// <param name="setUOM">set also UOM</param>
        public void SetM_Product_ID(int M_Product_ID, bool setUOM)
        {
            if (setUOM)
                SetProduct(MProduct.Get(GetCtx(), M_Product_ID));
            else
                base.SetM_Product_ID(M_Product_ID);
            SetM_AttributeSetInstance_ID(0);
        }

        /// <summary>
        /// Set Product and UOM
        /// </summary>
        /// <param name="M_Product_ID">product</param>
        /// <param name="C_UOM_ID">uom</param>
        public void SetM_Product_ID(int M_Product_ID, int C_UOM_ID)
        {
            base.SetM_Product_ID(M_Product_ID);
            if (C_UOM_ID != 0)
                base.SetC_UOM_ID(C_UOM_ID);
            SetM_AttributeSetInstance_ID(0);
        }

        /// <summary>
        ///Set Product - Callout 
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetM_Product_ID(String oldM_Product_ID, String newM_Product_ID, int windowNo)
        {
            if (newM_Product_ID == null || newM_Product_ID.Length == 0)
            {
                SetM_AttributeSetInstance_ID(0);
                return;
            }
            int M_Product_ID = int.Parse(newM_Product_ID);
            base.SetM_Product_ID(M_Product_ID);
            if (M_Product_ID == 0)
            {
                SetM_AttributeSetInstance_ID(0);
                return;
            }
            // Skip these steps for RMA. These fields are copied over from the orignal order instead.
            if (GetParent().IsReturnTrx())
                return;
            //
            SetC_Charge_ID(0);
            //	Set Attribute
            int M_AttributeSetInstance_ID = GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_AttributeSetInstance_ID");
            if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_Product_ID") == M_Product_ID && M_AttributeSetInstance_ID != 0)
                SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            else
                SetM_AttributeSetInstance_ID(0);

            /*****	Price Calculation see also qty	****/
            int C_BPartner_ID = GetCtx().GetContextAsInt(windowNo, "C_BPartner_ID");
            Decimal Qty = GetQtyOrdered();
            bool IsSOTrx = GetCtx().IsSOTrx(windowNo);
            MProductPricing pp = new MProductPricing(GetAD_Client_ID(), GetAD_Org_ID(),
                    M_Product_ID, C_BPartner_ID, Qty, IsSOTrx);
            //
            int M_PriceList_ID = GetCtx().GetContextAsInt(windowNo, "M_PriceList_ID");
            pp.SetM_PriceList_ID(M_PriceList_ID);
            /** PLV is only accurate if PL selected in header */
            int M_PriceList_Version_ID = GetCtx().GetContextAsInt(windowNo, "M_PriceList_Version_ID");
            pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
            DateTime? orderDate = GetDateOrdered();
            pp.SetPriceDate(orderDate);
            //
            SetPriceList(pp.GetPriceList());
            SetPriceLimit(pp.GetPriceLimit());
            SetPriceActual(pp.GetPriceStd());
            SetPriceEntered(pp.GetPriceStd());
            SetC_Currency_ID(pp.GetC_Currency_ID());
            SetDiscount(pp.GetDiscount());
            SetC_UOM_ID(pp.GetC_UOM_ID());
            SetQtyOrdered(GetQtyEntered());
            //if (p_changeVO != null)
            //{
            //    p_changeVO.setContext(GetCtx(), windowNo, "EnforcePriceLimit", pp.isEnforcePriceLimit());
            //    p_changeVO.setContext(GetCtx(), windowNo, "DiscountSchema", pp.isDiscountSchema());
            //}

            /**********************************Eclips commmented code*******************************/
            //	Check/Update Warehouse Setting
            //	int M_Warehouse_ID = ctx.getContextAsInt( Env.WINDOW_INFO, "M_Warehouse_ID");
            //	Integer wh = (Integer)mTab.getValue("M_Warehouse_ID");
            //	if (wh.intValue() != M_Warehouse_ID)
            //	{
            //		mTab.setValue("M_Warehouse_ID", new Integer(M_Warehouse_ID));
            //		ADialog.warn(,WindowNo, "WarehouseChanged");
            //	}
            /**********************************Eclips commmented code*******************************/


            if (IsSOTrx)
            {
                MProduct product = GetProduct();
                if (product.IsStocked())
                {
                    Decimal qtyOrdered = GetQtyOrdered();
                    int M_Warehouse_ID = GetM_Warehouse_ID();
                    M_AttributeSetInstance_ID = GetM_AttributeSetInstance_ID();
                    Decimal available = (Decimal)MStorage.GetQtyAvailable
                        (M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID, null);
                    if (available == null)
                        available = Env.ZERO;
                    if (available == 0)
                    {
                        //p_changeVO.addError(Msg.GetMsg(GetCtx(), "NoQtyAvailable", "0"));
                    }
                    else if (available.CompareTo(qtyOrdered) < 0)
                    {
                        // p_changeVO.addError(Msg.GetMsg(GetCtx(), "InsufficientQtyAvailable", available.toString()));
                    }
                    else
                    {
                        int C_OrderLine_ID = GetC_OrderLine_ID();
                        Decimal notReserved = MOrderLine.GetNotReserved(GetCtx(),
                            M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID,
                            C_OrderLine_ID);
                        if (notReserved == null)
                            notReserved = Env.ZERO;
                        //BigDecimal total = available.subtract(notReserved);
                        Decimal total = available - notReserved;
                        if (total.CompareTo(qtyOrdered) < 0)
                        {
                            String info = Msg.ParseTranslation(GetCtx(), "@QtyAvailable@=" + available
                                + " - @QtyNotReserved@=" + notReserved + " = " + total);
                            //p_changeVO.addError(Msg.GetMsg(GetCtx(), "InsufficientQtyAvailable", info));
                        }
                    }
                }
            }
            //
            SetTax(windowNo, "M_Product_ID");
        }

        /// <summary>
        /// Get Product
        /// </summary>
        /// <returns>product or null</returns>
        public MProduct GetProduct()
        {
            if (_product == null && GetM_Product_ID() != 0)
                _product = MProduct.Get(GetCtx(), GetM_Product_ID());
            return _product;
        }

        /// <summary>
        /// This function is used for costing calculation
        /// It gives consolidated product cost (taxable amt + tax amount + surcharge amt) based on setting
        /// </summary>
        /// <param name="orderline">order Line reference</param>
        /// <returns>LineNetAmount of Product</returns>
        public Decimal GetProductLineCost(MOrderLine orderline)
        {
            if (orderline == null || orderline.Get_ID() <= 0)
            {
                return 0;
            }

            // Get Taxable amount from orderline
            Decimal amt = orderline.GetTaxAbleAmt();

            // create object of tax - for checking tax to be include in cost or not
            MTax tax = MTax.Get(orderline.GetCtx(), orderline.GetC_Tax_ID());
            if (tax.Get_ColumnIndex("IsIncludeInCost") >= 0)
            {
                // add Tax amount in product cost
                if (tax.IsIncludeInCost())
                {
                    amt += orderline.GetTaxAmt();
                }

                // add Surcharge amount in product cost
                if (tax.Get_ColumnIndex("Surcharge_Tax_ID") >= 0 && tax.GetSurcharge_Tax_ID() > 0)
                {
                    if (MTax.Get(orderline.GetCtx(), tax.GetSurcharge_Tax_ID()).IsIncludeInCost())
                    {
                        amt += orderline.GetSurchargeAmt();
                    }
                }
            }

            // if amount is ZERO, then calculate as usual with Line net amount
            if (amt == 0)
            {
                amt = orderline.GetLineNetAmt();
            }

            return amt;
        }

        /// <summary>
        /// Get Base value for Expected Cost Distribution
        /// </summary>
        /// <param name="CostDistribution">cost Distribution</param>
        /// <returns>base number</returns>
        public Decimal GetBase(String CostDistribution)
        {
            if (MLandedCost.LANDEDCOSTDISTRIBUTION_Costs.Equals(CostDistribution))
            {
                return GetProductLineCost(this);
            }
            else if (MLandedCost.LANDEDCOSTDISTRIBUTION_Line.Equals(CostDistribution))
                return Env.ONE;
            else if (MLandedCost.LANDEDCOSTDISTRIBUTION_Quantity.Equals(CostDistribution))
                return GetQtyOrdered();
            else if (MLandedCost.LANDEDCOSTDISTRIBUTION_Volume.Equals(CostDistribution))
            {
                MProduct product = GetProduct();
                if (product == null)
                {
                    log.Severe("No Product");
                    return Env.ZERO;
                }
                return Decimal.Multiply(GetQtyOrdered(), (Decimal)product.GetVolume());
            }
            else if (MLandedCost.LANDEDCOSTDISTRIBUTION_Weight.Equals(CostDistribution))
            {
                MProduct product = GetProduct();
                if (product == null)
                {
                    log.Severe("No Product");
                    return Env.ZERO;
                }
                return Decimal.Multiply(GetQtyOrdered(), product.GetWeight());
            }
            log.Severe("Invalid Criteria: " + CostDistribution);
            return Env.ZERO;
        }


        /// <summary>
        /// Set M_AttributeSetInstance_ID
        /// </summary>
        /// <param name="M_AttributeSetInstance_ID">id</param>
        public new void SetM_AttributeSetInstance_ID(int M_AttributeSetInstance_ID)
        {
            if (M_AttributeSetInstance_ID == 0)		//	 0 is valid ID
                Set_Value("M_AttributeSetInstance_ID", 0);
            else
                base.SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
        }

        /// <summary>
        /// Set Warehouse
        /// </summary>
        /// <param name="M_Warehouse_ID">warehouse</param>
        public new void SetM_Warehouse_ID(int M_Warehouse_ID)
        {
            if (GetM_Warehouse_ID() > 0 && GetM_Warehouse_ID() != M_Warehouse_ID && !CanChangeWarehouse())
            {
                log.Severe("Ignored - Already Delivered/Invoiced/Reserved");
            }
            else
            {
                base.SetM_Warehouse_ID(M_Warehouse_ID);
            }
        }

        /// <summary>
        ///	Set Partner Location - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetC_BPartner_Location_ID(String oldC_BPartner_Location_ID,
               String newC_BPartner_Location_ID, int windowNo)
        {
            if (newC_BPartner_Location_ID == null || newC_BPartner_Location_ID.Length == 0)
                return;
            int C_BPartner_Location_ID = int.Parse(newC_BPartner_Location_ID);
            if (C_BPartner_Location_ID == 0)
                return;
            //
            base.SetC_BPartner_Location_ID(C_BPartner_Location_ID);
            SetTax(windowNo, "C_BPartner_Location_ID");
        }

        /// <summary>
        ///	SSet UOM - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetC_UOM_ID(String oldC_UOM_ID, String newC_UOM_ID, int windowNo)
        {
            if (newC_UOM_ID == null || newC_UOM_ID.Length == 0)
                return;
            int C_UOM_ID = int.Parse(newC_UOM_ID);
            if (C_UOM_ID == 0)
                return;
            //
            base.SetC_UOM_ID(C_UOM_ID);
            SetQty(windowNo, "C_UOM_ID");
            SetAmt(windowNo, "C_UOM_ID");
        }

        /// <summary>
        ///	Set AttributeSet Instance - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetM_AttributeSetInstance_ID(String oldM_AttributeSetInstance_ID,
                String newM_AttributeSetInstance_ID, int windowNo)
        {
            if (newM_AttributeSetInstance_ID == null || newM_AttributeSetInstance_ID.Length == 0)
                return;
            int M_AttributeSetInstance_ID = int.Parse(newM_AttributeSetInstance_ID);
            if (M_AttributeSetInstance_ID == 0)
                return;
            base.SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            SetQty(windowNo, "M_AttributeSetInstance_ID");
        }

        /// <summary>
        ///	Set Discount - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetDiscount(String oldDiscount, String newDiscount, int windowNo)
        {
            if (newDiscount == null || newDiscount.Length == 0)
                return;
            Decimal Discount = Convert.ToDecimal(newDiscount);
            base.SetDiscount(Discount);
            SetAmt(windowNo, "Discount");
        }

        /// <summary>
        ///	Set priceActual - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetPriceActual(String oldPriceActual, String newPriceActual, int windowNo)
        {
            if (newPriceActual == null || newPriceActual.Length == 0)
                return;
            Decimal priceActual = Convert.ToDecimal(newPriceActual);
            base.SetPriceActual(priceActual);
            SetAmt(windowNo, "priceActual");
        }

        /// <summary>
        ///	Set priceEntered - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetPriceEntered(String oldPriceEntered, String newPriceEntered, int windowNo)
        {
            if (newPriceEntered == null || newPriceEntered.Length == 0)
                return;
            Decimal priceEntered = Convert.ToDecimal(newPriceEntered);
            base.SetPriceEntered(priceEntered);
            SetAmt(windowNo, "priceEntered");
        }

        /// <summary>
        ///	Set PriceList - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetPriceList(String oldPriceList, String newPriceList, int windowNo)
        {
            if (newPriceList == null || newPriceList.Length == 0)
                return;
            Decimal PriceList = Convert.ToDecimal(newPriceList);
            base.SetPriceList(PriceList);
            SetAmt(windowNo, "PriceList");
        }

        /// <summary>
        ///	Set qtyEntered - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetQtyEntered(String oldQtyEntered, String newQtyEntered, int windowNo)
        {
            if (newQtyEntered == null || newQtyEntered.Length == 0)
                return;
            Decimal qtyEntered = Convert.ToDecimal(newQtyEntered);
            base.SetQtyEntered(qtyEntered);
            SetQty(windowNo, "QtyEntered");
            SetAmt(windowNo, "QtyEntered");
        }

        /// <summary>
        ///	Set qtyOrdered - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetQtyOrdered(String oldQtyOrdered, String newQtyOrdered, int windowNo)
        {
            if (newQtyOrdered == null || newQtyOrdered.Length == 0)
                return;
            Decimal qtyOrdered = Convert.ToDecimal(newQtyOrdered);
            base.SetQtyOrdered(qtyOrdered);
            SetQty(windowNo, "QtyOrdered");
            SetAmt(windowNo, "QtyOrdered");
        }

        /// <summary>
        ///	Set Resource Assignment - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetS_ResourceAssignment_ID(String oldS_ResourceAssignment_ID, String newS_ResourceAssignment_ID, int windowNo)
        {
            if (newS_ResourceAssignment_ID == null || newS_ResourceAssignment_ID.Length == 0)
                return;
            int S_ResourceAssignment_ID = int.Parse(newS_ResourceAssignment_ID);
            if (S_ResourceAssignment_ID == 0)
                return;
            //
            base.SetS_ResourceAssignment_ID(S_ResourceAssignment_ID);

            int M_Product_ID = 0;
            String Name = null;
            String Description = null;
            Decimal? Qty = null;
            String sql = "SELECT p.M_Product_ID, ra.Name, ra.Description, ra.Qty "
                + "FROM S_ResourceAssignment ra"
                + " INNER JOIN M_Product p ON (p.S_Resource_ID=ra.S_Resource_ID) "
                + "WHERE ra.S_ResourceAssignment_ID=" + S_ResourceAssignment_ID;
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    M_Product_ID = Utility.Util.GetValueOfInt(dr[0].ToString());//.getInt (1);
                    Name = dr[1].ToString();//.getString(2);
                    Description = dr[2].ToString();//.getString(3);
                    Qty = Utility.Util.GetValueOfDecimal(dr[3].ToString());//.getBigDecimal(4);
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            log.Fine("S_ResourceAssignment_ID=" + S_ResourceAssignment_ID
                    + " - M_Product_ID=" + M_Product_ID);
            if (M_Product_ID != 0)
            {
                SetM_Product_ID(M_Product_ID);
                if (Description != null)
                    Name += " (" + Description + ")";
                if (!".".Equals(Name))
                    SetDescription(Name);
                if (Qty != null)
                {
                    SetQtyOrdered((Decimal)Qty);
                }
            }
        }

        /// <summary>
        /// Set Amount (Callout)
        /// </summary>
        /// <param name="windowNo">window</param>
        /// <param name="columnName">changed column</param>
        /// <returns></returns>
        private bool SetAmt(int windowNo, String columnName)
        {
            int C_UOM_To_ID = GetC_UOM_ID();
            int M_Product_ID = GetM_Product_ID();
            int M_PriceList_ID = GetCtx().GetContextAsInt(windowNo, "M_PriceList_ID");
            int StdPrecision = MPriceList.GetPricePrecision(GetCtx(), M_PriceList_ID);
            Decimal qtyEntered, qtyOrdered, priceEntered, priceActual, PriceLimit, Discount, PriceList;
            //	get values
            qtyEntered = GetQtyEntered();
            qtyOrdered = GetQtyOrdered();
            log.Fine("qtyEntered=" + qtyEntered + ", Ordered=" + qtyOrdered + ", UOM=" + C_UOM_To_ID);
            //
            priceEntered = GetPriceEntered();
            priceActual = GetPriceActual();
            Discount = GetDiscount();
            PriceLimit = GetPriceLimit();
            PriceList = GetPriceList();
            log.Fine("PriceList=" + PriceList + ", Limit=" + PriceLimit + ", Precision=" + StdPrecision);
            log.Fine("priceEntered=" + priceEntered + ", Actual=" + priceActual + ", Discount=" + Discount);

            //	Qty changed - recalc price
            if ((columnName.Equals("QtyOrdered")
                || columnName.Equals("QtyEntered")
                || columnName.Equals("M_Product_ID"))
                && !"N".Equals(GetCtx().GetContext(windowNo, "DiscountSchema")))
            {
                int C_BPartner_ID = GetC_BPartner_ID();
                if (columnName.Equals("QtyEntered"))
                    qtyOrdered = (Decimal)MUOMConversion.ConvertProductTo(GetCtx(), M_Product_ID, C_UOM_To_ID, qtyEntered);
                if (qtyOrdered == null)
                    qtyOrdered = qtyEntered;
                bool IsSOTrx = GetCtx().IsSOTrx(windowNo);
                MProductPricing pp = new MProductPricing(GetAD_Client_ID(), GetAD_Org_ID(),
                        M_Product_ID, C_BPartner_ID, qtyOrdered, IsSOTrx);
                pp.SetM_PriceList_ID(M_PriceList_ID);
                int M_PriceList_Version_ID = GetCtx().GetContextAsInt(windowNo, "M_PriceList_Version_ID");
                pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
                DateTime? date = GetDateOrdered();
                pp.SetPriceDate(date);
                //
                priceEntered = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), M_Product_ID, C_UOM_To_ID, pp.GetPriceStd());
                if (priceEntered == null)
                    priceEntered = pp.GetPriceStd();
                //
                log.Fine("QtyChanged -> priceActual=" + pp.GetPriceStd()
                    + ", priceEntered=" + priceEntered + ", Discount=" + pp.GetDiscount());
                priceActual = pp.GetPriceStd();
                SetPriceActual(priceActual);
                SetDiscount(pp.GetDiscount());
                SetPriceEntered(priceEntered);
                //p_changeVO.setContext(GetCtx(), windowNo, "DiscountSchema", pp.isDiscountSchema());
            }
            else if (columnName.Equals("priceActual"))
            {
                priceEntered = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), M_Product_ID,
                    C_UOM_To_ID, priceActual);
                if (priceEntered == null)
                    priceEntered = priceActual;
                //
                log.Fine("priceActual=" + priceActual
                    + " -> priceEntered=" + priceEntered);
                SetPriceEntered(priceEntered);
            }
            else if (columnName.Equals("priceEntered"))
            {
                priceActual = (Decimal)MUOMConversion.ConvertProductTo(GetCtx(), M_Product_ID,
                    C_UOM_To_ID, priceEntered);
                if (priceActual == null)
                    priceActual = priceEntered;
                //
                log.Fine("priceEntered=" + priceEntered
                  + " -> priceActual=" + priceActual);
                SetPriceActual(priceActual);
            }

            //  Discount entered - Calculate Actual/Entered
            if (columnName.Equals("Discount"))
            {
                //priceActual = new BigDecimal((100.0 - Discount.doubleValue())/ 100.0 * PriceList.doubleValue());
                priceActual = (Decimal)(100.0 - Decimal.ToDouble(Discount) / 100.0 * Decimal.ToDouble(PriceList));
                if (Env.Scale(priceActual) > StdPrecision)
                {
                    priceActual = Decimal.Round(priceActual, StdPrecision, MidpointRounding.AwayFromZero);
                }
                priceEntered = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), M_Product_ID, C_UOM_To_ID, priceActual);
                if (priceEntered == null)
                    priceEntered = priceActual;
                SetPriceActual(priceActual);
                SetPriceEntered(priceEntered);
            }
            //	calculate Discount
            else
            {
                if (PriceList == 0)
                {
                    Discount = Env.ZERO;
                }
                else
                {
                    //Discount = new BigDecimal((PriceList.doubleValue() - priceActual.doubleValue()) / PriceList.doubleValue() * 100.0);
                    Discount = (Decimal)((Decimal.ToDouble(PriceList) - Decimal.ToDouble(priceActual)) / Decimal.ToDouble(PriceList) * 100.0);
                }
                if (Env.Scale(Discount) > 2)
                {
                    Discount = Decimal.Round(Discount, 2, MidpointRounding.AwayFromZero);
                }
                SetDiscount(Discount);
            }
            log.Fine("priceEntered=" + priceEntered + ", Actual=" + priceActual + ", Discount=" + Discount);

            //	Check PriceLimit
            bool epl = "Y".Equals(GetCtx().GetContext(windowNo, "EnforcePriceLimit"));
            bool enforce = epl && GetCtx().IsSOTrx(windowNo);
            if (enforce && MRole.GetDefault(GetCtx()).IsOverwritePriceLimit())
                enforce = false;
            //	Check Price Limit?
            if (enforce && (Double)PriceLimit != 0.0 && priceActual.CompareTo(PriceLimit) < 0)
            {
                priceActual = PriceLimit;
                priceEntered = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), M_Product_ID, C_UOM_To_ID, PriceLimit);
                if (priceEntered == null)
                {
                    priceEntered = PriceLimit;
                }
                log.Fine("(under) priceEntered=" + priceEntered + ", Actual" + PriceLimit);
                SetPriceActual(PriceLimit);
                SetPriceEntered(priceEntered);
                //p_changeVO.addError(Msg.GetMsg(GetCtx(), "UnderLimitPrice"));
                //	Repeat Discount calc
                if (PriceList != 0)
                {
                    //Discount = new BigDecimal((PriceList.doubleValue() - priceActual.doubleValue()) / PriceList.doubleValue() * 100.0);
                    Discount = (Decimal)((Double)PriceList - (Double)priceActual / (Double)PriceList * 100.0);
                    if (Env.Scale(Discount) > 2)
                    {
                        Discount = Decimal.Round(Discount, 2, MidpointRounding.AwayFromZero);
                    }
                    SetDiscount(Discount);
                }
            }
            //	Line Net Amt
            Decimal LineNetAmt = Decimal.Multiply(qtyOrdered, priceActual);
            if (Env.Scale(LineNetAmt) > StdPrecision)
            {
                LineNetAmt = Decimal.Round(LineNetAmt, StdPrecision, MidpointRounding.AwayFromZero);
            }
            log.Info("LineNetAmt=" + LineNetAmt);
            SetLineNetAmt(LineNetAmt);

            //	Calculate Tax Amount for PO
            bool isSOTrx = GetCtx().IsSOTrx(windowNo);
            if (!isSOTrx && Get_ColumnIndex("TaxAmt") > 0)
            {
                Decimal TaxAmt = Env.ZERO;
                if (columnName.Equals("TaxAmt"))
                {
                    TaxAmt = (Decimal)GetTaxAmt();
                }
                else
                {
                    int taxID = GetC_Tax_ID();
                    if (taxID != null)
                    {
                        int C_Tax_ID = taxID;
                        MTax tax = new MTax(GetCtx(), C_Tax_ID, null);
                        TaxAmt = tax.CalculateTax(LineNetAmt, IsTaxIncluded(), StdPrecision);
                        SetTaxAmt(TaxAmt);
                    }
                }
                //	Add it up
                SetLineTotalAmt(Decimal.Add(LineNetAmt, TaxAmt));
            }
            return true;
        }

        /// <summary>
        /// Set Qty (Callout follow-up).
        /// enforces qty UOM relationship
        /// </summary>
        /// <param name="windowNo">window</param>
        /// <param name="columnName">changed column</param>
        /// <returns></returns>
        private bool SetQty(int windowNo, String columnName)
        {
            int M_Product_ID = GetM_Product_ID();
            Decimal qtyOrdered = Env.ZERO;
            //Decimal qtyEntered = null;
            Decimal qtyEntered = Env.ZERO;
            ;
            Decimal priceActual, priceEntered;
            int C_UOM_To_ID = GetC_UOM_ID();
            bool isReturnTrx = GetParent().IsReturnTrx();

            //	No Product
            if (M_Product_ID == 0)
            {
                qtyEntered = GetQtyEntered();
                qtyOrdered = (decimal)qtyEntered;
                SetQtyOrdered(qtyOrdered);
            }
            //	UOM Changed - convert from Entered -> Product
            else if (columnName.Equals("C_UOM_ID") || columnName.Equals("Orig_InOutLine_ID"))
            {
                qtyEntered = GetQtyEntered();
                //Decimal QtyEntered1 = qtyEntered.setScale(MUOM.GetPrecision(GetCtx(), C_UOM_To_ID), BigDecimal.ROUND_HALF_UP);
                Decimal QtyEntered1 = Decimal.Round(qtyEntered, MUOM.GetPrecision(GetCtx(), C_UOM_To_ID), MidpointRounding.AwayFromZero);
                if (qtyEntered.CompareTo(QtyEntered1) != 0)
                {
                    log.Fine("Corrected qtyEntered Scale UOM=" + C_UOM_To_ID
                       + "; qtyEntered=" + qtyEntered + "->" + QtyEntered1);
                    qtyEntered = QtyEntered1;
                    SetQtyEntered(qtyEntered);
                }
                qtyOrdered = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), M_Product_ID, C_UOM_To_ID, qtyEntered);
                if (qtyOrdered == null)
                {
                    qtyOrdered = qtyEntered;
                }
                bool conversion = qtyEntered.CompareTo(qtyOrdered) != 0;
                priceActual = GetPriceActual();
                priceEntered = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), M_Product_ID, C_UOM_To_ID, priceActual);
                if (priceEntered == null)
                {
                    priceEntered = priceActual;
                }
                log.Fine("UOM=" + C_UOM_To_ID
                    + ", qtyEntered/priceActual=" + qtyEntered + "/" + priceActual
                    + " -> " + conversion
                   + " qtyOrdered/priceEntered=" + qtyOrdered + "/" + priceEntered);
                //p_changeVO.setContext(GetCtx(), windowNo, "UOMConversion", conversion);
                SetQtyOrdered(qtyOrdered);
                SetPriceEntered(priceEntered);
            }
            //	qtyEntered changed - calculate qtyOrdered
            else if (columnName.Equals("QtyEntered"))
            {
                qtyEntered = GetQtyEntered();
                Decimal QtyEntered1 = Decimal.Round(qtyEntered, MUOM.GetPrecision(GetCtx(), C_UOM_To_ID), MidpointRounding.AwayFromZero);
                if (qtyEntered.CompareTo(QtyEntered1) != 0)
                {
                    log.Fine("Corrected qtyEntered Scale UOM=" + C_UOM_To_ID
                       + "; qtyEntered=" + qtyEntered + "->" + QtyEntered1);
                    qtyEntered = QtyEntered1;
                    SetQtyEntered(qtyEntered);
                }
                qtyOrdered = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), M_Product_ID, C_UOM_To_ID, qtyEntered);
                if (qtyOrdered == null)
                {
                    qtyOrdered = qtyEntered;
                }
                bool conversion = qtyEntered.CompareTo(qtyOrdered) != 0;
                log.Fine("UOM=" + C_UOM_To_ID
                   + ", qtyEntered=" + qtyEntered
                    + " -> " + conversion
                    + " qtyOrdered=" + qtyOrdered);
                //p_changeVO.setContext(GetCtx(), windowNo, "UOMConversion", conversion);
                SetQtyOrdered(qtyOrdered);
            }
            //	qtyOrdered changed - calculate qtyEntered (should not happen)
            else if (columnName.Equals("QtyOrdered"))
            {
                qtyOrdered = GetQtyOrdered();
                int precision = GetProduct().GetUOMPrecision();
                //BigDecimal QtyOrdered1 = qtyOrdered.setScale(precision, BigDecimal.ROUND_HALF_UP);
                Decimal QtyOrdered1 = Decimal.Round(qtyOrdered, precision, MidpointRounding.AwayFromZero);
                if (qtyOrdered.CompareTo(QtyOrdered1) != 0)
                {
                    log.Fine("Corrected qtyOrdered Scale "
                        + qtyOrdered + "->" + QtyOrdered1);
                    qtyOrdered = QtyOrdered1;
                    SetQtyOrdered(qtyOrdered);
                }
                qtyEntered = (Decimal)MUOMConversion.ConvertProductTo(GetCtx(), M_Product_ID, C_UOM_To_ID, qtyOrdered);
                if (qtyEntered == null)
                {
                    qtyEntered = qtyOrdered;
                }
                bool conversion = qtyOrdered.CompareTo(qtyEntered) != 0;
                log.Fine("UOM=" + C_UOM_To_ID
                    + ", qtyOrdered=" + qtyOrdered
                    + " -> " + conversion
                    + " qtyEntered=" + qtyEntered);
                //p_changeVO.setContext(GetCtx(), windowNo, "UOMConversion", conversion);
                SetQtyEntered(qtyEntered);
            }
            else
            {
                //	qtyEntered = getQtyEntered();
                qtyOrdered = GetQtyOrdered();
            }

            // RMA : Check qty returned is less than qty shipped
            if (M_Product_ID != 0 && isReturnTrx)
            {
                int inOutLine_ID = GetOrig_InOutLine_ID();
                if (inOutLine_ID != 0)
                {
                    MInOutLine inOutLine = new MInOutLine(GetCtx(), inOutLine_ID, null);
                    Decimal shippedQty = inOutLine.GetMovementQty();
                    qtyOrdered = GetQtyOrdered();
                    if (shippedQty.CompareTo(qtyOrdered) < 0)
                    {
                        if (GetCtx().IsSOTrx(windowNo))
                        {
                            // p_changeVO.addError(Msg.GetMsg(GetCtx(), "QtyShippedLessThanQtyReturned", shippedQty));
                        }
                        else
                        {
                            //p_changeVO.addError(Msg.GetMsg(GetCtx(), "QtyReceivedLessThanQtyReturned", shippedQty));
                        }

                        SetQtyOrdered(shippedQty);
                        qtyOrdered = shippedQty;

                        qtyEntered = (Decimal)MUOMConversion.ConvertProductTo(GetCtx(), M_Product_ID, C_UOM_To_ID, qtyOrdered);
                        if (qtyEntered == null)
                        {
                            qtyEntered = qtyOrdered;
                        }
                        SetQtyEntered(qtyEntered);
                        log.Fine("qtyEntered : " + qtyEntered.ToString() +
                               "qtyOrdered : " + qtyOrdered.ToString());
                    }
                }
            }

            //	Storage
            if (M_Product_ID != 0 && GetCtx().IsSOTrx(windowNo) && qtyOrdered > 0 && !isReturnTrx)		//	no negative (returns)
            {
                MProduct product = GetProduct();
                if (product.IsStocked())
                {
                    int M_Warehouse_ID = GetM_Warehouse_ID();
                    int M_AttributeSetInstance_ID = GetM_AttributeSetInstance_ID();
                    Decimal available = (Decimal)MStorage.GetQtyAvailable(M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID, null);
                    if (available == null)
                    {
                        available = Env.ZERO;
                    }
                    if (available == 0)
                    {
                        //   p_changeVO.addError(Msg.GetMsg(GetCtx(), "NoQtyAvailable"));
                    }
                    else if (available.CompareTo(qtyOrdered) < 0)
                    {
                        // p_changeVO.addError(Msg.GetMsg(GetCtx(), "InsufficientQtyAvailable", available));
                    }
                    else
                    {
                        int C_OrderLine_ID = GetC_OrderLine_ID();
                        Decimal notReserved = MOrderLine.GetNotReserved(GetCtx(),
                            M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID,
                            C_OrderLine_ID);
                        if (notReserved == null)
                        {
                            notReserved = Env.ZERO;
                        }
                        Decimal total = Decimal.Subtract(available, notReserved);
                        if (total.CompareTo(qtyOrdered) < 0)
                        {
                            //String info = Msg.parseTranslation(GetCtx(), "@QtyAvailable@=" + available
                            //    + "  -  @QtyNotReserved@=" + notReserved + "  =  " + total);
                            //p_changeVO.addError(Msg.GetMsg(GetCtx(), "InsufficientQtyAvailable", info));
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Can Change Warehouse
        /// </summary>
        /// <returns>true if warehouse can be changed</returns>
        public bool CanChangeWarehouse()
        {
            if (GetQtyDelivered() != 0)
            {
                log.SaveError("Error", Msg.Translate(GetCtx(), "QtyDelivered") + "=" + GetQtyDelivered());
                return false;
            }
            if (GetQtyInvoiced() != 0)
            {
                log.SaveError("Error", Msg.Translate(GetCtx(), "QtyInvoiced") + "=" + GetQtyDelivered());
                return false;
            }
            if (GetQtyReserved() != 0)
            {
                log.SaveError("Error", Msg.Translate(GetCtx(), "QtyReserved") + "=" + GetQtyReserved());
                return false;
            }
            //	We can change
            return true;
        }

        /// <summary>
        /// Get C_Project_ID
        /// </summary>
        /// <returns>project</returns>
        public new int GetC_Project_ID()
        {
            int ii = base.GetC_Project_ID();
            if (ii == 0)
                ii = GetParent().GetC_Project_ID();
            return ii;
        }

        /// <summary>
        /// Get C_Activity_ID
        /// </summary>
        /// <returns>Activity</returns>
        public new int GetC_Activity_ID()
        {
            int ii = base.GetC_Activity_ID();
            if (ii == 0)
                ii = GetParent().GetC_Activity_ID();
            return ii;
        }

        /// <summary>
        /// Get C_Campaign_ID
        /// </summary>
        /// <returns>Campaign</returns>
        public new int GetC_Campaign_ID()
        {
            int ii = base.GetC_Campaign_ID();
            if (ii == 0)
                ii = GetParent().GetC_Campaign_ID();
            return ii;
        }

        /// <summary>
        /// Get User2_ID
        /// </summary>
        /// <returns>User2</returns>
        public new int GetUser1_ID()
        {
            int ii = base.GetUser1_ID();
            if (ii == 0)
                ii = GetParent().GetUser1_ID();
            return ii;
        }

        /// <summary>
        /// Get User2_ID
        /// </summary>
        /// <returns>User2</returns>
        public new int GetUser2_ID()
        {
            int ii = base.GetUser2_ID();
            if (ii == 0)
                ii = GetParent().GetUser2_ID();
            return ii;
        }

        /// <summary>
        /// Get AD_OrgTrx_ID
        /// </summary>
        /// <returns>trx org</returns>
        public new int GetAD_OrgTrx_ID()
        {
            int ii = base.GetAD_OrgTrx_ID();
            if (ii == 0)
                ii = GetParent().GetAD_OrgTrx_ID();
            return ii;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MOrderLine[")
                .Append(Get_ID()).Append(",Line=").Append(GetLine())
                .Append(",Ordered=").Append(GetQtyOrdered())
                .Append(",Delivered=").Append(GetQtyDelivered())
                .Append(",Invoiced=").Append(GetQtyInvoiced())
                .Append(",Reserved=").Append(GetQtyReserved())
                .Append(", LineNet=").Append(GetLineNetAmt())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Add to description
        /// </summary>
        /// <param name="description">text</param>
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
                SetDescription(description);
            else
                SetDescription(desc + " | " + description);
        }

        /// <summary>
        /// Get Description Text.
        /// For jsp access (vs. isDescription)
        /// </summary>
        /// <returns>description</returns>
        public String GetDescriptionText()
        {
            return base.GetDescription();
        }

        /// <summary>
        /// 	Get Name
        /// </summary>
        /// <returns>get the name of the line (from Product)</returns>
        public String GetName()
        {
            GetProduct();
            if (_product != null)
                return _product.GetName();
            if (GetC_Charge_ID() != 0)
            {
                MCharge charge = MCharge.Get(GetCtx(), GetC_Charge_ID());
                return charge.GetName();
            }
            return "";
        }

        /// <summary>
        /// Set C_Charge_ID
        /// </summary>
        /// <param name="C_Charge_ID">charge</param>
        public new void SetC_Charge_ID(int C_Charge_ID)
        {
            base.SetC_Charge_ID(C_Charge_ID);
            if (C_Charge_ID > 0)
                Set_ValueNoCheck("C_UOM_ID", null);
        }

        /// <summary>
        /// Set Charge - Callout
        /// </summary>
        /// <param name="oldC_Charge_ID">old value</param>
        /// <param name="newC_Charge_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetC_Charge_ID(String oldC_Charge_ID, String newC_Charge_ID, int windowNo)
        {
            if (newC_Charge_ID == null || newC_Charge_ID.Length == 0)
                return;
            int C_Charge_ID = int.Parse(newC_Charge_ID);
            if (C_Charge_ID == 0)
                return;
            // Skip these steps for RMA. These fields are copied over from the orignal order instead.
            if (GetParent().IsReturnTrx())
                return;
            //
            //	No Product defined
            if (GetM_Product_ID() != 0)
            {
                base.SetC_Charge_ID(0);
                //p_changeVO.addError(Msg.GetMsg(GetCtx(), "ChargeExclusively"));
                return;
            }

            base.SetC_Charge_ID(C_Charge_ID);
            SetM_AttributeSetInstance_ID(0);
            SetS_ResourceAssignment_ID(0);
            SetC_UOM_ID(100);	//	EA
            //p_changeVO.setContext(GetCtx(), windowNo, "DiscountSchema", "N");
            String sql = "SELECT ChargeAmt FROM C_Charge WHERE C_Charge_ID=" + C_Charge_ID;
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    SetPriceEntered(Convert.ToDecimal(dr[0]));//.getBigDecimal (1));
                    SetPriceActual(Convert.ToDecimal(dr[0]));//.getBigDecimal (1));
                    SetPriceLimit(Env.ZERO);
                    SetPriceList(Env.ZERO);
                    SetDiscount(Env.ZERO);
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            //
            SetTax(windowNo, "C_Charge_ID");
        }

        /// <summary>
        /// Set Discount
        /// </summary>
        public void SetDiscount()
        {
            Decimal list = GetPriceList();
            //	No List Price
            if (Env.ZERO.CompareTo(list) == 0)
                return;
            //BigDecimal discount = list.subtract(getPriceActual()).multiply(new BigDecimal(100)).divide(list, getPrecision(), BigDecimal.ROUND_HALF_UP);
            Decimal discount = Decimal.Round(Decimal.Divide(Decimal.Multiply(Decimal.Subtract(list, GetPriceEntered()), new Decimal(100)), list), GetPrecision(), MidpointRounding.AwayFromZero);

            // Change ConvertUOMWise
            if (DB.GetSQLValue(null, "SELECT VAPOS_POSTerminal_ID FROM C_Order WHERE C_Order_ID = " + GetC_Order_ID()) > 0)
            {
                string _CONVERTUOMWISE = Util.GetValueOfString(DB.ExecuteScalar("SELECT VAPOS_ConvertUomWise FROM M_Product WHERE M_product_ID=" + GetM_Product_ID()));
                if (_CONVERTUOMWISE == "Y")
                {
                    if (GetQtyEntered() <= 1)
                    {
                        Decimal? rVal = 1 - GetQtyEntered();
                        Decimal? val1 = GetQtyEntered() * GetPriceList();
                        Decimal? val2 = rVal * GetPriceList();
                        Decimal? resVal = Decimal.Round((Util.GetValueOfDecimal(val1 + val2)), 2);

                        log.Info("Difference In Conversion Price : Value1 : " + val1 + ", Value2 : " + val2 + ", Value3 : " + resVal + ", ");
                        if (resVal == GetPriceList())
                        {
                            discount = 0;
                        }
                    }
                    else
                    {
                        Decimal? rVal = GetQtyEntered() - 1;
                        Decimal? val1 = GetQtyEntered() * GetPriceEntered();
                        Decimal? val2 = rVal * GetPriceEntered();
                        Decimal? resVal = Decimal.Round((Util.GetValueOfDecimal(val1 - val2)), 2);

                        log.Info("Difference In Conversion Price : Value1 : " + val1 + ", Value2 : " + val2 + ", Value3 : " + resVal + ", ");
                        if (resVal == GetPriceEntered())
                        {
                            discount = 0;
                        }
                    }
                }
            }

            SetDiscount(discount);
        }

        /// <summary>
        /// Is Tax Included in Amount
        /// </summary>
        /// <returns>true if tax calculated</returns>
        public bool IsTaxIncluded()
        {
            if (_M_PriceList_ID == 0)
            {
                _M_PriceList_ID = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar("SELECT M_PriceList_ID FROM C_Order WHERE C_Order_ID=" + GetC_Order_ID(), null, Get_TrxName()));
            }
            MPriceList pl = MPriceList.Get(GetCtx(), _M_PriceList_ID, Get_TrxName());
            return pl.IsTaxIncluded();
        }

        /// <summary>
        /// Set Qty Entered/Ordered.
        /// Use this Method if the Line UOM is the Product UOM
        /// </summary>
        /// <param name="qty">qtyOrdered/Entered</param>
        public void SetQty(Decimal qty)
        {
            base.SetQtyEntered(qty);
            base.SetQtyOrdered(GetQtyEntered());
        }

        /// <summary>
        /// Set Qty Entered - enforce entered UOM 
        /// </summary>
        /// <param name="qtyEntered">qtyEntered</param>
        public new void SetQtyEntered(Decimal qtyEntered)
        {
            if (qtyEntered != null && GetC_UOM_ID() != 0)
            {
                int precision = MUOM.GetPrecision(GetCtx(), GetC_UOM_ID());
                qtyEntered = Decimal.Round(qtyEntered, precision, MidpointRounding.AwayFromZero);
            }
            base.SetQtyEntered(qtyEntered);
        }

        /// <summary>
        /// Set Qty Ordered - enforce Product UOM 
        /// </summary>
        /// <param name="qtyOrdered"></param>
        public new void SetQtyOrdered(Decimal qtyOrdered)
        {
            MProduct product = GetProduct();
            if (qtyOrdered != null && product != null)
            {
                int precision = product.GetUOMPrecision();
                qtyOrdered = Decimal.Round(qtyOrdered, precision, MidpointRounding.AwayFromZero);
            }
            base.SetQtyOrdered(qtyOrdered);
        }

        /// <summary>
        /// 	Set Original OrderLine for RMA
        /// 	SOTrx should be set.
        /// </summary>
        /// <param name="origOrderLine"> MInOutLine</param>
        public void SetOrigOrderLine(MOrderLine origOrderLine)
        {
            if (origOrderLine == null || origOrderLine.Get_ID() == 0)
                return;
            SetOrig_InOutLine_ID(-1);
            SetC_Tax_ID(origOrderLine.GetC_Tax_ID());
            SetPriceList(origOrderLine.GetPriceList());
            SetPriceLimit(origOrderLine.GetPriceLimit());
            SetPriceActual(origOrderLine.GetPriceActual());
            SetPriceEntered(origOrderLine.GetPriceEntered());
            SetC_Currency_ID(origOrderLine.GetC_Currency_ID());
            SetDiscount(origOrderLine.GetDiscount());

            return;

        }

        /// <summary>
        /// Set Original Order Line - Callout
        /// </summary>
        /// <param name="oldOrig_OrderLine_ID">old Orig Order</param>
        /// <param name="newOrig_OrderLine_ID">new Orig Order</param>
        /// <param name="windowNo">window no</param>
        /// //@UICallout
        public void SetOrig_OrderLine_ID(String oldOrig_OrderLine_ID, String newOrig_OrderLine_ID, int windowNo)
        {
            if (newOrig_OrderLine_ID == null || newOrig_OrderLine_ID.Length == 0)
                return;
            int Orig_OrderLine_ID = int.Parse(newOrig_OrderLine_ID);
            if (Orig_OrderLine_ID == 0)
                return;

            // For returns, Price Limit is not enforced
            //p_changeVO.setContext(GetCtx(), windowNo, "EnforcePriceLimit", false);
            // For returns, discount is copied over from the sales order
            //p_changeVO.setContext(GetCtx(), windowNo, "DiscountSchema", false);

            //		Get Details
            MOrderLine oLine = new MOrderLine(GetCtx(), Orig_OrderLine_ID, null);
            if (oLine.Get_ID() != 0)
                SetOrigOrderLine(oLine);
        }

        /// <summary>
        /// 	Set Original Shipment Line for RMA
        /// 	SOTrx should be set.
        /// </summary>
        /// <param name="Orig_InOutLine">MInOutLine</param>
        public void SetOrigInOutLine(MInOutLine Orig_InOutLine)
        {
            if (Orig_InOutLine == null || Orig_InOutLine.Get_ID() == 0)
                return;

            SetC_Project_ID(Orig_InOutLine.GetC_Project_ID());
            SetC_Campaign_ID(Orig_InOutLine.GetC_Campaign_ID());
            SetM_Product_ID(Orig_InOutLine.GetM_Product_ID());
            SetM_AttributeSetInstance_ID(Orig_InOutLine.GetM_AttributeSetInstance_ID());
            SetC_UOM_ID(Orig_InOutLine.GetC_UOM_ID());

            return;
        }

        /// <summary>
        ///Set Original Shipment Line - Callout
        /// </summary>
        /// <param name="oldC_Charge_ID">old value</param>
        /// <param name="newC_Charge_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetOrig_InOutLine_ID(String oldOrig_InOutLine_ID, String newOrig_InOutLine_ID, int windowNo)
        {
            if (newOrig_InOutLine_ID == null || newOrig_InOutLine_ID.Length == 0)
                return;
            int Orig_InOutLine_ID = int.Parse(newOrig_InOutLine_ID);
            if (Orig_InOutLine_ID == 0)
                return;

            //		Get Details
            MInOutLine ioLine = new MInOutLine(GetCtx(), Orig_InOutLine_ID, null);
            if (ioLine.Get_ID() != 0)
                SetOrigInOutLine(ioLine);
            SetQty(windowNo, "Orig_InOutLine_ID");
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">newRecord</param>
        /// <returns>true if it can be saved</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            MWarehouse wHouse = null;
            if (GetM_Product_ID() == 0 && GetC_Charge_ID() == 0)
            {
                log.SaveError("VIS_NOProductOrCharge", "");
                return false;
            }

            Decimal? PriceActual, PriceEntered;
            Decimal? QtyOrdered, QtyEntered, QtyEstimation, QtyBlanketPending, QtyReleased, QtyReserved;
            //	Get Defaults from Parent
            if (GetC_BPartner_ID() == 0 || GetC_BPartner_Location_ID() == 0
                || GetM_Warehouse_ID() == 0
                || GetC_Currency_ID() == 0)
                SetOrder(GetParent());
            if (_M_PriceList_ID == 0)
                SetHeaderInfo(GetParent());

            //	R/O Check - Product/Warehouse Change
            //SI_0595 : After Reactivation, When we change M_AttributeSetInstance_ID, then system will check isqtyReserved, isqtyDelivered, isqtyInvoiced then not to save this records
            if (!newRecord && (Is_ValueChanged("M_Product_ID") || Is_ValueChanged("M_Warehouse_ID") || Is_ValueChanged("M_AttributeSetInstance_ID")))
            {
                if (!CanChangeWarehouse())
                    return false;
            }	//	Product Changed

            //JID_1362 : Tax cant be change on Re-Activation 
            if (!newRecord && Is_ValueChanged("C_Tax_ID"))
            {
                if (GetQtyDelivered() != 0)
                {
                    log.SaveError("Error", Msg.Translate(GetCtx(), "QtyDelivered") + "=" + GetQtyDelivered());
                    return false;
                }
                if (GetQtyInvoiced() != 0)
                {
                    log.SaveError("Error", Msg.Translate(GetCtx(), "QtyInvoiced") + "=" + GetQtyDelivered());
                    return false;
                }
            }
            if (newRecord && Get_ColumnIndex("IsTaxExempt") > -1 && Get_ColumnIndex("C_TaxExemptReason_ID") > -1 && !IsTaxExempt())
            {
                //Set IsTaxExempt and TaxExemptReason
                SetTaxExemptReason();
            }
            else if (Is_ValueChanged("IsTaxExempt") && !IsTaxExempt() && GetC_TaxExemptReason_ID() > 0
                && Get_ColumnIndex("IsTaxExempt") > -1 && Get_ColumnIndex("C_TaxExemptReason_ID") > -1)
            {
                //taxExpemt is false but  tax exempt reason is selected
                SetC_TaxExemptReason_ID(0);
            }


            MOrder Ord = GetParent();
            MDocType docType = MDocType.Get(GetCtx(), Ord.GetC_DocTypeTarget_ID());

            // JID_1850 if product is there when qty delivered / invoicedcant be less than qtyordered
            if ((GetM_Product_ID()) > 0)
            {
                //SI_0643: If we reactive the Sales order, System will not allow to save Ordered Qty less than delivered qty.
                // when we void a record, then not to check this record, because first we set qtyOrdered/qtyenetered as 0 after that we update qtydelivered on line
                // JID_1362: when qty delivered / invoicedcant be less than qtyordered
                // JID_1403 : System do not allow to create order with -ve qty, On Completion system give error "Can't Server Qty" (for comparison - make absolute)
                if (!newRecord && (Math.Abs(GetQtyOrdered()) < Math.Abs(GetQtyDelivered()) || Math.Abs(GetQtyOrdered()) < Math.Abs(GetQtyInvoiced())) &&
                    (string.IsNullOrEmpty(GetDescription()) || !(!string.IsNullOrEmpty(GetDescription()) && GetDescription().Contains("Voided")))
                    && docType.GetDocBaseType() != "BOO")       // Skip for Blanket Order
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "VIS_QtydeliveredNotLess"));
                    return false;
                }
            }


            // Added by Vivek on 22/09/2017 assigned by Mukesh sir
            // if PO is drop ship type then new line should not allow 
            if (!Ord.IsSOTrx() && !Ord.IsReturnTrx() && Ord.IsDropShip() && !_fromProcess)
            {
                if (newRecord)
                {
                    log.SaveError("VIS_DropShipLineNotSave", "");
                    return false;
                }
            }

            //JID_0211: System is allowing to save promised date smaller than order date on header as well as on order lines.. There should be a validation.
            if (!Ord.IsReturnTrx() && GetDateOrdered() != null && GetDatePromised() != null)
            {
                if (GetDateOrdered().Value.Date > GetDatePromised().Value.Date)
                {
                    log.SaveError("VIS_OrderDateGrtrThanPromisedDate", "");
                    return false;
                }
            }

            // Check if order line is drop ship type then set drop ship type of warehouse at order line
            if (Ord.IsSOTrx())
            {
                if (IsDropShip())
                {
                    wHouse = MWarehouse.Get(GetCtx(), Ord.GetM_Warehouse_ID());
                    if (!wHouse.IsDropShip())
                    {
                        int _Warehouse_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_Warehouse_ID FROM M_Warehouse WHERE AD_Org_ID=" + GetAD_Org_ID() + " AND IsActive='Y' AND IsDropShip='Y'"));
                        if (_Warehouse_ID > 0)
                        {
                            SetM_Warehouse_ID(_Warehouse_ID);
                        }
                        // if drop ship type of warehouse does not exist in same organization then create a new warehouse and set that warehouse
                        else
                        {
                            _Warehouse_ID = CreateDropShipWareHouse(wHouse);
                            SetM_Warehouse_ID(_Warehouse_ID);
                        }
                    }
                }
                else
                {
                    SetM_Warehouse_ID(Ord.GetM_Warehouse_ID());
                }
            }

            int primaryAcctSchemaCurrency = 0;
            // get current cost from product cost on new record and when product changed
            // currency conversion also required if order has different currency with base currency
            if (newRecord || (Is_ValueChanged("M_Product_ID")) || (Is_ValueChanged("M_AttributeSetInstance_ID")))
            {
                // when record is not a return transaction then price should pick from product cost else from its Original record
                decimal currentcostprice = 0;
                if (!Ord.IsReturnTrx())
                {

                    //primaryAcctSchemaCurrency = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT C_Currency_ID from C_AcctSchema WHERE C_AcctSchema_ID = 
                    //                        (SELECT c_acctschema1_id FROM ad_clientinfo WHERE ad_client_id = " + GetAD_Client_ID() + ")", null, Get_Trx()));
                    currentcostprice = MCost.GetproductCosts(GetAD_Client_ID(), GetAD_Org_ID(), GetM_Product_ID(), Util.GetValueOfInt(GetM_AttributeSetInstance_ID()), Get_Trx(), Ord.GetM_Warehouse_ID());
                    primaryAcctSchemaCurrency = GetCtx().GetContextAsInt("$C_Currency_ID");
                    if (Ord.GetC_Currency_ID() != primaryAcctSchemaCurrency)
                    {
                        currentcostprice = MConversionRate.Convert(GetCtx(), currentcostprice, primaryAcctSchemaCurrency, Ord.GetC_Currency_ID(),
                                                                                    Ord.GetDateAcct(), Ord.GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID());
                    }
                }
                else if (Ord.IsReturnTrx() && GetOrig_OrderLine_ID() > 0)
                {
                    currentcostprice = Util.GetValueOfDecimal(DB.ExecuteScalar(@"SELECT CurrentCostPrice FROM C_OrderLine WHERE C_OrderLine_ID =" + GetOrig_OrderLine_ID(), null, Get_Trx()));
                    //MOrderLine origOrderLine = new MOrderLine(GetCtx(), GetOrig_OrderLine_ID(), Get_Trx());
                    //currentcostprice = origOrderLine.GetCurrentCostPrice();
                }
                SetCurrentCostPrice(currentcostprice);
            }

            //	Charge
            //pratap - added check  _IsReturnTrx==false 4/12/15
            _IsReturnTrx = Ord.IsReturnTrx();

            if (Ord.GetVAPOS_POSTerminal_ID() > 0)
            {
                if (GetC_Charge_ID() != 0 && GetM_Product_ID() != 0)
                    SetM_Product_ID(0);
                //	No Product
                if (GetM_Product_ID() == 0)
                    SetM_AttributeSetInstance_ID(0);
                //	Product
                else	//	Set/check Product Price
                {
                    // Set PriceActual as 0 in case of Void Line AND at time of total discounted order line
                    if (GetVAPOS_LineStatus() == "VO" || (GetQtyEntered() < 0 && Env.ZERO.CompareTo(GetPriceActual()) == 0))
                    {
                        SetPriceList(0);
                        SetDiscount(0);
                        SetVAPOS_DiscountAmount(0);
                    }
                    else
                    {
                        // Check for Modifiers, In case PriceActual is not set on OrderLine
                        if (Util.GetValueOfInt(GetRef_OrderLine_ID()) <= 0)
                        {
                            //	Set Price if Actual = 0
                            if (_productPrice == null && Env.ZERO.CompareTo(GetPriceActual()) == 0
                                && Env.ZERO.CompareTo(GetPriceList()) == 0)
                                SetPrice();
                            //	Check if on Price list
                            if (_productPrice == null)
                                GetProductPricing(_M_PriceList_ID);
                            /******** Commented for ViennaCRM. Now it will not be checked whether the Product is on PriceList or Not ********/
                            //if (!_productPrice.IsCalculated())
                            //{
                            //    log.SaveError("Error", Msg.GetMsg(GetCtx(), "ProductNotOnPriceList"));
                            //    return false;
                            //}
                        }
                    }
                }
                //	UOM
                if (GetC_UOM_ID() == 0
                    && (GetM_Product_ID() != 0 || GetPriceEntered().CompareTo(Env.ZERO) != 0
                        || GetC_Charge_ID() != 0))
                {
                    int C_UOM_ID = MUOM.GetDefault_UOM_ID(GetCtx());
                    if (C_UOM_ID > 0)
                        SetC_UOM_ID(C_UOM_ID);
                }
                //	Qty Precision
                if (newRecord || Is_ValueChanged("qtyEntered"))
                    SetQtyEntered(GetQtyEntered());
                if (newRecord || Is_ValueChanged("qtyOrdered"))
                    SetQtyOrdered(GetQtyOrdered());

            }
            //	Qty on instance ASI for SO
            if (_IsSOTrx && _IsReturnTrx == false
                && GetM_AttributeSetInstance_ID() != 0
                && (newRecord || Is_ValueChanged("M_Product_ID")
                    || Is_ValueChanged("M_AttributeSetInstance_ID")
                    || Is_ValueChanged("M_Warehouse_ID")))
            {
                MProduct product = GetProduct();
                if (product.IsStocked())
                {
                    int M_AttributeSet_ID = product.GetM_AttributeSet_ID();
                    bool isInstance = M_AttributeSet_ID != 0;
                    if (isInstance)
                    {
                        MAttributeSet mas = MAttributeSet.Get(GetCtx(), M_AttributeSet_ID);
                        isInstance = mas.IsInstanceAttribute();
                    }
                    //	Max
                    if (isInstance)
                    {
                        MStorage[] storages = MStorage.GetWarehouse(GetCtx(),
                            GetM_Warehouse_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(),
                            M_AttributeSet_ID, false, null, true, Get_TrxName());
                        Decimal qty = Env.ZERO;
                        for (int i = 0; i < storages.Length; i++)
                        {
                            if (storages[i].GetM_AttributeSetInstance_ID() == GetM_AttributeSetInstance_ID())
                            {
                                qty = Decimal.Add(qty, storages[i].GetQtyOnHand());
                            }
                        }
                        //Commented By Bharat as Issue given in Mentis 17/12/2015
                        //if (GetQtyOrdered().CompareTo(qty) > 0)
                        //{
                        //    log.Warning("Qty - Stock=" + qty + ", Ordered=" + GetQtyOrdered());
                        //    log.SaveError("QtyInsufficient", "=" + qty);
                        //    return false;
                        //}
                    }
                }	//	stocked
            }	//	SO instance
            //else
            //{
            //    MProduct pro = new MProduct(GetCtx(), GetM_Product_ID(), null);
            //    String qryUom = "SELECT vdr.C_UOM_ID FROM M_Product p LEFT JOIN M_Product_Po vdr ON p.M_Product_ID= vdr.M_Product_ID WHERE p.M_Product_ID=" + GetM_Product_ID() + " AND vdr.C_BPartner_ID = " + Ord.GetC_BPartner_ID();
            //    int uom = Util.GetValueOfInt(DB.ExecuteScalar(qryUom));
            //    if (pro.GetC_UOM_ID() != 0)
            //    {
            //        if (pro.GetC_UOM_ID() != uom && uom != 0)
            //        {
            //            decimal? Res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + pro.GetC_UOM_ID() + " AND C_UOM_To_ID = " + uom + " AND M_Product_ID= " + GetM_Product_ID() + " AND IsActive='Y'"));
            //            if (Res > 0)
            //            {
            //                SetQtyEntered(GetQtyEntered() * Res);
            //                //OrdQty = MUOMConversion.ConvertProductTo(GetCtx(), _M_Product_ID, UOM, OrdQty);
            //            }
            //            else
            //            {
            //                decimal? res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + pro.GetC_UOM_ID() + " AND C_UOM_To_ID = " + uom + " AND IsActive='Y'"));
            //                if (res > 0)
            //                {
            //                    SetQtyEntered(GetQtyEntered() * res);
            //                    //OrdQty = MUOMConversion.Convert(GetCtx(), prdUOM, UOM, OrdQty);
            //                }
            //            }
            //            SetC_UOM_ID(uom);
            //        }
            //        else
            //        {
            //            SetC_UOM_ID(pro.GetC_UOM_ID());
            //        }
            //    }
            //}

            if (!(Util.GetValueOfInt(Ord.GetVAPOS_POSTerminal_ID()) > 0))
            {
                if (GetC_Charge_ID() != 0 && GetM_Product_ID() != 0)
                    SetM_Product_ID(0);
                //	No Product
                if (GetM_Product_ID() == 0)
                    SetM_AttributeSetInstance_ID(0);
                //	Product
                else	//	Set/check Product Price
                {
                    QtyEntered = GetQtyEntered();
                    QtyOrdered = GetQtyOrdered();
                    QtyReleased = GetQtyReleased();
                    QtyReserved = GetQtyReserved();

                    int gp = MUOM.GetPrecision(GetCtx(), GetC_UOM_ID());
                    Decimal? QtyEntered1 = Decimal.Round(QtyEntered.Value, gp, MidpointRounding.AwayFromZero);
                    if (QtyEntered != QtyEntered1)
                    {
                        this.log.Fine("Corrected QtyEntered Scale UOM=" + GetC_UOM_ID()
                            + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                        QtyEntered = QtyEntered1;
                        SetQtyEntered(QtyEntered);
                    }
                    Decimal? pc = MUOMConversion.ConvertProductFrom(GetCtx(), GetM_Product_ID(), GetC_UOM_ID(), QtyEntered);
                    QtyOrdered = pc;
                    bool conversion = false;
                    if (QtyOrdered != null)
                    {
                        conversion = QtyEntered != QtyOrdered;
                    }
                    if (QtyOrdered == null)
                    {
                        conversion = false;
                        QtyOrdered = 1;
                        SetQtyOrdered(QtyOrdered * QtyEntered1);
                    }
                    else
                    {
                        SetQtyOrdered(QtyOrdered);
                    }


                    //if (conversion)
                    //{
                    Decimal? qtyRel = MUOMConversion.ConvertProductTo(GetCtx(), GetM_Product_ID(), GetC_UOM_ID(), QtyReleased);
                    Decimal? qtyOrd = MUOMConversion.ConvertProductTo(GetCtx(), GetM_Product_ID(), GetC_UOM_ID(), QtyOrdered);
                    Decimal? qtyRes = MUOMConversion.ConvertProductTo(GetCtx(), GetM_Product_ID(), GetC_UOM_ID(), QtyReserved);
                    if (qtyRel != null)
                    {
                        QtyReleased = qtyRel;
                    }
                    if (qtyOrd != null)
                    {
                        QtyOrdered = qtyOrd;
                    }
                    if (qtyRes != null)
                    {
                        QtyReserved = qtyRes;
                    }
                    //}

                    if (docType.GetDocBaseType() == "BOO")    ///docType.GetValue() == "BSO" || docType.GetValue() == "BPO")
                    {
                        QtyEstimation = GetQtyEstimation();
                        if ((QtyEntered + QtyReleased) > QtyEstimation)
                        {
                            log.SaveError(Msg.Translate(GetCtx(), "VIS_OrderQtyMoreThnEstimation"), "");
                            return false;
                        }
                    }

                    // JID_0969: System should not allow to set Qty Order more than blanket order qty.
                    if (docType.IsReleaseDocument() && (docType.GetDocBaseType() == "SOO" || docType.GetDocBaseType() == "POO"))
                    {
                        QtyBlanketPending = GetQtyBlanket();
                        QtyOrdered = Util.GetValueOfDecimal(DB.ExecuteScalar(@"SELECT SUM(QtyOrdered) FROM C_Order o INNER JOIN C_OrderLine ol ON ol.C_Order_ID = o.C_Order_ID
                            WHERE ol.C_OrderLine_Blanket_ID = " + GetC_OrderLine_Blanket_ID() + @" AND ol.IsActive = 'Y' AND o.DocStatus NOT IN ('RE' , 'VO' , 'CL' , 'CO') AND ol.C_OrderLine_ID <> "
                                                                + Get_ID(), null, Get_Trx()));
                        if (Decimal.Add(Convert.ToDecimal(QtyOrdered), GetQtyOrdered()) > QtyBlanketPending)
                        {
                            log.SaveError("VIS_OrderQtyMoreThnBlanketPending", "");
                            return false;
                        }
                    }



                    //	Set Price if Actual = 0
                    if (Env.IsModuleInstalled("ED011_"))
                    {
                    }
                    else
                    {
                        if (_productPrice == null && Env.ZERO.CompareTo(GetPriceActual()) == 0
                            && Env.ZERO.CompareTo(GetPriceList()) == 0)
                            SetPrice();
                        //	Check if on Price list
                        if (_productPrice == null)
                            GetProductPricing(_M_PriceList_ID);
                    }


                    /******** Commented for ViennaCRM. Now it will not be checked whether the Product is on PriceList or Not ********/
                    //if (!_productPrice.IsCalculated())
                    //{
                    //    log.SaveError("Error", Msg.GetMsg(GetCtx(), "ProductNotOnPriceList"));
                    //    return false;
                    //}
                }
                //	UOM
                if (GetC_UOM_ID() == 0
                    && (GetM_Product_ID() != 0 || GetPriceEntered().CompareTo(Env.ZERO) != 0
                        || GetC_Charge_ID() != 0))
                {
                    int C_UOM_ID = MUOM.GetDefault_UOM_ID(GetCtx());
                    if (C_UOM_ID > 0)
                        SetC_UOM_ID(C_UOM_ID);
                }
                //	Qty Precision
                if (newRecord || Is_ValueChanged("QtyEntered"))
                    SetQtyEntered(GetQtyEntered());
                if (newRecord || Is_ValueChanged("QtyOrdered"))
                    SetQtyOrdered(GetQtyOrdered());

                // after delivering we cant change the price entered / price actual
                if (Is_ValueChanged("PriceEntered") && (GetQtyDelivered() > 0 || GetQtyInvoiced() > 0))
                {
                    SetPriceEntered(Util.GetValueOfDecimal(Get_ValueOld("PriceEntered")));
                    SetPriceActual(GetPriceEntered());
                }

                //JID_1474 : if document is closed then we need to set Delivered qty as Ordered qty Suggested by Gagandeep kaur and Puneet that we do not
                // need to add return trx check and it will work for all orders
                if (isClosed)
                {
                    if (GetQtyDelivered() > 0)
                        SetQtyOrdered(GetQtyDelivered());
                }
                //end

            }
            /////////////

            //	FreightAmt Not used
            if (Env.ZERO.CompareTo(GetFreightAmt()) != 0)
                SetFreightAmt(Env.ZERO);

            //	Set Tax
            if (GetC_Tax_ID() == 0)
                SetTax();

            //	Get Line No
            if (GetLine() == 0)
            {
                String sql = "SELECT COALESCE(MAX(Line),0)+10 FROM C_OrderLine WHERE C_Order_ID=" + GetC_Order_ID();
                int ii = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql, null, Get_TrxName()));
                SetLine(ii);
            }


            //Calculations & Rounding            	    
            SetLineNetAmt(); //extended Amount with or without tax

            // if change the Quantity then recalculate tax and surcharge amount.
            if (((Decimal)GetTaxAmt()).CompareTo(Env.ZERO) == 0 || (Get_ColumnIndex("SurchargeAmt") > 0 && GetSurchargeAmt().CompareTo(Env.ZERO) == 0) || Is_ValueChanged("QtyEntered"))
                SetTaxAmt();

            SetDiscount();

            // set Tax Amount in base currency
            if (Get_ColumnIndex("TaxBaseAmt") > 0)
            {
                decimal taxAmt = 0;
                primaryAcctSchemaCurrency = GetCtx().GetContextAsInt("$C_Currency_ID");
                if (Ord.GetC_Currency_ID() != primaryAcctSchemaCurrency)
                {
                    taxAmt = MConversionRate.Convert(GetCtx(), GetTaxAmt(), primaryAcctSchemaCurrency, Ord.GetC_Currency_ID(),
                                                                               Ord.GetDateAcct(), Ord.GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID());
                }
                else
                {
                    taxAmt = GetTaxAmt();
                }
                SetTaxBaseAmt(taxAmt);
            }

            // set Taxable Amount -- (Line Total-Tax Amount)
            if (Get_ColumnIndex("TaxAbleAmt") >= 0)
            {
                if (Get_ColumnIndex("SurchargeAmt") > 0)
                {
                    SetTaxAbleAmt(Decimal.Subtract(Decimal.Subtract(GetLineTotalAmt(), GetTaxAmt()), GetSurchargeAmt()));
                }
                else
                {
                    SetTaxAbleAmt(Decimal.Subtract(GetLineTotalAmt(), GetTaxAmt()));
                }
            }

            if (Get_ColumnIndex("BasePrice") >= 0)
            {
                if (newRecord)
                {
                    if (((Decimal)GetPriceEntered()).CompareTo(Env.ZERO) != 0)
                    {
                        SetBasePrice(GetPriceEntered());
                    }
                }
                else
                {
                    if (Is_ValueChanged("M_Product_ID"))
                    {
                        if (((Decimal)GetPriceEntered()).CompareTo(Env.ZERO) != 0)
                        {
                            SetBasePrice(GetPriceEntered());
                        }
                    }
                }
            }

            if (Get_ColumnIndex("VAPOS_DiscountAmount") >= 0)
            {
                MCurrency currency = MCurrency.Get(GetCtx(), Ord.GetC_Currency_ID());
                SetVAPOS_DiscountAmount(Decimal.Round(GetQtyEntered() * Decimal.Subtract(GetPriceList(), GetPriceEntered()), currency.GetStdPrecision()));
            }

            // Validate Return Policy for RMA
            //MOrder order = new MOrder(GetCtx(), GetC_Order_ID(), Get_TrxName());
            MOrder order = Ord;
            bool isReturnTrx = order.IsReturnTrx();
            if (isReturnTrx)
            {
                Boolean withinPolicy = true;

                if (order.GetM_ReturnPolicy_ID() == 0)
                    order.SetM_ReturnPolicy_ID();

                if (order.GetM_ReturnPolicy_ID() == 0)
                    withinPolicy = false;
                else
                {
                    ////////////////////////////////////////////////////////////
                    DateTime? movementdate = Util.GetValueOfDateTime(DB.ExecuteScalar("SELECT MovementDate FROM M_InOut WHERE " +
                                                                                        "M_InOut_ID=" + order.GetOrig_InOut_ID(), null, Get_TrxName()));
                    // MInOut origInOut = new MInOut(GetCtx(), order.GetOrig_InOut_ID(), Get_TrxName());
                    MReturnPolicy rpolicy = new MReturnPolicy(GetCtx(), order.GetM_ReturnPolicy_ID(), Get_TrxName());

                    log.Fine("RMA Date : " + order.GetDateOrdered() + " Shipment Date : " + movementdate);
                    withinPolicy = rpolicy.CheckReturnPolicy(movementdate, order.GetDateOrdered(), GetM_Product_ID());
                }

                if (!withinPolicy)
                {
                    if (!MRole.GetDefault(GetCtx()).IsOverrideReturnPolicy())
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "ReturnPolicyExceeded"));
                        return false;
                    }
                    else
                    {
                        log.SaveWarning("Warning", Msg.GetMsg(GetCtx(), "ReturnPolicyExceeded"));
                    }
                }

                //pratap - Check if Qty > Movement Qty 4/12/15
                //compare movement qty with ordered qty for UOM handling
                decimal movementqty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT MovementQty FROM M_InOutLine WHERE " +
                                                                                "M_InOutLine_ID=" + GetOrig_InOutLine_ID(), null, Get_TrxName()));
                //MInOutLine origInOutLine = new MInOutLine(GetCtx(), GetOrig_InOutLine_ID(), Get_TrxName());
                if (GetQtyOrdered().CompareTo(movementqty) > 0)
                {
                    log.SaveError("VIS_ReturnQtyMoreThnOrder", "");
                    return false;
                }
                bool IsReveresed = false;
                string qry1 = null;
                if (order.GetDescription() != null)
                {
                    if (order.GetDescription().Contains("->"))
                    {
                        IsReveresed = true;
                    }
                }
                if (!IsReveresed)
                {
                    if (newRecord || Is_ValueChanged("IsActive") || Is_ValueChanged("QtyEntered") || Is_ValueChanged("QtyOrdered"))
                    {
                        qry1 = "SELECT Movementqty FROM M_InoutLine WHERE M_InoutLine_ID =" + GetOrig_InOutLine_ID();
                        Decimal? _result = Util.GetValueOfDecimal(DB.ExecuteScalar(qry1, null, Get_Trx()));
                        decimal QtyNotDelivered = 0;
                        if (newRecord)
                            QtyNotDelivered = Util.GetValueOfDecimal(DB.ExecuteScalar(@"SELECT SUM(QtyOrdered) FROM C_Order o INNER JOIN C_OrderLine ol ON o.C_Order_ID = ol.C_Order_ID
                            WHERE ol.Orig_InOutLine_ID = " + GetOrig_InOutLine_ID() + @" AND ol.Isactive = 'Y' AND o.docstatus NOT IN ('RE' , 'VO')", null, Get_Trx()));
                        else
                            QtyNotDelivered = Util.GetValueOfDecimal(DB.ExecuteScalar(@"SELECT SUM(QtyOrdered) FROM C_Order o INNER JOIN C_OrderLine ol ON o.C_Order_ID = ol.C_Order_ID                            
                            WHERE ol.Orig_InOutLine_ID = " + GetOrig_InOutLine_ID() + @" AND ol.Isactive = 'Y' AND o.docstatus NOT IN ('RE' , 'VO') AND ol.C_OrderLine_ID <> " + GetC_OrderLine_ID(), null, Get_Trx()));
                        if (GetQtyOrdered() > (_result - QtyNotDelivered))
                        {
                            log.SaveError("Error", Msg.GetMsg(GetCtx(), "QtyCantGrtrthanOrderedQty"));
                            return false;
                        }
                    }
                }
                //
            }
            //   Added By Bharat..  Mentis Issue no.  0000542
            if (DocActionVariables.STATUS_INPROGRESS.Equals(order.GetDocStatus()) && Is_ValueChanged("M_AttributeSetInstance_ID"))
            {
                //	Always check and (un) Reserve Inventory

                if (GetM_Product_ID() != 0)
                {
                    Decimal difference = Decimal.Negate(GetQtyReserved());
                    MProduct product = GetProduct();
                    if (product.IsStocked())
                    {
                        if (GetQtyReserved() > 0)
                        {
                            Decimal ordered = _IsSOTrx ? Env.ZERO : difference;
                            Decimal reserved = _IsSOTrx ? difference : Env.ZERO;
                            int M_Locator_ID = 0;
                            //	Get default Location'
                            MWarehouse wh = MWarehouse.Get(GetCtx(), GetM_Warehouse_ID());
                            M_Locator_ID = wh.GetDefaultM_Locator_ID();

                            //	Get Locator to reserve
                            if (M_Locator_ID == 0)
                            {
                                if (GetM_AttributeSetInstance_ID() != 0)	//	Get existing Location
                                    M_Locator_ID = MStorage.GetM_Locator_ID(GetM_Warehouse_ID(),
                                        GetM_Product_ID(), Util.GetValueOfInt(Get_ValueOld("M_AttributeSetInstance_ID")),
                                        ordered, Get_TrxName());
                            }
                            if (M_Locator_ID == 0)
                            {
                                string sql = "SELECT M_Locator_ID FROM M_Locator WHERE M_Warehouse_ID=" + GetM_Warehouse_ID();
                                M_Locator_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                            }
                            //	Update Storage
                            MStorage.Add(GetCtx(), GetM_Warehouse_ID(), M_Locator_ID,
                                GetM_Product_ID(),
                                Util.GetValueOfInt(Get_ValueOld("M_AttributeSetInstance_ID")), Util.GetValueOfInt(Get_ValueOld("M_AttributeSetInstance_ID")),
                                Env.ZERO, reserved, ordered, Get_TrxName());
                            //return false;
                            SetQtyReserved(Decimal.Add(GetQtyReserved(), difference));
                        }
                    }	//	stockec
                    //	update line                    
                }	//	product
            }	//	reverse inventory

            // Reset Amount Dimension if Line Amount is different
            if (!newRecord && Is_ValueChanged("LineNetAmt"))
            {
                if (Util.GetValueOfInt(Get_Value("AmtDimLineNetAmt")) > 0)
                {
                    string qry = "SELECT Amount FROM C_DimAmt WHERE C_DimAmt_ID=" + Util.GetValueOfInt(Get_Value("AmtDimLineNetAmt"));
                    decimal amtdimAmt = Util.GetValueOfDecimal(DB.ExecuteScalar(qry, null, Get_TrxName()));

                    if (amtdimAmt != GetLineNetAmt())
                    {
                        Set_Value("AmtDimLineNetAmt", null);
                    }
                }
                resetAmtDim = true;
            }

            // Reset Amount Dimension if Line Total Amount is different
            if (!newRecord && Is_ValueChanged("LineTotalAmt"))
            {
                if (Util.GetValueOfInt(Get_Value("AmtDimLineTotalAmt")) > 0)
                {
                    string qry = "SELECT Amount FROM C_DimAmt WHERE C_DimAmt_ID=" + Util.GetValueOfInt(Get_Value("AmtDimLineTotalAmt"));
                    decimal amtdimAmt = Util.GetValueOfDecimal(DB.ExecuteScalar(qry, null, Get_TrxName()));

                    if (amtdimAmt != GetLineTotalAmt())
                    {
                        Set_Value("AmtDimLineTotalAmt", null);
                    }
                }
                resetTotalAmtDim = true;
            }
            //  Work done for Purchase price and Margin Calculation
            if (Env.IsModuleInstalled("VA077_") && Ord.IsSOTrx() && !Ord.IsReturnTrx())
            {
                decimal marginper = 0;
                decimal margin = 0;

                var duration = Util.GetValueOfString(Get_Value("VA077_Duration"));

                if (Util.GetValueOfDecimal(Get_Value("VA077_PurchasePrice")).Equals(0))
                {
                    int po_PriceList_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT PO_PriceList_ID FROM C_BPartner WHERE C_BPartner_ID = " + GetParent().GetC_BPartner_ID(), null, Get_Trx()));
                    if (po_PriceList_ID > 0)
                    {
                        Set_Value("VA077_PurchasePrice", GetPurchasePrice(po_PriceList_ID));
                    }
                }

                if (GetPriceEntered() != 0)
                {
                    if (duration != "")
                    {
                        decimal sp = (GetPriceEntered() * GetQtyEntered() * Util.GetValueOfDecimal(duration)) / 12;
                        decimal pp = (Util.GetValueOfDecimal(Get_Value("VA077_PurchasePrice")) * GetQtyEntered() * Util.GetValueOfDecimal(duration)) / 12;
                        margin = Decimal.Round(Decimal.Subtract(sp, pp), GetPrecision(), MidpointRounding.AwayFromZero);

                        //handle divide by zero case
                        if (sp != 0)
                            marginper = Decimal.Round(Decimal.Multiply(Decimal.Divide(margin, sp)
                                , Env.ONEHUNDRED), GetPrecision(), MidpointRounding.AwayFromZero);
                        else
                            marginper = Env.ZERO;

                        SetLineNetAmt(Decimal.Round(sp, GetPrecision()));
                        decimal taxAmt;
                        decimal lineTotalAmt;

                        MOrderTax tax = MOrderTax.Get(this, GetPrecision(), false, Get_TrxName());
                        MTax taxRate = tax.GetTax();
                        if (taxRate.GetRate() != 0)
                        {
                            taxAmt = ((GetPriceEntered() * GetQtyEntered()) * (taxRate.GetRate() / 100) * Util.GetValueOfDecimal(duration)) / 12;
                        }
                        else
                        {
                            taxAmt = 0;
                        }
                        SetTaxAmt(Decimal.Round(taxAmt, GetPrecision()));
                        lineTotalAmt = sp + taxAmt;

                        SetLineTotalAmt(Decimal.Round(lineTotalAmt, GetPrecision()));


                        // set Tax Amount in base currency
                        if (Get_ColumnIndex("TaxBaseAmt") > 0)
                        {
                            primaryAcctSchemaCurrency = GetCtx().GetContextAsInt("$C_Currency_ID");
                            if (Ord.GetC_Currency_ID() != primaryAcctSchemaCurrency)
                            {
                                taxAmt = MConversionRate.Convert(GetCtx(), GetTaxAmt(), primaryAcctSchemaCurrency, Ord.GetC_Currency_ID(),
                                                                                           Ord.GetDateAcct(), Ord.GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID());
                            }
                            else
                            {
                                taxAmt = GetTaxAmt();
                            }
                            SetTaxBaseAmt(taxAmt);
                        }

                        // set Taxable Amount -- (Line Total-Tax Amount)
                        if (Get_ColumnIndex("TaxAbleAmt") >= 0)
                        {
                            if (Get_ColumnIndex("SurchargeAmt") > 0)
                            {
                                SetTaxAbleAmt(Decimal.Subtract(Decimal.Subtract(GetLineTotalAmt(), GetTaxAmt()), GetSurchargeAmt()));
                            }
                            else
                            {
                                SetTaxAbleAmt(Decimal.Subtract(GetLineTotalAmt(), GetTaxAmt()));
                            }
                        }
                    }
                    else
                    {
                        margin = Decimal.Subtract(GetPriceEntered(), Util.GetValueOfDecimal(Get_Value("VA077_PurchasePrice"))) * GetQtyEntered();
                        margin = Decimal.Round(margin, GetPrecision(), MidpointRounding.AwayFromZero);

                        //handle divide by zero case
                        if (GetQtyEntered() != 0)
                            marginper = Decimal.Round(Decimal.Multiply(Decimal.Divide(margin, (GetPriceEntered() * GetQtyEntered()))
                            , Env.ONEHUNDRED), GetPrecision(), MidpointRounding.AwayFromZero);
                        else
                            marginper = Env.ZERO;
                    }
                }

                Set_Value("VA077_MarginAmt", margin);
                Set_Value("VA077_MarginPercent", marginper);

                // Set Values from Product when saving the Quotation Line
                if (newRecord && GetM_Product_ID() > 0)
                {
                    string str = @"SELECT VA077_SNErweiterbar, VA077_ShowOldSN, VA077_ContractProduct, VA077_ShowCNAutodesk,
                    VA077_NonContractProd, Description, DocumentNote, VA077_DestinationOrg, VA077_AdditionalInfo, VA077_UpdateVersion,
                    VA077_RegEmail FROM M_Product WHERE M_Product_ID=" + GetM_Product_ID();

                    DataSet dt = DB.ExecuteDataset(str, null, Get_Trx());
                    if (dt != null && dt.Tables[0].Rows.Count > 0)
                    {
                        bool snerw = Util.GetValueOfString(dt.Tables[0].Rows[0]["VA077_SNErweiterbar"]).Equals("Y");
                        bool showoldsn = Util.GetValueOfString(dt.Tables[0].Rows[0]["VA077_ShowOldSN"]).Equals("Y");
                        bool contract = Util.GetValueOfString(dt.Tables[0].Rows[0]["VA077_ContractProduct"]).Equals("Y");
                        bool autodesk = Util.GetValueOfString(dt.Tables[0].Rows[0]["VA077_ShowCNAutodesk"]).Equals("Y");
                        bool noncontract = Util.GetValueOfString(dt.Tables[0].Rows[0]["VA077_NonContractProd"]).Equals("Y");
                        string desc = Util.GetValueOfString(dt.Tables[0].Rows[0]["Description"]);
                        string notes = Util.GetValueOfString(dt.Tables[0].Rows[0]["DocumentNote"]);
                        string org = Util.GetValueOfString(dt.Tables[0].Rows[0]["VA077_DestinationOrg"]);
                        bool addInfo = Util.GetValueOfString(dt.Tables[0].Rows[0]["VA077_AdditionalInfo"]).Equals("Y");
                        bool updateversion = Util.GetValueOfString(dt.Tables[0].Rows[0]["VA077_UpdateVersion"]).Equals("Y");
                        bool regemail = Util.GetValueOfString(dt.Tables[0].Rows[0]["VA077_RegEmail"]).Equals("Y");
                        if (org != "")
                            Set_Value("VA077_DestinationOrg", Util.GetValueOfInt(org));
                        Set_Value("Description", desc + " " + notes);
                        Set_Value("VA077_SNErweiterbar", snerw);
                        Set_Value("VA077_ShowOldSN", showoldsn);
                        Set_Value("VA077_ContractProduct", contract);
                        Set_Value("VA077_NonContractProd", noncontract);

                        if (contract)
                            Set_Value("VA077_IsContract", contract);

                        Set_Value("VA077_ShowCNAutodesk", autodesk);
                        Set_Value("VA077_UpdateVersion", updateversion);
                        Set_Value("VA077_AdditionalInfo", addInfo);
                        Set_Value("VA077_ChkRegEmail", regemail);
                    }
                }
            }
                        
            return true;
        }

        /// <summary>
        /// Set IsTaxExempt and TaxExemptReason
        /// </summary>
        /// <writer>1052</writer>
        private void SetTaxExemptReason()
        {
            if (GetC_Tax_ID() > 0 && GetC_TaxExemptReason_ID() == 0 && Util.GetValueOfInt(Get_Value("C_Quotation_Line_ID")) == 0)
            {
                DataSet ds = DB.ExecuteDataset("SELECT IsTaxExempt,C_TaxExemptReason_ID FROM C_Tax WHERE IsActive='Y' AND IsTaxExempt='Y' AND C_Tax_ID=" + GetC_Tax_ID(), null, Get_Trx());
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    SetC_TaxExemptReason_ID(Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_TaxExemptReason_ID"]));
                    SetIsTaxExempt(Util.GetValueOfString(ds.Tables[0].Rows[0]["IsTaxExempt"]).Equals("Y") ? true : false);
                }
            }
            else if (GetC_Tax_ID() > 0 && GetC_TaxExemptReason_ID() > 0 && !IsTaxExempt())
            {
                //taxExpemt is false but  tax exempt reason is selected
                SetC_TaxExemptReason_ID(0);
            }
        }
        /// <summary>
        /// Set property to check wheater order's close event is called or any other event is called
        /// </summary>
        /// <param name="closed"> True/False</param>
        public void SetIsClosedDocument(bool closed)
        {
            isClosed = closed;
        }

        /// <summary>
        /// Get property to check wheater order's close event is called or any other event is called
        /// </summary>
        /// <returns>True if document is closing</returns>
        public bool GetIsClosedDocument()
        {
            return isClosed;
        }
        //end

        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>true if it can be deleted</returns>
        protected override bool BeforeDelete()
        {
            //	R/O Check - Something delivered. etc.
            if (Env.ZERO.CompareTo(GetQtyDelivered()) != 0)
            {
                log.SaveError("DeleteError", Msg.Translate(GetCtx(), "QtyDelivered") + "=" + GetQtyDelivered());
                return false;
            }
            if (Env.ZERO.CompareTo(GetQtyInvoiced()) != 0)
            {
                log.SaveError("DeleteError", Msg.Translate(GetCtx(), "QtyInvoiced") + "=" + GetQtyDelivered());
                return false;
            }
            if (Env.ZERO.CompareTo(GetQtyReserved()) != 0)
            {
                //	For PO should be On Order
                log.SaveError("DeleteError", Msg.Translate(GetCtx(), "QtyReserved") + "=" + GetQtyReserved());
                return false;
            }

            // when document is other that Drafted stage, then we cannot delete documnet line
            // But allowed to delete lines from Quotation.
            // VIS0060: Can delete lines with zero quantity.
            MOrder order = new MOrder(GetCtx(), GetC_Order_ID(), Get_Trx());
            if (order.GetDocStatus() != "DR" && !order.IsSalesQuotation() && !GetQtyEntered().Equals(Env.ZERO))
            {
                log.SaveError("DeleteError", Msg.GetMsg(GetCtx(), "CannotDeleteTrx"));
                return false;
            }
            // VIS0060: UnLink all the Requisition Lines linked with this order line.
            DB.ExecuteQuery("UPDATE M_RequisitionLine SET C_OrderLine_ID = NULL WHERE C_OrderLine_ID = " + Get_ID(), null, Get_Trx());
            return true;
        }

        public bool DeleteCheck()
        {
            //	R/O Check - Something delivered. etc.
            if (Env.ZERO.CompareTo(GetQtyDelivered()) != 0)
            {
                log.SaveError("DeleteError", Msg.Translate(GetCtx(), "QtyDelivered") + "=" + GetQtyDelivered());
                return false;
            }
            if (Env.ZERO.CompareTo(GetQtyInvoiced()) != 0)
            {
                log.SaveError("DeleteError", Msg.Translate(GetCtx(), "QtyInvoiced") + "=" + GetQtyDelivered());
                return false;
            }
            if (Env.ZERO.CompareTo(GetQtyReserved()) != 0)
            {
                //	For PO should be On Order
                log.SaveError("DeleteError", Msg.Translate(GetCtx(), "QtyReserved") + "=" + GetQtyReserved());
                return false;
            }
            return true;
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new records</param>
        /// <param name="success">success</param>
        /// <returns>saved</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            MOrder Ord = null;
            if (!success)
                return success;

            // Reset Amount Dimension on header after save of new record
            if (newRecord && GetLineNetAmt() != 0)
            {
                resetAmtDim = true;
                resetTotalAmtDim = true;
            }

            if (!IsProcessed())
            {
                Ord = GetParent();
                if (!newRecord && Is_ValueChanged("C_Tax_ID") && !(!String.IsNullOrEmpty(Ord.GetConditionalFlag()) &&
                    Ord.GetConditionalFlag().Equals(MOrder.CONDITIONALFLAG_PrepareIt)))
                {
                    //	Recalculate Tax for old Tax
                    MOrderTax tax = MOrderTax.Get(this, GetPrecision(), true, Get_TrxName());	//	old Tax
                    if (tax != null)
                    {
                        if (!tax.CalculateTaxFromLines(null))
                            return false;
                        if (!tax.Save(Get_TrxName()))
                            return false;
                    }

                    // if Surcharge Tax is selected then calculate Tax for this Surcharge Tax.
                    if (Get_ColumnIndex("SurchargeAmt") > 0)
                    {
                        tax = MOrderTax.GetSurcharge(this, GetPrecision(), true, Get_TrxName());  //	old Tax
                        if (tax != null)
                        {
                            if (!tax.CalculateSurchargeFromLines())
                                return false;
                            if (!tax.Save(Get_TrxName()))
                                return false;
                        }
                    }
                }
                if (!(!String.IsNullOrEmpty(Ord.GetConditionalFlag()) &&
                    Ord.GetConditionalFlag().Equals(MOrder.CONDITIONALFLAG_PrepareIt)))
                {
                    if (!UpdateHeaderTax())
                        return false;
                }
                // Warning message needs to display in case Entered Price is less than Cost of the Product on Sales Order                
                if (Ord.IsSOTrx() && !Ord.IsReturnTrx() && GetM_Product_ID() > 0)
                {
                    if (GetProduct().GetProductType().Equals("I") && GetPriceEntered() < GetCurrentCostPrice())
                    {
                        log.SaveWarning("Warning", Msg.GetMsg(GetCtx(), "VIS_PrcEntCantlessPrdCost"));
                    }
                }
            }

            // nnayak : Changes for bug 1535824 - Order: Fully Invoiced
            if (!newRecord && Is_ValueChanged("QtyInvoiced"))
            {
                //Ord = new MOrder(GetCtx(), GetC_Order_ID(), Get_TrxName());
                //MOrderLine[] oLines = Ord.GetLines(true, null);
                //bool isInvoiced = true;
                //for (int i = 0; i < oLines.Length; i++)
                //{
                //    MOrderLine line = oLines[i];
                //    if (line.GetQtyInvoiced().CompareTo(line.GetQtyOrdered()) < 0)
                //    {
                //        isInvoiced = false;
                //        break;
                //    }
                //}
                //Ord.SetIsInvoiced(isInvoiced);

                //if (!Ord.Save())
                //    return false;

                bool isInvoiced = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(C_OrderLine_ID) FROM C_OrderLine WHERE C_Order_ID=" + GetC_Order_ID()
                        + " AND QtyInvoiced < QtyOrdered")) <= 0;
                int no = DB.ExecuteQuery("UPDATE C_Order SET IsInvoiced = " + (isInvoiced ? "'Y'" : "'N'") + " WHERE C_Order_ID=" + GetC_Order_ID(), null, Get_Trx());
                if (no < 0)
                {
                    return false;
                }

            }

            //Develop by Deekshant For VA077 Module for calculating purchase,sales,margin
            if (VAdvantage.Utility.Env.IsModuleInstalled("VA077_"))
            {
                //Check if the SO created from SQ
                if (Util.GetValueOfInt(Get_Value("C_Quotation_Line_ID")) > 0)
                {
                    // Copy the VA077_IsContract from SQ and update to SO 
                    string qry = @"UPDATE C_OrderLine 
                                   SET VA077_IsContract=(SELECT VA077_IsContract 
                                   FROM C_OrderLine WHERE C_OrderLine_ID =" + Util.GetValueOfInt(Get_Value("C_Quotation_Line_ID")) + @")                          
                                   WHERE C_OrderLine_ID=" + GetC_OrderLine_ID();
                    int no = DB.ExecuteQuery(qry, null, Get_TrxName());
                    if (no <= 0)
                    {
                        log.SaveWarning("", Msg.GetMsg(GetCtx(), "VIS_NotUpdated"));
                    }
                }

                if (!UpdateMargins())
                {
                    log.SaveWarning("", Msg.GetMsg(GetCtx(), "VIS_NotUpdated"));
                    return false;
                }
            }


            /********************************************************
                * Module Name    : official-VAFramework-ERP-CRM
                * Purpose        : Calculating Metal Consumption For cable Industory
                * Chronological Development
                * VIS0228      26 April 2021
             ******************************************************/

            if (VAdvantage.Utility.Env.IsModuleInstalled("VA076_"))
            {
                if (!UpdateMetalConsumption(GetC_Order_ID()))
                {
                    return false;
                }
            }
            return true;

        }

        /// <summary>
        /// After Delete
        /// </summary>
        /// <param name="success">success</param>
        /// <returns>deleted</returns>
        protected override bool AfterDelete(bool success)
        {
            if (!success)
                return success;

            if (GetS_ResourceAssignment_ID() != 0)
            {
                /////////////////////////////////////////////////////////////
                MResourceAssignment ra = new MResourceAssignment(GetCtx(), GetS_ResourceAssignment_ID(), Get_TrxName());
                ra.Delete(true);
            }

            // Reset Amount Dimension on header after delete of non zero line
            if (GetLineNetAmt() != 0)
            {
                resetAmtDim = true;
                resetTotalAmtDim = true;
            }

            //Develop by Deekshant For VA077 Module for calculating purchase,sales,margin
            if (VAdvantage.Utility.Env.IsModuleInstalled("VA077_"))
            {
                if (!UpdateMargins())
                {
                    return false;
                }
            }

            /********************************************************
                * Module Name    : official-VAFramework-ERP-CRM
                * Purpose        : Calculating Metal Consumption For cable Industory
                * Chronological Development
                * VIS0228      26 April 2021
             ******************************************************/
            if (VAdvantage.Utility.Env.IsModuleInstalled("VA076_"))
            {
                if (!UpdateMetalConsumption(GetC_Order_ID()))
                {
                    return false;
                }
            }

            return UpdateHeaderTax();
        }

        /// <summary>
        /// Update margins and other fields on header based on the lines saved
        /// </summary>
        /// <returns>true/false</returns>
        private bool UpdateMargins()
        {
            if (GetC_Order_ID() > 0)
            {
                DataSet contDS = null;

                string sql = @"SELECT VA077_ServiceContract_ID, VA077_HistoricContractDate, VA077_ContractCPStartDate, DocumentNo,
                                   C_Frequency_ID, VA077_ContractCPEndDate, VA077_AnnualValue FROM VA077_ServiceContract 
                                   WHERE VA077_ServiceContract_ID IN(SELECT MAX(VA077_ServiceContract_ID) FROM C_OrderLine 
                                   WHERE C_Order_ID =" + GetC_Order_ID() + " AND VA077_ServiceContract_ID IS NOT NULL)";
                contDS = DB.ExecuteDataset(sql, null, Get_TrxName());
                DateTime? HCDate = null;
                DateTime? StartDate = null;
                DateTime? EndDate = null;
                Decimal AnnualValue = 0;
                StringBuilder qry = new StringBuilder();

                if (contDS != null && contDS.Tables[0].Rows.Count > 0)
                {
                    HCDate = Util.GetValueOfDateTime(contDS.Tables[0].Rows[0]["VA077_HistoricContractDate"]);
                    StartDate = Util.GetValueOfDateTime(contDS.Tables[0].Rows[0]["VA077_ContractCPStartDate"]);
                    EndDate = Util.GetValueOfDateTime(contDS.Tables[0].Rows[0]["VA077_ContractCPEndDate"]);
                    if (GetQtyEntered() != 0) // In the case of void qty will be zero, so set annual value only if qty is greater than zero
                        AnnualValue = Util.GetValueOfDecimal(contDS.Tables[0].Rows[0]["VA077_AnnualValue"]);

                    qry.Clear();
                    qry.Append(@"UPDATE C_Order  p SET VA077_TotalMarginAmt=(SELECT COALESCE(SUM(pl.VA077_MarginAmt),0) FROM C_OrderLine pl 
                            WHERE pl.IsActive = 'Y' AND pl.C_Order_ID = " + GetC_Order_ID() + @"),
                            VA077_MarginPercent=(SELECT CASE WHEN Sum(LineNetAmt) <> 0 Then 
                            ROUND(COALESCE(((Sum(LineNetAmt) - Sum(
                                                                CASE WHEN VA077_Duration is null THEN
                                                                    COALESCE(VA077_PurchasePrice, 0) * QtyEntered
                                                                    WHEN VA077_Duration > 0 THEN
                                                                    COALESCE(VA077_PurchasePrice, 0) * QtyEntered * VA077_Duration/12 
                                                                END)) / Sum(LineNetAmt) * 100), 0), " + GetPrecision() + @") 
                            ELSE 0  END FROM C_OrderLine pl WHERE pl.IsActive = 'Y' AND pl.C_Order_ID = " + GetC_Order_ID() + @"),
                            VA077_TotalPurchaseAmt = (SELECT ROUND(COALESCE(SUM(
                                                                              CASE WHEN VA077_Duration is null THEN  
                                                                              pl.VA077_PurchasePrice * QtyEntered  
                                                                              WHEN VA077_Duration > 0 THEN  
                                                                              pl.VA077_PurchasePrice * QtyEntered * VA077_Duration / 12  
                                                                              END), 0), " + GetPrecision() + @") FROM C_OrderLine pl
                                WHERE pl.IsActive = 'Y' AND pl.C_Order_ID = " + GetC_Order_ID() + @"),
                            VA077_TotalSalesAmt = (SELECT COALESCE(SUM(pl.LineNetAmt), 0) FROM C_OrderLine pl
                               WHERE pl.IsActive = 'Y' AND pl.C_Order_ID = " + GetC_Order_ID() + @")");
                    if (contDS.Tables[0].Rows[0]["VA077_HistoricContractDate"] != null)
                    {
                        qry.Append(",VA077_HistoricContractDate=" + GlobalVariable.TO_DATE(HCDate, true) + @"");
                    }
                    if (contDS.Tables[0].Rows[0]["VA077_ContractCPStartDate"] != null)
                    {
                        qry.Append(",VA077_ContractCPStartDate = " + GlobalVariable.TO_DATE(StartDate, true) + @"");
                    }
                    if (contDS.Tables[0].Rows[0]["VA077_ContractCPEndDate"] != null)
                    {
                        qry.Append(",VA077_ContractCPEndDate= " + GlobalVariable.TO_DATE(EndDate, true) + @"");
                    }

                    qry.Append(",VA077_OldAnnualContractTotal= " + AnnualValue + @", 
                                     VA077_ChangeStartDate = (SELECT MIN(VA077_StartDate) FROM C_OrderLine pl WHERE pl.IsActive = 'Y' 
                                     AND pl.C_Order_ID = " + GetC_Order_ID() + @"),
                                     VA077_PartialAmtCatchUp =(SELECT COALESCE(SUM(pl.LineNetAmt),0) FROM C_OrderLine pl 
                                     WHERE pl.IsActive = 'Y' AND pl.VA077_ContractProduct='Y' AND pl.C_Order_ID = " + GetC_Order_ID() + @"),
                                     VA077_AdditionalAnnualCharge =(SELECT ROUND(COALESCE(SUM(pl.PriceEntered * QtyEntered),0)," + GetPrecision() + @") FROM C_OrderLine pl 
                                     WHERE pl.IsActive = 'Y' AND pl.VA077_ContractProduct='Y' AND pl.C_Order_ID = " + GetC_Order_ID() + @"),
                                     VA077_NewAnnualContractTotal=(SELECT " + AnnualValue + @" + ROUND(COALESCE(SUM(pl.PriceEntered * QtyEntered),0)," + GetPrecision() + @") FROM C_OrderLine pl 
                                     WHERE pl.IsActive = 'Y' AND pl.VA077_ContractProduct='Y' AND pl.C_Order_ID = " + GetC_Order_ID() + @")
                                     WHERE p.C_Order_ID=" + GetC_Order_ID() + "");
                }
                else
                {
                    qry.Clear();
                    qry.Append(@"UPDATE C_Order  p SET VA077_TotalMarginAmt=(SELECT COALESCE(SUM(pl.VA077_MarginAmt),0) FROM C_OrderLine pl 
                            WHERE pl.IsActive = 'Y' AND pl.C_Order_ID = " + GetC_Order_ID() + @"),                                                        
                            VA077_TotalSalesAmt=(SELECT COALESCE(SUM(pl.LineNetAmt),0) FROM C_OrderLine pl 
                            WHERE pl.IsActive = 'Y' AND pl.C_Order_ID = " + GetC_Order_ID() + @"),
                            VA077_MarginPercent=(SELECT CASE WHEN Sum(LineNetAmt) <> 0 Then 
                            ROUND(COALESCE(((Sum(LineNetAmt) - Sum(
                                                                CASE WHEN VA077_Duration is null THEN
                                                                    COALESCE(VA077_PurchasePrice, 0) * QtyEntered
                                                                    WHEN VA077_Duration > 0 THEN
                                                                    COALESCE(VA077_PurchasePrice, 0) * QtyEntered * VA077_Duration/12 
                                                                END)) / Sum(LineNetAmt) * 100), 0), " + GetPrecision() + @") 
                            ELSE 0  END FROM C_OrderLine pl WHERE pl.IsActive = 'Y' AND pl.C_Order_ID = " + GetC_Order_ID() + @"),
                            VA077_TotalPurchaseAmt=(SELECT ROUND(COALESCE(SUM(
                                                                            CASE WHEN VA077_Duration is null THEN
                                                                            pl.VA077_PurchasePrice * QtyEntered
                                                                            WHEN VA077_Duration > 0 THEN
                                                                            pl.VA077_PurchasePrice * QtyEntered * VA077_Duration/12
                                                                            END),0)," + GetPrecision() + @") FROM C_OrderLine pl  
                            WHERE pl.IsActive = 'Y' AND pl.C_Order_ID = " + GetC_Order_ID() + @"),
                            VA077_ChangeStartDate = (SELECT MIN(VA077_StartDate) FROM C_OrderLine pl WHERE pl.IsActive = 'Y' 
                            AND pl.C_Order_ID = " + GetC_Order_ID() + @"),
                            VA077_AdditionalAnnualCharge =(SELECT ROUND(COALESCE(SUM(pl.PriceEntered * QtyEntered),0)," + GetPrecision() + @") FROM C_OrderLine pl 
                            WHERE pl.IsActive = 'Y' AND pl.VA077_ContractProduct='Y' AND pl.C_Order_ID = " + GetC_Order_ID() + @"),
                            VA077_NewAnnualContractTotal=(SELECT " + AnnualValue + @" + ROUND(COALESCE(SUM(pl.PriceEntered * QtyEntered),0)," + GetPrecision() + @") FROM C_OrderLine pl 
                            WHERE pl.IsActive = 'Y' AND pl.VA077_ContractProduct='Y' AND pl.C_Order_ID = " + GetC_Order_ID() + @"),
                            VA077_PartialAmtCatchUp =(SELECT COALESCE(SUM(pl.LineNetAmt),0) FROM C_OrderLine pl 
                            WHERE pl.IsActive = 'Y' AND pl.VA077_ContractProduct='Y' AND pl.C_Order_ID = " + GetC_Order_ID() + @")");

                    qry.Append(" WHERE p.C_Order_ID=" + GetC_Order_ID() + "");
                }

                int no = DB.ExecuteQuery(qry.ToString(), null, Get_TrxName());
                if (no <= 0)
                {
                    log.SaveWarning("", Msg.GetMsg(GetCtx(), "VIS_NotUpdated"));
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Update Tax & Header
        /// </summary>
        /// <returns>true if header updated</returns>
        private bool UpdateHeaderTax()
        {
            //	Recalculate Tax for this Tax
            MOrderTax tax = MOrderTax.Get(this, GetPrecision(), false, Get_TrxName());	//	current Tax
            if (!tax.CalculateTaxFromLines(null))
                return false;
            if (!tax.Save(Get_TrxName()))
                return false;

            MTax taxRate = tax.GetTax();

            // If Tax selected is Summary Level then Create or Update Child Tax
            if (taxRate.IsSummary())
            {
                if (!CalculateChildTax(GetParent(), tax, taxRate, Get_TrxName()))
                {
                    return false;
                }
            }

            // if Surcharge Tax is selected then calculate Tax for this Surcharge Tax.
            else if (Get_ColumnIndex("SurchargeAmt") > 0 && taxRate.Get_ColumnIndex("Surcharge_Tax_ID") > 0 && taxRate.GetSurcharge_Tax_ID() > 0)
            {
                tax = MOrderTax.GetSurcharge(this, GetPrecision(), false, Get_TrxName());  //	current Tax
                if (!tax.CalculateSurchargeFromLines())
                    return false;
                if (!tax.Save(Get_TrxName()))
                    return false;
            }

            //	Update Order Header
            String sql = "UPDATE C_Order i"
                + " SET TotalLines="
                    + "(SELECT COALESCE(SUM(LineNetAmt),0) FROM C_OrderLine il WHERE i.C_Order_ID=il.C_Order_ID) "
                    + (resetAmtDim ? ", AmtDimSubTotal = null " : "")       // reset Amount Dimension if Sub Total Amount is different
                    + (resetTotalAmtDim ? ", AmtDimGrandTotal = null " : "")     // reset Amount Dimension if Grand Total Amount is different
                + "WHERE C_Order_ID=" + GetC_Order_ID();
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 1)
            {
                log.Warning("(1) #" + no);
            }

            if (IsTaxIncluded())
                sql = "UPDATE C_Order i "
                    + "SET GrandTotal=TotalLines "
                    + "WHERE C_Order_ID=" + GetC_Order_ID();
            else
                sql = "UPDATE C_Order i "
                    + "SET GrandTotal=TotalLines+"
                        + "(SELECT COALESCE(SUM(TaxAmt),0) FROM C_OrderTax it WHERE i.C_Order_ID=it.C_Order_ID) "
                        + "WHERE C_Order_ID=" + GetC_Order_ID();
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 1)
            {
                log.Warning("(2) #" + no);
            }
            _parent = null;
            return no == 1;
        }

        /// <summary>
        /// Create or Update Child Tax
        /// </summary>
        /// <param name="order">Order Hearder</param>
        /// <param name="oTax">Order Tax</param>
        /// <param name="tax">Tax</param>
        /// <param name="trxName">Trx Object</param>
        /// <returns>true, if Tax calculated</returns>
        private bool CalculateChildTax(MOrder order, MOrderTax oTax, MTax tax, Trx trxName)
        {
            MTax[] cTaxes = tax.GetChildTaxes(false);	//	Multiple taxes
            for (int j = 0; j < cTaxes.Length; j++)
            {
                MOrderTax newITax = null;
                MTax cTax = cTaxes[j];
                Decimal taxAmt = cTax.CalculateTax(oTax.GetTaxBaseAmt(), false, GetPrecision());

                // check child tax record is avialable or not 
                // if not then create new record
                String sql = "SELECT * FROM C_OrderTax WHERE C_Order_ID=" + order.GetC_Order_ID() + " AND C_Tax_ID=" + cTax.GetC_Tax_ID();
                try
                {
                    DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                    if (ds.Tables.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            newITax = new MOrderTax(GetCtx(), dr, trxName);
                        }
                    }
                }
                catch (Exception e)
                {
                    _log.Log(Level.SEVERE, sql, e);
                }

                if (newITax != null)
                {
                    newITax.Set_TrxName(trxName);
                }

                // Create New
                if (newITax == null)
                {
                    newITax = new MOrderTax(GetCtx(), 0, Get_TrxName());
                    newITax.SetClientOrg(this);
                    newITax.SetC_Order_ID(GetC_Order_ID());
                    newITax.SetC_Tax_ID(cTax.GetC_Tax_ID());
                }

                newITax.SetPrecision(GetPrecision());
                newITax.SetIsTaxIncluded(IsTaxIncluded());
                newITax.SetTaxBaseAmt(oTax.GetTaxBaseAmt());
                newITax.SetTaxAmt(taxAmt);
                //Set Tax Amount (Base Currency) on Invoice Tax Window 
                //                if (newITax.Get_ColumnIndex("TaxBaseCurrencyAmt") > 0)
                //                {
                //                    decimal? baseTaxAmt = taxAmt;
                //                    int primaryAcctSchemaCurrency = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT C_Currency_ID FROM C_AcctSchema WHERE C_AcctSchema_ID = 
                //                                            (SELECT c_acctschema1_id FROM ad_clientinfo WHERE ad_client_id = " + GetAD_Client_ID() + ")", null, Get_Trx()));
                //                    if (order.GetC_Currency_ID() != primaryAcctSchemaCurrency)
                //                    {
                //                        baseTaxAmt = MConversionRate.Convert(GetCtx(), taxAmt, primaryAcctSchemaCurrency, order.GetC_Currency_ID(),
                //                                                                                   order.GetDateAcct(), order.GetC_ConversionType_ID(), GetAD_Client_ID(), GetAD_Org_ID());
                //                    }
                //                    newITax.Set_Value("TaxBaseCurrencyAmt", baseTaxAmt);
                //                }
                if (!newITax.Save(Get_TrxName()))
                    return false;
            }
            // Delete Summary Level Tax Line
            if (!oTax.Delete(true, Get_TrxName()))
                return false;

            return true;
        }

        /// <summary>
        /// Create Drop Ship Warehouse
        /// </summary>
        /// <returns>Warehouse ID</returns>
        private int CreateDropShipWareHouse(MWarehouse wareH)
        {
            MWarehouse wh = new MWarehouse(GetCtx(), 0, Get_Trx());
            MOrg org = new MOrg(GetCtx(), GetAD_Org_ID(), Get_Trx());
            wh.SetAD_Client_ID(GetAD_Client_ID());
            wh.SetAD_Org_ID(GetAD_Org_ID());
            wh.SetName(org.GetName() + " Drop Ship WareHouse");
            wh.SetValue(org.GetName() + " Drop Ship WareHouse");
            wh.SetIsDropShip(true);
            int _Location_ID = 0;
            if (org.GetInfo() != null)
            {
                if (org.GetInfo().GetC_Location_ID() != 0)
                    _Location_ID = org.GetInfo().GetC_Location_ID();
            }
            if (_Location_ID == 0)
            {
                //int _wID = Util.GetValueOfInt(DB.ExecuteScalar("Select M_WareHouse_ID From C_Order Where C_Order_ID=" + GetC_Order_ID()));
                //MWarehouse wareH = new MWarehouse(GetCtx(), _wID, Get_Trx());
                _Location_ID = wareH.GetC_Location_ID();
            }
            wh.SetC_Location_ID(_Location_ID);
            if (wh.Save())
            {
                MLocator mLoc = new MLocator(wh, "Drop Ship Locator");
                mLoc.SetIsDefault(true);
                mLoc.SetPriorityNo(50);
                mLoc.Save();
            }
            return wh.GetM_Warehouse_ID();
        }

        // Added Methods to Set and Get from Process property, if record is created from Process or Window

        /// <summary>
        /// Set Value in From Process
        /// </summary>
        /// <param name="fromProcess">true or False</param>
        public void SetFromProcess(bool fromProcess)
        {
            _fromProcess = fromProcess;
        }

        /// <summary>
        /// Get Value of From Process
        /// </summary>
        /// <returns>bool, true if created from Process</returns>
        public bool GetFromProcess()
        {
            return _fromProcess;
        }

        /// <summary>
        /// Module Name    : official-VAFramework-ERP-CRM
        /// Purpose        : Calculating Metal Consumption For cable Industory
        /// Chronological Development
        /// VIS0228      26 April 2021
        /// <param name="c_order_ID"></param>
        /// <returns>bool</returns>
        public bool UpdateMetalConsumption(int c_order_ID)
        {
            string qry = @"UPDATE C_Order O
                                 SET (O.VA076_ALAlAlloyTotConMT, O.VA076_CUTotConMT,O.VA076_LeadTotConMT) =
                                    (
                                        SELECT ALMetal,CUMetal,LeadMetal FROM(
                                        SELECT OL.C_Order_ID, Sum(NVL(Atr.VA076_ALMetalWeight,0)*QtyEntered)/1000000 AS ALMetal,Sum(NVL(VA076_CUMetalWeight,0)*QtyEntered)/1000000 AS CUMetal,Sum(NVL(VA076_LeadMetalWeight,0)*QtyEntered)/1000000 AS LeadMetal 
                                        FROM C_OrderLine OL 
                                        INNER JOIN VA076_Attributes Atr ON Atr.M_Product_ID=OL.M_Product_ID
                                        WHERE OL.C_Order_ID=" + c_order_ID + @" group by OL.C_Order_ID) T
                                    ) WHERE  O.C_Order_ID =" + c_order_ID;

            int no = DB.ExecuteQuery(qry, null, Get_TrxName());
            if (no <= 0)
            {
                log.SaveWarning("", Msg.GetMsg(GetCtx(), "VIS_NotUpdated"));
                return false;
            }
            return true;
        }

    }
}
