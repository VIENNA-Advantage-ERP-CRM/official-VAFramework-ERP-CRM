using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class CardViewModel
    {
        public List<CardViewPropeties> GetCardView(int ad_Window_ID, int vaf_tab_ID, Ctx ctx)
        {
            List<CardViewPropeties> lstCardView = null;
            //string sqlQuery = "SELECT * FROM VAF_CardView WHERE AD_Window_id=" + ad_Window_ID + " and VAF_Tab_id=" + vaf_tab_ID + " AND (createdby=" + ctx.GetAD_User_ID() + " OR AD_USER_ID Is NULL OR AD_User_ID = " + ctx.GetAD_User_ID() + ") AND VAF_Client_ID=" + ctx.GetVAF_Client_ID();
            //string sqlQuery = " SELECT * FROM VAF_CardView c WHERE c.AD_Window_id=" + ad_Window_ID + " and c.VAF_Tab_id=" + vaf_tab_ID + " AND (c.createdby=" + ctx.GetAD_User_ID() +
            //                  " OR ((c.AD_USER_ID    IS NULL) AND exists (select * from VAF_CardView_role r where r.VAF_CardView_id = c.VAF_CardView_id and r.ad_role_id = " + ctx.GetAD_Role_ID() + ")) OR c.AD_User_ID     = " + ctx.GetAD_User_ID() +
            //                  " ) AND c.VAF_Client_ID  =" + ctx.GetVAF_Client_ID();

            //   string sqlQuery = " SELECT * FROM VAF_CardView c WHERE c.AD_Window_id=" + ad_Window_ID + " and c.VAF_Tab_id=" + vaf_tab_ID + " AND c.VAF_Client_ID  =" + ctx.GetVAF_Client_ID();

            string sqlQuery = @"SELECT VAF_CardView.*,AD_DefaultCardView.AD_DefaultCardView_ID,AD_DefaultCardView.AD_User_ID as userID
                            FROM VAF_CardView
                            LEFT OUTER JOIN AD_DefaultCardView
                            ON VAF_CardView.VAF_CardView_ID=AD_DefaultCardView.VAF_CardView_ID
                            WHERE VAF_CardView.AD_Window_id=" + ad_Window_ID + " and VAF_CardView.VAF_Tab_id=" + vaf_tab_ID + " AND VAF_CardView.VAF_Client_ID  =" + ctx.GetVAF_Client_ID() + @" 
                            ORDER BY VAF_CardView.Name Asc";


            DataSet ds = DB.ExecuteDataset(sqlQuery);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                lstCardView = new List<CardViewPropeties>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    bool isDefault=false;
                    if(ds.Tables[0].Rows[i]["AD_DefaultCardView_ID"] != null && ds.Tables[0].Rows[i]["AD_DefaultCardView_ID"] != DBNull.Value
                       && ds.Tables[0].Rows[i]["userID"] != null && ds.Tables[0].Rows[i]["userID"] != DBNull.Value 
                       && ctx.GetAD_User_ID() == Util.GetValueOfInt(ds.Tables[0].Rows[i]["userID"]))
                    {
                        isDefault = true;
                    }


                    CardViewPropeties objCardView = new CardViewPropeties()
                    {
                        CardViewName = Convert.ToString(ds.Tables[0].Rows[i]["NAME"]),
                        CardViewID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_CardView_ID"]),
                        UserID = VAdvantage.Utility.Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_USER_ID"]),
                        VAF_GroupField_ID = VAdvantage.Utility.Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_FIELD_ID"]),
                        CreatedBy = Convert.ToInt32(ds.Tables[0].Rows[i]["CREATEDBY"]),
                        DefaultID = isDefault
                    };
                    lstCardView.Add(objCardView);
                }
            }
            return lstCardView;
        }
        public List<RolePropeties> GetCardViewRole(int VAF_CardView_ID, Ctx ctx)
        {
            List<RolePropeties> lstCardViewRole = null;
            RolePropeties objCardView = null;
            string sqlQuery = "SELECT AD_ROLE_ID,VAF_CardView_ID from VAF_CardView_ROLE WHERE VAF_CardView_id=" + VAF_CardView_ID + " AND VAF_Client_ID=" + ctx.GetVAF_Client_ID();
            //  string sqlQuery = "SELECT * FROM VAF_CardView WHERE AD_Window_id=" + ad_Window_ID + " and VAF_Tab_id=" + vaf_tab_ID + " AND (AD_USER_ID=" + ctx.GetAD_User_ID() + " OR AD_USER_ID Is NULL )" ;
            // sqlQuery = MRole.GetDefault(ctx).AddAccessSQL(sqlQuery, "VAF_CardView", false, false);
            DataSet ds = DB.ExecuteDataset(sqlQuery);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                lstCardViewRole = new List<RolePropeties>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    objCardView = new RolePropeties()
                    {
                        AD_Role_ID = VAdvantage.Utility.Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Role_ID"]),
                        VAF_CardView_ID = VAdvantage.Utility.Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_CardView_ID"])
                    };
                    lstCardViewRole.Add(objCardView);
                }
            }
            return lstCardViewRole;
        }

        public List<CardViewConditionPropeties> GetCardViewCondition(int VAF_CardView_ID, Ctx ctx)
        {
            List<CardViewConditionPropeties> lstCardViewRole = null;
            CardViewConditionPropeties objCardView = null;
            string sqlQuery = "SELECT * FROM VAF_CardView_Condition WHERE VAF_CardView_id=" + VAF_CardView_ID + " AND VAF_Client_ID=" + ctx.GetVAF_Client_ID() + " ORDER BY VAF_CardView_Condition_ID";
            //  string sqlQuery = "SELECT * FROM VAF_CardView WHERE AD_Window_id=" + ad_Window_ID + " and VAF_Tab_id=" + vaf_tab_ID + " AND (AD_USER_ID=" + ctx.GetAD_User_ID() + " OR AD_USER_ID Is NULL )" ;
            // sqlQuery = MRole.GetDefault(ctx).AddAccessSQL(sqlQuery, "VAF_CardView", false, false);
            DataSet ds = DB.ExecuteDataset(sqlQuery);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                lstCardViewRole = new List<CardViewConditionPropeties>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    objCardView = new CardViewConditionPropeties()
                    {
                        Color = VAdvantage.Utility.Util.GetValueOfString(ds.Tables[0].Rows[i]["Color"]),
                        ConditionValue = VAdvantage.Utility.Util.GetValueOfString(ds.Tables[0].Rows[i]["ConditionValue"]),
                        ConditionText = VAdvantage.Utility.Util.GetValueOfString(ds.Tables[0].Rows[i]["ConditionText"]),
                    };
                    lstCardViewRole.Add(objCardView);
                }
            }
            return lstCardViewRole;
        }
        public List<UserPropeties> GetAllUsers(Ctx ctx)
        {
            List<UserPropeties> lstUser = null;
            string sqlQuery = "SELECT * FROM AD_User WHERE ISACTIVE='Y' AND VAF_Client_ID=" + ctx.GetVAF_Client_ID();
            //   sqlQuery = MRole.GetDefault(ctx).AddAccessSQL(sqlQuery, "AD_User", false, false);
            DataSet ds = DB.ExecuteDataset(sqlQuery);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                lstUser = new List<UserPropeties>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    UserPropeties objCardView = new UserPropeties()
                    {
                        UserName = Convert.ToString(ds.Tables[0].Rows[i]["NAME"]),
                        AD_User_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_User_ID"])
                    };
                    lstUser.Add(objCardView);
                }
            }
            return lstUser;
        }

        public List<RolePropeties> GetAllRoles(Ctx ctx)
        {
            List<RolePropeties> lstRole = null;

            string sqlQuery = "SELECT  r.AD_Role_ID,  r.Name  FROM AD_User u INNER JOIN AD_User_Roles ur ON (u.AD_User_ID=ur.AD_User_ID AND ur.IsActive ='Y') " +
                        " INNER JOIN AD_Role r ON (ur.AD_Role_ID =r.AD_Role_ID AND r.IsActive ='Y') WHERE u.AD_User_ID = " + ctx.GetAD_User_ID() + " AND u.IsActive ='Y' AND EXISTS " +
                        " (SELECT * FROM VAF_Client c WHERE u.VAF_Client_ID=c.VAF_Client_ID AND c.IsActive      ='Y' ) " +
                        " AND EXISTS (SELECT * FROM VAF_Client c WHERE r.VAF_Client_ID=c.VAF_Client_ID AND c.IsActive      ='Y' )";

            DataSet ds = DB.ExecuteDataset(sqlQuery);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                lstRole = new List<RolePropeties>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    RolePropeties objCardView = new RolePropeties()
                    {
                        RoleName = Convert.ToString(ds.Tables[0].Rows[i]["NAME"]),
                        AD_Role_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Role_ID"])
                    };
                    lstRole.Add(objCardView);
                }
            }
            return lstRole;
        }
        public List<CardViewPropeties> GetCardViewColumns(int VAF_CardView_id, Ctx ctx)
        {
            int uid = 0;
            int fid = 0;
            List<CardViewPropeties> lstCardViewColumns = new List<CardViewPropeties>();
            string sqlQuery1 = "SELECT AD_User_ID,VAF_Field_ID  FROM VAF_CardView WHERE VAF_CardView_id=" + VAF_CardView_id;
            DataSet ds1 = DB.ExecuteDataset(sqlQuery1);
            if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
            {
                uid = VAdvantage.Utility.Util.GetValueOfInt(ds1.Tables[0].Rows[0][0]);
                fid = VAdvantage.Utility.Util.GetValueOfInt(ds1.Tables[0].Rows[0][1]);
            }
            string sqlQuery = "SELECT * FROM(SELECT crdcol.*,fl.name FROM VAF_CardView_column crdcol INNER JOIN vaf_field fl on crdcol.vaf_field_id=fl.vaf_field_id  WHERE VAF_CardView_id=" + VAF_CardView_id + ") cardviewcols";
            //  sqlQuery = MRole.GetDefault(ctx).AddAccessSQL(sqlQuery, "cardviewcols", false, false);
            sqlQuery += " ORDER BY seqno";
            DataSet ds = DB.ExecuteDataset(sqlQuery);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    CardViewPropeties objCardView = new CardViewPropeties()
                    {
                        FieldName = Convert.ToString(ds.Tables[0].Rows[i]["NAME"]),
                        VAF_Field_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_FIELD_ID"]),
                        VAF_GroupField_ID = fid,
                        UserID = uid

                    };
                    lstCardViewColumns.Add(objCardView);
                }
            }
            else
            {
                CardViewPropeties objCardView = new CardViewPropeties()
                {
                    FieldName = "",
                    VAF_Field_ID = 0,
                    VAF_GroupField_ID = fid,
                    UserID = uid

                };
                lstCardViewColumns.Add(objCardView);
            }
            return lstCardViewColumns;
        }
        public int SaveCardViewRecord(string cardViewName, int ad_Window_ID, int vaf_tab_ID, int ad_User_ID, int vaf_field_ID, Ctx ctx, int cardViewID, List<RolePropeties> lstRoleId, List<CardViewConditionPropeties> lstCVCondition)
        {
            string conditionValue = string.Empty;
            string conditionText = string.Empty;
            bool isupdate = false;
            MCardView objCardView = null;
            if (cardViewID <= 0)
            {
                objCardView = new MCardView(ctx, 0, null);

            }
            else
            {
                objCardView = new MCardView(ctx, cardViewID, null);
                isupdate = true;
            }
            objCardView.SetAD_Window_ID(ad_Window_ID);
            objCardView.SetVAF_Tab_ID(vaf_tab_ID);
            objCardView.SetAD_User_ID(ad_User_ID);
            objCardView.SetVAF_Field_ID(vaf_field_ID);
            objCardView.SetName(cardViewName);
            if (!objCardView.Save())
            {
            }
            if (isupdate)
            {
                DeleteAllCardViewRole(objCardView.Get_ID(), ctx);
                DeleteAllCardViewCondition(objCardView.Get_ID(), ctx);
            }
            if (lstRoleId != null && lstRoleId.Count > 0)
            {
                for (int i = 0; i < lstRoleId.Count; i++)
                {
                    MCardViewRole objMCVR = new MCardViewRole(ctx, 0, null);
                    objMCVR.SetVAF_CardView_ID(objCardView.Get_ID());
                    objMCVR.SetAD_Role_ID(lstRoleId[i].AD_Role_ID);
                    if (!objMCVR.Save())
                    {
                    }
                }
            }

            if (lstCVCondition != null && lstCVCondition.Count > 0)
            {
                for (int i = 0; i < lstCVCondition.Count; i++)
                {
                    if (lstCVCondition[i].ConditionValue != null && lstCVCondition[i].ConditionText != null)
                    {
                        conditionValue = lstCVCondition[i].ConditionValue.Trim();
                        conditionText = lstCVCondition[i].ConditionText.Trim();
                        MCardViewCondition objMCVR = new MCardViewCondition(ctx, 0, null);
                        objMCVR.SetVAF_CardView_ID(objCardView.Get_ID());
                        objMCVR.SetColor(lstCVCondition[i].Color);
                        objMCVR.SetConditionValue(conditionValue.Trim());
                        objMCVR.SetConditionText(conditionText.Trim());
                        if (!objMCVR.Save())
                        {
                        }
                    }
                }
            }
            return objCardView.Get_ID();
        }

        public void SetDefaultCardView(Ctx ctx, int cardViewID, int VAF_Tab_ID)
        {
            string sql = "SELECT AD_DefaultCardView_ID FROM AD_DefaultCardView WHERE VAF_Tab_ID=" + VAF_Tab_ID + " AND AD_User_ID=" + ctx.GetAD_User_ID();
            object id = DB.ExecuteScalar(sql);

            int AD_DefaultCardView_ID = 0;
            if (id != null && id != DBNull.Value)
            {
                AD_DefaultCardView_ID = Convert.ToInt32(id);
            }

            X_AD_DefaultCardView cardView = new X_AD_DefaultCardView(ctx, AD_DefaultCardView_ID, null);
            cardView.SetVAF_Tab_ID(VAF_Tab_ID);
            cardView.SetAD_User_ID(ctx.GetAD_User_ID());
            cardView.SetVAF_CardView_ID(Convert.ToInt32(cardViewID));
            cardView.Save();
        }


        public void SaveCardViewColumns(int VAF_CardView_id, int vaf_field_ID, int sqNo, Ctx ctx)
        {

            MCardViewColumn objCardViewColumn = new MCardViewColumn(ctx, 0, null);
            objCardViewColumn.SetVAF_CardView_ID(VAF_CardView_id);
            objCardViewColumn.SetVAF_Field_ID(vaf_field_ID);
            objCardViewColumn.SetSeqNo(sqNo);
            if (!objCardViewColumn.Save())
            {
            }
        }
        public void DeleteCardView(int VAF_CardView_ID, Ctx ctx)
        {
            string sqlQuery = "DELETE FROM VAF_CardView WHERE VAF_CardView_ID=" + VAF_CardView_ID + " AND VAF_Client_ID=" + ctx.GetVAF_Client_ID();
            int result = DB.ExecuteQuery(sqlQuery);
            if (result < 1)
            {

            }
        }
        public void DeleteAllCardViewColumns(int VAF_CardView_ID, Ctx ctx)
        {
            string sqlQuery = "DELETE FROM VAF_CardView_COLUMN WHERE VAF_CardView_ID=" + VAF_CardView_ID + " AND VAF_Client_ID=" + ctx.GetVAF_Client_ID();
            int result = DB.ExecuteQuery(sqlQuery);
            if (result < 1)
            {

            }
        }
        public void DeleteAllCardViewRole(int VAF_CardView_ID, Ctx ctx)
        {
            string sqlQuery = "DELETE FROM VAF_CardView_ROLE WHERE VAF_CardView_ID=" + VAF_CardView_ID + " AND VAF_Client_ID=" + ctx.GetVAF_Client_ID();
            int result = DB.ExecuteQuery(sqlQuery);
            if (result < 1)
            {

            }
        }
        public void DeleteAllCardViewCondition(int VAF_CardView_ID, Ctx ctx)
        {
            string sqlQuery = "DELETE FROM VAF_CardView_CONDITION WHERE VAF_CardView_ID=" + VAF_CardView_ID + " AND VAF_Client_ID=" + ctx.GetVAF_Client_ID();
            int result = DB.ExecuteQuery(sqlQuery);
            if (result < 1)
            {

            }
        }
        public void DeleteCardViewColumns(int VAF_CardViewColumn_ID, Ctx ctx)
        {
            string sqlQuery = "DELETE FROM VAF_CardView_COLUMN WHERE VAF_CardView_COLUMN_ID=" + VAF_CardViewColumn_ID;
            int result = DB.ExecuteQuery(sqlQuery);
            if (result < 1)
            {

            }
        }
        public void UpdateCardViewColumnPosition(int VAF_CardViewColumn_ID, int seqNo, int VAF_CardView_ID, bool isUp, Ctx ctx)
        {
            int seqNo1 = 0;
            int id = 0; if (isUp)
            {
                seqNo1 = seqNo - 10;
            }
            else
            {
                seqNo1 = seqNo + 10;
            }
            string query = "SELECT VAF_CardView_column_ID FROM VAF_CardView_column WHERE VAF_CardView_id=" + VAF_CardView_ID + " AND seqno =" + seqNo1;
            id = Util.GetValueOfInt(DB.ExecuteScalar(query));

            MCardViewColumn objCardViewColumn = new MCardViewColumn(ctx, VAF_CardViewColumn_ID, null);
            if (isUp)
            {
                objCardViewColumn.SetSeqNo(seqNo - 10);
            }
            else
            {
                objCardViewColumn.SetSeqNo(seqNo + 10);
            }
            if (!objCardViewColumn.Save())
            {
            }
            MCardViewColumn objCardViewColumn1 = new MCardViewColumn(ctx, id, null);
            if (isUp)
            {
                objCardViewColumn1.SetSeqNo(seqNo);
            }
            else
            {
                objCardViewColumn1.SetSeqNo(seqNo);
            }
            if (!objCardViewColumn.Save())
            {
            }
        }
        public void DeleteCardViewRecord(int VAF_CardView_ID, Ctx ctx)
        {
            DeleteCardView(VAF_CardView_ID, ctx);
            DeleteAllCardViewColumns(VAF_CardView_ID, ctx);
            DeleteAllCardViewRole(VAF_CardView_ID, ctx);
        }


        public void SetDefaultView(Ctx ctx, int VAF_Tab_ID, int cardView)
        {

            string sql = "SELECT AD_DefaultCardView_ID FROM AD_DefaultCardView WHERE VAF_Tab_ID=" + VAF_Tab_ID + " AND AD_User_ID=" + ctx.GetAD_User_ID();
            object id = DB.ExecuteScalar(sql);
            int AD_DefaultcarView_ID = 0;
            if (id != null && id != DBNull.Value)
            {
                AD_DefaultcarView_ID = Convert.ToInt32(id);
            }


            X_AD_DefaultCardView userQuery = new X_AD_DefaultCardView(ctx, AD_DefaultcarView_ID, null);
            userQuery.SetVAF_Tab_ID(VAF_Tab_ID);
            userQuery.SetAD_User_ID(ctx.GetAD_User_ID());
            userQuery.SetVAF_CardView_ID(cardView);
            userQuery.Save();
        }

    }

    public class CardViewPropeties
    {
        public string CardViewName { get; set; }
        public int CardViewID { get; set; }
        public int AD_Window_ID { get; set; }
        public int VAF_Tab_ID { get; set; }
        public int UserID { get; set; }

        public string FieldName { get; set; }
        public int VAF_Field_ID { get; set; }
        public int VAF_GroupField_ID { get; set; }
        public int VAF_CardViewColumn_ID { get; set; }
        public int SeqNo { get; set; }
        public bool isNewRecord { get; set; }
        public bool IsDefault { get; set; }
        public int CreatedBy { get; set; }
        public bool DefaultID { get; set; }
    }

    public class UserPropeties
    {
        public string UserName { get; set; }
        public int AD_User_ID { get; set; }

    }
    public class RolePropeties
    {
        public string RoleName { get; set; }
        public int AD_Role_ID { get; set; }
        public int VAF_CardView_ID { get; set; }

    }
    public class CardViewConditionPropeties
    {
        public string Color { get; set; }
        public string ConditionValue { get; set; }
        public string ConditionText { get; set; }

    }
    public class ParameterPropeties
    {
        public List<UserPropeties> lstUserData { get; set; }
        public List<RolePropeties> lstRoleData { get; set; }
        public List<CardViewPropeties> lstCardViewData { get; set; }
        public List<List<RolePropeties>> lstCardViewRoleData { get; set; }
        public List<CardViewConditionPropeties> lstCardViewConditonData { get; set; }
    }
}