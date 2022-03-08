/********************************************************
 * Module Name    : Get Text accoding to a language
 * Purpose        : Get Message Text from database accoding to a language and 
 *                  fill and create a dictionary from it
 * Chronological Development
 * Veena Pandey     
  ******************************************************/
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using System.Data;
using VAdvantage.SqlExec;

using VAdvantage.Login;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Common;
using System.IO;
namespace VAdvantage.Utility
{
    /// <summary>
    /// Fills a dictinary with all message keywords and its values according to a language.
    /// </summary>
    public sealed class Msg
    {
        /** Initial size of HashMap     */
        private const int MAP_SIZE = 750;
        /**  Separator between Msg and optional Tip     */
        private const String SEPARATOR = Env.NL + Env.NL;

       // private static string lastkey = "";
       // private static string mainMessage = "";


        /**	Singleton						*/
        private static Msg _msg = null;

        /**	Logger							*/
        private static VLogger _log = VLogger.GetVLogger(typeof(Msg).FullName);


        //private static CCache<string, string> dic = null;

        //private static string lastkey = "";
        //private static string mainMessage = "";

        /**
         * 	Get Message Object
         *	@return Mag
         */
        public static Msg Get()
        {
            if (_msg == null)
                _msg = new Msg();
            return _msg;
        }	//	get


        /**************************************************************************
         *	Constructor
         */
        private Msg()
        {
        }	//	Msg

        /**  The Map                    */
        private CCache<String, CCache<String, String>> _languages
            = new CCache<String, CCache<String, String>>("msg_lang", 2, 0);

        /**
         *  Get Language specific Message Map
         *  @param ad_language Language Key
         *  @return HashMap of Language
         */
        public CCache<String, String> GetMsgMap(String ad_language)
        {
            String AD_Language = ad_language;
            if (AD_Language == null || AD_Language.Length == 0)
                AD_Language = Language.GetBaseAD_Language();

            //  Do we have the language ?
            CCache<String, String> retValue = (CCache<String, String>)_languages[AD_Language];
            if (retValue != null && retValue.Size() > 0)
                return retValue;

            //  Load Language
            retValue = InitMsg(AD_Language);
            if (retValue != null)
            {
                _languages.Add(AD_Language, retValue);
                return retValue;
            }
            return retValue;
        }   //  getMsgMap


        /**
         *	Init message HashMap.
         *	The initial call is from ALogin (ConfirmPanel init).
         *	The second from Env.verifyLanguage.
         *  @param AD_Language Language
         *  @return Cache HashMap
         */
        private CCache<String, String> InitMsg(String AD_Language)
        {
            //	Trace.printStack();
            CCache<String, String> msg = new CCache<String, String>("AD_Message", MAP_SIZE, 0);
            //
            if (!DataBase.DB.IsConnected())
            {
                //s_log.log(Level.SEVERE, "No DB Connection");

                // //ErrorLog.FillErrorLog("Msg.InitMsg(String AD_Language)", "No DB Connection ", "", Message.MessageType.ERROR);
                return null;
            }
            string sqlQry = "";
            IDataReader dr = null;
            try
            {
                //PreparedStatement pstmt = null;


                if (AD_Language == null || AD_Language.Length == 0 || Utility.Env.IsBaseLanguage(AD_Language, "AD_Language"))
                {
                    sqlQry = "SELECT Value, MsgText, MsgTip FROM AD_Message";
                    dr = DataBase.DB.ExecuteReader(sqlQry);
                }
                else
                {
                    sqlQry = "SELECT m.Value, t.MsgText, t.MsgTip " + "FROM AD_Message_Trl t, AD_Message m "
                        + "WHERE m.AD_Message_ID=t.AD_Message_ID"
                        + " AND t.AD_Language=@AD_Language";

                    SqlParameter[] param = new SqlParameter[1];
                    param[0] = new SqlParameter("@AD_Language", AD_Language);
                    dr = DataBase.DB.ExecuteReader(sqlQry, param);

                }


                //	get values
                while (dr.Read())
                {
                    String AD_Message = dr[0].ToString();
                    StringBuilder msgText = new StringBuilder();
                    String msgTip = null;
                    msgText.Append(dr[1].ToString());
                    // if (dr.GetString(2) != DBNull.Value.ToString())
                    // {
                    msgTip = dr[2].ToString();
                    // }
                    //
                    if (msgTip != null && msgTip.Length != 0)			//	messageTip on next line, if exists
                        msgText.Append(" ").Append(SEPARATOR).Append(msgTip);
                    msg.Add(AD_Message, msgText.ToString());
                }

                dr.Close();
                dr = null;
                //pstmt.close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                _log.Log(Level.SEVERE, "initMsg", e);
                ////ErrorLog.FillErrorLog("Msg.InitMsg(String AD_Language)", sqlQry, e.Message, Message.MessageType.ERROR);
                return null;
            }
            //
            if (msg.Size() < 100)
            {
                _log.Log(Level.SEVERE, "Too few (" + msg.Count + ") Records found for " + AD_Language);
                return null;
            }
            _log.Info("Records=" + msg.Count + " - " + AD_Language);
            return msg;
        }

