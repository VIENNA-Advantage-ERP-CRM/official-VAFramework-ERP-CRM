/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : TranslationTable
 * Purpose        : 
 * Chronological    Development
 * Raghunandan      01-May-2009 
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Common;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.SqlExec;
using System.Windows.Forms;

namespace VAdvantage.Model
{
    public class TranslationTable
    {
        #region Private variables
        //Active Translations
        private static int? _activeLanguages = null;
        //Cache
        private static CCache<string, TranslationTable> s_cache = new CCache<string, TranslationTable>("TranslationTable", 20);
        //Translation Table Name	
        private String _trlTableName = null;
        //Base Table Name			
        private String _baseTableName = null;
        // Column Names 		
        private List<String> _columns = new List<String>();
        #endregion

        /// <summary>
        ///Save translation for po
        /// </summary>
        /// <param name="po">persistent object</param>
        /// <param name="newRecord">newRecord new</param>
        /// <returns>true if no active language or translation saved/reset</returns>
        public static bool Save(PO po, bool newRecord)
        {
            if (!TranslationTable.IsActiveLanguages(false))
                return true;
            TranslationTable table = TranslationTable.Get(po.Get_TableName());
            if (newRecord)
                return table.CreateTranslation(po);
            return table.ResetTranslationFlag(po);
        }

        /// <summary>
        ///Are there active Translation Languages
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>true active Translations</returns>
        public static bool IsActiveLanguages(bool requery)
        {
            int no = GetActiveLanguages(requery);
            return no > 0;
        }

        /// <summary>
        ///Get Number of active Translation Languages
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>number of active Translations</returns>
        public static int GetActiveLanguages(bool requery)
        {
            if (_activeLanguages != null && !requery)
                //return _activeLanguages.intValue();
                return (int)_activeLanguages;
            int no = int.Parse(ExecuteQuery.ExecuteScalar("SELECT COUNT(*) FROM AD_Language WHERE IsActive='Y' AND IsSystemLanguage='Y'"));
            _activeLanguages = no;
            return (int)_activeLanguages;
        }

        /// <summary>
        ///Get TranslationTable from Cache
        /// </summary>
        /// <param name="baseTableName">base table name</param>
        /// <returns>TranslationTable</returns>
        public static TranslationTable Get(String baseTableName)
        {
            TranslationTable retValue = (TranslationTable)s_cache[baseTableName];
            if (retValue != null)
                return retValue;
            retValue = new TranslationTable(baseTableName);
            s_cache.Add(baseTableName, retValue);
            return retValue;
        }

        /// <summary>
        ///Translation Table
        /// </summary>
        /// <param name="baseTableName">base table name</param>
        protected TranslationTable(String baseTableName)
        {
            if (baseTableName == null)
                throw new ArgumentException("Base Table Name is null");
            _baseTableName = baseTableName;
            _trlTableName = baseTableName + "_Trl";
            InitColumns();
            //log.fine(toString());
        }

        /// <summary>
        ///Add Translation Columns
        /// </summary>
        private void InitColumns()
        {
            //MTable table = null;
            //MTable table = MTable.get(Env.getCtx(), m_trlTableName);
            MTable table = MTable.Get(Utility.Env.GetContext(), _trlTableName);
            if (table == null)
                throw new ArgumentException("Table Not found=" + _trlTableName);
            MColumn[] columns = table.GetColumns(false);
            for (int i = 0; i < columns.Length; i++)
            {
                MColumn column = columns[i];
                if (column.IsStandardColumn())
                    continue;
                String columnName = column.GetColumnName();
                if (columnName.EndsWith("_ID")
                    || columnName.StartsWith("AD_Language")
                    || columnName.Equals("IsTranslated"))
                    continue;
                //
                _columns.Add(columnName);
            }
            if (_columns.Count == 0)
                throw new ArgumentException("No Columns found=" + _trlTableName);
        }

