/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ViewImport
 * Purpose        : Import create view SQL file into Vienna AD
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           14-Jan-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.IO;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class ViewImport : ProcessEngine.SvrProcess
    {

        private static String Replace(String sin, char oldChar, char newChar)
        {
            if (oldChar != newChar)
            {
                int len = sin.Length;
                int i = -1;
                char[] val = sin.ToCharArray(); /* avoid getfield opcode */
                int off = 0;   /* avoid getfield opcode */

                while (++i < len)
                {
                    if (val[off + i] == oldChar)
                    {
                        break;
                    }
                }
                if (i < len)
                {
                    char[] buf = new char[len];
                    for (int j = 0; j < i; j++)
                    {
                        buf[j] = val[off + j];
                    }
                    while (i < len)
                    {
                        char c = val[off + i];
                        buf[i] = (c == oldChar) ? newChar : c;
                        i++;
                    }
                    return new String(buf);
                }
            }
            return sin;
        }
        private String SQLfile = null;
        private String entityType = null;
        private int _AD_Table_ID = 0;

        /// <summary>
        /// process Parameters
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            foreach (ProcessInfoParameter element in para)
            {
                String name = element.GetParameterName();
                if (element.GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("EntityType"))
                {
                    entityType = (String)element.GetParameter();
                }
                else if (name.Equals("SQLfile"))
                    SQLfile = (String)element.GetParameter();
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
        }	//	prepare


        private String Trim(String s)
        {
            int len = s.Length;
            int count = len;
            int st = 0;
            int off = 0;      /* avoid getfield opcode */
            char[] val = s.ToCharArray();    /* avoid getfield opcode */

            while ((st < len) && (val[off + st] <= ' '))
            {
                st++;
            }
            while ((st < len) && (val[off + len - 1] <= ' '))
            {
                len--;
            }
            return ((st > 0) || (len < count)) ? s.Substring(st, len) : s;
        }
        /// <summary>
        ///  findNext
        //A utility method to find the first occurence of a keyword within a SQL
        //query. When searching for key, this method will ignore the values that 
        //are surrounded by comment tags and quotes. It will also ignore values
        //that are surrounded by parantheses (subqueries).	
        /// </summary>
        /// <param name="query">sql query</param>
        /// <param name="keyword">the keyword that we are trying to find </param>
        /// <returns>the index of source at which key is found</returns>
        private int FindNext(String query, String keyword)
        {

            int Comments = 0;
            int Quotes = 0;
            int Parantheses = 0;

            for (int j = 0; j < query.Length; j++)
            {
                if (Comments > 0)
                {
                    //if (!query.StartsWith("*/",j))
                    if (!query.Substring(j).StartsWith("*/"))
                    {

                        continue;
                    }
                    Comments--; j++; continue;
                }
                if (Quotes > 0)
                {
                    //if (!query.StartsWith("'",j)) continue;
                    if (!query.Substring(j).StartsWith("'")) continue;
                    Quotes--; continue;
                }
                //if (query.StartsWith("/*",j)) {Comments ++; j++; continue;}
                if (query.Substring(j).StartsWith("/*")) { Comments++; j++; continue; }
                //if (query.StartsWith("'",j)) {Quotes ++; continue;}
                if (query.Substring(j).StartsWith("'")) { Quotes++; continue; }

                if (query[j] == '(') { Parantheses++; continue; }
                if (query[j] == ')') { Parantheses--; continue; }
                //if (Parantheses==0 && query.StartsWith(keyword, j))
                if (Parantheses == 0 && query.Substring(j).StartsWith(keyword))
                {
                    return j;
                }
            }

            return 0;
        }



        /// <summary>
        /// Process	
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            int viewNum = 0;
            //get SQLs
            //InputStream iin = null;
            StreamReader inn = null;
            String targetViewName = null;
            _AD_Table_ID = (GetProcessInfo() != null) ? GetRecord_ID() : 0;
            if (_AD_Table_ID > 0)
            {
                MTable targetTable = MTable.Get(Env.GetCtx(), _AD_Table_ID);
                targetViewName = targetTable.GetTableName();
                entityType = targetTable.GetEntityType();
            }
            try
            {
                inn = new StreamReader(SQLfile);//  FileInputStream(SQLfile);
            }
            catch (Exception e)
            {
                log.Severe(e.Message);
                throw new Exception("SQL file error. file name = " + SQLfile);
            }
            if (inn == null)
            {
                throw new Exception("SQL file error. file name = " + SQLfile);
            }

            List<String> commands = ViewUtil.ReadSqlFromFile(SQLfile);
            if (commands == null)
            {
                throw new Exception("SQL file error. file name = " + SQLfile);
            }


            //put SQL into AD
            // Initialization
            String tableName = null;
            List<String> colName = new List<String>();
            List<String> colSQL = new List<String>();
            List<String> colType = new List<String>();

            Trx myTrx = Trx.Get("ImportView");
            String Acommand = null;
       // endSQL:
            try
            {
                foreach (String command in commands)
                {
                    Acommand = command;
                    bool colNameDone = false;
                    int ir = command.IndexOf('\t');
                    while (ir > 0)
                    {
                        Acommand = Replace(Acommand, '\t', ' ');
                        ir = command.IndexOf('\t');
                    }
                    // Not care about "exit"
                    if (command.Equals("exit") || command.Equals("EXIT"))
                    {
                        // endSQL;
                        break;
                    }
                    if (command.StartsWith("DROP VIEW "))
                        continue;

                    tableName = null;
                    colName.Clear();

                    int iView = command.IndexOf(" VIEW ");
                //eachSQL:
                    if (command.StartsWith("CREATE") && iView > 0)
                    {
                        int iSel = command.IndexOf("SELECT ");
                        String tc = command.Substring(iView + 6, iSel);
                        int ileft = tc.IndexOf('(');
                        int iright = tc.IndexOf(')');
                        if (ileft > 0)
                        {
                            tableName = tc.Substring(0, ileft);
                            tc = tc.Substring(ileft + 1, iright);
                            String[] colNames = tc.Split(new Char[] { ',' });// tc.Substring(ileft + 1, iright).Split(",");
                            foreach (String cn in colNames)
                            {
                                colName.Add(Trim(cn));
                            }
                            colNameDone = true;
                        }
                        else
                        {
                            tableName = tc.Substring(0, tc.IndexOf(" AS "));
                        }
                        if (tableName != null)
                            //jz String trim() doesn't remove tail space tableName.trim();
                            tableName = Trim(tableName);
                        if (tableName == null || tableName.Length == 0)
                        {
                            log.Severe("No view name from the SQL: " + command);
                            continue;
                        }
                        if (_AD_Table_ID != 0 && !(targetViewName.Equals(tableName) || targetViewName.Equals(tableName.ToUpper())))
                        {
                            log.Fine("Skipping view " + targetViewName);
                            continue;
                        }
                        //int itc = tc.substring(tc.length()-1).hashCode();

                        //insert into/update ad_table for each view
                        MTable mt = MTable.Get(Env.GetCtx(), tableName);
                        if (mt != null && !mt.IsView())
                        {
                            log.Severe("Duplicated view name with an existing table for the SQL: " + command);
                            //myTrx.rollback();
                            // eachSQL;
                            break;
                        }

                        if (mt == null)
                        //mt.delete(true,  null);
                        {
                            mt = new MTable(Env.GetCtx(), 0, null);
                            //mt.delete(true,  myTrx.getTrxName());
                            //mt = new MTable(Env.getCtx(), 0, myTrx.getTrxName());
                            //MTable mt = MTable.get(Env.getCtx(), 0);
                            mt.SetTableName(tableName);
                            //mt.setAD_Org_ID(0);
                            //mt.setAD_Client_ID(0); also updatedby, createdby. jz: all default is 0 in PO
                            mt.SetAccessLevel(X_AD_Table.ACCESSLEVEL_ClientPlusOrganization);
                            mt.SetEntityType(entityType);
                            mt.SetIsActive(true);
                            mt.SetIsView(true);
                            mt.SetName("View_" + tableName);
                            mt.SetLoadSeq(900);
                            mt.SetImportTable(null);
                            if (!mt.Save())
                            {
                                log.Severe("Unable to insert into AD_Table for the SQL: " + command);
                                //myTrx.rollback();
                                //eachSQL;
                                break;
                            }
                            log.Info("Add " + tableName + " into AD_Table for the SQL: " + command);
                        }

                        //clean view components and their columns
                        String vcdel = "DELETE FROM AD_ViewComponent WHERE (AD_Table_ID, AD_Client_ID) IN (SELECT AD_Table_ID, AD_Client_ID FROM AD_Table WHERE TableName = '" + tableName + "')";
                        try
                        {
                            DataBase.DB.ExecuteQuery(vcdel, null);
                        }
                        catch (Exception e)
                        {
                            log.Log(Level.SEVERE, vcdel, e);
                        }

                        //insert into ad_viewComponent for each union part
                        Acommand = command.Substring(iSel);
                        //String[] selects =command.Split(new char[]{','});//UNION'},0); // currently does not handle other set operators (e.g. INTERSECT)
                        Regex reg = new Regex(Acommand);
                        String[] selects = reg.Split("UNION");
                        for (int i = 0; i < selects.Length; i++)
                        {
                            int iFrom = FindNext(selects[i], " FROM ");
                            if (iFrom < 0)
                            {
                                log.Severe("No from clause from the SQL: " + command);
                                //eachSQL;
                                break;
                            }
                            colSQL.Clear();
                            colType.Clear();
                            int isel = selects[i].IndexOf("SELECT ");
                            String colstr = selects[i].Substring(isel + 7, iFrom);
                            colstr = colstr + ",";//  concat(",");
                            int iComma = FindNext(colstr, ",");
                            if (iComma == 0)
                            {
                                log.Severe("No view column from the SQL: " + command);
                                ///eachSQL;
                                break;
                            }
                            int iPrevComma = -1;
                            while (iComma != iPrevComma)
                            {
                                String column = colstr.Substring(iPrevComma + 1, iComma);
                                int iAS = column.LastIndexOf(" AS ");
                                String cs = null;
                                if (iAS > 0)
                                {
                                    if (i == 0 && !colNameDone)
                                        colName.Add(column.Substring(iAS + 4, column.Length).Trim());
                                    cs = column.Substring(0, iAS);
                                    //colSQL.add(cols[j].substring(0, iAS));
                                }
                                else
                                {
                                    cs = column;
                                    if (i == 0 && !colNameDone)
                                    {
                                        int iDot = column.IndexOf('.');
                                        String cn = column;
                                        if (iDot > 0)
                                            cn = column.Substring(iDot + 1, column.Length).Trim();
                                        colName.Add(cn);
                                    }
                                }

                                cs = cs.Trim();
                                if (cs.Equals("NULLIF(1,1)") || cs.Equals("nullif(1,1)"))
                                {
                                    cs = null;
                                    colType.Add("I");
                                }
                                else if (cs.Equals("NULLIF('A','A')") || cs.Equals("nullif('A','A')"))
                                {
                                    cs = null;
                                    colType.Add("V");
                                }
                                else
                                    colType.Add(null);

                                colSQL.Add(cs);

                                iPrevComma = iComma;
                                if (iPrevComma + 1 < colstr.Length)
                                    iComma = iPrevComma + 1 + FindNext(colstr.Substring(iPrevComma + 1), ",");
                            }// while (iComma != iPrevComma)

                            String from = selects[i].Substring(iFrom + 1, selects[i].Length);
                            int iWH = from.IndexOf(" WHERE ");
                            String where = null;
                            String others = null;
                            int iGROUP = -1;
                            int iORDER = -1;
                            if (iWH > 0)
                            {
                                where = from.Substring(iWH + 1, from.Length);
                                from = from.Substring(0, iWH);
                                iGROUP = where.IndexOf(" GROUP BY ");
                                iORDER = where.IndexOf(" ORDER BY ");
                                if (iORDER > 0 && iGROUP == -1)
                                    iGROUP = iORDER;
                                if (iGROUP > 0)
                                {
                                    others = where.Substring(iGROUP + 1, where.Length);
                                    where = where.Substring(0, iGROUP);
                                }
                            }
                            else
                            {
                                iGROUP = from.IndexOf(" GROUP BY ");
                                iORDER = from.IndexOf(" ORDER BY ");
                                if (iORDER > 0 && iGROUP == -1)
                                    iGROUP = iORDER;
                                if (iGROUP > 0)
                                {
                                    others = from.Substring(iGROUP + 1, from.Length);
                                    from = from.Substring(0, iGROUP);
                                }
                            }

                            if (from == null || from.Length == 0)
                            {
                                log.Severe("No from clause from the SQL: " + command);
                                //myTrx.rollback();
                                // eachSQL;
                                break;
                            }

                            //insert into AD_ViewComponent
                            //MViewComponent mvc = new MViewComponent(Env.getCtx(), 0, myTrx.getTrxName());
                            MViewComponent mvc = new MViewComponent(Env.GetCtx(), 0, null);
                            mvc.SetName("VC_" + tableName);
                            mvc.SetAD_Table_ID(mt.Get_ID());
                            mvc.SetSeqNo((i + 1) * 10);
                            mvc.SetIsActive(true);
                            mvc.SetEntityType(entityType);
                            //mvc.setAD_Org_ID(0);
                            //mvc.setReferenced_Table_ID(mt.get_ID());
                            String from1 = from.Substring(5);
                            from1 = Trim(from1);
                            int rtix = from1.IndexOf(' ');
                            if (rtix < 0)
                                rtix = from1.Length;
                            String refTab = from1.Substring(0, rtix);
                            refTab = Trim(refTab);
                            MTable rt = MTable.Get(Env.GetCtx(), refTab);
                            if (rt != null)
                                mvc.SetReferenced_Table_ID(rt.Get_ID());
                            else
                                mvc.SetReferenced_Table_ID(0);

                            mvc.SetFromClause(from);
                            mvc.SetWhereClause(where);
                            mvc.SetOtherClause(others);
                            if (!mvc.Save())
                            {
                                log.Severe("unable to create view component " + i + ": " + command);
                                //myTrx.rollback();
                                //eachSQL;
                                break;
                            }

                            //insert into AD_ViewColumn
                            MViewColumn mvcol = null;
                            for (int j = 0; j < colName.Count; j++)
                            {
                                //mvcol = new MViewColumn(Env.getCtx(), 0, myTrx.getTrxName());
                                mvcol = new MViewColumn(Env.GetCtx(), 0, null);
                                //mvcol.setAD_Org_ID(0);
                                mvcol.SetAD_ViewComponent_ID(mvc.Get_ID());
                                mvcol.SetIsActive(true);
                                mvcol.SetEntityType(entityType);
                                log.Info("Importing View " + tableName + "(i,j) = (" + i + ", " + j + ")");
                                mvcol.SetDBDataType(colType[j]);//.get(j));
                                mvcol.SetColumnName(colName[j]);//.get(j));
                                mvcol.SetColumnSQL(colSQL[j]);//.get(j));

                                if (!mvcol.Save())
                                {
                                    log.Severe("unable to create view component " + i + " column: " + colName[j] + " in " + command);
                                    //myTrx.rollback();

                                    break; //eachSQL;
                                }
                            }
                        }//for selects

                        //myTrx.commit();
                        log.Info("Impored view: " + tableName);
                    }//handle create view
                    else
                    {
                        log.Warning("Ignore non create view SQL: " + command);
                        continue;
                    }

                    viewNum++;
                }  // for (String command : commands)
            }
            catch (Exception e)
            {
                log.Severe("Error at importing view SQL: " + Acommand + " \n " + e);
            }
            finally
            {
                if (myTrx != null && myTrx.IsActive())
                {
                    myTrx.Rollback();
                    myTrx.Close();
                }
            }

            if (_AD_Table_ID > 0)
            {
                if (viewNum == 0)
                    return ("Not able to import view " + targetViewName + " from " + SQLfile);
                else
                    return ("Created view " + targetViewName);
            }
            return "Imported View #" + viewNum;
        }


    }
}
