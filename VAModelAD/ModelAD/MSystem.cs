/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MSystem
 * Purpose        : System Record (just one)
 * Class Used     : ViennaServer
 * Chronological    Development
 * Raghunandan      18-Jan-2010
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using System.Data.SqlClient;

namespace VAdvantage.Model
{
    public class MSystem : X_AD_System
    {
        ////System - cached					
        private static MSystem _system = null;
        /**	Cache					*/
        private static CCache<int, MSystem> cache = new CCache<int, MSystem>("AD_System", 30, 60);

        /// <summary>
        /// Load System Record
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>System</returns>
        public static MSystem Get(Ctx ctx)
        {
            _system = (MSystem)cache[101];

            if (_system == null)
            {

                String sql = "SELECT * FROM AD_System ORDER BY AD_System_ID";	//	0 first
                IDataReader idr = null;
                try
                {
                    idr = DataBase.DB.ExecuteReader(sql, null, null);
                    if (idr.Read())
                    {
                        _system = new MSystem(ctx, idr, null);
                    }
                    idr.Close();
                }
                catch (Exception ex)
                {
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                    //String info = "No System - " + DataBase.getDatabaseInfo() + " - " + ex.getLocalizedMessage();
                    String info = "No System - " + DataBase.DB.GetConnection().ToString() + " - " + ex.Message;
                    //System.err.println(info);
                }
                cache.Add(101, _system);
                if (!Ini.IsClient() && _system.SetInfo())
                {
                    _system.Save();
                }
            }

            return _system;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">id - if < 0 not loaded</param>
        /// <param name="mtrxName">transaction</param>
        public MSystem(Ctx ctx, int ignored, Trx mtrxName)
            : base(ctx, 0, mtrxName)
        {
            Trx trxName = null;
            if (ignored >= 0)
            {
                Load(trxName);	//	load ID=0
            }
            if (_system == null)
            {
                _system = this;
            }
        }

        /// <summary>
        /// 	Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public MSystem(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {
            if (_system == null)
            {
                _system = this;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public MSystem()
            : this(new Ctx(), 0, null)
        {

        }

        /// <summary>
        /// Is LDAP Authentification defined
        /// </summary>
        /// <returns>true if ldap defined</returns>
        public bool IsLDAP()
        {
            String host = GetLDAPHost();
            if (host == null || host.Length == 0)
            {
                return false;
            }
            String domain = GetLDAPDomain();
            return domain != null && domain.Length > 0;
        }

        /// <summary>
        /// 	LDAP Authentification. Assumes that LDAP is defined.
        /// </summary>
        /// <param name="userName">user name</param>
        /// <param name="password">password</param>
        /// <returns>true if ldap authenticated</returns>
        public bool IsLDAP(String userName, String password,out string output)
        {
            output = "";
            return LDAP.Validate(GetLDAPHost(), GetLDAPDomain(), userName, password, GetLDAPAdminUser(), GetLDAPAdminPwd(),out output);
            //return false;
        }

        /// <summary>
        /// Get DB Address
        /// </summary>
        /// <param name="actual"></param>
        /// <returns>address</returns>
        public String GetDBAddress(bool actual)
        {
            String s = base.GetDBAddress();
            if (actual || s == null || s.Length == 0)
            {
                //CConnection cc = CConnection.get();
                IDbConnection cc = DataBase.DB.GetConnection();
                //s = cc.getConnectionURL() + "#" + cc.getDbUid();
                s = cc.ConnectionString;
                s = s.ToLower();
            }
            return s;
        }

        /// <summary>
        /// Get Statistics Info
        /// </summary>
        /// <param name="recalc">recalculate</param>
        /// <returns>statistics</returns>
        public String GetStatisticsInfo(bool recalc)
        {
            String s = base.GetStatisticsInfo();
            if (s == null || recalc)
            {
                String count = DataBase.DB.TO_CHAR("COUNT(*)", DisplayType.Number, Env.GetAD_Language(GetCtx()));
                String sql = "SELECT 'C'||(SELECT " + count + " FROM AD_Client)"
                    + " ||'U'||(SELECT " + count + " FROM AD_User)"
                    + " ||'B'||(SELECT " + count + " FROM C_BPartner)"
                    + " ||'P'||(SELECT " + count + " FROM M_Product)"
                    + " ||'I'||(SELECT " + count + " FROM C_Invoice)"
                    + " ||'L'||(SELECT " + count + " FROM C_InvoiceLine)"
                    + " ||'M'||(SELECT " + count + " FROM M_Transaction)"
                    + " ||'c'||(SELECT " + count + " FROM AD_Column WHERE EntityType NOT IN ('C','D'))"
                    + " ||'t'||(SELECT " + count + " FROM AD_Table WHERE EntityType NOT IN ('C','D'))"
                    + " ||'f'||(SELECT " + count + " FROM AD_Field WHERE EntityType NOT IN ('C','D'))"
                    + " FROM AD_System";
                IDataReader idr = null;
                try
                {
                    idr = DataBase.DB.ExecuteReader(sql, null, null);
                    if (idr.Read())
                    {
                        s = Utility.Util.GetValueOfString(idr[0]);//.getString(1);
                    }
                    idr.Close();
                    SetStatisticsInfo(s);
                }
                catch (Exception e)
                {
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                    log.Log(Level.SEVERE, sql, e);
                }
            }
            return s;
        }

        /// <summary>
        /// Get Profile Info
        /// </summary>
        /// <param name="recalc">recalculate</param>
        /// <returns>profile</returns>
        public String GetProfileInfo(bool recalc)
        {
            String s = base.GetProfileInfo();
            if (s == null || recalc)
            {
                String sql = "SELECT Value FROM AD_Client "
                    + "WHERE IsActive='Y' ORDER BY AD_Client_ID DESC";
                IDataReader idr = null;
                StringBuilder sb = new StringBuilder();
                try
                {
                    idr = DataBase.DB.ExecuteReader(sql, null, null);
                    while (idr.Read())
                    {
                        sb.Append(idr.GetString(0)).Append('|');
                    }
                    idr.Close();
                }
                catch (Exception e)
                {
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                    log.Log(Level.SEVERE, sql, e);
                }

                s = sb.ToString();
            }
            return s;
        }

        /// <summary>
        /// 	Before Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns>true/false</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	Mandatory Values
            //if (Get_Value("IsAutoErrorReport") == null)
            //{
            //    SetIsAutoErrorReport(true);
            //}
            ////
            //bool userChange = Ini.IsClient() &&
            //    (Is_ValueChanged("Name")
            //    || Is_ValueChanged("UserName")
            //    || Is_ValueChanged("Password")
            //    || Is_ValueChanged("LDAPHost")
            //    || Is_ValueChanged("LDAPDomain")
            //    || Is_ValueChanged("CustomPrefix")
            //    );
            //if (userChange)
            //{
            //    String name = GetName();
            //    if (name.Equals("?") || name.Length < 2)
            //    {
            //        log.SaveError("Error", "Define a unique System name (e.g. Company name) not " + name);
            //        return false;
            //    }
            //    if (GetUserName().Equals("?") || GetUserName().Length < 2)
            //    {
            //        log.SaveError("Error", "Use the same EMail address as1 in the Vienna Web Store");
            //        return false;
            //    }
            //    if (GetPassword().Equals("?") || GetPassword().Length < 2)
            //    {
            //        log.SaveError("Error", "Use the same Password as1 in the Vienna Web Store");
            //        return false;
            //    }
            //}
            //if (GetSupportLevel() == null)
            //{
            //    SetSupportLevel(SUPPORTLEVEL_Unsupported);
            //}
            ////
            //SetInfo();
            return true;
        }

        /// <summary>
        /// Save Record (ID=0)
        /// </summary>
        /// <returns>true if saved</returns>
        public override bool Save()
        {
            if (!BeforeSave(false))
            {
                return false;
            }
            return SaveUpdate();
        }

        /// <summary>
        /// 	String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            return "MSystem[" + GetName()
                + ",User=" + GetUserName()
                + ",ReleaseNo=" + GetReleaseNo()
                + "]";
        }


