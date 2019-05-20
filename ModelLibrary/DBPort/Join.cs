using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;

namespace VAdvantage.DBPort
{
    //
    public class Join
    {
        /// <summary>
        /// std constructor
        /// </summary>
        /// <param name="joinClause"></param>
        public Join(String joinClause)
        {
            if (joinClause == null)
                throw new ArgumentException("Join - clause cannot be null");
            Evaluate(joinClause);
        }   //  Join


        private String m_joinClause;
        private String m_mainTable;
        private String m_mainAlias;
        private String m_joinTable;
        private String m_joinAlias;
        private bool m_left;
        private String m_condition;

        /// <summary>
        ///  Evaluate the clause.
        ///  e.g.    tb.AD_User_ID(+)=?
        ///          f.AD_Column_ID = c.AD_Column_ID(+)
        /// </summary>
        /// <param name="joinClause">joinClause</param>
        private void Evaluate(String joinClause)
        {
            m_joinClause = joinClause;
            int indexEqual = joinClause.IndexOf('=');
            m_left = indexEqual < joinClause.IndexOf("(+)");    //  converts to LEFT if true
            //  get table alias of it
            if (m_left)     //  f.AD_Column_ID = c.AD_Column_ID(+)  => f / c
            {
                m_mainAlias = joinClause.Substring
                    (0, Util.FindIndexOf(joinClause, '.', '=')).Trim();          //  f
                int end = joinClause.IndexOf('.', indexEqual);
                if (end == -1)  //  no alias
                    end = joinClause.IndexOf('(', indexEqual);
                m_joinAlias = joinClause.Substring(indexEqual + 1, end).Trim();  //  c
            }
            else            //  f.AD_Column_ID(+) = c.AD_Column_ID  => c / f
            {
                int end = joinClause.IndexOf('.', indexEqual);
                if (end == -1)  //  no alias
                    end = joinClause.Length;
                m_mainAlias = joinClause.Substring(indexEqual + 1, end).Trim();  //  c
                m_joinAlias = joinClause.Substring
                    (0, Util.FindIndexOf(joinClause, '.', '(')).Trim();          //  f
            }
            m_condition = Util.Replace(joinClause, "(+)", "").Trim();
        }   //  evaluate

        /// <summary>
        ///Get origial Join Clause.
        ///  e.g. f.AD_Column_ID = c.AD_Column_ID(+)
        /// </summary>
        /// <returns>Join cluase</returns>
        public String GetJoinClause()
        {
            return m_joinClause;
        }   //  getJoinClause

        /// <summary>
        ///  Get Main Table Alias
        /// </summary>
        /// <returns>Main Table Alias</returns>
        public String GetMainAlias()
        {
            return m_mainAlias;
        }   //  getMainAlias

        /// <summary>
        /// Get Join Table Alias
        /// </summary>
        /// <returns>return Join Table Alias</returns>
        public String GetJoinAlias()
        {
            return m_joinAlias;
        }   //  getJoinAlias

        /// <summary>
        /// Is Left Aouter Join
        /// </summary>
        /// <returns>true if left outer join</returns>
        public bool IsLeft()
        {
            return m_left;
        }   //  isLeft

        /// <summary>
        /// Get Join condition.
        ///  e.g. f.AD_Column_ID = c.AD_Column_ID
        /// </summary>
        /// <returns>join condition</returns>
        public String GetCondition()
        {
            return m_condition;
        }   //  getCondition

        /// <summary>
        /// Set Main Table Name.
        ///  If table name equals alias, the alias is set to ""
        /// </summary>
        /// <param name="mainTable">mainTable</param>

        public void SetMainTable(String mainTable)
        {
            if (mainTable == null || mainTable.Length == 0)
                return;
            m_mainTable = mainTable;
            if (m_mainAlias.Equals(mainTable))
                m_mainAlias = "";
        }   //  setMainTable

        /// <summary>
        /// Get Main Table Name
        /// </summary>
        /// <returns>Main Table Name</returns>
        public String GetMainTable()
        {
            return m_mainTable;
        }   //  getMainTable

        /// <summary>
        /// Set Main Table Name.
        ///  If table name equals alias, the alias is set to ""
        /// </summary>
        /// <param name="joinTable">joinTable</param>
        public void SetJoinTable(String joinTable)
        {
            if (joinTable == null || joinTable.Length == 0)
                return;
            m_joinTable = joinTable;
            if (m_joinAlias.Equals(joinTable))
                m_joinAlias = "";
        }

        /// <summary>
        /// Get Join Table Name
        /// </summary>
        /// <returns>Join Table Name</returns>
        public String GetJoinTable()
        {
            return m_joinTable;
        }   //  getJoinTable

        /// <summary>
        ///This Join is a condition of the first Join.
        ///  e.g. tb.AD_User_ID(+)=?  or tb.AD_User_ID(+)='123'
        /// </summary>
        /// <param name="first">first</param>
        /// <returns>true if condition</returns>
        public bool IsConditionOf(Join first)
        {
            if (m_mainTable == null         //  did not find Table from "Alias"
                && (first.GetJoinTable().Equals(m_joinTable)        //  same join table
                    || first.GetMainAlias().Equals(m_joinTable)))   //  same main table
                return true;
            return false;
        }   //  isConditionOf

        /**
         *  String representation
         *  @return info
         */
        public override String ToString()
        { 
            StringBuilder sb = new StringBuilder("Join[");
            sb.Append(m_joinClause)
                .Append(" - Main=").Append(m_mainTable).Append("/").Append(m_mainAlias)
                .Append(", Join=").Append(m_joinTable).Append("/").Append(m_joinAlias)
                .Append(", Left=").Append(m_left)
                .Append(", Condition=").Append(m_condition)
                .Append("]");
            return sb.ToString();
        }   //  toString

    }   //  Join
}