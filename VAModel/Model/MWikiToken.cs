/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MWikiToken
 * Purpose        : Container Stage Model
 * Class Used     : X_CM_WikiToken
 * Chronological    Development
 * Deepak           12-Feb-2010
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
using System.Text.RegularExpressions;
using java.util.regex;
namespace VAdvantage.Model
{
   public class MWikiToken:X_CM_WikiToken
{
	/**	serialVersionUID	*/
	//private static  long serialVersionUID = 4980395873969221189L;

	/**************************************************************************
	 * 	Standard Constructor
	 *	@param ctx context
	 *	@param CM_WikiToken_ID id
	 *	@param trxName tansaction
	 */
	public MWikiToken (Ctx ctx, int CM_WikiToken_ID, Trx trxName):base(ctx, CM_WikiToken_ID, trxName)
	{
		
	}

	/**
	 * 	Load Constructor
	 *	@param ctx context
	 *	@param rs result set
	 *	@param trxName transaction
	 */
	public MWikiToken (Ctx ctx,DataRow rs, Trx trxName):base(ctx, rs, trxName)
	{
		
	}	//	MWikiToken
    public MWikiToken(Ctx ctx, IDataReader rs, Trx trxName)
        : base(ctx, rs, trxName)
    { }
	/** Logger								*/
	private static VLogger		_log = VLogger.GetVLogger(typeof(MWikiToken).FullName);//.class);
	/** Pattern								*/
	private Pattern pattern = null;
	
	/**
	 * 	getPattern
	 *	@return returns preCompiled RegEx Pattern
	 */
	public Pattern GetPattern()
	{
		if (pattern!=null)
			return pattern;
		pattern = Pattern.compile(GetName());
		return pattern;
	}
	
	/**
	 *  (re)Load record with m_ID[*]
	 *  @param trxName transaction
	 *  @return true if loaded
	 */
	public new bool Load (Trx trxName)
	{
		pattern = null;
		return base.Load(trxName);
	}

	
	/**
	 * 	get all Wiki Tokens on system level (i.e. to preload cache)
	 *  @param ctx 
	 *  @param trxName 
	 *	@return Array of previous DunningLevels
	 */
	public static MWikiToken[] GetAllForPreload(Ctx ctx, Trx trxName) 
	{
		List<MWikiToken> list = new List<MWikiToken>();
		String sql = "SELECT * FROM CM_WikiToken WHERE Ad_Client_ID=0 AND isActive='Y' ORDER By SeqNo";
		//PreparedStatement pstmt = null;
        IDataReader idr = null;
		try
		{
			//pstmt = DataBase.prepareStatement(sql, trxName);
			//ResultSet rs = pstmt.executeQuery();
            idr = DataBase.DB.ExecuteReader(sql, null, trxName);
            while (idr.Read())
            {
                list.Add(new MWikiToken(ctx, idr, trxName));
            }
            idr.Close();
		}
		catch (Exception e)
		{
            if (idr != null)
            {
                idr = null;
            }
			_log.Log(Level.SEVERE, sql, e);
		}
		MWikiToken[] retValue = new MWikiToken[list.Count];
        retValue=list.ToArray();
		return retValue;
	}
	
	/**
	 * 	process any token to a StringBuffer, the different types need to handle the results different.
	 *	@param source
	 *	@param CM_WebProject_ID
	 *	@return converted StringBuffer
	 */
	public StringBuilder ProcessToken(StringBuilder source, int CM_WebProject_ID, String MediaPath)
	{
		if (GetTokenType ().Equals (X_CM_WikiToken.TOKENTYPE_Style))
        {
			Matcher matcher = GetPattern ().matcher(source.ToString());
			source = new StringBuilder(matcher.replaceAll (GetMacro ()));
		} 
        else if (GetTokenType ().Equals (X_CM_WikiToken.TOKENTYPE_SQLCommand)) 
        {
			
		}
        else if (GetTokenType ().Equals (X_CM_WikiToken.TOKENTYPE_ExternalLink))
        {
			Matcher matcher = GetPattern ().matcher(source.ToString());
			source = new StringBuilder(matcher.replaceAll (GetMacro ()));
		} 
        else if (GetTokenType().Equals (X_CM_WikiToken.TOKENTYPE_InternalLink))
        {
			Matcher matcher = GetPattern ().matcher (source.ToString());
			while(matcher.find ()) 
            {
				if (matcher.group(1).Equals ("Media:")) 
				{
					String Name = matcher.group (2);
					MMedia thisMedia = MMedia.GetByName (GetCtx(), Name, CM_WebProject_ID, null);
					String replaceString = "";
					if (thisMedia != null)
					{
						if (matcher.groupCount ()>2)
							replaceString = "<img src=\"" + MediaPath + thisMedia.GetFileName () + "\" alt=\"" + matcher.group (3) + "\"/>";
					}
					source = new StringBuilder(matcher.replaceFirst (replaceString));
					matcher = GetPattern ().matcher (source.ToString());
				} 
                else
                {
					String link = matcher.group (1);
					MContainer thisContainer = MContainer.GetByName(GetCtx(), link, CM_WebProject_ID, null);
					if (thisContainer==null) 
						thisContainer = MContainer.GetByTitle(GetCtx(), link, CM_WebProject_ID, null);
					String replaceURL = "/index.html";
					if (thisContainer != null)
					{
						if (matcher.groupCount ()>1)
							replaceURL = "<a href=\"" + thisContainer.GetRelativeURL () + "\">" + matcher.group (2) + "</a>";
						else
							replaceURL = "<a href=\"" + thisContainer.GetRelativeURL () + "\">" + matcher.group (1) + "</a>";
					}
					source = new StringBuilder(matcher.replaceFirst (replaceURL));
					matcher = GetPattern ().matcher (source.ToString());
				}
			}
			
		} 
		return source;
	}
}	
}
