/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : SynchronizeTerminology
 * Purpose        : Synchronize Terminology Process
 * Class Used     : ProcessEngine.SvrProcess
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


using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class SynchronizeTerminology : ProcessEngine.SvrProcess
    {

        /// <summary>
        ///	Prepare (NOP)	 
        /// </summary>
        protected override void Prepare()
        {
        }
        bool result = true;
        /// <summary>
        ///	Process 
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            IDataReader idr = null;
            String sql = null;
            int steps = 0;
            SqlParameter[] param = null;
            log.Info("Adding missing Elements");
            try
            {
                sql = "SELECT DISTINCT ColumnName, Name, Description, Help, EntityType  " +
                        "FROM	VAF_Column c  " +
                        "WHERE NOT EXISTS  " +
                        "	(SELECT * FROM VAF_ColumnDic e  " +
                        "	WHERE UPPER(c.ColumnName)=UPPER(e.ColumnName))";
                idr = DataBase.DB.ExecuteReader(sql);
                DataSet ds = null;
                while (idr.Read())
                {
                    try
                    {
                        if (ds == null)
                        {
                            sql = "INSERT INTO VAF_ColumnDic"
                                    + " (VAF_ColumnDic_ID, VAF_Client_ID, VAF_Org_ID,"
                                    + " IsActive, Created, CreatedBy, Updated, UpdatedBy,"
                                    + " ColumnName, Name, PrintName, Description, Help, EntityType)"
                                    + " VALUES"
                                    + " (@param1, 0, 0,"  //1, NextNo
                                    + " 'Y', CURRENT_TIMESTAMP, 0, CURRENT_TIMESTAMP, 0,"
                                    + " @param2, @param3, @param4, @param5, @param6, @param7)"; //2-7 CC.ColumnName, CC.Name, CC.Name, CC.Description, CC.Help, CC.EntityType	

                        }

                        int id = DataBase.DB.GetNextID(GetVAF_Client_ID(), "VAF_ColumnDic", Get_Trx());
                        if (id <= 0)
                        {
                            log.Severe("Steps  " + steps + ", No NextID ( " + id + ")");
                            idr.Close();
                            return "Steps  " + steps + " No NextID for VAF_ColumnDic";
                        }
                        param = new SqlParameter[7];
                        //pstmt1.setInt(1, id);
                        param[0] = new SqlParameter("@param1", id);
                        //pstmt1.setString(2, rs.getString(1));
                        param[1] = new SqlParameter("@param2", Utility.Util.GetValueOfString(idr[0]));
                        //pstmt1.setString(3, rs.getString(2));
                        param[2] = new SqlParameter("@param3", Utility.Util.GetValueOfString(idr[1]));
                        //pstmt1.setString(4, rs.getString(2));
                        param[3] = new SqlParameter("@param4", Utility.Util.GetValueOfString(idr[1]));
                        //pstmt1.setString(5, rs.getString(3));
                        param[4] = new SqlParameter("@param5", Utility.Util.GetValueOfString(idr[2]));
                        //pstmt1.setString(6, rs.getString(4));
                        param[5] = new SqlParameter("@param6", Utility.Util.GetValueOfString(idr[3]));
                        //pstmt1.setString(7, rs.getString(5));
                        param[6] = new SqlParameter("@param7", Utility.Util.GetValueOfString(idr[4]));
                        DataBase.DB.ExecuteQuery(sql, param, Get_Trx());
                    }
                    catch (Exception e)
                    {
                        log.Log(Level.SEVERE, "Step  " + steps + ":  " + sql, e);
                        if (idr != null)
                        {
                            idr.Close();
                        }
                        return "Steps  " + steps + "  " + e.Message.ToString();
                    }


                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "Step  " + steps + ": " + sql, e);
                result = false;
            }

            steps++;
            try
            {
                sql = "SELECT DISTINCT ColumnName, Name, Description, Help, EntityType " +
                        "FROM	VAF_Job_Para p " +
                        "WHERE NOT EXISTS " +
                        "	(SELECT * FROM VAF_ColumnDic e " +
                        "	WHERE UPPER(p.ColumnName)=UPPER(e.ColumnName))";
                idr = DataBase.DB.ExecuteReader(sql);
                DataSet ds = null;
                while (idr.Read())
                {
                    try
                    {
                        if (ds == null)
                        {
                            sql = "INSERT INTO VAF_ColumnDic"
                                    + " (VAF_ColumnDic_ID, VAF_Client_ID, VAF_Org_ID,"
                                    + " IsActive, Created, CreatedBy, Updated, UpdatedBy,"
                                    + " ColumnName, Name, PrintName, Description, Help, EntityType)"
                                    + " VALUES"
                                    + " (@param1, 0, 0,"  //1, NextNo
                                    + " 'Y', CURRENT_TIMESTAMP, 0, CURRENT_TIMESTAMP, 0,"
                                    + " @param2, @param3, @param4, @param5, @param6, @param7)"; //2-7 CC.ColumnName, CC.Name, CC.Name, CC.Description, CC.Help, CC.EntityType	

                        }
                        int id = DataBase.DB.GetNextID(GetVAF_Client_ID(), "VAF_ColumnDic", Get_Trx());
                        if (id <= 0)
                        {
                            log.Severe("Steps " + steps + ", No NextID (" + id + ")");
                            idr.Close();
                            return "Steps " + steps + " No NextID for VAF_ColumnDic";
                        }
                        param = new SqlParameter[7];
                        param[0] = new SqlParameter("@param1", id);
                        param[1] = new SqlParameter("@param2", Utility.Util.GetValueOfString(idr[0]));
                        param[2] = new SqlParameter("@param3", Utility.Util.GetValueOfString(idr[1]));
                        param[3] = new SqlParameter("@param4", Utility.Util.GetValueOfString(idr[1]));
                        param[4] = new SqlParameter("@param5", Utility.Util.GetValueOfString(idr[2]));
                        param[5] = new SqlParameter("@param6", Utility.Util.GetValueOfString(idr[3]));
                        param[6] = new SqlParameter("@param7", Utility.Util.GetValueOfString(idr[4]));
                        DataBase.DB.ExecuteQuery(sql, param, Get_Trx());
                    }
                    catch (Exception e)
                    {
                        log.Log(Level.SEVERE, "Step " + steps + ": " + sql, e);
                        if (idr != null)
                        {
                            idr.Close();
                        }
                        return "Steps " + steps + " " + e.Message.ToString();
                    }

                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "Step " + steps + ": " + sql, e);
                result = false;
            }

            sql = "INSERT INTO VAF_ColumnDic_TL (VAF_ColumnDic_ID, VAF_Language, VAF_Client_ID, VAF_Org_ID,"
                + "IsActive, Created, CreatedBy, Updated, UpdatedBy,"
                + " Name, PrintName, Description, Help, IsTranslated)"
                + " SELECT m.VAF_ColumnDic_ID, l.VAF_Language, m.VAF_Client_ID, m.VAF_Org_ID,"
                + " m.IsActive, m.Created, m.CreatedBy, m.Updated, m.UpdatedBy,"
                + " m.Name, m.PrintName, m.Description, m.Help, 'N'"
                + " FROM	VAF_ColumnDic m, VAF_Language l"
                + " WHERE	l.IsActive = 'Y' AND l.IsSystemLanguage = 'Y'"
                + " AND	VAF_ColumnDic_ID || VAF_Language NOT IN"
                + " (SELECT VAF_ColumnDic_ID || VAF_Language FROM VAF_ColumnDic_TL)";
            Execute("Adding missing Element Translations", sql, "  rows added: ");

            sql = "INSERT INTO VAF_ColumnDicContext_TL (VAF_ColumnDicContext_ID, VAF_Language, VAF_Client_ID, VAF_Org_ID,"
                + "IsActive, Created, CreatedBy, Updated, UpdatedBy,"
                + " Name, PrintName, Description, Help, IsTranslated)"
                + " SELECT m.VAF_ColumnDicContext_ID, l.VAF_Language, m.VAF_Client_ID, m.VAF_Org_ID,"
                + " m.IsActive, m.Created, m.CreatedBy, m.Updated, m.UpdatedBy,"
                + " m.Name, m.PrintName, m.Description, m.Help, 'N'"
                + " FROM	VAF_ColumnDicContext m, VAF_Language l"
                + " WHERE	l.IsActive = 'Y' AND l.IsSystemLanguage = 'Y'"
                + " AND	VAF_ColumnDicContext_ID || VAF_Language NOT IN"
                + " (SELECT VAF_ColumnDicContext_ID || VAF_Language FROM VAF_ColumnDicContext_TL)";
            Execute("Adding missing context specific Element Translations", sql, "  rows added: ");

            /*
             * 	DBMS_OUTPUT.PUT_LINE('Creating link from Element to Column');
        UPDATE	VAF_Column c
        SET		VAF_ColumnDic_id = 
                    (SELECT VAF_ColumnDic_ID FROM VAF_ColumnDic e 
                    WHERE UPPER(c.ColumnName)=UPPER(e.ColumnName))
        WHERE VAF_ColumnDic_ID IS NULL;
        DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
        COMMIT;
             */
            sql = "UPDATE VAF_Column c"
                    + " SET VAF_ColumnDic_id ="
                    + " (SELECT VAF_ColumnDic_ID FROM VAF_ColumnDic e"
                    + " WHERE UPPER(c.ColumnName)=UPPER(e.ColumnName))"
                    + " WHERE VAF_ColumnDic_ID IS NULL";
            Execute("Creating link from Element to Column", sql, "  rows updated: ");

            /*
                DBMS_OUTPUT.PUT_LINE('Deleting unused Elements');
        DELETE	VAF_ColumnDic_TL
        WHERE	VAF_ColumnDic_ID IN
            (SELECT VAF_ColumnDic_ID FROM VAF_ColumnDic e 
            WHERE NOT EXISTS
                (SELECT * FROM VAF_Column c WHERE UPPER(e.ColumnName)=UPPER(c.ColumnName))
            AND NOT EXISTS
                (SELECT * FROM VAF_QuickSearchColumn c WHERE e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID)
            AND NOT EXISTS
                (SELECT * FROM VAF_Job_Para p WHERE UPPER(e.ColumnName)=UPPER(p.ColumnName)));

        DELETE	VAF_ColumnDic e
        WHERE NOT EXISTS
            (SELECT * FROM VAF_Column c WHERE UPPER(e.ColumnName)=UPPER(c.ColumnName))
        AND NOT EXISTS
            (SELECT * FROM VAF_QuickSearchColumn c WHERE e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID)
        AND NOT EXISTS
            (SELECT * FROM VAF_Job_Para p WHERE UPPER(e.ColumnName)=UPPER(p.ColumnName));
        DBMS_OUTPUT.PUT_LINE('  rows deleted: ' || SQL%ROWCOUNT);

             */
            sql = "DELETE	VAF_ColumnDic_TL"
                + " WHERE	VAF_ColumnDic_ID IN"
                + " (SELECT VAF_ColumnDic_ID FROM VAF_ColumnDic e"
                    + " WHERE NOT EXISTS"
                    + " (SELECT * FROM VAF_Column c WHERE UPPER(e.ColumnName)=UPPER(c.ColumnName))"
                    + " AND NOT EXISTS"
                    + " (SELECT * FROM VAF_QuickSearchColumn c WHERE e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID)"
                    + " AND NOT EXISTS"
                    + " (SELECT * FROM VAF_Job_Para p WHERE UPPER(e.ColumnName)=UPPER(p.ColumnName)))";
            //not do delete for now
            //executesql("Deleting unused Elements-TRL", sql,  "  rows deleted: ");

            sql = "DELETE	VAF_ColumnDic"
                + " WHERE NOT EXISTS"
                + " (SELECT * FROM VAF_Column c WHERE UPPER(e.ColumnName)=UPPER(c.ColumnName))"
                + " AND NOT EXISTS"
                + " (SELECT * FROM VAF_QuickSearchColumn c WHERE e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID)"
                + " AND NOT EXISTS"
                + " (SELECT * FROM VAF_Job_Para p WHERE UPPER(e.ColumnName)=UPPER(p.ColumnName)))";
            //executesql("Deleting unused Elements", sql,  "  rows deleted: ");

            /*
                --	Columns
        DBMS_OUTPUT.PUT_LINE('Synchronize Column');
	
    **  Identify offending column
    SELECT UPPER(ColumnName)
    FROM VAF_ColumnDic
    GROUP BY UPPER(ColumnName)
    HAVING COUNT(UPPER(ColumnName)) > 1

    SELECT c.ColumnName, e.ColumnName
    FROM VAF_Column c
      INNER JOIN VAF_ColumnDic e ON (c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID)
    WHERE c.ColumnName <> e.ColumnName
    **

        UPDATE VAF_Column c
            SET	(ColumnName, Name, Description, Help) = 
                    (SELECT ColumnName, Name, Description, Help 
                    FROM VAF_ColumnDic e WHERE c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID),
                Updated = SysDate
        WHERE EXISTS (SELECT * FROM VAF_ColumnDic e 
                    WHERE c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID
                      AND (c.ColumnName <> e.ColumnName OR c.Name <> e.Name 
                        OR NVL(c.Description,' ') <> NVL(e.Description,' ') OR NVL(c.Help,' ') <> NVL(e.Help,' ')));
        DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);

             */
            sql = "UPDATE	VAF_Column c"
                    + " SET	(ColumnName, Name, Description, Help, Updated) ="
                    + " (SELECT ColumnName, Name, Description, Help, CURRENT_TIMESTAMP"
                    + " FROM VAF_ColumnDic e WHERE c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID)"
                    + " WHERE EXISTS (SELECT * FROM VAF_ColumnDic e "
                    + "  WHERE c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID"
                    + "   AND (c.ColumnName <> e.ColumnName OR c.Name <> e.Name "
                    + "   OR NVL(c.Description,' ') <> NVL(e.Description,' ') OR NVL(c.Help,' ') <> NVL(e.Help,' ')))";
            Execute("Synchronize Column", sql, "  rows updated: ");

            /*
             * 	--	Fields should now be syncronized
        DBMS_OUTPUT.PUT_LINE('Synchronize Field');
        UPDATE VAF_Field f
            SET (Name, Description, Help) = 
                    (SELECT e.Name, e.Description, e.Help
                    FROM VAF_ColumnDic e, VAF_Column c
                    WHERE e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID AND c.VAF_Column_ID=f.VAF_Column_ID),
                Updated = SysDate
        WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y'
          AND EXISTS (SELECT * FROM VAF_ColumnDic e, VAF_Column c
                    WHERE f.VAF_Column_ID=c.VAF_Column_ID
                      AND c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID AND c.VAF_Job_ID IS NULL
                      AND (f.Name <> e.Name OR NVL(f.Description,' ') <> NVL(e.Description,' ') OR NVL(f.Help,' ') <> NVL(e.Help,' ')));
        DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);

             */
            sql = "UPDATE VAF_Field f"
                + " SET (Name, Description, Help, Updated) = "
                + "             (SELECT e.Name, e.Description, e.Help, CURRENT_TIMESTAMP"
                + "             FROM VAF_ColumnDic e, VAF_Column c"
                + "     	    WHERE e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID AND c.VAF_Column_ID=f.VAF_Column_ID)"
                + " 	WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y'"
                + " 	  AND EXISTS (SELECT * FROM VAF_ColumnDic e, VAF_Column c"
                + " 				WHERE f.VAF_Column_ID=c.VAF_Column_ID"
                + " 				  AND c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID AND c.VAF_Job_ID IS NULL"
                + " 				  AND (f.Name <> e.Name OR NVL(f.Description,' ') <> NVL(e.Description,' ') OR NVL(f.Help,' ') <> NVL(e.Help,' ')))"
                + "AND NOT EXISTS ("
                + "      SELECT *"
                + "      FROM VAF_Tab t, AD_Window w, VAF_Column c, VAF_ColumnDicContext e"
                + "      WHERE t.VAF_Tab_ID=f.VAF_Tab_ID AND w.AD_Window_ID=t.AD_Window_ID"
                + "      AND c.VAF_Column_ID=f.VAF_Column_ID AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID"
                + "      AND e.VAF_ContextScope_ID=COALESCE(t.VAF_ContextScope_ID, w.VAF_ContextScope_ID))";
            Execute("Synchronize Field", sql, "  rows updated: ");

            /*
             * 	--	Field Translations
    DBMS_OUTPUT.PUT_LINE('Synchronize Field Translations');
    UPDATE VAF_Field_TL trl
        SET Name = (SELECT e.Name FROM vaf_columndic_tl e, VAF_Column c, VAF_Field f
                    WHERE e.VAF_Language=trl.VAF_Language AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID 
                      AND c.VAF_Column_ID=f.VAF_Column_ID AND f.VAF_Field_ID=trl.VAF_Field_ID),
            Description = (SELECT e.Description FROM vaf_columndic_tl e, VAF_Column c, VAF_Field f
                    WHERE e.VAF_Language=trl.VAF_Language AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID 
                      AND c.VAF_Column_ID=f.VAF_Column_ID AND f.VAF_Field_ID=trl.VAF_Field_ID),
            Help = (SELECT e.Help FROM vaf_columndic_tl e, VAF_Column c, VAF_Field f
                    WHERE e.VAF_Language=trl.VAF_Language AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID 
                      AND c.VAF_Column_ID=f.VAF_Column_ID AND f.VAF_Field_ID=trl.VAF_Field_ID),
            IsTranslated = (SELECT e.IsTranslated FROM vaf_columndic_tl e, VAF_Column c, VAF_Field f
                    WHERE e.VAF_Language=trl.VAF_Language AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID 
                      AND c.VAF_Column_ID=f.VAF_Column_ID AND f.VAF_Field_ID=trl.VAF_Field_ID),
            Updated = SysDate
    WHERE EXISTS (SELECT * FROM VAF_Field f, vaf_columndic_tl e, VAF_Column c
                WHERE trl.VAF_Field_ID=f.VAF_Field_ID
                  AND f.VAF_Column_ID=c.VAF_Column_ID
                  AND c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID AND c.VAF_Job_ID IS NULL
                  AND trl.VAF_Language=e.VAF_Language
                  AND f.IsCentrallyMaintained='Y' AND f.IsActive='Y'
                  AND (trl.Name <> e.Name OR NVL(trl.Description,' ') <> NVL(e.Description,' ') OR NVL(trl.Help,' ') <> NVL(e.Help,' ')));
    DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);

             */
            sql = "	UPDATE VAF_Field_TL trl"
                + " SET Name = (SELECT e.Name FROM vaf_columndic_tl e, VAF_Column c, VAF_Field f"
                + " 				WHERE e.VAF_Language=trl.VAF_Language AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID"
                + " 				  AND c.VAF_Column_ID=f.VAF_Column_ID AND f.VAF_Field_ID=trl.VAF_Field_ID),"
                + " 		Description = (SELECT e.Description FROM vaf_columndic_tl e, VAF_Column c, VAF_Field f"
                + " 				WHERE e.VAF_Language=trl.VAF_Language AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID"
                + " 				  AND c.VAF_Column_ID=f.VAF_Column_ID AND f.VAF_Field_ID=trl.VAF_Field_ID),"
                + " 		Help = (SELECT e.Help FROM vaf_columndic_tl e, VAF_Column c, VAF_Field f"
                + " 				WHERE e.VAF_Language=trl.VAF_Language AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID"
                + " 				  AND c.VAF_Column_ID=f.VAF_Column_ID AND f.VAF_Field_ID=trl.VAF_Field_ID),"
                + " 		IsTranslated = (SELECT e.IsTranslated FROM vaf_columndic_tl e, VAF_Column c, VAF_Field f"
                + " 				WHERE e.VAF_Language=trl.VAF_Language AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID"
                + " 				  AND c.VAF_Column_ID=f.VAF_Column_ID AND f.VAF_Field_ID=trl.VAF_Field_ID),"
                + " 		Updated = CURRENT_TIMESTAMP"
                + " 	WHERE EXISTS (SELECT * FROM VAF_Field f, vaf_columndic_tl e, VAF_Column c"
                + " 				WHERE trl.VAF_Field_ID=f.VAF_Field_ID"
                + " 				  AND f.VAF_Column_ID=c.VAF_Column_ID"
                + " 				  AND c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID AND c.VAF_Job_ID IS NULL"
                + " 				  AND trl.VAF_Language=e.VAF_Language"
                + " 				  AND f.IsCentrallyMaintained='Y' AND f.IsActive='Y'"
                + " 				  AND (trl.Name <> e.Name OR NVL(trl.Description,' ') <> NVL(e.Description,' ') OR NVL(trl.Help,' ') <> NVL(e.Help,' ')))"
                + "AND NOT EXISTS ("
                + "      SELECT *"
                + "      FROM VAF_Tab t, AD_Window w, VAF_Column c, VAF_ColumnDicContext e, VAF_Field f"
                + "      WHERE t.VAF_Tab_ID=f.VAF_Tab_ID AND w.AD_Window_ID=t.AD_Window_ID"
                + "      AND c.VAF_Column_ID=f.VAF_Column_ID AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID"
                + "      AND e.VAF_ContextScope_ID=COALESCE(t.VAF_ContextScope_ID, w.VAF_ContextScope_ID)"
                + "      AND f.VAF_Field_ID = trl.VAF_Field_ID)";
            Execute("Synchronize Field Translations", sql, "  rows updated: ");

            /*	
    --	Fields should now be syncronized
    DBMS_OUTPUT.PUT_LINE('Synchronize PO Field');
    UPDATE VAF_Field f
        SET Name = (SELECT e.PO_Name FROM VAF_ColumnDic e, VAF_Column c
                    WHERE e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID AND c.VAF_Column_ID=f.VAF_Column_ID),
            Description = (SELECT e.PO_Description FROM VAF_ColumnDic e, VAF_Column c
                    WHERE e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID AND c.VAF_Column_ID=f.VAF_Column_ID),
            Help = (SELECT e.PO_Help FROM VAF_ColumnDic e, VAF_Column c
                    WHERE e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID AND c.VAF_Column_ID=f.VAF_Column_ID),
            Updated = SysDate
    WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y'
      AND EXISTS (SELECT * FROM VAF_ColumnDic e, VAF_Column c
                WHERE f.VAF_Column_ID=c.VAF_Column_ID
                  AND c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID AND c.VAF_Job_ID IS NULL
                  AND (f.Name <> e.PO_Name OR NVL(f.Description,' ') <> NVL(e.PO_Description,' ') OR NVL(f.Help,' ') <> NVL(e.PO_Help,' '))
                  AND e.PO_Name IS NOT NULL)
      AND EXISTS (SELECT * FROM VAF_Tab t, AD_Window w
                WHERE f.VAF_Tab_ID=t.VAF_Tab_ID
                  AND t.AD_Window_ID=w.AD_Window_ID
                  AND w.IsSOTrx='N');
    DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
    */
            /*TODO  ?
                        sql = "	UPDATE VAF_Field f"
                                + " SET Name = (SELECT e.Name FROM VAF_ColumnDic e, VAF_Column c"
                                + " 			WHERE e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID AND c.VAF_Column_ID=f.VAF_Column_ID),"
                                + " 	Description = (SELECT e.Description FROM VAF_ColumnDic e, VAF_Column c"
                                + " 			WHERE e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID AND c.VAF_Column_ID=f.VAF_Column_ID),"
                                + " 	Help = (SELECT e.Help FROM VAF_ColumnDic e, VAF_Column c"
                                + " 			WHERE e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID AND c.VAF_Column_ID=f.VAF_Column_ID),"
                                + " 	Updated = CURRENT_TIMESTAMP"
                                + " WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y'"
                                + "   AND EXISTS (SELECT * FROM VAF_ColumnDic e, VAF_Column c"
                                + " 			WHERE f.VAF_Column_ID=c.VAF_Column_ID"
                                + " 			  AND c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID AND c.VAF_Job_ID IS NULL"
                                + " 			  AND (f.Name <> e.Name OR NVL(f.Description,' ') <> NVL(e.Description,' ') OR NVL(f.Help,' ') <> NVL(e.Help,' '))"
                                + " 			  AND e.Name IS NOT NULL)"
                                + "   AND EXISTS (SELECT * FROM VAF_Tab t, AD_Window w"
                                + " 			WHERE f.VAF_Tab_ID=t.VAF_Tab_ID"
                                + " 			  AND t.AD_Window_ID=w.AD_Window_ID" 
                                + " 			  AND w.VAF_ContextScope_ID IS NULL" 
                                + " 			  AND t.VAF_ContextScope_ID IS NULL)";
                        executesql("Synchronize PO Translations", sql,  "  rows updated: ");
                        */

            /*
    --	Field Translations
    DBMS_OUTPUT.PUT_LINE('Synchronize PO Field Translations');
    UPDATE VAF_Field_TL trl
        SET Name = (SELECT e.PO_Name FROM vaf_columndic_tl e, VAF_Column c, VAF_Field f
                    WHERE e.VAF_Language=trl.VAF_Language AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID 
                      AND c.VAF_Column_ID=f.VAF_Column_ID AND f.VAF_Field_ID=trl.VAF_Field_ID),
            Description = (SELECT e.PO_Description FROM vaf_columndic_tl e, VAF_Column c, VAF_Field f
                    WHERE e.VAF_Language=trl.VAF_Language AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID 
                      AND c.VAF_Column_ID=f.VAF_Column_ID AND f.VAF_Field_ID=trl.VAF_Field_ID),
            Help = (SELECT e.PO_Help FROM vaf_columndic_tl e, VAF_Column c, VAF_Field f
                    WHERE e.VAF_Language=trl.VAF_Language AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID 
                      AND c.VAF_Column_ID=f.VAF_Column_ID AND f.VAF_Field_ID=trl.VAF_Field_ID),
            IsTranslated = (SELECT e.IsTranslated FROM vaf_columndic_tl e, VAF_Column c, VAF_Field f
                    WHERE e.VAF_Language=trl.VAF_Language AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID 
                      AND c.VAF_Column_ID=f.VAF_Column_ID AND f.VAF_Field_ID=trl.VAF_Field_ID),
            Updated = CURRENT_TIMESTAMP
    WHERE EXISTS (SELECT * FROM VAF_Field f, vaf_columndic_tl e, VAF_Column c
                WHERE trl.VAF_Field_ID=f.VAF_Field_ID
                  AND f.VAF_Column_ID=c.VAF_Column_ID
                  AND c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID AND c.VAF_Job_ID IS NULL
                  AND trl.VAF_Language=e.VAF_Language
                  AND f.IsCentrallyMaintained='Y' AND f.IsActive='Y'
                  AND (trl.Name <> e.PO_Name OR NVL(trl.Description,' ') <> NVL(e.PO_Description,' ') OR NVL(trl.Help,' ') <> NVL(e.PO_Help,' '))
                  AND e.PO_Name IS NOT NULL)
      AND EXISTS (SELECT * FROM VAF_Field f, VAF_Tab t, AD_Window w
                WHERE trl.VAF_Field_ID=f.VAF_Field_ID
                  AND f.VAF_Tab_ID=t.VAF_Tab_ID
                  AND t.AD_Window_ID=w.AD_Window_ID
                  AND w.IsSOTrx='N');
    DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
    */
            /*
            sql = "	UPDATE VAF_Field_TL trl"
                    + " SET Name = (SELECT e.Name FROM vaf_columndic_tl e, VAF_Column c, VAF_Field f"
                    + " 			WHERE e.VAF_Language=trl.VAF_Language AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID "
                    + " 			  AND c.VAF_Column_ID=f.VAF_Column_ID AND f.VAF_Field_ID=trl.VAF_Field_ID),"
                    + " 	Description = (SELECT e.Description FROM vaf_columndic_tl e, VAF_Column c, VAF_Field f"
                    + " 			WHERE e.VAF_Language=trl.VAF_Language AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID "
                    + " 			  AND c.VAF_Column_ID=f.VAF_Column_ID AND f.VAF_Field_ID=trl.VAF_Field_ID),"
                    + " 	Help = (SELECT e.Help FROM vaf_columndic_tl e, VAF_Column c, VAF_Field f"
                    + " 			WHERE e.VAF_Language=trl.VAF_Language AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID "
                    + " 			  AND c.VAF_Column_ID=f.VAF_Column_ID AND f.VAF_Field_ID=trl.VAF_Field_ID),"
                    + " 	IsTranslated = (SELECT e.IsTranslated FROM vaf_columndic_tl e, VAF_Column c, VAF_Field f"
                    + " 			WHERE e.VAF_Language=trl.VAF_Language AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID "
                    + " 			  AND c.VAF_Column_ID=f.VAF_Column_ID AND f.VAF_Field_ID=trl.VAF_Field_ID),"
                    + " 	Updated = CURRENT_TIMESTAMP"
                    + " 	WHERE EXISTS (SELECT * FROM VAF_Field f, vaf_columndic_tl e, VAF_Column c"
                    + " 				WHERE trl.VAF_Field_ID=f.VAF_Field_ID"
                    + " 				  AND f.VAF_Column_ID=c.VAF_Column_ID"
                    + " 				  AND c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID AND c.VAF_Job_ID IS NULL"
                    + " 				  AND trl.VAF_Language=e.VAF_Language"
                    + " 				  AND f.IsCentrallyMaintained='Y' AND f.IsActive='Y'"
                    + " 				  AND (trl.Name <> e.Name OR NVL(trl.Description,' ') <> NVL(e.Description,' ') OR NVL(trl.Help,' ') <> NVL(e.Help,' '))"
                    + " 				  AND e.Name IS NOT NULL)"
                    + " 	  AND EXISTS (SELECT * FROM VAF_Field f, VAF_Tab t, AD_Window w"
                    + " 				WHERE trl.VAF_Field_ID=f.VAF_Field_ID"
                    + " 				  AND f.VAF_Tab_ID=t.VAF_Tab_ID"
                    + " 				  AND t.AD_Window_ID=w.AD_Window_ID"
                    + " 			  AND w.VAF_ContextScope_ID IS NULL" 
                    + " 			  AND t.VAF_ContextScope_ID IS NULL)";
            executesql("Synchronize PO Field Translations", sql,  "  rows updated: ");
            */

            //
            sql = "	UPDATE VAF_Field f"
                + " SET Name = (SELECT e.Name FROM VAF_ColumnDicContext e JOIN VAF_Column c ON (e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID)"
                + " 			WHERE c.VAF_Column_ID=f.VAF_Column_ID"
                + "   				AND EXISTS (SELECT * FROM VAF_Tab t JOIN AD_Window w ON (t.AD_Window_ID=w.AD_Window_ID)"
                + " 					WHERE f.VAF_Tab_ID=t.VAF_Tab_ID"
                + " 			  		AND (w.VAF_ContextScope_ID IS NOT NULL AND t.VAF_ContextScope_ID IS NULL AND e.VAF_ContextScope_ID=w.VAF_ContextScope_ID"
                + " 			  			OR t.VAF_ContextScope_ID IS NOT NULL AND e.VAF_ContextScope_ID=t.VAF_ContextScope_ID))),"
                + " 	Description = (SELECT e.Description FROM VAF_ColumnDicContext e JOIN VAF_Column c ON (e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID)"
                + " 			WHERE c.VAF_Column_ID=f.VAF_Column_ID"
                + "   				AND EXISTS (SELECT * FROM VAF_Tab t JOIN AD_Window w ON (t.AD_Window_ID=w.AD_Window_ID)"
                + " 					WHERE f.VAF_Tab_ID=t.VAF_Tab_ID"
                + " 			  		AND (w.VAF_ContextScope_ID IS NOT NULL AND t.VAF_ContextScope_ID IS NULL AND e.VAF_ContextScope_ID=w.VAF_ContextScope_ID"
                + " 			  			OR t.VAF_ContextScope_ID IS NOT NULL AND e.VAF_ContextScope_ID=t.VAF_ContextScope_ID))),"
                + " 	Help = (SELECT e.Help FROM VAF_ColumnDicContext e JOIN VAF_Column c ON (e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID)"
                + " 			WHERE c.VAF_Column_ID=f.VAF_Column_ID"
                + "   				AND EXISTS (SELECT * FROM VAF_Tab t JOIN AD_Window w ON (t.AD_Window_ID=w.AD_Window_ID)"
                + " 					WHERE f.VAF_Tab_ID=t.VAF_Tab_ID"
                + " 			  		AND (w.VAF_ContextScope_ID IS NOT NULL AND t.VAF_ContextScope_ID IS NULL AND e.VAF_ContextScope_ID=w.VAF_ContextScope_ID"
                + " 			  			OR t.VAF_ContextScope_ID IS NOT NULL AND e.VAF_ContextScope_ID=t.VAF_ContextScope_ID))),"
                + " 	Updated = CURRENT_TIMESTAMP"
                + " WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y'"
                + "   AND EXISTS (SELECT * FROM VAF_ColumnDicContext e, VAF_Column c, VAF_Tab t, AD_Window w"
                + " 			WHERE f.VAF_Column_ID=c.VAF_Column_ID"
                + " 			  AND c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID AND c.VAF_Job_ID IS NULL"
                + " 			  AND (f.Name <> e.Name OR NVL(f.Description,' ') <> NVL(e.Description,' ') OR NVL(f.Help,' ') <> NVL(e.Help,' '))"
                + " 			  AND e.Name IS NOT NULL"
                + "               AND t.VAF_Tab_ID=f.VAF_Tab_ID AND w.AD_Window_ID=t.AD_Window_ID"
                + "		          AND e.VAF_ContextScope_ID = COALESCE(t.VAF_ContextScope_ID,w.VAF_ContextScope_ID))";
            Execute("Synchronize Fields with ElementCTX", sql, "  rows updated: ");

            //
            sql = "	UPDATE VAF_Field_TL trl"
                + " SET Name = (SELECT et.Name FROM VAF_ColumnDicContext_TL et "
                + " 				JOIN VAF_ColumnDicContext e ON (et.VAF_ColumnDicContext_ID=e.VAF_ColumnDicContext_ID)"
                + "					JOIN VAF_Column c ON (e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID) "
                + "					JOIN VAF_Field f ON (c.VAF_Column_ID=f.VAF_Column_ID)"
                + " 			WHERE et.VAF_Language=trl.VAF_Language AND f.VAF_Field_ID=trl.VAF_Field_ID"
                + "   				AND EXISTS (SELECT * FROM VAF_Tab t JOIN AD_Window w ON (t.AD_Window_ID=w.AD_Window_ID)"
                + " 					WHERE f.VAF_Tab_ID=t.VAF_Tab_ID"
                + " 			  		AND (w.VAF_ContextScope_ID IS NOT NULL AND t.VAF_ContextScope_ID IS NULL AND e.VAF_ContextScope_ID=w.VAF_ContextScope_ID"
                + " 			  			OR t.VAF_ContextScope_ID IS NOT NULL AND e.VAF_ContextScope_ID=t.VAF_ContextScope_ID))),"
                + " 	Description = (SELECT et.Description FROM VAF_ColumnDicContext_TL et "
                + " 				JOIN VAF_ColumnDicContext e ON (et.VAF_ColumnDicContext_ID=e.VAF_ColumnDicContext_ID)"
                + "					JOIN VAF_Column c ON (e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID) "
                + "					JOIN VAF_Field f ON (c.VAF_Column_ID=f.VAF_Column_ID)"
                + " 			WHERE et.VAF_Language=trl.VAF_Language AND f.VAF_Field_ID=trl.VAF_Field_ID"
                + "   				AND EXISTS (SELECT * FROM VAF_Tab t JOIN AD_Window w ON (t.AD_Window_ID=w.AD_Window_ID)"
                + " 					WHERE f.VAF_Tab_ID=t.VAF_Tab_ID"
                + " 			  		AND (w.VAF_ContextScope_ID IS NOT NULL AND t.VAF_ContextScope_ID IS NULL AND e.VAF_ContextScope_ID=w.VAF_ContextScope_ID"
                + " 			  			OR t.VAF_ContextScope_ID IS NOT NULL AND e.VAF_ContextScope_ID=t.VAF_ContextScope_ID))),"
                + " 	Help = (SELECT et.Help FROM VAF_ColumnDicContext_TL et "
                + " 				JOIN VAF_ColumnDicContext e ON (et.VAF_ColumnDicContext_ID=e.VAF_ColumnDicContext_ID)"
                + "					JOIN VAF_Column c ON (e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID) "
                + "					JOIN VAF_Field f ON (c.VAF_Column_ID=f.VAF_Column_ID)"
                + " 			WHERE et.VAF_Language=trl.VAF_Language AND f.VAF_Field_ID=trl.VAF_Field_ID"
                + "   				AND EXISTS (SELECT * FROM VAF_Tab t JOIN AD_Window w ON (t.AD_Window_ID=w.AD_Window_ID)"
                + " 					WHERE f.VAF_Tab_ID=t.VAF_Tab_ID"
                + " 			  		AND (w.VAF_ContextScope_ID IS NOT NULL AND t.VAF_ContextScope_ID IS NULL AND e.VAF_ContextScope_ID=w.VAF_ContextScope_ID"
                + " 			  			OR t.VAF_ContextScope_ID IS NOT NULL AND e.VAF_ContextScope_ID=t.VAF_ContextScope_ID))),"
                + " 	IsTranslated = (SELECT et.IsTranslated FROM VAF_ColumnDicContext_TL et "
                + " 				JOIN VAF_ColumnDicContext e ON (et.VAF_ColumnDicContext_ID=e.VAF_ColumnDicContext_ID)"
                + "					JOIN VAF_Column c ON (e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID) "
                + "					JOIN VAF_Field f ON (c.VAF_Column_ID=f.VAF_Column_ID)"
                + " 			WHERE et.VAF_Language=trl.VAF_Language AND f.VAF_Field_ID=trl.VAF_Field_ID"
                + "   				AND EXISTS (SELECT * FROM VAF_Tab t JOIN AD_Window w ON (t.AD_Window_ID=w.AD_Window_ID)"
                + " 					WHERE f.VAF_Tab_ID=t.VAF_Tab_ID"
                + " 			  		AND (w.VAF_ContextScope_ID IS NOT NULL AND t.VAF_ContextScope_ID IS NULL AND e.VAF_ContextScope_ID=w.VAF_ContextScope_ID"
                + " 			  			OR t.VAF_ContextScope_ID IS NOT NULL AND e.VAF_ContextScope_ID=t.VAF_ContextScope_ID))),"
                + " 	Updated = CURRENT_TIMESTAMP"
                + " 	WHERE EXISTS (SELECT * FROM VAF_Field f, VAF_ColumnDicContext_TL et, VAF_ColumnDicContext e,  VAF_Column c, VAF_Tab t, AD_Window w"
                + " 				WHERE trl.VAF_Field_ID=f.VAF_Field_ID"
                + " 				  AND f.VAF_Column_ID=c.VAF_Column_ID"
                + " 				  AND c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID AND et.VAF_ColumnDicContext_ID=e.VAF_ColumnDicContext_ID AND c.VAF_Job_ID IS NULL"
                + " 				  AND trl.VAF_Language=et.VAF_Language"
                + " 				  AND f.IsCentrallyMaintained='Y' AND f.IsActive='Y'"
                + " 				  AND (trl.Name <> et.Name OR NVL(trl.Description,' ') <> NVL(et.Description,' ') OR NVL(trl.Help,' ') <> NVL(et.Help,' '))"
                + " 				  AND et.Name IS NOT NULL"
                + "                   AND t.VAF_Tab_ID=f.VAF_Tab_ID AND w.AD_Window_ID=t.AD_Window_ID"
                + "                   AND e.VAF_ContextScope_ID = COALESCE(t.VAF_ContextScope_ID,w.VAF_ContextScope_ID))";
            Execute("Synchronize fields with context specific element translations", sql, "  rows updated: ");

            /*
                --	Fields from Process
                DBMS_OUTPUT.PUT_LINE('Synchronize Field from Process');
                UPDATE VAF_Field f
                    SET Name = (SELECT p.Name FROM VAF_Job p, VAF_Column c WHERE p.VAF_Job_ID=c.VAF_Job_ID
                                AND c.VAF_Column_ID=f.VAF_Column_ID),
                        Description = (SELECT p.Description FROM VAF_Job p, VAF_Column c WHERE p.VAF_Job_ID=c.VAF_Job_ID
                                AND c.VAF_Column_ID=f.VAF_Column_ID),
                        Help = (SELECT p.Help FROM VAF_Job p, VAF_Column c WHERE p.VAF_Job_ID=c.VAF_Job_ID
                                AND c.VAF_Column_ID=f.VAF_Column_ID),
                        Updated = SysDate
                WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y' 
                  AND EXISTS (SELECT * FROM VAF_Job p, VAF_Column c
                            WHERE c.VAF_Job_ID=p.VAF_Job_ID AND f.VAF_Column_ID=c.VAF_Column_ID
                            AND (f.Name<>p.Name OR NVL(f.Description,' ')<>NVL(p.Description,' ') OR NVL(f.Help,' ')<>NVL(p.Help,' ')));
                DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
                */


            //// Note 1 : Proeccess name was setting as field name in windows button so removed name and set only description and help ////

            // Removed This
            /* SET Name = (SELECT p.Name FROM VAF_Job p, VAF_Column c WHERE p.VAF_Job_ID=c.VAF_Job_ID
                                AND c.VAF_Column_ID=f.VAF_Column_ID),
             */


            // Commented 
            //sql = "	UPDATE VAF_Field f"
            //        + " SET Description = (SELECT p.Description FROM VAF_Job p, VAF_Column c WHERE p.VAF_Job_ID=c.VAF_Job_ID"
            //        + " 			AND c.VAF_Column_ID=f.VAF_Column_ID),"
            //        + " 	Help = (SELECT p.Help FROM VAF_Job p, VAF_Column c WHERE p.VAF_Job_ID=c.VAF_Job_ID"
            //        + " 			AND c.VAF_Column_ID=f.VAF_Column_ID),"
            //        + " 	Updated = CURRENT_TIMESTAMP"
            //        + " WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y' "
            //        + "   AND EXISTS (SELECT * FROM VAF_Job p, VAF_Column c"
            //        + " 			WHERE c.VAF_Job_ID=p.VAF_Job_ID AND f.VAF_Column_ID=c.VAF_Column_ID"
            //        + " 			AND (f.Name<>p.Name OR NVL(f.Description,' ')<>NVL(p.Description,' ') OR NVL(f.Help,' ')<>NVL(p.Help,' ')))";
            //Execute("Synchronize Field from Process", sql, "  rows updated: ");

            /*
            --	Field Translations from Process
            DBMS_OUTPUT.PUT_LINE('Synchronize Field Trl from Process Trl');
            UPDATE VAF_Field_TL trl
                SET Name = (SELECT p.Name FROM VAF_Job_TL p, VAF_Column c, VAF_Field f 
                            WHERE p.VAF_Job_ID=c.VAF_Job_ID AND c.VAF_Column_ID=f.VAF_Column_ID
                            AND f.VAF_Field_ID=trl.VAF_Field_ID AND p.VAF_Language=trl.VAF_Language),
                    Description = (SELECT p.Description FROM VAF_Job_TL p, VAF_Column c, VAF_Field f 
                            WHERE p.VAF_Job_ID=c.VAF_Job_ID AND c.VAF_Column_ID=f.VAF_Column_ID
                            AND f.VAF_Field_ID=trl.VAF_Field_ID AND p.VAF_Language=trl.VAF_Language),
                    Help = (SELECT p.Help FROM VAF_Job_TL p, VAF_Column c, VAF_Field f 
                            WHERE p.VAF_Job_ID=c.VAF_Job_ID AND c.VAF_Column_ID=f.VAF_Column_ID
                            AND f.VAF_Field_ID=trl.VAF_Field_ID AND p.VAF_Language=trl.VAF_Language),
                    IsTranslated = (SELECT p.IsTranslated FROM VAF_Job_TL p, VAF_Column c, VAF_Field f 
                            WHERE p.VAF_Job_ID=c.VAF_Job_ID AND c.VAF_Column_ID=f.VAF_Column_ID
                            AND f.VAF_Field_ID=trl.VAF_Field_ID AND p.VAF_Language=trl.VAF_Language),
                    Updated = SysDate
            WHERE EXISTS (SELECT * FROM VAF_Job_TL p, VAF_Column c, VAF_Field f
                        WHERE c.VAF_Job_ID=p.VAF_Job_ID AND f.VAF_Column_ID=c.VAF_Column_ID
                        AND f.VAF_Field_ID=trl.VAF_Field_ID AND p.VAF_Language=trl.VAF_Language
                        AND f.IsCentrallyMaintained='Y' AND f.IsActive='Y'
                        AND (trl.Name<>p.Name OR NVL(trl.Description,' ')<>NVL(p.Description,' ') OR NVL(trl.Help,' ')<>NVL(p.Help,' ')));
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */

            // Refer to Note 1 above


            sql = "UPDATE VAF_Field_TL trl"
                    + " SET Description = (SELECT p.Description FROM VAF_Job_TL p, VAF_Column c, VAF_Field f "
                    + " 			WHERE p.VAF_Job_ID=c.VAF_Job_ID AND c.VAF_Column_ID=f.VAF_Column_ID"
                    + " 			AND f.VAF_Field_ID=trl.VAF_Field_ID AND p.VAF_Language=trl.VAF_Language),"
                    + " 	Help = (SELECT p.Help FROM VAF_Job_TL p, VAF_Column c, VAF_Field f "
                    + " 			WHERE p.VAF_Job_ID=c.VAF_Job_ID AND c.VAF_Column_ID=f.VAF_Column_ID"
                    + " 			AND f.VAF_Field_ID=trl.VAF_Field_ID AND p.VAF_Language=trl.VAF_Language),"
                    + " 	IsTranslated = (SELECT p.IsTranslated FROM VAF_Job_TL p, VAF_Column c, VAF_Field f "
                    + " 			WHERE p.VAF_Job_ID=c.VAF_Job_ID AND c.VAF_Column_ID=f.VAF_Column_ID"
                    + " 			AND f.VAF_Field_ID=trl.VAF_Field_ID AND p.VAF_Language=trl.VAF_Language),"
                    + " 	Updated = CURRENT_TIMESTAMP"
                    + " WHERE EXISTS (SELECT * FROM VAF_Job_TL p, VAF_Column c, VAF_Field f"
                    + " 		WHERE c.VAF_Job_ID=p.VAF_Job_ID AND f.VAF_Column_ID=c.VAF_Column_ID"
                    + " 		AND f.VAF_Field_ID=trl.VAF_Field_ID AND p.VAF_Language=trl.VAF_Language"
                    + " 		AND f.IsCentrallyMaintained='Y' AND f.IsActive='Y'"
                    + " 		AND (trl.Name<>p.Name OR NVL(trl.Description,' ')<>NVL(p.Description,' ') OR NVL(trl.Help,' ')<>NVL(p.Help,' ')))";
            Execute("Synchronize Field Translations", sql, "  rows updated: ");

            /*
            --	Sync Parameter ColumnName
            UPDATE	VAF_Job_Para f
                SET	ColumnName = (SELECT e.ColumnName FROM VAF_ColumnDic e
                            WHERE UPPER(e.ColumnName)=UPPER(f.ColumnName))
            WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y'
              AND EXISTS (SELECT * FROM VAF_ColumnDic e
                WHERE UPPER(e.ColumnName)=UPPER(f.ColumnName)
                AND e.ColumnName<>f.ColumnName);
            */
            sql = "	UPDATE VAF_Job_Para f " +
                    " SET ColumnName = (SELECT e.ColumnName FROM VAF_ColumnDic e " +
                    " WHERE UPPER(e.ColumnName)=UPPER(f.ColumnName)) " +
                    " WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y' " +
                    " AND EXISTS (SELECT * FROM VAF_ColumnDic e " +
                    "  WHERE UPPER(e.ColumnName)=UPPER(f.ColumnName) " +
                    "  AND e.ColumnName<>f.ColumnName) ";
            Execute("Synchronize Parameter ColumnName", sql, "  rows updated: ");

            /*


            --	Paramenter Fields
            UPDATE	VAF_Job_Para p
              SET	IsCentrallyMaintained = 'N'
            WHERE	IsCentrallyMaintained <> 'N'
              AND NOT EXISTS (SELECT * FROM VAF_ColumnDic e WHERE p.ColumnName=e.ColumnName); 
            */
            sql = "UPDATE VAF_Job_Para p " +
                    " SET IsCentrallyMaintained = 'N' " +
                    " WHERE IsCentrallyMaintained <> 'N' " +
                    " AND NOT EXISTS (SELECT * FROM VAF_ColumnDic e WHERE p.ColumnName=e.ColumnName) ";
            Execute("Synchronize Paramenter Fields", sql, "  rows updated: ");

            /*
            --	Parameter Fields
            DBMS_OUTPUT.PUT_LINE('Synchronize Process Parameter');
            UPDATE VAF_Job_Para f
                SET Name = (SELECT e.Name FROM VAF_ColumnDic e
                            WHERE e.ColumnName=f.ColumnName),
                    Description = (SELECT e.Description FROM VAF_ColumnDic e
                            WHERE e.ColumnName=f.ColumnName),
                    Help = (SELECT e.Help FROM VAF_ColumnDic e
                            WHERE e.ColumnName=f.ColumnName),
                    Updated = SysDate
            WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y'
              AND EXISTS (SELECT * FROM VAF_ColumnDic e
                        WHERE e.ColumnName=f.ColumnName
                          AND (f.Name <> e.Name OR NVL(f.Description,' ') <> NVL(e.Description,' ') OR NVL(f.Help,' ') <> NVL(e.Help,' ')));
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            //TODO process, element, ctx relationships???
            sql = "	UPDATE VAF_Job_Para f " +
                    " SET Name = (SELECT e.Name FROM VAF_ColumnDic e " +
                    " WHERE e.ColumnName=f.ColumnName)," +
                    " Description = (SELECT e.Description FROM VAF_ColumnDic e " +
                    " WHERE e.ColumnName=f.ColumnName), " +
                    " Help = (SELECT e.Help FROM VAF_ColumnDic e " +
                    " WHERE e.ColumnName=f.ColumnName)," +
                    " Updated = CURRENT_TIMESTAMP" +
                    " WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y'" +
                    "   AND EXISTS (SELECT * FROM VAF_ColumnDic e " +
                    " 	  WHERE e.ColumnName=f.ColumnName " +
                    "     AND (f.Name <> e.Name OR NVL(f.Description,' ') <> NVL(e.Description,' ') OR NVL(f.Help,' ') <> NVL(e.Help,' '))) " +
                    " AND NOT EXISTS (" +
                    "   SELECT * FROM VAF_Job p, VAF_ColumnDic e, VAF_ColumnDicContext ec " +
                    "   WHERE p.VAF_Job_ID=f.VAF_Job_ID " +
                    "   AND e.ColumnName=f.ColumnName " +
                    "   AND ec.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID " +
                    "   AND ec.VAF_ContextScope_ID=p.VAF_ContextScope_ID) ";
            Execute("Synchronize Process Parameter with Element", sql, "  rows updated: ");

            sql = "	UPDATE VAF_Job_Para f " +
                    " SET Name = (SELECT e.Name FROM VAF_ColumnDicContext e " +
                    "				JOIN VAF_ColumnDic el ON (e.VAF_ColumnDic_ID=el.VAF_ColumnDic_ID) " +
                    "				JOIN VAF_Job p ON (p.VAF_ContextScope_ID=e.VAF_ContextScope_ID) " +
                    "   			WHERE el.ColumnName=f.ColumnName AND " +
                    "   	 			p.VAF_Job_ID=f.VAF_Job_ID)," +
                    " Description = (SELECT e.Description FROM VAF_ColumnDicContext e " +
                    "				JOIN VAF_ColumnDic el ON (e.VAF_ColumnDic_ID=el.VAF_ColumnDic_ID) " +
                    "				JOIN VAF_Job p ON (p.VAF_ContextScope_ID=e.VAF_ContextScope_ID) " +
                    "   			WHERE el.ColumnName=f.ColumnName AND " +
                    "   	 			p.VAF_Job_ID=f.VAF_Job_ID)," +
                    " Help = (SELECT e.Help FROM VAF_ColumnDicContext e " +
                    "				JOIN VAF_ColumnDic el ON (e.VAF_ColumnDic_ID=el.VAF_ColumnDic_ID) " +
                    "				JOIN VAF_Job p ON (p.VAF_ContextScope_ID=e.VAF_ContextScope_ID) " +
                    "   			WHERE el.ColumnName=f.ColumnName AND " +
                    "   	 			p.VAF_Job_ID=f.VAF_Job_ID)," +
                    " Updated = CURRENT_TIMESTAMP" +
                    " WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y'" +
                    "   AND EXISTS (SELECT * FROM VAF_Job p, VAF_ColumnDic e, VAF_ColumnDicContext ec " +
                    " 	  WHERE e.ColumnName=f.ColumnName " +
                    "     AND (f.Name <> e.Name OR NVL(f.Description,' ') <> NVL(e.Description,' ') OR NVL(f.Help,' ') <> NVL(e.Help,' ')) " +
                    "     AND p.VAF_Job_ID=f.VAF_Job_ID " +
                    "     AND ec.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID " +
                    "     AND ec.VAF_ContextScope_ID=p.VAF_ContextScope_ID) ";

            Execute("Synchronize Process Parameter with ElementCTX", sql, "  rows updated: ");

            /*

            --	Parameter Translations
            DBMS_OUTPUT.PUT_LINE('Synchronize Process Parameter Trl');
            UPDATE VAF_Job_Para_TL trl
                SET Name = (SELECT et.Name FROM VAF_ColumnDic_TL et, VAF_ColumnDic e, VAF_Job_Para f
                            WHERE et.VAF_Language=trl.VAF_Language AND et.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID
                              AND e.ColumnName=f.ColumnName AND f.VAF_Job_Para_ID=trl.VAF_Job_Para_ID),
                    Description = (SELECT et.Description FROM VAF_ColumnDic_TL et, VAF_ColumnDic e, VAF_Job_Para f
                            WHERE et.VAF_Language=trl.VAF_Language AND et.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID
                              AND e.ColumnName=f.ColumnName AND f.VAF_Job_Para_ID=trl.VAF_Job_Para_ID),
                    Help = (SELECT et.Help FROM VAF_ColumnDic_TL et, VAF_ColumnDic e, VAF_Job_Para f
                            WHERE et.VAF_Language=trl.VAF_Language AND et.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID
                              AND e.ColumnName=f.ColumnName AND f.VAF_Job_Para_ID=trl.VAF_Job_Para_ID),
                    IsTranslated = (SELECT et.IsTranslated FROM VAF_ColumnDic_TL et, VAF_ColumnDic e, VAF_Job_Para f
                            WHERE et.VAF_Language=trl.VAF_Language AND et.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID
                              AND e.ColumnName=f.ColumnName AND f.VAF_Job_Para_ID=trl.VAF_Job_Para_ID),
                    Updated = SysDate
            WHERE EXISTS (SELECT * FROM VAF_ColumnDic_TL et, VAF_ColumnDic e, VAF_Job_Para f
                            WHERE et.VAF_Language=trl.VAF_Language AND et.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID
                              AND e.ColumnName=f.ColumnName AND f.VAF_Job_Para_ID=trl.VAF_Job_Para_ID
                              AND f.IsCentrallyMaintained='Y' AND f.IsActive='Y'
                              AND (trl.Name <> et.Name OR NVL(trl.Description,' ') <> NVL(et.Description,' ') OR NVL(trl.Help,' ') <> NVL(et.Help,' ')));
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE VAF_Job_Para_TL trl " +
                    " SET Name = (SELECT et.Name FROM VAF_ColumnDic_TL et INNER JOIN  VAF_ColumnDic e ON (et.VAF_ColumnDic_ID = e.VAF_ColumnDic_ID) INNER JOIN VAF_Job_Para f ON (f.VAF_ColumnDic_ID = e.VAF_ColumnDic_ID) " +
                    " 	  WHERE et.VAF_Language=trl.VAF_Language AND et.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID " +
                    "     AND e.ColumnName=f.ColumnName AND f.VAF_Job_Para_ID=trl.VAF_Job_Para_ID), " +
                    "   Description = (SELECT et.Description FROM  VAF_ColumnDic_TL et INNER JOIN  VAF_ColumnDic e ON (et.VAF_ColumnDic_ID = e.VAF_ColumnDic_ID) INNER JOIN VAF_Job_Para f ON (f.VAF_ColumnDic_ID = e.VAF_ColumnDic_ID) " +
                    " 	  WHERE et.VAF_Language=trl.VAF_Language AND et.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID " +
                    "     AND e.ColumnName=f.ColumnName AND f.VAF_Job_Para_ID=trl.VAF_Job_Para_ID), " +
                    " 	Help = (SELECT et.Help FROM  VAF_ColumnDic_TL et INNER JOIN  VAF_ColumnDic e ON (et.VAF_ColumnDic_ID = e.VAF_ColumnDic_ID) INNER JOIN VAF_Job_Para f ON (f.VAF_ColumnDic_ID = e.VAF_ColumnDic_ID) " +
                    " 	  WHERE et.VAF_Language=trl.VAF_Language AND et.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID " +
                    " 	  AND e.ColumnName=f.ColumnName AND f.VAF_Job_Para_ID=trl.VAF_Job_Para_ID), " +
                    " 	IsTranslated = (SELECT et.IsTranslated FROM  VAF_ColumnDic_TL et INNER JOIN  VAF_ColumnDic e ON (et.VAF_ColumnDic_ID = e.VAF_ColumnDic_ID) INNER JOIN VAF_Job_Para f ON (f.VAF_ColumnDic_ID = e.VAF_ColumnDic_ID) " +
                    " 	  WHERE et.VAF_Language=trl.VAF_Language AND et.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID " +
                    "     AND e.ColumnName=f.ColumnName AND f.VAF_Job_Para_ID=trl.VAF_Job_Para_ID), " +
                    " 	Updated = CURRENT_TIMESTAMP " +
                    " 	WHERE EXISTS (SELECT * FROM  VAF_ColumnDic_TL et INNER JOIN  VAF_ColumnDic e ON (et.VAF_ColumnDic_ID = e.VAF_ColumnDic_ID) INNER JOIN VAF_Job_Para f ON (f.VAF_ColumnDic_ID = e.VAF_ColumnDic_ID) " +
                    " 	      WHERE et.VAF_Language=trl.VAF_Language AND et.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID " +
                    " 		  AND e.ColumnName=f.ColumnName AND f.VAF_Job_Para_ID=trl.VAF_Job_Para_ID " +
                    " 		  AND f.IsCentrallyMaintained='Y' AND f.IsActive='Y' " +
                    " 		  AND (trl.Name <> et.Name OR NVL(trl.Description,' ') <> NVL(et.Description,' ') OR NVL(trl.Help,' ') <> NVL(et.Help,' '))) " +
                    "  	AND NOT EXISTS (" +
                    "     SELECT * " +
                    "     FROM VAF_Job_Para f, VAF_Job p, VAF_ColumnDic e, VAF_ColumnDicContext ec " +
                    "     WHERE f.VAF_Job_Para_ID=trl.VAF_Job_Para_ID " +
                    "     AND p.VAF_Job_ID=f.VAF_Job_ID " +
                    "     AND e.ColumnName=f.ColumnName " +
                    "     AND ec.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID " +
                    "     AND ec.VAF_ContextScope_ID=p.VAF_ContextScope_ID) ";
            Execute("Synchronize Process Parameter Trl with Element Trl", sql, "  rows updated: ");


            sql = "	UPDATE VAF_Job_Para_TL trl " +
                    " SET Name = (SELECT et.Name FROM VAF_ColumnDicContext_TL et, VAF_ColumnDicContext ec, VAF_ColumnDic e, VAF_Job_Para f, VAF_Job p " +
                            " WHERE et.VAF_Language=trl.VAF_Language AND ec.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID AND et.VAF_ColumnDicContext_ID=ec.VAF_ColumnDicContext_ID " +
                            " AND e.ColumnName=f.ColumnName AND f.VAF_Job_Para_ID=trl.VAF_Job_Para_ID " +
                            "AND p.VAF_ContextScope_ID=ec.VAF_ContextScope_ID AND p.VAF_Job_ID=f.VAF_Job_ID), " +
                    "   Description = (SELECT et.Description FROM VAF_ColumnDicContext_TL et, VAF_ColumnDicContext ec, VAF_ColumnDic e, VAF_Job_Para f, VAF_Job p " +
                            " WHERE et.VAF_Language=trl.VAF_Language AND ec.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID AND et.VAF_ColumnDicContext_ID=ec.VAF_ColumnDicContext_ID " +
                            " AND e.ColumnName=f.ColumnName AND f.VAF_Job_Para_ID=trl.VAF_Job_Para_ID " +
                            "AND p.VAF_ContextScope_ID=ec.VAF_ContextScope_ID AND p.VAF_Job_ID=f.VAF_Job_ID), " +
                    " 	Help = (SELECT et.Help FROM VAF_ColumnDicContext_TL et, VAF_ColumnDicContext ec, VAF_ColumnDic e, VAF_Job_Para f, VAF_Job p " +
                            " WHERE et.VAF_Language=trl.VAF_Language AND ec.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID AND et.VAF_ColumnDicContext_ID=ec.VAF_ColumnDicContext_ID " +
                            " AND e.ColumnName=f.ColumnName AND f.VAF_Job_Para_ID=trl.VAF_Job_Para_ID " +
                            "AND p.VAF_ContextScope_ID=ec.VAF_ContextScope_ID AND p.VAF_Job_ID=f.VAF_Job_ID), " +
                    " 	IsTranslated = (SELECT et.IsTranslated FROM VAF_ColumnDicContext_TL et, VAF_ColumnDicContext ec, VAF_ColumnDic e, VAF_Job_Para f, VAF_Job p " +
                            " WHERE et.VAF_Language=trl.VAF_Language AND ec.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID AND et.VAF_ColumnDicContext_ID=ec.VAF_ColumnDicContext_ID " +
                            " AND e.ColumnName=f.ColumnName AND f.VAF_Job_Para_ID=trl.VAF_Job_Para_ID " +
                            "AND p.VAF_ContextScope_ID=ec.VAF_ContextScope_ID AND p.VAF_Job_ID=f.VAF_Job_ID), " +
                    " 	Updated = CURRENT_TIMESTAMP " +
                    " 	WHERE EXISTS (SELECT * FROM VAF_ColumnDicContext_TL et, VAF_ColumnDicContext ec, VAF_ColumnDic e, VAF_Job_Para f, VAF_Job p " +
                    " 	      WHERE et.VAF_Language=trl.VAF_Language AND ec.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID " +
                    "         AND et.VAF_ColumnDicContext_ID=ec.VAF_ColumnDicContext_ID " +
                    " 		  AND e.ColumnName=f.ColumnName AND f.VAF_Job_Para_ID=trl.VAF_Job_Para_ID " +
                    " 		  AND p.VAF_Job_ID=f.VAF_Job_ID AND trl.VAF_Job_Para_ID=f.VAF_Job_Para_ID " +
                    " 		  AND p.VAF_ContextScope_ID IS NOT NULL AND p.VAF_ContextScope_ID=ec.VAF_ContextScope_ID " +
                    " 		  AND f.IsCentrallyMaintained='Y' AND f.IsActive='Y' " +
                    " 		  AND (trl.Name <> et.Name OR NVL(trl.Description,' ') <> NVL(et.Description,' ') OR NVL(trl.Help,' ') <> NVL(et.Help,' '))) ";
            Execute("Synchronize Process Parameter Trl with ElementCTX Trl", sql, "  rows updated: ");

            /*


            --	Workflow Node - Window
            DBMS_OUTPUT.PUT_LINE('Synchronize Workflow Node from Window');
            UPDATE AD_WF_Node n
                SET Name = (SELECT w.Name FROM AD_Window w
                            WHERE w.AD_Window_ID=n.AD_Window_ID),
                    Description = (SELECT w.Description FROM AD_Window w
                            WHERE w.AD_Window_ID=n.AD_Window_ID),
                    Help = (SELECT w.Help FROM AD_Window w
                            WHERE w.AD_Window_ID=n.AD_Window_ID)
            WHERE n.IsCentrallyMaintained = 'Y'
              AND EXISTS  (SELECT * FROM AD_Window w
                        WHERE w.AD_Window_ID=n.AD_Window_ID
                          AND (w.Name <> n.Name OR NVL(w.Description,' ') <> NVL(n.Description,' ') OR NVL(w.Help,' ') <> NVL(n.Help,' ')));
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_WF_Node n " +
                    " SET Name = (SELECT coalesce(w.DisplayName,w.Name) FROM AD_Window w " +
                    " 	WHERE w.AD_Window_ID=n.AD_Window_ID), " +
                    " Description = (SELECT w.Description FROM AD_Window w " +
                    " 	WHERE w.AD_Window_ID=n.AD_Window_ID), " +
                    " Help = (SELECT w.Help FROM AD_Window w " +
                    " 	WHERE w.AD_Window_ID=n.AD_Window_ID) " +
                    " WHERE n.IsCentrallyMaintained = 'Y' " +
                    " 	  AND EXISTS  (SELECT * FROM AD_Window w " +
                    " 		WHERE w.AD_Window_ID=n.AD_Window_ID " +
                    " 		  AND (w.Name <> n.Name OR NVL(w.Description,' ') <> NVL(n.Description,' ') OR NVL(w.Help,' ') <> NVL(n.Help,' '))) ";
            Execute("Synchronize Workflow Node from Window", sql, "  rows updated: ");
            /*

            --	Workflow Translations - Window
            DBMS_OUTPUT.PUT_LINE('Synchronize Workflow Node Trl from Window Trl');
            UPDATE AD_WF_Node_Trl trl
                SET Name = (SELECT t.Name FROM AD_Window_trl t, AD_WF_Node n
                            WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Window_ID=t.AD_Window_ID
                              AND trl.VAF_Language=t.VAF_Language),
                    Description = (SELECT t.Description FROM AD_Window_trl t, AD_WF_Node n
                            WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Window_ID=t.AD_Window_ID
                              AND trl.VAF_Language=t.VAF_Language),
                    Help = (SELECT t.Help FROM AD_Window_trl t, AD_WF_Node n
                            WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Window_ID=t.AD_Window_ID
                              AND trl.VAF_Language=t.VAF_Language)
            WHERE EXISTS (SELECT * FROM AD_Window_Trl t, AD_WF_Node n
                        WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Window_ID=t.AD_Window_ID
                          AND trl.VAF_Language=t.VAF_Language AND n.IsCentrallyMaintained='Y' AND n.IsActive='Y'
                          AND (trl.Name <> t.Name OR NVL(trl.Description,' ') <> NVL(t.Description,' ') OR NVL(trl.Help,' ') <> NVL(t.Help,' ')));
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_WF_Node_Trl trl " +
                    " SET Name = (SELECT t.Name FROM AD_Window_trl t, AD_WF_Node n " +
                    " 	WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Window_ID=t.AD_Window_ID " +
                    " 		  AND trl.VAF_Language=t.VAF_Language), " +
                    " Description = (SELECT t.Description FROM AD_Window_trl t, AD_WF_Node n " +
                    " 	WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Window_ID=t.AD_Window_ID " +
                    " 		  AND trl.VAF_Language=t.VAF_Language), " +
                    " Help = (SELECT t.Help FROM AD_Window_trl t, AD_WF_Node n " +
                    " 	WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Window_ID=t.AD_Window_ID " +
                    " 		  AND trl.VAF_Language=t.VAF_Language) " +
                    " WHERE EXISTS (SELECT * FROM AD_Window_Trl t, AD_WF_Node n " +
                    " 	WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Window_ID=t.AD_Window_ID " +
                    " 		  AND trl.VAF_Language=t.VAF_Language AND n.IsCentrallyMaintained='Y' AND n.IsActive='Y' " +
                    " 		  AND (trl.Name <> t.Name OR NVL(trl.Description,' ') <> NVL(t.Description,' ') OR NVL(trl.Help,' ') <> NVL(t.Help,' '))) ";
            Execute("Synchronize Workflow Node Trl from Window Trl", sql, "  rows updated: ");

            /*

            --	Workflow Node - Form
            DBMS_OUTPUT.PUT_LINE('Synchronize Workflow Node from Form');
            UPDATE AD_WF_Node n
                SET (Name, Description, Help) = (SELECT f.Name, f.Description, f.Help 
                        FROM VAF_Page f
                        WHERE f.VAF_Page_ID=n.VAF_Page_ID)
            WHERE n.IsCentrallyMaintained = 'Y'
              AND EXISTS  (SELECT * FROM VAF_Page f
                        WHERE f.VAF_Page_ID=n.VAF_Page_ID
                          AND (f.Name <> n.Name OR NVL(f.Description,' ') <> NVL(n.Description,' ') OR NVL(f.Help,' ') <> NVL(n.Help,' ')));
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_WF_Node n" +
                    " SET Name = (SELECT coalesce(f.DisplayName,f.Name) " +
                    "   FROM VAF_Page f " +
                    " 	WHERE f.VAF_Page_ID=n.VAF_Page_ID) " +
                    " , Description = (SELECT  f.Description " +
                    "   FROM VAF_Page f " +
                    " 	WHERE f.VAF_Page_ID=n.VAF_Page_ID) " +
                    " ,  Help = (SELECT f.Help " +
                    "   FROM VAF_Page f" +
                    " 	WHERE f.VAF_Page_ID=n.VAF_Page_ID) " +
                    " WHERE n.IsCentrallyMaintained = 'Y' " +
                    "   AND EXISTS  (SELECT * FROM VAF_Page f " +
                    " 			WHERE f.VAF_Page_ID=n.VAF_Page_ID " +
                    " 			  AND (f.Name <> n.Name OR NVL(f.Description,' ') <> NVL(n.Description,' ') OR NVL(f.Help,' ') <> NVL(n.Help,' '))) ";
            Execute("Synchronize Workflow Node from Form", sql, "  rows updated: ");

            /*

            --	Workflow Translations - Form
            DBMS_OUTPUT.PUT_LINE('Synchronize Workflow Node Trl from Form Trl');
            UPDATE AD_WF_Node_Trl trl
                SET (Name, Description, Help) = (SELECT t.Name, t.Description, t.Help
                    FROM VAF_Page_TL t, AD_WF_Node n
                    WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.VAF_Page_ID=t.VAF_Page_ID
                      AND trl.VAF_Language=t.VAF_Language)
            WHERE EXISTS (SELECT * FROM VAF_Page_TL t, AD_WF_Node n
                        WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.VAF_Page_ID=t.VAF_Page_ID
                          AND trl.VAF_Language=t.VAF_Language AND n.IsCentrallyMaintained='Y' AND n.IsActive='Y'
                          AND (trl.Name <> t.Name OR NVL(trl.Description,' ') <> NVL(t.Description,' ') OR NVL(trl.Help,' ') <> NVL(t.Help,' ')));
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_WF_Node_Trl trl " +
                    " SET Name = (SELECT t.Name " +
                    "   FROM VAF_Page_TL t, AD_WF_Node n " +
                    " 	WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.VAF_Page_ID=t.VAF_Page_ID " +
                    " 	  AND trl.VAF_Language=t.VAF_Language) " +
                     " ,  Description = (SELECT  t.Description " +
                    "   FROM VAF_Page_TL t, AD_WF_Node n " +
                    " 	WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.VAF_Page_ID=t.VAF_Page_ID " +
                    " 	  AND trl.VAF_Language=t.VAF_Language) " +
                     " ,  Help = (SELECT  t.Help " +
                    "   FROM VAF_Page_TL t, AD_WF_Node n " +
                    " 	WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.VAF_Page_ID=t.VAF_Page_ID " +
                    " 	  AND trl.VAF_Language=t.VAF_Language) " +
                    " WHERE EXISTS (SELECT * FROM VAF_Page_TL t, AD_WF_Node n " +
                    " 	WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.VAF_Page_ID=t.VAF_Page_ID " +
                    " 	  AND trl.VAF_Language=t.VAF_Language AND n.IsCentrallyMaintained='Y' AND n.IsActive='Y' " +
                    " 	  AND (trl.Name <> t.Name OR NVL(trl.Description,' ') <> NVL(t.Description,' ') OR NVL(trl.Help,' ') <> NVL(t.Help,' '))) ";
            Execute("Synchronize Workflow Node Trl from Form Trl", sql, "  rows updated: ");

            /*

            --	Workflow Node - Report
            DBMS_OUTPUT.PUT_LINE('Synchronize Workflow Node from Process');
            UPDATE AD_WF_Node n
                SET (Name, Description, Help) = (SELECT f.Name, f.Description, f.Help 
                        FROM VAF_Job f
                        WHERE f.VAF_Job_ID=n.VAF_Job_ID)
            WHERE n.IsCentrallyMaintained = 'Y'
              AND EXISTS  (SELECT * FROM VAF_Job f
                        WHERE f.VAF_Job_ID=n.VAF_Job_ID
                          AND (f.Name <> n.Name OR NVL(f.Description,' ') <> NVL(n.Description,' ') OR NVL(f.Help,' ') <> NVL(n.Help,' ')));
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_WF_Node n " +
                    " SET Name = (SELECT f.Name " +
                    " 	FROM VAF_Job f " +
                    " 	WHERE f.VAF_Job_ID=n.VAF_Job_ID) " +
                    " ,  Description = (SELECT f.Description " +
                    " 	FROM VAF_Job f " +
                    " 	WHERE f.VAF_Job_ID=n.VAF_Job_ID) " +
                    " ,  Help = (SELECT  f.Help " +
                    " 	FROM VAF_Job f " +
                    " 	WHERE f.VAF_Job_ID=n.VAF_Job_ID) " +
                    " WHERE n.IsCentrallyMaintained = 'Y' " +
                    "   AND EXISTS  (SELECT * FROM VAF_Job f " +
                    " 		WHERE f.VAF_Job_ID=n.VAF_Job_ID " +
                    " 		  AND (f.Name <> n.Name OR NVL(f.Description,' ') <> NVL(n.Description,' ') OR NVL(f.Help,' ') <> NVL(n.Help,' '))) ";
            Execute("Synchronize Workflow Node from Process", sql, "  rows updated: ");

            /*

            --	Workflow Translations - Form
            DBMS_OUTPUT.PUT_LINE('Synchronize Workflow Node Trl from Process Trl');
            UPDATE AD_WF_Node_Trl trl
                SET (Name, Description, Help) = (SELECT t.Name, t.Description, t.Help
                    FROM VAF_Job_TL t, AD_WF_Node n
                    WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.VAF_Job_ID=t.VAF_Job_ID
                      AND trl.VAF_Language=t.VAF_Language)
            WHERE EXISTS (SELECT * FROM VAF_Job_TL t, AD_WF_Node n
                        WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.VAF_Job_ID=t.VAF_Job_ID
                          AND trl.VAF_Language=t.VAF_Language AND n.IsCentrallyMaintained='Y' AND n.IsActive='Y'
                          AND (trl.Name <> t.Name OR NVL(trl.Description,' ') <> NVL(t.Description,' ') OR NVL(trl.Help,' ') <> NVL(t.Help,' ')));
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_WF_Node_Trl trl " +
                    " SET Name = (SELECT t.Name " +
                    " 		FROM VAF_Job_TL t, AD_WF_Node n " +
                    " 		WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.VAF_Job_ID=t.VAF_Job_ID " +
                    " 		  AND trl.VAF_Language=t.VAF_Language) " +
                    " ,  Description = (SELECT  t.Description " +
                    " 		FROM VAF_Job_TL t, AD_WF_Node n " +
                    " 		WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.VAF_Job_ID=t.VAF_Job_ID " +
                    " 		  AND trl.VAF_Language=t.VAF_Language) " +
                    " ,  Help= (SELECT  t.Help " +
                    " 		FROM VAF_Job_TL t, AD_WF_Node n " +
                    " 		WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.VAF_Job_ID=t.VAF_Job_ID " +
                    " 		  AND trl.VAF_Language=t.VAF_Language) " +
                    " WHERE EXISTS (SELECT * FROM VAF_Job_TL t, AD_WF_Node n " +
                    " 		WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.VAF_Job_ID=t.VAF_Job_ID " +
                    " 		  AND trl.VAF_Language=t.VAF_Language AND n.IsCentrallyMaintained='Y' AND n.IsActive='Y' " +
                    " 		  AND (trl.Name <> t.Name OR NVL(trl.Description,' ') <> NVL(t.Description,' ') OR NVL(trl.Help,' ') <> NVL(t.Help,' '))) ";
            Execute("Synchronize Workflow Node Trl from Process Trl", sql, "  rows updated: ");

            /*

            --  Need centrally maintained flag here!
            DBMS_OUTPUT.PUT_LINE('Synchronize PrintFormatItem Name from Element');
            UPDATE VAF_Print_Rpt_LItem pfi
              SET Name = (SELECT e.Name 
                FROM VAF_ColumnDic e, VAF_Column c
                WHERE e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID
                  AND c.VAF_Column_ID=pfi.VAF_Column_ID)
            WHERE pfi.IsCentrallyMaintained='Y'
              AND EXISTS (SELECT * 
                FROM VAF_ColumnDic e, VAF_Column c
                WHERE e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID
                  AND c.VAF_Column_ID=pfi.VAF_Column_ID
                  AND e.Name<>pfi.Name)
              AND EXISTS (SELECT * FROM VAF_Client 
                WHERE VAF_Client_ID=pfi.VAF_Client_ID AND IsMultiLingualDocument='Y');
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE VAF_Print_Rpt_LItem pfi " +
                    "	  SET Name = (SELECT e.Name " +
                    "		FROM VAF_ColumnDic e, VAF_Column c " +
                    "		WHERE e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID " +
                    "		  AND c.VAF_Column_ID=pfi.VAF_Column_ID) " +
                    "	WHERE pfi.IsCentrallyMaintained='Y' " +
                    "      AND EXISTS (SELECT * " +
                    "		FROM VAF_ColumnDic e, VAF_Column c " +
                    "		WHERE e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID " +
                    "		  AND c.VAF_Column_ID=pfi.VAF_Column_ID " +
                    "		  AND e.Name<>pfi.Name) " +
                    "	  AND EXISTS (SELECT * FROM VAF_Client " +
                    "		WHERE VAF_Client_ID=pfi.VAF_Client_ID AND IsMultiLingualDocument='Y') ";
            Execute("Synchronize PrintFormatItem Name from Element", sql, "  rows updated: ");

            /*

            DBMS_OUTPUT.PUT_LINE('Synchronize PrintFormatItem PrintName from Element');
            UPDATE VAF_Print_Rpt_LItem pfi
              SET PrintName = (SELECT e.PrintName 
                FROM VAF_ColumnDic e, VAF_Column c
                WHERE e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID
                  AND c.VAF_Column_ID=pfi.VAF_Column_ID)
            WHERE pfi.IsCentrallyMaintained='Y'
              AND EXISTS (SELECT * 
                FROM VAF_ColumnDic e, VAF_Column c, VAF_Print_Rpt_Layout pf
                WHERE e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID
                  AND c.VAF_Column_ID=pfi.VAF_Column_ID
                  AND LENGTH(pfi.PrintName) > 0
                  AND e.PrintName<>pfi.PrintName
                  AND pf.VAF_Print_Rpt_Layout_ID=pfi.VAF_Print_Rpt_Layout_ID
                  AND pf.IsForm='N' AND IsTableBased='Y')
              AND EXISTS (SELECT * FROM VAF_Client 
                WHERE VAF_Client_ID=pfi.VAF_Client_ID AND IsMultiLingualDocument='Y');
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE VAF_Print_Rpt_LItem pfi " +
                    "	  SET PrintName = (SELECT e.PrintName " +
                    "		FROM VAF_ColumnDic e, VAF_Column c " +
                    "		WHERE e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID " +
                    "		  AND c.VAF_Column_ID=pfi.VAF_Column_ID) " +
                    "	WHERE pfi.IsCentrallyMaintained='Y' " +
                    "      AND EXISTS (SELECT * " +
                    "		FROM VAF_ColumnDic e, VAF_Column c, VAF_Print_Rpt_Layout pf " +
                    "		WHERE e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID " +
                    "		  AND c.VAF_Column_ID=pfi.VAF_Column_ID " +
                    "		  AND LENGTH(pfi.PrintName) > 0 " +
                    "		  AND e.PrintName<>pfi.PrintName " +
                    "		  AND pf.VAF_Print_Rpt_Layout_ID=pfi.VAF_Print_Rpt_Layout_ID " +
                    "		  AND pf.IsForm='N' AND IsTableBased='Y') " +
                    "	  AND EXISTS (SELECT * FROM VAF_Client " +
                    "		WHERE VAF_Client_ID=pfi.VAF_Client_ID AND IsMultiLingualDocument='Y') ";
            Execute("Synchronize PrintFormatItem PrintName from Element", sql, "  rows updated: ");

            /*

            DBMS_OUTPUT.PUT_LINE('Synchronize PrintFormatItem Trl from Element Trl (Multi-Lingual)');
            UPDATE VAF_Print_Rpt_LItem_TL trl
              SET PrintName = (SELECT e.PrintName 
                FROM VAF_ColumnDic_TL e, VAF_Column c, VAF_Print_Rpt_LItem pfi
                WHERE e.VAF_Language=trl.VAF_Language
                  AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID
                  AND c.VAF_Column_ID=pfi.VAF_Column_ID
                  AND pfi.VAF_Print_Rpt_LItem_ID=trl.VAF_Print_Rpt_LItem_ID)
            WHERE EXISTS (SELECT * 
                FROM VAF_ColumnDic_TL e, VAF_Column c, VAF_Print_Rpt_LItem pfi, VAF_Print_Rpt_Layout pf
                WHERE e.VAF_Language=trl.VAF_Language
                  AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID
                  AND c.VAF_Column_ID=pfi.VAF_Column_ID
                  AND pfi.VAF_Print_Rpt_LItem_ID=trl.VAF_Print_Rpt_LItem_ID
                  AND pfi.IsCentrallyMaintained='Y'
                  AND LENGTH(pfi.PrintName) > 0
                  AND (e.PrintName<>trl.PrintName OR trl.PrintName IS NULL)
                  AND pf.VAF_Print_Rpt_Layout_ID=pfi.VAF_Print_Rpt_Layout_ID 
                  AND pf.IsForm='N' AND IsTableBased='Y')
              AND EXISTS (SELECT * FROM VAF_Client 
                WHERE VAF_Client_ID=trl.VAF_Client_ID AND IsMultiLingualDocument='Y');
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE VAF_Print_Rpt_LItem_TL trl " +
                    "	  SET PrintName = (SELECT e.PrintName " +
                    "		FROM VAF_ColumnDic_TL e, VAF_Column c, VAF_Print_Rpt_LItem pfi " +
                    "		WHERE e.VAF_Language=trl.VAF_Language " +
                    "		  AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID " +
                    "		  AND c.VAF_Column_ID=pfi.VAF_Column_ID " +
                    "		  AND pfi.VAF_Print_Rpt_LItem_ID=trl.VAF_Print_Rpt_LItem_ID) " +
                    "	WHERE EXISTS (SELECT * " +
                    "		FROM VAF_ColumnDic_TL e, VAF_Column c, VAF_Print_Rpt_LItem pfi, VAF_Print_Rpt_Layout pf " +
                    "		WHERE e.VAF_Language=trl.VAF_Language " +
                    "		  AND e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID " +
                    "		  AND c.VAF_Column_ID=pfi.VAF_Column_ID " +
                    "		  AND pfi.VAF_Print_Rpt_LItem_ID=trl.VAF_Print_Rpt_LItem_ID " +
                    "          AND pfi.IsCentrallyMaintained='Y' " +
                    "		  AND LENGTH(pfi.PrintName) > 0 " +
                    "		  AND (e.PrintName<>trl.PrintName OR trl.PrintName IS NULL) " +
                    "		  AND pf.VAF_Print_Rpt_Layout_ID=pfi.VAF_Print_Rpt_Layout_ID " +
                    "		  AND pf.IsForm='N' AND IsTableBased='Y') " +
                    "	  AND EXISTS (SELECT * FROM VAF_Client " +
                    "		WHERE VAF_Client_ID=trl.VAF_Client_ID AND IsMultiLingualDocument='Y')";
            Execute("Synchronize PrintFormatItem Trl from Element Trl (Multi-Lingual)", sql, "  rows updated: ");

            /*

            DBMS_OUTPUT.PUT_LINE('Synchronize PrintFormatItem Trl (Not Multi-Lingual)');
            UPDATE VAF_Print_Rpt_LItem_TL trl
              SET PrintName = (SELECT pfi.PrintName 
                FROM VAF_Print_Rpt_LItem pfi
                WHERE pfi.VAF_Print_Rpt_LItem_ID=trl.VAF_Print_Rpt_LItem_ID)
            WHERE EXISTS (SELECT * 
                FROM VAF_Print_Rpt_LItem pfi, VAF_Print_Rpt_Layout pf
                WHERE pfi.VAF_Print_Rpt_LItem_ID=trl.VAF_Print_Rpt_LItem_ID
                  AND pfi.IsCentrallyMaintained='Y'
                  AND LENGTH(pfi.PrintName) > 0
                  AND pfi.PrintName<>trl.PrintName
                  AND pf.VAF_Print_Rpt_Layout_ID=pfi.VAF_Print_Rpt_Layout_ID 
                  AND pf.IsForm='N' AND pf.IsTableBased='Y')
              AND EXISTS (SELECT * FROM VAF_Client 
                WHERE VAF_Client_ID=trl.VAF_Client_ID AND IsMultiLingualDocument='N');
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE VAF_Print_Rpt_LItem_TL trl " +
                    "	  SET PrintName = (SELECT pfi.PrintName " +
                    "		FROM VAF_Print_Rpt_LItem pfi " +
                    "		WHERE pfi.VAF_Print_Rpt_LItem_ID=trl.VAF_Print_Rpt_LItem_ID) " +
                    "	WHERE EXISTS (SELECT * " +
                    "		FROM VAF_Print_Rpt_LItem pfi, VAF_Print_Rpt_Layout pf " +
                    "		WHERE pfi.VAF_Print_Rpt_LItem_ID=trl.VAF_Print_Rpt_LItem_ID " +
                    "          AND pfi.IsCentrallyMaintained='Y' " +
                    "		  AND LENGTH(pfi.PrintName) > 0 " +
                    "		  AND pfi.PrintName<>trl.PrintName " +
                    "		  AND pf.VAF_Print_Rpt_Layout_ID=pfi.VAF_Print_Rpt_Layout_ID " +
                    "		  AND pf.IsForm='N' AND pf.IsTableBased='Y') " +
                    "	  AND EXISTS (SELECT * FROM VAF_Client " +
                    "		WHERE VAF_Client_ID=trl.VAF_Client_ID AND IsMultiLingualDocument='N')";
            Execute("Synchronize PrintFormatItem Trl (Not Multi-Lingual)", sql, "  rows updated: ");

            /*

            DBMS_OUTPUT.PUT_LINE('Reset PrintFormatItem Trl where not used in base table');
            UPDATE VAF_Print_Rpt_LItem_TL trl
              SET PrintName = NULL
            WHERE PrintName IS NOT NULL
              AND EXISTS (SELECT *
                FROM VAF_Print_Rpt_LItem pfi
                WHERE pfi.VAF_Print_Rpt_LItem_ID=trl.VAF_Print_Rpt_LItem_ID 
                  AND pfi.IsCentrallyMaintained='Y'
                  AND (LENGTH (pfi.PrintName) = 0 OR pfi.PrintName IS NULL));
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE VAF_Print_Rpt_LItem_TL trl " +
                    "	  SET PrintName = NULL " +
                    "	WHERE PrintName IS NOT NULL " +
                    "	  AND EXISTS (SELECT * " +
                    "		FROM VAF_Print_Rpt_LItem pfi " +
                    "		WHERE pfi.VAF_Print_Rpt_LItem_ID=trl.VAF_Print_Rpt_LItem_ID " +
                    "          AND pfi.IsCentrallyMaintained='Y' " +
                    "		  AND (LENGTH (pfi.PrintName) = 0 OR pfi.PrintName IS NULL))";
            Execute("Synchronize PrintFormatItem Trl where not used in base table", sql, "  rows updated: ");

            /*

        **
        SELECT 	e.PrintName "Element", pfi.PrintName "FormatItem", trl.VAF_Language, trl.PrintName "Trl"
        FROM 	VAF_ColumnDic e
          INNER JOIN VAF_Column c ON (e.VAF_ColumnDic_ID=c.VAF_ColumnDic_ID)
          INNER JOIN VAF_Print_Rpt_LItem pfi ON (c.VAF_Column_ID=pfi.VAF_Column_ID)
          INNER JOIN VAF_Print_Rpt_LItem_TL trl ON (pfi.VAF_Print_Rpt_LItem_ID=trl.VAF_Print_Rpt_LItem_ID)
        WHERE pfi.VAF_Print_Rpt_LItem_ID=?
        **

            --	Sync Names - Window
            DBMS_OUTPUT.PUT_LINE('Synchronizing Menu with Window');
            UPDATE	VAF_MenuConfig m
            SET		Name = (SELECT Name FROM AD_Window w WHERE m.AD_Window_ID=w.AD_Window_ID),
                    Description = (SELECT Description FROM AD_Window w WHERE m.AD_Window_ID=w.AD_Window_ID)
            WHERE	AD_Window_ID IS NOT NULL
                AND Action = 'W';
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            executesql("Synchronize Field Translations", sql,  "  rows updated: ");
        **/
            sql = "UPDATE VAF_MenuConfig m "
                + "SET Name = (SELECT coalesce(w.DisplayName,w.Name) FROM AD_Window w WHERE m.AD_Window_ID=w.AD_Window_ID),"
                + "Description = (SELECT Description FROM AD_Window w WHERE m.AD_Window_ID=w.AD_Window_ID) "
                + "WHERE AD_Window_ID IS NOT NULL"
                + " AND Action = 'W'";
            Execute("Synchronize Menu with Window", sql, "  rows updated: ");

            /*
                UPDATE	VAF_MenuConfig_TL mt
                SET		Name = (SELECT wt.Name FROM AD_Window_Trl wt, VAF_MenuConfig m 
                                WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Window_ID=wt.AD_Window_ID 
                                AND mt.VAF_Language=wt.VAF_Language),
                        Description = (SELECT wt.Description FROM AD_Window_Trl wt, VAF_MenuConfig m 
                                WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Window_ID=wt.AD_Window_ID 
                                AND mt.VAF_Language=wt.VAF_Language),
                        IsTranslated = (SELECT wt.IsTranslated FROM AD_Window_Trl wt, VAF_MenuConfig m 
                                WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Window_ID=wt.AD_Window_ID 
                                AND mt.VAF_Language=wt.VAF_Language)
                WHERE EXISTS (SELECT * FROM AD_Window_Trl wt, VAF_MenuConfig m 
                                WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Window_ID=wt.AD_Window_ID 
                                AND mt.VAF_Language=wt.VAF_Language
                                AND m.AD_Window_ID IS NOT NULL
                                AND m.Action = 'W');
                DBMS_OUTPUT.PUT_LINE('  trl rows updated: ' || SQL%ROWCOUNT);
                */
            sql = "	UPDATE VAF_MenuConfig_TL mt " +
                    "	SET Name = (SELECT wt.Name FROM AD_Window_Trl wt, VAF_MenuConfig m " +
                    "					WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Window_ID=wt.AD_Window_ID " +
                    "					AND mt.VAF_Language=wt.VAF_Language), " +
                    "		Description = (SELECT wt.Description FROM AD_Window_Trl wt, VAF_MenuConfig m " +
                    "					WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Window_ID=wt.AD_Window_ID " +
                    "					AND mt.VAF_Language=wt.VAF_Language), " +
                    "			IsTranslated = (SELECT wt.IsTranslated FROM AD_Window_Trl wt, VAF_MenuConfig m " +
                    "					WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Window_ID=wt.AD_Window_ID " +
                    "					AND mt.VAF_Language=wt.VAF_Language) " +
                    "	WHERE EXISTS (SELECT * FROM AD_Window_Trl wt, VAF_MenuConfig m " +
                    "					WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Window_ID=wt.AD_Window_ID " +
                    "					AND mt.VAF_Language=wt.VAF_Language " +
                    "					AND m.AD_Window_ID IS NOT NULL " +
                    "					AND m.Action = 'W')";
            Execute("Synchronize Menu with Window Trl", sql, "  rows updated: ");

            /*

            --	Sync Names - Process
            DBMS_OUTPUT.PUT_LINE('Synchronizing Menu with Processes');
            UPDATE	VAF_MenuConfig m
            SET		Name = (SELECT p.Name FROM VAF_Job p WHERE m.VAF_Job_ID=p.VAF_Job_ID),
                    Description = (SELECT p.Description FROM VAF_Job p WHERE m.VAF_Job_ID=p.VAF_Job_ID)
            WHERE	m.VAF_Job_ID IS NOT NULL
              AND	m.Action IN ('R', 'P');
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */



            // Commented 
            //sql = "	UPDATE	VAF_MenuConfig m " +
            //        "	SET		Name = (SELECT p.value FROM VAF_Job p WHERE m.VAF_Job_ID=p.VAF_Job_ID), " +
            //        "			Description = (SELECT p.Description FROM VAF_Job p WHERE m.VAF_Job_ID=p.VAF_Job_ID) " +
            //        "	WHERE	m.VAF_Job_ID IS NOT NULL " +
            //        "	  AND	m.Action IN ('R', 'P')";
            //Execute("Synchronize Menu with Processes", sql, "  rows updated: ");

            // Commented 

            /*

            UPDATE	VAF_MenuConfig_TL mt
            SET		Name = (SELECT pt.Name FROM VAF_Job_TL pt, VAF_MenuConfig m
                            WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.VAF_Job_ID=pt.VAF_Job_ID
                            AND mt.VAF_Language=pt.VAF_Language),
                    Description = (SELECT pt.Description FROM VAF_Job_TL pt, VAF_MenuConfig m
                            WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.VAF_Job_ID=pt.VAF_Job_ID
                            AND mt.VAF_Language=pt.VAF_Language),
                    IsTranslated = (SELECT pt.IsTranslated FROM VAF_Job_TL pt, VAF_MenuConfig m
                            WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.VAF_Job_ID=pt.VAF_Job_ID
                            AND mt.VAF_Language=pt.VAF_Language)
            WHERE EXISTS (SELECT * FROM VAF_Job_TL pt, VAF_MenuConfig m
                            WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.VAF_Job_ID=pt.VAF_Job_ID
                            AND mt.VAF_Language=pt.VAF_Language
                            AND m.VAF_Job_ID IS NOT NULL
                            AND	Action IN ('R', 'P'));
            DBMS_OUTPUT.PUT_LINE('  trl rows updated: ' || SQL%ROWCOUNT);
            */


            //sql = "	UPDATE	VAF_MenuConfig_TL mt " +
            //        "	SET		Name = (SELECT pt.Name FROM VAF_Job_TL pt, VAF_MenuConfig m " +
            //        "					WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.VAF_Job_ID=pt.VAF_Job_ID " +
            //        "					AND mt.VAF_Language=pt.VAF_Language), " +
            //        "			Description = (SELECT pt.Description FROM VAF_Job_TL pt, VAF_MenuConfig m " +
            //        "					WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.VAF_Job_ID=pt.VAF_Job_ID " +
            //        "					AND mt.VAF_Language=pt.VAF_Language), " +
            //        "			IsTranslated = (SELECT pt.IsTranslated FROM VAF_Job_TL pt, VAF_MenuConfig m " +
            //        "					WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.VAF_Job_ID=pt.VAF_Job_ID" +
            //        "					AND mt.VAF_Language=pt.VAF_Language)" +
            //        "	WHERE EXISTS (SELECT * FROM VAF_Job_TL pt, VAF_MenuConfig m" +
            //        "					WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.VAF_Job_ID=pt.VAF_Job_ID" +
            //        "					AND mt.VAF_Language=pt.VAF_Language" +
            //        "					AND m.VAF_Job_ID IS NOT NULL" +
            //        "					AND	Action IN ('R', 'P'))";
            //Execute("Synchronize Menu with Processes Translations", sql, "  rows updated: ");

            /*

            --	Sync Names = Form
            DBMS_OUTPUT.PUT_LINE('Synchronizing Menu with Forms');
            UPDATE	VAF_MenuConfig m
            SET		Name = (SELECT Name FROM VAF_Page f WHERE m.VAF_Page_ID=f.VAF_Page_ID),
                    Description = (SELECT Description FROM VAF_Page f WHERE m.VAF_Page_ID=f.VAF_Page_ID)
            WHERE	VAF_Page_ID IS NOT NULL
              AND	Action = 'X';
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE VAF_MenuConfig m " +
                    "	SET Name = (SELECT coalesce(f.DisplayName,f.Name) FROM VAF_Page f WHERE m.VAF_Page_ID=f.VAF_Page_ID), " +
                    "			Description = (SELECT Description FROM VAF_Page f WHERE m.VAF_Page_ID=f.VAF_Page_ID) " +
                    "	WHERE VAF_Page_ID IS NOT NULL " +
                    "	  AND Action = 'X'";
            Execute("Synchronize Menu with Forms", sql, "  rows updated: ");

            /*

            UPDATE	VAF_MenuConfig_TL mt
            SET		Name = (SELECT ft.Name FROM VAF_Page_TL ft, VAF_MenuConfig m
                            WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.VAF_Page_ID=ft.VAF_Page_ID
                            AND mt.VAF_Language=ft.VAF_Language),
                    Description = (SELECT ft.Description FROM VAF_Page_TL ft, VAF_MenuConfig m
                            WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.VAF_Page_ID=ft.VAF_Page_ID
                            AND mt.VAF_Language=ft.VAF_Language),
                    IsTranslated = (SELECT ft.IsTranslated FROM VAF_Page_TL ft, VAF_MenuConfig m
                            WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.VAF_Page_ID=ft.VAF_Page_ID
                            AND mt.VAF_Language=ft.VAF_Language)
            WHERE EXISTS (SELECT * FROM VAF_Page_TL ft, VAF_MenuConfig m
                            WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.VAF_Page_ID=ft.VAF_Page_ID
                            AND mt.VAF_Language=ft.VAF_Language
                            AND m.VAF_Page_ID IS NOT NULL
                            AND	Action = 'X');
            DBMS_OUTPUT.PUT_LINE('  trl rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "UPDATE VAF_MenuConfig_TL mt " +
                    "	SET Name = (SELECT ft.Name FROM VAF_Page_TL ft, VAF_MenuConfig m " +
                    "					WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.VAF_Page_ID=ft.VAF_Page_ID " +
                    "					AND mt.VAF_Language=ft.VAF_Language), " +
                    "			Description = (SELECT ft.Description FROM VAF_Page_TL ft, VAF_MenuConfig m " +
                    "					WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.VAF_Page_ID=ft.VAF_Page_ID " +
                    "					AND mt.VAF_Language=ft.VAF_Language), " +
                    "			IsTranslated = (SELECT ft.IsTranslated FROM VAF_Page_TL ft, VAF_MenuConfig m " +
                    "					WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.VAF_Page_ID=ft.VAF_Page_ID " +
                    "					AND mt.VAF_Language=ft.VAF_Language) " +
                    "	WHERE EXISTS (SELECT * FROM VAF_Page_TL ft, VAF_MenuConfig m " +
                    "					WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.VAF_Page_ID=ft.VAF_Page_ID " +
                    "					AND mt.VAF_Language=ft.VAF_Language " +
                    "					AND m.VAF_Page_ID IS NOT NULL " +
                    "					AND	Action = 'X')";

            Execute("Synchronize Menu with Forms Trl", sql, "  rows updated: ");

            /*

            --	Sync Names - Workflow
            DBMS_OUTPUT.PUT_LINE('Synchronizing Menu with Workflows');
            UPDATE	VAF_MenuConfig m
            SET		Name = (SELECT p.Name FROM AD_Workflow p WHERE m.AD_Workflow_ID=p.AD_Workflow_ID),
                    Description = (SELECT p.Description FROM AD_Workflow p WHERE m.AD_Workflow_ID=p.AD_Workflow_ID)
            WHERE	m.AD_Workflow_ID IS NOT NULL
              AND	m.Action = 'F';
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE VAF_MenuConfig m " +
                    "	SET Name = (SELECT p.Name FROM AD_Workflow p WHERE m.AD_Workflow_ID=p.AD_Workflow_ID), " +
                    "			Description = (SELECT p.Description FROM AD_Workflow p WHERE m.AD_Workflow_ID=p.AD_Workflow_ID) " +
                    "	WHERE m.AD_Workflow_ID IS NOT NULL " +
                    "	  AND m.Action = 'F'";
            Execute("Synchronize Menu with Workflows", sql, "  rows updated: ");

            /*

            UPDATE	VAF_MenuConfig_TL mt
            SET		Name = (SELECT pt.Name FROM AD_Workflow_Trl pt, VAF_MenuConfig m
                            WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Workflow_ID=pt.AD_Workflow_ID
                            AND mt.VAF_Language=pt.VAF_Language),
                    Description = (SELECT pt.Description FROM AD_Workflow_Trl pt, VAF_MenuConfig m
                            WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Workflow_ID=pt.AD_Workflow_ID
                            AND mt.VAF_Language=pt.VAF_Language),
                    IsTranslated = (SELECT pt.IsTranslated FROM AD_Workflow_Trl pt, VAF_MenuConfig m
                            WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Workflow_ID=pt.AD_Workflow_ID
                            AND mt.VAF_Language=pt.VAF_Language)
            WHERE EXISTS (SELECT * FROM AD_Workflow_Trl pt, VAF_MenuConfig m
                            WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Workflow_ID=pt.AD_Workflow_ID
                            AND mt.VAF_Language=pt.VAF_Language
                            AND m.AD_Workflow_ID IS NOT NULL
                            AND	Action = 'F');
            DBMS_OUTPUT.PUT_LINE('  trl rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE VAF_MenuConfig_TL mt " +
                    "	SET Name = (SELECT pt.Name FROM AD_Workflow_Trl pt, VAF_MenuConfig m " +
                    "					WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Workflow_ID=pt.AD_Workflow_ID " +
                    "					AND mt.VAF_Language=pt.VAF_Language), " +
                    "			Description = (SELECT pt.Description FROM AD_Workflow_Trl pt, VAF_MenuConfig m " +
                    "					WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Workflow_ID=pt.AD_Workflow_ID " +
                    "					AND mt.VAF_Language=pt.VAF_Language), " +
                    "			IsTranslated = (SELECT pt.IsTranslated FROM AD_Workflow_Trl pt, VAF_MenuConfig m " +
                    "					WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Workflow_ID=pt.AD_Workflow_ID " +
                    "					AND mt.VAF_Language=pt.VAF_Language) " +
                    "	WHERE EXISTS (SELECT * FROM AD_Workflow_Trl pt, VAF_MenuConfig m " +
                    "					WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Workflow_ID=pt.AD_Workflow_ID " +
                    "					AND mt.VAF_Language=pt.VAF_Language " +
                    "					AND m.AD_Workflow_ID IS NOT NULL " +
                    "					AND  Action = 'F')";
            Execute("Synchronize Menu with Workflows Trl", sql, "  rows updated: ");

            /*

            --	Sync Names = Task
            DBMS_OUTPUT.PUT_LINE('Synchronizing Menu with Tasks');
            UPDATE	VAF_MenuConfig m
            SET		Name = (SELECT Name FROM AD_Task f WHERE m.AD_Task_ID=f.AD_Task_ID),
                    Description = (SELECT Description FROM AD_Task f WHERE m.AD_Task_ID=f.AD_Task_ID)
            WHERE	AD_Task_ID IS NOT NULL
              AND	Action = 'T';
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE VAF_MenuConfig m " +
                    "	SET Name = (SELECT Name FROM AD_Task f WHERE m.AD_Task_ID=f.AD_Task_ID), " +
                    "			Description = (SELECT Description FROM AD_Task f WHERE m.AD_Task_ID=f.AD_Task_ID) " +
                    "	WHERE AD_Task_ID IS NOT NULL " +
                    "	  AND Action = 'T'";
            Execute("Synchronize Menu with Tasks", sql, "  rows updated: ");

            /*

            UPDATE	VAF_MenuConfig_TL mt
            SET		Name = (SELECT ft.Name FROM AD_Task_Trl ft, VAF_MenuConfig m
                            WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Task_ID=ft.AD_Task_ID
                            AND mt.VAF_Language=ft.VAF_Language),
                    Description = (SELECT ft.Description FROM AD_Task_Trl ft, VAF_MenuConfig m
                            WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Task_ID=ft.AD_Task_ID
                            AND mt.VAF_Language=ft.VAF_Language),
                    IsTranslated = (SELECT ft.IsTranslated FROM AD_Task_Trl ft, VAF_MenuConfig m
                            WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Task_ID=ft.AD_Task_ID
                            AND mt.VAF_Language=ft.VAF_Language)
            WHERE EXISTS (SELECT * FROM AD_Task_Trl ft, VAF_MenuConfig m
                            WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Task_ID=ft.AD_Task_ID
                            AND mt.VAF_Language=ft.VAF_Language
                            AND m.AD_Task_ID IS NOT NULL
                            AND	Action = 'T');
            DBMS_OUTPUT.PUT_LINE('  trl rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE VAF_MenuConfig_TL mt " +
                    "	SET Name = (SELECT ft.Name FROM AD_Task_Trl ft, VAF_MenuConfig m " +
                    "					WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Task_ID=ft.AD_Task_ID " +
                    "					AND mt.VAF_Language=ft.VAF_Language), " +
                    "			Description = (SELECT ft.Description FROM AD_Task_Trl ft, VAF_MenuConfig m " +
                    "					WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Task_ID=ft.AD_Task_ID " +
                    "					AND mt.VAF_Language=ft.VAF_Language), " +
                    "			IsTranslated = (SELECT ft.IsTranslated FROM AD_Task_Trl ft, VAF_MenuConfig m " +
                    "					WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Task_ID=ft.AD_Task_ID " +
                    "					AND mt.VAF_Language=ft.VAF_Language) " +
                    "	WHERE EXISTS (SELECT * FROM AD_Task_Trl ft, VAF_MenuConfig m " +
                    "					WHERE mt.VAF_MenuConfig_ID=m.VAF_MenuConfig_ID AND m.AD_Task_ID=ft.AD_Task_ID " +
                    "					AND mt.VAF_Language=ft.VAF_Language " +
                    "					AND m.AD_Task_ID IS NOT NULL " +
                    "					AND  Action = 'T')";
            Execute("Synchronize Menu with Tasks Trl", sql, "  rows updated: ");

            /*

            --  Column Name + Element
            DBMS_OUTPUT.PUT_LINE('Synchronizing Column with Element');
            UPDATE VAF_Column c
              SET (Name,Description,Help) = 
                (SELECT e.Name,e.Description,e.Help 
                FROM VAF_ColumnDic e WHERE c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID)
            WHERE EXISTS 
                (SELECT * FROM VAF_ColumnDic e 
                WHERE c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID
                  AND c.Name<>e.Name);
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            executesql("Synchronize Field Translations", sql,  "  rows updated: ");
            UPDATE VAF_Column_TL ct
              SET Name = (SELECT e.Name
                FROM VAF_Column c INNER JOIN VAF_ColumnDic_TL e ON (c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID)
                WHERE ct.VAF_Column_ID=c.VAF_Column_ID AND ct.VAF_Language=e.VAF_Language)
            WHERE EXISTS 
                (SELECT * FROM VAF_Column c INNER JOIN VAF_ColumnDic_TL e ON (c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID)
                WHERE ct.VAF_Column_ID=c.VAF_Column_ID AND ct.VAF_Language=e.VAF_Language
                  AND ct.Name<>e.Name);
            DBMS_OUTPUT.PUT_LINE('  trl rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "    UPDATE VAF_Column c " +
                    "      SET Name = " +
                    "        (SELECT e.Name " +
                    "        FROM VAF_ColumnDic e WHERE c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID) " +
                      "      , Description = " +
                    "        (SELECT e.Description " +
                    "        FROM VAF_ColumnDic e WHERE c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID) " +
                      "      , Help = " +
                    "        (SELECT e.Help " +
                    "        FROM VAF_ColumnDic e WHERE c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID) " +
                    "    WHERE EXISTS " +
                    "        (SELECT * FROM VAF_ColumnDic e " +
                    "        WHERE c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID " +
                    "          AND c.Name<>e.Name)";
            Execute("Synchronize Column with Element", sql, "  rows updated: ");

            /*
    UPDATE VAF_Column_TL ct
      SET Name = (SELECT e.Name
        FROM VAF_Column c INNER JOIN VAF_ColumnDic_TL e ON (c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID)
        WHERE ct.VAF_Column_ID=c.VAF_Column_ID AND ct.VAF_Language=e.VAF_Language)
    WHERE EXISTS 
        (SELECT * FROM VAF_Column c INNER JOIN VAF_ColumnDic_TL e ON (c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID)
        WHERE ct.VAF_Column_ID=c.VAF_Column_ID AND ct.VAF_Language=e.VAF_Language
          AND ct.Name<>e.Name);
    */
            sql = "UPDATE VAF_Column_TL ct " +
                    "    SET Name = (SELECT e.Name " +
                    "        FROM VAF_Column c INNER JOIN VAF_ColumnDic_TL e ON (c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID) " +
                    "        WHERE ct.VAF_Column_ID=c.VAF_Column_ID AND ct.VAF_Language=e.VAF_Language) " +
                    "    WHERE EXISTS " +
                    "        (SELECT * FROM VAF_Column c INNER JOIN VAF_ColumnDic_TL e ON (c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID) " +
                    "        WHERE ct.VAF_Column_ID=c.VAF_Column_ID AND ct.VAF_Language=e.VAF_Language " +
                    "          AND ct.Name<>e.Name)";
            Execute("Synchronize Column with Element Trl", sql, "  rows updated: ");

            /*
   
    
            --  Table Name + Element
            DBMS_OUTPUT.PUT_LINE('Synchronizing Table with Element');
            UPDATE VAF_TableView t
              SET (Name,Description) = (SELECT e.Name,e.Description FROM VAF_ColumnDic e 
                WHERE t.TableName||'_ID'=e.ColumnName)
            WHERE EXISTS (SELECT * FROM VAF_ColumnDic e 
                WHERE t.TableName||'_ID'=e.ColumnName
                  AND t.Name<>e.Name);
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "    UPDATE VAF_TableView t " +
                    "      SET Name = (SELECT e.Name FROM VAF_ColumnDic e " +
                    "        WHERE t.TableName||'_ID'=e.ColumnName) " +
                      "      , Description = (SELECT e.Description FROM VAF_ColumnDic e " +
                    "        WHERE t.TableName||'_ID'=e.ColumnName) " +
                    "    WHERE EXISTS (SELECT * FROM VAF_ColumnDic e " +
                    "        WHERE t.TableName||'_ID'=e.ColumnName " +
                    "          AND t.Name<>e.Name)";
            Execute("Synchronize Table with Element", sql, "  rows updated: ");

            /*
            UPDATE VAF_TableView_TL tt
              SET Name = (SELECT e.Name 
                FROM VAF_TableView t INNER JOIN VAF_ColumnDic ex ON (t.TableName||'_ID'=ex.ColumnName)
                  INNER JOIN VAF_ColumnDic_TL e ON (ex.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID)
                WHERE tt.VAF_TableView_ID=t.VAF_TableView_ID AND tt.VAF_Language=e.VAF_Language)
            WHERE EXISTS (SELECT * 
                FROM VAF_TableView t INNER JOIN VAF_ColumnDic ex ON (t.TableName||'_ID'=ex.ColumnName)
                  INNER JOIN VAF_ColumnDic_TL e ON (ex.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID)
                WHERE tt.VAF_TableView_ID=t.VAF_TableView_ID AND tt.VAF_Language=e.VAF_Language
                  AND tt.Name<>e.Name);
            DBMS_OUTPUT.PUT_LINE('  trl rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "    UPDATE VAF_TableView_TL tt " +
                    "      SET Name = (SELECT e.Name " +
                    "        FROM VAF_TableView t INNER JOIN VAF_ColumnDic ex ON (t.TableName||'_ID'=ex.ColumnName) " +
                    "          INNER JOIN VAF_ColumnDic_TL e ON (ex.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID) " +
                    "        WHERE tt.VAF_TableView_ID=t.VAF_TableView_ID AND tt.VAF_Language=e.VAF_Language) " +
                    "    WHERE EXISTS (SELECT * " +
                    "        FROM VAF_TableView t INNER JOIN VAF_ColumnDic ex ON (t.TableName||'_ID'=ex.ColumnName) " +
                    "          INNER JOIN VAF_ColumnDic_TL e ON (ex.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID) " +
                    "        WHERE tt.VAF_TableView_ID=t.VAF_TableView_ID AND tt.VAF_Language=e.VAF_Language " +
                    "          AND tt.Name<>e.Name)";
            Execute("Synchronize Table with Element Trl", sql, "  rows updated: ");

            /*

            --  Trl Table Name + Element
            UPDATE VAF_TableView t
              SET (Name,Description) = (SELECT e.Name||' Trl', e.Description 
                FROM VAF_ColumnDic e 
                WHERE SUBSTR(t.TableName,1,LENGTH(t.TableName)-4)||'_ID'=e.ColumnName)
            WHERE TableName LIKE '%_Trl'
              AND EXISTS (SELECT * FROM VAF_ColumnDic e 
                WHERE SUBSTR(t.TableName,1,LENGTH(t.TableName)-4)||'_ID'=e.ColumnName
                  AND t.Name<>e.Name);
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "    UPDATE VAF_TableView t " +
                    "     SET Name = (SELECT e.Name||' Trl' " +
                    "        FROM VAF_ColumnDic e " +
                    "        WHERE SUBSTR(t.TableName,1,LENGTH(t.TableName)-4)||'_ID'=e.ColumnName) " +
                     "     , Description = (SELECT  e.Description " +
                    "        FROM VAF_ColumnDic e " +
                    "        WHERE SUBSTR(t.TableName,1,LENGTH(t.TableName)-4)||'_ID'=e.ColumnName) " +
                    "    WHERE TableName LIKE '%_Trl' " +
                    "      AND EXISTS (SELECT * FROM VAF_ColumnDic e " +
                    "        WHERE SUBSTR(t.TableName,1,LENGTH(t.TableName)-4)||'_ID'=e.ColumnName " +
                    "          AND t.Name<>e.Name)";
            Execute("Synchronize Trl Table Name + Element", sql, "  rows updated: ");

            /*
    UPDATE VAF_TableView_TL tt
      SET Name = (SELECT e.Name || ' **'
        FROM VAF_TableView t INNER JOIN VAF_ColumnDic ex ON (SUBSTR(t.TableName,1,LENGTH(t.TableName)-4)||'_ID'=ex.ColumnName)
          INNER JOIN VAF_ColumnDic_TL e ON (ex.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID)
        WHERE tt.VAF_TableView_ID=t.VAF_TableView_ID AND tt.VAF_Language=e.VAF_Language)
    WHERE EXISTS (SELECT * 
        FROM VAF_TableView t INNER JOIN VAF_ColumnDic ex ON (SUBSTR(t.TableName,1,LENGTH(t.TableName)-4)||'_ID'=ex.ColumnName)
          INNER JOIN VAF_ColumnDic_TL e ON (ex.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID)
        WHERE tt.VAF_TableView_ID=t.VAF_TableView_ID AND tt.VAF_Language=e.VAF_Language
          AND t.TableName LIKE '%_Trl'
          AND tt.Name<>e.Name);
    DBMS_OUTPUT.PUT_LINE('  trl rows updated: ' || SQL%ROWCOUNT);			 */
            sql = "    UPDATE VAF_TableView_TL tt " +
                    "      SET Name = (SELECT e.Name || ' **' " +
                    "        FROM VAF_TableView t INNER JOIN VAF_ColumnDic ex ON (SUBSTR(t.TableName,1,LENGTH(t.TableName)-4)||'_ID'=ex.ColumnName) " +
                    "          INNER JOIN VAF_ColumnDic_TL e ON (ex.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID) " +
                    "        WHERE tt.VAF_TableView_ID=t.VAF_TableView_ID AND tt.VAF_Language=e.VAF_Language) " +
                    "    WHERE EXISTS (SELECT * " +
                    "        FROM VAF_TableView t INNER JOIN VAF_ColumnDic ex ON (SUBSTR(t.TableName,1,LENGTH(t.TableName)-4)||'_ID'=ex.ColumnName) " +
                    "          INNER JOIN VAF_ColumnDic_TL e ON (ex.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID) " +
                    "        WHERE tt.VAF_TableView_ID=t.VAF_TableView_ID AND tt.VAF_Language=e.VAF_Language " +
                    "          AND t.TableName LIKE '%_Trl' " +
                    "          AND tt.Name<>e.Name)";
            Execute("Synchronize VAF_TableView_TL", sql, "  rows updated: ");

            sql = "UPDATE VAF_QuickSearchColumn ic " +
        "     SET Name = " +
        "          (SELECT e.Name " +
        "             FROM VAF_ColumnDic e  " +
        "            WHERE ic.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID) " +
        "     , Description = " +
        "          (SELECT e.Description " +
        "             FROM VAF_ColumnDic e  " +
        "            WHERE ic.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID) " +
        "     , Help = " +
        "          (SELECT e.Help " +
        "             FROM VAF_ColumnDic e  " +
        "            WHERE ic.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID) " +
        "    WHERE EXISTS " +
        "          (SELECT * FROM VAF_ColumnDic e " +
        "            WHERE ic.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID " +
        " 		       AND (ic.Name<>e.Name OR NVL(ic.Description,' ')<>NVL(e.Description,' ') OR NVL(ic.Help,' ')<>NVL(e.Help,' '))) " +
        "      AND ic.IsCentrallyMaintained='Y' AND ic.IsActive='Y'";
            Execute("Synchronize Info Column with Element", sql, "  rows updated: ");

            sql = "UPDATE VAF_QuickSearchColumn_TL ict " +
            "    SET Name = " +
            "        (SELECT e.Name  " +
            "           FROM VAF_QuickSearchColumn ic INNER JOIN VAF_ColumnDic_TL e ON (ic.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID) " +
            "          WHERE ict.VAF_QuickSearchColumn_ID=ic.VAF_QuickSearchColumn_ID AND ict.VAF_Language=e.VAF_Language) " +
              "    ,  Description = " +
            "        (SELECT  e.Description " +
            "           FROM VAF_QuickSearchColumn ic INNER JOIN VAF_ColumnDic_TL e ON (ic.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID) " +
            "          WHERE ict.VAF_QuickSearchColumn_ID=ic.VAF_QuickSearchColumn_ID AND ict.VAF_Language=e.VAF_Language) " +
              "    ,  Help = " +
            "        (SELECT  e.Help  " +
            "           FROM VAF_QuickSearchColumn ic INNER JOIN VAF_ColumnDic_TL e ON (ic.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID) " +
            "          WHERE ict.VAF_QuickSearchColumn_ID=ic.VAF_QuickSearchColumn_ID AND ict.VAF_Language=e.VAF_Language) " +
              "    ,  isTranslated = " +
            "        (SELECT  isTranslated  " +
            "           FROM VAF_QuickSearchColumn ic INNER JOIN VAF_ColumnDic_TL e ON (ic.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID) " +
            "          WHERE ict.VAF_QuickSearchColumn_ID=ic.VAF_QuickSearchColumn_ID AND ict.VAF_Language=e.VAF_Language) " +
            "    WHERE EXISTS " +
            "        (SELECT * FROM VAF_QuickSearchColumn ic  " +
            "          INNER JOIN VAF_ColumnDic_TL e ON (ic.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID) " +
            "          WHERE ict.VAF_QuickSearchColumn_ID=ic.VAF_QuickSearchColumn_ID AND ict.VAF_Language=e.VAF_Language " +
            "            AND ic.IsCentrallyMaintained='Y' AND ic.IsActive='Y' " +
            "	         AND (ict.Name<>e.Name OR NVL(ict.Description,' ')<>NVL(e.Description,' ') OR NVL(ict.Help,' ')<>NVL(e.Help,' ') OR NVL(ict.isTranslated,' ')<>NVL(e.isTranslated,' ')))";
            Execute("Synchronize Info Column with Element Trl", sql, "  rows updated: ");

            if (result)
                return "Sucessful Synchronized";
            else
                return "Synchronized with error(s)";
        }
        /// <summary>
        /// Execute Query
        /// </summary>
        /// <param name="msg">message</param>
        /// <param name="sql">Query</param>
        /// <param name="pmsg">pmsg</param>
        private void Execute(String msg, String sql, String pmsg)
        {
            log.Info(msg);
            try
            {
                int no = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
                log.Fine(pmsg + no);
                if (no < 0)
                {
                    result = false;
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
                result = false;
            }


        }


    }
}
