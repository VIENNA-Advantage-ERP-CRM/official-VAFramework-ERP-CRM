/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MContainer
 * Purpose        : Container Model
 * Class Used     : X_CM_Container
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
   public class MContainer: X_CM_Container
{
	/**	serialVersionUID	*/
	//private static long serialVersionUID = 395679572291279730L;
    private VConnection vcon = null; 
	/// <summary>
	/// get Container by Relative URL
	/// </summary>
	/// <param name="ctx">ctx</param>
	/// <param name="relURL">relurl</param>
	/// <param name="CM_WebProject_Id">id</param>
	/// <param name="trxName">trxname</param>
	/// <returns>Container or null if not found</returns>
	public static MContainer Get(Ctx ctx, String relURL, int CM_WebProject_Id, Trx trxName) 
    {
		MContainer thisContainer = null;
		String sql = "SELECT * FROM CM_Container WHERE (RelativeURL LIKE @param1 OR RelativeURL LIKE @param2) AND CM_WebProject_ID=@param3";
		IDataReader idr=null;
        SqlParameter[] param=new SqlParameter[3];
		try
		{
			//pstmt = DataBase.prepareStatement(sql, trxName);
			//pstmt.setString (1,relURL);
            param[0]=new SqlParameter("@param1",Utility.Util.GetValueOfString(relURL));
			//pstmt.setString (2,relURL+"/");
            param[1]=new SqlParameter("@param2",relURL+"/");
			//pstmt.setInt(3, CM_WebProject_Id);
            param[2]=new SqlParameter("@param3",CM_WebProject_Id);
			idr=DataBase.DB.ExecuteReader(sql,param,trxName);
			if (idr.Read())
            {
				thisContainer = (new MContainer(ctx,idr, trxName));
            }
			idr.Close();
		}
		catch (Exception e)
		{
            if(idr!=null)
            {
                idr.Close();
            }
			_log.Log(Level.SEVERE, sql, e);
		}
		return thisContainer;
	}
	
	/// <summary>
	/// get Container by Name
	/// </summary>
	/// <param name="ctx">ctx</param>
	/// <param name="Name">name</param>
	/// <param name="CM_WebProject_Id">id</param>
	/// <param name="trxName">trxname</param>
	/// <returns>Container or null if not found</returns>
	public static MContainer GetByName(Ctx ctx, String Name, int CM_WebProject_Id, Trx trxName) 
    {
		MContainer thisContainer = null;
		String sql = "SELECT * FROM CM_Container WHERE (Name LIKE @param1) AND CM_WebProject_ID=@param2";
		SqlParameter[] param=new SqlParameter[2];
        IDataReader idr=null;
		try
		{
			//pstmt = DataBase.prepareStatement(sql, trxName);
			//pstmt.setString (1,Name);
            param[0]=new SqlParameter("@param1",Utility.Util.GetValueOfString(Name));
			//pstmt.setInt(2, CM_WebProject_Id);
            param[1]=new SqlParameter("@param2",Utility.Util.GetValueOfInt(CM_WebProject_Id));
			idr=DataBase.DB.ExecuteReader(sql,param,trxName);
			if (idr.Read())
            {
				thisContainer = (new MContainer(ctx, idr , trxName));
            }
			idr.Close();
		}
		catch (Exception e)
		{
            if(idr!=null)
            {
                idr.Close();
            }
			_log.Log(Level.SEVERE, sql, e);
		}
        return thisContainer;
	}
	
	
	/// <summary>
	/// get Container by Title
	/// </summary>
	/// <param name="ctx">ctx</param>
	/// <param name="Title">title</param>
	/// <param name="CM_WebProject_Id">id</param>
	/// <param name="trxName">trxname</param>
	/// <returns>Container or null if not found</returns>
	public static MContainer GetByTitle(Ctx ctx, String Title, int CM_WebProject_Id, Trx trxName) {
		MContainer thisContainer = null;
		String sql = "SELECT * FROM CM_Container WHERE (Title LIKE @param1) AND CM_WebProject_ID=@param2";
		SqlParameter[] param=new SqlParameter[2];
        IDataReader idr=null;
		try
		{
			//pstmt = DataBase.prepareStatement(sql, trxName);
			//pstmt.setString (1,Title);
            param[0]=new SqlParameter("@param1",Title);
			//pstmt.setInt(2, CM_WebProject_Id);
            param[1]=new SqlParameter("@param2",CM_WebProject_Id);
			idr=DataBase.DB.ExecuteReader(sql,param,trxName);
			if (idr.Read())
            {
				thisContainer = (new MContainer(ctx, idr, trxName));
            }
            idr.Close();
		}
		catch (Exception e)
		{
            if(idr!=null)
            {
                idr.Close();
            }
			_log.Log(Level.SEVERE, sql, e);
		}
		return thisContainer;
	}
	
