/********************************************************
 * Module/Class Name    : Env (System Enviroment)
 * Purpose              : System Environment and static variables
 * Class Used           : EnvConstants
 * Chronological Development
 * Mukesh Arora    
 * Harwinder Singh 
 * Jagmohan Bhatt  
 ******************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Classes;
using System.Collections;
using VAdvantage.Login;
using VAdvantage.Logging;

using VAdvantage.DataBase;


namespace VAdvantage.Utility
{

    public class Env : EnvConstants
    {

        #region  "Declaration Variable"

        /** List of Unix browsers - sequence of test of existence */
        private static String[] UNIX_BROWSERS = { "firefox", "opera", "konqueror", "epiphany", "mozilla", "netscape" };

        public static String GOOGLE_MAPS_URL_PREFIX =
            "http://local.google.com/maps?q=";
        private static Context _sCtx = new Context();

        /**	Decimal 0	 */
        //public static Decimal ZERO = new Decimal(0.0);
        public const Decimal ZERO = Decimal.Zero;
        /**	Decimal 1	 */
        public const Decimal ONE = Decimal.One;
        /**	Decimal 100	 */
        public const Decimal ONEHUNDRED = 100.0M;

        /**	New Line 		 */
        //public const String NL = "\r\n";
        public const String NL = "   ";

        /** list of hidden Windows				*/
        //private static List<CFrame> s_hiddenWindows = new List<CFrame>();
        /** Closing Window Indicator			*/
        //private static bool s_closingWindows = false;

        /**	Logging								*/
        private static VLogger _log = VLogger.GetVLogger(typeof(Env).FullName);

        public static String ApplicationURL = "";

        private static CCache<string, Tuple<string, string, string, string>> _cacheModules = new CCache<string, Tuple<string, string, string, string>>("ModuleCache", 0);
        private static CCache<string, Tuple<string, string, string, string>> _cacheVISModules = new CCache<string, Tuple<string, string, string, string>>("ModuleVISCache", 0);

        #region "WindowNumber"
        //	Array of active Windows
        private static List<object> _sWindows = new List<object>(20);

        // property for Approve column name in case of workflow approval
        // for full record approval
        public static String ApproveColName = "IsApproved";


        #endregion

        #endregion


        /// <summary>
        /// Get Client Context
        /// </summary>

        public static Ctx GetCtx()
        {
            return _sCtx;
        }   //  getCtx

        public static Context GetContext()
        {
            return _sCtx;
        }

        public static void SetContext(Context ctx)
        {
            _sCtx = ctx;
        }



        ///Parse Context replaces global or Window context @tag@ with actual value.
        /// </summary>
        /// <param name="ctx">ctx context</param>
        /// <param name="WindowNo">Number of Window</param>
        /// <param name="value">Message to be parsed</param>
        /// <param name="onlyWindow">onlyWindow  if true, no defaults are used</param>
        /// <returns>parsed String or "" if not successful</returns>
        public static string ParseContext(Ctx ctx, int WindowNo, string value,
         bool onlyWindow)
        {
            return ParseContext(ctx, WindowNo, value, onlyWindow, false);
        }

        /// <summary>
        ///	Parse Context replaces global or Window context @tag@ with actual value.
        ///  @tag@ are ignored otherwise "" is returned
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="WindowNo">Number of Window</param>
        /// <param name="value">value Message to be parsed</param>
        /// <param name="onlyWindow">if true, no defaults are used</param>
        /// <param name="ignoreUnparsable">if true, unsuccessful @return parsed String or "" if not successful and ignoreUnparsable</param>
        /// <returns>parsed context </returns>
        public static string ParseContext(Ctx ctx, int WindowNo, string value,
        bool onlyWindow, bool ignoreUnparsable)
        {
            if (value == null || value.Length == 0)
                return "";

            string token;

            StringBuilder outStr = new StringBuilder("");

            int i = value.IndexOf('@');
            // Check whether the @ is not the last in line (i.e. in EMailAdress or with wrong entries)
            while (i != -1 && i != value.LastIndexOf("@"))
            {
                outStr.Append(value.Substring(0, i));			// up to @
                value = value.Substring(i + 1, value.Length - i - 1);	// from first @

                int j = value.IndexOf('@');						// next @
                if (j < 0)
                {
                    //_log.log(Level.SEVERE, "No second tag: " + inStr);
                    return "";						//	no second tag
                }

                token = value.Substring(0, j);

                string ctxInfo = ctx.GetContext(WindowNo, token, onlyWindow);	// get context
                if (ctxInfo.Length == 0 && (token.StartsWith("#") || token.StartsWith("$")))
                    ctxInfo = ctx.GetContext(token);	// get global context
                if (ctxInfo.Length == 0)
                {
                    //_log.config("No Context Win=" + WindowNo + " for: " + token);
                    if (!ignoreUnparsable)
                        return "";						//	token not found
                }
                else
                    outStr.Append(ctxInfo);				// replace context with Context

                value = value.Substring(j + 1, value.Length - j - 1);	// from second @
                i = value.IndexOf('@');
            }
            outStr.Append(value);						// add the rest of the string

            return outStr.ToString();
        }	//	parseContext

        /// <summary>
        /// Get Preference.
        ///  <pre>
        ///		0)	Current Setting
        /// 		1) 	Window Preference
        ///		2) 	Global Preference
        ///		3)	Login settings
        ///		4)	Accounting settings
        ///  </pre>
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Window_ID">window no</param>
        /// <param name="context">Entity to search</param>
        /// <param name="system">System level preferences (vs. user defined)</param>
        /// <returns>preference value</returns>
        public static String GetPreference(Ctx ctx, int AD_Window_ID, String context, bool system)
        {
            if (ctx == null || context == null)
                throw new ArgumentException("Require Context");
            String retValue = null;
            //
            if (!system)	//	User Preferences
            {
                retValue = ctx.GetContext("P" + AD_Window_ID + "|" + context);//	Window Pref
                if (retValue.Length == 0)
                    retValue = ctx.GetContext("P|" + context);  			//	Global Pref
            }
            else			//	System Preferences
            {
                retValue = ctx.GetContext("#" + context);   				//	Login setting
                if (retValue.Length == 0)
                    retValue = ctx.GetContext("$" + context);   			//	Accounting setting
            }
            return (retValue == null ? "" : retValue);
        }	//	getPreference

        /// <summary>
        ///Clean up context for Window (i.e. delete it)
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="WindowNo"></param>
        public static void ClearWinContext(Context ctx, int windowNo)
        {
            if (ctx == null)
            {
                throw new ArgumentException("Require Context");
            }
            //
            //IList<string> keys = ctx.GetKeys();
            //System.Collections.IEnumerator en = ctx.GetKeys().GetEnumerator();

            //while(en.MoveNext())
            //{
            //    object tag = en.Current;
            //    if (tag.ToString().StartsWith(windowNo + "|"))
            //    {
            //        ctx.Remove(tag);
            //        en = ctx.GetKeys().GetEnumerator();
            //        en.Reset();
            //    }
            //}

            Object[] keys = ctx.KeySet().ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                String tag = keys[i].ToString();
                if (tag.StartsWith(windowNo + "|"))
                {
                    ctx.Remove(keys[i].ToString());
                }
            }
            //  Clear Lookup Cache
            MLookupCache.CacheReset(windowNo);
            RemoveWindow(windowNo);
        }

        /// <summary>
        /// Clear Context
        /// </summary>
        /// <param name="windowNo">windowno</param>
        public static void ClearWinContext(int windowNo)
        {
            ClearWinContext(_sCtx, windowNo);
        }

        /// <summary>
        ///Remove window from active list
        /// </summary>
        /// <param name="WindowNo"></param>
        private static void RemoveWindow(int windowNo)
        {
            if (windowNo > -1 && windowNo < _sWindows.Count)
                _sWindows[windowNo] = null;
        }	//	removeWindow

        /// <summary>
        /// 	Add Container and return WindowNo.
        /// </summary>
        /// <param name="win"></param>
        /// <returns>WindowNo used for context</returns>
        public static int CreateWindowNo(object win)
        {
            int retValue = _sWindows.Count;
            _sWindows.Insert(_sWindows.Count, win);
            return retValue;

        }	//

        /// <summary>
        /// layout of login Language 
        /// </summary>
        /// <returns> true if LeftToRight, false if RightToLeft </returns>
        public static bool IsRightToLeft(string langCode)
        {
            try
            {
                System.Globalization.CultureInfo c = new System.Globalization.CultureInfo(langCode.Replace("_", "-"));
                return c.TextInfo.IsRightToLeft;
            }
            catch
            {
                if (langCode.ToLower().StartsWith("ar"))
                {
                    return true;
                }
            }
            return false;
        }

        //	getFrame



        /**
	    *  Get Base Language code. (e.g. en-US)
	    *  @return Base Language
	    */
        public static String GetBaseAD_Language()
        {
            return "en_US";// s_languages[0].getAD_Language();
        }   //  getBase

        public static int Scale(Decimal d)
        {
            long l = Decimal.ToInt64(d);
            Decimal d1 = d - l;
            int[] intbits = Decimal.GetBits(d1);
            long l2 = Convert.ToInt64(intbits[0]);

            int scaleDigit = d1.ToString().Length - 2;
            if (l2 == 0)
                return 0;


            if (scaleDigit == -1)
            {
                return 0;
            }
            return scaleDigit;

        }

        public static int Signum(Decimal d)
        {
            return Math.Sign(d);
            //if (d > 0)
            //    return 1;
            //else if (d < 0)
            //    return -1;
            //else
            //    return 0;
        }

        //*******************Language Issues***********************************************************************

        /** Context Language identifier */
        public const String LANGUAGE = "#AD_Language";
        public const String ISRIGHTTOLEFT = "#IsRightToLeft";

        public static String GetAD_Language(Context ctx)
        {
            if (ctx != null)
            {
                String lang = ctx.GetContext(LANGUAGE);
                if (lang != null && lang.Length > 0)
                    return lang;
            }
            return Language.GetBaseAD_Language();
        }	//	getAD_Language

        public static String GetAD_Language(Ctx ctx)
        {
            if (ctx != null)
            {
                String lang = ctx.GetContext(LANGUAGE);
                if (lang != null && lang.Length > 0)
                    return lang;
            }
            return Language.GetBaseAD_Language();
        }	//

        /// <summary>
        /// Get System Language
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>Language</returns>
        public static Language GetLanguage(Context ctx)
        {
            if (ctx != null)
            {
                String lang = ctx.GetContext(LANGUAGE);
                if (lang != null || lang.Length > 0)
                    return Language.GetLanguage(lang);
            }
            return Language.GetBaseLanguage();
        }	//	getLanguage

        /// <summary>
        /// Get System Language
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>Language</returns>
        public static Language GetLanguage(Ctx ctx)
        {
            if (ctx != null)
            {
                String lang = ctx.GetContext(LANGUAGE);
                if (lang != null || lang.Length > 0)
                    return Language.GetLanguage(lang);
            }
            return Language.GetBaseLanguage();
        }	//

        /// <summary>
        ///  Get Login Language
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>Language</returns>
        public static Language GetLoginLanguage(Context ctx)
        {
            return GetLanguage(ctx);
        }	//	getLanguage

        /// <summary>
        ///  Get Login Language
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>Language</returns>
        public static Language GetLoginLanguage(Ctx ctx)
        {
            return GetLanguage(ctx);
        }

        /// <summary>
        /// Get Type from packages
        /// </summary>
        /// <param name="fqClassame">Fully qualified classname</param>
        /// <returns>Return Type of class</returns>
        public static Type GetTypeFromPackage(string fqClassame)
        {
            Type type = null;
            foreach (string asm in GlobalVariable.PACKAGES)
            {
                try
                {
                    var asmbly = System.Reflection.Assembly.Load(asm);
                    type = asmbly.GetType(fqClassame);
                    if (type != null)
                        return type;
                }
                catch
                {
                    VLogger.Get().Warning("Error loading type " + fqClassame);
                }
            }
            return type;
        }


        //public static int GetAD_Language_ID()
        //{
        //    string sLang = GetLoginLanguage(Env.GetCtx()).GetAD_Language();
        //    MLanguage language = MLanguage.Get(Env.GetCtx(), sLang, null);
        //    return language.GetAD_Language_ID();
        //}

        /// <summary>
        /// Verify Language.
        /// Check that language is supported by the system
        /// </summary>
        /// <param name="ctx">might be updated with new AD_Language</param>
        /// <param name="language">language</param>
        /// <returns>Language</returns>
        public static Language VerifyLanguage(Context ctx, Language language)
        {
            if (language.IsBaseLanguage())
                return language;

            bool isSystemLanguage = false;
            List<String> AD_Languages = new List<String>();
            String sql = "SELECT DISTINCT AD_Language FROM AD_Message_Trl";
            System.Data.IDataReader dr = null;
            try
            {
                dr = VAdvantage.DataBase.DB.ExecuteReader(sql);
                while (dr.Read())
                {
                    String AD_Language = dr[0].ToString();
                    if (AD_Language.Equals(language.GetAD_Language()))
                    {
                        isSystemLanguage = true;
                        break;
                    }
                    AD_Languages.Add(AD_Language);
                }
                dr.Close();
            }
            catch
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }
            //	Found it
            if (isSystemLanguage)
                return language;
            //	No Language - set to System
            if (AD_Languages.Count <= 0)
            {
                return Language.GetBaseLanguage();
            }

            for (int i = 0; i < AD_Languages.Count; i++)
            {
                String AD_Language = (String)AD_Languages[i];	//	en_US
                String lang = AD_Language.Substring(0, 2);			//	en
                //
                String langCompare = language.GetAD_Language().Substring(0, 2);
                if (lang.Equals(langCompare))
                {
                    return Language.GetLanguage(AD_Language);
                }
            }

            //	We found same language
            //	if (!"0".equals(Msg.getMsg(AD_Language, "0")))

            return Language.GetBaseLanguage();
        }   //  verifyLanguage


        public static Language VerifyLanguage(Ctx ctx, Language language)
        {
            if (language.IsBaseLanguage())
                return language;

            bool isSystemLanguage = false;
            List<String> AD_Languages = new List<String>();
            String sql = "SELECT DISTINCT AD_Language FROM AD_Message_Trl";
            System.Data.IDataReader dr = null;
            try
            {
                dr = VAdvantage.DataBase.DB.ExecuteReader(sql);
                while (dr.Read())
                {
                    String AD_Language = dr[0].ToString();
                    if (AD_Language.Equals(language.GetAD_Language()))
                    {
                        isSystemLanguage = true;
                        break;
                    }
                    AD_Languages.Add(AD_Language);
                }
                dr.Close();
            }
            catch
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }
            //	Found it
            if (isSystemLanguage)
                return language;
            //	No Language - set to System
            if (AD_Languages.Count <= 0)
            {
                return Language.GetBaseLanguage();
            }

            for (int i = 0; i < AD_Languages.Count; i++)
            {
                String AD_Language = (String)AD_Languages[i];	//	en_US
                String lang = AD_Language.Substring(0, 2);			//	en
                //
                String langCompare = language.GetAD_Language().Substring(0, 2);
                if (lang.Equals(langCompare))
                {
                    return Language.GetLanguage(AD_Language);
                }
            }

            //	We found same language
            //	if (!"0".equals(Msg.getMsg(AD_Language, "0")))

            return Language.GetBaseLanguage();
        }   //  verifyLanguage

        /// <summary>
        /// Check Base Language
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="tableName">table to be translated</param>
        /// <returns>true if base language and table not translated</returns>
        public static bool IsBaseLanguage(Context ctx, String tableName)
        {
            return Language.IsBaseLanguage(GetAD_Language(ctx));
        }	//	isBaseLanguage

        public static bool IsBaseLanguage(Ctx ctx, String tableName)
        {
            return Language.IsBaseLanguage(GetAD_Language(ctx));
        }	//	isBaseLanguage

        /// <summary>
        /// Check Base Language
        /// </summary>
        /// <param name="AD_Language">language</param>
        /// <param name="tableName">name of the table</param>
        /// <returns>true if base language</returns>
        public static bool IsBaseLanguage(String AD_Language, String tableName)
        {
            return Language.IsBaseLanguage(AD_Language);
        }	//	isBaseLanguage

        /// <summary>
        /// Check Base Language
        /// </summary>
        /// <param name="language">language</param>
        /// <param name="tableName">table name</param>
        /// <returns>true if base language</returns>
        public static bool IsBaseLanguage(Language language, String tableName)
        {
            return language.IsBaseLanguage();
        }	//	isBaseLanguage

        /// <summary>
        /// Table is in Base Translation (AD)
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool IsBaseTranslation(string tableName)
        {
            if (tableName.StartsWith("AD")
                || tableName.Equals("C_Country_Trl"))
                return true;
            return false;
        }	//	

        /// <summary>
        ///Do we have Multi-Lingual Documents.
        // Set in DataBase.loadOrgs
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>rue if multi lingual documents</returns>
        //public static bool IsMultiLingualDocument(Ctx ctx)
        //{
        //    return VAdvantage.Model.MClient.Get((Ctx)ctx).IsMultiLingualDocument();//
        //    //MClient.get(ctx).isMultiLingualDocument();
        //}	//	isMultiLingualDocument

        /// <summary>
        ///Get Header info (connection, org, user)
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="WindowNo"></param>
        /// <returns></returns>
        public static String GetHeader(Ctx ctx, int windowNo)
        {
            StringBuilder sb = new StringBuilder();
            if (windowNo > 0)
                sb.Append(ctx.GetContext(windowNo, "WindowName", false)).Append("  ");
            sb.Append(ctx.GetContext("##AD_User_Name")).Append("@")
                .Append(ctx.GetContext("#AD_Org_Name")).Append(".")
                .Append(ctx.GetContext("#AD_Client_Name"))
                .Append(" [").Append(VConnection.Get().ToString()).Append("]");//                   CConnection.get().toString()).append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Show Window
        /// </summary>
        /// <param name="AD_Window_ID"></param>
        /// <returns></returns>
        public static bool ShowWindow(int AD_Window_ID)
        {
            //for (int i = 0; i < s_hiddenWindows.Count; i++)
            //{
            //    CFrame hidden = s_hiddenWindows[i];
            //    if (hidden.GetAD_Window_ID() == AD_Window_ID)
            //    {
            //        s_hiddenWindows.RemoveAt(i);
            //        _log.Info(hidden.ToString());
            //        hidden.Show();// .setVisible(true);
            //        hidden.BringToFront();
            //        return true;
            //    }
            //}
            return false;
        }

        /// <summary>
        ///Hide Window
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>


      
        
        /// <summary>
        /// Return the Form pointer of WindowNo - or null
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <returns></returns>



        /// <summary>
        ///    Exit System
        /// </summary>
        /// <param name="status"></param>
        public static void ExitEnv(int status)
        {
            //	End Session
            MSession session = MSession.Get(Env.GetContext(), false);	//	finish
            if (session != null)
                session.Logout();
            //
            Reset(true);	// final cache reset
            _log.Info("");
            //
            VLogMgt.Shutdown();
            //

            if (VAdvantage.DataBase.Ini.IsClient())
                Environment.Exit(status);
        }	//	close
        /// <summary>
        /// Reset Cache
        /// </summary>
        /// <param name="finalCall">everything otherwise login data remains</param>
        public static void Reset(bool finalCall)
        {

            _log.Info("finalCall=" + finalCall);
            // CloseWindows();
            _sWindows.Clear();

            //	Clear all Context
            if (finalCall)
            {
                _sCtx.Clear();
            }
            else	//	clear window context only
            {
                Object[] keys = _sCtx.KeySet().ToArray();
                for (int i = 0; i < keys.Length; i++)
                {
                    String tag = keys[i].ToString();
                    //if (Character.isDigit(tag.charAt(0)))
                    if (char.IsDigit(Convert.ToChar(tag.Substring(0, Convert.ToInt32(tag.Length - (tag.Length - 1))))))
                    {
                        _sCtx.Remove(keys[i].ToString());
                    }
                }
            }

            //	Cache
            CacheMgt.Get().Reset();
            DataBase.DB.CloseTarget();
            //	Reset Role Access
            if (!finalCall)
            {
                //DataBase.DB.SetDBTarget(CConnection.get());
                //DataBase.DB.SetDBTarget(DataBase.DB.GetConnection());
                //MRole defaultRole = MRole.GetDefault(_sCtx, false);
                //if (defaultRole != null)
                //{
                //    defaultRole.LoadAccess(true);	//	Reload
                //}
            }
        }


        public static void LogOut()
        {
            for (int i = 2; i < _sWindows.Count; i++)
            {
                object o = _sWindows[i];
                if (o != null)
                {
                    //if (o is System.Windows.Forms.Form)
                    //{
                    //    ((System.Windows.Forms.Form)o).Close();
                    //}
                    //else if (o is System.Windows.Forms.ContainerControl)
                    //{
                    //    System.Windows.Forms.Form frm = ((System.Windows.Forms.ContainerControl)o).ParentForm;
                    //    if (frm != null)
                    //    {
                    //        frm.Close();
                    //        frm = null;
                    //    }
                    //}
                }
            }

            MSession session = MSession.Get(Env.GetContext(), false);	//	finish
            if (session != null)
                session.Logout();
            VLogErrorBuffer.Get(true).ResetBuffer(false);
            Reset(true);
        }

        public static void StartBrowser(String url)
        {
            _log.Info(url);
            try
            {
                System.Diagnostics.Process.Start(url);
            }

            catch (Exception exc1)
            {

                // System.ComponentModel.Win32Exception is a known exception that occurs when Firefox is default browser.  

                // It actually opens the browser but STILL throws this exception so we can just ignore it.  If not this exception,

                // then attempt to open the URL in IE instead.

                if (exc1.GetType().ToString() != "System.ComponentModel.Win32Exception")
                {

                    // sometimes throws exception so we have to just ignore

                    // this is a common .NET bug that no one online really has a great reason for so now we just need to try to open

                    // the URL using IE if we can.

                    try
                    {

                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("IExplore.exe", url);

                        System.Diagnostics.Process.Start(startInfo);

                        startInfo = null;

                    }

                    catch (Exception exc2)
                    {

                        _log.Severe("Can't locate browser: " + exc2.Message);
                    }
                }
            }
        }

        public static bool IsWindows()
        {
            return true;
        }




        ///////////////////Manfacturing//////////////////////////////
       

        /**
         *	Parse Context replaces global or Window context @tag@ with actual value.
         *	The tag may have a | with a fixed value e.g. @C_BPartner_ID|0@
         *  @param ctx context
         *	@param WindowNo	Number of Window
         *	@param value Message to be parsed
         *  @param onlyWindow if true, no defaults are used
         * 	@param ignoreUnparsable if true, unsuccessful return parsed String or "" if not successful and ignoreUnparsable
         *	@return parsed context
         */
        public static QueryParams ParseContextUsingBindVariables(Ctx ctx, int WindowNo, String value,
                Boolean onlyWindow, Boolean ignoreUnparsable)
        {
            QueryParams result = new QueryParams();
            if (value == null || value.Length == 0)
            {
                return result;
            }

            String token;
            String inStr = value;
            StringBuilder outStr = new StringBuilder();

            int i = inStr.IndexOf('@');
            // Check whether the @ is not the last in line (i.e. in EMailAdress or with wrong entries)
            while (i != -1 && i != inStr.LastIndexOf("@"))
            {
                outStr.Append(inStr.Substring(0, i));			// up to @
                inStr = inStr.Substring(i + 1, inStr.Length);	// from first @

                int j = inStr.IndexOf('@');						// next @
                if (j < 0)
                {
                    _log.Log(Level.SEVERE, "No second tag: " + inStr);
                    return result;						//	no second tag
                }

                String defaultV = null;
                token = inStr.Substring(0, j);
                int idx = token.IndexOf("|");	//	or clause
                if (idx >= 0)
                {
                    defaultV = token.Substring(idx + 1, token.Length);
                    token = token.Substring(0, idx);
                }
                String ctxInfo = ctx.GetContext(WindowNo, token, onlyWindow);	// get context
                if (ctxInfo.Length == 0 && (token.StartsWith("#") || token.StartsWith("$")))
                    ctxInfo = ctx.GetContext(token);	// get global context
                if (ctxInfo.Length == 0 && defaultV != null)
                    ctxInfo = defaultV;

                if (ctxInfo.Length == 0)
                {
                    _log.Config("No Context Win=" + WindowNo + " for: " + token);
                    //		+ " Value=" + value);
                    if (!ignoreUnparsable)
                        return result;						//	token not found
                }
                else
                {
                    inStr = inStr.Substring(j + 1, inStr.Length);	// from second @
                    // if token is surrounded by single quotes, remove them; also means it's a string
                    //if (outStr.charAt(outStr.Length - 1) == '\'' && inStr.Trim().StartsWith("\'"))
                    if (outStr.ToString().Substring(0, outStr.Length - 1).EndsWith("\'")
                        && inStr.Trim().StartsWith("\'"))
                    {
                        outStr.Remove(outStr.Length - 1, outStr.Length);
                        inStr = inStr.Trim().Substring(1);
                        result.parameters.Add(ctxInfo);
                    }
                    else
                    {
                        // attempt to parse as number; if failure, pass as string
                        try
                        {
                            result.parameters.Add(Utility.Util.GetValueOfDecimal(ctxInfo));
                        }
                        catch
                        {
                            result.parameters.Add(ctxInfo);
                        }
                    }

                    outStr.Append('?');
                }
                i = inStr.IndexOf('@');
            }
            outStr.Append(inStr);						// add the rest of the string

            result.sql = outStr.ToString();
            return result;
        }

        public static int GetColumnID(string tableName, string columnName)
        {

            string sql = " SELECT cl.ad_column_id FROM ad_column cl WHERE cl.ad_table_id=" +
                     "(SELECT tb.ad_table_id FROM ad_table tb WHERE tb.tablename='" + tableName + "'" +
                      ") and cl.columnname='" + columnName + "'";

            return DB.GetSQLValue(null, sql);
        }


        //Module
        /// <summary>
        /// Check given name has module prefix
        /// </summary>
        /// <param name="name">name to match</param>
        /// <param name="asmInfo">out module asembly info</param>
        /// <returns>true , if has module prefix</returns>
        public static bool HasModulePrefix(string name, out Tuple<string, string, string> asmInfo)
        {
            bool retVal = false;


            asmInfo = null;

            LoadAllModules();

            if (name != null && name.Contains("_") && name.IndexOf('_') > 2)
            {
                string prefix = name.Substring(0, name.IndexOf('_') + 1);
                if (_cacheModules.ContainsKey(prefix))
                {
                    Tuple<string, string, string, string> val = _cacheModules[prefix];
                    asmInfo = new Tuple<string, string, string>(val.Item1, val.Item2, val.Item3);
                    retVal = true;
                }
            }
            return retVal;

            #region Commented Code
            ///* Tuple */
            ///* [Assembly Name, Vesion=version] , [Namespace]  */

            //if (name != null && name.Contains("_") && name.IndexOf('_') > 2)
            //{
            //    /* prefix */
            //    string prefix = name.Substring(0, name.IndexOf('_') + 1);
            //    string aName = "", nSpace = "", vNo = "", vId = "";
            //    int keyId = 0;
            //    bool isRead = false;
            //    System.Data.IDataReader dr = null;
            //    try
            //    {
            //        //dr = DataBase.DB.ExecuteReader(" SELECT AD_ModuleInfo_ID,AssemblyName,NameSpace,"
            //        //                                                     + " VersionNo,VersionID FROM AD_ModuleInfo WHERE prefix='" + prefix + "'");
            //        //if (dr.Read())
            //        //{
            //        //    aName = dr[1].ToString() + "Svc";
            //        //    nSpace = dr[2].ToString();
            //        //    vNo = dr[3].ToString();
            //        //    vId = dr[4].ToString();
            //        //    //aName = dr[1].ToString() + "Svr , Version="+vo;
            //        //    isRead = true;
            //        //}
            //        //dr.Close();

            //        //if (isRead)
            //        //{
            //        //    asmInfo = new Tuple<string, string>(aName, nSpace);
            //        //    retVal = true;
            //        //}

            //        //// Change




            //    }
            //    catch
            //    {
            //        if (dr != null)
            //            dr.Close();
            //    }
            //}
            //return retVal;
            #endregion
        }


        public static string GetVISInstalledVersion(string name)
        {
            if (_cacheVISModules.ContainsKey(name))
            {

                return _cacheVISModules[name].Item3;
            }
            return "";
        }



        public static bool GetModulePrefix(string name, out string retPrefix, out string nspace)
        {
            retPrefix = "";
            nspace = "";
            LoadAllModules();

            if (name != null && name.Contains("_") && name.IndexOf('_') > 2)
            {
                string prefix = name.Substring(0, name.IndexOf('_') + 1);
                if (_cacheModules.ContainsKey(prefix))
                {
                    var s = _cacheModules[prefix];
                    retPrefix = prefix;
                    nspace = s.Item2;
                    return true;
                }
            }
            return false;

            #region Commented Code
            ///* Tuple */
            ///* [Assembly Name, Vesion=version] , [Namespace]  */

            //if (name != null && name.Contains("_") && name.IndexOf('_') > 2)
            //{
            //    /* prefix */
            //    string prefix = name.Substring(0, name.IndexOf('_') + 1);
            //    string aName = "", nSpace = "", vNo = "", vId = "";
            //    int keyId = 0;
            //    bool isRead = false;
            //    System.Data.IDataReader dr = null;
            //    try
            //    {
            //        //dr = DataBase.DB.ExecuteReader(" SELECT AD_ModuleInfo_ID,AssemblyName,NameSpace,"
            //        //                                                     + " VersionNo,VersionID FROM AD_ModuleInfo WHERE prefix='" + prefix + "'");
            //        //if (dr.Read())
            //        //{
            //        //    aName = dr[1].ToString() + "Svc";
            //        //    nSpace = dr[2].ToString();
            //        //    vNo = dr[3].ToString();
            //        //    vId = dr[4].ToString();
            //        //    //aName = dr[1].ToString() + "Svr , Version="+vo;
            //        //    isRead = true;
            //        //}
            //        //dr.Close();

            //        //if (isRead)
            //        //{
            //        //    asmInfo = new Tuple<string, string>(aName, nSpace);
            //        //    retVal = true;
            //        //}

            //        //// Change




            //    }
            //    catch
            //    {
            //        if (dr != null)
            //            dr.Close();
            //    }
            //}
            //return retVal;
            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static bool IsModuleInstalled(string prefix)
        {
            bool retVal = false;
            LoadAllModules();

            if (prefix != null && prefix.Contains("_") && prefix.IndexOf('_') > 2)
            {
                if (_cacheModules.ContainsKey(prefix))
                {
                    retVal = true;
                }
            }

            return retVal;
        }

        //AssemblyName,NameSpace, VersionNo,VersionID,prefix
        public static bool? IsModuleAlreadyInstalled(string prefix, string versionNo, string name)
        {
            bool? retValue = false;
            Version vInstalled = null, vImport = null;
            LoadAllModules();

            if (_cacheModules.ContainsKey(prefix) && prefix != "VIS_")
            {
                var values = _cacheModules[prefix];

                vInstalled = new Version(values.Item3);
                vImport = new Version(versionNo);

                if (vInstalled >= vImport)
                {
                    retValue = true;
                }
                else
                {
                    retValue = null;
                }
            }
            else if (prefix == "VIS_")
            {

                if (_cacheVISModules.ContainsKey(name))
                {
                    var values = _cacheVISModules[name];

                    vInstalled = new Version(values.Item3);
                    vImport = new Version(versionNo);

                    if (vInstalled >= vImport)
                    {
                        retValue = true;
                    }
                    else
                    {
                        retValue = null;
                    }


                }
            }

            vInstalled = null;
            vImport = null;
            return retValue;
        }

        /// <summary>
        /// Returns the Version of the Module from the Cache List
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetModuleVersion(string name)
        {
            if (_cacheModules.Count > 0)
            {
                if (_cacheModules.ContainsKey(name))
                {
                    Tuple<string, string, string, string> x = _cacheModules[name];
                    return x.Item3;
                }
            }
            return "";
        }


        /// <summary>
        /// Load All Modules and maintain Cache
        /// </summary>
        private static void LoadAllModules()
        {
            string aName = "", nSpace = "", vNo = "", vId = "";
            //int keyId = 0;
            //bool isRead = false;
            System.Data.IDataReader dr = null;
            try
            {
                if (_cacheModules.Count == 0)
                {
                    dr = DataBase.DB.ExecuteReader(" SELECT AD_ModuleInfo_ID,AssemblyName,NameSpace,"
                                                                    + " VersionNo,VersionID,prefix FROM AD_ModuleInfo WHERE Prefix != 'VIS_' AND IsActive='Y' AND AD_Client_ID = 0 ");
                    Tuple<string, string, string, string> modules = null;

                    while (dr.Read())
                    {
                        aName = dr[1].ToString() + "Svc";
                        nSpace = dr[2].ToString();
                        vNo = dr[3].ToString();
                        vId = dr[4].ToString();
                        modules = new Tuple<string, string, string, string>(dr[1].ToString() + "Svc", dr[2].ToString(), dr[3].ToString(), dr[4].ToString());
                        if (!_cacheModules.ContainsKey(dr[5].ToString()))
                        {
                            _cacheModules.Add(dr[5].ToString(), modules);
                        }
                    }
                    dr.Close();

                    dr = DataBase.DB.ExecuteReader(" SELECT AD_ModuleInfo_ID,AssemblyName,NameSpace,"
                                                                    + " VersionNo,VersionID,prefix,Name FROM AD_ModuleInfo WHERE Prefix='VIS_' AND IsActive='Y' AND AD_Client_ID = 0 ");

                    while (dr.Read())
                    {
                        modules = new Tuple<string, string, string, string>(dr[1].ToString() + "Svc", dr[2].ToString(), dr[3].ToString(), dr[4].ToString());
                        if (!_cacheVISModules.ContainsKey(dr[6].ToString()))
                        {
                            _cacheVISModules.Add(dr[6].ToString(), modules);
                        }
                    }
                    dr.Close();

                }
            }
            catch (Exception ex)
            {
                _log.Config("Error Loading Modules" + ex.Message);
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
            }


        }

        /// <summary>
        /// trim module prefix form name
        /// </summary>
        /// <param name="name">name to check</param>
        /// <returns>trimmed name if contain module prefix</returns>
        public static string TrimModulePrefix(string name)
        {

            if (name != null && name.Contains("_") && name.IndexOf('_') > 2)
            {
                try
                {
                    int index = name.IndexOf('_');
                    string prefix = name.Substring(0, index + 1);
                    if (Convert.ToInt32(DataBase.DB.ExecuteScalar(" SELECT count(prefix) FROM AD_ModuleInfo WHERE prefix='" + prefix + "'")) > 0)
                    {
                        name = name.Substring(index + 1);
                    }
                }
                catch
                {
                    //log error
                }
            }
            return name;
        }

        private static KeyEdition _keyEdition = KeyEdition.NotRegisterd;

        public static KeyEdition GetKeyEdition()
        {
            return _keyEdition;
        }

        public static void InitEdition(KeyEdition key)
        {
            _keyEdition = key;
        }

        public static string GetApplicationURL(Ctx ctx)
        {
            return ctx.GetApplicationUrl();
        }

        public class QueryParams
        {
            public String sql = "";
            public List<Object> parameters;

            /// <summary>
            /// 
            /// </summary>
            public QueryParams()
            {
                parameters = new List<Object>();
            }

            /// <summary>
            /// </summary>
            /// <param name="sql">Sql</param>
            /// <param name="parameters">Param</param>
            public QueryParams(String sql, Object[] localparam)
            {
                this.sql = sql;
                //this.params = Arrays.asList(params);
                this.parameters = localparam.ToList();
                //this.parameters.AddRange(localparam.ToList());
            }
        }
    }
}