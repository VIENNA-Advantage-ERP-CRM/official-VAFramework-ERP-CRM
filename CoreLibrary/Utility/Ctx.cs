/********************************************************
 * Module Name    : Show Tree
 * Purpose        : used to store login information and other setting(user preference etc)
 * Class Used     : ---
 * Created By     : Harwinder 
 * Date           : 20-1-2009
**********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using VAdvantage.Classes;

namespace VAdvantage.Utility
{
    #region"CTX"
    [Serializable]
    public  class Ctx : Dictionary<string, string>, Evaluatee
    { 
        /// <summary>
        /// Constructor
        /// </summary>
        public Ctx()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="map"></param>
        public Ctx(IDictionary<string,string> map) : this()
        {
            Load(map);
        }	//	Ctx

         public Ctx(IDictionary<string,object> map) : this()
        {
            Load(map);
        }	

      /// <summary>
      ///	Context
      /// </summary>
        /// <param name="stringRepresentation"> stringRepresentation example {AD_Org_ID=11}</param>
        public Ctx(String stringRepresentation) : this()
        {
            Load(stringRepresentation);
        }	//	Ctx

        

        /* store key ,value */
        protected Dictionary<string,string> m_map = new Dictionary<string,string>() ;// Hashtable();
	/**	Logger		*/
	
        public Logging.VLogger log = Logging.VLogger.GetVLogger(typeof(Ctx).FullName);

    /// <summary>
    ///	Load list from string representaion
    /// </summary>
    /// <param name="stringRepresentation">stringRepresentation example {AD_Org_ID=11, IsDefault=N, IsActive=Y, Greeting=Mr, Name=Mr, VAB_Greeting_ID=100, AD_Client_ID=11, IsFirstNameOnly=N}</param>
       protected void Load(String stringRepresentation)
       {
           if (stringRepresentation == null
                   || !stringRepresentation.StartsWith("{")
                   || !stringRepresentation.EndsWith("}"))
               throw new ArgumentException("Cannot load: " + stringRepresentation);
           String string1 = stringRepresentation.Substring(1, stringRepresentation.Length - 1);
           StringTokenizer st = new StringTokenizer(string1, ",");
           while (st.HasMoreTokens())
           {
               String pair = st.NextToken().Trim();
               if (pair.Length == 0)
                   continue;
               int index = pair.IndexOf("=");
               if (index == -1)
               {
                   //log.warning("Load invalid format: " + pair);
                   continue;
               }
               String key = pair.Substring(0, index);
               String value = pair.Substring(index + 1);
               m_map[key] = value;
           }
           log.Info(ToString());
          // //Common.//ErrorLog.FillErrorLog("ctx.Load()", "", ToString(),VAdvantage.Framework.Message.MessageType.INFORMATION);
       }

        /// <summary>
        ///	Load context list from map
        /// </summary>
        /// <param name="map">dictionary Map</param>
        private void Load(IDictionary<string,string> map)
       {
           IEnumerator<KeyValuePair<string,string>> en =  map.GetEnumerator();
           while (en.MoveNext())// it.hasNext())
           {
               //Map.Entry entry = (Map.Entry)it.next();
               String key = en.Current.Key;
               String value = en.Current.Value;
               SetContext(key, value);
           }
       }

        private void Load(IDictionary<string, object> map)
        {
            IEnumerator<KeyValuePair<string, object>> en = map.GetEnumerator();
            while (en.MoveNext())// it.hasNext())
            {
                //Map.Entry entry = (Map.Entry)it.next();
                String key = en.Current.Key;
                String value = Convert.ToString(en.Current.Value);
                SetContext(key, value);
            }
        }
        /// <summary>
       /// Set Global Context to Value
       /// </summary>
       /// <param name="context">key</param>
       /// <param name="value">value</param>
       /// <returns>new Value</returns>
        public string SetContext(String context, String value)
        {
            if (context == null)    
                return null;

            //log.Finer(context + "=" + value);
            //
            if (value == null || value.Equals(""))
                return m_map[context] = null;
            else
                return m_map[context] = value;
        }

        /// <summary>
       ///Set Global Context to (int) Value
       /// </summary>
       /// <param name="context">key</param>
       /// <param name="value">value</param>
       /// <returns>new value</returns>
        public string SetContext(String context, int value)
        {
            return SetContext(context, value.ToString());
        }	

        /// <summary>
	    ///	Set Global Context to Y/N Value
        /// </summary>
        /// <param name="context">text</param>
        /// <param name="value">value</param>
        /// <returns>new value</returns>
        public string SetContext(String context, bool value)
        {
            return SetContext(context, value ? "Y" : "N");
        }	

        /// <summary>
        ///Set Context for Window to Value
        /// </summary>
        /// <param name="WindowNo">window number </param>
        /// <param name="context">key</param>
        /// <param name="value">value</param>
        public virtual void SetContext(int windowNo, String context, String value)
        {

            if (context == null)
                return;
            //		if (WindowNo != WINDOW_FIND && WindowNo != WINDOW_MLOOKUP)
            //if (Build.isVerbose())
            //    log.finer("(" + WindowNo + "): " + context + "=" + value);
            //
            if (value == null || value.Equals(""))
                //	m_map.remove(WindowNo+"|"+context);
                m_map[windowNo + "|" + context]= null;
            else
                m_map[windowNo + "|" + context] =  value;
        }	//	setC

        /// <summary>
        ///Add Map elements to context
        /// </summary>
        /// <param name="WindowNo">window number</param>
        /// <param name="addContext">list</param>
        public virtual void SetContext(int windowNo, IDictionary<string,string> addContext)
        {
            if (addContext == null)
                return;
            
            IEnumerator  it = addContext.Keys.GetEnumerator();
            while (it.MoveNext())
            {
                String key = (String)it.Current;
                String value;
                addContext.TryGetValue(key,out value);
                SetContext(windowNo, key, value);
            }
        }
        
        /// <summary>
        ///Set Context for Window to Value
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="context"></param>
        /// <param name="value"></param>
        public void SetContext(int windowNo, String context, DateTime value)
        {
           // String stringValue = null;
            if (value != null)
            {
               // long time = Convert.ToInt64(value);
               // stringValue = time.ToString();
            }
            SetContext(windowNo, context, value.ToString());
        }

        /// <summary>
        ///Set Context for Window to int Value
        /// </summary>
        /// <param name="WindowNo">window number</param>
        /// <param name="context">key</param>
        /// <param name="value">value</param>
        public void SetContext(int windowNo, String context, int value)
        {
            SetContext(windowNo, context, value.ToString());
        }

        /// <summary>
        ///Set Context for Window to Y/N Value
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="context"></param>
        /// <param name="value"></param>
        public void SetContext(int windowNo, String context, bool value)
        {
            SetContext(windowNo, context, value ? "Y" : "N");
        }
            
        /// <summary>
        ///Set Context for Window & Tab to Value
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="TabNo"></param>
        /// <param name="context"></param>
        /// <param name="value"></param>
        public void SetContext(int windowNo, int tabNo, String context, String value)
        {
            if (context == null)
                return;
            //		if (WindowNo != WINDOW_FIND && WindowNo != WINDOW_MLOOKUP)
            log.Finest("C(" + windowNo + "," + tabNo + "): " + context + "=" + value);
            //
            if (value == null || value.Equals(""))
                m_map[windowNo + "|" + tabNo + "|" + context] =  null;
            //	m_map.remove(WindowNo+"|"+TabNo+"|"+context);
            else
                m_map[windowNo + "|" + tabNo + "|" + context] =  value;
        }	
      
        /// <summary>
        ///Set Auto Commit
        /// <param name="autoCommit"></param>
        public void SetAutoCommit(bool autoCommit)
        {
            m_map["AutoCommit"] = autoCommit ? "Y" : "N";
        }

        /// <summary>
        ///Set Auto Commit for Window
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="autoCommit"></param>
        public void SetAutoCommit(int windowNo, bool autoCommit)
        {
            SetContext(windowNo, "AutoCommit", autoCommit ? "Y" : "N");
        }

       /// <summary>
       ///Set Auto New Record
       /// </summary>
       /// <param name="autoNew"></param>
        public void SetAutoNew(bool autoNew)
        {
            m_map["AutoNew"] =  autoNew ? "Y" : "N";
        }

        public void SetShowClientOrg(string hide)
        {
            SetContext("#ShowClientOrg", hide);
        }

       public void SetShowMiniGrid(string hide)
        {
            SetContext("#ShowMiniGrid", hide);
        }
        //public void SetHideClientOrg(int windowNo,string hide)
        //{
        //    SetContext(windowNo, "#HideClientOrg", hide);
        //}

        public bool GetShowClientOrg()
        {
          return   GetContext("#ShowClientOrg")=="Y"?true:false;
        }

        public bool GetShowMiniGrid()
        {
            return GetContext("#ShowMiniGrid") == "Y" ? true : false;
        }
        /// <summary>
        ///Set Auto New Record for Window
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="autoNew"></param>
        public void SetAutoNew(int windowNo, bool autoNew)
        {
            SetContext(windowNo, "AutoNew", autoNew ? "Y" : "N");
        }

       /// <summary>
       ///Set SO Trx
       /// </summary>
       /// <param name="isSOTrx"></param>
        public void SetIsSOTrx(bool isSOTrx)
        {
            m_map["IsSOTrx"] =  isSOTrx ? "Y" : "N";
        }

        /// <summary>
        ///Set SO Trx
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="isSOTrx"></param>
        public void SetIsSOTrx(int windowNo, bool isSOTrx)
        {
            SetContext(windowNo, "IsSOTrx", isSOTrx ? "Y" : "N");
        }

        /// <summary>
        ///	Get Primary Accounting Schema Currency Precision
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
        }

    /// <summary>
    ///	Set Printer Name
    /// </summary>
    /// <param name="printerName"></param>
        public void SetPrinterName(String printerName)
        {
            SetContext("#Printer", printerName);
        }	//	setPrinterName

        /// <summary>
        ///Get Printer Name
        /// </summary>
        /// <returns></returns>
        public String GetPrinterName()
        {
            return GetContext("#Printer");
        }

        /// <summary>
        ///Get Printer Name
        /// </summary>
        /// <returns></returns>
        public String GetSecureKey()
        {
            return GetContext("#key");
        }

        public void SetSecureKey(string key)
        {
            SetContext("#key", key);
        }


        public bool GetIsRightToLeft()
        {
            return GetContext("#IsRightToLeft") == "Y";
        }


        /// <summary>
        ///Get global Value of Context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public String GetContext(String context)
        {
            if (context == null)
                throw new ArgumentException("Require Context");
            string value;
            m_map.TryGetValue(context , out value);
            //m_map[context];
            if (value == null)
            {
                if (context.Equals("#AD_User_ID"))
                    return GetContext("#" + context);
                if (context.Equals("#AD_User_Name"))
                    return GetContext("#" + context);
                return "";
            }
            return value;
        }

        /// <summary>
        ///Get Value of Context for Window.
         ///	if not found global context if available and enabled
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="context"></param>
        /// <param name="onlyWindow"></param>
        /// <returns></returns>
        public virtual String GetContext(int windowNo, String context, bool onlyWindow)
        {
            if (context == null)
                throw new ArgumentException("Require Context");
            String key = windowNo + "|" + context;
            //if (windowNo == 0)
            //    key = context;
            string  value;
            m_map.TryGetValue(key,out value);
            //don't use value == null 'cuz there may be a null value for this key
            //if (value == null)
            if (!m_map.ContainsKey(key))
            {
                //	Explicit Base Values
                if (context.StartsWith("#") || context.StartsWith("$"))
                    return GetContext(context);
                if (onlyWindow)			//	no Default values
                    return "";
                return GetContext("#" + context);
            }
            if (value == null)
                return "";
            else
                return value;
        }

        /// <summary>
        ///Get Value of Context for Window.
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public String GetContext(int windowNo, String context)
        {
            return GetContext(windowNo, context, false);
        }	

        /// <summary>
        ///Get Value of Context for Window & Tab,
        ///	if not found global context if available
        /// </summary>
        /// <param name="WindowNo">window number</param>
        /// <param name="TabNo">ab number</param>
        /// <param name="context">key</param>
        /// <returns>*  @return value or ""</returns>
        public String GetContext(int windowNo, int tabNo, String context)
        {
            if (context == null)
                throw new ArgumentException("Require Context");
            string value;
            m_map.TryGetValue(windowNo + "|" + tabNo + "|" + context,out value);
            if (value == null)
                return GetContext(windowNo, context, false);
            return value;
        }

        /// <summary>
        ///Get Context and convert it to an integer (0 if error)
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public int GetContextAsInt(String context)
        {
            if (context == null)
                throw new ArgumentException("Require Context");
            String value = GetContext(context);
            if (value.Length == 0)
                value = GetContext(0, context, false);		//	search 0 and defaults
            if (value.Length == 0)
                return 0;
            //
            try
            {
                return int.Parse(value);
            }
            catch (Exception e)
            {
                //Common.//ErrorLog.FillErrorLog("Ctx.GetContextAsInt", value, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                log.Warning("(" + context + ") = " + value + " - " + e.Message);
            }
            return 0;
        }

        /// <summary>
        ///Get Context and convert it to an integer (0 if error)
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public int GetContextAsInt(int windowNo, String context)
        {
            String s = GetContext(windowNo, context, false);
            if (s.Length == 0)
                return 0;
            //
            try
            {
                return int.Parse(s);
            }
            catch (FormatException e)
            {
                //Common.//ErrorLog.FillErrorLog("Ctx.GetContextAsInt", s, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                log.Warning("(" + context + ") = " + s + " - " + e.Message);
            }
            return 0;
        }	//	getContextAsInt

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
                //Common.//ErrorLog.FillErrorLog("Ctx.GetContextAsInt", s, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
                log.Warning("(" + context + ") = " + s + " - " + e.Message);
            }
            return 0;
        }	
        
        /// <summary>
        ///Is AutoCommit
        /// </summary>
        /// <returns></returns>
        public bool IsAutoCommit()
        {
            String s = GetContext("AutoCommit");
            if (s != null && s.Equals("Y"))
                return true;
            return false;
        }

        /// <summary>
        /// Is Window AutoCommit (if not set use default)
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
        }

       /// <summary>
       /// 	Is Auto New Record
       /// </summary>
       /// <returns></returns>
        public bool IsAutoNew()
        {
            String s = GetContext("AutoNew");
            if (s != null && s.Equals("Y"))
                return true;
            return false;
        }	//	isAutoNew

        /// <summary>
        ///Is Window Auto New Record (if not set use default)
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <returns></returns>
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
        }	//	isAutoNew

       /// <summary>
       /// set IsSOTrx
       /// </summary>
       /// <returns></returns>
        public bool IsSOTrx()
        {
            String s = GetContext("IsSOTrx");
            if (s != null && s.Equals("N"))
                return false;
            return true;
        }	

        /// <summary>
        ///Is Sales Order Trx
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <returns></returns>
        public bool IsSOTrx(int windowNo)
        {
            String s = GetContext(windowNo, "IsSOTrx", true);
            if (s != null && s.Equals("N"))
                return false;
            return true;
        }

        /// <summary>
        /// Get Context and convert it to a Timestamp
        ///	if error return today's date
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public long GetContextAsTime(String context)
        {
            return GetContextAsTime(0, context);
        }	//	getContextAsDate

        /// <summary>
        ///	Get Context and convert it to a Timestamp
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public long GetContextAsTime(int windowNo, String context)
        {
            if (context == null)
                throw new ArgumentException("Require Context");
            String s = GetContext(windowNo, context, false);
            if (s == null || s.Equals(""))
            {
                log.Warning("No value for: " + context);
                return  CommonFunctions.CurrentTimeMillis();//  System.DateTime. .currentTimeMillis();
                //return Convert.ToInt64(DateTime.Now);
            }
            try
            {
                return long.Parse(s);
            }
            catch (FormatException e)
            {
                log.Warning("(" + context + ") = " + s + " - " + e.Message);
            }
            return CommonFunctions.CurrentTimeMillis();// Convert.ToInt64(DateTime.Now);// System.currentTimeMillis();
        }

     /// <summary>
     ///Get Login AD_Client_ID
     /// </summary>
     /// <returns></returns>
        public int GetAD_Client_ID()
        {
            return GetContextAsInt("#AD_Client_ID");
        }
	
        /// <summary>
        ///Set Login AD_Client_ID
        /// </summary>
        /// <param name="AD_Client_ID"></param>
        public void SetAD_Client_ID(int AD_Client_ID)
        {
            SetContext("#AD_Client_ID", AD_Client_ID);
        }	

        /// <summary>
        ///Get Login AD_Org_ID
        /// </summary>
        /// <returns></returns>
        public int GetAD_Org_ID()
        {
            return GetContextAsInt("#AD_Org_ID");
        }

        /// <summary>
        ///	Set Login AD_Org_ID
        /// </summary>
        /// <param name="AD_Org_ID"></param>
        public void SetAD_Org_ID(int AD_Org_ID)
        {
            SetContext("#AD_Org_ID", AD_Org_ID);
        }	

        /// <summary>
        ///Get Window AD_Org_ID
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <returns></returns>
        public int GetAD_Org_ID(int windowNo)
        {
            return GetContextAsInt(windowNo, "AD_Org_ID");
        }	

        /// <summary>
        ///Get Tab AD_Org_ID
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="TabNo"></param>
        /// <returns></returns>
        public int GetAD_Org_ID(int windowNo, int tabNo)
        {
            return GetContextAsInt(windowNo, tabNo, "AD_Org_ID");
        }	

        /// <summary>
        ///Get Login AD_User_ID
        /// </summary>
        /// <returns></returns>
        public int GetAD_User_ID()
        {
            return GetContextAsInt("##AD_User_ID");
        }	

        /// <summary>
        ///Set Login AD_User_ID
        /// </summary>
        /// <param name="AD_User_ID"></param>
        public void SetAD_User_ID(int AD_User_ID)
        {
            SetContext("##AD_User_ID", AD_User_ID);
        }	

        /// <summary>
        ///Get Login AD_Role_ID
        /// </summary>
        /// <returns></returns>
        public int GetAD_Role_ID()
        {
            return GetContextAsInt("#AD_Role_ID");
        }	

        /// <summary>
        ///	Set Login AD_Role_ID
        /// </summary>
        /// <param name="AD_Role_ID"></param>
        public void SetAD_Role_ID(int AD_Role_ID)
        {
            SetContext("#AD_Role_ID", AD_Role_ID);
        }	

        /// <summary>
        ///Is Processed
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <returns></returns>
        public bool IsProcessed(int windowNo)
        {
            return "Y".Equals(GetContext(windowNo, "Processed"));
        }	

        /// <summary>
        ///Is Processing
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <returns></returns>
        public bool IsProcessing(int windowNo)
        {
            return "Y".Equals(GetContext(windowNo, "Processing"));
        }	

        /// <summary>
        ///Is Active
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <returns></returns>
        public bool IsActive(int windowNo)
        {
            return "Y".Equals(GetContext(windowNo, "IsActive"));
        }

     /// <summary>
     ///Size
     /// </summary>
     /// <returns></returns>
        public int Size()
        {
            return m_map.Count;
        }	//	size

        /// <summary>
        ///String Info
        /// </summary>
        /// <returns></returns>
        public String ToStringShort()
        {
            StringBuilder sb = new StringBuilder("Ctx[#")
            .Append(m_map.Count)
            .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Extended String Info
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("Ctx[#")
            .Append(m_map.Count);
            sb.Append(m_map.ToString());
            sb.Append("]");
            return sb.ToString();
        }

      /// <summary>
      ///Get sorted Context as String array with format: key == value
      /// </summary>
      /// <returns></returns>
	 /// <summary>
        public virtual String[] GetEntireContext()
        {
            IEnumerator<string> en = m_map.Keys.GetEnumerator();
            List<String> sList = new  List<String>(m_map.Count);
            while (en.MoveNext())
            {
                string key = en.Current;
                String s = key + " == " + Get(key);
                sList.Add(s);
            }
            String[] retValue = new String[sList.Count];
            retValue = sList.ToArray();
            Array.Sort(retValue);
            return retValue;
        }	//	get

        /// <summary>
        /// Get Value 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string key)
        { 
            string value;
             m_map.TryGetValue(key, out value);
             return value;
        }

        /// <summary>
        ///Evalutee 
        /// </summary>
        /// <param name="variableName"></param>
        /// <returns></returns>
        public String GetValueAsString(String variableName)
        {
            return GetContext(variableName);
        }

        /**************************************************************************
	 */
       /// <summary>
       ///Get Ctx with just Window data and basic context
       /// </summary>
       /// <param name="windowNo"></param>
       /// <returns></returns>
        public Ctx GetCtx(int windowNo)
        {
            Ctx newCtx = new Ctx(GetMap(windowNo, false));
            return newCtx;
        }

        /// <summary>
        ///Get Map with spaces as null
        /// </summary>
        /// <param name="windowNo"></param>
        /// <returns></returns>
        public virtual IDictionary<string,string> GetMap(int windowNo)
        {
            return GetMap(windowNo, false);
        }

        /// <summary>
        /// Get Map with just Window data and basic context
        /// </summary>
        /// <param name="windowNo"></param>
        /// <param name="convertNullToEmptyString"></param>
        /// <returns></returns>
        public virtual IDictionary<string,string> GetMap(int windowNo, bool convertNullToEmptyString)
        {
            IDictionary<string, string> newMap = new Dictionary<string, string>();
            //Set set = (Set)m_map.entrySet();
            /**	Copy Window only 	**/
            String keyPart = windowNo + "|";
            String infoPart = EnvConstants.WINDOW_INFO + "|";
            int keyPartIndex = keyPart.Length;

            IEnumerator<KeyValuePair<string,string>> en = m_map.GetEnumerator();
            while (en.MoveNext())
            {
                string key = en.Current.Key;// entry (Map.Entry)it.next();
                string oo = en.Current.Value;// m_map[key];
                //String key = entry.getKey().toString();
                int index = key.IndexOf(keyPart);
                if (index == 0)		//	start of Key
                {
                    String newKey = key.Substring(keyPartIndex);
                    //m_map[key]; //entry.getValue();
                    if (oo == null && convertNullToEmptyString)
                        newMap[newKey]= "";
                    else
                        newMap[newKey]  = oo;
                }
                else
                {
                    index = key.IndexOf(infoPart);
                    if (index == 0)		//	start of Key
                    {
                        //String newKey = key.substring (infoPartIndex);
                        //only overwrite newMap when the key is not there. 
                        //for example, user may have changed 1|VAB_BusinessPartner_ID on base window to 118
                        //and 1113|1113|VAB_BusinessPartner_ID in info window ctx stays as 117. We
                        // don't want 117 to overwrite 118
                        //if(!newMap.containsKey(newKey))
                        newMap[key]  =  oo;
                    }
                }
            }
            return newMap;
        }

        public bool GetIsUseBetaFunctions()
        {
            string s = GetContext("#IsUseBetaFunctions");
            if (s != null && s.Equals("Y"))
                return true;
            return false;
        }
        public void SetIsUseBetaFunctions(bool IsUseBetaFunctions)
        {
            m_map["#IsUseBetaFunctions"] = IsUseBetaFunctions ? "Y" : "N";
        }

        /// <summary>
        ///Remove context for Window (i.e. delete it)
        /// </summary>
        /// <param name="windowNo"></param>
        public virtual void RemoveWindow(int windowNo)
        {

            string[] keys = null; ;
            m_map.Keys.CopyTo(keys, 0);
            for (int i = 0; i < keys.Length; i++)
            {
                String key = keys[i];
                if (key.StartsWith(windowNo + "|"))
                    m_map.Remove(key);
            }
        }

        /// <summary>
        ///Add Window Context
        /// </summary>
        /// <param name="windowNo"></param>
        /// <param name="m_map"></param>
        public virtual void AddWindow(int windowNo, IDictionary<string,string> map)
        {
            //Set set = map.entrySet();
            IEnumerator<KeyValuePair<string,string>> en = map.GetEnumerator();
            while (en.MoveNext())
            {
                //   Map.Entry entry = (Map.Entry)it.next();
                //KeyValuePair<object, object> entry =(KeyValuePair<object,object>) en.Current;
                String key = en.Current.Key;// Current.ToString();// entry.getKey().toString();
                String newKey = windowNo + "|" + key;
                if (key.IndexOf('|') != -1)
                    newKey = key;
                string value = en.Current.Value;
                String newValue = Null.NULLString;
                if (value != null)
                    newValue = value;
                SetContext(newKey, newValue);
            }
        }

        /// <summary>
        ///Remove Preferences
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="attributeValue"></param>
        public void DeletePreference(String attribute, String attributeValue)
        {
            IEnumerator<KeyValuePair<string, string>> en = m_map.GetEnumerator();

            List<string> lst = m_map.Values.ToList();

            foreach (String key in lst)
            {
                //String key = en.Current.Key;

                if (key == null)
                {
                    continue;
                }

                if (key.StartsWith("P") && key.IndexOf(attribute) != -1)
                {
                    String value = GetContext(key);
                    if (value.Equals(attributeValue))
                        m_map.Remove(key);
                    //en = m_map.GetEnumerator();
                }
            }

            //IEnumerator<KeyValuePair<string, string>> en = m_map.GetEnumerator();
            //while (en.MoveNext())
            //{
            //    String key = en.Current.Key;
            //    if (key.StartsWith("P") && key.IndexOf(attribute) != -1)
            //    {
            //        String value = GetContext(key);
            //        if (value.Equals(attributeValue))
            //            m_map.Remove(key);
            //        en = m_map.GetEnumerator();
            //    }
            //}
        }

        /// <summary>
        /// Clear Context
        /// </summary>
        public new  virtual void Clear()
        {
            m_map.Clear();
        }	//	clear

        /// <summary>
        ///Contains Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new virtual bool ContainsKey(string key)
        {
            return m_map.ContainsKey(key);
        }

        /// <summary>
        ///Contains Value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public new virtual bool ContainsValue(string value)
        {
            return m_map.ContainsValue(value);
        }

        /// <summary>
        ///entrySet
        /// </summary>
        /// <returns></returns>
        public ICollection EntrySet()
        {
            return m_map.Keys;
        }

        /// <summary>
        /// Is Context Empty
        /// </summary>
        /// <returns></returns>
        public virtual bool IsEmpty()
        {
            return m_map.Count == 0;
        }

        /// <summary>
        ///  	Set Context key / value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Put(Object key, Object value)
        {
            if (key == null)
                return null;
            if (value == null)
                return SetContext(key.ToString(), null);
            String stringValue = null;
            if (value != null)
                stringValue = value.ToString();
            return SetContext(key.ToString(), stringValue);
        }	//	put

        /// <summary>
        ///Put All
        /// </summary>
        /// <param name="m_map"></param>
        public void PutAll(IDictionary<string,string> map)
        {
            Load(map);
        }

        /// <summary>
        /// Remove key from context
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new bool Remove(string key)
        {
            return  m_map.Remove(key);
        }

        /// <summary>
       ///	values
       /// </summary>
       /// <returns></returns>
        public new virtual ICollection<string> Values()
        {
            return m_map.Values;
        }	//	

        /// <summary>
        ///Get Map
        /// </summary>
        /// <returns></returns>
        public virtual IDictionary<string, string> GetMap()
        {
            return m_map;
        }
     
        /// <summary>
        ///Copy the designated context variable from one windowNO to another
        /// </summary>
        /// <param name="from_windowNO"></param>
        /// <param name="to_windowNO"></param>
        /// <param name="context"></param>
        public void CopyContext(int from_windowNO, int to_windowNO, String context)
        {
            if (to_windowNO != from_windowNO)
                SetContext(to_windowNO, context, GetContext(from_windowNO, context));
        }

        /// <summary>
        /// Get Theme
        /// </summary>
        /// <returns></returns>
        public String GetUITheme()
        {
            return GetContext("#UITheme");
        }

        /// <summary>
        /// Set Theme
        /// </summary>
        /// <param name="value"></param>
        public void SetUITheme(String value)
        {
            SetContext("#UITheme", value);
        }

        /// <summary>
        /// Get IsShowAdvanced
        /// </summary>
        /// <returns></returns>
        public bool IsShowAdvanced()
        {
            return GetContext("#ShowAdvanced").Equals("Y");
        }

        /// <summary>
        /// Set Is Show Advance
        /// </summary>
        /// <param name="showAdvanced"></param>
        public void SetShowAdvanced(bool showAdvanced)
        {
            m_map["#ShowAdvanced"] = showAdvanced ? "Y" : "N";
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
        ///	Get Login AD_Client_ID
        /// </summary>
        /// <returns></returns>
        public string GetAD_Client_Name()
        {
            return GetContext("#AD_Client_Name");
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
            return GetContextAsInt("#M_Warehouse_ID");
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
        ///	Get Login AD_User_Name
        /// </summary>
        /// <returns></returns>
        public string GetAD_User_Name()
        {
            return GetContext("#AD_User_Name");
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
        ///Is Window is cached
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <returns></returns>
        public bool IsCacheWindow()
        {
            String s = GetContext("CacheWindows");
            if (s != null && s.Equals("Y"))
                return true;
            return false;
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

        //private bool _isVisualEditor = false;
        //public bool IsVisualEditor
        //{
        //    get { return _isVisualEditor; }
        //    set { _isVisualEditor = value; }
        //}

        private bool _skipLookUpCreation = false;
        //private IDictionary<string, object> ctxMap;
        public bool SkipLookup
        {
            get { return _skipLookUpCreation; }
            set { _skipLookUpCreation = value; }
        }

        //Manfacturing

        public void SetBatchMode(Boolean isBatch)
        {
            SetContext("##IsBatchMode", isBatch ? "Y" : "N");
        }
        public Boolean IsBatchMode()
        {
            return "Y".Equals(GetContext("##IsBatchMode"));
        }

        public void SetIsBasicDB(bool isBase)
        {
            SetContext("#IsBaseDatabase", isBase ? "Y" : "N");
        }
        public bool GetIsBasicDB()
        {
            return GetContext("#IsBaseDatabase") == "Y";
        }

        public void SetApplicationUrl(string url)
        {
            SetContext("#AppFullUrl", url);
        }

        public void SetIsSSL(bool isSsl)
        {
            SetContext("#IsSSL", isSsl ? "Y" : "N");
        }

        public string GetApplicationUrl()
        {
           // return GetContext("#AppUrl");
            return GetContext("#AppFullUrl");
        }

        public bool GetIsSSL()
        {
           return  GetContext("#IsSSL") == "Y";
        }

        public void SetContextUrl(string url)
        {
            SetContext("#ContextUrl", url);
        }

        public string GetContextUrl()
        {
            return GetContext("#ContextUrl");
        }

        //public void SetApplicationFullUrl(string url)
        //{
        //    SetContext("#AppFullUrl", url);
        //}
        //public string GetApplicationFullUrl()
        //{
        //    return GetContext("#AppFullUrl");
        //}

        /// <summary>
        ///Get Tab AD_Org_ID
        /// </summary>
        /// <param name="WindowNo"></param>
        /// <param name="TabNo"></param>
        /// <returns></returns>
        public string SetUseCrystalReportViewer(string use)
        {
            return SetContext("#USE_CRYSTAL_REPORT_VIEWER",use);
        }

        /// <summary>
        ///Get Login AD_User_ID
        /// </summary>
        /// <returns></returns>
        public string GetUseCrystalReportViewer()
        {
            return GetContext("#USE_CRYSTAL_REPORT_VIEWER");
        }	

    }
    #endregion
}
