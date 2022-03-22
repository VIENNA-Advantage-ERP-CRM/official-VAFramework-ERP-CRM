/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MLdapProcessor
 * Purpose        : MLdap Processor Model
 * Class Used     : X_AD_LdapProcessor, ViennaProcessor
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
    public class MLdapProcessor : X_AD_LdapProcessor, ViennaProcessor
    {
        /// <summary>
        ///	Get Active LDAP Server
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <returns>array of Servers</returns>
        public static MLdapProcessor[] GetActive(Ctx ctx)
        {
            List<MLdapProcessor> list = new List<MLdapProcessor>();
            String sql = "SELECT * FROM AD_LdapProcessor WHERE IsActive='Y'";
            IDataReader idr = null;
            DataTable dt = null;
            try
            {
                //pstmt = DataBase.prepareStatement (sql, null);
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MLdapProcessor(ctx, dr, null));
                }
                dt = null;
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                if (dt != null)
                {
                    dt = null;
                }
                _log.Log(Level.SEVERE, sql, e);
            }

            MLdapProcessor[] retValue = new MLdapProcessor[list.Count];
            retValue = list.ToArray();
            return retValue;
        }	//	getActive

        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MLdapProcessor).FullName);//.class);

        /// <summary>
        /// Ldap Processor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_LdapProcessor_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MLdapProcessor(Ctx ctx, int AD_LdapProcessor_ID, Trx trxName)
            : base(ctx, AD_LdapProcessor_ID, trxName)
        {

        }	//	MLdapProcessor

        /// <summary>
        /// Ldap Processor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">transaction</param>
        public MLdapProcessor(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }	//	MLdapProcessor

        /** Array of Clients		*/
        private MClient[] _clients = null;
        /** Array of Interest Areas	*/
        private MInterestArea[] _interests = null;

        private int _auth = 0;
        private int _ok = 0;
        private int _error = 0;

        /// <summary>
        /// Get Server ID
        /// </summary>
        /// <returns>id</returns>
        public String GetServerID()
        {
            return "Ldap" + Get_ID();
        }	//	getServerID

        /// <summary>
        /// Get Info
        /// </summary>
        /// <returns>info</returns>
        public String GetInfo()
        {
            return "Auth=" + _auth
                + ", OK=" + _ok + ", Error=" + _error;
        }	//	getInfo

        /// <summary>
        /// Get Date Next Run
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>date next run</returns>
        public DateTime? GetDateNextRun(bool requery)
        {
            if (requery)
            {
                Load(Get_TrxName());
            }
            return GetDateNextRun();
        }	//	getDateNextRun

        /// <summary>
        /// Get Logs
        /// </summary>
        /// <returns>logs</returns>
        public ViennaProcessorLog[] GetLogs()
        {
            List<MLdapProcessorLog> list = new List<MLdapProcessorLog>();
            String sql = "SELECT * "
                + "FROM AD_LdapProcessorLog "
                + "WHERE AD_LdapProcessor_ID=@param "
                + "ORDER BY Created DESC";
            SqlParameter[] param = new SqlParameter[1];
            IDataReader idr = null;
            try
            {
                //pstmt = DataBase.prepareStatement (sql, get_TrxName());
                //pstmt.setInt (1, getAD_LdapProcessor_ID());
                param[0] = new SqlParameter("@Param", GetAD_LdapProcessor_ID());
                idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
                while (idr.Read())
                {
                    list.Add(new MLdapProcessorLog(GetCtx(), idr, Get_TrxName()));
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }

            MLdapProcessorLog[] retValue = new MLdapProcessorLog[list.Count];
            retValue = list.ToArray();
            return retValue;
        }	//	getLogs

        /// <summary>
        /// Delete old Request Log
        /// </summary>
        /// <returns>number of records</returns>
        public int DeleteLog()
        {
            if (GetKeepLogDays() < 1)
                return 0;
            String sql = "DELETE FROM AD_LdapProcessorLog "
                + "WHERE AD_LdapProcessor_ID=" + GetAD_LdapProcessor_ID()
                //jz + " AND (Created+" + getKeepLogDays() + ") < SysDate";
                + " AND addDays(Created," + GetKeepLogDays() + ") < SysDate";
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            return no;
        }	//	deleteLog

        /// <summary>
        /// Get Frequency (n/a)
        /// </summary>
        /// <returns>1</returns>
        public int GetFrequency()
        {
            return 1;
        }	//	getFrequency

        /// <summary>
        /// Get Frequency Type (n/a)
        /// </summary>
        /// <returns>minute</returns>
        public String GetFrequencyType()
        {
            return X_R_RequestProcessor.FREQUENCYTYPE_Minute;
        }	//	getFrequencyType

        /// <summary>
        /// Get AD_Schedule_ID
        /// </summary>
        /// <returns>0</returns>
        public int GetAD_Schedule_ID()
        {
            return 0;
        }	//	getAD_Schedule_ID

        /// <summary>
        ///	String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MLdapProcessor[");
            sb.Append(Get_ID()).Append("-").Append(GetName())
                .Append(",Port=").Append(GetLdapPort())
                .Append("]");
            return sb.ToString();
        }	//	toString


        /// <summary>
        /// Authenticate and Authorize
        /// </summary>
        /// <param name="ldapUser"> MLdapUser object</param>
        /// <param name="usr">user name</param>
        /// <param name="o"> organization = Client Name</param>
        /// <param name="ou">ou optional organization unit = Interest Group Value 
        //or Aa<M_Product_ID>aA = Active Asset of Product of user</param>
        /// <param name="remoteHost">remote host name</param>
        /// <param name="remoteAddr">remote host ip address</param>
        /// <returns>MLdapUser with updated information</returns>
        public MLdapUser Authenticate(MLdapUser ldapUser, String usr, String o, String ou,
                    String remoteHost, String remoteAddr)
        {
            // Ensure something to return
            if (ldapUser == null)
            {
                ldapUser = new MLdapUser();
            }

            String error = null;
            String info = null;

            //	User
            if (usr == null || usr.Trim().Length == 0)
            {
                error = "@NotFound@ User";
                ldapUser.SetErrorString(error);
                _error++;
                log.Warning(error);
                return ldapUser;
            }
            usr = usr.Trim();
            //	Client
            if (o == null || o.Length == 0)
            {
                error = "@NotFound@ O";
                ldapUser.SetErrorString(error);
                _error++;
                log.Warning(error);
                return ldapUser;
            }
            int AD_Client_ID = FindClient(o);
            if (AD_Client_ID == 0)
            {
                error = "@NotFound@ O=" + o;
                ldapUser.SetErrorString(error);
                _error++;
                log.Config(error);
                return ldapUser;
            }
            //	Optional Interest Area or Asset
            int R_InterestArea_ID = 0;
            int M_Product_ID = 0;	//	Product of Asset
            if (ou != null && ou.Length > 0)
            {
                if (ou.StartsWith("Aa") && ou.EndsWith("aA"))
                {
                    try
                    {
                        String s = ou.Substring(2, ou.Length - 2);
                        M_Product_ID = Utility.Util.GetValueOfInt(s);
                    }
                    catch 
                    {
                    }
                }
                else
                    R_InterestArea_ID = FindInterestArea(AD_Client_ID, ou);
                if (R_InterestArea_ID == 0 && M_Product_ID == 0)
                {
                    error = "@NotFound@ OU=" + ou;
                    ldapUser.SetErrorString(error);
                    _error++;
                    log.Config(error);
                    return ldapUser;
                }
            }

            _auth++;
            //	Query 1 - Validate User
            int AD_User_ID = 0;
            String Value = null;
            String LdapUser = null;
            String EMail = null;
            String Name = null;
            String Password = null;
            bool isActive = false;
            String EMailVerify = null;	//	 is timestamp
            bool isUnique = false;
            //
            String sql = "SELECT AD_User_ID, Value, LdapUser, EMail,"	//	1..4
                + " Name, Password, IsActive, EMailVerify "
                + "FROM AD_User "
                + "WHERE AD_Client_ID=@param1 AND (EMail=@param2 OR Value=@param3 OR LdapUser=@param4)";
            IDataReader idr = null;
            SqlParameter[] param = new SqlParameter[4];
            try
            {
                //pstmt = DataBase.prepareStatement (sql, null);
                //pstmt.setInt (1, AD_Client_ID);
                param[0] = new SqlParameter("@param1", AD_Client_ID);
                //pstmt.setString (2, usr);
                param[1] = new SqlParameter("@param2", usr);
                //pstmt.setString (3, usr);
                param[2] = new SqlParameter("@param3", usr);
                //pstmt.setString (4, usr);
                param[3] = new SqlParameter("@param4", usr);
                idr = DataBase.DB.ExecuteReader(sql, param, null);
                if (idr.Read())
                {
                    AD_User_ID = Utility.Util.GetValueOfInt(idr[0]);// rs.getInt(1);
                    Value = Utility.Util.GetValueOfString(idr[1]);// rs.getString(2);
                    LdapUser = Utility.Util.GetValueOfString(idr[2]); //rs.getString(3);
                    EMail = Utility.Util.GetValueOfString(idr[3]); //rs.getString(4);
                    //
                    Name = Utility.Util.GetValueOfString(idr[4]);// rs.getString(5);
                    Password = Utility.Util.GetValueOfString(idr[5]); //rs.getString(6);
                    isActive = "Y".Equals(Utility.Util.GetValueOfString(idr[6]));
                    EMailVerify = Utility.Util.GetValueOfString(idr[7]);
                    isUnique = idr.NextResult();//  rs.next();
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }

                log.Log(Level.SEVERE, sql, e);
                error = "System Error";
            }
            if (error != null)
            {
                _error++;
                ldapUser.SetErrorString(error);
                return ldapUser;
            }
            //
            if (AD_User_ID == 0)
            {
                error = "@NotFound@ User=" + usr;
                info = "User not found - " + usr;
            }
            else if (!isActive)
            {
                error = "@NotFound@ User=" + usr;
                info = "User not active - " + usr;
            }
            else if (EMailVerify == null)
            {
                error = "@UserNotVerified@ User=" + usr;
                info = "User EMail not verified - " + usr;
            }
            else if (usr.ToLower().Equals(LdapUser.ToLower()))
            {
                info = "User verified - Ldap=" + usr
                    + (isUnique ? "" : " - Not Unique");
            }
            else if (usr.ToLower().Equals(Value.ToLower()))// usr.equalsIgnoreCase(Value))
                info = "User verified - Value=" + usr
                    + (isUnique ? "" : " - Not Unique");
            else if (usr.ToLower().Equals(EMail.ToLower()))//usr.equalsIgnoreCase(EMail))
                info = "User verified - EMail=" + usr
                    + (isUnique ? "" : " - Not Unique");
            else
                info = "User verified ?? " + usr
                    + " - Name=" + Name
                    + ", Ldap=" + LdapUser + ", Value=" + Value
                    + (isUnique ? "" : " - Not Unique");

            //	Error
            if (error != null)	//	should use Language of the User
            {
                LogAccess(AD_Client_ID, AD_User_ID, R_InterestArea_ID, 0, info, error,
                            remoteHost, remoteAddr);
                ldapUser.SetErrorString(Msg.Translate(GetCtx(), error));
                return ldapUser;
            }
            //	User Info
            ldapUser.SetOrg(o);
            ldapUser.SetOrgUnit(ou);
            ldapUser.SetUserId(usr);
            ldapUser.SetPassword(Password);
            //	Done
            if (R_InterestArea_ID == 0 && M_Product_ID == 0)
            {
                LogAccess(AD_Client_ID, AD_User_ID, 0, 0, info, null,
                            remoteHost, remoteAddr);
                return ldapUser;
            }

            if (M_Product_ID != 0)
                return AuthenticateAsset(ldapUser,
                        AD_User_ID, usr, M_Product_ID,
                        AD_Client_ID, remoteHost, remoteAddr);

            return AuthenticateSubscription(ldapUser,
                    AD_User_ID, usr, R_InterestArea_ID,
                    AD_Client_ID, remoteHost, remoteAddr);
        }	//	authenticate

        /// <summary>
        /// Authenticate Subscription
        /// </summary>
        /// <param name="ldapUser">user</param>
        /// <param name="AD_User_ID">id</param>
        /// <param name="usr">user authentification (email, ...)</param>
        /// <param name="R_InterestArea_ID">interested area</param>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="remoteHost">remote info</param>
        /// <param name="remoteAddr">remote info</param>
        /// <returns> user with error message set if error</returns>
        private MLdapUser AuthenticateSubscription(MLdapUser ldapUser,
                int AD_User_ID, String usr, int R_InterestArea_ID,
                int AD_Client_ID, String remoteHost, String remoteAddr)
        {
            String error = null;
            String info = null;

            //	Query 2 - Validate Subscription
            String OptOutDate = null;
            bool found = false;
            bool isActive = false;
            String sql = "SELECT IsActive, OptOutDate "
                + "FROM R_ContactInterest "
                + "WHERE R_InterestArea_ID=@param1 AND AD_User_ID=@param2";
            IDataReader idr = null;
            SqlParameter[] param = new SqlParameter[2];
            try
            {
                //pstmt = DataBase.prepareStatement (sql, null);
                //pstmt.setInt (1, R_InterestArea_ID);
                param[0] = new SqlParameter("@param1", R_InterestArea_ID);
                //pstmt.setInt (2, AD_User_ID);
                param[1] = new SqlParameter("@param2", AD_User_ID);
                idr = DataBase.DB.ExecuteReader(sql, param, null);
                if (idr.Read())
                {
                    found = true;
                    isActive = "Y".Equals(Utility.Util.GetValueOfString(idr[0]));//    rs.getString (1));
                    OptOutDate = Utility.Util.GetValueOfString(idr[1]);// rs.getString(2);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
                error = "System Error (2)";
            }

            //	System Error
            if (error != null)
            {
                _error++;
                ldapUser.SetErrorString(error);
                return ldapUser;
            }

            if (!found)
            {
                error = "@UserNotSubscribed@ User=" + usr;
                info = "No User Interest - " + usr
                    + " - R_InterestArea_ID=" + R_InterestArea_ID;
            }
            else if (OptOutDate != null)
            {
                error = "@UserNotSubscribed@ User=" + usr + " @OptOutDate@=" + OptOutDate;
                info = "Opted out - " + usr + " - OptOutDate=" + OptOutDate;
            }
            else if (!isActive)
            {
                error = "@UserNotSubscribed@ User=" + usr;
                info = "User Interest Not Active - " + usr;
            }
            else
                info = "User subscribed - " + usr;


            if (error != null)	//	should use Language of the User
            {
                LogAccess(AD_Client_ID, AD_User_ID, R_InterestArea_ID, 0, info, error,
                            remoteHost, remoteAddr);
                ldapUser.SetErrorString(Msg.Translate(GetCtx(), error));
                return ldapUser;
            }
            //	Done
            LogAccess(AD_Client_ID, AD_User_ID, R_InterestArea_ID, 0, info, null,
                        remoteHost, remoteAddr);
            return ldapUser;
        }	//	authenticateSubscription

        /// <summary>
        /// Authenticate Product Asset
        /// </summary>
        /// <param name="ldapUser">user</param>
        /// <param name="AD_User_ID">id</param>
        /// <param name="usr">user authentification (email, ...)</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="remoteHost">remote info</param>
        /// <param name="remoteAddr">remote info</param>
        /// <returns>user with error message set if error</returns>
        private MLdapUser AuthenticateAsset(MLdapUser ldapUser,
                int AD_User_ID, String usr, int M_Product_ID,
                int AD_Client_ID, String remoteHost, String remoteAddr)
        {
            String error = null;
            String info = null;

            //	Query 2 - Validate Asset
            MAsset asset = null;
            String sql = "SELECT * "
                + "FROM A_Asset "
                + "WHERE M_Product_ID=@param1"
                + " AND AD_User_ID=@param2";		//	only specific user
            //	Will have problems with multiple assets
            SqlParameter[] param = new SqlParameter[2];
            IDataReader idr = null;
            try
            {
                //pstmt = DataBase.prepareStatement (sql, null);
                //pstmt.setInt (1, M_Product_ID);
                param[0] = new SqlParameter("@param1", M_Product_ID);
                //pstmt.setInt (2, AD_User_ID);
                param[1] = new SqlParameter("@param2", AD_User_ID);
                idr = DataBase.DB.ExecuteReader(sql, param, null);
                if (idr.Read())
                {
                    asset = new MAsset(GetCtx(), idr, null);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
                error = "System Error (3)";
            }

            //	System Error
            if (error != null)
            {
                _error++;
                ldapUser.SetErrorString(error);
                return ldapUser;
            }
            int A_Asset_ID = 0;
            if (asset == null)
            {
                error = "@UserNoAsset@ User=" + usr;
                info = "No Asset - " + usr + " - " + M_Product_ID;
            }
            else if (!asset.IsActive())
            {
                A_Asset_ID = asset.GetA_Asset_ID();
                error = "@UserNoAsset@ User=" + usr;
                info = "Asset not active - " + usr;
            }
            else if (!asset.IsActive(true))
            {
                A_Asset_ID = asset.GetA_Asset_ID();
                error = "@UserNoAsset@ User=" + usr + " @GuaranteeDate@=" + asset.GetGuaranteeDate();
                info = "Expired - " + usr + " - GuaranteeDate=" + asset.GetGuaranteeDate();
            }
            else
            {
                info = "Asset - " + usr;
            }

            if (error != null)	//	should use Language of the User
            {
                LogAccess(AD_Client_ID, AD_User_ID, 0, A_Asset_ID, info, error,
                            remoteHost, remoteAddr);
                ldapUser.SetErrorString(Msg.Translate(GetCtx(), error));
                return ldapUser;
            }
            //	Done OK
            MLdapAccess log = LogAccess(AD_Client_ID, AD_User_ID, 0, asset.GetA_Asset_ID(), info, null,
                        remoteHost, remoteAddr);
            MAssetDelivery ad = new MAssetDelivery(asset, null, log.ToString(), AD_User_ID);
            ad.SetRemote_Host(remoteHost);
            ad.SetRemote_Addr(remoteAddr);
            ad.Save();
            return ldapUser;
        }	//	authenticateAsset


        /// <summary>
        /// Find Client
        /// </summary>
        /// <param name="client">client name</param>
        /// <returns>AD_Client_ID</returns>
        private int FindClient(String client)
        {
            if (_clients == null)
            {
                _clients = MClient.GetAll(GetCtx());
            }
            for (int i = 0; i < _clients.Length; i++)
            {
                if (client.ToLower().Equals(_clients[i].GetValue().ToLower()))// client.equalsIgnoreCase (_clients[i].GetValue())))
                    return _clients[i].GetAD_Client_ID();
            }
            return 0;
        }	//	findClient

        /// <summary>
        /// Find Interest Area
        /// </summary>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="interestArea">interset Name client name</param>
        /// <returns>AD_Client_ID</returns>
        private int FindInterestArea(int AD_Client_ID, String interestArea)
        {
            if (_interests == null)
            {
                _interests = MInterestArea.GetAll(GetCtx());
            }
            for (int i = 0; i < _interests.Length; i++)
            {
                if (AD_Client_ID == _interests[i].GetAD_Client_ID()
                    && interestArea.ToLower().Equals(_interests[i].GetValue().ToLower()))  //  interestArea.equalsIgnoreCase(_interests[i].GetValue()))
                {
                    return _interests[i].GetR_InterestArea_ID();
                }
            }
            return 0;
        }	//	findInterestArea

        /// <summary>
        /// Log Access
        /// </summary>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_User_ID">user</param>
        /// <param name="R_InterestArea_ID">interested area</param>
        /// <param name="A_Asset_ID">id</param>
        /// <param name="info">info</param>
        /// <param name="error">error</param>
        /// <param name="remoteHost">remote info</param>
        /// <param name="remoteAddr">remote info</param>
        /// <returns></returns>
        private MLdapAccess LogAccess(int AD_Client_ID,
            int AD_User_ID, int R_InterestArea_ID, int A_Asset_ID,
            String info, String error,
            String remoteHost, String remoteAddr)
        {
            if (error != null)
            {
                log.Log(Level.CONFIG, info);
                _error++;
            }
            else
            {
                log.Log(Level.INFO, info);
                _ok++;
            }
            //
            MLdapAccess access = new MLdapAccess(GetCtx(), 0, null);
            access.SetAD_Client_ID(AD_Client_ID);
            access.SetAD_Org_ID(0);
            access.SetAD_LdapProcessor_ID(GetAD_LdapProcessor_ID());
            access.SetAD_User_ID(AD_User_ID);
            access.SetR_InterestArea_ID(R_InterestArea_ID);
            access.SetA_Asset_ID(A_Asset_ID);
            access.SetRemote_Host(remoteHost);
            access.SetRemote_Addr(remoteAddr);

            access.SetIsError(error != null);
            access.SetSummary(info);
            access.Save();
            return access;
        }	//	logAccess

    }	//	MLdapProcessor

}
