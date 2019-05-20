using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.DBPort
{

    /// <summary>
    /// Convert from oracle syntax to sql 92 standard
    /// </summary>
    public abstract class ConvertSQL_SQL92 : ConvertSQL
    {
        /**	Logger	*/
        private static VLogger log = VLogger.GetVLogger(typeof(ConvertSQL_SQL92).FullName);


        /// <summary>
        ///   Convert Outer Join.
        /// Converting joins can ve very complex when multiple tables/keys are involved.
        ///  The main scenarios supported are two tables with multiple key columns
        ///  and multiple tables with single key columns.
        /// <pre>
        ///      SELECT a.Col1, b.Col2 FROM tableA a, tableB b WHERE a.ID=b.ID(+)
        ///      => SELECT a.Col1, b.Col2 FROM tableA a LEFT OUTER JOIN tableB b ON (a.ID=b.ID)
        ///     SELECT a.Col1, b.Col2 FROM tableA a, tableB b WHERE a.ID(+)=b.ID
        ///      => SELECT a.Col1, b.Col2 FROM tableA a RIGHT OUTER JOIN tableB b ON (a.ID=b.ID)
        ///  Assumptions:
        ///  - No outer joins in sub queries (ignores sub-queries)
        ///  - OR condition ignored (not sure what to do, should not happen)
        ///  Limitations:
        ///  - Parameters for outer joins must be first - as sequence of parameters changes
        ///  </pre>*/
        /// </summary>
        /// <param name="sqlStatement">sqlStatement</param>
        /// <returns>converted statement</returns>
        protected String ConvertOuterJoin(String sqlStatement)
    {
        bool trace = false;
        //
        int fromIndex = Util.FindIndexOf(sqlStatement.ToUpper(), " FROM ");
        int whereIndex = Util.FindIndexOf(sqlStatement.ToUpper(), " WHERE ");
        //begin vpj-cd e-evolution 03/14/2005 PostgreSQL
        //int endWhereIndex = Util.findIndexOf(sqlStatement.toUpperCase(), " GRPUP BY ");
        int endWhereIndex = Util.FindIndexOf (sqlStatement.ToUpper(), " GROUP BY ");
        //end vpj-cd e-evolution 03/14/2005	PostgreSQL
        if (endWhereIndex == -1)
            endWhereIndex = Util.FindIndexOf(sqlStatement.ToUpper(), " ORDER BY ");
        if (endWhereIndex == -1)
            endWhereIndex = sqlStatement.Length;
        //
        if (trace)
        {
            log.Info("OuterJoin<== " + sqlStatement);
            //	log.info("From=" + fromIndex + ", Where=" + whereIndex + ", End=" + endWhereIndex + ", Length=" + sqlStatement.length());
        }
        //
        String selectPart = sqlStatement.Substring(0, fromIndex);
        String fromPart = sqlStatement.Substring(fromIndex, whereIndex);
        String wherePart = sqlStatement.Substring(whereIndex, endWhereIndex);
        String rest = sqlStatement.Substring(endWhereIndex);

        //  find/remove all (+) from WHERE clase ------------------------------
        String newWherePart = wherePart;
        List<String> joins = new List<String>();
        int pos = newWherePart.IndexOf("(+)");
        while (pos != -1)
        {
            //  find starting point
            int start = newWherePart.LastIndexOf(" AND ", pos);
            int startOffset = 5;
            if (start == -1)
            {
                start = newWherePart.LastIndexOf(" OR ", pos);
                startOffset = 4;
            }
            if (start == -1)
            {
                start = newWherePart.LastIndexOf("WHERE ", pos);
                startOffset = 6;
            }
            if (start == -1)
            {
                String error = "Start point not found in clause " + wherePart;
                log.Severe(error);
                m_conversionError = error;
                return sqlStatement;
            }
            //  find end point
            int end = newWherePart.IndexOf(" AND ", pos);
            if (end == -1)
                end = newWherePart.IndexOf(" OR ", pos);
            if (end == -1)
                end = newWherePart.Length;
            //	log.info("<= " + newWherePart + " - Start=" + start + "+" + startOffset + ", End=" + end);

            //  extract condition
            String condition = newWherePart.Substring(start + startOffset, end);
            joins.Add(condition);
            if (trace)
                log.Info("->" + condition);
            //  new WHERE clause
            newWherePart = newWherePart.Substring(0, start) + newWherePart.Substring(end);
            //	log.info("=> " + newWherePart);
            //
            pos = newWherePart.IndexOf("(+)");
        }
        //  correct beginning
        newWherePart = newWherePart.Trim();
        if (newWherePart.StartsWith("AND "))
            newWherePart = "WHERE" + newWherePart.Substring(3);
        else if (newWherePart.StartsWith("OR "))
            newWherePart = "WHERE" + newWherePart.Substring(2);
        if (trace)
            log.Info("=> " + newWherePart);

        //  Correct FROM clause -----------------------------------------------
        //  Disassemble FROM
        String[] fromParts = fromPart.Trim().Substring(4).Split(',');
        Dictionary<String, String> fromAlias = new Dictionary<String, String>();      //  tables to be processed
        Dictionary<String, String> fromLookup = new Dictionary<String, String>();     //  used tabled
        for (int i = 0; i < fromParts.Length; i++)
        {
            String entry = fromParts[i].Trim();
            String alias = entry;   //  no alias
            String table = entry;
            int aPos = entry.LastIndexOf(' ');
            if (aPos != -1)
            {
                alias = entry.Substring(aPos + 1);
                table = entry.Substring(0, entry.IndexOf(' ')); // may have AS
            }
            fromAlias.Add(alias, table);
            fromLookup.Add(alias, table);
            if (trace)
                log.Info("Alias=" + alias + ", Table=" + table);
        }

        /** Single column
            SELECT t.TableName, w.Name FROM AD_Table t, AD_Window w
            WHERE t.AD_Window_ID=w.AD_Window_ID(+)
            --	275 rows
            SELECT t.TableName, w.Name FROM AD_Table t
            LEFT OUTER JOIN AD_Window w ON (t.AD_Window_ID=w.AD_Window_ID)

            SELECT t.TableName, w.Name FROM AD_Table t, AD_Window w
            WHERE t.AD_Window_ID(+)=w.AD_Window_ID
            --	239 rows
            SELECT t.TableName, w.Name FROM AD_Table t
            RIGHT OUTER JOIN AD_Window w ON (t.AD_Window_ID=w.AD_Window_ID)

        **  Multiple columns
            SELECT tn.Node_ID,tn.Parent_ID,tn.SeqNo,tb.IsActive
            FROM AD_TreeNode tn, AD_TreeBar tb
            WHERE tn.AD_Tree_ID=tb.AD_Tree_ID(+) AND tn.Node_ID=tb.Node_ID(+)
              AND tn.AD_Tree_ID=10
            --  235 rows
            SELECT	tn.Node_ID,tn.Parent_ID,tn.SeqNo,tb.IsActive
            FROM AD_TreeNode tn LEFT OUTER JOIN AD_TreeBar tb
              ON (tn.Node_ID=tb.Node_ID AND tn.AD_Tree_ID=tb.AD_Tree_ID AND tb.AD_User_ID=0)
            WHERE tn.AD_Tree_ID=10

            SELECT tn.Node_ID,tn.Parent_ID,tn.SeqNo,tb.IsActive
            FROM AD_TreeNode tn, AD_TreeBar tb
            WHERE tn.AD_Tree_ID=tb.AD_Tree_ID(+) AND tn.Node_ID=tb.Node_ID(+)
             AND tn.AD_Tree_ID=10 AND tb.AD_User_ID(+)=0
            --  214 rows
            SELECT tn.Node_ID,tn.Parent_ID,tn.SeqNo,tb.IsActive
            FROM AD_TreeNode tn LEFT OUTER JOIN AD_TreeBar tb
              ON (tn.Node_ID=tb.Node_ID AND tn.AD_Tree_ID=tb.AD_Tree_ID AND tb.AD_User_ID=0)
            WHERE tn.AD_Tree_ID=10

         */
        StringBuilder newFrom = new StringBuilder();
        for (int i = 0; i < joins.Count; i++)
        {
            Join first = new Join(joins[i]);
            first.SetMainTable(fromLookup[first.GetMainAlias()]);
            fromAlias.Remove(first.GetMainAlias());     //  remove from list
            first.SetJoinTable(fromLookup[first.GetJoinAlias()]);
            fromAlias.Remove(first.GetJoinAlias());     //  remove from list
            if (trace)
                log.Info("-First: " + first);
            //
            if (newFrom.Length == 0)
                newFrom.Append(" FROM ");
            else
                newFrom.Append(", ");
            newFrom.Append(first.GetMainTable()).Append(" ").Append(first.GetMainAlias())
                .Append(first.IsLeft() ? " LEFT" : " RIGHT").Append(" OUTER JOIN ")
                .Append(first.GetJoinTable()).Append(" ").Append(first.GetJoinAlias())
                .Append(" ON (").Append(first.GetCondition());
            //  keep it open - check for other key comparisons
            for (int j = i + 1; j < joins.Count; j++)
            {
                Join second = new Join(joins[j]);
                second.SetMainTable(fromLookup[second.GetMainAlias()]);
                second.SetJoinTable(fromLookup[second.GetJoinAlias()]);
                if ((first.GetMainTable().Equals(second.GetMainTable())
                        && first.GetJoinTable().Equals(second.GetJoinTable()))
                    || second.IsConditionOf(first))
                {
                    if (trace)
                        log.Info("-Second/key: " + second);
                    newFrom.Append(" AND ").Append(second.GetCondition());
                    joins.RemoveAt(j);                        //  remove from join list
                    fromAlias.Remove(first.GetJoinAlias()); //  remove from table list
                    //----
                    for (int k = i + 1; k < joins.Count; k++)
                    {
                        Join third = new Join(joins[k]);
                        third.SetMainTable(fromLookup[third.GetMainAlias()]);
                        third.SetJoinTable(fromLookup[third.GetJoinAlias()]);
                        if (third.IsConditionOf(second))
                        {
                            if (trace)
                                log.Info("-Third/key: " + third);
                            newFrom.Append(" AND ").Append(third.GetCondition());
                            joins.RemoveAt(k);                            //  remove from join list
                            fromAlias.Remove(third.GetJoinAlias());     //  remove from table list
                        }
                        else if (trace)
                            log.Info("-Third/key-skip: " + third);
                    }
                }
                else if (trace)
                    log.Info("-Second/key-skip: " + second);
            }
            newFrom.Append(")");    //  close ON
            //  check dependency on first table
            for (int j = i + 1; j < joins.Count; j++)
            {
                Join second = new Join(joins[j]);
                second.SetMainTable(fromLookup[second.GetMainAlias()]);
                second.SetJoinTable(fromLookup[second.GetJoinAlias()]);
                if (first.GetMainTable().Equals(second.GetMainTable()))
                {
                    if (trace)
                        log.Info("-Second/dep: " + second);
                    //   FROM (AD_Field f LEFT OUTER JOIN AD_Column c ON (f.AD_Column_ID = c.AD_Column_ID))
                    //  LEFT OUTER JOIN AD_FieldGroup fg ON (f.AD_FieldGroup_ID = fg.AD_FieldGroup_ID),
                    newFrom.Insert(6, '(');     //  _FROM ...
                    newFrom.Append(')');        //  add parantesis on previous relation
                    //
                    newFrom.Append(second.IsLeft() ? " LEFT" : " RIGHT").Append(" OUTER JOIN ")
                        .Append(second.GetJoinTable()).Append(" ").Append(second.GetJoinAlias())
                        .Append(" ON (").Append(second.GetCondition());
                    joins.RemoveAt(j);                            //  remove from join list
                    fromAlias.Remove(second.GetJoinAlias());    //  remove from table list
                    //  additional join colums would come here
                    newFrom.Append(")");    //  close ON
                    //----
                    for (int k = i + 1; k < joins.Count; k++)
                    {
                        Join third = new Join(joins[k]);
                        third.SetMainTable(fromLookup[third.GetMainAlias()]);
                        third.SetJoinTable(fromLookup[third.GetJoinAlias()]);
                        if (second.GetJoinTable().Equals(third.GetMainTable()))
                        {
                            if (trace)
                                log.Info("-Third-dep: " + third);
                            //   FROM ((C_BPartner p LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID))
                            //  LEFT OUTER JOIN C_BPartner_Location l ON (p.C_BPartner_ID=l.C_BPartner_ID))
                            //  LEFT OUTER JOIN C_Location a ON (l.C_Location_ID=a.C_Location_ID)
                            newFrom.Insert(6, '(');     //  _FROM ...
                            newFrom.Append(')');        //  add parantesis on previous relation
                            //
                            newFrom.Append(third.IsLeft() ? " LEFT" : " RIGHT").Append(" OUTER JOIN ")
                                .Append(third.GetJoinTable()).Append(" ").Append(third.GetJoinAlias())
                                .Append(" ON (").Append(third.GetCondition());
                            joins.RemoveAt(k);                            //  remove from join list
                            fromAlias.Remove(third.GetJoinAlias());     //  remove from table list
                            //  additional join colums would come here
                            newFrom.Append(")");    //  close ON
                        }
                        else if (trace)
                            log.Info("-Third-skip: " + third);
                    }
                }
                else if (trace)
                    log.Info("-Second/dep-skip: " + second);
            }   //  dependency on first table
        }
        //  remaining Tables
        var it = fromAlias.Keys.GetEnumerator();
        while (it.MoveNext())
        {
            String alias = it.Current;
            Object table = fromAlias[alias];
            newFrom.Append(", ").Append(table);
            if (!table.Equals(alias))
                newFrom.Append(" ").Append(alias);
        }
        if (trace)
            log.Info(newFrom.ToString());
        //
        StringBuilder retValue = new StringBuilder(sqlStatement.Length + 20);
        retValue.Append(selectPart)
            .Append(newFrom).Append(" ")
            .Append(newWherePart).Append(rest);
        //
        if (trace)
            log.Info("OuterJoin==> " + retValue.ToString());
        return retValue.ToString();

        }
        
        /// <summary>
        /// Converts Decode.
	 ///  <pre>
	 ///      DECODE (a, 1, 'one', 2, 'two', 'none')
	 ///       => CASE WHEN a = 1 THEN 'one' WHEN a = 2 THEN 'two' ELSE 'none' END
	 ///  </pre>
        /// </summary>
        /// <param name="sqlStatement">sqlStatement</param>
        /// <param name="fromIndex"></param>
        /// <returns> converted statement</returns>
        protected String ConvertDecode(String sqlStatement, int fromIndex)
        {
            //	log.info("DECODE<== " + sqlStatement);
            String statement = sqlStatement;
            StringBuilder sb = new StringBuilder("CASE");

            int index = statement.ToUpper().IndexOf("DECODE", fromIndex);
            if (index <= 0) return sqlStatement;

            char previousChar = statement[index - 1];
            if (!(Char.IsWhiteSpace(previousChar) || IsOperator(previousChar)))
                return sqlStatement;

            String firstPart = statement.Substring(0, index);

            //  find the opening (
            index = index + 6;
            while (index < statement.Length)
            {
                char c = statement[index];
                if (char.IsWhiteSpace(c))
                {
                    index++;
                    continue;
                }
                if (c == '(') break;
                return sqlStatement;
            }

            statement = statement.Substring(index + 1);

            //  find the expression "a" - find first , ignoring ()
            index = Util.FindIndexOf(statement, ',');
            String expression = statement.Substring(0, index).Trim();
            //	log.info("Expression=" + expression);

            //  Pairs "1, 'one',"
            statement = statement.Substring(index + 1);
            index = Util.FindIndexOf(statement, ',');
            while (index != -1)
            {
                String first = statement.Substring(0, index);
                char cc = statement[index];
                statement = statement.Substring(index + 1);
                //	log.info("First=" + first + ", Char=" + cc);
                //
                bool error = false;
                if (cc == ',')
                {
                    index = Util.FindIndexOf(statement, ',', ')');
                    if (index == -1)
                        error = true;
                    else
                    {
                        String second = statement.Substring(0, index);
                        sb.Append(" WHEN ").Append(expression).Append("=").Append(first.Trim())
                            .Append(" THEN ").Append(second.Trim());
                        //			log.info(">>" + sb.toString());
                        statement = statement.Substring(index + 1);
                        index = Util.FindIndexOf(statement, ',', ')');
                    }
                }
                else if (cc == ')')
                {
                    sb.Append(" ELSE ").Append(first.Trim()).Append(" END");
                    //		log.info(">>" + sb.toString());
                    index = -1;
                }
                else
                    error = true;
                if (error)
                {
                    log.Log(Level.SEVERE, "SQL=(" + sqlStatement
                        + ")\n====Result=(" + sb.ToString()
                        + ")\n====Statement=(" + statement
                        + ")\n====First=(" + first
                        + ")\n====Index=" + index);
                    m_conversionError = "Decode conversion error";
                }
            }
            sb.Append(statement);
            sb.Insert(0, firstPart);
            //	log.info("DECODE==> " + sb.toString());
            return sb.ToString();
        }	//  convertDecode

        /// <summary>
        /// Converts Delete.
	 /// <pre>
	 ///        DELETE C_Order i WHERE  
	 ///         =&gt; DELETE FROM C_Order WHERE  
	 /// </pre>
        /// </summary>
        /// <param name="sqlStatement">sqlStatement</param>
        /// <returns>converted statement</returns>
        protected String ConvertDelete(String sqlStatement)
        {

            int index = sqlStatement.ToUpper().IndexOf("DELETE ");
            if (index < 7)
            {
                return "DELETE FROM " + sqlStatement.Substring(index + 7);
            }

            return sqlStatement;
        } // convertD

        /// <summary>
        ///Is character a valid sql operator
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        protected bool IsOperator(char c)
        {
            if ('=' == c)
                return true;
            else if ('<' == c)
                return true;
            else if ('>' == c)
                return true;
            else if ('|' == c)
                return true;
            else if ('(' == c)
                return true;
            else if (')' == c)
                return true;
            else if ('+' == c)
                return true;
            else if ('-' == c)
                return true;
            else if ('*' == c)
                return true;
            else if ('/' == c)
                return true;
            else if ('!' == c)
                return true;
            else if (',' == c)
                return true;
            else if ('?' == c)
                return true;
            else if ('#' == c)
                return true;
            else if ('@' == c)
                return true;
            else if ('~' == c)
                return true;
            else if ('&' == c)
                return true;
            else if ('^' == c)
                return true;
            else if ('!' == c)
                return true;
            else
                return false;
        }

    }
}
