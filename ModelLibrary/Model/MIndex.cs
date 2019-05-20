/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MIndex
 * Purpose        : Text Index Model
 * Class Used     :  X_K_Index
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
using System.Text.RegularExpressions;

namespace VAdvantage.Model
{
 public class MIndex : X_K_Index
{
	/// <summary>
	/// Standard Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="K_Index_ID">id</param>
	/// <param name="trxName">trx</param>
	public MIndex (Ctx ctx, int K_Index_ID, Trx trxName):base (ctx, K_Index_ID, trxName)
	{
		
	}	//	MIndex

	/// <summary>
	/// Load Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="rs">datarow</param>
	/// <param name="trxName">trx</param>
	public MIndex (Ctx ctx, DataRow dr, Trx trxName):base (ctx, dr, trxName)
	{
		
	}	//	MIndex
     public MIndex (Ctx ctx,IDataReader idr, Trx trxName):base (ctx, idr, trxName)
     {}
	
	/// <summary>
	/// cleanUp
	/// </summary>
	/// <param name="trxName">trxname</param>
	/// <param name="AD_Client_ID">id</param>
	/// <param name="AD_Table_ID">id</param>
	/// <param name="Record_ID">id</param>
	/// <returns>Number of records cleaned</returns>
	public static int CleanUp(Trx trxName, int AD_Client_ID, int AD_Table_ID, int Record_ID)
	{
		StringBuilder sb = new StringBuilder ("DELETE FROM K_Index "
			+ "WHERE AD_Client_ID=" + AD_Client_ID + " AND "
			+ "AD_Table_ID=" + AD_Table_ID + " AND "
			+ "Record_ID=" + Record_ID);
		int no = DataBase.DB.ExecuteQuery(sb.ToString(),null, trxName);
		return no;
	}
	
	/// <summary>
	/// Set Keyword & standardize
	/// </summary>
	/// <param name="Keyword">keyword</param>
	public new void SetKeyword (String Keyword)
	{
		String kw = StandardizeKeyword(Keyword);
		if (kw == null)
			kw = "?";
		base.SetKeyword (kw);
	}	//	setKeyword
	
