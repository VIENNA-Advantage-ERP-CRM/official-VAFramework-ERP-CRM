/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MContainerElement
 * Purpose        : CStage Element
 * Class Used     : X_CM_Container_Element
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
    public class MContainerElement: X_CM_Container_Element
{
	/**	serialVersionUID	*/
	//private static long serialVersionUID = 7230036377422361941L;

	/** Logger */
	private static VLogger _log = VLogger.GetVLogger (typeof(MContainer).FullName);//.class);

	/// <summary>
    /// 	get Container Element by ID
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="CM_ContainerElement_ID">id</param>
	/// <param name="trxName">trx</param>
	/// <returns>container element</returns>
	public static MContainerElement Get(Ctx ctx, int CM_ContainerElement_ID, Trx trxName) 
    {
		MContainerElement thisContainerElement = null;
		String sql = "SELECT * FROM CM_Container_Element WHERE CM_Container_Element_ID=@param";
        SqlParameter[] param = new SqlParameter[1];
        IDataReader idr = null;
		try
		{
			//pstmt = DataBase.prepareStatement(sql, trxName);
			//pstmt.setInt(1, CM_ContainerElement_ID);
            param[0] = new SqlParameter("@param", CM_ContainerElement_ID);
            idr = DataBase.DB.ExecuteReader(sql, param, trxName);
            if (idr.Read())
            {
                thisContainerElement = (new MContainerElement(ctx, idr, trxName));
            }
            idr.Read();
		}
		catch (Exception e)
		{
            if (idr != null)
            {
                idr.Close();
            }
			_log.Log(Level.SEVERE, sql, e);
		}
		
		return thisContainerElement;
	}

	/// <summary>
    ///  Standard Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="CM_Container_Element_ID">id</param>
	/// <param name="trxName">trx</param>
	public MContainerElement (Ctx ctx, int CM_Container_Element_ID, Trx trxName):base(ctx, CM_Container_Element_ID, trxName)
    {
		
		if (CM_Container_Element_ID == 0)
		{
			SetIsValid(false);
		}
	}	// MContainerElement

	/// <summary>
    /// Load Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="rs">datarow</param>
	/// <param name="trxName">trx</param>
	public MContainerElement (Ctx ctx,DataRow dr, Trx trxName):base(ctx,dr, trxName)
	{
		
	}	// MContainerElement

    public MContainerElement(Ctx ctx, IDataReader idr, Trx trxName)
        : base(ctx, idr, trxName)
    { }

	/** Parent				*/
	private MContainer _parent = null;
	
	/// <summary>
    /// Get Container get's related Container
	/// </summary>
	/// <returns>MContainer</returns>
	public MContainer GetParent()
	{
		if (_parent == null)
			_parent = new MContainer (GetCtx(), GetCM_Container_ID(), Get_TrxName());
		return _parent;

		/** No reason to do this ?? - should never return null - always there - JJ
		int[] thisContainer = MContainer.getAllIDs("CM_Container","CM_Container_ID=" + this.getCM_Container_ID(), get_TrxName());
		if (thisContainer != null) 
		{
			if (thisContainer.length==1)
				return new MContainer(getCtx(), thisContainer[0], get_TrxName());
		}
		return null;
		**/
	}	//	getContainer
	
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
		ReIndex(newRecord);
		return success;
	}	//	afterSave
	
	/// <summary>
    /// reIndex
	/// </summary>
	/// <param name="newRecord">new record</param>
	public void ReIndex(bool newRecord)
	{
		if (GetParent().IsIndexed ()) 
        {
			int CMWebProjectID = 0;
			if (GetParent()!=null)
				CMWebProjectID = GetParent().GetCM_WebProject_ID();
			String [] toBeIndexed = new String[3];
			toBeIndexed[0] = this.GetName();
			toBeIndexed[1] = this.GetDescription();
			toBeIndexed[2] = this.GetContentHTML();
			MIndex.ReIndex (newRecord, toBeIndexed, GetCtx(), 
				GetAD_Client_ID(), Get_Table_ID(), Get_ID(), CMWebProjectID, this.GetUpdated());
		}
		if (!GetParent().IsIndexed () && !newRecord)
			MIndex.CleanUp(Get_TrxName(), GetAD_Client_ID(), Get_Table_ID(), Get_ID());
	}	// reIndex
	
}	//	MContainerElement

}
