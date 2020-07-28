/********************************************************
 * Project Name   : ViennaAdvantage
 * Class Name     : ActivateForAssignment
 * Class Used     : SvrProcess
 * Chronological    Development
 * Lokesh Chauhan   25-Apr-2012
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
////using VAdvantage.Common;
using ViennaAdvantage.Process;
////using System.Windows.Forms;
//using ViennaAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;
using VAdvantage.Process;
namespace ViennaAdvantage.Process
{
    public class ActivateForAssignment : SvrProcess
    {
        #region Private Variable
        private int S_Resource_id = 0;
        private int C_Bpartner_ID = 0;
        #endregion
        protected override void Prepare()
        {
            // C_ResourcePeriod_ID = Util.GetValueOfInt(GetRecord_ID());
            S_Resource_id = GetRecord_ID();

        }

        protected override String DoIt()
        {
            if (S_Resource_id == 0)
            {
                throw new ArgumentException("C_Project_ID == 0");
            }
            VAdvantage.Model.MResource Resource = new VAdvantage.Model.MResource(GetCtx(), S_Resource_id, Get_Trx());
            string sql = "select ProfileType from s_resource where s_resource_id=" + S_Resource_id + "";
            string ProfileType = VAdvantage.Utility.Util.GetValueOfString(DB.ExecuteScalar(sql));
            VAdvantage.Model.X_AD_User user = new VAdvantage.Model.X_AD_User(GetCtx(), 0, Get_Trx());
            VAdvantage.Model.MBPartner bp = new VAdvantage.Model.MBPartner(GetCtx(), C_Bpartner_ID, Get_Trx());
            if (ProfileType == "")
            {
                return Msg.GetMsg(GetCtx(), "ProfileTypeNotSelected");
            }
            else
            {


                if (VAdvantage.Utility.Util.GetValueOfInt(Resource.GetC_BP_Group_ID()) == 0)
                {
                    return Msg.GetMsg(GetCtx(), "ResourceNotSelected");
                }

                if (VAdvantage.Utility.Util.GetValueOfInt(Resource.GetAD_Role_ID()) == 0)
                {
                    return Msg.GetMsg(GetCtx(), "RoleNotSelected");
                }
                if (VAdvantage.Utility.Util.GetValueOfInt(Resource.GetC_Location_ID()) == 0)
                {
                    return Msg.GetMsg(GetCtx(), "AddressNotSelected");
                }
                if (ProfileType == "I")
                {

                    if (VAdvantage.Utility.Util.GetValueOfInt(Resource.GetC_BPartner_ID()) == 0 && VAdvantage.Utility.Util.GetValueOfInt(Resource.GetAD_User_ID()) == 0)
                    {
                        sql = "select count(*) from c_bpartner where upper(name) = '" + Resource.GetName().ToUpper() + "'";
                        if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) == 0)
                        {
                            bp.SetAD_Client_ID(Resource.GetAD_Client_ID());
                            bp.SetAD_Org_ID(Resource.GetAD_Org_ID());
                            bp.SetIsEmployee(true);
                            bp.SetName(Resource.GetName());
                            bp.SetEMail(Resource.GetEMail());
                            bp.SetMobile(Resource.GetMobile());
                            bp.SetC_Location_ID(Resource.GetC_Location_ID());
                            bp.SetC_BP_Group_ID(Resource.GetC_BP_Group_ID());
                            if (!bp.Save())
                            {
                                return GetRetrievedError(bp, "BusinessPartnerNotSaved");
                            }

                            VAdvantage.Model.MBPartnerLocation bploc = new VAdvantage.Model.MBPartnerLocation(GetCtx(), 0, Get_Trx());
                            bploc.SetAD_Client_ID(bp.GetAD_Client_ID());
                            bploc.SetAD_Org_ID(bp.GetAD_Org_ID());
                            bploc.SetC_BPartner_ID(bp.GetC_BPartner_ID());
                            bploc.SetPhone(Resource.GetMobile());
                            bploc.SetC_Location_ID(Resource.GetC_Location_ID());

                            if (!bploc.Save())
                            {
                               
                                //log.SaveError("BusinessPartnerLocationNotSaved", "");
                               // return "Error:- BusinessPartnerLocation" + "-" + pp.GetValue() + "," + pp.GetName();
                                return GetRetrievedError( bploc, "BusinessPartnerLocationNotSaved"); 
                            }
                        }
                        else
                        {
                            return Msg.GetMsg(GetCtx(), "BPNotSaved," + Resource.GetName() + "AlreadyExists!");
                        }

                        sql = "select count(*) from ad_user where upper(name) = '" + Resource.GetName().ToUpper() + "'";
                        if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) == 0)
                        {

                            user.SetAD_Client_ID(Resource.GetAD_Client_ID());
                            user.SetAD_Org_ID(Resource.GetAD_Org_ID());
                            user.SetC_BPartner_ID(bp.GetC_BPartner_ID());
                            user.SetEMail(Resource.GetEMail());
                            user.SetName(Resource.GetName());
                            user.SetPhone(Resource.GetMobile());
                            user.SetC_Location_ID(Resource.GetC_Location_ID());
                            user.SetIsLoginUser(true);
                            if (!user.Save())
                            {
                                //log.SaveError("UserNotSaved", "");
                               // return "Error:- User" + "-" + pp.GetValue() + "," + pp.GetName();
                                return GetRetrievedError( user, "UserNotSaved"); 
                            }
                            string usrname = user.GetName();
                            string Password = GenratePassword(usrname);
                            user.SetPassword(Password);
                            if (!user.Save())
                            {
                               // log.SaveError("PasswordNotSaved", "");
                                //return "Error:- User" + "-" + pp.GetValue() + "," + pp.GetName();
                                return GetRetrievedError( user,  "PasswordNotSaved"); 
                            }
                            int ad_role_id = Resource.GetAD_Role_ID();
                            VAdvantage.Model.X_AD_User_Roles userrole = new VAdvantage.Model.X_AD_User_Roles(GetCtx(), 0, Get_Trx());
                            userrole.SetAD_Role_ID(ad_role_id);
                            userrole.SetAD_Client_ID(Resource.GetAD_Client_ID());
                            userrole.SetAD_Org_ID(Resource.GetAD_Org_ID());
                            userrole.SetAD_User_ID(user.GetAD_User_ID());

                            if (!userrole.Save())
                            {
                                //log.SaveError("UserRoleNotSaved", "");
                                //return "Error:- User Role" + "-" + pp.GetValue() + "," + pp.GetName();
                                return GetRetrievedError( userrole,  "UserRoleNotSaved"); 
                            }

                            Resource.SetActivateForAssignment("Y");
                        }
                        else
                        {
                            return Msg.GetMsg(GetCtx(), "UserNotSaved" + Resource.GetName() + "AlreadyExists");
                        }
                    }
                    else if (VAdvantage.Utility.Util.GetValueOfInt(Resource.GetC_BPartner_ID()) != 0 && VAdvantage.Utility.Util.GetValueOfInt(Resource.GetAD_User_ID()) == 0)
                    {
                        sql = "select count(*) from ad_user where upper(name) = '" + Resource.GetName().ToUpper() + "'";
                        if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) == 0)
                        {
                            user.SetAD_Client_ID(Resource.GetAD_Client_ID());
                            user.SetAD_Org_ID(Resource.GetAD_Org_ID());
                            user.SetC_BPartner_ID(Resource.GetC_BPartner_ID());
                            user.SetEMail(Resource.GetEMail());
                            user.SetName(Resource.GetName());
                            user.SetPhone(Resource.GetMobile());
                            user.SetC_Location_ID(Resource.GetC_Location_ID());
                            user.SetIsLoginUser(true);
                            if (!user.Save())
                            {
                                //log.SaveError("UserNotSaved", "");
                                //return "Error:- User" + "-" + pp.GetValue() + "," + pp.GetName();
                                return GetRetrievedError( user,  "UserNotSaved"); 
                            }
                            //string sqlusr = "select name from Ad_user where ad_user_id=" + user.GetAD_User_ID() + "";
                            //string usrname = VAdvantage.Utility.Util.GetValueOfString(DB.ExecuteScalar(sqlusr));
                            //GenratePassword(usrname);
                            string usrname = user.GetName();
                            string Password = GenratePassword(usrname);
                            user.SetPassword(Password);
                            if (!user.Save())
                            {
                               // log.SaveError("PasswordNotSaved", "");
                                //return "Error:- User" + "-" + pp.GetValue() + "," + pp.GetName();
                                return GetRetrievedError( user, "PasswordNotSaved"); 
                            }

                            int ad_role_id = Resource.GetAD_Role_ID();
                            VAdvantage.Model.X_AD_User_Roles userrole = new VAdvantage.Model.X_AD_User_Roles(GetCtx(), 0, Get_Trx());
                            userrole.SetAD_Role_ID(ad_role_id);
                            userrole.SetAD_Client_ID(Resource.GetAD_Client_ID());
                            userrole.SetAD_Org_ID(Resource.GetAD_Org_ID());
                            userrole.SetAD_User_ID(user.GetAD_User_ID());

                            if (!userrole.Save())
                            {
                                //log.SaveError("UserRoleNotSaved", "");
                                //return "Error:- User Role" + "-" + pp.GetValue() + "," + pp.GetName();
                                return GetRetrievedError( userrole,  "UserRoleNotSaved"); 
                            }

                            Resource.SetActivateForAssignment("Y");
                        }
                        else
                        {
                            return Msg.GetMsg(GetCtx(), "UserNotSaved" + Resource.GetName() + "AlreadyExists");
                        }
                    }

                }
                else if (ProfileType == "E")
                {

                    if (VAdvantage.Utility.Util.GetValueOfInt(Resource.GetC_BPartner_ID()) == 0 && VAdvantage.Utility.Util.GetValueOfInt(Resource.GetAD_User_ID()) == 0)
                    {
                        sql = "select count(*) from c_bpartner where upper(name) = '" + Resource.GetName().ToUpper() + "'";
                        if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) == 0)
                        {
                            bp.SetAD_Client_ID(Resource.GetAD_Client_ID());
                            bp.SetAD_Org_ID(Resource.GetAD_Org_ID());
                            bp.SetIsVendor(true);
                            bp.SetName(Resource.GetName());
                            bp.SetEMail(Resource.GetEMail());
                            bp.SetMobile(Resource.GetMobile());
                            bp.SetC_Location_ID(Resource.GetC_Location_ID());
                            //if (VAdvantage.Utility.Util.GetValueOfInt(Resource.GetC_BP_Group_ID()) == 0)
                            // {
                            //  return Msg.GetMsg(GetCtx(), "plzselectresource");
                            // }
                            // else
                            // {
                            bp.SetC_BP_Group_ID(Resource.GetC_BP_Group_ID());
                            // }
                            if (!bp.Save())
                            {
                                //log.SaveError("BusinessPartnerNotSaved", "");
                                //return "Error:- BusinessPartner" + "-" + pp.GetValue() + "," + pp.GetName();
                                return GetRetrievedError( bp, "BusinessPartnerNotSaved"); 
                            }

                            VAdvantage.Model.MBPartnerLocation bploc = new VAdvantage.Model.MBPartnerLocation(GetCtx(), 0, Get_Trx());
                            bploc.SetAD_Client_ID(bp.GetAD_Client_ID());
                            bploc.SetAD_Org_ID(bp.GetAD_Org_ID());
                            bploc.SetC_BPartner_ID(bp.GetC_BPartner_ID());
                            bploc.SetPhone(Resource.GetMobile());

                            // if (VAdvantage.Utility.Util.GetValueOfInt(Resource.GetC_Location_ID()) == 0)
                            // {
                            //     return Msg.GetMsg(GetCtx(), "Plz enter Address");
                            // }
                            // else
                            //{
                            bploc.SetC_Location_ID(Resource.GetC_Location_ID());
                            //}

                            if (!bploc.Save())
                            {
                                //log.SaveError("BusinessPartnerLocationNotSaved", "");
                                //return "Error:- BusinessPartnerLocation" + "-" + pp.GetValue() + "," + pp.GetName();
                                return GetRetrievedError( bploc, "BusinessPartnerLocationNotSaved"); 
                            }
                        }
                        else
                        {
                            return Msg.GetMsg(GetCtx(), "BPNotSaved," + Resource.GetName() + "AlreadyExists!");
                        }

                        sql = "select count(*) from ad_user where upper(name) = '" + Resource.GetName().ToUpper() + "'";
                        if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) == 0)
                        {

                            user.SetAD_Client_ID(Resource.GetAD_Client_ID());
                            user.SetAD_Org_ID(Resource.GetAD_Org_ID());
                            user.SetC_BPartner_ID(bp.GetC_BPartner_ID());
                            user.SetName(Resource.GetName());
                            user.SetEMail(Resource.GetEMail());
                            user.SetPhone(Resource.GetMobile());
                            user.SetC_Location_ID(Resource.GetC_Location_ID());
                            user.SetIsLoginUser(true);
                            if (!user.Save())
                            {
                                //log.SaveError("UserNotSaved", "");
                                //return "Error:- User" + "-" + pp.GetValue() + "," + pp.GetName();
                                return GetRetrievedError( user,  "UserNotSaved"); 
                            }
                            //string sqlusr = "select name from Ad_user where ad_user_id=" + user.GetAD_User_ID() + "";
                            //string usrname = VAdvantage.Utility.Util.GetValueOfString(DB.ExecuteScalar(sqlusr));
                            string usrname = user.GetName();
                            string Password = GenratePassword(usrname);
                            user.SetPassword(Password);
                            if (!user.Save())
                            {
                                //log.SaveError("PasswordNotSaved", "");
                                //return "Error:- User" + "-" + pp.GetValue() + "," + pp.GetName();
                                return GetRetrievedError( user,  "PasswordNotSaved"); 
                            }

                            int ad_role_id = Resource.GetAD_Role_ID();
                            VAdvantage.Model.X_AD_User_Roles userrole = new VAdvantage.Model.X_AD_User_Roles(GetCtx(), 0, Get_Trx());
                            userrole.SetAD_Role_ID(ad_role_id);
                            userrole.SetAD_Client_ID(Resource.GetAD_Client_ID());
                            userrole.SetAD_Org_ID(Resource.GetAD_Org_ID());
                            userrole.SetAD_User_ID(user.GetAD_User_ID());

                            if (!userrole.Save())
                            {
                                //log.SaveError("UserRoleNotSaved", "");
                                //return "Error:- User Role" + "-" + pp.GetValue() + "," + pp.GetName();
                                return GetRetrievedError( userrole,  "UserRoleNotSaved"); 
                            }

                            Resource.SetActivateForAssignment("Y");
                        }
                        else
                        {
                            return Msg.GetMsg(GetCtx(), "UserNotSaved" + Resource.GetName() + "AlreadyExists");
                        }


                    }
                    else if (VAdvantage.Utility.Util.GetValueOfInt(Resource.GetC_BPartner_ID()) != 0 && VAdvantage.Utility.Util.GetValueOfInt(Resource.GetAD_User_ID()) == 0)
                    {

                        sql = "select count(*) from ad_user where upper(name) = '" + Resource.GetName().ToUpper() + "'";
                        if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) == 0)
                        {
                            user.SetAD_Client_ID(Resource.GetAD_Client_ID());
                            user.SetAD_Org_ID(Resource.GetAD_Org_ID());
                            user.SetC_BPartner_ID(Resource.GetC_BPartner_ID());
                            user.SetEMail(Resource.GetEMail());
                            user.SetName(Resource.GetName());
                            user.SetPhone(Resource.GetMobile());
                            user.SetC_Location_ID(Resource.GetC_Location_ID());
                            user.SetIsLoginUser(true);
                            if (!user.Save())
                            {
                                //log.SaveError("UserNotSaved", "");
                                //return "Error:- User" + "-" + pp.GetValue() + "," + pp.GetName();
                                return GetRetrievedError( user,  "UserNotSaved"); 
                            }
                            //string sqlusr = "select name from Ad_user where ad_user_id=" + user.GetAD_User_ID() + "";
                            //string usrname = VAdvantage.Utility.Util.GetValueOfString(DB.ExecuteScalar(sqlusr));
                            string usrname = user.GetName();
                            string Password = GenratePassword(usrname);
                            user.SetPassword(Password);
                            if (!user.Save())
                            {
                                //log.SaveError("PasswordNotSaved", "");
                                //return "Error:- User" + "-" + pp.GetValue() + "," + pp.GetName();
                                return GetRetrievedError( user,  "PasswordNotSaved"); 
                            }

                            int ad_role_id = Resource.GetAD_Role_ID();
                            VAdvantage.Model.X_AD_User_Roles userrole = new VAdvantage.Model.X_AD_User_Roles(GetCtx(), 0, Get_Trx());
                            userrole.SetAD_Role_ID(ad_role_id);
                            userrole.SetAD_Client_ID(Resource.GetAD_Client_ID());
                            userrole.SetAD_Org_ID(Resource.GetAD_Org_ID());
                            userrole.SetAD_User_ID(user.GetAD_User_ID());

                            if (!userrole.Save())
                            {
                                //log.SaveError("UserRoleNotSaved", "");
                                //return "Error:- User Role" + "-" + pp.GetValue() + "," + pp.GetName();
                                return GetRetrievedError( userrole,  "UserRoleNotSaved"); 
                            }

                            Resource.SetActivateForAssignment("Y");
                        }
                        else
                        {
                            return Msg.GetMsg(GetCtx(), "UserNotSaved" + Resource.GetName() + "AlreadyExists");
                        }
                    }

                }
            }
            if (bp != null)
            {
                if (Util.GetValueOfInt(bp.GetC_BPartner_ID()) != 0)
                {
                    Resource.SetC_BPartner_ID(bp.GetC_BPartner_ID());
                }
            }
            if (user != null)
            {
                if (Util.GetValueOfInt(user.GetAD_User_ID()) != 0)
                {
                    Resource.SetAD_User_ID(user.GetAD_User_ID());
                }
            }
            if (!Resource.Save())
            {
                //log.SaveError("ResourceNotSaved", "");
                //return "Error:- Resource" + "-" + "-" + pp.GetValue() + "," + pp.GetName();
                return GetRetrievedError( Resource, "ResourceNotSaved"); 
            }




            return Msg.GetMsg(GetCtx(), "ProcessCompleted");
        }

        private string GenratePassword(string value)
        {
            string password = "";
            //if (System.Web.Configuration.WebConfigurationManager.AppSettings["Mode"].ToString().Equals("Debug"))
            //{
            //    password = VAdvantage.Utility.SecureEngine.Encrypt(value);
            //    return password;
            //}

            Random ran = new Random();


            if (value.Length >= 4)
            {
                password = value.Substring(0, 4) + ran.Next(0, 9999).ToString();

            }
            else
            {
                password = value.Substring(0, 1).ToString() + ran.Next(0, 9999999);
                //ran = null;
            }
            ran = null;

            return password;

        }

    }
}
