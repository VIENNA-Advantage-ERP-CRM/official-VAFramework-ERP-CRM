/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVAMInvInOutLine
 * Purpose        : For 2nd tab of the shipment window
 * Class Used     : X_VAM_Inv_InOutLine
 * Chronological    Development
 * Raghunandan     05-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVAMInvInOutLine : X_VAM_Inv_InOutLine
    {
        #region variables
        //	Product					
        private MProduct _product = null;
        // Warehouse			
        private int _VAM_Warehouse_ID = 0;
        //Parent				
        private MVAMInvInOut _parent = null;
        // Matched Invoices		
        private MVAMMatchInvoice[] _matchInv = null;
        // Matched Purchase Orders	
        private MVAMMatchPO[] _matchPO = null;
        //	Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAMInvInOutLine).FullName);
        public Decimal? OnHandQty = 0;
        private Decimal? containerQty = 0;
        #endregion

        /* 	Get Ship lines Of Order Line
        *	@param ctx context
        *	@param VAB_OrderLine_ID line
        *	@param where optional addition where clause
        *  @param trxName transaction
        *	@return array of receipt lines
        */
        public static MVAMInvInOutLine[] GetOfOrderLine(Ctx ctx,
            int VAB_OrderLine_ID, String where, Trx trxName)
        {
            List<MVAMInvInOutLine> list = new List<MVAMInvInOutLine>();


            //String sql = "SELECT * FROM VAM_Inv_InOutLine WHERE VAB_OrderLine_ID=" + VAB_OrderLine_ID;    //// Commented and added following query, by SUkhwinder on 16-Nov-2017, for taking into COMPLETED/CLOSED lines only.

            String sql = " SELECT VAM_Inv_InOutLine.*    " +
                         " FROM VAM_Inv_InOutLine VAM_Inv_InOutLine    " +
                         " INNER JOIN VAM_Inv_InOut VAM_Inv_InOut    " +
                         " ON VAM_Inv_InOut.VAM_Inv_InOut_ID = VAM_Inv_InOutLine.VAM_Inv_InOut_ID    " +
                         " WHERE VAM_Inv_InOut.Docstatus       IN ('CO','CL')    " +
                         " AND VAM_Inv_InOutLine.VAB_Orderline_Id = " + VAB_OrderLine_ID;

            if (where != null && where.Length > 0)
                sql += " AND " + where;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVAMInvInOutLine(ctx, dr, trxName));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }


            MVAMInvInOutLine[] retValue = new MVAMInvInOutLine[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
         * 	Get Ship lines Of Order Line
         *	@param ctx context
         *	@param VAB_OrderLine_ID line
         *  @param trxName transaction
         *	@return array of receipt lines2
         */
        public static MVAMInvInOutLine[] Get(Ctx ctx, int VAB_OrderLine_ID, Trx trxName)
        {
            List<MVAMInvInOutLine> list = new List<MVAMInvInOutLine>();
            String sql = "SELECT * FROM VAM_Inv_InOutLine WHERE VAB_OrderLine_ID=" + VAB_OrderLine_ID;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVAMInvInOutLine(ctx, dr, trxName));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }

            MVAMInvInOutLine[] retValue = new MVAMInvInOutLine[list.Count];
            retValue = list.ToArray();
            return retValue;
        }


        /*
     * 	Standard Constructor
     *	@param ctx context
     *	@param VAM_Inv_InOutLine_ID id
     *	@param trxName trx name
     */
        public MVAMInvInOutLine(Ctx ctx, int VAM_Inv_InOutLine_ID, Trx trxName)
            : base(ctx, VAM_Inv_InOutLine_ID, trxName)
        {
            try
            {

                if (VAM_Inv_InOutLine_ID == 0)
                {
                    //	setLine (0);
                    //	setVAM_Locator_ID (0);
                    //	setVAB_UOM_ID (0);
                    //	setVAM_Product_ID (0);
                    SetVAM_PFeature_SetInstance_ID(0);
                    //	setMovementQty (Env.ZERO);
                    SetConfirmedQty(Env.ZERO);
                    SetPickedQty(Env.ZERO);
                    SetScrappedQty(Env.ZERO);
                    SetTargetQty(Env.ZERO);
                    SetIsInvoiced(false);
                    SetIsDescription(false);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MVAMInvInOutLine--Standard Constructor",ex.Message);
            }
        }

        /**
        *  Parent Constructor
        *  @param inout parent
        */
        public MVAMInvInOutLine(MVAMInvInOut inout)
            : this(inout.GetCtx(), 0, inout.Get_TrxName())
        {

            SetClientOrg(inout);
            SetVAM_Inv_InOut_ID(inout.GetVAM_Inv_InOut_ID());
            SetVAM_Warehouse_ID(inout.GetVAM_Warehouse_ID());
            SetVAB_Project_ID(inout.GetVAB_Project_ID());
            _parent = inout;
        }

        /**
        *  Load Constructor
        *  @param ctx context
        *  @param dr result set record
        *  @param trxName transaction
        */
        public MVAMInvInOutLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /* 	Get Parent
        *	@return parent
        */
        public MVAMInvInOut GetParent()
        {
            if (_parent == null)
                _parent = new MVAMInvInOut(GetCtx(), GetVAM_Inv_InOut_ID(), Get_TrxName());
            return _parent;
        }

        /**
     * 	Set Order Line.
     * 	Does not set Quantity!
     *	@param oLine order line
     *	@param VAM_Locator_ID locator
     * 	@param Qty used only to find suitable locator
     */
        public void SetOrderLine(MVABOrderLine oLine, int VAM_Locator_ID, Decimal Qty)
        {
            SetVAB_OrderLine_ID(oLine.GetVAB_OrderLine_ID());
            SetLine(oLine.GetLine());
            SetVAB_UOM_ID(oLine.GetVAB_UOM_ID());
            MProduct product = oLine.GetProduct();
            if (product == null)
            {
                SetVAM_Product_ID(0);
                SetVAM_PFeature_SetInstance_ID(0);
                base.SetVAM_Locator_ID(0);
            }
            else
            {
                SetVAM_Product_ID(oLine.GetVAM_Product_ID());
                SetVAM_PFeature_SetInstance_ID(oLine.GetVAM_PFeature_SetInstance_ID());
                //
                if (product.IsItem())
                {
                    if (VAM_Locator_ID == 0)
                        SetVAM_Locator_ID(Qty);	//	requires warehouse, product, asi
                    else
                        SetVAM_Locator_ID(VAM_Locator_ID);
                }
                else
                {
                    base.SetVAM_Locator_ID(0);
                }
            }
            SetVAB_Charge_ID(oLine.GetVAB_Charge_ID());
            SetDescription(oLine.GetDescription());
            SetIsDescription(oLine.IsDescription());
            //
            SetVAB_Project_ID(oLine.GetVAB_Project_ID());
            SetVAB_ProjectStage_ID(oLine.GetVAB_ProjectStage_ID());
            SetVAB_ProjectJob_ID(oLine.GetVAB_ProjectJob_ID());
            SetVAB_BillingCode_ID(oLine.GetVAB_BillingCode_ID());
            SetVAB_Promotion_ID(oLine.GetVAB_Promotion_ID());
            SetVAF_OrgTrx_ID(oLine.GetVAF_OrgTrx_ID());
            SetUser1_ID(oLine.GetUser1_ID());
            SetUser2_ID(oLine.GetUser2_ID());
        }

        /*	Set Order Line - Callout
        *	@param oldVAB_OrderLine_ID old BP
        *	@param newVAB_OrderLine_ID new BP
        *	@param windowNo window no
        */
        //@UICallout
        public void SetVAB_OrderLine_ID(String oldVAB_OrderLine_ID, String newVAB_OrderLine_ID, int windowNo)
        {
            if (newVAB_OrderLine_ID == null || newVAB_OrderLine_ID.Length == 0)
                return;
            int VAB_OrderLine_ID = int.Parse(newVAB_OrderLine_ID);
            if (VAB_OrderLine_ID == 0)
                return;
            MVABOrderLine ol = new MVABOrderLine(GetCtx(), VAB_OrderLine_ID, null);
            if (ol.Get_ID() != 0)
            {
                SetVAB_OrderLine_ID(VAB_OrderLine_ID);
                Decimal MovementQty = Decimal.Subtract(ol.GetQtyOrdered(), ol.GetQtyDelivered());
                SetMovementQty(MovementQty);
                SetOrderLine(ol, 0, MovementQty);
                Decimal QtyEntered = MovementQty;
                if (ol.GetQtyEntered().CompareTo(ol.GetQtyOrdered()) != 0)
                {
                    //QtyEntered = QtyEntered.multiply(ol.getQtyEntered()).divide(ol.getQtyOrdered(), 12, Decimal.ROUND_HALF_UP);
                    QtyEntered = Decimal.Divide((Decimal.Multiply(QtyEntered, ol.GetQtyEntered())), ol.GetQtyOrdered());
                }
                SetQtyEntered(QtyEntered);

                if (ol.GetParent().IsReturnTrx())
                {
                    MVAMInvInOutLine ioLine = new MVAMInvInOutLine(GetCtx(), ol.GetOrig_InOutLine_ID(), null);
                    SetVAM_Locator_ID(ioLine.GetVAM_Locator_ID());
                }

            }
        }

        /**
         * 	Set Invoice Line.
         * 	Does not set Quantity!
         *	@param iLine invoice line
         *	@param VAM_Locator_ID locator
         *	@param Qty qty only fo find suitable locator
         */
        public void SetInvoiceLine(MVABInvoiceLine iLine, int VAM_Locator_ID, Decimal Qty)
        {
            SetVAB_OrderLine_ID(iLine.GetVAB_OrderLine_ID());
            SetLine(iLine.GetLine());
            SetVAB_UOM_ID(iLine.GetVAB_UOM_ID());
            int VAM_Product_ID = iLine.GetVAM_Product_ID();
            if (VAM_Product_ID == 0)
            {
                Set_ValueNoCheck("VAM_Product_ID", null);
                Set_ValueNoCheck("VAM_Locator_ID", null);
                Set_ValueNoCheck("VAM_PFeature_SetInstance_ID", null);
            }
            else
            {
                SetVAM_Product_ID(VAM_Product_ID);
                SetVAM_PFeature_SetInstance_ID(iLine.GetVAM_PFeature_SetInstance_ID());
                if (VAM_Locator_ID == 0)
                    SetVAM_Locator_ID(Qty);	//	requires warehouse, product, asi
                else
                    SetVAM_Locator_ID(VAM_Locator_ID);
            }
            SetVAB_Charge_ID(iLine.GetVAB_Charge_ID());
            SetDescription(iLine.GetDescription());
            SetIsDescription(iLine.IsDescription());
            //
            SetVAB_Project_ID(iLine.GetVAB_Project_ID());
            SetVAB_ProjectStage_ID(iLine.GetVAB_ProjectStage_ID());
            SetVAB_ProjectJob_ID(iLine.GetVAB_ProjectJob_ID());
            SetVAB_BillingCode_ID(iLine.GetVAB_BillingCode_ID());
            SetVAB_Promotion_ID(iLine.GetVAB_Promotion_ID());
            SetVAF_OrgTrx_ID(iLine.GetVAF_OrgTrx_ID());
            SetUser1_ID(iLine.GetUser1_ID());
            SetUser2_ID(iLine.GetUser2_ID());
        }

        /**
         * 	Get Warehouse
         *	@return Returns the VAM_Warehouse_ID.
         */
        public int GetVAM_Warehouse_ID()
        {
            if (_VAM_Warehouse_ID == 0)
                _VAM_Warehouse_ID = GetParent().GetVAM_Warehouse_ID();
            return _VAM_Warehouse_ID;
        }

        /**
        * 	Set Warehouse
        *	@param warehouse_ID The VAM_Warehouse_ID to set.
        */
        public void SetVAM_Warehouse_ID(int warehouse_ID)
        {
            _VAM_Warehouse_ID = warehouse_ID;
        }

        /*	Set VAM_Locator_ID
        *	@param VAM_Locator_ID id
        */
        public void SetVAM_Locator_ID(int VAM_Locator_ID)
        {
            if (VAM_Locator_ID < 0)
                throw new ArgumentException("VAM_Locator_ID is mandatory.");
            //	set to 0 explicitly to reset
            Set_Value("VAM_Locator_ID", VAM_Locator_ID);
        }

        /**
         * 	Set (default) Locator based on qty.
         * 	@param Qty quantity
         * 	Assumes Warehouse is set
         */
        public void SetVAM_Locator_ID(Decimal Qty)
        {
            //	Locator esatblished
            if (GetVAM_Locator_ID() != 0)
                return;
            //	No Product
            if (GetVAM_Product_ID() == 0)
            {
                Set_ValueNoCheck("VAM_Locator_ID", null);
                return;
            }

            //	Get existing Location
            int VAM_Locator_ID = MStorage.GetVAM_Locator_ID(GetVAM_Warehouse_ID(),
                    GetVAM_Product_ID(), GetVAM_PFeature_SetInstance_ID(),
                    Qty, Get_TrxName());
            //	Get default Location
            if (VAM_Locator_ID == 0)
            {
                MProduct product = MProduct.Get(GetCtx(), GetVAM_Product_ID());
                VAM_Locator_ID = MProductLocator.GetFirstVAM_Locator_ID(product, GetVAM_Warehouse_ID());
                if (VAM_Locator_ID == 0)
                {
                    MWarehouse wh = MWarehouse.Get(GetCtx(), GetVAM_Warehouse_ID());
                    VAM_Locator_ID = wh.GetDefaultVAM_Locator_ID();
                }
            }
            SetVAM_Locator_ID(VAM_Locator_ID);
        }

        /**
        * 	Set Movement/Movement Qty
        *	@param Qty Entered/Movement Qty
        */
        public void SetQty(Decimal Qty)
        {
            SetQtyEntered(Qty);
            SetMovementQty(GetQtyEntered());
        }

        /*	Set Qty Entered - enforce entered UOM 
       *	@param QtyEntered
       */
        public void SetQtyEntered(Decimal QtyEntered)
        {
            if (QtyEntered != 0 && GetVAB_UOM_ID() != 0)
            {
                int precision = MUOM.GetPrecision(GetCtx(), GetVAB_UOM_ID());
                //QtyEntered = QtyEntered.setScale(precision, Decimal.ROUND_HALF_UP);
                QtyEntered = Decimal.Round(QtyEntered, precision, MidpointRounding.AwayFromZero);
            }
            base.SetQtyEntered(QtyEntered);
        }

        /**
         * 	Set Movement Qty - enforce Product UOM 
         *	@param MovementQty
         */
        public void SetMovementQty(Decimal MovementQty)
        {
            MProduct product = GetProduct();
            if (MovementQty != 0 && product != null)
            {
                int precision = product.GetUOMPrecision();
                //MovementQty = MovementQty.setScale(precision, Decimal.ROUND_HALF_UP);
                MovementQty = Decimal.Round(MovementQty, precision, MidpointRounding.AwayFromZero);
            }
            base.SetMovementQty(MovementQty);
        }

        /**
        * 	Get Product
        *	@return product or null
        */
        public MProduct GetProduct()
        {
            if (_product == null && GetVAM_Product_ID() != 0)
                _product = MProduct.Get(GetCtx(), GetVAM_Product_ID());
            return _product;
        }

        /*	Set Product
        *	@param product product
        */
        public void SetProduct(MProduct product)
        {
            _product = product;
            if (_product != null)
            {
                SetVAM_Product_ID(_product.GetVAM_Product_ID());
                SetVAB_UOM_ID(_product.GetVAB_UOM_ID());
            }
            else
            {
                SetVAM_Product_ID(0);
                SetVAB_UOM_ID(0);
            }
            SetVAM_PFeature_SetInstance_ID(0);
        }

        /**
         * 	Set VAM_Product_ID
         *	@param VAM_Product_ID product
         *	@param setUOM also set UOM from product
         */
        public void SetVAM_Product_ID(int VAM_Product_ID, bool setUOM)
        {
            if (setUOM)
                SetProduct(MProduct.Get(GetCtx(), VAM_Product_ID));
            else
                base.SetVAM_Product_ID(VAM_Product_ID);
            SetVAM_PFeature_SetInstance_ID(0);
        }

        /**
         * 	Set Product and UOM
         *	@param VAM_Product_ID product
         *	@param VAB_UOM_ID uom
         */
        public void SetVAM_Product_ID(int VAM_Product_ID, int VAB_UOM_ID)
        {
            if (VAM_Product_ID != 0)
            {
                base.SetVAM_Product_ID(VAM_Product_ID);
            }
            base.SetVAB_UOM_ID(VAB_UOM_ID);
            SetVAM_PFeature_SetInstance_ID(0);
            _product = null;
        }

        /**
        * 	Set Product and UOM
        *	@param VAM_Product_ID product
        *	@param VAB_UOM_ID uom
        *	@param VAM_PFeature_SetInstance_ID AttributeSetInstance
        */
        public void SetVAM_Product_ID(int VAM_Product_ID, int VAB_UOM_ID, int VAM_PFeature_SetInstance_ID)
        {
            if (VAM_Product_ID != 0)
            {
                base.SetVAM_Product_ID(VAM_Product_ID);
            }
            base.SetVAB_UOM_ID(VAB_UOM_ID);
            SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            _product = null;
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
            if (newVAM_Product_ID == null || newVAM_Product_ID.Length == 0)
                return;
            int VAM_Product_ID = int.Parse(newVAM_Product_ID);
            if (VAM_Product_ID == 0)
            {
                SetVAM_PFeature_SetInstance_ID(0);
                return;
            }
            //
            base.SetVAM_Product_ID(VAM_Product_ID);
            SetVAB_Charge_ID(0);

            //	Set Attribute & Locator
            int VAM_Locator_ID = 0;
            if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAM_Product_ID") == VAM_Product_ID
                && GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAM_PFeature_SetInstance_ID") != 0)
            {
                SetVAM_PFeature_SetInstance_ID(GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAM_PFeature_SetInstance_ID"));
                //	Locator from Info Window - ASI
                VAM_Locator_ID = GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAM_Locator_ID");
                if (VAM_Locator_ID != 0)
                    SetVAM_Locator_ID(VAM_Locator_ID);
            }
            else
                SetVAM_PFeature_SetInstance_ID(0);
            //
            bool IsSOTrx = GetCtx().IsSOTrx(windowNo);
            if (IsSOTrx)
                return;

            //	PO - Set UOM/Locator/Qty
            MProduct product = GetProduct();
            SetVAB_UOM_ID(product.GetVAB_UOM_ID());
            Decimal QtyEntered = GetQtyEntered();
            SetMovementQty(QtyEntered);
            if (VAM_Locator_ID != 0)
            {
                ;		//	already set via ASI
            }
            else
            {
                int VAM_Warehouse_ID = GetCtx().GetContextAsInt(windowNo, "VAM_Warehouse_ID");
                VAM_Locator_ID = MProductLocator.GetFirstVAM_Locator_ID(product, VAM_Warehouse_ID);
                if (VAM_Locator_ID != 0)
                    SetVAM_Locator_ID(VAM_Locator_ID);
                else
                {
                    MWarehouse wh = MWarehouse.Get(GetCtx(), VAM_Warehouse_ID);
                    SetVAM_Locator_ID(wh.GetDefaultVAM_Locator_ID());
                }
            }
        }

        /**
         * 	Set Product - Callout
         *	@param oldVAM_PFeature_SetInstance_ID old value
         *	@param newVAM_PFeature_SetInstance_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetVAM_PFeature_SetInstance_ID(String oldVAM_PFeature_SetInstance_ID,
            String newVAM_PFeature_SetInstance_ID, int windowNo)
        {
            if (newVAM_PFeature_SetInstance_ID == null || newVAM_PFeature_SetInstance_ID.Length == 0)
                return;
            int VAM_PFeature_SetInstance_ID = int.Parse(newVAM_PFeature_SetInstance_ID);
            SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            if (VAM_PFeature_SetInstance_ID == 0)
                return;
            //
            int VAM_Product_ID = GetVAM_Product_ID();
            int VAM_Warehouse_ID = GetCtx().GetContextAsInt(windowNo, "VAM_Warehouse_ID");
            int VAM_Locator_ID = GetVAM_Locator_ID();
            log.Fine("VAM_Product_ID=" + VAM_Product_ID
                + ", M_ASI_ID=" + VAM_PFeature_SetInstance_ID
                + " - VAM_Warehouse_ID=" + VAM_Warehouse_ID
                + ", VAM_Locator_ID=" + VAM_Locator_ID);
            //	Check Selection
            int M_ASI_ID = Env.GetContext().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAM_PFeature_SetInstance_ID");
            if (M_ASI_ID == VAM_PFeature_SetInstance_ID)
            {
                int selectedVAM_Locator_ID = Env.GetContext().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAM_Locator_ID");
                if (selectedVAM_Locator_ID != 0)
                {
                    log.Fine("Selected VAM_Locator_ID=" + selectedVAM_Locator_ID);
                    SetVAM_Locator_ID(selectedVAM_Locator_ID);
                }
            }
        }

        /**
         * 	Set UOM - Callout
         *	@param oldVAB_UOM_ID old value
         *	@param newVAB_UOM_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        ///@UICallout 
        public void SetVAB_UOM_ID(String oldVAB_UOM_ID,
                String newVAB_UOM_ID, int windowNo)
        {
            if (newVAB_UOM_ID == null || newVAB_UOM_ID.Length == 0)
                return;
            int VAB_UOM_ID = int.Parse(newVAB_UOM_ID);
            if (VAB_UOM_ID == 0)
                return;
            //
            base.SetVAB_UOM_ID(VAB_UOM_ID);
            SetQty(windowNo, "VAB_UOM_ID");
        }

        /**
         * 	Set QtyEntered - Callout
         *	@param oldQtyEntered old value
         *	@param newQtyEntered new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetQtyEntered(String oldQtyEntered, String newQtyEntered, int windowNo)
        {
            if (newQtyEntered == null || newQtyEntered.Length == 0)
                return;
            Decimal QtyEntered = Convert.ToDecimal(newQtyEntered);
            base.SetQtyEntered(QtyEntered);
            SetQty(windowNo, "QtyEntered");
        }

        /**
         * 	Set MovementQty - Callout
         *	@param oldMovementQty old value
         *	@param newMovementQty new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetMovementQty(String oldMovementQty, String newMovementQty, int windowNo)
        {
            if (newMovementQty == null || newMovementQty.Length == 0)
                return;
            Decimal MovementQty = Convert.ToDecimal(newMovementQty);
            base.SetMovementQty(MovementQty);
            SetQty(windowNo, "MovementQty");
        }

        /**
         * 	Set Qty
         *	@param windowNo window
         *	@param columnName column
         */
        private void SetQty(int windowNo, String columnName)
        {
            int VAM_Product_ID = GetVAM_Product_ID();
            log.Log(Level.WARNING, "qty - init - VAM_Product_ID=" + VAM_Product_ID);
            Decimal MovementQty, QtyEntered;
            int VAB_UOM_To_ID = GetVAB_UOM_ID();

            //	No Product
            if (VAM_Product_ID == 0)
            {
                QtyEntered = GetQtyEntered();
                SetMovementQty(QtyEntered);
            }
            //	UOM Changed - convert from Entered -> Product
            else if (columnName.Equals("VAB_UOM_ID"))
            {
                QtyEntered = GetQtyEntered();
                //Decimal QtyEntered1 = QtyEntered.setScale(MUOM.GetPrecision(GetCtx(), VAB_UOM_To_ID), Decimal.ROUND_HALF_UP);
                Decimal QtyEntered1 = Decimal.Round(QtyEntered, MUOM.GetPrecision(GetCtx(), VAB_UOM_To_ID), MidpointRounding.AwayFromZero);
                if (QtyEntered.CompareTo(QtyEntered1) != 0)
                {
                    log.Fine("Corrected QtyEntered Scale UOM=" + VAB_UOM_To_ID
                    + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                    QtyEntered = QtyEntered1;
                    SetQtyEntered(QtyEntered);
                }
                MovementQty = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), VAM_Product_ID, VAB_UOM_To_ID, QtyEntered);
                if (MovementQty == null)
                    MovementQty = QtyEntered;
                bool conversion = QtyEntered.CompareTo(MovementQty) != 0;

                log.Fine("UOM=" + VAB_UOM_To_ID
                    + ", QtyEntered=" + QtyEntered
                    + " -> " + conversion
                    + " MovementQty=" + MovementQty);

                //p_changeVO.setContext(getCtx(), windowNo, "UOMConversion", conversion);
                SetMovementQty(MovementQty);
            }
            //	No UOM defined
            else if (VAB_UOM_To_ID == 0)
            {
                QtyEntered = GetQtyEntered();
                SetMovementQty(QtyEntered);
            }
            //	QtyEntered changed - calculate MovementQty
            else if (columnName.Equals("QtyEntered"))
            {
                QtyEntered = GetQtyEntered();
                Decimal QtyEntered1 = Decimal.Round(QtyEntered, MUOM.GetPrecision(GetCtx(), VAB_UOM_To_ID), MidpointRounding.AwayFromZero);
                if (QtyEntered.CompareTo(QtyEntered1) != 0)
                {
                    log.Fine("Corrected QtyEntered Scale UOM=" + VAB_UOM_To_ID
                        + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                    QtyEntered = QtyEntered1;
                    SetQtyEntered(QtyEntered);
                }
                MovementQty = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(),
                    VAM_Product_ID, VAB_UOM_To_ID, QtyEntered);
                if (MovementQty == null)
                    MovementQty = QtyEntered;
                bool conversion = QtyEntered.CompareTo(MovementQty) != 0;

                log.Fine("UOM=" + VAB_UOM_To_ID
                   + ", QtyEntered=" + QtyEntered
                    + " -> " + conversion
                    + " MovementQty=" + MovementQty);

                //p_changeVO.setContext(getCtx(), windowNo, "UOMConversion", conversion);
                SetMovementQty(MovementQty);
            }
            //	MovementQty changed - calculate QtyEntered (should not happen)
            else if (columnName.Equals("MovementQty"))
            {
                MovementQty = GetMovementQty();
                int precision = MProduct.Get(GetCtx(), VAM_Product_ID).GetUOMPrecision();
                //Decimal MovementQty1 = MovementQty.setScale(precision, Decimal.ROUND_HALF_UP);
                Decimal MovementQty1 = Decimal.Round(MovementQty, precision, MidpointRounding.AwayFromZero);// Env.Scale(MovementQty);
                if (MovementQty.CompareTo(MovementQty1) != 0)
                {
                    log.Fine("Corrected MovementQty "
                       + MovementQty + "->" + MovementQty1);
                    MovementQty = MovementQty1;
                    SetMovementQty(MovementQty);
                }
                QtyEntered = (Decimal)MUOMConversion.ConvertProductTo(GetCtx(), VAM_Product_ID, VAB_UOM_To_ID, MovementQty);
                if (QtyEntered == null)
                    QtyEntered = MovementQty;
                bool conversion = MovementQty.CompareTo(QtyEntered) != 0;
                log.Fine("UOM=" + VAB_UOM_To_ID
                    + ", MovementQty=" + MovementQty
                    + " -> " + conversion
                    + " QtyEntered=" + QtyEntered);

                //p_changeVO.setContext(getCtx(), windowNo, "UOMConversion", conversion);
                SetQtyEntered(QtyEntered);
            }

            // RMA : Check qty returned is more than qty shipped
            bool IsReturnTrx = GetParent().IsReturnTrx();
            if (VAM_Product_ID != 0 && IsReturnTrx)
            {
                int oLine_ID = GetVAB_OrderLine_ID();
                MVABOrderLine oLine = new MVABOrderLine(GetCtx(), oLine_ID, null);
                if (oLine.Get_ID() != 0)
                {
                    int orig_IOLine_ID = oLine.GetOrig_InOutLine_ID();
                    if (orig_IOLine_ID != 0)
                    {
                        MVAMInvInOutLine orig_IOLine = new MVAMInvInOutLine(GetCtx(), orig_IOLine_ID, null);
                        Decimal shippedQty = orig_IOLine.GetMovementQty();
                        MovementQty = GetMovementQty();
                        if (shippedQty.CompareTo(MovementQty) < 0)
                        {
                            if (GetCtx().IsSOTrx(windowNo))
                            {
                                //   p_changeVO.addError(Msg.getMsg(getCtx(), "QtyShippedLessThanQtyReturned", shippedQty));
                            }
                            else
                            {
                                // p_changeVO.addError(Msg.getMsg(getCtx(), "QtyReceivedLessThanQtyReturned", shippedQty));
                            }

                            SetMovementQty(shippedQty);
                            MovementQty = shippedQty;
                            QtyEntered = (Decimal)MUOMConversion.ConvertProductTo(GetCtx(), VAM_Product_ID,
                                    VAB_UOM_To_ID, MovementQty);
                            if (QtyEntered == null)
                                QtyEntered = MovementQty;
                            SetQtyEntered(QtyEntered);
                            log.Fine("QtyEntered : " + QtyEntered.ToString() +
                                   "MovementQty : " + MovementQty.ToString());
                        }
                    }
                }
            }

        }

        /**
        * 	Add to Description
        *	@param description text
        */
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
                SetDescription(description);
            else
                SetDescription(desc + " | " + description);
        }


        /**
         * 	Get VAB_Project_ID
         *	@return project
         */
        public int GetVAB_Project_ID()
        {
            int ii = base.GetVAB_Project_ID();
            if (ii == 0)
                ii = GetParent().GetVAB_Project_ID();
            return ii;
        }

        /**
         * 	Get VAB_BillingCode_ID
         *	@return Activity
         */
        public int GetVAB_BillingCode_ID()
        {
            int ii = base.GetVAB_BillingCode_ID();
            if (ii == 0)
                ii = GetParent().GetVAB_BillingCode_ID();
            return ii;
        }

        /**
         * 	Get VAB_Promotion_ID
         *	@return Campaign
         */
        public int GetVAB_Promotion_ID()
        {
            int ii = base.GetVAB_Promotion_ID();
            if (ii == 0)
                ii = GetParent().GetVAB_Promotion_ID();
            return ii;
        }

        /**
         * 	Get User2_ID
         *	@return User2
         */
        public int GetUser1_ID()
        {
            int ii = base.GetUser1_ID();
            if (ii == 0)
                ii = GetParent().GetUser1_ID();
            return ii;
        }

        /**
         * 	Get User2_ID
         *	@return User2
         */
        public int GetUser2_ID()
        {
            int ii = base.GetUser2_ID();
            if (ii == 0)
                ii = GetParent().GetUser2_ID();
            return ii;
        }

        /**
         * 	Get VAF_OrgTrx_ID
         *	@return trx org
         */
        public int GetVAF_OrgTrx_ID()
        {
            int ii = base.GetVAF_OrgTrx_ID();
            if (ii == 0)
                ii = GetParent().GetVAF_OrgTrx_ID();
            return ii;
        }

        /**
         * 	Get Match POs
         *	@return matched purchase orders
         */
        public MVAMMatchPO[] GetMatchPO()
        {
            if (_matchPO == null)
                _matchPO = MVAMMatchPO.Get(GetCtx(), GetVAM_Inv_InOutLine_ID(), Get_TrxName());
            return _matchPO;
        }

        /**
         * 	Get Match PO Difference
         *	@return not matched qty (positive not - negative over)
         */
        public Decimal GetMatchPODifference()
        {
            if (IsDescription())
                return Env.ZERO;
            Decimal retValue = GetMovementQty();
            MVAMMatchPO[] po = GetMatchPO();
            for (int i = 0; i < po.Length; i++)
            {
                MVAMMatchPO matchPO = po[i];
                retValue = Decimal.Subtract(retValue, matchPO.GetQty());
            }
            log.Finer("#" + retValue);
            return retValue;
        }

        /**
         * 	Is Match PO posted
         *	@return true if posed
         */
        public bool IsMatchPOPosted()
        {
            MVAMMatchPO[] po = GetMatchPO();
            for (int i = 0; i < po.Length; i++)
            {
                MVAMMatchPO matchPO = po[i];
                if (!matchPO.IsPosted())
                    return false;
            }
            return true;
        }

        /**
         * 	Get Match Inv
         *	@return matched invoices
         */
        public MVAMMatchInvoice[] GetMatchInv()
        {
            if (_matchInv == null)
                _matchInv = MVAMMatchInvoice.Get(GetCtx(), GetVAM_Inv_InOutLine_ID(), Get_TrxName());
            return _matchInv;
        }

        /**
         * 	Get Match Inv Difference
         *	@return not matched qty (positive not - negative over)
         */
        public Decimal GetMatchInvDifference()
        {
            if (IsDescription())
                return Env.ZERO;
            Decimal retValue = GetMovementQty();
            MVAMMatchInvoice[] inv = GetMatchInv();
            for (int i = 0; i < inv.Length; i++)
            {
                MVAMMatchInvoice matchInv = inv[i];
                //retValue = retValue.subtract(matchInv.getQty());
                retValue = Decimal.Subtract(retValue, matchInv.GetQty());
            }
            log.Finer("#" + retValue);
            return retValue;
        }

        /**
         * 	Is Match Inv posted
         *	@return true if posed
         */
        public bool IsMatchInvPosted()
        {
            MVAMMatchInvoice[] inv = GetMatchInv();
            for (int i = 0; i < inv.Length; i++)
            {
                MVAMMatchInvoice matchInv = inv[i];
                if (!matchInv.IsPosted())
                    return false;
            }
            return true;
        }

        /****
         * 	Before Save
         *	@param newRecord new
         *	@return save
         */
        protected override bool BeforeSave(bool newRecord)
        {
            // chck pallet Functionality applicable or not
            bool isContainrApplicable = MTransaction.ProductContainerApplicable(GetCtx());

            Decimal? movementQty, QtyEntered, VA024_ProvisionPrice = 0;
            QtyEntered = GetQtyEntered();
            movementQty = GetMovementQty();
            log.Fine("");

            // JID_0899: If user do not select Product or Charge on Ship/Receipt Line, it will displayed the message "Please select the Product or charge
            if (GetVAB_Charge_ID() == 0 && GetVAM_Product_ID() == 0)
            {
                log.SaveError("VIS_NOProductOrCharge", "");
                return false;
            }
            //	Get Line No
            if (GetLine() == 0)
            {
                String sql = "SELECT COALESCE(MAX(Line),0)+10 FROM VAM_Inv_InOutLine WHERE VAM_Inv_InOut_ID=" + GetVAM_Inv_InOut_ID();
                int ii = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql, null, null));
                SetLine(ii);
            }
            // Costing column updation
            if (newRecord)
            {
                // when we create new record then need to set as false
                if (Get_ColumnIndex("IsCostCalculated") >= 0 && Get_ColumnIndex("ReversalDoc_ID") >= 0 && GetReversalDoc_ID() == 0)
                    SetIsCostCalculated(false);
                if (Get_ColumnIndex("IsReversedCostCalculated") >= 0)
                    SetIsReversedCostCalculated(false);
                if (Get_ColumnIndex("IsCostImmediate") >= 0)
                    SetIsCostImmediate(false);
            }
            //	UOM
            if (GetVAB_UOM_ID() == 0)
                SetVAB_UOM_ID(GetCtx().GetContextAsInt("#VAB_UOM_ID"));
            if (GetVAB_UOM_ID() == 0)
            {
                int VAB_UOM_ID = MUOM.GetDefault_UOM_ID(GetCtx());
                if (VAB_UOM_ID > 0)
                    SetVAB_UOM_ID(VAB_UOM_ID);
            }
            int gp = MUOM.GetPrecision(GetCtx(), GetVAB_UOM_ID());
            Decimal? QtyEntered1 = Decimal.Round(QtyEntered.Value, gp, MidpointRounding.AwayFromZero);
            if (QtyEntered != QtyEntered1)
            {
                this.log.Fine("Corrected QtyEntered Scale UOM=" + GetVAB_UOM_ID()
                    + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                QtyEntered = QtyEntered1;
                SetQtyEntered(QtyEntered);
            }
            Decimal? pc = MUOMConversion.ConvertProductFrom(GetCtx(), GetVAM_Product_ID(), GetVAB_UOM_ID(), QtyEntered);

            Decimal newMoveQty = movementQty ?? 0; //Arpit

            //Checking for conversion of UOM 
            MVAMInvInOut inO = new MVAMInvInOut(GetCtx(), GetVAM_Inv_InOut_ID(), Get_TrxName());
            MVABDocTypes dt = new MVABDocTypes(GetCtx(), inO.GetVAB_DocTypes_ID(), Get_TrxName());
            MProduct _Product = null;

            // Check if Product_ID is non zero then only create the object
            if (GetVAM_Product_ID() > 0)
            {
                _Product = new MProduct(GetCtx(), GetVAM_Product_ID(), Get_TrxName());
            }

            if (_Product != null && GetVAB_UOM_ID() != _Product.GetVAB_UOM_ID())
            {
                decimal? differenceQty = Util.GetValueOfDecimal(GetCtx().GetContext("DifferenceQty_"));
                if (differenceQty > 0 && !newRecord && !dt.IsSplitWhenDifference())
                {
                    //converted qty if found any difference then the Movement qty reduces by the difference amount
                    pc = MUOMConversion.ConvertProductFrom(GetCtx(), GetVAM_Product_ID(), GetVAB_UOM_ID(), QtyEntered - differenceQty);
                }
            }
            movementQty = pc;
            bool conversion = false;
            if (movementQty != null)
            {
                conversion = QtyEntered != movementQty;
            }
            if (movementQty == null)
            {
                conversion = false;
                movementQty = 1;
                //Arpit to set movement qty ,handled both case when movement qty changes or not  -::: when we do not having UOM conversion rate 
                //With Discussion with Mukesh Sir regarding this point
                SetMovementQty(newMoveQty);
                //SetMovementQty(movementQty * QtyEntered1);
            }
            else
            {
                SetMovementQty(movementQty);
            }

            String qry1 = "";

            // for Service Type Product or Charge set value in Locator field
            if (((_Product != null && _Product.GetProductType() != MProduct.PRODUCTTYPE_Item) || GetVAB_Charge_ID() > 0) && GetVAM_Locator_ID() == 0)
            {
                qry1 = "SELECT VAM_Locator_ID FROM VAM_Locator WHERE VAM_Warehouse_ID=" + inO.GetVAM_Warehouse_ID() + " AND IsDefault = 'Y'";
                int il = Util.GetValueOfInt(DB.ExecuteScalar(qry1, null, Get_TrxName()));
                if (il == 0)
                {
                    qry1 = "SELECT MAX(VAM_Locator_ID) FROM VAM_Locator WHERE VAM_Warehouse_ID=" + inO.GetVAM_Warehouse_ID() + " AND IsActive = 'Y'";
                    il = Util.GetValueOfInt(DB.ExecuteScalar(qry1, null, Get_TrxName()));
                }
                SetVAM_Locator_ID(il);
            }

            // check record is reversed or not
            //bool IsReveresed = false;
            //if (inO.GetDescription() != null)
            //{
            //    if (inO.GetDescription().Contains("RC"))
            //    {
            //        IsReveresed = true;
            //    }
            //    else if (inO.GetDescription().Contains("->"))
            //    {
            //        IsReveresed = true;
            //    }
            //}

            // dont verify qty during completion
            // on Ship/Receipt, do not check qty in warehouse for Lines of Charge.
            if ((!inO.IsProcessing() || newRecord) && _Product != null && _Product.IsStocked())
            {
                int VAM_Warehouse_ID = 0; MWarehouse wh = null;
                StringBuilder qry = new StringBuilder();
                qry.Append("select VAM_Warehouse_id from VAM_Locator where VAM_Locator_id=" + GetVAM_Locator_ID());
                VAM_Warehouse_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry.ToString()));

                wh = MWarehouse.Get(GetCtx(), VAM_Warehouse_ID);
                qry.Clear();
                qry.Append("SELECT QtyOnHand FROM VAM_Storage where VAM_Locator_id=" + GetVAM_Locator_ID() + " and VAM_Product_id=" + GetVAM_Product_ID());
                if (GetVAM_PFeature_SetInstance_ID() != 0)
                {
                    qry.Append(" AND VAM_PFeature_SetInstance_ID=" + GetVAM_PFeature_SetInstance_ID());
                }
                else
                {
                    //SI_0642 :  when we have to check qtyonhand where ASI is null or 0
                    qry.Append(" AND NVL(VAM_PFeature_SetInstance_ID, 0) = 0");
                }
                OnHandQty = Convert.ToDecimal(DB.ExecuteScalar(qry.ToString(), null, Get_Trx()));
                // when record is in completed & closed stage - then no need to check qty availablity in warehouse
                if (wh.IsDisallowNegativeInv() == true &&
                    (!(inO.GetDocStatus() == "CO" || inO.GetDocStatus() == "CL" || inO.GetDocStatus() == "RE" || inO.GetDocStatus() == "VO")))
                {
                    // pick container current qty from transaction based on locator / product / ASI / Container / Movement Date 
                    if (isContainrApplicable && Get_ColumnIndex("VAM_ProductContainer_ID") >= 0)
                    {
                        qry.Clear();
                        qry.Append(@"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.VAM_Product_ID, t.VAM_PFeature_SetInstance_ID ORDER BY t.MovementDate DESC, t.VAM_Inv_Trx_ID DESC) AS CurrentQty FROM VAM_Inv_Trx t 
                            INNER JOIN VAM_Locator l ON t.VAM_Locator_ID = l.VAM_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(inO.GetMovementDate(), true) +
                                    " AND t.VAF_Client_ID = " + GetVAF_Client_ID() + " AND t.VAM_Locator_ID = " + GetVAM_Locator_ID() +
                                    " AND t.VAM_Product_ID = " + GetVAM_Product_ID() + " AND NVL(t.VAM_PFeature_SetInstance_ID,0) = " + GetVAM_PFeature_SetInstance_ID() +
                                    " AND NVL(t.VAM_ProductContainer_ID, 0) = " + GetVAM_ProductContainer_ID());
                        containerQty = Util.GetValueOfDecimal(DB.ExecuteScalar(qry.ToString(), null, Get_Trx()));  // dont use Transaction here - otherwise impact goes wrong on completion
                    }

                    // Material receipt && Customer return
                    if ((!inO.IsSOTrx() && !inO.IsReturnTrx()) || (inO.IsSOTrx() && inO.IsReturnTrx()))
                    {
                        if ((OnHandQty + GetMovementQty()) < 0)
                        {
                            log.SaveError("Info", _Product.GetName() + " , " + Msg.GetMsg(GetCtx(), "VIS_InsufficientQty") + OnHandQty);
                            return false;
                        }
                        if (isContainrApplicable && Get_ColumnIndex("VAM_ProductContainer_ID") >= 0 && (containerQty + GetMovementQty()) < 0)
                        {
                            log.SaveError("Info", _Product.GetName() + " , " + Msg.GetMsg(GetCtx(), "VIS_InsufficientQtyContainer") + containerQty);
                            return false;
                        }
                    }
                    // Shipment && Vendor return
                    else if ((inO.IsSOTrx() && !inO.IsReturnTrx()) || (!inO.IsSOTrx() && inO.IsReturnTrx()))
                    {
                        if ((OnHandQty - GetMovementQty()) < 0)
                        {
                            log.SaveError("Info", _Product.GetName() + ", " + Msg.GetMsg(GetCtx(), "VIS_InsufficientQty") + OnHandQty);
                            return false;
                        }
                        if (isContainrApplicable && Get_ColumnIndex("VAM_ProductContainer_ID") >= 0 && (containerQty - GetMovementQty()) < 0)
                        {
                            log.SaveError("Info", _Product.GetName() + " , " + Msg.GetMsg(GetCtx(), "VIS_InsufficientQtyContainer") + containerQty);
                            return false;
                        }
                    }
                }
            }

            //during creating a line of VAM_Inv_InOut, system check qty on line cannot be greater than Ordered qty (Ordered - delivered)
            if (!inO.IsReversal())
            {
                if (GetVAB_OrderLine_ID() > 0 && (newRecord || Is_ValueChanged("IsActive") || Is_ValueChanged("MovementQty")))
                {
                    qry1 = "select  QtyOrdered - QtyDelivered from VAB_OrderLine where VAB_OrderLine_ID =" + GetVAB_OrderLine_ID();
                    Decimal? _result = Util.GetValueOfDecimal(DB.ExecuteScalar(qry1, null, Get_Trx()));
                    decimal QtyNotDelivered = 0;
                    if (newRecord)
                        QtyNotDelivered = Util.GetValueOfDecimal(DB.ExecuteScalar(@"SELECT SUM(MovementQty) FROM VAM_Inv_InOut i INNER JOIN VAM_Inv_InOutLine il ON i.VAM_Inv_InOut_ID = il.VAM_Inv_InOut_ID
                            WHERE il.VAB_OrderLine_ID = " + GetVAB_OrderLine_ID() + @" AND il.Isactive = 'Y' AND i.docstatus NOT IN ('RE' , 'VO' , 'CL' , 'CO')", null, Get_Trx()));
                    else
                        QtyNotDelivered = Util.GetValueOfDecimal(DB.ExecuteScalar(@"SELECT SUM(MovementQty) FROM VAM_Inv_InOut i INNER JOIN VAM_Inv_InOutLine il ON i.VAM_Inv_InOut_ID = il.VAM_Inv_InOut_ID
                            WHERE il.VAB_OrderLine_ID = " + GetVAB_OrderLine_ID() + @" AND il.Isactive = 'Y' AND i.docstatus NOT IN ('RE' , 'VO' , 'CL' , 'CO') AND il.VAM_Inv_InOutLine_ID <> " + GetVAM_Inv_InOutLine_ID(), null, Get_Trx()));
                    if (GetMovementQty() > (_result - QtyNotDelivered))
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "QtyCanNotbeGreater"));
                        return false;
                    }
                }
            }

            //	Qty Precision
            if (newRecord || Is_ValueChanged("QtyEntered"))
                SetQtyEntered(GetQtyEntered());
            if (newRecord || Is_ValueChanged("MovementQty"))
                SetMovementQty(GetMovementQty());
            //	Order Line
            if (GetVAB_OrderLine_ID() == 0)
            {
                if (GetParent().IsSOTrx())
                {
                    log.SaveError("FillMandatory", Msg.Translate(GetCtx(), "VAB_Order_ID"));
                    return false;
                }
            }

            // get current cost from Orignal Document on new record and when product changed
            // when record is  return transaction then price should pick from its Original record
            // now, not to set orignal record cost, system will update current cost
            //decimal currentcostprice = 0;
            //if (inO.IsReturnTrx() && GetVAB_OrderLine_ID() > 0)
            //{
            //    MVABOrderLine RMALine = new MVABOrderLine(GetCtx(), GetVAB_OrderLine_ID(), Get_Trx());
            //    if (RMALine != null)
            //    {
            //        MVAMInvInOutLine origInOutLine = new MVAMInvInOutLine(GetCtx(), RMALine.GetOrig_InOutLine_ID(), Get_Trx());
            //        if (origInOutLine != null)
            //        {
            //            currentcostprice = origInOutLine.GetCurrentCostPrice();
            //        }
            //    }
            //    SetCurrentCostPrice(currentcostprice);
            //}


            // By Amit for Obsolete Inventory - 25-May-2016
            if (Env.IsModuleInstalled("VA024_"))
            {
                //MVAMInvInOut inout = new MVAMInvInOut(GetCtx(), GetVAM_Inv_InOut_ID(), Get_Trx());
                //shipment and Return to vendor
                if ((!inO.IsSOTrx() && inO.IsReturnTrx()) || (inO.IsSOTrx() && !inO.IsReturnTrx()))
                {
                    qry1 = @"SELECT  SUM(o.VA024_UnitPrice)  FROM VA024_t_ObsoleteInventory o 
                                  WHERE o.IsActive = 'Y' AND  o.VAM_Product_ID = " + GetVAM_Product_ID() + @" and 
                                  ( o.VAM_PFeature_SetInstance_ID = " + GetVAM_PFeature_SetInstance_ID() + @" OR o.VAM_PFeature_SetInstance_ID IS NULL )" +
                              " AND o.VAF_Org_ID = " + GetVAF_Org_ID();
                    VA024_ProvisionPrice = Util.GetValueOfDecimal(DB.ExecuteScalar(qry1, null, Get_Trx()));
                    SetVA024_UnitPrice(Util.GetValueOfDecimal(VA024_ProvisionPrice * GetMovementQty()));

                    // is used to get cost of binded cost method / costing level of primary accounting schema
                    Decimal cost = MVAMProductCost.GetproductCosts(inO.GetVAF_Client_ID(), inO.GetVAF_Org_ID(), GetVAM_Product_ID(),
                         GetVAM_PFeature_SetInstance_ID(), Get_Trx(), inO.GetVAM_Warehouse_ID());
                    SetVA024_CostPrice((cost - VA024_ProvisionPrice) * GetMovementQty());
                }
            }

            //Mandatory
            if (Env.IsModuleInstalled("DTD001_"))
            {
                if (GetVAM_PFeature_SetInstance_ID() != 0)	//	Set to from
                    SetVAM_PFeature_SetInstance_ID(GetVAM_PFeature_SetInstance_ID());
                else
                {
                    MProduct product = GetProduct();
                    if (product != null
                        && product.GetVAM_PFeature_Set_ID() != 0)
                    {
                        //MVAMPFeatureSet mas = MVAMPFeatureSet.Get(GetCtx(), product.GetVAM_PFeature_Set_ID());
                        //if (mas.IsInstanceAttribute() 
                        //    && (mas.IsMandatory() || mas.IsMandatoryAlways()))
                        //{
                        //    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "VAM_PFeature_SetInstanceTo_ID"));
                        //    return false;
                        //}

                        // Code Addeded by Bharat as Discussed with Mukesh Sir
                        if (String.IsNullOrEmpty(GetDTD001_Attribute()))
                        {
                            return true;
                        }
                        else
                        {
                            //MVAMInvInOut inout = new MVAMInvInOut(Env.GetCtx(), GetVAM_Inv_InOut_ID(), Get_Trx());
                            if (inO.GetDescription() != "RC" && Util.GetValueOfBool(IsDTD001_IsAttributeNo()) == false)
                            {
                                if (GetDTD001_Attribute() == "" || GetDTD001_Attribute() == null)
                                {
                                    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "DTD001_Attribute"));

                                    return false;
                                }
                                //Check No Of Attributes Are Equal To Quantity Or Less Than
                                int Count = CountAttributes(GetDTD001_Attribute());
                                if (Count != GetQtyEntered())
                                {
                                    if (Count > GetQtyEntered())
                                    {
                                        log.SaveError("DTD001_MaterialAtrbteGreater", "");
                                        return false;
                                    }
                                    else
                                    {
                                        log.SaveError("DTD001_MaterialAtrbteless", "");
                                        return false;
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        if (product != null)
                        {
                            if (product.GetVAM_PFeature_Set_ID() == 0 && (GetDTD001_Attribute() == "" || GetDTD001_Attribute() == null))
                                return true;
                            else
                            {
                                //log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "DTD001_AttributeNumber"));
                                //ShowMessage.Info("a", true, "Product is not of Attribute Type", null); 
                                log.SaveError("Product is not of Attribute Type", Msg.GetElement(GetCtx(), "DTD001_Attribute"));
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        private static int CountAttributes(string Attributes)
        {
            int n = 0;
            foreach (var c in Attributes)
            {
                if ((c == '\n') || (c == '\r')) n++;
            }
            return n + 1;
        }

        /**
         * 	Before Delete
         *	@return true if drafted
         */
        protected override bool BeforeDelete()
        {
            if (GetParent().GetDocStatus().Equals(MVAMInvInOut.DOCSTATUS_Drafted))
                return true;
            log.SaveError("Error", Msg.GetMsg(GetCtx(), "CannotDelete"));
            return false;
        }

        /**
         * 	String Representation
         *	@return info
         */
        public String ToString()
        {
            StringBuilder sb = new StringBuilder("MVAMInvInOutLine[").Append(Get_ID())
                .Append(",VAM_Product_ID=").Append(GetVAM_Product_ID())
                .Append(",QtyEntered=").Append(GetQtyEntered())
                .Append(",MovementQty=").Append(GetMovementQty())
                .Append(",VAM_PFeature_SetInstance_ID=").Append(GetVAM_PFeature_SetInstance_ID())
                .Append("]");
            return sb.ToString();
        }

        /**
         * 	Get Base value for Cost Distribution
         *	@param CostDistribution cost Distribution
         *	@return base number
         */
        public Decimal GetBase(String CostDistribution)
        {
            if (MVABLCost.LANDEDCOSTDISTRIBUTION_Costs.Equals(CostDistribution))
            {
                //	TODO Costs!
                log.Severe("Not Implemented yet - Cost");
                return Decimal.Multiply(GetMovementQty(), (GetPostCurrentCostPrice() != 0 ? GetPostCurrentCostPrice() : GetCurrentCostPrice()));
            }
            else if (MVABLCost.LANDEDCOSTDISTRIBUTION_Line.Equals(CostDistribution))
                return Env.ONE;
            else if (MVABLCost.LANDEDCOSTDISTRIBUTION_Quantity.Equals(CostDistribution))
                return GetMovementQty();
            else if (MVABLCost.LANDEDCOSTDISTRIBUTION_Volume.Equals(CostDistribution))
            {
                MProduct product = GetProduct();
                if (product == null)
                {
                    log.Severe("No Product");
                    return Env.ZERO;
                }
                return Decimal.Multiply(GetMovementQty(), (Decimal)product.GetVolume());
            }
            else if (MVABLCost.LANDEDCOSTDISTRIBUTION_Weight.Equals(CostDistribution))
            {
                MProduct product = GetProduct();
                if (product == null)
                {
                    log.Severe("No Product");
                    return Env.ZERO;
                }
                return Decimal.Multiply(GetMovementQty(), product.GetWeight());
            }
            log.Severe("Invalid Criteria: " + CostDistribution);
            return Env.ZERO;
        }
    }
}
