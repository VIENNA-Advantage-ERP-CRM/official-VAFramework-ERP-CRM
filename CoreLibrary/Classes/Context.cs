/********************************************************
 * Module Name    : Show Tree
 * Purpose        : used to store login information and other setting(user preference etc)
 * Class Used     : Ctx
 * Created By     : Harwinder 
 * Date           : 20-1-2009
**********************************************************/

using System;
using System.Collections.Generic;
using System.Collections;

namespace VAdvantage.Classes
{
    #region "Old Context Class"
    /*  

    /// <summary>
    ///  intended to Store login infomation
    /// </summary>
    public class Context
    {

        
     
        
        #region declaration
        //sorted list
        //SortedList<string, String> ctx = new SortedList<string, string>();

        Hashtable ctx;

        #endregion

        /// <summary>
        /// standard constructor
        /// </summary>
        public Context()
        {
            ctx = new Hashtable();
        }

        /// <summary>
        /// Get value form list as integer type
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public int GetContextAsInt(string context)
        {
            if (context == null || context == "")
                throw new ArgumentException("Require Context");
            string value = GetContext(context);
            if (value == null || value.Length ==0)
                // value = GetContext(0, context, false);		//	search 0 and defaults
                //if (value.length() == 0)
                return 0;
            //
            try
            {
                return int.Parse(value.ToString());
            }
            catch (FormatException e)
            {
                //log
            }
            return 0;
        }
        /// <summary>
        ///Get Context and convert it to an integer (0 if error)
        /// </summary>
        /// <param name="WindowNo">window no</param>
        /// <param name="context">context key</param>
        /// <returns>value or 0</returns>
        /// <author>raghu</author>
        public int GetContextAsInt(int windowNo, string context)
        {
            string s = GetContext(windowNo, context, false);
            if (s == null || s.Length ==0)
                return 0;
            //
            try
            {
                return int.Parse(s);
            }
            catch (Exception e)
            {
                //log.warning("(" + context + ") = " + s + " - " + e.getMessage());
            }
            return 0;
        }
        /// <summary>
        ///Get Context and convert it to a Timestamp
        ///if error return today's date
        /// </summary>
        /// <param name="WindowNo">window no</param>
        /// <param name="context">context key</param>
        /// <returns>Time</returns>
        public long GetContextAsTime(int windowNo, String context)
        {
            if (context == null)
                throw new ArgumentException("Require Context");
           string  s = GetContext(windowNo, context, false);
            if (s == null || s.Equals(""))
            {
                return CommonFunctions.CurrentTimeMillis();
            }
            try
            {
                return long.Parse(s);
            }
            catch (Exception e)
            {
                //log.warning("(" + context + ") = " + s + " - " + e.getMessage());
            }
            return CommonFunctions.CurrentTimeMillis();
        }


        /// <summary>
        ///Get Context and convert it to an integer (0 if error)
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="TabNo"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public int GetContextAsInt(int windowNo, int tabNo, String context)
        {
            String s = GetContext(windowNo, tabNo, context);
            if (s.Length == 0)
                return 0;
            //
            try
            {
                return int.Parse(s);
            }
            catch (FormatException e)
            {
                //Common.////ErrorLog.FillErrorLog("Context.GetContextAsInt()", "", "(" + context + ") = " + s + " - " + e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
            }
            return 0;
        }	//	getContextAsInt


        /// <summary>
        /// Get Value of Key
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetContext(string context)
        {
            if (context == null || context == "")
                throw new ArgumentException("Require Context");
            object value = null;

            value = ctx[context];
            //if (ctx.IndexOfKey(context) != -1)
            //{
            //    value = ctx.Values[ctx.IndexOfKey(context)];
            //}

            if (value == null || value.ToString() == "")
            {
                if (context.Equals("#AD_User_ID"))
                    return GetContext("#" + context);
                else if (context.Equals("#AD_User_Name"))
                    return GetContext("#" + context);
                return "";
            }
            return value.ToString();
        }	//


        /// <summary>
        /// Gets the sortedlist as it is
        /// </summary>
        /// <returns>SortedList</returns>
        public Hashtable GetHashTable()
        {
            return ctx;
        }

        /// <summary>
        ///   Set Global Context to Value
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetContext(string context, string value)
        {
            if (context == null && context == "")
                return;
            //
            //if (ctx.ContainsKey(context))
            //{
            //    ctx.Remove(context);
            //}
            if (value == null || value == "")
                ctx[context] = null;
            else
                ctx[context] =  value;
        }	//	

        /// <summary>
        /// Set Login AD_User_ID
        /// </summary>
        /// <param name="AD_User_ID"></param>
        public void SetAD_User_ID(int AD_User_ID)
        {
            SetContext("##AD_User_ID", AD_User_ID.ToString());
        }	//	setAD_User_ID

        /// <summary>
        /// Add Role ID in list
        /// </summary>
        /// <param name="AD_Role_ID"></param>
        /// <returns></returns>
        public void SetAD_Role_ID(int AD_Role_ID)
        {
            SetContext("#AD_Role_ID", AD_Role_ID.ToString());
        }

        /// <summary>
        /// Get Role Id from Context
        /// </summary>
        /// <returns></returns>
        public int GetAD_Role_ID()
        {
            return GetContextAsInt("#AD_Role_ID");
        }

        /// <summary>
        ///	Get Login AD_Role_Name
        /// </summary>
        /// <returns></returns>
        public string GetAD_Role_Name()
        {
            return GetContext("#AD_Role_Name");
        }

        /// <summary>
        /// Get User Id from List
        /// </summary>
        /// <returns></returns>
        public int GetAD_User_ID()
        {
            return GetContextAsInt("#AD_User_ID");
        }

        /// <summary>
        ///	Get Login AD_User_Name
        /// </summary>
        /// <returns></returns>
        public string GetAD_User_Name()
        {
            return GetContext("#AD_User_Name");
        }

        /// <summary>
        /// Get Login AD_Org_ID
        /// </summary>
        /// <returns></returns>
        public int GetAD_Org_ID()
        {
            return GetContextAsInt("#AD_Org_ID");
        }

        /// <summary>
        ///Get Window AD_Org_ID
        /// </summary>
        /// <param name="WindowNo"> window</param>
        /// <returns></returns>
        public int GetAD_Org_ID(int windowNo)
        {
            return GetContextAsInt(windowNo, "AD_Org_ID");
        }


        /// <summary>
        ///	Get Login AD_Client_ID
        /// </summary>
        /// <returns></returns>
        public int GetAD_Client_ID()
        {
            return GetContextAsInt("#AD_Client_ID");
        }

        /// <summary>
        ///	Get Login AD_Client_ID
        /// </summary>
        /// <returns></returns>
        public string GetAD_Client_Name()
        {
            return GetContext("#AD_Client_Name");
        }

        /// <summary>
        /// Get AD_Client_ID
        /// </summary>
        /// <param name="WindowNo">Window</param>
        /// <returns>AD_Client_ID</returns>
        public int GetAD_Client_ID(int windowNo)
        {
            return GetContextAsInt(windowNo, "AD_Client_ID");
        }



        /// <summary>
        ///	Get Login AD_Client_ID
        /// </summary>
        /// <returns></returns>
        public string GetAD_Org_Name()
        {
            return GetContext("#AD_Org_Name");
        }

        /// <summary>
        /// Get Session Id from List
        /// </summary>
        /// <returns></returns>
        public int GetAD_Session_ID()
        {
            return GetContextAsInt("#AD_Session_ID");
        }

        /// <summary>
        /// Get Warehouse Id from List
        /// </summary>
        /// <returns></returns>
        public int GetAD_Warehouse_ID()
        {
            return GetContextAsInt("#AD_Warehouse_ID");
        }

        /// <summary>
        /// Get Language from List
        /// </summary>
        /// <returns></returns>
        public string GetAD_Language()
        {
            return GetContext("#AD_Language");
        }

        /// <summary>
        ///Get Value of Context for Window.
        ///if not found global context if available and enabled
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="context">context context key</param>
        /// <param name="onlyWindow">onlyWindow  if true, no defaults are used unless explicitly asked for</param>
        /// <returns></returns>
        public string GetContext(int WindowNo, string context, bool onlyWindow)
        {
            if (context == null)
                throw new ArgumentException("Require Context");
            string key = WindowNo + "|" + context;
            //if (WindowNo == 0)
            //    key = context;

            object value = null;
            //if (ctx.IndexOfKey(key) != -1)
            //{
            value = ctx[key];
            //}
            //don't use value == null 'cuz there may be a null value for this key
            //if (value == null)
            if (!ctx.ContainsKey(key))
            {
                //	Explicit Base Values
                if (context.StartsWith("#") || context.StartsWith("$"))
                    return GetContext(context);
                if (onlyWindow)			//	no Default values
                    return "";
                return GetContext("#" + context);
            }
            if (value == null || value.ToString() == "")
                return "";
            else
                return value.ToString();
        }	//	getCo

        /// <summary>
        ///	Get Value of Context for Window.
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public String GetContext(int WindowNo, String context)
        {
            return GetContext(WindowNo, context, false);
        }	//	getContext


        /// <summary>
        ///	Set Auto Commit for Window
        ///  WindowNo window no
        /// autoCommit auto commit (save)
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="autoCommit"></param>
        /// 
        public void SetAutoCommit(int WindowNo, bool autoCommit)
        {
            SetContext(WindowNo, "AutoCommit", autoCommit ? "Y" : "N");
        }	//	setAutoCommit

        /// <summary>
        ///	Set Auto New for Window
        ///  WindowNo window no
        /// autoCommit auto commit (save)
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="autoCommit"></param>
        /// 
        public void SetAutoNew(int WindowNo, bool autoNew)
        {
            SetContext(WindowNo, "AutoNew", autoNew ? "Y" : "N");
        }	//	setAutoCommit

        /// <summary>
        /// Set Key And Value In list
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="context"></param>
        /// <param name="value"></param>
        public void SetContext(int windowNo, string context, string value)
        {
            if (context == null)
                return;
            //if (ctx.ContainsKey(WindowNo.ToString() + "|" + context))
            //{
            //    ctx.Remove(WindowNo.ToString() + "|" + context);
            //}
            if (value == null || value.Equals(""))
                ctx[windowNo.ToString() + "|" + context]= null;
            else
                ctx[windowNo.ToString() + "|" + context] = value;
        }	//	

        /// <summary>
        ///	Is AutoCommit
        ///return true if auto commit
        /// </summary>
        /// <returns></returns>
        public bool IsAutoCommit()
        {
            string s = GetContext("AutoCommit");
            if (s != null && s.Equals("Y"))
                return true;
            return false;
        }	//	

        /// <summary>
        ///Is Window AutoCommit (if not set use default)
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <returns></returns>
        public bool IsAutoCommit(int windowNo)
        {
            String s = GetContext(windowNo, "AutoCommit", false);
            if (s != null)
            {
                if (s.Equals("Y"))
                    return true;
                else
                    return false;
            }
            return IsAutoCommit();
        }	//	

        /// <summary>
        ///Is Window AutoCommit (if not set use default)
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <returns></returns>
        public bool IsCacheWindow()
        {
            String s = GetContext("CacheWindows");
            if (s != null && s.Equals("Y"))
                return true;
            return false;

            //return IsAutoCommit();
        }	//	



        /// <summary>
        /// Is Auto New Record
        /// return true if auto new
        /// </summary>
        public bool IsAutoNew()
        {
            string s = GetContext("AutomaticNewRecord");
            if (s != null && s.Equals("Y"))
                return true;
            return false;
        }

        /// <summary>
        /// Is Auto New Record
        /// return true if auto new
        /// </summary>
        public bool IsAutoNew(int windowNo)
        {
            String s = GetContext(windowNo, "AutoNew", false);
            if (s != null)
            {
                if (s.Equals("Y"))
                    return true;
                else
                    return false;
            }
            return IsAutoNew();
        }

        /// <summary>
        /// If to show translation tabs on window
        /// </summary>
        /// <returns>return bool type true if show translation tab else false</returns>
        public bool IsShowTrl()
        {
            string s = GetContext("#ShowTrl");
            if (s != null && s.Equals("Y"))
                return true;
            return false;
        }

        /// <summary>
        /// Is Sales Order Trx
        /// </summary>
        /// <returns>true if SO (default)</returns>
        public bool IsSOTrx()
        {
            string s = GetContext("IsSOTrx");
            if (s != null && s.Equals("N"))
                return false;
            return true;
        }

        /// <summary>
        /// Is Sales Order Trx
        /// </summary>
        /// <param name="WindowNo">window no</param>
        /// <returns>true if SO (default)</returns>
        public bool IsSOTrx(int windowNo)
        {
            string s = GetContext(windowNo, "IsSOTrx", true);
            if (s != null && s.Equals("N"))
                return false;
            return true;
        }

        /// <summary>
        ///Set Context for Window & Tab to Value
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="TabNo"></param>
        /// <param name="context"></param>
        /// <param name="value"></param>
        public void SetContext(int WindowNo, int TabNo, string context, string value)
        {
            if (context == null)
                return;
            if (ctx.ContainsKey(WindowNo.ToString() + "|" + TabNo.ToString() + "|" + context))
            {
                ctx.Remove(WindowNo.ToString() + "|" + TabNo.ToString() + "|" + context);
            }

            //		if (WindowNo != WINDOW_FIND && WindowNo != WINDOW_MLOOKUP)
            //log.finest("C(" + WindowNo + "," + TabNo + "): " + context + "=" + value);
            //
            if (value == null || value.Equals(""))
                ctx.Add(WindowNo.ToString() + "|" + TabNo.ToString() + "|" + context, null);
            else
                ctx.Add(WindowNo.ToString() + "|" + TabNo.ToString() + "|" + context, value);
        }

        /// <summary>
        ///  Set Context for Window to int Value
        /// </summary>
        /// <param name="WindowNo">window no</param>
        /// <param name="context">context key</param>
        /// <param name="value">context value</param>
        public void SetContext(int WindowNo, String context, int value)
        {
            SetContext(WindowNo, context, value.ToString());
        }

        /// <summary>
        /// Set SO Trx
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="isSOTrx"></param>
        public void SetIsSOTrx(int WindowNo, bool isSOTrx)
        {
            SetContext(WindowNo, "IsSOTrx", isSOTrx ? "Y" : "N");
        }

        /// <summary>
        ///Set Context for Window to Y/N Value
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="context"></param>
        /// <param name="value"></param>
        public void SetContext(int WindowNo, string context, bool value)
        {
            SetContext(WindowNo, context, value ? "Y" : "N");
        }

        /// <summary>
        ///Set Context for Window to Value
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="context"></param>
        /// <param name="value"></param>
        public void SetContext(int WindowNo, string context, DateTime value)
        {
            string stringValue = null;
            if (value != null)
            {
                //long time = value.getTime();
                stringValue = value.ToString();// valueOf(time);
            }
            SetContext(WindowNo, context, stringValue);
        }

        ///<summary>
        ///Get Value of Context for Window & Tab,
        /// </summary>
        /// <param name="WindowNo">window no</param>
        /// <param name="TabNo">tab no</param>
        /// <param name="context">context key</param>
        /// <returns>value or ""</returns>
        public String GetContext(int windowNo, int tabNo, String context)
        {
            if (context == null)
                throw new ArgumentException("Require Context");
            object value;
             value =ctx[windowNo + "|" + tabNo + "|" + context];
            if (value == null)
                return GetContext(windowNo, context, false);
            return value.ToString();
        }	//	getC


        /// <summary>
        /// Clear Key Values of disposed or closed window
        /// </summary>
        public void ClearContext(int winNum)
        {
            //for (int i = 0; i < ctx.Count; i++)
            //{
                IEnumerator en = ctx.Keys.GetEnumerator();
                while (en.MoveNext())
                {
                    if (en.Current.ToString().StartsWith(winNum.ToString() + "|"))
                    {
                        ctx.Remove(en.Current);
                       en = ctx.Keys.GetEnumerator();
                    }
                }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            ctx.Clear();
            // ctx = null;
        }

        /// <summary>
        /// Get List Of Keys
        /// </summary>
        /// <returns></returns>
        public ICollection GetKeys()
        {
            return ctx.Keys;
        }
        /// <summary>
        /// RemoveKEy
        /// </summary>
        /// <param name="key"></param>
        public void Remove(object key)
        {
            ctx.Remove(key);
        }

        /// <summary>
        /// Clear List
        /// </summary>
        public void Clear()
        {
            ctx.Clear();
        }

        /// <summary>
        ///Get Primary Accounting Schema Currency Precision
        /// </summary>
        /// <returns></returns>
        public int GetStdPrecision()
        {
            return GetContextAsInt("#StdPrecision");
        }	//	getStdPrecision

        /// <summary>
        ///	Set Primary Accounting Schema Currency Precision
        /// </summary>
        /// <param name="precision"></param>
        public void SetStdPrecision(int precision)
        {
            SetContext("#StdPrecision", precision);
        }	//	setStdPrecision

        /// <summary>
        ///Set Global Context to (int) Value
        /// </summary>
        /// <param name="context"></param>
        /// <param name="value"></param>
        public void SetContext(String context, int value)
        {
            SetContext(context, value.ToString());
        }

       /// <summary>
       ///Get sorted Context as String array with format: key == value
       /// </summary>
       /// <returns></returns>
        public String[] GetEntireContext()
        {
            IEnumerator en = ctx.Keys.GetEnumerator();
            List<String> sList = new  List<String>(ctx.Count);
            while (en.MoveNext())
            {
                Object key = en.Current;
                String s = key + " == " + Get(key);
                sList.Add(s);
            }

            String[] retValue = new String[sList.Count];
            retValue = sList.ToArray();
            Array.Sort(retValue);
            return retValue;
        }	//	get

        public Object Get(Object key)
        {
            return ctx[key];
        }
    }
    */
    #endregion


