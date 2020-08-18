/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ImportReportLine
 * Purpose        : Import ReportLines from I_ReportLine
 * Class Used     : ProcessEngine.SvrProcess
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

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class ImportReportLine : ProcessEngine.SvrProcess
    {
        /**	Client to be imported to		*/
        private int _AD_Client_ID = 0;
        /** Default Report Line Set			*/
        private int _PA_ReportLineSet_ID = 0;
        /**	Delete old Imported				*/
        private bool _deleteOldImported = false;

        /** Effective						*/
        private DateTime? _DateValue = null;

        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("AD_Client_ID"))
                {
                    _AD_Client_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("PA_ReportLineSet_ID"))
                {
                    _PA_ReportLineSet_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("DeleteOldImported"))
                {
                    _deleteOldImported = "Y".Equals(para[i].GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            if (_DateValue == null)
            {
                _DateValue = DateTime.Now;// new Timestamp (System.currentTimeMillis());
            }
        }	//	prepare


        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>Info</returns>
        protected override String DoIt()
        {
            StringBuilder sql = null;
            int no = 0;
            String clientCheck = " AND AD_Client_ID=" + _AD_Client_ID;

            //	****	Prepare	****

            //	Delete Old Imported
            if (_deleteOldImported)
            {
                sql = new StringBuilder("DELETE FROM I_ReportLine "
                    + "WHERE I_IsImported='Y'").Append(clientCheck);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                log.Fine("Delete Old Impored =" + no);
            }

            //	Set Client, Org, IsActive, Created/Updated
            sql = new StringBuilder("UPDATE I_ReportLine "
                + "SET AD_Client_ID = COALESCE (AD_Client_ID, ").Append(_AD_Client_ID).Append("),"
                + " AD_Org_ID = COALESCE (AD_Org_ID, 0),"
                + " IsActive = COALESCE (IsActive, 'Y'),"
                + " Created = COALESCE (Created, SysDate),"
                + " CreatedBy = COALESCE (CreatedBy, 0),"
                + " Updated = COALESCE (Updated, SysDate),"
                + " UpdatedBy = COALESCE (UpdatedBy, 0),"
                + " I_ErrorMsg = NULL,"
                + " I_IsImported = 'N' "
                + "WHERE I_IsImported<>'Y' OR I_IsImported IS NULL");
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Reset=" + no);

            //	ReportLineSetName (Default)
            if (_PA_ReportLineSet_ID != 0)
            {
                sql = new StringBuilder("UPDATE I_ReportLine i "
                    + "SET ReportLineSetName=(SELECT Name FROM PA_ReportLineSet r"
                    + " WHERE PA_ReportLineSet_ID=").Append(_PA_ReportLineSet_ID).Append(" AND i.AD_Client_ID=r.AD_Client_ID) "
                    + "WHERE ReportLineSetName IS NULL AND PA_ReportLineSet_ID IS NULL"
                    + " AND I_IsImported<>'Y'").Append(clientCheck);
                no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                log.Fine("Set ReportLineSetName Default=" + no);
            }
            //	Set PA_ReportLineSet_ID
            sql = new StringBuilder("UPDATE I_ReportLine i "
                + "SET PA_ReportLineSet_ID=(SELECT PA_ReportLineSet_ID FROM PA_ReportLineSet r"
                + " WHERE i.ReportLineSetName=r.Name AND i.AD_Client_ID=r.AD_Client_ID) "
                + "WHERE PA_ReportLineSet_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set PA_ReportLineSet_ID=" + no);
            //
            String ts = DataBase.DB.IsPostgreSQL() ? "COALESCE(I_ErrorMsg,'')" : "I_ErrorMsg";  //java bug, it could not be used directly
            sql = new StringBuilder("UPDATE I_ReportLine "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid ReportLineSet, ' "
                + "WHERE PA_ReportLineSet_ID IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Invalid ReportLineSet=" + no);

            //	Ignore if there is no Report Line Name or ID
            sql = new StringBuilder("UPDATE I_ReportLine "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'Ignored=NoLineName, ' "
                + "WHERE PA_ReportLine_ID IS NULL AND Name IS NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Invalid LineName=" + no);

            //	Validate ElementValue
            sql = new StringBuilder("UPDATE I_ReportLine i "
                + "SET C_ElementValue_ID=(SELECT C_ElementValue_ID FROM C_ElementValue e"
                + " WHERE i.ElementValue=e.Value AND i.AD_Client_ID=e.AD_Client_ID) "
                + "WHERE C_ElementValue_ID IS NULL AND ElementValue IS NOT NULL"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set C_ElementValue_ID=" + no);

            //	Validate C_ElementValue_ID
            sql = new StringBuilder("UPDATE I_ReportLine "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid ElementValue, ' "
                + "WHERE C_ElementValue_ID IS NULL AND LineType<>'C'" // MReportLine.LINETYPE_Calculation
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Invalid AccountType=" + no);

            //	Set SeqNo
            sql = new StringBuilder("UPDATE I_ReportLine "
                + "SET SeqNo=I_ReportLine_ID "
                + "WHERE SeqNo IS NULL"
                + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set SeqNo Default=" + no);

            //	Copy/Sync from first Row of Line
            sql = new StringBuilder("UPDATE I_ReportLine i "
                + "SET (Description, SeqNo, IsSummary, IsPrinted, LineType, CalculationType, AmountType, PostingType)="
                + " (SELECT Description, SeqNo, IsSummary, IsPrinted, LineType, CalculationType, AmountType, PostingType"
                + " FROM I_ReportLine ii WHERE i.Name=ii.Name AND i.PA_ReportLineSet_ID=ii.PA_ReportLineSet_ID"
                + " AND ii.I_ReportLine_ID=(SELECT MIN(I_ReportLine_ID) FROM I_ReportLine iii"
                + " WHERE i.Name=iii.Name AND i.PA_ReportLineSet_ID=iii.PA_ReportLineSet_ID)) "
                + "WHERE EXISTS (SELECT *"
                + " FROM I_ReportLine ii WHERE i.Name=ii.Name AND i.PA_ReportLineSet_ID=ii.PA_ReportLineSet_ID"
                + " AND ii.I_ReportLine_ID=(SELECT MIN(I_ReportLine_ID) FROM I_ReportLine iii"
                + " WHERE i.Name=iii.Name AND i.PA_ReportLineSet_ID=iii.PA_ReportLineSet_ID))"
                + " AND I_IsImported='N'").Append(clientCheck);		//	 not if previous error
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Sync from first Row of Line=" + no);

            //	Validate IsSummary - (N) Y
            sql = new StringBuilder("UPDATE I_ReportLine "
                + "SET IsSummary='N' "
                + "WHERE IsSummary IS NULL OR IsSummary NOT IN ('Y','N')"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set IsSummary Default=" + no);

            //	Validate IsPrinted - (Y) N
            sql = new StringBuilder("UPDATE I_ReportLine "
                + "SET IsPrinted='Y' "
                + "WHERE IsPrinted IS NULL OR IsPrinted NOT IN ('Y','N')"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set IsPrinted Default=" + no);

            //	Validate Line Type - (S) C
            sql = new StringBuilder("UPDATE I_ReportLine "
                + "SET LineType='S' "
                + "WHERE LineType IS NULL OR LineType NOT IN ('S','C')"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set LineType Default=" + no);

            //	Validate Optional Calculation Type - A P R S
            sql = new StringBuilder("UPDATE I_ReportLine "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid CalculationType, ' "
                + "WHERE CalculationType IS NOT NULL AND CalculationType NOT IN ('A','P','R','S')"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Invalid CalculationType=" + no);

            //	Validate Optional Amount Type -
            sql = new StringBuilder("UPDATE I_ReportLine "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid CalculationType, ' "
                + "WHERE AmountType IS NOT NULL AND UPPER(AmountType) NOT IN ('BP','CP','DP','QP', 'BY','CY','DY','QY', 'BT','CT','DT','QT')"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Invalid AmountType=" + no);

            //	Validate Optional Posting Type - A B E S R
            sql = new StringBuilder("UPDATE I_ReportLine "
                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||'ERR=Invalid CalculationType, ' "
                + "WHERE PostingType IS NOT NULL AND PostingType NOT IN ('A','B','E','S','R')"
                + " AND I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Invalid PostingType=" + no);

            //	Set PA_ReportLine_ID
            sql = new StringBuilder("UPDATE I_ReportLine i "
                + "SET PA_ReportLine_ID=(SELECT MAX(PA_ReportLine_ID) FROM PA_ReportLine r"
                + " WHERE i.Name=r.Name AND i.PA_ReportLineSet_ID=r.PA_ReportLineSet_ID) "
                + "WHERE PA_ReportLine_ID IS NULL AND PA_ReportLineSet_ID IS NOT NULL"
                + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set PA_ReportLine_ID=" + no);

            Commit();

            //	-------------------------------------------------------------------
            int noInsertLine = 0;
            int noUpdateLine = 0;
            IDataReader idr = null;
            //	****	Create Missing ReportLines
            sql = new StringBuilder("SELECT DISTINCT PA_ReportLineSet_ID, Name "
                + "FROM I_ReportLine "
                + "WHERE I_IsImported='N' AND PA_ReportLine_ID IS NULL"
                + " AND I_IsImported='N'").Append(clientCheck);
            try
            {
                //	Insert ReportLine
                //PreparedStatement pstmt_insertLine = DataBase.prepareStatement
                String _insertLine = "INSERT INTO PA_ReportLine "
                    + "(PA_ReportLine_ID,PA_ReportLineSet_ID,"
                    + "AD_Client_ID,AD_Org_ID,IsActive,Created,CreatedBy,Updated,UpdatedBy,"
                    + "Name,SeqNo,IsPrinted,IsSummary,LineType)"
                    + "SELECT @param1,PA_ReportLineSet_ID,"
                    + "AD_Client_ID,AD_Org_ID,'Y',SysDate,CreatedBy,SysDate,UpdatedBy,"
                    + "Name,SeqNo,IsPrinted,IsSummary,LineType "
                    //jz + "FROM I_ReportLine "
                    // + "WHERE PA_ReportLineSet_ID=? AND Name=? AND ROWNUM=1"		//	#2..3
                    + "FROM I_ReportLine "
                    + "WHERE I_ReportLine_ID=(SELECT MAX(I_ReportLine_ID) "
                    + "FROM I_ReportLine "
                    + "WHERE PA_ReportLineSet_ID=@param2 AND Name=@param3 "		//	#2..3
                    //jz + clientCheck, Get_TrxName());
                    + clientCheck + ")";//, Get_TrxName());

                //PreparedStatement pstmt = DataBase.prepareStatement(sql.ToString(), Get_TrxName());
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                while (idr.Read())
                {
                    int PA_ReportLineSet_ID = Utility.Util.GetValueOfInt(idr[0]);// rs.getInt(1);
                    String Name = Utility.Util.GetValueOfString(idr[1]);// rs.getString(2);
                    //
                    try
                    {
                        int PA_ReportLine_ID = DataBase.DB.GetNextID(_AD_Client_ID, "PA_ReportLine", Get_TrxName());
                        if (PA_ReportLine_ID <= 0)
                        {
                            throw new Exception("No NextID (" + PA_ReportLine_ID + ")");
                        }
                        SqlParameter[] param = new SqlParameter[3];
                        //pstmt_insertLine.setInt(1, PA_ReportLine_ID);
                        param[0] = new SqlParameter("@param1", PA_ReportLine_ID);
                        //pstmt_insertLine.setInt(2, PA_ReportLineSet_ID);
                        param[1] = new SqlParameter("@param2", PA_ReportLineSet_ID);
                        //pstmt_insertLine.setString(3, Name);
                        param[2] = new SqlParameter("@param3", Name);
                        //
                        no = DataBase.DB.ExecuteQuery(_insertLine, param, Get_TrxName());
                        log.Finest("Insert ReportLine = " + no + ", PA_ReportLine_ID=" + PA_ReportLine_ID);
                        noInsertLine++;
                    }
                    catch (Exception ex)
                    {
                        log.Finest(ex.ToString());
                        continue;
                    }
                }
                idr.Close();
                //

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                log.Log(Level.SEVERE, "Create ReportLine", e);
            }

            //	Set PA_ReportLine_ID (for newly created)
            sql = new StringBuilder("UPDATE I_ReportLine i "
                + "SET PA_ReportLine_ID=(SELECT MAX(PA_ReportLine_ID) FROM PA_ReportLine r"
                + " WHERE i.Name=r.Name AND i.PA_ReportLineSet_ID=r.PA_ReportLineSet_ID) "
                + "WHERE PA_ReportLine_ID IS NULL AND PA_ReportLineSet_ID IS NOT NULL"
                + " AND I_IsImported='N'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Fine("Set PA_ReportLine_ID=" + no);

            //	****	Update ReportLine
            sql = new StringBuilder("UPDATE PA_ReportLine r "
                + "SET (Description,SeqNo,IsSummary,IsPrinted,LineType,CalculationType,AmountType,PostingType,Updated,UpdatedBy)="
                + " (SELECT Description,SeqNo,IsSummary,IsPrinted,LineType,CalculationType,AmountType,PostingType,SysDate,UpdatedBy"
                + " FROM I_ReportLine i WHERE r.Name=i.Name AND r.PA_ReportLineSet_ID=i.PA_ReportLineSet_ID"
                + " AND i.I_ReportLine_ID=(SELECT MIN(I_ReportLine_ID) FROM I_ReportLine iii"
                + " WHERE i.Name=iii.Name AND i.PA_ReportLineSet_ID=iii.PA_ReportLineSet_ID)) "
                + "WHERE EXISTS (SELECT *"
                + " FROM I_ReportLine i WHERE r.Name=i.Name AND r.PA_ReportLineSet_ID=i.PA_ReportLineSet_ID"
                + " AND i.I_ReportLine_ID=(SELECT MIN(I_ReportLine_ID) FROM I_ReportLine iii"
                + " WHERE i.Name=iii.Name AND i.PA_ReportLineSet_ID=iii.PA_ReportLineSet_ID AND i.I_IsImported='N'))")
                .Append(clientCheck);
            noUpdateLine = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            log.Config("Update PA_ReportLine=" + noUpdateLine);


            //	-------------------------------------------------------------------
            int noInsertSource = 0;
            int noUpdateSource = 0;

            //	****	Create ReportSource
            sql = new StringBuilder("SELECT I_ReportLine_ID, PA_ReportSource_ID "
                + "FROM I_ReportLine "
                + "WHERE PA_ReportLine_ID IS NOT NULL"
                + " AND I_IsImported='N'").Append(clientCheck);
            try
            {
                //	Insert ReportSource
                //PreparedStatement pstmt_insertSource = DataBase.prepareStatement
                String _insertSource = "INSERT INTO PA_ReportSource "
                    + "(PA_ReportSource_ID,"
                    + "AD_Client_ID,AD_Org_ID,IsActive,Created,CreatedBy,Updated,UpdatedBy,"
                    + "PA_ReportLine_ID,ElementType,C_ElementValue_ID) "
                    + "SELECT @param1,"
                    + "AD_Client_ID,AD_Org_ID,'Y',SysDate,CreatedBy,SysDate,UpdatedBy,"
                    + "PA_ReportLine_ID,'AC',C_ElementValue_ID "
                    + "FROM I_ReportLine "
                    + "WHERE I_ReportLine_ID=@param2"
                    + " AND I_IsImported='N'"
                    + clientCheck;

                //	Update ReportSource
                //jz 
                /*
                String sqlt="UPDATE PA_ReportSource "
                    + "SET (ElementType,C_ElementValue_ID,Updated,UpdatedBy)="
                    + " (SELECT 'AC',C_ElementValue_ID,SysDate,UpdatedBy"
                    + " FROM I_ReportLine"
                    + " WHERE I_ReportLine_ID=?) "
                    + "WHERE PA_ReportSource_ID=?"
                    + clientCheck;
                PreparedStatement pstmt_updateSource = DataBase.prepareStatement
                    (sqlt, Get_TrxName());
                    */

                //	Set Imported = Y
                //PreparedStatement pstmt_setImported = DataBase.prepareStatement
                String _setImported = "UPDATE I_ReportLine SET I_IsImported='Y',"
                    + " PA_ReportSource_ID=@param1, "
                    + " Updated=SysDate, Processed='Y' WHERE I_ReportLine_ID=@param2";// Get_TrxName());

                //PreparedStatement pstmt = DataBase.prepareStatement(sql.ToString(), Get_TrxName());
                //ResultSet rs = pstmt.executeQuery();
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                while (idr.Read())
                {
                    int I_ReportLine_ID = Utility.Util.GetValueOfInt(idr[0]);// rs.getInt(1);
                    int PA_ReportSource_ID = Utility.Util.GetValueOfInt(idr[1]);// rs.getInt(2);
                    //
                    SqlParameter[] param = new SqlParameter[2];
                    if (PA_ReportSource_ID == 0)			//	New ReportSource
                    {
                        try
                        {
                            PA_ReportSource_ID = DataBase.DB.GetNextID(_AD_Client_ID, "PA_ReportSource", Get_TrxName());
                            if (PA_ReportSource_ID <= 0)
                            {
                                if (idr != null)
                                {
                                    idr.Close();
                                    idr = null;
                                }
                                throw new Exception("No NextID (" + PA_ReportSource_ID + ")");
                            }

                            //pstmt_insertSource.setInt(1, PA_ReportSource_ID);
                            param[0] = new SqlParameter("@param1", PA_ReportSource_ID);
                            //pstmt_insertSource.setInt(2, I_ReportLine_ID);
                            param[1] = new SqlParameter("@param2", I_ReportLine_ID);
                            //
                            no = DataBase.DB.ExecuteQuery(_insertSource, param, Get_TrxName());// pstmt_insertSource.ExecuteQuery();
                            log.Finest("Insert ReportSource = " + no + ", I_ReportLine_ID=" + I_ReportLine_ID + ", PA_ReportSource_ID=" + PA_ReportSource_ID);
                            noInsertSource++;
                        }
                        catch (Exception ex)
                        {
                            log.Finest("Insert ReportSource - " + ex.ToString());
                            sql = new StringBuilder("UPDATE I_ReportLine i "
                                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||").Append(DataBase.DB.TO_STRING("Insert ElementSource: " + ex.ToString()))
                                .Append("WHERE I_ReportLine_ID=").Append(I_ReportLine_ID);
                            DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                            continue;
                        }
                    }
                    else								//	update Report Source
                    {
                        //jz
                        String sqlt = "UPDATE PA_ReportSource "
                            + "SET (ElementType,C_ElementValue_ID,Updated,UpdatedBy)="
                            + " (SELECT CAST('AC' AS CHAR(2)),C_ElementValue_ID,SysDate,UpdatedBy"  //jz
                            + " FROM I_ReportLine"
                            + " WHERE I_ReportLine_ID=" + I_ReportLine_ID + ") "
                            + "WHERE PA_ReportSource_ID=" + PA_ReportSource_ID + " "
                            + clientCheck;
                        //PreparedStatement pstmt_updateSource = DataBase.prepareStatement
                        //(sqlt, Get_TrxName());
                        //pstmt_updateSource.setInt(1, I_ReportLine_ID);
                        //pstmt_updateSource.setInt(2, PA_ReportSource_ID);
                        try
                        {
                            no = DataBase.DB.ExecuteQuery(sqlt, null, Get_TrxName());// pstmt_updateSource.ExecuteQuery();
                            //no = DataBase.DB.ExecuteQuery(sqlt, Get_TrxName());
                            log.Finest("Update ReportSource = " + no + ", I_ReportLine_ID=" + I_ReportLine_ID + ", PA_ReportSource_ID=" + PA_ReportSource_ID);
                            noUpdateSource++;
                        }
                        catch (Exception ex)
                        {
                            log.Finest("Update ReportSource - " + ex.ToString());
                            sql = new StringBuilder("UPDATE I_ReportLine i "
                                + "SET I_IsImported='E', I_ErrorMsg=" + ts + "||").Append(DataBase.DB.TO_STRING("Update ElementSource: " + ex.ToString()))
                                .Append("WHERE I_ReportLine_ID=").Append(I_ReportLine_ID);
                            DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
                            continue;
                        }

                    }	//	update source

                    //	Set Imported to Y
                    //pstmt_setImported.setInt(1, PA_ReportSource_ID);
                    param[0] = new SqlParameter("@param1", PA_ReportSource_ID);
                    //pstmt_setImported.setInt(2, I_ReportLine_ID);
                    param[0] = new SqlParameter("@param1", I_ReportLine_ID);
                    no = DataBase.DB.ExecuteQuery(_setImported, param, Get_TrxName());// pstmt_setImported.ExecuteQuery();
                    if (no != 1)
                        log.Log(Level.SEVERE, "Set Imported=" + no);
                    //
                    Commit();
                }
                idr.Close();
                //

                //jz pstmt_updateSource.close();

                //
            }
            catch 
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }

            //	Set Error to indicator to not imported
            sql = new StringBuilder("UPDATE I_ReportLine "
                + "SET I_IsImported='N', Updated=SysDate "
                + "WHERE I_IsImported<>'Y'").Append(clientCheck);
            no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());
            AddLog(0, null, Utility.Util.GetValueOfDecimal(no), "@Errors@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsertLine), "@PA_ReportLine_ID@: @Inserted@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noUpdateLine), "@PA_ReportLine_ID@: @Updated@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noInsertSource), "@PA_ReportSource_ID@: @Inserted@");
            AddLog(0, null, Utility.Util.GetValueOfDecimal(noUpdateSource), "@PA_ReportSource_ID@: @Updated@");

            return "";
        }	//	doIt

    }
}
