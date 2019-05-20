

using java.util.regex;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VAdvantage.Logging;


namespace VAdvantage.DBPort
{
    /// <summary>
    /// Convert SQL to Target DB
    /// </summary>
    public abstract class ConvertSQL
    {
        /** RegEx: insensitive and dot to include line end characters   */
        public static System.Text.RegularExpressions.RegexOptions REGEX_FLAGS = System.Text.RegularExpressions.RegexOptions.IgnoreCase | RegexOptions.Singleline;

	/** Statement used                  */
	protected string               m_stmt = null;

	/** Last Conversion Error           */
	protected String                  m_conversionError = null;
	/** Last Execution Error            */
	protected Exception               m_exception = null;
	/** Verbose Messages                */
	protected bool              m_verbose = true;

        protected bool m_isOracle = false;

	/**	Logger	*/
	private static VLogger	log	= VLogger.GetVLogger (typeof(ConvertSQL).FullName);
	
    //private static Stream tempFileOr = null;
    //private static StreamWriter writerOr;
    //private static Stream tempFilePg = null;
    //private static StreamWriter writerPg;
    //private static Stream tempFileMySQL = null;
    //private static StreamWriter writerMySQL;

    ///// <summary>
    ///// Set Verbose
    ///// </summary>
    ///// <param name="verbose"></param>
    //public void SetVerbose (bool verbose)
    //{
    //    m_verbose = verbose;
    //}   //  setVerbose

    /// <summary>
    ///True if the database support native oracle dialect, false otherwise.
    /// </summary>
    /// <returns></returns>
	
        public  bool IsOracle()
        {
            return m_isOracle;
        }


/// <summary>
///  Return last execution exception
/// </summary>
/// <returns>execution exception</returns>
	public Exception GetException()
	{

		return m_exception;
	}

	/// <summary>
	/// Returns true if a conversion or execution error had occurred.
	/// </summary>
	/// <returns>true if error had occurred</returns>
	public bool HasError()
	{
		return (m_exception != null) | (m_conversionError != null);
	}   //  hasError

	/// <summary>
	/// Convert SQL Statement (stops at first error).
	///  Statements are delimited by /
	///   If an error occurred hadError() returns true.
	///  You can get details via getConversionError()
	/// </summary>
	/// <param name="sqlStatements">qlStatements</param>
	/// <returns>converted statement as a string</returns>
	public String ConvertAll (String sqlStatements)
	{
		String[] sql = Convert (sqlStatements);
		StringBuilder sb = new StringBuilder (sqlStatements.Length + 10);
		for (int i = 0; i < sql.Length; i++)
		{
			//  line.separator
			sb.Append(sql[i]).Append("\n/\n");
			if (m_verbose)
				log.Info("Statement " + i + ": " + sql[i]);
		}
		return sb.ToString();
	}   //  convertAll

	/// <summary>
	/// Convert SQL Statement (stops at first error).
	///  If an error occurred hadError() returns true.
	///  You can get details via getConversionError()
	/// </summary>
	/// <param name="sqlStatements"></param>
	/// <returns></returns>
	public String[] Convert (String sqlStatements)
	{
		m_conversionError = null;
		if (sqlStatements == null || sqlStatements.Length == 0)
		{
			m_conversionError = "SQL_Statement is null or has zero length";
			log.Info(m_conversionError);
			return null;
		}
		//
		return ConvertIt (sqlStatements);
	}   //  convert

	/// <summary>
	/// Return last conversion error or null.
	/// </summary>
	/// <returns>lst conversion error</returns>
	public String GetConversionError()
	{
		return m_conversionError;
	}   //  getConversionError

	
	/// <summary>
	/// Conversion routine (stops at first error).
	 ///  <pre>
	///  - convertStatement
	 ///     - convertWithConvertMap
	 ///     - convertComplexStatement
	 ///     - decode, sequence, exception
	 /// </pre>
	/// </summary>
	/// <param name="sqlStatements">sqlStatements</param>
	/// <returns>array of converted statements</returns>
	protected String[] ConvertIt (String sqlStatements)
	{
		List<String> result = new List<String> ();
		result.AddRange(ConvertStatement(sqlStatements));     //  may return more than one target statement
		
		//  convert to array
		//String[] sql = new String[result.size()];
		return  result.ToArray();
	}   //  convertIt

