/********************************************************
 * Module Name    : VIS
 * Purpose        : Assign roles to user and windows, forms, processes to Roles
 * Chronological Development
 * Karan          3-June-2015
 ******************************************************/


using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.WF;
using VIS.DBase;

namespace VIS.Models
{
    public class Group
    {
        private Ctx ctx = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx"></param>
        public Group(Ctx ctx)
        {
            this.ctx = ctx;
        }

        /// <summary>
        /// Get All Users of current client and org
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="sortBy"></param>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<UserInfo> GetUserInfo(string searchText, int sortBy, int pageNo, int pageSize)
        {
            List<UserInfo> uInfo = new List<UserInfo>();
            int UserTableID = MVAFTableView.Get_Table_ID("VAF_UserContact");
            int UserWindowID = Convert.ToInt32(DB.ExecuteScalar("SELECT VAF_Screen_ID from VAF_Screen WHERE Name='User'", null, null));

            if (!(bool)MVAFRole.GetDefault(ctx).GetWindowAccess(UserWindowID))
            {
                return uInfo;
            }

            if (!MVAFRole.GetDefault(ctx).IsTableAccess(UserTableID, false))
            {
                return uInfo;
            }


            string sql = @"SELECT VAF_UserContact.Name,
                                      VAF_UserContact.Email,
                                      VAF_UserContact.VAF_UserContact_ID,
                                      VAF_UserContact.IsActive,
                                      VAF_UserContact.VAF_Image_ID,
                                        VAF_UserContact.VAF_Client_ID,
                                        VAF_UserContact.VAF_Org_ID,
                                      VAB_Country.Name as CName
                                    FROM VAF_UserContact
                                    LEFT OUTER JOIN VAB_Address
                                    ON VAF_UserContact.VAB_Address_ID=VAB_Address.VAB_Address_ID
                                    LEFT OUTER JOIN VAB_Country
                                    ON VAB_Country.VAB_Country_ID=VAB_Address.VAB_Country_ID WHERE IsLoginUser='Y' ";
            if (!String.IsNullOrEmpty(searchText))
            {
                sql += " AND ( upper(VAF_UserContact.Value) like Upper('%" + searchText + "%') OR upper(VAF_UserContact.Name) like Upper('%" + searchText + "%')  OR  upper(VAF_UserContact.Email) like Upper('%" + searchText + "%'))";
            }
            sql += " ORDER BY  VAF_UserContact.IsActive desc";

            if (sortBy == -1 || sortBy == 1)
            {
                sql += " , upper(VAF_UserContact.Name) ASC";
            }
            else if (sortBy == 2)
            {
                sql += " , upper(VAF_UserContact.Value) ASC";
            }
            else if (sortBy == 3)
            {
                sql += " , upper(VAF_UserContact.Email) ASC";
            }

            sql = MVAFRole.GetDefault(ctx).AddAccessSQL(sql, "VAF_UserContact", true, false);

            DataSet ds = DB.ExecuteDatasetPaging(sql, pageNo, pageSize);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    UserInfo userInfo = new UserInfo();

                    userInfo.HasAccess = MVAFRole.GetDefault(ctx).IsRecordAccess(UserTableID, Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_UserContact_ID"]), true);
                    userInfo.Username = Convert.ToString(ds.Tables[0].Rows[i]["Name"]);
                    userInfo.Email = Convert.ToString(ds.Tables[0].Rows[i]["Email"]);
                    userInfo.VAF_UserContactID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_UserContact_ID"]);
                    userInfo.VAF_OrgID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Org_ID"]);
                    userInfo.VAF_ClientID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Client_ID"]);
                    userInfo.Country = Convert.ToString(ds.Tables[0].Rows[i]["CName"]);
                    userInfo.UserTableID = UserTableID;
                    userInfo.UserWindowID = UserWindowID;
                    userInfo.IsActive = ds.Tables[0].Rows[i]["IsActive"].ToString() == "Y" ? true : false;

                    userInfo.IsUpdate = MVAFRole.GetDefault(ctx).CanUpdate(userInfo.VAF_ClientID, userInfo.VAF_OrgID, userInfo.UserTableID, userInfo.VAF_UserContactID, false);

                    if (ds.Tables[0].Rows[i]["VAF_Image_ID"] != DBNull.Value && ds.Tables[0].Rows[i]["VAF_Image_ID"] != null && Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Image_ID"]) > 0)
                    {
                        MVAFImage mimg = new MVAFImage(ctx, Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Image_ID"]), null);
                        var imgfll = mimg.GetThumbnailURL(46, 46);
                        userInfo.UserImage = imgfll;

                        if (userInfo.UserImage == "FileDoesn'tExist" || userInfo.UserImage == "NoRecordFound")
                        {
                            userInfo.UserImage = "";
                        }
                    }
                    else
                    {
                        userInfo.UserImage = "";
                    }
                    uInfo.Add(userInfo);
                }
            }

            return uInfo;

        }

        /// <summary>
        /// Set current user Active
        /// </summary>
        /// <param name="VAF_UserContact_ID"></param>
        /// <returns></returns>
        public int ActiveUser(int VAF_UserContact_ID)
        {
            return DB.ExecuteQuery("Update VAF_UserContact Set IsActive='Y' WHERE VAF_UserContact_ID=" + VAF_UserContact_ID, null, null);
        }

        /// <summary>
        /// Set current user inActive
        /// </summary>
        /// <param name="VAF_UserContact_ID"></param>
        /// <returns></returns>
        public int InActiveUser(int VAF_UserContact_ID)
        {
            return DB.ExecuteQuery("Update VAF_UserContact Set IsActive='N' WHERE VAF_UserContact_ID=" + VAF_UserContact_ID, null, null);
        }

        /// <summary>
        /// Get all the roles of current User. if a role is assigned then it will be shown as checked otherwise unchecked
        /// </summary>
        /// <param name="VAF_UserContact_ID"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<RolesInfo> GetRoleInfo(int VAF_UserContact_ID, string name)
        {
            List<RolesInfo> rInfo = new List<RolesInfo>();

            int RoleWindowID = Convert.ToInt32(DB.ExecuteScalar("SELECT VAF_Screen_ID from VAF_Screen WHERE Name='Role'", null, null));

            if (!(bool)MVAFRole.GetDefault(ctx).GetWindowAccess(RoleWindowID))
            {
                return rInfo;
            }

            int UserTableID = MVAFTableView.Get_Table_ID("VAF_UserContact");
            bool IsUpdate = true;
            String sql = "SELECT VAF_Client_ID, vaf_org_ID from VAF_UserContact WHERE VAF_UserContact_ID=" + VAF_UserContact_ID;
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                IsUpdate = MVAFRole.GetDefault(ctx).CanUpdate(Convert.ToInt32(ds.Tables[0].Rows[0]["VAF_Client_ID"]), Convert.ToInt32(ds.Tables[0].Rows[0]["VAF_Org_ID"]), UserTableID, VAF_UserContact_ID, false);
            }






            sql = @"Select VAF_Role.VAF_Role_ID, VAF_Role.Name from VAF_Role WHERE VAF_Role.VAF_Client_ID=" + ctx.GetVAF_Client_ID() + " AND VAF_Role.VAF_Role_ID > 0 AND  IsActive='Y'";


            //            string sql = @"SELECT VAF_Role.VAF_Role_ID,
            //                          VAF_Role.Name
            //                        FROM VAF_Role
            //                        JOIN VAF_UserContact_Roles
            //
            //                        on VAF_Role.VAF_Role_ID=VAF_UserContact_Roles.VAF_Role_ID
            //                    WHERE VAF_UserContact_Roles.IsActive='Y' AND VAF_Role.IsActive='Y' AND  VAF_UserContact_Roles.VAF_UserContact_ID=" + ctx.GetVAF_UserContact_ID();

            if (name != null && name.Length > 0)
            {
                sql += " AND upper(VAF_Role.Name) like ('%" + name.ToUpper() + "%')";
            }

            sql += " ORDER BY upper(VAF_Role.Name)";
            sql = MVAFRole.GetDefault(ctx).AddAccessSQL(sql, "VAF_Role", true, false);
            ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    RolesInfo roleInfo = new RolesInfo();
                    roleInfo.VAF_Role_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Role_ID"]);
                    roleInfo.Name = Convert.ToString(ds.Tables[0].Rows[i]["Name"]);
                    roleInfo.IsAssignedToUser = false;
                    roleInfo.roleWindowID = RoleWindowID;
                    roleInfo.IsUpdate = IsUpdate;
                    rInfo.Add(roleInfo);
                }


