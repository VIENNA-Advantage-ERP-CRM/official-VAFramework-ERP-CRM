/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : SequenceCheck
 * Purpose        : System + Document Sequence Check
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
    public class SequenceCheck : ProcessEngine.SvrProcess
    {
        /**	Static Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(SequenceCheck).FullName);//.class);

        /// <summary>
        ///  Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
        }	//	prepare

        /// <summary>
        /// Perform Process.(see also MSequenve.validate)
        /// </summary>
        /// <returns>Message to be translated</returns>
        protected override String DoIt()
        {
            log.Info("");
            //
            CheckTableSequences(GetCtx(), this);
            CheckTableID(GetCtx(), this);
            CheckClientSequences(GetCtx(), this);
            return "Sequence Check";
        }	//	doIt

        /// <summary>
        ///	Validate Sequences
        /// </summary>
        /// <param name="ctx">context</param>
        public static void Validate(Ctx ctx)
        {
            try
            {
                CheckTableSequences(ctx, null);
                CheckTableID(ctx, null);
                CheckClientSequences(ctx, null);
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, "validate", e);
            }
        }	//	validate



        /// <summary>
        /// Check existence of Table Sequences.
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="sp">server process or null</param>
        private static void CheckTableSequences(Ctx ctx, SvrProcess sp)
        {
            Trx trxName = null;
            if (sp != null)
            {
                trxName = sp.Get_Trx();
            }
            String sql = "SELECT TableName "
                + "FROM AD_Table t "
                + "WHERE IsActive='Y' AND IsView='N'"
                + " AND NOT EXISTS (SELECT * FROM AD_Sequence s "
                + "WHERE UPPER(s.Name)=UPPER(t.TableName) AND s.IsTableID='Y')";
            IDataReader idr = null;
            try
            {
                //pstmt = DataBase.prepareStatement(sql, trxName);
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                while (idr.Read())
                {
                    String tableName = Utility.Util.GetValueOfString(idr[0]);// rs.getString(1);
                    if (MSequence.CreateTableSequence(ctx, tableName, trxName))
                    {
                        if (sp != null)
                        {
                            sp.AddLog(0, null, null, tableName);
                        }
                        else
                        {
                            _log.Fine(tableName);
                        }
                    }
                    else
                    {
                        idr.Close();
                        throw new Exception("Error creating Table Sequence for " + tableName);
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
                _log.Log(Level.SEVERE, sql, e);
            }

            //	Sync Table Name case
            //jz replace s with AD_Sequence
            sql = "UPDATE AD_Sequence "
                + "SET Name = (SELECT TableName FROM AD_Table t "
                    + "WHERE t.IsView='N' AND UPPER(AD_Sequence.Name)=UPPER(t.TableName)) "
                + "WHERE AD_Sequence.IsTableID='Y'"
                + " AND EXISTS (SELECT * FROM AD_Table t "
                    + "WHERE t.IsActive='Y' AND t.IsView='N'"
                    + " AND UPPER(AD_Sequence.Name)=UPPER(t.TableName) AND AD_Sequence.Name<>t.TableName)";
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);// DataBase.executeUpdate(sql, trxName);
            if (no > 0)
            {
                if (sp != null)
                {
                    sp.AddLog(0, null, null, "SyncName #" + no);
                }
                else
                {
                    _log.Fine("Sync #" + no);
                }
            }
            if (no >= 0)
            {
                return;
            }
            /** Find Duplicates 		 */
            sql = "SELECT TableName, s.Name "
                + "FROM AD_Table t, AD_Sequence s "
                + "WHERE t.IsActive='Y' AND t.IsView='N'"
                + " AND UPPER(s.Name)=UPPER(t.TableName) AND s.Name<>t.TableName";
            //
            try
            {
                //pstmt = DataBase.prepareStatement (sql, null);
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                while (idr.Read())
                {
                    String TableName = Utility.Util.GetValueOfString(idr[0]);// rs.getString(1);
                    String SeqName = Utility.Util.GetValueOfString(idr[1]);
                    sp.AddLog(0, null, null, "ERROR: TableName=" + TableName + " - Sequence=" + SeqName);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
        }	//	checkTableSequences


        /// <summary>
        /// Check Table Sequence ID values
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="sp">server process or null</param>
        private static void CheckTableID(Ctx ctx, SvrProcess sp)
        {
            int IDRangeEnd = DataBase.DB.GetSQLValue(null,
                "SELECT IDRangeEnd FROM AD_System");
            if (IDRangeEnd <= 0)
            {
                IDRangeEnd = DataBase.DB.GetSQLValue(null,
                    "SELECT MIN(IDRangeStart)-1 FROM AD_Replication");
            }
            _log.Info("IDRangeEnd = " + IDRangeEnd);
            //
            String sql = "SELECT * FROM AD_Sequence "
                + "WHERE IsTableID='Y' "
                + "ORDER BY Name";
            int counter = 0;
            IDataReader idr = null;
            Trx trxName = null;
            if (sp != null)
            {
                trxName = sp.Get_Trx();
            }
            try
            {
                //pstmt = DataBase.prepareStatement(sql, trxName);
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                while (idr.Read())
                {
                    MSequence seq = new MSequence(ctx, idr, trxName);
                    int old = seq.GetCurrentNext();
                    int oldSys = seq.GetCurrentNextSys();
                    if (seq.ValidateTableIDValue())
                    {
                        if (seq.GetCurrentNext() != old)
                        {
                            String msg = seq.GetName() + " ID  "
                                + old + " -> " + seq.GetCurrentNext();
                            if (sp != null)
                            {
                                sp.AddLog(0, null, null, msg);
                            }
                            else
                            {
                                _log.Fine(msg);
                            }
                        }
                        if (seq.GetCurrentNextSys() != oldSys)
                        {
                            String msg = seq.GetName() + " Sys "
                                + oldSys + " -> " + seq.GetCurrentNextSys();
                            if (sp != null)
                            {
                                sp.AddLog(0, null, null, msg);
                            }
                            else
                            {
                                _log.Fine(msg);
                            }
                        }
                        if (seq.Save())
                        {
                            counter++;
                        }
                        else
                        {
                            _log.Severe("Not updated: " + seq);
                        }
                    }
                    //	else if (CLogMgt.isLevel(6)) 
                    //		log.fine("OK - " + tableName);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }

            _log.Fine("#" + counter);
        }	//	checkTableID


        /// <summary>
        /// Check/Initialize DocumentNo/Value Sequences for all Clients 	
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="sp">server process or null</param>
        private static void CheckClientSequences(Ctx ctx, SvrProcess sp)
        {
            Trx trxName = null;
            if (sp != null)
            {
                trxName = sp.Get_Trx();
            }
            //	Sequence for DocumentNo/Value
            MClient[] clients = MClient.GetAll(ctx);
            for (int i = 0; i < clients.Length; i++)
            {
                MClient client = clients[i];
                if (!client.IsActive())
                {
                    continue;
                }
                MSequence.CheckClientSequences(ctx, client.GetAD_Client_ID(), trxName);
            }	//	for all clients

        }	//	checkClientSequences

    }	//	SequenceCheck

}
