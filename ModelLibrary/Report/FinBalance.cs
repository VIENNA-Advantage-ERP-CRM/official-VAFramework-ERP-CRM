using System;
using System.Net;
using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Documents;
//using System.Windows.Ink;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Animation;
//using System.Windows.Shapes;
using VAdvantage.Process;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Model;
using System.Collections.Generic;

using System.Data;
using VAdvantage.ProcessEngine;


namespace VAdvantage.Report
{

    /// <summary>
    /// Financial Balance Maintenance Engine
    /// </summary>
    public class FinBalance : SvrProcess
    {
        #region Private Variables
        //Logger						
        protected static VLogger _log = VLogger.GetVLogger(typeof(FinBalance).FullName);
        //Acct Schema					
        private int p_VAB_AccountBook_ID = 0;
        //Date From				
        private DateTime? p_DateFrom = null;
        //Balance Aggregation         
        private int p_Actual_Accumulation_ID = 0;
        #endregion


        protected override void Prepare()
        {
            //	Parameter
            ProcessInfoParameter[] para = GetParameter();
            foreach (ProcessInfoParameter element in para)
            {
                String name = element.GetParameterName();
                if (element.GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("VAB_AccountBook_ID", StringComparison.OrdinalIgnoreCase))
                {
                    p_VAB_AccountBook_ID = Util.GetValueOfInt(element.GetParameter());
                }
                else if (name.Equals("DateFrom", StringComparison.OrdinalIgnoreCase))
                {
                    p_DateFrom = Util.GetValueOfDateTime(element.GetParameter());
                }
                else if (name.Equals("Actual_Accumulation_ID", StringComparison.OrdinalIgnoreCase))
                {
                    p_Actual_Accumulation_ID = Util.GetValueOfInt(element.GetParameter());
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }


        /// <summary>
        /// Perform process.
        /// </summary>
        /// <returns></returns>
        protected override String DoIt()
        {
            log.Fine("VAB_AccountBook_ID=" + p_VAB_AccountBook_ID
                + ",DateFrom=" + p_DateFrom);

            String msg = "";
            if (p_VAB_AccountBook_ID != 0)
            {
                msg = UpdateBalance(GetCtx(), p_VAB_AccountBook_ID,
                     p_DateFrom, Get_TrxName(), p_Actual_Accumulation_ID, this);
            }
            else
            {
                msg = UpdateBalanceClient(GetCtx(),
                     p_DateFrom, Get_TrxName(), p_Actual_Accumulation_ID, this);
            }
            return msg;
        }

        /**
         * 	Delete Balances
         * 	@param VAF_Client_ID client
         * 	@param VAB_AccountBook_ID	accounting schema 0 for all
         * 	@param dateFrom null for all or first date to delete
         * 	@param trx transaction
         * 	@param svrPrc optional server process
         *  @return Message to be translated
         */
        public static String DeleteBalance(int VAF_Client_ID, int VAB_AccountBook_ID,
            DateTime? dateFrom, Trx trxp, int Actual_Accumulation_ID, SvrProcess svrPrc)
        {
            Trx trx = trxp;
            //List<Object> param = new List<Object>();
            StringBuilder sql = new StringBuilder("DELETE FROM Actual_Acct_Balance WHERE VAF_Client_ID=" + VAF_Client_ID);
            //param.add(new Integer(VAF_Client_ID));
            if (VAB_AccountBook_ID != 0)
            {
                sql.Append(" AND VAB_AccountBook_ID=" + VAB_AccountBook_ID);
                // param.add(new Integer(VAB_AccountBook_ID));
            }
            if (dateFrom != null)
            {
                //SimpleDateFormat df = new SimpleDateFormat();
                //String finalDate = df.Format(dateFrom);
                //sql.Append(" AND DateAcct>= TO_DATE('" + finalDate + "','DD-MM-YYYY')");
                sql.Append(" AND DateAcct >= " + DB.TO_DATE(dateFrom));
            }
            if (Actual_Accumulation_ID != 0)
            {
                sql.Append(" AND Actual_Accumulation_ID = " + Actual_Accumulation_ID);
                //param.add(new Integer(Actual_Accumulation_ID));
            }
            //
            int no = DB.ExecuteQuery(sql.ToString(), null, trx);
            String msg = "@Deleted@=" + no;
            _log.Info("VAB_AccountBook_ID=" + VAB_AccountBook_ID
                + ",DateAcct=" + dateFrom
                + " #=" + no);
            if (svrPrc != null)
            {
                svrPrc.AddLog(0, dateFrom, new Decimal(no), "Deleted");
            }
            trx.Commit();
            //
            return msg;
        }

        /**
         * 	Update / Create Balances.
         * 	Called from FinReport, FactAcctReset (indirect)
         * 	@param VAF_Client_ID client
         * 	@param VAB_AccountBook_ID	accounting schema 0 for all
         * 	@param deleteFirst delete balances first
         * 	@param dateFrom null for all or first date to delete/calculate
         * 	@param trx transaction
         * 	@param svrPrc optional server process
         *  @return Message to be translated
         */
        public static String UpdateBalance(Ctx ctx, int VAB_AccountBook_ID,
            DateTime? dateFrom, Trx trxp, int Actual_Accumulation_ID,
            SvrProcess svrPrc)
        {
            Trx trx = trxp;
            _log.Info("VAB_AccountBook_ID=" + VAB_AccountBook_ID
                + "DateFrom=" + dateFrom);
            long start = CommonFunctions.CurrentTimeMillis();

            //List<Object> param = new List<Object>();
            List<MFactAccumulation> accums = null;
            int no = 0;

            if (Actual_Accumulation_ID == 0)
            {
                accums = MFactAccumulation.GetAll(ctx, VAB_AccountBook_ID);
                if (accums.Count == 0)
                {
                    // Create a Balance aggregation of type Daily.

                    MFactAccumulation defaultAccum = new MFactAccumulation(ctx, 0, trx);
                    defaultAccum.SetVAF_Client_ID(ctx.GetVAF_Client_ID());
                    defaultAccum.SetVAF_Org_ID(ctx.GetVAF_Org_ID());
                    defaultAccum.SetVAB_AccountBook_ID(VAB_AccountBook_ID);
                    defaultAccum.SetBALANCEACCUMULATION(X_Actual_Accumulation.BALANCEACCUMULATION_CalendarMonth);
                    defaultAccum.SetIsActive(true);
                    defaultAccum.SetIsDefault(true);
                    defaultAccum.SetISACTIVITY(true);
                    defaultAccum.SetISBUDGET(true);
                    defaultAccum.SetISBUSINESSPARTNER(true);
                    defaultAccum.SetISCAMPAIGN(true);
                    defaultAccum.SetISLOCATIONFROM(true);
                    defaultAccum.SetISLOCATIONTO(true);
                    defaultAccum.SetISPRODUCT(true);
                    defaultAccum.SetISPROJECT(true);
                    defaultAccum.SetISSALESREGION(true);
                    defaultAccum.SetISUSERLIST1(true);
                    defaultAccum.SetISUSERLIST2(true);
                    defaultAccum.SetISUSERELEMENT1(true);
                    defaultAccum.SetISUSERELEMENT2(true);
                    defaultAccum.SetIsUserElement3(true);
                    defaultAccum.SetIsUserElement4(true);
                    defaultAccum.SetIsUserElement5(true);
                    defaultAccum.SetIsUserElement6(true);
                    defaultAccum.SetIsUserElement7(true);
                    defaultAccum.SetIsUserElement8(true);
                    defaultAccum.SetIsUserElement9(true);
                    if (!defaultAccum.Save(trx))
                    {
                        _log.Log(Level.SEVERE, "Unable to create Default Balance Aggregation");
                        return "Unable to create Default Balance Aggregation";
                    }
                    else
                    {
                        accums.Add(defaultAccum);
                    }
                }
            }
            else
            {
                MFactAccumulation selectAccum = new MFactAccumulation(ctx, Actual_Accumulation_ID, trx);
                accums = new List<MFactAccumulation>();
                accums.Add(selectAccum);
            }

            foreach (MFactAccumulation accum in accums)
            {

                // dateFrom = MFactAccumulation.GetDateFrom(accum, dateFrom);
                dateFrom = accum.GetDateFrom();
                String type = accum.GetBALANCEACCUMULATION();
                //String trunc = null;

                if (String.IsNullOrEmpty(type))
                {
                    dateFrom = null;
                }

                //if (X_Actual_Accumulation.BALANCEACCUMULATION_Daily.Equals(type))
                //    trunc = TimeUtil.TRUNC_DAY;
                //else if (X_Actual_Accumulation.BALANCEACCUMULATION_CalendarWeek.Equals(type))
                //    trunc = TimeUtil.TRUNC_WEEK;
                //else if (X_Actual_Accumulation.BALANCEACCUMULATION_CalendarMonth.Equals(type))
                //    trunc = TimeUtil.TRUNC_MONTH;

                if (X_Actual_Accumulation.BALANCEACCUMULATION_PeriodOfAViennaCalendar.Equals(type) &&
                        !CheckPeriod(accum, dateFrom))
                {
                    _log.Log(Level.SEVERE, "Necessary Period doesn't exist for the calendar");
                    return "Necessary Period doesn't exist for the calendar";
                }

                String dateClause = null;
                if (X_Actual_Accumulation.BALANCEACCUMULATION_PeriodOfAViennaCalendar.Equals(type))
                {
                    dateClause = " Period.StartDate ";
                }
                else if (X_Actual_Accumulation.BALANCEACCUMULATION_Daily.Equals(type))
                {
                    dateClause = " TRUNC(a.DateAcct,'DD') ";
                }
                else if (X_Actual_Accumulation.BALANCEACCUMULATION_CalendarMonth.Equals(type))
                {
                    dateClause = " TRUNC(a.DateAcct,'MM') ";
                }
                else if (X_Actual_Accumulation.BALANCEACCUMULATION_CalendarWeek.Equals(type))
                {
                    dateClause = " TRUNC(a.DateAcct,'WW') "; // Calendar Week - WW  , Month Week - W
                }
                //else if (!String.IsNullOrEmpty(type))
                //{
                //    dateClause = " a.DateAcct ";
                //    //_log.Fine(trunc);
                //    //dateClause = " TRUNC(a.DateAcct, '" + trunc + "' ) ";
                //}

                // Delete the balances
                DeleteBalance(ctx.GetVAF_Client_ID(), VAB_AccountBook_ID,
                            dateFrom, trx, accum.GetACTUAL_ACCUMULATION_ID(), svrPrc);

                /** Insert		**/
                //param = new List<Object>();
                String insert = "INSERT INTO Actual_Acct_Balance "
                    + "(VAF_Client_ID, VAF_Org_ID, VAF_OrgTrx_ID, VAB_AccountBook_ID, DateAcct,"
                    + " Account_ID, PostingType, Actual_Accumulation_ID, M_Product_ID, VAB_BusinessPartner_ID,"
                    + "	VAB_Project_ID,	VAB_SalesRegionState_ID,VAB_BillingCode_ID,"
                    + " VAB_Promotion_ID, C_LocTo_ID, C_LocFrom_ID, User1_ID, User2_ID, VAGL_Budget_ID,"
                    + " UserElement1_ID, UserElement2_ID, UserElement3_ID, UserElement4_ID, UserElement5_ID, UserElement6_ID,"
                    + " UserElement7_ID, UserElement8_ID,UserElement9_ID, "
                    + " AmtAcctDr, AmtAcctCr, Qty) ";

                String select = " SELECT VAF_Client_ID, VAF_Org_ID, VAF_OrgTrx_ID, VAB_AccountBook_ID ";

                if (!String.IsNullOrEmpty(type))
                {
                    select = select + " , " + dateClause;
                }
                else
                {
                    // when balance Accumulation type = null, then consider system date as account date
                    select = select + " , SYSDATE  ";
                }
                select = select + " ,Account_ID, PostingType, " + accum.GetACTUAL_ACCUMULATION_ID();
                if (accum.IsPRODUCT())
                    select = select + " ,M_Product_ID ";
                else
                    select = select + " , NULL ";

                if (accum.IsBUSINESSPARTNER())
                    select = select + " , VAB_BusinessPartner_ID ";
                else
                    select = select + " ,NULL ";

                if (accum.IsPROJECT())
                    select = select + " ,VAB_Project_ID ";
                else
                    select = select + " , NULL ";

                if (accum.IsSALESREGION())
                    select = select + ", VAB_SalesRegionState_ID ";
                else
                    select = select + " ,NULL ";

                if (accum.IsACTIVITY())
                    select = select + " ,VAB_BillingCode_ID ";
                else
                    select = select + " ,NULL ";

                if (accum.IsCAMPAIGN())
                    select = select + " ,VAB_Promotion_ID ";
                else
                    select = select + " ,NULL ";

                if (accum.IsLOCATIONTO())
                    select = select + " ,C_LocTo_ID ";
                else
                    select = select + " ,NULL ";

                if (accum.IsLOCATIONFROM())
                    select = select + " ,C_LocFrom_ID ";
                else
                    select = select + " ,NULL ";

                if (accum.IsUSERLIST1())
                    select = select + " ,User1_ID ";
                else
                    select = select + " ,NULL ";

                if (accum.IsUSERLIST2())
                    select = select + " ,User2_ID ";
                else
                    select = select + " ,NULL ";

                if (accum.IsBUDGET())
                    select = select + " ,VAGL_Budget_ID ";
                else
                    select = select + " ,NULL ";

                if (accum.IsUSERELEMENT1())
                    select = select + " ,UserElement1_ID ";
                else
                    select = select + " ,NULL ";

                if (accum.IsUSERELEMENT2())
                    select = select + " , UserElement2_ID ";
                else
                    select = select + " ,NULL ";

                if (accum.IsUserElement3())
                    select = select + " ,UserElement3_ID ";
                else
                    select = select + " ,NULL ";

                if (accum.IsUserElement4())
                    select = select + " , UserElement4_ID ";
                else
                    select = select + " ,NULL ";

                if (accum.IsUserElement5())
                    select = select + " ,UserElement5_ID ";
                else
                    select = select + " ,NULL ";

                if (accum.IsUserElement6())
                    select = select + " , UserElement6_ID ";
                else
                    select = select + " ,NULL ";

                if (accum.IsUserElement7())
                    select = select + " ,UserElement7_ID ";
                else
                    select = select + " ,NULL ";

                if (accum.IsUserElement8())
                    select = select + " , UserElement8_ID ";
                else
                    select = select + " ,NULL ";

                if (accum.IsUserElement9())
                    select = select + " ,UserElement9_ID ";
                else
                    select = select + " ,NULL ";


                select = select + " ,COALESCE(SUM(AmtAcctDr),0), COALESCE(SUM(AmtAcctCr),0), COALESCE(SUM(Qty),0) ";

                String from = " FROM Actual_Acct_Detail a ";
                if (X_Actual_Accumulation.BALANCEACCUMULATION_PeriodOfAViennaCalendar.Equals(type))
                {
                    from += " ,(SELECT StartDate,EndDate FROM VAB_YearPeriod "
                        + " WHERE VAB_Year_ID IN (SELECT VAB_Year_ID "
                        + " FROM VAB_Year WHERE VAB_Calender_ID= " + accum.GetVAB_Calender_ID() + " ) "
                        + " AND IsActive='Y' AND PeriodType='S')  Period";
                    //param.add(new Integer(accum.GetVAB_Calender_ID()));
                }
                String where = " WHERE VAB_AccountBook_ID= " + VAB_AccountBook_ID;
                if (X_Actual_Accumulation.BALANCEACCUMULATION_PeriodOfAViennaCalendar.Equals(type))
                {
                    where += " AND a.DateAcct BETWEEN TRUNC(Period.StartDate,'DD') AND TRUNC(Period.EndDate,'DD') ";
                }
                //param.add(new Integer(VAB_AccountBook_ID));
                if (dateFrom != null)
                {
                    //SimpleDateFormat df = new SimpleDateFormat();//"dd-MM-yyyy"
                    //String finalDate = df.Format(dateFrom);
                    //where += " AND DateAcct>= TO_DATE('" + finalDate + "','DD-MM-YYYY') ";
                    where += " AND DateAcct >= " + DB.TO_DATE(dateFrom);
                }

                String groupBy = " GROUP BY VAF_Client_ID,VAB_AccountBook_ID, VAF_Org_ID, "
                    + " VAF_OrgTrx_ID,Account_ID, PostingType ";

                if (!String.IsNullOrEmpty(type))
                {
                    // when balance Accumulation type != null, then to consider in group by else not required
                    groupBy = groupBy + " , " + dateClause;
                }
                if (accum.IsPRODUCT())
                    groupBy = groupBy + " ,M_Product_ID ";
                if (accum.IsBUSINESSPARTNER())
                    groupBy = groupBy + " ,VAB_BusinessPartner_ID ";
                if (accum.IsPROJECT())
                    groupBy = groupBy + " ,VAB_Project_ID ";
                if (accum.IsSALESREGION())
                    groupBy = groupBy + " ,VAB_SalesRegionState_ID ";
                if (accum.IsACTIVITY())
                    groupBy = groupBy + ", VAB_BillingCode_ID ";
                if (accum.IsCAMPAIGN())
                    groupBy = groupBy + " ,VAB_Promotion_ID ";
                if (accum.IsLOCATIONTO())
                    groupBy = groupBy + ", C_LocTo_ID ";
                if (accum.IsLOCATIONFROM())
                    groupBy = groupBy + ", C_LocFrom_ID ";
                if (accum.IsUSERLIST1())
                    groupBy = groupBy + ", User1_ID";
                if (accum.IsUSERLIST2())
                    groupBy = groupBy + ", User2_ID ";
                if (accum.IsBUDGET())
                    groupBy = groupBy + ", VAGL_Budget_ID ";
                if (accum.IsUSERELEMENT1())
                    groupBy = groupBy + ", UserElement1_ID ";
                if (accum.IsUSERELEMENT2())
                    groupBy = groupBy + ", UserElement2_ID ";
                if (accum.IsUserElement3())
                    groupBy = groupBy + ", UserElement3_ID ";
                if (accum.IsUserElement4())
                    groupBy = groupBy + ", UserElement4_ID ";
                if (accum.IsUserElement5())
                    groupBy = groupBy + ", UserElement5_ID ";
                if (accum.IsUserElement6())
                    groupBy = groupBy + ", UserElement6_ID ";
                if (accum.IsUserElement7())
                    groupBy = groupBy + ", UserElement7_ID ";
                if (accum.IsUserElement8())
                    groupBy = groupBy + ", UserElement8_ID ";
                if (accum.IsUserElement9())
                    groupBy = groupBy + ", UserElement9_ID ";

                String sql = insert + select + from + where + groupBy;
                no = DB.ExecuteQuery(sql, null, trx);
                _log.Config("Inserts=" + no);
                if (svrPrc != null)
                    svrPrc.AddLog(0, dateFrom, new Decimal(no), "Inserts in " + accum.ToString());
                trx.Commit();
            }

            start = CommonFunctions.CurrentTimeMillis() - start;
            _log.Info((start / 1000) + " sec");
            return "#" + no;
        }

        /**
         * 	Update Balance of Client
         *	@param ctx context
         *	@param deleteFirst delete first
         * 	@param dateFrom null for all or first date to delete/calculate
         * 	@param trx transaction
         * 	@param svrPrc optional server process
         *	@return Info
         */
        public static String UpdateBalanceClient(Ctx ctx,
             DateTime? dateFrom, Trx trx, int Actual_Accumulation_ID, SvrProcess svrPrc)
        {
            int VAF_Client_ID = ctx.GetVAF_Client_ID();
            StringBuilder Info = new StringBuilder();
            MVABAccountBook[] ass = MVABAccountBook.GetClientAcctSchema(ctx, VAF_Client_ID);
            foreach (MVABAccountBook as1 in ass)
            {
                if (Info.Length > 0)
                    Info.Append(" - ");
                String msg = UpdateBalance(ctx, as1.GetVAB_AccountBook_ID(),
                     dateFrom, trx, Actual_Accumulation_ID, svrPrc);
                Info.Append(as1.GetName()).Append(":").Append(msg);
            }
            return Info.ToString();
        }

        private static Boolean CheckPeriod(MFactAccumulation accum, DateTime? dateFrom)
        {
            Boolean retVal = true;
            String sql = "  SELECT 1 FROM Actual_Acct_Detail a "
                        + " WHERE NOT EXISTS( SELECT 1 " +
                                            " FROM VAB_YearPeriod " +
                                            " WHERE VAB_Year_ID IN (SELECT VAB_Year_ID " +
                                                                " FROM VAB_Year " +
                                                                " WHERE VAB_Calender_ID= " + accum.GetVAB_Calender_ID() + ") " +
                                            " AND a.dateacct BETWEEN TRUNC(StartDate,'DD') AND TRUNC(EndDate,'DD')+ 0.999988 " +
                                            " AND VAB_YearPeriod.IsActive='Y' AND PeriodType='S') ";
            if (dateFrom != null)
            {
                sql += "  AND DateAcct >= '" + dateFrom.Value + "'";
            }

            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, accum.Get_TrxName());
                if (idr.Read())
                {
                    retVal = false;
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            return retVal;
        }


    }
}
