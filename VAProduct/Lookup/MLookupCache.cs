/********************************************************
// Module Name    : Show field 
// Purpose        : MLookup Data Cache.
                    Called from MLookup.
                    Only caches multiple use for a single window!
// Class Used     : GlobalVariable.cs, CommonFunctions.cs,Context.cs
// Created By     : Harwinder 
// Date           : -----   
**********************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Logging;
using VAdvantage.Model;

namespace VAdvantage.Model
{
    public class MLookupCache
    {

        private static VLogger _log = VLogger.GetVLogger(typeof(MLookupCache).FullName);
        /** Static Lookup data with MLookupInfo -> HashMap  */
        private static CCache<String, Dictionary<Object, NamePair>> s_loadedLookups
            = new CCache<String, Dictionary<Object, NamePair>>("MLookupCache", 50);
        private static CCache<String, KeyValuePair<bool,bool>> s_loadedLookupsParam
           = new CCache<String,  KeyValuePair<bool,bool>>("MLookupCacheParam", 50);


        /// <summary>
        /// MLookup Loader starts loading - ignore for now
        /// </summary>
        /// <param name="info"></param>
        public static void loadStart(VLookUpInfo info)
        {
        }   //  loadStart

        /// <summary>
        ///MLookup Loader ends loading, so add it to cache
        /// </summary>
        /// <param name="info"></param>
        /// <param name="lookup"></param>
        public static void LoadEnd(VLookUpInfo info, Dictionary<Object, NamePair> lookup,bool allLoaded, bool hasInActive)
        {
            if (info.isValidated && lookup.Count > 0)
            {
                 string key = GetKey(info);
                s_loadedLookups.Add(key, lookup);
                s_loadedLookupsParam.Add(key, new KeyValuePair<bool, bool>(allLoaded, hasInActive));
            }
        }

        /// <summary>
        /// Get Storage Key
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private static String GetKey(VLookUpInfo info)
        {
            if (info == null)
                return CommonFunctions.CurrentTimeMillis().ToString();
            //
            StringBuilder sb = new StringBuilder();
            sb.Append(info.column_ID).Append(":")
                .Append(info.keyColumn)
                .Append(info.AD_Reference_Value_ID)
                .Append(info.query)
                .Append(info.validationCode);
            //	does not include ctx
            return sb.ToString();
        }

        /// <summary>
        ///Load from Cache if applicable
        ///   Called from MLookup constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="lookupTarget">Target Dictionary</param>
        /// <returns></returns>
        public static bool LoadFromCache(VLookUpInfo info, Dictionary<Object, NamePair> lookupTarget,out bool allLoaded,out bool hasInActive)
        {
            String key = GetKey(info);
            Dictionary<object, NamePair> cache = null;
            allLoaded = false;
            hasInActive = false;

            s_loadedLookups.TryGetValue(key, out cache);
            if (cache == null)
                return false;
            //  Nothing cached
            if (cache.Count == 0)
            {
                s_loadedLookups.Remove(key);
                return false;
            }

            //  we can use iterator, as the lookup loading is complete (i.e. no additional entries)
            IEnumerator<Object> iterator = cache.Keys.GetEnumerator();
            while (iterator.MoveNext())
            {
                Object cacheKey = iterator.Current;
                NamePair cacheData = cache[cacheKey];
                lookupTarget.Add(cacheKey, cacheData);
            }

            if (s_loadedLookupsParam.ContainsKey(key))
            {
                KeyValuePair<bool, bool> o = s_loadedLookupsParam[key];
                allLoaded = o.Key;
                hasInActive = o.Value;
            }

            _log.Fine("#" + lookupTarget.Count);
            return true;
        }

       /// <summary>
       ///Clear Static Lookup Cache for Window
       /// </summary>
       /// <param name="WindowNo">WindowNo of Cache entries to delete</param>
        public static void CacheReset(int windowNo)
        {
            String key = windowNo.ToString() + ":";
            int startNo = s_loadedLookups.Count;
            //  find keys of Lookups to delete
            List<String> toDelete = new List<String>();
            IEnumerator<string> iterator = s_loadedLookups.Keys.GetEnumerator();
            while (iterator.MoveNext())
            {
                String info = (String)iterator.Current;
                if (info != null && info.StartsWith(key))
                    toDelete.Add(info);
            }

            //  Do the actual delete
            for (int i = 0; i < toDelete.Count; i++)
                s_loadedLookups.Remove(toDelete[i]);
            int endNo = s_loadedLookups.Count;
            _log.Fine("WindowNo=" + windowNo + " - " + startNo + " -> " + endNo);
        }
    }
}
