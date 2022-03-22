/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MLdapUser
 * Purpose        : MLdap User Model
 * Class Used     : ....
 * Chronological    Development
 * Deepak           03-Feb-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;
namespace VAdvantage.Model
{
    public class MLdapUser
    {
        /** organization */
        private String org = null;
        /** organization unit */
        private String orgUnit = null;
        /** user password */
        private String passwd = null;
        /** user id */
        private String userId = null;
        /** error string */
        private String errStr = null;

        public MLdapUser()
        {
        }

        /// <summary>
        /// Reset attributes
        /// </summary>
        public void Reset()
        {
            org = null;
            orgUnit = null;
            passwd = null;
            userId = null;
            errStr = null;
        }  // reset()

        /// <summary>
        /// Set the organization
        /// </summary>
        /// <param name="org">org organization</param>
        public void SetOrg(String org)
        {
            this.org = org;
        }  // setOrg()

        /// <summary>
        /// Set the organization unit
        /// </summary>
        /// <param name="orgUnit">organization unit</param>
        public void SetOrgUnit(String orgUnit)
        {
            this.orgUnit = orgUnit;
        }  // setOrg()

        /// <summary>
        /// Set the user password
        /// </summary>
        /// <param name="passwd">User password string</param>
        public void SetPassword(String passwd)
        {
            this.passwd = passwd;
        }  // setPassword()

        /// <summary>
        /// Set the user id
        /// </summary>
        /// <param name="userId">User id string</param>
        public void SetUserId(String userId)
        {
            this.userId = userId;
        }  // setUserId()

        /// <summary>
        /// Set the error String
        /// </summary>
        /// <param name="errStr">Error String</param>
        public void SetErrorString(String errStr)
        {
            this.errStr = errStr;
        }  // setErrorStr()

        /// <summary>
        ///Get the organization
        /// </summary>
        /// <returns>organization</returns>
        public String GetOrg()
        {
            return org;
        }  // getOrg()

        /// <summary>
        /// Get the organization unit
        /// </summary>
        /// <returns>organization unit</returns>
        public String GetOrgUnit()
        {
            return orgUnit;
        }  // getOrgUnit()

        /// <summary>
        /// Get the user password
        /// </summary>
        /// <returns>User password string</returns>
        public String GetPassword()
        {
            return passwd;
        }  // getPassword()

        /// <summary>
        ///Get the user id
        /// </summary>
        /// <returns>User id string</returns>
        public String GetUserId()
        {
            return userId;
        }  // getUserId()

        /// <summary>
        /// Get the error string
        /// </summary>
        /// <returns>Error String</returns>
        public String GetErrorMsg()
        {
            return errStr;
        }  // getErrorString()
    }  // MLdapUser
}
