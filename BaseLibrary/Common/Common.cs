using SecureEngineUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VAdvantage.DataBase;
using VAdvantage.Utility;

namespace BaseLibrary.Common
{
    public class Common
    {

        public static string Password_Valid_Upto_Key = "PASSWORD_VALID_UPTO";
        public static string Failed_Login_Count_Key = "FAILED_LOGIN_COUNT";

        public static int GetPassword_Valid_Upto
        {
            get { return 3; }
        }
        public static int GetFailed_Login_Count
        {
            get { return 5; }
        }

        /// <summary>
        ///  If validity is unknown but context  available, then get from context
        ///  if validity and context, both are unknown, the go with static values
        ///  Otherwise supply password validity
        /// </summary>
        /// <param name="newPwd"></param>
        /// <param name="AD_User_ID"></param>
        /// <param name="UpdatedBy"></param>
        /// <param name="passwordValidity"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static bool UpdatePasswordAndValidity(string newPwd, int AD_User_ID, int UpdatedBy, int passwordValidity = -1, Ctx ctx = null)
        {
            //If validity is unknow but context  available, then get from context
            if (passwordValidity == -1 && ctx != null)
            {
                passwordValidity = ctx.GetContextAsInt("#" + Common.Password_Valid_Upto_Key);
            }

            else if (passwordValidity == -1 && ctx == null)// if validity and context, both are unknown, the go with static values
            {
                passwordValidity = GetPassword_Valid_Upto;
            }
            //ELSE
            // Password validity is supllied.
            //


            //Check if User's pwd is to be encrypted or not
            if (DB.ExecuteScalar("SELECT IsEncrypted from AD_Column WHERE AD_Column_ID=" + 417).ToString().Equals("Y"))
                newPwd = VAdvantage.Utility.SecureEngine.Encrypt(newPwd);

            string newpwdExpireDate = GlobalVariable.TO_DATE(DateTime.Now.AddMonths(passwordValidity), true);

            string sql = "UPDATE AD_User set Updated=Sysdate,UpdatedBy=" + UpdatedBy + ",PasswordExpireOn=" + newpwdExpireDate + ",password='" + newPwd + "' WHERE AD_User_ID=" + AD_User_ID;
            int count = DB.ExecuteQuery(sql);
            if (count > 0)
                return true;
            return false;

        }
        /// <summary>
        /// Validate Password to check if meet all requirements.
        /// if saving new passwqord, then oldpassword value must be null in parameters.
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="NewPassword"></param>
        /// <param name="ConfirmNewPasseword"></param>
        /// <returns></returns>
        public static string ValidatePassword(string oldPassword, string NewPassword, string ConfirmNewPasseword)
        {
            if (NewPassword == null)
            {
                return "mustMatchCriteria";
            }
            if (NewPassword != null && !NewPassword.Equals(ConfirmNewPasseword))
            {
                return "BothPwdNotMatch";
            }
            if (oldPassword != null && oldPassword.Equals(NewPassword))
            {
                return "oldNewSamePwd";
            }
            string regex = @"(^(?=.*?[a-z])(?=.*?[A-Z])(?=.*?[0-9])(?=.*[@$!%*?&])[a-zA-Z][A-Za-z\d@$!%*?& ]{4,}$)";// Start with Alphabet, minimum 4 length
                                                                                                                    //@$!%*#?& allowed only
            Regex re = new Regex(regex);

            // The IsMatch method is used to validate 
            // a string or to ensure that a string 
            // conforms to a particular pattern. 
            if (!re.IsMatch(NewPassword))
            {
                return "mustMatchCriteria";
            }

            return "";
        }

    }
}