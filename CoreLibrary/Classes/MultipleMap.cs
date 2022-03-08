using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/*********************************************************************************
  * 
  *   MultiMap List
  * 
  *********************************************************************************/
/// <summary>
/// MultiMap allows multiple keys with their values.
///   It accepts null values as keys and values.
///   (implemented as two generic  lists)
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="V"></typeparam>

public class MultiMap<K, V>
{
    /// <summary>
    ///Constructor 
    /// </summary>
    public MultiMap()
    {
        m_keys = new List<K>();
        m_values = new List<V>();
    }   //  MultiMap

    /// <summary>
    ///Constructor
    /// </summary>
    /// <param name="initialCapacity"></param>
    public MultiMap(int initialCapacity)
    {
        //m_keys = new List<K>();
        //m_values = new List<V>();
    }   //  MultiMap

    private List<K> m_keys = null;
    private List<V> m_values = null;

    /// <summary>
    /// Return number of elements
    /// </summary>
    /// <returns></returns>
    public int Count()
    {
        return m_keys.Count;
    }   //  size

    /// <summary>
    ///Is Empty
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return (m_keys.Count == 0);
    }   //  isEmpty

    /// <summary>
    ///Contains Key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsKey(K key)
    {
        return m_keys.Contains(key);
    }   //  containsKey

    /// <summary>
    /// Contains Value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool ContainsValue(V value)
    {
        return m_values.Contains(value);
    }   //  containsKey

    /// <summary>
    ///Return ArrayList of Values of Key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<V> Get(K key)
    {
        return GetValues(key);
    }   //  get

    /// <summary>
    /// Return ArrayList of Values of Key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public List<V> GetValues(K key)
    {
        List<V> list = new List<V>();
        //  We don't have it
        if (!m_keys.Contains(key))
            return list;
        //  go through keys
        int size = m_keys.Count;
        for (int i = 0; i < size; i++)
        {
            if (m_keys[i].Equals(key))
                if (!list.Contains(m_values[i]))
                    list.Insert(list.Count, m_values[i]);
        }
        return list;
    }   //  getValues

    /// <summary>
    /// Return ArrayList of Keys with Value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public List<K> GetKeys(V value)
    {
        List<K> list = new List<K>();
        //  We don't have it
        if (!m_values.Contains(value))
            return list;
        //  go through keys
        int size = m_values.Count;
        for (int i = 0; i < size; i++)
        {
            if (m_values[i].Equals(value))
                if (!list.Contains(m_keys[i]))
                    list.Insert(list.Count, m_keys[i]);
        }
        return list;
    }

    /// <summary>
    /// Put Key & Value
    /// </summary>
    /// <param name="key">Key Of Record</param>
    /// <param name="value">Value OfRecord</param>
    /// <returns></returns>
    public void Add(K key, V value)
    {
        m_keys.Add(key);
        m_values.Add(value);
        //return null;
    }   //  put

    public void Insert(K key, V value)
    {
        m_keys.Insert(m_keys.Count, key);
        m_values.Insert(m_values.Count, value);
        //return null;
    }   //  put


    public void Clear()
    {
        m_keys.Clear();
        m_values.Clear();
    }   //  clear

    /// <summary>
    ///  Return HashSet of Keys
    /// </summary>
    /// <returns></returns>
    public HashSet<K> KeySet()
    {
        HashSet<K> keys = new HashSet<K>(m_keys);
        return keys;
    }   //  keySet

    /// <summary>
    /// Return Collection of values
    /// </summary>
    /// <returns></returns>
    public List<V> Values()
    {
        return m_values;
    }   //	values

    /// <summary>
    /// Returns class name and number of entries
    /// </summary>
    /// <returns></returns>
    public override String ToString()
    {
        return "MultiMap #" + m_keys.Count;
    }

}   //  MultiMap
