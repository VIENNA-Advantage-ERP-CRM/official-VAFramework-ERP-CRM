/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : DocLine
 * Purpose        : Create Document Line
 * Class Used     : none
 * Chronological    Development
 * Raghunandan      13-Jan-2010
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
using VAdvantage.Report;
using System.Data.SqlClient;

namespace VAdvantage.Acct
{
    public class DocLine
    {
        #region Private Variables

        // Persistent Object		
        protected PO _po = null;
        // Parent					
        private Doc _doc = null;
        //	 Log					
        protected VLogger log = null;
        // Qty                     
        private Decimal? _qty = null;

        //  -- GL Amounts
        // Debit Journal Amt   	
        private Decimal _AmtSourceDr = Env.ZERO;
        // Credit Journal Amt		
        private Decimal _AmtSourceCr = Env.ZERO;
        // Net Line Amt            
        private Decimal? _LineNetAmt = null;
        // List Amount             
        private Decimal _ListAmt = Env.ZERO;
        // Discount Amount         
        private Decimal _DiscountAmt = Env.ZERO;

        // Converted Amounts   	
        private Decimal? _AmtAcctDr = null;
        private Decimal? _AmtAcctCr = null;
        // Acct Schema				
        private int _C_AcctSchema_ID = 0;

        //	Product Costs			
        private ProductCost _productCost = null;
        // Production indicator	
        private bool _productionBOM = false;
        // Account used only for GL Journal    
        private MAccount _account = null;

        // Accounting Date				
        private DateTime? _DateAcct = null;
        // Document Date				
        private DateTime? _DateDoc = null;
        // Sales Region				
        private int _C_SalesRegion_ID = -1;
        // Sales Region				
        private int _C_BPartner_ID = -1;
        // Location From				
        private int _C_LocFrom_ID = 0;
        // Location To					
        private int _C_LocTo_ID = 0;
        // Item						
        private Boolean? _isItem = null;
        // Currency					
        private int _C_Currency_ID = -1;
        // Conversion Type				
        private int _C_ConversionType_ID = -1;
        // Period						
        private int _C_Period_ID = -1;

        #endregion


        /// <summary>
        /// Create Document Line
        /// </summary>
        /// <param name="po">line persistent object</param>
        /// <param name="doc">header</param>
        public DocLine(PO po, Doc doc)
        {
            log = VLogger.GetVLogger(this.GetType().FullName);
            if (po == null)
            {
                throw new ArgumentException("PO is null");
            }
            _po = po;
            _doc = doc;
            //
            //  Document Consistency
            if (_po.GetAD_Org_ID() == 0)
                _po.SetAD_Org_ID(_doc.GetAD_Org_ID());
        }

        /// <summary>
        /// Get Currency
        /// </summary>
        /// <returns>c_Currency_ID</returns>
        public int GetC_Currency_ID()
        {
            if (_C_Currency_ID == -1)
            {
                int index = _po.Get_ColumnIndex("C_Currency_ID");
                if (index != -1)
                {
                    int? ii = (int?)_po.Get_Value(index);
                    if (ii != null)
                    {
                        _C_Currency_ID = ii.Value;//.intValue();
                    }
                }
                if (_C_Currency_ID <= 0)
                {
                    _C_Currency_ID = _doc.GetC_Currency_ID();
                }
            }
            return _C_Currency_ID;
        }

        /// <summary>
        /// Get Conversion Type
        /// </summary>
        /// <returns>C_ConversionType_ID</returns>
        public int GetC_ConversionType_ID()
        {
            if (_C_ConversionType_ID == -1)
            {
                int index = _po.Get_ColumnIndex("C_ConversionType_ID");
                if (index != -1)
                {
                    int? ii = (int?)_po.Get_Value(index);
                    if (ii != null)
                        _C_ConversionType_ID = ii.Value;//.intValue();
                }
                if (_C_ConversionType_ID <= 0)
                {
                    _C_ConversionType_ID = _doc.GetC_ConversionType_ID();
                }
            }
            return _C_ConversionType_ID;
        }

