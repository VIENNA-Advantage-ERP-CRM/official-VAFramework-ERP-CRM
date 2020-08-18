using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Common;
using VAdvantage.Classes;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.DataBase;

using VAdvantage.ProcessEngine;
using VAdvantage.Logging;

namespace VAdvantage.Process
{
    [Serializable]
    public class PO_LOB
    {
       private static VLogger log = VLogger.GetVLogger(typeof(PO_LOB).FullName);


        /**	Table Name				*/
        private String _strTableName;
        /** Column Name				*/
        private String _strColumnName;
        /** Where Clause			*/
        private String _strWhereClause;

        /** Display Type			*/
        private int _iDisplayType;
        /** Data					*/
        private Object _objValue;

        public PO_LOB(String strTableName, String strColumnName, String strWhereClause,
            int iDisplayType, Object objValue)
        {
            _strTableName = strTableName;
            _strColumnName = strColumnName;
            _strWhereClause = strWhereClause;
            _iDisplayType = iDisplayType;
            _objValue = objValue;
        }

        public bool Save(String strWhereClause, Trx trxName)
        {
            _strWhereClause = strWhereClause;
            return Save(trxName);
        }

        public bool Save(Trx trxName)
        {
            StringBuilder sql;
            if (_objValue == null
                || (!(_objValue.GetType() == typeof(string) || _objValue.GetType() == typeof(byte[])))
                || (_objValue.GetType() == typeof(string) && _objValue.ToString().Length == 0)
                || (_objValue.GetType() == typeof(byte[]) && ((byte[])_objValue).Length == 0)
                )
            {
                sql = new StringBuilder("UPDATE ")
                    .Append(_strTableName)
                    .Append(" SET ").Append(_strColumnName)
                    .Append("=NULL WHERE ").Append(_strWhereClause);

                int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, trxName);
                if (no == 0)
                {
                    log.Warning("No Updated");
                }
                return true;
            }

            sql = new StringBuilder("UPDATE ")
                .Append(_strTableName)
                .Append(" SET ").Append(_strColumnName)
                .Append("=@objValue WHERE ").Append(_strWhereClause);
            //
            bool success = true;

            DataBase.Trx trx = null;
            if (trxName != null)
                trx = trxName;
            IDbConnection con = null;
            //	Create Connection
            if (trx != null)
                con = trx.GetConnection();
            if (con == null)
                con = DB.GetConnection();
            if (con == null)
            {
                log.Log(Level.SEVERE, "Could not get Connection");
                return false;
            }

            SqlParameter[] param = new SqlParameter[1];
            try
            {
                if (_iDisplayType == DisplayType.TextLong)
                    param[0] = new SqlParameter("@objValue", (string)_objValue);
                else
                    param[0] = new SqlParameter("@objValue", (byte[])_objValue);
                int no = DataBase.DB.ExecuteQuery(sql.ToString(), param, trxName);
                if (no != 1)
                {
                    success = false;
                }
            }
            catch 
            {
                success = false;
            }


            //	Success - commit local trx
            if (success)
            {
                if (trx != null)
                {
                    trx = null;
                    con = null;
                }
                else
                {
                    try
                    {
                        
                       //trx.Commit();
                        con.Close();
                        con = null;
                    }
                    catch (Exception e)
                    {
                       log.Log(Level.SEVERE, "[" + trxName + "] - commit ", e);
                        success = false;
                    }
                }
            }
            //	Error - roll back
            if (!success)
            {
                log.Severe("[" + trxName + "] - rollback");
                if (trx != null)
                {
                    trx.Rollback();
                    trx = null;
                    con = null;
                }
                else
                {
                    try
                    {
                        //con.rollback();
                        con.Close();
                        con = null;
                    }
                    catch (Exception ee)
                    {
                        log.Log(Level.SEVERE, "[" + trxName + "] - rollback", ee);
                    }
                }
            }

            //	Clean Connection
            try
            {
                if (con != null)
                    con.Close();
                con = null;
            }
            catch 
            {
                con = null;
            }



            return success;
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("PO_LOB[");
            sb.Append(_strTableName).Append(".").Append(_strColumnName)
                .Append(",iDisplayType=").Append(_iDisplayType)
                .Append("]");
            return sb.ToString();
        }

        public Object GetobjValue()
        {
            return _objValue;
        }

        public String GetSQL()
        {
            String sql = "UPDATE " + _strTableName + " SET " + _strColumnName +
                        "=@param1 WHERE " + _strWhereClause;
            return sql;
        }

        public int GetiDisplayType()
        {
            return _iDisplayType;
        }

        public String GetstrColumnName()
        {
            return _strColumnName;
        } 
    }
}
