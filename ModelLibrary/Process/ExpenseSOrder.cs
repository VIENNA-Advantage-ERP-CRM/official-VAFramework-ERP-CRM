/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ExpenseSOrder
 * Purpose        : Create Sales Orders from Expense Reports
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           2-Jan-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class ExpenseSOrder : ProcessEngine.SvrProcess
    {
    /**	 BPartner				*/
	private int			_C_BPartner_ID = 0;
	/** Date Drom				*/
	private DateTime?	_DateFrom = null;
	/** Date To					*/
	private DateTime?	_DateTo = null;

	/** No SO generated			*/
	private int			_noOrders = 0;
	/**	Current Order			*/
	private MOrder		_order = null;
	
	
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
			else if (name.Equals("C_BPartner_ID"))
            {
				_C_BPartner_ID = para[i].GetParameterAsInt();
            }
			else if (name.Equals("DateExpense"))
			{
				_DateFrom = (DateTime?)para[i].GetParameter();
				_DateTo = (DateTime?)para[i].GetParameter_To();
			}
			else
				log.Log(Level.SEVERE, "Unknown Parameter: " + name);
		}
	}	//	prepare


	/// <summary>
	/// Perform Process.
	/// </summary>
	/// <returns>Message to be translated</returns>
	protected override String DoIt() 
    {
        int index = 1;
		StringBuilder sql = new StringBuilder("SELECT * FROM S_TimeExpenseLine el "
			+ "WHERE el.AD_Client_ID=@param1"						//	#1
			+ " AND el.C_BPartner_ID>0 AND el.IsInvoiced='Y'"	//	Business Partner && to be invoiced
			+ " AND el.C_OrderLine_ID IS NULL"					//	not invoiced yet
            + " AND EXISTS (SELECT * FROM S_TimeExpense e "		//	processed only
				+ "WHERE el.S_TimeExpense_ID=e.S_TimeExpense_ID AND e.Processed='Y')");
        if (_C_BPartner_ID != 0 && _C_BPartner_ID != -1)
        {
            index++;
            sql.Append(" AND el.C_BPartner_ID=@param2");			//	#2
        }
		if (_DateFrom != null || _DateTo != null)
		{

            sql.Append(" AND EXISTS (SELECT * FROM S_TimeExpense e "
				+ "WHERE el.S_TimeExpense_ID=e.S_TimeExpense_ID");
            if (_DateFrom != null)
            {
                index++;
                sql.Append(" AND e.DateReport >= @param3");		//	#3
            }
            if (_DateTo != null)
            {
                index++;
                sql.Append(" AND e.DateReport <= @param4");		//	#4
            }

			sql.Append(")");
		}
		sql.Append(" ORDER BY el.C_BPartner_ID, el.C_Project_ID, el.S_TimeExpense_ID, el.Line");

		//
		MBPartner oldBPartner = null;
		int old_Project_ID = -1;
		MTimeExpense te = null;
		//
        SqlParameter[] param = new SqlParameter[index];
        IDataReader idr = null;
        DataTable dt = null;
		try
		{
			//pstmt = DataBase.prepareStatement(sql.toString(), get_TrxName());
			int par = 0;
			//pstmt.setInt(par++, getAD_Client_ID());
            param[par] = new SqlParameter("@param1", GetAD_Client_ID());
            if (_C_BPartner_ID != 0 && _C_BPartner_ID != -1)
            {
                //pstmt.setInt(par++, _C_BPartner_ID);
                par++;
                param[par] = new SqlParameter("@param2", _C_BPartner_ID);
            }
            if (_DateFrom != null)
            {
                par++;
                //pstmt.setTimestamp(par++, _DateFrom);
                param[par] = new SqlParameter("@param3", _DateFrom);
            }
            if (_DateTo != null)
            {
                //pstmt.setTimestamp(par++, _DateTo);
                par++;
                param[par] = new SqlParameter("@param4", _DateTo);
            }
			//ResultSet rs = pstmt.executeQuery();
            idr = DataBase.DB.ExecuteReader(sql.ToString(), param, Get_TrxName());
            dt = new DataTable();
            dt.Load(idr);
            idr.Close();
			foreach(DataRow dr in dt.Rows)				//	********* Expense Line Loop
			{
				MTimeExpenseLine tel = new MTimeExpenseLine(GetCtx(),dr, Get_TrxName());
                if (!tel.IsInvoiced())
                {
                    continue;
                }
				
				//	New BPartner - New Order
				if (oldBPartner == null 
					|| oldBPartner.GetC_BPartner_ID() != tel.GetC_BPartner_ID())
				{
					CompleteOrder ();
					oldBPartner = new MBPartner (GetCtx(), tel.GetC_BPartner_ID(), Get_TrxName());
				}
				//	New Project - New Order
				if (old_Project_ID != tel.GetC_Project_ID())
				{
					CompleteOrder ();
					old_Project_ID = tel.GetC_Project_ID();
				}
				if (te == null || te.GetS_TimeExpense_ID() != tel.GetS_TimeExpense_ID())
					te = new MTimeExpense (GetCtx(), tel.GetS_TimeExpense_ID(), Get_TrxName());
				//
				ProcessLine (te, tel, oldBPartner);
			}	
            //	********* Expense Line Loop
            dt = null;
           // dt.Clear();
		}
		catch (Exception e)
		{
            if (dt != null)
            {
                dt = null;
            }
            if (idr != null)
            {
                idr.Close();
            }

			log.Log(Level.SEVERE, sql.ToString(), e);
		}
		finally
		{
            if (dt != null)
            {
                dt = null;
            }
            if (idr != null)
            {
                idr.Close();
            }
		}
		CompleteOrder ();

        return "" + _noOrders + "@Invoices Generated Successfully@";
	}	//	doIt

	/// <summary>
	/// Process Expense Line
	/// </summary>
	/// <param name="te">header</param>
	/// <param name="tel">line</param>
	/// <param name="bp">bp</param>
	private void ProcessLine (MTimeExpense te, MTimeExpenseLine tel, MBPartner bp)
	{
		if (_order == null)
		{
			log.Info("New Order for " + bp + ", Project=" + tel.GetC_Project_ID());
			_order = new MOrder (GetCtx(), 0, Get_TrxName());
			_order.SetAD_Org_ID(tel.GetAD_Org_ID());
            _order.SetC_DocTypeTarget_ID(MOrder.DocSubTypeSO_Standard);
			//
			_order.SetBPartner(bp);
			if (_order.GetC_BPartner_Location_ID() == 0)
			{ 
				log.Log(Level.SEVERE, "No BP Location: " + bp);
				AddLog(0, te.GetDateReport(), 
					null, "No Location: " + te.GetDocumentNo() + " " + bp.GetName());
				_order = null;
				return;
			}
			_order.SetM_Warehouse_ID(te.GetM_Warehouse_ID());

            ////Added By Arpit asked by Surya Sir..................29-12-2015
            //_order.SetSalesRep_ID(GetCtx().GetAD_User_ID());
            //End
            if (tel.GetC_Activity_ID() != 0)
            {
                _order.SetC_Activity_ID(tel.GetC_Activity_ID());
            }
            if (tel.GetC_Campaign_ID() != 0)
            {
                _order.SetC_Campaign_ID(tel.GetC_Campaign_ID());
            }
			if (tel.GetC_Project_ID() != 0)
			{
				_order.SetC_Project_ID(tel.GetC_Project_ID());
				//	Optionally Overwrite BP Price list from Project
				MProject project = new MProject (GetCtx(), tel.GetC_Project_ID(), Get_TrxName());
				if (project.GetM_PriceList_ID() != 0)
					_order.SetM_PriceList_ID(project.GetM_PriceList_ID());
			}
			_order.SetSalesRep_ID(te.GetDoc_User_ID());
			//
			if (!_order.Save())
			{
				throw new Exception("Cannot save Order");
			}
		}
		else
		{
			//	Update Header info
            if (tel.GetC_Activity_ID() != 0 && tel.GetC_Activity_ID() != _order.GetC_Activity_ID())
            {
                _order.SetC_Activity_ID(tel.GetC_Activity_ID());
            }
            if (tel.GetC_Campaign_ID() != 0 && tel.GetC_Campaign_ID() != _order.GetC_Campaign_ID())
            {
                _order.SetC_Campaign_ID(tel.GetC_Campaign_ID());
            }
            if (!_order.Save())
            {
                new Exception("Cannot save Order");
            }
		}
		
		//	OrderLine
		MOrderLine ol = new MOrderLine (_order);
		//
        if (tel.GetM_Product_ID() != 0)
        {
            ol.SetM_Product_ID(tel.GetM_Product_ID(),
                tel.GetC_UOM_ID());
        }
        if (tel.GetS_ResourceAssignment_ID() != 0)
        {
            ol.SetS_ResourceAssignment_ID(tel.GetS_ResourceAssignment_ID());
        }
        // Set charge ID
        if (tel.GetC_Charge_ID() != 0)
        {
            ol.SetC_Charge_ID(tel.GetC_Charge_ID());
            ol.SetPriceActual(tel.GetExpenseAmt());
            ol.SetQty(tel.GetQty());
        }
		ol.SetQty(tel.GetQtyInvoiced());		//	
		ol.SetDescription(tel.GetDescription());
		//
		ol.SetC_Project_ID(tel.GetC_Project_ID());
		ol.SetC_ProjectPhase_ID(tel.GetC_ProjectPhase_ID());
		ol.SetC_ProjectTask_ID(tel.GetC_ProjectTask_ID());
		ol.SetC_Activity_ID(tel.GetC_Activity_ID());
		ol.SetC_Campaign_ID(tel.GetC_Campaign_ID());
		//
		Decimal price = tel.GetPriceInvoiced();	//	
        if ( price.CompareTo(Env.ZERO) != 0)
        {
            if (tel.GetC_Currency_ID() != _order.GetC_Currency_ID())
                price = MConversionRate.Convert(GetCtx(), price,
                    tel.GetC_Currency_ID(), _order.GetC_Currency_ID(),
                    _order.GetAD_Client_ID(), _order.GetAD_Org_ID());
            ol.SetPrice(price);
        }
        else
        {
            ol.SetPrice();
        }
        if (tel.GetC_UOM_ID() != 0 && ol.GetC_UOM_ID() == 0)
        {
            ol.SetC_UOM_ID(tel.GetC_UOM_ID());
        }
		ol.SetTax();
		if (!ol.Save())
		{
			throw new Exception("Cannot save Order Line");
		}
		//	Update TimeExpense Line
		tel.SetC_OrderLine_ID(ol.GetC_OrderLine_ID());
        if (tel.Save())
        {
            log.Fine("Updated " + tel + " with C_OrderLine_ID");
        }
        else
        {
            log.Log(Level.SEVERE, "Not Updated " + tel + " with C_OrderLine_ID");
        }
			
	}	//	processLine
	
	/// <summary>
    /// Complete Order
    /// </summary>
	private void CompleteOrder ()
	{
        if (_order == null)
        {
            return;
        }
		_order.SetDocAction(DocActionVariables.ACTION_PREPARE);
		_order.ProcessIt(DocActionVariables.ACTION_PREPARE);
        if (!_order.Save())
        {
            throw new Exception("Cannot save Order");
        }
		_noOrders++;
		AddLog(_order.Get_ID(), _order.GetDateOrdered(), _order.GetGrandTotal(), _order.GetDocumentNo());
		_order = null;
	}	//	completeOrder

}	//	ExpenseSOrder

}
