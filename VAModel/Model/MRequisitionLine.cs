/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRequisitionLine
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     08-July-2009
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
using System.IO;
using VAdvantage.Logging;


namespace VAdvantage.Model
{
    public class MRequisitionLine : X_M_RequisitionLine
    {
        // Parent					
        private MRequisition _parent = null;
        //	PriceList				
        private int _M_PriceList_ID = 0;
        // Temp BPartner			
        private int _C_BPartner_ID = 0;

        /**
         * 	Standard Constructor
         *	@param Ctx context
         *	@param M_RequisitionLine_ID id
         *	@param trxName transaction
         */
        public MRequisitionLine(Ctx ctx, int M_RequisitionLine_ID, Trx trxName)
            : base(ctx, M_RequisitionLine_ID, trxName)
        {
            try
            {
                if (M_RequisitionLine_ID == 0)
                {
                    //	setM_Requisition_ID (0);
                    SetLine(0);	// @SQL=SELECT COALESCE(MAX(Line),0)+10 AS DefaultValue FROM M_RequisitionLine WHERE M_Requisition_ID=@M_Requisition_ID@
                    SetLineNetAmt(Env.ZERO);
                    SetPriceActual(Env.ZERO);
                    SetQty(Env.ONE);	// 1
                }
            }
            catch (Exception e)
            {
                log.Warning("MRequisitionLine--Standard Constructor" + e.Message);
            }

        }

        /**
         * 	Load Constructor
         *	@param Ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MRequisitionLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }
        /**
         * 	Load Constructor
         *	@param Ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MRequisitionLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	Parent Constructor
         *	@param req requisition
         */
        public MRequisitionLine(MRequisition req)
            : this(req.GetCtx(), 0, req.Get_TrxName())
        {
            try
            {
                SetClientOrg(req);
                SetM_Requisition_ID(req.GetM_Requisition_ID());
                _M_PriceList_ID = req.GetM_PriceList_ID();
                _parent = req;
            }
            catch
            {

            }
        }

        /**
         * @return Returns the c_BPartner_ID.
         */
        public int GetC_BPartner_ID()
        {
            return _C_BPartner_ID;
        }
        /**
         * @param partner_ID The c_BPartner_ID to set.
         */
        public void SetC_BPartner_ID(int partner_ID)
        {
            _C_BPartner_ID = partner_ID;
        }

        /**
         * 	Get Parent
         *	@return parent
         */
        public MRequisition GetParent()
        {
            try
            {
                if (_parent == null)
                {
                    _parent = new MRequisition(GetCtx(), GetM_Requisition_ID(), Get_TrxName());
                }
            }
            catch
            {

            }
            return _parent;
        }

        /**
         * 	Set Price
         */
        public void SetPrice()
        {
            try
            {
                if (GetC_Charge_ID() != 0)
                {
                    MCharge charge = MCharge.Get(GetCtx(), GetC_Charge_ID());
                    SetPriceActual(charge.GetChargeAmt());
                }
                if (GetM_Product_ID() == 0)
                    return;
                if (_M_PriceList_ID == 0)
                    _M_PriceList_ID = GetParent().GetM_PriceList_ID();
                if (_M_PriceList_ID == 0)
                {
                    log.Log(Level.SEVERE, "PriceList unknown!");
                    return;
                }
                SetPrice(_M_PriceList_ID);
            }
            catch (Exception e)
            {
                log.Severe("SetPrice" + e.Message);
            }
        }

        /**
         * 	Set Price for Product and PriceList
         * 	@param M_PriceList_ID price list
         */
        public void SetPrice(int M_PriceList_ID)
        {
            try
            {
                if (GetM_Product_ID() == 0)
                    return;
                //
                log.Fine("M_PriceList_ID=" + M_PriceList_ID);
                bool isSOTrx = false;
                MProductPricing pp = new MProductPricing(GetAD_Client_ID(), GetAD_Org_ID(),
                    GetM_Product_ID(), GetC_BPartner_ID(), GetQty(), isSOTrx);
                pp.SetM_PriceList_ID(M_PriceList_ID);

                // 
                // JID_0495_1: Set unit price on Requisition Line based on selected Pricelist on header
                if (Env.IsModuleInstalled("ED011_"))
                {
                    pp.SetC_UOM_ID(Util.GetValueOfInt(Get_Value("C_UOM_ID")));
                }

                //	pp.setPriceDate(getDateOrdered());
                //
                SetPriceActual(pp.GetPriceStd());
            }
            catch
            {
                MessageBox.Show("MRequisitionLine--SetPrice(int M_PriceList_ID)");
            }
        }