	/// <summary>
	/// Clean up Statement. Remove trailing spaces, carriage return and tab 
	/// </summary>
	/// <param name="statement">statement</param>
	/// <returns>sql statement</returns>
	protected String CleanUpStatement(String statement) {
		String clean = statement.Trim();

		// Convert cr/lf/tab to single space
        clean = Regex.Replace(clean, "\\s+", " ", REGEX_FLAGS);
        
		clean = clean.Trim();
		return clean;
	} // removeComments
	
	/// <summary>
	/// Utility method to replace quoted string with a predefined marker
	/// </summary>
	/// <param name="inputValue"></param>
	/// <param name="retVars"></param>
	/// <returns></returns>
	protected virtual String ReplaceQuotedStrings(String inputValue, List<String>retVars) {
		// save every value  
		// Carlos Ruiz - globalqss - better matching regexp
		retVars.Clear();
		
		// First we need to replace double quotes to not be matched by regexp - Teo Sarca BF [3137355 ]
		String quoteMarker = "<--QUOTE"+  DateTime.Now.Ticks+"-->";
		inputValue = inputValue.Replace("''", quoteMarker);
		
     

		//Pattern p = Pattern.compile("'[[^']*]*'");
	//	Matcher m = p.matcher(inputValue);

        MatchCollection mc = Regex.Matches(inputValue, "'[[^']*]*'");

		int i = 0;
        StringBuilder retValue = new StringBuilder(inputValue.Length);
        int last = 0;
		foreach (Match m in mc)
        {
			String var = inputValue.Substring(m.Index, m.Index + m.Length).Replace(quoteMarker, "''"); // Put back quotes, if any
			retVars.Add(var);
			retValue.Append(m.Result("<--" + i + "-->"));
			i++;
            last = m.Index + m.Length;
		}
		retValue.Append(inputValue.Substring(last));
        return retValue.ToString()
            .Replace(quoteMarker, "''"); // Put back quotes, if any
	}

	/// <summary>
	/// Utility method to recover quoted string store in retVars
	/// </summary>
	/// <param name="retValue"></param>
	/// <param name="retVars"></param>
	/// <returns></returns>
	protected String RecoverQuotedStrings(String retValue, List<String>retVars) {
        StringBuilder sb = new StringBuilder();
		for (int i = 0; i < retVars.Count; i++) {
			//hengsin, special character in replacement can cause exception
			String replacement = retVars[i];
			replacement = EscapeQuotedString(replacement);
			retValue = retValue.Replace("<--" + i + "-->", replacement);
		}
		return retValue;
	}
	
	/// <summary>
	///hook for database specific escape of quoted string ( if needed )
	/// </summary>
	/// <param name="inn"></param>
	/// <returns></returns>
	protected virtual String EscapeQuotedString(String inn)
	{
		return inn;
	}
	
	/**
	 * Convert simple SQL Statement. Based on ConvertMap
	 * 
	 * @param sqlStatement
	 * @return converted Statement
	 */
	private String ApplyConvertMap(String sqlStatement) {
		// Error Checks
		if (sqlStatement.ToUpper().IndexOf("EXCEPTION WHEN") != -1) {
			String error = "Exception clause needs to be converted: "
					+ sqlStatement;
			log.Info(error);
			m_conversionError = error;
			return sqlStatement;
		}

		// Carlos Ruiz - globalqss
		// Standard Statement -- change the keys in ConvertMap
		
		String retValue = sqlStatement;

		
		// for each iteration in the conversion map
	IDictionary convertMap = GetConvertMap();
		if (convertMap != null) {
			var iter = convertMap.Keys.GetEnumerator();
			while (iter.MoveNext()) {
	
			    // replace the key on convertmap (i.e.: number by numeric)   
				String regex = (String) iter.Current;
				String replacement = (String) convertMap[regex];
				try {

                    //Regex e = new Regex(regex, REGEX_FLAGS | RegexOptions.Compiled);
					//p = Pattern.compile(regex, REGEX_FLAGS);
                    retValue = Regex.Replace(retValue,regex, replacement,REGEX_FLAGS);
					//m = p.matcher(retValue);
					//retValue = m.replaceAll(replacement);
	
				} catch (Exception e) {
					String error = "Error expression: " + regex + " - " + e;
					log.Info(error);
					m_conversionError = error;
				}
			}
		}
		return retValue;
	} // convertSimpleStatement
	
