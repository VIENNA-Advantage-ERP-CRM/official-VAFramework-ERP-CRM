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
        private int p_C_AcctSchema_ID = 0;
        //Date From				
        private DateTime? p_DateFrom = null;
        //Balance Aggregation         
        private int p_Fact_Accumulation_ID = 0;
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
                else if (name.Equals("C_AcctSchema_ID"))
                {
                    p_C_AcctSchema_ID = Util.GetValueOfInt(element.GetParameter());
                }
                else if (name.Equals("DateFrom"))
                {
                    p_DateFrom = Util.GetValueOfDateTime(element.GetParameter());
                }
                else if (name.Equals("Fact_Accumulation_ID"))
                {
                    p_Fact_Accumulation_ID = Util.GetValueOfInt(element.GetParameter());
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
            log.Fine("C_AcctSchema_ID=" + p_C_AcctSchema_ID
                + ",DateFrom=" + p_DateFrom);

            String msg = "";
            if (p_C_AcctSchema_ID != 0)
            {
                msg = UpdateBalance(GetCtx(), p_C_AcctSchema_ID,
                     p_DateFrom, Get_TrxName(), p_Fact_Accumulation_ID, this);
            }
            else
            {
                msg = UpdateBalanceClient(GetCtx(),
                     p_DateFrom, Get_TrxName(), p_Fact_Accumulation_ID, this);
            }
            return msg;
        }

        /**
         * 	Delete Balances
         * 	@param AD_Client_ID client
         * 	@param C_AcctSchema_ID	accounting schema 0 for all
         * 	@param dateFrom null for all or first date to delete
         * 	@param trx transaction
         * 	@param svrPrc optional server process
         *  @return Message to be translated
         */
        public static String DeleteBalance(int AD_Client_ID, int C_AcctSchema_ID,
            DateTime? dateFrom, Trx trxp, int Fact_Accumulation_ID, SvrProcess svrPrc)
        {
            Trx trx = trxp;
            //List<Object> param = new List<Object>();
            StringBuilder sql = new StringBuilder("DELETE FROM Fact_Acct_Balance WHERE AD_Client_ID=" + AD_Client_ID);
            //param.add(new Integer(AD_Client_ID));
            if (C_AcctSchema_ID != 0)
            {
                sql.Append(" AND C_AcctSchema_ID=" + C_AcctSchema_ID);
                // param.add(new Integer(C_AcctSchema_ID));
            }
            if (dateFrom != null)
            {
                //SimpleDateFormat df = new SimpleDateFormat();
                //String finalDate = df.Format(dateFrom);
                //sql.Append(" AND DateAcct>= TO_DATE('" + finalDate + "','DD-MM-YYYY')");
                sql.Append(" AND DateAcct >= " + DB.TO_DATE(dateFrom));
            }
            if (Fact_Accumulation_ID != 0)
            {
                sql.Append(" AND Fact_Accumulation_ID = " + Fact_Accumulation_ID);
                //param.add(new Integer(Fact_Accumulation_ID));
            }
            //
            int no = DB.ExecuteQuery(sql.ToString(), null, trx);
            String msg = "@Deleted@=" + no;
            _log.Info("C_AcctSchema_ID=" + C_AcctSchema_ID
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
         * 	@param AD_Client_ID client
         * 	@param C_AcctSchema_ID	accounting schema 0 for all
         * 	@param deleteFirst delete balances first
         * 	@param dateFrom null for all or first date to delete/calculate
         * 	@param trx transaction
         * 	@param svrPrc optional server process
         *  @return Message to be translated
         */
        public static String UpdateBalance(Ctx ctx, int C_AcctSchema_ID,
            DateTime? dateFrom, Trx trxp, int Fact_Accumulation_ID,
            SvrProcess svrPrc)
        {
            Trx trx = trxp;
            _log.Info("C_AcctSchema_ID=" + C_AcctSchema_ID
                + "DateFrom=" + dateFrom);
            long start = CommonFunctions.CurrentTimeMillis();

            //List<Object> param = new List<Object>();
            List<MFactAccumulation> accums = null;
            int no = 0;

            if (Fact_Accumulation_ID == 0)
            {
                accums = MFactAccumulation.GetAll(ctx, C_AcctSchema_ID);
                if (accums.Count == 0)
                {
                    // Create a Balance aggregation of type Daily.

                    MFactAccumulation defaultAccum = new MFactAccumulation(ctx, 0, trx);
                    defaultAccum.SetAD_Client_ID(ctx.GetAD_Client_ID());
                    defaultAccum.SetAD_Org_ID(ctx.GetAD_Org_ID());
                    defaultAccum.SetC_AcctSchema_ID(C_AcctSchema_ID);
                    defaultAccum.SetBALANCEACCUMULATION(X_Fact_Accumulation.BALANCEACCUMULATION_Daily);
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
                MFactAccumulation selectAccum = new MFactAccumulation(ctx, Fact_Accumulation_ID, trx);
                accums = new List<MFactAccumulation>();
                accums.Add(selectAccum);
            }

            foreach (MFactAccumulation accum in accums)
            {
                
               // dateFrom = MFactAccumulation.GetDateFrom(accum, dateFrom);
                dateFrom = accum.GetDateFrom();
                String type = accum.GetBALANCEACCUMULATION();
                String trunc = null;

                if (X_Fact_Accumulation.BALANCEACCUMULATION_Daily.Equals(type))
                    trunc = TimeUtil.TRUNC_DAY;
                else if (X_Fact_Accumulation.BALANCEACCUMULATION_CalendarWeek.Equals(type))
                    trunc = TimeUtil.TRUNC_WEEK;
                else if (X_Fact_Accumulation.BALANCEACCUMULATION_CalendarMonth.Equals(type))
                    trunc = TimeUtil.TRUNC_MONTH;

                if (X_Fact_Accumulation.BALANCEACCUMULATION_PeriodOfAViennaCalendar.Equals(type) &&
                        !CheckPeriod(accum, dateFrom))
                {
                    _log.Log(Level.SEVERE, "Necessary Period doesn't exist for the calendar");
                    return "Necessary Period doesn't exist for the calendar";
                }

                String dateClause = null;
                if (X_Fact_Accumulation.BALANCEACCUMULATION_PeriodOfAViennaCalendar.Equals(type))
                {
                    dateClause = " Period.StartDate ";

                }
                else
                {
                    dateClause = " a.DateAcct ";
                    _log.Fine(trunc);
                    //dateClause = " TRUNC(a.DateAcct, '" + trunc + "' ) ";
                }

                // Delete the balances
                DeleteBalance(ctx.GetAD_Client_ID(), C_AcctSchema_ID,
                            dateFrom, trx, accum.GetFACT_ACCUMULATION_ID(), svrPrc);

                /** Insert		**/
                //param = new List<Object>();
                String insert = "INSERT INTO Fact_Acct_Balance "
                    + "(AD_Client_ID, AD_Org_ID, AD_OrgTrx_ID, C_AcctSchema_ID, DateAcct,"
                    + " Account_ID, PostingType, Fact_Accumulation_ID, M_Product_ID, C_BPartner_ID,"
                    + "	C_Project_ID,	C_SalesRegion_ID,C_Activity_ID,"
                    + " C_Campaign_ID, C_LocTo_ID, C_LocFrom_ID, User1_ID, User2_ID, GL_Budget_ID,"
                    + " UserElement1_ID, UserElement2_ID, "
                    + " AmtAcctDr, AmtAcctCr, Qty) ";

                String select = " SELECT AD_Client_ID, AD_Org_ID, AD_OrgTrx_ID, C_AcctSchema_ID, ";
                select = select + dateClause;
                select = select + " ,Account_ID, PostingType, " + accum.GetFACT_ACCUMULATION_ID();
                if (accum.IsPRODUCT())
                    select = select + " ,M_Product_ID ";
                else
                    select = select + " , NULL ";

                if (accum.IsBUSINESSPARTNER())
                    select = select + " , C_BPartner_ID ";
                else
                    select = select + " ,NULL ";

                if (accum.IsPROJECT())
                    select = select + " ,C_Project_ID ";
                else
                    select = select + " , NULL ";

                if (accum.IsSALESREGION())
                    select = select + ", C_SalesRegion_ID ";
                else
                    select = select + " ,NULL ";

                if (accum.IsACTIVITY())
                    select = select + " ,C_Activity_ID ";
                else
                    select = select + " ,NULL ";

                if (accum.IsCAMPAIGN())
                    select = select + " ,C_Campaign_ID ";
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
                    select = select + " ,GL_Budget_ID ";
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

                select = select + " ,COALESCE(SUM(AmtAcctDr),0), COALESCE(SUM(AmtAcctCr),0), COALESCE(SUM(Qty),0) ";

                String from = " FROM Fact_Acct a ";
                if (X_Fact_Accumulation.BALANCEACCUMULATION_PeriodOfAViennaCalendar.Equals(type))
                {
                    from += " ,(SELECT StartDate,EndDate FROM C_Period "
                        + " WHERE C_Year_ID IN (SELECT C_Year_ID "
                        + " FROM C_Year WHERE C_Calendar_ID= " + accum.GetC_Calendar_ID() + " ) "
                        + " AND IsActive='Y' AND PeriodType='S')  Period";
                    //param.add(new Integer(accum.GetC_Calendar_ID()));
                }
                String where = " WHERE C_AcctSchema_ID= " + C_AcctSchema_ID;
                if (X_Fact_Accumulation.BALANCEACCUMULATION_PeriodOfAViennaCalendar.Equals(type))
                {
                    where += " AND a.DateAcct BETWEEN TRUNC(Period.StartDate,'DD') AND TRUNC(Period.EndDate,'DD') ";
                }
                //param.add(new Integer(C_AcctSchema_ID));
                if (dateFrom != null)
                {
                    //SimpleDateFormat df = new SimpleDateFormat();//"dd-MM-yyyy"
                    //String finalDate = df.Format(dateFrom);
                    //where += " AND DateAcct>= TO_DATE('" + finalDate + "','DD-MM-YYYY') ";
                    where += " AND DateAcct >= " + DB.TO_DATE(dateFrom);
                }

                String groupBy = " GROUP BY AD_Client_ID,C_AcctSchema_ID, AD_Org_ID, "
                    + " AD_OrgTrx_ID,Account_ID, PostingType," + dateClause;
                if (accum.IsPRODUCT())
                    groupBy = groupBy + " ,M_Product_ID ";
                if (accum.IsBUSINESSPARTNER())
                    groupBy = groupBy + " ,C_BPartner_ID ";
                if (accum.IsPROJECT())
                    groupBy = groupBy + " ,C_Project_ID ";
                if (accum.IsSALESREGION())
                    groupBy = groupBy + " ,C_SalesRegion_ID ";
                if (accum.IsACTIVITY())
                    groupBy = groupBy + ", C_Activity_ID ";
                if (accum.IsCAMPAIGN())
                    groupBy = groupBy + " ,C_Campaign_ID ";
                if (accum.IsLOCATIONTO())
                    groupBy = groupBy + ", C_LocTo_ID ";
                if (accum.IsLOCATIONFROM())
                    groupBy = groupBy + ", C_LocFrom_ID ";
                if (accum.IsUSERLIST1())
                    groupBy = groupBy + ", User1_ID";
                if (accum.IsUSERLIST2())
                    groupBy = groupBy + ", User2_ID ";
                if (accum.IsBUDGET())
                    groupBy = groupBy + ", GL_Budget_ID ";
                if (accum.IsUSERELEMENT1())
                    groupBy = groupBy + ", UserElement1_ID ";
                if (accum.IsUSERELEMENT2())
                    groupBy = groupBy + ", UserElement2_ID ";

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
             DateTime? dateFrom, Trx trx, int Fact_Accumulation_ID, SvrProcess svrPrc)
        {
            int AD_Client_ID = ctx.GetAD_Client_ID();
            StringBuilder Info = new StringBuilder();
            MAcctSchema[] ass = MAcctSchema.GetClientAcctSchema(ctx, AD_Client_ID);
            foreach (MAcctSchema as1 in ass)
            {
                if (Info.Length > 0)
                    Info.Append(" - ");
                String msg = UpdateBalance(ctx, as1.GetC_AcctSchema_ID(),
                     dateFrom, trx, Fact_Accumulation_ID, svrPrc);
                Info.Append(as1.GetName()).Append(":").Append(msg);
            }
            return Info.ToString();
        }

        private static Boolean CheckPeriod(MFactAccumulation accum, DateTime? dateFrom)
        {
            Boolean retVal = true;
            String sql = "  SELECT 1 FROM fact_acct a "
                        + " WHERE NOT EXISTS( SELECT 1 " +
                                            " FROM C_Period " +
                                            " WHERE C_Year_ID IN (SELECT C_Year_ID " +
                                                                " FROM C_Year " +
                                                                " WHERE C_Calendar_ID= " + accum.GetC_Calendar_ID() + ") " +
                                            " AND a.dateacct BETWEEN TRUNC(StartDate,'DD') AND TRUNC(EndDate,'DD')+ 0.999988 " +
                                            " AND c_period.IsActive='Y' AND PeriodType='S') ";
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