	/// <summary>
	///	get Container
	/// </summary>
	/// <param name="ctx">ctx</param>
	/// <param name="CM_Container_ID">id</param>
	/// <param name="CM_WebProject_Id">id</param>
	/// <param name="trxName">trxname</param>
	/// <returns> Container or null if not found</returns>
	public static MContainer Get(Ctx ctx, int CM_Container_ID, int CM_WebProject_Id, Trx trxName) {
		MContainer thisContainer = null;
		String sql = "SELECT * FROM CM_Container WHERE CM_Container_ID=@param1 AND CM_WebProject_ID=@param2";
		SqlParameter[] param=new SqlParameter[2];
        IDataReader idr=null;
		try
		{
			//pstmt = DataBase.prepareStatement(sql, trxName);
			//pstmt.setInt(1, CM_Container_ID);
            param[0]=new SqlParameter("@param1",CM_Container_ID);
			//pstmt.setInt(2, CM_WebProject_Id);
            param[1]=new SqlParameter("@param2",CM_WebProject_Id);
			idr=DataBase.DB.ExecuteReader(sql,param,trxName);
			if (idr.Read())
            {
				thisContainer = (new MContainer(ctx, idr, trxName));
            }
			idr.Close();
		}
		catch (Exception e)
		{
            if(idr!=null)
            {
                 idr.Close();
            }
			_log.Log(Level.SEVERE, sql, e);
		}
		
		return thisContainer;
	}
	
	/// <summary>
	///Deploy Stage into Container
    /// </summary>
	/// <param name="project">webproject</param>
	/// <param name="stage">Deploy Stage into Container</param>
	/// <param name="path">url to it</param>
	/// <returns>Container</returns>
	public static MContainer Deploy (MWebProject project, MCStage stage,
		String path)
	{
		MContainer cc = GetDirect (stage.GetCtx(), stage.GetCM_CStage_ID (),
			stage.Get_TrxName ());
		if (cc == null) // new
			cc = new MContainer (stage.GetCtx (), 0, stage.Get_TrxName ());
		cc.SetStage (project, stage, path);
		cc.Save ();
		if (!stage.IsSummary ())
		{
			cc.UpdateElements (project, stage, stage.Get_TrxName ());
			cc.UpdateTTables (project, stage, stage.Get_TrxName ());
		}
		return cc;
	}	// copy

