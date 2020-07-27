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
            catch (AuthenticationException e)
            {
                log.Severe("Authentication Error in LDAP for user " + userName + " : " + e.Message);
                output = e.Message;
                return false;
            }
            catch (Exception e)
            {
                log.Severe("Error in LDAP for user " + userName + " : " + e.Message);
                output = e.Message;

                DirectoryEntry user = new DirectoryEntry(ldapURL, userName, password);
                string attribName = "msDS-User-Account-Control-Computed";
                user.RefreshCache(new string[] { attribName });
                const int UF_LOCKOUT = 0x0010;
                int userFlags = (int)user.Properties[attribName].Value;
                if ((userFlags & UF_LOCKOUT) == UF_LOCKOUT)
                {
                    // if this is the case, the account is locked out
                    return true;
                }
                return false;


                return false;
            }
            if (log.IsLoggable(Level.INFO)) log.Info("OK: " + userName);
            return true;

        }	//	validate


    }
}
