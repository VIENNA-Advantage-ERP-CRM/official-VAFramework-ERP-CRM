
/********************************************************
 * Module Name    : Scheduler
 * Purpose        : Schedule the Events
 * Author         : Jagmohan Bhatt
 * Date           : 04-Nov-2009
 ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.Process;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Classes;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    /// <summary>
    /// Scheduler Parameter Model
    /// </summary>
    public class MSchedulerPara : X_AD_Scheduler_Para
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Scheduler_Para_ID">scheduler para id</param>
        /// <param name="trxName">optinal trans name</param>
        public MSchedulerPara(Ctx ctx, int AD_Scheduler_Para_ID, Trx trxName)
            : base(ctx, AD_Scheduler_Para_ID, trxName)
        {
            
        }	//	MSchedulerPara

        /// <summary>
        /// Load constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">optinal trans name</param>
        public MSchedulerPara(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
            
        }	//	MSchedulerPara


        /** Parameter Column Name		*/
        private MProcessPara m_parameter = null;

        /// <summary>
        /// Get Parameter Column Name 
        /// </summary>
        /// <returns>column name</returns>
        public String GetColumnName()
        {
            if (m_parameter == null)
                m_parameter = MProcessPara.Get(GetCtx(), GetAD_Process_Para_ID());
            return m_parameter.GetColumnName();
        }	//	getColumnName

        /// <summary>
        /// Get Display Type
        /// </summary>
        /// <returns>display type</returns>
        public int GetDisplayType()
        {
            if (m_parameter == null)
                m_parameter = MProcessPara.Get(GetCtx(), GetAD_Process_Para_ID());
            return m_parameter.GetAD_Reference_ID();
        }	//	getDisplayType

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>info</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("MSchedulerPara[");
            sb.Append(Get_ID()).Append("-")
                .Append(GetColumnName()).Append("=").Append(GetParameterDefault())
                .Append("]");
            return sb.ToString();
        }
    }
}
