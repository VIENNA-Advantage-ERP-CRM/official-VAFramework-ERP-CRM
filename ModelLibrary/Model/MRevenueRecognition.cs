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

        /// <summary>
        /// This function is used to create Recognition plan and run
        /// </summary>
        /// <param name="C_InvoiceLine_ID">invoice line</param>
        /// <param name="C_RevenueRecognition_ID">Revenue Recognition</param>
        /// <param name="Invoice">Invoice</param>
        /// <returns>true, when success</returns>
        public static bool CreateRevenueRecognitionPlan(int C_InvoiceLine_ID, int C_RevenueRecognition_ID, MInvoice Invoice)
        {
            try
            {
                MRevenueRecognitionRun revenueRecognitionRun = null;
                DateTime? RecognizationDate = null;
                int NoofMonths = 0;
                MRevenueRecognition revenueRecognition = new MRevenueRecognition(Invoice.GetCtx(), C_RevenueRecognition_ID, Invoice.Get_Trx());
                int defaultAccSchemaOrg_ID = GetDefaultActSchema(Invoice.GetCtx(), Invoice.GetAD_Client_ID(), Invoice.GetAD_Org_ID());


                MInvoiceLine invoiceLine = new MInvoiceLine(Invoice.GetCtx(), C_InvoiceLine_ID, Invoice.Get_Trx());
               // MInvoice invoice = new MInvoice(Invoice.GetCtx(), invoiceLine.GetC_Invoice_ID(), Invoice.Get_Trx());

               // string sql = "Select StartDate From C_InvoiceLine Where C_InvoiceLine_ID=" + invoiceLine.GetC_InvoiceLine_ID();
                RecognizationDate = Util.GetValueOfDateTime(invoiceLine.Get_Value("StartDate"));

                // precision to be handle based on std precision defined on acct schema
               string sql = "SELECT C.StdPrecision FROM C_AcctSchema a INNER JOIN C_Currency c ON c.C_Currency_ID= a.C_Currency_ID WHERE a.C_AcctSchema_ID=" + defaultAccSchemaOrg_ID;
                int stdPrecision = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));

                
                if (RecognizationDate.Value.Day != 1 && revenueRecognition.GetNoMonths()>0)
                {
                    //if startdate is in between day of month
                    NoofMonths = revenueRecognition.GetNoMonths() + 1;
                }
                else
                {
                    //if start date is the first day of the month
                    NoofMonths = revenueRecognition.GetNoMonths();
                }
                MRevenueRecognitionPlan revenueRecognitionPlan = new MRevenueRecognitionPlan(Invoice.GetCtx(), 0, Invoice.Get_Trx());
                revenueRecognitionPlan.SetRecognitionPlan(invoiceLine, Invoice, C_RevenueRecognition_ID);
                revenueRecognitionPlan.SetC_AcctSchema_ID(defaultAccSchemaOrg_ID);
                revenueRecognitionPlan.SetRecognizedAmt(0);
                if (!revenueRecognitionPlan.Save())
                {
                    ValueNamePair pp = VLogger.RetrieveError();
                    string error = pp != null ? pp.GetValue() : "";
                    if (pp != null && string.IsNullOrEmpty(error))
                    {
                        error = pp.GetName();
                    }
                    if (!string.IsNullOrEmpty(error))
                    {
                        _log.Log(Level.SEVERE, error);
                        return false;
                    }
                }
                else
                {
                    if (!revenueRecognition.IsTimeBased())
                    {

                    }
                    else
                    {
                        if (revenueRecognition.GetRecognitionFrequency().Equals("M"))
                        {
                          
                            decimal totaldays = Util.GetValueOfDecimal((RecognizationDate.Value.AddMonths(revenueRecognition.GetNoMonths()) - RecognizationDate.Value.Date).TotalDays);
                          
                            decimal perdayAmt = Math.Round(revenueRecognitionPlan.GetTotalAmt() /(totaldays>0?totaldays:1), 12);
                            decimal recognizedAmt = 0;
                            DateTime? lastdate = null;
                            int days = 0;
                            for (int i = 0; i < NoofMonths; i++)
                            {
                                if (i == 0)
                                {
                                    if (RecognizationDate.Value.Month == 12)
                                    {
                                        //last date of the month 
                                        lastdate = new DateTime(RecognizationDate.Value.Year, RecognizationDate.Value.Month, 1).AddMonths(1).AddDays(-1);
                                    }
                                    else
                                    {
                                        //last date of the month
                                        lastdate = new DateTime(RecognizationDate.Value.Year, RecognizationDate.Value.Month + 1, 1).AddDays(-1);
                                    }
                                    days = Util.GetValueOfInt((lastdate.Value.Date - RecognizationDate.Value.Date).TotalDays);
                                    days += 1;
                                }
                                else if (i == (revenueRecognition.GetNoMonths()))
                                {
                                    //last date of the month would the day before  the recoganizationdate
                                    lastdate = RecognizationDate.Value.AddMonths(i).AddDays(-1);
                                    DateTime startDate = new DateTime(lastdate.Value.Year, lastdate.Value.Month, 1);
                                    days = Util.GetValueOfInt((lastdate.Value.Date - startDate.Date).TotalDays);
                                    days += 1;
                                }
                                else
                                {
                                    DateTime startDate = lastdate.Value.AddDays(1);                                                                   
                                    days = DateTime.DaysInMonth(startDate.Year, startDate.Month);
                                    lastdate = startDate.AddDays(days-1);
                                }
                                recognizedAmt = Math.Round(Convert.ToDecimal(days) * perdayAmt, stdPrecision);
                                revenueRecognitionRun = new MRevenueRecognitionRun(Invoice.GetCtx(), 0, Invoice.Get_Trx());
                                revenueRecognitionRun.SetRecognitionRun(revenueRecognitionPlan);
                                revenueRecognitionRun.SetRecognizedAmt(recognizedAmt);
                                revenueRecognitionRun.SetRecognitionDate(lastdate);
                                if (!revenueRecognitionRun.Save())
                                {
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    string error = pp != null ? pp.GetValue() : "";
                                    if (pp != null && string.IsNullOrEmpty(error))
                                    {
                                        error = pp.GetName();
                                    }
                                    if (!string.IsNullOrEmpty(error))
                                    {
                                        _log.Log(Level.SEVERE, error);
                                        return false;
                                    }
                                }
                                recognizedAmt = 0;
                            }
                        }
                        else if (revenueRecognition.GetRecognitionFrequency().Equals("D"))
                        {
                            Decimal recognizedAmt = Math.Round(revenueRecognitionPlan.GetTotalAmt() / revenueRecognition.GetNoMonths(), stdPrecision);
                            int days = 0;
                            for (int i = 0; i < revenueRecognition.GetNoMonths(); i++)
                            {
                                revenueRecognitionRun = new MRevenueRecognitionRun(Invoice.GetCtx(), 0, Invoice.Get_Trx());
                                revenueRecognitionRun.SetRecognitionRun(revenueRecognitionPlan);
                                revenueRecognitionRun.SetRecognizedAmt(recognizedAmt);
                                revenueRecognitionRun.SetRecognitionDate(RecognizationDate.Value.AddDays(days));
                                days += 1;
                                if (!revenueRecognitionRun.Save())
                                {
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    string error = pp != null ? pp.GetValue() : "";
                                    if (pp != null && string.IsNullOrEmpty(error))
                                    {
                                        error = pp.GetName();
                                    }
                                    if (!string.IsNullOrEmpty(error))
                                    {
                                        _log.Log(Level.SEVERE, error);
                                        return false;
                                    }
                                }
                            }
                        }
                        else if (revenueRecognition.GetRecognitionFrequency().Equals("Y"))
                        {
                            DateTime? fstartDate = null;
                            DateTime? fendDate = null;                           
                            int calendar_ID = 0;
                            DataSet ds = new DataSet();

                            calendar_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_Calendar_ID FROM AD_OrgInfo WHERE ad_org_id = " + Invoice.GetAD_Org_ID()));
                            if (calendar_ID == 0)
                            {
                                calendar_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_Calendar_ID FROM AD_ClientInfo WHERE ad_client_id = " + Invoice.GetAD_Client_ID()));
                            }
                            sql = "SELECT startdate , enddate FROM c_period WHERE " +
                                "c_year_id = (SELECT c_year.c_year_id FROM c_year INNER JOIN C_period ON c_year.c_year_id = C_period.c_year_id " +
                                "WHERE  c_year.c_calendar_id ="+ calendar_ID + " AND "+GlobalVariable.TO_DATE(RecognizationDate,false)+" BETWEEN C_period.startdate AND C_period.enddate) AND periodno IN (1, 12)";
                            ds = DB.ExecuteDataset(sql);
                            if (ds != null && ds.Tables[0].Rows.Count > 0)
                            {
                                fstartDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["startdate"]);
                                fendDate = Convert.ToDateTime(ds.Tables[0].Rows[1]["enddate"]);
                            }
                            decimal totaldays = Util.GetValueOfDecimal((RecognizationDate.Value.AddYears(revenueRecognition.GetNoMonths()) - RecognizationDate.Value.Date).TotalDays);

                            decimal perdayAmt = Math.Round(revenueRecognitionPlan.GetTotalAmt() / (totaldays>0?totaldays:1), 12);
                            decimal recognizedAmt = 0;
                            DateTime? lastdate = null;
                            int days = 0;
                            for (int i = 0; i < NoofMonths; i++)
                            {
                                if (i == 0)
                                {   
                                    //last date will always be financial year's end date 
                                    lastdate = fendDate;
                                    days = Util.GetValueOfInt((lastdate.Value.Date - RecognizationDate.Value.Date).TotalDays);
                                    days += 1;
                                }
                                else if (i == (revenueRecognition.GetNoMonths()))
                                {
                                    //last date of the year would the day before the recoganizationdate
                                    lastdate = RecognizationDate.Value.AddYears(i).AddDays(-1);
                                    DateTime startDate = fstartDate.Value.AddYears(i);
                                    days = Util.GetValueOfInt((lastdate.Value.Date - startDate.Date).TotalDays);
                                    days += 1;
                                }
                                else
                                {
                                    lastdate = fendDate.Value.AddYears(i);
                                    DateTime _startDate = fstartDate.Value.AddYears(i);
                                    days = Util.GetValueOfInt((lastdate.Value.Date - _startDate.Date).TotalDays);
                                    days += 1;
                                }
                                recognizedAmt = Math.Round(Convert.ToDecimal(days) * perdayAmt, stdPrecision);
                                revenueRecognitionRun = new MRevenueRecognitionRun(Invoice.GetCtx(), 0, Invoice.Get_Trx());
                                revenueRecognitionRun.SetRecognitionRun(revenueRecognitionPlan);
                                revenueRecognitionRun.SetRecognizedAmt(recognizedAmt);
                                revenueRecognitionRun.SetRecognitionDate(lastdate);
                                if (!revenueRecognitionRun.Save())
                                {
                                    ValueNamePair pp = VLogger.RetrieveError();
                                    string error = pp != null ? pp.GetValue() : "";
                                    if (pp != null && string.IsNullOrEmpty(error))
                                    {
                                        error = pp.GetName();
                                    }
                                    if (!string.IsNullOrEmpty(error))
                                    {
                                        _log.Log(Level.SEVERE, error);
                                        return false;
                                    }
                                }
                                recognizedAmt = 0;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Severe("Exception during creation of Recognition Plan and Run. " + ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// This function is used to get Accounting Schema either binded on Organization or Primary Accounting SChema
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="Ad_Client_ID">AD_Client_ID</param>
        /// <param name="AD_Org_ID">Org ID</param>
        /// <returns>C_AcctSchema ID</returns>
        public static int GetDefaultActSchema(Ctx ctx, int Ad_Client_ID, int AD_Org_ID)
        {
            MAcctSchema acctSchema = null;
            if (AD_Org_ID > 0)
            {
                acctSchema = MOrg.Get(ctx, AD_Org_ID).GetAcctSchema();
            }
            if (acctSchema == null)
            {
                acctSchema = MClient.Get(ctx, Ad_Client_ID).GetAcctSchema();
            }
            return acctSchema.GetC_AcctSchema_ID();
        }

    }
}
