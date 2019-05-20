/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MCStage
 * Purpose        : MCStage Model
 * Class Used     : X_CM_CStage
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
    public class MCStage: X_CM_CStage
{
	/// <summary>
	///	Get Stages
	/// </summary>
	/// <param name="project">project</param>
	/// <returns>stages</returns>
	public static MCStage[] GetStages (MWebProject project)
	{
		List<MCStage> list = new List<MCStage>();
		SqlParameter[] param=new SqlParameter[1];      
        IDataReader idr=null;
		String sql = "SELECT * FROM CM_CStage WHERE CM_WebProject_ID=@param ORDER BY CM_CStage_ID";
		try
		{
			//pstmt = DataBase.prepareStatement (sql, project.get_TrxName());
			//pstmt.setInt (1, project.getCM_WebProject_ID());
            param[0]=new SqlParameter("@param",project.GetCM_WebProject_ID());
			idr=DataBase.DB.ExecuteReader(sql,param,project.Get_TrxName());

			while (idr.Read())
			{
				list.Add (new MCStage (project.GetCtx(),idr, project.Get_TrxName()));
			}
			idr.Close();
		}
		catch (Exception e)
		{
            if(idr!=null)
            {
                idr.Close();
            }
			_log.Log (Level.SEVERE, sql, e);
		}
		
		MCStage[] retValue = new MCStage[list.Count];
		retValue=list.ToArray ();
		return retValue;
	}	//	getStages

	/// <summary>
	/// Get Stage by Name
	/// </summary>
	/// <param name="project">project</param>
	/// <param name="RelativeURL">relurl</param>
	/// <param name="parent_ID">id</param>
	/// <returns>stages</returns>
	public static MCStage GetByName (MWebProject project, String RelativeURL, int parent_ID)
	{
		MCStage retValue = null;
		IDataReader idr=null;
        SqlParameter[] param=new SqlParameter[4];
		String sql = "SELECT * FROM CM_CStage WHERE CM_WebProject_ID=@param1 AND RelativeURL LIKE @param2 " + //1,2
				"AND CM_CStage_ID IN (SELECT Node_ID FROM AD_TreeNodeCMS WHERE " +
					" AD_Tree_ID=@param3 AND Parent_ID=@param4)" + // 3, 4
				"ORDER BY CM_CStage_ID";
		try
		{
			//pstmt = DataBase.prepareStatement (sql, project.get_TrxName());
			//pstmt.setInt (1, project.getCM_WebProject_ID());
           param[0]=new SqlParameter("@param1", project.GetCM_WebProject_ID());
			//pstmt.setString (2, RelativeURL);
            param[1]=new SqlParameter("@param2", RelativeURL);
			//pstmt.setInt (3, project.getAD_TreeCMS_ID ());
            param[2]=new SqlParameter("@param3",project.GetAD_TreeCMS_ID());
			//pstmt.setInt (4, parent_ID);
            param[3]=new SqlParameter("@param4",parent_ID);
	       	idr=DataBase.DB.ExecuteReader(sql,param,project.Get_TrxName());
    			if (idr.Read())
                {
				retValue = new MCStage (project.GetCtx(),idr, project.Get_TrxName());
                }
			idr.Close();
		}
		catch (Exception e)
		{
            if(idr!=null)
            {
                idr.Close();
            }

			_log.Log (Level.SEVERE, sql, e);
		}
		
		return retValue;
	}	//	getStages

	/// <summary>
	/// Get Stages by Template
	/// </summary>
	/// <param name="project">project</param>
	/// <param name="CM_Template_ID">id</param>
	/// <returns>stages</returns>
	public static MCStage[] GetStagesByTemplate (MWebProject project, int CM_Template_ID)
	{
		List<MCStage> list = new List<MCStage>();
		SqlParameter[] param=new SqlParameter[2];
        IDataReader idr=null;
		String sql = "SELECT * FROM CM_CStage WHERE CM_WebProject_ID=@param1 AND CM_Template_ID=@param2";
		try
		{
			//pstmt = DataBase.prepareStatement (sql, project.get_TrxName());
			//pstmt.setInt (1, project.getCM_WebProject_ID());
            param[0]=new SqlParameter("@param1",Utility.Util.GetValueOfInt(project.GetCM_WebProject_ID()));
			//pstmt.setInt (2, CM_Template_ID);
            param[1]=new SqlParameter("@param2",CM_Template_ID);
            idr=DataBase.DB.ExecuteReader(sql,param,project.Get_TrxName());
			while (idr.Read())
			{
				list.Add (new MCStage (project.GetCtx(), idr, project.Get_TrxName()));
			}
			idr.Close();
		}
		catch (Exception e)
		{
            if(idr!=null)
            {
                idr.Close();
            }

			_log.Log (Level.SEVERE, sql, e);
		}
		
		MCStage[] retValue = new MCStage[list.Count];
		retValue=list.ToArray ();
		return retValue;
	}	//	getStages

