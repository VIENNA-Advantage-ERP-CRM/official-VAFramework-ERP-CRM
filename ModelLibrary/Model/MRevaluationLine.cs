/********************************************************
 * Project Name   : VAdvantage
 * Module Name    : ModelLibrary
 * Class Name     : MRevaluationLine
 * Purpose        : Revaluate Line
 * Class Used     : none
 * Chronological  : Development
 * VIS_0045       : 10-March-2023
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MRevaluationLine : X_M_RevaluationLine
    {
        /** Is Update Header Difference */
        public bool isUpdateHeader = true;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Payment_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MRevaluationLine(Ctx ctx, int M_RevaluationLine_ID, Trx trxName)
           : base(ctx, M_RevaluationLine_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MRevaluationLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Implement After Save Logic
        /// </summary>
        /// <param name="newRecord">Is New Record</param>
        /// <param name="success">Is Record saved</param>
        /// <returns>True, when success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return success;

            // Update difference on Inventory revaluation Header 
            if (isUpdateHeader)
            {
                UpdateHeader();
            }
            return true;
        }

        /// <summary>
        /// Update difference on Inventory revaluation Header 
        /// </summary>
        /// <returns>true</returns>
        public bool UpdateHeader()
        {
            int no = DB.ExecuteQuery($@"UPDATE M_InventoryRevaluation ir SET TotalDifference  = 
                                (SELECT SUM(CASE WHEN ir.RevaluationType = 'A' THEN TotalDifference ELSE DifferenceValue END) 
                                FROM M_RevaluationLine irl WHERE irl.M_InventoryRevaluation_ID = ir.M_InventoryRevaluation_ID)
                                WHERE ir.M_InventoryRevaluation_ID = {GetM_InventoryRevaluation_ID()}", null, Get_Trx());
            if (no < 0)
            {
                log.Log(Level.WARNING, $"Revaluation Difference not updated, Inventory Revaluation ID = {GetM_InventoryRevaluation_ID()}");
            }
            return true;
        }
    }
}