        /// <summary>
    /// do convert map base conversion
        /// </summary>
        /// <param name="sqlStatement"></param>
        /// <returns></returns>
	protected String ConvertWithConvertMap(String sqlStatement) {
		try 
		{
			sqlStatement = ApplyConvertMap(CleanUpStatement(sqlStatement));
		}
		catch (Exception e) {
			log.Warning(e.Message);
			m_exception = e;
		}
		
		return sqlStatement;
	}

   /// <summary>
   ///Get convert map for use in sql convertion
 
   /// </summary>
   /// <returns></returns>
    protected virtual IDictionary GetConvertMap()
    {
        return null;
    }
	
	
	/// <summary>
	/// Convert single Statements.
	 ///  - remove comments
	///      - process FUNCTION/TRIGGER/PROCEDURE
	 ///     - process Statement
	/// </summary>
	/// <param name="sqlStatement">sqlStatement</param>
	/// <returns>converted statement</returns>
    protected abstract List<String> ConvertStatement(String sqlStatement);

	

	private static bool DontLog(String statement) {
		String [] exceptionTables = new String[] {
				"AD_ACCESSLOG",
				"AD_ALERTPROCESSORLOG",
				"AD_CHANGELOG",
				"AD_ISSUE",
				"AD_LDAPPROCESSORLOG",
				"AD_PACKAGE_IMP",
				"AD_PACKAGE_IMP_BACKUP",
				"AD_PACKAGE_IMP_DETAIL",
				"AD_PACKAGE_IMP_INST",
				"AD_PACKAGE_IMP_PROC",
				"AD_PINSTANCE",
				"AD_PINSTANCE_LOG",
				"AD_PINSTANCE_PARA",
				"AD_REPLICATION_LOG",
				"AD_SCHEDULERLOG",
				"AD_SESSION",
				"AD_WORKFLOWPROCESSORLOG",
				"CM_WEBACCESSLOG",
				"C_ACCTPROCESSORLOG",
				"K_INDEXLOG",
				"R_REQUESTPROCESSORLOG",
				"T_AGING",
				"T_ALTER_COLUMN",
				"T_DISTRIBUTIONRUNDETAIL",
				"T_INVENTORYVALUE",
				"T_INVOICEGL",
				"T_REPLENISH",
				"T_REPORT",
				"T_REPORTSTATEMENT",
				"T_SELECTION",
				"T_SELECTION2",
				"T_SPOOL",
				"T_TRANSACTION",
				"T_TRIALBALANCE",
				// Do not log *Access records - teo_Sarca BF [ 2782095 ]
				"AD_PROCESS_ACCESS",
				"AD_WINDOW_ACCESS",
				"AD_WORKFLOW_ACCESS",
				"AD_FORM_ACCESS",
				//
			};
		String uppStmt = statement.ToUpper().Trim();
		// don't log selects
		if (uppStmt.StartsWith("SELECT "))
			return true;
		// don't log update to statistic process
		if (uppStmt.StartsWith("UPDATE AD_PROCESS SET STATISTIC_"))
			return true;
		// Don't log DELETE FROM Some_Table WHERE AD_Table_ID=? AND Record_ID=?
		if (uppStmt.StartsWith("DELETE FROM ") && uppStmt.EndsWith(" WHERE AD_TABLE_ID=? AND RECORD_ID=?"))
			return true;
		for (int i = 0; i < exceptionTables.Length; i++) {
			if (uppStmt.StartsWith("INSERT INTO " + exceptionTables[i] + " "))
				return true;
			if (uppStmt.StartsWith("DELETE FROM " + exceptionTables[i] + " "))
				return true;
			if (uppStmt.StartsWith("DELETE " + exceptionTables[i] + " "))
				return true;
			if (uppStmt.StartsWith("UPDATE " + exceptionTables[i] + " "))
				return true;
			if (uppStmt.StartsWith("INSERT INTO " + exceptionTables[i] + "("))
				return true;
		}
		
		// don't log selects or insert/update for exception tables (i.e. AD_Issue, AD_ChangeLog)
		return false;
	}

	
    }
}