        /// <summary>
        /// Set C_ConversionType_ID
        /// </summary>
        /// <param name="C_ConversionType_ID"></param>
        public void SetC_ConversionType_ID(int C_ConversionType_ID)
        {
            _C_ConversionType_ID = C_ConversionType_ID;
        }

        /// <summary>
        /// Set Amount (DR)
        /// </summary>
        /// <param name="sourceAmt"></param>
        public void SetAmount(Decimal? sourceAmt)
        {
            _AmtSourceDr = sourceAmt == null ? Env.ZERO : sourceAmt.Value;
            _AmtSourceCr = Env.ZERO;
        }

        /// <summary>
        /// Set Amounts
        /// </summary>
        /// <param name="amtSourceDr">source amount dr</param>
        /// <param name="amtSourceCr">source amount cr</param>
        public void SetAmount(Decimal? amtSourceDr, Decimal? amtSourceCr)
        {
            _AmtSourceDr = amtSourceDr == null ? Env.ZERO : amtSourceDr.Value;
            _AmtSourceCr = amtSourceCr == null ? Env.ZERO : amtSourceCr.Value;
        }

        /// <summary>
        /// Set Converted Amounts
        /// </summary>
        /// <param name="C_AcctSchema_ID">acct schema</param>
        /// <param name="amtAcctDr">acct amount dr</param>
        /// <param name="amtAcctCr">acct amount cr</param>
        public void SetConvertedAmt(int C_AcctSchema_ID, Decimal amtAcctDr, Decimal amtAcctCr)
        {
            _C_AcctSchema_ID = C_AcctSchema_ID;
            _AmtAcctDr = amtAcctDr;
            _AmtAcctCr = amtAcctCr;
        }

        /// <summary>
        ///  Line Net Amount or Dr-Cr
        /// </summary>
        /// <returns>balance</returns>
        public Decimal GetAmtSource()
        {
            return Decimal.Subtract(_AmtSourceDr, _AmtSourceCr);
        }

        /// <summary>
        /// Get (Journal) Line Source Dr Amount
        /// </summary>
        /// <returns>DR source amount</returns>
        public Decimal GetAmtSourceDr()
        {
            return _AmtSourceDr;
        }

        /// <summary>
        /// Get (Journal) Line Source Cr Amount
        /// </summary>
        /// <returns>CR source amount</returns>
        public Decimal GetAmtSourceCr()
        {
            return _AmtSourceCr;
        }

        /// <summary>
        ///  Line Journal Accounted Dr Amount
        /// </summary>
        /// <returns>DR accounted amount</returns>
        public Decimal? GetAmtAcctDr()
        {
            return _AmtAcctDr;
        }

        /// <summary>
        /// Line Journal Accounted Cr Amount
        /// </summary>
        /// <returns>CR accounted amount</returns>
        public Decimal? GetAmtAcctCr()
        {
            return _AmtAcctCr;
        }

        /// <summary>
        /// Charge Amount
        /// </summary>
        /// <returns>charge amount</returns>
        public Decimal GetChargeAmt()
        {
            int index = _po.Get_ColumnIndex("ChargeAmt");
            if (index != -1)
            {
                Decimal? bd = (Decimal?)_po.Get_Value(index);
                if (bd != null)
                {
                    return bd.Value;
                }
            }
            return Env.ZERO;
        }

        /// <summary>
        /// Set Product Amounts
        /// </summary>
        /// <param name="LineNetAmt"></param>
        /// <param name="PriceList"></param>
        /// <param name="Qty">Qty for discount calc</param>
        public void SetAmount(Decimal? LineNetAmt, Decimal? PriceList, Decimal? Qty)
        {
            _LineNetAmt = LineNetAmt == null ? Env.ZERO : LineNetAmt;

            if (PriceList != null && Qty != null)
            {
                _ListAmt = Decimal.Multiply(PriceList.Value, Qty.Value);
            }
            if (_ListAmt.Equals(Env.ZERO))
            {
                _ListAmt = _LineNetAmt.Value;
            }
            _DiscountAmt = Decimal.Subtract(_ListAmt, _LineNetAmt.Value);
            //
            SetAmount(_ListAmt, _DiscountAmt);

        }