        /**
         *  Reset Message cache
         */
        public void Reset()
        {
            if (_languages == null)
                return;

            //  clear all languages
            IEnumerator<CCache<String, String>> iterator = _languages.Values.GetEnumerator();

            //if (iterator != null)
            //{
            //    CCache<String, String> hm = iterator.Current;
            //    hm.clear();

            //}
            while (iterator.MoveNext())
            {
                CCache<String, String> hm = iterator.Current;
                hm.Clear();
            }
            _languages.Clear();
        }   //  reset

        /**
         *  Return an array of the installed Languages
         *  @return Array of loaded Languages or null
         */
        public String[] GetLanguages()
        {
            if (_languages == null)
                return null;

            //make collection of cahce keys
            IEnumerator<String> iterator = _languages.Keys.GetEnumerator();

            //make total array size
            String[] retValue = new String[_languages.Size()];

            //assign array with key name
            int i = 0;
            while (iterator.MoveNext())
            {

                retValue[i] = iterator.Current.ToString();
                i++;
            }


            return retValue;
        }   //  getLanguages

        /**
         *  Check if Language is loaded
         *  @param language Language code
         *  @return true, if language is loaded
         */
        public Boolean IsLoaded(String language)
        {
            if (_languages == null)
                return false;
            return _languages.ContainsKey(language);
        }   //  isLoaded

        /**
         *  Lookup term
         *  @param AD_Language language
         *  @param text text
         *  @return translated term or null
         */
        private String Lookup(String AD_Language, String text)
        {
            if (text == null)
            {
                return null;
            }
            if (AD_Language == null || AD_Language.Length == 0)
            {
                return text;
            }
            //  hardcoded trl
            if (text.Equals("/") || text.Equals("\\"))
            {
                return System.IO.Path.DirectorySeparatorChar.ToString();
            }
            if (text.Equals(";") || text.Equals(":"))
            {
                //return File.pathSeparator;
                return System.IO.Path.PathSeparator.ToString();
            }

            CCache<String, String> langMap = GetMsgMap(AD_Language);
            if (langMap == null)
                return null;
            return (String)langMap[text];
        }   //  lookup


        /**************************************************************************
         *	Get translated text for AD_Message
         *  @param  ad_language - Language
         *  @param	AD_Message - Message Key
         *  @return translated text
         */
        public static String GetMsg(String ad_language, String AD_Message)
        {
           // return GetMsg(ad_language, AD_Message, 1);
            if (AD_Message == null || AD_Message.Length == 0)
                return "";
            //
            String AD_Language = ad_language;
            if (AD_Language == null || AD_Language.Length == 0)
                AD_Language = Language.GetBaseAD_Language();
            //
            if (DatabaseType.IsMSSql)
            {
                if (AD_Message.Equals("Date"))
                    AD_Message = "DATETIME";
            }
            String retStr = Get().Lookup(AD_Language, AD_Message);
            //
            if (retStr == null || retStr.Length == 0)
            {
                //s_log.warning("NOT found: " + AD_Message);
                ////ErrorLog.FillErrorLog("GetMsg", "NOT found: " + AD_Message, "", Message.MessageType.WARNING);
                return "[" + AD_Message + "]";
            }

            if (DatabaseType.IsMSSql)
                while (retStr.IndexOf(",NUMERIC,") > -1)
                    retStr = retStr.Replace(",NUMERIC,", ",number,");

            return retStr;
        }	//	getMsg

