/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAd
 * Purpose        : Container Model
 * Class Used     : X_CM_Ad
 * Chronological    Development
 * Deepak           05-Feb-2010
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


namespace VAdvantage.Model
{
   public class MAd: X_CM_Ad
{
	/// <summary>
	/// Standard constructor for AD
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="CM_Ad_ID">id</param>
	/// <param name="trxName">trx</param>
	public MAd (Ctx ctx, int CM_Ad_ID, Trx trxName):base(ctx, CM_Ad_ID, trxName)
	{
		
	}
	
	/// <summary>
	/// Standard constructor for AD
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="rs">datarow</param>
	/// <param name="trxName">trx</param>
	public MAd (Ctx ctx,DataRow dr, Trx trxName):base(ctx,dr, trxName)
	{
	
	}
    public MAd (Ctx ctx,IDataReader idr, Trx trxName):base(ctx,idr, trxName)
	{
	
	}
	/// <summary>
	/// Get's the relevant current Impression value which is Actual+Start
	/// </summary>
	/// <returns>int</returns>
	public int GetCurrentImpression() 
    {
		return GetActualImpression() + GetStartImpression();
	}
	
	/// <summary>
	/// Adds an Impression to the current Ad
	 // We will deactivate the Ad as soon as one of the Max Criterias are fullfiled
	/// </summary>
	public void AddImpression()
    {
		SetActualImpression(GetActualImpression()+1);
		if (GetMaxImpression()>0 && GetCurrentImpression()>=GetMaxImpression()) 
			SetIsActive(false);
		Save();
	}
	
	/**	Logger			*/
	private static VLogger _log = VLogger.GetVLogger(typeof(MAd).FullName);//.class);

	/// <summary>
    /// Get Next of this Category, this Procedure will return the next Ad in a category and expire it if needed
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="CM_Ad_Cat_ID">id</param>
	/// <param name="trxName">trx</param>
	/// <returns>MAD</returns>
	public static MAd GetNext(Ctx ctx, int CM_Ad_Cat_ID, Trx trxName) 
	{
		MAd thisAd = null;
		String sql = "SELECT * FROM CM_Ad WHERE IsActive='Y' AND (ActualImpression+StartImpression<MaxImpression OR MaxImpression=0) AND CM_Ad_Cat_ID=@param ORDER BY ActualImpression+StartImpression";
        SqlParameter[] param = new SqlParameter[1];
        IDataReader idr = null;
		try
		{
			//pstmt = DataBase.prepareStatement(sql, trxName);
			//pstmt.setInt(1, CM_Ad_Cat_ID);
            param[0] = new SqlParameter("@param", CM_Ad_Cat_ID);
            idr = DataBase.DB.ExecuteReader(sql, param, trxName);
            if (idr.Read())
            {
                thisAd = new MAd(ctx, idr, trxName);
            }
            idr.Close();
		}
		catch (Exception e)
		{
            if (idr != null)
            {
                idr.Close();
            }
			_log.Log(Level.SEVERE, sql, e);
		}

        if (thisAd != null)
        {
            thisAd.AddImpression();
        }
		return thisAd;
	}
	
	/// <summary>
	///Add Click Record to Log
	/// </summary>
	/// <param name="request">ServletReqeust</param>
    //public void AddClick(HttpServletRequest request) {
    //    SetActualClick(GetActualClick()+1);
    //    if (GetActualClick()>GetMaxClick()) 
    //        SetIsActive(true);
    //    save();
    //}
} // MAd

}
