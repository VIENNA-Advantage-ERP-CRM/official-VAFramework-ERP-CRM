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

        private static VLogger _log = VLogger.GetVLogger(typeof(MRevenueRecognitionPlan).FullName);

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
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MRevenueRecognitionPlan(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
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

        /// <summary>
        /// This function is used to set Values on Recognition Plan
        /// </summary>
        /// <param name="invoiceLine">invoice line object </param>
        /// <param name="invoice">invoice object</param>
        /// <param name="C_RevenueRecognition_ID">Recognition ID</param>
        /// <param name="ToCurrency">Currency</param>
        public void SetRecognitionPlan(MInvoiceLine invoiceLine, MInvoice invoice, int C_RevenueRecognition_ID,int ToCurrency)
        {
           
            SetAD_Client_ID(invoice.GetAD_Client_ID());
            SetAD_Org_ID(invoice.GetAD_Org_ID());
            SetC_Currency_ID(ToCurrency);
            SetC_InvoiceLine_ID(invoiceLine.GetC_InvoiceLine_ID());
            SetC_RevenueRecognition_ID(C_RevenueRecognition_ID);
            // when tax include into price list, then reduce tax from Line Net Amount
            bool isTaxIncide = (new MPriceList(invoice.GetCtx(), invoice.GetM_PriceList_ID(), invoice.Get_Trx())).IsTaxIncluded();
            Decimal Amount = invoiceLine.GetLineNetAmt() - (isTaxIncide ? (invoiceLine.GetTaxAmt() + invoiceLine.GetSurchargeAmt()) : 0);
            if (invoice.GetC_Currency_ID() != ToCurrency)
            {
                Amount= MConversionRate.Convert(GetCtx(), Amount, invoice.GetC_Currency_ID(), ToCurrency, invoice.GetDateInvoiced(), invoice.GetC_ConversionType_ID(), invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID());
            }
            SetTotalAmt(Amount);
            SetRecognizedAmt(Env.ZERO);
        }

        /// <summary>
        /// This Function is used to gete Plan data record based on respective parameters
        /// </summary>
        /// <param name="RevenueRecognition">Revenue Recognition Reference</param>
        /// <param name="InvoiceLine_ID">Invoice Line Reference</param>
        /// <param name="OrgId">Org ID</param>
        /// <returns>Array of MRevenueRecognitionPlan</returns>
        public static MRevenueRecognitionPlan[] GetRecognitionPlans(MRevenueRecognition RevenueRecognition, int InvoiceLine_ID, int OrgId)
        {
            List<MRevenueRecognitionPlan> list = new List<MRevenueRecognitionPlan>();
            string sql = "SELECT * FROM C_RevenueRecognition_Plan pl";
            if (InvoiceLine_ID > 0)
            {
                sql += @" INNER  JOIN c_invoiceline invl ON invl.c_invoiceline_id = pl.c_invoiceline_id 
                            WHERE pl.C_RevenueRecognition_ID=" + RevenueRecognition.GetC_RevenueRecognition_ID() +
                        " AND invl.c_invoiceLine_id=" + InvoiceLine_ID + " AND invl.ad_org_id=" + OrgId;
            }
            else
            {
                sql += " WHERE pl.C_RevenueRecognition_ID=" + RevenueRecognition.GetC_RevenueRecognition_ID() + " AND pl.ad_org_id=" + OrgId;
            }
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, RevenueRecognition.Get_Trx());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MRevenueRecognitionPlan(RevenueRecognition.GetCtx(), dr, RevenueRecognition.Get_Trx()));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }

            MRevenueRecognitionPlan[] retValue = new MRevenueRecognitionPlan[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

    }
}
