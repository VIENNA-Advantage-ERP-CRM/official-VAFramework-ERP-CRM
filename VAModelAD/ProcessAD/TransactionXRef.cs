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
		log.Info("VAM_Inv_InOut_ID=" + _Search_InOut_ID + ", VAB_Order_ID=" + _Search_Order_ID
			+ ", VAB_Invoice_ID=" + _Search_Invoice_ID);
		//
        if (_Search_InOut_ID != 0)
        {
            InsertTrx(
                "SELECT NVL(ma.VAM_PFeature_SetInstance_ID,iol.VAM_PFeature_SetInstance_ID) "
                + "FROM VAM_Inv_InOutLine iol"
                + " LEFT OUTER JOIN VAM_Inv_InOutLineMP ma ON (iol.VAM_Inv_InOutLine_ID=ma.VAM_Inv_InOutLine_ID) "
                + "WHERE VAM_Inv_InOut_ID=" + _Search_InOut_ID
                );
        }
        else if (_Search_Order_ID != 0)
        {
            InsertTrx(
                "SELECT NVL(ma.VAM_PFeature_SetInstance_ID,iol.VAM_PFeature_SetInstance_ID) "
                + "FROM VAM_Inv_InOutLine iol"
                + " LEFT OUTER JOIN VAM_Inv_InOutLineMP ma ON (iol.VAM_Inv_InOutLine_ID=ma.VAM_Inv_InOutLine_ID) "
                + " INNER JOIN VAM_Inv_InOut io ON (iol.VAM_Inv_InOut_ID=io.VAM_Inv_InOut_ID)"
                + "WHERE io.VAB_Order_ID=" + _Search_Order_ID
                );
        }
        else if (_Search_Invoice_ID != 0)
        {
            InsertTrx(
                "SELECT NVL(ma.VAM_PFeature_SetInstance_ID,iol.VAM_PFeature_SetInstance_ID) "
                + "FROM VAM_Inv_InOutLine iol"
                + " LEFT OUTER JOIN VAM_Inv_InOutLineMP ma ON (iol.VAM_Inv_InOutLine_ID=ma.VAM_Inv_InOutLine_ID) "
                + " INNER JOIN VAB_InvoiceLine il ON (iol.VAM_Inv_InOutLine_ID=il.VAM_Inv_InOutLine_ID) "
                + "WHERE il.VAB_Invoice_ID=" + _Search_Invoice_ID
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
		String sql = "INSERT INTO VAT_Transaction "
			+ "(VAF_JInstance_ID, VAM_Inv_Trx_ID,"
			+ " VAF_Client_ID, VAF_Org_ID, IsActive, Created,CreatedBy, Updated,UpdatedBy,"
			+ " MovementType, VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,"
			+ " MovementDate, MovementQty,"
			+ " VAM_Inv_InOutLine_ID, VAM_Inv_InOut_ID,"
			+ " VAM_InvTrf_Line_ID, VAM_InventoryTransfer_ID,"
			+ " VAM_InventoryLine_ID, VAM_Inventory_ID, "
			+ " VAB_ProjectSupply_ID, VAB_Project_ID, "
			+ " VAM_ProductionLine_ID, VAM_Production_ID ";

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
		sql +=	 ") SELECT " + GetVAF_JInstance_ID() + ", VAM_Inv_Trx_ID,"
			+ " VAF_Client_ID, VAF_Org_ID, IsActive, Created,CreatedBy, Updated,UpdatedBy,"
			+ " MovementType, VAM_Locator_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,"
			+ " MovementDate, MovementQty,"
			+ " VAM_Inv_InOutLine_ID, VAM_Inv_InOut_ID, "
			+ " VAM_InvTrf_Line_ID, VAM_InventoryTransfer_ID,"
			+ " VAM_InventoryLine_ID, VAM_Inventory_ID, "
			+ " VAB_ProjectSupply_ID, VAB_Project_ID, "
			+ " VAM_ProductionLine_ID, VAM_Production_ID ";
		
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
		sql += " FROM VAM_Inv_Trx_v "
			+ "WHERE VAM_PFeature_SetInstance_ID > 0 AND VAM_PFeature_SetInstance_ID IN (" 
			+ sqlSubSelect
			+ ") ";
        //Code changes by Anuj (behalf of Kanchan Rana)
         if (_Search_InOut_ID != 0)
        {
            sql += "AND VAM_Inv_InOut_ID=" + _Search_InOut_ID;
        }

         sql += " ORDER BY VAM_Inv_Trx_ID";
		// -------------code done------------

		int no = DataBase.DB.ExecuteQuery(sql,null, Get_Trx());
		log.Fine(sql);
		log.Config("#" + no);
		
		//	Multi-Level
		
	}	//	insertTrx
	
}	//	TransactionXRef

}
