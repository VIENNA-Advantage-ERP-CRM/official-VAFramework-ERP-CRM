using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Print
{
    public class HashMap<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public void Put(TKey key, TValue value)
        {
            if (!base.ContainsKey(key))
                base.Add(key, value);
            else
                base[key] = value;
        }

        public TValue Get(TKey key)
        {
            if (base.ContainsKey(key))
                return base[key];
            return default(TValue);
        }

        public TValue Get(object key)
        {
            if (base.ContainsKey((TKey)key))
                return base[(TKey)key];
            return base[(TKey)key];
        }

        /**
        * Returns a hash value for the specified object.  In addition to 
        * the object's own hashCode, this method applies a "supplemental
        * hash function," which defends against poor quality hash functions.
        * This is critical because HashMap uses power-of two length 
        * hash tables.<p>
        *
        * The shift distances in this function were chosen as the result
        * of an automated search over the entire four-dimensional search space.
        */
        public static int Hash(Object x) 
        {
        int h = x.GetHashCode();

        h += ~(h << 9);
        h ^=  (h >> 14);
        h +=  (h << 4);
        h ^=  (h >> 10);

        return h;
    }
    }
}
