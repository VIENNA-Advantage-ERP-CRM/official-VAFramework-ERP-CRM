using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;

namespace VAdvantage.DBPort
{
    /// <summary>
    /// Convert Oracle SQL to PostgreSQL SQL
    /// </summary>
    public class ConvertSQL_PostgreSQL : ConvertSQL_SQL92
    {

        /// <summary>
        /// 
        /// </summary>
        public ConvertSQL_PostgreSQL()
        {
            m_map = ConvertMap_PostgreSQL.GetConvertMap();
            m_isOracle = false;
        } // Convert

        /** RegEx: insensitive and dot to include line end characters */
        //public static RegexOptions REGEX_FLAGS = RegexOptions.IgnoreCase | RegexOptions.Singleline;

        private IDictionary<string, string> m_map;

        /** Logger */
        private static VLogger log = VLogger.GetVLogger(typeof(ConvertSQL_PostgreSQL).FullName);




        protected override List<string> ConvertStatement(string sqlStatement)
        {
            List<String> result = new List<String>();
            /** Vector to save previous values of quoted strings **/
            List<String> retVars = new List<String>();

            //Validate Next ID Function and use Native Sequence if the functionality is active
            int found_next_fuction = sqlStatement.ToUpper().IndexOf("NEXTIDFUNC(");
            if (found_next_fuction <= 0)
                found_next_fuction = sqlStatement.ToUpper().IndexOf("NEXTID(");
            if (found_next_fuction > 0)
            {

            }

            String statement = ReplaceQuotedStrings(sqlStatement, retVars);
            statement = ConvertWithConvertMap(statement);
            statement = statement.Replace(DB_PostgreSQL.NATIVE_MARKER, "");

            String cmpString = statement.ToUpper();
            bool isCreate = cmpString.StartsWith("CREATE ");

            // Process
            if (isCreate && cmpString.IndexOf(" FUNCTION ") != -1)
            {
                ;
            }
            else if (isCreate && cmpString.IndexOf(" TRIGGER ") != -1)
            { ;}
            else if (isCreate && cmpString.IndexOf(" PROCEDURE ") != -1)
            { ;}
            else if (isCreate && cmpString.IndexOf(" VIEW ") != -1)
            { ;}
            else if (cmpString.IndexOf("ALTER TABLE") != -1)
            {
                statement = RecoverQuotedStrings(statement, retVars);
                retVars.Clear();
                //statement = ConvertDDL(ConvertComplexStatement(statement));
                statement = ConvertComplexStatement(statement);
                /*
                } else if (cmpString.indexOf("ROWNUM") != -1) {
                    result.add(convertRowNum(convertComplexStatement(convertAlias(statement))));*/
            }
            else if (cmpString.IndexOf("DELETE ") != -1
                  && cmpString.IndexOf("DELETE FROM") == -1)
            {
                statement = ConvertDelete(statement);
                statement = ConvertComplexStatement(ConvertAlias(statement));
            }
            else if (cmpString.IndexOf("DELETE FROM") != -1)
            {
                statement = ConvertComplexStatement(ConvertAlias(statement));
            }
            else if (cmpString.IndexOf("UPDATE ") != -1)
            {
                statement = ConvertComplexStatement(ConvertUpdate(ConvertAlias(statement)));
            }
            else
            {
                statement = ConvertComplexStatement(ConvertAlias(statement));
            }
            if (retVars.Count > 0)
                statement = RecoverQuotedStrings(statement, retVars);
            result.Add(statement);

            return result;
        }

       


        protected override IDictionary GetConvertMap()
        {
            return (IDictionary)m_map;
        }

     
	protected override String EscapeQuotedString(String inStr)
	{
		StringBuilder oStr = new StringBuilder();
		bool escape = false;
		int size = inStr.Length;
		for(int i = 0; i < size; i++) {
			char c = inStr[i];
			oStr.Append(c);
			if (c == '\\')
			{
				escape  = true;
				oStr.Append(c);
			}
		}
		if (escape)
		{
			return "E" + oStr.ToString();
		}
		else
		{
			return oStr.ToString();
		}
	}