        /// <summary>
        ///Create Translation record from PO
        /// </summary>
        /// <param name="po">base table record</param>
        /// <returns>true if inserted or no translation</returns>
        public bool CreateTranslation(PO po)
        {
            if (!IsActiveLanguages(false))
                return true;
            if (po.Get_ID() == 0)
                throw new ArgumentException("PO ID is 0");
            //
            StringBuilder sql1 = new StringBuilder();
            sql1.Append("INSERT INTO ").Append(_trlTableName).Append(" (");
            StringBuilder sql2 = new StringBuilder();
            sql2.Append(") SELECT ");

            //	Key Columns
            sql1.Append(_baseTableName).Append("_ID,AD_Language");
            sql2.Append("b.").Append(_baseTableName).Append("_ID,l.AD_Language");

            //	Base Columns
            sql1.Append(", AD_Client_ID,AD_Org_ID,IsActive, Created,CreatedBy,Updated,UpdatedBy, IsTranslated");
            sql2.Append(", b.AD_Client_ID,b.AD_Org_ID,b.IsActive, b.Created,b.CreatedBy,b.Updated,b.UpdatedBy, 'N'");

            for (int i = 0; i < _columns.Count; i++)
            {
                String columnName = (String)_columns[i];
                Object value = po.Get_Value(columnName);
                //
                if (value == null)
                    continue;
                sql1.Append(",").Append(columnName);
                sql2.Append(",b.").Append(columnName);
            }
            //
            StringBuilder sql = new StringBuilder();
            sql.Append(sql1).Append(sql2)
                .Append(" FROM AD_Language l, " + _baseTableName
                    + " b WHERE l.IsActive = 'Y' AND l.IsSystemLanguage = 'Y' AND b."
                    + _baseTableName + "_ID=").Append(po.Get_ID());
            //int no = DataBase.executeUpdate(sql.toString(), po.get_TrxName());
            int no = DB.ExecuteQuery(sql.ToString(), null, po.Get_Trx());
            //log.fine(m_trlTableName + ": ID=" + po.get_ID() + " #" + no);
            return no != 0;
        }

        /// <summary>
        ///Reset Translation Flag
        /// </summary>
        /// <param name="po">po</param>
        /// <returns>true if updated or no translations</returns>
        public bool ResetTranslationFlag(PO po)
        {
            if (!IsActiveLanguages(false))
                return true;
            if (po.Get_ID() == 0)
                throw new ArgumentException("PO ID is 0");
            //
            StringBuilder sb = new StringBuilder("UPDATE ");
            sb.Append(_trlTableName)
                .Append(" SET IsTranslated='N',Updated=SysDate WHERE ")
                .Append(_baseTableName).Append("_ID=").Append(po.Get_ID());
            //int no = DataBase.executeUpdate(sb.toString(), po.get_TrxName());
            int no = DB.ExecuteQuery(sb.ToString(), null, po.Get_TrxName());
            //log.fine(m_trlTableName + ": ID=" + po.get_ID() + " #" + no);
            return no != 0;
        }

        /// <summary>
        ///Delete translation for po
        /// </summary>
        /// <param name="po">persistent object</param>
        /// <returns>true if no active language or translation deleted</returns>
        public static Boolean Delete(PO po)
        {
            if (!TranslationTable.IsActiveLanguages(false))
                return true;
            TranslationTable table = TranslationTable.Get(po.Get_TableName());
            return table.DeleteTranslation(po);
        }

        /// <summary>
        ///Delete Translation
        /// </summary>
        /// <param name="po">po</param>
        /// <returns>true if udeleted or no translations</returns>
        public Boolean DeleteTranslation(PO po)
        {
            if (!IsActiveLanguages(false))
                return true;
            if (po.Get_IDOld() == 0)
                throw new ArgumentException("PO Old ID is 0");
            StringBuilder sb = new StringBuilder("DELETE FROM ");
            sb.Append(_trlTableName)
                .Append(" WHERE ")
                .Append(_baseTableName).Append("_ID=").Append(po.Get_IDOld());

            int no = DB.ExecuteQuery(sb.ToString(), null, po.Get_TrxName());
            // int no = int.Parse(ExecuteQuery.ExecuteScalar(sb.ToString()));//, po.get_TrxName());
            //log.fine(m_trlTableName + ": ID=" + po.get_IDOld() + " #" + no);
            return no != 0;
        }

        /// <summary>
        ///String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("TranslationTable[");
            sb.Append(_trlTableName)
                .Append("(").Append(_baseTableName).Append(")");
            for (int i = 0; i < _columns.Count; i++)
                sb.Append("-").Append(_columns[i]);
            sb.Append("]");
            return sb.ToString();
        }
    }
}
