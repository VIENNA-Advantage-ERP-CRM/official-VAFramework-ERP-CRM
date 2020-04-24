/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : UserPassword
    * Purpose        : Reset Password
    * Class Used     : SvrProcess
    * Chronological    Development
    * Karan            24-May-2011
******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Common;
using VAdvantage.DataBase;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.ProcessEngine;
using System.Reflection;
using VAdvantage.Logging;


namespace VAdvantage.Process
{
    public class UserPassword : SvrProcess
    {
        private int p_AD_User_ID = -1;
        private String p_OldPassword = null;
        private String p_CurrentPassword = null;
        private String p_NewPassword = null;
        private String p_NewEMail = null;
        private String p_NewEMailUser = null;
        private String p_NewEMailUserPW = null;

        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i <= para.Length - 1; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                }
                else if (name.Equals("AD_User_ID"))
                    p_AD_User_ID = para[i].GetParameterAsInt();
                else if (name.Equals("OldPassword"))
                    p_OldPassword = para[i].GetParameter().ToString();
                else if (name.Equals("CurrentPassword"))
                    p_CurrentPassword = para[i].GetParameter().ToString();
                else if (name.Equals("NewPassword"))
                    p_NewPassword = para[i].GetParameter().ToString();
                else if (name.Equals("NewEMail"))
                    p_NewEMail = para[i].GetParameter().ToString();
                else if (name.Equals("NewEMailUser"))
                    p_NewEMailUser = para[i].GetParameter().ToString();
                else if (name.Equals("NewEMailUserPW"))
                    p_NewEMailUserPW = para[i].GetParameter().ToString();

            }
        }

        protected override string DoIt()
        {
            VLogger log = VLogger.GetVLogger(this.GetType().FullName);
            log.Log(Level.SEVERE, "UserPassword Change Log=>" + Convert.ToString(p_AD_User_ID));
            if (p_AD_User_ID == -1)
                p_AD_User_ID = GetAD_User_ID();

            MUser user = MUser.Get(GetCtx(), p_AD_User_ID);
            MUser current = MUser.Get(GetCtx(), GetAD_User_ID());


            if (!current.IsAdministrator() && p_AD_User_ID != GetAD_User_ID() && user.HasRole())
                throw new ArgumentException("@UserCannotUpdate@");

            // SuperUser and System passwords can only be updated by themselves
            if (user.IsSystemAdministrator() && p_AD_User_ID != GetAD_User_ID())
                throw new ArgumentException("@UserCannotUpdate@");

            log.Log(Level.SEVERE, "UserPassword Change Log Step Check for valid user=>" + Convert.ToString(p_AD_User_ID));
            if (string.IsNullOrEmpty(p_CurrentPassword))
            {
                if (string.IsNullOrEmpty(p_OldPassword))
                    throw new ArgumentException("@OldPasswordMandatory@");
                else if (!p_OldPassword.Equals(user.GetPassword()))
                {
                    if (!SecureEngine.Encrypt(p_OldPassword).Equals(user.GetPassword()))
                    {
                        throw new ArgumentException("@OldPasswordNoMatch@");
                    }
                }
            }

            else if (!p_CurrentPassword.Equals(current.GetPassword()))
                throw new ArgumentException("@OldPasswordNoMatch@");

            string validatePwd = Common.Common.ValidatePassword(null, p_NewPassword, p_NewPassword);
            if(validatePwd.Length>0)
                throw new ArgumentException("validatePwd");

            log.Log(Level.SEVERE, "UserPassword Change Log Step Password Change=>" + Convert.ToString(p_AD_User_ID));
            String originalPwd = p_NewPassword;

            String sql = "UPDATE AD_User SET Updated=SYSDATE,FailedloginCount=0, UpdatedBy=" + GetAD_User_ID();

            int passwordValidity = GetCtx().GetContextAsInt(Common.Common.Password_Valid_Upto_Key);

            Common.Common.UpdatePasswordAndValidity(p_NewPassword, p_AD_User_ID, GetAD_User_ID(), -1, GetCtx());

            if (!string.IsNullOrEmpty(p_NewPassword))
            {
                MColumn column = MColumn.Get(GetCtx(), 417); // Password Column 
                if (column.IsEncrypted())
                    p_NewPassword = SecureEngine.Encrypt(p_NewPassword);
                sql += ", Password=" + GlobalVariable.TO_STRING(p_NewPassword);
            }
            if (!string.IsNullOrEmpty(p_NewEMail))
                sql += ", Email=" + GlobalVariable.TO_STRING(p_NewEMail);
            if (!string.IsNullOrEmpty(p_NewEMailUser))
                sql += ", EmailUser=" + GlobalVariable.TO_STRING(p_NewEMailUser);
            if (!string.IsNullOrEmpty(p_NewEMailUserPW))
                sql += ", EmailUserPW=" + GlobalVariable.TO_STRING(p_NewEMailUserPW);
            sql += " WHERE AD_User_ID=" + p_AD_User_ID;
            log.Log(Level.SEVERE, "UserPassword Change Log=>" + sql);
            int iRes = DB.ExecuteQuery(sql, null, Get_Trx());
            if (iRes > 0)
            {
                bool error = false;
                //Check for yellowFin user password change if BI user is true..................
                object ModuleId = DB.ExecuteScalar("select ad_moduleinfo_id from ad_moduleinfo where prefix='VA037_' and IsActive = 'Y'"); // is active check by vinay bhatt on 18 oct 2018
                if (ModuleId != null && ModuleId != DBNull.Value)
                {
                    if (user.IsVA037_BIUser())
                    {
                        var Dll = Assembly.Load("VA037");
                        var BIUser = Dll.GetType("VA037.BIProcess.BIUsers");
                        var objBIUser = Activator.CreateInstance(BIUser);
                        var ChangeBIPassword = BIUser.GetMethod("ChangeBIPassword");
                        bool value = (bool)ChangeBIPassword.Invoke(objBIUser, new object[] { GetCtx(), GetAD_Client_ID(), Convert.ToString(user.GetVA037_BIUserName()), originalPwd });
                        if (value)
                        {
                            //user.SetPassword(p_NewPassword);
                            error = false;
                            user.SetPassword(originalPwd);
                            //return "OK";
                        }
                        else
                        {
                            error = true;
                            // return "@Error@";
                        }
                    }
                    else
                    {
                        error = false;
                        user.SetPassword(originalPwd);
                        // return "OK";
                    }
                }
                ModuleId = DB.ExecuteScalar("select ad_moduleinfo_id from ad_moduleinfo where prefix='VA039_' and IsActive = 'Y'"); // is active check by vinay bhatt
                if (ModuleId != null && ModuleId != DBNull.Value)
                {
                    MUser obj = new MUser(GetCtx(), p_AD_User_ID, null);
                    if (obj.IsVA039_IsJasperUser() == true)
                    {
                        var Dll = Assembly.Load("VA039");
                        var JasperUser = Dll.GetType("VA039.Classes.Users");
                        var objJasperUser = Activator.CreateInstance(JasperUser);
                        var BICreateUser = JasperUser.GetMethod("ModifyUserPassword");
                        object[] args = new object[] { GetCtx(), originalPwd };
                        bool value = (bool)BICreateUser.Invoke(objJasperUser, args);
                        if (value)
                        {
                            error = false;
                            user.SetPassword(originalPwd);

                            //return "@Error@";
                        }
                        else
                        {
                            error = true;
                            goto PasswordError;
                            // return "OK";
                        }
                    }

                }
                else
                {
                    error = false;
                    user.SetPassword(originalPwd);
                    // return "OK";
                }
            PasswordError:
                if (error)
                {
                    return "@Error@";
                }
                else
                {
                    return "OK";

                }
            }
            else
                return "@Error@";

        }
    }
}
