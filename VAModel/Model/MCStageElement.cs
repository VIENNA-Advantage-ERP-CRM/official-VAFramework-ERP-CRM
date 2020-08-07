/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MCStageElement
 * Purpose        : Stage Element
 * Class Used     : X_CM_CStage_Element
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
public class MCStageElement :X_CM_CStage_Element
{
	/// <summary>
	///  Standard Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="CM_CStage_Element_ID">id</param>
	/// <param name="trxName">trx</param>
	public MCStageElement(Ctx ctx, int CM_CStage_Element_ID, Trx trxName):base(ctx, CM_CStage_Element_ID, trxName)
	{
		
		if (CM_CStage_Element_ID == 0)
		{
			SetIsValid(false);
		}
	}	// MCStageElement

	/// <summary>
	/// Load constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="rs">datarow</param>
	/// <param name="trxName">trx</param>
	public MCStageElement (Ctx ctx, DataRow dr, Trx trxName):base(ctx,dr,trxName)
	{
		
	} 	// MCStageElement
    public MCStageElement (Ctx ctx,IDataReader idr, Trx trxName):base(ctx,idr,trxName)
    {}
	
	/** Logger								*/
	private static VLogger		_log = VLogger.GetVLogger(typeof(MCStageElement).FullName);//.class);
	
	/// <summary>
	/// get By ElementName
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="CM_CStage_ID">id</param>
	/// <param name="elementName">elementname</param>
	/// <param name="trxName">trx</param>
	/// <returns>element by name</returns>
	public static MCStageElement GetByName (Ctx ctx, int CM_CStage_ID, String elementName, Trx trxName)
	{
		String sql = "SELECT * FROM CM_CStage_Element WHERE CM_CStage_ID=@param1 AND Name LIKE @param2";
		MCStageElement thisElement = null;
        SqlParameter[] param = new SqlParameter[2];
        IDataReader idr = null;
		try
		{
			//pstmt = DataBase.prepareStatement (sql, trxName);
			//pstmt.setInt (1, CM_CStage_ID);
            param[0] = new SqlParameter("@param1", CM_CStage_ID);
			//pstmt.setString (2, elementName);
            param[1] = new SqlParameter("@param2", Utility.Util.GetValueOfString(elementName));
            idr = DataBase.DB.ExecuteReader(sql, param, trxName);
            if (idr.Read())
            {
                thisElement = new MCStageElement(ctx,idr, trxName);
            }
            idr.Close();
		}
		catch (Exception e)
		{
            if(idr!=null)
            {
                idr.Close();
            }
			_log.Log(Level.SEVERE, "getByName", e);
		}
	
		return thisElement;
	}	//	getByName
	
	/**	_parent					*/
	private MCStage _parent	= null;
	
	/// <summary>
	/// getParent MCStage Object
	/// </summary>
    /// <returns>Container Stage</returns>
	public MCStage GetParent() 
	{
		if (_parent!=null)
			return _parent;
		_parent = new MCStage(GetCtx(), GetCM_CStage_ID (), Get_TrxName ());
		return _parent;
	}
	
	/// <summary>
    /// After Save.
	/// </summary>
	/// <param name="newRecord">new</param>
	/// <param name="success">success</param>
	/// <returns>true if inserted</returns>
	protected override bool AfterSave (bool newRecord, bool success)
	{
		if (!success)
			return success;
		if (!GetParent().IsModified ()) 
		{
			GetParent().SetIsModified (true);
			GetParent().Save ();
		}
		return success;
	}	//	afterSave


}	//	MCStageElement

}