        /**
         * 	Calculate Line Net Amt
         */
        public void SetLineNetAmt()
        {
            try
            {
                Decimal lineNetAmt = Decimal.Multiply(GetQty(), GetPriceActual());
                base.SetLineNetAmt(lineNetAmt);
            }
            catch
            {

            }
        }

        /**
         * 	Set Product - Callout
         *	@param oldM_Product_ID old value
         *	@param newM_Product_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetM_Product_ID(String oldM_Product_ID, String newM_Product_ID, int windowNo)
        {
            try
            {
                if (newM_Product_ID == null || newM_Product_ID.Length == 0)
                    return;
                int M_Product_ID = int.Parse(newM_Product_ID);
                base.SetM_Product_ID(M_Product_ID);
                if (M_Product_ID == 0)
                {
                    SetM_AttributeSetInstance_ID(0);
                    return;
                }
                //	Set Attribute
                int M_AttributeSetInstance_ID = GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_AttributeSetInstance_ID");
                if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_Product_ID") == M_Product_ID
                    && M_AttributeSetInstance_ID != 0)
                {
                    SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
                }
                else
                {
                    SetM_AttributeSetInstance_ID(0);
                }

                int C_BPartner_ID = GetC_BPartner_ID();
                Decimal Qty = GetQty();
                bool isSOTrx = false;
                MProductPricing pp = new MProductPricing(GetAD_Client_ID(), GetAD_Org_ID(),
                    M_Product_ID, C_BPartner_ID, Qty, isSOTrx);
                //
                int M_PriceList_ID = GetCtx().GetContextAsInt(windowNo, "M_PriceList_ID");
                pp.SetM_PriceList_ID(M_PriceList_ID);
                int M_PriceList_Version_ID = GetCtx().GetContextAsInt(windowNo, "M_PriceList_Version_ID");
                pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
                //DateTime orderDate = new DateTime(GetCtx().GetContextAsTime(windowNo, "DateRequired"));
                DateTime orderDate = Convert.ToDateTime(GetCtx().GetContextAsTime(windowNo, "DateRequired"));
                pp.SetPriceDate(orderDate);
                //		
                SetPriceActual(pp.GetPriceStd());
                //p_changeVO.setContext(getContext(), windowNo, "EnforcePriceLimit", pp.isEnforcePriceLimit());	//	not used
                //p_changeVO.setContext(getContext(), windowNo, "DiscountSchema", pp.isDiscountSchema());
            }
            catch
            {

            }
        }

        /**
         * 	Set PriceActual - Callout
         *	@param oldPriceActual old value
         *	@param newPriceActual new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout 
        public void SetPriceActual(String oldPriceActual, String newPriceActual, int windowNo)
        {
            try
            {
                if (newPriceActual == null || newPriceActual.Length == 0)
                    return;
                Decimal PriceActual = Convert.ToDecimal(newPriceActual);
                base.SetPriceActual(PriceActual);
                SetAmt(windowNo, "PriceActual");
            }
            catch
            {

            }
        }

        /**
         * 	Set Qty - Callout
         *	@param oldQty old value
         *	@param newQty new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout 
        public void SetQty(String oldQty, String newQty, int windowNo)
        {

            if (newQty == null || newQty.Length == 0)
                return;
            Decimal Qty = Convert.ToDecimal(newQty);
            base.SetQty(Qty);
            SetAmt(windowNo, "Qty");


        }

        /**
         * 	Set Amount (Callout)
         *	@param windowNo window
         *	@param columnName changed column
         */
        private void SetAmt(int windowNo, String columnName)
        {
            try
            {
                Decimal qty = GetQty();
                //	Qty changed - recalc price
                if (columnName.Equals("Qty")
                    && "Y".Equals(GetCtx().GetContext(windowNo, "DiscountSchema")))
                {
                    int M_Product_ID = GetM_Product_ID();
                    int C_BPartner_ID = GetC_BPartner_ID();
                    bool isSOTrx = false;
                    MProductPricing pp = new MProductPricing(GetAD_Client_ID(), GetAD_Org_ID(),
                        M_Product_ID, C_BPartner_ID, qty, isSOTrx);
                    //
                    int M_PriceList_ID = GetCtx().GetContextAsInt(windowNo, "M_PriceList_ID");
                    pp.SetM_PriceList_ID(M_PriceList_ID);
                    int M_PriceList_Version_ID = GetCtx().GetContextAsInt(windowNo, "M_PriceList_Version_ID");
                    pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
                    //DateTime orderDate = new DateTime(getContext().getContextAsTime(windowNo, "DateInvoiced"));
                    DateTime orderDate = Convert.ToDateTime(GetCtx().GetContextAsTime(windowNo, "DateInvoiced"));
                    pp.SetPriceDate(orderDate);
                    //
                    SetPriceActual(pp.GetPriceStd());
                }

                int stdPrecision = GetCtx().GetStdPrecision();
                Decimal priceActual = GetPriceActual();

                //	get values
                log.Fine("Qty=" + qty + ", Price=" + priceActual + ", Precision=" + stdPrecision);

                //	Multiply
                Decimal lineNetAmt = Decimal.Multiply(qty, priceActual);
                if (Env.Scale(lineNetAmt) > stdPrecision)
                    lineNetAmt = Decimal.Round(lineNetAmt, stdPrecision, MidpointRounding.AwayFromZero);
                SetLineNetAmt(lineNetAmt);
                log.Info("LineNetAmt=" + lineNetAmt);
            }
            catch
            {

            }
        }

