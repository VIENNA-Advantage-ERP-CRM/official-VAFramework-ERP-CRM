using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using ViennaAdvantage.Model;

namespace ViennaAdvantage.Model
{
    public class MTeamMember : X_C_TeamMember
    {

        public MTeamMember(Ctx ctx, DataRow dr, Trx trx)
            : base(ctx, dr, trx)
        {

        }

        public MTeamMember(Ctx ctx, int C_TeamMember_ID, Trx trx)
            : base(ctx, C_TeamMember_ID, trx)
        {

        }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (newRecord)
            {
                string Sql = "SELECT object_name FROM all_objects WHERE object_type IN ('TABLE','VIEW') AND (object_name)  = UPPER('VADMS_TEAM_ACCESS') AND OWNER LIKE '" + DB.GetSchema() + "'";
                string ObjectName = Convert.ToString(DB.ExecuteScalar(Sql));
                if (ObjectName != "")
                {
                    Sql = @"SELECT Distinct AD_Table_ID,record_ID FROM VADMS_Team_Access where C_Team_ID=" + GetC_Team_ID() + " AND IsActive='Y'";
                    DataSet ds = DB.ExecuteDataset(Sql);
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            //MTable table = MTable.Get(GetCtx(), "VADMS_Team_Access");
                            //PO pos = table.GetPO(GetCtx(), 0, null);

                            //pos.Set_Value("AD_Table_ID", Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Table_ID"]));
                            //pos.Set_Value("AD_User_ID", GetAD_User_ID());
                            //pos.Set_Value("Record_ID", Convert.ToInt32(ds.Tables[0].Rows[i]["Record_ID"]));
                            //pos.Set_Value("C_Team_ID", GetC_Team_ID());
                            //pos.Set_Value("VADMS_Access", "20");
                            //pos.Save();


                            string strQuery = "SELECT VADMS_ACCESS FROM (SELECT record_id,vadms_access,ad_role_id, NULL as AD_USER_ID " +
                            ",vadms_role_access.vadms_role_access_id,null as vadms_user_access_ID " +
                             "FROM vadms_role_access WHERE RECORD_ID=" + Convert.ToInt32(ds.Tables[0].Rows[i]["Record_ID"]) + " AND ad_table_id = " + Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Table_ID"]) + "  AND record_id NOT IN" +
                             "(SELECT record_id FROM vadms_user_access WHERE ad_user_id = " + GetAD_User_ID() + " and ad_table_id = " + Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Table_ID"]) + ") " +

                             "UNION " +

                             "SELECT record_id, vadms_access, NULL as AD_Role_ID, AD_USER_ID ,NULL AS vadms_role_access_id,vadms_user_access.vadms_user_access_ID FROM vadms_user_access WHERE RECORD_ID=" + Convert.ToInt32(ds.Tables[0].Rows[i]["Record_ID"]) + " AND ad_user_id = " + GetAD_User_ID() + " and ad_table_id =" + Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Table_ID"]) +
                             ") ARQ ";

                            int documentAccess = Convert.ToInt32(DB.ExecuteScalar(strQuery));


                            if (documentAccess == 0)
                            {

                                MTable tableUserAccess = MTable.Get(GetCtx(), "VADMS_User_Access");
                                PO posUserAccess = tableUserAccess.GetPO(GetCtx(), 0, null);

                                posUserAccess.Set_Value("AD_Table_ID", Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Table_ID"]));
                                posUserAccess.Set_Value("AD_User_ID", GetAD_User_ID());
                                posUserAccess.Set_Value("Record_ID", Convert.ToInt32(ds.Tables[0].Rows[i]["Record_ID"]));
                                posUserAccess.Set_Value("VADMS_Access", "20");
                                posUserAccess.Save();
                            }

                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// If User already exists then prevent it from creating new line for the same user.
        /// Added by Sukhwinder on 4th january, 2018
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns></returns>
        protected override bool BeforeSave(bool newRecord)
        {
            string Sql = "SELECT AD_USER_ID FROM C_TEAMMEMBER WHERE AD_USER_ID = " + GetAD_User_ID() + " AND C_TEAM_ID = "+ GetC_Team_ID();
            int UserID = Util.GetValueOfInt(DB.ExecuteScalar(Sql));
            if (UserID > 0)
            {
                log.SaveError(Msg.Translate(GetCtx(), "VIS_MemberAlreadyExists"), "");
                return false;
            }

            return true;
        }
    }
}
