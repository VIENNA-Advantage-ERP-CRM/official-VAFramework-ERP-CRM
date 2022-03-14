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
                //VIS0060 : After Reactivation, When we change Product, Attribute then system will check isqtyReserved, isqtyDelivered then not to save this records
                if (!newRecord && (Is_ValueChanged("M_Product_ID") || Is_ValueChanged("M_AttributeSetInstance_ID")))
                {
                    if (GetDTD001_DeliveredQty() != 0)
                    {
                        log.SaveError("", Msg.Translate(GetCtx(), "DTD001_DeliveredQty") + "=" + GetDTD001_DeliveredQty());
                        return false;
                    }
                    if (GetDTD001_ReservedQty() != 0)
                    {
                        log.SaveError("", Msg.Translate(GetCtx(), "DTD001_ReservedQty") + "=" + GetDTD001_ReservedQty());
                        return false;
                    }
                    if (Get_ColumnIndex("QtyReserved") >= 0 && GetQtyReserved() != 0)
                    {
                        log.SaveError("", Msg.Translate(GetCtx(), "DTD001_QtyReserved") + "=" + GetQtyReserved());
                        return false;
                    }
                }	//	Product Changed

                MRequisition Req = GetParent();
                MProduct product = MProduct.Get(GetCtx(), GetM_Product_ID());
                if (GetLine() == 0)
                {
                    String sql = "SELECT COALESCE(MAX(Line),0)+10 FROM M_RequisitionLine WHERE M_Requisition_ID=@param1";
                    int ii = DataBase.DB.GetSQLValue(Get_TrxName(), sql, GetM_Requisition_ID());
                    SetLine(ii);
                }
                // Check Product_ID or charge_id for before save

                if (GetM_Product_ID() == 0 && GetC_Charge_ID() == 0)
                {
                    log.SaveError("VIS_NOProductOrCharge", "");
                    return false;
                }

                // VIS0060: if product is there then requisition qty can not be less than delivered qty
                if (GetM_Product_ID() > 0)
                {
                    if (!newRecord && (Math.Abs(GetQty()) < Math.Abs(Decimal.Add(GetDTD001_DeliveredQty(), GetDTD001_ReservedQty()))) &&
                        (string.IsNullOrEmpty(GetDescription()) || !(!string.IsNullOrEmpty(GetDescription()) && GetDescription().Contains("Voided"))))
                    {
                        log.SaveError("", Msg.GetMsg(GetCtx(), "ReqQtyNotLessReserved"));
                        return false;
                    }
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

                //QtyEntered should not be zero
                // VIS0060: code commented to handle case of Void.
                //if (Util.GetValueOfDecimal(Get_Value("QtyEntered")) == 0)
                //{
                //    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "Quantity"));
                //    return false;
                //}

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

                // VIS0060: If Attribute is changes after ReActivate then Unreserve the Qty against the old Attribute and Reserver against the new Attribute.
                if (Get_ColumnIndex("QtyReserved") >= 0 && DocActionVariables.STATUS_INPROGRESS.Equals(Req.GetDocStatus()) && Is_ValueChanged("M_AttributeSetInstance_ID"))
                {
                    MStorage storage = null;
                    MStorage Swhstorage = null;

                    if (GetM_Product_ID() != 0 && product.IsStocked())
                    {
                        Decimal difference = Decimal.Negate(GetQtyReserved());
                        if (GetQtyReserved() > 0)
                        {
                            int loc_id = GetOrderLocator_ID();

                            storage = MStorage.Get(GetCtx(), loc_id, GetM_Product_ID(), GetM_AttributeSetInstance_ID(), Get_Trx());
                            if (storage == null)
                            {
                                MStorage.Add(GetCtx(), Req.GetM_Warehouse_ID(), loc_id, GetM_Product_ID(), Util.GetValueOfInt(Get_ValueOld("M_AttributeSetInstance_ID")),
                                    Util.GetValueOfInt(Get_ValueOld("M_AttributeSetInstance_ID")), (Decimal)0, (Decimal)0, (Decimal)0, difference, Get_Trx());
                            }
                            else
                            {
                                storage.SetDTD001_QtyReserved(Decimal.Add(storage.GetDTD001_QtyReserved(), difference));
                                storage.Save();
                            }

                            int Sourcewhloc_id = GetReserveLocator_ID();
                            Swhstorage = MStorage.Get(GetCtx(), Sourcewhloc_id, GetM_Product_ID(), GetM_AttributeSetInstance_ID(), Get_Trx());
                            if (Swhstorage == null)
                            {
                                MStorage.Add(GetCtx(), Req.GetDTD001_MWarehouseSource_ID(), Sourcewhloc_id, GetM_Product_ID(), Util.GetValueOfInt(Get_ValueOld("M_AttributeSetInstance_ID")),
                                    Util.GetValueOfInt(Get_ValueOld("M_AttributeSetInstance_ID")), (Decimal)0, (Decimal)0, (Decimal)0, 0, Get_Trx());
                                MStorage StrgResrvQty = null;
                                StrgResrvQty = MStorage.GetCreate(GetCtx(), Sourcewhloc_id, GetM_Product_ID(), Util.GetValueOfInt(Get_ValueOld("M_AttributeSetInstance_ID")), Get_Trx());
                                StrgResrvQty.SetDTD001_SourceReserve(Decimal.Add(StrgResrvQty.GetDTD001_SourceReserve(), difference));
                                StrgResrvQty.Save();
                            }
                            else
                            {
                                Swhstorage.SetDTD001_SourceReserve(Decimal.Add(Swhstorage.GetDTD001_SourceReserve(), difference));
                                Swhstorage.Save();
                            }

                            SetQtyReserved(Decimal.Add(GetQtyReserved(), difference));
                        }
                    }
                }
                //
                if (GetPriceActual().CompareTo(Env.ZERO) == 0)
                {
                    SetPrice();
                }

                // Set Line Net Amount if not calculated
                if (GetLineNetAmt().CompareTo(Env.ZERO) == 0)
                {
                    SetLineNetAmt();
                }

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

        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>true if it can be deleted</returns>
        protected override bool BeforeDelete()
        {
            // VIS0060: Check if some quantities are reserved or delivered against this Requisition Line 
            if (GetDTD001_DeliveredQty() != 0)
            {
                log.SaveError("", Msg.Translate(GetCtx(), "DTD001_DeliveredQty") + "=" + GetDTD001_DeliveredQty());
                return false;
            }
            if (GetDTD001_ReservedQty() != 0)
            {
                log.SaveError("", Msg.Translate(GetCtx(), "DTD001_ReservedQty()") + "=" + GetDTD001_ReservedQty());
                return false;
            }            
            if (Get_ColumnIndex("QtyReserved") >= 0 && GetQtyReserved() != 0)
            {
                //	For PO should be On Order
                log.SaveError("", Msg.Translate(GetCtx(), "QtyReserved") + "=" + GetQtyReserved());
                return false;
            }
            return true;
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
