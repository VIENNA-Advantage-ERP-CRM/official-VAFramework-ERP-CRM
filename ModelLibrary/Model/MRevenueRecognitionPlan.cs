/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRevenueRecognitionPlan
 * Purpose        : Revenue Recognition Plan.
 * Class Used     : X_C_RevenueRecognition_Plan
 * Chronological    Development
 * Raghunandan      19-Jan-2010
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
using System.Data.SqlClient;
using VAdvantage.Acct;

namespace VAdvantage.Model
{
    /// <summary>
    /// Revenue Recognition Plan
    /// </summary>
    public class MRevenueRecognitionPlan : X_C_RevenueRecognition_Plan
    {
        /// <summary>
        /// 	Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_RevenueRecognition_Plan_ID"></param>
        /// <param name="trxName"></param>
        public MRevenueRecognitionPlan(Ctx ctx, int C_RevenueRecognition_Plan_ID, Trx trxName)
            : base(ctx, C_RevenueRecognition_Plan_ID, trxName)
        {
            if (C_RevenueRecognition_Plan_ID == 0)
            {
                //	setC_AcctSchema_ID (0);
                //	setC_Currency_ID (0);
                //	setC_InvoiceLine_ID (0);
                //	setC_RevenueRecognition_ID (0);
                //	setC_RevenueRecognition_Plan_ID (0);
                //	setP_Revenue_Acct (0);
                //	setUnEarnedRevenue_Acct (0);
                SetTotalAmt(Env.ZERO);
                SetRecognizedAmt(Env.ZERO);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public MRevenueRecognitionPlan(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <param name="success"></param>
        /// <returns>Success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (newRecord)
            {
                MRevenueRecognition rr = new MRevenueRecognition(GetCtx(), GetC_RevenueRecognition_ID(), Get_TrxName());
                if (rr.IsTimeBased())
                {
                    /**	Get InvoiveQty
                    SELECT	QtyInvoiced, M_Product_ID 
                      INTO	v_Qty, v_M_Product_ID
                    FROM	C_InvoiceLine 
                    WHERE 	C_InvoiceLine_ID=:new.C_InvoiceLine_ID;
                    --	Insert
                    AD_Sequence_Next ('C_ServiceLevel', :new.AD_Client_ID, v_NextNo);
                    INSERT INTO C_ServiceLevel
                        (C_ServiceLevel_ID, C_RevenueRecognition_Plan_ID,
                        AD_Client_ID,AD_Org_ID,IsActive,Created,CreatedBy,Updated,UpdatedBy,
                        M_Product_ID, Description, ServiceLevelInvoiced, ServiceLevelProvided,
                        Processing,Processed)
                    VALUES
                        (v_NextNo, :new.C_RevenueRecognition_Plan_ID,
                        :new.AD_Client_ID,:new.AD_Org_ID,'Y',SysDate,:new.CreatedBy,SysDate,:new.UpdatedBy,
                        v_M_Product_ID, NULL, v_Qty, 0,
                        'N', 'N');
                    **/
                }
            }
            return success;
        }	//	afterSave
    }
}
