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
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.IO;
using VAdvantage.Logging;


namespace VAdvantage.Model
{
    public class MRequisitionLine : X_VAM_RequisitionLine
    {
        // Parent					
        private MRequisition _parent = null;
        //	PriceList				
        private int _VAM_PriceList_ID = 0;
        // Temp BPartner			
        private int _VAB_BusinessPartner_ID = 0;

        /**
         * 	Standard Constructor
         *	@param Ctx context
         *	@param VAM_RequisitionLine_ID id
         *	@param trxName transaction
         */
        public MRequisitionLine(Ctx ctx, int VAM_RequisitionLine_ID, Trx trxName)
            : base(ctx, VAM_RequisitionLine_ID, trxName)
        {
            try
            {
                if (VAM_RequisitionLine_ID == 0)
                {
                    //	setVAM_Requisition_ID (0);
                    SetLine(0);	// @SQL=SELECT COALESCE(MAX(Line),0)+10 AS DefaultValue FROM VAM_RequisitionLine WHERE VAM_Requisition_ID=@VAM_Requisition_ID@
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
                SetVAM_Requisition_ID(req.GetVAM_Requisition_ID());
                _VAM_PriceList_ID = req.GetVAM_PriceList_ID();
                _parent = req;
            }
            catch
            {

            }
        }

        /**
         * @return Returns the VAB_BusinessPartner_ID.
         */
        public int GetVAB_BusinessPartner_ID()
        {
            return _VAB_BusinessPartner_ID;
        }
        /**
         * @param partner_ID The VAB_BusinessPartner_ID to set.
         */
        public void SetVAB_BusinessPartner_ID(int partner_ID)
        {
            _VAB_BusinessPartner_ID = partner_ID;
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
                    _parent = new MRequisition(GetCtx(), GetVAM_Requisition_ID(), Get_TrxName());
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
                if (GetVAB_Charge_ID() != 0)
                {
                    MVABCharge charge = MVABCharge.Get(GetCtx(), GetVAB_Charge_ID());
                    SetPriceActual(charge.GetChargeAmt());
                }
                if (GetVAM_Product_ID() == 0)
                    return;
                if (_VAM_PriceList_ID == 0)
                    _VAM_PriceList_ID = GetParent().GetVAM_PriceList_ID();
                if (_VAM_PriceList_ID == 0)
                {
                    log.Log(Level.SEVERE, "PriceList unknown!");
                    return;
                }
                SetPrice(_VAM_PriceList_ID);
            }
            catch (Exception e)
            {
                log.Severe("SetPrice" + e.Message);
            }
        }