        /// <summary>
        /// Line Discount
        /// </summary>
        /// <returns>discount amount</returns>
        public Decimal GetDiscount()
        {
            return _DiscountAmt;
        }

        /// <summary>
        /// Line List Amount
        /// </summary>
        /// <returns>list amount</returns>
        public Decimal GetListAmount()
        {
            return _ListAmt;
        }

        /// <summary>
        /// Set Line Net Amt Difference
        /// </summary>
        /// <param name="diff">difference (to be subtracted)</param>
        public void SetLineNetAmtDifference(Decimal diff)
        {
            String msg = "Diff=" + diff + " - LineNetAmt=" + _LineNetAmt;
            _LineNetAmt = Decimal.Subtract(_LineNetAmt.Value, diff);
            _DiscountAmt = Decimal.Subtract(_ListAmt, _LineNetAmt.Value);
            SetAmount(_ListAmt, _DiscountAmt);
            msg += " -> " + _LineNetAmt;
            log.Fine(msg);
        }

        /// <summary>
        /// Set Accounting Date
        /// </summary>
        /// <param name="dateAcct">acct date</param>
        public void SetDateAcct(DateTime? dateAcct)
        {
            _DateAcct = dateAcct;
        }

        /// <summary>
        /// Get Accounting Date
        /// </summary>
        /// <returns>accounting date</returns>
        public DateTime? GetDateAcct()
        {
            if (_DateAcct != null)
            {
                return _DateAcct;
            }
            int index = _po.Get_ColumnIndex("DateAcct");
            if (index != -1)
            {
                _DateAcct = (DateTime?)_po.Get_Value(index);
                if (_DateAcct != null)
                {
                    return _DateAcct;
                }
            }
            _DateAcct = _doc.GetDateAcct();
            return _DateAcct;
        }

        /// <summary>
        /// Set Document Date
        /// </summary>
        /// <param name="dateDoc">doc date</param>
        public void SetDateDoc(DateTime? dateDoc)
        {
            _DateDoc = dateDoc;
        }

        /// <summary>
        /// Get Document Date
        /// </summary>
        /// <returns>document date</returns>
        public DateTime? GetDateDoc()
        {
            if (_DateDoc != null)
            {
                return _DateDoc;
            }
            int index = _po.Get_ColumnIndex("DateDoc");
            if (index != -1)
            {
                _DateDoc = (DateTime?)_po.Get_Value(index);
                if (_DateDoc != null)
                {
                    return _DateDoc;
                }
            }
            _DateDoc = _doc.GetDateDoc();
            return _DateDoc;
        }


        /// <summary>
        /// Set GL Journal Account
        /// </summary>
        /// <param name="acct">account</param>
        public void SetAccount(MAccount acct)
        {
            _account = acct;
        }

        /// <summary>
        /// Get GL Journal Account
        /// </summary>
        /// <returns>account</returns>
        public MAccount GetAccount()
        {
            return _account;
        }

        /// <summary>
        /// Line Account from Product (or Charge).
        /// </summary>
        /// <param name="AcctType"></param>
        /// <param name="as1"></param>
        /// <returns>Requested Product Account</returns>
        public MAccount GetAccount(int AcctType, MAcctSchema as1)
        {
            //	Charge Account
            if (GetM_Product_ID() == 0 && GetC_Charge_ID() != 0)
            {
                Decimal amt = new Decimal(-1);		//	Revenue (-)
                if (!_doc.IsSOTrx())
                {
                    amt = new Decimal(+1);				//	Expense (+)
                }
                MAccount acct = GetChargeAccount(as1, amt);
                if (acct != null)
                {
                    return acct;
                }
            }
            //	Product Account
            return GetProductCost().GetAccount(AcctType, as1);
        }

        /// <summary>
        /// Get Charge
        /// </summary>
        /// <returns>C_Charge_ID</returns>
        public int GetC_Charge_ID()
        {
            int index = _po.Get_ColumnIndex("C_Charge_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return ii.Value;//.intValue();
                }
            }
            return 0;
        }

