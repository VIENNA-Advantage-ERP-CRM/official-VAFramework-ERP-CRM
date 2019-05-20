using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using VAdvantage.Model;
using VAdvantage.DataBase;
using System.Data;
using VAdvantage.Utility;

namespace VAdvantage.Process
{
    class CopyAllGroups : SvrProcess
    {
        int _C_FinRptConfig_ID = 0;
        int _C_AccountGroup_ID = 0;
        protected override void Prepare()
        {
            _C_FinRptConfig_ID = GetRecord_ID();
        }
        protected override String DoIt()
        {
            try
            {
                MFinRptConfig Report = new MFinRptConfig(GetCtx(), _C_FinRptConfig_ID, null );
                _C_AccountGroup_ID = Report.GetC_AccountGroupBatch_ID();
                String Query = "Select * from c_accountgroup where c_accountgroupbatch_id=" + _C_AccountGroup_ID ;
                //String _sql="Select * from c_accountgroup g inner join C_AccountSubGroup s on (g.c_accountgroup_ID=s.c_accountgroup_ID AND C_AccountBatchGroup_ID="+ _C_AccountGroup_ID +" ) ";
                DataTable Dt = new DataTable();
                IDataReader Idr = DB.ExecuteReader(Query);
                Dt.Load(Idr);
                if (Dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in Dt.Rows)
                    {
                        MFinRptAcctGroup AcctGroup = new MFinRptAcctGroup(GetCtx(), 0, null);
                        AcctGroup.SetAD_Client_ID(Util.GetValueOfInt(dr["AD_Client_ID"]));
                        AcctGroup.SetAD_Org_ID(Util.GetValueOfInt(dr["AD_Org_ID"]));
                        AcctGroup.SetC_AccountGroup_ID(Util.GetValueOfInt(dr["C_ACCOUNTGROUP_ID"]));
                        AcctGroup.SetC_FinRptConfig_ID(_C_FinRptConfig_ID);
                        AcctGroup.SetLine(Util.GetValueOfString(dr["LINE"]));
                        if (AcctGroup.Save())
                        {
                            String qry = "Select * from C_AccountSubGroup where c_accountgroup_id=" + AcctGroup.GetC_AccountGroup_ID();
                            DataTable DT1 = new DataTable();
                            IDataReader Idr1 = DB.ExecuteReader(qry);
                            DT1.Load(Idr1);
                            foreach (DataRow dr1 in DT1.Rows)
                            {
                                MFinRptAcctSubGroup AcctSubGroup = new MFinRptAcctSubGroup(GetCtx(), 0, null);
                                AcctSubGroup.SetAD_Client_ID(Util.GetValueOfInt(dr1["AD_Client_ID"]));
                                AcctSubGroup.SetAD_Org_ID(Util.GetValueOfInt(dr1["AD_Org_ID"]));
                                AcctSubGroup.SetC_AccountSubGroup_ID(Util.GetValueOfInt(dr1["C_ACCOUNTSUBGROUP_ID"]));
                                AcctSubGroup.SetC_FinRptAcctGroup_ID(AcctGroup.GetC_FinRptAcctGroup_ID());
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
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