        /**
         *  Get translated text message for AD_Message
         *  @param  ctx Context to retrieve language
         *  @param	AD_Message - Message Key
         *  @return translated text
        // */
        //public static String GetMsg(Context ctx, String AD_Message)
        //{
        //    return GetMsg(Utility.Env.GetAD_Language(ctx), AD_Message);
        //}   //  getMsg

        public static String GetMsg(Ctx ctx, String AD_Message)
        {
            return GetMsg(Utility.Env.GetAD_Language(ctx), AD_Message);
        }   //  getMsg


        public static String GetMsg(String AD_Language, String AD_Message, int a)
        {
            //String AD_Language = ctx.GetAD_Language();
            String sqlQry = "";
            IDataReader dr = null;

            if (AD_Language == null || AD_Language.Length == 0 || Utility.Env.IsBaseLanguage(AD_Language, "AD_Language"))
            {
                sqlQry = "SELECT Value, MsgText, MsgTip FROM AD_Message WHERE Value = '" + AD_Message + "'";
                dr = DataBase.DB.ExecuteReader(sqlQry);
            }
            else
            {
                sqlQry = "SELECT m.Value, t.MsgText, t.MsgTip " + "FROM AD_Message_Trl t, AD_Message m "
                    + "WHERE m.Value = '" + AD_Message + "' AND m.AD_Message_ID=t.AD_Message_ID"
                    + " AND t.AD_Language=@AD_Language";

                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@AD_Language", AD_Language);
                dr = DataBase.DB.ExecuteReader(sqlQry, param);

            }


            //	get values
            string msg = "";
            
            while (dr.Read())
            {
                //String AD_Message = dr[0].ToString();
                //StringBuilder msgText = new StringBuilder();
                //String msgTip = null;
                msg = dr[1].ToString();
                // if (dr.GetString(2) != DBNull.Value.ToString())
                // {
                //msgTip = dr[2].ToString();
                // }
                //
            }
            dr.Close();

            if (string.IsNullOrEmpty(msg))
            {
                return "[" + AD_Message + "]";
            }

            return msg;

        }



        /**
         *  Get translated text message for AD_Message
         *  @param ctx Context to retrieve language
         *  @param AD_Message - Message Key
         *  @param parameter optional parameter
         *  @return translated text
         */
        //public static String GetMsg (Context ctx, String AD_Message, Object parameter)
        //{
        //    StringBuilder msg = new StringBuilder(GetMsg(Utility.Env.GetAD_Language(ctx), AD_Message));
        //    if (parameter != null)
        //    {
        //        if (parameter is Array)
        //        {
        //            Object[] pars = (Object[])parameter;
        //            for (int i = 0; i < pars.Length; i++)
        //            {
        //                if (pars[i] == null)
        //                    continue;
        //                String stringValue = pars[i].ToString();
        //                if (stringValue.Length != 0)
        //                    continue;
        //                msg.Append(Env.NL).Append(stringValue);
        //            }
        //        }
        //        else
        //        {
        //            String stringValue = parameter.ToString();
        //            if (stringValue.Length != 0)
        //            {
        //                if (stringValue.Length > 20)
        //                    msg.Append(Utility.Env.NL);
        //                else
        //                    msg.Append(" ");
        //                msg.Append(stringValue);
        //            }
        //        }
        //    }
        //    return msg.ToString();
        //}   //  getMsg


