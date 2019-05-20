/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : TransactionXRef
 * Purpose        : Material Transaction Cross Reference
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           14-Jan-2010
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
    public class TransactionXRef:ProcessEngine.SvrProcess
    {
    private int		_Search_InOut_ID = 0;
	private int 	_Search_Order_ID = 0;
	private int		_Search_Invoice_ID = 0;
	
	/// <summary>
	/// Prepare - e.g., get Parameters.
	/// </summary>
	protected override void Prepare ()
	{
		ProcessInfoParameter[] para = GetParameter();
		for (int i = 0; i < para.Length; i++)
		{
			String name = para[i].GetParameterName();
			if (para[i].GetParameter() == null)
            {
				;
            }
			else if (name.Equals("Search_InOut_ID"))
            {
				_Search_InOut_ID = para[i].GetParameterAsInt();
            }
			else if (name.Equals("Search_Order_ID"))
            {
				_Search_Order_ID = para[i].GetParameterAsInt();
            }
			else if (name.Equals("Search_Invoice_ID"))
            {
				_Search_Invoice_ID = para[i].GetParameterAsInt();
            }
			else
            {
				log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
		}
	}	//	prepare

	/// <summary>
	/// Process it
	/// </summary>
	/// <returns>info</returns>
	protected override String DoIt() 
	{
		log.Info("M_InOut_ID=" + _Search_InOut_ID + ", C_Order_ID=" + _Search_Order_ID
			+ ", C_Invoice_ID=" + _Search_Invoice_ID);
		//
        if (_Search_InOut_ID != 0)
        {
            InsertTrx(
                "SELECT NVL(ma.M_AttributeSetInstance_ID,iol.M_AttributeSetInstance_ID) "
                + "FROM M_InOutLine iol"
                + " LEFT OUTER JOIN M_InOutLineMA ma ON (iol.M_InOutLine_ID=ma.M_InOutLine_ID) "
                + "WHERE M_InOut_ID=" + _Search_InOut_ID
                );
        }
        else if (_Search_Order_ID != 0)
        {
            InsertTrx(
                "SELECT NVL(ma.M_AttributeSetInstance_ID,iol.M_AttributeSetInstance_ID) "
                + "FROM M_InOutLine iol"
                + " LEFT OUTER JOIN M_InOutLineMA ma ON (iol.M_InOutLine_ID=ma.M_InOutLine_ID) "
                + " INNER JOIN M_InOut io ON (iol.M_InOut_ID=io.M_InOut_ID)"
                + "WHERE io.C_Order_ID=" + _Search_Order_ID
                );
        }
        else if (_Search_Invoice_ID != 0)
        {
            InsertTrx(
                "SELECT NVL(ma.M_AttributeSetInstance_ID,iol.M_AttributeSetInstance_ID) "
                + "FROM M_InOutLine iol"
                + " LEFT OUTER JOIN M_InOutLineMA ma ON (iol.M_InOutLine_ID=ma.M_InOutLine_ID) "
                + " INNER JOIN C_InvoiceLine il ON (iol.M_InOutLine_ID=il.M_InOutLine_ID) "
                + "WHERE il.C_Invoice_ID=" + _Search_Invoice_ID
                );
        }
        else
        {
            throw new Exception("Select one Parameter");
        }
		//
		return "";
	}	//	doIt
	
	/// <summary>
	/// Get Trx
	/// </summary>
	/// <param name="sqlSubSelect">sql</param>
	private void InsertTrx (String sqlSubSelect)
	{
		String sql = "INSERT INTO T_Transaction "
			+ "(AD_PInstance_ID, M_Transaction_ID,"
			+ " AD_Client_ID, AD_Org_ID, IsActive, Created,CreatedBy, Updated,UpdatedBy,"
			+ " MovementType, M_Locator_ID, M_Product_ID, M_AttributeSetInstance_ID,"
			+ " MovementDate, MovementQty,"
			+ " M_InOutLine_ID, M_InOut_ID,"
			+ " M_MovementLine_ID, M_Movement_ID,"
			+ " M_InventoryLine_ID, M_Inventory_ID, "
			+ " C_ProjectIssue_ID, C_Project_ID, "
			+ " M_ProductionLine_ID, M_Production_ID ";

        if (_Search_Order_ID != 0)
        {
            sql += ", Search_Order_ID ";
        }
        if (_Search_Invoice_ID != 0)
        {
            sql += ", Search_Invoice_ID ";
        }
        if (_Search_InOut_ID != 0)
        {
            sql += ", Search_InOut_ID ";
        }
		
		
			
			
			/*+ " Search_Order_ID, Search_Invoice_ID, Search_InOut_ID) "*/
			//	Data
		sql +=	 ") SELECT " + GetAD_PInstance_ID() + ", M_Transaction_ID,"
			+ " AD_Client_ID, AD_Org_ID, IsActive, Created,CreatedBy, Updated,UpdatedBy,"
			+ " MovementType, M_Locator_ID, M_Product_ID, M_AttributeSetInstance_ID,"
			+ " MovementDate, MovementQty,"
			+ " M_InOutLine_ID, M_InOut_ID, "
			+ " M_MovementLine_ID, M_Movement_ID,"
			+ " M_InventoryLine_ID, M_Inventory_ID, "
			+ " C_ProjectIssue_ID, C_Project_ID, "
			+ " M_ProductionLine_ID, M_Production_ID ";
		
			//	Parameter
        if (_Search_Order_ID != 0)
        {
            sql += ", " + _Search_Order_ID;
        }
        if (_Search_Invoice_ID != 0)
        {
            sql += ", " + _Search_Invoice_ID;
        }
        if (_Search_InOut_ID != 0)
        {
            sql += ", " + _Search_InOut_ID;
        }

			//+ _Search_Order_ID + ", " + _Search_Invoice_ID + "," + _Search_InOut_ID + " "
			//
		sql += " FROM M_Transaction_v "
			+ "WHERE M_AttributeSetInstance_ID > 0 AND M_AttributeSetInstance_ID IN (" 
			+ sqlSubSelect
			+ ") ORDER BY M_Transaction_ID";
		//
		int no = DataBase.DB.ExecuteQuery(sql,null, Get_Trx());
		log.Fine(sql);
		log.Config("#" + no);
		
		//	Multi-Level
		
	}	//	insertTrx
	
}	//	TransactionXRef

}