	/// <summary>
	/// 	Before Save
	/// </summary>
	/// <param name="newRecord">new</param>
	/// <returns>true</returns>
	protected override bool BeforeSave (bool newRecord)
	{
		if (newRecord || Is_ValueChanged("Keyword"))
			SetKeyword(GetKeyword());
		if (GetKeyword().Equals("?"))
		{
			log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "Keyword"));
			return false;
		}
		return true;
	}	//	beforeSave

     /// <summary>
     ///  Index/Keyword a StringBuffer
     /// </summary>
     /// <param name="thisText">The String to convert into Hash</param>
     /// <returns>Hashtable with String</returns>
   	//public static Hashtable indexStringBuffer(StringBuffer thisText)
     public static System.Collections.IDictionary IndexStringBuffer(StringBuilder thisText)
	{
		return IndexString(thisText.ToString());
	}
	
	/// <summary>
	/// /// <summary>
     ///  Index/Keyword a StringBuffer
     /// </summary>
     /// <param name="thisText">The String to convert into Hash</param>
     /// <returns>Hashtable with String</returns>
	/// </summary>
	/// <param name="thisText"></param>
	/// <returns></returns>
	//public static Hashtable<String,String> indexString(String thisText)
     public static System.Collections.Generic.Dictionary<String,String> IndexString(String thisText)
	{
 		thisText = RemoveHTML(thisText);
		if (thisText!=null) 
        {
            char[] delimiters = new char[] {'\t','\''};
            //String value = "\\t|\\p{Punct}|\\p{Space}";
            String[] keywords = thisText.ToUpper().Split(delimiters,StringSplitOptions.None);
            
            //String[] keywords = thisText.ToUpper().Split(value, "'\\t|\\p{Punct}|\\p{Space}'");
            System.Collections.Generic.Dictionary<String,String> keyhash = new System.Collections.Generic.Dictionary<String,String>(keywords.Length);
			int currentPos = 0;
			for (int i=0;i<keywords.Length;i++)
			{	
				String thisExcerpt = "";
				if (keywords[i].Length>=2) 
				{
					if (!keyhash.ToString().Contains(keywords[i]))
					{
						int startExcerpt = thisText.ToUpper().IndexOf(keywords[i],currentPos);
						if (startExcerpt>50)
                        {
							startExcerpt = startExcerpt - 50;
                        }
						if (startExcerpt>thisText.Length && startExcerpt>50 && thisText.Length>50) 
                        {
							startExcerpt = thisText.Length -50;
                        }
						int endExcerpt = thisText.ToUpper().IndexOf(keywords[i],currentPos) + keywords[i].Length;
						if (endExcerpt>currentPos)
							currentPos = endExcerpt;
						if (endExcerpt<thisText.Length -50)
							endExcerpt = endExcerpt + 50;
						if (endExcerpt>thisText.Length) 
							endExcerpt = thisText.Length;
						thisExcerpt = thisText.Substring(startExcerpt,endExcerpt);
						keyhash.Add(keywords[i],thisExcerpt);
					}
				}
			}
			return keyhash;
		} 
        else
        {
			return null;
		}
			
	}
	
	/// <summary>
	/// runIndex
	/// </summary>
	/// <param name="thisText">The text to be indexed</param>
	/// <param name="ctx">context</param>
	/// <param name="trxName">trx if needed</param>
	/// <param name="tableID">id</param>
	/// <param name="recordID">id</param>
	/// <param name="CMWebProjectID">id</param>
	/// <param name="sourceUpdated">update date of source</param>
	/// <returns>true if successfully indexed</returns>
	public static bool RunIndex(String thisText, Ctx ctx, Trx trxName, 
		int tableID, int recordID, int CMWebProjectID, DateTime? sourceUpdated) 
	{
		if (thisText!=null) {
			//Hashtable keyHash = indexString(thisText);
            System.Collections.IDictionary keyHash = IndexString(thisText);
            for (System.Collections.IEnumerator e = (System.Collections.IEnumerator)keyHash.Keys; e.MoveNext(); )
            {
	            String name = (String)e.Current;
	            String value = (String)keyHash[name];//.get(name);
	            MIndex thisIndex = new MIndex(ctx, 0, trxName);
	            thisIndex.SetAD_Table_ID(tableID);
	            if (CMWebProjectID>0)
                {
	                thisIndex.SetCM_WebProject_ID(CMWebProjectID);
                }
	            thisIndex.SetExcerpt(value);
	            thisIndex.SetKeyword(name);
	            thisIndex.SetRecord_ID(recordID);
	            thisIndex.SetSourceUpdated(sourceUpdated);
	            thisIndex.Save();
	        }
	        return true;
		}
        else 
        {
			return false;
		}
	}
	
	
	/// <summary>
	/// reIndex Document
	/// </summary>
	/// <param name="runCleanUp">clean old records</param>
	/// <param name="toBeIndexed">Array of Strings to be indexed</param>
	/// <param name="ctx">context</param>
	/// <param name="AD_Client_ID">id</param>
	/// <param name="AD_Table_ID">id</param>
	/// <param name="Record_ID">id</param>
	/// <param name="CM_WebProject_ID">webproject</param>
	/// <param name="lastUpdated">date of last update</param>
	public static void ReIndex(bool runCleanUp, String[] toBeIndexed, Ctx ctx, 
		int AD_Client_ID, int AD_Table_ID, int Record_ID, int CM_WebProject_ID, DateTime? lastUpdated) 
	{
		Trx trx = Trx.Get("ReIndex_" + AD_Table_ID + "_" + Record_ID);
		try {
			if (!runCleanUp)
			{
				MIndex.CleanUp(trx, AD_Client_ID, AD_Table_ID, Record_ID);
			}
			for (int i=0;i<toBeIndexed.Length;i++) 
            {
				MIndex.RunIndex(toBeIndexed[i], ctx, trx, AD_Table_ID, Record_ID, 
				CM_WebProject_ID, lastUpdated);
			
            }

            trx.Commit();
			//DataBase.DB.Commit (true, trx);
            
		} 
        catch 
        {
			try 
            {
				//DataBase.DB.Rollback(true, trx);
                trx.Rollback();
			} 
            catch 
            {
			}
		}
	}
	
	/// <summary>
	/// remove HTML Tags from content...
	/// </summary>
	/// <param name="textElement">textElement</param>
	/// <returns>cleanText</returns>
	protected static String RemoveHTML(String textElement) 
    {
		String retValue = textElement.Replace("<?\\w+((\\s+\\w+(\\s*=\\s*(?:\"b(.|\\n)*?\"|'(.|\\n)*?'|[^'\">\\s]+))?)+\\s*|\\s*)/?>", " ");
		retValue = retValue.Replace ("</", " ");
		retValue = retValue.Replace ("\\\\n", " ");
		return retValue.ToString();
	}
	
	
	/// <summary>
	/// Clean up & standardize Keyword
	/// </summary>
	/// <param name="keyword">keyword</param>
	/// <returns>keyword or null</returns>
	public static String StandardizeKeyword (String keyword)
	{
		if (keyword == null)
			return null;
		keyword = keyword.Trim();
		if (keyword.Length == 0)
			return null;
		//
		keyword = keyword.ToUpper();	//	default locale
		StringBuilder sb = new StringBuilder();
		char[] chars = keyword.ToCharArray();
		for (int i = 0; i < chars.Length; i++)
		{
			char c = chars[i];
			sb.Append(StandardizeCharacter(c));
		}
		return sb.ToString();
	}	//	standardizeKeyword
	
	/// <summary>
	/// Standardize Character
	/// </summary>
	/// <param name="c">character</param>
	/// <returns>string</returns>
	public static String StandardizeCharacter (char c)
	{
		if (c >= '!' && c <= 'Z')		//	includes Digits
			return Utility.Util.GetValueOfString(c);
		//
		int index =Array.BinarySearch(_char, c);
		if (index < 0)
			return Utility.Util.GetValueOfString(c);
		return _string[index];
	}	//	standardizeKeyword
	
	/**	Static Logger				*/
	private static VLogger		_log = VLogger.GetVLogger (typeof(MIndex).FullName);//.class);
	
	public static MIndex[] GetResults(String query, Ctx ctx, Trx trxName) 
	{
		String sql = "SELECT * FROM K_Index WHERE K_Index_ID IN (" +
				"SELECT MAX(K_Index_ID) FROM K_Index WHERE " +
				  "Keyword LIKE @param GROUP BY AD_Table_ID, Record_ID)";
		System.Collections.Generic.Dictionary<int,MIndex> tTable = new System.Collections.Generic.Dictionary<int,MIndex>();
		SqlParameter[] param=new SqlParameter[1];
        IDataReader idr=null;
		try
		{
			// First lookup full word
			//pstmt = DataBase.prepareStatement (sql, trxName);
			//pstmt.setString (1, query.toUpperCase ());
			param[0]=new SqlParameter("@param",query.ToUpper());
            idr=DataBase.DB.ExecuteReader(sql,param,trxName);
			while (idr.Read())
			{
				MIndex tIndex = new MIndex(ctx, idr, trxName);
				tTable.Add(tIndex.Get_ID (), tIndex);
			}
			idr.Close();
			// Second lookup with appended %
			//pstmt = DataBase.prepareStatement (sql, trxName);
			//pstmt.setString (1, query.toUpperCase () + "%");
            param[0]=new SqlParameter("@param",query.ToUpper() + "%");
            idr=DataBase.DB.ExecuteReader(sql,param,trxName);
			while (idr.Read())
			{
				MIndex tIndex = new MIndex(ctx, idr, trxName);
				if(!tTable.ContainsKey(tIndex.Get_ID ()))
                {
					tTable.Add(tIndex.Get_ID (), tIndex);
				}
			}
			idr.Close();
			// Third lookup with prefix% and appended %
			//pstmt = DataBase.prepareStatement (sql, trxName);
			//pstmt.setString (1, "%" + query.toUpperCase () + "%");
			param[0]=new SqlParameter("@param","%" + query.ToUpper() + "%");
            idr=DataBase.DB.ExecuteReader(sql,param,trxName);
			while (idr.Read())
			{
				MIndex tIndex = new MIndex(ctx, idr, trxName);
				if(!tTable.ContainsKey(tIndex.Get_ID ()))
                {
					tTable.Add(tIndex.Get_ID (), tIndex);
				}
			}
			idr.Close();
		}
		catch (Exception e)
		{
            if(idr!=null)
            {
                idr.Close();
            }
			_log.Log(Level.SEVERE, "getResults", e);
		}
		
		// Log the search and the number of results
		MIndexLog thisLog = new MIndexLog(ctx,0,trxName);
		thisLog.SetIndexQuery (query);
		thisLog.SetIndexQueryResult (tTable.Count);
		thisLog.SetQuerySource (X_K_IndexLog.QUERYSOURCE_CollaborationManagement);
		thisLog.Save ();
		
		MIndex [] entries = new MIndex [tTable.Count];
		//Enumeration E = tTable.keys ();
        System.Collections.IEnumerator E = tTable.Keys.GetEnumerator();           
		int i = 0;
		while (E.MoveNext())
        {
            entries[i++] = tTable[Utility.Util.GetValueOfInt(E.Current)];
        }
		return entries;
	}

	
	  	/// <summary>
	  	/// Foreign upper case characters ascending for binary Search.
	  	//Make sure that you use native2ascii to convert
	  	//(note Eclipse, etc. actually save the content as utf-8 - so the command is
	  	//native2ascii -encoding utf-8 filename)
	  	/// </summary>   
    
	private static  char[] _char 
		= new char[] 
        {
			'\u00c4', '\u00d6', '\u00dc', '\u00df'	//	Ã„Ã–ÃœÃŸ - German stuff
		};
	
	/// <summary>
	/// Foreign character string linked to s_char position
	/// </summary>
	private static  String[] _string 
		= new String[]
        {
			"AE", "OE", "UE", "SS"
		};
	
}	//	MIndex

}
