/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : RequisitionPOCreate
 * Purpose        : Create PO from Requisition 
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak          04-Nov-2009
  ******************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;


using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class RequisitionPOCreate : ProcessEngine.SvrProcess
    {
        #region private variable
        // Org				
        private int _AD_Org_ID = 0;
        // Warehouse			
        private int _M_Warehouse_ID = 0;
        //	Doc Date From		
        private DateTime? _DateDoc_From;
        //	Doc Date To			
        private DateTime? _DateDoc_To;
        //	Doc Date From		
        private DateTime? _DateRequired_From;
        //	Doc Date To			
        private DateTime? _DateRequired_To;
        // Priority			
        private String _PriorityRule = null;
        // User				
        private int _AD_User_ID = 0;
        // Product				
        private int _M_Product_ID = 0;
        // Requisition			
        private int _M_Requisition_ID = 0;

        // Consolidate			
        private Boolean _ConsolidateDocument = false;

        // Order				
        private MOrder _order = null;
        // Order Line			
        private MOrderLine _orderLine = null;
        private string createdPO = null; 

        private int _m_M_Requisition_ID = 0;
        private int _m_M_Product_ID = 0;
        private int _m_M_AttributeSetInstance_ID = 0;
        // BPartner				
        private MBPartner _m_bpartner = null;
#endregion

        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("AD_Org_ID"))
                {
                    _AD_Org_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("M_Warehouse_ID"))
                {
                    _M_Warehouse_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("DateDoc"))
                {
                    _DateDoc_From = (DateTime?)para[i].GetParameter();
                    _DateDoc_To = (DateTime?)para[i].GetParameter_To();
                }
                else if (name.Equals("DateRequired"))
                {
                    _DateRequired_From = (DateTime?)para[i].GetParameter();
                    _DateRequired_To = (DateTime?)para[i].GetParameter_To();
                }
                else if (name.Equals("PriorityRule"))
                {
                    _PriorityRule = (String)para[i].GetParameter();
                }
                else if (name.Equals("AD_User_ID"))
                {
                    _AD_User_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("M_Product_ID"))
                {
                    _M_Product_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("M_Requisition_ID"))
                {
                    _M_Requisition_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("ConsolidateDocument"))
                {
                    _ConsolidateDocument = "Y".Equals(para[i].GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        ///  	Process
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            //	Specific
            if (_M_Requisition_ID != 0)
            {
                log.Info("M_Requisition_ID=" + _M_Requisition_ID);
                MRequisition req = new MRequisition(GetCtx(), _M_Requisition_ID, Get_TrxName());
                if (!MRequisition.DOCSTATUS_Completed.Equals(req.GetDocStatus()))
                {
                    throw new Exception("@DocStatus@ = " + req.GetDocStatus());
                }
                MRequisitionLine[] lines = req.GetLines();
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].GetC_OrderLine_ID() == 0)
                    {
                        Process(lines[i]);
                    }
                }
                CloseOrder();
                return Msg.GetMsg(GetCtx(), "SuccessfullyCreatedPONo. " + createdPO);
            }	//	single Requisition

            //	
            log.Info("AD_Org_ID=" + _AD_Org_ID
                + ", M_Warehouse_ID=" + _M_Warehouse_ID
                + ", DateDoc=" + _DateDoc_From + "/" + _DateDoc_To
                + ", DateRequired=" + _DateRequired_From + "/" + _DateRequired_To
                + ", PriorityRule=" + _PriorityRule
                + ", AD_User_ID=" + _AD_User_ID
                + ", M_Product_ID=" + _M_Product_ID
                + ", ConsolidateDocument" + _ConsolidateDocument);

            StringBuilder sql = new StringBuilder("SELECT * FROM M_RequisitionLine rl ")
                .Append("WHERE rl.C_OrderLine_ID IS NULL");
            if (_AD_Org_ID != 0)
            {
                sql.Append(" AND AD_Org_ID=" + _AD_Org_ID);
            }
            if (_M_Product_ID != 0)
            {
                sql.Append(" AND M_Product_ID=" + _M_Product_ID);
            }
            //	Requisition Header
            sql.Append(" AND EXISTS (SELECT * FROM M_Requisition r WHERE rl.M_Requisition_ID=r.M_Requisition_ID")
                .Append(" AND r.DocStatus='CO'");
            if (_M_Warehouse_ID != 0)
            {
                sql.Append(" AND r.M_Warehouse_ID=" + _M_Warehouse_ID);
            }
            //
            if (_DateDoc_From != null && _DateDoc_To != null)
            {
                sql.Append(" AND r.DateDoc BETWEEN " + _DateDoc_From + " AND " + _DateDoc_To);
            }
            else if (_DateDoc_From != null)
            {
                sql.Append(" AND r.DateDoc =>" + _DateDoc_From);
            }
            else if (_DateDoc_To != null)
            {
                sql.Append(" AND r.DateDoc <= " + _DateDoc_To);
            }
            //
            if (_DateRequired_From != null && _DateRequired_To != null)
            {
                sql.Append(" AND r.DateRequired BETWEEN " + _DateRequired_From + " AND " + _DateRequired_To);
            }
            else if (_DateRequired_From != null)
            {
                sql.Append(" AND r.DateRequired =>" + _DateRequired_From);
            }
            else if (_DateRequired_To != null)
            {
                sql.Append(" AND r.DateRequired <=" + _DateRequired_To);
            }
            //
            if (_PriorityRule != null)
            {
                sql.Append(" AND r.PriorityRule =>" + _PriorityRule);
            }
            if (_AD_User_ID != 0)
            {
                sql.Append(" AND r.AD_User_ID=" + _AD_User_ID);
            }
            //
            sql.Append(") ORDER BY ");


            if (!_ConsolidateDocument)
            {
                sql.Append("M_Requisition_ID, ");
            }
            sql.Append("M_Product_ID, C_Charge_ID, M_AttributeSetInstance_ID");

            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    Process(new MRequisitionLine(GetCtx(), dr, Get_TrxName()));
                }

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql.ToString(), e);
            }
            finally
            {
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }

            CloseOrder();
            return Msg.GetMsg(GetCtx(), "SuccessfullyCreatedPONo. " + createdPO);
        }

        /// <summary>
        ///	  	Process Line
        /// </summary>
        /// <param name="rLine">request line</param>
        private void Process(MRequisitionLine rLine)
        {
            if (rLine.GetM_Product_ID() == 0 && rLine.GetC_Charge_ID() == 0)
            {
                log.Warning("Ignored Line" + rLine.GetLine()
                    + " " + rLine.GetDescription()
                    + " - " + rLine.GetLineNetAmt());
                return;
            }

            if (!_ConsolidateDocument
                && rLine.GetM_Requisition_ID() != _m_M_Requisition_ID)
            {
                CloseOrder();
            }
            if (_orderLine == null
                || rLine.GetM_Product_ID() != _m_M_Product_ID
                || rLine.GetM_AttributeSetInstance_ID() != _m_M_AttributeSetInstance_ID
                || rLine.GetC_Charge_ID() != 0)		//	single line per charge
            {
                NewLine(rLine);
            }

            //	Update Order Line
            _orderLine.SetQty(Decimal.Add(_orderLine.GetQtyOrdered(), rLine.GetQty()));

            //	Update Requisition Line
            rLine.SetC_OrderLine_ID(_orderLine.GetC_OrderLine_ID());
            if (!rLine.Save())
            {
                throw new Exception("Cannot update Request Line");
            }
        }

        /// <summary>
        /// Create new Order
        /// </summary>
        /// <param name="rLine">request line</param>
        private void NewOrder(MRequisitionLine rLine, int C_BPartner_ID)
        {
            if (_order != null)
            {
                CloseOrder();
            }
            //	BPartner
            if (_m_bpartner == null || C_BPartner_ID != _m_bpartner.GetC_BPartner_ID())
            {
                _m_bpartner = new MBPartner(GetCtx(), C_BPartner_ID, null);
            }
            //	Order
            _order = new MOrder(GetCtx(), 0, Get_TrxName());
            _order.SetIsSOTrx(false);
            _order.SetC_DocTypeTarget_ID();
            _order.SetBPartner(_m_bpartner);
            //	default po document type
            if (!_ConsolidateDocument)
            {
                _order.SetDescription(Msg.GetElement(GetCtx(), "M_Requisition_ID")
                    + ": " + rLine.GetParent().GetDocumentNo());
            }
            //	Prepare Save
            _m_M_Requisition_ID = rLine.GetM_Requisition_ID();
            if (!_order.Save())
            {
                throw new Exception("Cannot save Order");
            }
            else
            {
                if (String.IsNullOrEmpty(createdPO))
                {
                    createdPO = _order.GetDocumentNo();
                }
                else
                {
                    createdPO += " , " + _order.GetDocumentNo();
                }
            }
        }	

        /// <summary>
        ///Close Order
        /// </summary>
        private void CloseOrder()
        {
            if (_orderLine != null)
            {
                if (!_orderLine.Save())
                {
                    throw new Exception("Cannot update Order Line");
                }
            }
            if (_order != null)
            {
                _order.Load(Get_TrxName());
                AddLog(0, null, _order.GetGrandTotal(), _order.GetDocumentNo());
            }
            _order = null;
            _orderLine = null;
        }

        /// <summary>
        /// New Order Line (different Product)
        /// </summary>
        /// <param name="rLine">request line</param>
        private void NewLine(MRequisitionLine rLine)
        {
            if (_orderLine != null)
            {
                if (!_orderLine.Save())
                {
                    throw new Exception("Cannot update Order Line");
                }
            }
            _orderLine = null;
            MProduct product = null;

            //	Get Business Partner
            int C_BPartner_ID = rLine.GetC_BPartner_ID();
            if (C_BPartner_ID != 0)
            {
                ;
            }
            else if (rLine.GetC_Charge_ID() != 0)
            {
                MCharge charge = MCharge.Get(GetCtx(), rLine.GetC_Charge_ID());
                C_BPartner_ID = charge.GetC_BPartner_ID();
                if (C_BPartner_ID == 0)
                {
                    throw new Exception("No Vendor for Charge " + charge.GetName());
                }
            }
            else
            {
                //	Find Vendor from Produt
                product = MProduct.Get(GetCtx(), rLine.GetM_Product_ID());
                MProductPO[] ppos = MProductPO.GetOfProduct(GetCtx(), product.GetM_Product_ID(), null);
                for (int i = 0; i < ppos.Length; i++)
                {
                    if (ppos[i].IsCurrentVendor() && ppos[i].GetC_BPartner_ID() != 0)
                    {
                        C_BPartner_ID = ppos[i].GetC_BPartner_ID();
                        break;
                    }
                }
                if (C_BPartner_ID == 0 && ppos.Length > 0)
                {
                    C_BPartner_ID = ppos[0].GetC_BPartner_ID();
                }
                if (C_BPartner_ID == 0)
                {
                    throw new Exception("No Vendor for " + product.GetName());
                }
            }

            //	New Order - Different Vendor
            if (_order == null
                || _order.GetC_BPartner_ID() != C_BPartner_ID)
            {
                NewOrder(rLine, C_BPartner_ID);
            }

            //	No Order Line
            _orderLine = new MOrderLine(_order);
            if (product != null)
            {
                _orderLine.SetProduct(product);
                _orderLine.SetM_AttributeSetInstance_ID(rLine.GetM_AttributeSetInstance_ID());
            }
            else
            {
                _orderLine.SetC_Charge_ID(rLine.GetC_Charge_ID());
                _orderLine.SetPriceActual(rLine.GetPriceActual());
            }
            _orderLine.SetAD_Org_ID(rLine.GetAD_Org_ID());


            //	Prepare Save
            _m_M_Product_ID = rLine.GetM_Product_ID();
            _m_M_AttributeSetInstance_ID = rLine.GetM_AttributeSetInstance_ID();
            if (!_orderLine.Save())
            {
                throw new Exception("Cannot save Order Line");
            }
        }	
    }
}
