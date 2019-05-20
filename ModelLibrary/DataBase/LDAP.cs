using System;
using System.Collections.Generic;
using System.DirectoryServices;
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

        public static bool Validate(String ldapURL, String domain, String userName, String password)
        {

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
            DirectoryEntry entry = new DirectoryEntry(ldapURL, userName, password);
            try
            {
                DirectorySearcher ds = new DirectorySearcher(entry);
                ds.Filter = "(&(objectClass=user)(cn=" + userName + "))";
                SearchResult result = ds.FindOne();
                if (null == result)
                {
                    log.Warning("Error in LDAP: User Not Found" + userName);
                    return false;
                }
            }
            catch (AuthenticationException e)
            {
                log.Severe("Authentication Error in LDAP for user " + userName + " : " + e.Message);
                return false;
            }
            catch (Exception e)
            {
                log.Severe("Error in LDAP for user " + userName + " : " + e.Message);
                return false;
            }
            if (log.IsLoggable(Level.INFO)) log.Info("OK: " + userName);
            return true;

        }	//	validate


    }
}