	/// <summary>
	///  Get Container directly from DB (not cached)
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="CM_Container_ID">id</param>
	/// <param name="trxName">trx</param>
	/// <returns>container or null</returns>
	public static MContainer GetDirect (Ctx ctx, int CM_Container_ID,
        Trx trxName)
	{
		MContainer cc = null;
		SqlParameter[] param=new SqlParameter[1];
        IDataReader idr=null;
		String sql = "SELECT * FROM CM_Container WHERE CM_Container_ID=@param";
		try
		{
			//pstmt = DataBase.prepareStatement (sql, null);
			//pstmt.setInt (1, CM_Container_ID);
            param[0]=new SqlParameter("@param",CM_Container_ID);
			idr=DataBase.DB.ExecuteReader(sql,param,null);
			if (idr.Read())
            {
				cc = new MContainer (ctx, idr, trxName);
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

		return cc;
	} // getDirect

	/// <summary>
	/// Get Containers
	/// </summary>
	/// <param name="project">project</param>
	/// <returns>stages</returns>
	public static MContainer[] GetContainers (MWebProject project)
	{
		List<MContainer> list = new List<MContainer> ();
		SqlParameter[] param=new SqlParameter[1];
        IDataReader idr=null;
		String sql = "SELECT * FROM CM_Container WHERE CM_WebProject_ID=@param ORDER BY CM_Container_ID";
		try
		{
			//pstmt = DataBase.prepareStatement (sql, project.get_TrxName ());
			//pstmt.setInt (1, project.getCM_WebProject_ID ());
            param[0]=new SqlParameter("@param",project.GetCM_WebProject_ID ());
			idr=DataBase.DB.ExecuteReader(sql,param, project.Get_TrxName ());
			while (idr.Read())
			{
				list.Add (new MContainer (project.GetCtx (),idr, project
					.Get_Trx()));
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
		
		MContainer[] retValue = new MContainer[list.Count];;
		retValue=list.ToArray ();
		return retValue;
	} // getContainers

	/** Logger */
	private static VLogger _log = VLogger.GetVLogger (typeof(MContainer).FullName);//.class);

	
	/// <summary>
    /// Standard Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="CM_Container_ID">id</param>
	/// <param name="trxName">trx</param>
	public MContainer (Ctx ctx, int CM_Container_ID, Trx trxName):base(ctx, CM_Container_ID, trxName)
	{
		
		if (CM_Container_ID == 0)
		{
			SetIsValid(false);
			SetIsIndexed(false);
			SetIsSecure(false);
			SetIsSummary(false);
		}
	} // MContainer

	/// <summary>
    ///  Load Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="rs">datarow</param>
	/// <param name="trxName">trx</param>
	public MContainer (Ctx ctx,DataRow dr, Trx trxName):base(ctx, dr, trxName)
	{
		
	} // MContainer
       public MContainer (Ctx ctx,IDataReader idr, Trx trxName):base(ctx, idr, trxName)
       {}

	/** Web Project */
	private MWebProject _project = null;

	/** Stage Source */
	private MCStage	 _stage   = null;
	
	/** Template */
	private MTemplate _template = null;

	/// <summary>
    /// Get Web Project
	/// </summary>
	/// <returns>web project</returns>
	public MWebProject GetWebProject ()
	{
		if (_project == null)
			_project = MWebProject.Get(GetCtx (), GetCM_WebProject_ID ());
		return _project;
	} // getWebProject
	
	/// <summary>
    /// Get Template from Cache, or load it
	/// </summary>
	/// <returns>template</returns>
	public MTemplate GetTemplate() 
	{
        if (GetCM_Template_ID() > 0 && _template == null)
        {
            _template = MTemplate.Get(GetCtx(), GetCM_Template_ID(), null);
        }
		return _template;
	} // getTemplate

	/// <summary>
    /// Get AD_Tree_ID
	/// </summary>
	/// <returns>tree</returns>
	public int GetAD_Tree_ID ()
	{
		return GetWebProject ().GetAD_TreeCMC_ID ();
	} // getAD_Tree_ID;

	/// <summary>
    /// Set/Copy Stage
	/// </summary>
	/// <param name="project">parent</param>
	/// <param name="stage">stage</param>
	/// <param name="path">path</param>
	protected void SetStage(MWebProject project, MCStage stage, String path)
	{
		_stage = stage;
		PO.CopyValues (stage, this);
		SetAD_Client_ID (project.GetAD_Client_ID ());
		SetAD_Org_ID (project.GetAD_Org_ID ());
		SetIsActive(stage.IsActive());
		SetCM_ContainerLink_ID (stage.GetCM_CStageLink_ID ());
		//
		SetRelativeURL (path + stage.GetRelativeURL ());
		//
		if (GetMeta_Author() == null || GetMeta_Author ().Length == 0)
			SetMeta_Author (project.GetMeta_Author ());
		if (GetMeta_Content() == null || GetMeta_Content ().Length == 0)
			SetMeta_Content (project.GetMeta_Content ());
		if (GetMeta_Copyright () == null || GetMeta_Copyright ().Length == 0)
			SetMeta_Copyright (project.GetMeta_Copyright ());
		if (GetMeta_Publisher () == null || GetMeta_Publisher ().Length  == 0)
			SetMeta_Publisher (project.GetMeta_Publisher ());
		if (GetMeta_RobotsTag () == null || GetMeta_RobotsTag ().Length == 0)
			SetMeta_RobotsTag (project.GetMeta_RobotsTag ());
	} // setStage

	/// <summary>
    /// Update Elements in Container from Stage
	/// </summary>
	/// <param name="project">project</param>
	/// <param name="stage">stage</param>
	/// <param name="trxName">trx</param>
	protected void UpdateElements(MWebProject project, MCStage stage,
		Trx trxName)
	{
        vcon = new VConnection();
        VAdvantage.CM.CacheHandler thisHandler = new VAdvantage.CM.CacheHandler(
            VAdvantage.CM.CacheHandler.ConvertJNPURLToCacheURL(vcon.Apps_host), log, GetCtx (),
			Get_Trx());
		// First update the new ones...
		int[] tableKeys = X_CM_CStage_Element.GetAllIDs ("CM_CStage_Element",
			"CM_CStage_ID=" + stage.Get_ID (), trxName);
		if (tableKeys != null && tableKeys.Length > 0)
		{
			for (int i = 0; i < tableKeys.Length; i++)
			{
				X_CM_CStage_Element thisStageElement = new X_CM_CStage_Element (
					project.GetCtx (), tableKeys[i], trxName);
				int[] thisContainerElementKeys = X_CM_Container_Element
					.GetAllIDs ("CM_Container_Element", "CM_Container_ID="
						+ stage.Get_ID () + " AND Name LIKE '"
						+ thisStageElement.GetName () + "'", trxName);
				X_CM_Container_Element thisContainerElement;
				if (thisContainerElementKeys != null
					&& thisContainerElementKeys.Length > 0)
				{
					thisContainerElement = new X_CM_Container_Element (project
						.GetCtx (), thisContainerElementKeys[0], trxName);
				}
				else
				{
					thisContainerElement = new X_CM_Container_Element (project
						.GetCtx (), 0, trxName);
				}
				thisContainerElement.SetCM_Container_ID (stage.Get_ID ());
				X_CM_CStage_Element stageElement = new X_CM_CStage_Element (
					project.GetCtx (), tableKeys[i], trxName);
				thisContainerElement.SetName (stageElement.GetName ());
				thisContainerElement.SetDescription (stageElement.GetDescription());
				thisContainerElement.SetHelp (stageElement.GetHelp ());
				thisContainerElement.SetIsActive (stageElement.IsActive ());
				thisContainerElement.SetIsValid (stageElement.IsValid ());
				String contentHTML = thisStageElement.GetContentHTML ();
				thisContainerElement.SetContentHTML (contentHTML);
				// PO.copyValues(new
				// X_CM_CStage_Element(project.getCtx(),tableKeys[i],trxName),
				// thisContainerElement);
				thisContainerElement.Save(trxName);
				// Remove Container from cache
				thisHandler.CleanContainerElement (thisContainerElement
					.Get_ID ());
			}
		}
		// Now we are checking the existing ones to delete the unneeded ones...
		tableKeys = X_CM_Container_Element.GetAllIDs ("CM_Container_Element",
			"CM_Container_ID=" + stage.Get_ID (), trxName);
		if (tableKeys != null && tableKeys.Length > 0)
		{
			for (int i = 0; i < tableKeys.Length; i++)
			{
				X_CM_Container_Element thisContainerElement = new X_CM_Container_Element (
					project.GetCtx (), tableKeys[i], trxName);
				int[] thisCStageElementKeys = X_CM_CStage_Element
					.GetAllIDs ("CM_CStage_Element", "CM_CStage_ID="
						+ stage.Get_ID () + " AND Name LIKE '"
						+ thisContainerElement.GetName () + "'", trxName);
				// If we cannot find a representative in the Stage we will delete from production
				if (thisCStageElementKeys == null
					|| thisCStageElementKeys.Length < 1)
				{
					// First delete it from cache, then delete the record itself
					thisHandler.CleanContainerElement(thisContainerElement
						.Get_ID ());
					thisContainerElement.Delete (true);
				}
			}
		}
	}

	/// <summary>
    /// Update Elements in Container from Stage
	/// </summary>
	/// <param name="project">proejct</param>
	/// <param name="stage">stage</param>
	/// <param name="trxName">trx</param>
	protected void UpdateTTables(MWebProject project, MCStage stage,
		Trx trxName)
	{
		int[] tableKeys = X_CM_CStageTTable.GetAllIDs ("CM_CStageTTable",
			"CM_CStage_ID=" + stage.Get_ID (), trxName);
		if (tableKeys != null && tableKeys.Length > 0)
		{
			for (int i = 0; i < tableKeys.Length; i++)
			{
				X_CM_CStageTTable thisStageTTable = new X_CM_CStageTTable (
					project.GetCtx (), tableKeys[i], trxName);
				int[] thisContainerTTableKeys = X_CM_ContainerTTable.GetAllIDs (
					"CM_ContainerTTable", "CM_Container_ID=" + stage.Get_ID ()
						+ " AND CM_TemplateTable_ID="
						+ thisStageTTable.GetCM_TemplateTable_ID (), trxName);
				X_CM_ContainerTTable thisContainerTTable;
				if (thisContainerTTableKeys != null
					&& thisContainerTTableKeys.Length > 0)
				{
					thisContainerTTable = new X_CM_ContainerTTable (project
						.GetCtx (), thisContainerTTableKeys[0], trxName);
				}
				else
				{
					thisContainerTTable = new X_CM_ContainerTTable (project
						.GetCtx(), 0, trxName);
				}
				thisContainerTTable.SetCM_Container_ID (stage.Get_ID());
				PO.CopyValues(new X_CM_CStageTTable (project.GetCtx (),
					tableKeys[i], trxName), thisContainerTTable);
				thisContainerTTable.Save (trxName);
			}
		}
	}

	/// <summary>
    /// SaveNew getID
	/// </summary>
	/// <returns>id</returns>
	protected int SaveNew_getID ()
	{
		if (_stage != null)
			return _stage.GetCM_CStage_ID ();
		return 0;
	} // saveNew_getID

	/// <summary>
    /// String Representation
	/// </summary>
	/// <returns>info</returns>
	public override String ToString ()
	{
		StringBuilder sb = new StringBuilder ("MContainer[").Append (Get_ID ())
			.Append ("-").Append (GetName()).Append ("]");
		return sb.ToString ();
	} // toString

	/// <summary>
    /// After Save. Insert - create tree
	/// </summary>
	/// <param name="newRecord">new</param>
	/// <param name="success">success</param>
	/// <returns>true</returns>
	protected override bool AfterSave (bool newRecord, bool success)
	{
		if (!success)
			return success;
		if (newRecord)
		{
			StringBuilder sb = new StringBuilder (
				"INSERT INTO AD_TreeNodeCMC "
					+ "(AD_Client_ID,AD_Org_ID, IsActive,Created,CreatedBy,Updated,UpdatedBy, "
					+ "AD_Tree_ID, Node_ID, Parent_ID, SeqNo) " + "VALUES (")
				.Append (GetAD_Client_ID ()).Append (
					",0, 'Y', SysDate, 0, SysDate, 0,").Append (
					GetAD_Tree_ID ()).Append (",").Append (Get_ID ()).Append (
					", 0, 999)");
			int no = DataBase.DB.ExecuteQuery(sb.ToString (),null, Get_TrxName ());
			if (no > 0)
				log.Fine ("#" + no + " - TreeType=CMC");
			else
				log.Warning ("#" + no + " - TreeType=CMC");
			return no > 0;
		}
		ReIndex(newRecord);
		return success;
	} // afterSave
	
	protected MContainerElement[] GetAllElements()
	{
		int[] elements = MContainerElement.GetAllIDs("CM_Container_Element", "CM_Container_ID=" + Get_ID(), Get_Trx());
		if (elements !=null && elements.Length>0)
		{
			MContainerElement[] containerElements = new MContainerElement[elements.Length];
			for (int i=0;i<elements.Length;i++)
			{
				containerElements[i] = new MContainerElement(GetCtx(), elements[i], Get_Trx());
			}
			return containerElements;
		} 
        else
        {
			return null;
		}
	}
	
	protected override bool BeforeDelete()
	{
		// Clean own index
		MIndex.CleanUp(Get_Trx(), GetAD_Client_ID(), Get_Table_ID(), Get_ID());
		// Clean ElementIndex
		MContainerElement[] theseElements = GetAllElements();
		if (theseElements!=null)
		{
			for (int i=0;i<theseElements.Length;i++) 
			{
				theseElements[i].Delete(false);
			}
		}
		//
		StringBuilder sb = new StringBuilder ("DELETE FROM AD_TreeNodeCMC ")
			.Append (" WHERE Node_ID=").Append (Get_ID ()).Append (
				" AND AD_Tree_ID=").Append (GetAD_Tree_ID ());
		int no = DataBase.DB.ExecuteQuery(sb.ToString (),null, Get_TrxName ());
        if (no > 0)
        {
            log.Fine("#" + no + " - TreeType=CMC");
        }
        else
            log.Warning("#" + no + " - TreeType=CMC");
		return no > 0;
	}

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
		StringBuilder sb = new StringBuilder ("DELETE FROM AD_TreeNodeCMC ")
			.Append (" WHERE Node_ID=").Append (Get_IDOld ()).Append (
				" AND AD_Tree_ID=").Append (GetAD_Tree_ID ());
		int no = DataBase.DB.ExecuteQuery(sb.ToString(),null, Get_Trx());
		// If 0 than there is nothing to delete which is okay.
		if (no > 0)
			log.Fine ("#" + no + " - TreeType=CMC");
		else
			log.Warning ("#" + no + " - TreeType=CMC");
		return true;
	} // afterDelete
	
	/// <summary>
    /// 	reIndex
	/// </summary>
	/// <param name="newRecord">new record</param>
	public void ReIndex(bool newRecord)
	{
		if (IsIndexed())
        {
			String [] toBeIndexed = new String[8];
			toBeIndexed[0] = this.GetName();
			toBeIndexed[1] = this.GetDescription();
			toBeIndexed[2] = this.GetRelativeURL();
			toBeIndexed[3] = this.GetMeta_Author();
			toBeIndexed[4] = this.GetMeta_Copyright();
			toBeIndexed[5] = this.GetMeta_Description();
			toBeIndexed[6] = this.GetMeta_Keywords();
			toBeIndexed[7] = this.GetMeta_Publisher();
			MIndex.ReIndex (newRecord, toBeIndexed, GetCtx(), GetAD_Client_ID(), Get_Table_ID(), Get_ID(), GetCM_WebProject_ID(), this.GetUpdated());
			MContainerElement[] theseElements = GetAllElements();
			if (theseElements!=null) 
				for (int i=0;i<theseElements.Length;i++)
					theseElements[i].ReIndex (false);
		}
		if (!IsIndexed() && !newRecord)
			MIndex.CleanUp (Get_Trx(), GetAD_Client_ID(), Get_Table_ID(), Get_ID());
	} // reIndex
} // MContainer

}