        public static String GetMsg(Ctx ctx, String AD_Message, Object parameter)
        {
            StringBuilder msg = new StringBuilder(GetMsg(Utility.Env.GetAD_Language(ctx), AD_Message));
            if (parameter != null)
            {
                if (parameter is Array)
                {
                    Object[] pars = (Object[])parameter;
                    for (int i = 0; i < pars.Length; i++)
                    {
                        if (pars[i] == null)
                            continue;
                        String stringValue = pars[i].ToString();
                        if (stringValue.Length != 0)
                            continue;
                        msg.Append(Env.NL).Append(stringValue);
                    }
                }
                else
                {
                    String stringValue = parameter.ToString();
                    if (stringValue.Length != 0)
                    {
                        if (stringValue.Length > 20)
                            msg.Append(Utility.Env.NL);
                        else
                            msg.Append(" ");
                        msg.Append(stringValue);
                    }
                }
            }
            return msg.ToString();
        }   //  getMsg



        /**
         *  Get translated text message for AD_Message
         *  @param  language Language
         *  @param	AD_Message - Message Key
         *  @return translated text
         */
        public static String GetMsg(Language language, String AD_Message)
        {
            return GetMsg(language.GetAD_Language(), AD_Message);
        }   //  getMeg

        /**
         *  Get translated text message for AD_Message
         *  @param  ad_language - Language
         *  @param	AD_Message - Message Key
         *  @param  getText if true only return Text, if false only return Tip
         *  @return translated text
         */
        public static String GetMsg(String ad_language, String AD_Message, Boolean getText)
        {
            String retStr = GetMsg(ad_language, AD_Message);
            int pos = retStr.IndexOf(SEPARATOR);
            //  No Tip
            if (pos == -1)
            {
                if (getText)
                    return retStr;
                else
                    return "";
            }
            else    //  with Tip
            {
                if (getText)
                    retStr = retStr.Substring(0, pos);
                else
                {
                    int start = pos + SEPARATOR.Length;
                    //	int end = retStr.length();
                    retStr = retStr.Substring(start);
                }
            }
            return retStr;
        }	//	getMsg

        /**
         *  Get translated text message for AD_Message
         *  @param  ctx Context to retrieve language
         *  @param	AD_Message  Message Key
         *  @param  getText if true only return Text, if false only return Tip
         *  @return translated text
         */
        //public static String GetMsg(Context ctx, String AD_Message, Boolean getText)
        //{
        //    return GetMsg (Utility.Env.GetAD_Language(ctx), AD_Message, getText);
        //}   //  getMsg

        public static String GetMsg(Ctx ctx, String AD_Message, Boolean getText)
        {
            return GetMsg(Utility.Env.GetAD_Language(ctx), AD_Message, getText);
        }   //  getMsg

        /**
         *  Get translated text message for AD_Message
         *  @param  language Language
         *  @param	AD_Message  Message Key
         *  @param  getText if true only return Text, if false only return Tip
         *  @return translated text
         */
        public static String GetMsg(Language language, String AD_Message, Boolean getText)
        {
            return GetMsg(language.GetAD_Language(), AD_Message, getText);
        }   //  getMsg

        /**
         *	Get clear text for AD_Message with parameters
         *  @param  ctx Context to retrieve language
         *  @param AD_Message   Message yey
         *  @param args         MessageFormat arguments
         *  @return translated text
         *  @see java.text.MessageFormat for formatting options
         */
        //public static String GetMsg(Context ctx, String AD_Message, Object[] args)
        //{
        //    return GetMsg (Env.GetAD_Language(ctx), AD_Message, args);
        //}	//	getMsg

        public static String GetMsg(Ctx ctx, String AD_Message, Object[] args)
        {
            return GetMsg(Env.GetAD_Language(ctx), AD_Message, args);
        }

        /**
         *	Get clear text for AD_Message with parameters
         *  @param  language Language
         *  @param AD_Message   Message yey
         *  @param args         MessageFormat arguments
         *  @return translated text
         *  @see java.text.MessageFormat for formatting options
         */
        public static String GetMsg(Language language, String AD_Message, Object[] args)
        {
            return GetMsg(language.GetAD_Language(), AD_Message, args);
        }	//	getMsg

