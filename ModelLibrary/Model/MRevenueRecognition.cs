/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRevenueRecognition
 * Purpose        : Revenue Recognition Model
 * Class Used     : X_C_RevenueRecognition
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
    /// Revenue Recognition Model
    /// </summary>
    public class MRevenueRecognition : X_C_RevenueRecognition
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(MRevenueRecognition).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_RevenueRecognition_ID"></param>
        /// <param name="trxName"></param>
        public MRevenueRecognition(Ctx ctx, int C_RevenueRecognition_ID, Trx trxName)
            : base(ctx, C_RevenueRecognition_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName"></param>
        public MRevenueRecognition(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MRevenueRecognition(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// This Function is used to get RevenueRecognition Records
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="trx">trx</param>
        /// <returns>Array of MRevenueRecognition</returns>
        public static MRevenueRecognition[] GetRecognitions(Ctx ctx, Trx trx)
        {
            List<MRevenueRecognition> list = new List<MRevenueRecognition>();
            string sql = "Select * From C_RevenueRecognition Where AD_Client_ID=" + ctx.GetAD_Client_ID();

            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MRevenueRecognition(ctx, dr, trx));
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

            MRevenueRecognition[] retValue = new MRevenueRecognition[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

		public static bool CreateRevenueRecognitionPlan(int C_InvoiceLine_ID, int C_RevenueRecognition_ID, MInvoice Invoice)
		{
			try
			{
				MRevenueRecognitionRun revenueRecognitionRun = null;
				DateTime? RecognizationDate = null;
				MRevenueRecognition revenueRecognition = new MRevenueRecognition(Invoice.GetCtx(), C_RevenueRecognition_ID, Invoice.Get_Trx());
				int defaultAccSchemaOrg_ID = GetDefaultActSchema(Invoice.GetAD_Org_ID());
				if (defaultAccSchemaOrg_ID <= 0)
				{
					//_log.Log(Level.SEVERE, "Default Schema not found for the oraganization");
					_log.SaveError("Error", "Default Schema not found for the oraganization", false);
					return false;
				}
				MINT15AccountingSchemaOrg accountingSchemaOrg = new MINT15AccountingSchemaOrg(Invoice.GetCtx(), defaultAccSchemaOrg_ID, Invoice.Get_Trx());

				MRevenueRecognitionPlan revenueRecognitionPlan = new MRevenueRecognitionPlan(Invoice.GetCtx(), 0, Invoice.Get_Trx());
				MInvoiceLine invoiceLine = new MInvoiceLine(Invoice.GetCtx(), C_InvoiceLine_ID, Invoice.Get_Trx());
				MInvoice invoice = new MInvoice(Invoice.GetCtx(), invoiceLine.GetC_Invoice_ID(), Invoice.Get_Trx());

				string sql = "Select INT15_StartDate From C_InvoiceLine Where C_InvoiceLine_ID=" + invoiceLine.GetC_InvoiceLine_ID();
				RecognizationDate = Util.GetValueOfDateTime(DB.ExecuteScalar(sql));
				if (RecognizationDate == null)
				{
					RecognizationDate = invoice.GetDateInvoiced();
				}

				revenueRecognitionPlan.SetRecognitionPlan(invoiceLine, invoice, C_RevenueRecognition_ID);
				revenueRecognitionPlan.SetC_AcctSchema_ID(accountingSchemaOrg.GetC_AcctSchema_ID());
				revenueRecognitionPlan.SetRecognizedAmt(0);
				if (revenueRecognition.GetINT15_RecognizeType() == "R")
				{
					int UnearnedRevenueAcct_ID = RecognitionCombination(accountingSchemaOrg, "UR", invoiceLine);
					int RevenueAcct_ID = RecognitionCombination(accountingSchemaOrg, "TR", invoiceLine);
					if (UnearnedRevenueAcct_ID == 0 || RevenueAcct_ID == 0)
					{
						_log.SaveError("Error", "Accounts not defined for the product or charge", false);
						return false;
					}
					revenueRecognitionPlan.SetUnEarnedRevenue_Acct(UnearnedRevenueAcct_ID);
					revenueRecognitionPlan.SetP_Revenue_Acct(RevenueAcct_ID);
				}
				else
				{
					int PrepaidExpense_ID = RecognitionCombination(accountingSchemaOrg, "DE", invoiceLine);
					int ProductExpense_ID = RecognitionCombination(accountingSchemaOrg, "PE", invoiceLine);
					if (PrepaidExpense_ID == 0 || ProductExpense_ID == 0)
					{
						_log.SaveError("Error", "Accounts not defined for the product or charge", false);
						return false;
					}
					revenueRecognitionPlan.SetINT15_PrepaidExpense(PrepaidExpense_ID);
					revenueRecognitionPlan.SetINT15_ProductExpense(ProductExpense_ID);
				}
				if (!revenueRecognitionPlan.Save())
				{
					ValueNamePair pp = VLogger.RetrieveError();
					if (pp != null && !string.IsNullOrEmpty(pp.GetName()))
					{
						_log.Log(Level.SEVERE, pp.GetName());
						return false;
					}
				}
				else
				{
					if (!revenueRecognition.IsTimeBased())
					{
						MINT15RevenueService[] revenueService = GetServices(revenueRecognition);
						for (int i = 0; i < revenueService.Length; i++)
						{
							MINT15RevenueService revenueserviceline = revenueService[i];
							revenueRecognitionRun = new MRevenueRecognitionRun(Invoice.GetCtx(), 0, Invoice.Get_Trx());
							revenueRecognitionRun.SetRecognitionRun(revenueRecognition, revenueserviceline, revenueRecognitionPlan);
							Decimal recognizedAmt = Math.Round((revenueRecognitionPlan.GetTotalAmt() * revenueserviceline.GetINT15_Percentage()) / 100, 5);
							revenueRecognitionRun.SetRecognizedAmt(recognizedAmt);
							revenueRecognitionRun.SetINT15_RevenueService_ID(revenueserviceline.GetINT15_RevenueService_ID());

							if (!revenueRecognitionRun.Save())
							{
								ValueNamePair pp = VLogger.RetrieveError();
								if (pp != null && !string.IsNullOrEmpty(pp.GetName()))
								{
									_log.Log(Level.SEVERE, pp.GetName());
									return false;
								}
							}
						}
					}
					else
					{
						if (revenueRecognition.GetRecognitionFrequency() == "M")
						{
							//Decimal recognizedAmt = revenueRecognitionPlan.GetTotalAmt() / revenueRecognition.GetNoMonths();
							double totaldays = (RecognizationDate.Value.AddMonths(revenueRecognition.GetNoMonths()) - RecognizationDate.Value.Date).TotalDays;
							decimal perdayAmt = Math.Round(revenueRecognitionPlan.GetTotalAmt() / Convert.ToDecimal(totaldays), 5);
							decimal recognizedAmt = 0;
							DateTime? lastdate = null;
							int days = 0;
							for (int i = 0; i < revenueRecognition.GetNoMonths() + 1; i++)
							{
								if (i == 0)
								{
									if (RecognizationDate.Value.Month == 12)
									{
										lastdate = new DateTime(RecognizationDate.Value.Year, RecognizationDate.Value.Month, 1).AddMonths(1).AddDays(-1);
									}
									else
									{
										lastdate = new DateTime(RecognizationDate.Value.Year, RecognizationDate.Value.Month + 1, 1).AddDays(-1);
									}
									days = Util.GetValueOfInt((lastdate.Value.Date - RecognizationDate.Value.Date).TotalDays);
									days += 1;
								}
								else if (i == (revenueRecognition.GetNoMonths()))
								{
									DateTime EndDate = RecognizationDate.Value.AddMonths(i);
									var startDate = new DateTime(EndDate.Year, EndDate.Month, 1);
									days = Util.GetValueOfInt((EndDate.Date - startDate.Date).TotalDays);
								}
								else
								{
									DateTime startdate = RecognizationDate.Value.AddMonths(i);
									days = DateTime.DaysInMonth(startdate.Year, startdate.Month);
								}
								recognizedAmt = Convert.ToDecimal(days) * perdayAmt;
								revenueRecognitionRun = new MRevenueRecognitionRun(Invoice.GetCtx(), 0, Invoice.Get_Trx());
								revenueRecognitionRun.SetRecognitionRun(revenueRecognition, null, revenueRecognitionPlan);
								revenueRecognitionRun.SetRecognizedAmt(recognizedAmt);
								revenueRecognitionRun.SetINT15_RecognitionDate(RecognizationDate.Value.AddMonths(i));
								if (!revenueRecognitionRun.Save())
								{
									ValueNamePair pp = VLogger.RetrieveError();
									if (pp != null && !string.IsNullOrEmpty(pp.GetName()))
									{
										_log.Log(Level.SEVERE, pp.GetName());
										return false;
									}
								}
								recognizedAmt = 0;
							}
						}
						else if (revenueRecognition.GetRecognitionFrequency() == "D")
						{
							Decimal recognizedAmt = Math.Round(revenueRecognitionPlan.GetTotalAmt() / revenueRecognition.GetNoMonths(), 5);
							int days = 0;
							for (int i = 0; i < revenueRecognition.GetNoMonths(); i++)
							{
								revenueRecognitionRun = new MRevenueRecognitionRun(Invoice.GetCtx(), 0, Invoice.Get_Trx());
								revenueRecognitionRun.SetRecognitionRun(revenueRecognition, null, revenueRecognitionPlan);
								revenueRecognitionRun.SetRecognizedAmt(recognizedAmt);
								revenueRecognitionRun.SetINT15_RecognitionDate(RecognizationDate.Value.AddDays(days));
								days += 1;
								if (!revenueRecognitionRun.Save())
								{
									ValueNamePair pp = VLogger.RetrieveError();
									if (pp != null && !string.IsNullOrEmpty(pp.GetName()))
									{
										_log.Log(Level.SEVERE, pp.GetName());
										return false;
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				return false;
			}
			//return "Revenue Recognition Created";
			return true;
		}

		public static int GetDefaultActSchema(int AD_Org_ID)
		{
			string sql = "Select INT15_AccountingSchemaOrg_ID From INT15_AccountingSchemaOrg Where AD_Org_ID=" + AD_Org_ID;
			int C_AcctschemaOrg_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql));
			return C_AcctschemaOrg_ID;
		}

		public static int RecognitionCombination(MINT15AccountingSchemaOrg accountingSchemaOrg, string recognitionType, MInvoiceLine InvoiceLine)
		{
			string sql = "";
			if (InvoiceLine.GetM_Product_ID() > 0)
			{
				sql = "Select pacct.c_validcombination_id From FRPT_Product_Acct pacct INNER JOIN FRPT_AcctDefault fad on fad.FRPT_AcctDefault_ID = pacct.FRPT_AcctDefault_ID " +
						   "  where fad.INT15_RecognizeType = '" + recognitionType + "' and pacct.c_acctschema_id = " + accountingSchemaOrg.GetC_AcctSchema_ID() + " And pacct.M_PRoduct_ID=" + InvoiceLine.GetM_Product_ID();
			}
			else if (InvoiceLine.GetC_Charge_ID() > 0)
			{
				sql = "Select pacct.c_validcombination_id From FRPT_Charge_Acct pacct INNER JOIN FRPT_AcctDefault fad on fad.FRPT_AcctDefault_ID = pacct.FRPT_AcctDefault_ID " +
						   "  where fad.INT15_RecognizeType = '" + recognitionType + "' and pacct.c_acctschema_id = " + accountingSchemaOrg.GetC_AcctSchema_ID() + " And pacct.C_Charge_ID=" + InvoiceLine.GetC_Charge_ID();
			}
			else
			{
				sql = "select ad.c_validcombination_id from INT15_AcctSchema_Default ad inner join FRPT_AcctDefault fad on fad.FRPT_AcctDefault_ID = ad.FRPT_AcctDefault_ID" +
					" where fad.INT15_RecognizeType = '" + recognitionType + "' and ad.c_acctschema_id = " + accountingSchemaOrg.GetC_AcctSchema_ID();
			}
			int ValidCombination_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql));
			return ValidCombination_ID;
		}


	}
}
