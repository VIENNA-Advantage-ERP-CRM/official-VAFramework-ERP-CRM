/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRecordAccess
 * Purpose        : Record Access Model
 * Class Used     : X_AD_Record_Access
 * Chronological    Development
 * Raghunandan     05-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MRecordAccess : X_AD_Record_Access
    {
        #region Private Variables
       // private static long serialVersionUID = -5115765616266528435L;
        //	Key Column Name	
        private String _keyColumnName = null;
        //TableName	
        private String _tableName;
        #endregion

        /// <summary>
        /// Persistency Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="ignored"></param>
        /// <param name="trxName"></param>
        public MRecordAccess(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, 0, trxName)
        {
            if (ignored != 0)
            {
                throw new ArgumentException("Multi-Key");
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MRecordAccess(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MRecordAccess(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Full New Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Role_ID"></param>
        /// <param name="AD_Table_ID"></param>
        /// <param name="Record_ID"></param>
        /// <param name="trxName"></param>
        public MRecordAccess(Ctx ctx, int AD_Role_ID, int AD_Table_ID, int Record_ID, Trx trxName)
            : base(ctx, 0, trxName)
        {
            SetAD_Role_ID(AD_Role_ID);
            SetAD_Table_ID(AD_Table_ID);
            SetRecord_ID(Record_ID);
            SetIsExclude(true);
            SetIsReadOnly(false);
            SetIsDependentEntities(false);
        }

        /// <summary>
        /// Get Key Column Name
        /// </summary>
        /// <returns>Key Column Name</returns>
        public String GetKeyColumnName()
        {
            if (_keyColumnName != null)
            {
                return _keyColumnName;
            }
            //
            String sql = "SELECT ColumnName "
                + "FROM AD_Column "
                + "WHERE AD_Table_ID=" + GetAD_Table_ID() + " AND IsKey='Y' AND IsActive='Y'";
            IDataReader idr = null;
            try
            {

                idr = DB.ExecuteReader(sql);
                while (idr.Read())
                {
                    String s = Util.GetValueOfString(idr[0]);
                    if (_keyColumnName == null)
                    {
                        _keyColumnName = s;
                    }
                    else
                    {
                        log.Log(Level.SEVERE, "More than one key = " + s);
                    }
                }
                idr.Close();
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }

            if (_keyColumnName == null)
            {
                log.Log(Level.SEVERE, "Record Access requires Table with one key column");
            }
            return _keyColumnName;
        }

        /// <summary>
        /// Get Synonym of Column
        /// </summary>
        /// <returns>Synonym Column Name</returns>
        public String GetSynonym()
        {
            if ("AD_User_ID".Equals(GetKeyColumnName()))
            {
                return "SalesRep_ID";
            }
            else if ("C_ElementValue_ID".Equals(GetKeyColumnName()))
            {
                return "Account_ID";
            }
            return null;
        }

        /// <summary>
        /// Key Column has a Synonym
        /// </summary>
        /// <returns>true if Key Column has Synonym</returns>
        public bool IsSynonym()
        {
            return GetSynonym() == null;
        }

        /// <summary>
        /// Is Read Write
        /// </summary>
        /// <returns>rw - false if exclude</returns>
        public bool IsReadWrite()
        {
            if (IsExclude())
            {
                return false;
            }
            return !base.IsReadOnly();
        }

        /// <summary>
        /// Get Key Column Name with consideration of Synonym
        /// </summary>
        /// <param name="tableInfo">tableInfo</param>
        /// <returns>key column name</returns>
        public String GetKeyColumnName(AccessSqlParser.TableInfo[] tableInfo)
        {
            String columnSyn = GetSynonym();
            if (columnSyn == null)
            {
                return _keyColumnName;
            }
            //	We have a synonym - ignore it if base table inquired
            for (int i = 0; i < tableInfo.Length; i++)
            {
                if (_keyColumnName.Equals("AD_User_ID"))
                {
                    //	List of tables where not to use SalesRep_ID
                    if (tableInfo[i].GetTableName().Equals("AD_User"))
                        return _keyColumnName;
                }
                else if (_keyColumnName.Equals("AD_ElementValue_ID"))
                {
                    //	List of tables where not to use Account_ID
                    if (tableInfo[i].GetTableName().Equals("AD_ElementValue"))
                        return _keyColumnName;
                }
            }	//	tables to be ignored
            return columnSyn;
        }
        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MRecordAccess[AD_Role_ID=")
                .Append(GetAD_Role_ID())
                .Append(",AD_Table_ID=").Append(GetAD_Table_ID())
                .Append(",Record_ID=").Append(GetRecord_ID())
                .Append(",Active=").Append(IsActive())
                .Append(",Exclude=").Append(IsExclude())
                .Append(",ReadOnly=").Append(base.IsReadOnly())
                .Append(",Dependent=").Append(IsDependentEntities())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Extended String Representation
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>extended info</returns>
        public String ToStringX(Ctx ctx)
        {
            String en = Msg.GetMsg(ctx, "Include");
            String ex = Msg.GetMsg(ctx, "Exclude");
            StringBuilder sb = new StringBuilder();
            sb.Append(Msg.Translate(ctx, "AD_Table_ID"))
                    .Append("=").Append(GetTableName(ctx)).Append(", ")
                .Append(Msg.Translate(ctx, "Record_ID"))
                .Append("=").Append(GetRecord_ID())
                .Append(" - ").Append(Msg.Translate(ctx, "IsDependentEntities"))
                    .Append("=").Append(IsDependentEntities())
                .Append(" (").Append(Msg.Translate(ctx, "IsReadOnly"))
                    .Append("=").Append(base.IsReadOnly())
                .Append(") - ").Append(IsExclude() ? ex : en);
            return sb.ToString();
        }

        /// <summary>
        /// 	Get Table Name
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>table name</returns>
        public String GetTableName(Ctx ctx)
        {
            if (_tableName == null)
            {
                String sql = "SELECT TableName FROM AD_Table WHERE AD_Table_ID=" + GetAD_Table_ID();
                IDataReader idr = null;
                try
                {
                    idr = DB.ExecuteReader(sql);
                    if (idr.Read())
                    {
                        _tableName = Util.GetValueOfString(idr[0]);
                    }
                    idr.Close();
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, sql, e);
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                }

                //	Get Clear Text
                String realName = Msg.Translate(ctx, _tableName + "_ID");

                if (!realName.Equals(_tableName + "_ID"))
                {
                    _tableName = realName;
                }
            }
            return _tableName;
        }
    }
}