        /**
         *	Get clear text for AD_Message with parameters
         *  @param ad_language  Language
         *  @param AD_Message   Message yey
         *  @param args         MessageFormat arguments
         *  @return translated text
         *  @see java.text.MessageFormat for formatting options
         */
        public static String GetMsg(String ad_language, String AD_Message, Object[] args)
        {
            String msg = GetMsg(ad_language, AD_Message);
            String retStr = msg;
            try
            {
                if (DatabaseType.IsMSSql)
                    msg = msg.Replace("NUMERIC", "NUMBER");
                retStr = String.Format(msg, args);	//	format string
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, msg, e);
            }
            return retStr;
        }


        /**************************************************************************
         * 	Get Amount in Words
         * 	@param language language
         * 	@param amount numeric amount (352.80)
         * 	@return amount in words (three*five*two 80/100)
         */
        public static String GetAmtInWords(Language language, String amount)
        {
            if (amount == null || language == null)
                return amount;
            //	Try to find Class
            String className = "VAdvantage.Print.AmtInWords_";//
            try
            {
                className += language.GetLanguageCode().ToUpper();
                //Type clazz = Class.forName(className);
                Type clazz = Type.GetType(className);
                //AmtInWords aiw = (AmtInWords)clazz.newInstance();
                VAdvantage.Print.AmtInWords aiw = (VAdvantage.Print.AmtInWords)Activator.CreateInstance(clazz);
                return aiw.GetAmtInWords(amount);
            }

            catch (Exception e)
            {
                _log.Log(Level.SEVERE, className, e);
            }

            //	Fall-back
            StringBuilder sb = new StringBuilder();
            int pos = amount.LastIndexOf('.');
            int pos2 = amount.LastIndexOf(',');
            if (pos2 > pos)
                pos = pos2;
            for (int i = 0; i < amount.Length; i++)
            {
                if (pos == i)	//	we are done
                {
                    String cents = amount.Substring(i + 1);
                    sb.Append(' ').Append(cents).Append("/100");
                    break;
                }
                else
                {
                    //char c = amount.charAt(i);
                    char c = amount[i];
                    if (c == ',' || c == '.')	//	skip thousand separator
                        continue;
                    if (sb.Length > 0)
                        sb.Append("*");
                    sb.Append(GetMsg(language, Convert.ToString(c)));
                }
            }
            return sb.ToString();
            //return "";
        }	//	getAmtInWords


        /**************************************************************************
         *  Get Translation for Element
         *  @param ad_language language
         *  @param ColumnName column name
         *  @param isSOTrx if false PO terminology is used (if exists)
         *  @return Name of the Column or "" if not found
         */
        public static String GetElement(String ad_language, String ColumnName, Boolean isSOTrx)
        {
            if (ColumnName == null || ColumnName.Equals(""))
                return "";
            String AD_Language = ad_language;
            if (AD_Language == null || AD_Language.Length == 0)
                AD_Language = Language.GetBaseAD_Language();

            //	Check AD_Element
            String retStr = "";
            string sqlQry = "";

            IDataReader dr = null;
            try
            {
                //PreparedStatement pstmt = null;

                try
                {
                    if (AD_Language == null || AD_Language.Length == 0 || Env.IsBaseLanguage(AD_Language, "AD_Element"))
                    {
                        sqlQry = "SELECT Name, PO_Name FROM AD_Element WHERE UPPER(ColumnName)=@ColumnName";
                        SqlParameter[] param = new SqlParameter[1];
                        param[0] = new SqlParameter("@ColumnName", ColumnName.ToUpper());
                        dr = DataBase.DB.ExecuteReader(sqlQry, param);
                    }
                    else
                    {
                        sqlQry = "SELECT t.Name, t.PO_Name FROM AD_Element_Trl t, AD_Element e "
                            + "WHERE t.AD_Element_ID=e.AD_Element_ID AND UPPER(e.ColumnName)=@ColumnName "
                            + "AND t.AD_Language=@AD_Language";
                        SqlParameter[] param = new SqlParameter[2];
                        param[0] = new SqlParameter("@ColumnName", ColumnName.ToUpper());
                        param[1] = new SqlParameter("@AD_Language", AD_Language);
                        dr = DataBase.DB.ExecuteReader(sqlQry, param);
                    }
                }
                catch (Exception)
                {
                    if (dr != null)
                    {
                        dr.Close();
                    }
                    return ColumnName;

                }
                //pstmt.setString(1, ColumnName.toUpperCase());
                //ResultSet rs = pstmt.executeQuery();
                if (dr.Read())
                {
                    retStr = dr[0].ToString();
                    if (!isSOTrx)
                    {
                        String temp = dr[1].ToString();
                        if (temp != null && temp.Length > 0)
                            retStr = temp;
                    }
                }
                dr.Close();

            }
            catch (Exception e)
            {
                dr.Close();
                _log.Log(Level.SEVERE, "getElement", e);

                ////ErrorLog.FillErrorLog("Msg.GetElement (String ad_language, String ColumnName, Boolean isSOTrx)", "getElement", e.Message, Message.MessageType.ERROR);
                return "";
            }
            if (retStr != null)
                return retStr.Trim();
            return retStr;
        }   //  getElement

