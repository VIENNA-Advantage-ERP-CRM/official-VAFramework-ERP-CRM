/********************************************************
 * Module Name    : Model
 * Purpose        : Asssign Criteria Model
                    
 * Class Used     : -----
 * Created By     : Jagmohan 
 * Date           : 6-aug-2009
**********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MAssignCriteria : X_AD_AssignCriteria
    {
        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param AD_AssignCriteria_ID id
         *	@param trxName trx
         */
        public MAssignCriteria(Ctx ctx, int AD_AssignCriteria_ID, Trx trxName)
            : base(ctx, AD_AssignCriteria_ID, trxName)
        {

        }	//	MAssignCriteria

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param rs result set
         *	@param trxName trx
         */
        public MAssignCriteria(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }	//	MAssignCriteria

        /** The Source Column					*/
        private MColumn m_column = null;

        /**
         * 	Get Source Column
         *	@return source column
         */
        public MColumn GetSourceColumn()
        {
            if (m_column == null || m_column.GetAD_Column_ID() != GetAD_SourceColumn_ID())
                m_column = MColumn.Get(GetCtx(), GetAD_SourceColumn_ID());
            return m_column;
        }	//	GetSourceColumn

        /**
         * 	Is Criteria Met
         *	@param po po
         *	@return true if criteria is met
         */
        public bool IsMet(PO po)
        {
            MColumn column = GetSourceColumn();
            String columnName = column.GetColumnName();
            int index = po.Get_ColumnIndex(columnName);
            if (index == -1)
                throw new Exception(ToString() + ": AD_Column_ID not found");
            //	Get Value
            Object value = po.Get_Value(index);
            String op = GetOperation();
            //	Compare Value
            String compareString = GetValueString();
            if (op.Equals(OPERATION_Sql))
            {
                compareString = GetSQLValue();
                op = OPERATION_Eq;
            }
            //	NULL handling
            if (value == null)
            {
                if (compareString == null
                    || compareString.Length == 0
                    || compareString.Equals("NULL", StringComparison.OrdinalIgnoreCase))
                {
                    if (op.Equals(OPERATION_Eq))
                        return true;
                }
                else
                {
                    if (!op.Equals(OPERATION_Eq))
                        return true;
                }
                return false;
            }
            if (GetRecord_ID() == 0		//	no value to compare to
                && (compareString == null || compareString.Length == 0))
            {
                return false;
            }

            //	Like - String
            if (op.Equals(OPERATION_Like))
            {
                String s = value.ToString();
                String cmp = compareString;
                if (cmp.IndexOf('%') != -1)		//	SQL Like
                {
                    log.Warning(ToString() + ": SQL LIKE not supported yet");
                    //TODO: SQL Like
                }
                return s.ToUpper()
                    .IndexOf(cmp.ToUpper()) != 0;	//	substring
            }

            try
            {
                if (value is int)
                {
                    int ii = (int)value;
                    int? cmp = null;
                    if (GetRecord_ID() > 0)
                        cmp = GetRecord_ID();
                    else
                        cmp = (int)int.Parse(compareString);
                    //	Tree Handling
                    bool? treeOp = (bool?)TreeOperation(columnName, cmp, op, ii, po.GetAD_Client_ID());
                    if (treeOp != null)
                        return treeOp.Value;
                    //
                    if (op.Equals(OPERATION_Eq))
                        return ii.Equals(cmp);
                    else if (op.Equals(OPERATION_NotEq))
                        return !ii.Equals(cmp);
                    else if (op.Equals(OPERATION_Gt))
                        return ii.CompareTo(cmp) > 0;
                    else if (op.Equals(OPERATION_GtEq))
                        return ii.CompareTo(cmp) >= 0;
                    else if (op.Equals(OPERATION_Le))
                        return ii.CompareTo(cmp) < 0;
                    else if (op.Equals(OPERATION_LeEq))
                        return ii.CompareTo(cmp) <= 0;
                }
                else if (value is Decimal)
                {
                    Decimal bd = (Decimal)value;
                    Decimal cmp = decimal.Parse(compareString);
                    if (op.Equals(OPERATION_Eq))
                        return bd.Equals(cmp);
                    else if (op.Equals(OPERATION_NotEq))
                        return !bd.Equals(cmp);
                    else if (op.Equals(OPERATION_Gt))
                        return bd.CompareTo(cmp) > 0;
                    else if (op.Equals(OPERATION_GtEq))
                        return bd.CompareTo(cmp) >= 0;
                    else if (op.Equals(OPERATION_Le))
                        return bd.CompareTo(cmp) < 0;
                    else if (op.Equals(OPERATION_LeEq))
                        return bd.CompareTo(cmp) <= 0;
                }
                else if (value is DateTime)
                {
                    DateTime? ts = (DateTime?)value;
                    DateTime cmp = DateTime.Parse(compareString);
                    if (op.Equals(OPERATION_Eq))
                    {
                        return ts.Equals(cmp);
                    }
                    else if (op.Equals(OPERATION_NotEq))
                    {
                        return !ts.Equals(cmp);
                    }
                    else if (op.Equals(OPERATION_Gt))
                    {
                        return ts.Value.CompareTo(cmp) > 0;
                    }
                    else if (op.Equals(OPERATION_GtEq))
                    {
                        return ts.Value.CompareTo(cmp) >= 0;
                    }
                    else if (op.Equals(OPERATION_Le))
                    {
                        return ts.Value.CompareTo(cmp) < 0;
                    }
                    else if (op.Equals(OPERATION_LeEq))
                    {
                        return ts.Value.CompareTo(cmp) <= 0;
                    }
                }
                else
                // String
                {
                    String s = value.ToString();
                    String cmp = compareString;
                    if (op.Equals(OPERATION_Eq))
                        return s.Equals(cmp);
                    else if (op.Equals(OPERATION_NotEq))
                        return !s.Equals(cmp);
                    else if (op.Equals(OPERATION_Gt))
                        return s.CompareTo(cmp) > 0;
                    else if (op.Equals(OPERATION_GtEq))
                        return s.CompareTo(cmp) >= 0;
                    else if (op.Equals(OPERATION_Le))
                        return s.CompareTo(cmp) < 0;
                    else if (op.Equals(OPERATION_LeEq))
                        return s.CompareTo(cmp) <= 0;
                }
            }
            catch (Exception e)
            {
                log.Warning(ToString() + ": " + e);
            }
            return false;
        }	//	isMet

        /**
         * 	Get SQL Value
         *	@return sql value
         */
        private String GetSQLValue()
        {
            String sql = GetValueString();
            String retValue = null;
            IDataReader dr=null;
            try
            {
                 dr = DataBase.DB.ExecuteReader(sql);
                if (dr.Read())
                {
                    retValue = dr[0].ToString();
                    if (dr.NextResult())
                    {
                        log.Warning(ToString() + ": More than one sql value");
                    }
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }

                log.Log(Level.WARNING, ToString(), e);
            }
            return retValue;
        }	//	GetSQLValue

        /**
         * 	Tree Operation
         *	@param columnName columnName
         *	@param cmp compare value
         *	@param op operation (only == or !=)
         *	@param value user value
         *	@return null if n/a otherwise evaluation
         */
        private Boolean? TreeOperation(String columnName, int? cmp, String op, int value, int AD_Client_ID)
        {
            String tableName = null;
            //	Is this a Tree capable column
            if (columnName.EndsWith("_ID")
                && (op.Equals(OPERATION_Eq) || op.Equals(OPERATION_NotEq)))
            {
                String temp = columnName;
                if (temp.EndsWith("_ID"))
                    temp = columnName.Substring(0, columnName.Length - 3);
                if (MTree.HasTree(temp,GetCtx()))
                    tableName = temp;
            }
            if (tableName == null)
                return null;

            //	Is the value a Summary node
            StringBuilder sql = new StringBuilder("SELECT ").Append(columnName)
                .Append(" FROM ").Append(tableName)
                .Append(" WHERE ").Append(columnName).Append("=@param1 AND IsSummary='Y'");
            int id = DataBase.DB.GetSQLValue(null, sql.ToString(), Utility.Util.GetValueOfInt(cmp));
            if (id <= 0)
                return null;

            //	Get Tree
            int AD_Tree_ID = MTree.GetDefaultAD_Tree_ID(AD_Client_ID, tableName);
            if (AD_Tree_ID <= 0)
                return null;

            MTree tree = new MTree(GetCtx(), AD_Tree_ID, false, true, null);
            //VTreeNode node = tree.GetRootNode().findNode(id);
            //log.Finest("Root=" + node);
            //
            //if (node != null && node.IsSummary)
            //{
                //Enumeration<?> en = node.preorderEnumeration();
                //while (en.hasMoreElements())
                //{
                //    CTreeNode nn = (CTreeNode)en.nextElement();
                //    if (!nn.isSummary())
                //    {
                //        int cmp1 = nn.GetNode_ID();
                //        if (op.Equals(OPERATION_Eq))
                //        {
                //            if (value.Equals(cmp1))
                //                return Boolean.TRUE;
                //        }
                //        else if (op.Equals(OPERATION_NotEq))
                //        {
                //            if (!value.Equals(cmp1))
                //                return Boolean.TRUE;
                //        }
                //    }
                //}
            //}	//	tree elements
            return false;
        }	//	treeOperation


        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("MAssignCriteria[")
                .Append(Get_ID())
                .Append("-").Append(GetSeqNo())
                .Append(",AD_SourceColumn_ID=").Append(GetAD_SourceColumn_ID())
                .Append(",Operation=").Append(GetOperation());
            if (GetRecord_ID() != 0)
                sb.Append(",Record_ID=").Append(GetRecord_ID());
            if (GetValueString() != null)
                sb.Append(",ValueString=").Append(GetValueString());
            sb.Append("]");
            return sb.ToString();            
        }
    }
}