	/**	Logger	*/
	private static VLogger _log = VLogger.GetVLogger (typeof(MCStage).FullName);//.class);
	
	/** Template */
	private MTemplate _template = null;

	/// <summary>
    /// Get Template from Cache, or load it
	/// </summary>
	/// <returns>template</returns>
	public MTemplate GetTemplate() 
	{
		if (GetCM_Template_ID()>0 && _template==null)
			_template = MTemplate.Get(GetCtx(), GetCM_Template_ID(), null);
		return _template;
	} // getTemplate
	
	/// <summary>
    /// Standard Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="CM_CStage_ID">id</param>
	/// <param name="trxName">trx</param>
	public MCStage (Ctx ctx, int CM_CStage_ID, Trx trxName):base(ctx, CM_CStage_ID, trxName)
	{
		
		if (CM_CStage_ID == 0)
		{
			SetIsValid(false);
			SetIsModified(false);
			SetIsSecure(false);
			SetIsSummary(false);
			SetIsIndexed(false);
		}
	}	//	MCStage

	/// <summary>
	///	Load Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="rs">datarow</param>
	/// <param name="trxName">trx</param>
	public MCStage (Ctx ctx,DataRow dr, Trx trxName):	base (ctx,dr, trxName)
	{
	
	}	//	MCStage
    public MCStage(Ctx ctx,IDataReader idr, Trx trxName)
        : base(ctx, idr, trxName)
    { }
	
	/** Web Project			*/
	private MWebProject 	_project = null;
	
	/// <summary>
    /// Set Relative URL
    /// </summary>
    /// <param name="RelativeURL"> RelativeURL</param>
	public new void SetRelativeURL (String RelativeURL)
	{
		if (RelativeURL != null)
		{
			if (RelativeURL.EndsWith("/"))
				RelativeURL = RelativeURL.Substring(0, RelativeURL.Length -1);
			int index = RelativeURL.LastIndexOf("/");
			if (index != -1)
				RelativeURL = RelativeURL.Substring(index+1);
		}
		base.SetRelativeURL (RelativeURL);
	}	//	setRelativeURL
	
	/// <summary>
    /// Get Web Project
	/// </summary>
	/// <returns>webproject</returns>
	public MWebProject GetWebProject()
	{
		if (_project == null)
			_project = MWebProject.Get(GetCtx(), GetCM_WebProject_ID());
		return _project;
	}	//	getWebProject
	
	/// <summary>
    /// Get AD_Tree_ID
	/// </summary>
	/// <returns>tree</returns>
	public int GetAD_Tree_ID()
	{
		return GetWebProject().GetAD_TreeCMS_ID();
	}	//	getAD_Tree_ID;
	
	/// <summary>
    /// String Representation
	/// </summary>
	/// <returns>info</returns>
	public override String ToString ()
	{
		StringBuilder sb = new StringBuilder ("MCStage[")
			.Append (Get_ID()).Append ("-").Append (GetName()).Append ("]");
		return sb.ToString ();
	} 	//	toString
	
