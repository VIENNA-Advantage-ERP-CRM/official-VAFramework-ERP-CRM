/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MCurrencyAcct
 * Purpose        : Currency Account Model 
 * Class Used     : X_VAB_Currency_Acct
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
    public class MVABCurrencyAcct : X_VAB_Currency_Acct
    {
    /** Static Logger					*/
	private static VLogger _log = VLogger.GetVLogger(typeof(MVABCurrencyAcct).FullName);
	
	/// <summary>
	/// Get Currency Account for Currency
	/// </summary>
	/// <param name="a"> accounting schema default</param>
	/// <param name="VAB_Currency_ID">currency</param>
    /// <returns>Currency Account or null</returns>
	public static MVABCurrencyAcct Get(MVABAccountBookDefault a, int VAB_Currency_ID)
	{
		MVABCurrencyAcct retValue = null;
		String sql = "SELECT * FROM VAB_Currency_Acct "
			+ "WHERE VAB_AccountBook_ID=@Param1 AND VAB_Currency_ID=@Param2";

        SqlParameter[] Param=new SqlParameter[2];
        IDataReader idr=null;
        DataTable dt=null;
		//PreparedStatement pstmt = null;
        try
        {
            Param[0] = new SqlParameter("@Param1", a.GetVAB_AccountBook_ID());
            //pstmt = DataBase.prepareStatement(sql, null);
            //pstmt.setInt(1, as.getVAB_AccountBook_ID());
            //pstmt.setInt(2, VAB_Currency_ID);
            Param[1] = new SqlParameter("@Param1", a.GetVAB_AccountBook_ID());
            //ResultSet rs = pstmt.executeQuery();
            idr = CoreLibrary.DataBase.DB.ExecuteReader(sql, Param, null);
            dt = new DataTable();
            dt.Load(idr);
            idr.Close();
            foreach (DataRow dr in dt.Rows)
            {
                retValue = new MVABCurrencyAcct(a.GetCtx(), dr, null);
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
	public MVABCurrencyAcct(Ctx ctx, DataRow dr, Trx trxName):base(ctx, dr, trxName)
	{
		//super(ctx, rs, trxName);
	}	//	MCurrencyAcct

}	//	MCurrencyAcct

}