        /// <summary>
        /// Check valididity
        /// </summary>
        /// <returns>true if valid</returns>
        public bool IsValid()
        {
            if (GetName() == null || GetName().Length < 2)
            {
                log.Log(Level.WARNING, "Name not valid: " + GetName());
                return false;
            }
            if (GetPassword() == null || GetPassword().Length < 2)
            {
                log.Log(Level.WARNING, "Password not valid: " + GetPassword());
                return false;
            }
            if (GetInfo() == null || GetInfo().Length < 2)
            {
                log.Log(Level.WARNING, "Need to run Migration once");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Is there a PDF License
        /// </summary>
        /// <returns>true if there is a PDF License</returns>
        public bool IsPDFLicense()
        {
            String key = GetSummary();
            return key != null && key.Length > 25;
        }

        /// <summary>
        /// Get SupportLevel
        /// </summary>
        /// <returns>Support Level</returns>
        public new String GetSupportLevel()
        {
            String sl = null;
            if (Get_ColumnIndex("SupportLevel") != -1)
            {
                sl = base.GetSupportLevel();
            }
            if (sl == null)
            {
                return SUPPORTLEVEL_Unsupported;
            }
            return sl;
        }

        /// <summary>
        /// Get Record_ID
        /// </summary>
        /// <returns>record ID</returns>
        public new int GetRecord_ID()
        {
            if (Get_ColumnIndex("Record_ID") == -1)
            {
                return -1;
            }
            return base.GetRecord_ID();
        }

        /// <summary>
        /// Get SupportUnits
        /// </summary>
        /// <returns>SupportUnits</returns>
        public new int GetSupportUnits()
        {
            if (Get_ColumnIndex("SupportUnits") == -1)
            {
                return 0;
            }
            return base.GetSupportUnits();
        }

        /// <summary>
        /// Get System Status
        /// </summary>
        /// <returns>system status</returns>
        public new String GetSystemStatus()
        {
            String ss = null;
            if (Get_ColumnIndex("SystemStatus") != -1)
            {
                ss = base.GetSystemStatus();
            }
            if (ss == null)
            {
                ss = SYSTEMSTATUS_Evaluation;
            }
            return ss;
        }

        /// <summary>
        /// Set/Derive Info if more then a day old
        /// </summary>
        /// <returns>true if set</returns>
        public bool SetInfo()
        {
            //	log.severe("setInfo");
            if (!(TimeUtil.GetDay(GetUpdated()) < (TimeUtil.GetDay(null))))//before
            {
                return false;
            }
            try
            {
                SetDBInfo();
                SetInternalUsers();
                if (IsAllowStatistics())
                {
                    SetStatisticsInfo(GetStatisticsInfo(true));
                    SetProfileInfo(GetProfileInfo(true));
                }
            }
            catch (Exception e)
            {
                SetSupportUnits(9999);
                SetInfo(e.Message);
                log.Log(Level.SEVERE, "", e);
            }
            return true;
        }

        /// <summary>
        /// Set Internal User Count
        /// </summary>
        private void SetInternalUsers()
        {
            String sql = "SELECT COUNT(DISTINCT (u.AD_User_ID)) AS iu "
                + "FROM AD_User u"
                + " INNER JOIN AD_User_Roles ur ON (u.AD_User_ID=ur.AD_User_ID) "
                + "WHERE u.AD_Client_ID<>11"			//	no Demo
                + " AND u.AD_User_ID NOT IN (0,100)";	//	no System/SuperUser
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                if (idr.Read())
                {
                    int internalUsers = Utility.Util.GetValueOfInt(idr[0]);//.getInt(1);
                    SetSupportUnits(internalUsers);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                log.Log(Level.SEVERE, sql, e);
            }

        }

        /// <summary>
        /// Set DB Info
        /// </summary>
        private void SetDBInfo()
        {
            //if (!DataBase.isRemoteObjects())
            if (DataBase.DB.GetConnection() != null)
            {
                String dbAddress = GetDBAddress(true);
                SetDBAddress(dbAddress);
            }
            //
            if (!Ini.IsClient())
            {
                //int noProcessors = Runtime.getRuntime().availableProcessors();
                // SetNoProcessors(noProcessors);
            }
            //
            try
            {
                //DatabaseMetaData md = DataBase.getConnectionRO().getMetaData();
                String db1 = "";// md.getDatabaseProductName();
                String db2 = "";// md.getDatabaseProductVersion();
                if (db2.StartsWith(db1))
                {
                    db1 = db2;
                }
                else
                {
                    db1 += "-" + db2;
                }
                int fieldLength = p_info.GetFieldLength("DBInstance");
                if (db1.Length > fieldLength)
                {
                    db1 = Utility.Util.Replace(db1, "Database ", "");
                    db1 = Utility.Util.Replace(db1, "Version ", "");
                    db1 = Utility.Util.Replace(db1, "Edition ", "");
                    db1 = Utility.Util.Replace(db1, "Release ", "");
                }
                db1 = Utility.Util.RemoveCRLF(db1);
                if (db1.Length > fieldLength)
                {
                    db1 = db1.Substring(0, fieldLength);
                }
                SetDBInstance(db1);
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "MetaData", e);
            }
        }


        /// <summary>
        /// Print info
        /// </summary>
        public void Info()
        {
            if (!VLogMgt.IsLevelFine())
            {
                return;
            }
            ////	OS
            ////	OperatingSystemMXBean os = ManagementFactory.getOperatingSystemMXBean();
            ////	log.Fine(os.getName() + " " + os.getVersion() + " " + os.getArch() 
            ////		+ " Processors=" + os.getAvailableProcessors());
            ////	Runtime
            //RuntimeMXBean rt = ManagementFactory.getRuntimeMXBean();
            //log.Fine(rt.getName() + " (" + rt.getVmVersion() + ") Up=" + TimeUtil.formatElapsed(rt.getUptime()));
            ////	Memory
            //if (VLogMgt.IsLevelFiner())
            //{
            //    List<MemoryPoolMXBean> list = ManagementFactory.getMemoryPoolMXBeans();
            //    Iterator<MemoryPoolMXBean> it = list.iterator();
            //    while (it.hasNext())
            //    {
            //        MemoryPoolMXBean pool = (MemoryPoolMXBean)it.next();
            //        log.Finer(pool.getName() + " " + pool.getType()
            //            + ": " + new CMemoryUsage(pool.getUsage()));
            //    }
            //}
            //else
            //{
            //    MemoryMXBean memory = ManagementFactory.getMemoryMXBean();
            //    log.Fine("VM: " + new CMemoryUsage(memory.getNonHeapMemoryUsage()));
            //    log.Fine("Heap: " + new CMemoryUsage(memory.getHeapMemoryUsage()));
            //}
            ////	Thread
            //ThreadMXBean th = ManagementFactory.getThreadMXBean();
            //log.Fine("Threads=" + th.getThreadCount()
            //    + ", Peak=" + th.getPeakThreadCount()
            //    + ", Demons=" + th.getDaemonThreadCount()
            //    + ", Total=" + th.getTotalStartedThreadCount()
            //);
        }


        /// <summary>
        /// Test
        /// </summary>
        /// <param name="args"></param>
        //public static void Main(String[] args)
        //{
        //    new MSystem();
        //}

        /// <summary>
        /// Only save the license
        /// </summary>
        /// <returns>true if success; otherwise false</returns>
        public bool SaveLicenseOnly()
        {
            // Create the update statement for license
            StringBuilder sql = new StringBuilder("UPDATE ");
            sql.Append(p_info.GetTableName())
               .Append(" SET Name = '").Append(GetName()).Append("',")
               .Append("UserName = '").Append(GetUserName()).Append("',")
               .Append("Password = '").Append(GetPassword()).Append("',")
               .Append("Summary = '").Append(GetSummary()).Append("'")
               .Append(" WHERE AD_System_ID = ").Append(GetAD_System_ID());

            log.Fine(sql.ToString());

            // Send to database
            int no = DataBase.DB.ExecuteQuery(sql.ToString(), null, null);
            return no == 1;
        }

    }


}