                sql = "Select VAF_Role_ID, IsActive from VAF_UserContact_Roles where VAF_UserContact_ID=" + VAF_UserContact_ID;
                DataSet dsURoles = DB.ExecuteDataset(sql);
                if (dsURoles != null && dsURoles.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsURoles.Tables[0].Rows.Count; i++)
                    {
                        RolesInfo rrinfo = rInfo.Where(a => a.VAF_Role_ID == Convert.ToInt32(dsURoles.Tables[0].Rows[i]["VAF_Role_ID"])).FirstOrDefault();
                        if (rrinfo != null)
                        {
                            if (dsURoles.Tables[0].Rows[i]["IsActive"].ToString().Equals("Y"))
                            {
                                rrinfo.IsAssignedToUser = true;
                            }
                            else
                            {
                                rrinfo.IsAssignedToUser = false;
                            }
                        }
                    }
                }
            }

            rInfo = rInfo.OrderBy(a => !a.IsAssignedToUser).ToList();

            return rInfo;
        }

        /// <summary>
        /// Set User's Role Active/InActive in VAF_UserContact_Roles if already exist. If not exist for current role and user then create new entry.
        /// </summary>
        /// <param name="VAF_UserContact_ID"></param>
        /// <param name="roles"></param>
        public void UpdateUserRoles(int VAF_UserContact_ID, List<RolesInfo> roles)
        {
            string sql = "SELECT isActive, VAF_Role_ID FROM VAF_UserContact_Roles WHERE VAF_UserContact_ID= " + VAF_UserContact_ID;
            DataSet ds = DB.ExecuteDataset(sql);

            StringBuilder setActive = new StringBuilder();
            StringBuilder setInActive = new StringBuilder();

            for (int i = 0; i < roles.Count; i++)
            {
                if (roles[i].IsAssignedToUser)
                {
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow[] dr = ds.Tables[0].Select("VAF_Role_ID=" + roles[i].VAF_Role_ID);
                        if (dr != null && dr.Length > 0)
                        {
                            if (setActive.Length > 0)
                            {
                                setActive.Append(" OR ");
                            }
                            setActive.Append("(VAF_Role_ID=" + roles[i].VAF_Role_ID + " AND VAF_UserContact_ID=" + VAF_UserContact_ID + ")");
                        }
                        else
                        {
                            CreateNewUserRole(roles[i].VAF_Role_ID, VAF_UserContact_ID);
                        }
                    }
                    else
                    {
                        CreateNewUserRole(roles[i].VAF_Role_ID, VAF_UserContact_ID);
                    }
                }
                else
                {
                    DataRow[] dr = ds.Tables[0].Select("VAF_Role_ID=" + roles[i].VAF_Role_ID);
                    if (dr != null && dr.Length > 0)
                    {
                        if (setInActive.Length > 0)
                        {
                            setInActive.Append(" OR ");
                        }
                        setInActive.Append("(VAF_Role_ID=" + roles[i].VAF_Role_ID + " AND VAF_UserContact_ID=" + VAF_UserContact_ID + ")");
                    }
                }

            }

            if (setActive.Length > 0)
            {
                DB.ExecuteQuery("Update VAF_UserContact_Roles Set IsActive='Y' WHERE " + setActive.ToString(), null, null);
            }
            if (setInActive.Length > 0)
            {
                DB.ExecuteQuery("Update VAF_UserContact_Roles Set IsActive='N' WHERE " + setInActive.ToString(), null, null);
            }


        }

        /// <summary>
        /// Assign New Role to Current User.
        /// </summary>
        /// <param name="VAF_Role_ID"></param>
        /// <param name="VAF_UserContact_ID"></param>
        private void CreateNewUserRole(int VAF_Role_ID, int VAF_UserContact_ID)
        {
            MVAFUserContactRoles role = new MVAFUserContactRoles(ctx, 0, null);
            role.SetVAF_Org_ID(ctx.GetVAF_Org_ID());
            role.SetVAF_Client_ID(ctx.GetVAF_Client_ID());
            role.SetVAF_UserContact_ID(VAF_UserContact_ID);
            role.SetVAF_Role_ID(VAF_Role_ID);
            role.Save();
        }

        /// <summary>
        /// Get All the groups of current client. If a group is assigned to current role then it will be checked otherwise unchecked.
        /// </summary>
        /// <param name="VAF_Role_ID"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<GroupInfo> GetGroupInfo(int VAF_Role_ID, string name)
        {
            List<GroupInfo> gInfo = new List<GroupInfo>();
            int groupWindowID = Convert.ToInt32(DB.ExecuteScalar("SELECT VAF_Screen_ID from VAF_Screen WHERE Name='Group Rights'", null, null));

            if (MVAFRole.GetDefault(ctx).GetWindowAccess(groupWindowID) == null || !(bool)MVAFRole.GetDefault(ctx).GetWindowAccess(groupWindowID))
            {
                return gInfo;
            }

            string sql = @"SELECT Name, VAF_GroupInfo_ID FROM VAF_GroupInfo WHERE IsActive='Y'";

            if (!string.IsNullOrEmpty(name))
            {
                sql += " AND upper(Name) like ('%" + name.ToUpper() + "%')";
            }

            sql += " ORDER BY upper(name) ";
            sql = MVAFRole.GetDefault(ctx).AddAccessSQL(sql, "VAF_GroupInfo", true, false);

            DataSet ds = DB.ExecuteDataset(sql);        // get All Groups.
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    GroupInfo roleInfo = new GroupInfo();
                    roleInfo.VAF_Group_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_GroupInfo_ID"]);
                    roleInfo.Name = Convert.ToString(ds.Tables[0].Rows[i]["Name"]);
                    roleInfo.IsAssignedToUser = false;
                    roleInfo.GroupWindowID = groupWindowID;
                    gInfo.Add(roleInfo);
                }
            }

            sql = "select VAF_Role_group.VAF_GroupInfo_ID,VAF_Role_group.IsActive from VAF_Role_group join VAF_GroupInfo on VAF_Role_group.VAF_GroupInfo_ID=VAF_GroupInfo.VAF_GroupInfo_ID WHERE  VAF_Role_group.VAF_Role_ID=" + VAF_Role_ID;
            if (!string.IsNullOrEmpty(name))
            {
                sql += " AND upper(VAF_GroupInfo.Name) like ('%" + name.ToUpper() + "%')";
            }

            sql = MVAFRole.GetDefault(ctx).AddAccessSQL(sql, "VAF_GroupInfo", true, false);

            DataSet dsURoles = DB.ExecuteDataset(sql);                  // Get All groups that are assigned to current Role...
            if (dsURoles != null && dsURoles.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsURoles.Tables[0].Rows.Count; i++)
                {
                    GroupInfo ginofs = gInfo.Where(a => a.VAF_Group_ID == Convert.ToInt32(dsURoles.Tables[0].Rows[i]["VAF_GroupInfo_ID"])).FirstOrDefault();

                    if (ginofs != null)
                    {
                        if (dsURoles.Tables[0].Rows[i]["IsActive"].ToString().Equals("Y"))      // if a group is assigned to current role, then show it as checked
                        {
                            ginofs.IsAssignedToUser = true;
                        }
                        else
                        {
                            ginofs.IsAssignedToUser = false;
                        }
                    }
                }
            }

            gInfo = gInfo.OrderBy(a => !a.IsAssignedToUser).ToList();       // Show assigned Users on the Top.

            return gInfo;
        }

        /// <summary>
        /// Assign/UnAssign group to roles.
        /// </summary>
        /// <param name="VAF_Role_ID"></param>
        /// <param name="groups"></param>
        public void UpdateUserGroup(int VAF_Role_ID, List<GroupInfo> groups)
        {
            string sql = " select isActive,VAF_Groupinfo_id from VAF_Role_group where VAF_Role_ID =" + VAF_Role_ID;
            DataSet ds = DB.ExecuteDataset(sql);

            StringBuilder setActive = new StringBuilder();
            StringBuilder setInActive = new StringBuilder();

            for (int i = 0; i < groups.Count; i++)
            {
                bool newGroupCreated = false;
                if (groups[i].IsAssignedToUser)
                {
                    if (ds != null && ds.Tables[0].Rows.Count > 0)      // if current group is assigned to role 
                    {
                        DataRow[] dr = ds.Tables[0].Select("VAF_GroupInfo_ID=" + groups[i].VAF_Group_ID);
                        if (dr != null && dr.Length > 0)            // check if assigned group was inActive VAF_Role_Group then Active it.
                        {
                            if (setActive.Length > 0)
                            {
                                setActive.Append(" OR ");
                            }
                            setActive.Append("(VAF_GroupInfo_ID=" + groups[i].VAF_Group_ID + " AND VAF_Role_ID=" + VAF_Role_ID + ")");
                        }
                        else
                        {
                            CreateNewUserGroup(VAF_Role_ID, groups[i].VAF_Group_ID);  // else create new role group
                            newGroupCreated = true;
                        }
                    }
                    else
                    {
                        CreateNewUserGroup(VAF_Role_ID, groups[i].VAF_Group_ID);  // else create new role group
                        newGroupCreated = true;
                    }
                }
                else// if current is group is unchecked..
                {
                    DataRow[] dr = ds.Tables[0].Select("VAF_GroupInfo_ID=" + groups[i].VAF_Group_ID);
                    if (dr != null && dr.Length > 0)        // if group is already assigned and now marked unchecked, then delete it from VAF_Role_Group
                    {
                        if (setInActive.Length > 0)
                        {
                            setInActive.Append(" OR ");
                        }
                        setInActive.Append("(VAF_GroupInfo_ID=" + groups[i].VAF_Group_ID + " AND VAF_Role_ID=" + VAF_Role_ID + ")");
                    }
                }
               
                DataRow[] drr = ds.Tables[0].Select("VAF_GroupInfo_ID=" + groups[i].VAF_Group_ID);
                if ((drr != null && drr.Length > 0) || newGroupCreated)     // update group's window/ form/process/workflow    OR      window Create new entry in window/ form/process/workflow
                {
                    ProvideWindowAccessToRole(groups[i].VAF_Group_ID, VAF_Role_ID, groups[i].IsAssignedToUser);
                    ProvideFormAccessToRole(groups[i].VAF_Group_ID, VAF_Role_ID, groups[i].IsAssignedToUser);
                    ProvideProcessAccessToRole(groups[i].VAF_Group_ID, VAF_Role_ID, groups[i].IsAssignedToUser);
                    ProvideWorkflowAccessToRole(groups[i].VAF_Group_ID, VAF_Role_ID, groups[i].IsAssignedToUser);
                }

            }

            if (setActive.Length > 0)
            {
                DB.ExecuteQuery("Update VAF_Role_Group Set IsActive='Y' WHERE " + setActive.ToString(), null, null);
            }
            if (setInActive.Length > 0)
            {
                //DB.ExecuteQuery("Update VAF_Role_Group Set IsActive='N' WHERE " + setInActive.ToString(), null, null);
                DB.ExecuteQuery("DELETE FROM VAF_Role_Group WHERE " + setInActive.ToString(), null, null);
            }
            //}


        }

        /// <summary>
        /// Assign Role to group
        /// </summary>
        /// <param name="VAF_Role_ID"></param>
        /// <param name="VAF_Group_ID"></param>
        private void CreateNewUserGroup(int VAF_Role_ID, int VAF_Group_ID)
        {
            X_VAF_Role_Group role = new X_VAF_Role_Group(ctx, 0, null);
            role.SetVAF_Org_ID(ctx.GetVAF_Org_ID());
            role.SetVAF_Client_ID(ctx.GetVAF_Client_ID());
            role.SetVAF_Role_ID(VAF_Role_ID);
            role.SetVAF_GroupInfo_ID(VAF_Group_ID);
            role.Save();
        }

        /// <summary>
        /// Assign/UnAssign windows from group to role
        /// </summary>
        /// <param name="VAF_Group_ID"></param>
        /// <param name="VAF_Role_ID"></param>
        /// <param name="grantAccess"></param>
        private void ProvideWindowAccessToRole(int VAF_Group_ID, int VAF_Role_ID, bool grantAccess)
        {
            string sql = "SELECT VAF_Screen_ID from VAF_Group_Window WHERE IsActive='Y' AND VAF_GroupInfo_ID=" + VAF_Group_ID;
            DataSet ds = DB.ExecuteDataset(sql);
            List<int> groupWindowIDs = new List<int>();     // will contains all windows of group
            Dictionary<int, bool> roleWindowIDsDictinary = new Dictionary<int, bool>();     // this will contain all windows access for current role...
            StringBuilder winIDs = new StringBuilder();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i]["VAF_Screen_ID"] != null && ds.Tables[0].Rows[i]["VAF_Screen_ID"] != DBNull.Value)
                    {
                        if (winIDs.Length > 0)
                        {
                            winIDs.Append(",");
                        }
                        winIDs.Append(ds.Tables[0].Rows[i]["VAF_Screen_ID"].ToString());
                        groupWindowIDs.Add(Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Screen_ID"]));
                    }
                }

                groupWindowIDs.Sort();

                // get windowsID from WIndow Access for current role and windows in group 
                sql = "SELECT VAF_Screen_ID,IsReadWrite FROM VAF_Screen_Rights WHERE VAF_Role_ID=" + VAF_Role_ID + " AND VAF_Screen_ID IN(" + winIDs.ToString() + ")";
                ds = DB.ExecuteDataset(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        roleWindowIDsDictinary[Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Screen_ID"])] = ds.Tables[0].Rows[i]["IsReadWrite"].ToString() == "Y" ? true : false;
                    }
                }

                for (int i = 0; i < groupWindowIDs.Count(); i++)
                {

                    if (roleWindowIDsDictinary.ContainsKey(groupWindowIDs[i]))      // if Role window access already have current window then set window active/ inactive in window access.
                    {
                        if (roleWindowIDsDictinary[groupWindowIDs[i]] != grantAccess)
                        {
                            //wAccess.SetIsReadWrite(grantAccess);
                            //bool savenew = wAccess.Save();
                            if (grantAccess)
                            {
                                sql = "UPDATE VAF_Screen_Rights Set IsReadWrite='Y',IsActive='Y' WHERE VAF_Screen_ID=" + groupWindowIDs[i] + " AND VAF_Role_ID=" + VAF_Role_ID;
                            }
                            else
                            {
                                sql = "UPDATE VAF_Screen_Rights Set IsReadWrite='N',IsActive='N' WHERE VAF_Screen_ID=" + groupWindowIDs[i] + " AND VAF_Role_ID=" + VAF_Role_ID;
                            }
                            DB.ExecuteQuery(sql, null, null);


                        }
                    }
                    else                // Else create new entry....
                    {
                        MVAFScreen wind = new MVAFScreen(ctx, groupWindowIDs[i], null);
                        MVAFScreenRights wAccess = new MVAFScreenRights(wind, VAF_Role_ID);
                        wAccess.SetVAF_Client_ID(ctx.GetVAF_Client_ID());
                        wAccess.SetVAF_Org_ID(ctx.GetVAF_Org_ID());
                        wAccess.SetVAF_Role_ID(VAF_Role_ID);
                        wAccess.SetVAF_Screen_ID(groupWindowIDs[i]);
                        wAccess.SetIsReadWrite(grantAccess);
                        bool savenew = wAccess.Save();
                    }
                }
            }
        }

        /// <summary>
        /// Assign/UnAssign Forms from group to role
        /// </summary>
        /// <param name="VAF_Group_ID"></param>
        /// <param name="VAF_Role_ID"></param>
        /// <param name="grantAccess"></param>
        private void ProvideFormAccessToRole(int VAF_Group_ID, int VAF_Role_ID, bool grantAccess)
        {
            string sql = "SELECT VAF_Page_ID from VAF_Group_Form WHERE IsActive='Y' AND VAF_GroupInfo_ID=" + VAF_Group_ID;
            DataSet ds = DB.ExecuteDataset(sql);
            List<int> groupWindowIDs = new List<int>();
            Dictionary<int, bool> roleWindowIDsDictinary = new Dictionary<int, bool>();
            StringBuilder winIDs = new StringBuilder();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i]["VAF_Page_ID"] != null && ds.Tables[0].Rows[i]["VAF_Page_ID"] != DBNull.Value)
                    {
                        if (winIDs.Length > 0)
                        {
                            winIDs.Append(",");
                        }
                        winIDs.Append(ds.Tables[0].Rows[i]["VAF_Page_ID"].ToString());
                        groupWindowIDs.Add(Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Page_ID"]));
                    }
                }

                groupWindowIDs.Sort();

                sql = "SELECT VAF_Page_ID,IsReadWrite FROM VAF_Page_Rights WHERE VAF_Role_ID=" + VAF_Role_ID + " AND VAF_Page_ID IN(" + winIDs.ToString() + ")";
                ds = DB.ExecuteDataset(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        roleWindowIDsDictinary[Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Page_ID"])] = ds.Tables[0].Rows[i]["IsReadWrite"].ToString() == "Y" ? true : false;
                    }
                }

                for (int i = 0; i < groupWindowIDs.Count(); i++)
                {
                    MVAFPage wind = new MVAFPage(ctx, groupWindowIDs[i], null);
                    MVAFPageRights wAccess = new MVAFPageRights(wind, VAF_Role_ID);
                    if (roleWindowIDsDictinary.ContainsKey(groupWindowIDs[i]))
                    {
                        if (roleWindowIDsDictinary[groupWindowIDs[i]] != grantAccess)
                        {
                            //wAccess.SetIsReadWrite(grantAccess);
                            //wAccess.Save();
                            if (grantAccess)
                            {
                                sql = "UPDATE VAF_Page_Rights Set IsReadWrite='Y',IsActive='Y' WHERE VAF_Page_ID=" + groupWindowIDs[i] + " AND VAF_Role_ID=" + VAF_Role_ID;
                            }
                            else
                            {
                                sql = "UPDATE VAF_Page_Rights Set IsReadWrite='N',IsActive='N' WHERE VAF_Page_ID=" + groupWindowIDs[i] + " AND VAF_Role_ID=" + VAF_Role_ID;
                            }
                            DB.ExecuteQuery(sql, null, null);
                        }
                    }
                    else
                    {

                        wAccess.SetVAF_Client_ID(ctx.GetVAF_Client_ID());
                        wAccess.SetVAF_Org_ID(ctx.GetVAF_Org_ID());
                        wAccess.SetVAF_Role_ID(VAF_Role_ID);
                        wAccess.SetVAF_Page_ID(groupWindowIDs[i]);
                        wAccess.SetIsReadWrite(grantAccess);
                        wAccess.Save();
                    }
                }
            }
        }

        /// <summary>
        /// Assign/UnAssign process from group to role
        /// </summary>
        /// <param name="VAF_Group_ID"></param>
        /// <param name="VAF_Role_ID"></param>
        /// <param name="grantAccess"></param>
        private void ProvideProcessAccessToRole(int VAF_Group_ID, int VAF_Role_ID, bool grantAccess)
        {
            string sql = "SELECT VAF_Job_ID from VAF_Group_Process WHERE IsActive='Y' AND VAF_GroupInfo_ID=" + VAF_Group_ID;
            DataSet ds = DB.ExecuteDataset(sql);
            List<int> groupWindowIDs = new List<int>();
            Dictionary<int, bool> roleWindowIDsDictinary = new Dictionary<int, bool>();
            StringBuilder winIDs = new StringBuilder();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i]["VAF_Job_ID"] != null && ds.Tables[0].Rows[i]["VAF_Job_ID"] != DBNull.Value)
                    {
                        if (winIDs.Length > 0)
                        {
                            winIDs.Append(",");
                        }
                        winIDs.Append(ds.Tables[0].Rows[i]["VAF_Job_ID"].ToString());
                        groupWindowIDs.Add(Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Job_ID"]));
                    }
                }

                groupWindowIDs.Sort();

                sql = "SELECT VAF_Job_ID,IsReadWrite FROM VAF_Job_Rights WHERE VAF_Role_ID=" + VAF_Role_ID + " AND VAF_Job_ID IN(" + winIDs.ToString() + ")";
                ds = DB.ExecuteDataset(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        roleWindowIDsDictinary[Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Job_ID"])] = ds.Tables[0].Rows[i]["IsReadWrite"].ToString() == "Y" ? true : false;
                    }
                }

                for (int i = 0; i < groupWindowIDs.Count(); i++)
                {
                    MVAFJob wind = new MVAFJob(ctx, groupWindowIDs[i], null);
                    MVAFJobRights wAccess = new MVAFJobRights(wind, VAF_Role_ID);
                    if (roleWindowIDsDictinary.ContainsKey(groupWindowIDs[i]))
                    {
                        if (roleWindowIDsDictinary[groupWindowIDs[i]] != grantAccess)
                        {
                            //wAccess.SetIsReadWrite(grantAccess);
                            //wAccess.Save();
                            if (grantAccess)
                            {
                                sql = "UPDATE VAF_Job_Rights Set IsReadWrite='Y',IsActive='Y' WHERE VAF_Job_ID=" + groupWindowIDs[i] + " AND VAF_Role_ID=" + VAF_Role_ID;
                            }
                            else
                            {
                                sql = "UPDATE VAF_Job_Rights Set IsReadWrite='N',IsActive='N' WHERE VAF_Job_ID=" + groupWindowIDs[i] + " AND VAF_Role_ID=" + VAF_Role_ID;
                            }
                            DB.ExecuteQuery(sql, null, null);
                        }
                    }
                    else
                    {

                        wAccess.SetVAF_Client_ID(ctx.GetVAF_Client_ID());
                        wAccess.SetVAF_Org_ID(ctx.GetVAF_Org_ID());
                        wAccess.SetVAF_Role_ID(VAF_Role_ID);
                        wAccess.SetVAF_Job_ID(groupWindowIDs[i]);
                        wAccess.SetIsReadWrite(grantAccess);
                        wAccess.Save();
                    }
                }
            }
        }

        /// <summary>
        /// Assign/UnAssign workflow from group to role
        /// </summary>
        /// <param name="VAF_Group_ID"></param>
        /// <param name="VAF_Role_ID"></param>
        /// <param name="grantAccess"></param>
        private void ProvideWorkflowAccessToRole(int VAF_Group_ID, int VAF_Role_ID, bool grantAccess)
        {
            string sql = "SELECT VAF_Workflow_ID from VAF_Group_Workflow WHERE IsActive='Y' AND VAF_GroupInfo_ID=" + VAF_Group_ID;
            DataSet ds = DB.ExecuteDataset(sql);
            List<int> groupWindowIDs = new List<int>();
            Dictionary<int, bool> roleWindowIDsDictinary = new Dictionary<int, bool>();
            StringBuilder winIDs = new StringBuilder();
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i]["VAF_Workflow_ID"] != null && ds.Tables[0].Rows[i]["VAF_Workflow_ID"] != DBNull.Value)
                    {
                        if (winIDs.Length > 0)
                        {
                            winIDs.Append(",");
                        }
                        winIDs.Append(ds.Tables[0].Rows[i]["VAF_Workflow_ID"].ToString());
                        groupWindowIDs.Add(Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Workflow_ID"]));
                    }
                }

                groupWindowIDs.Sort();

                sql = "SELECT VAF_Workflow_ID,IsReadWrite FROM VAF_WFlow_Rights WHERE VAF_Role_ID=" + VAF_Role_ID + " AND VAF_Workflow_ID IN(" + winIDs.ToString() + ")";
                ds = DB.ExecuteDataset(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        roleWindowIDsDictinary[Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Workflow_ID"])] = ds.Tables[0].Rows[i]["IsReadWrite"].ToString() == "Y" ? true : false;
                    }
                }

                for (int i = 0; i < groupWindowIDs.Count(); i++)
                {
                    MVAFWorkflow wind = new MVAFWorkflow(ctx, groupWindowIDs[i], null);
                    MWorkflowAccess wAccess = new MWorkflowAccess(wind, VAF_Role_ID);
                    if (roleWindowIDsDictinary.ContainsKey(groupWindowIDs[i]))
                    {
                        if (roleWindowIDsDictinary[groupWindowIDs[i]] != grantAccess)
                        {
                            //wAccess.SetIsReadWrite(grantAccess);
                            //wAccess.Save();
                            if (grantAccess)
                            {
                                sql = "UPDATE VAF_WFlow_Rights Set IsReadWrite='Y',IsActive='Y' WHERE VAF_Workflow_ID=" + groupWindowIDs[i] + " AND VAF_Role_ID=" + VAF_Role_ID;
                            }
                            else
                            {
                                sql = "UPDATE VAF_WFlow_Rights Set IsReadWrite='N',IsActive='N' WHERE VAF_Workflow_ID=" + groupWindowIDs[i] + " AND VAF_Role_ID=" + VAF_Role_ID;
                            }
                            DB.ExecuteQuery(sql, null, null);
                        }
                    }
                    else
                    {

                        wAccess.SetVAF_Client_ID(ctx.GetVAF_Client_ID());
                        wAccess.SetVAF_Org_ID(ctx.GetVAF_Org_ID());
                        wAccess.SetVAF_Role_ID(VAF_Role_ID);
                        wAccess.SetVAF_Workflow_ID(groupWindowIDs[i]);
                        wAccess.SetIsReadWrite(grantAccess);
                        wAccess.Save();
                    }
                }
            }
        }

        /// <summary>
        /// Gets Name and ID of windows('User','Role', 'Group Rights')
        /// </summary>
        /// <returns></returns>
        public WindowID GetWindowIds()
        {
            WindowID wID = new WindowID();

            DataSet ds = DB.ExecuteDataset("SELECT name,VAF_Screen_ID from VAF_Screen where Name  In ( 'User','Role', 'Group Rights')");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if ("User".Equals(ds.Tables[0].Rows[i]["Name"].ToString()))
                    {
                        wID.UserWindowID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Screen_ID"]);
                    }
                    else if ("Role".Equals(ds.Tables[0].Rows[i]["Name"].ToString()))
                    {
                        wID.RoleWindowID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Screen_ID"]);
                    }
                    if ("Group Rights".Equals(ds.Tables[0].Rows[i]["Name"].ToString()))
                    {
                        wID.GroupWindowID = Convert.ToInt32(ds.Tables[0].Rows[i]["VAF_Screen_ID"]);
                    }
                }
            }
            return wID;
        }

        /// <summary>
        /// Get Information of group like windows, forms, processes, workflows it contained.
        /// </summary>
        /// <param name="groupID"></param>
        /// <returns></returns>
        public GroupChildInfo GetGroupInfo(int groupID)
        {
            GroupChildInfo gInfo = new GroupChildInfo();

            string sql = "SELECT Name, Description from VAF_GroupInfo WHERE VAF_GroupInfo_ID=" + groupID;
            sql = MVAFRole.GetDefault(ctx).AddAccessSQL(sql, "VAF_GroupInfo", true, false);

            DataSet ds = DB.ExecuteDataset(sql);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                return gInfo;
            }
            if (ds.Tables[0].Rows[0]["Name"] != null)
            {
                gInfo.GroupName = ds.Tables[0].Rows[0]["Name"].ToString();
            }
            if (ds.Tables[0].Rows[0]["Description"] != null)
            {
                gInfo.Description = ds.Tables[0].Rows[0]["Description"].ToString();
            }


            sql = @"SELECT AD_WIndow.Name FROM VAF_Group_Window  JOIN AD_WIndow
                         ON VAF_Group_Window.VAF_Screen_ID=VAF_Screen.VAF_Screen_ID
                         WHERE VAF_Group_Window.IsActive='Y' AND VAF_Group_Window.VAF_GroupInfo_ID=" + groupID + " ORDER BY AD_WIndow.Name";

            ds = DB.ExecuteDataset(sql);

            StringBuilder windows = new StringBuilder();

            if (ds == null || ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (windows.Length > 0)
                    {
                        windows.Append(",  ");
                    }
                    windows.Append(ds.Tables[0].Rows[i]["Name"].ToString());
                }
            }
            gInfo.WindowName = windows.ToString();

            sql = @"SELECT VAF_Page.Name FROM VAF_Group_Form  JOIN VAF_Page
                     ON VAF_Group_Form.VAF_Page_ID=VAF_Page.VAF_Page_ID
                     WHERE VAF_Group_Form.IsActive='Y' AND  VAF_Group_Form.VAF_GroupInfo_ID=" + groupID + " ORDER BY VAF_Page.Name";



            ds = DB.ExecuteDataset(sql);

            StringBuilder forms = new StringBuilder();

            if (ds == null || ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (forms.Length > 0)
                    {
                        forms.Append(",  ");
                    }
                    forms.Append(ds.Tables[0].Rows[i]["Name"].ToString());
                }
            }

            gInfo.FormName = forms.ToString();

            sql = @"SELECT VAF_Job.Name FROM VAF_Group_Process  JOIN VAF_Job
                     ON VAF_Group_Process.VAF_Job_ID=VAF_Job.VAF_Job_ID
                     WHERE VAF_Group_Process.IsActive='Y' AND VAF_Group_Process.VAF_GroupInfo_ID=" + groupID + " ORDER BY VAF_Job.Name";

            ds = DB.ExecuteDataset(sql);

            StringBuilder processes = new StringBuilder();

            if (ds == null || ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (processes.Length > 0)
                    {
                        processes.Append(",  ");
                    }
                    processes.Append(ds.Tables[0].Rows[i]["Name"].ToString());
                }
            }
            gInfo.ProcessName = processes.ToString();


            sql = @"SELECT VAF_Workflow.Name FROM VAF_Group_workflow  JOIN VAF_Workflow
                     ON VAF_Group_workflow.VAF_Workflow_ID=VAF_Workflow.VAF_Workflow_ID
                     WHERE VAF_Group_workflow.IsActive='Y' AND VAF_Group_workflow.VAF_GroupInfo_ID=" + groupID + " ORDER BY VAF_Workflow.Name";

            ds = DB.ExecuteDataset(sql);

            StringBuilder workflows = new StringBuilder();

            if (ds == null || ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (workflows.Length > 0)
                    {
                        workflows.Append(",  ");
                    }
                    workflows.Append(ds.Tables[0].Rows[i]["Name"].ToString());
                }
            }

            gInfo.WorkflowName = workflows.ToString();

            return gInfo;

        }

        /// <summary>
        /// Create New Role
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="userLevel"></param>
        /// <param name="OrgID"></param>
        /// <returns></returns>
        public String AddNewRole(string Name, string userLevel, List<int> OrgID)
        {
            
            string info ="";
            string msg;
            int VAF_Role_Table_ID = Convert.ToInt32(DB.ExecuteScalar("SELECT VAF_TableView_ID FROM VAF_TableView WHERE TableName='VAF_Role'", null, null));

            try
            {
                string sql = @"SELECT VAF_Column_ID,ColumnName,
                                  defaultvalue
                                FROM VAF_Column
                                WHERE VAF_TableView_ID =" + VAF_Role_Table_ID + @"
                                AND isActive      ='Y'
                                AND defaultvalue IS NOT NULL";

                DataSet ds = DB.ExecuteDataset(sql);                    // Get Default Values
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return VAdvantage.Utility.Msg.GetMsg(ctx, "DefaultValueNotFound");
                }

                MVAFRole role = new MVAFRole(ctx, 0, null);

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)           // Setting Default Values 
                {
                    string value = ds.Tables[0].Rows[i]["DefaultValue"].ToString();

                    if (value.StartsWith("@"))
                    {
                        value = value.Substring(0, value.Length - 1);
                        string columnName = value.Substring(value.IndexOf("@") + 1);
                        value = ctx.GetContext(columnName);	// get global context
                    }
                    role.Set_Value(ds.Tables[0].Rows[i]["ColumnName"].ToString(), value);
                }
                role.SetIsManual(true);
                role.SetName(Name);
                role.SetUserLevel(userLevel);
                if (role.Save())
                {
                    if (OrgID != null)
                    {
                        for (int i = 0; i < OrgID.Count; i++)           // Assigning org access to role
                        {
                            MVAFOrg org = new MVAFOrg(ctx, OrgID[i], null);
                            MVAFRoleOrgRights roles = new MVAFRoleOrgRights(org, role.GetVAF_Role_ID());
                            roles.SetVAF_Client_ID(ctx.GetVAF_Client_ID());
                            roles.SetVAF_Org_ID(OrgID[i]);
                            roles.SetIsReadOnly(false);
                            roles.Save();
                        }
                        
                    }
                    
                }
                else
                {
                    ValueNamePair ppE = VAdvantage.Logging.VLogger.RetrieveError();

                    if (ppE != null)
                    {
                        msg = ppE.GetValue();
                        info = ppE.GetName();
                    }
                }



                return info;
            }
            catch(Exception ex)
            {

                return ex.Message;
            }


            //rr.Set_Value(


           
                
        }

        /// <summary>
        /// Get User levels to assign to role.
        /// </summary>
        /// <returns></returns>
        public List<KeyVal> GetUserLevel()
        {
            List<KeyVal> keyval = new List<KeyVal>();

            string sql = @"SELECT VAF_CtrlRef_List.value,
                                      VAF_CtrlRef_List.Name
                                    FROM VAF_CtrlRef_List
                                    JOIN VAF_Control_Ref
                                    ON VAF_Control_Ref.VAF_Control_Ref_id = VAF_CtrlRef_List.VAF_Control_Ref_id
                                    WHERE VAF_Control_Ref.Name         ='VAF_Role User Level'
                                    ORDER BY VAF_CtrlRef_List_ID";
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ctx.GetVAF_Client_ID() > 0 && ds.Tables[0].Rows[i]["value"].ToString() == "S  ")
                    {
                        continue;
                    }
                    KeyVal ke = new KeyVal()
                    {
                        ID = ds.Tables[0].Rows[i]["value"].ToString(),
                        Name = ds.Tables[0].Rows[i]["Name"].ToString()
                    };
                    keyval.Add(ke);
                }
            }
            return keyval;
        }


    }

    public class UserData
    {
        public List<UserInfo> UserInfo { get; set; }

    }

    public class UserInfo
    {
        public string Username { get; set; }
        public int VAF_UserContactID { get; set; }
        public int VAF_OrgID { get; set; }
        public int VAF_ClientID { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public bool IsActive { get; set; }
        public bool IsUpdate { get; set; }
        public string UserImage { get; set; }
        public int UserTableID { get; set; }
        public int UserWindowID { get; set; }
        public bool HasAccess { get; set; }
    }

    public class RolesInfo
    {
        public int VAF_Role_ID { get; set; }
        public string Name { get; set; }
        public bool IsAssignedToUser { get; set; }
        public int roleWindowID { get; set; }
        public bool IsUpdate { get; set; }

        /*  These will be used for Gmail Contacts Settings */
        public int UserID { get; set; }
        public string Password { get; set; }
        public string ConnectionProfile { get; set; }
        /*      */
    }

    public class GroupInfo
    {
        public int VAF_Group_ID { get; set; }
        public string Name { get; set; }
        public bool IsAssignedToUser { get; set; }
        public int GroupWindowID { get; set; }
    }

    public class WindowID
    {
        public int UserWindowID { get; set; }
        public int RoleWindowID { get; set; }
        public int GroupWindowID { get; set; }
    }

    public class GroupChildInfo
    {
        public string GroupName { get; set; }
        public string Description { get; set; }
        public string WindowName { get; set; }
        public string FormName { get; set; }
        public string ProcessName { get; set; }
        public string WorkflowName { get; set; }
    }

    public class KeyVal
    {
        public string Name { get; set; }
        public string ID { get; set; }
    }

}