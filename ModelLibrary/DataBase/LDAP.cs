using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using VAdvantage.Logging;


namespace VAdvantage.DataBase
{
    public class LDAP
    {
        /**	Logger	*/
        private static VLogger log = VLogger.GetVLogger(typeof(LDAP).FullName);

        public static bool Validate(String ldapURL, String domain, String userName, String password, string adminUser, string adminPwd, out string output)
        {
            output = "";
            if (String.IsNullOrEmpty(password))
                return false;
            //LdapConnection
            // ldapURL = @"LDAP://192.168.0.1:389/OU=pfsense,DC=viennasolutions2013,DC=local";
            if (ldapURL.EndsWith("/"))
            {
                ldapURL = ldapURL + domain;
            }
            else
            {
                ldapURL = ldapURL + "/" + domain;
            }

            DirectoryEntry entry = null;
            try
            {

                if (string.IsNullOrEmpty(adminUser))

                {
                    entry = new DirectoryEntry(ldapURL, userName, password, AuthenticationTypes.Secure | AuthenticationTypes.Signing | AuthenticationTypes.Sealing);
                    DirectorySearcher ds = new DirectorySearcher(entry);
                    log.Warning("LDAP INFO: Step 1: " + userName);
                    SearchResult result = ds.FindOne();
                    if (null == result)
                    {
                        log.Warning("Error in LDAP: User Not Found" + userName);
                        output = "UserNotFound";
                        return false;
                    }
                }
                else
                {
                    log.Info("Using LDAP Admin user Credentials");
                    if (VAdvantage.Utility.SecureEngine.IsEncrypted(adminPwd))
                    {
                        adminPwd = VAdvantage.Utility.SecureEngine.Decrypt(adminPwd);
                    }
                    entry = new DirectoryEntry(ldapURL, adminUser, adminPwd, AuthenticationTypes.Secure | AuthenticationTypes.Signing | AuthenticationTypes.Sealing);
                    SearchResult result = null;
                    //Bind to the native AdsObject to force authentication.
                    try
                    {
                        object obj = entry.NativeObject;
                        DirectorySearcher search = new DirectorySearcher(entry);
                        search.Filter = "(SAMAccountName=" + userName + ")";
                        result = search.FindOne();
                        log.Info("LDAP Admin user Credentials and user verified");
                    }
                    catch (Exception e)
                    {
                        log.Severe("Error in LDAP for Admin user. User not found " + userName + " : " + e.Message);
                        output = e.Message;
                       
                        return false;
                    }


                    if (null == result)
                    {
                        log.Warning("Error in LDAP for Admin User: User Not Found: " + userName);
                        output = "UserNotFound";
                       
                        return false;
                    }
                    else if (result != null)
                    {
                        string guidfromAdmin = BitConverter.ToString((byte[])result.Properties["objectguid"][0]);
                        log.Info("LDAP checking user's credentials");
                        entry = new DirectoryEntry(ldapURL, userName, password, AuthenticationTypes.Secure | AuthenticationTypes.Signing | AuthenticationTypes.Sealing);
                        DirectorySearcher search1 = new DirectorySearcher(entry);

                        search1.Filter = "(SAMAccountName=" + userName + ")";
                        result = search1.FindOne();

                        if (null == result)
                        {
                            log.Warning("Error in LDAP: User Not Found" + userName);
                            output = "UserNotFound";
                            return false;
                        }
                        string guid = BitConverter.ToString((byte[])result.Properties["objectguid"][0]);
                        log.Info("LDAP checking user's credentials Worked");
                        if (guidfromAdmin.Equals(guid))
                        {
                            return true;
                        }
                        else
                        {
                            log.Warning("Error in LDAP: User Not Found" + userName);
                            output = "UserNotFound";
                            return false;
                        }

                    }
                }



            }
            catch (DirectoryServicesCOMException exception)
            {
                log.Severe("Error in LDAP for user " + userName + " : " + exception.Message);
                output = LDAPExceptions.TreatErrorMessage(exception);
                return false;
            }
            catch (Exception e)
            {
                log.Severe("Error in LDAP for user " + userName + " : " + e.Message);
                output = e.Message;
                return false;
            }
            if (log.IsLoggable(Level.INFO)) log.Info("OK: " + userName);
            return true;

        }	//	validate


    }


    public class LDAPExceptions
    {
        public static string TreatErrorMessage(DirectoryServicesCOMException e)
        {
            /** http://www-01.ibm.com/support/docview.wss?uid=swg21290631
             * 525 - user not found
             * 52e - invalid credentials
             * 530 - not permitted to logon at this time
             * 531 - not permitted to logon at this workstation
             * 532 - password expired
             * 533 - account disabled
             * 534 - The user has not been granted the requested logon type at this machine
             * 701 - account expired
             * 773 - user must reset password
             * 775 - user account locked */

            string msg = e.ExtendedErrorMessage ?? "";
            if (msg.Contains("525"))
            {
                return "UserNotfound";
            }
            if (msg.Contains("52e"))
            {
                return "invalid credentials";
            }
            if (msg.Contains("530"))
            {
                return "not permitted to logon at this time";
            }
            if (msg.Contains("531"))
            {
                return "not permitted to logon at this workstation";
            }
            if (msg.Contains("532"))
            {
                return "password expired";
            }
            if (msg.Contains("533"))
            {
                return "account disabled";
            }
            if (msg.Contains("534"))
            {
                return "The user has not been granted the requested logon type at this machine";
            }
            if (msg.Contains("701"))
            {
                return "account expired";
            }
            if (msg.Contains("773"))
            {
                return "user must reset password";
            }
            if (msg.Contains("775"))
            {
                return "user account locked";
            }
            return "";
        }
    }
}
