using System;
using System.Collections.Generic;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace CoreLibrary.Classes
{
   /// <summary>
    /// Cache class
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class CCache<K, V> : ICacheInterface
    {
        //	Name
        private String _name = null;
        // Expire after minutes
        private int _expire = 0;
        // Time
        private long _timeExp = 0;
        //	Just reset - not used
        private bool _justReset = true;

        // Vetoable Change Support
        //private VetoableChangeSupport _changeSupport = null;
        // Vetoable Change Support	Name
        private const String PROPERTYNAME = "cache";

        private Dictionary<K, V> cacheDic = null;

        /// <summary>
        /// VAdvantage Cache - expires after 2 hours
        /// </summary>
        /// <param name="name">(table) name of the cache</param>
        /// <param name="initialCapacity">The initial number of elements that the CCache can contain.</param>
        public CCache(String name, int initialCapacity)
            : this(name, initialCapacity, 120)
        {
        }

        /// <summary>
        /// VAdvantage Cache
        /// </summary>
        /// <param name="name">(table) name of the cache</param>
        /// <param name="initialCapacity">The initial number of elements that the CCache can contain.</param>
        /// <param name="expireMinutes">expire after minutes (0=no expire)</param>
        public CCache(String name, int initialCapacity, int expireMinutes)
        {
            cacheDic = new Dictionary<K, V>(initialCapacity);
            _name = name;
            SetExpireMinutes(expireMinutes);
            CacheMgt.Get().Register(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">key of the value to get or set</param>
        /// <returns></returns>
        public V this[K key]
        {
            get
            {
                Expire();
                if (cacheDic.ContainsKey(key))
                    return cacheDic[key];
                else
                    return default(V); // returns null by default
            }
            set
            {
                Expire();
                cacheDic[key] = value;
            }
        }

        /// <summary>
        /// Get (table) Name
        /// </summary>
        /// <returns>name</returns>
        public String GetName()
        {
            return _name;
        }

        /// <summary>
        /// Set Expire Minutes and start it
        /// </summary>
        /// <param name="expireMinutes">minutes or 0</param>
        public void SetExpireMinutes(int expireMinutes)
        {
            if (expireMinutes > 0)
            {
                _expire = expireMinutes;
                long addMS = 60000L * _expire;
                _timeExp = CommonFunctions.CurrentTimeMillis() + addMS;
            }
            else
            {
                _expire = 0;
                _timeExp = 0;
            }
        }

        /// <summary>
        /// Get Expire Minutes
        /// </summary>
        /// <returns>expire minutes</returns>
        public int GetExpireMinutes()
        {
            return _expire;
        }

        /// <summary>
        /// Cache was reset
        /// </summary>
        /// <returns>true if reset</returns>
        public bool IsReset()
        {
            return _justReset;
        }

        /// <summary>
        /// Resets the Reset flag
        /// </summary>
        public void SetUsed()
        {
            _justReset = false;
        }

        /// <summary>
        /// Reset Cache
        /// </summary>
        /// <returns>number of items cleared</returns>
        public int Reset()
        {
            int no = cacheDic.Count;
            Clear();
            return no;
        }

        /// <summary>
        /// Expire Cache if enabled
        /// </summary>
        private void Expire()
        {
            if (_expire != 0 && _timeExp < CommonFunctions.CurrentTimeMillis())
            {
                //	System.out.println ("------------ Expired: " + GetName() + " --------------------");
                Reset();
            }
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            return "CCache[" + _name + ",Exp=" + GetExpireMinutes() + ", #" + cacheDic.Count + "]";
        }

        /// <summary>
        /// Clear cache and calculate new expiry time
        /// </summary>
        public void Clear()
        {
            //if (_changeSupport != null)
            {
                try
                {
                    //_changeSupport.fireVetoableChange(PROPERTYNAME, super.size(), 0);
                }
                catch
                {
                    //System.out.println ("CCache.clear - " + e);
                    return;
                }
            }
            //	Clear
            cacheDic.Clear();
            if (_expire != 0)
            {
                long addMS = 60000L * _expire;
                _timeExp = CommonFunctions.CurrentTimeMillis() + addMS;
            }
            _justReset = true;
        }

        /// <summary>
        /// Determines whether the CCache the specific key
        /// </summary>
        /// <param name="key">The key to locate in CCache</param>
        /// <returns>true if key found</returns>
        public bool ContainsKey(K key)
        {
            Expire();
            return cacheDic.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether the CCache a specific value
        /// </summary>
        /// <param name="value">The value to locate in CCache</param>
        /// <returns>true if value found</returns>
        public bool ContainsValue(V value)
        {
            Expire();
            return cacheDic.ContainsValue(value);
        }

        /// <summary>
        /// Add value
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>previous value</returns>
        public void Add(K key, V value)
        {
            Expire();
            _justReset = false;
            if (cacheDic.ContainsKey(key))
            {
                cacheDic.Remove(key);
            }
            try
            {
                cacheDic.Add(key, value);
            }
            catch (Exception ex)
            {
                VLogger.Get().Log(VAdvantage.Logging.Level.SEVERE, "CCache-->" + ex.ToString());
            }
        }

        /// <summary>
        /// Gets the value associated with specified key
        /// </summary>
        /// <param name="key">key of the value to get</param>
        /// <param name="retval">output variable for value returned</param>
        /// <returns>true if key's value is returned</returns>
        public bool TryGetValue(K key, out V retval)
        {
            Expire();
            return cacheDic.TryGetValue(key, out retval);
        }

        /// <summary>
        /// Removes the value with the specified key from CCache
        /// </summary>
        /// <param name="key">The key of the element to remove</param>
        /// <returns>true if value removed</returns>
        public bool Remove(K key)
        {
            Expire();
            return cacheDic.Remove(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            Expire();
            if (cacheDic.Count == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Gets the number of key/value pairs
        /// </summary>
        /// <returns>total</returns>
        public int Size()
        {
            return this.Count;
        }

        /// <summary>
        /// Gets the number of key/value pairs
        /// </summary>
        public int Count
        {
            get
            {
                Expire();
                return cacheDic.Count;
            }
        }

        /// <summary>
        /// Get Size without Expire
        /// </summary>
        /// <returns>size</returns>
        public int SizeNoExpire()
        {
            return cacheDic.Count;
        }

        /// <summary>
        /// Gets a collection contaning the values in CCache
        /// </summary>
        public Dictionary<K, V>.ValueCollection Values
        {
            get
            {
                Expire();
                return cacheDic.Values;
            }
        }

        /// <summary>
        /// Gets a collection contaning the keys in CCache
        /// </summary>
        public Dictionary<K, V>.KeyCollection Keys
        {
            get
            {
                Expire();
                return cacheDic.Keys;
            }
        }

        public Object Get(K key)
        {
            V retVal;
            if (cacheDic.TryGetValue(key, out retVal))
            {
                return retVal;
            }
            return null;
        }

        /// <summary>
        /// Get cached object
        /// </summary>
        /// <param name="ctx">context (only used when the cached object is a PO - otherwise ignored)</param>
        /// <param name="key">the key value</param>
        /// <returns>cached value or if PO and ctx provided, the clone of a PO</returns>
        /// <date>07-march-2011</date>
        /// <writer>Raghu</writer>
        public V Get(Ctx ctx, K key)
        {
            Expire();
            //V v = m_cache.get(key);

            V v = default(V);

            if (cacheDic.Count > 0)
            {
                v = cacheDic[key];
            }

            //if ((ctx != null) && (v is PO))
            //{
            //    PO b = (PO)Convert.ChangeType(v, typeof(V));
            //    PO b1 = PO.Copy(ctx, b, null);
            //    return (V)Convert.ChangeType(b1, typeof(V)); ;
            //}
            return v;
        }
    }
}

namespace VAdvantage.Classes
{
    /// <summary>
    ///  Define basic function
    /// </summary>
    public interface ICacheInterface
    {
        int Reset();
        int Size();
        String GetName();
        int SizeNoExpire();
    }

    /// <summary>
    /// 
    /// </summary>
    public class CacheMgt
    {
        private static readonly object _lock = new object();

        private static CacheMgt _cache = null;
        //	List of Instances
        private List<ICacheInterface> instances = new List<ICacheInterface>();
        // List of Table Names
        private List<String> tableNames = new List<String>();
        //private int cacheResetCount = 0;
        // Logger
        //private static CLogger log = CLogger.getCLogger(CacheMgt.class);
        private static VLogger log = VLogger.GetVLogger(typeof(CacheMgt).FullName);

        private CacheMgt()
        {
        }

        public static CacheMgt Get()
        {
            if (_cache == null)
                _cache = new CacheMgt();
            return _cache;
        }

        /// <summary>
        /// Register Cache Instance
        /// </summary>
        /// <param name="instance">Cache</param>
        /// <returns>true</returns>
      //  [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Register(ICacheInterface instance)
        {
            lock (_lock)
            {
                if (instance == null)
                    return false;
                //if(instance.GetType().ToString() == "VAdvantage.Classes.CCache")
                if (instance.GetType().ToString().Contains("VAdvantage.Classes.CCache"))
                {
                    String tableName = instance.GetName();
                    //String tableName = ((CCache)instance).GetName();
                    tableNames.Add(tableName);
                }
                instances.Add(instance);
                return true;
            }
        }

        /// <summary>
        /// Un-Register Cache Instance
        /// </summary>
        /// <param name="instance">Cache</param>
        /// <returns>true if removed</returns>
        public bool UnRegister(ICacheInterface instance)
        {
            if (instance == null)
                return false;
            bool found = false;
            //	Could be included multiple times
            for (int i = instances.Count - 1; i >= 0; i--)
            {
                ICacheInterface stored = (ICacheInterface)instances[i];
                if (instance.Equals(stored))
                {
                    instances.RemoveAt(i);
                    found = true;
                }
            }
            return found;
        }

        /// <summary>
        /// Reset All registered Cache
        /// </summary>
        /// <returns>number of deleted cache entries</returns>
        public int Reset()
        {
            int counter = 0;
            int total = 0;
            for (int i = 0; i < instances.Count; i++)
            {
                ICacheInterface stored = (ICacheInterface)instances[i];
                if (stored != null && stored.Size() > 0)
                {
                    log.Fine(stored.ToString());
                    total += stored.Reset();
                    counter++;
                }
            }
            log.Info("#" + counter + " (" + total + ")");
            return total;
        }

        /// <summary>
        /// Reset registered Cache
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <returns>number of deleted cache entries</returns>
        public int Reset(String tableName)
        {
            return Reset(tableName, 0);
        }

        /// <summary>
        /// Reset registered Cache
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="Record_ID">record if applicable or 0 for all</param>
        /// <returns>number of deleted cache entries</returns>
        public int Reset(String tableName, int Record_ID)
        {
            if (tableName == null)
                return Reset();
            //	if (tableName.endsWith("Set"))
            //		tableName = tableName.substring(0, tableName.length()-3);
            if (!tableNames.Contains(tableName))
                return 0;
            //
            int counter = 0;
            int total = 0;
            for (int i = 0; i < instances.Count; i++)
            {
                ICacheInterface stored = (ICacheInterface)instances[i];
                //if (stored != null && stored.GetType().ToString() == "VAdvantage.Classes.CCache")
                if (stored != null && stored.GetType().ToString().Contains("VAdvantage.Classes.CCache"))
                {
                    //CCache cc = (CCache)stored;
                    //if (cc.GetName().StartsWith(tableName))
                    if (stored.GetName().StartsWith(tableName))		//	reset lines/dependent too
                    {
                        //	if (Record_ID == 0)
                        {
                            log.Fine("(all) - " + stored);
                            total += stored.Reset();
                            counter++;
                        }
                    }
                }
            }
            log.Info(tableName + ": #" + counter + " (" + total + ")");
            //	Update Server
            //if (DataBase.isRemoteObjects())
            //{
            //    Server server = CConnection.get().getServer();
            //    try
            //    {
            //        if (server != null)
            //        {	//	See ServerBean
            //            int serverTotal = server.cacheReset(tableName, 0);
            //int serverTotal = CacheReset(tableName, 0);
            //            if (CLogMgt.isLevelFinest())
            //            {
            //                log.Fine("Server => " + serverTotal);
            //            }
            //        }
            //    }
            //    catch (RemoteException ex)
            //    {
            //        log.Log(Level.SEVERE, "AppsServer error", ex);
            //    }
            //}
            return total;
        }

        /// <summary>
        /// Cache Reset
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="Record_ID">record or 0 for all</param>
        /// <returns>number of records reset</returns>
        private int CacheReset(String tableName, int Record_ID)
        {
            log.Config(tableName + " - " + Record_ID);
            //cacheResetCount++;
            return CacheMgt.Get().Reset(tableName, Record_ID);
        }


        //@SuppressWarnings("unchecked")
        /// <summary>
        /// Total Cached Elements
        /// </summary>
        /// <returns>count</returns>
        public int GetElementCount()
        {
            int total = 0;
            for (int i = 0; i < instances.Count; i++)
            {
                ICacheInterface stored = (ICacheInterface)instances[i];
                if (stored != null && stored.Size() > 0)
                {
                    log.Fine(stored.ToString());
                    //if(stored.GetType().ToString() == "VAdvantage.Classes.CCache")
                    if (stored != null && stored.GetType().ToString().Contains("VAdvantage.Classes.CCache"))
                    {
                        //total += ((CCache)stored).sizeNoExpire();
                        total += stored.SizeNoExpire();
                    }
                    else
                        total += stored.Size();
                }
            }
            return total;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("CacheMgt[");
            sb.Append("Instances=").Append(instances.Count).Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Extended String Representation
        /// </summary>
        /// <returns>info</returns>
        public String ToStringX()
        {
            StringBuilder sb = new StringBuilder("CacheMgt[");
            sb.Append("Instances=").Append(instances.Count).Append(", Elements=")
                .Append(GetElementCount()).Append("]");
            return sb.ToString();
        }

    }
}
