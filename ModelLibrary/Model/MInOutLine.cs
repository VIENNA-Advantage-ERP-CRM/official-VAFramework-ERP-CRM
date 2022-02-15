/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MInOutLine
 * Purpose        : For 2nd tab of the shipment window
 * Class Used     : X_M_InOutLine
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
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MInOutLine : X_M_InOutLine
    {
        #region variables
        //	Product					
        private MProduct _product = null;
        // Warehouse			
        private int _M_Warehouse_ID = 0;
        //Parent				
        private MInOut _parent = null;
        // Matched Invoices		
        private MMatchInv[] _matchInv = null;
        // Matched Purchase Orders	
        private MMatchPO[] _matchPO = null;
        //	Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MInOutLine).FullName);
        public Decimal? OnHandQty = 0;
        private Decimal? containerQty = 0;
        #endregion

        /* 	Get Ship lines Of Order Line
        *	@param ctx context
        *	@param C_OrderLine_ID line
        *	@param where optional addition where clause
        *  @param trxName transaction
        *	@return array of receipt lines
        */
        public static MInOutLine[] GetOfOrderLine(Ctx ctx,
            int C_OrderLine_ID, String where, Trx trxName)
        {
            List<MInOutLine> list = new List<MInOutLine>();


            //String sql = "SELECT * FROM M_InOutLine WHERE C_OrderLine_ID=" + C_OrderLine_ID;    //// Commented and added following query, by SUkhwinder on 16-Nov-2017, for taking into COMPLETED/CLOSED lines only.

            String sql = " SELECT M_Inoutline.*    " +
                         " FROM M_Inoutline M_Inoutline    " +
                         " INNER JOIN M_Inout M_Inout    " +
                         " ON M_inout.M_inout_ID = M_Inoutline.M_inout_ID    " +
                         " WHERE M_Inout.Docstatus       IN ('CO','CL')    " +
                         " AND M_Inoutline.C_Orderline_Id = " + C_OrderLine_ID;

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
                    list.Add(new MInOutLine(ctx, dr, trxName));
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


            MInOutLine[] retValue = new MInOutLine[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
         * 	Get Ship lines Of Order Line
         *	@param ctx context
         *	@param C_OrderLine_ID line
         *  @param trxName transaction
         *	@return array of receipt lines2
         */
        public static MInOutLine[] Get(Ctx ctx, int C_OrderLine_ID, Trx trxName)
        {
            List<MInOutLine> list = new List<MInOutLine>();
            String sql = "SELECT * FROM M_InOutLine WHERE C_OrderLine_ID=" + C_OrderLine_ID;
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
                    list.Add(new MInOutLine(ctx, dr, trxName));
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

            MInOutLine[] retValue = new MInOutLine[list.Count];
            retValue = list.ToArray();
            return retValue;
        }


        /*
     * 	Standard Constructor
     *	@param ctx context
     *	@param M_InOutLine_ID id
     *	@param trxName trx name
     */
        public MInOutLine(Ctx ctx, int M_InOutLine_ID, Trx trxName)
            : base(ctx, M_InOutLine_ID, trxName)
        {
            try
            {

                if (M_InOutLine_ID == 0)
                {
                    //	setLine (0);
                    //	setM_Locator_ID (0);
                    //	setC_UOM_ID (0);
                    //	setM_Product_ID (0);
                    SetM_AttributeSetInstance_ID(0);
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
                //MessageBox.Show("MInOutLine--Standard Constructor",ex.Message);
            }
        }

        /**
        *  Parent Constructor
        *  @param inout parent
        */
        public MInOutLine(MInOut inout)
            : this(inout.GetCtx(), 0, inout.Get_TrxName())
        {

            SetClientOrg(inout);
            SetM_InOut_ID(inout.GetM_InOut_ID());
            SetM_Warehouse_ID(inout.GetM_Warehouse_ID());
            SetC_Project_ID(inout.GetC_Project_ID());
            _parent = inout;
        }

        /**
        *  Load Constructor
        *  @param ctx context
        *  @param dr result set record
        *  @param trxName transaction
        */
        public MInOutLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /* 	Get Parent
        *	@return parent
        */
        public MInOut GetParent()
        {
            if (_parent == null)
                _parent = new MInOut(GetCtx(), GetM_InOut_ID(), Get_TrxName());
            return _parent;
        }

        /// <summary>
        /// Set Parent
        /// </summary>
        /// <param name="parent">parent</param>
        public void SetParent(MInOut parent)
        {
            _parent = parent;
        }

        /**
     * 	Set Order Line.
     * 	Does not set Quantity!
     *	@param oLine order line
     *	@param M_Locator_ID locator
     * 	@param Qty used only to find suitable locator
     */
        public void SetOrderLine(MOrderLine oLine, int M_Locator_ID, Decimal Qty)
        {
            SetC_OrderLine_ID(oLine.GetC_OrderLine_ID());
            SetLine(oLine.GetLine());
            SetC_UOM_ID(oLine.GetC_UOM_ID());
            MProduct product = oLine.GetProduct();
            if (product == null)
            {
                SetM_Product_ID(0);
                SetM_AttributeSetInstance_ID(0);
                base.SetM_Locator_ID(0);
            }
            else
            {
                SetM_Product_ID(oLine.GetM_Product_ID());
                SetM_AttributeSetInstance_ID(oLine.GetM_AttributeSetInstance_ID());
                //
                if (product.IsItem())
                {
                    if (M_Locator_ID == 0)
                        SetM_Locator_ID(Qty);	//	requires warehouse, product, asi
                    else
                        SetM_Locator_ID(M_Locator_ID);
                }
                else
                {
                    base.SetM_Locator_ID(0);
                }
            }
            SetC_Charge_ID(oLine.GetC_Charge_ID());
            SetDescription(oLine.GetDescription());
            SetIsDescription(oLine.IsDescription());
            //
            SetC_Project_ID(oLine.GetC_Project_ID());
            SetC_ProjectPhase_ID(oLine.GetC_ProjectPhase_ID());
            SetC_ProjectTask_ID(oLine.GetC_ProjectTask_ID());
            SetC_Activity_ID(oLine.GetC_Activity_ID());
            SetC_Campaign_ID(oLine.GetC_Campaign_ID());
            SetAD_OrgTrx_ID(oLine.GetAD_OrgTrx_ID());
            SetUser1_ID(oLine.GetUser1_ID());
            SetUser2_ID(oLine.GetUser2_ID());
        }

        /*	Set Order Line - Callout
        *	@param oldC_OrderLine_ID old BP
        *	@param newC_OrderLine_ID new BP
        *	@param windowNo window no
        */
        //@UICallout
        public void SetC_OrderLine_ID(String oldC_OrderLine_ID, String newC_OrderLine_ID, int windowNo)
        {
            if (newC_OrderLine_ID == null || newC_OrderLine_ID.Length == 0)
                return;
            int C_OrderLine_ID = int.Parse(newC_OrderLine_ID);
            if (C_OrderLine_ID == 0)
                return;
            MOrderLine ol = new MOrderLine(GetCtx(), C_OrderLine_ID, null);
            if (ol.Get_ID() != 0)
            {
                SetC_OrderLine_ID(C_OrderLine_ID);
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
                    MInOutLine ioLine = new MInOutLine(GetCtx(), ol.GetOrig_InOutLine_ID(), null);
                    SetM_Locator_ID(ioLine.GetM_Locator_ID());
                }

            }
        }

        /**
         * 	Set Invoice Line.
         * 	Does not set Quantity!
         *	@param iLine invoice line
         *	@param M_Locator_ID locator
         *	@param Qty qty only fo find suitable locator
         */
        public void SetInvoiceLine(MInvoiceLine iLine, int M_Locator_ID, Decimal Qty)
        {
            SetC_OrderLine_ID(iLine.GetC_OrderLine_ID());
            SetLine(iLine.GetLine());
            SetC_UOM_ID(iLine.GetC_UOM_ID());
            int M_Product_ID = iLine.GetM_Product_ID();
            if (M_Product_ID == 0)
            {
                Set_ValueNoCheck("M_Product_ID", null);
                Set_ValueNoCheck("M_Locator_ID", null);
                Set_ValueNoCheck("M_AttributeSetInstance_ID", null);
            }
            else
            {
                SetM_Product_ID(M_Product_ID);
                SetM_AttributeSetInstance_ID(iLine.GetM_AttributeSetInstance_ID());
                if (M_Locator_ID == 0)
                    SetM_Locator_ID(Qty);	//	requires warehouse, product, asi
                else
                    SetM_Locator_ID(M_Locator_ID);
            }
            SetC_Charge_ID(iLine.GetC_Charge_ID());
            SetDescription(iLine.GetDescription());
            SetIsDescription(iLine.IsDescription());
            //
            SetC_Project_ID(iLine.GetC_Project_ID());
            SetC_ProjectPhase_ID(iLine.GetC_ProjectPhase_ID());
            SetC_ProjectTask_ID(iLine.GetC_ProjectTask_ID());
            SetC_Activity_ID(iLine.GetC_Activity_ID());
            SetC_Campaign_ID(iLine.GetC_Campaign_ID());
            SetAD_OrgTrx_ID(iLine.GetAD_OrgTrx_ID());
            SetUser1_ID(iLine.GetUser1_ID());
            SetUser2_ID(iLine.GetUser2_ID());
        }

        /**
         * 	Get Warehouse
         *	@return Returns the m_Warehouse_ID.
         */
        public int GetM_Warehouse_ID()
        {
            if (_M_Warehouse_ID == 0)
                _M_Warehouse_ID = GetParent().GetM_Warehouse_ID();
            return _M_Warehouse_ID;
        }

        /**
        * 	Set Warehouse
        *	@param warehouse_ID The m_Warehouse_ID to set.
        */
        public void SetM_Warehouse_ID(int warehouse_ID)
        {
            _M_Warehouse_ID = warehouse_ID;
        }

        /*	Set M_Locator_ID
        *	@param M_Locator_ID id
        */
        public void SetM_Locator_ID(int M_Locator_ID)
        {
            if (M_Locator_ID < 0)
                throw new ArgumentException("M_Locator_ID is mandatory.");
            //	set to 0 explicitly to reset
            Set_Value("M_Locator_ID", M_Locator_ID);
        }

        /**
         * 	Set (default) Locator based on qty.
         * 	@param Qty quantity
         * 	Assumes Warehouse is set
         */
        public void SetM_Locator_ID(Decimal Qty)
        {
            //	Locator esatblished
            if (GetM_Locator_ID() != 0)
                return;
            //	No Product
            if (GetM_Product_ID() == 0)
            {
                Set_ValueNoCheck("M_Locator_ID", null);
                return;
            }

            //	Get existing Location
            int M_Locator_ID = MStorage.GetM_Locator_ID(GetM_Warehouse_ID(),
                    GetM_Product_ID(), GetM_AttributeSetInstance_ID(),
                    Qty, Get_TrxName());
            //	Get default Location
            if (M_Locator_ID == 0)
            {
                MProduct product = MProduct.Get(GetCtx(), GetM_Product_ID());
                M_Locator_ID = MProductLocator.GetFirstM_Locator_ID(product, GetM_Warehouse_ID());
                if (M_Locator_ID == 0)
                {
                    MWarehouse wh = MWarehouse.Get(GetCtx(), GetM_Warehouse_ID());
                    M_Locator_ID = wh.GetDefaultM_Locator_ID();
                }
            }
            SetM_Locator_ID(M_Locator_ID);
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
            if (QtyEntered != 0 && GetC_UOM_ID() != 0)
            {
                int precision = MUOM.GetPrecision(GetCtx(), GetC_UOM_ID());
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
            if (_product == null && GetM_Product_ID() != 0)
                _product = MProduct.Get(GetCtx(), GetM_Product_ID());
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
                SetM_Product_ID(_product.GetM_Product_ID());
                SetC_UOM_ID(_product.GetC_UOM_ID());
            }
            else
            {
                SetM_Product_ID(0);
                SetC_UOM_ID(0);
            }
            SetM_AttributeSetInstance_ID(0);
        }

        /**
         * 	Set M_Product_ID
         *	@param M_Product_ID product
         *	@param setUOM also set UOM from product
         */
        public void SetM_Product_ID(int M_Product_ID, bool setUOM)
        {
            if (setUOM)
                SetProduct(MProduct.Get(GetCtx(), M_Product_ID));
            else
                base.SetM_Product_ID(M_Product_ID);
            SetM_AttributeSetInstance_ID(0);
        }

        /**
         * 	Set Product and UOM
         *	@param M_Product_ID product
         *	@param C_UOM_ID uom
         */
        public void SetM_Product_ID(int M_Product_ID, int C_UOM_ID)
        {
            if (M_Product_ID != 0)
            {
                base.SetM_Product_ID(M_Product_ID);
            }
            base.SetC_UOM_ID(C_UOM_ID);
            SetM_AttributeSetInstance_ID(0);
            _product = null;
        }

        /**
        * 	Set Product and UOM
        *	@param M_Product_ID product
        *	@param C_UOM_ID uom
        *	@param M_AttributeSetInstance_ID AttributeSetInstance
        */
        public void SetM_Product_ID(int M_Product_ID, int C_UOM_ID, int M_AttributeSetInstance_ID)
        {
            if (M_Product_ID != 0)
            {
                base.SetM_Product_ID(M_Product_ID);
            }
            base.SetC_UOM_ID(C_UOM_ID);
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            _product = null;
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
            if (newM_Product_ID == null || newM_Product_ID.Length == 0)
                return;
            int M_Product_ID = int.Parse(newM_Product_ID);
            if (M_Product_ID == 0)
            {
                SetM_AttributeSetInstance_ID(0);
                return;
            }
            //
            base.SetM_Product_ID(M_Product_ID);
            SetC_Charge_ID(0);

            //	Set Attribute & Locator
            int M_Locator_ID = 0;
            if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_Product_ID") == M_Product_ID
                && GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_AttributeSetInstance_ID") != 0)
            {
                SetM_AttributeSetInstance_ID(GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_AttributeSetInstance_ID"));
                //	Locator from Info Window - ASI
                M_Locator_ID = GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_Locator_ID");
                if (M_Locator_ID != 0)
                    SetM_Locator_ID(M_Locator_ID);
            }
            else
                SetM_AttributeSetInstance_ID(0);
            //
            bool IsSOTrx = GetCtx().IsSOTrx(windowNo);
            if (IsSOTrx)
                return;

            //	PO - Set UOM/Locator/Qty
            MProduct product = GetProduct();
            SetC_UOM_ID(product.GetC_UOM_ID());
            Decimal QtyEntered = GetQtyEntered();
            SetMovementQty(QtyEntered);
            if (M_Locator_ID != 0)
            {
                ;		//	already set via ASI
            }
            else
            {
                int M_Warehouse_ID = GetCtx().GetContextAsInt(windowNo, "M_Warehouse_ID");
                M_Locator_ID = MProductLocator.GetFirstM_Locator_ID(product, M_Warehouse_ID);
                if (M_Locator_ID != 0)
                    SetM_Locator_ID(M_Locator_ID);
                else
                {
                    MWarehouse wh = MWarehouse.Get(GetCtx(), M_Warehouse_ID);
                    SetM_Locator_ID(wh.GetDefaultM_Locator_ID());
                }
            }
        }

        /**
         * 	Set Product - Callout
         *	@param oldM_AttributeSetInstance_ID old value
         *	@param newM_AttributeSetInstance_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetM_AttributeSetInstance_ID(String oldM_AttributeSetInstance_ID,
            String newM_AttributeSetInstance_ID, int windowNo)
        {
            if (newM_AttributeSetInstance_ID == null || newM_AttributeSetInstance_ID.Length == 0)
                return;
            int M_AttributeSetInstance_ID = int.Parse(newM_AttributeSetInstance_ID);
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            if (M_AttributeSetInstance_ID == 0)
                return;
            //
            int M_Product_ID = GetM_Product_ID();
            int M_Warehouse_ID = GetCtx().GetContextAsInt(windowNo, "M_Warehouse_ID");
            int M_Locator_ID = GetM_Locator_ID();
            log.Fine("M_Product_ID=" + M_Product_ID
                + ", M_ASI_ID=" + M_AttributeSetInstance_ID
                + " - M_Warehouse_ID=" + M_Warehouse_ID
                + ", M_Locator_ID=" + M_Locator_ID);
            //	Check Selection
            int M_ASI_ID = Env.GetContext().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_AttributeSetInstance_ID");
            if (M_ASI_ID == M_AttributeSetInstance_ID)
            {
                int selectedM_Locator_ID = Env.GetContext().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_Locator_ID");
                if (selectedM_Locator_ID != 0)
                {
                    log.Fine("Selected M_Locator_ID=" + selectedM_Locator_ID);
                    SetM_Locator_ID(selectedM_Locator_ID);
                }
            }
        }

        /**
         * 	Set UOM - Callout
         *	@param oldC_UOM_ID old value
         *	@param newC_UOM_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        ///@UICallout 
        public void SetC_UOM_ID(String oldC_UOM_ID,
                String newC_UOM_ID, int windowNo)
        {
            if (newC_UOM_ID == null || newC_UOM_ID.Length == 0)
                return;
            int C_UOM_ID = int.Parse(newC_UOM_ID);
            if (C_UOM_ID == 0)
                return;
            //
            base.SetC_UOM_ID(C_UOM_ID);
            SetQty(windowNo, "C_UOM_ID");
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
            int M_Product_ID = GetM_Product_ID();
            log.Log(Level.WARNING, "qty - init - M_Product_ID=" + M_Product_ID);
            Decimal MovementQty, QtyEntered;
            int C_UOM_To_ID = GetC_UOM_ID();

            //	No Product
            if (M_Product_ID == 0)
            {
                QtyEntered = GetQtyEntered();
                SetMovementQty(QtyEntered);
            }
            //	UOM Changed - convert from Entered -> Product
            else if (columnName.Equals("C_UOM_ID"))
            {
                QtyEntered = GetQtyEntered();
                //Decimal QtyEntered1 = QtyEntered.setScale(MUOM.GetPrecision(GetCtx(), C_UOM_To_ID), Decimal.ROUND_HALF_UP);
                Decimal QtyEntered1 = Decimal.Round(QtyEntered, MUOM.GetPrecision(GetCtx(), C_UOM_To_ID), MidpointRounding.AwayFromZero);
                if (QtyEntered.CompareTo(QtyEntered1) != 0)
                {
                    log.Fine("Corrected QtyEntered Scale UOM=" + C_UOM_To_ID
                    + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                    QtyEntered = QtyEntered1;
                    SetQtyEntered(QtyEntered);
                }
                MovementQty = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), M_Product_ID, C_UOM_To_ID, QtyEntered);
                if (MovementQty == null)
                    MovementQty = QtyEntered;
                bool conversion = QtyEntered.CompareTo(MovementQty) != 0;

                log.Fine("UOM=" + C_UOM_To_ID
                    + ", QtyEntered=" + QtyEntered
                    + " -> " + conversion
                    + " MovementQty=" + MovementQty);

                //p_changeVO.setContext(getCtx(), windowNo, "UOMConversion", conversion);
                SetMovementQty(MovementQty);
            }
            //	No UOM defined
            else if (C_UOM_To_ID == 0)
            {
                QtyEntered = GetQtyEntered();
                SetMovementQty(QtyEntered);
            }
            //	QtyEntered changed - calculate MovementQty
            else if (columnName.Equals("QtyEntered"))
            {
                QtyEntered = GetQtyEntered();
                Decimal QtyEntered1 = Decimal.Round(QtyEntered, MUOM.GetPrecision(GetCtx(), C_UOM_To_ID), MidpointRounding.AwayFromZero);
                if (QtyEntered.CompareTo(QtyEntered1) != 0)
                {
                    log.Fine("Corrected QtyEntered Scale UOM=" + C_UOM_To_ID
                        + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                    QtyEntered = QtyEntered1;
                    SetQtyEntered(QtyEntered);
                }
                MovementQty = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(),
                    M_Product_ID, C_UOM_To_ID, QtyEntered);
                if (MovementQty == null)
                    MovementQty = QtyEntered;
                bool conversion = QtyEntered.CompareTo(MovementQty) != 0;

                log.Fine("UOM=" + C_UOM_To_ID
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
                int precision = MProduct.Get(GetCtx(), M_Product_ID).GetUOMPrecision();
                //Decimal MovementQty1 = MovementQty.setScale(precision, Decimal.ROUND_HALF_UP);
                Decimal MovementQty1 = Decimal.Round(MovementQty, precision, MidpointRounding.AwayFromZero);// Env.Scale(MovementQty);
                if (MovementQty.CompareTo(MovementQty1) != 0)
                {
                    log.Fine("Corrected MovementQty "
                       + MovementQty + "->" + MovementQty1);
                    MovementQty = MovementQty1;
                    SetMovementQty(MovementQty);
                }
                QtyEntered = (Decimal)MUOMConversion.ConvertProductTo(GetCtx(), M_Product_ID, C_UOM_To_ID, MovementQty);
                if (QtyEntered == null)
                    QtyEntered = MovementQty;
                bool conversion = MovementQty.CompareTo(QtyEntered) != 0;
                log.Fine("UOM=" + C_UOM_To_ID
                    + ", MovementQty=" + MovementQty
                    + " -> " + conversion
                    + " QtyEntered=" + QtyEntered);

                //p_changeVO.setContext(getCtx(), windowNo, "UOMConversion", conversion);
                SetQtyEntered(QtyEntered);
            }

            // RMA : Check qty returned is more than qty shipped
            bool IsReturnTrx = GetParent().IsReturnTrx();
            if (M_Product_ID != 0 && IsReturnTrx)
            {
                int oLine_ID = GetC_OrderLine_ID();
                MOrderLine oLine = new MOrderLine(GetCtx(), oLine_ID, null);
                if (oLine.Get_ID() != 0)
                {
                    int orig_IOLine_ID = oLine.GetOrig_InOutLine_ID();
                    if (orig_IOLine_ID != 0)
                    {
                        MInOutLine orig_IOLine = new MInOutLine(GetCtx(), orig_IOLine_ID, null);
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
                            QtyEntered = (Decimal)MUOMConversion.ConvertProductTo(GetCtx(), M_Product_ID,
                                    C_UOM_To_ID, MovementQty);
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
         * 	Get C_Project_ID
         *	@return project
         */
        public int GetC_Project_ID()
        {
            int ii = base.GetC_Project_ID();
            if (ii == 0)
                ii = GetParent().GetC_Project_ID();
            return ii;
        }

        /**
         * 	Get C_Activity_ID
         *	@return Activity
         */
        public int GetC_Activity_ID()
        {
            int ii = base.GetC_Activity_ID();
            if (ii == 0)
                ii = GetParent().GetC_Activity_ID();
            return ii;
        }

        /**
         * 	Get C_Campaign_ID
         *	@return Campaign
         */
        public int GetC_Campaign_ID()
        {
            int ii = base.GetC_Campaign_ID();
            if (ii == 0)
                ii = GetParent().GetC_Campaign_ID();
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
         * 	Get AD_OrgTrx_ID
         *	@return trx org
         */
        public int GetAD_OrgTrx_ID()
        {
            int ii = base.GetAD_OrgTrx_ID();
            if (ii == 0)
                ii = GetParent().GetAD_OrgTrx_ID();
            return ii;
        }

        /**
         * 	Get Match POs
         *	@return matched purchase orders
         */
        public MMatchPO[] GetMatchPO()
        {
            if (_matchPO == null)
                _matchPO = MMatchPO.Get(GetCtx(), GetM_InOutLine_ID(), Get_TrxName());
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
            MMatchPO[] po = GetMatchPO();
            for (int i = 0; i < po.Length; i++)
            {
                MMatchPO matchPO = po[i];
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
            MMatchPO[] po = GetMatchPO();
            for (int i = 0; i < po.Length; i++)
            {
                MMatchPO matchPO = po[i];
                if (!matchPO.IsPosted())
                    return false;
            }
            return true;
        }

        /**
         * 	Get Match Inv
         *	@return matched invoices
         */
        public MMatchInv[] GetMatchInv()
        {
            if (_matchInv == null)
                _matchInv = MMatchInv.Get(GetCtx(), GetM_InOutLine_ID(), Get_TrxName());
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
            MMatchInv[] inv = GetMatchInv();
            for (int i = 0; i < inv.Length; i++)
            {
                MMatchInv matchInv = inv[i];
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
            MMatchInv[] inv = GetMatchInv();
            for (int i = 0; i < inv.Length; i++)
            {
                MMatchInv matchInv = inv[i];
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
            if (GetC_Charge_ID() == 0 && GetM_Product_ID() == 0)
            {
                log.SaveError("VIS_NOProductOrCharge", "");
                return false;
            }

            //	Get Line No
            if (GetLine() == 0)
            {
                String sql = "SELECT COALESCE(MAX(Line),0)+10 FROM M_InOutLine WHERE M_InOut_ID=" + GetM_InOut_ID();
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
            if (GetC_UOM_ID() == 0)
                SetC_UOM_ID(GetCtx().GetContextAsInt("#C_UOM_ID"));
            if (GetC_UOM_ID() == 0)
            {
                int C_UOM_ID = MUOM.GetDefault_UOM_ID(GetCtx());
                if (C_UOM_ID > 0)
                    SetC_UOM_ID(C_UOM_ID);
            }
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

            Decimal newMoveQty = movementQty ?? 0; //Arpit

            //Checking for conversion of UOM 
            MInOut inO = GetParent();
            MDocType dt = MDocType.Get(GetCtx(), inO.GetC_DocType_ID());
            MProduct _Product = null;

            // VIS0060: if the Shipment date is less than Asset Depreciation Date it will not save the record
            if (Is_ValueChanged("A_Asset_ID") && inO.IsSOTrx() && Env.IsModuleInstalled("VAFAM_") && GetA_Asset_ID() > 0)
            {
                DateTime? depDate = Util.GetValueOfDateTime(DB.ExecuteScalar("SELECT MAX(VAFAM_DepDate) FROM VAFAM_AssetSchedule WHERE VAFAM_DepAmor='Y' AND A_Asset_ID=" + GetA_Asset_ID()));
                if (inO.GetDateAcct() <= depDate)
                {
                    log.SaveError("VAFAM_CouldNotSave", "");
                    return false;
                }
            }

            // Check if Product_ID is non zero then only create the object
            if (GetM_Product_ID() > 0)
            {
                _Product = new MProduct(GetCtx(), GetM_Product_ID(), Get_TrxName());
            }

            if (_Product != null && GetC_UOM_ID() != _Product.GetC_UOM_ID())
            {
                decimal? differenceQty = Util.GetValueOfDecimal(GetCtx().GetContext("DifferenceQty_"));
                if (differenceQty > 0 && !newRecord && !dt.IsSplitWhenDifference())
                {
                    //converted qty if found any difference then the Movement qty reduces by the difference amount
                    pc = MUOMConversion.ConvertProductFrom(GetCtx(), GetM_Product_ID(), GetC_UOM_ID(), QtyEntered - differenceQty);
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
            if (((_Product != null && _Product.GetProductType() != MProduct.PRODUCTTYPE_Item) || GetC_Charge_ID() > 0) && GetM_Locator_ID() == 0)
            {
                qry1 = "SELECT M_Locator_ID FROM M_Locator WHERE M_Warehouse_ID=" + inO.GetM_Warehouse_ID() + " AND IsDefault = 'Y'";
                int il = Util.GetValueOfInt(DB.ExecuteScalar(qry1, null, Get_TrxName()));
                if (il == 0)
                {
                    qry1 = "SELECT MAX(M_Locator_ID) FROM M_Locator WHERE M_Warehouse_ID=" + inO.GetM_Warehouse_ID() + " AND IsActive = 'Y'";
                    il = Util.GetValueOfInt(DB.ExecuteScalar(qry1, null, Get_TrxName()));
                }
                SetM_Locator_ID(il);
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

            //	Qty Precision
            if (newRecord || Is_ValueChanged("QtyEntered"))
                SetQtyEntered(GetQtyEntered());
            if (newRecord || Is_ValueChanged("MovementQty"))
                SetMovementQty(GetMovementQty());

            // dont verify qty during completion
            // on Ship/Receipt, do not check qty in warehouse for Lines of Charge.
            if ((!inO.IsProcessing() || newRecord) && _Product != null && _Product.IsStocked())
            {
                //int M_Warehouse_ID = 0; MWarehouse wh = null;
                StringBuilder qry = new StringBuilder();
                //qry.Append("select m_warehouse_id from m_locator where m_locator_id=" + GetM_Locator_ID());
                //M_Warehouse_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry.ToString()));

                //wh = MWarehouse.Get(GetCtx(), M_Warehouse_ID);

                string IsDisallowNegativeInv = Util.GetValueOfString(DB.ExecuteScalar(@"SELECT IsDisallowNegativeInv FROM M_Warehouse WHERE M_Warehouse_ID = 
                            (SELECT M_Warehouse_ID FROM M_Locator WHERE M_Locator_ID = " + GetM_Locator_ID() + ")", null, Get_Trx()));
                qry.Clear();
                qry.Append("SELECT QtyOnHand FROM M_Storage WHERE M_Locator_ID=" + GetM_Locator_ID() + " AND M_Product_ID=" + GetM_Product_ID());
                if (GetM_AttributeSetInstance_ID() != 0)
                {
                    qry.Append(" AND M_AttributeSetInstance_ID=" + GetM_AttributeSetInstance_ID());
                }
                else
                {
                    //SI_0642 :  when we have to check qtyonhand where ASI is null or 0
                    qry.Append(" AND NVL(M_AttributeSetInstance_ID, 0) = 0");
                }
                OnHandQty = Convert.ToDecimal(DB.ExecuteScalar(qry.ToString(), null, Get_Trx()));
                // when record is in completed & closed stage - then no need to check qty availablity in warehouse
                if (IsDisallowNegativeInv.Equals("Y") &&
                    (!(inO.GetDocStatus() == "CO" || inO.GetDocStatus() == "CL" || inO.GetDocStatus() == "RE" || inO.GetDocStatus() == "VO")))
                {
                    // pick container current qty from transaction based on locator / product / ASI / Container / Movement Date 
                    if (isContainrApplicable && Get_ColumnIndex("M_ProductContainer_ID") >= 0)
                    {
                        qry.Clear();
                        qry.Append(@"SELECT DISTINCT First_VALUE(t.ContainerCurrentQty) OVER (PARTITION BY t.M_Product_ID, t.M_AttributeSetInstance_ID ORDER BY t.MovementDate DESC, t.M_Transaction_ID DESC) AS CurrentQty FROM m_transaction t 
                            INNER JOIN M_Locator l ON t.M_Locator_ID = l.M_Locator_ID WHERE t.MovementDate <= " + GlobalVariable.TO_DATE(inO.GetMovementDate(), true) +
                                    " AND t.AD_Client_ID = " + GetAD_Client_ID() + " AND t.M_Locator_ID = " + GetM_Locator_ID() +
                                    " AND t.M_Product_ID = " + GetM_Product_ID() + " AND NVL(t.M_AttributeSetInstance_ID,0) = " + GetM_AttributeSetInstance_ID() +
                                    " AND NVL(t.M_ProductContainer_ID, 0) = " + GetM_ProductContainer_ID());
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
                        if (isContainrApplicable && Get_ColumnIndex("M_ProductContainer_ID") >= 0 && (containerQty + GetMovementQty()) < 0)
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
                        if (isContainrApplicable && Get_ColumnIndex("M_ProductContainer_ID") >= 0 && (containerQty - GetMovementQty()) < 0)
                        {
                            log.SaveError("Info", _Product.GetName() + " , " + Msg.GetMsg(GetCtx(), "VIS_InsufficientQtyContainer") + containerQty);
                            return false;
                        }
                    }
                }
            }

            //during creating a line of M_inout, system check qty on line cannot be greater than Ordered qty (Ordered - delivered)
            if (!inO.IsReversal())
            {
                if (GetC_OrderLine_ID() > 0 && (newRecord || Is_ValueChanged("IsActive") || Is_ValueChanged("MovementQty")))
                {
                    qry1 = "select  QtyOrdered - QtyDelivered from C_OrderLine where C_OrderLine_ID =" + GetC_OrderLine_ID();
                    Decimal? _result = Util.GetValueOfDecimal(DB.ExecuteScalar(qry1, null, Get_Trx()));
                    decimal QtyNotDelivered = 0;
                    if (newRecord)
                        QtyNotDelivered = Util.GetValueOfDecimal(DB.ExecuteScalar(@"SELECT SUM(MovementQty) FROM M_Inout i INNER JOIN M_InoutLine il ON i.M_Inout_ID = il.M_Inout_ID
                            WHERE il.C_OrderLine_ID = " + GetC_OrderLine_ID() + @" AND il.Isactive = 'Y' AND i.docstatus NOT IN ('RE' , 'VO' , 'CL' , 'CO')", null, Get_Trx()));
                    else
                        QtyNotDelivered = Util.GetValueOfDecimal(DB.ExecuteScalar(@"SELECT SUM(MovementQty) FROM M_Inout i INNER JOIN M_InoutLine il ON i.M_Inout_ID = il.M_Inout_ID
                            WHERE il.C_OrderLine_ID = " + GetC_OrderLine_ID() + @" AND il.Isactive = 'Y' AND i.docstatus NOT IN ('RE' , 'VO' , 'CL' , 'CO') AND il.M_InoutLine_ID <> " + GetM_InOutLine_ID(), null, Get_Trx()));
                    if (GetMovementQty() > (_result - QtyNotDelivered))
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "QtyCanNotbeGreater"));
                        return false;
                    }
                }

                //VA230: Quantity cannot be greater than Asset Quantity in case of shipment.
                if (inO.IsSOTrx() && GetA_Asset_ID() > 0 && Env.IsModuleInstalled("VAFAM_") && Get_ColumnIndex("VAFAM_Quantity") >= 0 && GetMovementQty() > GetVAFAM_Quantity())
                {
                    log.SaveError("Error", Msg.GetMsg(GetCtx(), "QtyCanNotbeGreaterThanAssetQty"));
                    return false;
                }
            }

            //	Order Line
            if (GetC_OrderLine_ID() == 0)
            {
                if (GetParent().IsSOTrx())
                {
                    log.SaveError("FillMandatory", Msg.Translate(GetCtx(), "C_Order_ID"));
                    return false;
                }
            }

            // get current cost from Orignal Document on new record and when product changed
            // when record is  return transaction then price should pick from its Original record
            // now, not to set orignal record cost, system will update current cost
            //decimal currentcostprice = 0;
            //if (inO.IsReturnTrx() && GetC_OrderLine_ID() > 0)
            //{
            //    MOrderLine RMALine = new MOrderLine(GetCtx(), GetC_OrderLine_ID(), Get_Trx());
            //    if (RMALine != null)
            //    {
            //        MInOutLine origInOutLine = new MInOutLine(GetCtx(), RMALine.GetOrig_InOutLine_ID(), Get_Trx());
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
                //MInOut inout = new MInOut(GetCtx(), GetM_InOut_ID(), Get_Trx());
                //shipment and Return to vendor
                if ((!inO.IsSOTrx() && inO.IsReturnTrx()) || (inO.IsSOTrx() && !inO.IsReturnTrx()))
                {
                    qry1 = @"SELECT  SUM(o.VA024_UnitPrice)  FROM VA024_t_ObsoleteInventory o 
                                  WHERE o.IsActive = 'Y' AND  o.M_Product_ID = " + GetM_Product_ID() + @" and 
                                  ( o.M_AttributeSetInstance_ID = " + GetM_AttributeSetInstance_ID() + @" OR o.M_AttributeSetInstance_ID IS NULL )" +
                              " AND o.AD_Org_ID = " + GetAD_Org_ID();
                    VA024_ProvisionPrice = Util.GetValueOfDecimal(DB.ExecuteScalar(qry1, null, Get_Trx()));
                    SetVA024_UnitPrice(Util.GetValueOfDecimal(VA024_ProvisionPrice * GetMovementQty()));

                    // is used to get cost of binded cost method / costing level of primary accounting schema
                    Decimal cost = MCost.GetproductCosts(inO.GetAD_Client_ID(), inO.GetAD_Org_ID(), GetM_Product_ID(),
                         GetM_AttributeSetInstance_ID(), Get_Trx(), inO.GetM_Warehouse_ID());
                    SetVA024_CostPrice((cost - VA024_ProvisionPrice) * GetMovementQty());
                }
            }

            //Mandatory
            if (Env.IsModuleInstalled("DTD001_"))
            {
                if (GetM_AttributeSetInstance_ID() != 0)	//	Set to from
                    SetM_AttributeSetInstance_ID(GetM_AttributeSetInstance_ID());
                else
                {
                    //MProduct product = GetProduct();
                    if (_Product != null
                        && _Product.GetM_AttributeSet_ID() != 0)
                    {
                        //MAttributeSet mas = MAttributeSet.Get(GetCtx(), product.GetM_AttributeSet_ID());
                        //if (mas.IsInstanceAttribute() 
                        //    && (mas.IsMandatory() || mas.IsMandatoryAlways()))
                        //{
                        //    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "M_AttributeSetInstanceTo_ID"));
                        //    return false;
                        //}

                        // Code Addeded by Bharat as Discussed with Mukesh Sir
                        if (String.IsNullOrEmpty(GetDTD001_Attribute()))
                        {
                            return true;
                        }
                        else
                        {
                            //MInOut inout = new MInOut(Env.GetCtx(), GetM_InOut_ID(), Get_Trx());
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
                        if (_Product != null)
                        {
                            if (_Product.GetM_AttributeSet_ID() == 0 && (GetDTD001_Attribute() == "" || GetDTD001_Attribute() == null))
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

        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return success;

            //Develop by Deekshant For VA077 Module for calculating purchase,sales,margin
            if (VAdvantage.Utility.Env.IsModuleInstalled("VA077_"))
            {
                if (!UpdateMargins())
                {
                    log.SaveWarning("", Msg.GetMsg(GetCtx(), "VIS_NotUpdated"));
                    return false;
                }
            }

            return true;
        }

        protected override bool AfterDelete(bool success)
        {
            if (!success)
                return success;

            //Develop by Deekshant For VA077 Module for calculating purchase,sales,margin
            if (VAdvantage.Utility.Env.IsModuleInstalled("VA077_"))
            {
                if (!UpdateMargins())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Update margins and other fields on header based on the lines saved
        /// </summary>
        /// <returns>true/false</returns>
        private bool UpdateMargins()
        {
            if (GetM_InOut_ID() > 0)
            {
                DataSet contDS = null;

                string sql = @"SELECT VA077_ServiceContract_ID, VA077_HistoricContractDate, VA077_ContractCPStartDate, DocumentNo,
                                   C_Frequency_ID, VA077_ContractCPEndDate, VA077_AnnualValue FROM VA077_ServiceContract 
                                   WHERE VA077_ServiceContract_ID IN(SELECT MAX(VA077_ServiceContract_ID) FROM M_InOutLine 
                                   WHERE IsActive = 'Y' AND M_InOut_ID =" + GetM_InOut_ID() + " AND VA077_ServiceContract_ID IS NOT NULL)";
                contDS = DB.ExecuteDataset(sql);
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
                    if (GetQtyEntered() > 0) // In the case of void qty will be zero, so set annual value only if qty is greater than zero
                        AnnualValue = Util.GetValueOfDecimal(contDS.Tables[0].Rows[0]["VA077_AnnualValue"]);
                    else if (GetQtyEntered() < 0)  // handle the reverse functionality case, make amt negative
                        AnnualValue = Util.GetValueOfDecimal(contDS.Tables[0].Rows[0]["VA077_AnnualValue"]) * -1;

                    qry.Clear();
                    qry.Append(@"UPDATE M_InOut  p SET VA077_TotalMarginAmt=(SELECT COALESCE(SUM(pl.VA077_MarginAmt),0) FROM M_InOutLine pl INNER JOIN C_OrderLine ol ON (pl.C_OrderLine_ID = ol.C_OrderLine_ID) 
                            WHERE pl.IsActive = 'Y' AND pl.M_InOut_ID = " + GetM_InOut_ID() + @"),
                            VA077_TotalPurchaseAmt=(SELECT ROUND(COALESCE(SUM(
                                                                              CASE WHEN ol.VA077_Duration is null THEN  
                                                                              ol.VA077_PurchasePrice * pl.QtyEntered  
                                                                              WHEN ol.VA077_Duration > 0 THEN  
                                                                              ol.VA077_PurchasePrice * pl.QtyEntered * ol.VA077_Duration / 12  
                                                                              END), 0), 2) FROM M_InOutLine pl INNER JOIN C_OrderLine ol ON (pl.C_OrderLine_ID = ol.C_OrderLine_ID)  
                            WHERE pl.IsActive = 'Y' AND pl.M_InOut_ID = " + GetM_InOut_ID() + @"),
                            VA077_MarginPercent=(SELECT CASE WHEN Sum(ol.LineNetAmt) <> 0 AND Sum(pl.QtyEntered) <> 0 Then 
                                                 ROUND(COALESCE((((CASE WHEN Sum(pl.QtyEntered) > 0 Then 
                                                                    COALESCE(SUM(ol.LineNetAmt),0) 
                                                                    WHEN Sum(pl.QtyEntered) < 0 Then 
                                                                    COALESCE(SUM(ol.LineNetAmt * -1),0) 
                                                                    ELSE 0  END) - Sum(
                                                                CASE WHEN ol.VA077_Duration is null THEN
                                                                    COALESCE(ol.VA077_PurchasePrice, 0) * pl.QtyEntered
                                                                    WHEN ol.VA077_Duration > 0 THEN
                                                                    COALESCE(ol.VA077_PurchasePrice, 0) * pl.QtyEntered * ol.VA077_Duration/12 
                                                                END)) / (CASE WHEN Sum(pl.QtyEntered) > 0 Then 
                                                                    COALESCE(SUM(ol.LineNetAmt),0) 
                                                                    WHEN Sum(pl.QtyEntered) < 0 Then 
                                                                    COALESCE(SUM(ol.LineNetAmt * -1),0) 
                                                                    ELSE 0  END) * 100), 0), 2) ELSE 0  END  
                            FROM M_InOutLine pl INNER JOIN C_OrderLine ol ON (pl.C_OrderLine_ID = ol.C_OrderLine_ID) WHERE pl.IsActive = 'Y' AND pl.M_InOut_ID = " + GetM_InOut_ID() + @"),
                            VA077_TotalSalesAmt=(SELECT CASE WHEN Sum(pl.QtyEntered) > 0 Then 
                                                                    COALESCE(SUM(ol.LineNetAmt),0) 
                                                                    WHEN Sum(pl.QtyEntered) < 0 Then 
                                                                    COALESCE(SUM(ol.LineNetAmt * -1),0) 
                                                                    ELSE 0  END FROM M_InOutLine pl  INNER JOIN C_OrderLine ol 
                                                ON (pl.C_OrderLine_ID = ol.C_OrderLine_ID)
                            WHERE pl.IsActive = 'Y' AND pl.M_InOut_ID = " + GetM_InOut_ID() + @")");
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
                                     VA077_ChangeStartDate = (SELECT MIN(ol.VA077_StartDate) FROM M_InOutLine pl INNER JOIN C_OrderLine ol ON (pl.C_OrderLine_ID = ol.C_OrderLine_ID) 
                                     WHERE pl.IsActive = 'Y' AND pl.M_InOut_ID = " + GetM_InOut_ID() + @"),
                                     VA077_PartialAmtCatchUp =(SELECT CASE WHEN Sum(pl.QtyEntered) > 0 Then 
                                                                    COALESCE(SUM(ol.LineNetAmt),0) 
                                                                    WHEN Sum(pl.QtyEntered) < 0 Then 
                                                                    COALESCE(SUM(ol.LineNetAmt * -1),0) 
                                                                    ELSE 0  END FROM M_InOutLine pl INNER JOIN C_OrderLine ol 
                                                               ON (pl.C_OrderLine_ID = ol.C_OrderLine_ID)
                                     WHERE pl.IsActive = 'Y' AND ol.VA077_ContractProduct='Y' AND pl.M_InOut_ID = " + GetM_InOut_ID() + @"),
                                     VA077_AdditionalAnnualCharge =(SELECT ROUND(COALESCE(SUM(ol.PriceEntered * pl.QtyEntered),0),2) FROM M_InOutLine pl INNER JOIN C_OrderLine ol ON (pl.C_OrderLine_ID = ol.C_OrderLine_ID)
                                     WHERE pl.IsActive = 'Y' AND ol.VA077_ContractProduct='Y' AND pl.M_InOut_ID = " + GetM_InOut_ID() + @"),
                                     VA077_NewAnnualContractTotal=(SELECT " + AnnualValue + @" + ROUND(COALESCE(SUM(ol.PriceEntered * pl.QtyEntered),0),2) FROM M_InOutLine pl INNER JOIN C_OrderLine ol ON (pl.C_OrderLine_ID = ol.C_OrderLine_ID) 
                                     WHERE pl.IsActive = 'Y' AND ol.VA077_ContractProduct='Y' AND pl.M_InOut_ID = " + GetM_InOut_ID() + @"),
                                     VA077_OrderRef = (SELECT VA077_OrderRef FROM C_Order WHERE C_Order_Id = (SELECT C_Order_Id FROM M_InOut WHERE M_InOut_ID=" + GetM_InOut_ID() + @")) 
                                     WHERE p.M_InOut_ID=" + GetM_InOut_ID() + "");
                }
                else
                {
                    qry.Clear();
                    qry.Append(@"UPDATE M_InOut  p SET VA077_TotalMarginAmt=(SELECT COALESCE(SUM(pl.VA077_MarginAmt),0) FROM M_InOutLine pl INNER JOIN C_OrderLine ol ON (pl.C_OrderLine_ID = ol.C_OrderLine_ID)
                            WHERE pl.IsActive = 'Y' AND pl.M_InOut_ID = " + GetM_InOut_ID() + @"),
                            VA077_TotalPurchaseAmt=(SELECT ROUND(COALESCE(SUM(
                                                                              CASE WHEN ol.VA077_Duration is null THEN  
                                                                              ol.VA077_PurchasePrice * pl.QtyEntered  
                                                                              WHEN ol.VA077_Duration > 0 THEN  
                                                                              ol.VA077_PurchasePrice * pl.QtyEntered * ol.VA077_Duration / 12  
                                                                              END), 0), 2) FROM M_InOutLine pl INNER JOIN C_OrderLine ol ON (pl.C_OrderLine_ID = ol.C_OrderLine_ID) 
                            WHERE pl.IsActive = 'Y' AND pl.M_InOut_ID = " + GetM_InOut_ID() + @"),
                            VA077_MarginPercent=(SELECT CASE WHEN Sum(ol.LineNetAmt) <> 0 AND Sum(pl.QtyEntered) <> 0 Then 
                                                 ROUND(COALESCE((((CASE WHEN Sum(pl.QtyEntered) > 0 Then 
                                                                    COALESCE(SUM(ol.LineNetAmt),0) 
                                                                    WHEN Sum(pl.QtyEntered) < 0 Then 
                                                                    COALESCE(SUM(ol.LineNetAmt * -1),0) 
                                                                    ELSE 0  END) - Sum(
                                                                CASE WHEN ol.VA077_Duration is null THEN
                                                                    COALESCE(ol.VA077_PurchasePrice, 0) * pl.QtyEntered
                                                                    WHEN ol.VA077_Duration > 0 THEN
                                                                    COALESCE(ol.VA077_PurchasePrice, 0) * pl.QtyEntered * ol.VA077_Duration/12 
                                                                END)) / ((CASE WHEN Sum(pl.QtyEntered) > 0 Then 
                                                                            COALESCE(SUM(ol.LineNetAmt),0) 
                                                                            WHEN Sum(pl.QtyEntered) < 0 Then 
                                                                            COALESCE(SUM(ol.LineNetAmt * -1),0) 
                                                                            ELSE 0  END)) * 100), 0), 2) ELSE 0  END  
                            FROM M_InOutLine pl INNER JOIN C_OrderLine ol ON (pl.C_OrderLine_ID = ol.C_OrderLine_ID) WHERE pl.IsActive = 'Y' AND pl.M_InOut_ID = " + GetM_InOut_ID() + @"),
                            VA077_TotalSalesAmt=(SELECT CASE WHEN Sum(pl.QtyEntered) > 0 Then 
                                                                    COALESCE(SUM(ol.LineNetAmt),0) 
                                                                    WHEN Sum(pl.QtyEntered) < 0 Then 
                                                                    COALESCE(SUM(ol.LineNetAmt * -1),0) 
                                                                    ELSE 0  END
                                                FROM M_InOutLine pl INNER JOIN C_OrderLine ol 
                                                ON (pl.C_OrderLine_ID = ol.C_OrderLine_ID) 
                            WHERE pl.IsActive = 'Y' AND pl.M_InOut_ID = " + GetM_InOut_ID() + @"),
                            VA077_ChangeStartDate = (SELECT MIN(ol.VA077_StartDate) FROM M_InOutLine pl INNER JOIN C_OrderLine ol ON (pl.C_OrderLine_ID = ol.C_OrderLine_ID) 
                            WHERE pl.IsActive = 'Y' AND pl.M_InOut_ID = " + GetM_InOut_ID() + @"),
                            VA077_PartialAmtCatchUp =(SELECT CASE WHEN Sum(pl.QtyEntered) > 0 Then 
                                                                    COALESCE(SUM(ol.LineNetAmt),0) 
                                                                    WHEN Sum(pl.QtyEntered) < 0 Then 
                                                                    COALESCE(SUM(ol.LineNetAmt * -1),0) 
                                                                    ELSE 0  END 
                                                     FROM M_InOutLine pl INNER JOIN C_OrderLine ol 
                                                     ON (pl.C_OrderLine_ID = ol.C_OrderLine_ID) 
                            WHERE pl.IsActive = 'Y' AND ol.VA077_ContractProduct='Y' AND pl.M_InOut_ID = " + GetM_InOut_ID() + @"),
                            VA077_AdditionalAnnualCharge =(SELECT ROUND(COALESCE(SUM(ol.PriceEntered * pl.QtyEntered),0),2) FROM M_InOutLine pl 
                            INNER JOIN C_OrderLine ol ON (pl.C_OrderLine_ID = ol.C_OrderLine_ID)
                            WHERE pl.IsActive = 'Y' AND ol.VA077_ContractProduct='Y' AND pl.M_InOut_ID = " + GetM_InOut_ID() + @"),
                            VA077_NewAnnualContractTotal=(SELECT " + AnnualValue + @" + ROUND(COALESCE(SUM(ol.PriceEntered * pl.QtyEntered),0),2) FROM M_InOutLine pl INNER JOIN C_OrderLine ol ON (pl.C_OrderLine_ID = ol.C_OrderLine_ID) 
                            WHERE pl.IsActive = 'Y' AND ol.VA077_ContractProduct='Y' AND pl.M_InOut_ID = " + GetM_InOut_ID() + @"),
                            VA077_OrderRef = (SELECT VA077_OrderRef FROM C_Order WHERE C_Order_Id = (SELECT C_Order_Id FROM M_InOut 
                            WHERE M_InOut_ID=" + GetM_InOut_ID() + @"))");

                    qry.Append(" WHERE p.M_InOut_ID=" + GetM_InOut_ID() + "");
                }

                int no = DB.ExecuteQuery(qry.ToString(), null, Get_TrxName());
                if (no <= 0)
                {
                    return false;
                }
            }
            return true;
        }

        /**
         * 	Before Delete
         *	@return true if drafted
         */
        protected override bool BeforeDelete()
        {
            if (GetParent().GetDocStatus().Equals(MInOut.DOCSTATUS_Drafted))
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
            StringBuilder sb = new StringBuilder("MInOutLine[").Append(Get_ID())
                .Append(",M_Product_ID=").Append(GetM_Product_ID())
                .Append(",QtyEntered=").Append(GetQtyEntered())
                .Append(",MovementQty=").Append(GetMovementQty())
                .Append(",M_AttributeSetInstance_ID=").Append(GetM_AttributeSetInstance_ID())
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
            if (MLandedCost.LANDEDCOSTDISTRIBUTION_Costs.Equals(CostDistribution))
            {
                //	TODO Costs!
                log.Severe("Not Implemented yet - Cost");
                return Decimal.Multiply(GetMovementQty(), (GetPostCurrentCostPrice() != 0 ? GetPostCurrentCostPrice() : GetCurrentCostPrice()));
            }
            else if (MLandedCost.LANDEDCOSTDISTRIBUTION_Line.Equals(CostDistribution))
                return Env.ONE;
            else if (MLandedCost.LANDEDCOSTDISTRIBUTION_Quantity.Equals(CostDistribution))
                return GetMovementQty();
            else if (MLandedCost.LANDEDCOSTDISTRIBUTION_Volume.Equals(CostDistribution))
            {
                MProduct product = GetProduct();
                if (product == null)
                {
                    log.Severe("No Product");
                    return Env.ZERO;
                }
                return Decimal.Multiply(GetMovementQty(), (Decimal)product.GetVolume());
            }
            else if (MLandedCost.LANDEDCOSTDISTRIBUTION_Weight.Equals(CostDistribution))
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
