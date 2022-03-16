/********************************************************
 * Module Name    : Cache
 * Purpose        : To store the values of disfferent tables in a cache
 * Class Used     : 
 * Chronological Development
 * Veena Pandey     29-Apr-2009
 ******************************************************/

using System;

using VAdvantage.Model;
using VAdvantage.Utility;

namespace VAdvantage.Classes
{

    /// <summary>
    /// Cache class
    /// </summary>  
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class CCache<K, V> : CoreLibrary.Classes.CCache<K, V>
    {

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
        public CCache(String name, int initialCapacity, int expireMinutes) : base(name, initialCapacity, expireMinutes)
        {

        }

        public new V Get(Ctx ctx, K key)
        {
            V v = base.Get(ctx, key);

            if ((ctx != null) && (v is PO))
            {
                PO b = (PO)Convert.ChangeType(v, typeof(V));
                PO b1 = PO.Copy(ctx, b, null);
                return (V)Convert.ChangeType(b1, typeof(V)); ;
            }
            return v;
        }
    }
}