        /*
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            try
            {
                MProduct product = MProduct.Get(GetCtx(), GetM_Product_ID());
                if (GetLine() == 0)
                {
                    String sql = "SELECT COALESCE(MAX(Line),0)+10 FROM M_RequisitionLine WHERE M_Requisition_ID=@param1";
                    int ii = DataBase.DB.GetSQLValue(Get_TrxName(), sql, GetM_Requisition_ID());
                    SetLine(ii);
                }

                // change to set Converted Quantity in Movement quantity if there is differnce in UOM of Base Product and UOM Selected on line
                if (GetM_Product_ID() > 0 && Get_ColumnIndex("QtyEntered") > 0 && (newRecord || Is_ValueChanged("QtyEntered") || Is_ValueChanged("C_UOM_ID")))
                {
                    Decimal? qty = Util.GetValueOfDecimal(Get_Value("QtyEntered"));
                    if (product.GetC_UOM_ID() != Util.GetValueOfInt(Get_Value("C_UOM_ID")))
                    {
                        SetQty(MUOMConversion.ConvertProductFrom(GetCtx(), GetM_Product_ID(), Util.GetValueOfInt(Get_Value("C_UOM_ID")), Util.GetValueOfDecimal(Get_Value("QtyEntered"))));
                    }
                }

                // SI_0657_3 - precision of Qty should be according to the precision of UOM attached.
                if (newRecord || Is_ValueChanged("Qty"))
                {
                    if (product != null)
                    {
                        int precision = product.GetUOMPrecision();
                        SetQty(Decimal.Round(GetQty(), precision, MidpointRounding.AwayFromZero));
                    }
                }

                //	Product & ASI - Charge
                if (GetM_Product_ID() != 0 && GetC_Charge_ID() != 0)
                {
                    SetC_Charge_ID(0);
                }
                if (GetM_AttributeSetInstance_ID() != 0 && GetC_Charge_ID() != 0)
                {
                    SetM_AttributeSetInstance_ID(0);
                }
                //
                if (GetPriceActual().CompareTo(Env.ZERO) == 0)
                {
                    SetPrice();
                }
                SetLineNetAmt();
            }
            catch
            {

            }
            return true;
        }

        /**
         * 	After Save.
         * 	Update Total on Header
         *	@param newRecord if new record
         *	@param success save was success
         *	@return true if saved
         */
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
            {
                return success;
            }
            return UpdateHeader();
        }

        /**
         * 	After Delete
         *	@param success
         *	@return true/false
         */
        protected override bool AfterDelete(bool success)
        {
            if (!success)
            {
                return success;
            }
            return UpdateHeader();
        }

        /**
         * 	Update Header
         *	@return header updated
         */
        private bool UpdateHeader()
        {
            log.Fine("");
            String sql = "UPDATE M_Requisition r"
                + " SET TotalLines="
                    + "(SELECT COALESCE(SUM(LineNetAmt),0) FROM M_RequisitionLine rl "
                    + "WHERE r.M_Requisition_ID=rl.M_Requisition_ID) "
                + "WHERE M_Requisition_ID=" + GetM_Requisition_ID();
            int no = Convert.ToInt32(DataBase.DB.ExecuteQuery(sql, null, Get_TrxName()));
            if (no != 1)
            {
                log.Log(Level.SEVERE, "Header update #" + no);
            }
            _parent = null;
            return no == 1;
        }
    }
}
