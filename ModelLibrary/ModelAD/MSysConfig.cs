using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MSysConfig : X_AD_SysConfig
    {
        /**	Static Logger	*/
        private static VLogger s_log = VLogger.GetVLogger(typeof(MSysConfig).Name);
        /** Cache			*/
        private static CCache<String, String> s_cache = new CCache<String, String>(Table_Name, 40, 0);
        /** resetCache			*/
        private static bool resetCache = false;
        /**
        * 	Load Constructor
        *	@param ctx context
        *	@param dr result set
        *	@param trxName transaction
        */
        public MSysConfig(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        public MSysConfig(Ctx ctx, int AD_Sequence_ID, Trx trxName)
            : base(ctx, AD_Sequence_ID, trxName)
        {

        }

        /** Reset Cache
	 * 
	 */
        public static void ResetCache()
        {
            s_cache.Reset();
        }


        /**
	 * Get system configuration property of type string
	 * @param Name
	 * @param defaultValue
	 * @return String
	 */
        public static String GetValue(String Name, String defaultValue)
        {
            return GetValue(Name, defaultValue, 0, 0);
        }


        /**
	 * Get system configuration property of type string
	 * @param Name
	 * @return String
	 */
        public static String GetValue(String Name)
        {
            return GetValue(Name, null);
        }

        /**
	 * Get system configuration property of type int
	 * @param Name
	 * @param defaultValue
	 * @return int
	 */
        public static int GetIntValue(String Name, int defaultValue)
        {
            String s = GetValue(Name);
            if (s == null)
                return defaultValue;

            if (s.Length == 0)
                return defaultValue;
            //
            try
            {
                return int.Parse(s);
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, "getIntValue (" + Name + ") = " + s, e);
            }
            return defaultValue;
        }


        /**
	 * Get system configuration property of type double
	 * @param Name
	 * @param defaultValue
	 * @return double
	 */
        public static double GetDoubleValue(String Name, double defaultValue)
        {
            String s = GetValue(Name);
            if (s == null || s.Length == 0)
                return defaultValue;
            //
            try
            {
                return Double.Parse(s);
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, "getDoubleValue (" + Name + ") = " + s, e);
            }
            return defaultValue;
        }

        /**
         * Get system configuration property of type boolean
         * @param Name
         * @param defaultValue
         * @return boolean
         */
        public static bool GetBooleanValue(String Name, bool defaultValue)
        {
            String s = GetValue(Name);
            if (s == null || s.Length == 0)
                return defaultValue;

            if ("Y".Equals(s, StringComparison.OrdinalIgnoreCase))
                return true;
            else if ("N".Equals(s, StringComparison.OrdinalIgnoreCase))
                return false;
            else
                return false;
        }

        /**
         * Get client configuration property of type string
         * @param Name
         * @param defaultValue
         * @param Client ID
         * @return String
         */
        public static String GetValue(String Name, String defaultValue, int AD_Client_ID)
        {
            return GetValue(Name, defaultValue, AD_Client_ID, 0);
        }

        /**
         * Get system configuration property of type string
         * @param Name
         * @param Client ID
         * @return String
         */
        public static String GetValue(String Name, int AD_Client_ID)
        {
            return (GetValue(Name, null, AD_Client_ID));
        }

        /**
         * Get system configuration property of type int
         * @param Name
         * @param defaultValue
         * @param Client ID
         * @return int
         */
        public static int getIntValue(String Name, int defaultValue, int AD_Client_ID)
        {
            String s = GetValue(Name, AD_Client_ID);
            if (s == null)
                return defaultValue;

            if (s.Length == 0)
                return defaultValue;
            //
            try
            {
                return int.Parse(s);
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, "getIntValue (" + Name + ") = " + s, e);
            }
            return defaultValue;
        }

        /**
         * Get system configuration property of type double
         * @param Name
         * @param defaultValue
         * @param Client ID
         * @return double
         */
        public static double GetDoubleValue(String Name, double defaultValue, int AD_Client_ID)
        {
            String s = GetValue(Name, AD_Client_ID);
            if (s == null || s.Length == 0)
                return defaultValue;
            //
            try
            {
                return Double.Parse(s);
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, "getDoubleValue (" + Name + ") = " + s, e);
            }
            return defaultValue;
        }

        /**
         * Get system configuration property of type boolean
         * @param Name
         * @param defaultValue
         * @param Client ID
         * @return boolean
         */
        public static bool GetBooleanValue(String Name, bool defaultValue, int AD_Client_ID)
        {
            String s = GetValue(Name, AD_Client_ID);
            if (s == null || s.Length == 0)
                return defaultValue;

            if ("Y".Equals(s, StringComparison.OrdinalIgnoreCase))
                return true;
            else if ("N".Equals(s, StringComparison.OrdinalIgnoreCase))
                return false;
            else
                return false;
        }

        /**
         * Get client configuration property of type string
         * @param Name
         * @param defaultValue
         * @param Client ID
         * @param Organization ID
         * @return String
         */
        public static String GetValue(String Name, String defaultValue, int AD_Client_ID, int AD_Org_ID)
        {
            String key = "" + AD_Client_ID + "_" + AD_Org_ID + "_" + Name;
            String str = Convert.ToString(s_cache.Get(key));
            if (str != null && str.Length > 0)
                return str;
            if (str == null && s_cache.ContainsKey(key)) // found null key
                return defaultValue;

            //
            String sql = "SELECT Value FROM AD_SysConfig"
                            + " WHERE Name='" + Name + "' AND AD_Client_ID IN (0, " + AD_Client_ID + ") AND AD_Org_ID IN (0, " + AD_Org_ID + ") AND IsActive='Y'"
                            + " ORDER BY AD_Client_ID DESC, AD_Org_ID DESC";
            //IDataReader pstmt = null;
            IDataReader rs = null;
            try
            {
                rs = DB.ExecuteReader(sql, null);
                //pstmt.setString(1, Name);
                //pstmt.setInt(2, AD_Client_ID);
                //pstmt.setInt(3, AD_Org_ID);
                //rs = pstmt.executeQuery();
                if (rs.Read())
                    str = rs.GetString(0);
            }
            catch (Exception e)
            {
                if (rs != null)
                {
                    rs.Close();
                    rs = null;
                }
                s_log.Log(Level.SEVERE, "getValue", e);
            }
            finally
            {
                if (rs != null)
                {
                    rs.Close();
                    rs = null;
                }
            }
            //
            if (str != null)
            {
                str = str.Trim();
                s_cache.Add(key, str);
                return str;
            }
            else
            {
                // anyways, put the not found key as null
                s_cache.Add(key, null);
                return defaultValue;
            }
        }

        /**
         * Get system configuration property of type string
         * @param Name
         * @param Client ID
         * @param Organization ID
         * @return String
         */
        public static String GetValue(String Name, int AD_Client_ID, int AD_Org_ID)
        {
            return GetValue(Name, null, AD_Client_ID, AD_Org_ID);
        }

        /**
         * Get system configuration property of type int
         * @param Name
         * @param defaultValue
         * @param Client ID
         * @param Organization ID
         * @return int
         */
        public static int getIntValue(String Name, int defaultValue, int AD_Client_ID, int AD_Org_ID)
        {
            String s = GetValue(Name, AD_Client_ID, AD_Org_ID);
            if (s == null)
                return defaultValue;

            if (s.Length == 0)
                return defaultValue;
            //
            try
            {
                return int.Parse(s);
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, "getIntValue (" + Name + ") = " + s, e);
            }
            return defaultValue;
        }

        /**
         * Get system configuration property of type double
         * @param Name
         * @param defaultValue
         * @param Client ID
         * @param Organization ID
         * @return double
         */
        public static double GetDoubleValue(String Name, double defaultValue, int AD_Client_ID, int AD_Org_ID)
        {
            String s = GetValue(Name, AD_Client_ID, AD_Org_ID);
            if (s == null || s.Length == 0)
                return defaultValue;
            //
            try
            {
                return Double.Parse(s);
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, "getDoubleValue (" + Name + ") = " + s, e);
            }
            return defaultValue;
        }

        /**
         * Get system configuration property of type boolean
         * @param Name
         * @param defaultValue
         * @param Client ID
         * @param Organization ID
         * @return boolean
         */
        public static bool GetBooleanValue(String Name, bool defaultValue, int AD_Client_ID, int AD_Org_ID)
        {
            String s = GetValue(Name, AD_Client_ID, AD_Org_ID);
            if (s == null || s.Length == 0)
                return defaultValue;

            if ("Y".Equals(s, StringComparison.OrdinalIgnoreCase))
                return true;
            else if ("N".Equals(s, StringComparison.OrdinalIgnoreCase))
                return false;
            else
                return false;
        }

        /**************************************************************************
         * 	Before Save
         *	@param newRecord
         *	@return true if save
         */
        protected bool BeforeSave(bool newRecord)
        {
            log.Fine("New=" + newRecord);

            if (GetAD_Client_ID() != 0 || GetAD_Org_ID() != 0)
            {

                // Get the configuration level from the System Record
                String configLevel = null;
                String sql = "SELECT ConfigurationLevel FROM AD_SysConfig WHERE Name='" + GetName() + "' AND AD_Client_ID = 0 AND AD_Org_ID = 0";
                // PreparedStatement pstmt = null;
                IDataReader rs = null;
                try
                {
                    rs = DB.ExecuteReader(sql, null);
                    if (rs.Read())
                        configLevel = rs.GetString(1);
                }
                catch (Exception e)
                {
                    if (rs != null)
                    {
                        rs.Close();
                        rs = null;
                    }
                    s_log.Log(Level.SEVERE, "getValue", e);
                }
                finally
                {
                    if (rs != null)
                    {
                        rs.Close();
                        rs = null;
                    }
                }

                if (configLevel == null)
                {
                    // not found for system
                    // if saving an org parameter - look config in client
                    if (GetAD_Org_ID() != 0)
                    {
                        // Get the configuration level from the System Record
                        sql = "SELECT ConfigurationLevel FROM AD_SysConfig WHERE Name=" + GetName() + " AND AD_Client_ID = " + GetAD_Client_ID() + " AND AD_Org_ID = 0";
                        try
                        {
                            //pstmt = DB.prepareStatement(sql, null);
                            //pstmt.setString(1, GetName());
                            //pstmt.setInt(2, GetAD_Client_ID());
                            rs = DB.ExecuteReader(sql);
                            if (rs.Read())
                                configLevel = rs.GetString(1);
                        }
                        catch (Exception e)
                        {
                            if (rs != null)
                            {
                                rs.Close();
                                rs = null;
                            }
                            s_log.Log(Level.SEVERE, "getValue", e);
                        }
                        finally
                        {
                            if (rs != null)
                            {
                                rs.Close();
                                rs = null;
                            }
                        }
                    }
                }

                if (configLevel != null)
                {

                    SetConfigurationLevel(configLevel);

                    // Disallow saving org parameter if the system parameter is marked as 'S' or 'C'
                    if (GetAD_Org_ID() != 0 &&
                            (configLevel.Equals(MSysConfig.CONFIGURATIONLEVEL_System) ||
                             configLevel.Equals(MSysConfig.CONFIGURATIONLEVEL_Client)))
                    {
                        log.SaveError("Can't Save Org Level", "This is a system or client parameter, you can't save it as organization parameter");
                        return false;
                    }

                    // Disallow saving client parameter if the system parameter is marked as 'S'
                    if (GetAD_Client_ID() != 0 && configLevel.Equals(MSysConfig.CONFIGURATIONLEVEL_System))
                    {
                        log.SaveError("Can't Save Client Level", "This is a system parameter, you can't save it as client parameter");
                        return false;
                    }

                }
                else
                {

                    // fix possible wrong config level
                    if (GetAD_Org_ID() != 0)
                        SetConfigurationLevel(CONFIGURATIONLEVEL_Organization);
                    else if (GetAD_Client_ID() != 0 && GetConfigurationLevel().Equals(MSysConfig.CONFIGURATIONLEVEL_System))
                        SetConfigurationLevel(CONFIGURATIONLEVEL_Client);

                }

            }

            return true;
        }	//	beforeSave

        //@Override
        //public String toString()
        //{
        //    return getClass().getSimpleName()+"["+get_ID()
        //        +", "+getName()+"="+getValue()
        //        +", ConfigurationLevel="+getConfigurationLevel()
        //        +", Client|Org="+getAD_Client_ID()+"|"+getAD_Org_ID()
        //        +", EntityType="+getEntityType()
        //        +"]";
        //}



    }
}