        /**
         * 	Set Price for Product and PriceList
         * 	@param VAM_PriceList_ID price list
         */
        public void SetPrice(int VAM_PriceList_ID)
        {
            try
            {
                if (GetVAM_Product_ID() == 0)
                    return;
                //
                log.Fine("VAM_PriceList_ID=" + VAM_PriceList_ID);
                bool isSOTrx = false;
                MProductPricing pp = new MProductPricing(GetVAF_Client_ID(), GetVAF_Org_ID(),
                    GetVAM_Product_ID(), GetVAB_BusinessPartner_ID(), GetQty(), isSOTrx);
                pp.SetVAM_PriceList_ID(VAM_PriceList_ID);

                // 
                // JID_0495_1: Set unit price on Requisition Line based on selected Pricelist on header
                if (Env.IsModuleInstalled("ED011_"))
                {
                    pp.SetVAB_UOM_ID(Util.GetValueOfInt(Get_Value("VAB_UOM_ID")));
                }

                //	pp.setPriceDate(getDateOrdered());
                //
                SetPriceActual(pp.GetPriceStd());
            }
            catch
            {
               // MessageBox.Show("MRequisitionLine--SetPrice(int VAM_PriceList_ID)");
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
         *	@param oldVAM_Product_ID old value
         *	@param newVAM_Product_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetVAM_Product_ID(String oldVAM_Product_ID, String newVAM_Product_ID, int windowNo)
        {
            try
            {
                if (newVAM_Product_ID == null || newVAM_Product_ID.Length == 0)
                    return;
                int VAM_Product_ID = int.Parse(newVAM_Product_ID);
                base.SetVAM_Product_ID(VAM_Product_ID);
                if (VAM_Product_ID == 0)
                {
                    SetVAM_PFeature_SetInstance_ID(0);
                    return;
                }
                //	Set Attribute
                int VAM_PFeature_SetInstance_ID = GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAM_PFeature_SetInstance_ID");
                if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAM_Product_ID") == VAM_Product_ID
                    && VAM_PFeature_SetInstance_ID != 0)
                {
                    SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
                }
                else
                {
                    SetVAM_PFeature_SetInstance_ID(0);
                }

                int VAB_BusinessPartner_ID = GetVAB_BusinessPartner_ID();
                Decimal Qty = GetQty();
                bool isSOTrx = false;
                MProductPricing pp = new MProductPricing(GetVAF_Client_ID(), GetVAF_Org_ID(),
                    VAM_Product_ID, VAB_BusinessPartner_ID, Qty, isSOTrx);
                //
                int VAM_PriceList_ID = GetCtx().GetContextAsInt(windowNo, "VAM_PriceList_ID");
                pp.SetVAM_PriceList_ID(VAM_PriceList_ID);
                int VAM_PriceListVersion_ID = GetCtx().GetContextAsInt(windowNo, "VAM_PriceListVersion_ID");
                pp.SetVAM_PriceListVersion_ID(VAM_PriceListVersion_ID);
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
                    int VAM_Product_ID = GetVAM_Product_ID();
                    int VAB_BusinessPartner_ID = GetVAB_BusinessPartner_ID();
                    bool isSOTrx = false;
                    MProductPricing pp = new MProductPricing(GetVAF_Client_ID(), GetVAF_Org_ID(),
                        VAM_Product_ID, VAB_BusinessPartner_ID, qty, isSOTrx);
                    //
                    int VAM_PriceList_ID = GetCtx().GetContextAsInt(windowNo, "VAM_PriceList_ID");
                    pp.SetVAM_PriceList_ID(VAM_PriceList_ID);
                    int VAM_PriceListVersion_ID = GetCtx().GetContextAsInt(windowNo, "VAM_PriceListVersion_ID");
                    pp.SetVAM_PriceListVersion_ID(VAM_PriceListVersion_ID);
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
                // Check Product_ID or charge_ID before save
                if (GetVAM_Product_ID() == 0 && GetVAB_Charge_ID() == 0)
                {
                    log.SaveError("VIS_NOProductOrCharge", "");
                    return false;
                }

                // QtyEntered should not be zero
                if (Util.GetValueOfInt(Get_Value("QtyEntered")) == 0)
                {
                    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "Quantity"));
                    return false;
                }

                MProduct product = MProduct.Get(GetCtx(), GetVAM_Product_ID());
                if (GetLine() == 0)
                {
                    String sql = "SELECT COALESCE(MAX(Line),0)+10 FROM VAM_RequisitionLine WHERE VAM_Requisition_ID=@param1";
                    int ii = DataBase.DB.GetSQLValue(Get_TrxName(), sql, GetVAM_Requisition_ID());
                    SetLine(ii);
                }

                // change to set Converted Quantity in Movement quantity if there is differnce in UOM of Base Product and UOM Selected on line
                if (GetVAM_Product_ID() > 0 && Get_ColumnIndex("QtyEntered") > 0 && (newRecord || Is_ValueChanged("QtyEntered") || Is_ValueChanged("VAB_UOM_ID")))
                {
                    Decimal? qty = Util.GetValueOfDecimal(Get_Value("QtyEntered"));
                    if (product.GetVAB_UOM_ID() != Util.GetValueOfInt(Get_Value("VAB_UOM_ID")))
                    {
                        SetQty(MVABUOMConversion.ConvertProductFrom(GetCtx(), GetVAM_Product_ID(), Util.GetValueOfInt(Get_Value("VAB_UOM_ID")), Util.GetValueOfDecimal(Get_Value("QtyEntered"))));
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
                if (GetVAM_Product_ID() != 0 && GetVAB_Charge_ID() != 0)
                {
                    SetVAB_Charge_ID(0);
                }
                if (GetVAM_PFeature_SetInstance_ID() != 0 && GetVAB_Charge_ID() != 0)
                {
                    SetVAM_PFeature_SetInstance_ID(0);
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
            String sql = "UPDATE VAM_Requisition r"
                + " SET TotalLines="
                    + "(SELECT COALESCE(SUM(LineNetAmt),0) FROM VAM_RequisitionLine rl "
                    + "WHERE r.VAM_Requisition_ID=rl.VAM_Requisition_ID) "
                + "WHERE VAM_Requisition_ID=" + GetVAM_Requisition_ID();
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
