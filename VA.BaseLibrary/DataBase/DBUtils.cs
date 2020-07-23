/********************************************************
 * Module/class Name    : Database Utility
 * Purpose        : Convert the query from oracle to target database
 *                  
 * Chronological Development
 * Mukesh Arora     8-Dec-2008
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.DataBase
{
    class DBUtils
    {/**	Logger	*/
	//private static CLogger	log	= CLogger.getCLogger (DBUtils.class);

	
	    /// <summary>
        /// change update set (...) = (select ... from ) standard format 
        /// </summary>
        /// <param name="sql">string sql update clause </param>
        /// <returns>new sql string</returns>
	public static String UpdateSetSelectList (String sql)
	{
		StringBuilder newSQL = null;
		int iSet = sql.IndexOf(" SET (");
        int iSetEnd;

        if (iSet == -1)
        { 
            iSetEnd = sql.IndexOf(")", 0); 
        }
        else
        { 
            iSetEnd = sql.IndexOf(")", iSet); 
        }

        int iSelect;

        if (iSetEnd == -1)
        {
            iSelect = sql.IndexOf("(SELECT ", 0);
        }
        else 
		{ 
            iSelect = sql.IndexOf("(SELECT ",iSetEnd);
        }


        int iSelectEnd;
        if (iSelect == -1)
        { 
            iSelectEnd = sql.IndexOf(" FROM ", 0); 
        }
        else
        { 
            iSelectEnd = sql.IndexOf(" FROM ", iSelect); 
        }
            

        
		int il = 1;
		int ir = 0;
		int isql = sql.Length;
		
		if (iSet == -1 || iSetEnd == -1 || iSelect == -1 || iSelectEnd == -1)
			return sql;
		
		//get lists
        String[] setList = sql.Substring(iSet + 6, iSetEnd - (iSet + 6)).Split(',');
		
		String[] selectList = null;
		String selListStr = sql.Substring(iSelect+7,iSelectEnd - (iSelect + 7));
		//if (selListStr.indexOf("), ")>0)
		if (selListStr.IndexOf(")")>0)
			selectList = selListStr.Split(',');
		else
			selectList = selListStr.Split(',');
		
		//get subQuery end
		int end = iSelectEnd+5;
		while (il > ir && ++end < isql)
		{
            if (sql.Substring(end,1) == ")")
                ir++;
            else if (sql.Substring(end, 1) == "(")
                il++;

            //if (sql.charAt(end)==')')
            //    ir++;
            //else if (sql.charAt(end)=='(')
            //    il++;
		}
		if (end < isql)
			end++;
		
		//construct new sql
		newSQL = new StringBuilder(sql.Substring(0,iSet+5));
		
		for (int i=0; i<setList.Length; i++)
		{
			if (i>0)
				newSQL.Append(" , ");
			newSQL.Append(setList[i]+" = (SELECT "+selectList[i]+" "+sql.Substring(iSelectEnd, end-iSelectEnd));
		}
		
		if (end <=isql)
			newSQL.Append(sql.Substring(end, isql-end));

		return newSQL.ToString();		
	}   //  

	
    }
}
