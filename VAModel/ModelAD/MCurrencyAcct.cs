/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MCurrencyAcct
 * Purpose        : Currency Account Model 
 * Class Used     : X_C_Currency_Acct
 * Chronological    Development
 * Deepak           23-Nov-2009
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

namespace VAdvantage.Model
{
    public class MCurrencyAcct : X_C_Currency_Acct
    {
    /** Static Logger					*/
	private static VLogger _log = VLogger.GetVLogger(typeof(MCurrencyAcct).FullName);
	
	/// <summary>
	/// Get Currency Account for Currency
	/// </summary>
	/// <param name="a"> accounting schema default</param>
	/// <param name="C_Currency_ID">currency</param>
    /// <returns>Currency Account or null</returns>
	public static MCurrencyAcct Get(MAcctSchemaDefault a, int C_Currency_ID)
	{
		MCurrencyAcct retValue = null;
		String sql = "SELECT * FROM C_Currency_Acct "
			+ "WHERE C_AcctSchema_ID=@Param1 AND C_Currency_ID=@Param2";

        SqlParameter[] Param=new SqlParameter[2];
        IDataReader idr=null;
        DataTable dt=null;
		//PreparedStatement pstmt = null;
        try
        {
            Param[0] = new SqlParameter("@Param1", a.GetC_AcctSchema_ID());
            //pstmt = DataBase.prepareStatement(sql, null);
            //pstmt.setInt(1, as.getC_AcctSchema_ID());
            //pstmt.setInt(2, C_Currency_ID);
            Param[1] = new SqlParameter("@Param1", a.GetC_AcctSchema_ID());
            //ResultSet rs = pstmt.executeQuery();
            idr = DataBase.DB.ExecuteReader(sql, Param, null);
            dt = new DataTable();
            dt.Load(idr);
            idr.Close();
            foreach (DataRow dr in dt.Rows)
            {
                retValue = new MCurrencyAcct(a.GetCtx(), dr, null);
            }

        }
        catch (Exception e)
        {
            if (idr != null)
            {
                idr.Close();
            }
            _log.Log(Level.SEVERE, "get", e);
        }
        finally
        {
            dt = null;
        }
		return retValue;
	}	//	get
	
	
	/// <summary>
	/// Load Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="dr">datarow</param>
    /// <param name="trxName">transaction</param>
	public MCurrencyAcct(Ctx ctx, DataRow dr, Trx trxName):base(ctx, dr, trxName)
	{
		//super(ctx, rs, trxName);
	}	//	MCurrencyAcct

}	//	MCurrencyAcct

}