    #region "New Context Class"

    /// <summary>
    /// this Class implement generic dictionary to store context value
    /// and aother dictionanry to store window number and its context values
    /// </summary>
    [Serializable()]
    public class Context : Utility.Ctx
    {
        /* store window context*/
        private IDictionary<int, IDictionary<string, string>> windows = new Dictionary<int, IDictionary<string, string>>();

        /// <summary>
        /// Context
        /// </summary>
        public Context()
            : base()
        {
        }

        /// <summary>
        /// Context construtor .load info from list
        /// </summary>
        /// <param name="map">Dictionary</param>
        public Context(IDictionary<String, String> map)
            : base(map)
        {
        }

        /// <summary>
        /// Context Construtor get info from string value
        /// </summary>
        /// <param name="stringRepresentation">text</param>
        public Context(String stringRepresentation)
            : base(stringRepresentation)
        {

        }

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="ctx">Context Object</param>
        public Context(Context ctx)
            : base(ctx.m_map)
        {

        }


        /// <summary>
        ///Get context Map with spaces as null
        /// </summary>
        /// <param name="windowNo">window number</param>
        /// <returns></returns>
        public override IDictionary<String, String> GetMap(int windowNo)
        {
            return GetMap(windowNo, false);
        }

        /// <summary>
        ///Get Context for Window as Map
        /// </summary>
        /// <param name="windowNo">window number</param>
        /// <param name="convertNullToEmptyString">bool value indicate null value convert to emoty string</param>
        /// <returns></returns>
        public override IDictionary<String, String> GetMap(int windowNo, bool convertNullToEmptyString)
        {
            IDictionary<String, String> map = windows[windowNo];
            if (!convertNullToEmptyString)
                return map;
            //
            IDictionary<String, String> newMap = new Dictionary<String, String>();
            //Set<Entry<String, String>> set = (Set<Entry<String, String>>)map.entrySet();
            IEnumerator<KeyValuePair<string, string>> en = map.GetEnumerator();// set.iterator();
            while (en.MoveNext())// it.hasNext())
            {
                //Map.Entry<String, String> entry = it.next();
                String key = en.Current.Key;// entry.getKey();
                String value = en.Current.Value; // entry.getValue();
                if (convertNullToEmptyString && value == null)
                    newMap[key] = "";
                else
                    newMap[key] = value;
            }
            return newMap;
        }