        /// <summary>
        /// Converts Decode and Outer Join.
        /// <pre>
        ///        DECODE (a, 1, 'one', 2, 'two', 'none')
        ///         =&gt; CASE WHEN a = 1 THEN 'one' WHEN a = 2 THEN 'two' ELSE 'none' END
        ///  
        /// </pre>
        /// </summary>
        /// <param name="sqlStatement">sqlStatement</param>
        /// <returns>converted statement</returns>
        protected String ConvertComplexStatement(String sqlStatement)
        {
            String retValue = sqlStatement;
            StringBuilder sb = null;

            // Convert all decode parts
            int found = retValue.ToUpper().IndexOf("DECODE");
            int fromIndex = 0;
            while (found != -1)
            {
                retValue = ConvertDecode(retValue, fromIndex);
                fromIndex = found + 6;
                found = retValue.ToUpper().IndexOf("DECODE", fromIndex);
            }

            // Outer Join Handling -----------------------------------------------
            int index = retValue.ToUpper().IndexOf("SELECT ");
            if (index != -1 && retValue.IndexOf("(+)", index) != -1)
                retValue = ConvertOuterJoin(retValue);

            // Convert datatypes from CAST(.. as datatypes):
            retValue = ConvertCast(retValue);

            return retValue;
        } // convertComplexStatement

        /// <summary>
        /// Convert datatypes from CAST sentences
        /// <pre>
        /// 		cast(NULL as NVARCHAR2(255))
        /// 		=&gt;cast(NULL as VARCHAR)
        /// </pre>
        ///
        /// </summary>
        /// <param name="sqlStatement"></param>
        /// <returns></returns>
        private String ConvertCast(String sqlStatement)
        {
            String PATTERN_String = "\'([^']|(''))*\'";
            String PATTERN_DataType = "([\\w]+)(\\(\\d+\\))?";
            String pattern =
                               "\\bCAST\\b[\\s]*\\([\\s]*"					// CAST<sp>(<sp>		
                               + "((" + PATTERN_String + ")|([^\\s]+))"		//	arg1				1(2,3)
                               + "[\\s]*AS[\\s]*"						//	<sp>AS<sp>
                               + "(" + PATTERN_DataType + ")"				//	arg2 (datatype)		4
                               + "\\s*\\)"								//	<sp>)
           ;
            int gidx_arg1 = 1;
            int gidx_arg2 = 7;	// datatype w/o length
            // Pattern p = Pattern.compile(pattern, Pattern.CASE_INSENSITIVE);
            MatchCollection mc = Regex.Matches(sqlStatement, pattern,RegexOptions.IgnoreCase);

            IDictionary convertMap = GetConvertMap();
            StringBuilder retValue = new StringBuilder(sqlStatement.Length);
            int last = 0;
            foreach(Match m in mc)
            {
                String arg1 = m.Groups[gidx_arg1].Value;
                String arg2 = m.Groups[gidx_arg2].Value;
                //
                String datatype = (String)convertMap["\\b" + arg2.ToUpper() + "\\b"];
                if (datatype == null)
                    datatype = arg2;
                 retValue.Append(sqlStatement.Substring(last, m.Index - last));
                 retValue.Append(m.Result("cast(" + arg1 + " as " + datatype + ")"));
                 last = m.Index + m.Length;
            }
            retValue.Append(sqlStatement.Substring(last));
            return retValue.ToString();
        }


