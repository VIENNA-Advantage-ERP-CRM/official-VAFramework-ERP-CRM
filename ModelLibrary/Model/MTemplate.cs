/********************************************************
 * Project Name   : VAdvantage
 * Class Name     :  MTemplate
 * Purpose        :  MTemplate Model
 * Class Used     : X_CM_Template
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
    public class MTemplate :X_CM_Template
{
	/// <summary>
	///Get MTemplate from Cache
   	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="CM_Template_ID">id</param>
	/// <param name="trxName">trx</param>
	/// <returns>MWEbproject</returns>
	public static MTemplate Get(Ctx ctx, int CM_Template_ID, Trx trxName)
	{
		MTemplate retValue = new MTemplate (ctx, CM_Template_ID, trxName);
		if (retValue != null)
			return retValue;
		retValue = new MTemplate (ctx, CM_Template_ID, null);
		return retValue;
	}	// get

	/// <summary>
	///  Standard Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="CM_Template_ID">id</param>
	/// <param name="trxName">trx</param>
	public MTemplate (Ctx ctx, int CM_Template_ID, Trx trxName):base(ctx, CM_Template_ID, trxName)
	{
		
	} // MTemplate

	/// <summary>
	///  Load Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="rs">datarow</param>
	/// <param name="trxName">trx</param>
	public MTemplate (Ctx ctx,DataRow dr, Trx trxName):base(ctx,dr, trxName)
	{
		
	} 
public MTemplate (Ctx ctx,IDataReader idr, Trx trxName):base(ctx,idr, trxName)
	{
		
	} 
	/** Web Project */
	private MWebProject _project = null;
	/** 
     * preBuildTemplate contains a preset Version including needed Subtemplates
     */
	private StringBuilder _preBuildTemplate;
	
	/** Logger								*/
	private static VLogger		_log = VLogger.GetVLogger (typeof(MTemplate).FullName);//.class);

	/// <summary>
    ///  	get Template by Name
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="name">name</param>
	/// <param name="projectID">id</param>
	/// <param name="trxName">trx</param>
	/// <returns>Template</returns>
	public static MTemplate GetByName (Ctx ctx, String name, int projectID, Trx trxName)
	{
		String sql = "SELECT * FROM CM_Template WHERE Value LIKE @param1 AND CM_WebProject_ID=@param2";
		MTemplate thisElement = null;
        SqlParameter[] param = new SqlParameter[2];
        IDataReader idr = null;
		try
		{
			//pstmt = DataBase.prepareStatement (sql, trxName);

			//pstmt.setString (1, name);
            param[0]=new SqlParameter("@param1",Utility.Util.GetValueOfString(name));
			//pstmt.setInt (2, projectID);
            param[1] = new SqlParameter("@param2", projectID);
            idr = DataBase.DB.ExecuteReader(sql, param, trxName);
            if (idr.Read())
            {
                thisElement = new MTemplate(ctx, idr, trxName);
            }
            idr.Close();
		}
		catch (Exception e)
		{
            if (idr != null)
            {
                idr.Close();
            }
			_log.Log(Level.SEVERE, "getByName", e);
		}
		return thisElement;
	}	//	getEntries

	
	/// <summary>
    /// Get Web Project
	/// </summary>
    /// <returns>Web Project</returns>
	public MWebProject GetWebProject ()
	{
        if (_project == null)
        {
            _project = MWebProject.Get(GetCtx(), GetCM_WebProject_ID());
        }
		return _project;
	}	// getWebProject

	/// <summary>
    /// Get AD_Tree_ID
	/// </summary>
	/// <returns>tree</returns>
	public int GetAD_Tree_ID ()
	{
		return GetWebProject ().GetAD_TreeCMT_ID ();
	}	// getAD_Tree_ID;
	
	/**	_isUseAd Global Use Ad **/
	private bool _isUseAd = false;
	
	/** _adTemplates StringBuffer with all IDs of Templates **/
	private StringBuilder _adTemplates = new StringBuilder();
	
	/// <summary>
    /// Check for isUseAd if needed including subtemplates
	/// </summary>
	/// <param name="includeSub">include sub</param>
	/// <returns>_isusead</returns>
	public bool IsUseAd(bool includeSub) 
	{
		if (!includeSub)
		{
			return IsUseAd();
		} 
        else
        {
            if (_preBuildTemplate == null)
            {
                RebuildTemplate();
            }
            if (!_isUseAd && IsUseAd())
            {
                _isUseAd = IsUseAd();
            }
            if (IsUseAd())
            {
                _adTemplates.Append(Get_ID() + ",");
            }
			return _isUseAd;
		}
	}
	
	/**	_isNews Global Use News **/
	private bool _isNews = false;
	
	/// <summary>
    /// Check for isNews if needed including subtemplates
	/// </summary>
	/// <param name="includeSub">include sub</param>
	/// <returns>is new</returns>
	public bool IsNews(bool includeSub) 
	{
		if (!includeSub)
		{
			return IsNews ();
		} 
        else
        {
			if (_preBuildTemplate==null) 
				RebuildTemplate();
			if (_isNews!=IsNews()) 
				_isNews = IsNews();
			return _isNews;
		}
	}
	
	/** _isRequest Global Request News **/
	private bool _isRequest = false;
	
	/// <summary>
    /// 	Check whether we need to include Request data
	/// </summary>
	/// <param name="includeSub">include sub</param>
	/// <returns>is Request</returns>
	public bool IsRequest(bool includeSub) 
	{
		if (_preBuildTemplate==null)
			RebuildTemplate();
		return _isRequest;
	}

	/// <summary>
	///Get the Template we prebuild (this means with added subtemplates)
     /// </summary>
    /// <returns>StringBuffer with complete XSL Template</returns>
	public StringBuilder GetPreBuildTemplate ()
	{
		if (_preBuildTemplate == null)
			RebuildTemplate();
		return _preBuildTemplate;
	}

	/// <summary>
	///   * Prebuild Template, this also set's parameters of subtemplates 
     // on the main template
	/// </summary>
	public void RebuildTemplate ()
	{
		// We will build the prebuild code, so we check which subs are
		// needed and build it depending on them
		_preBuildTemplate = new StringBuilder (GetTemplateXST ());
		// Let's see whether the template calls Subtemplates...
		if (_preBuildTemplate.ToString().IndexOf ("<xsl:call-template") >= 0)
		{
			StringBuilder subTemplates = new StringBuilder ();
			int pos = 0;
			List<String> subTemplateNames = new List<String> ();
			while (_preBuildTemplate.ToString().IndexOf ("<xsl:call-template", pos) >= 0)
			{
				String thisName = null;
				int beginPos = _preBuildTemplate.ToString().IndexOf (
					"<xsl:call-template", pos);
				int endPos = _preBuildTemplate.ToString().IndexOf ("/>", beginPos);
				if (_preBuildTemplate.ToString().IndexOf (">", beginPos) < endPos)
				{
					endPos = _preBuildTemplate.ToString().IndexOf (">", beginPos) + 1;
				}
				String tempTemplate = _preBuildTemplate.ToString().Substring (beginPos,
					endPos);
				pos = _preBuildTemplate.ToString().IndexOf ("<xsl:call-template", pos)
					+ tempTemplate.Length;
				if (tempTemplate.ToString().IndexOf ("name=") >= 0)
				{
					thisName = tempTemplate.ToString().Substring (tempTemplate.ToString()
						.IndexOf ("name=\"") + 6, tempTemplate.ToString().IndexOf (
						"\"", tempTemplate.ToString().IndexOf ("name=\"") + 7));
					if (!subTemplateNames.Contains (thisName))
						subTemplateNames.Add(thisName);
				}
			}
			// Build all the subtemplates and add them to the main template
			for (int i=0;i<subTemplateNames.Count ;i++) 
			{
				MTemplate subTemplate = GetByName(GetCtx(), subTemplateNames[i], GetCM_WebProject_ID(), Get_TrxName());
				if (subTemplate != null)
				{
					if (subTemplate.ContainsSubtemplates (true, subTemplateNames)) 
					{
						subTemplateNames = subTemplate.getSubTemplateList();
					}
					subTemplates.Append (subTemplate.GetTemplateXST ());
					if (subTemplate.IsUseAd ())
					{
						_isUseAd = true;
						_adTemplates.Append(subTemplate.Get_ID () + ",");
					}
					if (subTemplate.IsNews())
						_isNews = true;
				}
			}
			_preBuildTemplate.Append (subTemplates);
			_preBuildTemplate = new StringBuilder (_preBuildTemplate.ToString()
				.Substring (0, _preBuildTemplate.ToString()
					.IndexOf ("</xsl:stylesheet>"))
				+ subTemplates.ToString () + "\n</xsl:stylesheet>");
			// Check whether we need Request functionality for handling
			if (_preBuildTemplate.ToString().IndexOf ("/webCM/requestTables/")>=0) 
				_isRequest = true;
		}
	}	//	getPreBuildTemplate
	
	private bool _hasSubtemplates = true;
	private List<String> _subTemplates = null;
	
	private List<String> getSubTemplateList()
	{
		return _subTemplates;
	}
	
	private bool ContainsSubtemplates(bool refresh, List<String> existingSubTemplates) 
	{
		if (refresh)
			_subTemplates = null;
		if (_subTemplates!=null)
			return _hasSubtemplates;
		_subTemplates = new List<String> ();
		// Procedure to get the Subtemplates as an ArrayList
		if (existingSubTemplates!=null) 
		{
			for (int i=0;i<existingSubTemplates.Count;i++)
			{
				String thisTemplate = existingSubTemplates[i];//.get (i);
				_subTemplates.Add (thisTemplate);
			}
			//	_subTemplates.add(existingSubTemplates.get (i));
		}
		if (GetTemplateXST().IndexOf ("<xsl:call-template") >= 0)
		{
			int pos = 0;
			while (GetTemplateXST().IndexOf ("<xsl:call-template", pos) >= 0)
			{
				String thisName = null;
				int beginPos = GetTemplateXST().IndexOf (
					"<xsl:call-template", pos);
				int endPos = GetTemplateXST().IndexOf ("/>", beginPos);
				if (GetTemplateXST().IndexOf (">", beginPos) < endPos)
				{
					endPos = GetTemplateXST().IndexOf (">", beginPos) + 1;
				}
				String tempTemplate = GetTemplateXST().Substring (beginPos,
					endPos);
				pos = GetTemplateXST().IndexOf ("<xsl:call-template", pos)
					+ tempTemplate.Length;
				if (tempTemplate.IndexOf ("name=") >= 0)
				{
					thisName = tempTemplate.Substring (tempTemplate
						.IndexOf ("name=\"") + 6, tempTemplate.IndexOf (
						"\"", tempTemplate.IndexOf ("name=\"") + 7));
					if (!_subTemplates.Contains (thisName))
						_subTemplates.Add (thisName);
				}
			}
			_hasSubtemplates = true;
		} 
        else
        {
			_hasSubtemplates = false;
		}
		return _hasSubtemplates;
	}
	
	/// <summary>
    /// Before Save
	/// </summary>
	/// <param name="newRecord">new</param>
	/// <returns>true</returns>
	protected override bool BeforeSave (bool newRecord)
	{
		// TODO: We should implement the validation, until then we enforce it
		if (!IsValid())
        {
			SetIsValid(true);
		}
		return true;
	}	//	beforeSave

	/// <summary>
	///After Save. Insert - create tree
    /// </summary>
	/// <param name="newRecord">insert</param>
	/// <param name="success">success</param>
	/// <returns>true if inserted</returns>
	protected override bool AfterSave (bool newRecord, bool success)
	{
		if (!success)
			return success;
		if (newRecord)
		{
			StringBuilder sb = new StringBuilder (
				"INSERT INTO AD_TreeNodeCMT "
					+ "(AD_Client_ID,AD_Org_ID, IsActive,Created,CreatedBy,Updated,UpdatedBy, "
					+ "AD_Tree_ID, Node_ID, Parent_ID, SeqNo) " + "VALUES (")
				.Append (GetAD_Client_ID ()).Append (
					",0, 'Y', SysDate, 0, SysDate, 0,").Append (
					GetAD_Tree_ID ()).Append (",").Append (Get_ID ()).Append (
					", 0, 999)");
			int no = DataBase.DB.ExecuteQuery (sb.ToString (),null, Get_TrxName ());
			if (no > 0)
				log.Fine ("#" + no + " - TreeType=CMT");
			else
				log.Warning ("#" + no + " - TreeType=CMT");
			return no > 0;
		}
		if (!newRecord)
		{
            VAdvantage.CM.CacheHandler thisHandler = new VAdvantage.CM.CacheHandler(
                VAdvantage.CM.CacheHandler.ConvertJNPURLToCacheURL(GetCtx()
					.GetContext(System.Net.Dns.GetHostName())), log, GetCtx (),
				Get_TrxName ());
			if (!IsInclude ())
			{
				// Clean Main Templates on a single level.
				thisHandler.CleanTemplate (this.Get_ID ());
				// Check the elements in the Stage Area
				MCStage[] theseStages = MCStage.GetStagesByTemplate (GetWebProject(), Get_ID());
                for (int i = 0; i < theseStages.Length; i++)
                {
                    theseStages[i].CheckElements();
                }
			}
			else
			{
				// Since we not know which main templates we will clean up all!
				thisHandler.EmptyTemplate ();
			}
		}
		return success;
	}	// afterSave

	/// <summary>
	///After Delete
    /// </summary>
	/// <param name="success">success</param>
    /// <returns>deleted</returns>
	protected override bool AfterDelete (bool success)
	{
		if (!success)
			return success;
		//
		StringBuilder sb = new StringBuilder ("DELETE FROM AD_TreeNodeCMT ")
			.Append (" WHERE Node_ID=").Append (Get_IDOld ()).Append (
				" AND AD_Tree_ID=").Append (GetAD_Tree_ID ());
		int no = DataBase.DB.ExecuteQuery(sb.ToString (),null, Get_TrxName ());
		if (no > 0)
			log.Fine ("#" + no + " - TreeType=CMT");
		else
			log.Warning ("#" + no + " - TreeType=CMT");
		return no > 0;
	}	// afterDelete

	/// <summary>
    /// Get's all the Ads from Template AD Cat (including all subtemplates)
	/// </summary>
	/// <returns>array of MAd</returns>
	public MAd[] GetAds()
	{
		int[] AdCats = null;
		String sql = "SELECT count(*) FROM CM_Template_AD_Cat WHERE CM_Template_ID IN (" + _adTemplates.ToString ().Substring (0,_adTemplates.Length -1) + ")";
        IDataReader idr = null;
		try
		{
			int numberAdCats = 0;
			//pstmt = DataBase.prepareStatement (sql, get_TrxName ());
            idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
			if (idr.Read())
			{
                numberAdCats = Utility.Util.GetValueOfInt(idr[0]);// rs.getInt(1);
			}
            idr.Close();
			AdCats = new int[numberAdCats];
			int i = 0;
			sql = "SELECT CM_Ad_Cat_ID FROM CM_Template_AD_Cat WHERE CM_Template_ID IN (" + _adTemplates.ToString ().Substring (0,_adTemplates.Length -1) + ")";
			//pstmt = DataBase.prepareStatement (sql, get_TrxName ());
            idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
			while (idr.Read())
			{
                AdCats[i] = Utility.Util.GetValueOfInt(idr[0]);// rs.getInt(1);
				i++;
			}
            idr.Close();
		}
		catch (Exception ex)
		{
            if (idr != null)
            {
                idr.Close();
            }
			log.Log (Level.SEVERE, sql, ex);
		}
		
		
		if (AdCats != null && AdCats.Length > 0)
		{
			MAd[] returnAds = new MAd[AdCats.Length];
			for (int i = 0; i < AdCats.Length; i++)
			{
				MAd thisAd = MAd.GetNext(GetCtx (), AdCats[i],
					Get_TrxName ());
				if (thisAd!=null) 
					returnAds[i] = thisAd;
			}
			return returnAds;
		}
		else
		{
			return null;
		}
	}	//	getAds
	
} // MTemplate

}