        /// <summary>
        /// Get Charge Account
        /// </summary>
        /// <param name="as1">account schema</param>
        /// <param name="amount">amount for expense(+)/revenue(-)</param>
        /// <returns>Charge Account or null</returns>
        public MAccount GetChargeAccount(MAcctSchema as1, Decimal? amount)
        {
            int C_Charge_ID = GetC_Charge_ID();
            if (C_Charge_ID == 0)
            {
                return null;
            }
            return MCharge.GetAccount(C_Charge_ID, as1, amount.Value);
        }

        /// <summary>
        /// Get Period
        /// </summary>
        /// <returns>C_Period_ID</returns>
        public int GetC_Period_ID()
        {
            if (_C_Period_ID == -1)
            {
                int index = _po.Get_ColumnIndex("C_Period_ID");
                if (index != -1)
                {
                    int? ii = (int?)_po.Get_Value(index);
                    if (ii != null)
                    {
                        _C_Period_ID = Utility.Util.GetValueOfInt(ii);//.intValue();
                    }
                }
                if (_C_Period_ID == -1)
                {
                    _C_Period_ID = 0;
                }
            }
            return _C_Period_ID;
        }

        /// <summary>
        /// Set C_Period_ID
        /// </summary>
        /// <param name="C_Period_ID"></param>
        public void SetC_Period_ID(int C_Period_ID)
        {
            _C_Period_ID = C_Period_ID;
        }

        /// <summary>
        /// Get (Journal) AcctSchema
        /// </summary>
        /// <returns>C_AcctSchema_ID</returns>
        public int GetC_AcctSchema_ID()
        {
            return _C_AcctSchema_ID;
        }

        /// <summary>
        /// Get Line ID
        /// </summary>
        /// <returns>id</returns>
        public int Get_ID()
        {
            return _po.Get_ID();
        }

        /// <summary>
        /// Get AD_Org_ID
        /// </summary>
        /// <returns>org</returns>
        public int GetAD_Org_ID()
        {
            return _po.GetAD_Org_ID();
        }

        /// <summary>
        /// Get Order AD_Org_ID
        /// </summary>
        /// <returns>order org if defined</returns>
        public int GetOrder_Org_ID()
        {
            int C_OrderLine_ID = GetC_OrderLine_ID();
            if (C_OrderLine_ID != 0)
            {
                String sql = "SELECT AD_Org_ID FROM C_OrderLine WHERE C_OrderLine_ID=@param1";
                int AD_Org_ID = DataBase.DB.GetSQLValue(null, sql, C_OrderLine_ID);
                if (AD_Org_ID > 0)
                {
                    return AD_Org_ID;
                }
            }
            return GetAD_Org_ID();
        }