        /// <summary>
        ///Add table alias to identifier in where clause
        /// </summary>
        /// <param name="where">where</param>
        /// <param name="alias">alias</param>
        /// <returns>converted where clause</returns>
        private String AddAliasToIdentifier(String where, String alias)
        {
            String sqlkey = "AND,OR,FROM,WHERE,JOIN,BY,GROUP,IN,INTO,SELECT,NOT,SET,UPDATE,DELETE,HAVING,IS,NULL,EXISTS,BETWEEN,LIKE,INNER,OUTER";

            StringTokenizer st = new StringTokenizer(where);
            String result = "";
            String token = "";
            int o = -1;
            while (true)
            {
                token = st.NextToken();
                String test = token.StartsWith("(") ? token.Substring(1) : token;
                if (sqlkey.IndexOf(test) == -1)
                {

                    token = token.Trim();
                    //skip subquery, non identifier and fully qualified identifier
                    if (o != -1)
                        result = result + " " + token;
                    else
                    {
                        result = result + " ";
                        StringBuilder t = new StringBuilder();
                        for (int i = 0; i < token.Length; i++)
                        {
                            char c = token[i];
                            if (IsOperator(c))
                            {
                                if (t.Length > 0)
                                {
                                    if (c == '(')
                                        result = result + t.ToString();
                                    else if (IsIdentifier(t.ToString()) &&
                                        t.ToString().IndexOf('.') == -1)
                                        result = result + alias + "." + t.ToString();
                                    else
                                        result = result + t.ToString();
                                    t = new StringBuilder();
                                }
                                result = result + c;
                            }
                            else
                            {
                                t.Append(c);
                            }
                        }
                        if (t.Length > 0)
                        {
                            if ("SELECT".Equals(t.ToString().ToUpper()))
                            {
                                o = 0;
                                result = result + t.ToString();
                            }
                            else if (IsIdentifier(t.ToString()) &&
                                t.ToString().IndexOf('.') == -1)
                                result = result + alias + "." + t.ToString();
                            else
                                result = result + t.ToString();
                        }
                    }

                    if (o != -1)
                    {
                        for (int i = 0; i < token.Length; i++)
                        {
                            char c = token[i];
                            if (c == '(')
                                o++;
                            if (c == ')')
                                o--;
                        }
                    }

                }
                else
                {
                    result = result + " " + token;
                    if ("SELECT".Equals(test,StringComparison.OrdinalIgnoreCase))
                    {
                        o = 0;
                    }
                }
                if (!st.HasMoreElements())
                    break;
            }
            return result;
        }

       /// <summary>
       /// Check if one of the field is using standard sql aggregate function
       /// </summary>
       /// <param name="fields"></param>
       /// <returns></returns>
        private bool UseAggregateFunction(String fields)
        {
            String fieldsUpper = fields.ToUpper();
            int size = fieldsUpper.Length;
            StringBuilder buffer = new StringBuilder();
            String token = null;
            for (int i = 0; i < size; i++)
            {
                char ch = fieldsUpper[i];
                if (Char.IsWhiteSpace(ch))
                {
                    if (buffer.Length > 0)
                    {
                        token = buffer.ToString();
                        buffer = new StringBuilder();
                    }
                }
                else
                {
                    if (IsOperator(ch))
                    {
                        if (buffer.Length > 0)
                        {
                            token = buffer.ToString();
                            buffer = new StringBuilder();
                        }
                        else
                        {
                            token = null;
                        }
                        if (ch == '(' && token != null)
                        {
                            if (token.Equals("SUM") || token.Equals("MAX") || token.Equals("MIN")
                                || token.Equals("COUNT") || token.Equals("AVG"))
                            {
                                return true;
                            }
                        }
                    }
                    else
                        buffer.Append(ch);
                }
            }

            return false;
        }

       
	 /// <summary>
	 ///Converts Update.
	 /// <pre>
	 ///        UPDATE C_Order i SET 
	 ///         =&gt; UPDATE C_Order SET
	 /// </pre>
	 /// </summary>
	 /// <param name="sqlStatement">sqlStatement</param>
	 /// <returns>converted statement</returns>

