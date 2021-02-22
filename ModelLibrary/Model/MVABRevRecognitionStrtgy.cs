/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRevenueRecognitionPlan
 * Purpose        : Revenue Recognition Plan.
 * Class Used     : X_VAB_Rev_RecognitionStrtgy
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
    public class MVABRevRecognitionStrtgy : X_VAB_Rev_RecognitionStrtgy
    {

        private static VLogger _log = VLogger.GetVLogger(typeof(MVABRevRecognitionStrtgy).FullName);

        /// <summary>
        /// 	Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAB_Rev_RecognitionStrtgy_ID"></param>
        /// <param name="trxName"></param>
        public MVABRevRecognitionStrtgy(Ctx ctx, int VAB_Rev_RecognitionStrtgy_ID, Trx trxName)
            : base(ctx, VAB_Rev_RecognitionStrtgy_ID, trxName)
        {
            if (VAB_Rev_RecognitionStrtgy_ID == 0)
            {
                //	setVAB_AccountBook_ID (0);
                //	setVAB_Currency_ID (0);
                //	setVAB_InvoiceLine_ID (0);
                //	setVAB_Rev_Recognition_ID (0);
                //	setVAB_Rev_RecognitionStrtgy_ID (0);
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
        public MVABRevRecognitionStrtgy(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MVABRevRecognitionStrtgy(Ctx ctx, DataRow rs, Trx trxName)
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
                MVABRevRecognition rr = new MVABRevRecognition(GetCtx(), GetVAB_Rev_Recognition_ID(), Get_TrxName());
                if (rr.IsTimeBased())
                {
                    /**	Get InvoiveQty
                    SELECT	QtyInvoiced, VAM_Product_ID 
                      INTO	v_Qty, v_VAM_Product_ID
                    FROM	VAB_InvoiceLine 
                    WHERE 	VAB_InvoiceLine_ID=:new.VAB_InvoiceLine_ID;
                    --	Insert
                    VAF_Record_Seq_Next ('VAB_SLevelCriteria', :new.VAF_Client_ID, v_NextNo);
                    INSERT INTO VAB_SLevelCriteria
                        (VAB_SLevelCriteria_ID, VAB_Rev_RecognitionStrtgy_ID,
                        VAF_Client_ID,VAF_Org_ID,IsActive,Created,CreatedBy,Updated,UpdatedBy,
                        VAM_Product_ID, Description, ServiceLevelInvoiced, ServiceLevelProvided,
                        Processing,Processed)
                    VALUES
                        (v_NextNo, :new.VAB_Rev_RecognitionStrtgy_ID,
                        :new.VAF_Client_ID,:new.VAF_Org_ID,'Y',SysDate,:new.CreatedBy,SysDate,:new.UpdatedBy,
                        v_VAM_Product_ID, NULL, v_Qty, 0,
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
        /// <param name="VAB_Rev_Recognition_ID">Recognition ID</param>
        /// <param name="ToCurrency">Currency</param>
        public void SetRecognitionPlan(MVABInvoiceLine invoiceLine, MVABInvoice invoice, int VAB_Rev_Recognition_ID, int ToCurrency)
        {

            SetVAF_Client_ID(invoice.GetVAF_Client_ID());
            SetVAF_Org_ID(invoice.GetVAF_Org_ID());
            SetVAB_Currency_ID(ToCurrency);
            SetVAB_InvoiceLine_ID(invoiceLine.GetVAB_InvoiceLine_ID());
            SetVAB_Rev_Recognition_ID(VAB_Rev_Recognition_ID);
            // when tax include into price list, then reduce tax from Line Net Amount
            bool isTaxIncide = (new MVAMPriceList(invoice.GetCtx(), invoice.GetVAM_PriceList_ID(), invoice.Get_Trx())).IsTaxIncluded();
            Decimal Amount = invoiceLine.GetLineNetAmt() - (isTaxIncide ? (invoiceLine.GetTaxAmt() + invoiceLine.GetSurchargeAmt()) : 0);
            if (invoice.GetVAB_Currency_ID() != ToCurrency)
            {
                Amount = MVABExchangeRate.Convert(GetCtx(), Amount, invoice.GetVAB_Currency_ID(), ToCurrency, invoice.GetDateInvoiced(), invoice.GetVAB_CurrencyType_ID(), invoice.GetVAF_Client_ID(), invoice.GetVAF_Org_ID());
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
        public static MVABRevRecognitionStrtgy[] GetRecognitionPlans(MVABRevRecognition RevenueRecognition, int InvoiceLine_ID, int OrgId)
        {
            List<MVABRevRecognitionStrtgy> list = new List<MVABRevRecognitionStrtgy>();
            string sql = "SELECT * FROM VAB_Rev_RecognitionStrtgy pl";
            if (InvoiceLine_ID > 0)
            {
                sql += @" INNER  JOIN VAB_InvoiceLine invl ON invl.VAB_InvoiceLine_id = pl.VAB_InvoiceLine_id 
                            WHERE pl.VAB_Rev_Recognition_ID=" + RevenueRecognition.GetVAB_Rev_Recognition_ID() +
                        " AND invl.VAB_InvoiceLine_id=" + InvoiceLine_ID + " AND invl.vaf_org_id=" + OrgId;
            }
            else
            {
                sql += " WHERE pl.VAB_Rev_Recognition_ID=" + RevenueRecognition.GetVAB_Rev_Recognition_ID() + " AND pl.vaf_org_id=" + OrgId;
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
                    list.Add(new MVABRevRecognitionStrtgy(RevenueRecognition.GetCtx(), dr, RevenueRecognition.Get_Trx()));
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

            MVABRevRecognitionStrtgy[] retValue = new MVABRevRecognitionStrtgy[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

    }
}
