using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.ProcessEngine;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;


namespace VAdvantage.Process
{
    public class GenerateGroupSubGroup: SvrProcess
    {
        private int _C_FinRecordConfig_ID = 0;
        protected override void Prepare()
        {
            _C_FinRecordConfig_ID = GetRecord_ID();
           
        }

        protected override String DoIt()
        {
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                MFinRptConfig Report = new MFinRptConfig(GetCtx(), _C_FinRecordConfig_ID, Get_TrxName());
                Report.GetC_AccountGroupBatch_ID();
                Report.GetC_ReportType();

                StringBuilder sql =new StringBuilder();
                sql.Append( "Select * from c_accountgroup where c_accountgroupbatch_id=" + Report.GetC_AccountGroupBatch_ID());

                if (Report.GetC_ReportType() == "B")
                {
                    sql.Append(" AND SHOWINBALANCESHEET='Y'");
                }
                else if (Report.GetC_ReportType() == "P")
                {
                    sql.Append(" AND SHOWINPROFITLOSS='Y'");
                }
                else if (Report.GetC_ReportType() == "C")
                {
                    sql.Append(" AND SHOWINCASHFLOW='Y'");
                }
                else
                {
                    ;
                }

                dt = new DataTable();
                idr = DB.ExecuteReader(sql.ToString(), null,null);
                dt.Load(idr);
                idr.Close();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        MFinRptAcctGroup acctGroup = new MFinRptAcctGroup(GetCtx(), 0, null);
                        acctGroup.SetAD_Client_ID(Util.GetValueOfInt(dr["AD_Client_ID"]));
                        acctGroup.SetAD_Org_ID(Util.GetValueOfInt(dr["AD_Org_ID"]));
                        acctGroup.SetC_AccountGroup_ID(Util.GetValueOfInt(dr["C_ACCOUNTGROUP_ID"]));
                        acctGroup.SetC_FinRptConfig_ID(_C_FinRecordConfig_ID);
                        acctGroup.SetLine(Util.GetValueOfString(dr["Line"]));
                        if (acctGroup.Save())
                        {
                            StringBuilder query = new StringBuilder();
                            query.Append("Select * from C_AccountSubGroup where c_accountgroup_id=" + Util.GetValueOfInt(dr["C_ACCOUNTGROUP_ID"]));

                            if (Report.GetC_ReportType() == "B")
                            {
                                sql.Append(" AND SHOWINBALANCESHEET='Y'");
                            }
                            else if (Report.GetC_ReportType() == "P")
                            {
                                sql.Append(" AND SHOWINPROFITLOSS='Y'");
                            }
                            else if (Report.GetC_ReportType() == "C")
                            {
                                sql.Append(" AND SHOWINCASHFLOW='Y'");
                            }
                            else
                            {
                                ;
                            }
                            DataTable DT1 = new DataTable();
                            IDataReader Idr1 = DB.ExecuteReader(query.ToString());
                            DT1.Load(Idr1);
                            foreach (DataRow dr1 in DT1.Rows)
                            {
                                MFinRptAcctSubGroup AcctSubGroup = new MFinRptAcctSubGroup(GetCtx(), 0, null);
                                AcctSubGroup.SetAD_Client_ID(Util.GetValueOfInt(dr1["AD_Client_ID"]));
                                AcctSubGroup.SetAD_Org_ID(Util.GetValueOfInt(dr1["AD_Org_ID"]));
                                AcctSubGroup.SetC_AccountSubGroup_ID(Util.GetValueOfInt(dr1["C_ACCOUNTSUBGROUP_ID"]));
                                AcctSubGroup.SetC_FinRptAcctGroup_ID(acctGroup.GetC_FinRptAcctGroup_ID());
                                AcctSubGroup.SetLine(Util.GetValueOfString(dr1["LINE"]));
                                if (!AcctSubGroup.Save())
                                {
                                    return Msg.GetMsg(GetCtx(), "Lines not generated");
                                }
                            }
                        }

                    }
                    return Msg.GetMsg(GetCtx(), "Lines generated");
                }
                else
                {
                    return Msg.GetMsg(GetCtx(), "No record found");
                }
            }

            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                return e.Message;
            }
            finally
            {
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }
            return "";
        }
    }
}