        /// <summary>
        ///Remove context for Window (i.e. delete it)
        /// </summary>
        /// <param name="windowNo">window number</param>
        public override void RemoveWindow(int windowNo)
        {
            windows.Remove(windowNo);
        }

        /// <summary>
        /// Add Window Context
        /// </summary>
        /// <param name="?">window number</param>
        public override void  AddWindow(int windowNo, IDictionary<string, string> map)
        {
            //Set set = map.entrySet();
            //Iterator it = set.iterator();
            IEnumerator<KeyValuePair<string, string>> en = map.GetEnumerator();
            while (en.MoveNext())//  it.hasNext())
            {
                //Map.Entry entry = (Map.Entry)it.next();
                String key = en.Current.Key;
                int index = key.IndexOf('|');
                /*if (key.indexOf('|') != -1)
                {
                    int wNo = Integer.parseInt(key.substring(0,index));
                    key = key.substring(index + 1);
                    Object value = entry.getValue();
                    String newValue = Null.NULLString;
                    if (value != null)
                        newValue = value.toString();
                    setContext(wNo, key, newValue);
                }
                */
                if (index != -1)
                {
                    m_map[key] = map[key];
                    map.Remove(key);
                    en = map.GetEnumerator();
                }
            }
            windows[windowNo] = map;
        }