        private String ConvertUpdate(String sqlStatement)
        {
            String targetTable = null;
            String targetAlias = null;

            String sqlUpper = sqlStatement.ToUpper();
            StringBuilder token = new StringBuilder();
            String previousToken = null;
            int charIndex = 0;
            int sqlLength = sqlUpper.Length;
            int cnt = 0;
            bool isUpdate = false;

            //get target table and alias
            while (charIndex < sqlLength)
            {
                char c = sqlStatement[charIndex];
                if (Char.IsWhiteSpace(c))
                {
                    if (token.Length > 0)
                    {
                        cnt++;
                        if (cnt == 1)
                            isUpdate = "UPDATE".Equals(token.ToString(),StringComparison.OrdinalIgnoreCase);
                        else if (cnt == 2)
                            targetTable = token.ToString();
                        else if (cnt == 3)
                        {
                            targetAlias = token.ToString().Trim();
                            if ("SET".Equals(targetAlias,StringComparison.OrdinalIgnoreCase)) //no alias
                                targetAlias = targetTable;
                        }
                        previousToken = token.ToString();
                        token = new StringBuilder();
                    }
                }
                else
                {
                    if ("SET".Equals(previousToken,StringComparison.OrdinalIgnoreCase))
                        break;
                    else
                        token.Append(c);
                }
                charIndex++;
            }

            if (isUpdate && targetTable != null && sqlUpper[charIndex] == '(')
            {
                int updateFieldsBegin = charIndex;
                String updateFields = null;

                String select = "";

                //get the sub query
                String beforePreviousToken = null;
                previousToken = null;
                token = new StringBuilder();
                while (charIndex < sqlLength)
                {
                    char c = sqlUpper[charIndex];
                    if (Char.IsWhiteSpace(c))
                    {
                        if (token.Length > 0)
                        {
                            String currentToken = token.ToString();
                            if ("(".Equals(currentToken) || (currentToken != null && currentToken.StartsWith("(")))
                            {
                                if ((")".Equals(beforePreviousToken) ||
                                    (beforePreviousToken != null && beforePreviousToken.EndsWith(")"))) &&
                                    "=".Equals(previousToken))
                                {
                                    select = sqlStatement.Substring(charIndex - currentToken.Length);
                                    updateFields = sqlStatement.Substring(updateFieldsBegin, charIndex);
                                    updateFields = updateFields.Substring(0, updateFields.LastIndexOf(')'));
                                    break;
                                }
                                else if (")=".Equals(previousToken))
                                {
                                    select = sqlStatement.Substring(charIndex - currentToken.Length);
                                    updateFields = sqlStatement.Substring(updateFieldsBegin, charIndex);
                                    updateFields = updateFields.Substring(0, updateFields.LastIndexOf(')'));
                                    break;
                                }
                                else if (previousToken != null && previousToken.EndsWith(")="))
                                {
                                    select = sqlStatement.Substring(charIndex - currentToken.Length);
                                    updateFields = sqlStatement.Substring(updateFieldsBegin, charIndex);
                                    updateFields = updateFields.Substring(0, updateFields.LastIndexOf(')'));
                                    break;
                                }

                            }
                            if (")=(".Equals(currentToken))
                            {
                                select = sqlStatement.Substring(charIndex - 1);
                                updateFields = sqlStatement.Substring(updateFieldsBegin, charIndex);
                                updateFields = updateFields.Substring(0, updateFields.LastIndexOf(')'));
                                break;
                            }
                            else if (currentToken.EndsWith(")=(SELECT"))
                            {
                                select = sqlStatement.Substring(charIndex - 7);
                                updateFields = sqlStatement.Substring(updateFieldsBegin, charIndex);
                                updateFields = updateFields.Substring(0, updateFields.LastIndexOf(')'));
                                break;
                            }
                            else if ("=(".Equals(currentToken) || (currentToken != null && currentToken.StartsWith("=(")))
                            {
                                if (")".Equals(previousToken) || (previousToken != null && previousToken.EndsWith(")")))
                                {
                                    select = sqlStatement.Substring(charIndex - currentToken.Length);
                                    updateFields = sqlStatement.Substring(updateFieldsBegin, charIndex);
                                    updateFields = updateFields.Substring(0, updateFields.LastIndexOf(')'));
                                    break;
                                }
                            }
                            beforePreviousToken = previousToken;
                            previousToken = token.ToString();
                            token = new StringBuilder();
                        }
                    }
                    else
                    {
                        token.Append(c);
                    }
                    charIndex++;
                }
                if (updateFields != null && updateFields.StartsWith("("))
                    updateFields = updateFields.Substring(1);

                int subQueryEnd = 0;
                int subQueryStart = select.IndexOf('(');
                String subWhere = null;
                int open = -1;
                for (int i = subQueryStart; i < select.Length; i++)
                {
                    char c = select[i];
                    if (c == '(')
                        open++;

                    if (c == ')')
                        open--;

                    if (open == -1)
                    {
                        subQueryEnd = i + 1;
                        break;
                    }
                }

                String mainWhere = "";
                String otherUpdateFields = "";
                //get update where clause
                token = new StringBuilder();
                for (int i = subQueryEnd; i < select.Length; i++)
                {
                    char c = select[i];
                    if (Char.IsWhiteSpace(c))
                    {
                        if (token.Length > 0)
                        {
                            if ("WHERE".Equals(token.ToString().ToUpper()))
                            {
                                otherUpdateFields = select.Substring(subQueryEnd, i - 5).Trim();
                                mainWhere = select.Substring(i + 1);
                                break;
                            }
                            token = new StringBuilder();
                        }
                    }
                    else
                    {
                        token.Append(c);
                    }
                }

                String subQuery = select.Substring(subQueryStart, subQueryEnd);

                //get join table and alias
                String joinTable = null;
                String joinAlias = null;
                token = new StringBuilder();
                previousToken = null;
                int joinFieldsBegin = 0;
                String joinFields = null;
                String joinFromClause = null;
                int joinFromClauseStart = 0;
                open = -1;
                for (int i = 0; i < subQuery.Length; i++)
                {
                    char c = subQuery[i];
                    if (Char.IsWhiteSpace(c))
                    {
                        if (token.Length > 0 && open < 0)
                        {
                            if ("FROM".Equals(previousToken,StringComparison.OrdinalIgnoreCase))
                            {
                                joinTable = token.ToString();
                            }
                            if ("WHERE".Equals(token.ToString().ToUpper()))
                            {
                                subWhere = subQuery.Substring(i + 1, subQuery.Length - 1);
                                joinFromClause = subQuery.Substring(joinFromClauseStart, i - 5).Trim();
                                break;
                            }
                            if ("FROM".Equals(token.ToString(),StringComparison.OrdinalIgnoreCase))
                            {
                                joinFields = subQuery.Substring(joinFieldsBegin, i - 4);
                                joinFromClauseStart = i;
                            }
                            if (previousToken != null && previousToken.Equals(joinTable))
                            {
                                joinAlias = token.ToString();
                            }
                            previousToken = token.ToString();
                            token = new StringBuilder();
                        }
                    }
                    else
                    {
                        if (joinFieldsBegin == 0)
                        {
                            if (token.Length == 0 &&
                                ("SELECT".Equals(previousToken) ||
                                  (previousToken != null && previousToken.ToUpper().EndsWith("SELECT"))))
                                joinFieldsBegin = i;
                        }
                        else if (c == '(')
                            open++;
                        else if (c == ')')
                            open--;
                        token.Append(c);
                    }
                }
                if (joinFromClause == null) joinFromClause = subQuery.Substring(joinFromClauseStart).Trim();
                if (joinAlias == null) joinAlias = joinTable;

                //construct update clause
                StringBuilder Update = new StringBuilder("UPDATE ");
                Update.Append(targetTable);
                if (!targetAlias.Equals(targetTable))
                    Update.Append(" " + targetAlias);

                Update.Append(" SET ");

                int f = updateFields.Length;
                int fj = joinFields.Length;
                String updateField = null;
                String joinField = null;

                bool useSubQuery = false;
                if (UseAggregateFunction(joinFields))
                    useSubQuery = true;

                while (f > 0)
                {
                    f = Utility.Util.FindIndexOf(updateFields, ',');
                    if (f < 0)
                    {
                        updateField = updateFields;
                        joinField = joinFields.Trim();
                        if (joinField.IndexOf('.') < 0 && IsIdentifier(joinField))
                        {
                            joinField = joinAlias + "." + joinField;
                        }

                        Update.Append(updateField.Trim());
                        Update.Append("=");
                        if (useSubQuery)
                        {
                            Update.Append("( SELECT ");
                            Update.Append(joinField);
                            Update.Append(" FROM ");
                            Update.Append(joinFromClause);
                            Update.Append(" WHERE ");
                            Update.Append(subWhere.Trim());
                            Update.Append(" ) ");
                            Update.Append(otherUpdateFields);
                            if (mainWhere != null)
                            {
                                Update.Append(" WHERE ");
                                Update.Append(mainWhere);
                            }
                        }
                        else
                        {
                            Update.Append(joinField);
                            Update.Append(otherUpdateFields);
                            Update.Append(" FROM ");
                            Update.Append(joinFromClause);
                            Update.Append(" WHERE ");
                            subWhere = AddAliasToIdentifier(subWhere, joinAlias);
                            Update.Append(subWhere.Trim());

                            if (mainWhere != null)
                                mainWhere = " AND " + mainWhere;

                            else
                                mainWhere = "";

                            mainWhere = AddAliasToIdentifier(mainWhere, targetAlias);
                            Update.Append(mainWhere);
                        }
                    }
                    else
                    {

                        updateField = updateFields.Substring(0, f);
                        fj = Utility.Util.FindIndexOf(joinFields, ',');
                        // fieldsjoin.indexOf(',');

                        joinField = fj > 0 ? joinFields.Substring(0, fj).Trim() : joinFields.Trim();
                        if (joinField.IndexOf('.') < 0 && IsIdentifier(joinField))
                        {
                            joinField = joinAlias + "." + joinField;
                        }
                        Update.Append(updateField.Trim  ());
                        Update.Append("=");
                        if (useSubQuery)
                        {
                            Update.Append("( SELECT ");
                            Update.Append(joinField);
                            Update.Append(" FROM ");
                            Update.Append(joinFromClause);
                            Update.Append(" WHERE ");
                            Update.Append(subWhere.Trim());
                            Update.Append(" ) ");
                        }
                        else
                        {
                            Update.Append(joinField);
                        }
                        Update.Append(",");
                        joinFields = joinFields.Substring(fj + 1);
                    }

                    updateFields = updateFields.Substring(f + 1);

                    // System.out.println("Update" + Update);
                }

                return Update.ToString();

            }
            return sqlStatement;
        } // convertDecode