        /**
         *  Get Translation for System Element using Sales terminology
         *  @param ctx context
         *  @param ColumnName column name
         *  @return Name of the Column or "" if not found
         */
        //public static String GetElement(Context ctx, String ColumnName)
        //{
        //    return GetElement (Env.GetAD_Language(ctx), ColumnName, true);
        //}   //  getElement

        public static String GetElement(Ctx ctx, String ColumnName)
        {
            return GetElement(Env.GetAD_Language(ctx), ColumnName, true);
        }

        /**
         *  Get Translation for System Element
         *  @param ctx context
         *  @param ColumnName column name
         *  @param isSOTrx sales transaction
         *  @return Name of the Column or "" if not found
         */
        //public static String GetElement(Context ctx, String ColumnName, Boolean isSOTrx)
        //{
        //    return GetElement (Env.GetAD_Language(ctx), ColumnName, isSOTrx);
        //}   //  getElement

        public static String GetElement(Ctx ctx, String ColumnName, Boolean isSOTrx)
        {
            return GetElement(Env.GetAD_Language(ctx), ColumnName, isSOTrx);
        }

        /**************************************************************************
         *	"Translate" text.
         *  <pre>
         *		- Check AD_Element.ColumnName	->	Name
         *		- Check AD_Message.AD_Message 	->	MsgText
         *  </pre>
         *  If checking AD_Element, the SO terminology is used.
         *  @param ad_language  Language
         *  @param isSOTrx sales order context
         *  @param text	Text - MsgText or Element Name
         *  @return translated text or original text if not found
         */
        public static String Translate(String ad_language, Boolean isSOTrx, String text)
        {
            if (text == null || text.Equals(""))
                return "";
            String AD_Language = ad_language;
            if (AD_Language == null || AD_Language.Length == 0)
                AD_Language = Language.GetBaseAD_Language();

            //	Check AD_Element
            String retStr = GetElement(AD_Language, text, isSOTrx);
            if (!retStr.Equals(""))
                return retStr.Trim();

            //	Check AD_Message
            if (DatabaseType.IsMSSql)
            {
                if (text.Equals("Date"))
                    text = "DATETIME";
            }
            retStr = Get().Lookup(AD_Language, text);
            if (retStr != null)
                return retStr;

            //	Nothing found
            if (!text.StartsWith("*"))
            {
                //s_log.warning("NOT found: " + text);
                // //ErrorLog.FillErrorLog("Msg.Translate(String ad_language, Boolean isSOTrx, String text)", "NOT found: " + text, "", Message.MessageType.WARNING);
            }
            return text;
        }	//	translate

        /***
         *	"Translate" text (SO Context).
         *  <pre>
         *  	- Check Context
         *		- Check AD_Message.AD_Message 	->	MsgText
         *		- Check AD_Element.ColumnName	->	Name
         *  </pre>
         *  If checking AD_Element, the SO terminology is used.
         *  @param ad_language  Language
         *  @param text	Text - MsgText or Element Name
         *  @return translated text or original text if not found
         */
        public static String Translate(String ad_language, String text)
        {
            return Translate(ad_language, true, text);
        }	//	translate

