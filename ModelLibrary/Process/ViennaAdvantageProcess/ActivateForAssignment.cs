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
//////using System.Windows.Forms;
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
        private int VAS_Resource_ID = 0;
        private int VAB_BusinessPartner_ID = 0;
        #endregion
        protected override void Prepare()
        {
            // C_ResourcePeriod_ID = Util.GetValueOfInt(GetRecord_ID());
            VAS_Resource_ID = GetRecord_ID();

        }

        protected override String DoIt()
        {
            if (VAS_Resource_ID == 0)
            {
                throw new ArgumentException("VAB_Project_ID == 0");
            }
            VAdvantage.Model.MResource Resource = new VAdvantage.Model.MResource(GetCtx(), VAS_Resource_ID, Get_Trx());
            string sql = "select ProfileType from VAS_Resource where VAS_Resource_ID=" + VAS_Resource_ID + "";
            string ProfileType = VAdvantage.Utility.Util.GetValueOfString(DB.ExecuteScalar(sql));
            VAdvantage.Model.X_VAF_UserContact user = new VAdvantage.Model.X_VAF_UserContact(GetCtx(), 0, Get_Trx());
            VAdvantage.Model.MVABBusinessPartner bp = new VAdvantage.Model.MVABBusinessPartner(GetCtx(), VAB_BusinessPartner_ID, Get_Trx());
            if (ProfileType == "")
            {
                return Msg.GetMsg(GetCtx(), "ProfileTypeNotSelected");
            }
            else
            {


                if (VAdvantage.Utility.Util.GetValueOfInt(Resource.GetVAB_BPart_Category_ID()) == 0)
                {
                    return Msg.GetMsg(GetCtx(), "ResourceNotSelected");
                }

                if (VAdvantage.Utility.Util.GetValueOfInt(Resource.GetVAF_Role_ID()) == 0)
                {
                    return Msg.GetMsg(GetCtx(), "RoleNotSelected");
                }
                if (VAdvantage.Utility.Util.GetValueOfInt(Resource.GetVAB_Address_ID()) == 0)
                {
                    return Msg.GetMsg(GetCtx(), "AddressNotSelected");
                }
                if (ProfileType == "I")
                {

                    if (VAdvantage.Utility.Util.GetValueOfInt(Resource.GetVAB_BusinessPartner_ID()) == 0 && VAdvantage.Utility.Util.GetValueOfInt(Resource.GetVAF_UserContact_ID()) == 0)
                    {
                        sql = "select count(*) from VAB_BusinessPartner where upper(name) = '" + Resource.GetName().ToUpper() + "'";
                        if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) == 0)
                        {
                            bp.SetVAF_Client_ID(Resource.GetVAF_Client_ID());
                            bp.SetVAF_Org_ID(Resource.GetVAF_Org_ID());
                            bp.SetIsEmployee(true);
                            bp.SetName(Resource.GetName());
                            bp.SetEMail(Resource.GetEMail());
                            bp.SetMobile(Resource.GetMobile());
                            bp.SetVAB_Address_ID(Resource.GetVAB_Address_ID());
                            bp.SetVAB_BPart_Category_ID(Resource.GetVAB_BPart_Category_ID());
                            if (!bp.Save())
                            {
                                return GetRetrievedError(bp, "BusinessPartnerNotSaved");
                            }

                            VAdvantage.Model.MVABBPartLocation bploc = new VAdvantage.Model.MVABBPartLocation(GetCtx(), 0, Get_Trx());
                            bploc.SetVAF_Client_ID(bp.GetVAF_Client_ID());
                            bploc.SetVAF_Org_ID(bp.GetVAF_Org_ID());
                            bploc.SetVAB_BusinessPartner_ID(bp.GetVAB_BusinessPartner_ID());
                            bploc.SetPhone(Resource.GetMobile());
                            bploc.SetVAB_Address_ID(Resource.GetVAB_Address_ID());

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

                        sql = "select count(*) from VAF_UserContact where upper(name) = '" + Resource.GetName().ToUpper() + "'";
                        if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) == 0)
                        {

                            user.SetVAF_Client_ID(Resource.GetVAF_Client_ID());
                            user.SetVAF_Org_ID(Resource.GetVAF_Org_ID());
                            user.SetVAB_BusinessPartner_ID(bp.GetVAB_BusinessPartner_ID());
                            user.SetEMail(Resource.GetEMail());
                            user.SetName(Resource.GetName());
                            user.SetPhone(Resource.GetMobile());
                            user.SetVAB_Address_ID(Resource.GetVAB_Address_ID());
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
                            int VAF_Role_id = Resource.GetVAF_Role_ID();
                            VAdvantage.Model.X_VAF_UserContact_Roles userrole = new VAdvantage.Model.X_VAF_UserContact_Roles(GetCtx(), 0, Get_Trx());
                            userrole.SetVAF_Role_ID(VAF_Role_id);
                            userrole.SetVAF_Client_ID(Resource.GetVAF_Client_ID());
                            userrole.SetVAF_Org_ID(Resource.GetVAF_Org_ID());
                            userrole.SetVAF_UserContact_ID(user.GetVAF_UserContact_ID());

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
                    else if (VAdvantage.Utility.Util.GetValueOfInt(Resource.GetVAB_BusinessPartner_ID()) != 0 && VAdvantage.Utility.Util.GetValueOfInt(Resource.GetVAF_UserContact_ID()) == 0)
                    {
                        sql = "select count(*) from VAF_UserContact where upper(name) = '" + Resource.GetName().ToUpper() + "'";
                        if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) == 0)
                        {
                            user.SetVAF_Client_ID(Resource.GetVAF_Client_ID());
                            user.SetVAF_Org_ID(Resource.GetVAF_Org_ID());
                            user.SetVAB_BusinessPartner_ID(Resource.GetVAB_BusinessPartner_ID());
                            user.SetEMail(Resource.GetEMail());
                            user.SetName(Resource.GetName());
                            user.SetPhone(Resource.GetMobile());
                            user.SetVAB_Address_ID(Resource.GetVAB_Address_ID());
                            user.SetIsLoginUser(true);
                            if (!user.Save())
                            {
                                //log.SaveError("UserNotSaved", "");
                                //return "Error:- User" + "-" + pp.GetValue() + "," + pp.GetName();
                                return GetRetrievedError( user,  "UserNotSaved"); 
                            }
                            //string sqlusr = "select name from VAF_UserContact where VAF_UserContact_id=" + user.GetVAF_UserContact_ID() + "";
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

                            int VAF_Role_id = Resource.GetVAF_Role_ID();
                            VAdvantage.Model.X_VAF_UserContact_Roles userrole = new VAdvantage.Model.X_VAF_UserContact_Roles(GetCtx(), 0, Get_Trx());
                            userrole.SetVAF_Role_ID(VAF_Role_id);
                            userrole.SetVAF_Client_ID(Resource.GetVAF_Client_ID());
                            userrole.SetVAF_Org_ID(Resource.GetVAF_Org_ID());
                            userrole.SetVAF_UserContact_ID(user.GetVAF_UserContact_ID());

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

                    if (VAdvantage.Utility.Util.GetValueOfInt(Resource.GetVAB_BusinessPartner_ID()) == 0 && VAdvantage.Utility.Util.GetValueOfInt(Resource.GetVAF_UserContact_ID()) == 0)
                    {
                        sql = "select count(*) from VAB_BusinessPartner where upper(name) = '" + Resource.GetName().ToUpper() + "'";
                        if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) == 0)
                        {
                            bp.SetVAF_Client_ID(Resource.GetVAF_Client_ID());
                            bp.SetVAF_Org_ID(Resource.GetVAF_Org_ID());
                            bp.SetIsVendor(true);
                            bp.SetName(Resource.GetName());
                            bp.SetEMail(Resource.GetEMail());
                            bp.SetMobile(Resource.GetMobile());
                            bp.SetVAB_Address_ID(Resource.GetVAB_Address_ID());
                            //if (VAdvantage.Utility.Util.GetValueOfInt(Resource.GetVAB_BPart_Category_ID()) == 0)
                            // {
                            //  return Msg.GetMsg(GetCtx(), "plzselectresource");
                            // }
                            // else
                            // {
                            bp.SetVAB_BPart_Category_ID(Resource.GetVAB_BPart_Category_ID());
                            // }
                            if (!bp.Save())
                            {
                                //log.SaveError("BusinessPartnerNotSaved", "");
                                //return "Error:- BusinessPartner" + "-" + pp.GetValue() + "," + pp.GetName();
                                return GetRetrievedError( bp, "BusinessPartnerNotSaved"); 
                            }

                            VAdvantage.Model.MVABBPartLocation bploc = new VAdvantage.Model.MVABBPartLocation(GetCtx(), 0, Get_Trx());
                            bploc.SetVAF_Client_ID(bp.GetVAF_Client_ID());
                            bploc.SetVAF_Org_ID(bp.GetVAF_Org_ID());
                            bploc.SetVAB_BusinessPartner_ID(bp.GetVAB_BusinessPartner_ID());
                            bploc.SetPhone(Resource.GetMobile());

                            // if (VAdvantage.Utility.Util.GetValueOfInt(Resource.GetVAB_Address_ID()) == 0)
                            // {
                            //     return Msg.GetMsg(GetCtx(), "Plz enter Address");
                            // }
                            // else
                            //{
                            bploc.SetVAB_Address_ID(Resource.GetVAB_Address_ID());
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

                        sql = "select count(*) from VAF_UserContact where upper(name) = '" + Resource.GetName().ToUpper() + "'";
                        if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) == 0)
                        {

                            user.SetVAF_Client_ID(Resource.GetVAF_Client_ID());
                            user.SetVAF_Org_ID(Resource.GetVAF_Org_ID());
                            user.SetVAB_BusinessPartner_ID(bp.GetVAB_BusinessPartner_ID());
                            user.SetName(Resource.GetName());
                            user.SetEMail(Resource.GetEMail());
                            user.SetPhone(Resource.GetMobile());
                            user.SetVAB_Address_ID(Resource.GetVAB_Address_ID());
                            user.SetIsLoginUser(true);
                            if (!user.Save())
                            {
                                //log.SaveError("UserNotSaved", "");
                                //return "Error:- User" + "-" + pp.GetValue() + "," + pp.GetName();
                                return GetRetrievedError( user,  "UserNotSaved"); 
                            }
                            //string sqlusr = "select name from VAF_UserContact where VAF_UserContact_id=" + user.GetVAF_UserContact_ID() + "";
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

                            int VAF_Role_id = Resource.GetVAF_Role_ID();
                            VAdvantage.Model.X_VAF_UserContact_Roles userrole = new VAdvantage.Model.X_VAF_UserContact_Roles(GetCtx(), 0, Get_Trx());
                            userrole.SetVAF_Role_ID(VAF_Role_id);
                            userrole.SetVAF_Client_ID(Resource.GetVAF_Client_ID());
                            userrole.SetVAF_Org_ID(Resource.GetVAF_Org_ID());
                            userrole.SetVAF_UserContact_ID(user.GetVAF_UserContact_ID());

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
                    else if (VAdvantage.Utility.Util.GetValueOfInt(Resource.GetVAB_BusinessPartner_ID()) != 0 && VAdvantage.Utility.Util.GetValueOfInt(Resource.GetVAF_UserContact_ID()) == 0)
                    {

                        sql = "select count(*) from VAF_UserContact where upper(name) = '" + Resource.GetName().ToUpper() + "'";
                        if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx())) == 0)
                        {
                            user.SetVAF_Client_ID(Resource.GetVAF_Client_ID());
                            user.SetVAF_Org_ID(Resource.GetVAF_Org_ID());
                            user.SetVAB_BusinessPartner_ID(Resource.GetVAB_BusinessPartner_ID());
                            user.SetEMail(Resource.GetEMail());
                            user.SetName(Resource.GetName());
                            user.SetPhone(Resource.GetMobile());
                            user.SetVAB_Address_ID(Resource.GetVAB_Address_ID());
                            user.SetIsLoginUser(true);
                            if (!user.Save())
                            {
                                //log.SaveError("UserNotSaved", "");
                                //return "Error:- User" + "-" + pp.GetValue() + "," + pp.GetName();
                                return GetRetrievedError( user,  "UserNotSaved"); 
                            }
                            //string sqlusr = "select name from VAF_UserContact where VAF_UserContact_id=" + user.GetVAF_UserContact_ID() + "";
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

                            int VAF_Role_id = Resource.GetVAF_Role_ID();
                            VAdvantage.Model.X_VAF_UserContact_Roles userrole = new VAdvantage.Model.X_VAF_UserContact_Roles(GetCtx(), 0, Get_Trx());
                            userrole.SetVAF_Role_ID(VAF_Role_id);
                            userrole.SetVAF_Client_ID(Resource.GetVAF_Client_ID());
                            userrole.SetVAF_Org_ID(Resource.GetVAF_Org_ID());
                            userrole.SetVAF_UserContact_ID(user.GetVAF_UserContact_ID());

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
                if (Util.GetValueOfInt(bp.GetVAB_BusinessPartner_ID()) != 0)
                {
                    Resource.SetVAB_BusinessPartner_ID(bp.GetVAB_BusinessPartner_ID());
                }
            }
            if (user != null)
            {
                if (Util.GetValueOfInt(user.GetVAF_UserContact_ID()) != 0)
                {
                    Resource.SetVAF_UserContact_ID(user.GetVAF_UserContact_ID());
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
