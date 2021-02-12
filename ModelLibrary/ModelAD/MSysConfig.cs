using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MSysConfig : X_AD_SysConfig
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(MSysConfig).FullName);

        /** Cache			*/
        private static CCache<String, String> cache = new CCache<String, String>("MSysConfig", 3);

        public const string SYSTEM_NATIVE_SEQUENCE = "SYSTEM_NATIVE_SEQUENCE";
        public const string REPORT_PAGE_SIZE = "REPORT_PAGE_SIZE";
        public const string BULK_REPORT_DOWNLOAD = "BULK_REPORT_DOWNLOAD";
        public const string DEFAULT_ACCOUNTING_APPLICABLE = "DEFAULT_ACCOUNTING_APPLICABLE";
        public const string USE_CRYSTAL_REPORT_VIEWER = "USE_CRYSTAL_REPORT_VIEWER";
        public const string PRODUCT_CONTAINER_APPLICABLE = "PRODUCT_CONTAINER_APPLICABLE";
        public const string FRONTEND_LIB_VERSION = "FRONTEND_LIB_VERSION";
        public const string FAILED_LOGIN_COUNT = "SYSTEM_FAILED_LOGIN_COUNTNATIVE_SEQUENCE";
        public const string FRAMEWORK_VERSION = "FRAMEWORK_VERSION";
        public const string PASSWORD_VALID_UPTO = "PASSWORD_VALID_UPTO";
        public const string SAVE_ACTION_LOG = "SAVE_ACTION_LOG";


        public MSysConfig(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor */


        public MSysConfig(Ctx ctx, int AD_SysConfig_ID, Trx trxName) : base(ctx, AD_SysConfig_ID, trxName) { }/** if (AD_SysConfig_ID == 0){SetAD_SysConfig_ID (0);} */


        public static String GetValue(String Name, bool Reload)
        {
            String str = "";
            if (!Reload)
            {
                str = Util.GetValueOfString(cache.Get(Name));
                if (!string.IsNullOrEmpty(str))
                    return str;
            }

            //
            String sql = "SELECT Value FROM AD_SysConfig"
                            + " WHERE IsActive='Y' AND Name='" + Name + "'"
                            + " ORDER BY AD_Client_ID DESC, AD_Org_ID DESC";
            IDataReader dr = null;
            try
            {
                dr = DB.ExecuteReader(sql, null);
                while (dr.Read())
                    str = dr.GetString(0);
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, "getValue", e);
            }
            finally
            {
                if (dr != null)
                    dr.Close();
                dr = null;
            }
            //
            if (str != null)
            {
                cache[Name] = str;
                return str;
            }
            else
            {
                // anyways, put the not found key as null
                cache[Name] = null;
                return null;
            }
        }

        /// <summary>
        /// Return Native sequence is enabled or not.
        /// </summary>
        /// <param name="reload">If true, Fetches value form DB, otherwise from cache</param>
        /// <returns></returns>
        public static bool IsNativeSequence(bool reload)
        {

            return MSysConfig.GetValue(MSysConfig.SYSTEM_NATIVE_SEQUENCE, reload) == "Y";

        }
    }
}
