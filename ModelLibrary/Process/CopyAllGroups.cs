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
        int _VAB_FinRptConfig_ID = 0;
        int _VAB_AccountGroup_ID = 0;
        protected override void Prepare()
        {
            _VAB_FinRptConfig_ID = GetRecord_ID();
        }
        protected override String DoIt()
        {
            try
            {
                MFinRptConfig Report = new MFinRptConfig(GetCtx(), _VAB_FinRptConfig_ID, null );
                _VAB_AccountGroup_ID = Report.GetVAB_AccountGroupBatch_ID();
                String Query = "Select * from VAB_AccountGroup where VAB_AccountGroupbatch_id=" + _VAB_AccountGroup_ID ;
                //String _sql="Select * from VAB_AccountGroup g inner join VAB_AccountSubGroup s on (g.VAB_AccountGroup_ID=s.VAB_AccountGroup_ID AND C_AccountBatchGroup_ID="+ _VAB_AccountGroup_ID +" ) ";
                DataTable Dt = new DataTable();
                IDataReader Idr = DB.ExecuteReader(Query);
                Dt.Load(Idr);
                if (Dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in Dt.Rows)
                    {
                        MFinRptAcctGroup AcctGroup = new MFinRptAcctGroup(GetCtx(), 0, null);
                        AcctGroup.SetVAF_Client_ID(Util.GetValueOfInt(dr["VAF_Client_ID"]));
                        AcctGroup.SetVAF_Org_ID(Util.GetValueOfInt(dr["VAF_Org_ID"]));
                        AcctGroup.SetVAB_AccountGroup_ID(Util.GetValueOfInt(dr["VAB_ACCOUNTGROUP_ID"]));
                        AcctGroup.SetVAB_FinRptConfig_ID(_VAB_FinRptConfig_ID);
                        AcctGroup.SetLine(Util.GetValueOfString(dr["LINE"]));
                        if (AcctGroup.Save())
                        {
                            String qry = "Select * from VAB_AccountSubGroup where VAB_AccountGroup_id=" + AcctGroup.GetVAB_AccountGroup_ID();
                            DataTable DT1 = new DataTable();
                            IDataReader Idr1 = DB.ExecuteReader(qry);
                            DT1.Load(Idr1);
                            foreach (DataRow dr1 in DT1.Rows)
                            {
                                MFinRptAcctSubGroup AcctSubGroup = new MFinRptAcctSubGroup(GetCtx(), 0, null);
                                AcctSubGroup.SetVAF_Client_ID(Util.GetValueOfInt(dr1["VAF_Client_ID"]));
                                AcctSubGroup.SetVAF_Org_ID(Util.GetValueOfInt(dr1["VAF_Org_ID"]));
                                AcctSubGroup.SetVAB_AccountSubGroup_ID(Util.GetValueOfInt(dr1["VAB_ACCOUNTSUBGROUP_ID"]));
                                AcctSubGroup.SetVAB_FinRptAcctGroup_ID(AcctGroup.GetVAB_FinRptAcctGroup_ID());
                                AcctSubGroup.SetLine(Util.GetValueOfString(dr1["LINE"]));
                                if (!AcctSubGroup.Save())
                                {
                                    return GetRetrievedError(AcctSubGroup, "Lines not generated");
                                   /// return Msg.GetMsg(GetCtx(), "Lines not generated");
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
