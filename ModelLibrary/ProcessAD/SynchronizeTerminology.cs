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


using VAdvantage.ProcessEngine;namespace VAdvantage.Process
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
                        "FROM	AD_Column c  " +
                        "WHERE NOT EXISTS  " +
                        "	(SELECT * FROM AD_Element e  " +
                        "	WHERE UPPER(c.ColumnName)=UPPER(e.ColumnName))";
                idr = DataBase.DB.ExecuteReader(sql);
                DataSet ds = null;
                while (idr.Read())
                {
                    try
                    {
                        if (ds == null)
                        {
                            sql = "INSERT INTO AD_Element"
                                    + " (AD_Element_ID, AD_Client_ID, AD_Org_ID,"
                                    + " IsActive, Created, CreatedBy, Updated, UpdatedBy,"
                                    + " ColumnName, Name, PrintName, Description, Help, EntityType)"
                                    + " VALUES"
                                    + " (@param1, 0, 0,"  //1, NextNo
                                    + " 'Y', CURRENT_TIMESTAMP, 0, CURRENT_TIMESTAMP, 0,"
                                    + " @param2, @param3, @param4, @param5, @param6, @param7)"; //2-7 CC.ColumnName, CC.Name, CC.Name, CC.Description, CC.Help, CC.EntityType	

                        }

                        int id = DataBase.DB.GetNextID(GetAD_Client_ID(), "AD_Element", Get_Trx());
                        if (id <= 0)
                        {
                            log.Severe("Steps  " + steps + ", No NextID ( " + id + ")");
                            idr.Close();
                            return "Steps  " + steps + " No NextID for AD_Element";
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
                        "FROM	AD_Process_Para p " +
                        "WHERE NOT EXISTS " +
                        "	(SELECT * FROM AD_Element e " +
                        "	WHERE UPPER(p.ColumnName)=UPPER(e.ColumnName))";
                idr = DataBase.DB.ExecuteReader(sql);
                DataSet ds = null;
                while (idr.Read())
                {
                    try
                    {
                        if (ds == null)
                        {
                            sql = "INSERT INTO AD_Element"
                                    + " (AD_Element_ID, AD_Client_ID, AD_Org_ID,"
                                    + " IsActive, Created, CreatedBy, Updated, UpdatedBy,"
                                    + " ColumnName, Name, PrintName, Description, Help, EntityType)"
                                    + " VALUES"
                                    + " (@param1, 0, 0,"  //1, NextNo
                                    + " 'Y', CURRENT_TIMESTAMP, 0, CURRENT_TIMESTAMP, 0,"
                                    + " @param2, @param3, @param4, @param5, @param6, @param7)"; //2-7 CC.ColumnName, CC.Name, CC.Name, CC.Description, CC.Help, CC.EntityType	

                        }
                        int id = DataBase.DB.GetNextID(GetAD_Client_ID(), "AD_Element", Get_Trx());
                        if (id <= 0)
                        {
                            log.Severe("Steps " + steps + ", No NextID (" + id + ")");
                            idr.Close();
                            return "Steps " + steps + " No NextID for AD_Element";
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
                        if(idr != null)
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

            sql = "INSERT INTO AD_Element_Trl (AD_Element_ID, AD_Language, AD_Client_ID, AD_Org_ID,"
                + "IsActive, Created, CreatedBy, Updated, UpdatedBy,"
                + " Name, PrintName, Description, Help, IsTranslated)"
                + " SELECT m.AD_Element_ID, l.AD_Language, m.AD_Client_ID, m.AD_Org_ID,"
                + " m.IsActive, m.Created, m.CreatedBy, m.Updated, m.UpdatedBy,"
                + " m.Name, m.PrintName, m.Description, m.Help, 'N'"
                + " FROM	AD_Element m, AD_Language l"
                + " WHERE	l.IsActive = 'Y' AND l.IsSystemLanguage = 'Y'"
                + " AND	AD_Element_ID || AD_Language NOT IN"
                + " (SELECT AD_Element_ID || AD_Language FROM AD_Element_Trl)";
            Execute("Adding missing Element Translations", sql, "  rows added: ");

            sql = "INSERT INTO AD_ElementCTX_Trl (AD_ElementCTX_ID, AD_Language, AD_Client_ID, AD_Org_ID,"
                + "IsActive, Created, CreatedBy, Updated, UpdatedBy,"
                + " Name, PrintName, Description, Help, IsTranslated)"
                + " SELECT m.AD_ElementCTX_ID, l.AD_Language, m.AD_Client_ID, m.AD_Org_ID,"
                + " m.IsActive, m.Created, m.CreatedBy, m.Updated, m.UpdatedBy,"
                + " m.Name, m.PrintName, m.Description, m.Help, 'N'"
                + " FROM	AD_ElementCTX m, AD_Language l"
                + " WHERE	l.IsActive = 'Y' AND l.IsSystemLanguage = 'Y'"
                + " AND	AD_ElementCTX_ID || AD_Language NOT IN"
                + " (SELECT AD_ElementCTX_ID || AD_Language FROM AD_ElementCTX_Trl)";
            Execute("Adding missing context specific Element Translations", sql, "  rows added: ");

            /*
             * 	DBMS_OUTPUT.PUT_LINE('Creating link from Element to Column');
        UPDATE	AD_Column c
        SET		AD_Element_id = 
                    (SELECT AD_Element_ID FROM AD_Element e 
                    WHERE UPPER(c.ColumnName)=UPPER(e.ColumnName))
        WHERE AD_Element_ID IS NULL;
        DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
        COMMIT;
             */
            sql = "UPDATE AD_Column c"
                    + " SET AD_Element_id ="
                    + " (SELECT AD_Element_ID FROM AD_Element e"
                    + " WHERE UPPER(c.ColumnName)=UPPER(e.ColumnName))"
                    + " WHERE AD_Element_ID IS NULL";
            Execute("Creating link from Element to Column", sql, "  rows updated: ");

            /*
                DBMS_OUTPUT.PUT_LINE('Deleting unused Elements');
        DELETE	AD_Element_Trl
        WHERE	AD_Element_ID IN
            (SELECT AD_Element_ID FROM AD_Element e 
            WHERE NOT EXISTS
                (SELECT * FROM AD_Column c WHERE UPPER(e.ColumnName)=UPPER(c.ColumnName))
            AND NOT EXISTS
                (SELECT * FROM AD_InfoColumn c WHERE e.AD_Element_ID=c.AD_Element_ID)
            AND NOT EXISTS
                (SELECT * FROM AD_Process_Para p WHERE UPPER(e.ColumnName)=UPPER(p.ColumnName)));

        DELETE	AD_Element e
        WHERE NOT EXISTS
            (SELECT * FROM AD_Column c WHERE UPPER(e.ColumnName)=UPPER(c.ColumnName))
        AND NOT EXISTS
            (SELECT * FROM AD_InfoColumn c WHERE e.AD_Element_ID=c.AD_Element_ID)
        AND NOT EXISTS
            (SELECT * FROM AD_Process_Para p WHERE UPPER(e.ColumnName)=UPPER(p.ColumnName));
        DBMS_OUTPUT.PUT_LINE('  rows deleted: ' || SQL%ROWCOUNT);

             */
            sql = "DELETE	AD_Element_Trl"
                + " WHERE	AD_Element_ID IN"
                + " (SELECT AD_Element_ID FROM AD_Element e"
                    + " WHERE NOT EXISTS"
                    + " (SELECT * FROM AD_Column c WHERE UPPER(e.ColumnName)=UPPER(c.ColumnName))"
                    + " AND NOT EXISTS"
                    + " (SELECT * FROM AD_InfoColumn c WHERE e.AD_Element_ID=c.AD_Element_ID)"
                    + " AND NOT EXISTS"
                    + " (SELECT * FROM AD_Process_Para p WHERE UPPER(e.ColumnName)=UPPER(p.ColumnName)))";
            //not do delete for now
            //executesql("Deleting unused Elements-TRL", sql,  "  rows deleted: ");

            sql = "DELETE	AD_Element"
                + " WHERE NOT EXISTS"
                + " (SELECT * FROM AD_Column c WHERE UPPER(e.ColumnName)=UPPER(c.ColumnName))"
                + " AND NOT EXISTS"
                + " (SELECT * FROM AD_InfoColumn c WHERE e.AD_Element_ID=c.AD_Element_ID)"
                + " AND NOT EXISTS"
                + " (SELECT * FROM AD_Process_Para p WHERE UPPER(e.ColumnName)=UPPER(p.ColumnName)))";
            //executesql("Deleting unused Elements", sql,  "  rows deleted: ");

            /*
                --	Columns
        DBMS_OUTPUT.PUT_LINE('Synchronize Column');
	
    **  Identify offending column
    SELECT UPPER(ColumnName)
    FROM AD_Element
    GROUP BY UPPER(ColumnName)
    HAVING COUNT(UPPER(ColumnName)) > 1

    SELECT c.ColumnName, e.ColumnName
    FROM AD_Column c
      INNER JOIN AD_Element e ON (c.AD_Element_ID=e.AD_Element_ID)
    WHERE c.ColumnName <> e.ColumnName
    **

        UPDATE AD_Column c
            SET	(ColumnName, Name, Description, Help) = 
                    (SELECT ColumnName, Name, Description, Help 
                    FROM AD_Element e WHERE c.AD_Element_ID=e.AD_Element_ID),
                Updated = SysDate
        WHERE EXISTS (SELECT * FROM AD_Element e 
                    WHERE c.AD_Element_ID=e.AD_Element_ID
                      AND (c.ColumnName <> e.ColumnName OR c.Name <> e.Name 
                        OR NVL(c.Description,' ') <> NVL(e.Description,' ') OR NVL(c.Help,' ') <> NVL(e.Help,' ')));
        DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);

             */
            sql = "UPDATE	AD_Column c"
                    + " SET	(ColumnName, Name, Description, Help, Updated) ="
                    + " (SELECT ColumnName, Name, Description, Help, CURRENT_TIMESTAMP"
                    + " FROM AD_Element e WHERE c.AD_Element_ID=e.AD_Element_ID)"
                    + " WHERE EXISTS (SELECT * FROM AD_Element e "
                    + "  WHERE c.AD_Element_ID=e.AD_Element_ID"
                    + "   AND (c.ColumnName <> e.ColumnName OR c.Name <> e.Name "
                    + "   OR NVL(c.Description,' ') <> NVL(e.Description,' ') OR NVL(c.Help,' ') <> NVL(e.Help,' ')))";
            Execute("Synchronize Column", sql, "  rows updated: ");

            /*
             * 	--	Fields should now be syncronized
        DBMS_OUTPUT.PUT_LINE('Synchronize Field');
        UPDATE AD_Field f
            SET (Name, Description, Help) = 
                    (SELECT e.Name, e.Description, e.Help
                    FROM AD_Element e, AD_Column c
                    WHERE e.AD_Element_ID=c.AD_Element_ID AND c.AD_Column_ID=f.AD_Column_ID),
                Updated = SysDate
        WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y'
          AND EXISTS (SELECT * FROM AD_Element e, AD_Column c
                    WHERE f.AD_Column_ID=c.AD_Column_ID
                      AND c.AD_Element_ID=e.AD_Element_ID AND c.AD_Process_ID IS NULL
                      AND (f.Name <> e.Name OR NVL(f.Description,' ') <> NVL(e.Description,' ') OR NVL(f.Help,' ') <> NVL(e.Help,' ')));
        DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);

             */
            sql = "UPDATE AD_Field f"
                + " SET (Name, Description, Help, Updated) = "
                + "             (SELECT e.Name, e.Description, e.Help, CURRENT_TIMESTAMP"
                + "             FROM AD_Element e, AD_Column c"
                + "     	    WHERE e.AD_Element_ID=c.AD_Element_ID AND c.AD_Column_ID=f.AD_Column_ID)"
                + " 	WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y'"
                + " 	  AND EXISTS (SELECT * FROM AD_Element e, AD_Column c"
                + " 				WHERE f.AD_Column_ID=c.AD_Column_ID"
                + " 				  AND c.AD_Element_ID=e.AD_Element_ID AND c.AD_Process_ID IS NULL"
                + " 				  AND (f.Name <> e.Name OR NVL(f.Description,' ') <> NVL(e.Description,' ') OR NVL(f.Help,' ') <> NVL(e.Help,' ')))"
                + "AND NOT EXISTS ("
                + "      SELECT *"
                + "      FROM AD_Tab t, AD_Window w, AD_Column c, AD_ElementCTX e"
                + "      WHERE t.AD_Tab_ID=f.AD_Tab_ID AND w.AD_Window_ID=t.AD_Window_ID"
                + "      AND c.AD_Column_ID=f.AD_Column_ID AND e.AD_Element_ID=c.AD_Element_ID"
                + "      AND e.AD_CTXArea_ID=COALESCE(t.AD_CTXArea_ID, w.AD_CTXArea_ID))";
            Execute("Synchronize Field", sql, "  rows updated: ");

            /*
             * 	--	Field Translations
    DBMS_OUTPUT.PUT_LINE('Synchronize Field Translations');
    UPDATE AD_Field_trl trl
        SET Name = (SELECT e.Name FROM AD_Element_trl e, AD_Column c, AD_Field f
                    WHERE e.AD_Language=trl.AD_Language AND e.AD_Element_ID=c.AD_Element_ID 
                      AND c.AD_Column_ID=f.AD_Column_ID AND f.AD_Field_ID=trl.AD_Field_ID),
            Description = (SELECT e.Description FROM AD_Element_trl e, AD_Column c, AD_Field f
                    WHERE e.AD_Language=trl.AD_Language AND e.AD_Element_ID=c.AD_Element_ID 
                      AND c.AD_Column_ID=f.AD_Column_ID AND f.AD_Field_ID=trl.AD_Field_ID),
            Help = (SELECT e.Help FROM AD_Element_trl e, AD_Column c, AD_Field f
                    WHERE e.AD_Language=trl.AD_Language AND e.AD_Element_ID=c.AD_Element_ID 
                      AND c.AD_Column_ID=f.AD_Column_ID AND f.AD_Field_ID=trl.AD_Field_ID),
            IsTranslated = (SELECT e.IsTranslated FROM AD_Element_trl e, AD_Column c, AD_Field f
                    WHERE e.AD_Language=trl.AD_Language AND e.AD_Element_ID=c.AD_Element_ID 
                      AND c.AD_Column_ID=f.AD_Column_ID AND f.AD_Field_ID=trl.AD_Field_ID),
            Updated = SysDate
    WHERE EXISTS (SELECT * FROM AD_Field f, AD_Element_trl e, AD_Column c
                WHERE trl.AD_Field_ID=f.AD_Field_ID
                  AND f.AD_Column_ID=c.AD_Column_ID
                  AND c.AD_Element_ID=e.AD_Element_ID AND c.AD_Process_ID IS NULL
                  AND trl.AD_Language=e.AD_Language
                  AND f.IsCentrallyMaintained='Y' AND f.IsActive='Y'
                  AND (trl.Name <> e.Name OR NVL(trl.Description,' ') <> NVL(e.Description,' ') OR NVL(trl.Help,' ') <> NVL(e.Help,' ')));
    DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);

             */
            sql = "	UPDATE AD_Field_trl trl"
                + " SET Name = (SELECT e.Name FROM AD_Element_trl e, AD_Column c, AD_Field f"
                + " 				WHERE e.AD_Language=trl.AD_Language AND e.AD_Element_ID=c.AD_Element_ID"
                + " 				  AND c.AD_Column_ID=f.AD_Column_ID AND f.AD_Field_ID=trl.AD_Field_ID),"
                + " 		Description = (SELECT e.Description FROM AD_Element_trl e, AD_Column c, AD_Field f"
                + " 				WHERE e.AD_Language=trl.AD_Language AND e.AD_Element_ID=c.AD_Element_ID"
                + " 				  AND c.AD_Column_ID=f.AD_Column_ID AND f.AD_Field_ID=trl.AD_Field_ID),"
                + " 		Help = (SELECT e.Help FROM AD_Element_trl e, AD_Column c, AD_Field f"
                + " 				WHERE e.AD_Language=trl.AD_Language AND e.AD_Element_ID=c.AD_Element_ID"
                + " 				  AND c.AD_Column_ID=f.AD_Column_ID AND f.AD_Field_ID=trl.AD_Field_ID),"
                + " 		IsTranslated = (SELECT e.IsTranslated FROM AD_Element_trl e, AD_Column c, AD_Field f"
                + " 				WHERE e.AD_Language=trl.AD_Language AND e.AD_Element_ID=c.AD_Element_ID"
                + " 				  AND c.AD_Column_ID=f.AD_Column_ID AND f.AD_Field_ID=trl.AD_Field_ID),"
                + " 		Updated = CURRENT_TIMESTAMP"
                + " 	WHERE EXISTS (SELECT * FROM AD_Field f, AD_Element_trl e, AD_Column c"
                + " 				WHERE trl.AD_Field_ID=f.AD_Field_ID"
                + " 				  AND f.AD_Column_ID=c.AD_Column_ID"
                + " 				  AND c.AD_Element_ID=e.AD_Element_ID AND c.AD_Process_ID IS NULL"
                + " 				  AND trl.AD_Language=e.AD_Language"
                + " 				  AND f.IsCentrallyMaintained='Y' AND f.IsActive='Y'"
                + " 				  AND (trl.Name <> e.Name OR NVL(trl.Description,' ') <> NVL(e.Description,' ') OR NVL(trl.Help,' ') <> NVL(e.Help,' ')))"
                + "AND NOT EXISTS ("
                + "      SELECT *"
                + "      FROM AD_Tab t, AD_Window w, AD_Column c, AD_ElementCTX e, AD_Field f"
                + "      WHERE t.AD_Tab_ID=f.AD_Tab_ID AND w.AD_Window_ID=t.AD_Window_ID"
                + "      AND c.AD_Column_ID=f.AD_Column_ID AND e.AD_Element_ID=c.AD_Element_ID"
                + "      AND e.AD_CTXArea_ID=COALESCE(t.AD_CTXArea_ID, w.AD_CTXArea_ID)"
                + "      AND f.AD_Field_ID = trl.AD_Field_ID)";
            Execute("Synchronize Field Translations", sql, "  rows updated: ");

            /*	
    --	Fields should now be syncronized
    DBMS_OUTPUT.PUT_LINE('Synchronize PO Field');
    UPDATE AD_Field f
        SET Name = (SELECT e.PO_Name FROM AD_Element e, AD_Column c
                    WHERE e.AD_Element_ID=c.AD_Element_ID AND c.AD_Column_ID=f.AD_Column_ID),
            Description = (SELECT e.PO_Description FROM AD_Element e, AD_Column c
                    WHERE e.AD_Element_ID=c.AD_Element_ID AND c.AD_Column_ID=f.AD_Column_ID),
            Help = (SELECT e.PO_Help FROM AD_Element e, AD_Column c
                    WHERE e.AD_Element_ID=c.AD_Element_ID AND c.AD_Column_ID=f.AD_Column_ID),
            Updated = SysDate
    WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y'
      AND EXISTS (SELECT * FROM AD_Element e, AD_Column c
                WHERE f.AD_Column_ID=c.AD_Column_ID
                  AND c.AD_Element_ID=e.AD_Element_ID AND c.AD_Process_ID IS NULL
                  AND (f.Name <> e.PO_Name OR NVL(f.Description,' ') <> NVL(e.PO_Description,' ') OR NVL(f.Help,' ') <> NVL(e.PO_Help,' '))
                  AND e.PO_Name IS NOT NULL)
      AND EXISTS (SELECT * FROM AD_Tab t, AD_Window w
                WHERE f.AD_Tab_ID=t.AD_Tab_ID
                  AND t.AD_Window_ID=w.AD_Window_ID
                  AND w.IsSOTrx='N');
    DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
    */
            /*TODO  ?
                        sql = "	UPDATE AD_Field f"
                                + " SET Name = (SELECT e.Name FROM AD_Element e, AD_Column c"
                                + " 			WHERE e.AD_Element_ID=c.AD_Element_ID AND c.AD_Column_ID=f.AD_Column_ID),"
                                + " 	Description = (SELECT e.Description FROM AD_Element e, AD_Column c"
                                + " 			WHERE e.AD_Element_ID=c.AD_Element_ID AND c.AD_Column_ID=f.AD_Column_ID),"
                                + " 	Help = (SELECT e.Help FROM AD_Element e, AD_Column c"
                                + " 			WHERE e.AD_Element_ID=c.AD_Element_ID AND c.AD_Column_ID=f.AD_Column_ID),"
                                + " 	Updated = CURRENT_TIMESTAMP"
                                + " WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y'"
                                + "   AND EXISTS (SELECT * FROM AD_Element e, AD_Column c"
                                + " 			WHERE f.AD_Column_ID=c.AD_Column_ID"
                                + " 			  AND c.AD_Element_ID=e.AD_Element_ID AND c.AD_Process_ID IS NULL"
                                + " 			  AND (f.Name <> e.Name OR NVL(f.Description,' ') <> NVL(e.Description,' ') OR NVL(f.Help,' ') <> NVL(e.Help,' '))"
                                + " 			  AND e.Name IS NOT NULL)"
                                + "   AND EXISTS (SELECT * FROM AD_Tab t, AD_Window w"
                                + " 			WHERE f.AD_Tab_ID=t.AD_Tab_ID"
                                + " 			  AND t.AD_Window_ID=w.AD_Window_ID" 
                                + " 			  AND w.AD_CtxArea_ID IS NULL" 
                                + " 			  AND t.AD_CtxArea_ID IS NULL)";
                        executesql("Synchronize PO Translations", sql,  "  rows updated: ");
                        */

            /*
    --	Field Translations
    DBMS_OUTPUT.PUT_LINE('Synchronize PO Field Translations');
    UPDATE AD_Field_trl trl
        SET Name = (SELECT e.PO_Name FROM AD_Element_trl e, AD_Column c, AD_Field f
                    WHERE e.AD_Language=trl.AD_Language AND e.AD_Element_ID=c.AD_Element_ID 
                      AND c.AD_Column_ID=f.AD_Column_ID AND f.AD_Field_ID=trl.AD_Field_ID),
            Description = (SELECT e.PO_Description FROM AD_Element_trl e, AD_Column c, AD_Field f
                    WHERE e.AD_Language=trl.AD_Language AND e.AD_Element_ID=c.AD_Element_ID 
                      AND c.AD_Column_ID=f.AD_Column_ID AND f.AD_Field_ID=trl.AD_Field_ID),
            Help = (SELECT e.PO_Help FROM AD_Element_trl e, AD_Column c, AD_Field f
                    WHERE e.AD_Language=trl.AD_Language AND e.AD_Element_ID=c.AD_Element_ID 
                      AND c.AD_Column_ID=f.AD_Column_ID AND f.AD_Field_ID=trl.AD_Field_ID),
            IsTranslated = (SELECT e.IsTranslated FROM AD_Element_trl e, AD_Column c, AD_Field f
                    WHERE e.AD_Language=trl.AD_Language AND e.AD_Element_ID=c.AD_Element_ID 
                      AND c.AD_Column_ID=f.AD_Column_ID AND f.AD_Field_ID=trl.AD_Field_ID),
            Updated = CURRENT_TIMESTAMP
    WHERE EXISTS (SELECT * FROM AD_Field f, AD_Element_trl e, AD_Column c
                WHERE trl.AD_Field_ID=f.AD_Field_ID
                  AND f.AD_Column_ID=c.AD_Column_ID
                  AND c.AD_Element_ID=e.AD_Element_ID AND c.AD_Process_ID IS NULL
                  AND trl.AD_Language=e.AD_Language
                  AND f.IsCentrallyMaintained='Y' AND f.IsActive='Y'
                  AND (trl.Name <> e.PO_Name OR NVL(trl.Description,' ') <> NVL(e.PO_Description,' ') OR NVL(trl.Help,' ') <> NVL(e.PO_Help,' '))
                  AND e.PO_Name IS NOT NULL)
      AND EXISTS (SELECT * FROM AD_Field f, AD_Tab t, AD_Window w
                WHERE trl.AD_Field_ID=f.AD_Field_ID
                  AND f.AD_Tab_ID=t.AD_Tab_ID
                  AND t.AD_Window_ID=w.AD_Window_ID
                  AND w.IsSOTrx='N');
    DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
    */
            /*
            sql = "	UPDATE AD_Field_trl trl"
                    + " SET Name = (SELECT e.Name FROM AD_Element_trl e, AD_Column c, AD_Field f"
                    + " 			WHERE e.AD_Language=trl.AD_Language AND e.AD_Element_ID=c.AD_Element_ID "
                    + " 			  AND c.AD_Column_ID=f.AD_Column_ID AND f.AD_Field_ID=trl.AD_Field_ID),"
                    + " 	Description = (SELECT e.Description FROM AD_Element_trl e, AD_Column c, AD_Field f"
                    + " 			WHERE e.AD_Language=trl.AD_Language AND e.AD_Element_ID=c.AD_Element_ID "
                    + " 			  AND c.AD_Column_ID=f.AD_Column_ID AND f.AD_Field_ID=trl.AD_Field_ID),"
                    + " 	Help = (SELECT e.Help FROM AD_Element_trl e, AD_Column c, AD_Field f"
                    + " 			WHERE e.AD_Language=trl.AD_Language AND e.AD_Element_ID=c.AD_Element_ID "
                    + " 			  AND c.AD_Column_ID=f.AD_Column_ID AND f.AD_Field_ID=trl.AD_Field_ID),"
                    + " 	IsTranslated = (SELECT e.IsTranslated FROM AD_Element_trl e, AD_Column c, AD_Field f"
                    + " 			WHERE e.AD_Language=trl.AD_Language AND e.AD_Element_ID=c.AD_Element_ID "
                    + " 			  AND c.AD_Column_ID=f.AD_Column_ID AND f.AD_Field_ID=trl.AD_Field_ID),"
                    + " 	Updated = CURRENT_TIMESTAMP"
                    + " 	WHERE EXISTS (SELECT * FROM AD_Field f, AD_Element_trl e, AD_Column c"
                    + " 				WHERE trl.AD_Field_ID=f.AD_Field_ID"
                    + " 				  AND f.AD_Column_ID=c.AD_Column_ID"
                    + " 				  AND c.AD_Element_ID=e.AD_Element_ID AND c.AD_Process_ID IS NULL"
                    + " 				  AND trl.AD_Language=e.AD_Language"
                    + " 				  AND f.IsCentrallyMaintained='Y' AND f.IsActive='Y'"
                    + " 				  AND (trl.Name <> e.Name OR NVL(trl.Description,' ') <> NVL(e.Description,' ') OR NVL(trl.Help,' ') <> NVL(e.Help,' '))"
                    + " 				  AND e.Name IS NOT NULL)"
                    + " 	  AND EXISTS (SELECT * FROM AD_Field f, AD_Tab t, AD_Window w"
                    + " 				WHERE trl.AD_Field_ID=f.AD_Field_ID"
                    + " 				  AND f.AD_Tab_ID=t.AD_Tab_ID"
                    + " 				  AND t.AD_Window_ID=w.AD_Window_ID"
                    + " 			  AND w.AD_CtxArea_ID IS NULL" 
                    + " 			  AND t.AD_CtxArea_ID IS NULL)";
            executesql("Synchronize PO Field Translations", sql,  "  rows updated: ");
            */

            //
            sql = "	UPDATE AD_Field f"
                + " SET Name = (SELECT e.Name FROM AD_ElementCTX e JOIN AD_Column c ON (e.AD_Element_ID=c.AD_Element_ID)"
                + " 			WHERE c.AD_Column_ID=f.AD_Column_ID"
                + "   				AND EXISTS (SELECT * FROM AD_Tab t JOIN AD_Window w ON (t.AD_Window_ID=w.AD_Window_ID)"
                + " 					WHERE f.AD_Tab_ID=t.AD_Tab_ID"
                + " 			  		AND (w.AD_CtxArea_ID IS NOT NULL AND t.AD_CtxArea_ID IS NULL AND e.AD_CtxArea_ID=w.AD_CtxArea_ID"
                + " 			  			OR t.AD_CtxArea_ID IS NOT NULL AND e.AD_CtxArea_ID=t.AD_CtxArea_ID))),"
                + " 	Description = (SELECT e.Description FROM AD_ElementCTX e JOIN AD_Column c ON (e.AD_Element_ID=c.AD_Element_ID)"
                + " 			WHERE c.AD_Column_ID=f.AD_Column_ID"
                + "   				AND EXISTS (SELECT * FROM AD_Tab t JOIN AD_Window w ON (t.AD_Window_ID=w.AD_Window_ID)"
                + " 					WHERE f.AD_Tab_ID=t.AD_Tab_ID"
                + " 			  		AND (w.AD_CtxArea_ID IS NOT NULL AND t.AD_CtxArea_ID IS NULL AND e.AD_CtxArea_ID=w.AD_CtxArea_ID"
                + " 			  			OR t.AD_CtxArea_ID IS NOT NULL AND e.AD_CtxArea_ID=t.AD_CtxArea_ID))),"
                + " 	Help = (SELECT e.Help FROM AD_ElementCTX e JOIN AD_Column c ON (e.AD_Element_ID=c.AD_Element_ID)"
                + " 			WHERE c.AD_Column_ID=f.AD_Column_ID"
                + "   				AND EXISTS (SELECT * FROM AD_Tab t JOIN AD_Window w ON (t.AD_Window_ID=w.AD_Window_ID)"
                + " 					WHERE f.AD_Tab_ID=t.AD_Tab_ID"
                + " 			  		AND (w.AD_CtxArea_ID IS NOT NULL AND t.AD_CtxArea_ID IS NULL AND e.AD_CtxArea_ID=w.AD_CtxArea_ID"
                + " 			  			OR t.AD_CtxArea_ID IS NOT NULL AND e.AD_CtxArea_ID=t.AD_CtxArea_ID))),"
                + " 	Updated = CURRENT_TIMESTAMP"
                + " WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y'"
                + "   AND EXISTS (SELECT * FROM AD_ElementCTX e, AD_Column c, AD_Tab t, AD_Window w"
                + " 			WHERE f.AD_Column_ID=c.AD_Column_ID"
                + " 			  AND c.AD_Element_ID=e.AD_Element_ID AND c.AD_Process_ID IS NULL"
                + " 			  AND (f.Name <> e.Name OR NVL(f.Description,' ') <> NVL(e.Description,' ') OR NVL(f.Help,' ') <> NVL(e.Help,' '))"
                + " 			  AND e.Name IS NOT NULL"
                + "               AND t.AD_Tab_ID=f.AD_Tab_ID AND w.AD_Window_ID=t.AD_Window_ID"
                + "		          AND e.AD_CtxArea_ID = COALESCE(t.AD_CtxArea_ID,w.AD_CtxArea_ID))";
            Execute("Synchronize Fields with ElementCTX", sql, "  rows updated: ");

            //
            sql = "	UPDATE AD_Field_trl trl"
                + " SET Name = (SELECT et.Name FROM AD_ElementCTX_trl et "
                + " 				JOIN AD_ElementCTX e ON (et.AD_ElementCTX_ID=e.AD_ElementCTX_ID)"
                + "					JOIN AD_Column c ON (e.AD_Element_ID=c.AD_Element_ID) "
                + "					JOIN AD_Field f ON (c.AD_Column_ID=f.AD_Column_ID)"
                + " 			WHERE et.AD_Language=trl.AD_Language AND f.AD_Field_ID=trl.AD_Field_ID"
                + "   				AND EXISTS (SELECT * FROM AD_Tab t JOIN AD_Window w ON (t.AD_Window_ID=w.AD_Window_ID)"
                + " 					WHERE f.AD_Tab_ID=t.AD_Tab_ID"
                + " 			  		AND (w.AD_CtxArea_ID IS NOT NULL AND t.AD_CtxArea_ID IS NULL AND e.AD_CtxArea_ID=w.AD_CtxArea_ID"
                + " 			  			OR t.AD_CtxArea_ID IS NOT NULL AND e.AD_CtxArea_ID=t.AD_CtxArea_ID))),"
                + " 	Description = (SELECT et.Description FROM AD_ElementCTX_trl et "
                + " 				JOIN AD_ElementCTX e ON (et.AD_ElementCTX_ID=e.AD_ElementCTX_ID)"
                + "					JOIN AD_Column c ON (e.AD_Element_ID=c.AD_Element_ID) "
                + "					JOIN AD_Field f ON (c.AD_Column_ID=f.AD_Column_ID)"
                + " 			WHERE et.AD_Language=trl.AD_Language AND f.AD_Field_ID=trl.AD_Field_ID"
                + "   				AND EXISTS (SELECT * FROM AD_Tab t JOIN AD_Window w ON (t.AD_Window_ID=w.AD_Window_ID)"
                + " 					WHERE f.AD_Tab_ID=t.AD_Tab_ID"
                + " 			  		AND (w.AD_CtxArea_ID IS NOT NULL AND t.AD_CtxArea_ID IS NULL AND e.AD_CtxArea_ID=w.AD_CtxArea_ID"
                + " 			  			OR t.AD_CtxArea_ID IS NOT NULL AND e.AD_CtxArea_ID=t.AD_CtxArea_ID))),"
                + " 	Help = (SELECT et.Help FROM AD_ElementCTX_trl et "
                + " 				JOIN AD_ElementCTX e ON (et.AD_ElementCTX_ID=e.AD_ElementCTX_ID)"
                + "					JOIN AD_Column c ON (e.AD_Element_ID=c.AD_Element_ID) "
                + "					JOIN AD_Field f ON (c.AD_Column_ID=f.AD_Column_ID)"
                + " 			WHERE et.AD_Language=trl.AD_Language AND f.AD_Field_ID=trl.AD_Field_ID"
                + "   				AND EXISTS (SELECT * FROM AD_Tab t JOIN AD_Window w ON (t.AD_Window_ID=w.AD_Window_ID)"
                + " 					WHERE f.AD_Tab_ID=t.AD_Tab_ID"
                + " 			  		AND (w.AD_CtxArea_ID IS NOT NULL AND t.AD_CtxArea_ID IS NULL AND e.AD_CtxArea_ID=w.AD_CtxArea_ID"
                + " 			  			OR t.AD_CtxArea_ID IS NOT NULL AND e.AD_CtxArea_ID=t.AD_CtxArea_ID))),"
                + " 	IsTranslated = (SELECT et.IsTranslated FROM AD_ElementCTX_trl et "
                + " 				JOIN AD_ElementCTX e ON (et.AD_ElementCTX_ID=e.AD_ElementCTX_ID)"
                + "					JOIN AD_Column c ON (e.AD_Element_ID=c.AD_Element_ID) "
                + "					JOIN AD_Field f ON (c.AD_Column_ID=f.AD_Column_ID)"
                + " 			WHERE et.AD_Language=trl.AD_Language AND f.AD_Field_ID=trl.AD_Field_ID"
                + "   				AND EXISTS (SELECT * FROM AD_Tab t JOIN AD_Window w ON (t.AD_Window_ID=w.AD_Window_ID)"
                + " 					WHERE f.AD_Tab_ID=t.AD_Tab_ID"
                + " 			  		AND (w.AD_CtxArea_ID IS NOT NULL AND t.AD_CtxArea_ID IS NULL AND e.AD_CtxArea_ID=w.AD_CtxArea_ID"
                + " 			  			OR t.AD_CtxArea_ID IS NOT NULL AND e.AD_CtxArea_ID=t.AD_CtxArea_ID))),"
                + " 	Updated = CURRENT_TIMESTAMP"
                + " 	WHERE EXISTS (SELECT * FROM AD_Field f, AD_ElementCTX_trl et, AD_ElementCTX e,  AD_Column c, AD_Tab t, AD_Window w"
                + " 				WHERE trl.AD_Field_ID=f.AD_Field_ID"
                + " 				  AND f.AD_Column_ID=c.AD_Column_ID"
                + " 				  AND c.AD_Element_ID=e.AD_Element_ID AND et.AD_ElementCTX_ID=e.AD_ElementCTX_ID AND c.AD_Process_ID IS NULL"
                + " 				  AND trl.AD_Language=et.AD_Language"
                + " 				  AND f.IsCentrallyMaintained='Y' AND f.IsActive='Y'"
                + " 				  AND (trl.Name <> et.Name OR NVL(trl.Description,' ') <> NVL(et.Description,' ') OR NVL(trl.Help,' ') <> NVL(et.Help,' '))"
                + " 				  AND et.Name IS NOT NULL"
                + "                   AND t.AD_Tab_ID=f.AD_Tab_ID AND w.AD_Window_ID=t.AD_Window_ID"
                + "                   AND e.AD_CtxArea_ID = COALESCE(t.AD_CtxArea_ID,w.AD_CtxArea_ID))";
            Execute("Synchronize fields with context specific element translations", sql, "  rows updated: ");

            /*
                --	Fields from Process
                DBMS_OUTPUT.PUT_LINE('Synchronize Field from Process');
                UPDATE AD_Field f
                    SET Name = (SELECT p.Name FROM AD_Process p, AD_Column c WHERE p.AD_Process_ID=c.AD_Process_ID
                                AND c.AD_Column_ID=f.AD_Column_ID),
                        Description = (SELECT p.Description FROM AD_Process p, AD_Column c WHERE p.AD_Process_ID=c.AD_Process_ID
                                AND c.AD_Column_ID=f.AD_Column_ID),
                        Help = (SELECT p.Help FROM AD_Process p, AD_Column c WHERE p.AD_Process_ID=c.AD_Process_ID
                                AND c.AD_Column_ID=f.AD_Column_ID),
                        Updated = SysDate
                WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y' 
                  AND EXISTS (SELECT * FROM AD_Process p, AD_Column c
                            WHERE c.AD_Process_ID=p.AD_Process_ID AND f.AD_Column_ID=c.AD_Column_ID
                            AND (f.Name<>p.Name OR NVL(f.Description,' ')<>NVL(p.Description,' ') OR NVL(f.Help,' ')<>NVL(p.Help,' ')));
                DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
                */


            //// Note 1 : Proeccess name was setting as field name in windows button so removed name and set only description and help ////

            // Removed This
            /* SET Name = (SELECT p.Name FROM AD_Process p, AD_Column c WHERE p.AD_Process_ID=c.AD_Process_ID
                                AND c.AD_Column_ID=f.AD_Column_ID),
             */ 


            // Commented 
            //sql = "	UPDATE AD_Field f"
            //        + " SET Description = (SELECT p.Description FROM AD_Process p, AD_Column c WHERE p.AD_Process_ID=c.AD_Process_ID"
            //        + " 			AND c.AD_Column_ID=f.AD_Column_ID),"
            //        + " 	Help = (SELECT p.Help FROM AD_Process p, AD_Column c WHERE p.AD_Process_ID=c.AD_Process_ID"
            //        + " 			AND c.AD_Column_ID=f.AD_Column_ID),"
            //        + " 	Updated = CURRENT_TIMESTAMP"
            //        + " WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y' "
            //        + "   AND EXISTS (SELECT * FROM AD_Process p, AD_Column c"
            //        + " 			WHERE c.AD_Process_ID=p.AD_Process_ID AND f.AD_Column_ID=c.AD_Column_ID"
            //        + " 			AND (f.Name<>p.Name OR NVL(f.Description,' ')<>NVL(p.Description,' ') OR NVL(f.Help,' ')<>NVL(p.Help,' ')))";
            //Execute("Synchronize Field from Process", sql, "  rows updated: ");

            /*
            --	Field Translations from Process
            DBMS_OUTPUT.PUT_LINE('Synchronize Field Trl from Process Trl');
            UPDATE AD_Field_trl trl
                SET Name = (SELECT p.Name FROM AD_Process_trl p, AD_Column c, AD_Field f 
                            WHERE p.AD_Process_ID=c.AD_Process_ID AND c.AD_Column_ID=f.AD_Column_ID
                            AND f.AD_Field_ID=trl.AD_Field_ID AND p.AD_Language=trl.AD_Language),
                    Description = (SELECT p.Description FROM AD_Process_trl p, AD_Column c, AD_Field f 
                            WHERE p.AD_Process_ID=c.AD_Process_ID AND c.AD_Column_ID=f.AD_Column_ID
                            AND f.AD_Field_ID=trl.AD_Field_ID AND p.AD_Language=trl.AD_Language),
                    Help = (SELECT p.Help FROM AD_Process_trl p, AD_Column c, AD_Field f 
                            WHERE p.AD_Process_ID=c.AD_Process_ID AND c.AD_Column_ID=f.AD_Column_ID
                            AND f.AD_Field_ID=trl.AD_Field_ID AND p.AD_Language=trl.AD_Language),
                    IsTranslated = (SELECT p.IsTranslated FROM AD_Process_trl p, AD_Column c, AD_Field f 
                            WHERE p.AD_Process_ID=c.AD_Process_ID AND c.AD_Column_ID=f.AD_Column_ID
                            AND f.AD_Field_ID=trl.AD_Field_ID AND p.AD_Language=trl.AD_Language),
                    Updated = SysDate
            WHERE EXISTS (SELECT * FROM AD_Process_Trl p, AD_Column c, AD_Field f
                        WHERE c.AD_Process_ID=p.AD_Process_ID AND f.AD_Column_ID=c.AD_Column_ID
                        AND f.AD_Field_ID=trl.AD_Field_ID AND p.AD_Language=trl.AD_Language
                        AND f.IsCentrallyMaintained='Y' AND f.IsActive='Y'
                        AND (trl.Name<>p.Name OR NVL(trl.Description,' ')<>NVL(p.Description,' ') OR NVL(trl.Help,' ')<>NVL(p.Help,' ')));
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */

            // Refer to Note 1 above


            sql = "UPDATE AD_Field_trl trl"
                    + " SET Description = (SELECT p.Description FROM AD_Process_trl p, AD_Column c, AD_Field f "
                    + " 			WHERE p.AD_Process_ID=c.AD_Process_ID AND c.AD_Column_ID=f.AD_Column_ID"
                    + " 			AND f.AD_Field_ID=trl.AD_Field_ID AND p.AD_Language=trl.AD_Language),"
                    + " 	Help = (SELECT p.Help FROM AD_Process_trl p, AD_Column c, AD_Field f "
                    + " 			WHERE p.AD_Process_ID=c.AD_Process_ID AND c.AD_Column_ID=f.AD_Column_ID"
                    + " 			AND f.AD_Field_ID=trl.AD_Field_ID AND p.AD_Language=trl.AD_Language),"
                    + " 	IsTranslated = (SELECT p.IsTranslated FROM AD_Process_trl p, AD_Column c, AD_Field f "
                    + " 			WHERE p.AD_Process_ID=c.AD_Process_ID AND c.AD_Column_ID=f.AD_Column_ID"
                    + " 			AND f.AD_Field_ID=trl.AD_Field_ID AND p.AD_Language=trl.AD_Language),"
                    + " 	Updated = CURRENT_TIMESTAMP"
                    + " WHERE EXISTS (SELECT * FROM AD_Process_Trl p, AD_Column c, AD_Field f"
                    + " 		WHERE c.AD_Process_ID=p.AD_Process_ID AND f.AD_Column_ID=c.AD_Column_ID"
                    + " 		AND f.AD_Field_ID=trl.AD_Field_ID AND p.AD_Language=trl.AD_Language"
                    + " 		AND f.IsCentrallyMaintained='Y' AND f.IsActive='Y'"
                    + " 		AND (trl.Name<>p.Name OR NVL(trl.Description,' ')<>NVL(p.Description,' ') OR NVL(trl.Help,' ')<>NVL(p.Help,' ')))";
            Execute("Synchronize Field Translations", sql, "  rows updated: ");

            /*
            --	Sync Parameter ColumnName
            UPDATE	AD_Process_Para f
                SET	ColumnName = (SELECT e.ColumnName FROM AD_Element e
                            WHERE UPPER(e.ColumnName)=UPPER(f.ColumnName))
            WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y'
              AND EXISTS (SELECT * FROM AD_Element e
                WHERE UPPER(e.ColumnName)=UPPER(f.ColumnName)
                AND e.ColumnName<>f.ColumnName);
            */
            sql = "	UPDATE AD_Process_Para f " +
                    " SET ColumnName = (SELECT e.ColumnName FROM AD_Element e " +
                    " WHERE UPPER(e.ColumnName)=UPPER(f.ColumnName)) " +
                    " WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y' " +
                    " AND EXISTS (SELECT * FROM AD_Element e " +
                    "  WHERE UPPER(e.ColumnName)=UPPER(f.ColumnName) " +
                    "  AND e.ColumnName<>f.ColumnName) ";
            Execute("Synchronize Parameter ColumnName", sql, "  rows updated: ");

            /*


            --	Paramenter Fields
            UPDATE	AD_Process_Para p
              SET	IsCentrallyMaintained = 'N'
            WHERE	IsCentrallyMaintained <> 'N'
              AND NOT EXISTS (SELECT * FROM AD_Element e WHERE p.ColumnName=e.ColumnName); 
            */
            sql = "UPDATE AD_Process_Para p " +
                    " SET IsCentrallyMaintained = 'N' " +
                    " WHERE IsCentrallyMaintained <> 'N' " +
                    " AND NOT EXISTS (SELECT * FROM AD_Element e WHERE p.ColumnName=e.ColumnName) ";
            Execute("Synchronize Paramenter Fields", sql, "  rows updated: ");

            /*
            --	Parameter Fields
            DBMS_OUTPUT.PUT_LINE('Synchronize Process Parameter');
            UPDATE AD_Process_Para f
                SET Name = (SELECT e.Name FROM AD_Element e
                            WHERE e.ColumnName=f.ColumnName),
                    Description = (SELECT e.Description FROM AD_Element e
                            WHERE e.ColumnName=f.ColumnName),
                    Help = (SELECT e.Help FROM AD_Element e
                            WHERE e.ColumnName=f.ColumnName),
                    Updated = SysDate
            WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y'
              AND EXISTS (SELECT * FROM AD_Element e
                        WHERE e.ColumnName=f.ColumnName
                          AND (f.Name <> e.Name OR NVL(f.Description,' ') <> NVL(e.Description,' ') OR NVL(f.Help,' ') <> NVL(e.Help,' ')));
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            //TODO process, element, ctx relationships???
            sql = "	UPDATE AD_Process_Para f " +
                    " SET Name = (SELECT e.Name FROM AD_Element e " +
                    " WHERE e.ColumnName=f.ColumnName)," +
                    " Description = (SELECT e.Description FROM AD_Element e " +
                    " WHERE e.ColumnName=f.ColumnName), " +
                    " Help = (SELECT e.Help FROM AD_Element e " +
                    " WHERE e.ColumnName=f.ColumnName)," +
                    " Updated = CURRENT_TIMESTAMP" +
                    " WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y'" +
                    "   AND EXISTS (SELECT * FROM AD_Element e " +
                    " 	  WHERE e.ColumnName=f.ColumnName " +
                    "     AND (f.Name <> e.Name OR NVL(f.Description,' ') <> NVL(e.Description,' ') OR NVL(f.Help,' ') <> NVL(e.Help,' '))) " +
                    " AND NOT EXISTS (" +
                    "   SELECT * FROM AD_Process p, AD_Element e, AD_ElementCTX ec " +
                    "   WHERE p.AD_Process_ID=f.AD_Process_ID " +
                    "   AND e.ColumnName=f.ColumnName " +
                    "   AND ec.AD_Element_ID=e.AD_Element_ID " +
                    "   AND ec.AD_CtxArea_ID=p.AD_CtxArea_ID) ";
            Execute("Synchronize Process Parameter with Element", sql, "  rows updated: ");

            sql = "	UPDATE AD_Process_Para f " +
                    " SET Name = (SELECT e.Name FROM AD_ElementCTX e " +
                    "				JOIN AD_Element el ON (e.AD_Element_ID=el.AD_Element_ID) " +
                    "				JOIN AD_Process p ON (p.AD_CtxArea_ID=e.AD_CtxArea_ID) " +
                    "   			WHERE el.ColumnName=f.ColumnName AND " +
                    "   	 			p.AD_Process_ID=f.AD_Process_ID)," +
                    " Description = (SELECT e.Description FROM AD_ElementCTX e " +
                    "				JOIN AD_Element el ON (e.AD_Element_ID=el.AD_Element_ID) " +
                    "				JOIN AD_Process p ON (p.AD_CtxArea_ID=e.AD_CtxArea_ID) " +
                    "   			WHERE el.ColumnName=f.ColumnName AND " +
                    "   	 			p.AD_Process_ID=f.AD_Process_ID)," +
                    " Help = (SELECT e.Help FROM AD_ElementCTX e " +
                    "				JOIN AD_Element el ON (e.AD_Element_ID=el.AD_Element_ID) " +
                    "				JOIN AD_Process p ON (p.AD_CtxArea_ID=e.AD_CtxArea_ID) " +
                    "   			WHERE el.ColumnName=f.ColumnName AND " +
                    "   	 			p.AD_Process_ID=f.AD_Process_ID)," +
                    " Updated = CURRENT_TIMESTAMP" +
                    " WHERE f.IsCentrallyMaintained='Y' AND f.IsActive='Y'" +
                    "   AND EXISTS (SELECT * FROM AD_Process p, AD_Element e, AD_ElementCTX ec " +
                    " 	  WHERE e.ColumnName=f.ColumnName " +
                    "     AND (f.Name <> e.Name OR NVL(f.Description,' ') <> NVL(e.Description,' ') OR NVL(f.Help,' ') <> NVL(e.Help,' ')) " +
                    "     AND p.AD_Process_ID=f.AD_Process_ID " +
                    "     AND ec.AD_Element_ID=e.AD_Element_ID " +
                    "     AND ec.AD_CtxArea_ID=p.AD_CtxArea_ID) ";

            Execute("Synchronize Process Parameter with ElementCTX", sql, "  rows updated: ");

            /*

            --	Parameter Translations
            DBMS_OUTPUT.PUT_LINE('Synchronize Process Parameter Trl');
            UPDATE AD_Process_Para_Trl trl
                SET Name = (SELECT et.Name FROM AD_Element_Trl et, AD_Element e, AD_Process_Para f
                            WHERE et.AD_Language=trl.AD_Language AND et.AD_Element_ID=e.AD_Element_ID
                              AND e.ColumnName=f.ColumnName AND f.AD_Process_Para_ID=trl.AD_Process_Para_ID),
                    Description = (SELECT et.Description FROM AD_Element_Trl et, AD_Element e, AD_Process_Para f
                            WHERE et.AD_Language=trl.AD_Language AND et.AD_Element_ID=e.AD_Element_ID
                              AND e.ColumnName=f.ColumnName AND f.AD_Process_Para_ID=trl.AD_Process_Para_ID),
                    Help = (SELECT et.Help FROM AD_Element_Trl et, AD_Element e, AD_Process_Para f
                            WHERE et.AD_Language=trl.AD_Language AND et.AD_Element_ID=e.AD_Element_ID
                              AND e.ColumnName=f.ColumnName AND f.AD_Process_Para_ID=trl.AD_Process_Para_ID),
                    IsTranslated = (SELECT et.IsTranslated FROM AD_Element_Trl et, AD_Element e, AD_Process_Para f
                            WHERE et.AD_Language=trl.AD_Language AND et.AD_Element_ID=e.AD_Element_ID
                              AND e.ColumnName=f.ColumnName AND f.AD_Process_Para_ID=trl.AD_Process_Para_ID),
                    Updated = SysDate
            WHERE EXISTS (SELECT * FROM AD_Element_Trl et, AD_Element e, AD_Process_Para f
                            WHERE et.AD_Language=trl.AD_Language AND et.AD_Element_ID=e.AD_Element_ID
                              AND e.ColumnName=f.ColumnName AND f.AD_Process_Para_ID=trl.AD_Process_Para_ID
                              AND f.IsCentrallyMaintained='Y' AND f.IsActive='Y'
                              AND (trl.Name <> et.Name OR NVL(trl.Description,' ') <> NVL(et.Description,' ') OR NVL(trl.Help,' ') <> NVL(et.Help,' ')));
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_Process_Para_Trl trl " +
                    " SET Name = (SELECT et.Name FROM AD_Element_Trl et INNER JOIN  AD_Element e ON (et.AD_Element_ID = e.AD_Element_ID) INNER JOIN AD_Process_Para f ON (f.AD_Element_ID = e.AD_Element_ID) " +
                    " 	  WHERE et.AD_Language=trl.AD_Language AND et.AD_Element_ID=e.AD_Element_ID " +
                    "     AND e.ColumnName=f.ColumnName AND f.AD_Process_Para_ID=trl.AD_Process_Para_ID), " +
                    "   Description = (SELECT et.Description FROM  AD_Element_Trl et INNER JOIN  AD_Element e ON (et.AD_Element_ID = e.AD_Element_ID) INNER JOIN AD_Process_Para f ON (f.AD_Element_ID = e.AD_Element_ID) " +
                    " 	  WHERE et.AD_Language=trl.AD_Language AND et.AD_Element_ID=e.AD_Element_ID " +
                    "     AND e.ColumnName=f.ColumnName AND f.AD_Process_Para_ID=trl.AD_Process_Para_ID), " +
                    " 	Help = (SELECT et.Help FROM  AD_Element_Trl et INNER JOIN  AD_Element e ON (et.AD_Element_ID = e.AD_Element_ID) INNER JOIN AD_Process_Para f ON (f.AD_Element_ID = e.AD_Element_ID) " +
                    " 	  WHERE et.AD_Language=trl.AD_Language AND et.AD_Element_ID=e.AD_Element_ID " +
                    " 	  AND e.ColumnName=f.ColumnName AND f.AD_Process_Para_ID=trl.AD_Process_Para_ID), " +
                    " 	IsTranslated = (SELECT et.IsTranslated FROM  AD_Element_Trl et INNER JOIN  AD_Element e ON (et.AD_Element_ID = e.AD_Element_ID) INNER JOIN AD_Process_Para f ON (f.AD_Element_ID = e.AD_Element_ID) " +
                    " 	  WHERE et.AD_Language=trl.AD_Language AND et.AD_Element_ID=e.AD_Element_ID " +
                    "     AND e.ColumnName=f.ColumnName AND f.AD_Process_Para_ID=trl.AD_Process_Para_ID), " +
                    " 	Updated = CURRENT_TIMESTAMP " +
                    " 	WHERE EXISTS (SELECT * FROM  AD_Element_Trl et INNER JOIN  AD_Element e ON (et.AD_Element_ID = e.AD_Element_ID) INNER JOIN AD_Process_Para f ON (f.AD_Element_ID = e.AD_Element_ID) " +
                    " 	      WHERE et.AD_Language=trl.AD_Language AND et.AD_Element_ID=e.AD_Element_ID " +
                    " 		  AND e.ColumnName=f.ColumnName AND f.AD_Process_Para_ID=trl.AD_Process_Para_ID " +
                    " 		  AND f.IsCentrallyMaintained='Y' AND f.IsActive='Y' " +
                    " 		  AND (trl.Name <> et.Name OR NVL(trl.Description,' ') <> NVL(et.Description,' ') OR NVL(trl.Help,' ') <> NVL(et.Help,' '))) " +
                    "  	AND NOT EXISTS (" +
                    "     SELECT * " +
                    "     FROM AD_Process_Para f, AD_Process p, AD_Element e, AD_ElementCtx ec " +
                    "     WHERE f.AD_Process_Para_ID=trl.AD_Process_Para_ID " +
                    "     AND p.AD_Process_ID=f.AD_Process_ID " +
                    "     AND e.ColumnName=f.ColumnName " +
                    "     AND ec.AD_Element_ID=e.AD_Element_ID " +
                    "     AND ec.AD_CtxArea_ID=p.AD_CtxArea_ID) ";
            Execute("Synchronize Process Parameter Trl with Element Trl", sql, "  rows updated: ");


            sql = "	UPDATE AD_Process_Para_Trl trl " +
                    " SET Name = (SELECT et.Name FROM AD_ElementCTX_Trl et, AD_ElementCTX ec, AD_Element e, AD_Process_Para f, AD_Process p " +
                            " WHERE et.AD_Language=trl.AD_Language AND ec.AD_Element_ID=e.AD_Element_ID AND et.AD_ElementCTX_ID=ec.AD_ElementCTX_ID " +
                            " AND e.ColumnName=f.ColumnName AND f.AD_Process_Para_ID=trl.AD_Process_Para_ID " +
                            "AND p.AD_CtxArea_ID=ec.AD_CtxArea_ID AND p.AD_Process_ID=f.AD_Process_ID), " +
                    "   Description = (SELECT et.Description FROM AD_ElementCTX_Trl et, AD_ElementCTX ec, AD_Element e, AD_Process_Para f, AD_Process p " +
                            " WHERE et.AD_Language=trl.AD_Language AND ec.AD_Element_ID=e.AD_Element_ID AND et.AD_ElementCTX_ID=ec.AD_ElementCTX_ID " +
                            " AND e.ColumnName=f.ColumnName AND f.AD_Process_Para_ID=trl.AD_Process_Para_ID " +
                            "AND p.AD_CtxArea_ID=ec.AD_CtxArea_ID AND p.AD_Process_ID=f.AD_Process_ID), " +
                    " 	Help = (SELECT et.Help FROM AD_ElementCTX_Trl et, AD_ElementCTX ec, AD_Element e, AD_Process_Para f, AD_Process p " +
                            " WHERE et.AD_Language=trl.AD_Language AND ec.AD_Element_ID=e.AD_Element_ID AND et.AD_ElementCTX_ID=ec.AD_ElementCTX_ID " +
                            " AND e.ColumnName=f.ColumnName AND f.AD_Process_Para_ID=trl.AD_Process_Para_ID " +
                            "AND p.AD_CtxArea_ID=ec.AD_CtxArea_ID AND p.AD_Process_ID=f.AD_Process_ID), " +
                    " 	IsTranslated = (SELECT et.IsTranslated FROM AD_ElementCTX_Trl et, AD_ElementCTX ec, AD_Element e, AD_Process_Para f, AD_Process p " +
                            " WHERE et.AD_Language=trl.AD_Language AND ec.AD_Element_ID=e.AD_Element_ID AND et.AD_ElementCTX_ID=ec.AD_ElementCTX_ID " +
                            " AND e.ColumnName=f.ColumnName AND f.AD_Process_Para_ID=trl.AD_Process_Para_ID " +
                            "AND p.AD_CtxArea_ID=ec.AD_CtxArea_ID AND p.AD_Process_ID=f.AD_Process_ID), " +
                    " 	Updated = CURRENT_TIMESTAMP " +
                    " 	WHERE EXISTS (SELECT * FROM AD_ElementCTX_Trl et, AD_ElementCTX ec, AD_Element e, AD_Process_Para f, AD_Process p " +
                    " 	      WHERE et.AD_Language=trl.AD_Language AND ec.AD_Element_ID=e.AD_Element_ID " +
                    "         AND et.AD_ElementCTX_ID=ec.AD_ElementCTX_ID " +
                    " 		  AND e.ColumnName=f.ColumnName AND f.AD_Process_Para_ID=trl.AD_Process_Para_ID " +
                    " 		  AND p.AD_Process_ID=f.AD_Process_ID AND trl.AD_Process_Para_ID=f.AD_Process_Para_ID " +
                    " 		  AND p.AD_CtxArea_ID IS NOT NULL AND p.AD_CtxArea_ID=ec.AD_CtxArea_ID " +
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
                              AND trl.AD_Language=t.AD_Language),
                    Description = (SELECT t.Description FROM AD_Window_trl t, AD_WF_Node n
                            WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Window_ID=t.AD_Window_ID
                              AND trl.AD_Language=t.AD_Language),
                    Help = (SELECT t.Help FROM AD_Window_trl t, AD_WF_Node n
                            WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Window_ID=t.AD_Window_ID
                              AND trl.AD_Language=t.AD_Language)
            WHERE EXISTS (SELECT * FROM AD_Window_Trl t, AD_WF_Node n
                        WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Window_ID=t.AD_Window_ID
                          AND trl.AD_Language=t.AD_Language AND n.IsCentrallyMaintained='Y' AND n.IsActive='Y'
                          AND (trl.Name <> t.Name OR NVL(trl.Description,' ') <> NVL(t.Description,' ') OR NVL(trl.Help,' ') <> NVL(t.Help,' ')));
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_WF_Node_Trl trl " +
                    " SET Name = (SELECT t.Name FROM AD_Window_trl t, AD_WF_Node n " +
                    " 	WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Window_ID=t.AD_Window_ID " +
                    " 		  AND trl.AD_Language=t.AD_Language), " +
                    " Description = (SELECT t.Description FROM AD_Window_trl t, AD_WF_Node n " +
                    " 	WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Window_ID=t.AD_Window_ID " +
                    " 		  AND trl.AD_Language=t.AD_Language), " +
                    " Help = (SELECT t.Help FROM AD_Window_trl t, AD_WF_Node n " +
                    " 	WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Window_ID=t.AD_Window_ID " +
                    " 		  AND trl.AD_Language=t.AD_Language) " +
                    " WHERE EXISTS (SELECT * FROM AD_Window_Trl t, AD_WF_Node n " +
                    " 	WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Window_ID=t.AD_Window_ID " +
                    " 		  AND trl.AD_Language=t.AD_Language AND n.IsCentrallyMaintained='Y' AND n.IsActive='Y' " +
                    " 		  AND (trl.Name <> t.Name OR NVL(trl.Description,' ') <> NVL(t.Description,' ') OR NVL(trl.Help,' ') <> NVL(t.Help,' '))) ";
            Execute("Synchronize Workflow Node Trl from Window Trl", sql, "  rows updated: ");

            /*

            --	Workflow Node - Form
            DBMS_OUTPUT.PUT_LINE('Synchronize Workflow Node from Form');
            UPDATE AD_WF_Node n
                SET (Name, Description, Help) = (SELECT f.Name, f.Description, f.Help 
                        FROM AD_Form f
                        WHERE f.AD_Form_ID=n.AD_Form_ID)
            WHERE n.IsCentrallyMaintained = 'Y'
              AND EXISTS  (SELECT * FROM AD_Form f
                        WHERE f.AD_Form_ID=n.AD_Form_ID
                          AND (f.Name <> n.Name OR NVL(f.Description,' ') <> NVL(n.Description,' ') OR NVL(f.Help,' ') <> NVL(n.Help,' ')));
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_WF_Node n" +
                    " SET Name = (SELECT coalesce(f.DisplayName,f.Name) " +
                    "   FROM AD_Form f " +
                    " 	WHERE f.AD_Form_ID=n.AD_Form_ID) " +
                    " , Description = (SELECT  f.Description " +
                    "   FROM AD_Form f " +
                    " 	WHERE f.AD_Form_ID=n.AD_Form_ID) " +
                    " ,  Help = (SELECT f.Help " +
                    "   FROM AD_Form f" +
                    " 	WHERE f.AD_Form_ID=n.AD_Form_ID) " +
                    " WHERE n.IsCentrallyMaintained = 'Y' " +
                    "   AND EXISTS  (SELECT * FROM AD_Form f " +
                    " 			WHERE f.AD_Form_ID=n.AD_Form_ID " +
                    " 			  AND (f.Name <> n.Name OR NVL(f.Description,' ') <> NVL(n.Description,' ') OR NVL(f.Help,' ') <> NVL(n.Help,' '))) ";
            Execute("Synchronize Workflow Node from Form", sql, "  rows updated: ");

            /*

            --	Workflow Translations - Form
            DBMS_OUTPUT.PUT_LINE('Synchronize Workflow Node Trl from Form Trl');
            UPDATE AD_WF_Node_Trl trl
                SET (Name, Description, Help) = (SELECT t.Name, t.Description, t.Help
                    FROM AD_Form_trl t, AD_WF_Node n
                    WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Form_ID=t.AD_Form_ID
                      AND trl.AD_Language=t.AD_Language)
            WHERE EXISTS (SELECT * FROM AD_Form_Trl t, AD_WF_Node n
                        WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Form_ID=t.AD_Form_ID
                          AND trl.AD_Language=t.AD_Language AND n.IsCentrallyMaintained='Y' AND n.IsActive='Y'
                          AND (trl.Name <> t.Name OR NVL(trl.Description,' ') <> NVL(t.Description,' ') OR NVL(trl.Help,' ') <> NVL(t.Help,' ')));
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_WF_Node_Trl trl " +
                    " SET Name = (SELECT t.Name " +
                    "   FROM AD_Form_trl t, AD_WF_Node n " +
                    " 	WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Form_ID=t.AD_Form_ID " +
                    " 	  AND trl.AD_Language=t.AD_Language) " +
                     " ,  Description = (SELECT  t.Description " +
                    "   FROM AD_Form_trl t, AD_WF_Node n " +
                    " 	WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Form_ID=t.AD_Form_ID " +
                    " 	  AND trl.AD_Language=t.AD_Language) " +
                     " ,  Help = (SELECT  t.Help " +
                    "   FROM AD_Form_trl t, AD_WF_Node n " +
                    " 	WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Form_ID=t.AD_Form_ID " +
                    " 	  AND trl.AD_Language=t.AD_Language) " +
                    " WHERE EXISTS (SELECT * FROM AD_Form_Trl t, AD_WF_Node n " +
                    " 	WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Form_ID=t.AD_Form_ID " +
                    " 	  AND trl.AD_Language=t.AD_Language AND n.IsCentrallyMaintained='Y' AND n.IsActive='Y' " +
                    " 	  AND (trl.Name <> t.Name OR NVL(trl.Description,' ') <> NVL(t.Description,' ') OR NVL(trl.Help,' ') <> NVL(t.Help,' '))) ";
            Execute("Synchronize Workflow Node Trl from Form Trl", sql, "  rows updated: ");

            /*

            --	Workflow Node - Report
            DBMS_OUTPUT.PUT_LINE('Synchronize Workflow Node from Process');
            UPDATE AD_WF_Node n
                SET (Name, Description, Help) = (SELECT f.Name, f.Description, f.Help 
                        FROM AD_Process f
                        WHERE f.AD_Process_ID=n.AD_Process_ID)
            WHERE n.IsCentrallyMaintained = 'Y'
              AND EXISTS  (SELECT * FROM AD_Process f
                        WHERE f.AD_Process_ID=n.AD_Process_ID
                          AND (f.Name <> n.Name OR NVL(f.Description,' ') <> NVL(n.Description,' ') OR NVL(f.Help,' ') <> NVL(n.Help,' ')));
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_WF_Node n " +
                    " SET Name = (SELECT f.Name " +
                    " 	FROM AD_Process f " +
                    " 	WHERE f.AD_Process_ID=n.AD_Process_ID) " +
                    " ,  Description = (SELECT f.Description " +
                    " 	FROM AD_Process f " +
                    " 	WHERE f.AD_Process_ID=n.AD_Process_ID) " +
                    " ,  Help = (SELECT  f.Help " +
                    " 	FROM AD_Process f " +
                    " 	WHERE f.AD_Process_ID=n.AD_Process_ID) " +
                    " WHERE n.IsCentrallyMaintained = 'Y' " +
                    "   AND EXISTS  (SELECT * FROM AD_Process f " +
                    " 		WHERE f.AD_Process_ID=n.AD_Process_ID " +
                    " 		  AND (f.Name <> n.Name OR NVL(f.Description,' ') <> NVL(n.Description,' ') OR NVL(f.Help,' ') <> NVL(n.Help,' '))) ";
            Execute("Synchronize Workflow Node from Process", sql, "  rows updated: ");

            /*

            --	Workflow Translations - Form
            DBMS_OUTPUT.PUT_LINE('Synchronize Workflow Node Trl from Process Trl');
            UPDATE AD_WF_Node_Trl trl
                SET (Name, Description, Help) = (SELECT t.Name, t.Description, t.Help
                    FROM AD_Process_trl t, AD_WF_Node n
                    WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Process_ID=t.AD_Process_ID
                      AND trl.AD_Language=t.AD_Language)
            WHERE EXISTS (SELECT * FROM AD_Process_Trl t, AD_WF_Node n
                        WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Process_ID=t.AD_Process_ID
                          AND trl.AD_Language=t.AD_Language AND n.IsCentrallyMaintained='Y' AND n.IsActive='Y'
                          AND (trl.Name <> t.Name OR NVL(trl.Description,' ') <> NVL(t.Description,' ') OR NVL(trl.Help,' ') <> NVL(t.Help,' ')));
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_WF_Node_Trl trl " +
                    " SET Name = (SELECT t.Name " +
                    " 		FROM AD_Process_trl t, AD_WF_Node n " +
                    " 		WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Process_ID=t.AD_Process_ID " +
                    " 		  AND trl.AD_Language=t.AD_Language) " +
                    " ,  Description = (SELECT  t.Description " +
                    " 		FROM AD_Process_trl t, AD_WF_Node n " +
                    " 		WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Process_ID=t.AD_Process_ID " +
                    " 		  AND trl.AD_Language=t.AD_Language) " +
                    " ,  Help= (SELECT  t.Help " +
                    " 		FROM AD_Process_trl t, AD_WF_Node n " +
                    " 		WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Process_ID=t.AD_Process_ID " +
                    " 		  AND trl.AD_Language=t.AD_Language) " +
                    " WHERE EXISTS (SELECT * FROM AD_Process_Trl t, AD_WF_Node n " +
                    " 		WHERE trl.AD_WF_Node_ID=n.AD_WF_Node_ID AND n.AD_Process_ID=t.AD_Process_ID " +
                    " 		  AND trl.AD_Language=t.AD_Language AND n.IsCentrallyMaintained='Y' AND n.IsActive='Y' " +
                    " 		  AND (trl.Name <> t.Name OR NVL(trl.Description,' ') <> NVL(t.Description,' ') OR NVL(trl.Help,' ') <> NVL(t.Help,' '))) ";
            Execute("Synchronize Workflow Node Trl from Process Trl", sql, "  rows updated: ");

            /*

            --  Need centrally maintained flag here!
            DBMS_OUTPUT.PUT_LINE('Synchronize PrintFormatItem Name from Element');
            UPDATE AD_PrintFormatItem pfi
              SET Name = (SELECT e.Name 
                FROM AD_Element e, AD_Column c
                WHERE e.AD_Element_ID=c.AD_Element_ID
                  AND c.AD_Column_ID=pfi.AD_Column_ID)
            WHERE pfi.IsCentrallyMaintained='Y'
              AND EXISTS (SELECT * 
                FROM AD_Element e, AD_Column c
                WHERE e.AD_Element_ID=c.AD_Element_ID
                  AND c.AD_Column_ID=pfi.AD_Column_ID
                  AND e.Name<>pfi.Name)
              AND EXISTS (SELECT * FROM AD_Client 
                WHERE AD_Client_ID=pfi.AD_Client_ID AND IsMultiLingualDocument='Y');
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_PrintFormatItem pfi " +
                    "	  SET Name = (SELECT e.Name " +
                    "		FROM AD_Element e, AD_Column c " +
                    "		WHERE e.AD_Element_ID=c.AD_Element_ID " +
                    "		  AND c.AD_Column_ID=pfi.AD_Column_ID) " +
                    "	WHERE pfi.IsCentrallyMaintained='Y' " +
                    "      AND EXISTS (SELECT * " +
                    "		FROM AD_Element e, AD_Column c " +
                    "		WHERE e.AD_Element_ID=c.AD_Element_ID " +
                    "		  AND c.AD_Column_ID=pfi.AD_Column_ID " +
                    "		  AND e.Name<>pfi.Name) " +
                    "	  AND EXISTS (SELECT * FROM AD_Client " +
                    "		WHERE AD_Client_ID=pfi.AD_Client_ID AND IsMultiLingualDocument='Y') ";
            Execute("Synchronize PrintFormatItem Name from Element", sql, "  rows updated: ");

            /*

            DBMS_OUTPUT.PUT_LINE('Synchronize PrintFormatItem PrintName from Element');
            UPDATE AD_PrintFormatItem pfi
              SET PrintName = (SELECT e.PrintName 
                FROM AD_Element e, AD_Column c
                WHERE e.AD_Element_ID=c.AD_Element_ID
                  AND c.AD_Column_ID=pfi.AD_Column_ID)
            WHERE pfi.IsCentrallyMaintained='Y'
              AND EXISTS (SELECT * 
                FROM AD_Element e, AD_Column c, AD_PrintFormat pf
                WHERE e.AD_Element_ID=c.AD_Element_ID
                  AND c.AD_Column_ID=pfi.AD_Column_ID
                  AND LENGTH(pfi.PrintName) > 0
                  AND e.PrintName<>pfi.PrintName
                  AND pf.AD_PrintFormat_ID=pfi.AD_PrintFormat_ID
                  AND pf.IsForm='N' AND IsTableBased='Y')
              AND EXISTS (SELECT * FROM AD_Client 
                WHERE AD_Client_ID=pfi.AD_Client_ID AND IsMultiLingualDocument='Y');
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_PrintFormatItem pfi " +
                    "	  SET PrintName = (SELECT e.PrintName " +
                    "		FROM AD_Element e, AD_Column c " +
                    "		WHERE e.AD_Element_ID=c.AD_Element_ID " +
                    "		  AND c.AD_Column_ID=pfi.AD_Column_ID) " +
                    "	WHERE pfi.IsCentrallyMaintained='Y' " +
                    "      AND EXISTS (SELECT * " +
                    "		FROM AD_Element e, AD_Column c, AD_PrintFormat pf " +
                    "		WHERE e.AD_Element_ID=c.AD_Element_ID " +
                    "		  AND c.AD_Column_ID=pfi.AD_Column_ID " +
                    "		  AND LENGTH(pfi.PrintName) > 0 " +
                    "		  AND e.PrintName<>pfi.PrintName " +
                    "		  AND pf.AD_PrintFormat_ID=pfi.AD_PrintFormat_ID " +
                    "		  AND pf.IsForm='N' AND IsTableBased='Y') " +
                    "	  AND EXISTS (SELECT * FROM AD_Client " +
                    "		WHERE AD_Client_ID=pfi.AD_Client_ID AND IsMultiLingualDocument='Y') ";
            Execute("Synchronize PrintFormatItem PrintName from Element", sql, "  rows updated: ");

            /*

            DBMS_OUTPUT.PUT_LINE('Synchronize PrintFormatItem Trl from Element Trl (Multi-Lingual)');
            UPDATE AD_PrintFormatItem_Trl trl
              SET PrintName = (SELECT e.PrintName 
                FROM AD_Element_Trl e, AD_Column c, AD_PrintFormatItem pfi
                WHERE e.AD_Language=trl.AD_Language
                  AND e.AD_Element_ID=c.AD_Element_ID
                  AND c.AD_Column_ID=pfi.AD_Column_ID
                  AND pfi.AD_PrintFormatItem_ID=trl.AD_PrintFormatItem_ID)
            WHERE EXISTS (SELECT * 
                FROM AD_Element_Trl e, AD_Column c, AD_PrintFormatItem pfi, AD_PrintFormat pf
                WHERE e.AD_Language=trl.AD_Language
                  AND e.AD_Element_ID=c.AD_Element_ID
                  AND c.AD_Column_ID=pfi.AD_Column_ID
                  AND pfi.AD_PrintFormatItem_ID=trl.AD_PrintFormatItem_ID
                  AND pfi.IsCentrallyMaintained='Y'
                  AND LENGTH(pfi.PrintName) > 0
                  AND (e.PrintName<>trl.PrintName OR trl.PrintName IS NULL)
                  AND pf.AD_PrintFormat_ID=pfi.AD_PrintFormat_ID 
                  AND pf.IsForm='N' AND IsTableBased='Y')
              AND EXISTS (SELECT * FROM AD_Client 
                WHERE AD_Client_ID=trl.AD_Client_ID AND IsMultiLingualDocument='Y');
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_PrintFormatItem_Trl trl " +
                    "	  SET PrintName = (SELECT e.PrintName " +
                    "		FROM AD_Element_Trl e, AD_Column c, AD_PrintFormatItem pfi " +
                    "		WHERE e.AD_Language=trl.AD_Language " +
                    "		  AND e.AD_Element_ID=c.AD_Element_ID " +
                    "		  AND c.AD_Column_ID=pfi.AD_Column_ID " +
                    "		  AND pfi.AD_PrintFormatItem_ID=trl.AD_PrintFormatItem_ID) " +
                    "	WHERE EXISTS (SELECT * " +
                    "		FROM AD_Element_Trl e, AD_Column c, AD_PrintFormatItem pfi, AD_PrintFormat pf " +
                    "		WHERE e.AD_Language=trl.AD_Language " +
                    "		  AND e.AD_Element_ID=c.AD_Element_ID " +
                    "		  AND c.AD_Column_ID=pfi.AD_Column_ID " +
                    "		  AND pfi.AD_PrintFormatItem_ID=trl.AD_PrintFormatItem_ID " +
                    "          AND pfi.IsCentrallyMaintained='Y' " +
                    "		  AND LENGTH(pfi.PrintName) > 0 " +
                    "		  AND (e.PrintName<>trl.PrintName OR trl.PrintName IS NULL) " +
                    "		  AND pf.AD_PrintFormat_ID=pfi.AD_PrintFormat_ID " +
                    "		  AND pf.IsForm='N' AND IsTableBased='Y') " +
                    "	  AND EXISTS (SELECT * FROM AD_Client " +
                    "		WHERE AD_Client_ID=trl.AD_Client_ID AND IsMultiLingualDocument='Y')";
            Execute("Synchronize PrintFormatItem Trl from Element Trl (Multi-Lingual)", sql, "  rows updated: ");

            /*

            DBMS_OUTPUT.PUT_LINE('Synchronize PrintFormatItem Trl (Not Multi-Lingual)');
            UPDATE AD_PrintFormatItem_Trl trl
              SET PrintName = (SELECT pfi.PrintName 
                FROM AD_PrintFormatItem pfi
                WHERE pfi.AD_PrintFormatItem_ID=trl.AD_PrintFormatItem_ID)
            WHERE EXISTS (SELECT * 
                FROM AD_PrintFormatItem pfi, AD_PrintFormat pf
                WHERE pfi.AD_PrintFormatItem_ID=trl.AD_PrintFormatItem_ID
                  AND pfi.IsCentrallyMaintained='Y'
                  AND LENGTH(pfi.PrintName) > 0
                  AND pfi.PrintName<>trl.PrintName
                  AND pf.AD_PrintFormat_ID=pfi.AD_PrintFormat_ID 
                  AND pf.IsForm='N' AND pf.IsTableBased='Y')
              AND EXISTS (SELECT * FROM AD_Client 
                WHERE AD_Client_ID=trl.AD_Client_ID AND IsMultiLingualDocument='N');
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_PrintFormatItem_Trl trl " +
                    "	  SET PrintName = (SELECT pfi.PrintName " +
                    "		FROM AD_PrintFormatItem pfi " +
                    "		WHERE pfi.AD_PrintFormatItem_ID=trl.AD_PrintFormatItem_ID) " +
                    "	WHERE EXISTS (SELECT * " +
                    "		FROM AD_PrintFormatItem pfi, AD_PrintFormat pf " +
                    "		WHERE pfi.AD_PrintFormatItem_ID=trl.AD_PrintFormatItem_ID " +
                    "          AND pfi.IsCentrallyMaintained='Y' " +
                    "		  AND LENGTH(pfi.PrintName) > 0 " +
                    "		  AND pfi.PrintName<>trl.PrintName " +
                    "		  AND pf.AD_PrintFormat_ID=pfi.AD_PrintFormat_ID " +
                    "		  AND pf.IsForm='N' AND pf.IsTableBased='Y') " +
                    "	  AND EXISTS (SELECT * FROM AD_Client " +
                    "		WHERE AD_Client_ID=trl.AD_Client_ID AND IsMultiLingualDocument='N')";
            Execute("Synchronize PrintFormatItem Trl (Not Multi-Lingual)", sql, "  rows updated: ");

            /*

            DBMS_OUTPUT.PUT_LINE('Reset PrintFormatItem Trl where not used in base table');
            UPDATE AD_PrintFormatItem_Trl trl
              SET PrintName = NULL
            WHERE PrintName IS NOT NULL
              AND EXISTS (SELECT *
                FROM AD_PrintFormatItem pfi
                WHERE pfi.AD_PrintFormatItem_ID=trl.AD_PrintFormatItem_ID 
                  AND pfi.IsCentrallyMaintained='Y'
                  AND (LENGTH (pfi.PrintName) = 0 OR pfi.PrintName IS NULL));
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_PrintFormatItem_Trl trl " +
                    "	  SET PrintName = NULL " +
                    "	WHERE PrintName IS NOT NULL " +
                    "	  AND EXISTS (SELECT * " +
                    "		FROM AD_PrintFormatItem pfi " +
                    "		WHERE pfi.AD_PrintFormatItem_ID=trl.AD_PrintFormatItem_ID " +
                    "          AND pfi.IsCentrallyMaintained='Y' " +
                    "		  AND (LENGTH (pfi.PrintName) = 0 OR pfi.PrintName IS NULL))";
            Execute("Synchronize PrintFormatItem Trl where not used in base table", sql, "  rows updated: ");

            /*

        **
        SELECT 	e.PrintName "Element", pfi.PrintName "FormatItem", trl.AD_Language, trl.PrintName "Trl"
        FROM 	AD_Element e
          INNER JOIN AD_Column c ON (e.AD_Element_ID=c.AD_Element_ID)
          INNER JOIN AD_PrintFormatItem pfi ON (c.AD_Column_ID=pfi.AD_Column_ID)
          INNER JOIN AD_PrintFormatItem_Trl trl ON (pfi.AD_PrintFormatItem_ID=trl.AD_PrintFormatItem_ID)
        WHERE pfi.AD_PrintFormatItem_ID=?
        **

            --	Sync Names - Window
            DBMS_OUTPUT.PUT_LINE('Synchronizing Menu with Window');
            UPDATE	AD_Menu m
            SET		Name = (SELECT Name FROM AD_Window w WHERE m.AD_Window_ID=w.AD_Window_ID),
                    Description = (SELECT Description FROM AD_Window w WHERE m.AD_Window_ID=w.AD_Window_ID)
            WHERE	AD_Window_ID IS NOT NULL
                AND Action = 'W';
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            executesql("Synchronize Field Translations", sql,  "  rows updated: ");
        **/
            sql = "UPDATE AD_Menu m "
                + "SET Name = (SELECT coalesce(w.DisplayName,w.Name) FROM AD_Window w WHERE m.AD_Window_ID=w.AD_Window_ID),"
                + "Description = (SELECT Description FROM AD_Window w WHERE m.AD_Window_ID=w.AD_Window_ID) "
                + "WHERE AD_Window_ID IS NOT NULL"
                + " AND Action = 'W'";
            Execute("Synchronize Menu with Window", sql, "  rows updated: ");

            /*
                UPDATE	AD_Menu_Trl mt
                SET		Name = (SELECT wt.Name FROM AD_Window_Trl wt, AD_Menu m 
                                WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Window_ID=wt.AD_Window_ID 
                                AND mt.AD_Language=wt.AD_Language),
                        Description = (SELECT wt.Description FROM AD_Window_Trl wt, AD_Menu m 
                                WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Window_ID=wt.AD_Window_ID 
                                AND mt.AD_Language=wt.AD_Language),
                        IsTranslated = (SELECT wt.IsTranslated FROM AD_Window_Trl wt, AD_Menu m 
                                WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Window_ID=wt.AD_Window_ID 
                                AND mt.AD_Language=wt.AD_Language)
                WHERE EXISTS (SELECT * FROM AD_Window_Trl wt, AD_Menu m 
                                WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Window_ID=wt.AD_Window_ID 
                                AND mt.AD_Language=wt.AD_Language
                                AND m.AD_Window_ID IS NOT NULL
                                AND m.Action = 'W');
                DBMS_OUTPUT.PUT_LINE('  trl rows updated: ' || SQL%ROWCOUNT);
                */
            sql = "	UPDATE AD_Menu_Trl mt " +
                    "	SET Name = (SELECT wt.Name FROM AD_Window_Trl wt, AD_Menu m " +
                    "					WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Window_ID=wt.AD_Window_ID " +
                    "					AND mt.AD_Language=wt.AD_Language), " +
                    "		Description = (SELECT wt.Description FROM AD_Window_Trl wt, AD_Menu m " +
                    "					WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Window_ID=wt.AD_Window_ID " +
                    "					AND mt.AD_Language=wt.AD_Language), " +
                    "			IsTranslated = (SELECT wt.IsTranslated FROM AD_Window_Trl wt, AD_Menu m " +
                    "					WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Window_ID=wt.AD_Window_ID " +
                    "					AND mt.AD_Language=wt.AD_Language) " +
                    "	WHERE EXISTS (SELECT * FROM AD_Window_Trl wt, AD_Menu m " +
                    "					WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Window_ID=wt.AD_Window_ID " +
                    "					AND mt.AD_Language=wt.AD_Language " +
                    "					AND m.AD_Window_ID IS NOT NULL " +
                    "					AND m.Action = 'W')";
            Execute("Synchronize Menu with Window Trl", sql, "  rows updated: ");

            /*

            --	Sync Names - Process
            DBMS_OUTPUT.PUT_LINE('Synchronizing Menu with Processes');
            UPDATE	AD_Menu m
            SET		Name = (SELECT p.Name FROM AD_Process p WHERE m.AD_Process_ID=p.AD_Process_ID),
                    Description = (SELECT p.Description FROM AD_Process p WHERE m.AD_Process_ID=p.AD_Process_ID)
            WHERE	m.AD_Process_ID IS NOT NULL
              AND	m.Action IN ('R', 'P');
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */



            // Commented 
            //sql = "	UPDATE	AD_Menu m " +
            //        "	SET		Name = (SELECT p.value FROM AD_Process p WHERE m.AD_Process_ID=p.AD_Process_ID), " +
            //        "			Description = (SELECT p.Description FROM AD_Process p WHERE m.AD_Process_ID=p.AD_Process_ID) " +
            //        "	WHERE	m.AD_Process_ID IS NOT NULL " +
            //        "	  AND	m.Action IN ('R', 'P')";
            //Execute("Synchronize Menu with Processes", sql, "  rows updated: ");

            // Commented 

            /*

            UPDATE	AD_Menu_Trl mt
            SET		Name = (SELECT pt.Name FROM AD_Process_Trl pt, AD_Menu m
                            WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Process_ID=pt.AD_Process_ID
                            AND mt.AD_Language=pt.AD_Language),
                    Description = (SELECT pt.Description FROM AD_Process_Trl pt, AD_Menu m
                            WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Process_ID=pt.AD_Process_ID
                            AND mt.AD_Language=pt.AD_Language),
                    IsTranslated = (SELECT pt.IsTranslated FROM AD_Process_Trl pt, AD_Menu m
                            WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Process_ID=pt.AD_Process_ID
                            AND mt.AD_Language=pt.AD_Language)
            WHERE EXISTS (SELECT * FROM AD_Process_Trl pt, AD_Menu m
                            WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Process_ID=pt.AD_Process_ID
                            AND mt.AD_Language=pt.AD_Language
                            AND m.AD_Process_ID IS NOT NULL
                            AND	Action IN ('R', 'P'));
            DBMS_OUTPUT.PUT_LINE('  trl rows updated: ' || SQL%ROWCOUNT);
            */


            //sql = "	UPDATE	AD_Menu_Trl mt " +
            //        "	SET		Name = (SELECT pt.Name FROM AD_Process_Trl pt, AD_Menu m " +
            //        "					WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Process_ID=pt.AD_Process_ID " +
            //        "					AND mt.AD_Language=pt.AD_Language), " +
            //        "			Description = (SELECT pt.Description FROM AD_Process_Trl pt, AD_Menu m " +
            //        "					WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Process_ID=pt.AD_Process_ID " +
            //        "					AND mt.AD_Language=pt.AD_Language), " +
            //        "			IsTranslated = (SELECT pt.IsTranslated FROM AD_Process_Trl pt, AD_Menu m " +
            //        "					WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Process_ID=pt.AD_Process_ID" +
            //        "					AND mt.AD_Language=pt.AD_Language)" +
            //        "	WHERE EXISTS (SELECT * FROM AD_Process_Trl pt, AD_Menu m" +
            //        "					WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Process_ID=pt.AD_Process_ID" +
            //        "					AND mt.AD_Language=pt.AD_Language" +
            //        "					AND m.AD_Process_ID IS NOT NULL" +
            //        "					AND	Action IN ('R', 'P'))";
            //Execute("Synchronize Menu with Processes Translations", sql, "  rows updated: ");

            /*

            --	Sync Names = Form
            DBMS_OUTPUT.PUT_LINE('Synchronizing Menu with Forms');
            UPDATE	AD_Menu m
            SET		Name = (SELECT Name FROM AD_Form f WHERE m.AD_Form_ID=f.AD_Form_ID),
                    Description = (SELECT Description FROM AD_Form f WHERE m.AD_Form_ID=f.AD_Form_ID)
            WHERE	AD_Form_ID IS NOT NULL
              AND	Action = 'X';
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_Menu m " +
                    "	SET Name = (SELECT coalesce(f.DisplayName,f.Name) FROM AD_Form f WHERE m.AD_Form_ID=f.AD_Form_ID), " +
                    "			Description = (SELECT Description FROM AD_Form f WHERE m.AD_Form_ID=f.AD_Form_ID) " +
                    "	WHERE AD_Form_ID IS NOT NULL " +
                    "	  AND Action = 'X'";
            Execute("Synchronize Menu with Forms", sql, "  rows updated: ");

            /*

            UPDATE	AD_Menu_Trl mt
            SET		Name = (SELECT ft.Name FROM AD_Form_Trl ft, AD_Menu m
                            WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Form_ID=ft.AD_Form_ID
                            AND mt.AD_Language=ft.AD_Language),
                    Description = (SELECT ft.Description FROM AD_Form_Trl ft, AD_Menu m
                            WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Form_ID=ft.AD_Form_ID
                            AND mt.AD_Language=ft.AD_Language),
                    IsTranslated = (SELECT ft.IsTranslated FROM AD_Form_Trl ft, AD_Menu m
                            WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Form_ID=ft.AD_Form_ID
                            AND mt.AD_Language=ft.AD_Language)
            WHERE EXISTS (SELECT * FROM AD_Form_Trl ft, AD_Menu m
                            WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Form_ID=ft.AD_Form_ID
                            AND mt.AD_Language=ft.AD_Language
                            AND m.AD_Form_ID IS NOT NULL
                            AND	Action = 'X');
            DBMS_OUTPUT.PUT_LINE('  trl rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "UPDATE AD_Menu_Trl mt " +
                    "	SET Name = (SELECT ft.Name FROM AD_Form_Trl ft, AD_Menu m " +
                    "					WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Form_ID=ft.AD_Form_ID " +
                    "					AND mt.AD_Language=ft.AD_Language), " +
                    "			Description = (SELECT ft.Description FROM AD_Form_Trl ft, AD_Menu m " +
                    "					WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Form_ID=ft.AD_Form_ID " +
                    "					AND mt.AD_Language=ft.AD_Language), " +
                    "			IsTranslated = (SELECT ft.IsTranslated FROM AD_Form_Trl ft, AD_Menu m " +
                    "					WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Form_ID=ft.AD_Form_ID " +
                    "					AND mt.AD_Language=ft.AD_Language) " +
                    "	WHERE EXISTS (SELECT * FROM AD_Form_Trl ft, AD_Menu m " +
                    "					WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Form_ID=ft.AD_Form_ID " +
                    "					AND mt.AD_Language=ft.AD_Language " +
                    "					AND m.AD_Form_ID IS NOT NULL " +
                    "					AND	Action = 'X')";

            Execute("Synchronize Menu with Forms Trl", sql, "  rows updated: ");

            /*

            --	Sync Names - Workflow
            DBMS_OUTPUT.PUT_LINE('Synchronizing Menu with Workflows');
            UPDATE	AD_Menu m
            SET		Name = (SELECT p.Name FROM AD_Workflow p WHERE m.AD_Workflow_ID=p.AD_Workflow_ID),
                    Description = (SELECT p.Description FROM AD_Workflow p WHERE m.AD_Workflow_ID=p.AD_Workflow_ID)
            WHERE	m.AD_Workflow_ID IS NOT NULL
              AND	m.Action = 'F';
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_Menu m " +
                    "	SET Name = (SELECT p.Name FROM AD_Workflow p WHERE m.AD_Workflow_ID=p.AD_Workflow_ID), " +
                    "			Description = (SELECT p.Description FROM AD_Workflow p WHERE m.AD_Workflow_ID=p.AD_Workflow_ID) " +
                    "	WHERE m.AD_Workflow_ID IS NOT NULL " +
                    "	  AND m.Action = 'F'";
            Execute("Synchronize Menu with Workflows", sql, "  rows updated: ");

            /*

            UPDATE	AD_Menu_Trl mt
            SET		Name = (SELECT pt.Name FROM AD_Workflow_Trl pt, AD_Menu m
                            WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Workflow_ID=pt.AD_Workflow_ID
                            AND mt.AD_Language=pt.AD_Language),
                    Description = (SELECT pt.Description FROM AD_Workflow_Trl pt, AD_Menu m
                            WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Workflow_ID=pt.AD_Workflow_ID
                            AND mt.AD_Language=pt.AD_Language),
                    IsTranslated = (SELECT pt.IsTranslated FROM AD_Workflow_Trl pt, AD_Menu m
                            WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Workflow_ID=pt.AD_Workflow_ID
                            AND mt.AD_Language=pt.AD_Language)
            WHERE EXISTS (SELECT * FROM AD_Workflow_Trl pt, AD_Menu m
                            WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Workflow_ID=pt.AD_Workflow_ID
                            AND mt.AD_Language=pt.AD_Language
                            AND m.AD_Workflow_ID IS NOT NULL
                            AND	Action = 'F');
            DBMS_OUTPUT.PUT_LINE('  trl rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_Menu_Trl mt " +
                    "	SET Name = (SELECT pt.Name FROM AD_Workflow_Trl pt, AD_Menu m " +
                    "					WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Workflow_ID=pt.AD_Workflow_ID " +
                    "					AND mt.AD_Language=pt.AD_Language), " +
                    "			Description = (SELECT pt.Description FROM AD_Workflow_Trl pt, AD_Menu m " +
                    "					WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Workflow_ID=pt.AD_Workflow_ID " +
                    "					AND mt.AD_Language=pt.AD_Language), " +
                    "			IsTranslated = (SELECT pt.IsTranslated FROM AD_Workflow_Trl pt, AD_Menu m " +
                    "					WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Workflow_ID=pt.AD_Workflow_ID " +
                    "					AND mt.AD_Language=pt.AD_Language) " +
                    "	WHERE EXISTS (SELECT * FROM AD_Workflow_Trl pt, AD_Menu m " +
                    "					WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Workflow_ID=pt.AD_Workflow_ID " +
                    "					AND mt.AD_Language=pt.AD_Language " +
                    "					AND m.AD_Workflow_ID IS NOT NULL " +
                    "					AND  Action = 'F')";
            Execute("Synchronize Menu with Workflows Trl", sql, "  rows updated: ");

            /*

            --	Sync Names = Task
            DBMS_OUTPUT.PUT_LINE('Synchronizing Menu with Tasks');
            UPDATE	AD_Menu m
            SET		Name = (SELECT Name FROM AD_Task f WHERE m.AD_Task_ID=f.AD_Task_ID),
                    Description = (SELECT Description FROM AD_Task f WHERE m.AD_Task_ID=f.AD_Task_ID)
            WHERE	AD_Task_ID IS NOT NULL
              AND	Action = 'T';
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_Menu m " +
                    "	SET Name = (SELECT Name FROM AD_Task f WHERE m.AD_Task_ID=f.AD_Task_ID), " +
                    "			Description = (SELECT Description FROM AD_Task f WHERE m.AD_Task_ID=f.AD_Task_ID) " +
                    "	WHERE AD_Task_ID IS NOT NULL " +
                    "	  AND Action = 'T'";
            Execute("Synchronize Menu with Tasks", sql, "  rows updated: ");

            /*

            UPDATE	AD_Menu_Trl mt
            SET		Name = (SELECT ft.Name FROM AD_Task_Trl ft, AD_Menu m
                            WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Task_ID=ft.AD_Task_ID
                            AND mt.AD_Language=ft.AD_Language),
                    Description = (SELECT ft.Description FROM AD_Task_Trl ft, AD_Menu m
                            WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Task_ID=ft.AD_Task_ID
                            AND mt.AD_Language=ft.AD_Language),
                    IsTranslated = (SELECT ft.IsTranslated FROM AD_Task_Trl ft, AD_Menu m
                            WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Task_ID=ft.AD_Task_ID
                            AND mt.AD_Language=ft.AD_Language)
            WHERE EXISTS (SELECT * FROM AD_Task_Trl ft, AD_Menu m
                            WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Task_ID=ft.AD_Task_ID
                            AND mt.AD_Language=ft.AD_Language
                            AND m.AD_Task_ID IS NOT NULL
                            AND	Action = 'T');
            DBMS_OUTPUT.PUT_LINE('  trl rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "	UPDATE AD_Menu_Trl mt " +
                    "	SET Name = (SELECT ft.Name FROM AD_Task_Trl ft, AD_Menu m " +
                    "					WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Task_ID=ft.AD_Task_ID " +
                    "					AND mt.AD_Language=ft.AD_Language), " +
                    "			Description = (SELECT ft.Description FROM AD_Task_Trl ft, AD_Menu m " +
                    "					WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Task_ID=ft.AD_Task_ID " +
                    "					AND mt.AD_Language=ft.AD_Language), " +
                    "			IsTranslated = (SELECT ft.IsTranslated FROM AD_Task_Trl ft, AD_Menu m " +
                    "					WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Task_ID=ft.AD_Task_ID " +
                    "					AND mt.AD_Language=ft.AD_Language) " +
                    "	WHERE EXISTS (SELECT * FROM AD_Task_Trl ft, AD_Menu m " +
                    "					WHERE mt.AD_Menu_ID=m.AD_Menu_ID AND m.AD_Task_ID=ft.AD_Task_ID " +
                    "					AND mt.AD_Language=ft.AD_Language " +
                    "					AND m.AD_Task_ID IS NOT NULL " +
                    "					AND  Action = 'T')";
            Execute("Synchronize Menu with Tasks Trl", sql, "  rows updated: ");

            /*

            --  Column Name + Element
            DBMS_OUTPUT.PUT_LINE('Synchronizing Column with Element');
            UPDATE AD_Column c
              SET (Name,Description,Help) = 
                (SELECT e.Name,e.Description,e.Help 
                FROM AD_Element e WHERE c.AD_Element_ID=e.AD_Element_ID)
            WHERE EXISTS 
                (SELECT * FROM AD_Element e 
                WHERE c.AD_Element_ID=e.AD_Element_ID
                  AND c.Name<>e.Name);
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            executesql("Synchronize Field Translations", sql,  "  rows updated: ");
            UPDATE AD_Column_Trl ct
              SET Name = (SELECT e.Name
                FROM AD_Column c INNER JOIN AD_Element_Trl e ON (c.AD_Element_ID=e.AD_Element_ID)
                WHERE ct.AD_Column_ID=c.AD_Column_ID AND ct.AD_Language=e.AD_Language)
            WHERE EXISTS 
                (SELECT * FROM AD_Column c INNER JOIN AD_Element_Trl e ON (c.AD_Element_ID=e.AD_Element_ID)
                WHERE ct.AD_Column_ID=c.AD_Column_ID AND ct.AD_Language=e.AD_Language
                  AND ct.Name<>e.Name);
            DBMS_OUTPUT.PUT_LINE('  trl rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "    UPDATE AD_Column c " +
                    "      SET Name = " +
                    "        (SELECT e.Name " +
                    "        FROM AD_Element e WHERE c.AD_Element_ID=e.AD_Element_ID) " +
                      "      , Description = " +
                    "        (SELECT e.Description " +
                    "        FROM AD_Element e WHERE c.AD_Element_ID=e.AD_Element_ID) " +
                      "      , Help = " +
                    "        (SELECT e.Help " +
                    "        FROM AD_Element e WHERE c.AD_Element_ID=e.AD_Element_ID) " +
                    "    WHERE EXISTS " +
                    "        (SELECT * FROM AD_Element e " +
                    "        WHERE c.AD_Element_ID=e.AD_Element_ID " +
                    "          AND c.Name<>e.Name)";
            Execute("Synchronize Column with Element", sql, "  rows updated: ");

            /*
    UPDATE AD_Column_Trl ct
      SET Name = (SELECT e.Name
        FROM AD_Column c INNER JOIN AD_Element_Trl e ON (c.AD_Element_ID=e.AD_Element_ID)
        WHERE ct.AD_Column_ID=c.AD_Column_ID AND ct.AD_Language=e.AD_Language)
    WHERE EXISTS 
        (SELECT * FROM AD_Column c INNER JOIN AD_Element_Trl e ON (c.AD_Element_ID=e.AD_Element_ID)
        WHERE ct.AD_Column_ID=c.AD_Column_ID AND ct.AD_Language=e.AD_Language
          AND ct.Name<>e.Name);
    */
            sql = "UPDATE AD_Column_Trl ct " +
                    "    SET Name = (SELECT e.Name " +
                    "        FROM AD_Column c INNER JOIN AD_Element_Trl e ON (c.AD_Element_ID=e.AD_Element_ID) " +
                    "        WHERE ct.AD_Column_ID=c.AD_Column_ID AND ct.AD_Language=e.AD_Language) " +
                    "    WHERE EXISTS " +
                    "        (SELECT * FROM AD_Column c INNER JOIN AD_Element_Trl e ON (c.AD_Element_ID=e.AD_Element_ID) " +
                    "        WHERE ct.AD_Column_ID=c.AD_Column_ID AND ct.AD_Language=e.AD_Language " +
                    "          AND ct.Name<>e.Name)";
            Execute("Synchronize Column with Element Trl", sql, "  rows updated: ");

            /*
   
    
            --  Table Name + Element
            DBMS_OUTPUT.PUT_LINE('Synchronizing Table with Element');
            UPDATE AD_Table t
              SET (Name,Description) = (SELECT e.Name,e.Description FROM AD_Element e 
                WHERE t.TableName||'_ID'=e.ColumnName)
            WHERE EXISTS (SELECT * FROM AD_Element e 
                WHERE t.TableName||'_ID'=e.ColumnName
                  AND t.Name<>e.Name);
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "    UPDATE AD_Table t " +
                    "      SET Name = (SELECT e.Name FROM AD_Element e " +
                    "        WHERE t.TableName||'_ID'=e.ColumnName) " +
                      "      , Description = (SELECT e.Description FROM AD_Element e " +
                    "        WHERE t.TableName||'_ID'=e.ColumnName) " +
                    "    WHERE EXISTS (SELECT * FROM AD_Element e " +
                    "        WHERE t.TableName||'_ID'=e.ColumnName " +
                    "          AND t.Name<>e.Name)";
            Execute("Synchronize Table with Element", sql, "  rows updated: ");

            /*
            UPDATE AD_Table_Trl tt
              SET Name = (SELECT e.Name 
                FROM AD_Table t INNER JOIN AD_Element ex ON (t.TableName||'_ID'=ex.ColumnName)
                  INNER JOIN AD_Element_Trl e ON (ex.AD_Element_ID=e.AD_Element_ID)
                WHERE tt.AD_Table_ID=t.AD_Table_ID AND tt.AD_Language=e.AD_Language)
            WHERE EXISTS (SELECT * 
                FROM AD_Table t INNER JOIN AD_Element ex ON (t.TableName||'_ID'=ex.ColumnName)
                  INNER JOIN AD_Element_Trl e ON (ex.AD_Element_ID=e.AD_Element_ID)
                WHERE tt.AD_Table_ID=t.AD_Table_ID AND tt.AD_Language=e.AD_Language
                  AND tt.Name<>e.Name);
            DBMS_OUTPUT.PUT_LINE('  trl rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "    UPDATE AD_Table_Trl tt " +
                    "      SET Name = (SELECT e.Name " +
                    "        FROM AD_Table t INNER JOIN AD_Element ex ON (t.TableName||'_ID'=ex.ColumnName) " +
                    "          INNER JOIN AD_Element_Trl e ON (ex.AD_Element_ID=e.AD_Element_ID) " +
                    "        WHERE tt.AD_Table_ID=t.AD_Table_ID AND tt.AD_Language=e.AD_Language) " +
                    "    WHERE EXISTS (SELECT * " +
                    "        FROM AD_Table t INNER JOIN AD_Element ex ON (t.TableName||'_ID'=ex.ColumnName) " +
                    "          INNER JOIN AD_Element_Trl e ON (ex.AD_Element_ID=e.AD_Element_ID) " +
                    "        WHERE tt.AD_Table_ID=t.AD_Table_ID AND tt.AD_Language=e.AD_Language " +
                    "          AND tt.Name<>e.Name)";
            Execute("Synchronize Table with Element Trl", sql, "  rows updated: ");

            /*

            --  Trl Table Name + Element
            UPDATE AD_Table t
              SET (Name,Description) = (SELECT e.Name||' Trl', e.Description 
                FROM AD_Element e 
                WHERE SUBSTR(t.TableName,1,LENGTH(t.TableName)-4)||'_ID'=e.ColumnName)
            WHERE TableName LIKE '%_Trl'
              AND EXISTS (SELECT * FROM AD_Element e 
                WHERE SUBSTR(t.TableName,1,LENGTH(t.TableName)-4)||'_ID'=e.ColumnName
                  AND t.Name<>e.Name);
            DBMS_OUTPUT.PUT_LINE('  rows updated: ' || SQL%ROWCOUNT);
            */
            sql = "    UPDATE AD_Table t " +
                    "     SET Name = (SELECT e.Name||' Trl' " +
                    "        FROM AD_Element e " +
                    "        WHERE SUBSTR(t.TableName,1,LENGTH(t.TableName)-4)||'_ID'=e.ColumnName) " +
                     "     , Description = (SELECT  e.Description " +
                    "        FROM AD_Element e " +
                    "        WHERE SUBSTR(t.TableName,1,LENGTH(t.TableName)-4)||'_ID'=e.ColumnName) " +
                    "    WHERE TableName LIKE '%_Trl' " +
                    "      AND EXISTS (SELECT * FROM AD_Element e " +
                    "        WHERE SUBSTR(t.TableName,1,LENGTH(t.TableName)-4)||'_ID'=e.ColumnName " +
                    "          AND t.Name<>e.Name)";
            Execute("Synchronize Trl Table Name + Element", sql, "  rows updated: ");

            /*
    UPDATE AD_Table_Trl tt
      SET Name = (SELECT e.Name || ' **'
        FROM AD_Table t INNER JOIN AD_Element ex ON (SUBSTR(t.TableName,1,LENGTH(t.TableName)-4)||'_ID'=ex.ColumnName)
          INNER JOIN AD_Element_Trl e ON (ex.AD_Element_ID=e.AD_Element_ID)
        WHERE tt.AD_Table_ID=t.AD_Table_ID AND tt.AD_Language=e.AD_Language)
    WHERE EXISTS (SELECT * 
        FROM AD_Table t INNER JOIN AD_Element ex ON (SUBSTR(t.TableName,1,LENGTH(t.TableName)-4)||'_ID'=ex.ColumnName)
          INNER JOIN AD_Element_Trl e ON (ex.AD_Element_ID=e.AD_Element_ID)
        WHERE tt.AD_Table_ID=t.AD_Table_ID AND tt.AD_Language=e.AD_Language
          AND t.TableName LIKE '%_Trl'
          AND tt.Name<>e.Name);
    DBMS_OUTPUT.PUT_LINE('  trl rows updated: ' || SQL%ROWCOUNT);			 */
            sql = "    UPDATE AD_Table_Trl tt " +
                    "      SET Name = (SELECT e.Name || ' **' " +
                    "        FROM AD_Table t INNER JOIN AD_Element ex ON (SUBSTR(t.TableName,1,LENGTH(t.TableName)-4)||'_ID'=ex.ColumnName) " +
                    "          INNER JOIN AD_Element_Trl e ON (ex.AD_Element_ID=e.AD_Element_ID) " +
                    "        WHERE tt.AD_Table_ID=t.AD_Table_ID AND tt.AD_Language=e.AD_Language) " +
                    "    WHERE EXISTS (SELECT * " +
                    "        FROM AD_Table t INNER JOIN AD_Element ex ON (SUBSTR(t.TableName,1,LENGTH(t.TableName)-4)||'_ID'=ex.ColumnName) " +
                    "          INNER JOIN AD_Element_Trl e ON (ex.AD_Element_ID=e.AD_Element_ID) " +
                    "        WHERE tt.AD_Table_ID=t.AD_Table_ID AND tt.AD_Language=e.AD_Language " +
                    "          AND t.TableName LIKE '%_Trl' " +
                    "          AND tt.Name<>e.Name)";
            Execute("Synchronize AD_Table_Trl", sql, "  rows updated: ");

            sql = "UPDATE AD_InfoColumn ic " +
        "     SET Name = " +
        "          (SELECT e.Name " +
        "             FROM AD_Element e  " +
        "            WHERE ic.AD_Element_ID=e.AD_Element_ID) " +
        "     , Description = " +
        "          (SELECT e.Description " +
        "             FROM AD_Element e  " +
        "            WHERE ic.AD_Element_ID=e.AD_Element_ID) " +
        "     , Help = " +
        "          (SELECT e.Help " +
        "             FROM AD_Element e  " +
        "            WHERE ic.AD_Element_ID=e.AD_Element_ID) " +
        "    WHERE EXISTS " +
        "          (SELECT * FROM AD_Element e " +
        "            WHERE ic.AD_Element_ID=e.AD_Element_ID " +
        " 		       AND (ic.Name<>e.Name OR NVL(ic.Description,' ')<>NVL(e.Description,' ') OR NVL(ic.Help,' ')<>NVL(e.Help,' '))) " +
        "      AND ic.IsCentrallyMaintained='Y' AND ic.IsActive='Y'";
            Execute("Synchronize Info Column with Element", sql, "  rows updated: ");

            sql = "UPDATE AD_InfoColumn_Trl ict " +
            "    SET Name = " +
            "        (SELECT e.Name  " +
            "           FROM AD_InfoColumn ic INNER JOIN AD_Element_Trl e ON (ic.AD_Element_ID=e.AD_Element_ID) " +
            "          WHERE ict.AD_InfoColumn_ID=ic.AD_InfoColumn_ID AND ict.AD_Language=e.AD_Language) " +
              "    ,  Description = " +
            "        (SELECT  e.Description " +
            "           FROM AD_InfoColumn ic INNER JOIN AD_Element_Trl e ON (ic.AD_Element_ID=e.AD_Element_ID) " +
            "          WHERE ict.AD_InfoColumn_ID=ic.AD_InfoColumn_ID AND ict.AD_Language=e.AD_Language) " +
              "    ,  Help = " +
            "        (SELECT  e.Help  " +
            "           FROM AD_InfoColumn ic INNER JOIN AD_Element_Trl e ON (ic.AD_Element_ID=e.AD_Element_ID) " +
            "          WHERE ict.AD_InfoColumn_ID=ic.AD_InfoColumn_ID AND ict.AD_Language=e.AD_Language) " +
              "    ,  isTranslated = " +
            "        (SELECT  isTranslated  " +
            "           FROM AD_InfoColumn ic INNER JOIN AD_Element_Trl e ON (ic.AD_Element_ID=e.AD_Element_ID) " +
            "          WHERE ict.AD_InfoColumn_ID=ic.AD_InfoColumn_ID AND ict.AD_Language=e.AD_Language) " +
            "    WHERE EXISTS " +
            "        (SELECT * FROM AD_InfoColumn ic  " +
            "          INNER JOIN AD_Element_Trl e ON (ic.AD_Element_ID=e.AD_Element_ID) " +
            "          WHERE ict.AD_InfoColumn_ID=ic.AD_InfoColumn_ID AND ict.AD_Language=e.AD_Language " +
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
