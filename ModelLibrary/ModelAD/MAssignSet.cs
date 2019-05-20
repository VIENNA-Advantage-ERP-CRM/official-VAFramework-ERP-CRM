/********************************************************
 * Module Name    : Model
 * Purpose        : Assign Set Model
                    
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
    public class MAssignSet : X_AD_AssignSet
    {
        	/**	Logger			*/
        private static VLogger s_log = VLogger.GetVLogger(typeof(MAssignSet).FullName);
        /// <summary>
        /// Get all Assignments 
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <returns>Assognment array</returns>
        static public MAssignSet[] GetAll(Ctx ctx)
        {
            List<MAssignSet> list = new List<MAssignSet>();
            String sql = "SELECT * FROM AD_AssignSet";
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MAssignSet(ctx, dr, null));
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql,e);
            }

            MAssignSet[] retValue = new MAssignSet[list.Count];
            retValue = list.ToArray();
            return retValue;
        }	//	getAll

        /// <summary>
        /// Execute Auto Assignment
        /// </summary>
        /// <param name="po">PO to be modified</param>
        /// <param name="newRecord">new</param>
        /// <returns>true if modified</returns>
        static public bool Execute(PO po, bool newRecord)
        {
            if (s_assignments == null)
                s_assignments = GetAll(po.GetCtx());
            bool modified = false;
            for (int i = 0; i < s_assignments.Length; i++)
            {
                MAssignSet set = s_assignments[i];
                if (!set.IsActive())
                    continue;
                //	Check IDs
                if (po.Get_Table_ID() == set.GetAD_Table_ID()
                    && (po.GetAD_Client_ID() == set.GetAD_Client_ID()
                        || set.GetAD_Client_ID() == 0))
                {
                    //	Check Timing
                    String rule = set.GetAutoAssignRule();
                    if (!newRecord && rule.Equals(AUTOASSIGNRULE_CreateOnly))
                        continue;
                    if (newRecord
                        && (rule.Equals(AUTOASSIGNRULE_UpdateOnly)
                            || rule.Equals(AUTOASSIGNRULE_UpdateIfNotProcessed)))
                        continue;
                    //	Eliminate Processed
                    if (rule.Equals(AUTOASSIGNRULE_CreateAndUpdateIfNotProcessed)
                        || rule.Equals(AUTOASSIGNRULE_UpdateIfNotProcessed))
                    {
                        int indexProcessed = po.Get_ColumnIndex("Processed");
                        if (indexProcessed != -1
                            && "Y".Equals(po.Get_Value(indexProcessed)))
                            continue;
                    }
                    //
                    if (set.ExecuteIt(po))
                        modified = true;
                }
            }
            return modified;
        }	//	execute


        	/**	Logger			*/
        //private static CLogger s_log = CLogger.getCLogger(MAssignSet.class);
        /**	Assignments		*/
        private static MAssignSet[] s_assignments = null;

        /**	The Target Lines				*/
        private MAssignTarget[] m_targets = null;


        public MAssignSet(Ctx ctx, int AD_AssignSet_ID, Trx trxName)
            : base(ctx, AD_AssignSet_ID, trxName)
        {
            
        }	//	MAssignSet

        public MAssignSet(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
            
        }	//	MAssignSet

        /// <summary>
        /// Get all Target Lines
        /// </summary>
        /// <param name="reload">reload data</param>
        /// <returns>array of lines</returns>
        public MAssignTarget[] GetTargets(bool reload)
        {
            if (m_targets != null && !reload)
                return m_targets;
            String sql = "SELECT * FROM AD_AssignTarget "
                + "WHERE AD_AssignSet_ID=@AD_AssignSet_ID ORDER BY SeqNo";
            List<MAssignTarget> list = new List<MAssignTarget>();
            
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@AD_AssignSet_ID", GetAD_AssignSet_ID());
                DataSet ds = DataBase.DB.ExecuteDataset(sql, param, Get_TrxName());
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MAssignTarget(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            //
            m_targets = new MAssignTarget[list.Count];
            m_targets = list.ToArray();
            return m_targets;
        }	//	getTargets

        /// <summary>
        /// Execute Auto Assignment
        /// </summary>
        /// <param name="po">PO to be modified</param>
        /// <returns>true if modified</returns>
        public bool ExecuteIt(PO po)
        {
            if (m_targets == null)
                m_targets = GetTargets(false);
            bool modified = false;
            for (int i = 0; i < m_targets.Length; i++)
            {
                MAssignTarget target = m_targets[i];
                if (!target.IsActive())
                    continue;
                //	Chck consistency
                MColumn tColumn = target.GetTargetColumn();
                if (tColumn.GetAD_Table_ID() != GetAD_Table_ID())
                    throw new Exception(ToString()
                        + ": AD_Table_ID inconsistent for " + target);
                //
                try
                {
                    modified = target.ExecuteIt(po);
                }
                catch (Exception e)
                {
                    log.Severe(e.ToString());
                    modified = false;
                }
            }
            return modified;
        }	//	execute

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("MAssignSet[")
                .Append(Get_ID()).Append("-").Append(GetName())
                .Append(",AD_Table_ID=").Append(GetAD_Table_ID())
                .Append("]");
            return sb.ToString();            
        }
    }
}
