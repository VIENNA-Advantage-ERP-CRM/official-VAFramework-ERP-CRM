/********************************************************
 * Project Name         : VAdvantage
 * Module/Class Name    : Change Log Model
 * Purpose              : Track all the changes
 * Class Used           : MWindow inherits X_AD_Window
 * Chronological        Development
 * Mukesh Arora          05-May-2009 
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;
namespace VAdvantage.Model
{
    public class MChangeLog : X_AD_ChangeLog
    {

        /**	Change Log				*/
        //private static int[] changeLog = null;

        private static CCache<int, int> changeLogAll = new CCache<int, int>("MChangeLogAll", 50);
        private static CCache<int, int> changeLogUpdate = new CCache<int, int>("MChangeLogUpdate", 50);
        //	Logger					
        private static VLogger _log = VLogger.GetVLogger(typeof(MChangeLog).FullName);
        /** NULL Value				*/
        public static String NULL = "NULL";


        /// <summary>
        /// 	Do we track all changes for this table
        /// </summary>
        /// <param name="AD_Table_ID"> table</param>
        /// <param name="type"></param>
        /// <returns>true if changes are tracked</returns>
        public static Boolean IsLogged(int AD_Table_ID, String type)
        {

            if (changeLogAll.Count == 0 && changeLogUpdate.Count == 0)
                FillChangeLog();

            //int index = Arrays.binarySearch(s_logAllChanges, AD_Table_ID);

            if (changeLogAll.ContainsKey(AD_Table_ID))
            {
                return true;
            }

            if (!CHANGELOGTYPE_Insert.Equals(type)) // Update and Deletes
                return changeLogUpdate.ContainsKey(AD_Table_ID);
            else
                return false;







            //if (!CHANGELOGTYPE_Insert.Equals(type) &&
            //    (AD_Table_ID == MRole.Table_ID
            //    || AD_Table_ID == MUser.Table_ID
            //    || AD_Table_ID == MPreference.Table_ID
            //    || AD_Table_ID == MClient.Table_ID
            //    || AD_Table_ID == MOrg.Table_ID))
            //    return true;
            //if (changeLogAll.Count == 0 && changeLogUpdate.Count == 0)
            //    FillChangeLog();

            ////
            //if (type == X_AD_ChangeLog.CHANGELOGTYPE_Update || type == X_AD_ChangeLog.CHANGELOGTYPE_Delete)
            //{
            //    return changeLogUpdate.ContainsKey(AD_Table_ID);
            //}
            //else
            //{
            //    return changeLogAll.ContainsKey(AD_Table_ID);
            //}
            ////int index = Array.BinarySearch(changeLog, AD_Table_ID);
            //return false;
        }	//	trackChanges


        /// <summary>
        /// Not Logged
        /// </summary>
        /// <param name="AD_Table_ID">table</param>
        /// <param name="tableName">column</param>
        /// <param name="AD_Column_ID">type</param>
        /// <param name="type"></param>
        /// <returns>true if not logged</returns>
        public static Boolean IsNotLogged(int AD_Table_ID, String tableName,
        int AD_Column_ID, String type)
        {

            if (AD_Table_ID == X_AD_ChangeLog.Table_ID
                   || AD_Table_ID == X_AD_WindowLog.Table_ID
                   || AD_Table_ID == X_AD_QueryLog.Table_ID
                   || AD_Table_ID == X_AD_Issue.Table_ID
                   || AD_Column_ID == 6652 // AD_Process.Statistics_Count
                   || AD_Column_ID == 6653) // AD_Process.Statistics_Seconds
                return true;

            // Don't log Log entries
            if (CHANGELOGTYPE_Insert.Equals(type)
             && (tableName.IndexOf("Log") != -1
              || AD_Table_ID == X_AD_Session.Table_ID))
                return true;
            //
            return false;





            //if (AD_Table_ID == MChangeLog.Table_ID
            //    || AD_Table_ID == MWindowLog.Table_ID
            //    || AD_Table_ID == MQueryLog.Table_ID
            //    //|| AD_Table_ID == MIssue.Table_ID
            //    || AD_Column_ID == 6652
            //    || AD_Column_ID == 6653)		//	AD_Process.Statistics_
            //    return true;
            ////	Don't log Log entries
            //if (CHANGELOGTYPE_Insert.Equals(type)
            //    && (tableName.IndexOf("Log") != -1
            //        || AD_Table_ID == MSession.Table_ID))
            //    return true;
            ////
            //return false;
        }	//	isNotLogged

        /// <summary>
        /// 	Fill Log with tables to be logged 
        /// </summary>
        private static void FillChangeLog()
        {
            List<int> list = new List<int>(40);
            String sql = "SELECT t.AD_Table_ID ,t.ChangeLogLevel FROM AD_Table t "
                + "WHERE (t.IsChangeLog='Y' AND (t.ChangeLogLevel='A' or t.ChangeLogLevel='U') )"					//	also inactive
                + " OR EXISTS (SELECT * FROM AD_Column c "
                    + "WHERE t.AD_Table_ID=c.AD_Table_ID AND c.ColumnName='EntityType') "
                + "ORDER BY t.AD_Table_ID";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                int totalCount = dt.Rows.Count;
                for (int i = 0; i < totalCount; i++)
                {
                    DataRow dr = dt.Rows[i];
                    String changeLogLevel = (String)dr[1];
                    int AD_Table_ID = Convert.ToInt32(dr[0]);
                    if (changeLogLevel.Equals("A"))
                    {
                        changeLogAll.Add(AD_Table_ID, AD_Table_ID);
                    }
                    else if (changeLogLevel.Equals("U"))
                    {
                        changeLogUpdate.Add(AD_Table_ID, AD_Table_ID);
                    }
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);

            }
            finally
            {
                dt = null;
            }
            ////	Convert to Array
            //changeLog = new int[list.Count];
            //for (int i = 0; i < changeLog.Length; i++)
            //{
            //    int id = list[i];
            //    changeLog[i] = int.Parse(id.ToString());
            //}
            _log.Info("#" + changeLogAll.Count);

        }	//	fillChangeLog

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName"></param>
        public MChangeLog(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// 	Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_ChangeLog_ID"></param>
        /// <param name="trxName"></param>
        public MChangeLog(Ctx ctx, int AD_ChangeLog_ID, Trx trxName)
            : base(ctx, 0, trxName)
        {

            if (AD_ChangeLog_ID == 0)
            {
                int AD_Role_ID = ctx.GetAD_Role_ID();
                SetAD_Role_ID(AD_Role_ID);
                SetRecord_ID(0);
            }
        }	//	MChangeLog

        /// <summary>
        /// Full Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_ChangeLog_ID">AD_ChangeLog_ID 0 for new change log</param>
        /// <param name="TrxName">TrxName change transaction name</param>
        /// <param name="AD_Session_ID">AD_Session_ID session</param>
        /// <param name="AD_Table_ID">AD_Table_ID table</param>
        /// <param name="AD_Column_ID">column</param>
        /// <param name="keyInfo">keyInfo record key(s)</param>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_Org_ID">org</param>
        /// <param name="OldValue">old</param>
        /// <param name="NewValue">new</param>
        public MChangeLog(Ctx ctx, int AD_ChangeLog_ID, String trxName,
        int AD_Session_ID,
        int AD_Table_ID, int AD_Column_ID, Object keyInfo,
        int AD_Client_ID, int AD_Org_ID,
        Object oldValue, Object newValue)
            : base(ctx, 0, null)
        {
            //this (ctx, 0, null);	//	 out of trx
            if (AD_ChangeLog_ID == 0)
            {
                AD_ChangeLog_ID = DataBase.DB.GetNextID(AD_Client_ID, Table_Name, null);
                if (AD_ChangeLog_ID <= 0)
                {
                    log.Severe("No NextID (" + AD_ChangeLog_ID + ")");
                }
            }
            SetAD_ChangeLog_ID(AD_ChangeLog_ID);
            SetTrxName(trxName);
            SetAD_Session_ID(AD_Session_ID);
            //
            SetAD_Table_ID(AD_Table_ID);
            SetAD_Column_ID(AD_Column_ID);
            //	Key
            if (keyInfo == null)
            {
                log.Severe("No Key Info");
            }
            if (keyInfo.GetType() == typeof(int))
                SetRecord_ID(int.Parse(keyInfo.ToString()));
            else
                SetRecord2_ID(keyInfo.ToString());
            //
            SetClientOrg(AD_Client_ID, AD_Org_ID);
            //
            SetOldValue(oldValue);
            SetNewValue(newValue);

        }	//	MChangeLog

        /// <summary>
        /// Set Old Value
        /// </summary>
        /// <param name="oldValue"></param>
        public void SetOldValue(Object oldValue)
        {
            if (oldValue == null)
                base.SetOldValue(NULL);
            else
                base.SetOldValue(oldValue.ToString());
        }	//	setOldValue

        /// <summary>
        /// 	Is Old Value Null
        /// </summary>
        /// <returns>true if null</returns>
        public Boolean IsOldNull()
        {
            String value = GetOldValue();
            return value == null || value.Equals(NULL);
        }	//	isOldNull

        /// <summary>
        /// 	Set New Value
        /// </summary>
        /// <param name="NewValue">new</param>
        public void SetNewValue(Object newValue)
        {
            if (newValue == null)
                base.SetNewValue(NULL);
            else
                base.SetNewValue(newValue.ToString());
        }	//	setNewValue


        /// <summary>
        /// Is New Value Null
        /// </summary>
        /// <returns>true if null</returns>
        public Boolean IsNewNull()
        {
            String value = GetNewValue();
            return value == null || value.Equals(NULL);
        }	//	isNewNull

        /// <summary>
        /// Get Record2_ID (not null)
        /// </summary>
        /// <returns>record key or ""</returns>
        public new String GetRecord2_ID()
        {
            String s = base.GetRecord2_ID();
            if (s == null)
                return "";
            return s;
        }	//	getRecord2_ID

    }
}
