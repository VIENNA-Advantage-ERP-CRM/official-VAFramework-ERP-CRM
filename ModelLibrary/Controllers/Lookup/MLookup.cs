/********************************************************
// Module Name    : Show field 
// Purpose        : store the datasource for combobox 's and cached them 
// Class Used     : GlobalVariable.cs, CommonFunctions.cs,Ctx.cs
// Created By     : Harwinder 
// Date           : -----   
**********************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.Login;
using VAdvantage.Model;

namespace VAdvantage.Model
{
    public class MLookup : Lookup
    {

        public MLookup(Ctx ctx, int windowNo, int displayType)
            : base(ctx, windowNo, displayType)
        {
        }

        /** Inactive Marker Start       */
        public const String INACTIVE_S = "~";
        /** Inactive Marker End         */
        public const String INACTIVE_E = "~";
        /** Number of max rows to load	*/
        private const int MAX_ROWS = 10000;
        /**	Indicator for Null			*/
        private static int MINUS_ONE = -1;


        /** The Lookup Info Value Object        */
        public VLookUpInfo _vInfo = null;

        /** Storage of data  Key-NamePair	*/
        public volatile Dictionary<Object, NamePair> _lookup = new Dictionary<Object, NamePair>();
        
        /** The Data Loader                 */
        private System.Threading.Thread _loader;
        //

        /** All Data loaded                 */
        public bool _allLoaded = false;
        /** Inactive records exists         */
        public bool _hasInactive = false;
        /** Next Read for Parent			*/
        private long _nextRead = 0;


        /**	Save getDirect last return value */
        private Dictionary<Object, Object> _lookupDirect = null;
        /**	Save last unsuccessful				*/
        private Object _directNullKey = null;




        /// <summary>
        /// Initialize LookUp Data
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public MLookup Initialize(VLookUpInfo info)
        {
          //  _vInfo = info;

          //  log.Fine(_vInfo.keyColumn);

          //  //  load into local lookup, if already cached

          //  //if (MLookupCache.LoadFromCache(_vInfo, _lookup,out this._allLoaded, out this._hasInactive))
          // // {
          // //     return this;
          // // }
          //  // Don't load Search or CreatedBy/UpdatedBy
          //  if (GetDisplayType() == DisplayType.Search || _vInfo.isCreadedUpdatedBy)
          //  {
          //      return this;
          //  }
          //  // Don't load Parents/Keys
          //  if (_vInfo.isParent || _vInfo.isKey)
          //  {
          //      _hasInactive = true;		//	creates focus listener for dynamic loading
          //      return this;				//	required when parent needs to be selected (e.g. price from product)
          //  }

          // // Load();
          // // _loader = new System.Threading.Thread  (new System.Threading.ThreadStart(Load));
          
       
          // // _loader.Start();
          //  // Load();
          //  return this;




            //Lakhwinder
            _vInfo = info;

            log.Fine(_vInfo.keyColumn);

            //  load into local lookup, if already cached
            //if (MLookupCache.LoadFromCache(_vInfo, _lookup))
            //{
            //    return this;
            //}

            if (_vInfo.isCreadedUpdatedBy)
            {
                return this;
            }

            //if (GetDisplayType() == DisplayType.Search || _vInfo.isParent)
            //{
            //    //FillDirectList();
            //    return this;
            //}
            if (_vInfo.isParent || _vInfo.isKey)
            {
                _hasInactive = true;		//	creates focus listener for dynamic loading
                return this;				//	required when parent needs to be selected (e.g. price from product)
            }

            //_loader = new System.Threading.Thread(new System.Threading.ThreadStart(Load));
            //_loader.IsBackground = true;
            //_loader.CurrentCulture = Utility.Env.GetLanguage(Utility.Envs.GetContext()).GetCulture(Utility.Env.GetLoginLanguage(Utility.Envs.GetContext()).GetAD_Language());
            //_loader.CurrentUICulture = Utility.Env.GetLanguage(Utility.Envs.GetContext()).GetCulture(Utility.Env.GetLoginLanguage(Utility.Envs.GetContext()).GetAD_Language());

            //_loader.Start();

            //if (!_vIn---fo.isValidated)
            //{
            //    FillDirectList();
            //}
            //else
            //{
            //Load();
            //}
            return this;



        }

        /// <summary>
        /// Load LookUp data by second thread
        /// </summary>
        private void Load()
        {
            long startTime = CommonFunctions.CurrentTimeMillis();// System.currentTimeMillis();

            string sql = _vInfo.query;

            //	not validated
            if (!_vInfo.isValidated)
            {
                string validation = Utility.Env.ParseContext(GetCtx(), GetWindowNo(), _vInfo.validationCode, false);
                if (validation.Length == 0 && _vInfo.validationCode.Length > 0)
                {
                    log.Fine(_vInfo.keyColumn + ": Loader NOT Validated: " + _vInfo.validationCode);
                    return;
                }
                else
                {
                    int posFrom = sql.LastIndexOf(" FROM ");
                    bool hasWhere = sql.IndexOf(" WHERE ", posFrom) != -1;
                    //
                    int posOrder = sql.LastIndexOf(" ORDER BY ");
                    if (posOrder != -1)
                        sql = sql.Substring(0, posOrder)
                            + (hasWhere ? " AND " : " WHERE ")
                            + validation
                            + sql.Substring(posOrder);
                    else
                        sql += (hasWhere ? " AND " : " WHERE ")

                            + validation;
                    if (VLogMgt.IsLevelFinest())
                        log.Fine(_vInfo.keyColumn + ": Validation=" + validation);
                }
            }
            //if (_loader.ThreadState == System.Threading.ThreadState.Suspended)
            //{
            //    return;
            //}
            if (VLogMgt.IsLevelFiner())
                GetCtx().SetContext(Env.WINDOW_MLOOKUP, _vInfo.column_ID, _vInfo.keyColumn, sql);
            if (VLogMgt.IsLevelFinest())
                log.Fine(_vInfo.keyColumn + ": " + sql);

            _lookup.Clear();
            
            bool isNumber = _vInfo.keyColumn.EndsWith("_ID");
            _hasInactive = false;
            int rows = 0;
            // KeyValuePair<string, Object> p = new KeyValuePair<string, Object>("", "");
            System.Data.IDataReader dr = null;
            try
            {
                dr = SqlExec.ExecuteQuery.ExecuteReader(sql);
                _allLoaded = true;

                while (dr.Read())
                {
                    if (rows++ > MAX_ROWS)
                    {
                        log.Warning(_vInfo.keyColumn + ": Loader - Too many records");
                        _allLoaded = false;
                        break;
                    }

                    string name = dr[2].ToString();
                    bool isActive = dr[3].ToString().Equals("Y");
                    if (!isActive)
                    {
                        name = INACTIVE_S + name + INACTIVE_E;
                        _hasInactive = true;
                    }

                    if (isNumber)
                    {
                       // object key = dr[0];
                        int key = Utility.Util.GetValueOfInt(dr[0]);
                        KeyNamePair p = new KeyNamePair(key, name);
                        _lookup[key] = p;
                    }

                    else
                    {
                        string value = dr.GetString(1);
                        ValueNamePair p = new ValueNamePair(value, name);
                        _lookup.Add(value, p);
                    }
                    //KeyValuePair<string, string> p = new KeyValuePair<string, string>();
                }
                dr.Close();
                dr = null;
                
            }
            catch(System.Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                    dr = null;
                }
                log.Log(Level.SEVERE, _vInfo.keyColumn + ": Loader - " + sql, e);
               ////Common.////ErrorLog.FillErrorLog("Mlookup.Load()",sql,e.Message,VAdvantage.Framework.Message.MessageType.ERROR);
            }
            
            int size = _lookup.Count;
            log.Finer(_vInfo.keyColumn
                    + " (" + _vInfo.column_ID + "):"
                //	+ " ID=" + m_info.AD_Column_ID + " " +
                    + " - Loader complete #" + size + " - all=" + _allLoaded
                    + " - ms=" +(CommonFunctions.CurrentTimeMillis() -  startTime));
                    //+ " (" + String.valueOf(System.currentTimeMillis() - startTime) + ")");
            //	if (m_allLoaded)

            MLookupCache.LoadEnd(_vInfo, _lookup,_allLoaded,_hasInactive);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public override void Dispose()
        {
            if (_vInfo != null)
                log.Fine(_vInfo.keyColumn + ": dispose");
                if (_loader != null && _loader.IsAlive)
                    _loader.Interrupt();
            _loader = null;
            //
            if (_lookup != null)
                _lookup.Clear();
            _lookup = null;
            if (_lookupDirect != null)
                _lookupDirect.Clear();
            _lookupDirect = null;
            //
            _vInfo = null;

            base.Dispose();
            //
        }   //  dispose


        /// <summary>
        /// Wait until async Load Complete
        /// </summary>
        public void LoadComplete()
        {
            if (_loader != null && _loader.IsAlive)
            {
                try
                {
                    _loader.Join();
                    _loader = null;
                }
                catch (Exception ie)
                {
                    log.Log(Level.SEVERE, _vInfo.keyColumn + ": Interrupted", ie);
                }
            }
        }

        /// <summary>
        ///Get value (name) for key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>If not found return null;</returns>
        public override NamePair Get(Object key)
        {
            if (key == null || MINUS_ONE.Equals(key))	//	indicator for null
                return null;

            if (_vInfo.isParent && _nextRead < CommonFunctions.CurrentTimeMillis())
            {
                _lookup.Clear();
                if (_lookupDirect != null)
                    _lookupDirect.Clear();
                _nextRead = CommonFunctions.CurrentTimeMillis() + 500;	//	1/2 sec
            }
            //	try cache
            NamePair retValue;

            _lookup.TryGetValue(key, out retValue);
            if (retValue != null)
                return retValue;

            //	Not found and waiting for loader
            if (_loader != null && _loader.IsAlive)
            {
                log.Finer((_vInfo.keyColumn == null ? "ID=" + _vInfo.column_ID : _vInfo.keyColumn) + ": waiting for Loader");
                LoadComplete();
                //	is most current

                _lookup.TryGetValue(key, out retValue);
                if (retValue != null)
                    return retValue;
            }

            //  Always check for parents - not if we SQL was validated and completely loaded
            if (!_vInfo.isParent && _vInfo.isValidated && _allLoaded)
            {
                log.Finer(_vInfo.keyColumn + ": <NULL> - " + key // + "(" + key.getClass()
                    + "; Size=" + _lookup.Count);
                	//log.Finest( _lookup.Keys.ToString(), "ContainsKey = " + _lookup.ContainsKey(key));
                //  also for new values and inactive ones
                return GetDirect(key, false, true);		//	cache locally    
            }

            log.Finest(_vInfo.keyColumn + ": " + key
                  + "; Size=" + _lookup.Count + "; Validated=" + _vInfo.isValidated
                 + "; All Loaded=" + _allLoaded + "; HasInactive=" + _hasInactive);
            //	never loaded
            if (!_allLoaded
                && _lookup.Count == 0
                && !_vInfo.isCreadedUpdatedBy
                && !_vInfo.isParent
                && GetDisplayType() != DisplayType.Search)
            {
                Load();
                
                //	sync!
               // LoadComplete();

                _lookup.TryGetValue(key, out retValue);
                if (retValue != null)
                    return retValue;
            }
            //	Try to get it directly
            bool cacheLocal = _vInfo.isValidated;
            return GetDirect(key, false, cacheLocal);	//	do NOT cache	
        }	//	get

        /// <summary>
        ///Get Display value (name).
        /// found return key embedded in inactive signs.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Value</returns>
        public override String GetDisplay(Object key)
        {
            if (key == null || key == DBNull.Value || key.Equals(-1))
                return "";
            //
            Object display = Get(key);
            if (display == null || display==DBNull.Value)
                return "<" + key.ToString() + ">";
            return display.ToString();
        }	//	getDisplay

        /// <summary>
        ///  The Lookup contains the key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(Object key)
        {
            return _lookup.ContainsKey(key);
        }   //  containsKey

        public override String ToString()
        {
            return "MLookup[" + _vInfo.keyColumn + ",Column_ID=" + _vInfo.column_ID
                + ",Size=" + _lookup.Count + ",Validated=" + IsValidated()
                + "-" + GetValidation()
                + "]";
        }	//	toString


        /// <summary>
        /// Indicates whether some other object is "equal to" this one.

        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(Object obj)
        {
            if (obj is MLookup)
            {
                MLookup ll = (MLookup)obj;
                if (ll._vInfo.column_ID == this._vInfo.column_ID)
                    return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public int Size()
        {
            return _lookup.Count;
        }	//	size

        /**
         *	Is it all loaded
         *  @return true, if all loaded
         */
        public bool IsAllLoaded()
        {
            return _allLoaded;
        }	//	isAllLoaded

        /**
        /**
	 * 	Disable Validation
	 */
        public override void DisableValidation()
        {
            String validationCode = _vInfo.validationCode;
            base.DisableValidation();
            //super.disableValidation();
            _vInfo.isValidated = true;
            _vInfo.validationCode = "";
            //	Switch to Search for Table/Dir Validation
            if (validationCode != null && validationCode.Length > 0)
            {
                if (validationCode.IndexOf("@") != -1
                    && GetDisplayType() == DisplayType.Table || GetDisplayType() == DisplayType.TableDir)
                {
                    SetDisplayType(DisplayType.Search);
                }
            }
        }

        /// <summary>
        ///Is the List fully Validated
        /// </summary>
        /// <returns>true, if validated</returns>
        public override bool IsValidated()
        {
            if (IsValidationDisabled())
                return true;
            if (_vInfo == null)
                return false;
            return _vInfo.isValidated;
        }

        /// <summary>
        ///Get Validation SQL
        /// </summary>
        /// <returns>Validation SQL</returns>
        public override String GetValidation()
        {
            if (IsValidationDisabled())
                return "";
            return _vInfo.validationCode;
        }

        /// <summary>
        /// Get Reference Value
        /// </summary>
        /// <returns>Reference Value</returns>
        public int GetAD_Reference_Value_ID()
        {
            return _vInfo.AD_Reference_Value_ID;
        }

        /// <summary>
        ///  Has inactive elements in list
        /// </summary>
        /// <returns>true, if list contains inactive values</returns>
        public override bool HasInactive()
        {
            return _hasInactive;
        }

        /// <summary>
        /// Return info as List containing Value/KeyNamePair
        /// </summary>
        /// <param name="onlyValidated">only validated</param>
        /// <param name="loadParent">get Data even for parent lookups</param>
        /// <returns>List Namepair</returns>
        private List<NamePair> GetData(bool onlyValidated, bool loadParent)
        {
            if (_loader != null && _loader.IsAlive)
            {
                log.Fine((_vInfo.keyColumn == null ? "ID=" + _vInfo.column_ID : _vInfo.keyColumn)
                   + ": waiting for Loader");
                LoadComplete();
            }

            //	Never Loaded (correctly)
            if (!_allLoaded || _lookup.Count == 0)
                Refresh(loadParent);

            //	already validation included
            if (_vInfo.isValidated)
                return new List<NamePair>(_lookup.Values);

            if (!_vInfo.isValidated && onlyValidated)
            {
                Refresh(loadParent);
                log.Fine(_vInfo.keyColumn + ": Validated - #" + _lookup.Count);
            }
            return new List<NamePair>(_lookup.Values);
        }	//	getData

        /// <summary>
        ///Return data as Array containing Value/KeyNamePair
        /// </summary>
        /// <param name="mandatory">if not mandatory, an additional empty value is inserted</param>
        /// <param name="onlyValidated">only validated</param>
        /// <param name="onlyActive">only active</param>
        /// <param name="temporary">force load for temporary display e.g. parent</param>
        /// <returns>list</returns>
        public override List<NamePair> GetData(bool mandatory, bool onlyValidated,
                bool onlyActive, bool temporary)
        {
            //	create list
            List<NamePair> list = GetData(onlyValidated, temporary);

            //  Remove inactive choices
            if (onlyActive && _hasInactive)
            {
                //  list from the back
                for (int i = list.Count; i > 0; i--)
                {
                    Object o = list[i - 1];
                    if (o != null)
                    {
                        String s = o.ToString();
                        if (s.StartsWith(INACTIVE_S) && s.EndsWith(INACTIVE_E))
                            list.RemoveAt(i - 1);
                    }
                }
            }

            //	Add Optional (empty) selection
            if (!mandatory)
            {
                NamePair p = null;
                if (_vInfo.keyColumn != null && _vInfo.keyColumn.EndsWith("_ID"))
                    p = new KeyNamePair(-1, "");
                else
                    p = new ValueNamePair(" ", "");
                list.Insert(0, p);
            }

            return list;
        }

        /// <summary>
        /// Get Data Direct from Table.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="saveInCache">save in cache for r/w</param>
        /// <param name="cacheLocal">cache locally for r/o</param>
        /// <returns></returns>

        public override NamePair GetDirect(Object key, bool saveInCache, bool cacheLocal)
        {
            //	Nothing to query
            if (key == null || _vInfo.queryDirect == null || _vInfo.queryDirect.Length == 0)
                return null;
            if (key.Equals(_directNullKey))
                return null;
            //
            NamePair directValue = null;
            if (_lookupDirect != null)		//	Lookup cache
            {
                object o = null;
                _lookupDirect.TryGetValue(key, out o);
                if (o != null && o is NamePair)
                    return (NamePair)o;
            }
            log.Finer(_vInfo.keyColumn + ": " + key
                  + ", SaveInCache=" + saveInCache + ",Local=" + cacheLocal);
            bool isNumber = _vInfo.keyColumn.EndsWith("_ID");
            System.Data.IDataReader dr = null;
            try
            {
                //	SELECT Key, Value, Name FROM ...
                System.Data.SqlClient.SqlParameter[] param = new System.Data.SqlClient.SqlParameter[1];
                if (isNumber)
                    param[0] = new System.Data.SqlClient.SqlParameter("@key", int.Parse(key.ToString()));
                else
                    param[0] = new System.Data.SqlClient.SqlParameter("@key", key.ToString());

                dr = SqlExec.ExecuteQuery.ExecuteReader(_vInfo.queryDirect, param);
                if (dr.Read())
                {
                    String name = dr[2].ToString();
                    if (isNumber)
                    {
                        int keyValue = Utility.Util.GetValueOfInt(dr[0]);
                        //object keyValue = dr[0];
                        KeyNamePair p = new KeyNamePair(keyValue, name);
                        if (saveInCache)		//	save if
                            _lookup.Add(keyValue, p);
                        directValue = p;
                    }
                    else
                    {
                        String value = dr.GetString(1);
                        ValueNamePair p = new ValueNamePair(value, name);
                        if (saveInCache)		//	save if
                            _lookup.Add(value, p);
                        directValue = p;
                    }
                    if (dr.Read())
                        log.Log(Level.SEVERE, _vInfo.keyColumn + ": Not unique (first returned) for "
                            + key + " SQL=" + _vInfo.queryDirect);
                }
                else
                {
                    _directNullKey = key;
                    directValue = null;
                }
                if (VLogMgt.IsLevelFinest())
                {
                    log.Finest(_vInfo.keyColumn + ": " + directValue + " - " + _vInfo);
                }
                dr.Close();
                dr = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, _vInfo.keyColumn + ": SQL=" + _vInfo.queryDirect + "; Key=" + key, e);
                if(dr!=null)
                {
                    dr.Close(); 
                    dr = null;
                }
                directValue = null;
            }
            

            //	Cache Local if not added to R/W cache
            if (cacheLocal && !saveInCache && directValue != null)
            {
                if (_lookupDirect == null)
                    _lookupDirect = new Dictionary<object, object>();
                _lookupDirect.Add(key, directValue);
            }
            _hasInactive = true;
            return directValue;
        }
        /// <summary>
        ///Get Zoom
        ///Zoom AD_Window_ID
        /// </summary>
        /// <returns></returns>
        public override  int GetZoomWindow()
        {
            return _vInfo.zoomWindow;
        }

        /// <summary>
        /// Get Zoom
        /// </summary>
        /// <param name="query">query</param>
        /// <returns>Zoom Window</returns>
        public override  int GetZoomWindow(Query query)
        {

            /* Handle cases where you have multiple windows for a single table.
             * So far it is just the tables that have a PO Window defined.
             * For eg., Order, Invoice and Shipments
             * This will need to be expanded to add more tables if they have
             * multiple windows.
             */
            int AD_Window_ID = ZoomTarget.GetZoomAD_Window_ID(_vInfo.tableName, _WindowNo, query.GetWhereClause(), GetCtx().IsSOTrx(_WindowNo),GetCtx());
            return AD_Window_ID;
        }

        /// <summary>
        /// Get Zoom Query String
        /// </summary>
        /// <returns>Zoom SQL Where Clause</returns>
        public override Query GetZoomQuery()
        {
            return _vInfo.zoomQuery;
        }

        /// <summary>
        ///Get underlying fully qualified Table.Column Name
        /// </summary>
        /// <returns>Key Column</returns>
        public override  String GetColumnName()
        {
            return _vInfo.keyColumn;
        }

        /// <summary>
        ///Refresh & return number of items read.
        ///  	Get get data of parent lookups
        /// </summary>
        /// <returns></returns>
        public override  int Refresh()
        {
            return Refresh(true);
        }

        /// <summary>
        ///Refresh & return number of items read
        /// </summary>
        /// <param name="loadParent">get data of parent lookups</param>
        /// <returns>no of items read</returns>
        public int Refresh(bool loadParent)
        {
            if (!loadParent && _vInfo.isParent)
                return 0;
            //  Don't load Search or CreatedBy/UpdatedBy
            if (GetDisplayType() == DisplayType.Search
                || _vInfo.isCreadedUpdatedBy)
                return 0;
            //
            log.Fine(_vInfo.keyColumn + ": start");
            Load();
            
            //	m_loader.run();		//	test sync call
            log.Fine(_vInfo.keyColumn + ": #" + _lookup.Count);
            //
            return _lookup.Count;
        }
        /// <summary>
        /// Remove All cached Elements
        /// </summary>
        public void removeAllElements()
        {
            //base.removeAllElements ();
            _lookup.Clear();
            if (_lookupDirect != null)
                _lookupDirect.Clear();
        }

    }
}