        /**
         *	"Translate" text.
         *  <pre>
         *  	- Check Context
         *		- Check AD_Element.ColumnName	->	Name
         *		- Check AD_Message.AD_Message 	->	MsgText
         *  </pre>
         *  @param ctx  Context
         *  @param text	Text - MsgText or Element Name
         *  @return translated text or original text if not found
         */
        //public static String Translate(Context ctx, String text)
        //{
        //    if (text == null || text.Length == 0)
        //        return text;
        //    String s = (String)ctx.Get(text);
        //    if (s != null && s.Length > 0)
        //        return s;
        //    return Translate (Utility.Env.GetAD_Language(ctx), ctx.IsSOTrx(), text);
        //}   //  translate

        public static String Translate(Ctx ctx, String text)
        {
            if (text == null || text.Length == 0)
                return text;
            String s = (String)ctx.Get(text);
            if (s != null && s.Length > 0)
                return s;
            return Translate(Utility.Env.GetAD_Language(ctx), ctx.IsSOTrx(), text);
        }

        /**
         *	"Translate" text.
         *  <pre>
         *		- Check AD_Element.ColumnName	->	Name
         *		- Check AD_Message.AD_Message 	->	MsgText
         *  </pre>
         *  @param language Language
         *  @param text     Text
         *  @return translated text or original text if not found
         */
        public static String Translate(Language language, String text)
        {
            return Translate(language.GetAD_Language(), false, text);
        }   //  translate

        /**
         *	Translate elements enclosed in "@" (at sign)
         *  @param ctx      Context
         *  @param text     Text
         *  @return translated text or original text if not found
         */
        //public static String ParseTranslation(Context ctx, String text)
        //{
        //    if (text == null || text.Length == 0)
        //        return text;

        //    String inStr = text;
        //    String token;
        //    StringBuilder outStr = new StringBuilder();

        //    int i = inStr.IndexOf("@");
        //    while (i != -1)
        //    {
        //        outStr.Append(inStr.Substring(0, i));			// up to @
        //        inStr = inStr.Substring(i+1, inStr.Length - (i+1));	// from first @

        //        int j = inStr.IndexOf("@");						// next @
        //        if (j < 0)										// no second tag
        //        {
        //            inStr = "@" + inStr;
        //            break;
        //        }

        //        token = inStr.Substring(0, j);
        //        outStr.Append(Translate(ctx, token));			// replace context

        //        inStr = inStr.Substring(j + 1, inStr.Length - (j + 1));	// from second @
        //        i = inStr.IndexOf("@");
        //    }

        //    outStr.Append(inStr);           					//	add remainder
        //    return outStr.ToString();
        //}   //  parseTranslation

        public static String ParseTranslation(Ctx ctx, String text)
        {
            if (text == null || text.Length == 0)
                return text;

            String inStr = text;
            String token;
            StringBuilder outStr = new StringBuilder();

            int i = inStr.IndexOf("@");
            while (i != -1)
            {
                outStr.Append(inStr.Substring(0, i));			// up to @
                inStr = inStr.Substring(i + 1, inStr.Length - (i + 1));	// from first @

                int j = inStr.IndexOf("@");						// next @
                if (j < 0)										// no second tag
                {
                    inStr = "@" + inStr;
                    break;
                }

                token = inStr.Substring(0, j);
                outStr.Append(Translate(ctx, token));			// replace context

                inStr = inStr.Substring(j + 1, inStr.Length - (j + 1));	// from second @
                i = inStr.IndexOf("@");
            }

            outStr.Append(inStr);           					//	add remainder
            return outStr.ToString();
        }


        //internal static string GetElement(string p)
        //{
        //    throw new NotImplementedException();
        //}

        public static string GetMessageText(Ctx m_ctx, string msg)
        {
            return Msg.GetMsg(m_ctx, msg);
        }
    }
}