	/// <summary>
    /// 	Before Save
	/// </summary>
	/// <param name="newRecord">new</param>
	/// <returns>true</returns>
	protected override bool BeforeSave (bool newRecord)
	{
		//	Length >0 if not (Binary, Image, Text Long)
		if ((!this.IsSummary() || this.GetContainerType().Equals ("L")) && GetCM_Template_ID()==0)
		{
			log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "Template"));
			return false;
		}
		// On Modification set isModified
		if (Is_Changed () && !Is_ValueChanged("IsModified"))
			SetIsModified(true);
		//	Validate
		SetRelativeURL(GetRelativeURL());
		return true;
	}	//	beforeSave
	
	/// <summary>
    /// After Save.
	/// </summary>
	/// <param name="newRecord">insert</param>
	/// <param name="success">success</param>
	/// <returns>true if saved</returns>
	protected override bool AfterSave (bool newRecord, bool success)
	{
		if (!success)
			return success;
		// If Not Summary Node check whether all Elements and Templatetable Records exist.
		if (!IsSummary()) { 
			CheckElements();
			CheckTemplateTable();
		}
		if (newRecord)
		{
			StringBuilder sb = new StringBuilder ("INSERT INTO AD_TreeNodeCMS "
				+ "(AD_Client_ID,AD_Org_ID, IsActive,Created,CreatedBy,Updated,UpdatedBy, "
				+ "AD_Tree_ID, Node_ID, Parent_ID, SeqNo) "
				+ "VALUES (")
				.Append(GetAD_Client_ID()).Append(",0, 'Y', SysDate, 0, SysDate, 0,")
				.Append(GetAD_Tree_ID()).Append(",").Append(Get_ID())
				.Append(", 0, 999)");
			int no = DataBase.DB.ExecuteQuery(sb.ToString(),null, Get_TrxName());
			if (no > 0)
				log.Fine("#" + no + " - TreeType=CMS");
			else
				log.Warning("#" + no + " - TreeType=CMS");
			return no > 0;
		}
		/*if (success) {
		}*/
		return success;
	}	//	afterSave
	/// <summary>
    /// After Delete
	/// </summary>
	/// <param name="success">success</param>
	/// <returns>true</returns>
	protected override bool AfterDelete (bool success)
	{
		if (!success)
			return success;
		//
		StringBuilder sb = new StringBuilder ("DELETE FROM AD_TreeNodeCMS ")
			.Append(" WHERE Node_ID=").Append(Get_IDOld())
			.Append(" AND AD_Tree_ID=").Append(GetAD_Tree_ID());
		int no = DataBase.DB.ExecuteQuery(sb.ToString(),null, Get_TrxName());
		if (no > 0)
			log.Fine("#" + no + " - TreeType=CMS");
		else
			log.Warning("#" + no + " - TreeType=CMS");
		return no > 0;
	}	//	afterDelete
	
	/// <summary>
    /// Validate
	/// </summary>
	/// <returns>imfo</returns>
	public String Validate()
	{
		return "";
	}	//	validate
	
	/// <summary>
    /// Check whether all Elements exist
	/// </summary>
	/// <returns>true if updated</returns>
	public bool CheckElements () 
    {
		MTemplate thisTemplate = new MTemplate(GetCtx(), this.GetCM_Template_ID(), Get_TrxName());
		StringBuilder thisElementList = new StringBuilder(thisTemplate.GetElements());
		while (thisElementList.ToString().IndexOf("\n")>=0) 
        {
			String thisElement = thisElementList.ToString().Substring(0,thisElementList.ToString().IndexOf("\n"));
			//thisElementList.delete(0,thisElementList.indexOf("\n")+1);
            thisElementList.Remove(0, thisElementList.ToString().IndexOf("\n") + 1);
			if (thisElement!=null && !thisElement.Equals(""))
				CheckElement(thisElement);
		}
		String thisElement1 = thisElementList.ToString();
		if (thisElement1!=null && !thisElement1.Equals(""))
			CheckElement(thisElement1);
		return true;
	}
	
	public MCStageElement GetElementByName(String elementName) 
    {
		return MCStageElement.GetByName (GetCtx(), Get_ID(), elementName, Get_TrxName());
	}

	/// <summary>
    /// Check single Element, if not existing create it...
	/// </summary>
	/// <param name="elementName">element name</param>
	public void CheckElement(String elementName) {
		MCStageElement thisElement = GetElementByName(elementName);
		if (thisElement==null) 
        {
			thisElement = new MCStageElement(GetCtx(), 0, Get_TrxName());
			thisElement.SetAD_Client_ID(GetAD_Client_ID());
			thisElement.SetAD_Org_ID(GetAD_Org_ID());
			thisElement.SetCM_CStage_ID(this.Get_ID());
			thisElement.SetContentHTML(" ");
			thisElement.SetName(elementName);
			thisElement.Save(Get_TrxName());
		}
	}
	
	/// <summary>
    /// Check whether all Template Table records exits
	/// </summary>
	/// <returns>true if updated</returns>
	public bool CheckTemplateTable () {
		int [] tableKeys = X_CM_TemplateTable.GetAllIDs("CM_TemplateTable", "CM_Template_ID=" + this.GetCM_Template_ID(), Get_TrxName());
		if (tableKeys!=null) 
        {
			for (int i=0;i<tableKeys.Length;i++)
            {
				X_CM_TemplateTable thisTemplateTable = new X_CM_TemplateTable(GetCtx(), tableKeys[i], Get_TrxName());
				int [] existingKeys = X_CM_CStageTTable.GetAllIDs("CM_CStageTTable", "CM_TemplateTable_ID=" + thisTemplateTable.Get_ID(), Get_TrxName());
				if (existingKeys==null || existingKeys.Length==0)
                {
					X_CM_CStageTTable newCStageTTable = new X_CM_CStageTTable(GetCtx(), 0, Get_TrxName());
					newCStageTTable.SetAD_Client_ID(GetAD_Client_ID());
					newCStageTTable.SetAD_Org_ID(GetAD_Org_ID());
					newCStageTTable.SetCM_CStage_ID(Get_ID());
					newCStageTTable.SetCM_TemplateTable_ID(thisTemplateTable.Get_ID());
					newCStageTTable.SetDescription(thisTemplateTable.GetDescription());
					newCStageTTable.SetName(thisTemplateTable.GetName());
					newCStageTTable.SetOtherClause(thisTemplateTable.GetOtherClause());
					newCStageTTable.SetWhereClause(thisTemplateTable.GetWhereClause());
					newCStageTTable.Save();
				}
			}
		}
		return true;
	}
	
}	//	MCStage

}
