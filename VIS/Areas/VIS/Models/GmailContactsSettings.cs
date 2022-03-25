using Google.Contacts;
using Google.GData.Client;
using Google.GData.Contacts;
using Google.GData.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class GmailContactsSettings
    {
//        string username = string.Empty;
//        string password = string.Empty;

//        private bool isFirstTime = false;
//        private bool isImport = true;
//        ObservableCollection<ContactsInfo> contactInfo = new ObservableCollection<ContactsInfo>();
//        static int PAGE_SIZE = 100;
//        int startPage = 0;
//        bool isImportDone = true;
//        private List<string> UIDList = new List<string>();
//        int role = -1;
//        bool isReloading = false;
//        bool UpdateExistingRecord = false;
//        List<RolesInfo> lstRole = null;
//        DataSet dsCheckExist = null;
//        Result result = null;
//        int total = 0;
//        bool isfullyImported = false;
//       public ObservableCollection<MyContacts> objMyContact = new ObservableCollection<MyContacts>();

//        /// <summary>
//        /// Get Role
//        /// </summary>
//        /// <param name="ctx"></param>
//        /// <returns></returns>
//        public List<RolesInfo> FillRole(Ctx ctx)
//        {

//            string sql = @"  SELECT u.AD_User_ID ,
//                          r.AD_Role_ID       ,
//                          r.Name             ,
//                          u.ConnectionProfile,
//                          u.Password
//                           FROM AD_User u
//                        INNER JOIN AD_User_Roles ur
//                             ON (u.AD_User_ID=ur.AD_User_ID
//                        AND ur.IsActive      ='Y')
//                        INNER JOIN AD_Role r
//                             ON (ur.AD_Role_ID             =r.AD_Role_ID
//                        AND r.IsActive                     ='Y')
//                          WHERE COALESCE(u.LDAPUser,u.Name)='" + ctx.GetAD_User_Name() + @"'
//                        AND u.IsActive                     ='Y'
//                        AND u.IsLoginUser                  ='Y'
//                        AND EXISTS
//                          (SELECT *
//                             FROM AD_Client c
//                            WHERE u.AD_Client_ID=c.AD_Client_ID
//                          AND c.IsActive        ='Y'
//                          )
//                        AND EXISTS
//                          (SELECT *
//                             FROM AD_Client c
//                            WHERE r.AD_Client_ID=c.AD_Client_ID
//                          AND c.IsActive        ='Y'
//                          )
//                            ";
//            lstRole = new List<RolesInfo>();
//            DataSet ds = DB.ExecuteDataset(sql, null);
//            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
//            {
//                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
//                {
//                    lstRole.Add(new RolesInfo()
//                    {
//                        UserID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_User_ID"]),
//                        AD_Role_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Role_ID"]),
//                        Name = Convert.ToString(ds.Tables[0].Rows[i]["NAME"]),
//                        ConnectionProfile = Convert.ToString(ds.Tables[0].Rows[i]["ConnectionProfile"]),
//                        Password = Convert.ToString(ds.Tables[0].Rows[i]["Password"]),
//                    });
//                }
//            }
//            return lstRole;

//        }

//        /// <summary>
//        /// Get Saved Detail from Gmail Configuration
//        /// </summary>
//        /// <param name="ctx"></param>
//        /// <returns></returns>
//        public Dictionary<string, string> GetSavedGmailCredential(Ctx ctx)
//        {
//            Dictionary<string, string> retDic = new Dictionary<string, string>();
//            string sql = "Select WSP_GmailConfiguration_ID from WSP_GmailConfiguration where AD_User_ID=" + ctx.GetAD_User_ID() + " and AD_Client_ID=" + ctx.GetAD_Client_ID();
//            //+ " and AD_Org_ID=" + ctx.GetAD_Org_ID();
//            DataSet dsConfig = DB.ExecuteDataset(sql, null);
//            if (dsConfig != null && dsConfig.Tables[0].Rows.Count > 0)
//            {
//                if (dsConfig.Tables[0].Rows[0]["WSP_GmailConfiguration_ID"] != null && dsConfig.Tables[0].Rows[0]["WSP_GmailConfiguration_ID"].ToString() != "")
//                {
//                    X_WSP_GmailConfiguration config = new X_WSP_GmailConfiguration(ctx, Convert.ToInt32(dsConfig.Tables[0].Rows[0]["WSP_GmailConfiguration_ID"]), null);

//                    string username = config.GetWSP_Username();
//                    string password = config.GetWSP_Password();
//                    int Role = config.GetAD_Role_ID();
//                    bool isUpdate = config.IsWSP_IsUpdateExistingRecord();
//                    retDic["Username"] = username;
//                    retDic["Password"] = password;
//                    retDic["Role"] = Convert.ToString(Role);
//                    retDic["IsUpdate"] = Convert.ToString(isUpdate);

//                }
//            }
//            else
//            {
//                retDic["Username"] = string.Empty;
//                retDic["Password"] = string.Empty;
//                retDic["Role"] = string.Empty;
//                retDic["IsUpdate"] = string.Empty;
//            }
//            return retDic;
//        }






//        /// <summary>
//        /// Get Contacts from gmail and their saved information from database
//        /// </summary>
//        /// <param name="ctx"></param>
//        /// <param name="txtUsername"></param>
//        /// <param name="txtPassword"></param>
//        /// <param name="role_ID"></param>
//        /// <param name="chkUpdateExisting"></param>
//        /// <returns></returns>
//        public string GetContacts(Ctx ctx, string txtUsername, string txtPassword, int role_ID, bool chkUpdateExisting)
//        {
//            username = txtUsername;
//            password = txtPassword;
//            if (role_ID > 0)
//            {
//                role = role_ID;
//            }
//            UpdateExistingRecord = chkUpdateExisting;
//            string sql = "Select WSP_GmailConfiguration_ID from WSP_GmailConfiguration where  isActive='Y' and AD_User_ID=" + ctx.GetAD_User_ID() +
//                    " and AD_Client_ID=" + ctx.GetAD_Client_ID();
//            DataSet ds = DB.ExecuteDataset(sql, null);
//            X_WSP_GmailConfiguration Configs = null;
//            if (ds == null || ds.Tables[0].Rows.Count == 0)
//            {
//                Configs = new X_WSP_GmailConfiguration(ctx, 0, null);
//            }
//            else
//            {
//                int config_ID = Convert.ToInt32(ds.Tables[0].Rows[0]["WSP_GmailConfiguration_ID"]);
//                Configs = new X_WSP_GmailConfiguration(ctx, config_ID, null);
//            }
//            if (username != null)
//            {
//                Configs.SetWSP_Username(username);
//            }
//            if (password != null)
//            {
//                Configs.SetWSP_Password(password);
//            }
//            if (role > 0)
//            {
//                Configs.SetAD_Role_ID(role);
//            }
//            else
//            {
//                Configs.SetAD_Role_ID(ctx.GetAD_Role_ID());
//            }
//            if (UpdateExistingRecord != null)
//            {
//                Configs.SetWSP_IsUpdateExistingRecord(UpdateExistingRecord);
//            }


//            Configs.SetAD_User_ID(ctx.GetAD_User_ID());
//            if (!Configs.Save())
//            {

//            }

//            if (username != null && username != "" && password != null && password != "")
//            {
//                StartImportProcess(ctx);
//                for (int i = 0; i < objMyContact.Count; i++)
//                {
//                    LoadContactList(ctx, objMyContact, i);
//                }
//                if (objMyContact.Count == 0 || objMyContact == null)
//                {
//                    return result.Message;
//                }
//            }
//            return "";
//        }

////        public string SaveGmailContactSettings(Ctx ctx, string txtUsername, string txtPassword, int role_ID, bool chkUpdateExisting)
////        {
////            username = txtUsername;
////            password = txtPassword;
////            if (role_ID > 0)
////            {
////                role = role_ID;
////            }
////            UpdateExistingRecord = chkUpdateExisting;
////            string sql = "Select WSP_GmailConfiguration_ID from WSP_GmailConfiguration where  isActive='Y' and AD_User_ID=" + ctx.GetAD_User_ID() +
////                    " and AD_Client_ID=" + ctx.GetAD_Client_ID();
////            //string sql = "Select WSP_GmailConfiguration_ID from WSP_GmailConfiguration where  isActive='Y' and AD_User_ID=" + ctx.GetAD_User_ID() +
////            //       " and AD_Client_ID=" + ctx.GetAD_Client_ID() + " and AD_Org_ID=" + ctx.GetAD_Org_ID(); added by  BY Sarab
////            DataSet ds = DB.ExecuteDataset(sql, null);
////            X_WSP_GmailConfiguration Configs = null;
////            if (ds == null || ds.Tables[0].Rows.Count == 0)
////            {
////                Configs = new X_WSP_GmailConfiguration(ctx, 0, null);
////            }
////            else
////            {
////                int config_ID = Convert.ToInt32(ds.Tables[0].Rows[0]["WSP_GmailConfiguration_ID"]);
////                Configs = new X_WSP_GmailConfiguration(ctx, config_ID, null);
////            }
////            if (username != null)
////            {
////                Configs.SetWSP_Username(username);
////            }
////            if (password != null)
////            {
////                Configs.SetWSP_Password(password);
////            }
////            if (role > 0)
////            {
////                Configs.SetAD_Role_ID(role);
////            }
////            else
////            {
////                Configs.SetAD_Role_ID(ctx.GetAD_Role_ID());
////            }
////            if (UpdateExistingRecord != null)
////            {
////                Configs.SetWSP_IsUpdateExistingRecord(UpdateExistingRecord);
////            }


////            Configs.SetAD_User_ID(ctx.GetAD_User_ID());
////            if (!Configs.Save())
////            {
////                return "false";
////            }

////            return "true";
////            //this.DialogResult = true;

////        }



////        /// <summary>
////        /// Load Contact List and fill the detail whether the contact is linked or not
////        /// </summary>
////        /// <param name="ctx"></param>
////        /// <param name="items"></param>
////        /// <param name="index"></param>
////        private void LoadContactList(Ctx ctx, ObservableCollection<MyContacts> items, int index)
////        {
////            if (items != null)
////            {
////                if (items.Count > 0)
////                {
////                    if (isReloading)        //it is true when uncheck showExisting Records..Used to avoid checks of Already imported contacts
////                    {
////                        //if (e.Row.Presenter != null)
////                        //{
////                        if (UIDList.Contains(((MyContacts)items[index]).ID.ToString()))
////                        {
////                            ((MyContacts)items[index]).IsSelected = false;
////                            items[index].IsLinked = true;
////                            //   e.Row.Presenter.Background = new SolidColorBrush(Colors.LightGray);
////                            //if (!UpdateExistingRecord)
////                            //{
////                            //    e.Row.Presenter.IsHitTestVisible = false;
////                            //    e.Row.Presenter.IsEnabled = false;
////                            //}
////                            return;
////                        }

////                        //e.Row.Presenter.IsHitTestVisible = true;
////                        isExistingRecord(ctx, index, items, dsCheckExist);
////                        return;
////                        //  }
////                    }

////                    if (index >= items.Count)
////                    {
////                        return;
////                    }

////                    if (((MyContacts)items[index]).ID != null)
////                    {
////                        if (UIDList.Contains(((MyContacts)items[index]).ID.ToString()))
////                        {
////                            items[index].IsLinked = true;
////                            //e.Row.Presenter.Background = new SolidColorBrush(Colors.LightGray);
////                            //if (!UpdateExistingRecord)
////                            //{
////                            //    e.Row.Presenter.IsHitTestVisible = false;
////                            //    e.Row.Presenter.IsEnabled = false;
////                            //}
////                        }
////                        else if (((MyContacts)items[index]).AllEmails.Count > 0)
////                        {
////                            isExistingRecord(ctx, index, items, dsCheckExist);
////                        }
////                        else if (((MyContacts)items[index]).Mobile != null && ((MyContacts)items[index]).Mobile != "")
////                        {
////                            isExistingRecord(ctx, index, items, dsCheckExist);
////                        }
////                    }
////                }
////            }
////        }

////        /// <summary>
////        /// get existing records
////        /// </summary>
////        /// <param name="ctx"></param>
////        /// <param name="index"></param>
////        /// <param name="items"></param>
////        /// <param name="dsCheckExist"></param>
////        public void isExistingRecord(Ctx ctx, int index, ObservableCollection<MyContacts> items, DataSet dsCheckExist)
////        {
////            try
////            {
////                StringBuilder dbMobile = new StringBuilder();
////                StringBuilder gMobile = new StringBuilder();
////                bool ischecked = false;
////                if (index >= items.Count)
////                {
////                    return;
////                }
////                if (((MyContacts)items[index]).AllEmails.Count > 0)
////                {
////                    for (int i = 0; i < ((MyContacts)items[index]).AllEmails.Count; i++)
////                    {
////                        for (int j = 0; j < dsCheckExist.Tables[0].Rows.Count; j++)
////                        {
////                            if (dsCheckExist.Tables[0].Rows[j]["MOBILE"] != null && dsCheckExist.Tables[0].Rows[j]["MOBILE"].ToString() != "")
////                            {
////                                dbMobile.Clear();
////                                //dbMobile.Append(dsCheckExist.Tables[0].Rows[j]["MOBILE"].ToString());
////                                if (dsCheckExist.Tables[0].Rows[j]["MOBILE"].ToString().Length > 10)
////                                {
////                                    dbMobile.Append(dsCheckExist.Tables[0].Rows[j]["MOBILE"].ToString().Substring(dsCheckExist.Tables[0].Rows[j]["MOBILE"].ToString().Length - 10));
////                                }
////                                else
////                                {
////                                    dbMobile.Append(dsCheckExist.Tables[0].Rows[j]["MOBILE"].ToString());
////                                }
////                            }

////                            if (((MyContacts)items[index]).Mobile != null)
////                            {
////                                if (((MyContacts)items[index]).Mobile.Length > 10)
////                                {
////                                    gMobile.Append(((MyContacts)items[index]).Mobile.Substring(((MyContacts)items[index]).Mobile.Length - 10));
////                                }
////                                else
////                                {
////                                    gMobile.Append(((MyContacts)items[index]).Mobile);
////                                }
////                            }

////                            if (((MyContacts)items[index]).ContactName != null)
////                            {
////                                if (dsCheckExist.Tables[0].Rows[j]["EMAIL"] != null && dsCheckExist.Tables[0].Rows[j]["EMAIL"].ToString() != "")
////                                {
////                                    //if (((MyContacts)items[index]).AllEmails[i].ToUpper() == dsCheckExist.Tables[0].Rows[j]["EMAIL"].ToString().ToUpper() &&
////                                    //    ((MyContacts)items[index]).ContactName.ToUpper() == dsCheckExist.Tables[0].Rows[j]["NAME"].ToString().ToUpper())

////                                    if (((MyContacts)items[index]).AllEmails[i].ToUpper() == dsCheckExist.Tables[0].Rows[j]["EMAIL"].ToString().ToUpper())
////                                    {
////                                        ischecked = true;
////                                        //e.Row.Presenter.Background = new SolidColorBrush(Color.FromArgb(255, 217, 150, 148));
////                                        //if (!UpdateExistingRecord)
////                                        //{
////                                        //   // e.Row.Presenter.IsHitTestVisible = false;
////                                        //   // e.Row.Presenter.IsEnabled = false;
////                                        //}


////                                        items[index].IsColor = true;
////                                        SetBpTypeOfExisting(ctx, ((MyContacts)items[index]), j);
////                                        dbMobile.Clear();
////                                        gMobile.Clear();
////                                        break;
////                                    }
////                                }
////                                if (!ischecked)
////                                {
////                                    if (dbMobile.ToString() != null && dbMobile.ToString() != "")
////                                    {
////                                        if (dbMobile.ToString() == gMobile.ToString() &&
////                                             ((MyContacts)items[index]).ContactName.ToUpper() == dsCheckExist.Tables[0].Rows[j]["NAME"].ToString().ToUpper())
////                                        {
////                                            ischecked = true;
////                                            items[index].IsColor = true;
////                                            //   e.Row.Presenter.Background = new SolidColorBrush(Color.FromArgb(255, 217, 150, 148));
////                                            //if (!UpdateExistingRecord)
////                                            //{
////                                            //    e.Row.Presenter.IsHitTestVisible = false;
////                                            //    e.Row.Presenter.IsEnabled = false;
////                                            //}
////                                            SetBpTypeOfExisting(ctx, ((MyContacts)items[index]), j);
////                                            dbMobile.Clear();
////                                            gMobile.Clear();
////                                            break;
////                                        }
////                                    }
////                                }
////                            }
////                            dbMobile.Clear();
////                            gMobile.Clear();
////                        }
////                        if (ischecked)
////                        {
////                            break;
////                        }
////                    }
////                }
////                else
////                {
////                    for (int j = 0; j < dsCheckExist.Tables[0].Rows.Count; j++)
////                    {
////                        if (dsCheckExist.Tables[0].Rows[j]["MOBILE"] != null && dsCheckExist.Tables[0].Rows[j]["MOBILE"].ToString() != "")
////                        {
////                            dbMobile.Clear();
////                            //dbMobile.Append(dsCheckExist.Tables[0].Rows[j]["MOBILE"].ToString());
////                            if (dsCheckExist.Tables[0].Rows[j]["MOBILE"].ToString().Length > 10)
////                            {
////                                dbMobile.Append(dsCheckExist.Tables[0].Rows[j]["MOBILE"].ToString().Substring(dsCheckExist.Tables[0].Rows[j]["MOBILE"].ToString().Length - 10));
////                            }
////                            else
////                            {
////                                dbMobile.Append(dsCheckExist.Tables[0].Rows[j]["MOBILE"].ToString());
////                            }
////                        }
////                        if (((MyContacts)items[index]).Mobile != null)
////                        {
////                            gMobile.Clear();
////                            if (((MyContacts)items[index]).Mobile.Length > 10)
////                            {
////                                gMobile.Append(((MyContacts)items[index]).Mobile.Substring(((MyContacts)items[index]).Mobile.Length - 10));
////                            }
////                            else
////                            {
////                                gMobile.Append(((MyContacts)items[index]).Mobile);
////                            }
////                        }
////                        if (dbMobile.ToString() != null && dbMobile.ToString() != "")
////                        {
////                            if (dbMobile.ToString() == gMobile.ToString() &&
////                                 ((MyContacts)items[index]).ContactName.ToUpper() == dsCheckExist.Tables[0].Rows[j]["NAME"].ToString().ToUpper())
////                            {
////                                ischecked = true;
////                                items[index].IsColor = true;
////                                //  e.Row.Presenter.Background = new SolidColorBrush(Color.FromArgb(255, 217, 150, 148));
////                                //if (!UpdateExistingRecord)
////                                //{
////                                //    e.Row.Presenter.IsHitTestVisible = false;
////                                //    e.Row.Presenter.IsEnabled = false;
////                                //}
////                                SetBpTypeOfExisting(ctx, ((MyContacts)items[index]), j);
////                                dbMobile.Clear();
////                                gMobile.Clear();
////                            }
////                        }
////                        dbMobile.Clear();
////                        gMobile.Clear();
////                    }
////                }

////            }
////            catch (Exception ex)
////            {

////            }
////        }

//        /// <summary>
//        /// Set Details of Existing Business Partner
//        /// </summary>
//        /// <param name="dsempdtls"></param>
//        /// <param name="j"></param>
//        /// <param name="mc"></param>
//        /// <param name="isImported"></param>
//        private void SetDetailsOfExistingBpartner(DataSet dsempdtls, int j, MyContacts mc, bool isImported)
//        {
//            ObservableCollection<ContactAddress> ExistingAddress = new ObservableCollection<ContactAddress>();
//            ObservableCollection<string> ExistingdisplayAddress = new ObservableCollection<string>();
//            for (int k = j; k <= j; k++)
//            {
//                ContactAddress newWorkAddress = new ContactAddress();
//                string Addres = string.Empty;

//                if (dsempdtls.Tables[0].Rows[j]["ADDRESS1"] != null && dsempdtls.Tables[0].Rows[j]["ADDRESS1"].ToString() != "" && dsempdtls.Tables[0].Rows[j]["ADDRESS1"].ToString() != ",")
//                {
//                    newWorkAddress.WStreetno = dsempdtls.Tables[0].Rows[j]["ADDRESS1"].ToString();
//                    Addres = dsempdtls.Tables[0].Rows[j]["ADDRESS1"].ToString() + " ";
//                }
//                if (dsempdtls.Tables[0].Rows[j]["BPCOUNTRY"] != null && dsempdtls.Tables[0].Rows[j]["BPCOUNTRY"].ToString() != "" && dsempdtls.Tables[0].Rows[j]["BPCOUNTRY"].ToString() != ",")
//                {
//                    newWorkAddress.WCountry = dsempdtls.Tables[0].Rows[j]["BPCOUNTRY"].ToString();
//                    Addres += dsempdtls.Tables[0].Rows[j]["BPCOUNTRY"].ToString() + ", ";
//                }
//                if (dsempdtls.Tables[0].Rows[j]["BPCITY"] != null && dsempdtls.Tables[0].Rows[j]["BPCITY"].ToString() != "" && dsempdtls.Tables[0].Rows[j]["BPCITY"].ToString() != ",")
//                {
//                    newWorkAddress.WCity = dsempdtls.Tables[0].Rows[j]["BPCITY"].ToString();
//                    Addres += dsempdtls.Tables[0].Rows[j]["BPCITY"].ToString() + ", ";

//                }
//                else if (dsempdtls.Tables[0].Rows[j]["CITY"] != null && dsempdtls.Tables[0].Rows[j]["CITY"].ToString() != "" && dsempdtls.Tables[0].Rows[j]["CITY"].ToString() != ",")
//                {
//                    newWorkAddress.WCity = dsempdtls.Tables[0].Rows[j]["CITY"].ToString();
//                    Addres += dsempdtls.Tables[0].Rows[j]["CITY"].ToString() + ", ";
//                }

//                if (dsempdtls.Tables[0].Rows[j]["BPREGION"] != null && dsempdtls.Tables[0].Rows[j]["BPREGION"].ToString() != "" && dsempdtls.Tables[0].Rows[j]["BPREGION"].ToString() != ",")
//                {
//                    newWorkAddress.WRegion = dsempdtls.Tables[0].Rows[j]["BPREGION"].ToString();
//                    Addres += dsempdtls.Tables[0].Rows[j]["BPREGION"].ToString() + ", ";
//                }
//                else if (dsempdtls.Tables[0].Rows[j]["REGIONNAME"] != null && dsempdtls.Tables[0].Rows[j]["REGIONNAME"].ToString() != "" && dsempdtls.Tables[0].Rows[j]["REGIONNAME"].ToString() != ",")
//                {
//                    newWorkAddress.WRegion = dsempdtls.Tables[0].Rows[j]["REGIONNAME"].ToString();
//                    Addres += dsempdtls.Tables[0].Rows[j]["REGIONNAME"].ToString() + ", ";
//                }

//                if (dsempdtls.Tables[0].Rows[j]["BPPOSTAL"] != null && dsempdtls.Tables[0].Rows[k]["BPPOSTAL"].ToString() != "" && dsempdtls.Tables[0].Rows[j]["BPPOSTAL"].ToString() != ",")
//                {
//                    newWorkAddress.WPostcode = dsempdtls.Tables[0].Rows[j]["BPPOSTAL"].ToString();
//                    Addres += dsempdtls.Tables[0].Rows[j]["BPPOSTAL"].ToString() + ", ";
//                }
//                ExistingAddress.Add(newWorkAddress);
//                if (Addres.ToString().EndsWith(", "))
//                {
//                    Addres = Addres.Remove(Addres.Length - 2);
//                }
//                ExistingdisplayAddress.Add(Addres);
//                if (j < dsempdtls.Tables[0].Rows.Count - 1)
//                {
//                    if (dsempdtls.Tables[0].Rows[j]["AD_USER_ID"].ToString() == dsempdtls.Tables[0].Rows[j + 1]["AD_USER_ID"].ToString())//Check if next row contains same user,,If yes then insert that row's address in Current row...
//                    {
//                        j++;
//                    }
//                }
//            }
//            mc.BPAddress = ExistingdisplayAddress;
//            if (ExistingdisplayAddress.Count > 0)
//            {
//                mc.SelectedAddress = 0;
//            }
//            else
//            {
//                mc.SelectedAddress = -1;
//            }
//            mc.CWorkAddress = ExistingAddress;


//        }


//        /// <summary>
//        /// Get Unique ID List
//        /// </summary>
//        /// <param name="ctx"></param>
//        private void GetIDList(Ctx ctx)
//        {
//            string sql = "SELECT AD_Org_ID FROM ad_role_orgaccess WHERE ad_role_id=" + ctx.GetAD_Role_ID() + " and AD_CLIENT_Id=" + ctx.GetAD_Client_ID();
//            DataSet dsorgList = DB.ExecuteDataset(sql, null);
//            if (dsorgList == null || dsorgList.Tables[0].Rows.Count == 0)
//            {
//                return;
//            }

//            StringBuilder strorg = new StringBuilder();
//            for (int i = 0; i < dsorgList.Tables[0].Rows.Count; i++)
//            {
//                strorg.Append(dsorgList.Tables[0].Rows[i]["AD_ORG_ID"].ToString() + ", ");
//            }
//            if (strorg.ToString().EndsWith(", "))
//            {
//                strorg.Remove(strorg.Length - 2, strorg.Length - (strorg.Length - 2));
//            }



//            //sql = "SELECT GMAIL_UID,C_Bpartner_ID FROM AD_user WHERE AD_Client_ID=" + ctx.GetAD_Client_ID()
//            //    + " and  GMAIL_UID is not null and AD_Org_ID in (" + strorg.ToString() + ") and createdby=" + ctx.GetAD_User_ID(); //by sarab

//            sql = "SELECT GMAIL_UID,C_Bpartner_ID FROM AD_user WHERE AD_Client_ID=" + ctx.GetAD_Client_ID()
//                           + " and  GMAIL_UID is not null and AD_Org_ID in (" + strorg.ToString() + ")";
//            DataSet ds = DB.ExecuteDataset(sql, null);
//            if (ds != null && ds.Tables[0].Rows.Count != 0)
//            {
//                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
//                {
//                    UIDList.Add(ds.Tables[0].Rows[i]["GMAIL_UID"].ToString());
//                }
//            }

//            sql = @" SELECT Distinct AD_User.email  ,bp.Name As BPName,bp.C_BP_Group_ID,
//                      AD_User.name         ,
//                      AD_User.mobile       ,AD_User.phone,AD_User.phone2,AD_User.birthday,
//                      AD_User.AD_User_ID   ,
//                      AD_User.c_bpartner_id,
//                      AD_User.GMAIL_UID,
//                      bp.isemployee, bp.isvendor, bp.iscustomer, bp.c_bp_group_id,
//                      cl.City,cl.regionname, cl.address1,cco.name as BPcountry, cci.name as BPCity, cr.name as BPregion,cl.postal as BPPostal,
//                                              cbl.C_Bpartner_Location_ID,CBL.c_location_ID 
//                     FROM ad_user AD_User
//                     Left outer JOIN c_bpartner BP
//                     ON AD_User.c_bpartner_id=bp.c_bpartner_id
//                     Left Outer JOIn c_bpartner_location CBL on bp.c_bpartner_id= cbl.c_bpartner_id 
//                     Left Outer Join c_location CL  on Cl.c_location_id= cbl.c_location_id
//                     Left outer JOIN c_country CCO on cco.c_country_id=cl.c_country_id
//                     Left outer JOIn c_city CCI on CCI.c_city_ID=cl.c_city_ID
//                     Left outer JOIN c_region CR on cr.c_region_id=cl.c_region_id";


//            //sql = MRole.GetDefault().AddAccessSQL(sql, "AD_User", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);

//            sql += " order by AD_User.AD_User_ID desc";
//            dsCheckExist = DB.ExecuteDataset(sql, null);

//        }

////        /// <summary>
////        /// Set Type of Existing Business Partner
////        /// </summary>
////        /// <param name="ctx"></param>
////        /// <param name="mc"></param>
////        /// <param name="j"></param>
////        public void SetBpTypeOfExisting(Ctx ctx, MyContacts mc, int j)
////        {

////            mc.IsContactExist = true;
////            mc.AD_User_ID = Util.GetValueOfInt(dsCheckExist.Tables[0].Rows[j]["AD_USER_ID"].ToString());
////            mc.BPartner_ID = Util.GetValueOfInt(dsCheckExist.Tables[0].Rows[j]["C_BPARTNER_ID"].ToString());
////            mc.UserUEmailID = dsCheckExist.Tables[0].Rows[j]["GMAIL_UID"].ToString();


////            string sql = "SELECT c_bp_group_id, name FROM c_bp_group";
////            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "C_BP_Group", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
////            DataSet dsBPGroup = DB.ExecuteDataset(sql, null);

////            //if (vdgvContacts.SortedColumns.Count() == 0)
////            //{


////            if (dsCheckExist.Tables[0].Rows[j]["ISCUSTOMER"].ToString().Equals("Y"))
////            {
////                mc.IsCustomer = true;
////            }
////            else
////            {
////                mc.IsCustomer = false;
////            }

////            if (dsCheckExist.Tables[0].Rows[j]["ISVENDOR"].ToString().Equals("Y"))
////            {
////                mc.IsVendor = true;
////            }
////            else
////            {
////                mc.IsVendor = false;
////            }

////            if (dsCheckExist.Tables[0].Rows[j]["ISEMPLOYEE"].ToString().Equals("Y"))
////            {
////                mc.IsEmployee = true;
////            }
////            else
////            {
////                mc.IsEmployee = false;
////            }
////            if (dsCheckExist.Tables[0].Rows[j]["C_BP_GROUP_ID"] != null)
////            {
////                for (int k = 0; k < dsBPGroup.Tables[0].Rows.Count; k++)
////                {

////                    if (dsBPGroup.Tables[0].Rows[k]["C_BP_GROUP_ID"].ToString() == dsCheckExist.Tables[0].Rows[j]["C_BP_GROUP_ID"].ToString())
////                    {
////                        mc.SelectedVal = Util.GetValueOfInt(dsCheckExist.Tables[0].Rows[j]["C_BP_GROUP_ID"].ToString());
////                        mc.SelectedItems = k;
////                        break;
////                    }
////                }
////            }

////            if (dsCheckExist.Tables[0].Rows[j]["BPNAME"] != null && dsCheckExist.Tables[0].Rows[j]["BPNAME"].ToString() != "")
////            {
////                mc.BpartnerName = dsCheckExist.Tables[0].Rows[j]["BPNAME"].ToString();
////            }

////            //}
////            SetDetailsOfExistingBpartner(dsCheckExist, j, mc, false);
////        }



 

////        /// <summary>
////        /// Get Business Partner Group
////        /// </summary>
////        /// <param name="ctx"></param>
////        /// <returns></returns>
////        public List<BPInfo> GetBPGroup(Ctx ctx)
////        {
////            List<BPInfo> bpinfo = new List<BPInfo>();
////            string sql = "SELECT c_bp_group_id, name FROM c_bp_group";
////            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "C_BP_Group", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
////            DataSet dsBPGroup = DB.ExecuteDataset(sql, null);
////            if (dsBPGroup != null && dsBPGroup.Tables[0].Rows.Count > 0)
////            {
////                for (int i = 0; i < dsBPGroup.Tables[0].Rows.Count; i++)
////                {

////                    bpinfo.Add(new BPInfo()
////                    {
////                        ID = Convert.ToInt32(dsBPGroup.Tables[0].Rows[i]["c_bp_group_id"]),
////                        Name = Convert.ToString(dsBPGroup.Tables[0].Rows[i]["NAME"])
////                    });

////                    //bpgroup.Add("C_BP_GROUP_ID", Convert.ToString(dsBPGroup.Tables[0].Rows[i]["c_bp_group_id"]));
////                }
////            }
////            return bpinfo;

////        }

////        /// <summary>
////        /// Fill Organization
////        /// </summary>
////        /// <param name="ctx"></param>
////        /// <returns></returns>
////        public List<BPInfo> FillOrg(Ctx ctx)
////        {

////            List<BPInfo> org = new List<BPInfo>();
////            string sql = "SELECT name,AD_Org_ID FROM AD_Org ";
////            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "AD_Org", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
////            DataSet ds = DB.ExecuteDataset(sql, null);

////            if (ds != null && ds.Tables[0].Rows.Count != 0)
////            {
////                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
////                {
////                    org.Add(new BPInfo()
////                    {
////                        ID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Org_ID"]),
////                        Name = Convert.ToString(ds.Tables[0].Rows[i]["NAME"])
////                    });
////                }
////            }
////            return org;

////        }


////        /// <summary>
////        /// Import the Selected Contact in database
////        /// </summary>
////        /// <param name="ctx"></param>
////        /// <param name="orgID"></param>
////        /// <returns></returns>
////        public string Import(Ctx ctx, int orgID)
////        {

////            //  int selectedIndex = vcmbPage.SelectedIndex;
////            if (!isImportDone)
////            {
////                return "";
////            }
////            //  SetBusy(Msg.GetMsg("Saving"), true);
////            isImportDone = false;
////            //  BusyContact.IsBusy = true;

////            string isAunthicated = IsUserAunthicated(ctx);
////            if (isAunthicated == "NWF")
////            {
////                //ShowMessage.Info("NoWindowFoundWithTable", true, null, null);
////                //Dispatcher.BeginInvoke(delegate
////                //{
////                isImportDone = true;
////                //    BusyContact.IsBusy = false;

////                //     SetBusy(Msg.GetMsg("Done"), false);
////                // });
////                return "NoWindowFoundWithTable";
////            }
////            if (isAunthicated == "NA")
////            {
////                //ShowMessage.Info("NoAccess", true, null, null);
////                //Dispatcher.BeginInvoke(delegate
////                //{
////                isImportDone = true;
////                //    BusyContact.IsBusy = false;
////                //    SetBusy(Msg.GetMsg("Done"), false);
////                //});
////                return "NoAccess";
////            }
////            bool verified = false;
////            bool canUpdate = false;

////            //   PagedCollectionView items = vdgvContacts.ItemsSource as PagedCollectionView;

////            string val = IsValidSelection(objMyContact, true, ctx);
////            if (val != "True")
////            {
////                //ShowMessage.Error(val, true);
////                //Dispatcher.BeginInvoke(delegate
////                //{
////                //    BusyContact.IsBusy = false;
////                verified = true;
////                isImportDone = true;
////                //  SetBusy(Msg.GetMsg("Done"), false);
////                return val;
////                // });
////            }
////            else
////            {
////                canUpdate = true;
////                verified = true;
////            }

////            while (!verified)
////            {
////                System.Threading.Thread.Sleep(1);
////            }

////            if (!canUpdate)
////            {
////                //Dispatcher.BeginInvoke(delegate
////                //{
////                //    BusyContact.IsBusy = false;
////                verified = true;
////                isImportDone = true;
////                ///    SetBusy(Msg.GetMsg("Done"), false);
////                return Msg.GetMsg(ctx, "Done");
////            }


////            //if (this.form != null)
////            //{
////            //    this.form.Visibility = Visibility.Collapsed;
////            //}
////            //      selectedIndex = vcmbPage.SelectedIndex;  //temp
////            // PagedCollectionView items = vdgvContacts.ItemsSource as PagedCollectionView;
////            int count = 0;
////            int bpCount = 0;
////            int bpUpdateCount = 0;
////            int userUpdateCount = 0;
////            StringBuilder result = new StringBuilder();
////            if (objMyContact.Count > 0)
////            {
////                // vbtnSave.IsEnabled = false;
////                // BindingSource orgSource = vcmbOrg.ItemsSource as BindingSource;
////                int Org_ID = Util.GetValueOfInt(orgID);
////                StringBuilder sql = new StringBuilder();
////                for (int i = 0; i < objMyContact.Count; i++)
////                {
////                    if (((MyContacts)objMyContact[i]).IsSelected)          //check If record is selected 
////                    {
////                        int BP_ID = 0;
////                        if (((MyContacts)objMyContact[i]).BPartner_ID == 0)
////                        {
////                            BP_ID = isBPExist(Convert.ToString(((MyContacts)objMyContact[i]).BpartnerName), ctx);
////                        }
////                        else
////                        {
////                            BP_ID = ((MyContacts)objMyContact[i]).BPartner_ID;
////                        }

////                        Trx trx = Trx.Get(Trx.CreateTrxName("UserContact"), true);
////                        if (((MyContacts)objMyContact[i]).IsContactExist)
////                        {
////                            sql.Clear();
////                            if (((MyContacts)objMyContact[i]).AD_User_ID != 0 && ((MyContacts)objMyContact[i]).AD_User_ID != null)
////                            {
////                                MUser user = new MUser(ctx, ((MyContacts)objMyContact[i]).AD_User_ID, null);
////                                string re = insertORUpdateUser(ctx, user, i, Org_ID, objMyContact, trx, ref count);
////                                result.Append(re);
////                                if (re != "")
////                                {
////                                    trx.Rollback();
////                                    trx.Close();
////                                    continue;
////                                }
////                                user.SetGmail_UID(((MyContacts)objMyContact[i]).ID);
////                                if (!user.Save())
////                                {
////                                    trx.Rollback();
////                                    trx.Close();
////                                }
////                                else
////                                {
////                                    userUpdateCount++;
////                                    count--;
////                                    if (BP_ID != 0 && BP_ID != null)
////                                    {
////                                        //----------------Added By Sarab------------
////                                        //string res = InsertBP(ctx, objMyContact, i, Org_ID, trx, ref count, ref bpCount, user);

////                                        //bpUpdateCount++;
////                                        //bpCount--;
////                                        //result.Append(res);
////                                        //if (res == "")
////                                        //{
////                                        //    trx.Commit();
////                                        //}
////                                        //else
////                                        //{
////                                        //    trx.Rollback();
////                                        //    trx.Close();
////                                        //    continue;
////                                        //}

////                                        //-------------------
////                                        user.SetC_BPartner_ID(BP_ID);
////                                        if (!user.Save(trx))
////                                        {
////                                            trx.Rollback();
////                                            trx.Close();
////                                            result.Append("User " + user.GetName() + " not updated");
////                                            count--;
////                                            bpCount--;
////                                            continue;
////                                        }
////                                        else
////                                        {
////                                            DateTime updatedTime = user.GetUpdated();
////                                            int ID = user.GetAD_User_ID();
////                                            string sql1 = "Update ad_user set updated=" + GlobalVariable.TO_DATE(updatedTime, false) + ", LastLocalUpdated=" + GlobalVariable.TO_DATE(updatedTime, false) + " where ad_user_ID=" + ID;
////                                            if (DB.ExecuteQuery(sql1.ToString(), null, trx) == -1)
////                                            {
////                                                trx.Rollback();
////                                                trx.Close();
////                                                result.Append("ad_user " + user.GetName() + " not Updated");
////                                                count--;
////                                                bpCount--;
////                                                continue;
////                                            }


////                                            trx.Commit();
////                                            trx.Close();
////                                        }
////                                    }
////                                    else
////                                    {
////                                        string res = InsertBP(ctx, objMyContact, i, Org_ID, trx, ref count, ref bpCount, user);
////                                        result.Append(res);
////                                        if (res == "")
////                                        {
////                                            trx.Commit();
////                                            trx.Close();
////                                        }
////                                        else
////                                        {
////                                            trx.Rollback();
////                                            trx.Close();
////                                        }
////                                    }
////                                    //else
////                                    //{
////                                    //    trx.Commit();
////                                    //    trx.Close();
////                                    //}

////                                }
////                            }

////                            continue;
////                        }


////                        try
////                        {
////                            //------------------ Create Location_ID from home Address for AD_user.C_Location---------------------------//




////                            //--------------Add AD_user---------------------//

////                            sql.Clear();
////                            MUser user = null;
////                            if (BP_ID != null && BP_ID != 0)
////                            {
////                                sql.Append("SELECT AD_User_ID FROM aD_user WHERE c_bpartner_id=" + BP_ID + " and name ='" + ((MyContacts)objMyContact[i]).ContactName + "'");
////                                object userID = DB.ExecuteScalar(sql.ToString());
////                                if (userID != null && userID != DBNull.Value)
////                                {
////                                    user = new MUser(ctx, Util.GetValueOfInt(userID), trx);
////                                }
////                                else
////                                {
////                                    user = new MUser(ctx, 0, trx);
////                                }
////                            }
////                            else
////                            {
////                                user = new MUser(ctx, 0, trx);
////                            }
////                            string re = insertORUpdateUser(ctx, user, i, Org_ID, objMyContact, trx, ref count);
////                            result.Append(re);
////                            if (re != "")
////                            {
////                                trx.Rollback();
////                                trx.Close();
////                                continue;
////                            }


////                            //----------------------------Create C_Bpartner-----------------------------//
////                            if (BP_ID == 0)
////                            {
////                                string res = InsertBP(ctx, objMyContact, i, Org_ID, trx, ref count, ref bpCount, user);
////                                result.Append(res);
////                                if (res == "")
////                                {
////                                    trx.Commit();
////                                }
////                                else
////                                {
////                                    trx.Rollback();
////                                    trx.Close();
////                                    continue;
////                                }
////                            }
////                            else
////                            {

////                                ////----------------Added By Sarab------------
////                                //string res = InsertBP(ctx, objMyContact, i, Org_ID, trx, ref count, ref bpCount, user);

////                                //bpUpdateCount++;
////                                //bpCount--;
////                                //result.Append(res);
////                                //if (res == "")
////                                //{
////                                //    trx.Commit();
////                                //}
////                                //else
////                                //{
////                                //    trx.Rollback();
////                                //    trx.Close();
////                                //    continue;
////                                //}

////                                ////-------------------
////                                user.SetC_BPartner_ID(BP_ID);
////                                if (!user.Save(trx))
////                                {
////                                    trx.Rollback();
////                                    trx.Close();
////                                    result.Append("User " + user.GetName() + " not updated");
////                                    count--;
////                                    bpCount--;
////                                    continue;
////                                }

////                                DateTime updatedTime = user.GetUpdated();
////                                int ID = user.GetAD_User_ID();
////                                string sql1 = "Update ad_user set updated=" + GlobalVariable.TO_DATE(updatedTime, false) + ", LastLocalUpdated=" + GlobalVariable.TO_DATE(updatedTime, false) + " where ad_user_ID=" + ID;
////                                if (DB.ExecuteQuery(sql1.ToString(), null, trx) == -1)
////                                {
////                                    trx.Rollback();
////                                    trx.Close();
////                                    result.Append("ad_user " + user.GetName() + " not Updated");
////                                    count--;
////                                    bpCount--;
////                                    continue;
////                                }
////                            }
////                            UIDList.Add(((MyContacts)objMyContact[i]).ID);
////                            trx.Commit();
////                            trx.Close();
////                        }
////                        catch (Exception ex)
////                        {
////                            //Dispatcher.BeginInvoke(delegate
////                            //{
////                            //    BusyContact.IsBusy = false;
////                            //});
////                            trx.Rollback();
////                            trx.Close();
////                            result.Append(ex.Message);
////                            count--;
////                            bpCount--;
////                        }
////                    }
////                }
////                //Dispatcher.BeginInvoke(delegate
////                //{
////                //    BusyContact.IsBusy = false;
////                //if (this.form != null)
////                //{
////                //    this.form.Close();
////                //}
////                //else
////                //{
////                //if (vcmbPage.SelectedIndex == selectedIndex)
////                //{
////                ReloadData(ctx);
////                //}
////                //else
////                //{
////                //    vbtnSave.IsEnabled = true;
////                //    SetBusy(Msg.GetMsg("Done"), false);
////                //}
////                // selectedIndex = -1;
////                //}
////                //  });
////                isImportDone = true;
////                //ShowMessage.Info(count.ToString() + " user inserted.  \n " + bpCount + "  business partner inserted.  \n " + result.ToString(), true, null, null);
////                //if (isUpdated)
////                //{
////                //    return count.ToString() + " user inserted.  \n " + bpUpdateCount + "  business partner updated.  \n " + bpCount + "  business partner inserted.  \n " + result.ToString();
////                //}
////                return " " + count.ToString() + " user inserted.  \n " + bpCount + "  business partner inserted.  \n " + userUpdateCount.ToString() + " user updated.  \n " + bpUpdateCount + "  business partner updated.  \n " + result.ToString();
////                // });
////            }
////            else
////            {
////                //Dispatcher.BeginInvoke(delegate
////                //{
////                //    BusyContact.IsBusy = false;
////                isImportDone = true;
////                //    SetBusy(Msg.GetMsg("Done"), false);
////                return Msg.GetMsg(ctx, "Done");
////                //});
////            }
////            // });

////            return Msg.GetMsg(ctx, "Done");
////        }

////        /// <summary>
////        /// Check if user has authentication to access the business partner window
////        /// </summary>
////        /// <param name="ctx"></param>
////        /// <returns></returns>
////        private string IsUserAunthicated(Ctx ctx)
////        {
////            string sql = " SELECT AD_Window_ID FROM ad_table WHERE upper(tablename)='C_BPARTNER'";
////            DataSet ds = DB.ExecuteDataset(sql, null);
////            if (ds == null || ds.Tables[0].Rows.Count == 0)
////            {
////                return "NWF";
////            }
////            MRole role = new MRole(ctx, ctx.GetAD_Role_ID(), null);
////            if (ds.Tables[0].Rows[0]["AD_WINDOW_ID"] != null && ds.Tables[0].Rows[0]["AD_WINDOW_ID"].ToString() != "")
////            {
////                if (role.GetWindowAccess(Util.GetValueOfInt(Util.GetValueOfString(ds.Tables[0].Rows[0]["AD_WINDOW_ID"]))) == null)
////                {
////                    return "NA";
////                }
////                if (!(bool)role.GetWindowAccess(Util.GetValueOfInt(Util.GetValueOfString(ds.Tables[0].Rows[0]["AD_WINDOW_ID"]))))
////                {
////                    return "NA";
////                }


////            }
////            else
////            {
////                return "NWF";
////            }
////            return "true";
////        }

////        /// <summary>
////        /// Check if selected REcord is valid or not
////        /// </summary>
////        /// <param name="objMyContact"></param>
////        /// <param name="isImport"></param>
////        /// <param name="ctx"></param>
////        /// <returns></returns>
////        private string IsValidSelection(ObservableCollection<MyContacts> objMyContact, bool isImport, Ctx ctx)
////        {

////            bool isSelected = false;
////            if (objMyContact == null)
////            {
////                return Msg.GetMsg(ctx, "NoRecordFound");
////            }


////            if (objMyContact.Count > 0)
////            {

////                List<MyContacts> localList = new List<MyContacts>();

////                foreach (object item in objMyContact)
////                {
////                    localList.Add((MyContacts)item);
////                }

////                var dat = from iq in localList
////                          group iq by new { iq.ContactName, iq.Mobile, iq.EmailAddress } into it
////                          where it.Count() > 1
////                          select it.Key;

////                List<string> contactName = new List<string>();
////                List<string> mobile = new List<string>();
////                List<string> EmailAddress = new List<string>();
////                List<int> index = new List<int>();
////                for (int i = 0; i < objMyContact.Count; i++)
////                {
////                    if ((objMyContact[i].IsSelected))
////                    {
////                        isSelected = true;
////                        if (((MyContacts)objMyContact[i]).BpartnerName == null)
////                        {
////                            if (isImport)
////                            {
////                                return Msg.GetMsg(ctx, "BPNotNUll") + (i + 1).ToString();
////                            }
////                            else
////                            {
////                                return Msg.GetMsg(ctx, "EmailNotNull") + (i + 1).ToString();
////                            }
////                        }
////                        if (isImport)
////                        {
////                            if (((MyContacts)objMyContact[i]).ContactName == null)
////                            {
////                                return Msg.GetMsg(ctx, "ContactnotNUll") + (i + 1).ToString();
////                            }
////                        }

////                        int BP_ID = 0;
////                        if (((MyContacts)objMyContact[i]).BPartner_ID == 0)
////                        {
////                            BP_ID = isBPExist(Convert.ToString(((MyContacts)objMyContact[i]).BpartnerName), ctx);
////                        }
////                        else
////                        {
////                            BP_ID = ((MyContacts)objMyContact[i]).BPartner_ID;
////                        }
////                        if (BP_ID == 0)
////                        {
////                            if (((MyContacts)objMyContact[i]).IsCustomer == false && ((MyContacts)objMyContact[i]).IsEmployee == false && ((MyContacts)objMyContact[i]).IsVendor == false)
////                            {
////                                return Msg.GetMsg(ctx, "BPTypeNotNull") + (i + 1).ToString();

////                            }
////                        }

////                        foreach (var item in dat)
////                        {
////                            if (item.ContactName == ((MyContacts)objMyContact[i]).ContactName &&
////                                item.Mobile == ((MyContacts)objMyContact[i]).Mobile &&
////                                item.EmailAddress == ((MyContacts)objMyContact[i]).EmailAddress)
////                            {
////                                if (contactName.Count > 0)
////                                {
////                                    if (contactName.Contains(((MyContacts)objMyContact[i]).ContactName) &&
////                                        mobile.Contains(((MyContacts)objMyContact[i]).Mobile) &&
////                                        EmailAddress.Contains(((MyContacts)objMyContact[i]).EmailAddress))
////                                    {
////                                        int ind = EmailAddress.IndexOf(((MyContacts)objMyContact[i]).EmailAddress);
////                                        // return Msg.GetMsg("DuplicateContact" + (index[ind] + 1) + " and " + (i + 1));
////                                    }
////                                }
////                                contactName.Add(item.ContactName);
////                                mobile.Add(item.Mobile);
////                                EmailAddress.Add(item.EmailAddress);
////                                index.Add(i);
////                            }
////                        }

////                    }
////                }
////                if (!isSelected)
////                {
////                    return Msg.GetMsg(ctx, "NoRecordSelected");
////                    //ShowMessage.Error("NoRecordSelected", true);
////                    //return;
////                }
////            }
////            else
////            {
////                return Msg.GetMsg(ctx, "NoRecordFound");
////            }
////            return "True";
////        }

////        /// <summary>
////        /// Check if Business Partner exist or not
////        /// </summary>
////        /// <param name="BPName"></param>
////        /// <param name="ctx"></param>
////        /// <returns></returns>
////        private int isBPExist(string BPName, Ctx ctx)
////        {
////            if (BPName.Contains("'"))
////            {
////                BPName = BPName.Replace("'", "''");
////            }
////            string sql = "SELECT C_Bpartner_ID FROM c_Bpartner WHERE UPPER(name)='" + BPName.ToUpper() + "' and isactive='Y' ";
////            //and AD_Client_ID="+ctx.GetAD_Client_ID()
////            //+" And AD_Org_ID=" + ctx.GetAD_Org_ID();

////            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "C_BPartner", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);


////            DataSet ds = DB.ExecuteDataset(sql, null);
////            if (ds == null || ds.Tables[0].Rows.Count == 0)
////            {
////                return 0;
////            }
////            if (ds.Tables[0].Rows[0]["C_BPARTNER_ID"] != null)
////            {
////                return Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_BPARTNER_ID"].ToString());
////            }
////            return 0;
////            //object ID = DB.ExecuteScalar(sql);
////            //if (ID == null || ID == DBNull.Value)
////            //{
////            //    return 0;
////            //}
////            //return Util.GetValueOfInt(ID);
////        }

////        /// <summary>
////        /// Insert and Update User Detail
////        /// </summary>
////        /// <param name="ctx"></param>
////        /// <param name="user"></param>
////        /// <param name="i"></param>
////        /// <param name="Org_ID"></param>
////        /// <param name="objMyContact"></param>
////        /// <param name="trx"></param>
////        /// <param name="count"></param>
////        /// <returns></returns>
////        public string insertORUpdateUser(Ctx ctx, MUser user, int i, int Org_ID, ObservableCollection<MyContacts> objMyContact, Trx trx, ref int count)
////        {
////            StringBuilder sql = new StringBuilder();
////            StringBuilder result = new StringBuilder();
////            user.SetAD_Org_ID(Org_ID);
////            user.SetAD_Client_ID(ctx.GetAD_Client_ID());
////            user.SetIsFullBPAccess(true);
////            user.SetName(((MyContacts)objMyContact[i]).ContactName.ToString());
////            user.SetNotificationType("E");
////            user.SetEMail(((MyContacts)objMyContact[i]).EmailAddress);
////            user.SetGmail_UID(((MyContacts)objMyContact[i]).ID);

////            if (((MyContacts)objMyContact[i]).Title != null)
////            {
////                user.SetTitle(((MyContacts)objMyContact[i]).Title);
////            }
////            if (((MyContacts)objMyContact[i]).BirthDay != null)
////            {
////                user.SetBirthday(((MyContacts)objMyContact[i]).BirthDay);
////            }

////            if (((MyContacts)objMyContact[i]).Mobile != null)
////            {
////                user.SetMobile(((MyContacts)objMyContact[i]).Mobile);
////            }
////            if (((MyContacts)objMyContact[i]).PhoneNumber != null)
////            {
////                user.SetPhone(((MyContacts)objMyContact[i]).PhoneNumber);
////            }
////            if (((MyContacts)objMyContact[i]).PhoneNumber2 != null)
////            {
////                user.SetPhone2(((MyContacts)objMyContact[i]).PhoneNumber2);
////            }

////            if (!user.Save(trx))
////            {
////                trx.Rollback();
////                trx.Close();
////                result.Append("User " + ((MyContacts)objMyContact[i]).ContactName.ToString() + " not saved");
////                return result.ToString();
////            }
////            //else
////            //{
////            //    trx.Commit();
////            count++;
////            //}

////            sql.Clear();
////            sql.Append("Update AD_user set LastGmailUpdated =" + SetTime(((MyContacts)objMyContact[i]).Updated) + " WHERE    aD_User_ID=" + user.GetAD_User_ID());
////            if (DB.ExecuteQuery(sql.ToString(), null, trx) == -1)
////            {
////                trx.Rollback();
////                trx.Close();
////                result.Append("User " + user.GetName() + " not Updated");
////                count--;
////                return result.ToString();
////            }
////            return result.ToString();
////        }

////        /// <summary>
////        /// Set Time
////        /// </summary>
////        /// <param name="time"></param>
////        /// <returns></returns>
////        public string SetTime(DateTime time)
////        {
////            StringBuilder dateString = new StringBuilder("TO_DATE('");
////            dateString.Append(time.ToString("yyyy-MM-dd HH:mm:ss"));	//	cut off miliseconds
////            dateString.Append("','YYYY-MM-DD HH24:MI:SS')");
////            return dateString.ToString();
////        }


////        /// <summary>
////        /// Insert or Update Business partner Detail
////        /// </summary>
////        /// <param name="ctx"></param>
////        /// <param name="objMyContact"></param>
////        /// <param name="i"></param>
////        /// <param name="Org_ID"></param>
////        /// <param name="trx"></param>
////        /// <param name="count"></param>
////        /// <param name="bpCount"></param>
////        /// <param name="user"></param>
////        /// <returns></returns>
////        public string InsertBP(Ctx ctx, ObservableCollection<MyContacts> objMyContact, int i, int Org_ID, Trx trx, ref int count, ref int bpCount, MUser user)
////        {
////            MBPartner partner = null;
////            string docno = string.Empty;
////            MLocation location = null;
////            StringBuilder result = new StringBuilder();
////            StringBuilder sql = new StringBuilder();
////            if (((MyContacts)objMyContact[i]).BPartner_ID != null && ((MyContacts)objMyContact[i]).BPartner_ID != 0)
////            {
////                partner = new MBPartner(ctx, ((MyContacts)objMyContact[i]).BPartner_ID, trx);
////                docno = ((MyContacts)objMyContact[i]).BPDocNo.ToString();
////            }
////            else
////            {
////                partner = new MBPartner(ctx, 0, trx);
////                docno = MSequence.GetDocumentNo(ctx.GetAD_Client_ID(), "C_BPartner", trx, ctx);
////            }
////            partner.SetAD_Org_ID(Org_ID);
////            partner.SetAD_Client_ID(ctx.GetAD_Client_ID());
////            partner.SetName(((MyContacts)objMyContact[i]).BpartnerName.ToString());
////            partner.SetC_BP_Group_ID(((MyContacts)objMyContact[i]).SelectedVal);
////            partner.SetIsCustomer(((MyContacts)objMyContact[i]).IsCustomer);
////            partner.SetIsEmployee(((MyContacts)objMyContact[i]).IsEmployee);
////            //if (((MyContacts)items[i]).Title != null)
////            //{
////            //    partner.SetDescription(((MyContacts)items[i]).Title);
////            //}
////            partner.SetIsOneTime(false);
////            partner.SetIsProspect(false);
////            partner.SetIsSalesRep(false);
////            partner.SetIsSummary(false);
////            partner.SetIsVendor(((MyContacts)objMyContact[i]).IsVendor);
////            partner.SetSO_CreditLimit(0);
////            partner.SetSO_CreditUsed(0);
////            partner.SetSendEMail(false);
////            partner.SetValue(docno);
////            // partner.SetEmployee_Filter(false); //temp
////            // partner.SetUEmailID(((MyContacts)items[i]).ID);
////            if (!partner.Save(trx))
////            {
////                trx.Rollback();
////                trx.Close();
////                result.Append(((MyContacts)objMyContact[i]).BpartnerName + " not saved into business partner");
////                count--;
////                //     continue;ret
////                return result.ToString();
////            }
////            else
////            {
////                bpCount++;
////            }

////            sql.Clear();
////            sql.Append("Update C_Bpartner set LastGmailUpdated =" + SetTime(((MyContacts)objMyContact[i]).Updated) + " where    C_Bpartner_ID=" + partner.GetC_BPartner_ID());
////            if (DB.ExecuteQuery(sql.ToString(), null, trx) == -1)
////            {
////                trx.Rollback();
////                trx.Close();
////                result.Append("C_Bpartner " + partner.GetName() + " not Updated");
////                count--;
////                bpCount--;
////                // continue;
////                return result.ToString();
////            }

////            DateTime updatedTime = partner.GetUpdated();
////            int ID = partner.GetC_BPartner_ID();
////            string sql1 = "Update C_Bpartner set updated=" + GlobalVariable.TO_DATE(updatedTime, false) + ",LastLocalUpdated=" + GlobalVariable.TO_DATE(updatedTime, false) + " where C_Bpartner_ID=" + ID;
////            if (DB.ExecuteQuery(sql1.ToString(), null, trx) == -1)
////            {
////                trx.Rollback();
////                trx.Close();
////                result.Append("C_Bpartner " + partner.GetName() + " not Updated");
////                count--;
////                bpCount--;
////                // continue;
////                return result.ToString();
////            }


////            //----------------- Update AD_user----------------------------------------\\
////            user.SetC_BPartner_ID(partner.GetC_BPartner_ID());
////            if (!user.Save(trx))
////            {
////                trx.Rollback();
////                trx.Close();
////                result.Append("User " + user.GetName() + " not updated");
////                count--;
////                bpCount--;
////                // continue;
////                return result.ToString();
////            }

////            updatedTime = user.GetUpdated();
////            ID = user.GetAD_User_ID();
////            sql1 = "Update ad_user set updated=" + GlobalVariable.TO_DATE(updatedTime, false) + ", LastLocalUpdated=" + GlobalVariable.TO_DATE(updatedTime, false) + " where ad_user_ID=" + ID;
////            if (DB.ExecuteQuery(sql1.ToString(), null, trx) == -1)
////            {
////                trx.Rollback();
////                trx.Close();
////                result.Append("ad_user " + user.GetName() + " not Updated");
////                count--;
////                bpCount--;
////                // continue;
////                return result.ToString();
////            }

////            //------------------ Create Location_ID from home Address for C_Bpartner.C_Location---------------------------//

////            ObservableCollection<ContactAddress> cAddress = ((MyContacts)objMyContact[i]).CWorkAddress;
////            if (cAddress.Count > 0)
////            {
////                int BPLCount = 0;
////                sql.Clear();
////                //int workAddressCount = 0;
////                sql.Append("SELECT c_location_id,c_bpartner_location_ID FROM c_bpartner_location WHERE c_bpartner_id=" + ((MyContacts)objMyContact[i]).BPartner_ID + " order by c_location_id");
////                DataSet ds = DB.ExecuteDataset(sql.ToString(), null);
////                if (ds == null || ds.Tables[0].Rows.Count == 0)
////                {

////                }
////                else
////                {
////                    BPLCount = ds.Tables[0].Rows.Count;
////                }
////                for (int k = 0; k < cAddress.Count; k++)
////                {
////                    if (k < BPLCount)
////                    {
////                        location = new MLocation(ctx, Util.GetValueOfInt(ds.Tables[0].Rows[k]["C_LOCATION_ID"].ToString()), trx);
////                    }
////                    else
////                    {
////                        location = new MLocation(ctx, 0, trx);
////                    }
////                    //if (!cAddress[k].IsHomeAddress)
////                    //{
////                    //workAddressCount++;
////                    object WCountry_ID = 100;
////                    object WCity_ID = null;
////                    object WRegion_ID = null;
////                    if (cAddress[k].WCountry != null)            //Search Country_ID using Name for work Address
////                    {
////                        sql.Clear();
////                        sql.Append("SELECT c_country_ID FROM c_country WHERE upper(name)='" + cAddress[k].WCountry.ToUpper() + "'");
////                        WCountry_ID = DB.ExecuteScalar(sql.ToString());
////                        if (WCountry_ID == null || WCountry_ID == DBNull.Value)
////                        {
////                            WCountry_ID = 100;
////                        }
////                    }
////                    if (cAddress[k].WCity != null)               //Search CIty_ID using Name for work Address
////                    {
////                        sql.Clear();
////                        sql.Append("SELECT C_city_ID FROM c_city WHERE upper(name)='" + cAddress[k].WCity.ToUpper() + "'");
////                        WCity_ID = DB.ExecuteScalar(sql.ToString());
////                    }
////                    if (cAddress[k].WRegion != null)             //Search Region_ID using Name for work Address
////                    {
////                        sql.Clear();
////                        sql.Append("SELECT c_region_id FROM c_region WHERE upper(name)='" + cAddress[k].WRegion.ToUpper() + "'");
////                        WRegion_ID = DB.ExecuteScalar(sql.ToString());
////                    }


////                    //-----------Insset data into C_Location for C_BPartner-------------\\


////                    sql.Clear();
////                    location.SetAD_Org_ID(Org_ID);
////                    location.SetAD_Client_ID(ctx.GetAD_Client_ID());
////                    location.SetC_Country_ID(Util.GetValueOfInt(WCountry_ID));
////                    if (WRegion_ID != null && WRegion_ID != DBNull.Value)
////                    {
////                        location.SetC_Region_ID(Util.GetValueOfInt(WRegion_ID));
////                    }
////                    if (WCity_ID != null && WCity_ID != DBNull.Value)
////                    {
////                        location.SetC_City_ID(Util.GetValueOfInt(WCity_ID));
////                    }
////                    string address1 = string.Empty;
////                    if (cAddress[k].WHouseNo != null && cAddress[k].WHouseNo != "" && cAddress[k].WHouseNo != ",")
////                    {
////                        address1 = cAddress[k].WHouseNo + ",";
////                    }
////                    if (cAddress[k].WStreetno != null && cAddress[k].WStreetno != "" && cAddress[k].WStreetno != ",")
////                    {
////                        address1 += cAddress[k].WStreetno;
////                    }

////                    location.SetAddress1(address1);
////                    if (cAddress[k].WPostcode != null)
////                    {
////                        location.SetPostal(cAddress[k].WPostcode.ToString());
////                    }
////                    if (cAddress[k].WCity != null)
////                    {
////                        location.SetCity(cAddress[k].WCity.ToString());
////                    }
////                    if (cAddress[k].WRegion != null)
////                    {
////                        location.SetRegionName(cAddress[k].WRegion.ToString());
////                    }
////                    if (!location.Save(trx))
////                    {
////                        trx.Rollback();
////                        trx.Close();
////                        result.Append("Location2 not saved");
////                        count--;
////                        bpCount--;
////                        continue;
////                    }
////                    sql.Clear();

////                    sql.Append("Update C_Location set LastGmailUpdated =" + SetTime(((MyContacts)objMyContact[i]).Updated) + " where    C_Location_ID=" + location.GetC_Location_ID());
////                    if (DB.ExecuteQuery(sql.ToString(), null, trx) == -1)
////                    {
////                        trx.Rollback();
////                        trx.Close();
////                        result.Append("C_Location " + location.GetC_Location_ID() + " not Updated");
////                        count--;
////                        bpCount--;
////                        continue;
////                    }

////                    updatedTime = location.GetUpdated();
////                    ID = location.GetC_Location_ID();
////                    sql1 = "Update C_Location set updated=" + GlobalVariable.TO_DATE(updatedTime, false) + ", LastLocalUpdated=" + GlobalVariable.TO_DATE(updatedTime, false) + " where C_Location_ID=" + ID;
////                    if (DB.ExecuteQuery(sql1.ToString(), null, trx) == -1)
////                    {
////                        trx.Rollback();
////                        trx.Close();
////                        result.Append("C_Location " + location.GetC_Location_ID() + " not Updated");
////                        count--;
////                        bpCount--;
////                        continue;
////                    }

////                    //------------------Create C_Bpartner_Location for business partner using C_Location.---------------------\\
////                    MBPartnerLocation bpLocation = null;
////                    if (k < BPLCount)
////                    {
////                        bpLocation = new MBPartnerLocation(ctx, Util.GetValueOfInt(ds.Tables[0].Rows[k]["C_BPARTNER_LOCATION_ID"].ToString()), trx);
////                    }
////                    else
////                    {
////                        bpLocation = new MBPartnerLocation(ctx, 0, trx);
////                    }
////                    bpLocation.SetAD_Org_ID(Org_ID);
////                    bpLocation.SetAD_Client_ID(ctx.GetAD_Client_ID());
////                    bpLocation.SetC_Location_ID(location.GetC_Location_ID());
////                    bpLocation.SetC_BPartner_ID(partner.GetC_BPartner_ID());
////                    bpLocation.SetIsBillTo(true);
////                    bpLocation.SetIsPayFrom(true);
////                    bpLocation.SetIsRemitTo(true);
////                    bpLocation.SetIsShipTo(true);
////                    bpLocation.SetName(((MyContacts)objMyContact[i]).BpartnerName.ToString());
////                    if (!bpLocation.Save(trx))
////                    {
////                        trx.Rollback();
////                        trx.Close();
////                        result.Append("BPLocation for " + ((MyContacts)objMyContact[i]).BpartnerName.ToString() + " not saved.");
////                        count--;
////                        bpCount--;
////                        continue;
////                    }

////                    sql.Clear();
////                    sql.Append("Update C_BPartner_Location set LastGmailUpdated =" + SetTime(((MyContacts)objMyContact[i]).Updated) + " where    C_BPartner_Location_ID=" + bpLocation.GetC_BPartner_Location_ID());
////                    if (DB.ExecuteQuery(sql.ToString(), null, trx) == -1)
////                    {
////                        trx.Rollback();
////                        trx.Close();
////                        result.Append("C_BPartner_Location for  " + bpLocation.GetName() + " not Updated");
////                        count--;
////                        bpCount--;
////                        continue;
////                    }

////                    updatedTime = bpLocation.GetUpdated();
////                    ID = bpLocation.GetC_BPartner_Location_ID();
////                    sql1 = "Update C_BPartner_Location set updated=" + GlobalVariable.TO_DATE(updatedTime, false) + ", LastLocalUpdated=" + GlobalVariable.TO_DATE(updatedTime, false) + " where C_BPartner_Location_ID=" + bpLocation.GetC_BPartner_Location_ID();
////                    if (DB.ExecuteQuery(sql1.ToString(), null, trx) == -1)
////                    {
////                        trx.Rollback();
////                        trx.Close();
////                        result.Append("C_BPartner_Location for " + bpLocation.GetName() + " not Updated");
////                        count--;
////                        bpCount--;
////                        continue;
////                    }
////                }
////                if (cAddress.Count < BPLCount)
////                {

////                    for (int k = cAddress.Count; k < BPLCount; k++)
////                    {
////                        sql.Clear();
////                        sql.Append("delete from c_bpartner_location WHERE c_location_id=" + ds.Tables[0].Rows[k]["C_LOCATION_ID"].ToString());
////                        if (DB.ExecuteQuery(sql.ToString(), null, trx) == -1)
////                        {
////                            trx.Rollback();
////                            trx.Close();
////                            result.Append("Unable to delete BPlocationID WHERE C_LocationID=" + ds.Tables[0].Rows[k]["C_LOCATION_ID"].ToString());
////                            count--;
////                            bpCount--;
////                            continue;
////                        }
////                        sql.Clear();
////                        sql.Append("delete from c_location WHERE c_location_id=" + ds.Tables[0].Rows[k]["C_LOCATION_ID"].ToString());
////                        if (DB.ExecuteQuery(sql.ToString(), null, trx) == -1)
////                        {
////                            trx.Rollback();
////                            trx.Close();
////                            result.Append("Unable to delete C_LocationID=" + ds.Tables[0].Rows[k]["C_LOCATION_ID"].ToString());
////                            count--;
////                            bpCount--;
////                            continue;
////                        }
////                    }
////                }
////            }
////            return result.ToString();
////        }


////        /// <summary>
////        /// Reload Data
////        /// </summary>
////        /// <param name="ctx"></param>
////        private void ReloadData(Ctx ctx)
////        {
////            //SetBusy(Msg.GetMsg("Refreshing"), true);
////            //gridHeader.Visibility = Visibility.Collapsed;
////            //ischecked = (bool)vchkShowExisintg.IsChecked;
////            //vchkShowExisintg.IsChecked = true;
////            //isReloading = false;
////            //vdgvContacts.ItemsSource = null;
////            //vdgvContacts.Refresh();
////            //isFirstTime = false;
////            //vbtnSave.IsEnabled = true;
////            //StartImportProcess();


////            // StartImportProcess(userName, passWord, UpdateExistingRecord, form);
////            //ImportContact(contactInfo);
////            //ImportContact(null, 1);  //selected pageindex instead of 1
////            // UserContactsModel objUserContactModel = new UserContactsModel(ctx);
////            GetDataBaseContactBPInformation(null, 1, ctx);
////        }


////        /// <summary>
////        /// Verify Data
////        /// </summary>
////        /// <param name="ctx"></param>
////        /// <returns></returns>
////        public string VerifyData(Ctx ctx)
////        {
////            //vbtnVerify.IsEnabled = false;
////            //   BusyContact.IsBusy = true;
////            //System.Threading.ThreadPool.QueueUserWorkItem(delegate
////            //{
////            string val = IsValidSelection(objMyContact, isImport, ctx);
////            if (val == "True")
////            {
////                //Dispatcher.BeginInvoke(delegate
////                //{
////                //    vbtnVerify.IsEnabled = true;
////                //    BusyContact.IsBusy = false;
////                //});
////                return "DataVerified";
////            }
////            else
////            {
////                //Dispatcher.BeginInvoke(delegate
////                //{
////                //    vbtnVerify.IsEnabled = true;
////                //    BusyContact.IsBusy = false;
////                //});
////                return val;
////            }
////            // });

////        }

//        /// <summary>
//        /// Start Import Process
//        /// </summary>
//        /// <param name="ctx"></param>
//        public void StartImportProcess(Ctx ctx)
//        {
//            try
//            {

//                bool CanUpdateExiastingRecord = true;
//                string sql = "SELECT WSP_GmailConfiguration_ID FROM WSP_GmailConfiguration WHERE WSP_GmailConfiguration.AD_USER_ID=" + ctx.GetAD_User_ID() + " and WSP_GmailConfiguration.isactive='Y' AND AD_Client_ID=" + ctx.GetAD_Client_ID();// +" AND AD_Org_ID=" + ctx.GetAD_Org_ID();
//                // sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "WSP_GmailConfiguration", MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);

//                Object gID = DB.ExecuteScalar(sql, null, null);

//                if (gID == null || gID == DBNull.Value)
//                {

//                    //ShowMessage.Info("NoRecordFound", true, null, null);
//                    //Dispatcher.BeginInvoke(delegate
//                    //{
//                    //    BusyContact.IsBusy = false;
//                    //});
//                    return;
//                }
//                MGmailConfiguration gSettings = new MGmailConfiguration(ctx, Util.GetValueOfInt(gID), null);
//                CanUpdateExiastingRecord = gSettings.IsWSP_IsUpdateExistingRecord();
//                username = gSettings.GetWSP_Username();
//                password = gSettings.GetWSP_Password();
//                UpdateExistingRecord = gSettings.IsWSP_IsUpdateExistingRecord();
//                if (username == null || username == "")
//                {

//                    //ShowMessage.Info("UsernameNotFound", true, null, null);
//                    //Dispatcher.BeginInvoke(delegate
//                    //{
//                    //    BusyContact.IsBusy = false;
//                    //});
//                    return;
//                }
//                if (password == null || password == "")
//                {
//                    return;
//                }

//                result = new Result();
//                result = GetGmailContacts(username, password, PAGE_SIZE);
//                GetDataBaseContactBPInformation(result.info, 0, ctx);

//            }
//            catch (Exception ex)
//            {
//                //VLogger.Get().Info("Error in Importing Contacts--->" + ex.Message);
//            }
//        }


//        /// <summary>
//        /// Import Contacts
//        /// </summary>
//        /// <param name="username"></param>
//        /// <param name="password"></param>
//        /// <param name="pageSize"></param>
//        /// <returns></returns>
//        private Result GetGmailContacts(string username, string password, int pageSize)
//        {
//            // IResult result = null;
//            // result = OperationContext.Current.GetCallbackChannel<IResult>();
//            Stopwatch watch = new Stopwatch();
//            watch.Start();
//            ObservableCollection<ContactsInfo> contactinfo = new ObservableCollection<ContactsInfo>();
//            RequestSettings rs = new RequestSettings("Vienna", username, password);
//            rs.AutoPaging = true;
//            ContactsRequest cr = new ContactsRequest(rs);
//            ContactsQuery query = new ContactsQuery(ContactsQuery.CreateContactsUri("default"));
//            query.NumberToRetrieve = 1000;
//            query.OrderBy = "lastmodified";
//            query.SortOrder = "descending";
//            //query.StartIndex = 1;
//            Feed<Contact> f = cr.Get<Contact>(query);
//            List<ContactPhone> contactPhone = null;
//            DateTime datet;
//            List<ContactAddressInfo> CAdd = null;
//            ContactPhone cphone = null;
//            ContactAddressInfo conAdd = null;
//            List<ContactEmail> emailAdd = null;
//            List<string> allemails = null;
//            Result res = new Result();
//            try
//            {
//                int i = 0;
//                foreach (Contact e in f.Entries)
//                {
//                    i++;
//                    ContactsInfo cinfo = new ContactsInfo();
//                    cinfo.Title = e.Title;
//                    cinfo.Content = e.Content;
//                    cinfo.ID = e.Id;
//                    cinfo.Index = Convert.ToString(i - 1);
//                    cinfo.ContactName = e.Name.FullName;

//                    if (e.ContactEntry.Birthday != null)
//                    {
//                        string[] date = e.ContactEntry.Birthday.Split('-');
//                        datet = new DateTime(Convert.ToInt32(date.GetValue(0)), Convert.ToInt32(date.GetValue(1)), Convert.ToInt32(date.GetValue(2)));
//                        cinfo.BirthDay = datet;
//                    }
//                    else
//                    {
//                        cinfo.BirthDay = null;
//                    }

//                    contactPhone = new List<ContactPhone>();
//                    foreach (Google.GData.Extensions.PhoneNumber PhoneNumber in e.Phonenumbers)
//                    {
//                        cphone = new ContactPhone();
//                        if (PhoneNumber.Rel == ContactsRelationships.IsWork)
//                        {
//                            cphone.IsPhone = true;
//                        }
//                        else if (PhoneNumber.Rel == ContactsRelationships.IsMobile)
//                        {
//                            cphone.IsMobile = true;
//                        }
//                        else if (PhoneNumber.Rel == ContactsRelationships.IsHome)
//                        {
//                            cphone.IsPhone2 = true;
//                        }
//                        cphone.PhoneNumber = PhoneNumber.Value;
//                        contactPhone.Add(cphone);
//                    }
//                    cinfo.PhoneNumber = contactPhone;

//                    CAdd = new List<ContactAddressInfo>();
//                    foreach (Google.GData.Extensions.StructuredPostalAddress pAdd in e.PostalAddresses)
//                    {
//                        conAdd = new ContactAddressInfo();
//                        if (pAdd.Rel != null)
//                        {
//                            if (pAdd.Rel == ContactsRelationships.IsHome)
//                            {
//                                conAdd.IsHomeAddress = true;
//                            }
//                            else if (pAdd.Rel == ContactsRelationships.IsWork)
//                            {
//                                conAdd.IsHomeAddress = false;

//                            }
//                            conAdd.City = pAdd.City;
//                            conAdd.Country = pAdd.Country;
//                            conAdd.Housename = pAdd.Housename;
//                            conAdd.Region = pAdd.Region;
//                            conAdd.StreetNo = pAdd.Street;
//                            conAdd.Postcode = pAdd.Postcode;
//                            CAdd.Add(conAdd);
//                        }
//                    }
//                    cinfo.Address = CAdd;
//                    foreach (Organization Organizations in e.Organizations)
//                    {
//                        cinfo.Organization = Organizations.Name;
//                    }


//                    emailAdd = new List<ContactEmail>();
//                    bool findWorkAddress = false;
//                    allemails = new List<string>();

//                    foreach (Google.GData.Extensions.EMail email in e.Emails)
//                    {
//                        allemails.Add(email.Address);
//                        if (email.Rel == ContactsRelationships.IsWork && email.Primary == true)
//                        {
//                            findWorkAddress = true;
//                            cinfo.EmailAddress = email.Address;
//                        }
//                        else if (email.Rel == ContactsRelationships.IsWork)
//                        {
//                            if (!findWorkAddress)
//                            {
//                                findWorkAddress = true;
//                                cinfo.EmailAddress = email.Address;
//                            }
//                        }
//                        else if (email.Primary)
//                        {
//                            if (!findWorkAddress)
//                            {
//                                cinfo.EmailAddress = email.Address;
//                            }
//                        }
//                    }
//                    cinfo.AllEmails = allemails;


//                    foreach (GroupMembership g in e.GroupMembership)
//                    {
//                        cinfo.Groupmembership = g.HRef;
//                    }
//                    foreach (IMAddress im in e.IMs)
//                    {
//                        cinfo.IMAddress = im.Address;
//                    }
//                    cinfo.IsCustomer = false;
//                    cinfo.IsEmployee = false;
//                    cinfo.IsVendor = false;
//                    cinfo.IsSelected = false;
//                    cinfo.Updated = e.Updated;
//                    contactinfo.Add(cinfo);
//                }

//                res.info = contactinfo;
//                res.IsCompleted = false;
//            }
//            catch (Exception ex)
//            {
//                res.Message = ex.Message;
//            }
//            watch.Stop();
//            TimeSpan time = watch.Elapsed;
//            return res;


//        }


////        /// <summary>
////        /// Set Result
////        /// </summary>
////        /// <param name="cinfo"></param>
////        /// <param name="isCompleted"></param>
////        /// <param name="message"></param>
////        /// <returns></returns>
////        public Result SetResult(ObservableCollection<ContactsInfo> cinfo, bool isCompleted, string message)
////        {
////            Result res = new Result();
////            res.info = cinfo;
////            res.IsCompleted = isCompleted;
////            if (message != null)
////            {
////                res.Message = message;
////            }
////            else
////            {
////                res.Message = "NoError";
////            }

////            return res;
////        }


//        /// <summary>
//        /// Import
//        /// </summary>
//        /// <param name="cInfo"></param>
//        /// <param name="v"></param>
//        /// <param name="ctx"></param>
//        private void GetDataBaseContactBPInformation(ObservableCollection<ContactsInfo> cInfo, int v, Ctx ctx)
//        {
//            //VLogger.Get().Info("ImportContact");
//            DataSet ds = null;
//            isImport = true;
//            if (cInfo == null)
//            {
//                return;
//            }



//            string sql = "SELECT c_bp_group_id, name FROM c_bp_group";

//            GetIDList(ctx);
//            DataSet dsUserlst = null;
//            DataSet dsempdtls = SetEmployeeDetails();
//            ds = DB.ExecuteDataset(sql, null);

//            //Dispatcher.BeginInvoke(delegate
//            //{
//            try
//            {
//                ObservableCollection<MyContacts> myContacts = new ObservableCollection<MyContacts>();
//                if (ds == null || ds.Tables[0].Rows.Count == 0)
//                {
//                }
//                else
//                {
//                    isFirstTime = true;
//                    if (cInfo.Count > 0)
//                    {
//                        for (int i = 0; i < cInfo.Count; i++)
//                        {
//                            try
//                            {
//                                bool alreadyImported = false;
//                                MyContacts mc = new MyContacts();
//                                mc.Title = cInfo[i].Title;


//                                mc.EmailAddress = cInfo[i].EmailAddress;
//                                mc.ContactName = cInfo[i].ContactName;
//                                mc.Content = cInfo[i].Content;
//                                mc.Groupmembership = cInfo[i].Groupmembership;
//                                mc.IMAddress = cInfo[i].IMAddress;
//                                mc.Updated = cInfo[i].Updated;
//                                mc.Organization = cInfo[i].Organization;
//                                mc.BpartnerName = cInfo[i].Bpartner;
//                                mc.BirthDay = cInfo[i].BirthDay;
//                                mc.Index = cInfo[i].Index;
//                                if (mc.BirthDay != null)
//                                {
//                                    DateTime bDay = Convert.ToDateTime(mc.BirthDay);
//                                    mc.BirthDay = Convert.ToDateTime(bDay.ToString(VAdvantage.Login.Language.GetLoginLanguage().GetDateFormat().ShortDatePattern));
//                                }

//                                for (int k = 0; k < cInfo[i].PhoneNumber.Count; k++)
//                                {
//                                    if (cInfo[i].PhoneNumber[k].IsMobile)
//                                    {
//                                        mc.Mobile = cInfo[i].PhoneNumber[k].PhoneNumber;
//                                    }
//                                    if (cInfo[i].PhoneNumber[k].IsPhone)
//                                    {
//                                        mc.PhoneNumber = cInfo[i].PhoneNumber[k].PhoneNumber;
//                                    }
//                                    if (cInfo[i].PhoneNumber[k].IsPhone2)
//                                    {
//                                        mc.PhoneNumber2 = cInfo[i].PhoneNumber[k].PhoneNumber;
//                                    }
//                                }
//                                mc.IsSelected = cInfo[i].IsSelected;
//                                mc.AllEmails = cInfo[i].AllEmails;
//                                mc.ID = cInfo[i].ID;
//                                mc.IsContactExist = false;
//                                if (dsempdtls == null)
//                                {
//                                    mc.IsCustomer = cInfo[i].IsCustomer;
//                                    mc.IsVendor = cInfo[i].IsVendor;
//                                    mc.IsEmployee = cInfo[i].IsEmployee;
//                                    mc.SelectedVal = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_BP_GROUP_ID"]);
//                                    mc.SelectedItems = 0;
//                                }
//                                else
//                                {
//                                    if (UIDList.Contains(cInfo[i].ID))
//                                    {
//                                        for (int j = 0; j < dsempdtls.Tables[0].Rows.Count; j++)
//                                        {
//                                            if (dsempdtls.Tables[0].Rows[j]["GMAIL_UID"].ToString() == cInfo[i].ID)     //Check if user already exist
//                                            {
//                                                if (dsempdtls.Tables[0].Rows[j]["C_BPARTNER_ID"] != null && dsempdtls.Tables[0].Rows[j]["C_BPARTNER_ID"].ToString() != "")
//                                                {
//                                                    alreadyImported = true;

//                                                    mc.ContactName = Util.GetValueOfString(dsempdtls.Tables[0].Rows[j]["NAME"]);
//                                                    mc.AD_User_ID = Util.GetValueOfInt(dsempdtls.Tables[0].Rows[j]["AD_User_ID"]);
//                                                    mc.IsContactExist = true;
//                                                    //-----------------------------------

//                                                    mc.ImportedRecord = true;
//                                                    mc.BPartner_ID = Util.GetValueOfInt(dsempdtls.Tables[0].Rows[j]["C_BPARTNER_ID"].ToString());
//                                                    mc.BPDocNo = dsempdtls.Tables[0].Rows[j]["VALUE"].ToString();
//                                                    mc.IsCustomer = dsempdtls.Tables[0].Rows[j]["ISCUSTOMER"].ToString().Equals("Y") ? true : false;
//                                                    mc.IsEmployee = dsempdtls.Tables[0].Rows[j]["ISEMPLOYEE"].ToString().Equals("Y") ? true : false;
//                                                    mc.IsVendor = dsempdtls.Tables[0].Rows[j]["ISVENDOR"].ToString().Equals("Y") ? true : false;
//                                                    if (dsempdtls.Tables[0].Rows[j]["BPNAME"] != null && dsempdtls.Tables[0].Rows[j]["BPNAME"].ToString() != "")
//                                                    {
//                                                        mc.BpartnerName = dsempdtls.Tables[0].Rows[j]["BPNAME"].ToString();
//                                                    }

//                                                    SetDetailsOfExistingBpartner(dsempdtls, j, mc, true);

//                                                    for (int k = 0; k < ds.Tables[0].Rows.Count; k++)
//                                                    {
//                                                        if (ds.Tables[0].Rows[k]["C_BP_GROUP_ID"].ToString() == dsempdtls.Tables[0].Rows[j]["C_BP_GROUP_ID"].ToString())
//                                                        {
//                                                            mc.SelectedVal = Util.GetValueOfInt(dsempdtls.Tables[0].Rows[j]["C_BP_GROUP_ID"].ToString());
//                                                            mc.SelectedItems = k;
//                                                            break;
//                                                        }
//                                                    }
//                                                    break;
//                                                }
//                                            }
//                                        }
//                                    }
//                                    else
//                                    {
//                                        mc.SelectedVal = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_BP_GROUP_ID"]);
//                                        mc.SelectedItems = 0;
//                                    }
//                                }

//                                if (!alreadyImported)
//                                {
//                                    List<ContactAddressInfo> add = cInfo[i].Address;
//                                    ObservableCollection<ContactAddress> workaddress = new ObservableCollection<ContactAddress>();
//                                    ObservableCollection<string> displayAddress = new ObservableCollection<string>();

//                                    for (int j = 0; j < add.Count; j++)
//                                    {
//                                        ContactAddress newHomeAddress = new ContactAddress();
//                                        ContactAddress newWorkAddress = new ContactAddress();
//                                        if (add[j].IsHomeAddress)
//                                        {

//                                            newHomeAddress.HomeCity = add[j].City;
//                                            newHomeAddress.HomeStreetno = add[j].StreetNo;
//                                            newHomeAddress.HomeHouseNo = add[j].Housename;
//                                            newHomeAddress.HomePostcode = add[j].Postcode;
//                                            newHomeAddress.HomeRegion = add[j].Region;
//                                            newHomeAddress.HomeCountry = add[j].Country;
//                                            newHomeAddress.IsHomeAddress = true;
//                                        }
//                                        else
//                                        {
//                                            newWorkAddress.WCity = add[j].City;
//                                            newWorkAddress.WStreetno = add[j].StreetNo;
//                                            newWorkAddress.WHouseNo = add[j].Housename;
//                                            newWorkAddress.WPostcode = add[j].Postcode;
//                                            newWorkAddress.WRegion = add[j].Region;
//                                            newWorkAddress.WCountry = add[j].Country;
//                                            newWorkAddress.IsHomeAddress = false;
//                                            workaddress.Add(newWorkAddress);
//                                        }

//                                        if (!add[j].IsHomeAddress)
//                                        {
//                                            string Addres = string.Empty;

//                                            if (add[j].Housename != null && add[j].Housename != "" && add[j].Housename != ",")
//                                            {
//                                                Addres = add[j].Housename + " ";
//                                            }
//                                            if (add[j].StreetNo != null && add[j].StreetNo != "" && add[j].StreetNo != ",")
//                                            {
//                                                Addres += add[j].StreetNo + ", ";
//                                            }
//                                            if (add[j].Postcode != null && add[j].Postcode != "" && add[j].Postcode != ",")
//                                            {
//                                                Addres += add[j].Postcode + ", ";
//                                            }
//                                            if (add[j].City != null && add[j].City != "" && add[j].City != ",")
//                                            {
//                                                Addres += add[j].City + ", ";
//                                            }
//                                            if (add[j].Region != null && add[j].Region != "" && add[j].Region != ",")
//                                            {
//                                                Addres += add[j].Region + ", ";
//                                            }
//                                            if (add[j].Country != null && add[j].Country != "" && add[j].Country != ",")
//                                            {
//                                                Addres += add[j].Country + ", ";
//                                            }
//                                            if (Addres.ToString().EndsWith(", "))
//                                            {
//                                                Addres = Addres.Remove(Addres.Length - 2);
//                                            }
//                                            displayAddress.Add(Addres);
//                                        }
//                                    }
//                                    mc.BPAddress = displayAddress;

//                                    if (displayAddress.Count > 0)
//                                    {
//                                        mc.SelectedAddress = 0;
//                                    }
//                                    else
//                                    {
//                                        mc.SelectedAddress = -1;
//                                    }
//                                    mc.CWorkAddress = workaddress;
//                                }
//                                myContacts.Add(mc);
//                                objMyContact.Add(mc);
//                            }
//                            catch
//                            { }
//                        }
//                        if (objMyContact.Count == total && isfullyImported)
//                        {
                         
//                        }
//                        if (objMyContact.Count == 1)
//                        {
                         
//                        }
//                    }

//                }
//            }
//            catch (Exception ex)
//            {
//                // ShowMessage.Error(ex.Message, true);
//            }
//            //});
//            //});
//        }



//        /// <summary>
//        /// Get Data to set employee Detail
//        /// </summary>
//        /// <returns></returns>
//        private DataSet SetEmployeeDetails()
//        {
//            //            string sql = @"select usr.GMAIL_UID, bp.isemployee, bp.isvendor, bp.iscustomer, bp.c_bp_group_id,bp.c_bpartner_ID, bp.name as BPName,
//            //                            usr.value from c_bpartner bp join AD_user usr on bp.c_bpartner_id= usr.c_bpartner_id where usr.GMAIL_UID is not null";

//            string sql = @"SELECT AD_user.GMAIL_UID, C_BPartner.isemployee, C_BPartner.isvendor, C_BPartner.iscustomer, C_BPartner.c_bp_group_id,C_BPartner.c_bpartner_ID, C_BPartner.name as BPName,
//                          AD_user.AD_User_ID,       AD_user.value ,AD_user.name,
//                          cl.City,cl.regionname, cl.address1,cco.name as BPcountry, cci.name as BPCity, cr.name as BPregion,cl.postal as BPPostal,
//                          cbl.C_Bpartner_Location_ID,CBL.c_location_ID 
//                          FROM c_bpartner C_BPartner right outer join AD_user AD_user on C_BPartner.c_bpartner_id= AD_user.c_bpartner_id 
//                          Left Outer JOIn c_bpartner_location CBL on C_BPartner.c_bpartner_id= cbl.c_bpartner_id 
//                          Left Outer Join c_location CL  on Cl.c_location_id= cbl.c_location_id
//                          Left outer JOIN c_country CCO on cco.c_country_id=cl.c_country_id
//                          Left outer JOIn c_city CCI on CCI.c_city_ID=cl.c_city_ID
//                          Left outer JOIN c_region CR on cr.c_region_id=cl.c_region_id
//                          WHERE AD_user.GMAIL_UID is not null ";

//            sql += " order by AD_user.AD_User_ID";

//            DataSet ds = DB.ExecuteDataset(sql, null);
//            if (ds != null && ds.Tables[0].Rows.Count != 0)
//            {
//                return ds;
//            }
//            return null;
//        }
    }







    public class ContactsInfo
    {
       
        public string Title { get; set; }
        public string Index { get; set; }
        public string EmailAddress { get; set; }
      
        public string ContactName { get; set; }
     
        public string Content { get; set; }
   
        public List<ContactPhone> PhoneNumber { get; set; }
       
        public string Organization { get; set; }
        public string Bpartner { get; set; }
       
        public string Groupmembership { get; set; }
        
        public string IMAddress { get; set; }
      
        public bool IsCustomer { get; set; }
       
        public bool IsVendor { get; set; }
       
        public bool IsEmployee { get; set; }
       
        public bool IsSelected { get; set; }
        public bool IsLinked { get; set; }
        public bool IsColor { get; set; }
        public string ID { get; set; }
       
        public List<ContactAddressInfo> Address { get; set; }
      
        public string Error { get; set; }
      
        public string BPID { get; set; }
        
        public DateTime Updated { get; set; }
       
        public List<string> AllEmails { get; set; }
       
        public DateTime? BirthDay { get; set; }

    }
    public class ContactPhone
    {
       
        public string PhoneNumber { get; set; }
     
        public bool IsPhone { get; set; }
      
        public bool IsPhone2 { get; set; }
       
        public bool IsMobile { get; set; }


    }
    public class ContactAddressInfo
    {
       
        public string City { get; set; }
        
        public string Country { get; set; }
       
        public string Housename { get; set; }
       
        public string Region { get; set; }
       
        public string StreetNo { get; set; }
       
        public string Postcode { get; set; }
       
        public bool IsHomeAddress { get; set; }
    }
    public class ContactEmail
    {
        
        public string Email { get; set; }       
        public bool IsPersonalEmail { get; set; }
    }

    public class MyContacts 
    {

        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (value != isSelected)
                {
                    isSelected = value;
                    //NotifyProperty("IsSelected");
                }
            }
        }
        private bool isLinked;
        public bool IsLinked
        {
            get
            {
                return isLinked;
            }
            set
            {
                if (value != isLinked)
                {
                    isLinked = value;
                    //NotifyProperty("IsSelected");
                }
            }
        }
        private bool isColor;
        public bool IsColor
        {
            get
            {
                return isColor;
            }
            set
            {
                if (value != isColor)
                {
                    isColor = value;
                    //NotifyProperty("IsSelected");
                }
            }
        }
        private string contactName;
        public string ContactName
        {
            get
            {
                return contactName;
            }
            set
            {
                if (value != contactName)
                {
                    contactName = value;
                    //NotifyProperty("ContactName");
                }
            }
        }
        private string index;
        public string Index
        {
            get
            {
                return index;
            }
            set
            {
                if (value != index)
                {
                    index = value;
                    //NotifyProperty("ContactName");
                }
            }
        }
        private string organization;
        public string Organization
        {
            get
            {
                return organization;
            }
            set
            {
                if (value != organization)
                {
                    organization = value;
                    //NotifyProperty("Organization");
                }
            }
        }

        private string bpartnerName;
        public string BpartnerName
        {
            get
            {
                return bpartnerName;
            }
            set
            {
                if (value != bpartnerName)
                {
                    bpartnerName = value;
                    //NotifyProperty("Organization");
                }
            }
        }

        private bool isCustomer;
        public bool IsCustomer
        {
            get
            {
                return isCustomer;
            }
            set
            {
                if (value != isCustomer)
                {
                    isCustomer = value;
                    //NotifyProperty("IsCustomer");
                }
            }
        }

        private bool isVendor;
        public bool IsVendor
        {
            get
            {
                return isVendor;
            }
            set
            {
                if (value != isVendor)
                {
                    isVendor = value;
                    //NotifyProperty("IsVendor");
                }
            }
        }

        private bool isEmployee;
        public bool IsEmployee
        {
            get
            {
                return isEmployee;
            }
            set
            {
                if (value != isEmployee)
                {
                    isEmployee = value;
                    //NotifyProperty("IsEmployee");
                }
            }
        }

        private string bpGroup;  //By Sarab (type changed from binding source to string)
        public string BpGroup
        {
            get
            {
                return bpGroup;
            }
            set
            {
                if (value != bpGroup)
                {
                    bpGroup = value;
                    //NotifyProperty("BpGroup");
                }
            }
        }


       
        private string title;
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                if (value != title)
                {
                    title = value;
                    //NotifyProperty("Title");
                }
            }
        }

        private string emailAddress;
        public string EmailAddress
        {
            get
            {
                return emailAddress;
            }
            set
            {
                if (value != emailAddress)
                {
                    emailAddress = value;
                    //NotifyProperty("EmailAddress");
                }
            }
        }

        private string mobile;
        public string Mobile
        {
            get
            {
                return mobile;
            }
            set
            {
                if (value != mobile)
                {
                    mobile = value;
                    //NotifyProperty("Mobile");
                }
            }
        }

        private string phoneNumber;
        public string PhoneNumber
        {
            get
            {
                return phoneNumber;
            }
            set
            {
                if (value != phoneNumber)
                {
                    phoneNumber = value;
                    //NotifyProperty("PhoneNumber");
                }
            }
        }

        private string phoneNumber2;
        public string PhoneNumber2
        {
            get
            {
                return phoneNumber2;
            }
            set
            {
                if (value != phoneNumber2)
                {
                    phoneNumber2 = value;
                    //NotifyProperty("PhoneNumber2");
                }
            }
        }
        private ObservableCollection<string> bpAddress;
        public ObservableCollection<string> BPAddress
        {
            get
            {
                return bpAddress;
            }
            set
            {
                if (value != bpAddress)
                {
                    bpAddress = value;
                    //NotifyProperty("BPAddress");
                }
            }
        }
        private DateTime? birthDay;
        public DateTime? BirthDay
        {
            get
            {
                return birthDay;
            }
            set
            {
                if (value != birthDay)
                {
                    birthDay = value;
                    //NotifyProperty("BirthDay");
                }
            }
        }

        private string id;
        public string ID
        {
            get
            {
                return id;
            }
            set
            {
                if (value != id)
                {
                    id = value;
                    //NotifyProperty("ID");
                }
            }
        }


        private bool isContactExist;
        public bool IsContactExist
        {
            get
            {
                return isContactExist;
            }
            set
            {
                if (value != isContactExist)
                {
                    isContactExist = value;
                    //NotifyProperty("IsContactExist");
                }
            }
        }





        private string bpEmailAddress;
        public string BPEmailAddress
        {
            get
            {
                return bpEmailAddress;
            }
            set
            {
                if (value != bpEmailAddress)
                {
                    bpEmailAddress = value;
                    //NotifyProperty("BPEmailAddress");
                }
            }
        }




        private string content;
        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                if (value != content)
                {
                    content = value;
                    //NotifyProperty("Content");
                }
            }
        }

        //private ObservableCollection<ContactPhone> phoneNumber;
        //public ObservableCollection<ContactPhone> PhoneNumber
        //{
        //    get
        //    {
        //        return phoneNumber;
        //    }
        //    set
        //    {
        //        if (value != phoneNumber)
        //        {
        //            phoneNumber = value;
        //            //NotifyProperty("PhoneNumber");
        //        }
        //    }
        //}




        private DateTime updated;
        public DateTime Updated
        {
            get
            {
                return updated;
            }
            set
            {
                if (value != updated)
                {
                    updated = value;
                    //NotifyProperty("Updated");
                }
            }
        }





        private string bpPhoneNumber;
        public string BPPhoneNumber
        {
            get
            {
                return bpPhoneNumber;
            }
            set
            {
                if (value != bpPhoneNumber)
                {
                    bpPhoneNumber = value;
                    //NotifyProperty("BPPhoneNumber");
                }
            }
        }



        private string groupmembership;
        public string Groupmembership
        {
            get
            {
                return groupmembership;
            }
            set
            {
                if (value != groupmembership)
                {
                    groupmembership = value;
                    //NotifyProperty("Groupmembership");
                }
            }
        }

        private string iMAddress;
        public string IMAddress
        {
            get
            {
                return iMAddress;
            }
            set
            {
                if (value != iMAddress)
                {
                    iMAddress = value;
                    //NotifyProperty("IMAddress");
                }
            }
        }




        private ObservableCollection<ContactAddress> cworkAddress;
        public ObservableCollection<ContactAddress> CWorkAddress
        {
            get
            {
                return cworkAddress;
            }
            set
            {
                if (value != cworkAddress)
                {
                    cworkAddress = value;
                    //NotifyProperty("CWorkAddress");
                }
            }
        }

        private ObservableCollection<ContactAddress> chomeAddress;
        public ObservableCollection<ContactAddress> CHomeAddress
        {
            get
            {
                return chomeAddress;
            }
            set
            {
                if (value != chomeAddress)
                {
                    chomeAddress = value;
                    //NotifyProperty("CHomeAddress");
                }
            }
        }


        private int selectedItems;
        public int SelectedItems
        {
            get
            {
                return selectedItems;
            }
            set
            {
                if (value != selectedItems)
                {
                    selectedItems = value;
                    //NotifyProperty("SelectedItems");
                }
            }
        }

        //

        private int selectedIndex;
        public int SelectedIndex
        {
            get
            {
                return selectedIndex;
            }
            set
            {
                if (value != selectedIndex)
                {
                    selectedIndex = value;
                    //NotifyProperty("SelectedIndex");
                }
            }
        }


        private int selectedval;
        public int SelectedVal
        {
            get
            {
                return selectedval;
            }
            set
            {
                if (value != selectedval)
                {
                    selectedval = value;
                    //NotifyProperty("SelectedVal");
                }
            }
        }

        private int bpartner_ID;
        public int BPartner_ID
        {
            get
            {
                return bpartner_ID;
            }
            set
            {
                if (value != bpartner_ID)
                {
                    bpartner_ID = value;
                    //NotifyProperty("BPartner_ID");
                }
            }
        }

        private int ad_User_ID;
        public int AD_User_ID
        {
            get
            {
                return ad_User_ID;
            }
            set
            {
                if (value != ad_User_ID)
                {
                    ad_User_ID = value;
                    //NotifyProperty("AD_User_ID");
                }
            }
        }


        private string bPdocno;
        public string BPDocNo
        {
            get
            {
                return bPdocno;
            }
            set
            {
                if (value != bPdocno)
                {
                    bPdocno = value;
                    //NotifyProperty("BPDocNo");
                }
            }
        }


        private bool isContactEnabled;
        public bool IsContactEnabled
        {
            get
            {
                return isContactEnabled;
            }
            set
            {
                if (value != isContactEnabled)
                {
                    isContactEnabled = value;
                    //NotifyProperty("IsContactEnabled");
                }
            }
        }

        private bool isOrgEnabled;
        public bool IsOrgEnabled
        {
            get
            {
                return isOrgEnabled;
            }
            set
            {
                if (value != isOrgEnabled)
                {
                    isOrgEnabled = value;
                    //NotifyProperty("IsOrgEnabled");
                }
            }
        }


        private string userUEmailID;
        public string UserUEmailID
        {
            get
            {
                return userUEmailID;
            }
            set
            {
                if (value != userUEmailID)
                {
                    userUEmailID = value;
                    //NotifyProperty("UserUEmailID");
                }
            }
        }

     

        private List<string> allEmails;
        public List<string> AllEmails
        {
            get
            {
                return allEmails;
            }
            set
            {
                if (value != allEmails)
                {
                    allEmails = value;
                  //  //NotifyProperty("AllEmails");
                }
            }
        }

        private bool existingRecords;
        public bool ImportedRecord
        {
            get
            {
                return existingRecords;
            }
            set
            {
                if (value != existingRecords)
                {
                    existingRecords = value;
                  //  //NotifyProperty("ImportedRecord");
                }
            }
        }

        private int selectedAddress;
        public int SelectedAddress
        {
            get
            {
                return selectedAddress;
            }
            set
            {
                if (value != selectedAddress)
                {
                    selectedAddress = value;
                    //NotifyProperty("SelectedAddress");
                }
            }
        }

        //HeaderItem
        //private bool headerItem;
        //public bool HeaderItem
        //{
        //    get
        //    {
        //        return headerItem;
        //    }
        //    set
        //    {
        //        if (value != headerItem)
        //        {
        //            headerItem = value;
        //            //NotifyProperty("HeaderItem");
        //        }
        //    }
        //}


    }

    public class ContactAddress
    {

        private string homeStreetno;
        public string HomeStreetno
        {
            get
            {
                return homeStreetno;
            }
            set
            {
                if (value != homeStreetno)
                {
                    homeStreetno = value;
                   // NotifyProperty("HomeStreetno");
                }
            }
        }

        private bool ishomeAddress;
        public bool IsHomeAddress
        {
            get
            {
                return ishomeAddress;
            }
            set
            {
                if (value != ishomeAddress)
                {
                    ishomeAddress = value;
                   // NotifyProperty("IsHomeAddress");
                }
            }
        }

        private string homeHouseNo;
        public string HomeHouseNo
        {
            get
            {
                return homeHouseNo;
            }
            set
            {
                if (value != homeHouseNo)
                {
                    homeHouseNo = value;
                  //  NotifyProperty("HomeHouseNo");
                }
            }
        }

        private string homeRegion;
        public string HomeRegion
        {
            get
            {
                return homeRegion;
            }
            set
            {
                if (value != homeRegion)
                {
                    homeRegion = value;
                  //  NotifyProperty("HomeRegion");
                }
            }
        }

        private string homeCity;
        public string HomeCity
        {
            get
            {
                return homeCity;
            }
            set
            {
                if (value != homeCity)
                {
                    homeCity = value;
                   // NotifyProperty("HomeCity");
                }
            }
        }

        private string homePostcode;
        public string HomePostcode
        {
            get
            {
                return homePostcode;
            }
            set
            {
                if (value != homePostcode)
                {
                    homePostcode = value;
                   // NotifyProperty("HomePostcode");
                }
            }
        }

        private string homeCountry;
        public string HomeCountry
        {
            get
            {
                return homeCountry;
            }
            set
            {
                if (value != homeCountry)
                {
                    homeCountry = value;
                   // NotifyProperty("HomeCountry");
                }
            }
        }

        private string wStreetno;
        public string WStreetno
        {
            get
            {
                return wStreetno;
            }
            set
            {
                if (value != wStreetno)
                {
                    wStreetno = value;
                  //  NotifyProperty("WStreetno");
                }
            }
        }

        private string wHouseNo;
        public string WHouseNo
        {
            get
            {
                return wHouseNo;
            }
            set
            {
                if (value != wHouseNo)
                {
                    wHouseNo = value;
                  //  NotifyProperty("WHouseNo");
                }
            }
        }

        private string wRegion;
        public string WRegion
        {
            get
            {
                return wRegion;
            }
            set
            {
                if (value != wRegion)
                {
                    wRegion = value;
                  //  NotifyProperty("WRegion");
                }
            }
        }

        private string wCity;
        public string WCity
        {
            get
            {
                return wCity;
            }
            set
            {
                if (value != wCity)
                {
                    wCity = value;
                   // NotifyProperty("WCity");
                }
            }
        }

        private string wPostcode;
        public string WPostcode
        {
            get
            {
                return wPostcode;
            }
            set
            {
                if (value != wPostcode)
                {
                    wPostcode = value;
                 //   NotifyProperty("WPostcode");
                }
            }
        }

        private string wCountry;
        public string WCountry
        {
            get
            {
                return wCountry;
            }
            set
            {
                if (value != wCountry)
                {
                    wCountry = value;
                  //  NotifyProperty("WCountry");
                }
            }
        }
    }

    public class Result
    {
        public ObservableCollection<ContactsInfo> info { get; set; }
        public bool IsCompleted { get; set; }
        public string Message { get; set; }
    }
    public class UserContactsModel
    {
        private bool isFirstTime = false;
        private List<string> UIDList = new List<string>();
        private string userName = string.Empty;
        private string passWord = string.Empty;
        private bool isImport = false;
        ObservableCollection<ContactsInfo> contactInfo = new ObservableCollection<ContactsInfo>();
        static int PAGE_SIZE = 100;
        int startPage = 0;


        public UserContactsModel(Ctx ctx)
        {
            

        }
        bool UpdateExistingRecord = true;
        public UserContactsModel(string username, string password, bool canUserUpdate, object form,Ctx ctx)
        {           
            //StartImportProcess(ctx);

        }

        public UserContactsModel(object form, bool isRoleCenter,Ctx ctx)
        {           
           // StartImportProcess(ctx);
        }
    }

}