        /// <summary>
        /// Product
        /// </summary>
        /// <returns>M_Product_ID</returns>
        public int GetM_Product_ID()
        {
            int index = _po.Get_ColumnIndex("M_Product_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);//.intValue();
                }
            }
            return 0;
        }

        /// <summary>
        /// Is this an Item Product (vs. not a Service, a charge)
        /// </summary>
        /// <returns>true if product</returns>
        public bool IsItem()
        {
            if (_isItem != null)
            {
                return Utility.Util.GetValueOfBool(_isItem);//.booleanValue();
            }

            _isItem = false;
            if (GetM_Product_ID() != 0)
            {
                MProduct product = MProduct.Get(_po.GetCtx(), GetM_Product_ID());
                if (product.Get_ID() == GetM_Product_ID() && product.IsItem())
                {
                    _isItem = true;
                }
            }
            return Utility.Util.GetValueOfBool(_isItem);//.booleanValue();
        }

        /// <summary>
        ///  ASI
        /// </summary>
        /// <returns>M_AttributeSetInstance_ID</returns>
        public int GetM_AttributeSetInstance_ID()
        {
            int index = _po.Get_ColumnIndex("M_AttributeSetInstance_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                    return Utility.Util.GetValueOfInt(ii);//.intValue();
            }
            return 0;
        }

        /// <summary>
        ///  Get Warehouse Locator (from)
        /// </summary>
        /// <returns>M_Locator_ID</returns>
        public int GetM_Locator_ID()
        {
            int index = _po.Get_ColumnIndex("M_Locator_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);//.intValue();
                }
            }
            return 0;
        }

        /// <summary>
        /// Get Warehouse Locator To
        /// </summary>
        /// <returns>M_Locator_ID</returns>
        public int GetM_LocatorTo_ID()
        {
            int index = _po.Get_ColumnIndex("M_LocatorTo_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);//.intValue();
                }
            }
            return 0;
        }

        /// <summary>
        /// Set Production BOM flag
        /// </summary>
        /// <param name="productionBOM">flag</param>
        public void SetProductionBOM(bool productionBOM)
        {
            _productionBOM = productionBOM;
        }

        /// <summary>
        /// Is this the BOM to be produced
        /// </summary>
        /// <returns>true if BOM</returns>
        public bool IsProductionBOM()
        {
            return _productionBOM;
        }

        /// <summary>
        /// Get Production Plan
        /// </summary>
        /// <returns>M_ProductionPlan_ID</returns>
        public int GetM_ProductionPlan_ID()
        {
            int index = _po.Get_ColumnIndex("M_ProductionPlan_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);//.intValue();
                }
            }
            return 0;
        }

        /// <summary>
        ///  Get Order Line Reference
        /// </summary>
        /// <returns>C_OrderLine_ID</returns>
        public int GetC_OrderLine_ID()
        {
            int index = _po.Get_ColumnIndex("C_OrderLine_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);//.intValue();
                }
            }
            return 0;
        }

        /// <summary>
        /// Get C_LocFrom_ID
        /// </summary>
        /// <returns>loc from</returns>
        public int GetC_LocFrom_ID()
        {
            return _C_LocFrom_ID;
        }

        /// <summary>
        /// Set C_LocFrom_ID
        /// </summary>
        /// <param name="C_LocFrom_ID">loc from</param>
        public void SetC_LocFrom_ID(int C_LocFrom_ID)
        {
            _C_LocFrom_ID = C_LocFrom_ID;
        }

        /// <summary>
        /// Get C_LocTo_ID
        /// </summary>
        /// <returns>loc to</returns>
        public int GetC_LocTo_ID()
        {
            return _C_LocTo_ID;
        }

        /// <summary>
        /// Set C_LocTo_ID
        /// </summary>
        /// <param name="C_LocTo_ID">loc to</param>
        public void SetC_LocTo_ID(int C_LocTo_ID)
        {
            _C_LocTo_ID = C_LocTo_ID;
        }

        /// <summary>
        /// Get Product Cost Info
        /// </summary>
        /// <returns>product cost</returns>
        public ProductCost GetProductCost()
        {
            if (_productCost == null)
            {
                _productCost = new ProductCost(_po.GetCtx(),
                    GetM_Product_ID(), GetM_AttributeSetInstance_ID(), _po.Get_TrxName());
            }
            return _productCost;
        }

        /// <summary>
        /// Get Total Product Costs
        /// </summary>
        /// <param name="as1"></param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="zeroCostsOK">zero/no costs are OK</param>
        /// <returns>costs</returns>
        public Decimal GetProductCosts(MAcctSchema as1, int AD_Org_ID, bool zeroCostsOK)
        {
            ProductCost pc = GetProductCost();
            int C_OrderLine_ID = GetC_OrderLine_ID();
            String costingMethod = null;
            Decimal? costs = pc.GetProductCosts(as1, AD_Org_ID, costingMethod, C_OrderLine_ID, zeroCostsOK);
            if (costs != null)
            {
                return costs.Value;
            }
            return Env.ZERO;
        }

        /// <summary>
        /// 	Get Product 
        /// </summary>
        /// <returns>product or null if no product</returns>
        public MProduct GetProduct()
        {
            if (_productCost == null)
            {
                _productCost = new ProductCost(_po.GetCtx(),
                    GetM_Product_ID(), GetM_AttributeSetInstance_ID(), _po.Get_TrxName());
            }
            if (_productCost != null)
            {
                return _productCost.GetProduct();
            }
            return null;
        }

        /// <summary>
        /// Get Revenue Recognition
        /// </summary>
        /// <returns>C_RevenueRecognition_ID or 0</returns>
        public int GetC_RevenueRecognition_ID()
        {
            MProduct product = GetProduct();
            if (product != null)
            {
                return product.GetC_RevenueRecognition_ID();
            }
            return 0;
        }

        /// <summary>
        /// Quantity UOM
        /// </summary>
        /// <returns>Transaction or Storage M_UOM_ID</returns>
        public int GetC_UOM_ID()
        {
            //	Trx UOM
            int index = _po.Get_ColumnIndex("C_UOM_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);//.intValue();
                }
            }
            //  Storage UOM
            MProduct product = GetProduct();
            if (product != null)
            {
                return product.GetC_UOM_ID();
            }
            //
            return 0;
        }

        /// <summary>
        /// Quantity
        /// </summary>
        /// <param name="qty">transaction Qty</param>
        /// <param name="isSOTrx">SL order trx (i.e. negative qty)</param>
        public void SetQty(Decimal? qty, bool isSOTrx)
        {
            if (qty == null)
            {
                _qty = Env.ZERO;
            }
            else if (isSOTrx)
            {
                _qty = Decimal.Negate(qty.Value);
            }
            else
            {
                _qty = qty;
            }
            GetProductCost().SetQty(qty);
        }

        /// <summary>
        /// Quantity
        /// </summary>
        /// <returns>transaction Qty</returns>
        public Decimal? GetQty()
        {
            return _qty;
        }


        /// <summary>
        /// Description
        /// </summary>
        /// <returns>doc line description</returns>
        public String GetDescription()
        {
            int index = _po.Get_ColumnIndex("Description");
            if (index != -1)
            {
                return (String)_po.Get_Value(index);
            }
            return null;
        }

        /// <summary>
        /// Line Tax
        /// </summary>
        /// <returns>C_Tax_ID</returns>
        public int GetC_Tax_ID()
        {
            int index = _po.Get_ColumnIndex("C_Tax_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);//.intValue();
                }
            }
            return 0;
        }

        /// <summary>
        /// Get Line Number
        /// </summary>
        /// <returns>line no</returns>
        public int GetLine()
        {
            int index = _po.Get_ColumnIndex("Line");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);//.intValue();
                }
            }
            return 0;
        }

        /// <summary>
        /// Get BPartner
        /// </summary>
        /// <returns>C_BPartner_ID</returns>
        public int GetC_BPartner_ID()
        {
            if (_C_BPartner_ID == -1)
            {
                int index = _po.Get_ColumnIndex("C_BPartner_ID");
                if (index != -1)
                {
                    int? ii = (int?)_po.Get_Value(index);
                    if (ii != null)
                    {
                        _C_BPartner_ID = Utility.Util.GetValueOfInt(ii);//.intValue();
                    }
                }
                if (_C_BPartner_ID <= 0)
                {
                    _C_BPartner_ID = _doc.GetC_BPartner_ID();
                }
            }
            return _C_BPartner_ID;
        }

        /// <summary>
        /// Set C_BPartner_ID
        /// </summary>
        /// <param name="C_BPartner_ID">id</param>
        protected void SetC_BPartner_ID(int C_BPartner_ID)
        {
            _C_BPartner_ID = C_BPartner_ID;
        }

        /// <summary>
        /// Get C_BPartner_Location_ID
        /// </summary>
        /// <returns>BPartner Location</returns>
        public int GetC_BPartner_Location_ID()
        {
            int index = _po.Get_ColumnIndex("C_BPartner_Location_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);//.intValue();
                }
            }
            return _doc.GetC_BPartner_Location_ID();
        }

        /// <summary>
        /// Get TrxOrg
        /// </summary>
        /// <returns>AD_OrgTrx_ID</returns>
        public int GetAD_OrgTrx_ID()
        {
            int index = _po.Get_ColumnIndex("AD_OrgTrx_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);//.intValue();
                }
            }
            return 0;
        }

        /// <summary>
        /// Get SalesRegion.
        /// - get Sales Region from BPartner
        /// </summary>
        /// <returns>C_SalesRegion_ID</returns>
        public int GetC_SalesRegion_ID()
        {
            if (_C_SalesRegion_ID == -1)	//	never tried
            {
                if (GetC_BPartner_Location_ID() != 0)
                //	&& m_acctSchema.isAcctSchemaElement(MAcctSchemaElement.ELEMENTTYPE_SalesRegion))
                {
                    String sql = "SELECT COALESCE(C_SalesRegion_ID,0) FROM C_BPartner_Location WHERE C_BPartner_Location_ID=@param1";
                    _C_SalesRegion_ID = DataBase.DB.GetSQLValue(null,
                        sql, GetC_BPartner_Location_ID());
                    log.Fine("C_SalesRegion_ID=" + _C_SalesRegion_ID + " (from BPL)");
                    if (_C_SalesRegion_ID == 0)
                    {
                        _C_SalesRegion_ID = -2;	//	don't try again
                    }
                }
                else
                {
                    _C_SalesRegion_ID = -2;		//	don't try again
                }
            }
            if (_C_SalesRegion_ID < 0)				//	invalid
            {
                return 0;
            }
            return _C_SalesRegion_ID;
        }

        /// <summary>
        /// Get Project
        /// </summary>
        /// <returns>C_Project_ID</returns>
        public int GetC_Project_ID()
        {
            int index = _po.Get_ColumnIndex("C_Project_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);//.intValue();
                }
            }
            return 0;
        }

        /// <summary>
        /// Get Campaign
        /// </summary>
        /// <returns>C_Campaign_ID</returns>
        public int GetC_Campaign_ID()
        {
            int index = _po.Get_ColumnIndex("C_Campaign_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);//.intValue();
                }
            }
            return 0;
        }

        /// <summary>
        /// Get Activity
        /// </summary>
        /// <returns>C_Activity_ID</returns>
        public int GetC_Activity_ID()
        {
            int index = _po.Get_ColumnIndex("C_Activity_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);//.intValue();
                }
            }
            return 0;
        }

        /// <summary>
        /// Get User 1
        /// </summary>
        /// <returns>user defined 1</returns>
        public int GetUser1_ID()
        {
            int index = _po.Get_ColumnIndex("User1_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);//.intValue();
                }
            }
            return 0;
        }

        /// <summary>
        /// Get User 2
        /// </summary>
        /// <returns>user defined 2</returns>
        public int GetUser2_ID()
        {
            int index = _po.Get_ColumnIndex("User2_ID");
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);//.intValue();
                }
            }
            return 0;
        }

        /// <summary>
        /// Get User Defined Column
        /// </summary>
        /// <param name="ColumnName">column name</param>
        /// <returns>user defined column value</returns>
        public int GetValue(String ColumnName)
        {
            int index = _po.Get_ColumnIndex(ColumnName);
            if (index != -1)
            {
                int? ii = (int?)_po.Get_Value(index);
                if (ii != null)
                {
                    return Utility.Util.GetValueOfInt(ii);//.intValue();
                }
            }
            return 0;
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>String</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("DocLine=[");
            sb.Append(_po.Get_ID());
            if (GetDescription() != null)
            {
                sb.Append(",").Append(GetDescription());
            }
            if (GetM_Product_ID() != 0)
            {
                sb.Append(",M_Product_ID=").Append(GetM_Product_ID());
            }
            sb.Append(",Qty=").Append(_qty)
                .Append(",Amt=").Append(GetAmtSource())
                .Append("]");
            return sb.ToString();
        }

        public int GetM_WorkOrderOperation_ID()
        {
            int index = _po.Get_ColumnIndex("VAMFG_M_WorkOrderOperation_ID");
            if (index != -1)
            {
                int ii = Util.GetValueOfInt(_po.Get_Value(index));
                
                    return ii;
                
            }
            return 0;
        }

        public String GetBasisType()
        {
            int index = _po.Get_ColumnIndex("BasisType");
            if (index != -1)
                return Util.GetValueOfString(_po.Get_Value(index));
            return null;
        }
    }
}