        /// <summary>
        ///Clear values
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            windows.Clear();
        }

        /// <summary>
        ///Get sorted Context as String array with format: key == value
        /// </summary>
        /// <returns></returns>
        public override String[] GetEntireContext()
        {
            IDictionary<string, string> map = GetMap();

            IEnumerator<KeyValuePair<string, string>> en = map.GetEnumerator();
            List<String> sList = new List<String>(map.Count);
            while (en.MoveNext())
            {
                Object key = en.Current.Key;
                String s = key + " == " + en.Current.Value;
                sList.Add(s);
            }

            String[] retValue = new String[sList.Count];
            retValue = sList.ToArray();
            Array.Sort(retValue);
            return retValue;
        }

        /// <summary>
        /// check context Contains Key
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>true if context contain key</returns>
        public override bool ContainsKey(String key)
        {
            if (base.ContainsKey(key))
                return true;
            if (key == null)
                return false;
            IEnumerator<KeyValuePair<int, IDictionary<String, String>>> itWin = windows.GetEnumerator();
            while (itWin.MoveNext())
            {
                KeyValuePair<int, IDictionary<string, string>> entryWin = itWin.Current;
                int windowNo = entryWin.Key;
                if (!key.StartsWith(windowNo.ToString()))
                    continue;
                IEnumerator<String> itEntries = entryWin.Value.Keys.GetEnumerator();// .entrySet().iterator();
                while (itEntries.MoveNext())
                {
                    if (key.Equals(itEntries.Current))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        ///has Value in context
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>true if contain</returns>
        public override bool ContainsValue(string value)
        {
            if (base.ContainsValue(value))
                return true;
            if (value == null)
                return false;
            IEnumerator<KeyValuePair<int, IDictionary<String, String>>> itWin = windows.GetEnumerator();// .entrySet().iterator();
            while (itWin.MoveNext())
            {
                KeyValuePair<int, IDictionary<String, String>> entryWin = itWin.Current;
                IEnumerator<string> itEntries = entryWin.Value.Values.GetEnumerator();
                while (itEntries.MoveNext())
                {
                    //Entry<String,String> entry = itEntries.next();
                    if (value.Equals(itEntries.Current))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get Context data as List
        /// </summary>
        /// <returns>list of key value pair</returns>
        public override IDictionary<String, String> GetMap()
        {
            IDictionary<String, String> map = new Dictionary<string, string>(base.GetMap());
            IEnumerator<KeyValuePair<int, IDictionary<String, String>>> itWin = windows.GetEnumerator();// .GetEnumerator();//  .entrySet().iterator();
            while (itWin.MoveNext())
            {
                KeyValuePair<int, IDictionary<String, String>> entryWin = itWin.Current;
                int windowNo = entryWin.Key;
                IEnumerator<KeyValuePair<String, String>> itEntries = entryWin.Value.GetEnumerator();//.entrySet().iterator();
                while (itEntries.MoveNext())
                {
                    KeyValuePair<String, String> entry = itEntries.Current;
                    map[windowNo + "|" + entry.Key] = entry.Value;
                }
            }
            return map;
        }

        /// <summary>
        /// context of all windows
        /// </summary>
        /// <returns></returns>
        public new IDictionary<String, String> EntrySet()
        {
            return GetMap();
        }

        /// <summary>
        ///Is context Empty
        /// </summary>
        /// <returns>true if empty</returns>
        public override bool IsEmpty()
        {
            bool empty = base.IsEmpty();
            if (!empty)
                return false;
            IEnumerator<KeyValuePair<int, IDictionary<string, string>>> itWin = windows.GetEnumerator();
            while (itWin.MoveNext())
            {
                KeyValuePair<int, IDictionary<String, String>> entryWin = itWin.Current;
                ICollection<string> itEntries = entryWin.Value.Values;
                if (itEntries.Count > 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Key Collection
        /// </summary>
        /// <returns>return key collection</returns>
        public ICollection<string> KeySet()
        {
            return GetMap().Keys;
        }

        /// <summary>
        ///return values collection
        /// </summary>
        /// <returns></returns>
        public override ICollection<string> Values()
        {
            return GetMap().Values;
        }

        /**************************************************************************/
        /// <summary>
        ///Set Context for Window 
        /// </summary>
        /// <param name="windowNo">window no</param>
        /// <param name="context">key</param>
        /// <param name="value">value</param>
        public override void SetContext(int windowNo, String context, String value)
        {
            if (context == null)
                return;
            //		if (WindowNo != WINDOW_FIND && WindowNo != WINDOW_MLOOKUP)
            //log.finer("("+windowNo+"): " + context + "=" + value);

            IDictionary<String, String> map;
            windows.TryGetValue(windowNo, out map);
            if (map == null)
            {
                map = new Dictionary<String, String>();
                windows[windowNo] = map;
            }
            if (value == null || value.Equals(""))
                map[context] = null;
            //	map.remove(context);
            else
                map[context] = value;
        }

        /// <summary>
        ///Get Value of Context for Window.
        /// 	if not found global context if available and enabled
        /// </summary>
        /// <param name="windowNo">window number</param>
        /// <param name="context">key string</param>
        /// <param name="onlyWindow">check against only window number</param>
        /// <returns>value against key</returns>
        public override String GetContext(int windowNo, String context, bool onlyWindow)
        {
            if (context == null)
                throw new ArgumentException("Require Context");
            IDictionary<String, String> map;
            windows.TryGetValue(windowNo, out map);
            if (map != null)
            {
                String value;
                map.TryGetValue(context, out value);
                if (value == null)
                {
                    //	Explicit Base Values
                    if (context.StartsWith("#") || context.StartsWith("$"))
                        return GetContext(context);
                    if (onlyWindow)			//	no Default values
                        return "";
                    return GetContext("#" + context);
                }
                return value;
            }
            else
            {
                if (context.StartsWith("#") || context.StartsWith("$"))
                    return GetContext(context);
                if (!onlyWindow)
                    return GetContext("#" + context);
            }
            return "";
        }	//	getContext

        /// <summary>
        ///Add Map elements to context
        /// </summary>
        /// <param name="windowNo">window number</param>
        /// <param name="addContext">list of string value</param>
        public override void SetContext(int windowNo, IDictionary<string, string> addContext)
        {
            if (addContext == null)
                return;
            IDictionary<String, String> map = windows[windowNo];
            if (map == null)
            {
                map = new Dictionary<String, String>();
                windows[windowNo] = map;
            }

            //ZD, to keep the semantics right, remove keys that has "" or null values
            IEnumerator<KeyValuePair<string, string>> it = addContext.GetEnumerator();
            while (it.MoveNext())
            {
                String key = it.Current.Key;
                String value = it.Current.Value;
                if (value != null && value.Length > 0)
                    map[key] = value;
                else
                    map[key] = null;
            }
        }

        
        // public static final:  JLS says recommended style is to omit these modifiers
        // because they are the default

        /**
         * Constant that holds the name of the environment property
         * for specifying the initial context factory to use. The value
         * of the property should be the fully qualified class name
         * of the factory class that will create an initial context.
         * This property may be specified in the environment parameter
         * passed to the initial context constructor, an applet parameter,
         * a system property, or an application resource file.
         * If it is not specified in any of these sources,
         * <tt>NoInitialContextException</tt> is thrown when an initial
         * context is required to complete an operation.
         *
         * <p> The value of this constant is "java.naming.factory.initial".
         *
         * @see InitialContext
         * @see javax.naming.directory.InitialDirContext
         * @see javax.naming.spi.NamingManager#getInitialContext
         * @see javax.naming.spi.InitialContextFactory
         * @see NoInitialContextException
         * @see #addToEnvironment(String, Object)
         * @see #removeFromEnvironment(String)
         * @see #APPLET
         */
        public static String INITIAL_CONTEXT_FACTORY = "java.naming.factory.initial";

        /**
         * Constant that holds the name of the environment property
         * for specifying the list of object factories to use. The value
         * of the property should be a colon-separated list of the fully
         * qualified class names of factory classes that will create an object
         * given information about the object.
         * This property may be specified in the environment, an applet
         * parameter, a system property, or one or more resource files.
         *
         * <p> The value of this constant is "java.naming.factory.object".
         *
         * @see javax.naming.spi.NamingManager#getObjectInstance
         * @see javax.naming.spi.ObjectFactory
         * @see #addToEnvironment(String, Object)
         * @see #removeFromEnvironment(String)
         * @see #APPLET
         */
        public static String OBJECT_FACTORIES = "java.naming.factory.object";

        /**
         * Constant that holds the name of the environment property
         * for specifying the list of state factories to use. The value
         * of the property should be a colon-separated list of the fully
         * qualified class names of state factory classes that will be used
         * to get an object's state given the object itself.
         * This property may be specified in the environment, an applet
         * parameter, a system property, or one or more resource files.
         *
         * <p> The value of this constant is "java.naming.factory.state".
         *
         * @see javax.naming.spi.NamingManager#getStateToBind
         * @see javax.naming.spi.StateFactory
         * @see #addToEnvironment(String, Object)
         * @see #removeFromEnvironment(String)
         * @see #APPLET
         * @since 1.3
         */
        public static String STATE_FACTORIES = "java.naming.factory.state";

        /**
         * Constant that holds the name of the environment property
         * for specifying the list of package prefixes to use when
         * loading in URL context factories. The value
         * of the property should be a colon-separated list of package
         * prefixes for the class name of the factory class that will create
         * a URL context factory.
         * This property may be specified in the environment,
         * an applet parameter, a system property, or one or more
         * resource files.
         * The prefix <tt>com.sun.jndi.url</tt> is always appended to
         * the possibly empty list of package prefixes.
         *
         * <p> The value of this constant is "java.naming.factory.url.pkgs".
         *
         * @see javax.naming.spi.NamingManager#getObjectInstance
         * @see javax.naming.spi.NamingManager#getURLContext
         * @see javax.naming.spi.ObjectFactory
         * @see #addToEnvironment(String, Object)
         * @see #removeFromEnvironment(String)
         * @see #APPLET
          */
        public static String URL_PKG_PREFIXES = "java.naming.factory.url.pkgs";

        /**
         * Constant that holds the name of the environment property
         * for specifying configuration information for the service provider
         * to use. The value of the property should contain a URL string
         * (e.g. "ldap://somehost:389").
         * This property may be specified in the environment,
         * an applet parameter, a system property, or a resource file.
         * If it is not specified in any of these sources,
         * the default configuration is determined by the service provider.
         *
         * <p> The value of this constant is "java.naming.provider.url".
         *
         * @see #addToEnvironment(String, Object)
         * @see #removeFromEnvironment(String)
         * @see #APPLET
         */
        //public static string PROVIDER_URL = "tcp://localhost:2090/Vframwork";
        public static String PROVIDER_URL = "java.naming.provider.url";

        /**
         * Constant that holds the name of the environment property
         * for specifying the DNS host and domain names to use for the
         * JNDI URL context (for example, "dns://somehost/wiz.com").
         * This property may be specified in the environment,
         * an applet parameter, a system property, or a resource file.
         * If it is not specified in any of these sources
         * and the program attempts to use a JNDI URL containing a DNS name,
         * a <tt>ConfigurationException</tt> will be thrown.
         *
         * <p> The value of this constant is "java.naming.dns.url".
         *
         * @see #addToEnvironment(String, Object)
         * @see #removeFromEnvironment(String)
         */
        public static String DNS_URL = "java.naming.dns.url";

        /**
         * Constant that holds the name of the environment property for
         * specifying the authoritativeness of the service requested.
         * If the value of the property is the string "true", it means
         * that the access is to the most authoritative source (i.e. bypass
         * any cache or replicas). If the value is anything else,
         * the source need not be (but may be) authoritative.
         * If unspecified, the value defaults to "false".
         *
         * <p> The value of this constant is "java.naming.authoritative".
         *
         * @see #addToEnvironment(String, Object)
         * @see #removeFromEnvironment(String)
         */
        public static String AUTHORITATIVE = "java.naming.authoritative";

        /**
         * Constant that holds the name of the environment property for
         * specifying the batch size to use when returning data via the
         * service's protocol. This is a hint to the provider to return
         * the results of operations in batches of the specified size, so
         * the provider can optimize its performance and usage of resources.
         * The value of the property is the string representation of an
         * integer.
         * If unspecified, the batch size is determined by the service
         * provider.
         *
         * <p> The value of this constant is "java.naming.batchsize".
         *
         * @see #addToEnvironment(String, Object)
         * @see #removeFromEnvironment(String)
         */
        public static String BATCHSIZE = "java.naming.batchsize";

        /**
         * Constant that holds the name of the environment property for
         * specifying how referrals encountered by the service provider
         * are to be processed. The value of the property is one of the
         * following strings:
         * <dl>
         * <dt>"follow"
         * <dd>follow referrals automatically
         * <dt>"ignore"
         * <dd>ignore referrals
         * <dt>"throw"
         * <dd>throw <tt>ReferralException</tt> when a referral is encountered.
         * </dl>
         * If this property is not specified, the default is
         * determined by the provider.
         *
         * <p> The value of this constant is "java.naming.referral".
         *
         * @see #addToEnvironment(String, Object)
         * @see #removeFromEnvironment(String)
         */
        public static String REFERRAL = "java.naming.referral";

        /**
         * Constant that holds the name of the environment property for
         * specifying the security protocol to use.
         * Its value is a string determined by the service provider
         * (e.g. "ssl").
         * If this property is unspecified,
         * the behaviour is determined by the service provider.
         *
         * <p> The value of this constant is "java.naming.security.protocol".
         *
         * @see #addToEnvironment(String, Object)
         * @see #removeFromEnvironment(String)
         */
        public static String SECURITY_PROTOCOL = "java.naming.security.protocol";

        /**
         * Constant that holds the name of the environment property for
         * specifying the security level to use.
         * Its value is one of the following strings:
         * "none", "simple", "strong".
         * If this property is unspecified,
         * the behaviour is determined by the service provider.
         *
         * <p> The value of this constant is "java.naming.security.authentication".
         *
         * @see #addToEnvironment(String, Object)
         * @see #removeFromEnvironment(String)
         */
        public static String SECURITY_AUTHENTICATION = "java.naming.security.authentication";

        /**
         * Constant that holds the name of the environment property for
         * specifying the identity of the principal for authenticating
         * the caller to the service. The format of the principal
         * depends on the authentication scheme.
         * If this property is unspecified,
         * the behaviour is determined by the service provider.
         *
         * <p> The value of this constant is "java.naming.security.principal".
         *
         * @see #addToEnvironment(String, Object)
         * @see #removeFromEnvironment(String)
         */
        public static String SECURITY_PRINCIPAL = "System.Security.Principal";// "java.naming.security.principal";

        /**
         * Constant that holds the name of the environment property for
         * specifying the credentials of the principal for authenticating
         * the caller to the service. The value of the property depends
         * on the authentication scheme. For example, it could be a hashed
         * password, clear-text password, key, certificate, and so on.
         * If this property is unspecified,
         * the behaviour is determined by the service provider.
         *
         * <p> The value of this constant is "java.naming.security.credentials".
         *
         * @see #addToEnvironment(String, Object)
         * @see #removeFromEnvironment(String)
         */

        public static String SECURITY_CREDENTIALS = "System.Security.Cryptography";// "java.naming.security.credentials";
        /**
         * Constant that holds the name of the environment property for
         * specifying the preferred language to use with the service.
         * The value of the property is a colon-separated list of language
         * tags as defined in RFC 1766.
         * If this property is unspecified,
         * the language preference is determined by the service provider.
         *
         * <p> The value of this constant is "java.naming.language".
         *
         * @see #addToEnvironment(String, Object)
         * @see #removeFromEnvironment(String)
         */
        public static String LANGUAGE = "java.naming.language";

        /**
         * Constant that holds the name of the environment property for
         * specifying an applet for the initial context constructor to use
         * when searching for other properties.
         * The value of this property is the
         * <tt>java.applet.Applet</tt> instance that is being executed.
         * This property may be specified in the environment parameter
         * passed to the initial context constructor.
         * When this property is set, each property that the initial context
         * constructor looks for in the system properties is first looked for
         * in the applet's parameter list.
         * If this property is unspecified, the initial context constructor
         * will search for properties only in the environment parameter
         * passed to it, the system properties, and application resource files.
         *
         * <p> The value of this constant is "java.naming.applet".
         *
         * @see #addToEnvironment(String, Object)
         * @see #removeFromEnvironment(String)
         * @see InitialContext
         *
         * @since 1.3
         */
        public static String APPLET = "java.naming.applet";

       /// <summary>
       /// The class fingerprint that is set to indicate
       /// serialization compatibility with a previous
       /// version of the class.
       /// From Name Interface
       /// </summary>
       // public static long serialVersionUID = -3617482732056931635L;

    }

    #endregion

   

}
