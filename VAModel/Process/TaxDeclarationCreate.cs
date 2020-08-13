/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : TaxDeclarationCreate
 * Purpose        : Create Tax Declaration
 * Class Used     : ProcessEngine.SvrProcess class
 * Chronological    Development
 * Deepak           20-Nov-2009
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
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Data.SqlClient;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public  class TaxDeclarationCreate:ProcessEngine.SvrProcess
    {
        /**	Tax Declaration			*/
	private int 				_C_TaxDeclaration_ID = 0;
	/** Delete Old Lines		*/
	private Boolean				_DeleteOld = true;
	
	/**	Tax Declaration			*/
	private MTaxDeclaration 	_td = null;
	/** TDLines					*/
	private int					_noLines = 0;
	/** TDAccts					*/
	private int					_noAccts = 0;
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
			else if (name.Equals("DeleteOld"))
            {
				_DeleteOld = "Y".Equals(para[i].GetParameter());
            }
			else
            {
				log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
		}
		_C_TaxDeclaration_ID = GetRecord_ID();
	}	//	prepare

	
	/// <summary>
	/// Process
	/// </summary>
	/// <returns>info</returns>
	protected override String DoIt() 
	{
		log.Info("C_TaxDeclaration_ID=" + _C_TaxDeclaration_ID);
		_td = new MTaxDeclaration (GetCtx(), _C_TaxDeclaration_ID, Get_Trx());
		if (_td.Get_ID() == 0)
        {
			throw new Exception("@NotDound@ @C_TaxDeclaration_ID@ = " + _C_TaxDeclaration_ID);
        }
		
		if (_DeleteOld)
		{
			//	Delete old
            SqlParameter[] Param = new SqlParameter[1];
           
			String sql = "DELETE FROM C_TaxDeclarationLine WHERE C_TaxDeclaration_ID=@Param1";
            Param[0] = new SqlParameter("@Param1", _C_TaxDeclaration_ID);
			//int no = DataBase.executeUpdate(sql, _C_TaxDeclaration_ID, false, Get_Trx());
            int no = DataBase.DB.ExecuteQuery(sql, Param, Get_Trx());
            if (no != 0)
            {
                log.Config("Delete Line #" + no);
            }
			sql = "DELETE FROM C_TaxDeclarationAcct WHERE C_TaxDeclaration_ID=@Param1";
            Param[0] = new SqlParameter("@Param1", _C_TaxDeclaration_ID);
			//no = DataBase.executeUpdate(sql, _C_TaxDeclaration_ID, false, Get_Trx());
            int no1 = DataBase.DB.ExecuteQuery(sql, Param, Get_Trx());
            if (no1 != 0)
            {
                log.Config("Delete Acct #" + no1);
            }
		}

		//	Get Invoices
		 String  sql1 = "SELECT * FROM C_Invoice i "
			+ "WHERE TRUNC(i.DateInvoiced,'DD') >=@Param1 AND TRUNC(i.DateInvoiced,'DD') <=@Param2 "
			+ " AND Processed='Y'"
			+ " AND NOT EXISTS (SELECT * FROM C_TaxDeclarationLine tdl "
				+ "WHERE i.C_Invoice_ID=tdl.C_Invoice_ID)";
		//PreparedStatement pstmt = null;
        SqlParameter[] Param1=new SqlParameter[2];
        IDataReader idr=null;
        DataTable dt=null;
		int noInvoices = 0;
		try
		{
			//pstmt = DataBase.prepareStatement (sql, Get_Trx());
            Param1[0]=new SqlParameter("@Param1",_td.GetDateFrom());
			//pstmt.setTimestamp(1, _td.getDateFrom());
            Param1[1]=new SqlParameter("@Param2", _td.GetDateTo());
			//pstmt.setTimestamp(2, _td.getDateTo());
			//ResultSet rs = pstmt.executeQuery ();
            idr=DataBase.DB.ExecuteReader(sql1,Param1,Get_Trx());
            dt=new DataTable();
            dt.Load(idr);
            idr.Close();
			//while (rs.next ())
            foreach(DataRow dr in dt.Rows)
			{
				Create (new MInvoice (GetCtx(), dr, null));	//	no lock
				noInvoices++;
			}
		}
		catch (Exception e)
		{
            if (idr != null)
            {
                idr.Close();
            }
			log.Log (Level.SEVERE, sql1, e);
		}
		finally
        {
            if (idr != null)
            {
                idr.Close();
            }
            dt=null;
        
        }
		
		return "@C_Invoice_ID@ #" + noInvoices 
			+ " (" + _noLines + ", " + _noAccts + ")";
	}	//	doIt
	
	/// <summary>
	/// Create Data
	/// </summary>
	/// <param name="invoice">invoice</param>
	private void Create (MInvoice invoice)
	{
		/**	Lines					**
		MInvoiceLine[] lines = invoice.getLines();
		for (int i = 0; i < lines.length; i++)
		{
			MInvoiceLine line = lines[i];
			if (line.isDescription())
				continue;
			//
			MTaxDeclarationLine tdl = new MTaxDeclarationLine (_td, invoice, line);
			tdl.setLine((_noLines+1) * 10);
			if (tdl.save())
				_noLines++;
		}
		/** **/

		/** Invoice Tax				**/
		MInvoiceTax[] taxes = invoice.GetTaxes(false);
		for (int i = 0; i < taxes.Length; i++)
		{
			MInvoiceTax tLine = taxes[i];
			//
			MTaxDeclarationLine tdl = new MTaxDeclarationLine (_td, invoice, tLine);
			tdl.SetLine((_noLines+1) * 10);
			if (tdl.Save())
            {
				_noLines++;
            }
		}
		/** **/

		/**	Acct					**/
		String sql = "SELECT * FROM Fact_Acct WHERE AD_Table_ID=@Param1 AND Record_ID=@Param2";
        SqlParameter[] Param=new SqlParameter[2];
        IDataReader idr=null;
        DataTable dt=null;
		//PreparedStatement pstmt = null;
		try
		{
			//pstmt = DataBase.prepareStatement (sql, null);

			//pstmt.setInt (1, MInvoice.Table_ID);
            Param[0]=new SqlParameter("@Param1",MInvoice.Table_ID);
			//pstmt.setInt (2, invoice.getC_Invoice_ID());
            Param[1]=new SqlParameter("@Param2",invoice.GetC_Invoice_ID());
			//ResultSet rs = pstmt.executeQuery ();
            idr=DataBase.DB.ExecuteReader(sql,Param,null);
            dt=new DataTable();
            dt.Load(idr);
            idr.Close();
			//while (rs.next ())
            foreach(DataRow dr in dt.Rows)
			{
				MFactAcct fact = new MFactAcct(GetCtx(), dr, null);	//	no lock
				MTaxDeclarationAcct tda = new MTaxDeclarationAcct (_td, fact);
				tda.SetLine((_noAccts+1) * 10);
				if (tda.Save())
                {
					_noAccts++;
                }
			}
			
		}
		catch (Exception e)
		{
            if (idr != null)
            {
                idr.Close();
            }
			log.Log (Level.SEVERE, sql, e);
		}
		finally
        {
            if (idr != null)
            {
                idr.Close();
            }
            dt=null;
        }
		/** **/
	}	//	invoice
	
}	//	TaxDeclarationCreate

}