       /// <summary>
       /// Check if token is a valid sql identifier
       /// </summary>
       /// <param name="token"></param>
       /// <returns>True if token is a valid sql identifier, false otherwise</returns>
        private bool IsIdentifier(String token)
        {
            int size = token.Length;
            for (int i = 0; i < size; i++)
            {
                char c = token[i];
                if (IsOperator(c))
                    return false;
            }
            if (token.StartsWith("'") && token.EndsWith("'"))
                return false;
            else
            {
                try
                {
                    Decimal.Parse(token);
                    return false;
                }
                catch  { }
            }

            if (IsSQLFunctions(token))
                return false;

            return true;
        }

        private bool IsSQLFunctions(String token)
        {
            if (token.Equals("current_timestamp",StringComparison.OrdinalIgnoreCase))
                return true;
            else if (token.Equals("current_time",StringComparison.OrdinalIgnoreCase))
                return true;
            else if (token.Equals("current_date",StringComparison.OrdinalIgnoreCase))
                return true;
            else if (token.Equals("localtime",StringComparison.OrdinalIgnoreCase))
                return true;
            else if (token.Equals("localtimestamp",StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }

        // begin 

       /// <summary>
       ///convertAlias - for compatibility with 8.1
       /// </summary>
       /// <param name="sqlStatement">sqlstatement</param>
       /// <returns>converted statementf</returns>
        private String ConvertAlias(String sqlStatement)
        {
            //string[] str =new string[1];
            //str.SetValue("\\s",0);
            String[] tokens = sqlStatement.Split(' ');//str,StringSplitOptions.None);
            String table = null;
            String alias = null;
            if ("UPDATE".Equals(tokens[0],StringComparison.OrdinalIgnoreCase))
            {
                if ("SET".Equals(tokens[2],StringComparison.OrdinalIgnoreCase))
                    return sqlStatement;
                table = tokens[1];
                alias = tokens[2];
            }
            else if ("INSERT".Equals(tokens[0],StringComparison.OrdinalIgnoreCase))
            {
                if ("VALUES".Equals(tokens[3],StringComparison.OrdinalIgnoreCase) ||
                    "SELECT".Equals(tokens[3], StringComparison.OrdinalIgnoreCase))
                    return sqlStatement;
                if (tokens[2].IndexOf('(') > 0)
                    return sqlStatement;
                else if ((tokens[3].IndexOf('(') < 0) ||
                        tokens[3].IndexOf('(') > 0)
                {
                    table = tokens[2];
                    alias = tokens[3];
                }
                else
                {
                    return sqlStatement;
                }
            }
            else if ("DELETE".Equals(tokens[0],StringComparison.OrdinalIgnoreCase))
            {
                if (tokens.Length < 4) return sqlStatement;
                if ("WHERE".Equals(tokens[3],StringComparison.OrdinalIgnoreCase))
                    return sqlStatement;
                table = tokens[2];
                alias = tokens[3];
            }
            if (table != null && alias != null)
            {
                if (alias.IndexOf('(') > 0) alias = alias.Substring(0, alias.IndexOf('('));
                String converted = Utility.Util.ReplaceFirst(sqlStatement,"\\s" + alias + "\\s", " ");
                converted = converted.Replace("\\b" + alias + "\\.", table + ".");
                converted = converted.Replace("[+]" + alias + "\\.", "+" + table + ".");
                converted = converted.Replace("[-]" + alias + "\\.", "-" + table + ".");
                converted = converted.Replace("[*]" + alias + "\\.", "*" + table + ".");
                converted = converted.Replace("[/]" + alias + "\\.", "/" + table + ".");
                converted = converted.Replace("[%]" + alias + "\\.", "%" + table + ".");
                converted = converted.Replace("[<]" + alias + "\\.", "<" + table + ".");
                converted = converted.Replace("[>]" + alias + "\\.", ">" + table + ".");
                converted = converted.Replace("[=]" + alias + "\\.", "=" + table + ".");
                converted = converted.Replace("[|]" + alias + "\\.", "|" + table + ".");
                converted = converted.Replace("[(]" + alias + "\\.", "(" + table + ".");
                converted = converted.Replace("[)]" + alias + "\\.", ")" + table + ".");
                return converted;
            }
            else
            {
                return sqlStatement;
            }
        } // 

        // ALTER TABLE AD_FieldGroup MODIFY IsTab CHAR(1) DEFAULT N;
        // ALTER TABLE AD_FieldGroup ALTER COLUMN IsTab TYPE CHAR(1); ALTER TABLE
        // AD_FieldGroup ALTER COLUMN SET DEFAULT 'N';
        private String ConvertDDL(String sqlStatement)
        {
            if (sqlStatement.ToUpper().IndexOf("ALTER TABLE ") == 0)
            {
                String action = null;
                int begin_col = -1;
                if (sqlStatement.ToUpper().IndexOf(" MODIFY ") > 0)
                {
                    action = " MODIFY ";
                    begin_col = sqlStatement.ToUpper().IndexOf(" MODIFY ")
                            + action.Length;
                }
                else if (sqlStatement.ToUpper().IndexOf(" ADD ") > 0)
                {
                    if (sqlStatement.ToUpper().IndexOf(" ADD CONSTRAINT ") < 0 &&
                            sqlStatement.ToUpper().IndexOf(" ADD FOREIGN KEY ") < 0)
                    {
                        action = " ADD ";
                        begin_col = sqlStatement.ToUpper().IndexOf(" ADD ")
                                + action.Length;
                    }
                }

                // System.out.println( "MODIFY :" +
                // sqlStatement.toUpperCase().indexOf(" MODIFY "));
                // System.out.println( "ADD :" +
                // sqlStatement.toUpperCase().indexOf(" ADD "));
                // System.out.println( "begincolumn:" + sqlStatement +
                // "begincolumn:" + begin_col );

                if (begin_col < 0)
                    return sqlStatement;

                int end_col = 0;
                int begin_default = -1;

                String column = null;
                String type = null;
                String defaultvalue = null;
                String nullclause = null;
                String DDL = null;

                if (begin_col != -1)
                {
                    column = sqlStatement.Substring(begin_col);
                    end_col = begin_col + column.IndexOf(' ');
                    column = sqlStatement.Substring(begin_col, end_col - begin_col);
                    // System.out.println(" column:" + column + " begincolumn:" +
                    // begin_col + "en column:" + end_col );
                    // System.out.println(" type " + sqlStatement.substring(end_col
                    // + 1));
                    String rest = sqlStatement.Substring(end_col + 1);

                    if (action.Equals(" ADD "))
                    {
                        if (rest.ToUpper().IndexOf(" DEFAULT ") != -1)
                        {
                            String beforeDefault = rest.Substring(0, rest.ToUpper().IndexOf(" DEFAULT "));
                            begin_default = rest.ToUpper().IndexOf(
                                    " DEFAULT ") + 9;
                            defaultvalue = rest.Substring(begin_default);
                            int nextspace = defaultvalue.IndexOf(' ');
                            if (nextspace > -1)
                            {
                                rest = defaultvalue.Substring(nextspace);
                                defaultvalue = defaultvalue.Substring(0, defaultvalue.IndexOf(' '));
                            }
                            else
                            {
                                rest = "";
                            }
                            if (defaultvalue.Equals("NULL", StringComparison.OrdinalIgnoreCase))
                            {
                                DDL = sqlStatement.Substring(0, begin_col
                                        - action.Length)
                                        + " ADD COLUMN "
                                        + column
                                        + " " + beforeDefault.Trim()
                                        + " DEFAULT "
                                        + defaultvalue.Trim() + " " + rest.Trim();
                            }
                            else
                            {
                                // Check if default value is already quoted, no need to double quote
                                if (defaultvalue.StartsWith("'") && defaultvalue.EndsWith("'"))
                                {
                                    DDL = sqlStatement.Substring(0, begin_col
                                        - action.Length)
                                        + " ADD COLUMN "
                                        + column
                                        + " " + beforeDefault.Trim()
                                        + " DEFAULT "
                                        + defaultvalue.Trim() + " " + rest.Trim();
                                }
                                else
                                {
                                    DDL = sqlStatement.Substring(0, begin_col
                                            - action.Length)
                                        + " ADD COLUMN "
                                        + column
                                        + " " + beforeDefault.Trim()
                                        + " DEFAULT '"
                                        + defaultvalue.Trim() + "' " + rest.Trim();
                                }
                            }
                        }
                        else
                        {
                            DDL = sqlStatement
                                .Substring(0, begin_col - action.Length)
                                + action + "COLUMN " + column + " " + rest.Trim();
                        }
                    }
                    else if (action.Equals(" MODIFY "))
                    {
                        rest = rest.Trim();
                        if (rest.ToUpper().StartsWith("NOT ") || rest.ToUpper().StartsWith("NULL ")
                                || rest.ToUpper().Equals("NULL") || rest.ToUpper().Equals("NOT NULL"))
                        {
                            type = null;
                        }
                        else
                        {
                            int typeEnd = rest.IndexOf(' ');
                            type = typeEnd > 0 ? rest.Substring(0, typeEnd).Trim() : rest;
                            rest = typeEnd > 0 ? rest.Substring(typeEnd) : "";
                        }

                        if (rest.ToUpper().IndexOf(" DEFAULT ") != -1)
                        {
                            begin_default = rest.ToUpper().IndexOf(
                                    " DEFAULT ") + 9;
                            defaultvalue = rest.Substring(begin_default);
                            int nextspace = defaultvalue.IndexOf(' ');
                            if (nextspace > -1)
                            {
                                rest = defaultvalue.Substring(nextspace);
                                defaultvalue = defaultvalue.Substring(0, defaultvalue.IndexOf(' '));
                            }
                            else
                            {
                                rest = "";
                            }
                            // Check if default value is already quoted
                            defaultvalue = defaultvalue.Trim();
                            if (defaultvalue.StartsWith("'") && defaultvalue.EndsWith("'"))
                                defaultvalue = defaultvalue.Substring(1, defaultvalue.Length - 1);

                            if (rest != null && rest.ToUpper().IndexOf("NOT NULL") >= 0)
                                nullclause = "NOT NULL";
                            else if (rest != null && rest.ToUpper().IndexOf("NULL") >= 0)
                                nullclause = "NULL";

                            // return DDL;
                        }
                        else if (rest != null && rest.ToUpper().IndexOf("NOT NULL") >= 0)
                        {
                            nullclause = "NOT NULL";

                        }
                        else if (rest != null && rest.ToUpper().IndexOf("NULL") >= 0)
                        {
                            nullclause = "NULL";

                        }

                        DDL = "INSERT INTO t_alter_column values('";
                        String tableName = sqlStatement.Substring(0, begin_col - action.Length);
                        tableName = tableName.ToUpper().Replace("ALTER TABLE", "");
                        tableName = tableName.Trim().ToLower();
                        DDL = DDL + tableName + "','" + column + "',";
                        if (type != null)
                            DDL = DDL + "'" + type + "',";
                        else
                            DDL = DDL + "null,";
                        if (nullclause != null)
                            DDL = DDL + "'" + nullclause + "',";
                        else
                            DDL = DDL + "null,";
                        if (defaultvalue != null)
                            DDL = DDL + "'" + defaultvalue + "'";
                        else
                            DDL = DDL + "null";
                        DDL = DDL + ")";
                    }
                    return DDL;
                }
            }

            return sqlStatement;

        }
    }
}
