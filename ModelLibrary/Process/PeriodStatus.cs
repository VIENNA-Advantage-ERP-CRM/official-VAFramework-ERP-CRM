/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : PeriodStatus
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     25-Jun-2009
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

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class PeriodStatus : ProcessEngine.SvrProcess
    {
        //Period					
        private int _C_Period_ID = 0;
        //Action					
        private String _PeriodAction = null;


        /// <summary>
        ///  Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {

                }
                else if (name.Equals("PeriodAction"))
                {
                    _PeriodAction = (String)para[i].GetParameter();
                }
                else
                {
                    //log.log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _C_Period_ID = GetRecord_ID();
        }

        /**
         * 	Process
         *	@return message
         *	@throws Exception
         */
        protected override String DoIt()
        {
            //log.info("C_Period_ID=" + _C_Period_ID + ", PeriodAction=" + _PeriodAction);
            MPeriod period = new MPeriod(GetCtx(), _C_Period_ID, Get_TrxName());
            if (period.Get_ID() == 0)
                throw new Exception("@NotFound@  @C_Period_ID@=" + _C_Period_ID);

            StringBuilder sql = new StringBuilder("UPDATE C_PeriodControl ");
            sql.Append("SET PeriodStatus='");
            //	Open
            if (MPeriodControl.PERIODACTION_OpenPeriod.Equals(_PeriodAction))
                sql.Append(MPeriodControl.PERIODSTATUS_Open);
            //	Close
            else if (MPeriodControl.PERIODACTION_ClosePeriod.Equals(_PeriodAction))
                sql.Append(MPeriodControl.PERIODSTATUS_Closed);
            //	Close Permanently
            else if (MPeriodControl.PERIODACTION_PermanentlyClosePeriod.Equals(_PeriodAction))
                sql.Append(MPeriodControl.PERIODSTATUS_PermanentlyClosed);
            else
                return "-";
            //
            sql.Append("', PeriodAction='N', Updated=SysDate,UpdatedBy=").Append(GetAD_User_ID());
            //	WHERE
            sql.Append(" WHERE C_Period_ID=").Append(period.GetC_Period_ID())
                .Append(" AND PeriodStatus<>'P'")
                .Append(" AND PeriodStatus<>'").Append(_PeriodAction).Append("'");



            int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName());

            // Lokesh Chauhan 17-Dec-2013
            /* Change For setting date From in Balance Aggregation window
              For Fact Account Balance updation */
            if (no >= 0)
            {
                if (MPeriodControl.PERIODACTION_ClosePeriod.Equals(_PeriodAction))
                {
                    try
                    {
                        string sqlSchID = "SELECT C_AcctSchema_ID FROM C_AcctSchema WHERE IsActive = 'Y' AND AD_Client_ID = " + GetCtx().GetAD_Client_ID();
                        DataSet ds = DB.ExecuteDataset(sqlSchID);

                        if (ds != null)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                int C_AcctSchema_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_AcctSchema_ID"]);
                                string sqlUpd = "UPDATE Fact_Accumulation SET DateFrom = " + DB.TO_DATE(period.GetStartDate().Value.AddDays(-1)) + " WHERE IsActive = 'Y' AND AD_Client_ID = " + GetCtx().GetAD_Client_ID();
                                no = DB.ExecuteQuery(sqlUpd, null, Get_TrxName());
                                if (Get_Trx().Commit())
                                {
                                    VAdvantage.Report.FinBalance.UpdateBalance(GetCtx(), C_AcctSchema_ID, period.GetStartDate().Value.AddDays(-1), Get_TrxName(), 0, this);
                                }
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }

            CacheMgt.Get().Reset("C_PeriodControl", 0);
            CacheMgt.Get().Reset("C_Period", _C_Period_ID);
            return "@Period Updated@ #" + no;
        }
    }
}
