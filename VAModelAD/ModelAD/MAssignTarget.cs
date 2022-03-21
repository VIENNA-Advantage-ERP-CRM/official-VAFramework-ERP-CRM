/********************************************************
 * Module Name    : Model
 * Purpose        : Assign Target Model
                    
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
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MAssignTarget : X_AD_AssignTarget
    {
        public MAssignTarget(Ctx ctx, int AD_AssignTarget_ID, Trx trxName)
            : base(ctx, AD_AssignTarget_ID, trxName)
        {

        }	//	MAssignTarget

        public MAssignTarget(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }	//	MAssignTarget

        /**	The Criteria Lines					*/
        private MAssignCriteria[] m_criteria = null;
        /** The Target Column					*/
        private MColumn m_column = null;

        /// <summary>
        /// Get Criteria Lines
        /// </summary>
        /// <param name="reload">reload data</param>
        /// <returns>array of lines</returns>
        public MAssignCriteria[] GetCriteria(bool reload)
        {
            if (m_criteria != null && !reload)
                return m_criteria;
            String sql = "SELECT * FROM AD_AssignCriteria "
                + "WHERE AD_AssignTarget_ID=@AD_AssignTarget_ID ORDER BY SeqNo";
            List<MAssignCriteria> list = new List<MAssignCriteria>();
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@AD_AssignTarget_ID", GetAD_AssignTarget_ID());
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MAssignCriteria(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE , sql, e);
            }
            //
            m_criteria = new MAssignCriteria[list.Count];
            m_criteria = list.ToArray();
            return m_criteria;
        }	//	getCriteria


        /// <summary>
        /// Get Target Column
        /// </summary>
        /// <returns>target column</returns>
        public MColumn GetTargetColumn()
        {
            if (m_column == null || m_column.GetAD_Column_ID() != GetAD_TargetColumn_ID())
                m_column = MColumn.Get(GetCtx(), GetAD_TargetColumn_ID());
            return m_column;
        }	//	getTargetColumn


        /// <summary>
        /// Execute Auto Assignment
        /// </summary>
        /// <param name="po">PO to be modified</param>
        /// <returns>true if modified</returns>
        public bool ExecuteIt(PO po)
        {
            //	Check Column
            MColumn column = GetTargetColumn();
            String columnName = column.GetColumnName();
            int index = po.Get_ColumnIndex(columnName);
            if (index == -1)
                throw new Exception(ToString() + ": AD_Column_ID not found");
            //	Check Value
            Object value = po.Get_Value(index);
            String assignRule = GetAssignRule();
            if (value == null && assignRule.Equals(ASSIGNRULE_OnlyIfNOTNULL))
                return false;
            else if (value != null && assignRule.Equals(ASSIGNRULE_OnlyIfNULL))
                return false;

            //	Check Criteria
            if (m_criteria == null)
                GetCriteria(false);
            bool modified = false;
            for (int i = 0; i < m_criteria.Length; i++)
            {
                MAssignCriteria criteria = m_criteria[i];
                if (criteria.IsMet(po))
                {
                    modified = true;
                    break;
                }
            }
            if (!modified)
                return false;

            //	Assignment
            String methodName = "set" + columnName;
            Type parameterType = null;
            Object parameter = null;
            int displayType = column.GetAD_Reference_ID();
            String valueString = GetValueString();
            if (DisplayType.IsText(displayType) || displayType == DisplayType.List)
            {
                parameterType = typeof(string);
                parameter = valueString;
            }
            else if (DisplayType.IsID(displayType) || displayType == DisplayType.Integer)
            {
                parameterType = typeof(int);
                if (GetRecord_ID() != 0)
                    parameter = GetRecord_ID();
                else if (valueString != null && valueString.Length > 0)
                {
                    try
                    {
                        parameter = int.Parse(valueString);
                    }
                    catch (Exception e)
                    {
                        log.Warning(ToString() + " " + e);
                        return false;
                    }
                }
            }
            else if (DisplayType.IsNumeric(displayType))
            {
                parameterType = typeof(Decimal);
                if (valueString != null && valueString.Length > 0)
                {
                    try
                    {
                        parameter = Decimal.Parse(valueString);
                    }
                    catch (Exception e)
                    {
                        log.Warning(ToString() + " " + e);
                        return false;
                    }
                }
            }
            else if (DisplayType.IsDate(displayType))
            {
                parameterType = typeof(DateTime);
                if (valueString != null && valueString.Length > 0)
                {
                    try
                    {
                        parameter = DateTime.Parse(valueString);
                    }
                    catch (Exception e)
                    {
                        log.Warning(ToString() + " " + e);
                        return false;
                    }
                }
            }
            else if (displayType == DisplayType.YesNo)
            {
                parameterType = typeof(bool);
                parameter = "Y".Equals(valueString);
            }
            else if (displayType == DisplayType.Button)
            {
                parameterType = typeof(string);
                parameter = GetValueString();
            }
            else if (DisplayType.IsLOB(displayType))	//	CLOB is String
            {
                parameterType = typeof(byte[]);
                //	parameter = getValueString();
            }

            //	Assignment
            try
            {

                Type clazz = po.GetType();
                System.Reflection.MethodInfo method = clazz.GetMethod(methodName, new Type[] { parameterType });
                method.Invoke(po, new Object[] { parameter });
            }
            catch (Exception e)
            {
                log.Log(Level.WARNING, ToString(), e);
                //	fallback
                if (parameter is Boolean)
                    po.Set_Value(index, valueString);
                else
                    po.Set_Value(index, parameter);
                //	modified = false;
            }
            return modified;
        }	//	executeIt

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("MAssignTarget[")
                .Append(Get_ID())
                .Append(",AD_TargetColumn_ID=").Append(GetAD_TargetColumn_ID());
            if (GetRecord_ID() != 0)
                sb.Append(",Record_ID=").Append(GetRecord_ID());
            if (GetValueString() != null)
                sb.Append(",ValueString=").Append(GetValueString());
            sb.Append("]");
            return sb.ToString();            
        }
    }
}
